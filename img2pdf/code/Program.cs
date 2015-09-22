//Copyright © 2014 kt@katbyte.me
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using iTextSharp.text;
using iTextSharp.text.pdf;

using katbyte.extend;
using katbyte.console;
using katbyte.data;
using katbyte.utility;



//TODO:
// - CBR/CBZ
// - .lib folder for dependencies
// - add some color to version
// - better output for drag & drop
// - more switchs
//      - q no output
//      - c check for existing pdfs
//      - t disable sub-folder file search
//      - save pdfs to this path
//      - set page size
// - md file & docs & comp (move this TODO there?)
// - set page size and then fit images?
// - winforms img2pdfDrop

//NOTES:
//  - drag and drop gives the files in the order they were selected



namespace katbyte.img2pdf {

    sealed partial class Program : KonsoleProgram {

    //constants
        private static string appname  = "img2pdf";
        private static string homepage = "https://github.com/katbyte/cs.img2pdf";



    //KonsoleProgram setup
        //window title
        public override string title => appname;

        //program entry point, create new program and start
        static void Main(string[] args) {
            new Program().Start(args);
        }



    //program options/cfg

        //private fields for simplicity, revisit

        private string[] paths = new string[0];

        private string output;
        private string basepath = Directory.GetCurrentDirectory() + "\\"; //should be root of where all imaged are added from

        //switches
        private bool ensmallen;
        private bool embiggen;



        //private KSize pageSize;


    //program
        public override int Code(string[] args) {


            //returns true if we should quit
            if (ParseArguments(args)) {
                return 0;
            }


        //the magic

            //multiple folders to pdfs (can assume every input is a folder)
            if (output == null && ! paths.IsEmpty()) {

                //display whats going on
                Konsole.Write(CT.N("creating individual PDFs for: ", CC.WHITE));
                foreach (var i in paths) {
                    Konsole.Write(CT.N(Path.GetFileName(i), CC.MAGENTA), CT.N(", ", CC.GRAY));
                }
                Konsole.WriteLine();
                Konsole.WriteLine();


                //pdf each folder
                foreach (var path in paths) {
                    var pdf = path + ".pdf";
                    Konsole.WriteLine(
                        CT.N(Path.GetDirectoryName(path)+"\\", CC.GRAY),
                        CT.N(Path.GetFileName(path), CC.WHITE),
                        CT.N(" ==> ", CC.CYAN),
                        CT.N(Path.GetDirectoryName(pdf)+"\\", CC.GRAY),
                        CT.N(Path.GetFileName(pdf), CC.WHITE)
                    );

                    var files = KPath.GetAllFiles(path);
                    CreatePdfFromFiles(path + ".pdf", files, FileCallback);
                    Konsole.WriteLine(CT.N("    finished!", CC.GREEN));
                    Konsole.WriteLine();
                }


            //all inputs to a single pdf
            } else if (! paths.IsEmpty()) {

                //display whats going on
                Konsole.Write(
                    CT.N("creating ", CC.GRAY),
                    CT.N(output, CC.CYAN),
                    CT.N(" for: ", CC.GRAY));
                foreach (var i in paths) {
                    Konsole.Write(CT.N(i, CC.MAGENTA), CT.N(", ", CC.GRAY));
                }
                Konsole.WriteLine();


                var files = KPath.GetAllFiles(paths);
                CreatePdfFromFiles(output , files, FileCallback);
                Konsole.WriteLine(CT.N("    finished!", CC.GREEN));

            //read files and folders from stdin
            } else {
                throw new NotImplementedException("todo: stream input");
            }

            return 0;
        }




