
using System;
using System.Collections.Generic;

public class BlockPool : IObjectPool
{
    public class Settings
    {
        public int blockSize;

        public int maxBlockNum;

        public bool extendAble;

        public Settings(int s, int n, bool e)
        {
            blockSize = s;
            maxBlockNum = n;
            extendAble = e;
        }
    }

    private static BlockPool m_sharedInstance;

    private static Dictionary<int, Settings> DefaultSettings;

    private Dictionary<int, BlockAllocator> m_bPool = new Dictionary<int, BlockAllocator>();

    public static BlockPool sharedInstance => m_sharedInstance;

    static BlockPool()
    {
        m_sharedInstance = null;
        DefaultSettings = new Dictionary<int, Settings>();
        DefaultSettings.Add(4, new Settings(4, 16, e: true));
        DefaultSettings.Add(8, new Settings(8, 64, e: true));
        DefaultSettings.Add(16, new Settings(16, 64, e: true));
        DefaultSettings.Add(32, new Settings(32, 64, e: true));
        DefaultSettings.Add(64, new Settings(64, 64, e: true));
        DefaultSettings.Add(128, new Settings(128, 32, e: true));
        DefaultSettings.Add(256, new Settings(256, 16, e: true));
        m_sharedInstance = new BlockPool();
    }

    private BlockAllocator CreateAllocator(int size)
    {
        size = AlignToBlock(size);
        Settings value = null;
        if (DefaultSettings.TryGetValue(size, out value))
        {
            return new BlockAllocator(value.blockSize, value.maxBlockNum, value.extendAble);
        }

        return new BlockAllocator(size, 8, extendAble: true);
    }

    public static int AlignToBlock(int size)
    {
        size = (size + 3) & -4;
        if ((size & (size - 1)) != 0)
        {
            size = NextPowerOf2(size);
        }

        return size;
    }

    public static int NextPowerOf2(int value)
    {
        int num;
        for (num = 1; num < value; num <<= 1)
        {
        }

        return num;
    }

    public static void MemSet(byte[] array, byte value)
    {
        if (array == null)
        {
            throw new ArgumentNullException("array");
        }

        int num = 32;
        int num2 = 0;
        int num3 = Math.Min(num, array.Length);
        while (num2 < num3)
        {
            array[num2++] = value;
        }

        num3 = array.Length;
        while (num2 < num3)
        {
            Buffer.BlockCopy(array, 0, array, num2, Math.Min(num, num3 - num2));
            num2 += num;
            num *= 2;
        }
    }

    public byte[] Allocate(int size)
    {
        size = AlignToBlock(size);
        BlockAllocator value = null;
        if (!m_bPool.TryGetValue(size, out value))
        {
            value = CreateAllocator(size);
            m_bPool.Add(size, value);
        }

        return value.Allocate();
    }

    public bool Free(ref byte[] p)
    {
        if (p != null)
        {
            BlockAllocator value = null;
            try
            {
                if (m_bPool.TryGetValue(p.Length, out value))
                {
                    return value.Free(p);
                }
            }
            finally
            {
                p = null;
            }
        }

        return false;
    }

    public BlockAllocator TryGet(int size)
    {
        size = AlignToBlock(size);
        BlockAllocator value = null;
        m_bPool.TryGetValue(size, out value);
        return value;
    }

    public BlockAllocator AddPool(int size, int maxnum, bool extendAble = true)
    {
        size = AlignToBlock(size);
        BlockAllocator value = null;
        if (!m_bPool.TryGetValue(size, out value))
        {
            value = new BlockAllocator(size, maxnum, extendAble);
            m_bPool.Add(size, value);
        }

        return value;
    }

    public int GetTotalSize()
    {
        int num = 0;
        foreach (KeyValuePair<int, BlockAllocator> item in m_bPool)
        {
            num += item.Value.GetTotalSize();
        }

        return num;
    }

    public void Sweep()
    {
        foreach (KeyValuePair<int, BlockAllocator> item in m_bPool)
        {
            item.Value.Sweep();
        }
    }
}