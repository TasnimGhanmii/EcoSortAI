using Microsoft.ML.Data;
using System;

namespace sevrer.Api.ML
{
    public class WasteImageInput
    {
        [ColumnName("ImageBytes")]
        public byte[] ImageBytes { get; set; } = null!;
    }

    public class WastePredictionOutput
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedLabelKey { get; set; }

        [ColumnName("Score")]
        public float[] Score { get; set; } = null!;

        public float Confidence => Score?.Max() ?? 0f;
    }
}