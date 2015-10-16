namespace katbyte.img2pdf.drop {
    partial class OutputDirectoryForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputDirectoryForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_pickFolder = new System.Windows.Forms.Button();
            this.txt_outputPath = new System.Windows.Forms.TextBox();
            this.rb_outputdir = new System.Windows.Forms.RadioButton();
            this.rb_inputDir = new System.Windows.Forms.RadioButton();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_pickFolder);
            this.groupBox1.Controls.Add(this.txt_outputPath);
            this.groupBox1.Controls.Add(this.rb_outputdir);
            this.groupBox1.Controls.Add(this.rb_inputDir);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output Directory Options";
            // 
            // btn_pickFolder
            // 
            this.btn_pickFolder.Location = new System.Drawing.Point(364, 70);
            this.btn_pickFolder.Name = "btn_pickFolder";
            this.btn_pickFolder.Size = new System.Drawing.Size(30, 21);
            this.btn_pickFolder.TabIndex = 3;
            this.btn_pickFolder.Text = "...";
            this.btn_pickFolder.UseVisualStyleBackColor = true;
            this.btn_pickFolder.Click += new System.EventHandler(this.btn_pickFolder_Click);
            // 
            // txt_outputPath
            // 
            this.txt_outputPath.Location = new System.Drawing.Point(12, 70);
            this.txt_outputPath.Name = "txt_outputPath";
            this.txt_outputPath.Size = new System.Drawing.Size(346, 20);
            this.txt_outputPath.TabIndex = 2;
            this.txt_outputPath.TextChanged += new System.EventHandler(this.PathChanged);
            // 
            // rb_outputdir
            // 
            this.rb_outputdir.AutoSize = true;
            this.rb_outputdir.Location = new System.Drawing.Point(7, 44);
            this.rb_outputdir.Name = "rb_outputdir";
            this.rb_outputdir.Size = new System.Drawing.Size(121, 17);
            this.rb_outputdir.TabIndex = 1;
            this.rb_outputdir.TabStop = true;
            this.rb_outputdir.Text = "Use Other Directory:";
            this.rb_outputdir.UseVisualStyleBackColor = true;
            // 
            // rb_inputDir
            // 
            this.rb_inputDir.AutoSize = true;
            this.rb_inputDir.Location = new System.Drawing.Point(7, 20);
            this.rb_inputDir.Name = "rb_inputDir";
            this.rb_inputDir.Size = new System.Drawing.Size(138, 17);
            this.rb_inputDir.TabIndex = 0;
            this.rb_inputDir.TabStop = true;
            this.rb_inputDir.Text = "Same as Input Directory";
            this.rb_inputDir.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(337, 115);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 1;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(256, 115);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // OutputDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 146);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputDirectoryForm";
            this.Text = "Output Directory Options";
            this.Load += new System.EventHandler(this.FormLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_pickFolder;
        private System.Windows.Forms.TextBox txt_outputPath;
        private System.Windows.Forms.RadioButton rb_outputdir;
        private System.Windows.Forms.RadioButton rb_inputDir;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_ok;
    }
}