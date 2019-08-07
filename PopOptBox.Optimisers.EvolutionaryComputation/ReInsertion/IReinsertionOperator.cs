﻿using PopOptBox.Base.Management;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// Interface for all re-insertion operators, that will be used to override <see cref="Optimiser.ReInsert(Individual)"/>.
    /// </summary>
    public interface IReinsertionOperator
    {
        /// <summary>
        /// Perform the re-insertion.
        /// </summary>
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <param name="individual">The <see cref="Individual"/> candidate for re-insertion.</param>
        /// <returns><see langword="true"/> if re-insertion has occurred.</returns>
        bool ReInsert(Population population, Individual individual);
    }
}