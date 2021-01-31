using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Reflection;
using System.Drawing;
using System.IO;

namespace Amphibian.Patrol.UserGuide
{
    public static class ScreenshotExtensions
    {
        public static Screenshot TakeScreenShot(this IWebDriver driver)
        {
            return ((ITakesScreenshot)driver).GetScreenshot();
        }

        public static Image TakeImage(this IWebDriver driver)
        {
            var shot = driver.TakeScreenShot();
            using (var stream = new MemoryStream(shot.AsByteArray))
            {
                return Bitmap.FromStream(stream);
            }
        }

        public static Image WithGraphics(this Image img, Action<Graphics> graphics)
        {
            using (var g = Graphics.FromImage(img))
            {
                graphics(g);
                g.Flush();
            }
            return img;
        }

        public static Image CropAroundElement(this Image img, IWebElement element, int? outputWidth=null, int? outputHeight=null)
        {
            //Assert.GreaterOrEqual(img.Width, outputWidth);
            //Assert.GreaterOrEqual(img.Height, outputHeight);

            int elementCenterX = element.Location.X + (element.Size.Width / 2);
            int elementCenterY = element.Location.Y + (element.Size.Height / 2);

            if (!outputWidth.HasValue)
            {
                outputWidth = element.Size.Width;

                //if no width was specified, but height was specified, assume only the top of the element is desired
                if(outputHeight.HasValue)
                {
                    elementCenterY = elementCenterY + (outputHeight.Value / 2);
                }
            }
            
            if (!outputHeight.HasValue)
            {
                outputHeight = element.Size.Height;

                //if no height was specified, but width was specified, assume only the left of the element is desired
                if (outputWidth.HasValue)
                {
                    elementCenterX = elementCenterX + (outputWidth.Value / 2);
                }
            }


            //find top left and bottom right coords in source image
            int srcXTL = elementCenterX - (outputWidth.Value / 2);
            int srcYTL = elementCenterY - (outputHeight.Value / 2);

            if (srcXTL < 0)
            {
                srcXTL = 0;
            }
            if (srcYTL < 0)
            {
                srcYTL = 0;
            }
            if (srcXTL + outputWidth > img.Width)
            {
                srcXTL = img.Width - outputWidth.Value;
            }
            if (srcYTL + outputHeight > img.Height)
            {
                srcXTL = img.Height - outputHeight.Value;
            }

            var buffer = new Bitmap(outputWidth.Value, outputHeight.Value);

            buffer.WithGraphics(g =>
            {
                g.DrawImage(img
                    , new Rectangle(0, 0, outputWidth.Value, outputHeight.Value)
                    , new Rectangle(srcXTL, srcYTL, outputWidth.Value, outputHeight.Value)
                    , GraphicsUnit.Pixel);
            });

            return buffer;
        }

        public static void SaveAsFile(this Image img, string path)
        {
            img.Save(path);
        }

        public static void CircleElement(this Graphics g, IWebElement element, Color color, float width, int margin = 0)
        {
            var pen = new Pen(new SolidBrush(color), width);
            g.DrawEllipse(pen, element.Location.X - margin, element.Location.Y - margin, element.Size.Width + margin * 2, element.Size.Height + margin * 2);
        }

        public static void CircleAndScreenshotElement(this IWebDriver driver, string url, string imagePath, By element, Color circleColor, int? width = null, int? height = null, float circleThickness = 5f, int circleMargin = 10)
        {
            driver.Navigate().GoToUrl(url);
            var theElement = driver.FindElement(element);
            driver.TakeImage()
                .WithGraphics(g =>
                {
                    g.CircleElement(theElement, circleColor, circleThickness, circleMargin);
                })
                .CropAroundElement(theElement, width, height)
                .SaveAsFile(imagePath);
        }
    }
}
