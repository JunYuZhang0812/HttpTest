using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// 程序集获取方法
    /// </summary>
    public static class TypeHelper
    {
        public static readonly string[] ListTypeStrs = { "List`1", "HashSet`1", "IList`1", "ISet`1", "ICollection`1", "IEnumerable`1" };
        public static readonly string[] DicTypeStrs = { "Dictionary`2", "IDictionary`2" };
        static readonly StackTrace StackTrace = new StackTrace(true);

        public static Type[] GetDefalt()
        {
            var frames = StackTrace.GetFrames();
            MethodBase[] mbs = new MethodBase[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                mbs[i] = frames[i].GetMethod();
                if(mbs[i].Name.Equals("Generate") || mbs[i].Name.Equals("Start"))
                {
                    return frames[i + 1].GetMethod().DeclaringType.Assembly.GetTypes();
                }
            }
            return null;
        }
        /// <summary>
        /// 获取类型名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            if(type.IsClass || type.IsInterface )
            {
                if(type.IsGenericType)
                {
                    var tName = type.Name.Substring(0, type.Name.Length - 2) + "<";
                    var gArgs = type.GetGenericArguments();
                    for (int i = 0; i < gArgs.Length; i++)
                    {
                        if( i == gArgs.Length - 1)
                        {
                            tName += GetTypeName(gArgs[i]);
                        }
                        else
                        {
                            tName += GetTypeName(gArgs[i]) + ",";
                        }
                    }
                    tName += ">";
                    return tName;
                }
            }
            return type.Name;
        }

        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<string, TypeInfo> InstanceCache = new Dictionary<string, TypeInfo>();
        /// <summary>
        /// 获取或添加实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static TypeInfo GetOrAddInstance(Type type , string methodName = "Add")
        {
            if( type.IsInterface)
            {
                throw new Exception("服务方法中不能包含接口内容！");
            }
            else if(type.IsClass)
            {
                var fullName = type.FullName + methodName;
                TypeInfo typeInfo = null;
                if(InstanceCache.ContainsKey(fullName))
                {
                    typeInfo = InstanceCache[fullName];
                }
                else
                {
                    Type[] argsTypes = null;
                    if (type.IsGenericType)
                    {
                        argsTypes = type.GetGenericArguments();
                        type = type.GetGenericTypeDefinition().MakeGenericType(argsTypes);
                    }
                    var mi = type.GetMethod(methodName);
                    typeInfo = new TypeInfo()
                    {
                        Type = type,
                        MethodInfo = mi,
                        ArgTypes = argsTypes
                    };
                    InstanceCache.Add(fullName,typeInfo);
                }
                typeInfo.Instance = Activator.CreateInstance(type);
                return typeInfo;
            }
            else if(type.IsValueType )
            {
                var fullName = type.FullName + methodName;
                TypeInfo typeInfo = null;
                if (InstanceCache.ContainsKey(fullName))
                {
                    typeInfo = InstanceCache[fullName];
                }
                else
                {
                    Type[] argsTypes = null;
                    if (type.IsGenericType)
                    {
                        argsTypes = type.GetGenericArguments();
                        type = type.GetGenericTypeDefinition().MakeGenericType(argsTypes);
                    }
                    var mi = type.GetMethod(methodName);
                    typeInfo = new TypeInfo()
                    {
                        Type = type,
                        MethodInfo = mi,
                        ArgTypes = argsTypes
                    };
                    InstanceCache.Add(fullName, typeInfo);
                }
                typeInfo.Instance = Activator.CreateInstance(type);
                return typeInfo;
            }
            return null;
        }
        public static TypeInfo GetOrAddInstance(Type type , MethodInfo mb )
        {
            lock(SyncRoot)
            {
                if(type.IsInterface)
                {
                    throw new Exception("服务方法中不能包含接口内容！");
                }
                var fullName = type.FullName + mb.Name;
                TypeInfo typeInfo = null;
                if (InstanceCache.ContainsKey(fullName))
                {
                    typeInfo = InstanceCache[fullName];
                }
                else
                {
                    typeInfo = new TypeInfo()
                    {
                        Type = type,
                        MethodInfo = mb
                    };
                    InstanceCache.Add(fullName, typeInfo);
                }
                typeInfo.Instance = Activator.CreateInstance(type);
                return typeInfo;
            }
        }
        public class TypeInfo
        {
            public Type Type { get; set; }
            public Object Instance { get; set; }
            public Type[] ArgTypes { get; set; }
            public MethodInfo MethodInfo { get; set; }
        }
    }
}
