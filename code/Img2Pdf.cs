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
using katbyte.utility;



//TODO:
// - CBR/CBZ
// - .lib folder for dependencies
// - add some color to version
// - better output for drag & drop
// - more switchs
//      - q for no output
//      - c to check for existing pdfs
//      - t to disable sub-folder file search
//      - p save pdfs to this path
// - md file & docs & comp (move this TODO there?)
// - set page size and then fit images?
// - winforms img2pdfDrop

//NOTES:
//  - drag and drop gives the files in the order they were selected



namespace katbyte.img2pdf {

    sealed partial class Img2Pdf : KonsoleProgram {

    //constants
        private static string appname  = "img2pdf";
        private static string homepage = "https://github.com/katbyte/cs.img2pdf";



    //KonsoleProgram setup
        //window title
        public override string title => appname;

        //program entry point, create new program and start
        static void Main(string[] args) {
            new Img2Pdf().Start(args);
        }



    //program options/cfg

        //private fields for simplicity, revisit

        private string[] inputs = new string[0];

        private string output;
        private string curpath = Directory.GetCurrentDirectory() + "\\";

        //switches
        private bool ensmallen;
        private bool embiggen;

        //keep track of if files/folders are found to determine "mode"
        private bool inputFolders;
        private bool inputFiles;




    //program
        public override int Code(string[] args) {


            //returns true if we should quit
            if (ParseArguments(args)) {
                return 0;
            }


        //the magic

            //multiple folders to pdfs (can assume every input is a folder)
            if (output == null && ! inputs.IsEmpty()) {

                //display whats going on
                Konsole.Write(CT.N("creating individual PDFs for: ", CC.WHITE));
                foreach (var i in inputs) {
                    Konsole.Write(CT.N(Path.GetFileName(i), CC.MAGENTA), CT.N(", ", CC.GRAY));
                }
                Konsole.WriteLine();
                Konsole.WriteLine();

                //pdf each folder
                foreach (var i in inputs) {
                    var pdf = i + ".pdf";
                    Konsole.WriteLine(
                        CT.N(Path.GetDirectoryName(i)+"\\", CC.GRAY),
                        CT.N(Path.GetFileName(i), CC.WHITE),
                        CT.N(" ==> ", CC.CYAN),
                        CT.N(Path.GetDirectoryName(pdf)+"\\", CC.GRAY),
                        CT.N(Path.GetFileName(pdf), CC.WHITE)
                    );

                    Document doc = NewPdf(i + ".pdf");

                    foreach (string file in Directory.GetFiles(i, "*", SearchOption.AllDirectories)) {
                        var r = AddImage(doc, file);
                        //todo cleanup doc.ConsoleAddImage(file, curpath); or something
                        if (r != null) {
                            PrintWarning(r);
                        } else {
                            PrintAdd(file, curpath);
                        }
                    }

                    doc.Close();
                    Konsole.WriteLine(CT.N("    finished!", CC.GREEN));
                }

            //all inputs to a single pdf
            } else if (! inputs.IsEmpty()) {

                //display whats going on
                Konsole.Write(
                    CT.N("creating ", CC.GRAY),
                    CT.N(output, CC.CYAN),
                    CT.N(" for: ", CC.GRAY));
                foreach (var i in inputs) {
                    Konsole.Write(CT.N(i, CC.MAGENTA), CT.N(", ", CC.GRAY));
                }
                Konsole.WriteLine();


                Document doc = NewPdf(output);


                foreach (var i in inputs) {
                    if (Directory.Exists(i)) {
                        foreach (string file in Directory.GetFiles(i, "*", SearchOption.AllDirectories)) {
                            var r = AddImage(doc, file);
                             //todo cleanup doc.ConsoleAddImage(file, curpath); or something
                            if (r != null) {
                                PrintWarning(r);
                            } else {
                                PrintAdd(file, curpath);
                            }
                        }
                    } else {
                        var r = AddImage(doc, i);
                        //todo cleanup doc.ConsoleAddImage(file, curpath); or something
                        if (r != null) {
                            PrintWarning(r);
                        } else {
                            PrintAdd(i, curpath);
                        }
                    }
                }

                doc.Close();
                Konsole.WriteLine(CT.N("    finished!", CC.GREEN));

            //read files and folders from stdin
            } else {
                throw new NotImplementedException("todo: steam input");
            }

            return 0;
        }




    //arguments
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
            var inputs = inputsList.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();


        //check sanity

            //can't do both... or can we ;)
            if (ensmallen && embiggen) {
                throw new Exception("can not use switches -s and -l at the same time");
            }


            //ensure we are configured for a single output when both files and folders are inputs
            if (inputFiles && inputFolders) {
                if (output == null) {

                    //if they all are in the same directory use that

                    //quick hack to grab the first n chars that are the same over all inputs
                    //TODO HACKALERT optimize/google a better solution
                    string start = "" + inputs[0][0];
                    for (int i = 0; i < inputs[0].Length && inputs.Any(input => input.StartsWith(start)); start = inputs[0].Substring(0, ++i)) {}

                    //TODO HACKALERT this does have the possibility to place the pdf in strange places
                    //maybe force the user to pick a location instead of trying to be smart?
                    var dir = Path.GetDirectoryName(start);
                    if (dir == null || ! Directory.Exists(dir)) {
                        throw new Exception("Unable to determine common directory to place PDF, please specify a path"); //make error clearer TODO
                    }

                    output = dir + "\\" + Path.GetFileName(dir) + ".pdf";
                    Konsole.WriteLine(CT.N("WARNING: ", CC.YELLOW), CT.N("found both file and folder inputs without an output pdf file specified, using '" + output + "'", CC.GREEN));
                    Konsole.WriteLine();
                }
            }

            return false;
        }



    //pdf helpers
        /// <summary>
        /// generates a new PDF document and opens it for writing, don't forget to close!
        /// </summary>
        private static Document NewPdf(string path) {
            Document doc = new Document();
            doc.SetMargins(0, 0, 0, 0);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
            //writer.SetPdfVersion(PdfWriter.VERSION_1_7);
            writer.SetFullCompression();


            doc.Open();

            //configure doc
            doc.AddTitle(Path.GetFileName(path).RemoveFromEnd(".pdf"));
            doc.AddCreationDate();
            doc.AddCreator("created by " + appname + " @ " + homepage);

            return doc;

        }


        /// <summary>
        /// adds an image to the document, returns null for success, error message for error
        /// </summary>
        private static string AddImage(Document doc, string path) {

            Image i;
            try {
                i = Image.GetInstance(new Uri(path));
            } catch (Exception ex) {
                return ex.Message;
            }

            //set page size and add a new page
            doc.SetPageSize(new Rectangle(i.Width, i.Height));
            doc.NewPage();


            //scale and add image
            i.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
            doc.Add(i);
            return null;
        }


    //output helpers
        private static void PrintWarning(string w) {
            Konsole.WriteLine(
                CT.N("    warn: ", CC.YELLOW),
                CT.N(w, CC.GRAY)
            );
        }

        private static void PrintAdd(string file, string curpath) {
            Konsole.WriteLine(
                CT.N("    add:  ", CC.GREEN),
                CT.N(file.RemoveFromStart(curpath), CC.GRAY)
            );
        }
    }
}