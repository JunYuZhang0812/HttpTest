using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using HttpTest;

public class NetSender : Singleton<NetSender>
{
    public void SendRequestKeepAlive()
    {
        var msg = new U2GS_KeepAlive();
        msg.m_timestamp = (uint)TimeManager.Instance.ServerTime;
        Networker.Instance.SendMessage(msg);
    }
    public void CacheEnterMapMsg(ushort id, Byte[] cache, ushort size)
    {
        /*if (size == 0)
        {
            UDebug.LogError(string.Format("The Message id = {0} size error", size));
        }
        if (id == MessageType.MSG_U2GS_RequestEnterMap
            || id == MessageType.MSG_U2GS_EnterDemonMap
            || id == MessageType.MSG_U2GS_Guild_enter_Camp
            || id == MessageType.MSG_U2GS_EnterCopyMap
            || id == MessageType.MSG_U2GS_EnterArtiCopyMap
            || id == MessageType.MSG_U2GS_EnterExceCopyMap
            || id == MessageType.MSG_U2GS_EnterDungeonDemonCopyMap
            || id == MessageType.MSG_U2GS_EnterActiveExtendDungeon
            || id == MessageType.MSG_U2GS_enterDungeonTeamExp
            || id == MessageType.MSG_U2GS_enterDungeonDragon
            || id == MessageType.MSG_U2GS_EnterArena
            || id == MessageType.MSG_U2GS_EnterBossWorld
            || id == MessageType.MSG_U2GS_BattlefieldEnter
            || id == MessageType.MSG_U2GS_MeleeEnter
            || id == MessageType.MSG_U2GS_AshuraEnter
            || id == MessageType.MSG_U2GS_TrystEnter
            || id == MessageType.MSG_U2GS_RequestEnterCeremonyMap
            || id == MessageType.MSG_U2GS_career_tower_enter_req
            || id == MessageType.MSG_U2GS_arena_match
            //|| id == MessageType.MSG_U2GS_startEnterTeamMap
            //|| id == MessageType.MSG_U2GS_replyEnterTeamMap
            || id == MessageType.MSG_U2GS_EnterRuneTower
            || id == MessageType.MSG_U2GS_YanMoRequestEnterMap
            || id == MessageType.MSG_U2GS_EnterBondYard
            || id == MessageType.MSG_U2GS_EnterGuildGuardMap
            || id == MessageType.MSG_U2GS_RequestEnterGC
            || id == MessageType.MSG_U2GS_RequestXOEnterMap
            || id == MessageType.MSG_U2GS_EnterGuildCamp
            || id == MessageType.MSG_U2GS_EnterDungeonMount)
        {
            Array.Copy(cache, m_enterMapCache, size);
            m_size = size;
            m_id = id;
        }*/
    }
}

#region U2GS_InitMsg
public class U2GS_InitMsg:BaseMessage
{
    public const UInt16 ID = MessageType.MSG_U2GS_InitMsg;
    override public UInt16 GetId()
    {
        return ID;
    }
    #region members
    public UInt32 m_serverTime = 0;
    public Int32 m_serverWaitTime = 0;
    #endregion

    #region methods
    public override int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_UInt32(writer, m_serverTime);
        MessageSerializer.Write_Int32(writer, m_serverWaitTime);
        return (int)(writer.BaseStream.Position - pos);
    }
    #endregion
}
#endregion

#region U2GS_KeepAlive
public class U2GS_KeepAlive : BaseMessage
{
    override public UInt16 GetId()
    {
        return ID;
    }
    public const UInt16 ID = MessageType.MSG_U2GS_KeepAlive;

    #region members
    public UInt32 m_timestamp = 0;
    #endregion

    #region methods
    override public int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_UInt32(writer, m_timestamp);
        return (int)(writer.BaseStream.Position - pos);
    }
    #endregion
}
#endregion
#region U2GS_PlayerIPSync
public partial class MessageSerializer
{
    public static U2GS_PlayerIPSync Read_U2GS_PlayerIPSync(BinaryReader s)
    {
        var ret = new U2GS_PlayerIPSync();
        ret.Deserialize(s);
        return ret;
    }
    public static List<U2GS_PlayerIPSync> ReadList_U2GS_PlayerIPSync(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if (count <= 0)
        {
            return null;
        }
        var ret = new List<U2GS_PlayerIPSync>(count);
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_U2GS_PlayerIPSync(s));
        }
        return ret;
    }
}
public class U2GS_PlayerIPSync : BaseMessage
{
    static new public BaseMessage Create(BinaryReader s)
    {
        return MessageSerializer.Read_U2GS_PlayerIPSync(s);
    }
    public const UInt16 ID = MessageType.MSG_U2GS_PlayerIPSync;
    public override ushort GetId()
    {
        return ID;
    }
    #region members
    public string m_name;
    public string m_toIP;
    public string m_message;
    #endregion
    public override int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_String(writer, m_name);
        MessageSerializer.Write_String(writer, m_toIP);
        MessageSerializer.Write_String(writer, m_message);
        return (int)(writer.BaseStream.Position - pos);
    }
    public override int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_name = MessageSerializer.Read_String(reader);
        m_toIP = MessageSerializer.Read_String(reader);
        m_message = MessageSerializer.Read_String(reader);
        return (int)(reader.BaseStream.Position - pos);
    }
}
#endregion
#region U2GS_Chat
public partial class MessageSerializer
{
    public static U2GS_Chat Read_U2GS_Chat(BinaryReader s)
    {
        var ret = new U2GS_Chat();
        ret.Deserialize(s);
        return ret;
    }
    public static List<U2GS_Chat> ReadList_U2GS_Chat(BinaryReader s)
    {
        Int16 count = s.ReadInt16();
        if (count <= 0)
        {
            return null;
        }
        var ret = new List<U2GS_Chat>(count);
        for (int i = 0; i < count; i++)
        {
            ret.Add(Read_U2GS_Chat(s));
        }
        return ret;
    }
}
public class U2GS_Chat:BaseMessage
{
    static new public BaseMessage Create(BinaryReader s)
    {
        return MessageSerializer.Read_U2GS_Chat(s);
    }
    public const UInt16 ID = MessageType.MSG_U2GS_Chat;
    public override ushort GetId()
    {
        return ID;
    }
    #region members
    public string m_toIP;
    public string m_message;
    public UInt32 m_opType;
    #endregion
    public override int Serialize(BinaryWriter writer)
    {
        long pos = writer.BaseStream.Position;
        MessageSerializer.Write_String(writer, m_toIP);
        MessageSerializer.Write_String(writer, m_message);
        MessageSerializer.Write_UInt32(writer, m_opType);
        return (int)(writer.BaseStream.Position - pos);
    }
    public override int Deserialize(BinaryReader reader)
    {
        long pos = reader.BaseStream.Position;
        m_toIP = MessageSerializer.Read_String(reader);
        m_message = MessageSerializer.Read_String(reader);
        m_opType = MessageSerializer.Read_UInt32(reader);
        return (int)(reader.BaseStream.Position - pos);
    }
}
#endregion
