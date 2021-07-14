#region Uses

using System;
using System.Collections.Generic;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Casts;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.TypeInfoWriter;
using CodeRefractor.Util;
using static System.String;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    internal static class CppCastRelatedOperations
    {
        private static readonly string BoxingTemplate = @"
template<class T>
struct BoxedT : public System_Object
{
	T Data;
};

template<class T>
" + TypeNamerUtils.StdSharedPtr + @"<System_Object> box_value(T value, int typeId){
	auto result = std::make_shared<BoxedT< T > >();
	result->_typeId = typeId;
	result->Data = value;
	return result;
}

template<class T>
T unbox_value(" + TypeNamerUtils.StdSharedPtr + @"<System_Object> value){
	auto resultObject = value.get();
	auto castedUnboxing = (BoxedT<T>*)resultObject;
	return castedUnboxing->Data;
}";

        private static void HandleIsInstance(IsInstance operation, StringBuilder bodySb, ClosureEntities crRuntime)
        {
            LinkingData.Instance.IsInstTable.GenerateInstructionCode(operation, bodySb, crRuntime);
        }

        private static void HandleUnbox(Unboxing unboxing, StringBuilder bodySb, ClosureEntities closureEntities)
        {
            var typeDescription = unboxing.AssignedTo.ComputedType();
            bodySb
                .AppendFormat("{0} = unbox_value<{2}>({1});",
                    unboxing.AssignedTo.Name,
                    unboxing.Right.Name,
                    typeDescription.GetClrType(closureEntities).ToDeclaredVariableType(EscapingMode.Stack));
        }

        private static void HandleBox(Boxing boxing, StringBuilder bodySb, TypeDescriptionTable typeTable,
            ClosureEntities closureEntities)
        {
            var typeDescription = boxing.Right.ComputedType();
            bodySb
                .AppendFormat("{0} = box_value<{2}>({1}, {3});",
                    boxing.AssignedTo.Name,
                    boxing.Right.Name,
                    typeDescription.GetClrType(closureEntities).ToDeclaredVariableType(EscapingMode.Stack),
                    typeTable.GetTypeId(typeDescription.GetClrType(closureEntities)));
        }

        private static void HandleCastClass(ClassCasting casting, StringBuilder bodySb, ClosureEntities closureEntities)
        {
            var typeDescription = casting.AssignedTo.ComputedType();
            bodySb
                .AppendFormat("{0} = std::static_pointer_cast<{2}>({1});",
                    casting.AssignedTo.Name,
                    casting.Value.Name,
                    typeDescription.GetClrType(closureEntities).ToDeclaredVariableType(EscapingMode.Stack));
        }

        public static bool HandleCastRelatedOperations(TypeDescriptionTable typeTable, ClosureEntities crRuntime,
            LocalOperation operation, StringBuilder bodySb, OperationKind opKind)
        {
            switch (opKind)
            {
                case OperationKind.Box:
                    if (!Boxing.IsUsed)
                    {
                        Boxing.IsUsed = true;
                        bodySb.Append(BoxingTemplate);
                    }
                    HandleBox((Boxing)operation, bodySb, typeTable, crRuntime);
                    break;

                case OperationKind.CastClass:
                    HandleCastClass((ClassCasting)operation, bodySb, crRuntime);
                    break;

                case OperationKind.Unbox:
                    HandleUnbox((Unboxing)operation, bodySb, crRuntime);
                    break;

                case OperationKind.IsInstance:
                {
                    var declarations = new List<string>
                    {
                        "bool IsInstanceOf(int typeSource, int typeImplementation);",
                        "System_Void buildTypesTable();",
                        "std::map<int, std::vector<int> > GlobalMappingType;"
                    };

                    bodySb.Append(Join(Environment.NewLine, declarations));

                    HandleIsInstance((IsInstance) operation, bodySb, crRuntime);
                }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}