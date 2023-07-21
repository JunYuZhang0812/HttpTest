using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MessageFactory
{
    public const Int32 ProtocolVersion = 824;
    public static void Initialize()
    {
        RegisterMessageId(MessageType.MSG_GS2U_Chat, GS2U_Chat.Create);
        RegisterMessageId(MessageType.MSG_GS2U_PlayerIPSync, GS2U_PlayerIPSync.Create);
    }

    public delegate BaseMessage CreateFunc(BinaryReader s);
    public class Item
    {
        public CreateFunc create;
    }
    static private Dictionary<UInt16, Item> items = new Dictionary<UInt16, Item>();
    private static Dictionary<Type, UInt16> types = new Dictionary<Type, UInt16>();

    public static UInt16 GetMessageId<T>() where T : BaseMessage
    {
        UInt16 id;
        if( types.TryGetValue(typeof(T), out id) )
        {
            return id;
        }
        return MessageType.MSG_Undefined;
    }
    static public UInt16 GetMessageId(Type type)
    {
        UInt16 id;
        if (types.TryGetValue(type, out id))
        {
            return id;
        }
        else
        {
            return MessageType.MSG_Undefined;
        }
    }
    static public bool RegisterMessageId(UInt16 id, CreateFunc func)
    {
        if (items.ContainsKey(id))
        {
            return false;
        }
        if (types.ContainsKey(func.Method.DeclaringType))
        {
            return false;
        }
        Item item = new Item();
        item.create = func;
        items.Add(id, item);
        types.Add(func.Method.DeclaringType, id);
        return true;
    }
    static public BaseMessage Create(UInt16 id, BinaryReader s)
    {
        Item item;
        if (items.TryGetValue(id, out item))
        {
            return item.create(s);
        }
        else
        {
            return null;
        }
    }
}