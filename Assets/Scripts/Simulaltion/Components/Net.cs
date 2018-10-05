using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct NetData
    {
        /// <summary>
        /// The array of layer sizes. The first layer is the input and the last layer is the output.
        /// </summary>
        public int[] LayerSizes;

        /// <summary>
        /// The weights of the neural network;
        /// </summary>
        public double[] Weights;
    }

    public class Net : MonoBehaviour
    {
        public NetData Data;
    }
}
