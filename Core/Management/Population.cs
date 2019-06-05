using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Management
{
    /// <summary>
    /// The Population contains and manages a number of solutions for an optimisation problem.
    /// <seealso cref="Individual"/>.
    /// </summary>
    public class Population : IReadOnlyCollection<Individual>
    {
        #region Fields

        private readonly List<Individual> members;
        
        /// <summary>
        /// Whether or not one should expect every individual to have the same length of Decision Space. 
        /// </summary>
        public readonly bool ConstantLengthDecisionVector;
        
        // TODO: Remove??
        private readonly int initialSize;

        /// <summary>
        /// Whether we have exceeded our desired size of population.
        /// TODO: Remove??
        /// </summary>
        public bool IsTargetSizeReached => members.Count >= initialSize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise population with a target size and an initial array of individuals
        /// </summary>
        /// <param name="initialPopulation">An array of individuals</param>
        /// <param name="initialSize">Expected max size of population</param>
        /// <param name="constantLengthDv">Whether the decision vector should be expected to be constant</param>
        public Population(
            int initialSize = 100,
            IEnumerable<Individual> initialPopulation = null,
            bool constantLengthDv = true)
        {
            if (initialSize > 0)
                this.initialSize = initialSize;

            ConstantLengthDecisionVector = constantLengthDv;

            members = new List<Individual>(this.initialSize);

            if (initialPopulation == null)
                return;

            foreach (var ind in initialPopulation) AddIndividual(ind);
        }

        /// <summary>
        /// Returns a shallow copy of the Population
        /// (member list is copied)
        /// </summary>
        public virtual Population Clone()
        {
            return new Population(
                initialSize,
                members.Select(m => m.Clone()),
                ConstantLengthDecisionVector);
        }

        #endregion

        #region FakeProperties

        /// <summary>
        /// An indexer to allow direct indexation to the Population 
        /// class, which will return the individual of interest 
        /// </summary>
        /// <param name="index">Integer index into population</param>
        /// <returns>Individual</returns>
        public Individual this[int index] => members[index];

        /*
        /// <summary>
        /// Allows read-only access to the individuals
        /// </summary>
        /// <returns>List of <see cref="Individual"/>s.</returns>
        public IReadOnlyList<Individual> GetMemberList()
        {
            return members;
        }
        */
        
        /// <summary>
        /// Gets the individual with the best (lowest) fitness.
        /// </summary>
        /// <returns>An evaluated <see cref="Individual"/> with the best fitness.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public Individual Best()
        {
            return members.Count > 0
                ? members[0]
                : throw new InvalidOperationException("Population has no members.");
        }

        /// <summary>
        /// Gets the individual with the worst (highest) fitness.
        /// </summary>
        /// <returns>An evaluated <see cref="Individual"/> with the worst fitness.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public Individual Worst()
        {
            return members.Count > 0
                ? members[members.Count - 1]
                : throw new InvalidOperationException("Population has no members.");
        }

        /// <summary>
        /// Get the Fitness values of all individuals
        /// </summary>
        /// <returns>List of doubles: Fitness</returns>
        public IEnumerable<double> GetMemberFitnesses()
        {
            return members.Select(i => i.Fitness);
        }

        /// <summary>
        /// Get the Scores of all individuals
        /// </summary>
        /// <returns>List of double arrays: Score</returns>
        public IEnumerable<double[]> GetMemberScores()
        {
            return members.Select(i => i.Score);
        }

        /// <summary>
        /// Get the Solution Vectors of all individuals
        /// </summary>
        /// <returns>List of double arrays: Solution Vector</returns>
        public IEnumerable<double[]> GetMemberSolutionVectors()
        {
            return members.Select(i => i.SolutionVector);
        }

        /// <summary>
        /// Get the DVs of all individuals
        /// </summary>
        /// <returns>List of TDecVec arrays: DV</returns>
        public IEnumerable<object[]> GetMemberDecisionVectors()
        {
            return members.Select(i => i.DecisionVector.Vector.ToArray());
        }

        #endregion

        #region Management

        /// <summary>
        /// Adds an <see cref="Individual"/> to the population
        /// </summary>
        /// <param name="ind">The individual to add.</param>
        /// <exception cref="ArgumentException">Thrown if: 
        /// 1) we are expecting individuals to have the same length Decision Vector and it is not true; 
        /// 2) The Individual is not yet evaluated.</exception>
        public void AddIndividual(Individual ind)
        {
            if (ConstantLengthDecisionVector && members.Count > 0)
                if (ind.DecisionVector.Vector.Count != members[0].DecisionVector.Vector.Count)
                    throw new ArgumentException(
                        "Decision Vector is not the right length!");
            
            if (ind.State != IndividualStates.Evaluated)
                throw new ArgumentException("Individual is not yet evaluated.");
            
            // Add to population
            members.Add(ind);

            Sort();
        }

        /// <summary>
        /// Sorts array by fitness, best (lowest!) at top.
        /// </summary>
        private void Sort()
        {
            members.Sort((p, q) => p.Fitness.CompareTo(q.Fitness));
        }
        
        /// <summary>
        /// Replaces worst individual with a new one
        /// </summary>
        /// <param name="ind">New individual to insert</param>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public void ReplaceWorst(Individual ind)
        {
            if (members.Count == 0)
                throw new InvalidOperationException("Population has no members.");
            
            members[members.Count - 1] = ind;
        }
        
        #endregion

        #region Implementation of IEnumerable

        /// <inheritdoc />
        public IEnumerator<Individual> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) members).GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out Individual<TDecVec>>

        /// <inheritdoc />
        public int Count => members.Count;

        #endregion
    }
}