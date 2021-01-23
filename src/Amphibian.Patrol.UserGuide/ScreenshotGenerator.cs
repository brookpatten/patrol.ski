using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.IO;
using System.Drawing;

namespace Amphibian.Patrol.UserGuide
{
    public static class ScreenshotGenerator
    {
        public static void WaitUntilNoLoadingDivs(this IWebDriver driver, string selector)
        {
            while (driver.FindElements(By.CssSelector(selector)).Count > 0)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
        public static string Generate(string name,string baseUrl,string outputPath,string imagePath, string markdown,string loginUrl,string loadingDivSelector= ".loading-container")
        {
            List<string> screenshotTags = new List<string>();
            IWebDriver driver=null;
            int index = 0;

            for (var i = 0; i < markdown.Length; i++)
            {
                i = markdown.IndexOf("screenshot({",i);
                if (i > 0)
                {
                    var start = markdown.IndexOf("screenshot({", i);
                    var jsonStart = start + 11;
                    var tagEnd = markdown.IndexOf("})", start)+2;
                    var screenshotTag = markdown.Substring(start, tagEnd - start);
                    var jsonEnd = markdown.IndexOf("}", start) + 1;
                    var json = markdown.Substring(jsonStart, jsonEnd - jsonStart);

                    var args = JsonConvert.DeserializeObject<ScreenShotArgs>(json);

                    index++;
                    var outputUrl = imagePath+"/"+ name+"-"+index + ".jpg";
                    var outputFile = Path.Combine(outputPath, imagePath,name+"-"+index+".jpg");

                    if (driver == null)
                    {
                        var options = new ChromeOptions();
                        options.AddArgument("--allow-insecure-localhost");
                        options.AddArgument("--window-size=1200,1200");
                        driver = new ChromeDriver(options);
                    }

                    if(args.Login)
                    {
                        driver.Navigate().GoToUrl(baseUrl + loginUrl);
                        driver.WaitUntilNoLoadingDivs(loadingDivSelector);
                    }
                    driver.Navigate().GoToUrl(baseUrl + args.Url);
                    driver.WaitUntilNoLoadingDivs(loadingDivSelector);

                    //extra wait... because reasons
                    System.Threading.Thread.Sleep(1000);

                    var image = driver.TakeImage();

                    if (args.Circle && !string.IsNullOrEmpty(args.Element))
                    {
                        var element = driver.FindElement(By.CssSelector(args.Element));
                        image.WithGraphics(g =>
                        {
                            g.CircleElement(element, Color.Red, 5);
                        });
                    }

                    if(args.Width.HasValue && args.Height.HasValue && !string.IsNullOrEmpty(args.Element))
                    {
                        var element = driver.FindElement(By.CssSelector(args.Element));
                        image = image.CropAroundElement(element, args.Width.Value, args.Height.Value);
                    }
                    if (!string.IsNullOrEmpty(args.Element))
                    {
                        var element = driver.FindElement(By.CssSelector(args.Element));
                        image = image.CropAroundElement(element);
                    }

                    image.SaveAsFile(outputFile);


                    markdown = markdown.Replace(screenshotTag, outputUrl);
                    i = i + outputUrl.Length;
                }
                else
                {
                    i = markdown.Length;
                }
            }

            if(driver!=null)
            {
                driver.Close();
                driver.Dispose();
            }

            return markdown;
        }

        public class ScreenShotArgs
        {
            public string Url { get; set; }
            public bool Login { get; set; }
            public string Element { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
            public bool Circle { get; set; }

            public ScreenShotArgs()
            {
                Url = "";
                Login = true;
            }
        }
    }
}
