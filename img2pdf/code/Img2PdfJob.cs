//Copyright © 2015 kt@katbyte.me
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using katbyte.utility;



namespace katbyte.img2pdf {


    /// <summary>
    /// represents a img2pdf PDF creation job
    /// </summary>
    public class Img2PdfJob {


    //properties
        /// <summary>
        /// pdf file to create
        /// </summary>
        public readonly string   pdf;


        /// <summary>
        /// files to put into the pdf
        /// </summary>
        public readonly string[] files;


        /// <summary>
        /// files processed
        /// </summary>
        public int processed = 0;


        /// <summary>
        /// true if processing (lazy way for now)
        /// </summary>
        public bool processing => processed > 0;

        //callback?


    //constructors

        /// <summary>
        /// create PDF at path with images found in paths[]
        /// </summary>
        public Img2PdfJob(string pdf, IEnumerable<string> paths) {
            this.files = paths.ToArray();
            this.pdf   = pdf;
        }


    //methods
        /// <summary>
        /// run the job synchronously
        /// </summary>
        public void Run() {
            Img2Pdf.CreatePdfFromFiles(pdf, files, ( (b, s, arg3, arg4) => {
                Interlocked.Increment(ref processed);
            }));
        }



    //static helper
        /// <summary>
        /// creates new pdfs from paths intelligently with options
        /// </summary>
        public static IEnumerable<Img2PdfJob> For(IEnumerable<string> paths, IImg2PdfOptions options = null) {

            string[] pathsa = paths.ToArray();

            //if options isn't null validate it
            options?.Validate();


            bool inputFolders = false;
            bool inputFiles   = false;


            //see if we have folders (many jobs) or folders & files (one job) or files (one job)
            //and check for bad paths
            foreach (var path in pathsa.OrderBy(p => p)) {
                 //is an input, ensure it exists
                if (Directory.Exists(path)) {
                    inputFolders = true;
                } else if (File.Exists(path)) {
                    inputFiles = true;
                } else {
                    throw new Exception("Invalid path '" + path + "' in inputs.");
                }
            }


            //clean up
            // Job takes files and can figure things out, pass in directory, page size etc

            List<Img2PdfJob> newJobs = new List<Img2PdfJob>();
            if (inputFolders && ! inputFiles) {

                //all paths are folders so make a pdf for each folder

                foreach (var folder in pathsa) {
                    var dir = options?.outputDirectory ?? Path.GetDirectoryName(folder);
                    newJobs.Add(new Img2PdfJob(dir + "\\" + Path.GetFileName(folder) + ".pdf", KPath.GetAllFiles(folder)));
                }
            } else {
                //todo, is hack, put into katbyte.dll strings.FindCommonStart ?
                string start = "" + pathsa[0][0];
                for (int i = 0; i < pathsa[0].Length && pathsa.Any(input => input.StartsWith(start)); start = pathsa[0].Substring(0, ++i)) {}



                var dir = options?.outputDirectory ?? Path.GetDirectoryName(start);
                if (dir == null || ! Directory.Exists(dir)) {
                    throw new Exception("Unable to determine common outputDirectory to place PDF.... what now?");
                }

                // get jobs
                newJobs.Add(new Img2PdfJob(dir + "\\" + Path.GetFileName(dir) + ".pdf", KPath.GetAllFiles(pathsa)));

            }

            return newJobs;

        }
    }
}