using System;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace site.classesHelp
{
    /// <summary>класс возвращает картинку с набором символов(CAPTCHA) для проверки того, что на сайте не бот
    /// функция getCaptImg(string pathToTempCaptFile) - возвращает картинку. В переменную pathToTempCaptFile передаётся путь в папку, в которую будет записан
    /// файл с картинкой с символами. Путь должен вести к папке /files/temp
    /// </summary>
    public class CaptchaClass
    {
        private string captchaTxt;

        public CaptchaClass() { }

        /// <summary>функция возвращает картинку CAPTCHA, для проверки на бота</summary>
        /// <param name="pathToTempCaptFile"></param>
        /// <returns></returns>
        public System.Web.UI.WebControls.Image GetCaptImg(string pathToTempCaptFile)
        {
            var imgCaptcha = new System.Web.UI.WebControls.Image();

            #region ОСНОВНОЙ КОД

            int height = 30;
            int width = 100;
            var bmp = new Bitmap(width, height);
            var rectf = new RectangleF(10, 5, 0, 0);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var random = new Random();
            string combination = "0123456789ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            var captchaTemp = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                captchaTemp.Append(combination[random.Next(combination.Length)]);
            }
            captchaTxt = captchaTemp.ToString();

            var p = new Page();
            p.Session["captchaTxtReal"] = captchaTxt;   //инициализация(первая) переменной, которая будет хранить строку символов, выводимую на картинку

            g.DrawString(captchaTxt, new Font("Thaoma", 12, FontStyle.Italic), Brushes.Green, rectf);
            g.DrawRectangle(new Pen(Color.Green), 1, 1, width - 2, height - 2);
            g.Flush();
            try
            {
                bmp.Save(pathToTempCaptFile + "captcha.jpg", ImageFormat.Jpeg);
            }
            catch { }

            g.Dispose();
            bmp.Dispose();

            imgCaptcha.ImageUrl = "~/files/temp/captcha.jpg";

            #endregion

            return imgCaptcha;
        }
    }
}