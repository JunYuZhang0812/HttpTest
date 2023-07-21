using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Common;
using System.Collections;
using System.Reflection;
#pragma warning disable 0219
public class ObjTool
{
	public static string ToString(object obj)
	{
		if (obj == null)
		{
			return "{null}";
		}
        if(obj is string)
        {
            return obj.ToString();
        }
		StringBuilder sb = new StringBuilder();
		string[] values = obj.ToString().Split('.');
		if (values.Length > 0)
		{
			sb.Append(values[values.Length - 1]);
		}
		sb.Append("{");
		Obj2Str(obj, sb);
		sb.Append("}");
		return sb.ToString();
	}

	private static void Obj2Str(object obj, StringBuilder sb)
	{
		if (obj == null)
		{
			sb.Append("null");
			return;
		}

		Type t = obj.GetType();
		if (t.IsValueType || t.IsInstanceOfType(typeof(IConvertible)))
		{
			sb.Append(obj.ToString());
			return;
		}
        if( t.IsGenericType )
        {
            Obj2Str2(obj, sb, obj);
            return;
        }
		var field = t.GetFields();

		foreach (var item in field)
		{
			sb.Append(item.Name);
			sb.Append(":");
			object value = item.GetValue(obj);
            Obj2Str2(value, sb , obj );
        }
		if (field.Length > 0)
		{
			sb.Remove(sb.Length - 1, 1);
		}
	}
	private static void Obj2Str2(object value, StringBuilder sb,object obj)
	{
        if (value == null)
        {
            sb.Append("null,");
            return;
        }
        Type t = obj.GetType();
        var itemType = value.GetType();
        if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(List<>))
        {
            sb.Append("{");
            Type listType = itemType.GetGenericArguments()[0];
            foreach (object listItem in (value as IEnumerable))
            {
                sb.Append("[");
                Obj2Str(listItem, sb);
                sb.Append("],");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
        }
        else if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            sb.Append("{");
            foreach (var iDictItem in (value as IDictionary).Keys)
            {
                sb.Append("[");
                sb.Append(iDictItem.ToString());
                sb.Append("=");
                Obj2Str((value as IDictionary)[iDictItem], sb);
                sb.Append("],");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
        }
        else if (itemType == typeof(Byte[]))
        {
            sb.Append('{');
            Byte[] bytes = value as Byte[];
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("X2"));
                }
            }
            sb.Append('}');
        }
        else if (itemType.IsValueType || t.IsInstanceOfType(typeof(IConvertible)) || itemType.Name.Equals("String"))
        {
            sb.Append(value.ToString());
        }
        else
        {
            Obj2Str(value, sb);
        }
        sb.Append(",");
    }
}
