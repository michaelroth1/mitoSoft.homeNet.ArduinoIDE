namespace mitoSoft.homeNet.ArduinoIDE
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            button1 = new Button();
            label1 = new Label();
            TextBox = new RichTextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            button2 = new Button();
            SaveFileDialog = new SaveFileDialog();
            StatusLabel = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(1353, 20);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 5;
            button1.Text = "Clipboard";
            button1.UseVisualStyleBackColor = true;
            button1.Click += CopyButton_Clicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(167, 32);
            label1.TabIndex = 4;
            label1.Text = "Arduino Code:";
            // 
            // TextBox
            // 
            TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextBox.Location = new Point(12, 72);
            TextBox.Name = "TextBox";
            TextBox.Size = new Size(1491, 672);
            TextBox.TabIndex = 3;
            TextBox.Text = "";
            TextBox.WordWrap = false;
            // 
            // timer1
            // 
            timer1.Interval = 2500;
            timer1.Tick += Timer_Tick;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(1197, 20);
            button2.Name = "button2";
            button2.Size = new Size(150, 46);
            button2.TabIndex = 7;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += SaveButton_Clicked;
            // 
            // SaveFileDialog
            // 
            SaveFileDialog.Filter = "Arduino |*.ino";
            // 
            // StatusLabel
            // 
            StatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StatusLabel.AutoSize = true;
            StatusLabel.Location = new Point(12, 766);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(0, 32);
            StatusLabel.TabIndex = 8;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1515, 807);
            Controls.Add(StatusLabel);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(TextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form2";
            Text = "Arduino IDE";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private RichTextBox TextBox;
        private System.Windows.Forms.Timer timer1;
        private Button button2;
        private SaveFileDialog SaveFileDialog;
        private Label StatusLabel;
    }
}