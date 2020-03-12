using System;
using System.Collections.Generic;
using System.Linq;

namespace AffirmativeClassifier.Trainer
{
    public class UtteranceRepository : IUtteranceRepository
    {
        private readonly String _textFilePath = string.Empty;

        public UtteranceRepository(String textFilePath)
        {
            _textFilePath = textFilePath;
        }

        public IEnumerable<Utterance> GetAllUtterances()
        {
            return System.IO.File
                .ReadAllLines(_textFilePath)
                .Select(l => l.ToUtterance())
                .Where(u => !(u is null));
        }

    }
}
