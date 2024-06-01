namespace MyWinFormsApp;

partial class MainForm
{

    private Button btnBrowse, btnCrop, btnIdentifyArea, btnSave, btnSaveText, buttonShare,
     btnRecordAudio, btnGallery, checkConditionButton, Comparebutton, enhanceButton, btnChooseColor;
    private PictureBox pictureBoxOriginal, pictureBoxEnhanced;

    private TextBox txtInput;
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
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

        btnChooseColor = new Button
        {
            AutoSize = true,
            Text = "Choose Color",
            Margin = new Padding(10)
        };
        btnChooseColor.Click += btnChooseColor_Click;
        ToolTip btnChooseColorToolTip = new ToolTip();
        btnChooseColorToolTip.SetToolTip(btnChooseColor, "Choose a color");

        buttonShare = new Button
        {
            AutoSize = true,
            Text = "Share",
            Margin = new Padding(10)
        };
        buttonShare.Click += buttonShare_Click;
        ToolTip buttonShareToolTip = new ToolTip();
        buttonShareToolTip.SetToolTip(buttonShare, "share an image.");

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
        // checking the condition 
        checkConditionButton = new Button
        {
            Text = "Check Condition",
            // Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(10)

        };
        checkConditionButton.Click += CheckConditionButton_Click;
        enhanceButton = new Button
        {
            Text = "Enhance Image",
            AutoSize = true,
            Margin = new Padding(10)
        };
        enhanceButton.Click += (sender, e) =>
        {
            ProcessImage();
        };
        ToolTip enhanceButtonToolTip = new ToolTip();
        enhanceButtonToolTip.SetToolTip(enhanceButton, "Enhance the image using Fourier transformation.");


        pictureBoxEnhanced = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Margin = new Padding(10)
        };
        Comparebutton = new Button
        {
            AutoSize = true,
            Text = "Compare",
            Margin = new Padding(10)
        };
        Comparebutton.Click += Comparebutton_click;
        ToolTip Compare = new ToolTip();
        Compare.SetToolTip(Comparebutton, "Compare two pictures");

        flowPanel.Controls.Add(btnBrowse);
        flowPanel.Controls.Add(btnSave);
        flowPanel.Controls.Add(enhanceButton);
        flowPanel.Controls.Add(buttonShare);
        flowPanel.Controls.Add(lblColorMap);
        flowPanel.Controls.Add(cmbColorMap);
        flowPanel.Controls.Add(lblShape);
        flowPanel.Controls.Add(cmbShape);
        flowPanel.Controls.Add(btnCrop);
        flowPanel.Controls.Add(btnIdentifyArea);
        flowPanel.Controls.Add(checkConditionButton);
        flowPanel.Controls.Add(Comparebutton);
        flowPanel.Controls.Add(btnChooseColor);



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
        btnGallery = new Button
        {
            AutoSize = true,
            Text = "Gallery",
            Margin = new Padding(10)
        };
        btnGallery.Click += btnGallery_Click;
        ToolTip btnGalleryToolTip = new ToolTip();
        btnGalleryToolTip.SetToolTip(btnGallery, "Launch the image gallery.");
        flowPanel.Controls.Add(btnGallery);
        imagePanel.Controls.Add(pictureBoxEnhanced);
    }


    #endregion
}
