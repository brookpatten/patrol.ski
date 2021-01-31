using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using System;
using System.IO;

namespace Amphibian.Patrol.UserGuide
{
    /// <summary>
    /// This builds the html user guide by generating screenshots and compiling markdown to html
    /// must be pointed at a functional app instance url for it to work generating screenshots
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            string input = "../../../src";
            string output = "../../../dist";
            string image = "img";

            string outputFormat ="html";//or "md", "pdf";

            string imageOutput = Path.Combine(output, image);

            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
            Directory.CreateDirectory(output);
            Directory.CreateDirectory(imageOutput);

            var files = Directory.GetFiles(input, "*.md");
            foreach(var file in files)
            {
                var info = new FileInfo(file);

                var md = File.ReadAllText(file);

                //hack, should do this with markdig extension
                md = ScreenshotGenerator.Generate(info.Name.Replace(".md",""), "http://localhost:8080/", output,image, md, "#/test-drive");

                if (outputFormat == "html" || outputFormat=="pdf")
                {
                    var html = Markdown.ToHtml(md, pipeline: pipeline);

                    //total bodge, should do this with a markdig extension
                    html = html.Replace(".md", ".html");

                    if(outputFormat=="pdf")
                    {
                        //TODO: this doesn't work yet
                        //NOTE: to converto to pdf you need to have wkhtmltox in the bin
                        //https://wkhtmltopdf.org/downloads.html
                        var bin = new WkHtmlToPdf.WkHtmlToPdfBinary();
                        
                        var htmlToPdf = new WkHtmlToPdf.PdfGenerator(bin);
                        
                        var bytes = htmlToPdf.Generate(html).Result;
                        File.WriteAllBytes(Path.Combine(output, info.Name.Replace(".md", ".pdf")), bytes);
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(output, info.Name.Replace(".md", ".html")), html);
                    }
                }
                else if(outputFormat == "md")
                {
                    File.WriteAllText(Path.Combine(output, info.Name), md);
                }
            }
        }
    }
}
