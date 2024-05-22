using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.IO;
using System.Drawing.Drawing2D;
using NAudio;
using NAudio.Wave;

namespace MyWinFormsApp
{
    public partial class MainForm : Form
    {
        private Bitmap originalImage;
        private Bitmap grayImage;
        private Bitmap modifiedGrayImage;
        private Button btnBrowse;
        private Button btnCrop;
        private Button btnIdentifyArea;
        private PictureBox pictureBoxOriginal;
        private Button btnSave;
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing = false;
        private ComboBox cmbColorMap;
        private ComboBox cmbShape;
        private TextBox txtInput;
        private Button btnSaveText;
        private enum ColorMap
        {
            Rainbow,
            Jet,
            Ocean,
            None,
        }

        private enum Shape
        {
            Rectangle,
            Circle,
            Triangle,
        }

        private ColorMap selectedColorMap;
        private Shape selectedShape;
        // for recording the audio part
        private Button btnRecordAudio;
        private WaveIn waveSource;
        private WaveFileWriter waveFileWriter;
        private string audioFilePath;
        // end of the audio part

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Medical Image Editor";
            this.MinimumSize = new Size(1280, 960);

            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            btnBrowse = new Button
            {
                AutoSize = true,
                Text = "Browse",
                Margin = new Padding(10)
            };
            btnBrowse.Click += btnBrowse_Click;
            ToolTip btnBrowseToolTip = new ToolTip();
            btnBrowseToolTip.SetToolTip(btnBrowse, "Browse and load an image.");

            btnSave = new Button
            {
                AutoSize = true,
                Text = "Save Image",
                Margin = new Padding(10)
            };
            btnSave.Click += btnSave_Click;
            ToolTip btnSaveToolTip = new ToolTip();
            btnSaveToolTip.SetToolTip(btnSave, "Save the modified image.");

