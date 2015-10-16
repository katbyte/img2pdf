using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

using katbyte.extend;
using katbyte.utility;



namespace katbyte.img2pdf.drop {

    /// <summary>
    /// img2pdfDrop about form
    /// </summary>
    public partial class AboutForm : Form {

    //constructor and form load
        #pragma warning disable 1591
        public AboutForm() {
            InitializeComponent();
        }
        #pragma warning restore 1591

        private void AboutForm_Load(object sender, EventArgs e) {
            CenterToScreen();

            TopMost = true;

            MinimizeBox = false;
            MaximizeBox = false;

            KAssembly assembly = new KAssembly(Assembly.GetExecutingAssembly());

            this.Text          = "img2pdfDrop v" + assembly.version;
            lbl_buildDate.Text = "Compiled " + assembly.linkDate.ToIsoDate() + " (" + assembly.config + ")";

        }



    //actions
        private void btn_ok_Click(object sender, EventArgs e) {
            Close();
        }

        private void lbl_link_Clicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/katbyte/img2pdf");
        }

        //close on esc key press
        #pragma warning disable 1591
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #pragma warning restore 1591

    }
}