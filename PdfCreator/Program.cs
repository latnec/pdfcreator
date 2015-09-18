using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.IO;
using PdfSharp.Drawing.Layout;
using System.Text.RegularExpressions;

namespace PdfCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length<1 )
            {
                Console.WriteLine("Please provide the path to the folder.\nPress Return to end this...");
                Console.ReadLine();
                return;
            }
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Title";

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            

            var theText = GetAllText(args[0]);

            string theTextString = theText.ToString();

            var textLines = theTextString.Split('\n');
            
            int numLine = textLines.Length;
            //Console.WriteLine(numLine);
            int linesPerPage = 60;
            int numOfEmptyLine = numLine % linesPerPage;


            int k = 0;
            for (int i = 0; i < numLine-linesPerPage; i += linesPerPage)
            {
                // Create an empty page
                PdfPage page = document.AddPage();


                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XTextFormatter tf = new XTextFormatter(gfx);

                tf.Alignment = XParagraphAlignment.Left;

                //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

                // Create a font
                XFont font = new XFont("Times New Roman", 10, XFontStyle.Regular, options);

                //Console.WriteLine(i);

                string bal = string.Concat(Enumerable.Range(i, linesPerPage).Select(r => string.Format("{0} ", textLines[r])));

                // Draw the text
                tf.DrawString(bal, font, XBrushes.Black,
                  new XRect(30, 30, page.Width-30, page.Height-30),
                  XStringFormats.TopLeft);

                k = i;
            }
            k += linesPerPage;
            PdfPage pagef = document.AddPage();


            // Get an XGraphics object for drawing
            XGraphics gfxf = XGraphics.FromPdfPage(pagef);

            XTextFormatter tff = new XTextFormatter(gfxf);

            tff.Alignment = XParagraphAlignment.Left;

            //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            // Create a font
            XFont fontf = new XFont("Times New Roman", 10, XFontStyle.Regular, options);


            string balf = string.Concat(Enumerable.Range(k, numLine-k -1).Select(l => string.Format("{0} ", textLines[l])));

            // Draw the text
            tff.DrawString(balf, fontf, XBrushes.Black,
              new XRect(30, 30, pagef.Width - 30, pagef.Height - 30),
              XStringFormats.TopLeft);


            // Save the document...
            const string filename = "Name.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);


        }

        static StringBuilder GetAllText(string pathOfFolder)
        {
            //var allTextFiles = Directory.GetFiles(pathOfFolder, "*", SearchOption.AllDirectories);
            var allTextFiles = Directory.EnumerateFiles(pathOfFolder,"*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".txt"));
            StringBuilder sb = new StringBuilder();

            foreach (var fl in allTextFiles)
            {
                string readText = GetTextFromFile(fl);// File.ReadAllText(fl);
                sb.AppendLine(String.Format("{0}:",fl.ToString()));
                sb.AppendLine(Environment.NewLine);
                sb.Append(readText);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("--------------------- // ----------------------");

            }
            return sb;
        }

        static string GetTextFromFile(string fileLocation)
        {
            //V1
            //return File.ReadAllText(fileLocation);

            //V2 -- fancy
            StringBuilder sbl = new StringBuilder();

            int maxLineLenght = 70;


            using (StreamReader srl = new StreamReader(fileLocation))
            {
                string line;
                while ((line = srl.ReadLine()) != null)
                {
                    //var blabla = Enumerable.Range(0, line.Length / maxLineLenght).Select(i => line.Substring(i * maxLineLenght, maxLineLenght));
                    //var blabla = (from Match m in Regex.Matches(line, @"{.1,70}") select m.Value).ToList();
                    var blabla = Regex.Split(line, @"(?<=\G.{100})", RegexOptions.Singleline);
                    foreach (var ll in blabla)
                    {
                        sbl.AppendLine(ll);
                    }
                }
            }

            return sbl.ToString();
        }
    }
}
