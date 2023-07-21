using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public partial class MessageSerializer {
        static public Boolean Read_PB_Boolean( BinaryReader reader ) {
            return reader.ReadBoolean();
        }
        static public SByte Read_PB_SByte( BinaryReader reader ) {
            return (SByte)Read_PB_UInt32(reader);
        }
        static public Byte Read_PB_Byte( BinaryReader reader ) {
            return (Byte)Read_PB_UInt32(reader);
        }
        static public Byte[] Read_PB_Bytes( BinaryReader reader ) {
            int len = Read_PB_Int32(reader);
            return reader.ReadBytes( (int)len );
        }
        static public Byte[] Read_PB_BytesFixSize( BinaryReader reader, UInt16 length ) {
            if ( length > 0 ) {
                return reader.ReadBytes( (int)length );
            } else {
                return null;
            }
        }
        static public Double Read_PB_Double( BinaryReader reader ) {
            return reader.ReadDouble();
        }
        static public Single Read_PB_Single( BinaryReader reader ) {
            return reader.ReadSingle();
        }
        static public Int16 Read_PB_Int16( BinaryReader reader ) {
            return (Int16)Read_PB_Int32(reader);
        }
        static public Int32 Read_PB_Int32( BinaryReader reader ) {
            return (Int32)Read_PB_UInt32(reader);
        }
        static public Int64 Read_PB_Int64( BinaryReader reader ) {
            return (Int64)Read_PB_UInt64(reader);
        }
        static public UInt16 Read_PB_UInt16( BinaryReader reader ) {
            return (UInt16)Read_PB_Int32(reader);
        }
        static public UInt32 Read_PB_UInt32( BinaryReader reader ) {
            uint tmp = reader.ReadByte();
            if (tmp < 128)
            {
                return tmp;
            }
            uint result = tmp & 0x7f;
            if ((tmp = reader.ReadByte()) < 128)
            {
                result |= tmp << 7;
            }
            else
            {
                result |= (tmp & 0x7f) << 7;
                if ((tmp = reader.ReadByte()) < 128)
                {
                    result |= tmp << 14;
                }
                else
                {
                    result |= (tmp & 0x7f) << 14;
                    if ((tmp = reader.ReadByte()) < 128)
                    {
                        result |= tmp << 21;
                    }
                    else
                    {
                        result |= (tmp & 0x7f) << 21;
                        result |= (tmp = reader.ReadByte()) << 28;
                        if (tmp >= 128)
                        {
                            // Discard upper 32 bits.
                            for (int i = 0; i < 5; i++)
                            {
                                if (reader.ReadByte() < 128)
                                {
                                    return result;
                                }
                            }
                            throw new Exception("encountered a malformed varin");
                        }
                    }
                }
            }
            return result;
        }
        static public UInt64 Read_PB_UInt64( BinaryReader reader ) {
            int shift = 0;
            ulong result = 0;
            do
            {
                byte b = reader.ReadByte();
                result |= (ulong)(b & 0x7F) << shift;
                if (b < 0x80)
                {
                    return result;
                }
                shift += 7;
            }
            while (shift < 64);
            throw new Exception("encountered a malformed varin");
        }
        static public String Read_PB_String( BinaryReader reader ) {
            Int16 len = Read_PB_Int16(reader);
            if ( len > 0 ) {
                Byte[] s = null;
                if ( len > ___string_temp_buffer.Length ) {
                    s = new Byte[len];
                } else {
                    s = ___string_temp_buffer;
                }
                reader.Read( s, 0, len );
                return System.Text.Encoding.UTF8.GetString( s, 0, len );
            } else {
                return String.Empty;
            }
        }
        static public Byte[] Read_PB_RawString( BinaryReader reader, bool cstyle = false ) {
            Int16 len = Read_PB_Int16(reader);
            Byte[] ret = null;
            if ( cstyle ) {
                ret = new Byte[len + 1];
                ret[len] = 0;
            } else {
                if ( len > 0 ) {
                    ret = new Byte[len];
                } else {
                    return null;
                }
            }
            if ( len > 0 ) {
                reader.Read( ret, 0, (int)len );
            }
            return ret;
        }
        static public String Read_PB_StringFixSize( BinaryReader reader, UInt16 length ) {
            if ( length > 0 ) {
                Byte[] s = null;
                if ( length <= ___string_temp_buffer.Length ) {
                    s = ___string_temp_buffer;
                    reader.Read( s, 0, length );
                } else {
                    s = reader.ReadBytes( (int)length );
                }
                var value = System.Text.Encoding.UTF8.GetString( s, 0, length );
                return value.Trim('\0');
            } else {
                return String.Empty;
            }
        }
        static public List<Boolean> ReadList_PB_Boolean( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Boolean>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add( reader.ReadBoolean() );
            }
            return ret;
        }
        static public List<SByte> ReadList_PB_SByte( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<SByte>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add( Read_PB_SByte(reader));
            }
            return ret;
        }
        static public List<Byte> ReadList_PB_Byte( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Byte>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_Byte(reader));
            }
            return ret;
        }
        static public List<Byte[]> ReadList_PB_Bytes( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Byte[]>( count );
            for ( int i = 0; i < count; ++i ) {
                Int16 len = Read_PB_Int16(reader);
                ret.Add( reader.ReadBytes( len ) );
            }
            return ret;
        }
        static public List<Double> ReadList_PB_Double( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Double>();
            for ( int i = 0; i < count; ++i ) {
                ret.Add( reader.ReadDouble() );
            }
            return ret;
        }
        static public List<Single> ReadList_PB_Single( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Single>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add( reader.ReadSingle() );
            }
            return ret;
        }
        static public List<Int16> ReadList_PB_Int16( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Int16>();
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_Int16(reader));
            }
            return ret;
        }
        static public List<Int32> ReadList_PB_Int32( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Int32>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_Int32(reader));
            }
            return ret;
        }
        static public List<Int64> ReadList_PB_Int64( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<Int64>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_Int64(reader));
            }
            return ret;
        }
        static public List<UInt16> ReadList_PB_UInt16( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<UInt16>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_UInt16(reader));
            }
            return ret;
        }
        static public List<UInt32> ReadList_PB_UInt32( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<UInt32>( count );

            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_UInt32(reader));
            }
            return ret;
        }
        static public List<UInt64> ReadList_PB_UInt64( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<UInt64>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add(Read_PB_UInt64(reader));
            }
            return ret;
        }
        static public List<String> ReadList_PB_String( BinaryReader reader ) {
            UInt16 count = reader.ReadUInt16();
            if ( count <= 0 ) {
                return null;
            }
            var ret = new List<String>( count );
            for ( int i = 0; i < count; ++i ) {
                ret.Add( Read_PB_String( reader ) );
            }
            return ret;
        }
        static public void Write_PB_Boolean( BinaryWriter writer, Boolean value ) {
            writer.Write( value );
        }
        static public void Write_PB_SByte( BinaryWriter writer, SByte value ) {
            Write_PB_Int32(writer, value);
        }
        static public void Write_PB_Byte( BinaryWriter writer, Byte value ) {
            Write_PB_UInt32(writer, value);
        }
        static public void Write_PB_Bytes( BinaryWriter writer, Byte[] value ) {
            Write_PB_Int32(writer, value.Length);
            writer.Write( value );
        }
        static public void Write_PB_BytesFixSize( BinaryWriter writer, Byte[] value, UInt16 length = UInt16.MaxValue ) {
            if ( length > 0 ) {
                if ( length == UInt16.MaxValue ) {
                    length = (UInt16)value.Length;
                }
                writer.Write( value, 0, Math.Min( length, value.Length ) );
                for ( int i = value.Length; i < length; ++i ) {
                    writer.Write( (Byte)0 );
                }
            }
        }
        static public void Write_PB_Double( BinaryWriter writer, Double value ) {
            writer.Write( value );
        }
        static public void Write_PB_Single( BinaryWriter writer, Single value ) {
            writer.Write( value );
        }
        static public void Write_PB_Int16( BinaryWriter writer, Int16 value ) {
            Write_PB_UInt32(writer, (UInt32)value);
        }
        static public void Write_PB_Int32( BinaryWriter writer, Int32 value ) {
            Write_PB_UInt32(writer, (UInt32)value);
        }
        static public void Write_PB_Int64( BinaryWriter writer, Int64 value ) {
            Write_PB_UInt64(writer, (UInt64)value);
        }
        static public void Write_PB_UInt16( BinaryWriter writer, UInt16 value ) {
            Write_PB_UInt32(writer, (UInt32)value);
        }
        static public void Write_PB_UInt32( BinaryWriter writer, UInt32 value ) {
            if (value < 128)
            {
                writer.Write((byte)value);
                return;
            }
            while (value > 127)
            {
                writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            writer.Write((byte)value);
        }
        static public void Write_PB_UInt64( BinaryWriter writer, UInt64 value ) {
            while (value > 127)
            {
                writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            writer.Write((byte)value);
        }
        static public void Write_PB_String( BinaryWriter writer, String value ) {
            if ( value.Length > 0 ) {
                Byte[] bytes = null;
                int length = 0;
                if ( value.Length * 6 <= ___string_temp_buffer.Length ) {
                    length = System.Text.Encoding.UTF8.GetBytes( value, 0, value.Length, ___string_temp_buffer, 0 );
                    bytes = ___string_temp_buffer;
                } else {
                    bytes = System.Text.Encoding.UTF8.GetBytes( value );
                    length = bytes.Length;
                }
                Write_PB_UInt16(writer, (UInt16)length);
                writer.Write( bytes, 0, length );
            } else {
                writer.Write( (UInt16)0 );
            }
        }
        static public void Write_PB_StringFixSize( BinaryWriter writer, String value, UInt16 length = UInt16.MaxValue ) {
            if ( length > 0 ) {
                Byte[] bytes = null;
                int _length = 0;
                if ( value.Length * 6 <= ___string_temp_buffer.Length ) {
                    _length = System.Text.Encoding.UTF8.GetBytes( value, 0, value.Length, ___string_temp_buffer, 0 );
                    bytes = ___string_temp_buffer;
                } else {
                    bytes = System.Text.Encoding.UTF8.GetBytes( value );
                    _length = bytes.Length;
                }
                if ( length == UInt16.MaxValue ) {
                    length = (UInt16)bytes.Length;
                }
                var _realLength = Math.Min( length, _length );
                writer.Write( bytes, 0, _realLength );
                for ( int i = _realLength; i < length; ++i ) {
                    writer.Write( (Byte)0 );
                }
            }
        }
        static public void WriteList_PB_Boolean( BinaryWriter writer, List<Boolean> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Boolean( writer, value[i] );
            }
        }
        static public void WriteList_PB_SByte( BinaryWriter writer, List<SByte> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_SByte( writer, value[i] );
            }
        }
        static public void WriteList_PB_Byte( BinaryWriter writer, List<Byte> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Byte( writer, value[i] );
            }
        }
        static public void WriteList_PB_Bytes( BinaryWriter writer, List<Byte[]> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Bytes( writer, value[i] );
            }
        }
        static public void WriteList_PB_Double( BinaryWriter writer, List<Double> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Double( writer, value[i] );
            }
        }
        static public void WriteList_PB_Single( BinaryWriter writer, List<Single> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Single( writer, value[i] );
            }
        }
        static public void WriteList_PB_Int16( BinaryWriter writer, List<Int16> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Int16( writer, value[i] );
            }
        }
        static public void WriteList_PB_Int32( BinaryWriter writer, List<Int32> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Int32( writer, value[i] );
            }
        }
        static public void WriteList_PB_Int64( BinaryWriter writer, List<Int64> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_Int64( writer, value[i] );
            }
        }
        static public void WriteList_PB_UInt16( BinaryWriter writer, List<UInt16> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_UInt16( writer, value[i] );
            }
        }
        static public void WriteList_PB_UInt32( BinaryWriter writer, List<UInt32> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_UInt32( writer, value[i] );
            }
        }
        static public void WriteList_PB_UInt64( BinaryWriter writer, List<UInt64> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_UInt64( writer, value[i] );
            }
        }
        static public void WriteList_PB_String( BinaryWriter writer, List<String> value ) {
            if ( value == null || value.Count == 0 ) {
                writer.Write((UInt16)0);
                return;
            }
            writer.Write((UInt16)value.Count);
            for ( int i = 0; i < value.Count; ++i ) {
                Write_PB_String( writer, value[i] );
            }
        }
        static public Vector4 Read_PB_Vector4( BinaryReader reader ) {
            return new Vector4( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }
        static public Vector3 Read_PB_Vector3( BinaryReader reader ) {
            return new Vector3( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }
        static public Quaternion Read_PB_Quaternion( BinaryReader reader ) {
            return new Quaternion( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }
        static public Vector2 Read_PB_Vector2( BinaryReader reader ) {
            return new Vector2( reader.ReadSingle(), reader.ReadSingle() );
        }
        static public Rect Read_PB_Rect(BinaryReader reader)
        {
            return new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
        static public void Write_PB_Quaternion( BinaryWriter writer, Quaternion value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
            writer.Write( value.w );
        }
        static public void Write_PB_Vector4( BinaryWriter writer, Vector4 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
            writer.Write( value.w );
        }
        static public void Write_PB_Vector3( BinaryWriter writer, Vector3 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
        }
        static public void Write_PB_Vector2( BinaryWriter writer, Vector2 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
        }
        static public void Read_PB_Vector4( BinaryReader reader, ref Vector4 value ) {
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            value.w = reader.ReadSingle();
        }
        static public void Read_PB_Vector3( BinaryReader reader, ref Vector3 value ) {
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
        }
        static public void Read_PB_Quaternion( BinaryReader reader, ref Quaternion value ) {
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            value.w = reader.ReadSingle();
        }
        static public void Read_PB_Vector2( BinaryReader reader, ref Vector2 value ) {
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
        }
        static public void Write_PB_Vector4( BinaryWriter writer, ref Vector4 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
            writer.Write( value.w );
        }
        static public void Write_PB_Vector3( BinaryWriter writer, ref Vector3 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
        }
        static public void Write_PB_Quaternion( BinaryWriter writer, ref Quaternion value ) {
            writer.Write( value.x );
            writer.Write( value.y );
            writer.Write( value.z );
            writer.Write( value.w );
        }
        static public void Write_PB_Vector2( BinaryWriter writer, ref Vector2 value ) {
            writer.Write( value.x );
            writer.Write( value.y );
        }
        static public void Write_PB_Rect(BinaryWriter writer, ref Rect value)
        {
            writer.Write(value.xMin);
            writer.Write(value.yMin);
            writer.Write(value.width);
            writer.Write(value.height);
        }
    }
