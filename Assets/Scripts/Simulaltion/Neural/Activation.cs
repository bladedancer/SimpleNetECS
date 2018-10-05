using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural
{
    /// <summary>
    /// Activation functions.
    /// </summary>
    public class Activation
    {
        public delegate double func(double i);

        /// <summary>
        /// The sigmoid activation function with values from 0 to 1.
        /// </summary>
        /// <param name="v">The value to convert.</param>
        /// <returns>The resulting value.</returns>
        public static double sigmoid(double v)
        {
            return (1 / (1 + Math.Pow(Math.E, -v)));
        }

        /// <summary>
        /// The tanh activation function with values from -1 to 1.
        /// </summary>
        /// <param name="v">The value to convert.</param>
        /// <returns>The resulting value.</returns>
        public static double tanh(double v)
        {
            return Math.Tanh(v);
        }
    }
}
