using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public class BinarySerializer
    {
        #region 对外接口

        #region 写入
        public static void Write_Byte(BinaryWriter w, byte value)
        {
            Write(w, value);
        }
        public static void Write_SByte(BinaryWriter w, sbyte value)
        {
            Write(w, value);
        }
        public static void Write_Bytes(BinaryWriter w, byte[] value)
        {
            Write(w, value);
        }
        public static void Write_Int16(BinaryWriter w, short value)
        {
            Write(w, value);
        }
        public static void Write_UInt16(BinaryWriter w, ushort value)
        {
            Write(w, value);
        }
        public static void Write_Int32(BinaryWriter w , int value )
        {
            Write(w,value);
        }
        public static void Write_UInt32(BinaryWriter w, uint value)
        {
            Write(w, value);
        }
        public static void Write_Int64(BinaryWriter w, long value)
        {
            Write(w, value);
        }
        public static void Write_UInt64(BinaryWriter w, ulong value)
        {
            Write(w, value);
        }
        public static void Write_Single(BinaryWriter w, float value)
        {
            Write(w, value);
        }
        public static void Write_Double(BinaryWriter w, double value)
        {
            Write(w, value);
        }
        public static void Write_Boolean(BinaryWriter w, bool value)
        {
            Write(w, value);
        }
        public static void Write_String(BinaryWriter w, string value)
        {
            Write(w, value);
        }
        public static void WriteList_Boolean(BinaryWriter w, List<bool> value)
        {
            Write(w, value);
        }
        public static void WriteList_Byte(BinaryWriter w, List<byte> value)
        {
            Write(w, value);
        }
        public static void WriteList_Bytes(BinaryWriter w, List<byte[]> value)
        {
            Write(w, value);
        }
        public static void WriteList_Double(BinaryWriter w, List<double> value)
        {
            Write(w, value);
        }
        public static void WriteList_Int16(BinaryWriter w, List<short> value)
        {
            Write(w, value);
        }
        public static void WriteList_Int32(BinaryWriter w, List<int> value)
        {
            Write(w, value);
        }
        public static void WriteList_Int64(BinaryWriter w, List<long> value)
        {
            Write(w, value);
        }
        public static void WriteList_SByte(BinaryWriter w, List<sbyte> value)
        {
            Write(w, value);
        }
        public static void WriteList_Single(BinaryWriter w, List<float> value)
        {
            Write(w, value);
        }
        public static void WriteList_String(BinaryWriter w, List<string> value)
        {
            Write(w, value);
        }
        public static void WriteList_UInt16(BinaryWriter w, List<ushort> value)
        {
            Write(w, value);
        }
        public static void WriteList_UInt32(BinaryWriter w, List<uint> value)
        {
            Write(w, value);
        }
        public static void WriteList_UInt64(BinaryWriter w, List<ulong> value)
        {
            Write(w, value);
        }
        public static void Write(BinaryWriter w, object value)
        {
            try
            {
                w.Write(Serialize(value));
            }
            catch(Exception e)
            {

            }
        }
        #endregion

        #region 读取
        public static bool Read_Boolean(BinaryReader r)
        {
            return Read<bool>(r);
        }
        public static byte Read_Byte(BinaryReader r)
        {
            return Read<byte>(r);
        }
        public static byte[] Read_Bytes(BinaryReader r)
        {
            return Read<byte[]>(r);
        }
        public static double Read_Double(BinaryReader r)
        {
            return Read<double>(r);
        }
        public static short Read_Int16(BinaryReader r)
        {
            return Read<short>(r);
        }
        public static int Read_Int32(BinaryReader r)
        {
            return Read<int>(r);
        }
        public static long Read_Int64(BinaryReader r)
        {
            return Read<long>(r);
        }
        public static sbyte Read_SByte(BinaryReader r)
        {
            return Read<sbyte>(r);
        }
        public static float Read_Single(BinaryReader r)
        {
            return Read<float>(r);
        }
        public static string Read_String(BinaryReader r)
        {
            return Read<string>(r);
        }
        public static ushort Read_UInt16(BinaryReader r)
        {
            return Read<ushort>(r);
        }
        public static uint Read_UInt32(BinaryReader r)
        {
            return Read<uint>(r);
        }
        public static ulong Read_UInt64(BinaryReader r)
        {
            return Read<ulong>(r);
        }
        public static List<bool> ReadList_Boolean(BinaryReader r)
        {
            return Read<List<bool>>(r);
        }
        public static List<byte> ReadList_Byte(BinaryReader r)
        {
            return Read<List<byte>>(r);
        }
        public static List<byte[]> ReadList_Bytes(BinaryReader r)
        {
            return Read<List<byte[]>>(r);
        }
        public static List<double> ReadList_Double(BinaryReader r)
        {
            return Read<List<double>>(r);
        }
        public static List<short> ReadList_Int16(BinaryReader r)
        {
            return Read<List<short>>(r);
        }
        public static List<int> ReadList_Int32(BinaryReader r)
        {
            return Read<List<int>>(r);
        }
        public static List<long> ReadList_Int64(BinaryReader r)
        {
            return Read<List<long>>(r);
        }
        public static List<sbyte> ReadList_SByte(BinaryReader r)
        {
            return Read<List<sbyte>>(r);
        }
        public static List<float> ReadList_Single(BinaryReader r)
        {
            return Read<List<float>>(r);
        }
        public static List<string> ReadList_String(BinaryReader r)
        {
            return Read<List<string>>(r);
        }
        public static List<ushort> ReadList_UInt16(BinaryReader r)
        {
            return Read<List<ushort>>(r);
        }
        public static List<uint> ReadList_UInt32(BinaryReader r)
        {
            return Read<List<uint>>(r);
        }
        public static List<ulong> ReadList_UInt64(BinaryReader r)
        {
            return Read<List<ulong>>(r);
        }
        public static bool Read_IsEnd(BinaryReader r)
        {
            return r.PeekChar() == -1;
        }

        public static T Read<T>(BinaryReader r)
        {
            try
            {
                var len = r.ReadInt32();
                if (len > 0)
                {
                    var bytes = r.ReadBytes(len);
                    return Deserialize<T>(bytes);
                }
            }
            catch (Exception e)
            {

            }
            return default(T);
        }
        #endregion

        public const string DefaultBinPath = "Assets/StreamingAssets/Bin";
        //写入例子
        private void WriteExample()
        {
            string binPath = DefaultBinPath + "//Node.bin";
            if( File.Exists(binPath))
            {
                File.Delete(binPath);
            }
            var bw = new BinaryWriter(new FileStream(binPath, FileMode.Create));
            Write_Int32(bw, 11);
            bw.Close();
        }
        //读取例子
        private void ReadExample()
        {
            string binPath = DefaultBinPath + "//Node.bin";
            using (Stream configfs = File.Open(binPath, FileMode.Open))
            {
                using(var re = new BinaryReader(configfs))
                {
                    int a = Read_Int32(re);
                }
            }
        }
        #endregion
        static Type stringType = typeof(string);
        #region 序列化
        //序列化
        public static byte[] Serialize(object param)
        {
            List<byte> datas = new List<byte>();
            var len = 0;
            byte[] data = null;
            if (param == null)
            {
                len = 0;
            }
            else
            {
                if (param is string)
                {
                    data = Encoding.UTF8.GetBytes((string)param);
                }
                else if (param is byte)
                {
                    data = new byte[] { (byte)param };
                }
                else if (param is bool)
                {
                    data = BitConverter.GetBytes((bool)param);
                }
                else if (param is short)
                {
                    data = BitConverter.GetBytes((short)param);
                }
                else if (param is ushort)
                {
                    data = BitConverter.GetBytes((ushort)param);
                }
                else if (param is int)
                {
                    data = BitConverter.GetBytes((int)param);
                }
                else if (param is uint)
                {
                    data = BitConverter.GetBytes((uint)param);
                }
                else if (param is long)
                {
                    data = BitConverter.GetBytes((long)param);
                }
                else if (param is ulong)
                {
                    data = BitConverter.GetBytes((ulong)param);
                }
                else if (param is float)
                {
                    data = BitConverter.GetBytes((float)param);
                }
                else if (param is double)
                {
                    data = BitConverter.GetBytes((double)param);
                }
                else if (param is DateTime)
                {
                    var str = "w1" + ((DateTime)param).Ticks;
                    data = Encoding.UTF8.GetBytes(str);
                }
                else if (param is Enum)
                {
                    var enumValType = Enum.GetUnderlyingType(param.GetType());
                    if (enumValType == typeof(byte))
                    {
                        data = new byte[] { (byte)param };
                    }
                    else if (enumValType == typeof(short))
                    {
                        data = BitConverter.GetBytes((short)param);
                    }
                    else if (enumValType == typeof(int))
                    {
                        data = BitConverter.GetBytes((int)param);
                    }
                    else
                    {
                        data = BitConverter.GetBytes((long)param);
                    }
                }
                else if (param is byte[])
                {
                    data = (byte[])param;
                }
                else
                {
                    var type = param.GetType();
                    if (type.IsGenericType || type.IsArray)
                    {
                        if (TypeHelper.DicTypeStrs.Contains(type.Name))
                        {
                            data = SerializeDic((IDictionary)param);
                        }
                        else if( TypeHelper.ListTypeStrs.Contains(type.Name) || type.IsArray )
                        {
                            data = SerializeList((IEnumerable)param);
                        }
                        else
                        {
                            data = SerializeClass(param, type);
                        }
                    }
                    else if(type.IsClass )
                    {
                        data = SerializeClass(param, type);
                    }
                    else if(type.IsValueType )
                    {
                        data = SerializeValueType(param,type);
                    }
                }
                if(data != null)
                {
                    len = data.Length;
                }
            }
            datas.AddRange(BitConverter.GetBytes(len));
            if(len > 0)
            {
                datas.AddRange(data);
            }
            return datas.ToArray();
        }
        public static byte[] Serialize(params object[] @params)
        {
            List<byte> datas = new List<byte>();
            if(@params != null )
            {
                foreach (var param in @params)
                {
                    datas.AddRange(Serialize(param));
                }
            }
            return datas.Count == 0 ? null : datas.ToArray();
        }
        private static byte[] SerializeClass(object obj , Type type)
        {
            if (obj == null) return null;
            List<byte> datas = new List<byte>();
            var len = 0;
            byte[] data = null;
            var ps = type.GetProperties();
            if( ps != null && ps.Length > 0)
            {
                List<object> clist = new List<object>();
                foreach (var p in ps)
                {
                    clist.Add(p.GetValue(obj, null));
                }
                data = Serialize(clist.ToArray());
                len = data.Length;
            }
            if(len > 0)
            {
                return data;
            }
            return null;
        }
        private static byte[] SerializeList(IEnumerable param)
        {
            if(param != null)
            {
                List<byte> slist = new List<byte>();
                var itemtype = param.AsQueryable().ElementType;
                foreach (var item in param)
                {
                    if(itemtype.IsClass && itemtype != stringType )
                    {
                        var ps = itemtype.GetProperties();
                        if( ps!= null && ps.Length > 0)
                        {
                            List<object> clist = new List<object>();
                            foreach (var p in ps)
                            {
                                clist.Add(p.GetValue(item, null));
                            }
                            var clen = 0;
                            var cdata = Serialize(clist.ToArray());
                            if(cdata != null)
                            {
                                clen = cdata.Length;
                            }
                            slist.AddRange(BitConverter.GetBytes(clen));
                            slist.AddRange(cdata);
                        }
                    }
                    else
                    {
                        var clen = 0;
                        var cdata = Serialize(item);
                        if(cdata != null)
                        {
                            clen = cdata.Length;
                        }
                        slist.AddRange(BitConverter.GetBytes(clen));
                        slist.AddRange(cdata);
                    }
                }
                if(slist.Count > 0)
                {
                    return slist.ToArray();
                }
            }
            return null;
        }
        private static byte[] SerializeDic(IDictionary param)
        {
            if( param != null && param.Count > 0 )
            {
                List<byte> list = new List<byte>();
                foreach (var item in param)
                {
                    var type = item.GetType();
                    var ps = type.GetProperties();
                    if( ps != null && ps.Length > 0 )
                    {
                        List<object> clist = new List<object>();
                        foreach (var p in ps)
                        {
                            clist.Add(p.GetValue(item, null));
                        }
                        var clen = 0;
                        var cdata = Serialize(clist.ToArray());
                        if( cdata != null )
                        {
                            clen = cdata.Length;
                        }
                        if(clen > 0)
                        {
                            list.AddRange(cdata);
                        }
                    }
                }
                return list.ToArray();
            }
            return null;
        }
        private static byte[] SerializeValueType(object obj, Type type)
        {
            if (obj == null) return null;
            List<byte> datas = new List<byte>();
            var len = 0;
            byte[] data = null;
            var ps = type.GetFields();
            if (ps != null && ps.Length > 0)
            {
                List<object> clist = new List<object>();
                foreach (var p in ps)
                {
                    if(p.IsPublic && !p.IsLiteral )
                    {
                        clist.Add(p.GetValue(obj));
                    }
                }
                data = Serialize(clist.ToArray());
                len = data.Length;
            }
            if (len > 0)
            {
                return data;
            }
            return null;
        }
        #endregion

        #region 反序列化
        public static object[] Deserialize(Type[] types , byte[] datas)
        {
            List<object> list = new List<object>();
            int offset = 0;
            for (int i = 0; i < types.Length; i++)
            {
                list.Add(Deserialize(types[i], datas, ref offset));
            }
            return list.ToArray();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            //int offset = 0;
            //return (T)Deserialize(typeof(T), data, ref offset);
            return (T)Deserialize(typeof(T), data);
        }
        private static object Deserialize(Type type, byte[] datas)
        {
            object obj = null;
            if (type == stringType)
            {
                obj = Encoding.UTF8.GetString(datas);
            }
            else if (type == typeof(byte))
            {
                obj = (datas);
            }
            else if (type == typeof(bool))
            {
                obj = (BitConverter.ToBoolean(datas, 0));
            }
            else if (type == typeof(short))
            {
                obj = (BitConverter.ToInt16(datas, 0));
            }
            else if (type == typeof(ushort))
            {
                obj = (BitConverter.ToUInt16(datas, 0));
            }
            else if (type == typeof(int))
            {
                obj = (BitConverter.ToInt32(datas, 0));
            }
            else if (type == typeof(uint))
            {
                obj = (BitConverter.ToUInt32(datas, 0));
            }
            else if (type == typeof(long))
            {
                obj = (BitConverter.ToInt64(datas, 0));
            }
            else if (type == typeof(ulong))
            {
                obj = (BitConverter.ToUInt64(datas, 0));
            }
            else if (type == typeof(float))
            {
                obj = (BitConverter.ToSingle(datas, 0));
            }
            else if (type == typeof(double))
            {
                obj = (BitConverter.ToDouble(datas, 0));
            }
            else if (type == typeof(decimal))
            {
                obj = (BitConverter.ToDouble(datas, 0));
            }
            else if (type == typeof(DateTime))
            {
                var dstr = Encoding.UTF8.GetString(datas);
                var ticks = long.Parse(dstr.Substring(2));
                obj = (new DateTime(ticks));
            }
            else if (type.BaseType == typeof(Enum))
            {
                var numType = Enum.GetUnderlyingType(type);
                if (numType == typeof(byte))
                {
                    obj = Enum.ToObject(type, datas[0]);
                }
                else if (numType == typeof(short))
                {
                    obj = Enum.ToObject(type, BitConverter.ToInt16(datas, 0));
                }
                else if (numType == typeof(int))
                {
                    obj = Enum.ToObject(type, BitConverter.ToInt32(datas, 0));
                }
                else
                {
                    obj = Enum.ToObject(type, BitConverter.ToInt64(datas, 0));
                }
            }
            else if (type == typeof(byte[]))
            {
                obj = (byte[])datas;
            }
            else if (type.IsGenericType)
            {
                if (TypeHelper.ListTypeStrs.Contains(type.Name))
                {
                    obj = DeserializeList(type, datas);
                }
                else if (TypeHelper.DicTypeStrs.Contains(type.Name))
                {
                    obj = DeserializeDic(type, datas);
                }
                else
                {
                    obj = DeserializeClass(type, datas);
                }
            }
            else if (type.IsClass)
            {
                obj = DeserializeClass(type, datas);
            }
            else if (type.IsArray)
            {
                obj = DeserializeArray(type, datas);
            }
            else if (type.IsValueType)
            {
                obj = DeserializeValueType(type, datas);
            }
            else
            {
                throw new Exception("SAEASerialize.Deserialize 未定义的类型：" + type.ToString());
            }
            return obj;
        }
        private static object Deserialize(Type type , byte[] datas , ref int offset)
        {
            object obj = null;
            var len = 0;
            byte[] data = null;

            len = BitConverter.ToInt32(datas, offset);
            offset += 4;

            if(len > 0)
            {
                data = new byte[len];
                Buffer.BlockCopy(datas, offset, data, 0, len);
                offset += len;
                if (type == stringType)
                {
                    obj = Encoding.UTF8.GetString(data);
                }
                else if (type == typeof(byte))
                {
                    obj = (data);
                }
                else if (type == typeof(bool))
                {
                    obj = (BitConverter.ToBoolean(data, 0));
                }
                else if (type == typeof(short))
                {
                    obj = (BitConverter.ToInt16(data, 0));
                }
                else if (type == typeof(ushort))
                {
                    obj = (BitConverter.ToUInt16(data, 0));
                }
                else if (type == typeof(int))
                {
                    obj = (BitConverter.ToInt32(data, 0));
                }
                else if (type == typeof(uint))
                {
                    obj = (BitConverter.ToUInt32(data, 0));
                }
                else if (type == typeof(long))
                {
                    obj = (BitConverter.ToInt64(data, 0));
                }
                else if (type == typeof(ulong))
                {
                    obj = (BitConverter.ToUInt64(data, 0));
                }
                else if (type == typeof(float))
                {
                    obj = (BitConverter.ToSingle(data, 0));
                }
                else if (type == typeof(double))
                {
                    obj = (BitConverter.ToDouble(data, 0));
                }
                else if (type == typeof(decimal))
                {
                    obj = (BitConverter.ToDouble(data, 0));
                }
                else if (type == typeof(DateTime))
                {
                    var dstr = Encoding.UTF8.GetString(data);
                    var ticks = long.Parse(dstr.Substring(2));
                    obj = (new DateTime(ticks));
                }
                else if (type.BaseType == typeof(Enum))
                {
                    var numType = Enum.GetUnderlyingType(type);
                    if( numType == typeof(byte))
                    {
                        obj = Enum.ToObject(type, data[0]);
                    }
                    else if( numType == typeof(short))
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt16(data, 0));
                    }
                    else if (numType == typeof(int))
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt32(data, 0));
                    }
                    else
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt64(data, 0));
                    }
                }
                else if(type == typeof(byte[]))
                {
                    obj = (byte[])data;
                }
                else if(type.IsGenericType)
                {
                    if(TypeHelper.ListTypeStrs.Contains(type.Name))
                    {
                        obj = DeserializeList(type, data);
                    }
                    else if (TypeHelper.DicTypeStrs.Contains(type.Name))
                    {
                        obj = DeserializeDic(type, data);
                    }
                    else
                    {
                        obj = DeserializeClass(type, data);
                    }
                }
                else if (type.IsClass)
                {
                    obj = DeserializeClass(type, data);
                }
                else if (type.IsArray)
                {
                    obj = DeserializeArray(type, data);
                }
                else if (type.IsValueType)
                {
                    obj = DeserializeValueType(type, data);
                }
                else
                {
                    throw new Exception("SAEASerialize.Deserialize 未定义的类型：" + type.ToString());
                }
            }
            return obj;
        }
        private static object DeserializeClass(Type type, byte[] datas)
        {
            var tinfo = TypeHelper.GetOrAddInstance(type);

            var instance = tinfo.Instance;

            var ts = new List<Type>();

            var ps = type.GetProperties();

            foreach (var p in ps)
            {
                ts.Add(p.PropertyType);
            }

            var vas = Deserialize(ts.ToArray(), datas);

            for (int j = 0; j < ps.Length; j++)
            {
                try
                {
                    if (!ps[j].PropertyType.IsGenericType)
                    {
                        ps[j].SetValue(instance, vas[j], null);
                    }
                    else
                    {
                        Type genericTypeDefinition = ps[j].PropertyType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            ps[j].SetValue(instance, Convert.ChangeType(vas[j], Nullable.GetUnderlyingType(ps[j].PropertyType)), null);
                        }
                        else
                        {
                            ps[j].SetValue(instance, vas[j], null);
                        }
                    }
                }
                catch
                {
                    throw new Exception("SAEASerialize.Deserialize 未定义的类型：" + type.ToString());
                }
            }

            return instance;
        }

        private static object DeserializeList(Type type, byte[] datas)
        {
            var info = TypeHelper.GetOrAddInstance(type);

            var instance = info.Instance;

            if (info.ArgTypes[0].IsClass && info.ArgTypes[0] != stringType)
            {
                //子项内容
                var slen = 0;
                var soffset = 0;
                while (soffset < datas.Length)
                {
                    slen = BitConverter.ToInt32(datas, soffset);
                    if (slen > 0)
                    {
                        var sobj = Deserialize(info.ArgTypes[0], datas, ref soffset);
                        if (sobj != null)
                            info.MethodInfo.Invoke(instance, new object[] { sobj });

                    }
                    else
                    {
                        info.MethodInfo.Invoke(instance, null);
                    }
                }
                return instance;
            }
            else
            {
                //子项内容
                var slen = 0;
                var soffset = 0;
                while (soffset < datas.Length)
                {
                    var len = BitConverter.ToInt32(datas, soffset);
                    var data = new byte[len];
                    Buffer.BlockCopy(datas, soffset + 4, data, 0, len);
                    soffset += 4;
                    slen = BitConverter.ToInt32(datas, soffset);
                    if (slen > 0)
                    {
                        var sobj = Deserialize(info.ArgTypes[0], datas, ref soffset);
                        if (sobj != null)
                            info.MethodInfo.Invoke(instance, new object[] { sobj });
                    }
                    else
                    {
                        info.MethodInfo.Invoke(instance, null);
                    }
                }
                return instance;
            }

        }

        private static object DeserializeArray(Type type, byte[] datas)
        {
            var obj = DeserializeList(type, datas);

            if (obj == null) return null;

            var list = (obj as List<object>);

            return list.ToArray();
        }

        private static object DeserializeDic(Type type, byte[] datas)
        {
            var tinfo = TypeHelper.GetOrAddInstance(type);

            var instance = tinfo.Instance;

            //子项内容
            var slen = 0;

            var soffset = 0;

            int m = 1;

            object key = null;
            object val = null;

            while (soffset < datas.Length)
            {
                slen = BitConverter.ToInt32(datas, soffset);
                var sdata = new byte[slen + 4];
                Buffer.BlockCopy(datas, soffset, sdata, 0, slen + 4);
                soffset += slen + 4;
                if (m % 2 == 1)
                {
                    object v = null;
                    if (slen > 0)
                    {
                        int lloffset = 0;
                        var sobj = Deserialize(tinfo.ArgTypes[0], sdata, ref lloffset);
                        if (sobj != null)
                            v = sobj;
                    }
                    key = v;
                }
                else
                {
                    object v = null;
                    if (slen > 0)
                    {
                        int lloffset = 0;
                        var sobj = Deserialize(tinfo.ArgTypes[1], sdata, ref lloffset);
                        if (sobj != null)
                            v = sobj;
                    }
                    val = v;
                    tinfo.MethodInfo.Invoke(instance, new object[] { key, val });
                }
                m++;
            }
            return instance;
        }
        private static object DeserializeValueType(Type type, byte[] datas)
        {
            var tinfo = TypeHelper.GetOrAddInstance(type);

            var instance = tinfo.Instance;

            var ts = new List<Type>();
            var ps = type.GetFields();
            var values = new List<FieldInfo>();
            foreach (var p in ps)
            {
                if( p.IsPublic && !p.IsLiteral )
                {
                    values.Add(p);
                    ts.Add(p.FieldType);
                }
            }

            var vas = Deserialize(ts.ToArray(), datas);

            for (int j = 0; j < values.Count; j++)
            {
                try
                {
                    if (!values[j].FieldType.IsGenericType)
                    {
                        values[j].SetValue(instance, vas[j]);
                    }
                    else
                    {
                        Type genericTypeDefinition = values[j].FieldType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            values[j].SetValue(instance, Convert.ChangeType(vas[j], Nullable.GetUnderlyingType(values[j].FieldType)));
                        }
                        else
                        {
                            values[j].SetValue(instance, vas[j]);
                        }
                    }
                }
                catch
                {
                    throw new Exception("SAEASerialize.Deserialize 未定义的类型：" + type.ToString());
                }
            }

            return instance;
        }
        #endregion
    }
}
