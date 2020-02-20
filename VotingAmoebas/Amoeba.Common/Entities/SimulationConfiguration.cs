using System;
using System.Collections.Generic;
using System.Text;

namespace Amoeba.Common.Entities
{
    public class SimulationConfiguration
    {
        public int Dimensions { get; set; } // problem dimension (number of variables to solve for)
        public int AmoebaSize { get; set; } // # of potential solutions in the organism

        public double MinX { get; set; }
        public double MaxX { get; set; }

        public int MaxEpochs { get; set; }
        
        public double Alpha { get; set; }
        public double Beta { get; set; }
        public double Gamma { get; set; }

        public string OutputFolder { get; set; }

    }
}
