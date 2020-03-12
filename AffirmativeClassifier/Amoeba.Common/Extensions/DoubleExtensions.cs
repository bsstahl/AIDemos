using System;
using System.Collections.Generic;
using System.Text;

namespace Amoeba.Common.Extensions
{
    public static class DoubleExtensions
    {
        public static int Scale(this double value, double scale)
        {
            return Convert.ToInt32(Math.Round(value * scale));
        }
    }
}
