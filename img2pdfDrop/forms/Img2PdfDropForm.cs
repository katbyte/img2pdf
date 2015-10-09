//Copyright © 2015 kt@katbyte.me
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Drawing;

using katbyte.data;
using katbyte.extend;
using katbyte.win32;
using katbyte.winforms;

using Timer = System.Windows.Forms.Timer;



//TODO:
// menu:
//  - about (link to github, copyright notice, inspired by lamedrop)
//  - set page size
// log window?
// log file
// edit background image to be nicer, add text?
// save settings to ini/reg
// notifications ?

//cbr cbz
//icon (www.fiverr.com?)
// rotating/pulsing  icon
// about form build info
// esc closes about/dialog forms



namespace katbyte.img2pdf.drop {

    /// <summary>
    /// img2pdfDrop main form
    /// </summary>
    sealed public partial class Img2PdfDropForm : Form, IImg2PdfOptions {



    //IOutputDirectorySettings
    #pragma warning disable 1591 //comments from interface are sufficient

        public string outputDirectory { get; set; }

        public bool ensmallen {
            get { return mi_ensmallen.Checked; }
            set { mi_ensmallen.Checked = value; }
        }

        public bool embiggen {
            get { return mi_embiggen.Checked; }
            set { mi_embiggen.Checked = value; }
        }

    #pragma warning restore 1591



    //form elements
        private readonly ContextMenu menu;
        private readonly MenuItem    mi_alwaysOnTop;
        private readonly MenuItem    mi_openAfterCreation;
        private readonly MenuItem    mi_embiggen;
        private readonly MenuItem    mi_ensmallen;


    //other objects
        private Timer  uiUpdateTimer = new Timer();
        private Thread workerThread = null;


    //state
        private readonly ConcurrentQueue<Img2PdfJob> jobs  = new ConcurrentQueue<Img2PdfJob>();
        private readonly object                      jlock = new object(); //lock object for adding new jobs

        private Img2PdfJob currentJob ; //only updated by worker thread
        private int totalJobsQueued = 0; //increased until all jobs are finished for %done of current jobs


    //component init
        /// <summary>
        /// img2pdfDrop main form constructor
        /// </summary>
        public Img2PdfDropForm() {

        //configures designer components
            InitializeComponent();


        //setup 'invisible' controls
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

            //move somewhere/improve, SetupMenu() or static menuitems field?

            menu = new ContextMenu();
            menu.MenuItems.Add("About", (s, e)                    => {(new AboutForm()).Show();});
            menu.MenuItems.Add("Output Directory Options", (s, e) => {(new OutputDirectoryForm(this)).Show();});
            menu.MenuItems.Add("-");
            mi_alwaysOnTop        = menu.MenuItems.New("Always on top", (s, e) => {
                var mi = ((MenuItem)s);
                mi.CheckedMenuItemClicked();
                Win32.User32.SetWindowTopMost(Handle, mi.Checked);
            });
            mi_openAfterCreation  = menu.MenuItems.New("Open PDFs",    (s, e) => { ((MenuItem)s).CheckedMenuItemClicked();});
            menu.MenuItems.Add("-");
            mi_embiggen  = menu.MenuItems.New("Largest as page size",  (s, e) => { ((MenuItem)s).CheckedMenuItemClicked(); mi_ensmallen.Checked = false; });
            mi_ensmallen = menu.MenuItems.New("Smallest as page size", (s, e) => { ((MenuItem)s).CheckedMenuItemClicked(); mi_embiggen.Checked  = false; });
            //menu.MenuItems.Add("Set page size");
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem("Exit", (s, e) => {
                if ( ! workerThread.IsNotNullAndAlive()) {
                    Application.Exit();
                    return;
                }

                (new Thread(() => {
                    if (MessageBox.Show("Images are currently being processed, exit anyways?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                        //quick and dirty, but probably not optimal
                        workerThread.Abort();
                        Application.Exit();
                    }
                })).Start();

            }, Shortcut.AltF4));


        //setup ui timer
            uiUpdateTimer.Interval = 333;
            uiUpdateTimer.Tick    += UpdateUi;

        }


        private void FormLoad(object sender, EventArgs e) {

            //move to centre of the active screen
            CenterToScreen();

            //update all UI elements
            UpdateUi();

            //set topmost
            mi_alwaysOnTop.Checked = true;
            Win32.User32.SetWindowTopMost(Handle);

            //this wasn't necessary before.. but now it is?
            //try and track down someday, not really worth the time however
            Win32.User32.SetWindowTopMost(Handle);
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
        private void DragEnterEvent(object s, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void DragDropEvent(object s, DragEventArgs e) {
            Img2PdfJob[] newjobs;
            try {
                newjobs = Img2PdfJob.For((string[]) e.Data.GetData(DataFormats.FileDrop), this).ToArray();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //lock to prevent worker from "finishing" while we are adding new jobs
            lock (jlock) {

                //enqueue new jobs and add to totalJobsQueued
                newjobs.ForEach(j => jobs.Enqueue(j));
                totalJobsQueued += newjobs.Length;

                //start worker thread if its not already started
                if (! workerThread.IsNotNullAndAlive()) {
                    workerThread = new Thread(Worker);
                    workerThread.Start();
                }

                //force a ui update && start ui update timer
                UpdateUi();
                uiUpdateTimer.Start();
            }
        }



    //worker
        private void Worker() {

            while (true) {

                //while jobs are queued
                while (! jobs.IsEmpty) {
                    if (jobs.TryDequeue(out currentJob)) {
                        //only this thread will change current job
                        currentJob.Run();

                        if (mi_openAfterCreation.Checked) {
                            Process.Start(currentJob.pdf);
                        }
                    }
                }

                //jobs is empty, lock, check to make sure jobs is STILL empty and then clean up
                lock (jlock) {

                    if (! jobs.IsEmpty) {
                        continue;
                    }

                    currentJob      = null;
                    totalJobsQueued = 0;
                    workerThread    = null;


                    //stop ui updating timer as theres nothing more to show
                    uiUpdateTimer.Stop();

                    //make sure ui is updated to reflect our "done" state
                    //using Invoke because we are not on the main thread here
                    Invoke(new Action(() => UpdateUi()));

                    return;
                }
            }
        }



    //ui
        private void UpdateUi(object sender = null, EventArgs e = null) {
            lbl_queue.Text = jobs.ToArray().Select(j => Path.GetFileName(j.pdf)).Join(Environment.NewLine);

            // get a local copy of currentJob, it could change at any point
            var job = currentJob;

            if (job != null) {
                lbl_queueprocess.Width = (int) (Width *  (((float)totalJobsQueued - jobs.Count -1) / totalJobsQueued ));
                lbl_jobprocess.Width   = (int) (Width * ( 1.0 - ((job.files.Length - job.processed) / (float)job.files.Length)));
                lbl_jobprocess.Text    = "" + job.processed + "/" + job.files.Length + " " + Path.GetFileName(job.pdf);
            } else {
                lbl_queueprocess.Text  = "";
                lbl_queueprocess.Width = 0;
                lbl_jobprocess.Width   = 0;
            }
        }

    }
}