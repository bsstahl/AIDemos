using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Classifier.MLDotNet
{
    internal static class MLContextExtensions
    {
        public static Microsoft.Data.DataView.IDataView LoadData(this MLContext mlContext, string dataPath)
        {
            var textLoader = mlContext.Data.CreateTextLoader(
                columns: new TextLoader.Column[]
                {
                    new TextLoader.Column("Label", DataKind.Bool,0),
                    new TextLoader.Column("UtteranceText", DataKind.Text,1)
                },
                separatorChar: ',',
                hasHeader: true);

            return textLoader.Read(dataPath);
        }
    }
}
