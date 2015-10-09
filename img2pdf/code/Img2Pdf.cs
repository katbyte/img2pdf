//Copyright © 2014 kt@katbyte.me
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using iTextSharp.text;
using iTextSharp.text.pdf;

using katbyte.extend;
using katbyte.data;



namespace katbyte.img2pdf {

    /// <summary>
    /// Img2Pdf converter
    /// </summary>
    public static class Img2Pdf {

        private static string appname = "img2pdf";
        private static string homepage = "https://github.com/katbyte/cs.img2pdf";

        /// <summary>
        /// creates a PDF at path for the given files, for each file callback(success, file, message, Image) will be called
        /// </summary>
        public static void CreatePdfFromFiles(string path, IEnumerable<string> filese, Action<bool, string, string, Image> callback = null, bool ensmallen = false, bool embiggen = false) {

            var files = filese.ToArray();

            //sanity checks
            //can't do both... or can we ;)
            if (ensmallen && embiggen) {
                throw new Exception("can not use switches -s and -l at the same time");
            }


            //if no callback use an empty one
            callback = callback ?? ( (b, s1, s2, i) => { } );


            //create doc
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
                    float rp = (float) psize.w / (float) psize.h;
                    float ri = i.Width / i.Height;

                    float iw = rp > ri ? i.Width * psize.h / i.Height : psize.w;
                    float ih = rp > ri ? psize.h : i.Height * psize.w / i.Width;
                    i.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                    i.SetAbsolutePosition(( psize.w - iw ) / 2, ( psize.h - ih ) / 2);
                }

                i.Alt = Path.GetFileName(file);
                doc.Add(i);

                callback(true, file, null, i);
            }

            doc.Close();
        }

    }

}