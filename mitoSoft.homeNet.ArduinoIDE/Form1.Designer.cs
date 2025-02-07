namespace mitoSoft.homeNet.ArduinoIDE
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            YamlTextBox = new RichTextBox();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            createToolStripMenuItem = new ToolStripMenuItem();
            checkYAMLToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            selectHomeNetNodeToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            WarningTextBox = new RichTextBox();
            toolStrip1 = new ToolStrip();
            toolStripComboBox1 = new ToolStripComboBox();
            BuildToolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            OpenFileDialog = new OpenFileDialog();
            SaveFileDialog = new SaveFileDialog();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // YamlTextBox
            // 
            YamlTextBox.Dock = DockStyle.Fill;
            YamlTextBox.Location = new Point(0, 0);
            YamlTextBox.Name = "YamlTextBox";
            YamlTextBox.Size = new Size(1443, 507);
            YamlTextBox.TabIndex = 0;
            YamlTextBox.Text = "";
            YamlTextBox.TextChanged += RichTextBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 82);
            label1.Name = "label1";
            label1.Size = new Size(407, 32);
            label1.TabIndex = 1;
            label1.Text = "Home Assistant - configuration.yaml:";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1443, 40);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(71, 36);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(206, 44);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(206, 44);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createToolStripMenuItem, checkYAMLToolStripMenuItem, toolStripSeparator2, selectHomeNetNodeToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(89, 36);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // createToolStripMenuItem
            // 
            createToolStripMenuItem.Name = "createToolStripMenuItem";
            createToolStripMenuItem.Size = new Size(428, 44);
            createToolStripMenuItem.Text = "Create homeNet Elements";
            createToolStripMenuItem.Click += CreateHomeNetElementsToolStripMenuItem_Clicked;
            // 
            // checkYAMLToolStripMenuItem
            // 
            checkYAMLToolStripMenuItem.Name = "checkYAMLToolStripMenuItem";
            checkYAMLToolStripMenuItem.Size = new Size(428, 44);
            checkYAMLToolStripMenuItem.Text = "Check YAML";
            checkYAMLToolStripMenuItem.Click += CheckYAMLToolStripMenuItem_Clicked;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(425, 6);
            // 
            // selectHomeNetNodeToolStripMenuItem
            // 
            selectHomeNetNodeToolStripMenuItem.Name = "selectHomeNetNodeToolStripMenuItem";
            selectHomeNetNodeToolStripMenuItem.Size = new Size(428, 44);
            selectHomeNetNodeToolStripMenuItem.Text = "Select HomeNet node";
            selectHomeNetNodeToolStripMenuItem.Click += SelectHomeNetNodeToolStripMenuItem_Clicked;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(45, 36);
            toolStripMenuItem1.Text = "?";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(233, 44);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Clicked;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 127);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(YamlTextBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(WarningTextBox);
            splitContainer1.Size = new Size(1443, 733);
            splitContainer1.SplitterDistance = 507;
            splitContainer1.TabIndex = 5;
            // 
            // WarningTextBox
            // 
            WarningTextBox.Dock = DockStyle.Fill;
            WarningTextBox.Location = new Point(0, 0);
            WarningTextBox.Name = "WarningTextBox";
            WarningTextBox.Size = new Size(1443, 222);
            WarningTextBox.TabIndex = 0;
            WarningTextBox.Text = "";
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripComboBox1, BuildToolStripButton, toolStripSeparator1, toolStripButton1, toolStripButton2 });
            toolStrip1.Location = new Point(0, 40);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1443, 42);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(350, 42);
            // 
            // BuildToolStripButton
            // 
            BuildToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BuildToolStripButton.Image = (Image)resources.GetObject("BuildToolStripButton.Image");
            BuildToolStripButton.ImageTransparentColor = Color.Magenta;
            BuildToolStripButton.Name = "BuildToolStripButton";
            BuildToolStripButton.Size = new Size(46, 36);
            BuildToolStripButton.Text = "Build";
            BuildToolStripButton.Click += BuildToolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 42);
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(46, 36);
            toolStripButton1.Text = "toolStripButton1";
            toolStripButton1.Click += CommentToolStripButton_Clicked;
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(46, 36);
            toolStripButton2.Text = "toolStripButton2";
            toolStripButton2.Click += UncommentToolStripButton_Clicked;
            // 
            // OpenFileDialog
            // 
            OpenFileDialog.Filter = "YAML|*.yaml";
            // 
            // SaveFileDialog
            // 
            SaveFileDialog.FileName = "export";
            SaveFileDialog.Filter = "YAML|*.yaml";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1443, 860);
            Controls.Add(label1);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Arduino IDE";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox YamlTextBox;
        private Label label1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private SplitContainer splitContainer1;
        private RichTextBox WarningTextBox;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem checkYAMLToolStripMenuItem;
        private OpenFileDialog OpenFileDialog;
        private SaveFileDialog SaveFileDialog;
        private ToolStrip toolStrip1;
        private ToolStripButton BuildToolStripButton;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem selectHomeNetNodeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
    }
}
