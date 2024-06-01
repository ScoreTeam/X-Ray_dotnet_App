using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing.Drawing2D;
using NAudio.Wave;
using Microsoft.VisualBasic;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace MyWinFormsApp
{
    public partial class MainForm : Form
    {
        private Bitmap originalImage, grayImage, modifiedGrayImage;
        private Point startPoint, endPoint;
        private bool isDrawing = false;
        private ComboBox cmbColorMap, cmbShape;
        private TelegramBotClient botClient;
        private string chatId = "1096093478"; //for hamza 
        // private string chatId = "1185312313"; //for noore
        private enum ColorMap { Rainbow, Jet, Ocean, CustomGradient, None }
        private enum Shape { Rectangle, Circle, Triangle }
        private ColorMap selectedColorMap;
        private Shape selectedShape;
        private double totalArea = 0;
        private Color selectedColor = Color.Yellow;

        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            botClient = new TelegramBotClient("7267075155:AAF-UnqhU_M0moogBcP7ICLSY1WBvGhtYR0");
        }
        private void Comparebutton_click(object? sender, EventArgs e)
        {
            ComparePictures page2Form = new ComparePictures();
            page2Form.ShowDialog(); // Show as a modal dialog
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(openFileDialog.FileName);
            }
        }
        private async void buttonShare_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                await SendFileToTelegram(filePath);
            }
        }

        private async Task SendFileToTelegram(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileToSend = new InputOnlineFile(fileStream, Path.GetFileName(filePath));
                await botClient.SendDocumentAsync(chatId, fileToSend, "Here's your file!");
            }
        }

        private void LoadImage(string filePath)
        {
            originalImage = new Bitmap(filePath);

            using (Mat imagetest = CvInvoke.Imread(filePath, ImreadModes.AnyColor))
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
        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                selectedColor = colorDialog.Color;
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
                Pen p = new Pen(selectedColor);

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
                case ColorMap.CustomGradient: // Handle the new color map
                    return MapBrightnessToColor_CustomGradient(brightness);
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
        private Color MapBrightnessToColor_CustomGradient(int brightness)
        {
            double ratio = (double)brightness / 255.0;
            int r = (int)(selectedColor.R * ratio);
            int g = (int)(selectedColor.G * ratio);
            int b = (int)(selectedColor.B * ratio);
            return Color.FromArgb(r, g, b);
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

            int area = rect.Width * rect.Height;
            totalArea += area;

        }

        private void IdentifyAndColorCircle()
        {
            Rectangle boundingBox = GetCircleBoundingBox(startPoint, endPoint);
            ColorArea_Circle(boundingBox, selectedColorMap);
            pictureBoxOriginal.Invalidate();

            // Calculate the size of the circle
            int diameter = Math.Min(boundingBox.Width, boundingBox.Height);
            double radius = diameter / 2.0;
            double area = Math.PI * Math.Pow(radius, 2);

            totalArea += area;
        }

        private void IdentifyAndColorTriangle()
        {
            Point[] triangle = GetTrianglePoints(startPoint, endPoint);
            ColorArea_Triangle(triangle, selectedColorMap);
            pictureBoxOriginal.Invalidate();

            // size (delete later )
            // Calculate the size of the triangle
            double baseLength = Math.Abs(triangle[2].X - triangle[0].X);
            double height = Math.Abs(triangle[1].Y - triangle[0].Y);
            double area = (baseLength * height) / 2.0;

            totalArea += area;

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

            // Prompt the user to enter the filename
            string fileName = PromptForFileName();

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Filename cannot be empty.");
                return;
            }

            // Specify the path where you want to save the image
            string savePath = Path.Combine("C:\\Users\\Hamza\\Desktop\\newtest", fileName + ".jpg");


            modifiedGrayImage.Save(savePath);
            MessageBox.Show("Colored image saved successfully at: " + savePath);
        }

        private string PromptForFileName()
        {
            string fileName = Interaction.InputBox("Enter the filename for the modified image (without extension):", "Enter Filename", "");

            return fileName;
        }
        private void btnSaveText_Click(object sender, EventArgs e)
        {
            string text = txtInput.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Please enter some text to save.");
                return;
            }
            using (Graphics g = Graphics.FromImage(modifiedGrayImage))
            {
                using (Font font = new Font("Arial", 16))
                {
                    g.DrawString(txtInput.Text, font, Brushes.Yellow, startPoint);
                }
            }

            pictureBoxOriginal.Image = modifiedGrayImage;
        }


        private void btnRecordAudio_Click(object sender, EventArgs e)
        {
            // StartRecording();
            string outputAudioFile = "C:\\Users\\Hamza\\Desktop\\newtest";
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

        private void btnGallery_Click(object sender, EventArgs e)
        {
            // Create a new form for the gallery
            Form galleryForm = new Form
            {
                Text = "Image Gallery",
                Width = 800,
                Height = 600
            };

            // Create a flow layout panel to hold the images
            FlowLayoutPanel galleryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Create the switch button
            Button switchButton = new Button
            {
                Text = "Switch Sorting",
                Dock = DockStyle.Top,
                Size = new Size(30, 30),

            };

            // Create the label to display the current sorting option
            Label sortingLabel = new Label
            {
                Text = "Sorting by: Size",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
            };

            // Sort images based on the selected option (size or last modified date)
            List<string> sortedImages;
            bool sortBySize = true; // Set to true for sorting by size, false for sorting by last modified date

            void SwitchSorting()
            {
                sortBySize = !sortBySize;
                if (sortBySize)
                {
                    sortingLabel.Text = "Sorting by: Size";
                    sortedImages = GetSortedImages();
                }
                else
                {
                    sortingLabel.Text = "Sorting by: Last Modified";
                    sortedImages = GetSortedImagesLastModified();
                }

                // Clear existing images in the gallery panel
                galleryPanel.Controls.Clear();

                // Load and display the sorted images in the gallery panel
                foreach (string imagePath in sortedImages)
                {
                    PictureBox pictureBox = new PictureBox
                    {
                        Image = System.Drawing.Image.FromFile(imagePath),
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Margin = new Padding(10),
                        Tag = imagePath  // Store the image path in the Tag property
                    };

                    pictureBox.Click += PictureBox_Click;  // Assign the event handler
                    galleryPanel.Controls.Add(pictureBox);
                }
            }

            // switchButton.Click += (s, ev) => SwitchSorting(); // Assign the event handler for the switch button
            switchButton.Click += (s, ev) => SwitchSorting();

            galleryForm.Controls.Add(switchButton);
            galleryForm.Controls.Add(sortingLabel);
            galleryForm.Controls.Add(galleryPanel);

            // Show the gallery form
            galleryForm.ShowDialog();
        }
        private List<string> GetSortedImagesLastModified()
        {
            string directoryPath = "C:\\Users\\Hamza\\Desktop\\newtest";
            string[] imageFiles = Directory.GetFiles(directoryPath, "*.jpg", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories))
                .ToArray();

            // Sort the image files by last modified date in descending order
            Array.Sort(imageFiles, (a, b) =>
            {
                FileInfo fileInfoA = new FileInfo(a);
                FileInfo fileInfoB = new FileInfo(b);
                return fileInfoB.LastWriteTime.CompareTo(fileInfoA.LastWriteTime); // Compare in reverse order
            });

            // Convert the array to a list and return
            return new List<string>(imageFiles);
        }
        private List<string> GetSortedImages()
        {
            string directoryPath = "D://newtest"; // Replace with your actual directory path
                                                  // string[] imageFiles = Directory.GetFiles(directoryPath, "*.jpg"); // Change the file extension as needed
            string[] imageFiles = Directory.GetFiles(directoryPath, "*.jpg", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories))
            .ToArray();

            // Sort the image files by size in ascending order
            Array.Sort(imageFiles, (a, b) =>
            {
                FileInfo fileInfoA = new FileInfo(a);
                FileInfo fileInfoB = new FileInfo(b);
                return fileInfoA.Length.CompareTo(fileInfoB.Length);
            });

            // Convert the array to a list and return
            return new List<string>(imageFiles);
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            string imagePath = pictureBox.Tag.ToString();
            LoadImage(imagePath);

            Form galleryForm = (Form)pictureBox.Parent.Parent;
            galleryForm.Close();
        }

        private void CheckConditionButton_Click(object sender, EventArgs e)
        {
            string condition;

            if (totalArea >= 0 && totalArea <= 70000)
            {
                condition = "Mild";
            }
            else if (totalArea > 70000 && totalArea <= 250000)
            {
                condition = "Medium";
            }
            else
            {
                condition = "Severe";
            }

            string message = "Total Area: " + totalArea + "\nCondition: " + condition;
            MessageBox.Show(message, "Condition Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ProcessImage()
        {
            if (pictureBoxOriginal.Image != null)
            {
                // Get the original image from the PictureBox
                Bitmap originalImage = new Bitmap(pictureBoxOriginal.Image);
                int newWidth = (int)Math.Pow(2, Math.Ceiling(Math.Log(originalImage.Width, 2)));
                int newHeight = (int)Math.Pow(2, Math.Ceiling(Math.Log(originalImage.Height, 2)));
                // Resize the image
                ResizeBilinear resizeFilter = new ResizeBilinear(newWidth, newHeight);
                Bitmap resizedImage = resizeFilter.Apply(originalImage);

                // Convert the image to grayscale
                Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayscaleImage = grayscaleFilter.Apply(resizedImage);

                // Apply Fourier transformation
                ComplexImage complexImage = ComplexImage.FromBitmap(grayscaleImage);
                complexImage.ForwardFourierTransform();

                // Apply high-pass filter to enhance the image
                int width = complexImage.Width;
                int height = complexImage.Height;
                double cutoff = 0.1; // Adjust this value based on desired enhancement

                for (int u = 0; u < width; u++)
                {
                    for (int v = 0; v < height; v++)
                    {
                        double distance = Math.Sqrt(Math.Pow(u - width / 2, 2) + Math.Pow(v - height / 2, 2));
                        if (distance < cutoff * Math.Min(width, height))
                        {
                            complexImage.Data[v, u] = new Complex(0, 0);
                        }
                    }
                }


                // Apply inverse Fourier transformation
                complexImage.BackwardFourierTransform();

                // Get the enhanced image
                Bitmap enhancedImage = complexImage.ToBitmap();

                // Save the enhanced image
                SaveEnhancedImage(enhancedImage);
            }
            else
            {
                MessageBox.Show("No image selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveEnhancedImage(Bitmap image)
        {
            string fileName = PromptForFileName();

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Filename cannot be empty.");
                return;
            }

            string savePath = Path.Combine("C:\\Users\\Hamza\\Desktop\\newtest", fileName + ".jpg");

            image.Save(savePath);
            MessageBox.Show("Enhanced image saved successfully at: " + savePath);
        }
    }
}