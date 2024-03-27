namespace MMS_Slika
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
            if(disposing && (components != null)) {
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
            cbDithering = new CheckBox();
            cbBaseFilter = new CheckBox();
            cbAdvancedFilter = new CheckBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            pbOriginal = new PictureBox();
            pbFiltered = new PictureBox();
            btnLoad = new Button();
            btnSave = new Button();
            groupBox1 = new GroupBox();
            rbDS256 = new RadioButton();
            rbDS16 = new RadioButton();
            groupBox2 = new GroupBox();
            ofdImage = new OpenFileDialog();
            sfdImage = new SaveFileDialog();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbOriginal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbFiltered).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // cbDithering
            // 
            cbDithering.AutoSize = true;
            cbDithering.Location = new Point(6, 22);
            cbDithering.Name = "cbDithering";
            cbDithering.Size = new Size(61, 19);
            cbDithering.TabIndex = 0;
            cbDithering.Text = "Burkes";
            cbDithering.UseVisualStyleBackColor = true;
            // 
            // cbBaseFilter
            // 
            cbBaseFilter.AutoSize = true;
            cbBaseFilter.Location = new Point(6, 47);
            cbBaseFilter.Name = "cbBaseFilter";
            cbBaseFilter.Size = new Size(56, 19);
            cbBaseFilter.TabIndex = 1;
            cbBaseFilter.Text = "Invert";
            cbBaseFilter.UseVisualStyleBackColor = true;
            // 
            // cbAdvancedFilter
            // 
            cbAdvancedFilter.AutoSize = true;
            cbAdvancedFilter.Location = new Point(6, 72);
            cbAdvancedFilter.Name = "cbAdvancedFilter";
            cbAdvancedFilter.Size = new Size(78, 19);
            cbAdvancedFilter.TabIndex = 2;
            cbAdvancedFilter.Text = "Kuwahara";
            cbAdvancedFilter.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(pbOriginal, 0, 0);
            tableLayoutPanel1.Controls.Add(pbFiltered, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 115);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(760, 334);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // pbOriginal
            // 
            pbOriginal.BackColor = SystemColors.Control;
            pbOriginal.Dock = DockStyle.Fill;
            pbOriginal.Location = new Point(3, 3);
            pbOriginal.Name = "pbOriginal";
            pbOriginal.Size = new Size(374, 328);
            pbOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbOriginal.TabIndex = 0;
            pbOriginal.TabStop = false;
            // 
            // pbFiltered
            // 
            pbFiltered.BackColor = SystemColors.Control;
            pbFiltered.Dock = DockStyle.Fill;
            pbFiltered.Location = new Point(383, 3);
            pbFiltered.Name = "pbFiltered";
            pbFiltered.Size = new Size(374, 328);
            pbFiltered.TabIndex = 1;
            pbFiltered.TabStop = false;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(12, 12);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 4;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(292, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbDS256);
            groupBox1.Controls.Add(rbDS16);
            groupBox1.Location = new Point(189, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(97, 74);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Dictionary size";
            // 
            // rbDS256
            // 
            rbDS256.AutoSize = true;
            rbDS256.Location = new Point(6, 47);
            rbDS256.Name = "rbDS256";
            rbDS256.Size = new Size(43, 19);
            rbDS256.TabIndex = 1;
            rbDS256.Text = "256";
            rbDS256.UseVisualStyleBackColor = true;
            // 
            // rbDS16
            // 
            rbDS16.AutoSize = true;
            rbDS16.Checked = true;
            rbDS16.Location = new Point(6, 22);
            rbDS16.Name = "rbDS16";
            rbDS16.Size = new Size(37, 19);
            rbDS16.TabIndex = 0;
            rbDS16.TabStop = true;
            rbDS16.Text = "16";
            rbDS16.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cbDithering);
            groupBox2.Controls.Add(cbBaseFilter);
            groupBox2.Controls.Add(cbAdvancedFilter);
            groupBox2.Location = new Point(93, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(90, 97);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Filters";
            // 
            // ofdImage
            // 
            ofdImage.FileName = "openFileDialog1";
            ofdImage.InitialDirectory = "C:\\Users\\Aleksa\\Desktop\\Elfak\\VIII_semestar\\MMS\\MMS_Slika\\MMS_Slika\\Test images";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnSave);
            Controls.Add(btnLoad);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbOriginal).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbFiltered).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private CheckBox cbDithering;
        private CheckBox cbBaseFilter;
        private CheckBox cbAdvancedFilter;
        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox pbOriginal;
        private PictureBox pbFiltered;
        private Button btnLoad;
        private Button btnSave;
        private GroupBox groupBox1;
        private RadioButton rbDS256;
        private RadioButton rbDS16;
        private GroupBox groupBox2;
        private OpenFileDialog ofdImage;
        private SaveFileDialog sfdImage;
    }
}