    //parse arguments
        /// <summary>
        /// parses all arguments and configures the program, returns true if we should exit
        /// </summary>
        /// <returns>
        /// true if help or version or no args were specified and we should exit
        /// </returns>
        public bool ParseArguments(string[] args) {

             //if no args output short help
            if (args.Length == 0) {
                Konsole.WriteLine(CC.WHITE, Help.basic);
                return true;
            }


            //temp list to collect inputs
            var inputsList = new List<string>();

            //keep track of if files/folders are found to determine "mode"
            bool inputFolders = false;
            bool inputFiles   = false;

            //parse command line
            var argq = args.ToQueue(); //using a queue so we can easily grab the next arg at any point
            while (argq.Count != 0) {
                string a = argq.Dequeue();


                //check if a is last, matches *.pdf and is not a folder
                //NOTE: in rare case img2pdf folder1 folder2 folder.pdf where folder.pdf is a folder but the user wanted it as the output, they can use -o folder.pdf
                if (argq.IsEmpty() && a.EndsWith(".pdf") && ! Directory.Exists(a)) {
                    output = a;
                    continue;
                }


                //switches
                if (a.StartsWith("-") && ! a.StartsWith("--")) {

                    //we use a character array and a for loop so we can tell if we are at the end of a -abcd group
                    //for switches that take a parameter like -o output.pdf so ensure they are the last in the group
                    var carray  = a.RemoveFromStart("-").ToLower().ToCharArray();

                    for (int i = 0; i < carray.Length; i++ ) {
                        char c = carray[i];
                        switch (c) {
                            case 'v':
                                var vinfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
                                //TODO add some color
                                Konsole.WriteLine(CC.WHITE, appname + ".exe " + vinfo);
                                Konsole.WriteLine(CC.WHITE, "Copyright (c) 2014 kt@katbyte.me @ " + homepage);
                                return true;

                            case 'h':
                                var v = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion;
                                Konsole.WriteLine(CC.GRAY, Help.full.FormatIt(v));
                                return true;

                            case 'o':
                                if (i != carray.Length - 1) {
                                    throw new Exception("-o can only be used at the end of a switch group. example -tso c:\\output.pdf");
                                }
                                if (argq.IsEmpty()) {
                                    throw new Exception("-o used without a output folder/file");
                                }

                                //check for .pdf ending?
                                output = argq.Dequeue();
                                break;

                            case 'l':
                                embiggen = true;
                                break;

                            case 's':
                                ensmallen = true;
                                break;

                            default:
                                throw new Exception("unknown switch '"+c+"'");
                        }
                    }

                    continue;
                }


                //is an input, ensure it exists
                if (Directory.Exists(a)) {
                    inputFolders = true;
                } else if (File.Exists(a)) {
                    inputFiles = true;
                } else {
                    throw new Exception("input file/folder '" + a + "' doesn't exist.");
                }

                inputsList.Add(KPath.EnsureRooted(a));
            }

            //remove duplicates and convert to array because why not
            paths = inputsList.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();


        //check sanity

            //can't do both... or can we ;)
            if (ensmallen && embiggen) {
                throw new Exception("can not use switches -s and -l at the same time");
            }


            //ensure we are configured for a single output when both files and folders are inputs
            if ((inputFiles && inputFolders) || (inputFiles && ! inputFolders)) {
                if (output == null) {

                    //if they all are in the same directory use that

                    //quick hack to grab the first n chars that are the same over all inputs
                    //TODO HACKALERT optimize/google a better solution
                    string start = "" + paths[0][0];
                    for (int i = 0; i < paths[0].Length && paths.Any(input => input.StartsWith(start)); start = paths[0].Substring(0, ++i)) {}

                    //TODO HACKALERT this does have the possibility to place the pdf in strange places
                    //maybe force the user to pick a location instead of trying to be smart?
                    var dir = Path.GetDirectoryName(start);
                    if (dir == null || ! Directory.Exists(dir)) {
                        throw new Exception("Unable to determine common directory to place PDF, please specify a path"); //make error clearer TODO
                    }

                    output = dir + "\\" + Path.GetFileName(dir) + ".pdf";
                    Konsole.WriteLine(CT.N("WARNING: ", CC.YELLOW), CT.N("found only files or both file and folder inputs without an output pdf file specified, using '" + output + "'", CC.GREEN));
                    Konsole.WriteLine();
                }
            }

            return false;
        }



    //pdf

        /// <summary>
        /// creates a PDF at path for the given files, for each file callback(success, file, message, Image) will be called
        /// </summary>
        public void CreatePdfFromFiles(string path, IEnumerable<string> files, Action<bool, string, string, Image> callback = null) {

            //if no callback use an empty one
            callback = callback ?? ((b,s1,s2,i) => {  });


            //create document and open
            Document doc = new Document();
            doc.SetMargins(0, 0, 0, 0);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
            //writer.SetPdfVersion(PdfWriter.VERSION_1_7);
            writer.SetFullCompression();

            doc.Open();


            //set some metadata
            doc.AddTitle(Path.GetFileName(path).RemoveFromEnd(".pdf"));
            doc.AddCreationDate();
            doc.AddCreator("created by " + appname + " @ " + homepage);


            //calculate page size if desired
            KSize psize = new KSize();
            if (ensmallen || embiggen) {

                var e = files.Select(file => {
                    var i = Image.GetInstance(new Uri(file));
                    return new KSize((int) i.Width, (int) i.Height);
                });

                psize = embiggen ? e.MaxBy(s => s.area) : e.MinBy(s => s.area);
            }


            //process files
            foreach (string file in files) {
                Image i;
                try {
                    i = Image.GetInstance(new Uri(file));
                } catch (Exception ex) {
                    callback(false, file, ex.Message, null);
                    continue;
                }



                //set page size and add a new page (technically don't have to do this every time if psize isn't empty)
                doc.SetPageSize(psize.empty ? new Rectangle(i.Width, i.Height) : new Rectangle(psize.w, psize.h));
                doc.NewPage();


                //scale and add image
                if (psize.empty) {
                    i.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                } else {
                    // see https://stackoverflow.com/questions/6565703/math-algorithm-fit-image-to-screen-retain-aspect-ratio
                    float rp = (float)psize.w / (float)psize.h;
                    float ri = i.Width / i.Height;

                    float iw = rp > ri ? i.Width * psize.h /  i.Height : psize.w;
                    float ih = rp > ri ? psize.h                       : i.Height * psize.w /  i.Width;
                    i.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                    i.SetAbsolutePosition((psize.w - iw)/2, (psize.h - ih)/2);
                }

                i.Alt = Path.GetFileName(file);
                doc.Add(i);

                callback(true, file, null, i);
            }

            doc.Close();
        }


    //output
        private void FileCallback(bool success, string file, string message, Image image) {
            if (success) {
                Konsole.WriteLine(
                    CT.N("    add:  ", CC.GREEN),
                    CT.N(file.RemoveFromStart(basepath), CC.GRAY)
                );
            } else {
                Konsole.WriteLine(
                    CT.N("    warn: ", CC.YELLOW),
                    CT.N(message, CC.GRAY)
                );
            }
        }
    }
}