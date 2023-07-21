using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class FuncExtend
{
	#region StringBuilder
	public static void Clear(this StringBuilder sb)
	{
		sb.Length = 0;
	}

	public static void AppendLineEx(this StringBuilder sb, string str = "")
	{
		sb.Append(str + "\r\n");
	}
	#endregion
}

