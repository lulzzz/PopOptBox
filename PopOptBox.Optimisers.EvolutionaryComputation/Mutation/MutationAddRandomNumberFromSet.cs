﻿using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for <see cref="VariableContinuous"/> <see cref="DecisionVector"/> elements, which can add a random number chosen from a set provided.
    /// </summary>
    public class MutationAddRandomNumberFromSet : Operator, IMutationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly double[] numberSet;
        private readonly double mutationProbability;
        private readonly int maximumNumberOfMutations;

        /// <summary>
        /// Constructs a mutation operator that adds a number, chosen randomly from a set of numbers provided, to zero or more elements in the <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="numberSet">The set of numbers to choose from.</param>
        /// <param name="mutationProbability">The probability that any mutation will occur.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of times a mutation should be tried.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the input values are illegal.</exception>
        public MutationAddRandomNumberFromSet(IEnumerable<double> numberSet, double mutationProbability, int maximumNumberOfMutations) 
            : base($"Add random number from set of size {numberSet.Count()}")
        {
            var numbers = numberSet.ToArray();

            if (mutationProbability < 0 || mutationProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationProbability),
                    "Mutation probability must be a value between 0 and 1.");
            this.mutationProbability = mutationProbability;

            if (maximumNumberOfMutations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations),
                    "Maximum number of mutations must be greater than 0.");
            this.maximumNumberOfMutations = maximumNumberOfMutations;

            this.numberSet = numbers;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector where continuous elements have potentially been mutated.
        /// Uses <see cref="IVariable"/> to wrap the added number, ensuring a valid Decision Vector is always created.
        /// </summary>
        /// <param name="decisionVector">The existing Decision Vector.</param>
        /// <returns>A new Decision Vector.</returns>
        /// <exception cref="ArgumentException">Thrown when Decision Vector is zero length or has no <seealso cref="VariableContinuous"/> elements.</exception>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            var oldVectorContinuousElements = decisionVector.GetContinuousElements();

            if (oldVectorContinuousElements.Count == 0)
                throw new ArgumentException("Decision Vector must have continuous elements",
                    nameof(decisionVector));

            var locationsToMutate = rngManager.GetLocations(
                oldVectorContinuousElements.Count, maximumNumberOfMutations,
                true, mutationProbability);

            var newDv = new object[decisionVector.Count];
            var newDs = decisionVector.GetDecisionSpace();
            var offset = 0;
            for (var i = 0; i < decisionVector.Count; i++)
            {
                newDv[i] = decisionVector.ElementAt(i);

                // If variable is not continuous, just copy
                if (newDs.ElementAt(i).GetType() != typeof(VariableContinuous))
                {
                    offset++;
                    continue;
                }

                // Variable is continuous - it may be mutated multiple times.
                var numTimesToMutate = locationsToMutate.Count(l => l == (i - offset));

                for (var j = 0; j < numTimesToMutate; j++)
                {
                    var mutationLocation = rngManager.Rng.Next(0, numberSet.Length);
                    newDv[i] = newDs.ElementAt(i).AddOrWrap(newDv[i], numberSet[mutationLocation]);
                }
            }

            return DecisionVector.CreateFromArray(newDs, newDv);
        }
    }
}