using System.IO;
using System.Text;

public class FastBinaryReader : BinaryReader
{
    private byte[] m_buffer = new byte[32];

    public FastBinaryReader(Stream input)
        : base(input)
    {
    }

    public FastBinaryReader(Stream input, Encoding encoding)
        : base(input, encoding)
    {
    }

    public unsafe override double ReadDouble()
    {
        base.Read(m_buffer, 0, 8);
        fixed (byte* ptr = m_buffer)
        {
            return *(double*)ptr;
        }
    }

    public unsafe override short ReadInt16()
    {
        base.Read(m_buffer, 0, 2);
        fixed (byte* ptr = m_buffer)
        {
            return *(short*)ptr;
        }
    }

    public unsafe override int ReadInt32()
    {
        base.Read(m_buffer, 0, 4);
        fixed (byte* ptr = m_buffer)
        {
            return *(int*)ptr;
        }
    }

    public unsafe override long ReadInt64()
    {
        base.Read(m_buffer, 0, 8);
        fixed (byte* ptr = m_buffer)
        {
            return *(long*)ptr;
        }
    }

    public unsafe override float ReadSingle()
    {
        base.Read(m_buffer, 0, 4);
        fixed (byte* ptr = m_buffer)
        {
            return *(float*)ptr;
        }
    }

    public unsafe override ushort ReadUInt16()
    {
        base.Read(m_buffer, 0, 2);
        fixed (byte* ptr = m_buffer)
        {
            return *(ushort*)ptr;
        }
    }

    public unsafe override uint ReadUInt32()
    {
        base.Read(m_buffer, 0, 4);
        fixed (byte* ptr = m_buffer)
        {
            return *(uint*)ptr;
        }
    }

    public unsafe override ulong ReadUInt64()
    {
        base.Read(m_buffer, 0, 8);
        fixed (byte* ptr = m_buffer)
        {
            return *(ulong*)ptr;
        }
    }
}
