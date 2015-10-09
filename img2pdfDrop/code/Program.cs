//Copyright © 2015 kt@katbyte.me
using System;
using System.Windows.Forms;



namespace katbyte.img2pdf.drop {

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Img2PdfDropForm());
        }
    }
}