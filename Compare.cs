using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace MyWinFormsApp
{
    public partial class ComparePictures : Form
    {
        private Bitmap originalImage;
        private Bitmap coloredImage;
        private PictureBox pictureBoxOriginal;
        private PictureBox pictureBoxColored;
        private Point? startPoint = null;
        private Point? endPoint = null;
        private Color selectedColor = Color.Red; // Default color
        private Mat hsv1;
        private Mat hsv2;
        private double hsvDifference = 0;

        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button btnChooseImage1;
        private Button btnChooseImage2;
        private Label label;

        public ComparePictures()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; // Open in full-screen mode
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            // Create panels
            panel1 = new Panel { Dock = DockStyle.Left, Width = 300 };
            panel2 = new Panel { Dock = DockStyle.Right, Width = 300 };

            // Create label
            label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            UpdateLabelText();

            // Create picture boxes
            pictureBox1 = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
            pictureBox2 = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };

            // Add picture boxes to panels
            panel1.Controls.Add(pictureBox1);
            panel2.Controls.Add(pictureBox2);

            // Add panels to the form
            Controls.Add(panel1);
            Controls.Add(panel2);
            Controls.Add(label);

            // Create buttons
            btnChooseImage1 = new Button
            {
                Text = "Choose Image",
                Dock = DockStyle.Bottom
            };
            btnChooseImage1.Click += BtnChooseImage1_Click;

            btnChooseImage2 = new Button
            {
                Text = "Choose Image",
                Dock = DockStyle.Bottom
            };
            btnChooseImage2.Click += BtnChooseImage2_Click;

            // Add buttons to panels
            panel1.Controls.Add(btnChooseImage1);
            panel2.Controls.Add(btnChooseImage2);

            // Set form properties
            Text = "Dual Image Viewer";
            Size = new Size(800, 600);
        }

        private void LoadImageToPictureBox1(string imagePath)
        {
            try
            {
                pictureBox1.Image = Image.FromFile(imagePath);
                originalImage = new Bitmap(imagePath);

                using (Mat imagetest = CvInvoke.Imread(imagePath, ImreadModes.AnyColor))
                {
                    hsv1 = new Mat();
                    CvInvoke.CvtColor(imagetest, hsv1, ColorConversion.Bgr2Hsv);
                }

                UpdateLabelText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImageToPictureBox2(string imagePath)
        {
            try
            {
                pictureBox2.Image = Image.FromFile(imagePath);
                originalImage = new Bitmap(imagePath);

                using (Mat imagetest = CvInvoke.Imread(imagePath, ImreadModes.AnyColor))
                {
                    hsv2 = new Mat();
                    CvInvoke.CvtColor(imagetest, hsv2, ColorConversion.Bgr2Hsv);
                }

                UpdateLabelText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnChooseImage1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadImageToPictureBox1(openFileDialog.FileName);
                }
            }
        }

        private void BtnChooseImage2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadImageToPictureBox2(openFileDialog.FileName);
                }
            }
        }

        private void UpdateLabelText()
        {
            if (hsv1 != null && hsv2 != null)
            {
                hsvDifference = CalculateHSVDifference(hsv1, hsv2);
            }

            string text;

            if (hsvDifference > 0 && pictureBox2.Image != null)
            {
                label.ForeColor = Color.Green;
                text = "Getting better";
            }
            else if (hsvDifference < 0)
            {
                label.ForeColor = Color.Red;
                text = "Getting worse";
            }
            else
            {
                label.ForeColor = Color.Blue;
                text = "Insert images";
            }

            label.Font = new Font(label.Font.FontFamily, 12, FontStyle.Bold);
            label.Text = text;
        }

        private double CalculateHSVDifference(Mat hsvImage1, Mat hsvImage2)
        {
            Mat diff = new Mat();
            CvInvoke.AbsDiff(hsvImage1, hsvImage2, diff);
            MCvScalar sum = CvInvoke.Sum(diff);

            return (sum.V0 + sum.V1 + sum.V2) / (hsvImage1.Rows * hsvImage1.Cols);
        }
    }
}
