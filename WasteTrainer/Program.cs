using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WasteTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
            // ================================
            // 1️⃣ Load dataset
            // ================================
            var datasetDir = @"C:\Users\tasnim\Desktop\dataset-resized";
            var classes = new[] { "cardboard", "glass", "metal", "paper", "plastic", "trash" };

            var images = new List<ImageData>();

            foreach (var label in classes)
            {
                var dir = Path.Combine(datasetDir, label);
                if (!Directory.Exists(dir)) continue;

                foreach (var file in Directory.GetFiles(dir, "*.jpg"))
                {
                    images.Add(new ImageData
                    {
                        ImagePath = file,
                        Label = label
                    });
                }
            }

            Console.WriteLine($"✅ Loaded {images.Count} images");

            foreach (var g in images.GroupBy(i => i.Label))
                Console.WriteLine($"{g.Key}: {g.Count()}");

            // ================================
            // 2️⃣ ML Context
            // ================================
            var mlContext = new MLContext(seed: 1);

            var data = mlContext.Data.LoadFromEnumerable(images);

            // ================================
            // 3️⃣ Train / Test split
            // ================================
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainData = split.TrainSet;
            var testData = split.TestSet;

            // ================================
            // 4️⃣ Preprocessing pipeline
            // ================================
            var preprocessingPipeline =
                mlContext.Transforms.LoadRawImageBytes(
                    outputColumnName: "ImageBytes",
                    imageFolder: "",
                    inputColumnName: nameof(ImageData.ImagePath))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "LabelKey",
                    inputColumnName: nameof(ImageData.Label)));

            // FIT ONLY ON TRAIN DATA
            var preprocessor = preprocessingPipeline.Fit(trainData);

            // TRANSFORM BOTH
            var trainTransformed = preprocessor.Transform(trainData);
            var testTransformed = preprocessor.Transform(testData);

            // ================================
            // 5️⃣ Trainer
            // ================================
            var trainer = mlContext.MulticlassClassification.Trainers.ImageClassification(
                new ImageClassificationTrainer.Options
                {
                    FeatureColumnName = "ImageBytes",
                    LabelColumnName = "LabelKey",
                    Arch = ImageClassificationTrainer.Architecture.MobilenetV2,
                    Epoch = 30,
                    BatchSize = 32,
                    LearningRate = 0.001f,
                    ValidationSet = testTransformed,
                    MetricsCallback = m => Console.WriteLine(m),
                    ReuseTrainSetBottleneckCachedValues = true
                });

            // ================================
            // 6️⃣ Training pipeline
            // ================================
            // ❌ DO NOT append MapKeyToValue — keep output as LabelKey (uint)
            var trainingPipeline = trainer;

            // ================================
            // 7️⃣ Train
            // ================================
            Console.WriteLine("🚀 Training started...");
            var model = trainingPipeline.Fit(trainTransformed);
            Console.WriteLine("✅ Training finished");

            // ================================
            // 8️⃣ Evaluate
            // ================================
            var predictions = model.Transform(testTransformed);

            var metrics = mlContext.MulticlassClassification.Evaluate(
                predictions,
                labelColumnName: "LabelKey",
                predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"📊 MacroAccuracy: {metrics.MacroAccuracy:P2}");
            Console.WriteLine($"📊 MicroAccuracy: {metrics.MicroAccuracy:P2}");

            // ================================
            // 9️⃣ Save model
            // ================================
            mlContext.Model.Save(model, trainTransformed.Schema, "waste_classification_model.zip");
            Console.WriteLine("💾 Model saved successfully");
        }
    }

    // ================================
    // Image data model
    // ================================
    public class ImageData
    {
        public string ImagePath { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}
