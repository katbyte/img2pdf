//Copyright © 2015 kt@katbyte.me
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;



namespace katbyte.img2pdf.drop {


    /// <summary>
    /// img2pdfDrop output outputDirectory options form
    /// </summary>
    public partial class OutputDirectoryForm : Form { 


    //properties and fields
        /// <summary>
        /// the object apply the setting too
        /// </summary>
        private readonly IImg2PdfOptions options;


        /// <summary>
        /// returns true if the directory in the output path textbox is valid
        /// </summary>
        public bool isValid { get { return Directory.Exists(txt_outputPath.Text); } }



    //constructor & load
        /// <summary>
        /// img2pdfDrop output outputDirectory options form constructor
        /// </summary>
        public OutputDirectoryForm(IImg2PdfOptions settings) {
            this.options = settings;

            InitializeComponent();
        }


        private void FormLoad(object sender, EventArgs e = null) {

            CenterToScreen();

            if (options.outputDirectory == null) {
                rb_inputDir.Checked = true;
            } else {
                rb_inputDir.Checked  = false;
                rb_outputdir.Checked = true;
                options.outputDirectory = options.outputDirectory;
            }

            TopMost = true;

            MinimizeBox = false;
            MaximizeBox = false;
        }



    //button actions
        private void btn_ok_Click(object sender, EventArgs e = null) {
            if (rb_inputDir.Checked) {
                //null sets it to input
                options.outputDirectory = null;
            } else {
                if ( ! isValid) {
                    MessageBox.Show("Output outputDirectory is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                options.outputDirectory = txt_outputPath.Text;
            }

            Close();
        }


        private void btn_cancel_Click(object sender, EventArgs e = null) {
            Close();
        }


        private void btn_pickFolder_Click(object sender, EventArgs e = null) {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK) {
                txt_outputPath.Text = dialog.SelectedPath;
            }
        }



    //path color
        private void PathChanged(object sender = null, EventArgs e = null) {

            //if the user changes the path auto select the radio button
            rb_outputdir.Checked = true;

            //live validation via text color
            txt_outputPath.ForeColor = isValid ? Color.Green : Color.Red;
        }
    }
}