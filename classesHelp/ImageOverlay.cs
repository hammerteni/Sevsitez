using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace site.classesHelp
{
    /* Пример использования класса
      string pathToRoot = HttpContext.Current.Server.MapPath("~");
      var newImage = ImageOverlay(new Bitmap(pathToRoot + @"images/исходное.jpg"), new Bitmap(pathToRoot + @"images/наложение_большегоразрешения.png"));
      newImage.Save(pathToRoot + @"imagesResult\jpg_bolshesize.jpg", ImageFormat.Jpeg);
      newImage.Dispose();
     */

    #region Класс ImageOverlay

    /// <summary>Класс содержит методы для обработки изображений</summary>
    public class ImageOverlay
    {
        #region Конструктор

        public ImageOverlay(){}

        #endregion
        #region Метод Overlay(...)

        /// <summary>Функция для наложения изображения на изображение</summary>
        /// <param name="image">картинка, на которую накладывается</param>
        /// <param name="image2">картинка, которая накладывается</param>
        /// <returns></returns>
        public Bitmap Overlay(Bitmap image, Bitmap image2)
        {
            //подгонка высоты и ширины накладываемого изображения по размеру изображения, на которое оно накладывается
            var fonImage = ResizeImage(image2, image.Width, image.Height);

            var bmp = new Bitmap(image.Width, image.Height);
            Color im, fon;
            int iR, iG, iB;
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    fon = fonImage.GetPixel(i, j);
                    im = image.GetPixel(i, j);
                    iR = fon.R * (255 - im.A) / 255 + im.R * im.A / 255;
                    iG = fon.G * (255 - im.A) / 255 + im.G * im.A / 255;
                    iB = fon.B * (255 - im.A) / 255 + im.B * im.A / 255;
                    bmp.SetPixel(i, j, Color.FromArgb(Math.Max(fon.A, im.A), iR * (255 - fon.A) / 255 + fon.R * fon.A / 255, iG * (255 - fon.A) / 255 + fon.G * fon.A / 255, iB * (255 - fon.A) / 255 + fon.B * fon.A / 255));
                }
            return bmp;
        }

        #endregion
        #region Метод ResizeImage(...)

        /// <summary>Функция изменения размера изображения на нужный без потери качества.</summary>
        /// <param name="image">исходное изображения</param>
        /// <param name="width">ширина, к которой нужно привести изображение</param>
        /// <param name="height">высота, к которой нужно привести изображение</param>
        /// <returns>возвращается объект изображения к изменённым размером</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        #endregion
        #region Метод GetImageSize(...)

        /// <summary>Функция возвращает массив, который включает элементы: 0-й - реальная ширина картинки; 1-й - реальная высота картинки. 
        /// Функция возвращает один элемент со значением -1, если по заданному пути картинки не нашлось.</summary>
        /// <param name="pathToFile">путь к файлу картинки</param>
        /// <returns>возвращает массив, который включает элементы: 0-й - реальная ширина картинки; 1-й - реальная высота картинки или -1 в случае, если по указанному пути не нашлось файла</returns>
        public static string[] GetImageSize(string pathToFile)
        {
            string[] result = new[] { "-1" };
            System.Drawing.Image objImage = null;
            try
            {
                objImage = System.Drawing.Image.FromFile(pathToFile);
                result = new[] { objImage.Width.ToString(), objImage.Height.ToString() };
            }
            catch
            {
                result = new[] { "-1" };
            }
            finally {
                if (objImage != null) {
                    objImage.Dispose();
                    objImage = null;
                }
            }

            return result;
        }

        #endregion
        #region Метод ResizeImageToNeeded(int height)

        /// <summary>Метод преобразует изображение к нужной ширине или высоте с пережатием или без такового</summary>
        /// <param name="pathToImg">путь к изображению</param>
        /// <param name="cifir">нужная ширина или высота выходного изображения</param>
        /// <param name="isHeight">true - преобразовать по высоте, переданной в cifir, false - преобразовать по ширине</param>
        /// <returns></returns>
        public Bitmap ResizeImageToNeed(string pathToImg, int cifir, bool isHeight = true, long compession = -1, ImageTypesMy imgType = ImageTypesMy.JPG)
        {
            Image sourceImg = Image.FromFile(pathToImg);
            int width = sourceImg.Width / 2, height = sourceImg.Height / 2;

            if (isHeight)   //если преобразовать по высоте, то..
            {
                width = (width * cifir) / height;
                height = cifir;
            }
            else            //если преобразовать по ширине, то..
            {
                height = (height * cifir) / width;
                width = cifir;
            }

            Bitmap result = ResizeImage(sourceImg, width, height);
            sourceImg.Dispose();

            //Пережатие изображения
            if (compession > -1)
            {
                result = CompressBitmap(result, imgType, compession);
            }

            return result;
        }

        #endregion

        #region Метод CompressBitmap(Bitmap sourceBitmap, ImageTypesMy imgType)

        /// <summary>Метод пережимает изображение и возвращает его в нужном формате</summary>
        /// <param name="sourceBitmap">исходное изображение</param>
        /// <param name="imgType">тип изображения к которому нужно привести выходное</param>
        /// <returns></returns>
        public Bitmap CompressBitmap(Bitmap sourceBitmap, ImageTypesMy imgType, long compession)
        {
            #region Код

            //Получаем кодировщик
            ImageCodecInfo codec;
            if (imgType == ImageTypesMy.JPG)
            {
                codec = GetEncoder(ImageFormat.Jpeg);
            }
            else if (imgType == ImageTypesMy.JPEG)
            {
                codec = GetEncoder(ImageFormat.Jpeg);
            }
            else if (imgType == ImageTypesMy.PNG)
            {
                codec = GetEncoder(ImageFormat.Png);
            }
            else if (imgType == ImageTypesMy.TIFF)
            {
                codec = GetEncoder(ImageFormat.Tiff);
            }
            else
            {
                codec = GetEncoder(ImageFormat.Jpeg);
            }

            //Формируем параметры кодирования
            Encoder _encoder = Encoder.Quality;
            EncoderParameters _encoderParams = new EncoderParameters(1);
            EncoderParameter _encoderParam = new EncoderParameter(_encoder, compession);
            _encoderParams.Param[0] = _encoderParam;

            MemoryStream stream = new MemoryStream();
            sourceBitmap.Save(stream, codec, _encoderParams);
            Bitmap result = new Bitmap(stream);

            #endregion

            return result;
        }

        public bool ResizeImageNew(string sourceFile, string destFile, ImageFormat format, int percent)
        {
            try
            {
                using (Image img = Image.FromFile(sourceFile))
                {
                    int width = (int)(img.Width * (percent * .01));
                    int height = (int)(img.Height * (percent * .01));
                    Image thumbNail = new Bitmap(width, height, img.PixelFormat);
                    Graphics g = Graphics.FromImage(thumbNail);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new Rectangle(0, 0, width, height);
                    g.DrawImage(img, rect);
                    thumbNail.Save(destFile, format);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Bitmap FixedSize(string sourceFile, int percent)
        {
            using (Image imgPhoto = Image.FromFile(sourceFile))
            {
                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                int Width = (int)(sourceWidth * (percent * .01));
                int Height = (int)(sourceHeight * (percent * .01));

                nPercentW = ((float)Width / (float)sourceWidth);
                nPercentH = ((float)Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((Width -
                                  (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = System.Convert.ToInt16((Height -
                                  (sourceHeight * nPercent)) / 2);
                }

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap bmPhoto = new Bitmap(Width, Height,
                                  PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                 imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(Color.Red);
                grPhoto.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();
                return bmPhoto;
            }
        }

        #endregion
        #region Метод GetEncoder(ImageFormat format)

        /// <summary>Метод возвращает объект ImageCodecInfo. Метод является вспомогательным для метода CompressBitmap</summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        #endregion
    }

    #endregion
}