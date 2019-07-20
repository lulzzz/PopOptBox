using Optimisation.Base.Variables;
using System.Linq;
using Xunit;

namespace Optimisation.Problems.SingleObjective.Continuous.Test
{
    public class RosenbrockTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 1.0 })]
        [InlineData(new[] { 1.0, 1.0, 0.0 })]
        [InlineData(new[] { 1.0, -1.0, 400.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Rosenbrock(2);
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, new[] { values[0], values[1] }));
            Assert.Equal(values[2], result.ElementAt(0));
        }
    }
}
