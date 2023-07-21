using HttpTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#region GS2U_InitMsg
public partial class MessageSerializer
{
    static public GS2U_InitMsg Read_GS2U_InitMsg( BinaryReader s)
    {
        var ret = new GS2U_InitMsg();
        ret.Deserialize(s);
        return ret;
    }
    static public List<GS2U_InitMsg> ReadList_GS2U_InitMsg( BinaryReader s )
    {
        Int16 count = s.ReadInt16();
        if( count <= 0 )
        {
            return null;
        }
        var ret = new List<GS2U_InitMsg>( count );
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_GS2U_InitMsg(s));
        }
        return ret;
    }
}
public class GS2U_InitMsg:BaseMessage
{
    public static new BaseMessage Create( BinaryReader s )
    {
        return MessageSerializer.Read_GS2U_InitMsg( s );
    }
    public override ushort GetId()
    {
        return ID;
    }
    public const UInt16 ID = MessageType.MSG_GS2U_InitMsg;

    #region members
    public UInt32 m_serverTime = 0;
    public Int32 m_serverWaitTime = 0;
    #endregion

    #region methods
    public override int Deserialize(BinaryReader reader )
    {
        long pos = reader.BaseStream.Position;
        m_serverTime = MessageSerializer.Read_UInt32 ( reader );
        m_serverWaitTime = MessageSerializer.Read_Int32(reader);
        return (int)(reader.BaseStream.Position - pos);
    }
    #endregion
}
#endregion

#region GS2U_KeepAlive
public partial class MessageSerializer
{
    static public GS2U_KeepAlive Read_GS2U_KeepAlive(BinaryReader s)
    {
        var ret = new GS2U_KeepAlive();
        ret.Deserialize(s);
        return ret;
    }
    static public List<GS2U_KeepAlive> ReadList_GS2U_KeepAlive(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if (count <= 0)
        {
            return null;
        }
        var ret = new List<GS2U_KeepAlive>(count);
        for (int i = 0; i < count; ++i)
        {
            ret.Add(Read_GS2U_KeepAlive(s));
        }
        return ret;
    }
}
public class GS2U_KeepAlive:BaseMessage
{
    static new public BaseMessage Create(BinaryReader s)
    {
        return MessageSerializer.Read_GS2U_KeepAlive(s);
    }
    override public UInt16 GetId()
    {
        return ID;
    }
    public const UInt16 ID = MessageType.MSG_GS2U_KeepAlive;

    #region members
    public Int32 m_timesTamp = 0;
    public UInt32 m_resVersion = 0;
    #endregion

    #region methods
    override public int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_timesTamp = MessageSerializer.Read_Int32(reader);
        m_resVersion = MessageSerializer.Read_UInt32(reader);
        return (int)(reader.BaseStream.Position - pos);
    }
    #endregion
}
#endregion

#region GS2U_PlayerIPSync
public partial class MessageSerializer
{
    public static PlayerIP Read_PlayerIP(BinaryReader s)
    {
        var ret = new PlayerIP();
        ret.Deserialize(s);
        return ret;
    }
    public static List<PlayerIP> ReadList_PlayerIP(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if (count <= 0)
        {
            return null;
        }
        var ret = new List<PlayerIP>(count);
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_PlayerIP(s));
        }
        return ret;
    }
    static public void Write_PlayerIP(BinaryWriter s, PlayerIP value)
    {
        value.Serialize(s);
    }
    static public void WriteList_PlayerIP(BinaryWriter s, List<PlayerIP> value)
    {
        Write_Int16(s, (Int16)value.Count);
        for (int i = 0; i < value.Count; ++i)
        {
            value[i].Serialize(s);
        }
    }
}
public class PlayerIP : SerializeAble
{
    #region members
    public string m_ip;
    public string m_name;
    #endregion

    #region methods
    override public int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_String(writer, m_ip);
        MessageSerializer.Write_String(writer, m_name);
        return (int)(writer.BaseStream.Position - pos);
    }
    override public int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_ip = MessageSerializer.Read_String(reader);
        m_name = MessageSerializer.Read_String(reader);
        return (int)(reader.BaseStream.Position - pos);
    }
    #endregion
}
public partial class MessageSerializer
{
    public static GS2U_PlayerIPSync Read_GS2U_PlayerIPSync(BinaryReader s)
    {
        var ret = new GS2U_PlayerIPSync();
        ret.Deserialize(s);
        return ret;
    }
    public static List<GS2U_PlayerIPSync> ReadList_GS2U_PlayerIPSync(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if (count <= 0)
        {
            return null;
        }
        var ret = new List<GS2U_PlayerIPSync>(count);
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_GS2U_PlayerIPSync(s));
        }
        return ret;
    }
}
public class GS2U_PlayerIPSync : BaseMessage
{
    static new public BaseMessage Create(BinaryReader s)
    {
        return MessageSerializer.Read_GS2U_PlayerIPSync(s);
    }
    public override ushort GetId()
    {
        return ID;
    }
    public const UInt16 ID = MessageType.MSG_GS2U_PlayerIPSync;

    #region members
    public List<PlayerIP> m_playerIPList;
    #endregion
    public override int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.WriteList_PlayerIP(writer, m_playerIPList);
        return (int)(writer.BaseStream.Position - pos);
    }
    public override int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_playerIPList = MessageSerializer.ReadList_PlayerIP(reader);
        return (int)(reader.BaseStream.Position - pos);
    }

}
#endregion

#region GS2U_Chat
public partial class MessageSerializer
{
    public static GS2U_Chat Read_GS2U_Chat(BinaryReader s)
    {
        var ret = new GS2U_Chat();
        ret.Deserialize(s);
        return ret;
    }
    public static List<GS2U_Chat> ReadList_GS2U_Chat(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if( count <= 0 )
        {
            return null;
        }
        var ret = new List<GS2U_Chat>(count);
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_GS2U_Chat(s));
        }
        return ret;
    }
}
public class GS2U_Chat : BaseMessage
{
    static new public BaseMessage Create(BinaryReader s)
    {
        return MessageSerializer.Read_GS2U_Chat(s);
    }
    public override ushort GetId()
    {
        return ID;
    }
    public const UInt16 ID = MessageType.MSG_GS2U_Chat;

    #region members
    public string m_name;
    public string m_formIP;
    public string m_message;
    public uint m_opType;
    #endregion
    public override int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_String(writer, m_name);
        MessageSerializer.Write_String(writer, m_formIP);
        MessageSerializer.Write_String(writer, m_message);
        MessageSerializer.Write_UInt32(writer, m_opType);
        return (int)(writer.BaseStream.Position - pos);
    }
    public override int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_name = MessageSerializer.Read_String(reader);
        m_formIP = MessageSerializer.Read_String(reader);
        m_message = MessageSerializer.Read_String(reader);
        m_opType = MessageSerializer.Read_UInt32(reader);
        return (int)(reader.BaseStream.Position - pos);
    }

}
#endregion
