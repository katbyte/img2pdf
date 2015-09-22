//Copyright © 2015 kt@katbyte.me
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using katbyte.data;
using katbyte.extend;
using katbyte.img2pdf;
using katbyte.utility;
using katbyte.winforms;

using Timer = System.Windows.Forms.Timer;



//TODO:
// menu:
//  - about (link to github, copyright notice, inspired by lamedrop)
//  - set size
//  - show log
//  - always ontop option
//  - open after creation option
// rotating PDF icon logo
// if jobs running pop up a message box on exit
// center in screen
// make 4.5 only
// multi threaded?? is there any point as its diskbound?
// notifications


namespace katbyte.img2pdfDrop {

    /// <summary>
    /// img2pdfDrop main form
    /// </summary>
    sealed public partial class Img2PdfDropForm : Form {

        //TODO: move to katbyte.win32
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        /// <summary>
        /// see name
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


    //job class
        //represents a collection of files to turns into a pdf
        private class Job {

            /// <summary>
            /// files to put into the pdf
            /// </summary>
            public readonly string[] files;

            /// <summary>
            /// pdf file to create
            /// </summary>
            public readonly string   pdf;


            /// <summary>
            /// files processed
            /// </summary>
            public int processed = 0;


            //construct
            public Job(IEnumerable<string> paths, string pdf) {
                this.files = paths.ToArray();
                this.pdf   = pdf;
            }

        }


    //form data
        private readonly ContextMenu menu;
        private readonly MenuItem    embiggen;
        private readonly MenuItem    ensmallen;

        private readonly ConcurrentQueue<Job>  jobs       = new ConcurrentQueue<Job>();

        private Job currentJob ; //only updated by worker thread
        private int totalJobsQueued = 0;

        private Timer  uiUpdateTimer = new Timer();
        private Thread workerThread = null;


    //component init
        /// <summary>
        /// img2pdfDrop main form constructor
        /// </summary>
        public Img2PdfDropForm() {

            //configures designer components
            InitializeComponent();

            //always on top
            TopMost = true;


            //setup inviable controls
            Control[] invisControls =  {this, lbl_draghere, lbl_queue, lbl_queueprocess, lbl_queueprocess, lbl_jobprocess};
            foreach (var ctrl in invisControls) {

                //add mouse events
                ctrl.MouseDown += MouseDownEvent;
                ctrl.MouseMove += MouseMoveEvent;
                ctrl.MouseUp   += MouseUpEvent;

                //drag n drop
                ctrl.AllowDrop  = true;
                ctrl.DragEnter += DragEnterEvent;
                ctrl.DragDrop  += DragDropEvent;

            }


            //setup menu
            menu = new ContextMenu();
            menu.MenuItems.Add("About", ShowAbout);
            menu.MenuItems.Add("-");
            embiggen  = new MenuItem("Largest as page size",  (s, e) => { ((MenuItem)s).CheckedMenuItemClicked(); ensmallen.Checked = false; });
            ensmallen = new MenuItem("Smallest as page size", (s, e) => { ((MenuItem)s).CheckedMenuItemClicked(); embiggen.Checked  = false; });
            menu.MenuItems.Add(embiggen);
            menu.MenuItems.Add(ensmallen);
            //menu.MenuItems.Add("Set page size");
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem("Exit", Exit, Shortcut.AltF4));


            //setup check jobs timer
            uiUpdateTimer.Interval = 333;
            uiUpdateTimer.Tick    += UpdateUi;



        }

        private void FormLoad(object sender, EventArgs e) {
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            UpdateUi();
        }

    //menu

        private void ShowAbout(object s, EventArgs e) {
            MessageBox.Show("ToDo!");
        }

        private void Exit(object sender, EventArgs e) {
            Application.Exit();
        }



    //mouse events
        private bool   dragging;
        private KPoint mouseDownLocation;
        private KPoint mouseDownWindowLocation;

