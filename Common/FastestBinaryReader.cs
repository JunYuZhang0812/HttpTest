using System;
using System.IO;
using System.Runtime.InteropServices;
public class FastestBinaryReader : IDisposable
{
    public class _BaseStream
    {
        internal FastestBinaryReader _this;

        public unsafe long Position
        {
            get
            {
                return _this.m_current - _this.m_head;
            }
            set
            {
                _this.m_current = _this.m_head + value;
            }
        }

        public unsafe long Seek(long offset, SeekOrigin opt)
        {
            switch (opt)
            {
                case SeekOrigin.Begin:
                    _this.m_current = _this.m_head + offset;
                    break;
                case SeekOrigin.Current:
                    _this.m_current += offset;
                    break;
                case SeekOrigin.End:
                    _this.m_current = _this.m_head + _this.m_buff.Length + offset;
                    break;
            }

            return (long)_this.m_current;
        }
    }

    private GCHandle m_gcHandle;

    private bool m_pinned = false;

    private byte[] m_buff = null;

    private unsafe byte* m_current = default(byte*);

    private unsafe byte* m_head = default(byte*);

    private _BaseStream m_baseStream = null;

    public _BaseStream BaseStream => m_baseStream;

    public unsafe byte* Current => m_current;

    public unsafe FastestBinaryReader(byte[] buff)
    {
        m_buff = buff;
        if (m_buff != null)
        {
            m_gcHandle = GCHandle.Alloc(m_buff, GCHandleType.Pinned);
            m_head = (byte*)(void*)m_gcHandle.AddrOfPinnedObject();
            m_current = m_head;
            m_pinned = true;
        }

        m_baseStream = new _BaseStream
        {
            _this = this
        };
    }

    public unsafe FastestBinaryReader(byte[] buff, byte* _buff)
    {
        m_buff = buff;
        m_pinned = false;
        m_head = _buff;
        m_current = m_head;
        m_baseStream = new _BaseStream
        {
            _this = this
        };
    }

    public unsafe byte* Forward(int size)
    {
        byte* current = m_current;
        m_current += size;
        return current;
    }

    public byte[] GetBuffer()
    {
        return m_buff;
    }

    public unsafe int GetOffset()
    {
        return (int)(m_current - m_head);
    }

    public unsafe long GetOffsetL()
    {
        return m_current - m_head;
    }

    ~FastestBinaryReader()
    {
        Dispose(disposing: false);
    }

    public void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (m_buff != null)
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    private void Dispose(bool disposing)
    {
        if (m_buff != null)
        {
            if (disposing)
            {
                m_buff = null;
                m_baseStream._this = null;
            }

            if (m_pinned)
            {
                m_gcHandle.Free();
                m_pinned = false;
            }
        }
    }

    public unsafe byte[] ReadBytes(int length)
    {
        byte[] array = new byte[length];
        Marshal.Copy((IntPtr)m_current, array, 0, length);
        m_current += length;
        return array;
    }

    public unsafe int Read(byte[] buffer, int index, int count)
    {
        Marshal.Copy((IntPtr)m_current, buffer, 0, count);
        m_current += count;
        return count;
    }

    public unsafe bool ReadBoolean()
    {
        bool result = ((Marshal.ReadByte((IntPtr)m_current) != 0) ? true : false);
        m_current++;
        return result;
    }

    public unsafe byte ReadByte()
    {
        byte result = Marshal.ReadByte((IntPtr)m_current);
        m_current++;
        return result;
    }

    public unsafe sbyte ReadSByte()
    {
        sbyte result = (sbyte)Marshal.ReadByte((IntPtr)m_current);
        m_current++;
        return result;
    }

    public unsafe char ReadChar()
    {
        char result = (char)Marshal.ReadInt16((IntPtr)m_current);
        m_current += 2;
        return result;
    }

    public unsafe double ReadDouble()
    {
        long num = Marshal.ReadInt64((IntPtr)m_current);
        double* ptr = (double*)(&num);
        m_current += 8;
        return *ptr;
    }

    public unsafe short ReadInt16()
    {
        short result = Marshal.ReadInt16((IntPtr)m_current);
        m_current += 2;
        return result;
    }

    public unsafe int ReadInt32()
    {
        int result = Marshal.ReadInt32((IntPtr)m_current);
        m_current += 4;
        return result;
    }

    public unsafe long ReadInt64()
    {
        long result = Marshal.ReadInt64((IntPtr)m_current);
        m_current += 8;
        return result;
    }

    public unsafe float ReadSingle()
    {
        int num = Marshal.ReadInt32((IntPtr)m_current);
        float* ptr = (float*)(&num);
        m_current += 4;
        return *ptr;
    }

    public unsafe ushort ReadUInt16()
    {
        ushort result = (ushort)Marshal.ReadInt16((IntPtr)m_current);
        m_current += 2;
        return result;
    }

    public unsafe uint ReadUInt32()
    {
        uint result = (uint)Marshal.ReadInt32((IntPtr)m_current);
        m_current += 4;
        return result;
    }

    public unsafe ulong ReadUInt64()
    {
        ulong result = (ulong)Marshal.ReadInt64((IntPtr)m_current);
        m_current += 8;
        return result;
    }
}