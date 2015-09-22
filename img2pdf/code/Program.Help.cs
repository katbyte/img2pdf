//Copyright © 2014 kt@katbyte.me
using katbyte.console;



namespace katbyte.img2pdf {

    sealed partial class Program : KonsoleProgram {

        public static class Help {

            /// <summary>
            /// help text shown when there are no arguments
            /// </summary>
            public static string basic =
            @"
img2pdf: missing input folder[s]
usage:
    img2pdf [OPTION] folder1 folder2 ...
        ==> ./folder1.pdf ./folder2.pdf ...

    OR on windows drag folder[s] to exe

try 'img2pdf --help' or 'img2pdf -h' for more options.";


            /// <summary>
            /// help text shown for -h
            /// </summary>
            /// <remarks>
            /// {0} is replaced with the current version
            /// </remarks>
            public static string full =
            @"
img2pdf {0}, creates a pdf from all images found in folders & subfolders with minimal formatting
Copyright (c) 2014 kt@katbyte.me @ " + homepage + @"

create multiple pdfs:
    img2pdf [OPTION] folder1 folder2 ...
        ==> ./folder1.pdf ./folder2.pdf ...

create single pdf:
    img2pdf [OPTION] folder1 folder2 file1 file2 ... [path/]all_in_one.pdf
        ==> path/all_in_one.pdf

create pdf from files or folders on stdin
    (ls|dir) | img2pdf  [path/]all_in_one.pdf
        ==> path/all_in_one.pdf

options:
    -o path     saves all pdfs to this folder or single file (path/output.pdf)
    -t          top level only, disables searching sub folders
    -s          limits page size to the smallest image and shrink all larger images (proportions constrained)
    -l          limits page size to the largest  image and place  all smaller images in the middle
";

        }

    }
}