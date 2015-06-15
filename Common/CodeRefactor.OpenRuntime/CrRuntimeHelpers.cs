using System;
using System.Runtime.CompilerServices;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (RuntimeHelpers))]
    public class CrRuntimeHelpers
    {
        [CppMethodBody(
            Code = @"
	auto arrayDestRef = array.get();
	memcpy(arrayDestRef->Items, data, bytesCount);"
            )]
        public static unsafe void InitializeArray(Array array, byte* data, int bytesCount)
        {
        }
    }
}