using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MessageData
{
    static MessageData()
    {
        SimplePool<MessageData>.CtorFunc = (m) => {
            m.id = 0;
            m.data = null;
            m.dataSize = 0;
        };
        SimplePool<MessageData>.DtorFunc = (m) => {
            m.id = 0;
            m.data = null;
            m.dataSize = 0;
        };
        SimplePool<MessageData>.CreateInstance = () => {
            return new MessageData();
        };
    }

    public static void ResetPool()
    {
        SimplePool<MessageData>.sharedInstance.Sweep();
    }

    public static MessageData Allocate()
    {
        return SimplePool<MessageData>.sharedInstance.Allocate();
    }

    public static void Free(ref MessageData m)
    {
        SimplePool<MessageData>.sharedInstance.Free(ref m);
        m = null;
    }

    public const int HeaderBufferSize = 4; // sizeof( dataSize : UInt16 ) + sizeof( id : UInt16 )

    public UInt16 id = 0;
    public UInt16 dataSize = 0;
    public Byte[] data = null;
    public int timeStamp = 0;

    public bool FromReceivedBuf(byte[] buf)
    {
        if (buf.Length < HeaderBufferSize)
        {
            return false;
        }
        dataSize = BitConverter.ToUInt16(buf, 0);
        id = BitConverter.ToUInt16(buf, 2);
        dataSize -= HeaderBufferSize;
        return true;
    }
}
