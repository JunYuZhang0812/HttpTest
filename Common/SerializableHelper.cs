using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgRead
{
    public byte[] m_bytes = null;
    public int m_index = 0;

    private bool m_isEnd = false;
    public T Read<T>()
    {
        if(IsEnd())
        {
            return default(T);
        }
        //获取类型T占用的字节数
        var length = System.Runtime.InteropServices.Marshal.SizeOf<T>();
        var bytes = new byte[length];
        var endIndex = m_index + length;
        if( endIndex > m_bytes.Length )
        {
            endIndex = m_bytes.Length;
            m_isEnd = true;
        }
        for (int i = m_index; i < endIndex; i++)
        {
            bytes[i - m_index] = m_bytes[i];
        }
        return BinarySerializer.Deserialize<T>(bytes);
    }
    public bool IsEnd()
    {
        return m_isEnd;
    }
}
public class MsgWrite
{
    StringBuilder m_str = new StringBuilder();
    public void Write( object obj )
    {
        var bytes = BinarySerializer.Serialize(obj);
        m_str.Append( bytes );
    }
    public new string ToString()
    {
        return m_str.ToString();
    }
}
public class SerializableHelper
{
    public static MsgRead CreateMsgRead(string msg)
    {
        return new MsgRead()
        {
            m_bytes = BinarySerializer.Serialize(msg),
            m_index = 0,
        };
    }
    public static T Read<T>(MsgRead read )
    {
        return read.Read<T>();
    }
    public static MsgWrite CreateMsgWrite()
    {
        return new MsgWrite();
    }
    public static void Write(MsgWrite write , object obj)
    {
        write.Write(obj);
    }
}
