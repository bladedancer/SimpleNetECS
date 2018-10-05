using Components;
using System;
using System.Collections.Generic;

namespace Neural
{
    public class Options : Dictionary<string, object> { };
    public delegate List<NetData> MutatorFunc(NetData[] nets, Options options = null);

    public class Mutators
    {
        private static Random random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Direct clones.
        /// No options
        /// </summary>
        public static MutatorFunc Clone = (nets, options) =>
        {
            if (nets.Length == 0)
            {
                return null;
            }
            NetData child = nets[0].Clone();
            return new List<NetData>() { child };
        };

        /// <summary>
        /// Interleaves the weights from the nets, round robins.
        /// No options
        /// </summary>
        public static MutatorFunc InterLeaved = (nets, options) =>
        {
            if (nets.Length == 0)
            {
                return null;
            }
            NetData child = nets[0].Clone();

            if (nets.Length >= 2)
            {
                for (int i = 1; i < nets[0].Weights.Length; i += nets.Length)
                {
                    for (int j = 0; j < nets.Length - 1; ++j)
                    {
                        child.Weights[i + j] = nets[j + 1].Weights[i + j];
                    }
                }
            }
            return new List<NetData>() { child };
        };

        /// <summary>
        /// Takes alternate layers from the nets.
        /// No options
        /// </summary>
/*
        public static MutatorFunc LayerCake = (nets, options) =>
        {
            if (nets.Length == 0)
            {
                return null;
            }
            NetData child = nets[0].Clone();

            if (!(child is FeedForward))
            {
                throw new Exception("LayerCake only works for FeedForward");
            }

            FeedForward ffchild = child as FeedForward;

            if (nets.Length >= 2)
            {
                int offset = 0;
                int selected = 0;
                for (int i = 1; i < ffchild.layerSizes.Length; ++i)
                {
                    int weightCount = ffchild.layerSizes[i] * (ffchild.layerSizes[i - 1] + 1 );
                    Array.Copy(nets[selected].Weights, offset, child.Weights, offset, weightCount);
                    selected = (selected + 1) % nets.Length;
                    offset += weightCount;
                }
            }
            return new List<Net>() { child };
        };
*/
        /// <summary>
        /// Takes a single NetData and mutates it.
        /// Options:
        /// cloneNet: If true a copy is made and it is mutated.
        /// mutationProbability: 0-1
        /// mutationFactor: n (multiplier)
        /// mutationRange: +-n (mutation if weight is 0) 
        /// </summary>
        public static MutatorFunc SelfMutate = (nets, options) =>
        {
            double mutationProbability = 0;
            if (options.ContainsKey("mutationProbability"))
            {
                mutationProbability = Convert.ToDouble(options["mutationProbability"]);
            }

            double mutationFactor = 0;
            if (options.ContainsKey("mutationFactor"))
            {
                mutationFactor = Convert.ToDouble(options["mutationFactor"]);
            }

            double mutationRange = 0;
            if (options.ContainsKey("mutationRange"))
            {
                mutationRange = Convert.ToDouble(options["mutationRange"]);
            }

            bool clone = false;
            if (options.ContainsKey("clone"))
            {
                clone = Convert.ToBoolean(options["clone"]);
            }

            NetData mutant = nets[0];

            if (clone)
            {
                mutant = mutant.Clone();
            }

            for (int i = 0; i < mutant.Weights.Length; ++i)
            {
                if (random.NextDouble() < mutationProbability)
                {
                    if (Math.Abs(mutant.Weights[i]) < 0.000000001)
                    {
                        mutant.Weights[i] = ((random.NextDouble() * 2) - 1) * mutationRange;
                    }
                    else
                    {
                        double multiplier = ((random.NextDouble() * 2) - 1) * mutationFactor;
                        mutant.Weights[i] = mutant.Weights[i] + (mutant.Weights[i] * multiplier);
                    }
                }
            }
            return new List<NetData>() { mutant };
        };

        /// <summary>
        /// Randomly mixes the nets.
        /// </summary>
        public static MutatorFunc RandomMix = (nets, options) =>
        {
            bool clone = false;
            if (options != null && options.ContainsKey("clone"))
            {
                clone = Convert.ToBoolean(options["clone"]);
            }

            NetData mutant = nets[0];

            if (clone)
            {
                mutant = mutant.Clone();
            }

            if (nets.Length > 1)
            {
                for (int i = 0; i < mutant.Weights.Length; ++i)
                {
                    int n = (int)Math.Floor(random.NextDouble() * nets.Length);
                    mutant.Weights[i] = nets[n].Weights[i];
                }
            }
            return new List<NetData>() { mutant };
        };

        /// <summary>
        /// Averages the nets.
        /// </summary>
        public static MutatorFunc Average = (nets, options) =>
        {
            NetData mutant = nets[0].Clone();

            if (nets.Length > 1)
            {
                for (int i = 0; i < mutant.Weights.Length; ++i)
                {
                    mutant.Weights[i] /= nets.Length;
                    for (int j = 1; j < nets.Length; ++j)
                    {
                        mutant.Weights[i] += (nets[j].Weights[i] / nets.Length);
                    }
                }
            }
            return new List<NetData>() { mutant };
        };
    }
}
