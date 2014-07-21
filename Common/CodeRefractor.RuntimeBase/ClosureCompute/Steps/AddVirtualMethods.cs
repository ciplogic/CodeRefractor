using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

namespace CodeRefractor.ClosureCompute.Steps
{
    class AddVirtualMethods : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var result = false;
          
            List<Type> usedTypes = closureEntities.MappedTypes.Values.ToList();
            var typeTable = new TypeDescriptionTable(usedTypes);
            closureEntities.VirtualMethodTable = new VirtualMethodTable(typeTable);
            var methodImps = closureEntities.MethodImplementations.ToList();
            foreach (var method in methodImps)
            {


                var subClasses = method.Key.DeclaringType.ImplementorsOfT(closureEntities.MappedTypes.Values);



                var methods = subClasses.SelectMany(s => s.GetMethods(ClosureEntitiesBuilder.AllFlags).Where(j => j.MethodMatches(method.Key))).ToArray();

                foreach (var methodImp in methods)
                {
                    var interpreter = closureEntities.ResolveMethod(methodImp) ?? new CilMethodInterpreter(methodImp);
                    closureEntities.VirtualMethodTable.RegisterMethod(methodImp, interpreter, closureEntities);
                }



            }

            return result;
        }
    }
}