//namespace Sepia
//{
//    partial class Form1
//    {
//        /// <summary>
//        /// Wymagana zmienna projektanta.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Wyczyść wszystkie używane zasoby.
//        /// </summary>
//        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Kod generowany przez Projektanta formularzy systemu Windows

//        /// <summary>
//        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
//        /// jej zawartości w edytorze kodu.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.processingTimeLabel = new System.Windows.Forms.Label();
//            this.filePathTextBox = new System.Windows.Forms.TextBox();
//            this.originalPictureBox = new System.Windows.Forms.PictureBox();
//            this.threadsNumericUpDown = new System.Windows.Forms.NumericUpDown();
//            this.sepiaStrengthTrackBar = new System.Windows.Forms.TrackBar();
//            this.sepiaNoiseTrackBar = new System.Windows.Forms.TrackBar();
//            this.cppRadioButton = new System.Windows.Forms.RadioButton();
//            this.asmRadioButton = new System.Windows.Forms.RadioButton();
//            this.processedPictureBox = new System.Windows.Forms.PictureBox();
//            this.makeSepiaButton = new System.Windows.Forms.Button();
//            this.chooseFileButton = new System.Windows.Forms.Button();
//            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.threadsNumericUpDown)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.sepiaStrengthTrackBar)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.sepiaNoiseTrackBar)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.processedPictureBox)).BeginInit();
//            this.SuspendLayout();
//            // 
//            // processingTimeLabel
//            // 
//            this.processingTimeLabel.AutoSize = true;
//            this.processingTimeLabel.Location = new System.Drawing.Point(44, 392);
//            this.processingTimeLabel.Name = "processingTimeLabel";
//            this.processingTimeLabel.Size = new System.Drawing.Size(80, 20);
//            this.processingTimeLabel.TabIndex = 0;
//            this.processingTimeLabel.Text = "Time: [ms]";
//            this.processingTimeLabel.Click += new System.EventHandler(this.processingTimeLabel_Click);
//            // 
//            // filePathTextBox
//            // 
//            this.filePathTextBox.Location = new System.Drawing.Point(48, 234);
//            this.filePathTextBox.Name = "filePathTextBox";
//            this.filePathTextBox.Size = new System.Drawing.Size(403, 26);
//            this.filePathTextBox.TabIndex = 1;
//            // 
//            // originalPictureBox
//            // 
//            this.originalPictureBox.Location = new System.Drawing.Point(461, 35);
//            this.originalPictureBox.Name = "originalPictureBox";
//            this.originalPictureBox.Size = new System.Drawing.Size(229, 181);
//            this.originalPictureBox.TabIndex = 2;
//            this.originalPictureBox.TabStop = false;
//            this.originalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

//            // 
//            // threadsNumericUpDown
//            // 
//            this.threadsNumericUpDown.Location = new System.Drawing.Point(188, 131);
//            this.threadsNumericUpDown.Maximum = new decimal(new int[] {
//            64,
//            0,
//            0,
//            0});
//            this.threadsNumericUpDown.Minimum = new decimal(new int[] {
//            1,
//            0,
//            0,
//            0});
//            this.threadsNumericUpDown.Name = "threadsNumericUpDown";
//            this.threadsNumericUpDown.Size = new System.Drawing.Size(120, 26);
//            this.threadsNumericUpDown.TabIndex = 3;
//            this.threadsNumericUpDown.Value = new decimal(new int[] {
//            1,
//            0,
//            0,
//            0});
//            // 
//            // sepiaStrengthTrackBar
//            // 
//            this.sepiaStrengthTrackBar.Location = new System.Drawing.Point(48, 301);
//            this.sepiaStrengthTrackBar.Maximum = 40;
//            this.sepiaStrengthTrackBar.Minimum = 20;
//            this.sepiaStrengthTrackBar.Name = "sepiaStrengthTrackBar";
//            this.sepiaStrengthTrackBar.Size = new System.Drawing.Size(104, 69);
//            this.sepiaStrengthTrackBar.TabIndex = 4;
//            this.sepiaStrengthTrackBar.Value = 20;
//            // 
//            // sepiaNoiseTrackBar
//            // 
//            this.sepiaNoiseTrackBar.Location = new System.Drawing.Point(287, 301);
//            this.sepiaNoiseTrackBar.Maximum = 50;
//            this.sepiaNoiseTrackBar.Name = "sepiaNoiseTrackBar";
//            this.sepiaNoiseTrackBar.Size = new System.Drawing.Size(104, 69);
//            this.sepiaNoiseTrackBar.TabIndex = 5;
//            this.sepiaNoiseTrackBar.Value = 25;
//            // 
//            // cppRadioButton
//            // 
//            this.cppRadioButton.AutoSize = true;
//            this.cppRadioButton.Location = new System.Drawing.Point(48, 85);
//            this.cppRadioButton.Name = "cppRadioButton";
//            this.cppRadioButton.Size = new System.Drawing.Size(63, 24);
//            this.cppRadioButton.TabIndex = 6;
//            this.cppRadioButton.TabStop = true;
//            this.cppRadioButton.Text = "C++";
//            this.cppRadioButton.UseVisualStyleBackColor = true;
//            // 
//            // asmRadioButton
//            // 
//            this.asmRadioButton.AutoSize = true;
//            this.asmRadioButton.Location = new System.Drawing.Point(48, 115);
//            this.asmRadioButton.Name = "asmRadioButton";
//            this.asmRadioButton.Size = new System.Drawing.Size(69, 24);
//            this.asmRadioButton.TabIndex = 7;
//            this.asmRadioButton.TabStop = true;
//            this.asmRadioButton.Text = "ASM";
//            this.asmRadioButton.UseVisualStyleBackColor = true;
//            // 
//            // processedPictureBox
//            // 
//            this.processedPictureBox.Location = new System.Drawing.Point(713, 35);
//            this.processedPictureBox.Name = "processedPictureBox";
//            this.processedPictureBox.Size = new System.Drawing.Size(277, 181);
//            this.processedPictureBox.TabIndex = 8;
//            this.processedPictureBox.TabStop = false;
//            this.processedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

