using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace sevrer.Api.ML
{
    //for training only to define schema if loading labeled data
    public class WasteImageData
    {
        [LoadColumn(0)]
        public string ImagePath { get; set; }

        [LoadColumn(1)]
        public string Label { get; set; }
    }

    //training pipeline helper
    public static class WasteModelTrainer
    {
        public static ITransformer TrainModel(MLContext mlContext, IDataView trainDataView)
        {
            var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: "", inputColumnName: nameof(WasteImageData.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: 224, imageHeight: 224))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input"))
                .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(labelColumnName: "Label", featureColumnName: "input"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            return pipeline.Fit(trainDataView);
        }
    }
}