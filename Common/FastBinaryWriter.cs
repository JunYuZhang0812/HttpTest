using System.IO;
using System.Text;

public class FastBinaryWriter : BinaryWriter
{
    private byte[] m_buffer = new byte[32];

    public FastBinaryWriter(Stream input)
        : base(input)
    {
    }

    public FastBinaryWriter(Stream input, Encoding encoding)
        : base(input, encoding)
    {
    }

    public unsafe override void Write(double value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(double*)ptr = value;
        }

        base.Write(m_buffer, 0, 8);
    }

    public unsafe override void Write(float value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(float*)ptr = value;
        }

        base.Write(m_buffer, 0, 4);
    }

    public unsafe override void Write(int value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(int*)ptr = value;
        }

        base.Write(m_buffer, 0, 4);
    }

    public unsafe override void Write(long value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(long*)ptr = value;
        }

        base.Write(m_buffer, 0, 8);
    }

    public unsafe override void Write(short value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(short*)ptr = value;
        }

        base.Write(m_buffer, 0, 2);
    }

    public unsafe override void Write(uint value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(uint*)ptr = value;
        }

        base.Write(m_buffer, 0, 4);
    }

    public unsafe override void Write(ulong value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(ulong*)ptr = value;
        }

        base.Write(m_buffer, 0, 8);
    }

    public unsafe override void Write(ushort value)
    {
        fixed (byte* ptr = m_buffer)
        {
            *(ushort*)ptr = value;
        }

        base.Write(m_buffer, 0, 2);
    }
}