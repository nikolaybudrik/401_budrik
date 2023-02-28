using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using YOLOv4MLNet.DataStructures;
using Lol;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using Lol.Yovo4.DataStructures;

namespace YOLOv4MLNet
{
    //https://towardsdatascience.com/yolo-v4-optimal-speed-accuracy-for-object-detection-79896ed47b50
    class Program
    {
        // model is available here:
        // https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4
        const string modelPath = @"D:\Dev\Aplication\yolov4.onnx";

        const string imageFolder = @"Assets\Images";

        const string imageOutputFolder = @"Assets\Output";

        static readonly string[] classesNames = new string[] { "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
        
        public static void Detection(string ImagePath, string imageName, Lol.MainWindow mainWindow)
        {
            MLContext mlContext = new MLContext();
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: modelPath, recursionLimit: 100));
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));
            var predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
            using (var bitmap = new Bitmap(Image.FromFile(ImagePath)))
            {
                // predict
                var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                var results = predict.GetResults(classesNames, bitmap, mainWindow, 0.3f, 0.7f);
                if(results == null)
                    MessageBox.Show("Image has not been processed");
                else
                {
                    foreach(var result in results)
                    {
                        using (var db = new LibraryContext())
                        {
                            db.AddAsync(new DBYoloV4 { Path = ImagePath, BBox0 = result.BBox0, BBox1 = result.BBox1, BBox2 = result.BBox2, BBox3 = result.BBox3, Label = result.Label, Confidence = result.Confidence });
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

    }
}