            Label lblColorMap = new Label
            {
                Text = "Select Color Map:",
                Margin = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            cmbColorMap = new ComboBox
            {
                Width = 120,
                Margin = new Padding(10)
            };
            cmbColorMap.Items.AddRange(Enum.GetNames(typeof(ColorMap)));
            cmbColorMap.SelectedIndex = 0;
            cmbColorMap.SelectedIndexChanged += cmbColorMap_SelectedIndexChanged;
            ToolTip cmbColorMapToolTip = new ToolTip();
            cmbColorMapToolTip.SetToolTip(cmbColorMap, "Select a color map for the image.");

            Label lblShape = new Label
            {
                Text = "Select Shape:",
                Margin = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            cmbShape = new ComboBox
            {
                Width = 120,
                Margin = new Padding(10)
            };
            cmbShape.Items.AddRange(Enum.GetNames(typeof(Shape)));
            cmbShape.SelectedIndex = 0;
            cmbShape.SelectedIndexChanged += cmbShape_SelectedIndexChanged;
            ToolTip cmbShapeToolTip = new ToolTip();
            cmbShapeToolTip.SetToolTip(cmbShape, "Select a shape to draw.");

            btnCrop = new Button
            {
                AutoSize = true,
                Text = "Crop",
                Margin = new Padding(10)
            };
            btnCrop.Click += btnCrop_Click;
            ToolTip btnCropToolTip = new ToolTip();
            btnCropToolTip.SetToolTip(btnCrop, "Crop the selected region of the image.");

            btnIdentifyArea = new Button
            {
                AutoSize = true,
                Text = "Identify Area",
                Margin = new Padding(10)
            };
            btnIdentifyArea.Click += btnIdentifyArea_Click;
            ToolTip btnIdentifyAreaToolTip = new ToolTip();
            btnIdentifyAreaToolTip.SetToolTip(btnIdentifyArea, "Apply color map to the selected region.");




            flowPanel.Controls.Add(btnBrowse);
            flowPanel.Controls.Add(btnSave);
            flowPanel.Controls.Add(lblColorMap);
            flowPanel.Controls.Add(cmbColorMap);
            flowPanel.Controls.Add(lblShape);
            flowPanel.Controls.Add(cmbShape);
            flowPanel.Controls.Add(btnCrop);
            flowPanel.Controls.Add(btnIdentifyArea);

            Panel imagePanel = new Panel
            {
                Height = 720,
                Dock = DockStyle.Top,
                AutoScroll = true,
                Padding = new Padding(10),
            };

            pictureBoxOriginal = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                Margin = new Padding(10)
            };
            pictureBoxOriginal.MouseDown += pictureBoxOriginal_MouseDown;
            pictureBoxOriginal.MouseMove += pictureBoxOriginal_MouseMove;
            pictureBoxOriginal.MouseUp += pictureBoxOriginal_MouseUp;
            pictureBoxOriginal.Paint += pictureBoxOriginal_Paint;

            imagePanel.Controls.Add(pictureBoxOriginal);

            FlowLayoutPanel textPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            Label lblTextInput = new Label
            {
                Text = "Enter Annotation Text:",
                Margin = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            txtInput = new TextBox
            {
                Width = 400,
                Height = 100,
                Multiline = true,
                Margin = new Padding(10)
            };

            btnSaveText = new Button
            {
                AutoSize = true,
                Text = "Save Text",
                Margin = new Padding(10)
            };
            btnSaveText.Click += btnSaveText_Click;
            ToolTip btnSaveTextToolTip = new ToolTip();
            btnSaveTextToolTip.SetToolTip(btnSaveText, "Add a text annotation to the image.");
            // the audio part again 😀
            btnRecordAudio = new Button
            {
                AutoSize = true,
                Text = "Record Audio",
                Margin = new Padding(10)
            };
            btnRecordAudio.Click += btnRecordAudio_Click;
            ToolTip btnRecordAudioToolTip = new ToolTip();
            btnRecordAudioToolTip.SetToolTip(btnRecordAudio, "Record 10 seconds of audio.");
            textPanel.Controls.Add(btnRecordAudio);
            // the end of audio part again 🙃
            textPanel.Controls.Add(lblTextInput);
            textPanel.Controls.Add(txtInput);
            textPanel.Controls.Add(btnSaveText);

            this.Controls.Add(textPanel);
            this.Controls.Add(imagePanel);
            this.Controls.Add(flowPanel);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);

                using (Mat imagetest = CvInvoke.Imread(openFileDialog.FileName, ImreadModes.AnyColor))
                {
                    Mat grayscale = new Mat();
                    CvInvoke.CvtColor(imagetest, grayscale, ColorConversion.Bgr2Gray);

                    modifiedGrayImage = new Bitmap(grayscale.Width, grayscale.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(modifiedGrayImage))
                    {
                        g.DrawImage(grayscale.ToBitmap(), new Rectangle(0, 0, grayscale.Width, grayscale.Height));
                    }

                    pictureBoxOriginal.Image = modifiedGrayImage;
                    grayImage = new Bitmap(grayscale.Width, grayscale.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(grayImage))
                    {
                        g.DrawImage(grayscale.ToBitmap(), new Rectangle(0, 0, grayscale.Width, grayscale.Height));
                    }
                }
            }
        }

