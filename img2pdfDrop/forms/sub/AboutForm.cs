using System;
using System.Windows.Forms;



namespace katbyte.img2pdf.drop {

    /// <summary>
    /// img2pdfDrop about form
    /// </summary>
    public partial class AboutForm : Form {

        /// <summary>
        ///
        /// </summary>
        public AboutForm() {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e) {
            CenterToScreen();

            TopMost = true;

            MinimizeBox = false;
            MaximizeBox = false;

            //grab version and compile date to modify about box
            //TODO,
        }

        private void btn_ok_Click(object sender, EventArgs e) {
            Close();
        }
    }
}