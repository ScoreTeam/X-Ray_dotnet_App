using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace MyWinFormsApp
{
    public partial class ComparePictures : Form
    {
        private Bitmap? originalImage;
        private Button? btnBrowse;
        private PictureBox? pictureBoxOriginal;
        private PictureBox? pictureBoxColored;
        private Button? btnIdentifyAreas;
        private Button? btnColorAreas;
        private Button? btnChooseColor;
        private Button? btnSave;
        private Point? startPoint = null;
        private Point? endPoint = null;
        private Color selectedColor = Color.Red; // Default color
        private Mat hsv;
        private Mat hsv1;
        private Mat hsv2;
        private int hsv1value = 0;
        private int hsv2value = 0;
        // noore
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button btnChooseImage1;
        private Button btnChooseImage2;
        private Label label;
        private int beforevalue;
        private int aftervalue;

        public ComparePictures()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; // Open in full-screen mode
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            // Create panels
            panel1 = new Panel();
            panel1.Dock = DockStyle.Left;
            panel1.Width = 300;

            panel2 = new Panel();
            panel2.Dock = DockStyle.Right;
            panel2.Width = 300;
            // Create label
            label = new Label();
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            UpdateLabelText();

            // Create picture boxes
            pictureBox1 = new PictureBox();
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox2 = new PictureBox();
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            // Add picture boxes to panels
            panel1.Controls.Add(pictureBox1);
            panel2.Controls.Add(pictureBox2);

            // Add panels to the form
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(label);
            // Create buttons
            btnChooseImage1 = new Button();
            btnChooseImage1.Text = "Choose Image";
            btnChooseImage1.Dock = DockStyle.Bottom;
            btnChooseImage1.Click += BtnChooseImage1_Click;

            btnChooseImage2 = new Button();
            btnChooseImage2.Text = "Choose Image";
            btnChooseImage2.Dock = DockStyle.Bottom;
            btnChooseImage2.Click += BtnChooseImage2_Click;

            // Add picture boxes and buttons to panels
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(btnChooseImage1);

            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(btnChooseImage2);

            // Set form properties
            Text = "Dual Image Viewer";
            Size = new Size(800, 600);
        }

        private void LoadImageToPictureBox1(string imagePath)
        {
            pictureBox1.Image = Image.FromFile(imagePath);
            // Load the original image using Bitmap
            originalImage = new Bitmap(imagePath);

            // Convert the original image to HSV using Emgu.CV
            using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.AnyColor))
            {
                hsv1 = new Mat();
                CvInvoke.CvtColor(image, hsv1, ColorConversion.Bgr2Hsv);

                // Extract the value channel from the HSV image
                VectorOfMat hsvChannels = new VectorOfMat();
                CvInvoke.Split(hsv1, hsvChannels);
                Mat valueChannel = hsvChannels[2];

                hsv1value = CalculateSumOfPixelValues(valueChannel);
                UpdateLabelText();
            }
        }

        private void LoadImageToPictureBox2(string imagePath)
        {
            pictureBox2.Image = Image.FromFile(imagePath);
            // Load the original image using Bitmap
            originalImage = new Bitmap(imagePath);

            // Convert the original image to HSV using Emgu.CV
            using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.AnyColor))
            {
                hsv2 = new Mat();
                CvInvoke.CvtColor(image, hsv2, ColorConversion.Bgr2Hsv);

                // Extract the value channel from the HSV image
                VectorOfMat hsvChannels = new VectorOfMat();
                CvInvoke.Split(hsv2, hsvChannels);
                Mat valueChannel = hsvChannels[2];

                hsv2value = CalculateSumOfPixelValues(valueChannel);
                UpdateLabelText();
            }
        }

        private void BtnChooseImage1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImageToPictureBox1(openFileDialog.FileName);
            }
        }

        private void BtnChooseImage2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImageToPictureBox2(openFileDialog.FileName);
            }
        }

        private void UpdateLabelText()
        {
            int textvalue = Test();
            string text = "";

            if (textvalue > 0 && hsv2value>0&& hsv1value>0)
            {
                label.ForeColor = Color.Green;
                text = "Getting better";
            }
            else if (textvalue < 0&& hsv2value>0&& hsv1value>0)
            {
                label.ForeColor = Color.Red;
                text = "Getting Worse";
            }
            else
            {
                label.ForeColor = Color.Blue;
                text = "Insert images";
            }
            label.Font = new Font(label.Font.FontFamily, 12, FontStyle.Bold);
            label.Text = text;
        }

        private int Test()
        {
            return hsv1value - hsv2value;
        }

        public static int CalculateSumOfPixelValues(Mat valueChannel)
        {
            if (valueChannel == null || valueChannel.IsEmpty)
            {
                throw new ArgumentException("Unable to read the value channel from the HSV image");
            }

            // Calculate the sum of all pixel values
            MCvScalar sum = CvInvoke.Sum(valueChannel);

            return (int)sum.V0;
        }
    }
}