        private void MouseDownEvent(object s, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                mouseDownLocation       = new KPoint(e.Location.X, e.Location.Y);
                mouseDownWindowLocation = new KPoint(Left, Top);
                dragging = true;
            }
        }

        private void MouseMoveEvent(object s, MouseEventArgs e) {
            if (dragging && e.Button == MouseButtons.Left) {
                Left = mouseDownWindowLocation.x + (e.X - mouseDownLocation.x);
                Top  = mouseDownWindowLocation.y + (e.Y - mouseDownLocation.y);

                mouseDownWindowLocation = new KPoint(Left, Top);
            }
        }

        private void MouseUpEvent(object s, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                dragging = false;
            }

            if (e.Button == MouseButtons.Right) {
                menu.Show(this, e.Location);
            }
        }



    //drag and drop
        void DragEnterEvent(object s, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Link;
            }
        }

        void DragDropEvent(object s, DragEventArgs e) {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            //see if we have folders (many jobs) or folders & files (one job) or files (one job)

            bool inputFolders = false;
            bool inputFiles   = false;

            foreach (var path in paths.OrderBy(p => p)) {
                 //is an input, ensure it exists
                if (Directory.Exists(path)) {
                    inputFolders = true;
                } else if (File.Exists(path)) {
                    inputFiles = true;
                } else {
                    MessageBox.Show("Invalid path '" + path + "' in drag and drop!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


            }

            List<Job> newJobs = new List<Job>();
            if (inputFolders && ! inputFiles) {
                foreach (var folder in paths) {
                    newJobs.Add(new Job(KPath.GetAllFiles(folder), folder + ".pdf"));
                }
            } else {
                //todo, from img2pdf, is hack, put into katbyte.dll
                string start = "" + paths[0][0];
                for (int i = 0; i < paths[0].Length && paths.Any(input => input.StartsWith(start)); start = paths[0].Substring(0, ++i)) {}



                var dir = Path.GetDirectoryName(start);
                if (dir == null || ! Directory.Exists(dir)) {
                    throw new Exception("Unable to determine common directory to place PDF.... what now?");
                }

                // get jobs
                newJobs.Add(new Job(KPath.GetAllFiles(), dir + "\\" + Path.GetFileName(dir) + ".pdf"));

            }

            //enqueue new jobs and add to totalJobsQueued
            newJobs.ForEach(j => jobs.Enqueue(j));
            totalJobsQueued += newJobs.Count;

            //start worker if its not already started
            workerThread = new Thread(Worker);
            workerThread.Start();

            //force a ui update && start ui update timer
            UpdateUi();
            uiUpdateTimer.Start();

        }


    //worker

        private void Worker() {

            while (! jobs.IsEmpty) {
                if (jobs.TryDequeue(out currentJob)) {
                    var j = currentJob;
                    var img2pdf = new Img2Pdf();
                    img2pdf.CreatePdfFromFiles(j.pdf, j.files, ( (b, s, arg3, arg4) => {
                        Interlocked.Increment(ref j.processed);
                    }));
                }


            }

            //
            currentJob = null;
            totalJobsQueued = 0;

        }


    //ui
        private void UpdateUi(object sender = null, EventArgs e = null) {
            lbl_queue.Text = jobs.ToArray().Select(j => Path.GetFileName(j.pdf)).Join(Environment.NewLine);

            // prevents currentJob from going null halfway through ui update
            var job = currentJob;

            if (job != null) {
                lbl_queueprocess.Width = (int) (166 * ( 1.0 - (jobs.Count / (float)totalJobsQueued )));
                lbl_jobprocess.Width   = (int) (166 * ( 1.0 - ((job.files.Length - job.processed) / (float)job.files.Length)));
                lbl_jobprocess.Text    = Path.GetFileName(job.pdf);
            } else {
                lbl_queueprocess.Text = "";
                lbl_queueprocess.Width = 0;
                lbl_jobprocess.Width = 0;
                uiUpdateTimer.Stop();
            }

        }

    }
}