        private void pictureBoxOriginal_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
        }

        private void pictureBoxOriginal_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;
                pictureBoxOriginal.Invalidate();
            }
        }

        private void pictureBoxOriginal_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            endPoint = e.Location;
        }

        private void pictureBoxOriginal_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing && startPoint != Point.Empty && endPoint != Point.Empty)
            {
                Pen p = new Pen(Color.Yellow);

                switch (selectedShape)
                {
                    case Shape.Rectangle:
                        Rectangle rect = GetRectangle(startPoint, endPoint);
                        e.Graphics.DrawRectangle(p, rect);
                        break;
                    case Shape.Circle:
                        Rectangle boundingBox = GetCircleBoundingBox(startPoint, endPoint);
                        e.Graphics.DrawEllipse(p, boundingBox);
                        break;
                    case Shape.Triangle:
                        Point[] triangle = GetTrianglePoints(startPoint, endPoint);
                        e.Graphics.DrawPolygon(p, triangle);
                        break;
                }
            }
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));
        }

        private Rectangle GetCircleBoundingBox(Point p1, Point p2)
        {
            int radius = (int)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            return new Rectangle(p1.X - radius, p1.Y - radius, 2 * radius, 2 * radius);
        }

        private Point[] GetTrianglePoints(Point p1, Point p2)
        {
            Point p3 = new Point((p1.X + p2.X) / 2, p1.Y);
            return new Point[] { p1, p2, p3 };
        }

        private void cmbColorMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modifiedGrayImage != null)
            {
                selectedColorMap = (ColorMap)Enum.Parse(typeof(ColorMap), cmbColorMap.SelectedItem.ToString());
                pictureBoxOriginal.Image = modifiedGrayImage;
            }
        }

        private void cmbShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modifiedGrayImage != null)
            {
                selectedShape = (Shape)Enum.Parse(typeof(Shape), cmbShape.SelectedItem.ToString());
                pictureBoxOriginal.Image = modifiedGrayImage;
            }
        }

        private void ColorArea(Rectangle rect, ColorMap colorMap)
        {
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    if (x >= 0 && x < modifiedGrayImage.Width && y >= 0 && y < modifiedGrayImage.Height)
                    {
                        Color originalColor = grayImage.GetPixel(x, y);
                        int brightness = originalColor.R;

                        Color mappedColor = MapBrightnessToColor(brightness, colorMap);

                        modifiedGrayImage.SetPixel(x, y, mappedColor);
                    }
                }
            }
        }

        private void ColorArea_Circle(Rectangle boundingBox, ColorMap colorMap)
        {
            int radius = boundingBox.Width / 2;
            Point center = new Point(boundingBox.X + radius, boundingBox.Y + radius);

            for (int x = boundingBox.Left; x < boundingBox.Right; x++)
            {
                for (int y = boundingBox.Top; y < boundingBox.Bottom; y++)
                {
                    int dx = x - center.X;
                    int dy = y - center.Y;
                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        if (x >= 0 && x < modifiedGrayImage.Width && y >= 0 && y < modifiedGrayImage.Height)
                        {
                            Color originalColor = grayImage.GetPixel(x, y);
                            int brightness = originalColor.R;

                            Color mappedColor = MapBrightnessToColor(brightness, colorMap);

                            modifiedGrayImage.SetPixel(x, y, mappedColor);
                        }
                    }
                }
            }
        }

        private void ColorArea_Triangle(Point[] triangle, ColorMap colorMap)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(triangle);
                using (Region region = new Region(path))
                {
                    Rectangle bounds = Rectangle.Round(region.GetBounds(Graphics.FromImage(modifiedGrayImage)));
                    for (int x = bounds.Left; x < bounds.Right; x++)
                    {
                        for (int y = bounds.Top; y < bounds.Bottom; y++)
                        {
                            if (region.IsVisible(x, y) && x >= 0 && x < modifiedGrayImage.Width && y >= 0 && y < modifiedGrayImage.Height)
                            {
                                Color originalColor = grayImage.GetPixel(x, y);
                                int brightness = originalColor.R;

                                Color mappedColor = MapBrightnessToColor(brightness, colorMap);

                                modifiedGrayImage.SetPixel(x, y, mappedColor);
                            }
                        }
                    }
                }
            }
        }

        private Color MapBrightnessToColor(int brightness, ColorMap colorMap)
        {
            switch (colorMap)
            {
                case ColorMap.Rainbow:
                    return MapBrightnessToColor_Rainbow(brightness);
                case ColorMap.Jet:
                    return MapBrightnessToColor_Jet(brightness);
                case ColorMap.Ocean:
                    return MapBrightnessToColor_Ocean(brightness);
                default:
                    return Color.FromArgb(brightness, brightness, brightness);
            }
        }

        private Color MapBrightnessToColor_Rainbow(int brightness)
        {
            if (brightness < 64)
            {
                //between blue and green
                int blue = 255;
                int green = brightness * 4;
                return Color.FromArgb(0, green, blue);
            }
            else if (brightness < 128)
            {
                //between green and yellow
                int green = 255;
                int red = (brightness - 64) * 4;
                return Color.FromArgb(red, green, 0);
            }
            else if (brightness < 192)
            {
                //between yellow and red
                int red = 255;
                int green = 255 - ((brightness - 128) * 4);
                return Color.FromArgb(red, green, 0);
            }
            else
            {
                //between red and violet
                int red = 255;
                int blue = (brightness - 192) * 4;
                return Color.FromArgb(red, 0, blue);
            }
        }
        private Color MapBrightnessToColor_Jet(int brightness)
        {
            int r = (int)(255 * Math.Min(1.0, Math.Max(0.0, (brightness - 64) / 64.0)));
            int g = (int)(255 * Math.Min(1.0, Math.Max(0.0, (brightness - 128) / 64.0)));
            int b = (int)(255 * Math.Min(1.0, Math.Max(0.0, (brightness - 192) / 64.0)));
            return Color.FromArgb(r, g, b);
        }

        private Color MapBrightnessToColor_Ocean(int brightness)
        {
            if (brightness < 64)
            {
                int blue = 192 + brightness;
                return Color.FromArgb(0, 0, blue);
            }
            else if (brightness < 128)
            {
                int green = 128 + (brightness - 64);
                return Color.FromArgb(0, green, 255);
            }
            else if (brightness < 192)
            {
                int green = 255 - (brightness - 128);
                return Color.FromArgb(64, green, 192);
            }
            else
            {
                int green = 128 - (brightness - 192);
                return Color.FromArgb(127 + green, 255, 255);
            }
        }

        private void btnIdentifyArea_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            if (startPoint == Point.Empty || endPoint == Point.Empty)
            {
                MessageBox.Show("Please select a region to identify and color.");
                return;
            }

            switch (selectedShape)
            {
                case Shape.Rectangle:
                    IdentifyAndColorRectangle();
                    break;
                case Shape.Circle:
                    IdentifyAndColorCircle();
                    break;
                case Shape.Triangle:
                    IdentifyAndColorTriangle();
                    break;
            }

            startPoint = Point.Empty;
            endPoint = Point.Empty;
        }

        private void IdentifyAndColorRectangle()
        {
            Rectangle rect = GetRectangle(startPoint, endPoint);
            ColorArea(rect, selectedColorMap);
            pictureBoxOriginal.Invalidate();
        }

        private void IdentifyAndColorCircle()
        {
            Rectangle boundingBox = GetCircleBoundingBox(startPoint, endPoint);
            ColorArea_Circle(boundingBox, selectedColorMap);
            pictureBoxOriginal.Invalidate();
        }

        private void IdentifyAndColorTriangle()
        {
            Point[] triangle = GetTrianglePoints(startPoint, endPoint);
            ColorArea_Triangle(triangle, selectedColorMap);
            pictureBoxOriginal.Invalidate();
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            if (startPoint == Point.Empty || endPoint == Point.Empty)
            {
                MessageBox.Show("Please select a region to crop.");
                return;
            }

            Rectangle cropRect = GetRectangle(startPoint, endPoint);

            grayImage = CropImage(grayImage, cropRect);
            modifiedGrayImage = CropImage(modifiedGrayImage, cropRect);

            pictureBoxOriginal.Image = modifiedGrayImage;

            startPoint = Point.Empty;
            endPoint = Point.Empty;
        }

        private Bitmap CropImage(Bitmap source, Rectangle cropRect)
        {
            Bitmap croppedImage = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(source, new Rectangle(0, 0, cropRect.Width, cropRect.Height),
                            cropRect, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (modifiedGrayImage == null)
            {
                MessageBox.Show("No colored image to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg|PNG Image|*.png"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                modifiedGrayImage.Save(saveFileDialog.FileName);
                MessageBox.Show("Colored image saved successfully.");
            }
        }

        private void btnSaveText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                MessageBox.Show("Please enter text to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text File|*.txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, txtInput.Text);
                MessageBox.Show("Text saved successfully.");
            }
        }
        private void btnRecordAudio_Click(object sender, EventArgs e)
        {
            // StartRecording();
            RecordAudio2("D://newtest/");
        }
        static void RecordAudio2(string outputAudioFile)
        {
            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = new WaveFormat(44100, 1);
                WaveFileWriter waveFileWriter = null;
                waveIn.DataAvailable += (sender, e) =>
                {
                    if (waveFileWriter == null)
                    {
                        waveFileWriter = new WaveFileWriter(outputAudioFile + "Rec.wav", waveIn.WaveFormat);
                    }
                    waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                };


                waveIn.StartRecording();
                Console.WriteLine("Recording for 10 seconds...");
                MessageBox.Show("Recording started. Please speak for 10 seconds.");
                Task.Delay(10000).Wait();
                waveIn.StopRecording();


                waveFileWriter?.Dispose();
                Console.WriteLine("Recording stopped. Audio saved to: " + outputAudioFile);
                MessageBox.Show("Recording stopped. Audio saved to: " + outputAudioFile);

            }
        }

    }
}
