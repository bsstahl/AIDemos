using System;
using System.Collections.Generic;
using System.Text;

namespace Amoeba.Common.Entities
{
    public class Solution : IComparable<Solution>
    {
        // a potential solution (array of double) and associated value (so can be sorted against several potential solutions
        public double[] vector;
        public double value;

        static readonly Random random = new Random(1);  // to allow creation of random solutions

        public Solution(int dim, double minX, double maxX, Func<double[], double> objectiveFunction)
        {
            // a random Solution
            this.vector = new double[dim];
            for (int i = 0; i < dim; ++i)
                this.vector[i] = (maxX - minX) * random.NextDouble() + minX;
            this.value = objectiveFunction.Invoke(this.vector);
        }

        public Solution(Func<double[], double> objectiveFunction, double[] vector)
        {
            // a specifiede solution
            this.vector = new double[vector.Length];
            Array.Copy(vector, this.vector, vector.Length);
            this.value = objectiveFunction.Invoke(this.vector);
        }

        public int CompareTo(Solution other) // based on vector/solution value
        {
            if (this.value < other.value)
                return -1;
            else if (this.value > other.value)
                return 1;
            else
                return 0;
        }

        public override string ToString()
        {
            string s = "[ ";
            for (int i = 0; i < this.vector.Length; ++i)
            {
                if (vector[i] >= 0.0) s += " ";
                s += vector[i].ToString("F2") + " ";
            }
            s += "]  val = " + this.value.ToString("F4");
            return s;
        }
    }

}
