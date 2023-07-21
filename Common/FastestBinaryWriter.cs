using System;
using System.IO;
using System.Runtime.InteropServices;

public class FastestBinaryWriter : IDisposable
{
    public class _BaseStream
    {
        internal FastestBinaryWriter _this;

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

    private byte[] m_buff = null;

    private bool m_pinned = false;

    private unsafe byte* m_current = default(byte*);

    private unsafe byte* m_head = default(byte*);

    private _BaseStream m_baseStream = null;

    public _BaseStream BaseStream => m_baseStream;

    public unsafe byte* Current => m_current;

    public unsafe FastestBinaryWriter(byte[] buff)
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

    public unsafe FastestBinaryWriter(byte[] buff, byte* _buff)
    {
        m_buff = buff;
        m_head = _buff;
        m_current = m_head;
        m_pinned = false;
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

    ~FastestBinaryWriter()
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
                m_pinned = false;
                m_gcHandle.Free();
            }
        }
    }

    public unsafe long Seek(long offset, SeekOrigin opt)
    {
        switch (opt)
        {
            case SeekOrigin.Begin:
                m_current = m_head + offset;
                break;
            case SeekOrigin.Current:
                m_current += offset;
                break;
            case SeekOrigin.End:
                m_current = m_head + m_buff.Length + offset;
                break;
        }

        return (long)m_current;
    }

    public unsafe void Write(byte* bytes, int size)
    {
        m_current += size;
    }

    public unsafe void Write(byte[] bytes)
    {
        Marshal.Copy(bytes, 0, (IntPtr)m_current, bytes.Length);
        m_current += bytes.Length;
    }

    public unsafe void Write(byte[] bytes, int startIndex, int length)
    {
        Marshal.Copy(bytes, startIndex, (IntPtr)m_current, length);
        m_current += length;
    }

    public unsafe void Write(bool value)
    {
        Marshal.WriteByte((IntPtr)m_current, (byte)(value ? 1u : 0u));
        m_current++;
    }

    public unsafe void Write(char value)
    {
        Marshal.WriteInt16((IntPtr)m_current, (short)value);
        m_current += 2;
    }

    public unsafe void Write(byte value)
    {
        Marshal.WriteByte((IntPtr)m_current, value);
        m_current++;
    }

    public unsafe void Write(sbyte value)
    {
        Marshal.WriteByte((IntPtr)m_current, (byte)value);
        m_current++;
    }

    public unsafe void Write(double value)
    {
        long* ptr = (long*)(&value);
        Marshal.WriteInt64((IntPtr)m_current, *ptr);
        m_current += 8;
    }

    public unsafe void Write(float value)
    {
        int* ptr = (int*)(&value);
        Marshal.WriteInt32((IntPtr)m_current, *ptr);
        m_current += 4;
    }

    public unsafe void Write(int value)
    {
        Marshal.WriteInt32((IntPtr)m_current, value);
        m_current += 4;
    }

    public unsafe void Write(long value)
    {
        Marshal.WriteInt64((IntPtr)m_current, value);
        m_current += 8;
    }

    public unsafe void Write(short value)
    {
        Marshal.WriteInt16((IntPtr)m_current, value);
        m_current += 2;
    }

    public unsafe void Write(uint value)
    {
        Marshal.WriteInt32((IntPtr)m_current, (int)value);
        m_current += 4;
    }

    public unsafe void Write(ulong value)
    {
        Marshal.WriteInt64((IntPtr)m_current, (long)value);
        m_current += 8;
    }

    public unsafe void Write(ushort value)
    {
        Marshal.WriteInt16((IntPtr)m_current, (short)value);
        m_current += 2;
    }
}
