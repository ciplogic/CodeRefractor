using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.Linker
{
	public static class NamerUtils{
		public static string ValidName(this string name)
		{
			return FieldNameTable.Instance.GetFieldName (name);
		}
	}
    
}