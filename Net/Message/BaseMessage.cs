using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class SerializeAble
{
    abstract public int Serialize( BinaryWriter s);
    abstract public int Deserialize( BinaryReader s);
}

public abstract class BaseMessage :SerializeAble
{
    public const int BYTE_BUFFER_SIZE = 40960;
    public const int MAX_STRING_LEN = 4096 * 8;

    abstract public UInt16 GetId();
    static public BaseMessage Create( BinaryReader s )
    {
        return null;
    }
    public override int Serialize(BinaryWriter s)
    {
        throw new NotImplementedException();
    }
    public override int Deserialize(BinaryReader s)
    {
        throw new NotSupportedException();
    }
}
