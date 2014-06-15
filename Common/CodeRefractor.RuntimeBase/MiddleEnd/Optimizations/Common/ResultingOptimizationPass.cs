#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Common
{
    public abstract class ResultingOptimizationPass
    {
        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true

        private bool _result;

        protected ResultingOptimizationPass(OptimizationKind kind)
        {
            Kind = kind;
        }

        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public abstract void OptimizeOperations(MethodInterpreter interpreter);

    


        public OptimizationKind Kind { get; set; }

        public virtual bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            return true;
        }
         //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true
        public bool Optimize(MethodInterpreter intermediateCode)
        {
            _result = false;
            try
            {
                OptimizeOperations(intermediateCode);
            }
            catch
            {
            }
            return _result;
        }
    }
}