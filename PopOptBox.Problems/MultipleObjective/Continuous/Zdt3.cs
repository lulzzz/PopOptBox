using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Zdt3 : ProblemMultipleObjective
    {
        /// <summary>
        /// Creates an evaluator for the ZDT3 problem.
        /// 30D Decision space constrained on [0,1]
        /// Two objectives, with Pareto optimal values at [x1, 1 - sqrt(x1) - x1.sin(10.pi.x1)]
        /// </summary>
        public Zdt3() : base(
            "ZDT3", 
            DecisionSpace.CreateForUniformDoubleArray(30, 0,1,0,1),
            ContinuousProblemPropertyNames.TheLocation,
            ContinuousProblemPropertyNames.Result1, ContinuousProblemPropertyNames.Result2)
        {
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            // Zitzler, Deb, Thiele, "Comparison of Multiobjective Evolutionary Algorithms: Empirical Results", 2000 
            var numDims = definition.Count;

            var f1 = (double)definition.ElementAt(0);

            var g = 0.0;
            for (var i = 1; i < numDims; i++)
            {
                g += (double) definition.ElementAt(i) / (numDims - 1);
            }
            g *= 9;
            g += 1;

            var h = 1 - Math.Sqrt(f1 / g) - (f1 / g) * Math.Sin(10 * Math.PI * f1);

            var f2 = g * h;

            return new[] {f1, f2};
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            var xm = Enumerable.Repeat(0.0, 29);
            var x1 = Enumerable.Range(0, numberOfPoints).Select(
                i => new List<double> {(double) i / numberOfPoints});
            
            var pf = new List<DecisionVector>();
            foreach (var f1 in x1)
            {
                f1.AddRange(xm.ToArray());
                pf.Add(DecisionVector.CreateFromArray(decisionSpace,
                    f1));
            }

            return pf.ToArray();
        }
    }
}