using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Threading;
using Lol.Yovo4.DataStructures;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Lol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string ImagePath;
        private static string ImageName;
        public CancellationTokenSource cts;
        public CancellationToken cT;
        private Task t;

        public MainWindow()
        {
            InitializeComponent();
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == true)
            {
                ImagePath = openFileDialog1.FileName;
                mainImage.Source = new BitmapImage(new Uri(ImagePath));
                ImageName = System.IO.Path.GetFileName(ImagePath);
                MessageBox.Show(ImageName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var db = new LibraryContext())
            {
                if (db.DBYoloV4s.Any())
                {
                    MessageBox.Show("Here");
                    var element = db.DBYoloV4s.Where(a => a.Path.Equals(ImagePath));
                    if (element == null)
                    {
                        mainImage.Source = new BitmapImage(new Uri(ImagePath));
                        cts = new CancellationTokenSource();
                        cT = cts.Token;
                        t = Task.Factory.StartNew(() =>
                        {
                            YOLOv4MLNet.Program.Detection(ImagePath, ImageName, this);
                        }, cT);
                    }
                    else
                    {
                        cts = new CancellationTokenSource();
                        cT = cts.Token;
                        var t = Task.Factory.StartNew(() =>
                        {
                            using (var bitmap = new Bitmap(System.Drawing.Image.FromFile(ImagePath)))
                            {
                                using (var g = Graphics.FromImage(bitmap))
                                {
                                    foreach (var res in element)
                                    {
                                        // draw predictions
                                        var x1 = res.BBox0;
                                        var y1 = res.BBox1;
                                        var x2 = res.BBox2;
                                        var y2 = res.BBox3;
                                        g.DrawRectangle(Pens.Red, x1, y1, x2 - x1, y2 - y1);
                                        using (var brushes = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.Red)))
                                        {
                                            g.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                                        }

                                        g.DrawString(res.Label + " " + res.Confidence.ToString("0.00"),
                                                     new Font("Arial", 12), System.Drawing.Brushes.Blue, new PointF(x1, y1));
                                    }
                                    bitmap.Save(System.IO.Path.Combine(System.IO.Path.GetTempPath(), @"tmp.jpg"));
                                    this.Change_Image(System.IO.Path.Combine(System.IO.Path.GetTempPath(), @"tmp.jpg"));
                                    //bitmap.Save(Path.Combine(imageOutputFolder, Path.ChangeExtension(imageName, "_processed" + Path.GetExtension(imageName))));
                                    //Console.WriteLine(Path.Combine(imageOutputFolder, imageName));
                                }
                            }
                        }, cT);
                    }
                    return;
                }
                mainImage.Source = new BitmapImage(new Uri(ImagePath));
                cts = new CancellationTokenSource();
                cT = cts.Token;
                t = Task.Factory.StartNew(() =>
                {
                    YOLOv4MLNet.Program.Detection(ImagePath, ImageName, this);
                }, cT);
                //YOLOv4MLNet.Program.Main1();
            }
        }
        public void Change_Image(string imagePath)
        {
            var result = new BitmapImage(new Uri(imagePath));
            result.Freeze();
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                mainImage.Source = result;
            }));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
        
    }
}
