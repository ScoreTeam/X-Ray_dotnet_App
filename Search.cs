using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImageSizeSearchApp
{
    public class ImageSizeSearchForm : Form
    {
        private ComboBox comboBoxSizeRange;
        private Button btnSearch;
        private ListBox listBoxImages;
        private Label label1;
        private TextBox textBoxPath;
        private Button btnBrowse;

        public ImageSizeSearchForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.comboBoxSizeRange = new ComboBox();
            this.btnSearch = new Button();
            this.listBoxImages = new ListBox();
            this.label1 = new Label();
            this.textBoxPath = new TextBox();
            this.btnBrowse = new Button();

            // 
            // comboBoxSizeRange
            // 
            this.comboBoxSizeRange.FormattingEnabled = true;
            this.comboBoxSizeRange.Items.AddRange(new object[] {
            "100KB - 500KB",
            "500KB - 1MB",
            "> 1MB"});
            this.comboBoxSizeRange.Location = new Point(30, 60);
            this.comboBoxSizeRange.Name = "comboBoxSizeRange";
            this.comboBoxSizeRange.Size = new Size(180, 21);
            this.comboBoxSizeRange.TabIndex = 0;

            // 
            // btnSearch
            // 
            this.btnSearch.Location = new Point(230, 60);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new EventHandler(this.BtnSearch_Click);

            // 
            // listBoxImages
            // 
            this.listBoxImages.FormattingEnabled = true;
            this.listBoxImages.Location = new Point(30, 100);
            this.listBoxImages.Name = "listBoxImages";
            this.listBoxImages.Size = new Size(740, 290);
            this.listBoxImages.TabIndex = 2;

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(30, 20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Path:";

            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new Point(70, 20);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new Size(600, 20);
            this.textBoxPath.TabIndex = 4;

            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new Point(690, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new Size(75, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.BtnBrowse_Click);

            // 
            // ImageSizeSearchForm
            // 
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxImages);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.comboBoxSizeRange);
            this.Name = "ImageSizeSearchForm";
            this.Text = "Image Size Search";
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string selectedPath = textBoxPath.Text;
            if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
            {
                MessageBox.Show("Please select a valid path.");
                return;
            }

            string selectedRange = comboBoxSizeRange.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedRange))
            {
                MessageBox.Show("Please select a size range.");
                return;
            }

            listBoxImages.Items.Clear();
            var images = Directory.EnumerateFiles(selectedPath, "*.*", SearchOption.AllDirectories)
                                  .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                                              s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase));

            long minSize = 0;
            long maxSize = long.MaxValue;

            switch (selectedRange)
            {
                case "100KB - 500KB":
                    minSize = 100 * 1024;
                    maxSize = 500 * 1024;
                    break;
                case "500KB - 1MB":
                    minSize = 500 * 1024;
                    maxSize = 1 * 1024 * 1024;
                    break;
                case "> 1MB":
                    minSize = 1 * 1024 * 1024;
                    break;
            }

            foreach (var image in images)
            {
                FileInfo fileInfo = new FileInfo(image);
                if (fileInfo.Length >= minSize && fileInfo.Length <= maxSize)
                {
                    listBoxImages.Items.Add(image);
                }
            }
        }

    }
}
