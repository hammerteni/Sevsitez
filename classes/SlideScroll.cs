using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using site.classesHelp;
using Image = System.Web.UI.WebControls.Image;

/* файл с классами для работы с анимированным списком фотографий (файлы в папке files\slidescroll\) */
namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>класс, который формирует html-код, jquery-код и стили для аним. списка фото и возвращает его через функцию</summary>
    public class SlideScrollForm
    {
        private Page _pag;

        //Конструктор
        public SlideScrollForm(Page newPag)
        {
            _pag = newPag;
        }

        /// <summary>Метод берёт данные для аним. списка из БД и возвращает HTML-код для аним. списка фото</summary>
        /// <returns></returns>
        public LiteralControl GetSlideScroll(string sliderNum)
        {
            //получим структуру данных нужного слайд-шоу по его id (sliderNum)
            var sliderWork = new SlideScrollWork(_pag);
            var sliderStruct = sliderWork.GetSliderStructForId(int.Parse(sliderNum));
            if (sliderStruct.OnOff == "off") { return new LiteralControl(""); }
            if (sliderStruct.Id == -1) { return new LiteralControl(""); }
            int id = sliderStruct.Id;
            int ssAnimTime = int.Parse(sliderStruct.SsAnimTime);
            int btnsAnimTime = int.Parse(sliderStruct.BtnsAnimTime);
            int bigImgDivTime = int.Parse(sliderStruct.BigImgDivTime);
            int littleImageSize = int.Parse(sliderStruct.LittleImageSize);
            int visibleElemCount = int.Parse(sliderStruct.VisibleElemCount);

            string fotoBackColor = "#B22222";       //цвет фона фото при нажатии на них
            string bordersColor = "#216396";        //цвет бордюров вокруг фото (такое же значение нужно выставить в стилях .ssUlWrap ul li, .ssBigImgDiv)
            string bigFotoWidth = "900";            //нужная нам ширина большой картинки (высота вычисляется пропорционально)


            string[] strSplit;
            var listOfHtmlStrings = new List<string>();
            var imgOverLay = new ImageOverlay();
            string[] imgSizes;
            string littleFotoPath = "";
            string replBigFotoPath = "";
            var listOfBigFotoSize = new List<string>();
            string rootPath = HttpContext.Current.Server.MapPath("~");
            int counter = 1;
            foreach (string line in sliderStruct.ListOfData)
            {
                strSplit = line.Split(new[] { '|' });
                replBigFotoPath = strSplit[1].Replace("../", "");

                //создадим список размеров полноразмерных фото
                imgSizes = ImageOverlay.GetImageSize(rootPath + replBigFotoPath);
                listOfBigFotoSize.Add(imgSizes[0] + "|" + imgSizes[1]);

                littleFotoPath = replBigFotoPath.Replace(".", "_l.");
                imgSizes = ImageOverlay.GetImageSize(rootPath + littleFotoPath);
                if (imgSizes[0] == "-1") //если уменьшенного фото ещё не существует, то создаём его
                {
                    try
                    {
                        imgOverLay = new ImageOverlay();
                        Bitmap littleImg = imgOverLay.ResizeImage(new Bitmap(rootPath + replBigFotoPath), littleImageSize, littleImageSize);
                        littleImg.Save(rootPath + littleFotoPath, ImageFormat.Jpeg);
                        littleImg.Dispose();
                    }
                    catch { return new LiteralControl("Не удалось добавить аним. список фото по причине ошибки во время создания уменьшенного фото.."); }
                }
                else                    //если существует, то проверяем его на соответствие нужному размеру..
                {
                    if (imgSizes[0] != littleImageSize.ToString())  //если размер существующего уменьшенного фото отличается от размера, заданного в параметре аним. списка, то..
                    {
                        try
                        {
                            File.Delete(rootPath + littleFotoPath);
                        }
                        catch { return new LiteralControl("Не удалось добавить аним. список фото по причине невозможности доступа к файлу уменьшенного изображения при удалении.."); }
                        try
                        {
                            imgOverLay = new ImageOverlay();
                            Bitmap littleImg = imgOverLay.ResizeImage(new Bitmap(rootPath + replBigFotoPath), littleImageSize, littleImageSize);
                            littleImg.Save(rootPath + littleFotoPath, ImageFormat.Jpeg);
                            littleImg.Dispose();
                        }
                        catch { return new LiteralControl("Не удалось добавить аним. список фото по причине ошибки во время пересоздания уменьшенного фото.."); }
                    }
                }

                listOfHtmlStrings.Add("<img alt='" + counter + "' title='' style='width: " + littleImageSize + "px; height: " + littleImageSize + "px;' src='../../../" + littleFotoPath + "' />");
                counter++;
            }

            return SlideScroll(listOfHtmlStrings, listOfBigFotoSize, ssAnimTime, btnsAnimTime, bigImgDivTime, visibleElemCount, fotoBackColor, bordersColor, bigFotoWidth, id);
        }

        /// <summary>Функция возвращает HTML-код, jQuery-код, CSS-код для анимированного списка фотографий</summary>
        /// <param name="slideList">список строк с готовым html-кодом содержимого слайдов (обязательно должны содержать теги img)</param>
        /// <param name="visibleElemCount">кол-во фотографий, видимых в аним. списке</param>
        /// <param name="fotoBackColor">цвет фона уменьшенных фото в списке при нажатии на них</param>
        /// <param name="bigFotoWidth">нужная нам ширина большой картинки</param>
        /// <param name="bordersColor">цвет бордюров маленьких картинок (ещё должен быть установлен таким же в стилях)</param>
        /// <param name="uniqueNum">уникальный номер(для данной html-страницы), который будет использоваться для обозначения переменных и т.п. для одного аним. списка фото</param>
        /// <param name="listOfBigFotoSize">список размеров больших фотографий (ширина|длина)</param>
        /// <param name="ssAnimTime">длительность анимации сдвига аним.списка на одну позицию</param>
        /// <param name="btnsAnimTime">длительность анимации прозрачности при активации кнопок ВЛЕВО и ВПРАВО</param>
        /// <param name="bigImgDivTime">длительность анимации появления полноразмерного фото</param>
        /// <returns>возвращает всё необходимое для добавления аним. списка фото на страницу</returns>
        private LiteralControl SlideScroll(List<string> slideList, List<string> listOfBigFotoSize, int ssAnimTime, int btnsAnimTime, int bigImgDivTime, int visibleElemCount, string fotoBackColor, string bordersColor, string bigFotoWidth, int uniqueNum)
        {
            StringBuilder htmlString = new StringBuilder();

            #region Формирование HTML-кода

            //сделаем объявления для определения размеров картинок
            string[] imgSizes;
            string rootPath = HttpContext.Current.Server.MapPath("~");

            htmlString.Append("<div id='ssWrap" + uniqueNum + "' class='ssWrap'>");
            imgSizes = ImageOverlay.GetImageSize(rootPath + @"files/slidescroll/btnLeftSs.png");
            htmlString.Append("<img id='ssBtnLeft" + uniqueNum + "' class='ssBtns' src='files/slidescroll/btnLeftSs.png' style='width: " + imgSizes[0] + "px; height: " + imgSizes[1] + "px;' alt='Влево'/>");
            htmlString.Append("<div id='ssUlWrap" + uniqueNum + "' class='ssUlWrap'>");
            htmlString.Append("<ul>");
            foreach (string line in slideList)
            {
                htmlString.Append("<li>" + line + "</li>");
            }
            htmlString.Append("</ul>");
            htmlString.Append("</div>");
            imgSizes = ImageOverlay.GetImageSize(rootPath + @"files/slidescroll/btnRightSs.png");
            htmlString.Append("<img id='ssBtnRight" + uniqueNum + "' class='ssBtns' src='files/slidescroll/btnRightSs.png' style='width: " + imgSizes[0] + "px; height: " + imgSizes[1] + "px;' alt='Вправо'/>");
            htmlString.Append("</div>");

            #endregion

            var jQueryString = new StringBuilder();

            #region Формирование jQuery-кода и добавление его в конец страницы

            jQueryString.Append("<noindex><script type='text/javascript'>");
            jQueryString.Append("$(document).ready(function () { ");

            //выставляем количество элементов li списка ul, которые будут видимы
            jQueryString.Append("var visibleElements" + uniqueNum + " = " + visibleElemCount + "; ");
            //выставляем длительность анимации прокрутки
            jQueryString.Append("var ssAnimTime" + uniqueNum + " = " + ssAnimTime + "; ");
            //выставляем длительность анимации изменения прозрачности на кнопках ВЛЕВО и ВПРАВО
            jQueryString.Append("var btnsAnimTime" + uniqueNum + " = " + btnsAnimTime + "; ");
            //выставляем длительность анимации изменения прозрачности полноразмерного изображения
            jQueryString.Append("var bigImgDivTime" + uniqueNum + " = " + bigImgDivTime + "; ");
            //переменная хранит true, если в данный момент отображается полноразмерная картинка
            jQueryString.Append("var bigImgDivPopUp" + uniqueNum + " = false; ");
            //переменная хранит индекс в массиве ssImgBigSize0, который соответствует разрешению активной в данный момент картинки
            jQueryString.Append("var index" + uniqueNum + " = 0; ");
            //переменная хранит цвет фона при нажатии на фото в списке
            jQueryString.Append("var actColor" + uniqueNum + " = '" + fotoBackColor + "'; ");

            //Создадим массив со значением длины и ширины полноразмерных изображений для маленьких фото.
            //Индексы элементов этого массива соответствуют параметру alt тегов img списка прокрутки. То бишь в этом массиве
            //хранятся <ширина>|<высота> полноразмерных изображений, соответствующих маленьким.
            jQueryString.Append("var ssImgBigSize" + uniqueNum + " = new Array(); ");
            foreach (string line in listOfBigFotoSize)
            {
                jQueryString.Append("ssImgBigSize" + uniqueNum + ".push('" + line + "'); ");
            }

            //определим и установим новую ширину списка ul. Нужно для того, чтобы все элементы li выстроились в ряд
            jQueryString.Append("var ulNewWidth" + uniqueNum + " = 0, elemLiCount" + uniqueNum + " = 0, oneElemLiWidth" + uniqueNum + " = 0; ");
            jQueryString.Append("var $ssUlWrap" + uniqueNum + " = $('#ssUlWrap" + uniqueNum + " ul li'); ");
            jQueryString.Append("$ssUlWrap" + uniqueNum + ".each(function () { ");
            jQueryString.Append("ulNewWidth" + uniqueNum + " += $(this).outerWidth(true); ");
            jQueryString.Append("elemLiCount" + uniqueNum + "++; ");
            jQueryString.Append("$(this).children('img').click(function () { ");            //Навешиваем события нажатия на изображения
            jQueryString.Append("ssImgClick" + uniqueNum + "($(this)); ");
            jQueryString.Append("}); ");
            jQueryString.Append("}); ");
            jQueryString.Append("oneElemLiWidth" + uniqueNum + " = ulNewWidth" + uniqueNum + " / elemLiCount" + uniqueNum + "; ");
            jQueryString.Append("var $ul" + uniqueNum + " = $('#ssUlWrap" + uniqueNum + " ul'); ");
            jQueryString.Append("var $divWrap" + uniqueNum + " = $('#ssUlWrap" + uniqueNum + "'); ");
            jQueryString.Append("if (elemLiCount" + uniqueNum + " >= visibleElements" + uniqueNum + " + 2) { ");            //если в списке нужное кол-во элементов, чтобы их нужно было скрывать, то..
            jQueryString.Append("$ul" + uniqueNum + ".width(ulNewWidth" + uniqueNum + "); ");
            jQueryString.Append("$divWrap" + uniqueNum + ".width(oneElemLiWidth" + uniqueNum + " * visibleElements" + uniqueNum + "); ");
            jQueryString.Append("$ul" + uniqueNum + ".offset({ top: $ul" + uniqueNum + ".offset().top, left: $ul" + uniqueNum + ".offset().left - (oneElemLiWidth" + uniqueNum + " * ((elemLiCount" + uniqueNum + " - visibleElements" + uniqueNum + ") / 2)) }); ");   //устанавливает свойство left списка ul таким образом, чтобы была видна середина списка
            jQueryString.Append("} else { ");
            jQueryString.Append("$ul" + uniqueNum + ".width(ulNewWidth" + uniqueNum + "); ");
            jQueryString.Append("$divWrap" + uniqueNum + ".width(ulNewWidth" + uniqueNum + "); ");
            jQueryString.Append("} ");
            //вычислим и сохраним ширину обёрточного div'а
            jQueryString.Append("var divWrapperWidth" + uniqueNum + " = $divWrap" + uniqueNum + ".width(); ");
            //выставим на нужное место кнопки ВЛЕВО и ВПРАВО
            jQueryString.Append("var $ssBtnLeft" + uniqueNum + " = $('#ssBtnLeft" + uniqueNum + "'); ");
            jQueryString.Append("var $ssBtnRight" + uniqueNum + " = $('#ssBtnRight" + uniqueNum + "'); ");
            jQueryString.Append("$ssBtnLeft" + uniqueNum + ".offset({ top: ($divWrap" + uniqueNum + ".offset().top + $divWrap" + uniqueNum + ".outerHeight(true) / 2) - ($ssBtnLeft" + uniqueNum + ".height() / 2), left: $divWrap" + uniqueNum + ".offset().left - $ssBtnLeft" + uniqueNum + ".width() }); ");
            jQueryString.Append("$ssBtnRight" + uniqueNum + ".offset({ top: ($divWrap" + uniqueNum + ".offset().top + $divWrap" + uniqueNum + ".outerHeight(true) / 2) - ($ssBtnRight" + uniqueNum + ".height() / 2), left: $divWrap" + uniqueNum + ".offset().left + $divWrap" + uniqueNum + ".outerWidth(true) }); ");
            //Функции событий для нажатия кнопок ВЛЕВО и ВПРАВО
            jQueryString.Append("var $liObj" + uniqueNum + "; ");
            jQueryString.Append("$ssBtnLeft" + uniqueNum + ".click(function () { ");
            jQueryString.Append("if (ulNewWidth" + uniqueNum + " > divWrapperWidth" + uniqueNum + ") { ");
            jQueryString.Append("moveUlToRight" + uniqueNum + "(); ");
            jQueryString.Append("if (bigImgDivPopUp" + uniqueNum + ") { ");
            jQueryString.Append("replaceBigImg" + uniqueNum + "('left'); ");
            jQueryString.Append("} ");
            jQueryString.Append("} ");
            jQueryString.Append("}); ");
            jQueryString.Append("$ssBtnRight" + uniqueNum + ".click(function () { ");
            jQueryString.Append("if (ulNewWidth" + uniqueNum + " > divWrapperWidth" + uniqueNum + ") { ");
            jQueryString.Append("moveUlToLeft" + uniqueNum + "(); ");
            jQueryString.Append("if (bigImgDivPopUp" + uniqueNum + ") { ");
            jQueryString.Append("replaceBigImg" + uniqueNum + "('right'); ");
            jQueryString.Append("} ");
            jQueryString.Append("} ");
            jQueryString.Append("}); ");
            //Функция сдвига списка влево
            jQueryString.Append("function moveUlToLeft" + uniqueNum + "() { ");
            jQueryString.Append("$liObj" + uniqueNum + " = $('#ssUlWrap" + uniqueNum + " ul').children().first(); ");            //сохраним объект первого элемента li в списке ul
            jQueryString.Append("$liObj" + uniqueNum + ".hide(ssAnimTime" + uniqueNum + ", function () { ");
            jQueryString.Append("$(this).appendTo('#ssUlWrap" + uniqueNum + " ul').show(); ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            //Функция сдвига списка вправо
            jQueryString.Append("function moveUlToRight" + uniqueNum + "() { ");
            jQueryString.Append("$liObj" + uniqueNum + " = $('#ssUlWrap" + uniqueNum + " ul').children().last(); ");             //сохраним объект последнего элемента li в списке ul
            jQueryString.Append("$liObj" + uniqueNum + ".hide(0, function () { ");
            jQueryString.Append("$(this).prependTo('#ssUlWrap" + uniqueNum + " ul').show(ssAnimTime" + uniqueNum + "); ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            //Функции анимации проявления кнопок ВЛЕВО и ВПРАВО, при наведении на блок полосы прокрутки
            //Мышка вошла в область элемента
            jQueryString.Append("$('#ssWrap" + uniqueNum + "').mouseenter(function () { ");
            jQueryString.Append("$ssBtnLeft" + uniqueNum + ".animate({ opacity: 1 }, btnsAnimTime" + uniqueNum + "); ");
            jQueryString.Append("$ssBtnRight" + uniqueNum + ".animate({ opacity: 1 }, btnsAnimTime" + uniqueNum + "); ");
            jQueryString.Append("}); ");
            //Мышка вышла из области элемента
            jQueryString.Append("$('#ssWrap" + uniqueNum + "').mouseleave(function () { ");
            jQueryString.Append("$ssBtnLeft" + uniqueNum + ".animate({ opacity: 0.3 }, btnsAnimTime" + uniqueNum + "); ");
            jQueryString.Append("$ssBtnRight" + uniqueNum + ".animate({ opacity: 0.3 }, btnsAnimTime" + uniqueNum + "); ");
            jQueryString.Append("}); ");
            //Функция при нажатии на изображение в списке
            jQueryString.Append("function ssImgClick" + uniqueNum + "($imgObj" + uniqueNum + ") { ");
                //Получим размеры полноразмерного изображения, соответствующего маленькому и путь к увеличенному изображению
                //Записываем в переменную строку вида 800|1035 (ширина|высота)
            jQueryString.Append("var sizes" + uniqueNum + " = ssImgBigSize" + uniqueNum + "[$imgObj" + uniqueNum + ".attr('alt') - 1]; ");
            jQueryString.Append("var sizeArr" + uniqueNum + " = sizes" + uniqueNum + ".split('|'); ");

            // подгоним реальный размер увеличенного фото под нужный нам размер (pageWidth)
            jQueryString.Append("var k = sizeArr" + uniqueNum + "[0] / sizeArr" + uniqueNum + "[1]; ");
            jQueryString.Append("var pageWidth" + uniqueNum + " = " + bigFotoWidth + "; ");
            jQueryString.Append("if (sizeArr" + uniqueNum + "[0] >= pageWidth" + uniqueNum + ") ");
            jQueryString.Append("{ ");
            jQueryString.Append("sizeArr" + uniqueNum + "[0] = pageWidth" + uniqueNum + "; ");
            jQueryString.Append("sizeArr" + uniqueNum + "[1] = sizeArr" + uniqueNum + "[0] / k; ");
            jQueryString.Append("} ");

            jQueryString.Append("index" + uniqueNum + " = $imgObj" + uniqueNum + ".attr('alt') - 1; ");             //Изменим индекс активной картинки
            jQueryString.Append("var pathToBigImg" + uniqueNum + " = $imgObj" + uniqueNum + ".attr('src').replace('_l', ''); ");    //Определяем путь к увеличенному изображению, соответствующему маленькому
            jQueryString.Append("ssPopUpBigImg" + uniqueNum + "(sizeArr" + uniqueNum + ", pathToBigImg" + uniqueNum + "); ");
            jQueryString.Append("selectActImg" + uniqueNum + "(index" + uniqueNum + "); ");
            jQueryString.Append("moveListIfEdge" + uniqueNum + "($imgObj" + uniqueNum + "); ");
            jQueryString.Append("}; ");
            //функция показа полноразмерного изображения
            jQueryString.Append("function ssPopUpBigImg" + uniqueNum + "(sizeArray" + uniqueNum + ", pathToImg" + uniqueNum + ") { ");
            jQueryString.Append("$('#ssBigImgDiv" + uniqueNum + "').remove(); ");           //Удалим для начала блок, если он есть
            jQueryString.Append("var htmlString" + uniqueNum + " = \"<div id='ssBigImgDiv" + uniqueNum + "' class='ssBigImgDiv'><img id='bigImg" + uniqueNum + "' src='\" + pathToImg" + uniqueNum + " + \"' alt='Image' style='width:\" + sizeArray" + uniqueNum + "[0] + \"px; height:\" + sizeArray" + uniqueNum + "[1] + \"px;' /><img id='ssBtnClose" + uniqueNum + "' style='width:30px; height:30px;' class='ssBtnClose' src='../images/btnClose.png' alt='Close' /></div>\"; ");
            jQueryString.Append("var $ssWrap" + uniqueNum + " = $('#ssWrap" + uniqueNum + "'); ");
            jQueryString.Append("$('body').append(htmlString" + uniqueNum + "); ");
            jQueryString.Append("var $ssBigImgDiv" + uniqueNum + " = $('#ssBigImgDiv" + uniqueNum + "'); ");            //Установим блок с изображением в нужно место
            jQueryString.Append("$ssBigImgDiv" + uniqueNum + ".offset({ top: $ssWrap" + uniqueNum + ".offset().top - sizeArray" + uniqueNum + "[1] - 25, left: ($ssWrap" + uniqueNum + ".offset().left + $ssWrap" + uniqueNum + ".outerWidth(true) / 2) - (sizeArray" + uniqueNum + "[0] / 2) }); ");
            jQueryString.Append("var $ssBtnClose" + uniqueNum + " = $('#ssBtnClose" + uniqueNum + "'); ");          //Установим кнопку закрытия блока с полноразмерным изображением
            jQueryString.Append("$ssBtnClose" + uniqueNum + ".offset({ top: $ssBigImgDiv" + uniqueNum + ".offset().top - $ssBtnClose" + uniqueNum + ".height() / 2, left: $ssBigImgDiv" + uniqueNum + ".offset().left + $ssBigImgDiv" + uniqueNum + ".outerWidth(true) - $ssBtnClose" + uniqueNum + ".width() / 2 }); ");
            jQueryString.Append("$ssBtnClose" + uniqueNum + ".bind('click', function () { ");       //Навесим на неё событие нажатия для закрытия блока
            jQueryString.Append("ssBigImgDivClose" + uniqueNum + "(); ");
            jQueryString.Append("}); ");
            jQueryString.Append("$ssBigImgDiv" + uniqueNum + ".bind('click', function () { ");      //Навесим на весь блок с изображением событие нажатия для закрытия этого блока
            jQueryString.Append("ssBigImgDivClose" + uniqueNum + "(); ");
            jQueryString.Append("}); ");
            jQueryString.Append("$ssBigImgDiv" + uniqueNum + ".animate({ opacity: 1 }, bigImgDivTime" + uniqueNum + "); ");     //Анимация появления блока с увеличенным изображением
            //функция закрытия и удаления таблицы с полноразмерным слайдом
            jQueryString.Append("function ssBigImgDivClose" + uniqueNum + "() { ");
            jQueryString.Append("$('#ssBtnClose" + uniqueNum + "').unbind('click'); ");
            jQueryString.Append("$('#ssBigImgDiv" + uniqueNum + "').unbind('click'); ");
            jQueryString.Append("$ssBigImgDiv" + uniqueNum + ".animate({ opacity: 0 }, bigImgDivTime" + uniqueNum + "); ");
            jQueryString.Append("$('#ssBigImgDiv" + uniqueNum + "').remove(); ");
            jQueryString.Append("$ssUlWrap" + uniqueNum + ".each(function () { ");      //Убираем выделение активного фона у всех картинок в списке
            jQueryString.Append("$(this).css('border', '2px " + bordersColor + " solid'); ");
            jQueryString.Append("}); ");
            jQueryString.Append("bigImgDivPopUp" + uniqueNum + " = false; ");   //Обозначим, что полноразмерная картинка более не отображается
            jQueryString.Append("}; ");
            jQueryString.Append("bigImgDivPopUp" + uniqueNum + " = true; ");    //Обозначим, что в данный момент отображается полноразмерная картинка
            jQueryString.Append("}; ");
            //Функция отвечает за подбор данных для смены активной полноразмерной картинки на следующую или предыдущую
            //в зависимости от переданного в переменную direction0 значения. Если left - то в на предыдущую, если right - то на следущую
            jQueryString.Append("function replaceBigImg" + uniqueNum + "(direction" + uniqueNum + ") { ");
            jQueryString.Append("var newIndex" + uniqueNum + ", sizess" + uniqueNum + ", sizeArrs" + uniqueNum + ", pathToBigImg" + uniqueNum + "; ");
            jQueryString.Append("if (direction" + uniqueNum + " == 'left') { ");
                //Получим размеры полноразмерного изображения, соответствующего маленькому и путь к увеличенному изображению
                //Записываем в переменную строку вида 800|1035 (ширина|высота)
            jQueryString.Append("if (index" + uniqueNum + " <= 0) { newIndex" + uniqueNum + " = ssImgBigSize" + uniqueNum + ".length - 1; index" + uniqueNum + " = ssImgBigSize" + uniqueNum + ".length - 1; } else { newIndex" + uniqueNum + " = index" + uniqueNum + " - 1; index" + uniqueNum + " -= 1; } ");
            jQueryString.Append("sizess" + uniqueNum + " = ssImgBigSize" + uniqueNum + "[newIndex" + uniqueNum + "]; ");
            jQueryString.Append("sizeArrs" + uniqueNum + " = sizess" + uniqueNum + ".split('|'); ");

            // подгоним реальный размер увеличенного фото под нужный нам размер (pageWidth)
            jQueryString.Append("var k = sizeArrs" + uniqueNum + "[0] / sizeArrs" + uniqueNum + "[1]; ");
            jQueryString.Append("var pageWidths" + uniqueNum + " = " + bigFotoWidth + "; ");
            jQueryString.Append("if (sizeArrs" + uniqueNum + "[0] >= pageWidths" + uniqueNum + ") ");
            jQueryString.Append("{ ");
            jQueryString.Append("sizeArrs" + uniqueNum + "[0] = pageWidths" + uniqueNum + "; ");
            jQueryString.Append("sizeArrs" + uniqueNum + "[1] = sizeArrs" + uniqueNum + "[0] / k; ");
            jQueryString.Append("} ");

            //Определяем путь к увеличенному изображению, соответствующему маленькому
            jQueryString.Append("pathToBigImg" + uniqueNum + " = $($ssUlWrap" + uniqueNum + "[newIndex" + uniqueNum + "]).children('img').attr('src').replace('_l', ''); ");
            jQueryString.Append("ssPopUpBigImg" + uniqueNum + "(sizeArrs" + uniqueNum + ", pathToBigImg" + uniqueNum + "); ");
            jQueryString.Append("selectActImg" + uniqueNum + "(index" + uniqueNum + "); ");
            jQueryString.Append("} ");
            jQueryString.Append("else if (direction" + uniqueNum + " == 'right') { ");
                //Получим размеры полноразмерного изображения, соответствующего маленькому и путь к увеличенному изображению
                //Записываем в переменную строку вида 800|1035 (ширина|высота)
            jQueryString.Append("if (index" + uniqueNum + " >= ssImgBigSize" + uniqueNum + ".length - 1) { newIndex" + uniqueNum + " = 0; index" + uniqueNum + " = 0; } else { newIndex" + uniqueNum + " = index" + uniqueNum + " + 1; index" + uniqueNum + " += 1; } ");
            jQueryString.Append("sizess" + uniqueNum + " = ssImgBigSize" + uniqueNum + "[newIndex" + uniqueNum + "]; ");
            jQueryString.Append("sizeArrs" + uniqueNum + " = sizess" + uniqueNum + ".split('|'); ");

            // подгоним реальный размер увеличенного фото под нужный нам размер (pageWidth)
            jQueryString.Append("var k = sizeArrs" + uniqueNum + "[0] / sizeArrs" + uniqueNum + "[1]; ");
            jQueryString.Append("var pageWidths" + uniqueNum + " = " + bigFotoWidth + "; ");
            jQueryString.Append("if (sizeArrs" + uniqueNum + "[0] >= pageWidths" + uniqueNum + ") ");
            jQueryString.Append("{ ");
            jQueryString.Append("sizeArrs" + uniqueNum + "[0] = pageWidths" + uniqueNum + "; ");
            jQueryString.Append("sizeArrs" + uniqueNum + "[1] = sizeArrs" + uniqueNum + "[0] / k; ");
            jQueryString.Append("} ");

            //Определяем путь к увеличенному изображению, соответствующему маленькому
            jQueryString.Append("pathToBigImg" + uniqueNum + " = $($ssUlWrap" + uniqueNum + "[newIndex" + uniqueNum + "]).children('img').attr('src').replace('_l', ''); ");
            jQueryString.Append("ssPopUpBigImg" + uniqueNum + "(sizeArrs" + uniqueNum + ", pathToBigImg" + uniqueNum + "); ");
            jQueryString.Append("selectActImg" + uniqueNum + "(index" + uniqueNum + "); ");
            jQueryString.Append("} ");
            jQueryString.Append("}; ");
            //Функция выделения активного фото другим цветом
            jQueryString.Append("function selectActImg" + uniqueNum + "(actIndex" + uniqueNum + ") { ");
            jQueryString.Append("$ssUlWrap" + uniqueNum + ".each(function () { ");
            jQueryString.Append("$(this).css('border', '2px " + bordersColor + " solid'); ");
            jQueryString.Append("}); ");
            jQueryString.Append("$($ssUlWrap" + uniqueNum + "[actIndex" + uniqueNum + "]).css('border', '2px ' + actColor" + uniqueNum + " + ' solid'); ");
            jQueryString.Append("}; ");
            //Функция сдвига списка на одну позицию, если это крайнее изображение (неважно, с какого края)
            jQueryString.Append("function moveListIfEdge" + uniqueNum + "($imgObject" + uniqueNum + ") { ");
                //Если посетитель кликнул по крайней левой картинке, то сдвинем список вправо.
                //Если по крайней правой, то сдвинем список влево
            jQueryString.Append("if (($imgObject" + uniqueNum + ".offset().left - $divWrap" + uniqueNum + ".offset().left) < $imgObject" + uniqueNum + ".width()) { ");
            jQueryString.Append("moveUlToRight" + uniqueNum + "(); ");
            jQueryString.Append("} ");
            jQueryString.Append("else if (($divWrap" + uniqueNum + ".offset().left + $divWrap" + uniqueNum + ".width() - $imgObject" + uniqueNum + ".offset().left) < $imgObject" + uniqueNum + ".width() * 2) { ");
            jQueryString.Append("moveUlToLeft" + uniqueNum + "(); ");
            jQueryString.Append("} ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");

            jQueryString.Append("</script></noindex> ");

            _pag.Controls.Add(new LiteralControl(jQueryString.ToString()));

            #endregion

            var cssString = new StringBuilder();

            #region Формирование css-кода и добавление его в шапку страницы

            //CSS-код к данному классу находится в файле - txtpages.css (или slidescroll.css)
            /* Вот этот код
              .ssWrap {
    position: relative; display: block;
    margin: 0;
}
.ssUlWrap {
    position: relative; display: inline-block;
    margin: 0 auto;
    overflow: hidden;
}
.ssUlWrap ul {
    position: relative;
    margin: 0; padding: 0;
    list-style-type: none;
}
.ssUlWrap ul li {
    display: inline-block;
    float: left;
    margin: 9px;
            
    -ms-border-radius: 4px; border-radius: 4px;
    border: 2px #F3B746 solid; 
            
    -webkit-box-shadow: 2px 2px 9px 0px rgba(0,0,0,0.75);
    -moz-box-shadow: 2px 2px 9px 0px rgba(0,0,0,0.75);
    -o-box-shadow: 2px 2px 9px 0px rgba(0,0,0,0.75);
    -ms-box-shadow: 2px 2px 9px 0px rgba(0,0,0,0.75);
    box-shadow: 2px 2px 9px 0px rgba(0,0,0,0.75);
            
    cursor: pointer;
}
.ssBtns {
    position: absolute; display: inline-block;
    -ms-opacity: 0.3; opacity: 0.3;
    z-index: 1;
    cursor: pointer;
}
.ssBigImgDiv {
    position: absolute;
    display: inline-block;
    -ms-opacity: 0; opacity: 0;
            
    -ms-border-radius: 8px; border-radius: 8px;
    border: 4px #F3B746 solid; 
            
    -webkit-box-shadow: 9px 9px 27px 0px rgba(0,0,0,0.75);
    -moz-box-shadow: 9px 9px 27px 0px rgba(0,0,0,0.75);
    -o-box-shadow: 9px 9px 27px 0px rgba(0,0,0,0.75);
    -ms-box-shadow: 9px 9px 27px 0px rgba(0,0,0,0.75);
    box-shadow: 9px 9px 27px 0px rgba(0,0,0,0.75);
            
    z-index: 9;
}
.ssBtnClose {
    position: absolute;
    cursor: pointer;
    z-index: 9;
}
            */

            #endregion

            return new LiteralControl(htmlString.ToString());
        }
    }

    /// <summary>Класс, который формирует HTML-формы редактирования аним. списка фото для консоли администрирования</summary>
    public class SlideScrollFormAdm
    {
        private Page _pag;

        public SlideScrollFormAdm(Page pagenew) { _pag = pagenew; }

        /// <summary>функция возвращает таблицу с выбором списка для редактирования из выпадающего списка и кнопкой СОЗДАТЬ АНИМИРОВАННЫЙ СПИСОК ФОТОГРАФИЙ</summary>
        /// <returns></returns>
        public Panel GetSlideScrollChoosePanel()
        {
            var panelWrapper = new Panel();

            var sliderWork = new SlideScrollWork(_pag);

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР АНИМИРОВАННЫХ СПИСКОВ ФОТОГРАФИЙ"; panelWrapper.Controls.Add(lbl);

            //Кнопка добавления нового списка
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "+ список фото";
            lBtn.ToolTip = "Создать новый анимированный список фотографий."; lBtn.Command += (lBtnCreateSlider_Command); panelWrapper.Controls.Add(lBtn);

            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //Выпадающий список с выбором списка для редактирования
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выберите анимированный список для редактирования: "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            int counter = 0; int index = 0;
            foreach (string name in sliderWork.GetListOfAllSliderNames())
            {
                ddl.Items.Add(name);
                if (_pag.Session["SlideScrollStruct"] != null)
                {
                    if (name == ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Name) { index = counter; }
                }
                counter++;
            }
            ddl.SelectedIndex = index; ddl.ID = "ddlSliderSelect"; panelLine.Controls.Add(ddl);

            //Запуск редактирования выбранного в списке аним списка
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "редактировать";
            lBtn.ToolTip = "Запуск редактирования выбранного в списке анимированного списка фотографий."; lBtn.Command += (lBtnEditSlider_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetSliderChoosePanel()

        /// <summary>событие для выпадающего списка с выбором аним списков для редактирования</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEditSlider_Command(object sender, CommandEventArgs e)
        {
            var sliderWork = new SlideScrollWork(_pag);
            string val = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlSliderSelect")).SelectedValue;
            if (val == "") { _pag.Session["SlideScrollStruct"] = null; }
            else { _pag.Session["SlideScrollStruct"] = sliderWork.GetSliderStructForName(val); }

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>события нажатия на кнопку "+ список фото"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCreateSlider_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["SlideScrollStruct"] = new SlideScrollStruct();
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Name = "";
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).OnOff = "on";
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).VisibleElemCount = "4";
            var sliderWork = new SlideScrollWork(_pag);
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Id = sliderWork.GetIdForNewSlider();
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData = new List<string>();
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).SsAnimTime = "200";
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BtnsAnimTime = "200";
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BigImgDivTime = "200";
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).LittleImageSize = "100";

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        /// <summary>функция возвращает таблицу с редактором аним. списка (функция используется так же при создании нового аним списка)</summary>
        /// <returns></returns>
        public Panel GetSlideScrollEditPanel()
        {
            var panelWrapper = new Panel();

            //проверка на то, что временная структура с данными по слайд-шоу существует
            if (_pag.Session["SlideScrollStruct"] == null) return panelWrapper;

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "Панель редактирования основных параметров аним. списка фотографий"; panelWrapper.Controls.Add(lbl);

            //имя аним. списка
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Имя аним. списка фото:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Name;
            txtBox.ID = "txtBox_SliderName"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ID аним. списка фото
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "ID аним. списка фото: "; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Id.ToString(); txtBox.ReadOnly = true;
            txtBox.ID = "txtBox_SliderId"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Чекбокс включения или выключения аним. списка фото
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Включить аним. список фото:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var chkBox = new CheckBox(); chkBox.ID = "slideShowOnOff";
            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).OnOff == "on") { chkBox.Checked = true; }
            else if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).OnOff == "off") { chkBox.Checked = false; }
            panelLine.Controls.Add(chkBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка количество видимых фотографий в списке
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Установить количество видимых фото в аним. списке:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).VisibleElemCount;
            txtBox.ID = "txtBox_VisibleElemCount"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка длительности анимации одной прокрутки элементов списка (в миллисекундах)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Установить длительность анимации прокрутки списка(в миллисекундах):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).SsAnimTime;
            txtBox.ID = "txtBox_SsAnimTime"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка длительности анимации изменения прозрачности при наведении на кнопки ВЛЕВО и ВПРАВО (в миллисекундах)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Установить длительность анимации появления стрелок ВЛЕВО и ВПРАВО(в миллисекундах):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BtnsAnimTime;
            txtBox.ID = "txtBox_BtnsAnimTime"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка длительности анимации изменения прозрачности полноразмерного изображения (в миллисекундах)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Установить длительность анимации появления полноразмерного изображения (в миллисекундах):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BigImgDivTime;
            txtBox.ID = "txtBox_BigImgDivTime"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Высота и ширина уменьшенных изображений в списке (в пикселях)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Высота (и ширина) уменьшенных изображений в списке (в пикселях):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).LittleImageSize;
            txtBox.ID = "txtBox_LittleImageSize"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Кнопки для загрузки нового фото
            var fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fotoUpload";
            fUpload.ToolTip = "Можно добавлять изображения в форматах JPEG и PNG размером не более 500 килобайт"; panelWrapper.Controls.Add(fUpload);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "+ фото к списку";
            lBtn.ToolTip = "Добавить фотографию к аним. списку фото";
            lBtn.CommandArgument = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Id.ToString();
            lBtn.Command += (lBtnAddFoto_Command); panelWrapper.Controls.Add(lBtn); panelWrapper.Controls.Add(new LiteralControl("<br />"));

            //Кнопки СОХРАНИТЬ И УДАЛИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "СОХРАНИТЬ ВСЁ"; lBtn.ToolTip = "Сохраняются все изменения, которые были сделаны в аним. списке";
            lBtn.Command += (lBtnSliderSave_Command); lBtn.ID = "btnSliderSave"; panelWrapper.Controls.Add(lBtn);
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "УДАЛИТЬ ВСЁ"; lBtn.ToolTip = "Полное удаление аним. списка фото";
            lBtn.OnClientClick = "return confirm('Аним. список фото будет полностью удалён. Удалить?');";
            lBtn.Command += (btnSliderDelete_Command); lBtn.ID = "btnSliderDelete"; panelWrapper.Controls.Add(lBtn);

            //Ниже добавляются элементы для редактирования каждого фото в аним. списке
            string[] lineSplit;
            int counter = 0;
            //цикл для отображения панелей редактирования каждого слайда, входящего в слайд-шоу
            foreach (string line in ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData)
            {
                lineSplit = line.Split(new[] { '|' });

                //строка с заглавием редактора аним. списка
                lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "Панель редактирования фото № " + (counter + 1); panelWrapper.Controls.Add(lbl);

                //строка с фото
                var img = new Image(); img.ImageUrl = lineSplit[1]; img.CssClass = "imgBckGrd imgBckGrdWidth"; panelWrapper.Controls.Add(img);

                //строка с кнопками ВВЕРХ, ВНИЗ, УДАЛИТЬ ФОТО
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Вверх";
                lBtn.ToolTip = "Временно передвинуть фото вверх в аним. списке. Для окончательного сохранения изменений нажмите СОХРАНИТЬ ВСЁ.";
                lBtn.Command += (lBtnOneSlidUp_Command); lBtn.ID = "btnOneSlidUp_" + counter; panelWrapper.Controls.Add(lBtn);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Вниз";
                lBtn.ToolTip = "Временно передвинуть фото вниз в аним. списке. Для окончательного сохранения изменений нажмите СОХРАНИТЬ ВСЁ.";
                lBtn.Command += (lBtnOneSlidDown_Command); lBtn.ID = "btnOneSlidDown_" + counter; panelWrapper.Controls.Add(lBtn);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Удалить фото";
                lBtn.ToolTip = "Фото удаляется из списка фото, так же удаляется сам файл фото с жёсткого диска." +
                              " Так же рекомендуется нажать на кнопку СОХРАНИТЬ ВСЁ.";
                lBtn.OnClientClick = "return confirm('Данные, касающиеся данного фото будут полностью удалены. Удалить?');";
                lBtn.Command += (lBtnOneSlidDelete_Command); lBtn.ID = "btnOneSlidDelete_" + counter; panelWrapper.Controls.Add(lBtn);

                counter++;
            }

            return panelWrapper;
        }

        #region События для функции GetSliderEditPanel()

        /// <summary>событие нажатия на кнопку - "+ фото к списку"</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит Id</param>
        protected void lBtnAddFoto_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fotoUpload");
            var warning = new WarnClass();

            if (fUpload.HasFile)
            {
                warning.HideWarning(_pag.Master);

                string fileName = _pag.Server.HtmlEncode(fUpload.FileName);
                string extension = Path.GetExtension(fileName);

                if (extension != null && ((extension.ToLower() == ".jpg") || (extension.ToLower() == ".jpeg") || (extension.ToLower() == ".png")))     //проверка на допустимые расширения закачиваемого файла
                {
                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 500000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        int num = 0;

                        #region КОД ОПРЕДЕЛЕНИЯ ПОСЛЕДНЕЙ ЦИФРЫ С ИМЕНИ ФАЙЛА ДЛЯ НОВОЙ ФОТОГРАФИИ

                        if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Count > 0)    //если для аним. списка уже добавлены фотографии, то..
                        {
                            var listOfNum = new List<int>();
                            string[] lineSplit, strSplit, splitFName, splitLeftPart; string filename;
                            foreach (string line in ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData)
                            {
                                lineSplit = line.Split(new[] { '|' });

                                strSplit = lineSplit[1].Split(new[] { '/' });       //разделим путь к файлу картинки для получения имени файла
                                filename = strSplit[strSplit.Length - 1];
                                splitFName = filename.Split(new[] { '.' });         //разделим имя файла для получения имени файла без расширения
                                splitLeftPart = splitFName[0].Split(new[] { '_' }); //разделим левую часть имени файла для получения порядкового номера файла картинки
                                listOfNum.Add(int.Parse(splitLeftPart[1]));
                            }
                            listOfNum.Sort();
                            num = listOfNum[listOfNum.Count - 1] + 1;
                        }

                        #endregion

                        string newFileName = e.CommandArgument + "_" + num + extension;
                        string savePath = _pag.Server.MapPath("~") + @"files\slidescroll\" + newFileName;
                        try
                        {
                            fUpload.SaveAs(savePath);
                            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Add("img|../files/slidescroll/" + newFileName);

                            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
                        }
                        catch { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", _pag.Master); }

                    }
                    else warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 500 килобайт.", _pag.Master);
                }
                else
                {
                    warning.ShowWarning("ВНИМАНИЕ. Доступно добавление только файлов формата 'jpg', 'jpeg' или 'png'.", _pag.Master);
                }
            }
            else warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", _pag.Master);
        }

        /// <summary>событие нажатия на кнопку - "СОХРАНИТЬ ВСЁ"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSliderSave_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideScrollWork(_pag);
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);

            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Name = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderName")).Text;
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Id = int.Parse(((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderId")).Text);
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).SsAnimTime = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SsAnimTime")).Text;
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BtnsAnimTime = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_BtnsAnimTime")).Text;
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BigImgDivTime = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_BigImgDivTime")).Text;
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).LittleImageSize = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_LittleImageSize")).Text;
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).VisibleElemCount = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_VisibleElemCount")).Text;

            bool checker = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$slideShowOnOff")).Checked;
            if (checker) { ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).OnOff = "on"; } else { ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).OnOff = "off"; }

            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).VisibleElemCount.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели кол-во отображаемых фото в аним. списке.", _pag.Master); return; }
            int parseToInt = StringToNum.ParseInt(((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).VisibleElemCount);
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение кол-ва отображаемых фото в аним. списке.", _pag.Master); return; }

            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).Name.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели имя для аним. списка.", _pag.Master); return; }
            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).LittleImageSize.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели высоту (и ширину) для фото в аним. списке.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).LittleImageSize);
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение высоты (и ширины) для фото в аним. списке.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).SsAnimTime);
            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).SsAnimTime.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели длительности анимации одной прокрутки элементов списка.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение длительности анимации одной прокрутки элементов списка.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BtnsAnimTime);
            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BtnsAnimTime.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели длительность анимации изменения прозрачности при наведении на кнопки ВЛЕВО и ВПРАВО.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение длительности анимации изменения прозрачности при наведении на кнопки ВЛЕВО и ВПРАВО.", _pag.Master); return; }

            parseToInt = StringToNum.ParseInt(((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BigImgDivTime);
            if (((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).BigImgDivTime.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели длительность анимации изменения прозрачности полноразмерного изображения.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение длительности анимации изменения прозрачности полноразмерного изображения.", _pag.Master); return; }

            if (!sliderWork.ReplaceOrDeleteSlider((SlideScrollStruct)_pag.Session["SlideScrollStruct"]))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения аним. списка. Попробуйте повторить.", _pag.Master); }
        }

        /// <summary>событие нажатия на кнопку - "УДАЛИТЬ ВСЁ"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSliderDelete_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideScrollWork(_pag);
            var warning = new WarnClass();

            if (!sliderWork.ReplaceOrDeleteSlider((SlideScrollStruct)_pag.Session["SlideScrollStruct"], true))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе удаления аним. списка фото. Попробуйте повторить.", _pag.Master);
            }
            else
            {
                #region УДАЛЯЕМ ВСЕ ДАННЫЕ ПО АНИМ. СПИСКУ И ВСЕ ФОТОГРАФИИ, ЕСЛИ ТАКОВЫЕ ИМЕЮТСЯ

                foreach (string line in ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData)
                {
                    //удалим фотографии
                    string[] slideDataSplit = line.Split(new[] { '|' });
                    try
                    {
                        File.Delete(_pag.Server.MapPath("~") + slideDataSplit[1].Replace("../", ""));
                    }
                    catch { }
                }

                #endregion

                _pag.Session["SlideScrollStruct"] = null;
                if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
            }
        }

        /// <summary>событие нажатия на кнопку - "Вверх". Временно переместить слайд вверх в списке слайдов.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOneSlidUp_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            //получим идекс слайда
            string[] strSplit = ((LinkButton)sender).ID.Split(new[] { '_' });       //индекс будет содержаться в элементе массива strSplit[1]

            //проверим, есть ли смысл двигать слайд вверх или двигать уже некуда
            if (strSplit[1] == "0") { return; }

            //получим, изменим и перезапишем нужную строку в списке ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData
            string line = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData[int.Parse(strSplit[1])];
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));             //удалим старую строку из списка
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Insert(int.Parse(strSplit[1]) - 1, line);     //добавим новую строку в список на нужное место

            //перезагрузим страницу, чтобы увидеть изменения и перезагрузить значения ID элементов управления
            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку - "Вниз". Временное переместить слайд вниз в списке слайдов.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOneSlidDown_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            //получим идекс слайда
            string[] strSplit = ((LinkButton)sender).ID.Split(new[] { '_' });       //индекс будет содержаться в элементе массива strSplit[1]

            //проверим, есть ли смысл двигать фото вниз или двигать уже некуда
            if (int.Parse(strSplit[1]) == ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Count - 1) { return; }

            //получим, изменим и перезапишем нужную строку в списке ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData
            string line = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData[int.Parse(strSplit[1])];
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старое фото из списка
            if (int.Parse(strSplit[1]) == ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Count - 1)
            {
                ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Add(line);    //добавим новое фото в конец списка, если фото находился на предпоследнем месте
            }
            else
            {
                ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.Insert(int.Parse(strSplit[1]) + 1, line);    //добавим новое фото в список на нужное место
            }

            //перезагрузим страницу, чтобы увидеть изменения и перезагрузить значения ID элементов управления
            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку - "Удалить фото". Полное удаления фото из аним. списка фото.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOneSlidDelete_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideScrollWork(_pag);
            var warning = new WarnClass();

            //получим идекс фото
            string[] strSplit = ((LinkButton)sender).ID.Split(new[] { '_' });       //индекс будет содержаться в элементе массива strSplit[1]

            //сохраним строку данных фото
            string slideData = ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData[int.Parse(strSplit[1])];

            //удалим строку с данными слайда из списка
            ((SlideScrollStruct)_pag.Session["SlideScrollStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));

            //перезапишем все данные по аним. списку в файле структур аним. списков
            if (!sliderWork.ReplaceOrDeleteSlider((SlideScrollStruct)_pag.Session["SlideScrollStruct"]))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения новых данных по аним. списку фото в файл. Попробуйте повторить.", _pag.Master); return; }
            else { warning.HideWarning(_pag.Master); }

            //удалим фотографию
            string[] slideDataSplit = slideData.Split(new[] { '|' });
            try
            {
                File.Delete(_pag.Server.MapPath("~") + slideDataSplit[1].Replace("../", ""));
            }
            catch { }

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    public class SlideScrollWork
    {
        private Page pag;

        public SlideScrollWork(Page pagenew) { pag = pagenew; }

        /// <summary>функция возвращает все данные по списку(в виде структур), содержащиеся в файле /files/slidescroll/slider, кроме структуры одного списка, переданного в качестве аргумента</summary>
        /// <param name="structForExclude">структура слайдера, которую нужно исключить из возвращаемого списка</param>
        /// <returns></returns>
        public List<SlideScrollStruct> GetAllSliderStruct(SlideScrollStruct structForExclude = null)
        {
            var listResult = new List<SlideScrollStruct>();

            #region КОД

            var structure = new SlideScrollStruct();
            var dataList = new List<string>();
            string[] str, strSplit;
            bool chknewslider = false;
            string excludeId = "";
            if (structForExclude != null) { excludeId = structForExclude.Id.ToString(); }

            string pathtofile = pag.Server.MapPath("~") + @"files\slidescroll\slider";
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string line in str)
                {
                    strSplit = line.Split(new[] { '|' });
                    if (strSplit.Length > 1)
                    {
                        if (strSplit[0] == "sliderStart")
                        {
                            if (strSplit[1] != excludeId)   //если не нужно исключить данные списка, то..
                            {
                                chknewslider = true;
                                structure = new SlideScrollStruct();
                                dataList = new List<string>();

                                structure.Id = int.Parse(strSplit[1]);
                                structure.Name = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.VisibleElemCount = strSplit[4];
                                structure.SsAnimTime = strSplit[5];
                                structure.BtnsAnimTime = strSplit[6];
                                structure.BigImgDivTime = strSplit[7];
                                structure.LittleImageSize = strSplit[8];
                            }
                            else                            //если из результирующего списка нужно исключить слайдер, то..
                            {
                                chknewslider = false;
                            }
                        }
                        else if (strSplit[0] == "sliderEnd")
                        {
                            if (chknewslider)
                            {
                                structure.ListOfData = dataList;
                                listResult.Add(structure);
                            }
                            chknewslider = false;
                        }

                        if (chknewslider)
                        {
                            if (strSplit[0] == "img") { dataList.Add(line); }
                        }
                    }
                }
            }
            else
            {
                //создадим новый файл для структур слайдера
                try { File.WriteAllLines(pathtofile, new List<string>(), Encoding.Default); }
                catch { }
            }

            #endregion

            return listResult;
        }

        /// <summary>функция возвращает сортированный по алфавиту список всех имён списков, которые есть в файле /files/slidescroll/slider</summary>
        /// <returns></returns>
        public List<string> GetListOfAllSliderNames()
        {
            var listresult = new List<string>();

            #region КОД

            string[] str, strSplit;
            string pathtofile = pag.Server.MapPath("~") + @"files\slidescroll\slider";
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string line in str)
                {
                    strSplit = line.Split(new[] { '|' });
                    if (strSplit.Length > 1)
                    {
                        if (strSplit[0] == "sliderStart")
                        {
                            listresult.Add(strSplit[2]);
                        }
                    }
                }
            }
            else
            {
                //создадим новый файл для структур слайдера
                try { File.WriteAllLines(pathtofile, new List<string>(), Encoding.Default); }
                catch { }
            }

            if (listresult.Count > 0) { listresult.Sort(); }

            #endregion

            return listresult;
        }

        /// <summary>функция возвращает структуру данных одного списка по переданному Id списка</summary>
        /// <param name="sliderId">ID списка</param>
        /// <returns></returns>
        public SlideScrollStruct GetSliderStructForId(int sliderId)
        {
            var structure = new SlideScrollStruct();
            structure.Id = -1;                 //это значение будет являться признаком того, что список с таким ID не существует.

            #region КОД

            var dataList = new List<string>();
            string[] str, strSplit;
            bool chkslider = false;

            string pathtofile = pag.Server.MapPath("~") + @"files\slidescroll\slider";
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string line in str)
                {
                    strSplit = line.Split(new[] { '|' });
                    if (strSplit.Length > 1)
                    {
                        if (strSplit[0] == "sliderStart")
                        {
                            if (strSplit[1] == sliderId.ToString())   //если это нужно нам слайд-шоу, то..
                            {
                                chkslider = true;
                                structure = new SlideScrollStruct();
                                dataList = new List<string>();

                                structure.Id = int.Parse(strSplit[1]);
                                structure.Name = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.VisibleElemCount = strSplit[4];
                                structure.SsAnimTime = strSplit[5];
                                structure.BtnsAnimTime = strSplit[6];
                                structure.BigImgDivTime = strSplit[7];
                                structure.LittleImageSize = strSplit[8];
                            }
                            else
                            {
                                chkslider = false;
                            }
                        }
                        else if (strSplit[0] == "sliderEnd")
                        {
                            if (chkslider)
                            {
                                structure.ListOfData = dataList;
                                break;
                            }
                            chkslider = false;
                        }

                        if (chkslider)
                        {
                            if (strSplit[0] == "img") { dataList.Add(line); }
                        }
                    }
                }
            }

            #endregion

            return structure;
        }

        /// <summary>функция возвращает структуру данных одного списка по переданному ИМЕНИ слайд-шоу</summary>
        /// <param name="sliderName">ИМЯ списка</param>
        /// <returns></returns>
        public SlideScrollStruct GetSliderStructForName(string sliderName)
        {
            var structure = new SlideScrollStruct();

            #region КОД

            var dataList = new List<string>();
            string[] str, strSplit;
            bool chkslider = false;

            string pathtofile = pag.Server.MapPath("~") + @"files\slidescroll\slider";
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string line in str)
                {
                    strSplit = line.Split(new[] { '|' });
                    if (strSplit.Length > 1)
                    {
                        if (strSplit[0] == "sliderStart")
                        {
                            if (strSplit[2] == sliderName)   //если это  слайд-шоу нужно нам, то..
                            {
                                chkslider = true;
                                structure = new SlideScrollStruct();
                                dataList = new List<string>();

                                structure.Id = int.Parse(strSplit[1]);
                                structure.Name = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.VisibleElemCount = strSplit[4];
                                structure.SsAnimTime = strSplit[5];
                                structure.BtnsAnimTime = strSplit[6];
                                structure.BigImgDivTime = strSplit[7];
                                structure.LittleImageSize = strSplit[8];
                            }
                            else
                            {
                                chkslider = false;
                            }
                        }
                        else if (strSplit[0] == "sliderEnd")
                        {
                            if (chkslider)
                            {
                                structure.ListOfData = dataList;
                                break;
                            }
                            chkslider = false;
                        }

                        if (chkslider)
                        {
                            if (strSplit[0] == "img") { dataList.Add(line); }
                        }
                    }
                }
            }

            #endregion

            return structure;
        }

        /// <summary>функция возвращает очередной порядковый номер для нового списка</summary>
        /// <returns></returns>
        public int GetIdForNewSlider()
        {
            int newid = 1;

            #region КОД

            var listOfSliderStructs = GetAllSliderStruct();
            if (listOfSliderStructs.Count > 0)
            {
                listOfSliderStructs.Sort((a, b) => a.Id.CompareTo(b.Id)); //сортировка структур по свойству - Id (от 'А до Я')
                newid = listOfSliderStructs[listOfSliderStructs.Count - 1].Id + 1;
            }

            #endregion

            return newid;
        }

        /// <summary>функция заменяет или удаляет данные по одному списку в файле /files/slidescroll/slider. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных списка, которую нужно заменить или удалить в файле</param>
        /// <param name="delete">true - удалить переданную структуру из файла, false - заменить</param>
        /// <returns></returns>
        public bool ReplaceOrDeleteSlider(SlideScrollStruct Struct, bool delete = false)
        {
            string pathtofile = pag.Server.MapPath("~") + @"files\slidescroll\slider";
            string pathtotemp = pag.Server.MapPath("~") + @"files\temp\slider";

            #region КОД ЗАМЕНЫ И УДАЛЕНИЯ ДАННЫХ СЛАЙДЕРА

            var listOfStructs = GetAllSliderStruct(Struct);     //получим список структур всех списков
            if (!delete) { listOfStructs.Add(Struct); }         //если нужно перезаписать данные списка, то добавим в список новую переданную в функцию структуру списка

            //преобразуем список listOfStructs в строковый список listForDBFile, который пригоден для записи в файл
            var listForDbFile = new List<string>();
            foreach (var onestruct in listOfStructs) { listForDbFile.AddRange(onestruct.GetListFromStruct()); }
            listOfStructs.Clear();

            //перезапишем файл /files/slidescroll/slider

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try { fs = new FileStream(pathtofile, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return false; }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try { File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default); }
            catch { return false; }
            try
            {
                try { fs.Close(); fs.Dispose(); }
                catch { }
                File.Copy(pathtotemp + tempFileName, pathtofile, true);
            }
            catch { return false; }
            try { File.Delete(pathtotemp + tempFileName); }
            catch { }

            #endregion

            return true;
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>Класс описывающий структуру данных одного анимированного списка фотографий</summary>
    public class SlideScrollStruct
    {
        public int Id { get; set; }                     //содержит ID списка
        public string Name { get; set; }                //содержит Имя списка
        public string OnOff { get; set; }               //включен или выключен список (может быть on, либо off)
        public string VisibleElemCount { get; set; }    //количестов фотографий в списке, которые будут видимы
        public string SsAnimTime { get; set; }          //длительность анимации одной прокрутки элементов списка (в миллисекундах)
        public string BtnsAnimTime { get; set; }        //длительность анимации изменения прозрачности при наведении на кнопки ВЛЕВО и ВПРАВО (в миллисекундах)
        public string BigImgDivTime { get; set; }       //длительность анимации изменения прозрачности полноразмерного изображения (в миллисекундах)
        public string LittleImageSize { get; set; }     //высота и ширина уменьшенных изображений в списке (в пикселях)
        public List<string> ListOfData { get; set; }    //содержит список строк данных с путями к сохранённым полноразмерным изображениям: img|~/files/slider/4.jpg

        /// <summary>функция возвращает список в формате List/string/ из данной структуры слайд-шоу. 
        /// Строки в списке полностью подготовлены для записи в файл /files/slidescroll/slider.</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();

            list.Add("sliderStart|" + Id + "|" + Name + "|" + OnOff + "|" + VisibleElemCount + "|" + SsAnimTime + "|" + BtnsAnimTime + "|" +
                                      BigImgDivTime + "|" + LittleImageSize);
            list.AddRange(ListOfData);
            list.Add("sliderEnd|");

            return list;
        }
    }

    #endregion

}