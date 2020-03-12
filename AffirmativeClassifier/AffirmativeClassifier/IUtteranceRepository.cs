using System;
using System.Collections.Generic;
using System.Text;

namespace AffirmativeClassifier
{
    public interface IUtteranceRepository
    {
        IEnumerable<Utterance> GetAllUtterances();
    }
}
