using Components;
using Neural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(SensorSystem))]
    [UpdateBefore(typeof(MovementSystem))]
    class NetMovementSystem : ComponentSystem
    {
        private struct Filter
        {
            public Net Net;
            public SensorData SensorData;
            public MotionInput MotionInput;
        }

        /// <summary>
        /// The bias value.
        /// </summary>
        private float bias = 1;

        /// <summary>
        /// The activation function of the network. Defaults to Activcation.sigmoid.
        /// </summary>
        private Activation.func activationFunc = Activation.tanh;

        protected override void OnUpdate()
        {
            foreach (Filter entity in GetEntities<Filter>())
            {
                double[] output = evalNet(entity.SensorData.Data, entity.Net.Data.LayerSizes, entity.Net.Data.Weights);
                entity.MotionInput.Horizontal = (float) output[0];
                entity.MotionInput.Vertical = (float) (output[1] + 1.0f) / 2.0f;
            }
        }

        // Evaluate the Network with the specified inputs.
        private double[] evalNet(double[] inputs, int[] layerSizes, double[] weights) {
            if (inputs.Length != layerSizes[0])
            {
                throw new Exception("Invalid input count. Expected " + layerSizes[0] + " but got " + inputs.Length);
            }

            List<double> layerVals = new List<double>();
            layerVals.AddRange(inputs);

            // TODO GET A BETTER WAY OF DOING THIS, LOOK AT THE MATRIX
            int w = 0;
            for (int layer = 1; layer < layerSizes.Length; ++layer)
            {
                layerVals.Add(bias);
                double[] nextVals = new double[layerSizes[layer]];

                // All the weights by all the previous layer weights plus bias.
                for (int node = 0; node < layerVals.Count; ++node)
                {
                    for (int resultNode = 0; resultNode < layerSizes[layer]; ++resultNode)
                    {
                        nextVals[resultNode] = nextVals[resultNode] + (layerVals[node] * weights[w++]);
                    }
                }

                for (int i = 0; i < nextVals.Length; ++i)
                {
                    nextVals[i] = activationFunc(nextVals[i]);
                }

                // Update the inputs
                layerVals.Clear();
                layerVals.AddRange(nextVals);

            }

            return layerVals.ToArray();
        }
    }
}
