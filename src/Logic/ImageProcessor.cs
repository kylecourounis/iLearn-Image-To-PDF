namespace ImageToPDF.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using PdfSharp.Drawing;
    using PdfSharp.Pdf;

    internal class ImageProcessor
    {
        /// <summary>
        /// Creates the <see cref="PdfDocument"/> from a <see cref="List{string}"/> of Base64 images.
        /// </summary>
        internal static PdfDocument CreatePDF(List<string> imagesAsBase64)
        {
            var document = new PdfDocument();

            Console.WriteLine("Compiling...");

            foreach (var image in imagesAsBase64)
            {
                var base64 = image.Split(',').Last();

                using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    var page = new PdfPage(document);

                    var graphics = XGraphics.FromPdfPage(page);
                    graphics.DrawImage(XImage.FromStream(stream), 0, 0, 600, 800);

                    document.AddPage(page);
                }
            }

            return document;
        }

        /// <summary>
        /// Creates the <see cref="PdfDocument"/> from a <see cref="List{PdfDocument}"/> of PdfDocuments.
        /// </summary>
        internal static PdfDocument CreatePDF(List<PdfDocument> pdfDocuments)
        {
            var document = new PdfDocument();

            Console.WriteLine("Compiling...");

            foreach (var pdf in pdfDocuments)
            {
                document.AddPage(pdf.Pages[0]);
            }

            return document;
        }
    }
}
