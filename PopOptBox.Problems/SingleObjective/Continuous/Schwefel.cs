﻿using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Continuous
{
    public class Schwefel : ProblemSingleObjectiveContinuous
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Schwefel Function.
        /// Constrained on [-500, 500]
        /// Global optimum is at [420.9687, 420.9687, 420.9687,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Schwefel(int numDims) : base(
            "Schwefel Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, 
                    -500.0, 500.0,
                    -250, 475),
                Enumerable.Repeat(420.9687, numDims).ToArray()))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://www.sfu.ca/~ssurjano/schwef.html
            // http://benchmarkfcns.xyz/benchmarkfcns/schwefelfcn.html

            var result = 418.9829 * location.Count;
            foreach (var d in location)
            {
                var t = Convert.ToDouble(d);
                result -= t * Math.Sin(Math.Sqrt(Math.Abs(t)));
            }
            return new[] { result };
        }

        #endregion
    }
}
