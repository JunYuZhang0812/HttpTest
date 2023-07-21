using System.Collections.Generic;

public class BlockAllocator
{
    public delegate void SweepAction();

    private const int DefaultMaxBlockNum = 100;

    private int m_blockSize = 0;

    private int m_maxBlockNum = 0;

    private bool m_extendAble = false;

    private int m_initMaxBlockNum = 0;

    private List<byte[]> m_freeBlocks = null;

    public SweepAction sweeper { get; set; }

    public bool extendAble
    {
        get
        {
            return extendAble;
        }
        set
        {
            extendAble = value;
        }
    }

    public int allocatedCount => m_maxBlockNum - m_freeBlocks.Count;

    public int freeCount => m_freeBlocks.Count;

    public void TrimExcess()
    {
        if (m_freeBlocks.Count > m_initMaxBlockNum)
        {
            m_freeBlocks.RemoveRange(m_initMaxBlockNum, m_freeBlocks.Count - m_initMaxBlockNum);
            m_freeBlocks.TrimExcess();
        }
    }

    public void Sweep()
    {
        if (sweeper == null)
        {
            TrimExcess();
        }
        else
        {
            sweeper();
        }
    }

    public BlockAllocator(int blockSize, int maxBlock, bool extendAble)
    {
        m_blockSize = blockSize;
        if (maxBlock <= 0)
        {
            maxBlock = 100;
        }

        m_freeBlocks = new List<byte[]>(maxBlock);
        m_maxBlockNum = m_freeBlocks.Capacity;
        m_initMaxBlockNum = m_maxBlockNum;
        m_extendAble = extendAble;
        for (int i = 0; i < m_maxBlockNum; i++)
        {
            m_freeBlocks.Add(new byte[blockSize]);
        }
    }

    public void Clear()
    {
        m_freeBlocks.Clear();
    }

    public void DeepClear()
    {
        m_freeBlocks.Clear();
        m_freeBlocks.TrimExcess();
    }

    public byte[] Allocate()
    {
        if (m_freeBlocks.Count == 0)
        {
            if (!m_extendAble)
            {
                return null;
            }

            m_freeBlocks.Add(new byte[m_blockSize]);
            m_maxBlockNum = m_freeBlocks.Capacity;
        }

        int index = m_freeBlocks.Count - 1;
        byte[] result = m_freeBlocks[index];
        m_freeBlocks.RemoveAt(index);
        return result;
    }

    public bool Free(byte[] p)
    {
        if (p.Length == m_blockSize)
        {
            m_freeBlocks.Add(p);
            return true;
        }

        return false;
    }

    public int GetTotalSize()
    {
        return m_freeBlocks.Capacity * m_blockSize;
    }
}