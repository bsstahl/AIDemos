using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace Classifier.MLDotNet
{
    internal class AffirmativeData
    {
        [Column(ordinal: "0", name: "Label")]
        public float IsAffirmative { get; set;  }

        [Column(ordinal: "1")]
        public string UtteranceText { get; set; }
    }

}