//            // 
//            // makeSepiaButton
//            // 
//            this.makeSepiaButton.Location = new System.Drawing.Point(757, 356);
//            this.makeSepiaButton.Name = "makeSepiaButton";
//            this.makeSepiaButton.Size = new System.Drawing.Size(143, 65);
//            this.makeSepiaButton.TabIndex = 9;
//            this.makeSepiaButton.Text = "Make Sepia";
//            this.makeSepiaButton.UseVisualStyleBackColor = true;
//            this.makeSepiaButton.Click += new System.EventHandler(this.MakeSepiaButton_Click);

//            // 
//            // chooseFileButton
//            // 
//            this.chooseFileButton.Location = new System.Drawing.Point(48, 172);
//            this.chooseFileButton.Name = "chooseFileButton";
//            this.chooseFileButton.Size = new System.Drawing.Size(168, 44);
//            this.chooseFileButton.TabIndex = 10;
//            this.chooseFileButton.Text = "Choose File";
//            this.chooseFileButton.UseVisualStyleBackColor = true;
//            this.chooseFileButton.Click += new System.EventHandler(this.ChooseFileButton_Click);
//            // 
//            // Form1
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(1023, 450);
//            this.Controls.Add(this.chooseFileButton);
//            this.Controls.Add(this.makeSepiaButton);
//            this.Controls.Add(this.processedPictureBox);
//            this.Controls.Add(this.asmRadioButton);
//            this.Controls.Add(this.cppRadioButton);
//            this.Controls.Add(this.sepiaNoiseTrackBar);
//            this.Controls.Add(this.sepiaStrengthTrackBar);
//            this.Controls.Add(this.threadsNumericUpDown);
//            this.Controls.Add(this.originalPictureBox);
//            this.Controls.Add(this.filePathTextBox);
//            this.Controls.Add(this.processingTimeLabel);
//            this.Name = "Form1";
//            this.Text = "Form1";
//            this.Load += new System.EventHandler(this.Form1_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.threadsNumericUpDown)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.sepiaStrengthTrackBar)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.sepiaNoiseTrackBar)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.processedPictureBox)).EndInit();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }



//        #endregion

//        private System.Windows.Forms.Label processingTimeLabel;
//        private System.Windows.Forms.TextBox filePathTextBox;
//        private System.Windows.Forms.PictureBox originalPictureBox;
//        private System.Windows.Forms.NumericUpDown threadsNumericUpDown;
//        private System.Windows.Forms.TrackBar sepiaStrengthTrackBar;
//        private System.Windows.Forms.TrackBar sepiaNoiseTrackBar;
//        private System.Windows.Forms.RadioButton cppRadioButton;
//        private System.Windows.Forms.RadioButton asmRadioButton;
//        private System.Windows.Forms.PictureBox processedPictureBox;
//        private System.Windows.Forms.Button makeSepiaButton;
//        private System.Windows.Forms.Button chooseFileButton;
//    }
//}