//Copyright © 2015 kt@katbyte.me
using System;
using System.IO;



namespace katbyte.img2pdf {


    /// <summary>
    /// interface for and object that understands OutputDirectory Settings
    /// </summary>
    public interface IImg2PdfOptions {


        /// <summary>
        /// if null, use input location, otherwise the path to save too
        /// </summary>
        string outputDirectory { get; set; }


        /// <summary>
        /// true to use the smallest image as the page size
        /// </summary>
        bool ensmallen { get; set; }


        /// <summary>
        /// true to use the largest image as the page size
        /// </summary>
        bool embiggen { get; set; }

    }


    /// <summary>
    /// container class for IImg2PdfOptions extensions
    /// </summary>
    public static class Extends_IImg2PdfOptions {

        /// <summary>
        /// validates options, throws exception with details if there is a problem
        /// </summary>
        public static void Validate(this IImg2PdfOptions options) {
            //can't do both... or can we ;)
            if (options.ensmallen && options.embiggen) {
                throw new Exception("can not use switches -s and -l at the same time");
            }

            if (options.outputDirectory != null && ! Directory.Exists(options.outputDirectory)) {
                throw new Exception("Output directory '" + options.outputDirectory + "' does not exist.");
            }
        }
    }

}