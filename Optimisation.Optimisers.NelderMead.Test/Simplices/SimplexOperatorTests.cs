using System.Linq;
using Optimisation.Base.Variables;
using Optimisation.Optimisers.NelderMead.Test;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Simplices.Test
{
    public class SimplexOperatorTests
    {
        private readonly OperatorMock operatorMock;

        public SimplexOperatorTests()
        {
            operatorMock = new OperatorMock(1);
        }

        [Theory]
        [InlineData(new[] { 0.5, 0.5 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void GetMean_TwoDim_ReturnsCorrectValue(double[] expectedAnswer, params double[][] testValues)
        {
            var inds = Helpers.CreateEvaluatedIndividualsFromArray(testValues);
            var simplex = new Simplex(inds);
            
            // Operate() is set up to call GetMean() function.
            var meanLocation = operatorMock.Operate(simplex);

            Assert.True(meanLocation.Vector.Count == expectedAnswer.Length);
            Assert.Equal(expectedAnswer, meanLocation.Vector.Select(d => (double)d).ToArray());
        }

        public class OperatorMock : SimplexOperator
        {
            public OperatorMock(double coefficient) : base(coefficient)
            {
            }

            public override DecisionVector Operate(Simplex simplex)
            {
                return GetMean(simplex);
            }

            protected override bool CheckCoefficientAcceptable(double value)
            {
                return true;
            }
        }
    }
}
