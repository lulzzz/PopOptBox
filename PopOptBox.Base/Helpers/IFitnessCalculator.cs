using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Helpers
{
    public interface IFitnessCalculator
    {
        /// <summary>
        /// Performs the fitness calculation using the contract specified by the constructor of an <see cref="Optimiser"/>.
        /// </summary>
        /// <param name="individualsOfInterest">The individuals which needs their fitness assigned.</param>
        /// <param name="restOfPopulation">The other individuals which should be taken into consideration when assigning fitness.</param>
        void CalculateAndAssignFitness(
            IEnumerable<Individual> individualsOfInterest,
            IEnumerable<Individual> restOfPopulation);
    }
}