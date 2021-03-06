﻿using System;
using System.Globalization;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for any <see cref="DecisionVector"/> elements, which can swap a random pair.
    /// </summary>
    public class MutationRandomSwap : Operator, IMutationOperator
    {
        private readonly double mutationProbability;
        private readonly RandomNumberManager rngManager;

        /// <summary>
        /// Constructs a mutation operator that does nothing or swaps two elements in the <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="mutationProbability">The probability that the mutation will occur.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mutation probability is illegal.</exception>
        public MutationRandomSwap(double mutationProbability)
            : base($"Swap random pair with chance {mutationProbability.ToString("F2", CultureInfo.InvariantCulture)}")
        {
            if (mutationProbability < 0 || mutationProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationProbability), "Mutation probability must be a value between 0 and 1.");
            this.mutationProbability = mutationProbability;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector whose elements have potentially been mutated.
        /// </summary>
        /// <param name="decisionVector">The existing Decision Vector.</param>
        /// <returns>A new Decision Vector.</returns>
        /// <exception cref="ArgumentException">Thrown when the Decision Vector has less than 2 elements.</exception>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            if (decisionVector.Count < 2)
                throw new ArgumentException("Decision Vector must more than one element.",
                    nameof(decisionVector));

            if (rngManager.Rng.NextDouble() >= mutationProbability)
                return decisionVector;

            var locationsToSwap = rngManager.GetLocations(
                decisionVector.Count, 2, false, 1);

            // New vector - swap elements
            var vector = decisionVector.ToList();
            var firstItem = vector.ElementAt(locationsToSwap.ElementAt(0));
            var secondItem = vector.ElementAt(locationsToSwap.ElementAt(1));
            vector.RemoveAt(locationsToSwap.ElementAt(0));
            vector.Insert(locationsToSwap.ElementAt(0), secondItem);
            vector.RemoveAt(locationsToSwap.ElementAt(1));
            vector.Insert(locationsToSwap.ElementAt(1), firstItem);

            // Assumption, we are swapping the values of the items, not the actual dimensions.
            return DecisionVector.CreateFromArray(
                decisionVector.GetDecisionSpace(),
                vector);
        }
    }
}
