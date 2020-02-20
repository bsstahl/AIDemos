using Amoeba.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Amoeba.Common.Entities
{
    public class Organism
    {

        public int Size { get; set; }  // number of solutions
        public Solution[] Solutions { get; set; } // potential solutions (vector + value)

        private readonly SimulationConfiguration _config;
        private readonly Func<double[], double> _objectiveFunction;

        public Organism(SimulationConfiguration config, Func<double[], double> objectiveFunction)
        {
            _config = config;
            _objectiveFunction = objectiveFunction;

            this.Size = config.AmoebaSize;
            this.Solutions = new Solution[config.AmoebaSize];

            for (int i = 0; i < this.Solutions.Length; ++i)
                this.Solutions[i] = new Solution(config.Dimensions, config.MinX, config.MaxX, _objectiveFunction);  // the Solution ctor calls the objective function to compute value

            Array.Sort(this.Solutions);
        }

        public Solution Centroid(Func<double[], double> objectiveFunction)
        {
            // return the centroid of all solution vectors except for the worst (highest index) vector
            double[] c = new double[_config.Dimensions];
            for (int i = 0; i < this.Size - 1; ++i)
                for (int j = 0; j < _config.Dimensions; ++j)
                    c[j] += this.Solutions[i].vector[j];  // accumulate sum of each vector component

            for (int j = 0; j < _config.Dimensions; ++j)
                c[j] = c[j] / (this.Size - 1);

            Solution s = new Solution(objectiveFunction, c);  // feed vector to ctor which calls objective function to compute value
            return s;
        }

        public Solution Reflected(Func<double[], double> objectiveFunction, Solution centroid)
        {
            // the reflected solution extends from the worst (lowest index) solution through the centroid
            double[] r = new double[_config.Dimensions];
            double[] worst = this.Solutions[this.Size - 1].vector;  // convenience only
            for (int j = 0; j < _config.Dimensions; ++j)
                r[j] = ((1 + _config.Alpha) * centroid.vector[j]) - (_config.Alpha * worst[j]);
            Solution s = new Solution(objectiveFunction, r);
            return s;
        }

        public Solution Expanded(Func<double[], double> objectiveFunction, Solution reflected, Solution centroid)
        {
            // expanded extends even more, from centroid, thru reflected
            double[] e = new double[_config.Dimensions];
            for (int j = 0; j < _config.Dimensions; ++j)
                e[j] = (_config.Gamma * reflected.vector[j]) + ((1 - _config.Gamma) * centroid.vector[j]);
            Solution s = new Solution(objectiveFunction, e);
            return s;
        }

        public Solution Contracted(Func<double[], double> objectiveFunction, Solution centroid)
        {
            // contracted extends from worst (lowest index) towards centoid, but not past centroid
            double[] v = new double[_config.Dimensions];  // didn't want to reuse 'c' from centoid routine
            double[] worst = this.Solutions[this.Size - 1].vector;  // convenience only
            for (int j = 0; j < _config.Dimensions; ++j)
                v[j] = (_config.Beta * worst[j]) + ((1 - _config.Beta) * centroid.vector[j]);
            Solution s = new Solution(objectiveFunction, v);
            return s;
        }

        public void Shrink(Func<double[], double> objectiveFunction)
        {
            // move all vectors, except for the best vector (at index 0), halfway to the best vector
            // compute new objective function values and sort result
            for (int i = 1; i < this.Size; ++i)  // note we don't start at [0]
            {
                for (int j = 0; j < _config.Dimensions; ++j)
                {
                    this.Solutions[i].vector[j] = (this.Solutions[i].vector[j] + this.Solutions[0].vector[j]) / 2.0;
                    this.Solutions[i].value = objectiveFunction.Invoke(this.Solutions[i].vector);
                }
            }
            Array.Sort(this.Solutions);
        }

        public void ReplaceWorst(Solution newSolution)
        {
            // replace the worst solution (at index size-1) with contents of parameter newSolution's vector
            for (int j = 0; j < _config.Dimensions; ++j)
                this.Solutions[this.Size - 1].vector[j] = newSolution.vector[j];
            this.Solutions[this.Size - 1].value = newSolution.value;
            Array.Sort(this.Solutions);
        }

        public bool IsWorseThanAllButWorst(Solution reflected)
        {
            // Solve needs to know if the reflected vector is worse (greater value) than every vector in the amoeba, except for the worst vector (highest index)
            for (int i = 0; i < this.Size - 1; ++i)  // not the highest index (worst)
            {
                if (reflected.value <= this.Solutions[i].value)  // reflected is better (smaller value) than at least one of the non-worst solution vectors
                    return false;
            }
            return true;
        }


        public Solution Solve() => this.Solve(false);

        public Solution Solve(bool logToConsole)
        {
            int t = 0;  // loop counter
            while (t < _config.MaxEpochs)
            {
                ++t;

                if (logToConsole)
                    Console.WriteLine($"At t = {t:00000} curr best solution = {_objectiveFunction.Invoke(this.Solutions[0].vector).ToString("000.0000000000000")}");

                if (!string.IsNullOrEmpty(_config.OutputFolder))
                {
                    string filePath = System.IO.Path.Combine(_config.OutputFolder, $"Amoeba_{t.ToString("000000")}.png");
                    this.WriteImage(filePath, this.Solutions[0]);
                }
                
                Solution centroid = this.Centroid(_objectiveFunction);  // compute centroid
                Solution reflected = this.Reflected(_objectiveFunction, centroid);  // compute reflected

                if (reflected.value < this.Solutions[0].value)  // reflected is better than the curr best
                {
                    Solution expanded = this.Expanded(_objectiveFunction, reflected, centroid);  // can we do even better??
                    if (expanded.value < this.Solutions[0].value)  // winner! expanded is better than curr best
                        this.ReplaceWorst(expanded);  // replace curr worst solution with expanded
                    else
                        this.ReplaceWorst(reflected);  // it was worth a try . . . 
                }
                else if (this.IsWorseThanAllButWorst(reflected) == true)  // reflected is worse (larger value) than all solution vectors (except possibly the worst one)
                {
                    if (reflected.value <= this.Solutions[this.Size - 1].value)  // reflected is better (smaller) than the curr worst (last index) vector
                        this.ReplaceWorst(reflected);

                    Solution contracted = this.Contracted(_objectiveFunction, centroid);  // compute a point 'inside' the amoeba

                    if (contracted.value > this.Solutions[this.Size - 1].value)  // contracted is worse (larger value) than curr worst (last index) solution vector
                        this.Shrink(_objectiveFunction);
                    else
                        this.ReplaceWorst(contracted);
                }
                else
                {
                    this.ReplaceWorst(reflected);
                }
            }

            return this.Solutions[0];  // best solution is always at [0]
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < this.Solutions.Length; ++i)
                s += "[" + i + "] " + this.Solutions[i].ToString() + Environment.NewLine;
            return s;
        }

        public void WriteImage(string filePath, Solution bestSolution)
        {
            int imageWidth = 800;
            int imageHeight = 600;

            double rangeX = _config.MaxX - _config.MinX;

            Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);
            var graphics = Graphics.FromImage(image);

            var blackPen = new Pen(Color.Black, 1);
            var redPen = new Pen(Color.Red, 5);
            var bluePen = new Pen(Color.LightBlue, 1);
            var whitePen = new Pen(Color.White, 1);

            double xScale = imageWidth / rangeX;
            double yScale = imageHeight / rangeX;

            int xQuarterScale = (int)Math.Round(xScale / 4.0);
            int yQuarterScale = (int)Math.Round(yScale / 4.0);

            graphics.DrawRectangle(new Pen(Brushes.White, imageWidth), 0, 0, imageWidth, imageHeight);

            for (int x = 0; x <= imageWidth; x += xQuarterScale)
            {
                graphics.DrawLine(bluePen, x, 0, x, imageHeight);
            }

            for (int y = 0; y <= imageHeight; y += yQuarterScale)
            {
                graphics.DrawLine(bluePen, 0, y, imageWidth, y);
            }

            // Draw all solutions


            double? lastX = null, lastY = null;
            foreach (var solution in this.Solutions)
            {
                Brush brush = Brushes.Black;
                if (solution.value == bestSolution.value)
                    brush = Brushes.Red;
                var pen = new Pen(brush);

                int x = solution.vector[0].Scale(xScale) - 2;
                int y = solution.vector[1].Scale(yScale) - 2;
                graphics.DrawEllipse(pen, x, y, 5, 5);

                if (lastX.HasValue)
                    graphics.DrawLine(new Pen(Brushes.Black), lastX.Value.Scale(xScale), lastY.Value.Scale(yScale), solution.vector[0].Scale(xScale), solution.vector[1].Scale(yScale));
                lastX = solution.vector[0];
                lastY = solution.vector[1];
            }
            graphics.DrawLine(new Pen(Brushes.Black), lastX.Value.Scale(xScale), lastY.Value.Scale(yScale), this.Solutions[0].vector[0].Scale(xScale), this.Solutions[0].vector[1].Scale(yScale));

            image.Save(filePath, ImageFormat.Png);
        }

    }

}
