using Microsoft.ML;
using System;
using System.IO;
using static sevrer.Api.ML.WastePredictionMapper;
using sevrer.Api.Enums;

namespace sevrer.Api.ML
{
    public class WastePredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<WasteImageInput, WastePredictionOutput> _predictionEngine;

        public WastePredictionService()
        {
            _mlContext = new MLContext(seed: 0);
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ML", "Model", "waste_classification_model.zip");

            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"ML model not found at: {modelPath}");

            _model = _mlContext.Model.Load(modelPath, out var schema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<WasteImageInput, WastePredictionOutput>(_model);
        }

        public (WasteCategory Category, float Confidence) ClassifyImage(string imagePath)
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            var input = new WasteImageInput { ImageBytes = imageBytes };
            var prediction = _predictionEngine.Predict(input);

            var category = MapKeyToCategory(prediction.PredictedLabelKey);
            return (category, prediction.Confidence);
        }
    }
}