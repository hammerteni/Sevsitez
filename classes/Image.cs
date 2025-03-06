using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using System.Text;
using System.Web;
using site.classesHelp;

/* файл с классом для работы с картинками. Возвращает готовый html-код картинки нужной ширины, с тенью или без, с возможность увеличения или без такой возможности, по заданному пути */
namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>класс, который формирует html-код, jquery-код и стили для картинки</summary>
    public class ImageForm
    {
        private Page _pag;

        //Конструктор
        public ImageForm(Page newPag) { _pag = newPag; }

        #region Метод GetImage(...)

        /// <summary>Функция возвращает готовый html-код картинки нужной ширины, с тенью или без, с возможность увеличения или без, по заданному пути.</summary>
        /// <param name="pathToImg">путь к файлу (например - ../img/img.jpg)</param>
        /// <param name="width">ширина картинки в пикселях (если не нужно выставлять ширину, то оставить значение пустым)</param>
        /// <param name="height">высота картинки в пикселях (если не нужно выставлять высоту, то оставить значение пустым)</param>
        /// <param name="shadowOnOff">включение/выключение тени у картинки (может быть 'on' либо 'off')</param>
        /// <param name="increaseOnOff">включение/выключение возможности просмотра полноразмерного варианта картинки (может быть 'on' либо 'off')</param>
        /// <param name="alt">значение параметра alt в теге img</param>
        /// <param name="title">значение параметра title в теге img</param>
        /// <param name="uniqImgNum">уникальная цифра для этой картинки</param>
        /// <returns></returns>
        public LiteralControl GetImage(string width, string height, string shadowOnOff, string increaseOnOff, string pathToImg, string alt, string title, string uniqImgNum)
        {
            #region Код получения пути к уменьшенной версии изображения

            string[] imgSizes = null;
            if ((width != "" && height == "") || (width == "" && height != ""))
            {
                imgSizes = ImageOverlay.GetImageSize(HttpContext.Current.Server.MapPath("~") + pathToImg.Replace("../", "").Replace("~/", ""));
                //картинки нет
                if (imgSizes[0] == "-1") return null;

                if (int.Parse(imgSizes[0]) > int.Parse(imgSizes[1]))
                    width = "785";
                else
                    width = "540";
            }
            if (imgSizes == null) imgSizes = ImageOverlay.GetImageSize(HttpContext.Current.Server.MapPath("~") + pathToImg.Replace("../", "").Replace("~/", ""));

            string pathToLittleImg = pathToImg;
            pathToLittleImg = GetPathToLittleFoto(pathToImg, width, height);
            if (pathToLittleImg == "err") 
                return new LiteralControl("Не удалось добавить фото по причине ошибки во время создания уменьшенного фото..");

            #endregion

            var htmlString = new StringBuilder();

            #region Формирование HTML-кода

            if (shadowOnOff == "on")
            {
                if (width != "") { width = "width: " + width + "px; "; }    //формируем CSS код фиксированный ширины для добавления к картинке  
                if (height != "") { height = "height: " + height + "px; "; }      //формируем CSS код фиксированный высоты для добавления к картинке
                if (increaseOnOff == "on")
                {
                    htmlString.Append("<img id='img_as_" + uniqImgNum + "' src='../../" + pathToLittleImg + "?dt=" + DateTime.Now.Ticks.ToString() + "' class='imageStyle imageShadow' style='" + width + height + "cursor: pointer;' alt='" + alt + "' loading='lazy' title='" + title + "' />");
                }
                if (increaseOnOff == "off")
                {
                    htmlString.Append("<img id='img_as_" + uniqImgNum + "' src='../../" + pathToLittleImg + "?dt=" + DateTime.Now.Ticks.ToString() + "' class='imageStyle imageShadow' style='" + width + height + "cursor: default;' alt='" + alt + "' loading='lazy' title='" + title + "' />");
                }
            }
            if (shadowOnOff == "off")
            {
                if (width != "") { width = "width: " + width + "px; "; }    //формируем CSS код фиксированный ширины для добавления к картинке  
                if (height != "") { height = "height: " + height + "px; "; }      //формируем CSS код фиксированный высоты для добавления к картинке
                if (increaseOnOff == "on")
                {
                    htmlString.Append("<img id='img_as_" + uniqImgNum + "' src='../../" + pathToLittleImg + "?dt=" + DateTime.Now.Ticks.ToString() + "' class='imageStyle' style='" + width + height + "cursor: pointer;' alt='" + alt + "' loading='lazy' title='" + title + "' />");
                }
                if (increaseOnOff == "off")
                {
                    htmlString.Append("<img id='img_as_" + uniqImgNum + "' src='../../" + pathToLittleImg + "?dt=" + DateTime.Now.Ticks.ToString() + "' class='imageStyle' style='" + width + height + "cursor: default;' alt='" + alt + "' loading='lazy' title='" + title + "' />");
                }
            }

            /*Для картинки используются классы CSS, описанные в файле txtpages.css
             Вот их код:
             .imageStyle {
                position: relative; display: inline-block;
                margin: 16px;
             }
            .imageShadow {
                -webkit-box-shadow: 4px 4px 16px #000000;
                -webkit-box-shadow: 4px 4px 16px rgba(0,0,0,.9);
                -ms-box-shadow: 4px 4px 16px #000000;
                -ms-box-shadow: 4px 4px 16px rgba(0,0,0,.9);
                box-shadow: 4px 4px 16px #000000;
                box-shadow: 4px 4px 16px rgba(0,0,0,.9);
            }
             */

            #endregion

            var jQueryString = new StringBuilder();

            #region Формирование jQuery-кода и добавление его в конец страницы

            if (increaseOnOff == "on")
            {
                jQueryString.Append("<noindex><script type='text/javascript'>");
                jQueryString.Append("$(document).ready(function () { ");

                jQueryString.Append("var $btnIncrease" + uniqImgNum + " = $('#img_as_" + uniqImgNum + "'); ");

                jQueryString.Append("$btnIncrease" + uniqImgNum + ".bind('click', function () { ");
                jQueryString.Append("increaseSlide" + uniqImgNum + "(); ");
                jQueryString.Append("}); ");

                jQueryString.Append("function increaseSlide" + uniqImgNum + "() { ");
                //if (pathToImg.Contains("~/"))
                //{
                //    pathToImg = pathToImg.Replace("~/", "../");
                //}
                jQueryString.Append("var pathToImg = '../../" + pathToLittleImg + "'; ");
                jQueryString.Append("var htmlString = \"<table id='imgOpacTbl" + uniqImgNum + "'><tr><td><div id='divImgWrapOpac" + uniqImgNum + "'><img class='imgOpacAppearing' id='img_asopac_" + uniqImgNum + "' src='\" + pathToImg + ");
                #region Определение размеров оригинального изображения

                #endregion
                jQueryString.Append("\"' alt='Image' style='width:" + imgSizes[0] + "px;height:" + imgSizes[1] + "px;' /><img id='btnImgClose" + uniqImgNum + "' src='../../../files/slider/btnClose.png' alt='Close' /></div></td></tr></table>\"; ");
                jQueryString.Append("$('body').prepend(htmlString); ");
                jQueryString.Append("var $tbl = $('#imgOpacTbl" + uniqImgNum + "'); ");
                jQueryString.Append("$tbl.css('opacity', '0'); ");
                jQueryString.Append("var $btnClose = $('#btnImgClose" + uniqImgNum + "'); ");
                jQueryString.Append("$btnClose.hide(); ");
                jQueryString.Append("var $img = $('#img_asopac_" + uniqImgNum + "'); ");
                jQueryString.Append("var imgRealWidth = $img.width(); ");
                jQueryString.Append("var imgRealHeight = $img.height(); ");

                //Подгоним реальный размер фото под размер окна браузера, если размер фото больше
                jQueryString.Append("var kw = imgRealWidth / imgRealHeight; ");
                jQueryString.Append("var kh = imgRealHeight / imgRealWidth; ");
                jQueryString.Append("var windowWidth = $(window).width() - 40; ");
                jQueryString.Append("var windowHeight = $(window).height() - 40; ");
                jQueryString.Append("if (imgRealWidth >= windowWidth && imgRealHeight <= windowHeight) ");       //подгонка по ширине
                jQueryString.Append("{ ");
                jQueryString.Append(" imgRealWidth = windowWidth; ");
                jQueryString.Append(" imgRealHeight = imgRealWidth / kw; ");
                jQueryString.Append("} ");
                jQueryString.Append("else if (imgRealHeight >= windowHeight && imgRealWidth <= windowWidth) ");       //подгонка по высоте
                jQueryString.Append("{ ");
                jQueryString.Append(" imgRealHeight = windowHeight; ");
                jQueryString.Append(" imgRealWidth = imgRealHeight / kh; ");
                jQueryString.Append("} ");
                jQueryString.Append("else if (imgRealHeight >= windowHeight && imgRealWidth >= windowWidth) ");       //подгонка по высоте и ширине
                jQueryString.Append("{ ");
                jQueryString.Append(" if(imgRealHeight >= imgRealWidth) ");     //высота
                jQueryString.Append(" { ");
                jQueryString.Append("  imgRealHeight = windowHeight; ");
                jQueryString.Append("  imgRealWidth = imgRealHeight / kh; ");
                jQueryString.Append("  if(imgRealWidth > windowWidth) ");       //окончательная подгонка
                jQueryString.Append("  { ");
                jQueryString.Append("   imgRealWidth = windowWidth; ");
                jQueryString.Append("   imgRealHeight = imgRealWidth / kw; ");
                jQueryString.Append("  } ");
                jQueryString.Append(" } ");
                jQueryString.Append(" else if(imgRealWidth >= imgRealHeight) ");    //ширина
                jQueryString.Append(" { ");
                jQueryString.Append("  imgRealWidth = windowWidth; ");
                jQueryString.Append("  imgRealHeight = imgRealWidth / kw; ");
                jQueryString.Append("  if(imgRealHeight > windowHeight) ");       //окончательная подгонка
                jQueryString.Append("  { ");
                jQueryString.Append("   imgRealHeight = windowHeight; ");
                jQueryString.Append("   imgRealWidth = imgRealHeight / kh; ");
                jQueryString.Append("  } ");
                jQueryString.Append(" } ");
                jQueryString.Append("} ");

                jQueryString.Append("$img.width(imgRealWidth / 2); ");
                jQueryString.Append("$img.height(imgRealHeight / 2); ");
                jQueryString.Append("var $imgWrap = $('#divImgWrapOpac" + uniqImgNum + "'); ");
                jQueryString.Append("$imgWrap.css('opacity', '0'); ");
                jQueryString.Append("$tbl.animate({ ");
                jQueryString.Append("opacity: 1 ");
                jQueryString.Append("}, 200); ");
                jQueryString.Append("$imgWrap.css('opacity', '1'); ");
                jQueryString.Append("$img.animate({ ");
                jQueryString.Append("width: imgRealWidth + 'px', ");
                jQueryString.Append("height: imgRealHeight + 'px' ");
                jQueryString.Append("}, 200, function () { ");
                jQueryString.Append("$btnClose.show().offset({ top: $imgWrap.offset().top, left: $imgWrap.offset().left + $imgWrap.outerWidth(true) - $btnClose.width() }); ");
                jQueryString.Append("$btnClose.bind('click', function () { ");
                jQueryString.Append("opacClose" + uniqImgNum + "(); ");
                jQueryString.Append("}); ");

                jQueryString.Append("$tbl.bind('click', function (e) { ");
                jQueryString.Append("opacClose" + uniqImgNum + "(e); ");
                jQueryString.Append("}); ");

                jQueryString.Append("}); ");

                jQueryString.Append("function opacClose" + uniqImgNum + "(e) { ");

                jQueryString.Append("var e = e || event; ");
                jQueryString.Append("console.log(e); ");
                jQueryString.Append("try { e.stopPropagation(); }catch(ex) { e.cancelBubble = true; } ");   //отключение бабблинга (хотя в данном случае оно не совсем уместно..)

                jQueryString.Append("$btnClose.unbind('click'); ");
                jQueryString.Append("$tbl.animate({ ");
                jQueryString.Append("opacity: 0 ");
                jQueryString.Append("}, 200, function () { ");
                jQueryString.Append("$tbl.remove(); ");
                jQueryString.Append("}); ");
                jQueryString.Append("}; ");
                jQueryString.Append("}; ");

                jQueryString.Append("}); ");
                jQueryString.Append("</script></noindex> ");

                _pag.Controls.Add(new LiteralControl(jQueryString.ToString()));
            }

            #endregion

            var cssString = new StringBuilder();

            #region Формирование css-кода и добавление его в шапку страницы

            if (increaseOnOff == "on")
            {
                cssString.Append("<style type='text/css'> ");

                cssString.Append("#divImgWrapOpac" + uniqImgNum + " { ");
                cssString.Append(" position: relative; display: inline-block; ");
                //cssString.Append(" padding: 3px; ");
                //cssString.Append(" border-radius: 3px; ");
                //cssString.Append(" border: 1px #ffffff solid; ");
                cssString.Append(" background-color: #ffffff; ");
                cssString.Append(" -webkit-box-shadow: 0 0 12px #000000; -webkit-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append(" -ms-box-shadow: 0 0 12px #000000; -ms-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append(" box-shadow: 0 0 12px #000000; box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append(" z-index: 667; ");
                cssString.Append("} ");

                cssString.Append("img.imgOpacAppearing");
                cssString.Append("{");
                cssString.Append(" position: relative; display: inline-block;");
                cssString.Append(" vertical-align: top;");
                cssString.Append("}");

                cssString.Append("#imgOpacTbl" + uniqImgNum + " { ");
                cssString.Append(" position: fixed; width: 100%; height: 100%; ");
                cssString.Append(" top:0; left:0; ");
                cssString.Append(" z-index:666; ");
                cssString.Append(" border-collapse: collapse; ");
                cssString.Append(" background-color: #CCC; background-color: rgba(0,0,0,.7); ");
                cssString.Append("} ");

                cssString.Append("#imgOpacTbl" + uniqImgNum + " tr td { ");
                cssString.Append(" text-align: center; vertical-align: middle; ");
                cssString.Append("} ");

                cssString.Append("#btnImgClose" + uniqImgNum + " { ");
                cssString.Append(" position: absolute; display: inline-block; ");
                cssString.Append(" -ms-opacity: 0.5; opacity: 0.5; ");
                cssString.Append(" z-index: 668; ");
                cssString.Append(" cursor: pointer; ");
                cssString.Append(" -webkit-transition: all .3s; ");
                cssString.Append(" -moz-transition: all .3s; ");
                cssString.Append(" -o-transition: all .3s; ");
                cssString.Append(" transition: all .3s; ");
                cssString.Append("} ");

                cssString.Append("#btnImgClose" + uniqImgNum + ":hover { ");
                cssString.Append(" -ms-opacity: 1; opacity: 1; ");
                cssString.Append("} ");

                cssString.Append("#btnImgClose" + uniqImgNum + ":active { ");
                cssString.Append(" -ms-opacity: 0.5; opacity: 0.5; ");
                cssString.Append("} ");

                cssString.Append("</style> ");

                _pag.Header.Controls.Add(new LiteralControl(cssString.ToString()));
            }

            #endregion

            return new LiteralControl(htmlString.ToString());
        }

        #endregion
        #region Метод GetPathToLittleFoto(...)

        /// <summary>Метод возвращает путь к изображению с изменённым размером (уменьшенным). Нельзя передать пустую длину или пустую ширину.</summary>
        /// <param name="pathToBigFoto">путь к изображению оригинального размера</param>
        /// <param name="width">ширина, до которой будет преобразовано изображение (может быть "" - изменить по высоте)</param>
        /// <param name="height">высота, до которой будет преобразовано изображение (может быть "" - изменить по ширине)</param>
        /// <returns>возвращает путь к изображению с изменённым размером (уменьшенным) или "err" в случае возникновения ошибки.</returns>
        public string GetPathToLittleFoto(string pathToBigFoto, string width, string height, bool needToDeleteLittleFoto = false)
        {
            string result = "";

            #region Проверки

            if (width == "" && height == "") return "err";
            if (width == "" && StringToNum.ParseInt(height) == -1) return "err";
            if (height == "" && StringToNum.ParseInt(width) == -1) return "err";

            #endregion
            #region Начальные установки длины и ширины

            float newWidth = 0, newHeight = 0;
            if (width == "" && height != "") newHeight = float.Parse(height);
            if (height == "" && width != "") newWidth = float.Parse(width);
            if (height != "" && width != "")
            {
                newHeight = float.Parse(height);
                newWidth = float.Parse(width);
            }

            #endregion
            #region Вычленение нужного пути к большому изображению

            string replBigFotoPath = pathToBigFoto.Replace("../", "").Replace("~/", "");

            #endregion

            string rootPath = HttpContext.Current.Server.MapPath("~");

            #region Определение размера большого фото и вычисление пропорций соотношений длины и ширины, если один из парамен

            if (newHeight == 0 || newWidth == 0)
            {
                try
                {
                    string[] bigImgSizes = ImageOverlay.GetImageSize(rootPath + replBigFotoPath);
                    float kw = 1, kh = 1;
                    if (bigImgSizes[0] != "-1")
                    {
                        kw = float.Parse(bigImgSizes[0]) / float.Parse(bigImgSizes[1]);
                        kh = float.Parse(bigImgSizes[1]) / float.Parse(bigImgSizes[0]);
                        if (newHeight == 0)    //если нужно посчитать новые размеры по ширине
                        {
                            newHeight = newWidth / kw;
                        }
                        if (newWidth == 0)    //если нужно посчитать новые размеры по высоте
                        {
                            newWidth = newHeight / kh;
                        }
                    }
                    else
                    {
                        return "../" + replBigFotoPath;
                    }
                }
                catch (Exception ex)
                {
                    return "../" + replBigFotoPath;
                }
            }

            #endregion

            string littleFotoPath = replBigFotoPath.Replace(".", "_l.");

            if (needToDeleteLittleFoto && File.Exists(rootPath + littleFotoPath))
                File.Delete(rootPath + littleFotoPath);

            string[] imgSizes = ImageOverlay.GetImageSize(rootPath + littleFotoPath);

            if (imgSizes[0] == "-1")
            {
                #region Если уменьшенного фото ещё не существует, то создаём его
                Bitmap littleImg = null;
                Bitmap target = null;
                try
                {
                    target = new Bitmap(rootPath + replBigFotoPath);
                    ImageOverlay imgOverLay = new ImageOverlay();
                    littleImg = imgOverLay.ResizeImage(target, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
                    littleImg.Save(rootPath + littleFotoPath, ImageFormat.Jpeg);
                    result = "../" + littleFotoPath;
                }
                catch (Exception)
                {
                    return "err";
                }
                finally
                {
                    target.Dispose();
                    target = null;
                    littleImg.Dispose();
                    littleImg = null;
                }
                #endregion
            }
            else
            {
                #region Если существует, то проверяем его на соответствие нужному размеру..

                if (imgSizes[0] != width)  //если размер существующего уменьшенного фото отличается от размера, заданного в параметре аним. списка, то..
                {
                    try
                    {
                        File.Delete(rootPath + littleFotoPath);
                    }
                    catch
                    {
                        return "err";
                    }
                    Bitmap littleImg = null;
                    Bitmap target = null;
                    try
                    {
                        target = new Bitmap(rootPath + replBigFotoPath);
                        ImageOverlay imgOverLay = new ImageOverlay();
                        littleImg = imgOverLay.ResizeImage(target, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
                        littleImg.Save(rootPath + littleFotoPath, ImageFormat.Jpeg);
                        result = "../" + littleFotoPath;
                    }
                    catch
                    {
                        return "err";
                    }
                    finally {
                        target.Dispose();
                        target = null;
                        littleImg.Dispose();
                        littleImg = null;
                    }
                }
                else
                {
                    result = "../" + littleFotoPath;
                }

                #endregion
            }

            return result;
        }

        #endregion
    }

    #endregion
}