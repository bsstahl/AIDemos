using System;
using System.Collections.Generic;
using System.Text;

namespace AffirmativeClassifier.Trainer
{
    internal class ModelInput: Utterance
    {
        public IEnumerable<String> ParsedUtterance { get; set; }

        public UInt64 TokenizedUtterance { get; set; }
    }
}
