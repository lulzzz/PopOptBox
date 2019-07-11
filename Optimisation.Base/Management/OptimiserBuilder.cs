using System;
using System.Collections.Generic;
using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Management
{
    /// <summary>
    /// Helper class for creating a full optimisation environment
    /// </summary>
    public abstract class OptimiserBuilder
    {
        private readonly Dictionary<IVariable, object> settings;

        protected OptimiserBuilder()
        {
            settings = new Dictionary<IVariable, object>();
        }

        public IEnumerable<IVariable> GetTunableSettings()
        {
            return settings.Keys;
        }

        /// <summary>
        /// Allows setting an optimisation hyperparameter
        /// </summary>
        /// <param name="definition">Setting definition in form of a <see cref="IVariable"/></param>
        /// <param name="value">The value for the setting</param>
        /// <returns><see langword="true" /> if set ok</returns>
        public bool SetSetting(IVariable definition, object value)
        {
            try
            {
                settings[definition] = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     The optimiser
        /// </summary>
        /// <returns></returns>
        public abstract Optimiser CreateOptimiser();

        /// <summary>
        ///     The model
        /// </summary>
        /// <returns></returns>
        public abstract IModel CreateModel();

        /// <summary>
        ///     The Decision Vector generator for population initialisation
        /// </summary>
        /// <returns></returns>
        protected Func<Individual> CreateDecisionVectorGenerator()
        {
            var model = CreateModel();
            return model.GetNewIndividual;
        }

        /// <summary>
        ///     The score to fitness converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreateScoreToFitness();
        
        /// <summary>
        ///     The solution vector to score converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double[]> CreateSolutionToScore();

        /// <summary>
        ///     The solution vector to penalty converter for illegal individuals
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreatePenalty();
    }
}