using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.FitnessCalculation;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// The Optimiser handles the logic for deciding which individuals to try.
    /// As a meta-heuristic, it only knows about the Decision Vector and the Fitness of each Individual.
    /// </summary>
    public abstract class Optimiser : IOptimiser
    {
        protected readonly IFitnessCalculator fitnessCalculator;

        #region Constructor

        /// <summary>
        /// Constructs the optimiser.
        /// </summary>
        /// <param name="initialPopulation">An initial population (can be empty).</param>
        /// <param name="fitnessCalculator">Conversion function to assign Fitness to an Individual. <seealso cref="Individual.SetFitness"/></param>
        protected Optimiser(Population initialPopulation, 
            IFitnessCalculator fitnessCalculator)
        {
            Population = initialPopulation;
            this.fitnessCalculator = fitnessCalculator;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The current population in the optimiser
        /// </summary>
        public Population Population { get; }

        #endregion

        #region Activity

        /// <summary>
        ///     Optimiser-specific logic to implement, which works out what to try next.
        /// </summary>
        /// <returns>
        ///     The most useful next Decision Vector to evaluate. 
        ///     Return <see cref="DecisionVector.CreateForEmpty"/> to terminate <see cref="Runtime.OptimiserRunnerBasic"/>.
        /// </returns>
        protected abstract DecisionVector GetNewDecisionVector();

        /// <summary>
        /// Gets (up to) a certain number of individuals for evaluation.
        /// </summary>
        /// <param name="numDesired">Number of individuals wished for.</param>
        /// <returns>List of <see cref="Individual"/> objects.</returns>
        /// <exception cref="TimeoutException">Thrown if not enough individuals can be created.</exception>
        public IReadOnlyList<Individual> GetNextToEvaluate(int numDesired)
        {
            var listOfInds = new List<Individual>();
            var i = 0;
            while (listOfInds.Count < numDesired)
            {
                try
                {
                    var newDv = GetNewDecisionVector();
                    var newInd = new Individual(newDv);
                    newInd.SetProperty(OptimiserPropertyNames.CreationTime, DateTime.Now);
                    listOfInds.Add(newInd);
                }
                catch { }

                i++;

                if (i > numDesired + 20)
                    //Allow max 20 retries
                    throw new TimeoutException("Could not create enough acceptable new individuals");
            }

            return listOfInds;
        }

        /// <summary>
        /// Try to reinsert Individuals back into the Population.
        /// </summary>
        /// <remarks>A basic implementation to override.</remarks>
        /// <param name="individuals">The individuals to try to reinsert.</param>
        /// <returns>The number of individuals actually inserted.</returns>
        /// <exception cref="InvalidOperationException">Propagates error from <see cref="Population"/> insertion.</exception>
        protected virtual int AssessFitnessAndDecideFate(IEnumerable<Individual> individuals)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            fitnessCalculator.CalculateAndAssignFitness(inds);
            
            var numberReinserted = 0;
            foreach (var ind in inds)
            {
                try
                {
                    Population.AddIndividual(ind);
                    numberReinserted++;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Error occurred while re-inserting individuals", e);
                }
            }
            return numberReinserted;
        }

        /// <summary>
        /// Reinserts individuals, sets their fitness based on the class provided in the constructor.
        /// <seealso cref="AssessFitnessAndDecideFate(IEnumerable{Individual})"/>
        /// </summary>
        /// <param name="individuals">List of <see cref="Individual"/>s to reinsert.</param>
        /// <returns>The number of individuals successfully reinserted.</returns>
        /// <exception cref="InvalidOperationException">Thrown when any individual is incorrect.</exception>
        public int ReInsert(IEnumerable<Individual> individuals)
        {
            var individualsToReinsert = new List<Individual>();
            
            foreach (var ind in individuals)
            {
                if (ind.State != IndividualState.Evaluated)
                    throw new InvalidOperationException("Individual is not evaluated!");

                if (ind.GetPropertyNames().Contains(OptimiserPropertyNames.EvaluationError))
                    throw new InvalidOperationException(
                        "Individual evaluation failed, arresting optimisation.",
                        ind.GetProperty<Exception>(OptimiserPropertyNames.EvaluationError));

                ind.SetProperty(
                    OptimiserPropertyNames.ReinsertionTime,
                    DateTime.Now);
                
                if (Population.Contains(ind)) 
                {
                    ind.SetProperty(
                        OptimiserPropertyNames.ReinsertionError,
                        new InvalidOperationException("Individual already exists in Population."));
                    continue;
                }
                
                individualsToReinsert.Add(ind);
            }

            if (individualsToReinsert.Count == 0)
                return 0;

            return AssessFitnessAndDecideFate(individualsToReinsert);
        }

        #endregion
    }
}