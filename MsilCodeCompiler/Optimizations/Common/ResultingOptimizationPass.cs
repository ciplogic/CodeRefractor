#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.Common
{
    public abstract class ResultingOptimizationPass : OptimizationPass
    {
        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true
        public abstract void OptimizeOperations(MetaMidRepresentation intermediateCode);

        private bool _result;

        public override bool Optimize(MetaMidRepresentation intermediateCode)
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

        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}