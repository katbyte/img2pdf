namespace katbyte.img2pdf.drop {
    partial class Img2PdfDropForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Img2PdfDropForm));
            this.lbl_jobprocess = new System.Windows.Forms.Label();
            this.lbl_queueprocess = new System.Windows.Forms.Label();
            this.lbl_queue = new System.Windows.Forms.Label();
            this.lbl_draghere = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_jobprocess
            // 
            this.lbl_jobprocess.BackColor = System.Drawing.Color.Silver;
            this.lbl_jobprocess.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_jobprocess.Location = new System.Drawing.Point(1, 137);
            this.lbl_jobprocess.Name = "lbl_jobprocess";
            this.lbl_jobprocess.Size = new System.Drawing.Size(166, 15);
            this.lbl_jobprocess.TabIndex = 3;
            this.lbl_jobprocess.Text = "job process";
            // 
            // lbl_queueprocess
            // 
            this.lbl_queueprocess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbl_queueprocess.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_queueprocess.Location = new System.Drawing.Point(1, 152);
            this.lbl_queueprocess.Name = "lbl_queueprocess";
            this.lbl_queueprocess.Size = new System.Drawing.Size(166, 15);
            this.lbl_queueprocess.TabIndex = 4;
            this.lbl_queueprocess.Text = "queue process";
            // 
            // lbl_queue
            // 
            this.lbl_queue.BackColor = System.Drawing.Color.Transparent;
            this.lbl_queue.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_queue.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lbl_queue.Location = new System.Drawing.Point(0, 20);
            this.lbl_queue.Name = "lbl_queue";
            this.lbl_queue.Size = new System.Drawing.Size(1977, 115);
            this.lbl_queue.TabIndex = 5;
            this.lbl_queue.Text = "queue";
            this.lbl_queue.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lbl_draghere
            // 
            this.lbl_draghere.BackColor = System.Drawing.Color.Transparent;
            this.lbl_draghere.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_draghere.Location = new System.Drawing.Point(0, 2);
            this.lbl_draghere.Name = "lbl_draghere";
            this.lbl_draghere.Size = new System.Drawing.Size(170, 24);
            this.lbl_draghere.TabIndex = 6;
            this.lbl_draghere.Text = "Drag Folders Here";
            this.lbl_draghere.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Img2PdfDropForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(170, 170);
            this.ControlBox = false;
            this.Controls.Add(this.lbl_draghere);
            this.Controls.Add(this.lbl_queue);
            this.Controls.Add(this.lbl_queueprocess);
            this.Controls.Add(this.lbl_jobprocess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Img2PdfDropForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbl_jobprocess;
        private System.Windows.Forms.Label lbl_queueprocess;
        private System.Windows.Forms.Label lbl_queue;
        private System.Windows.Forms.Label lbl_draghere;
    }
}

