using System;
using System.Collections.Generic;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using site.classesHelp;

/* файл с классами для работы со СЛАЙД-ШОУ (файлы в папке files\slider\) */
namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>класс, который формирует html-код, jquery-код и стили для слайдера и возвращает через функцию слайдер</summary>
    public class SlideShowForm
    {
        private Page _pag;
        private string _sliderNum;      //уникальный номер слайдера

        //Конструктор
        public SlideShowForm(Page newPag, string sliderNumnew)
        {
            _pag = newPag;
            _sliderNum = sliderNumnew;
        }

        /// <summary>Метод берёт данные для слайд-шоу из БД и возвращает HTML-код для слайд-шоу (или для слайдера)</summary>
        /// <returns></returns>
        public LiteralControl GetSlideShow()
        {
            //получим структуру данных нужного слайд-шоу по его id (sliderNum)
            var sliderWork = new SlideShowWork(_pag);
            var sliderStruct = sliderWork.GetSliderStructForId(int.Parse(_sliderNum));
            if (sliderStruct.OnOff == "off") { return new LiteralControl(""); }
            if (sliderStruct.SlideShowId == -1) { return new LiteralControl(""); }
            int id = sliderStruct.SlideShowId;
            int interval = int.Parse(sliderStruct.Interval);
            int scrollTime = int.Parse(sliderStruct.ScrollTime);
            int slideHeight = int.Parse(sliderStruct.SlideHeight);
            int slideIconSize = int.Parse(sliderStruct.SlideIconSize);
            bool iconAdd = true;
            if (sliderStruct.OnOffIcons == "off") { iconAdd = false; }
            SlideShowType typ = SlideShowType.Appear;
            if (sliderStruct.Typ == "scrollleft") { typ = SlideShowType.ScrollLeft; }
            if (sliderStruct.Typ == "scrollright") { typ = SlideShowType.ScrollRight; }
            bool onOffIncrease = true;
            if (sliderStruct.OnOffIncrease == "off") { onOffIncrease = false; }
            //цвет фона кнопок внизу слайдера или слайд-шоу
            string iconBackColor = "#B7C1B1";
            string btnBackColor = "#008040";

            string[] strSplit;
            var listOfHtmlStrings = new List<string>();
            var slideWork = new SlideShowWork(null);
            string[] imageSizes;
            foreach (string line in sliderStruct.ListOfData)
            {
                strSplit = line.Split(new[] { '|' });
                if (strSplit[2] == "no_action")
                {
                    imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + strSplit[1].Replace("../", ""), slideHeight);
                    listOfHtmlStrings.Add("<img src='" + strSplit[1] + "' style='width:" + imageSizes[0] + "px; height: " +
                                            slideHeight + "px;' " + "alt='фото слайд-шоу' />");
                }
                else
                {
                    imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + strSplit[1].Replace("../", ""), slideHeight);
                    listOfHtmlStrings.Add("<img src='" + strSplit[1] + "' " + "style='width:" + imageSizes[0] + "px; height: " +
                                            slideHeight + "px; cursor: pointer;' " + "alt='фото слайд-шоу' onclick=\"location.href='/default.aspx?p=" +
                                            strSplit[2].Trim() + "'\" />");
                }
            }

            if (sliderStruct.OnOffSliderMode == "off")      //если режим слайдера выключен, то функция вернёт слайд-шоу
            {
                return Slide_Show(listOfHtmlStrings, typ, scrollTime, interval, slideHeight, slideIconSize, iconAdd, iconBackColor, btnBackColor, id);
            }
            else if (sliderStruct.OnOffSliderMode == "on")  //если режим слайдера включён, то функция вернёт слайдер
            {
                return GetSlider(listOfHtmlStrings, scrollTime, slideHeight, slideIconSize, iconAdd, onOffIncrease, iconBackColor, id);
            }

            return new LiteralControl("");
        }

        /// <summary>Функция возвращает HTML-код слайдшоу; добавляет jQuery код для слайдшоу на страницу; добавляет стили для слайдшоу на страницу</summary>
        /// <param name="slideList">список строк с готовым html-кодом содержимого слайдов (обязательно должны содержать теги img)</param>
        /// <param name="typ">перечисление, которое устанавливает тип возвращаемого слайдшоу</param>
        /// <param name="scrollTime">время прокрутки слайда</param>
        /// <param name="interval">время между прокрутками</param>
        /// <param name="slideHeight">высота картинок слайдов</param>
        /// <param name="slideIconSize">высота и ширина(одинаковые) иконок слайдов внизу слайдера</param>
        /// <param name="iconAdd">true - если нужно добавлять иконки внизу, false - если не надо</param>
        /// <param name="iconBackColor">шестнадцатиричный код цвета фона кнопок-ссылок с иконками внизу</param>
        /// <param name="btnBckColor">шестнадцатиричный код цвета фона кнопок-ссылок внизу</param>
        /// <param name="uniqueNum">уникальный номер(для данной html-страницы), который будет использоваться для обозначения стилей, переменных и т.п. для одного слайда</param>
        /// <returns></returns>
        private LiteralControl Slide_Show(List<string> slideList, SlideShowType typ, int scrollTime, int interval, int slideHeight, int slideIconSize, bool iconAdd, string iconBackColor, string btnBckColor, int uniqueNum)
        {
            var htmlString = new StringBuilder();

            #region Формирование HTML-кода

            //сделаем объявления для определения размеры картинок для кнопок
            var slideWork = new SlideShowWork(null);
            string[] imageSizes;

            htmlString.Append("<div id='divMainSSWrap" + uniqueNum + "'>");
            htmlString.Append("<div id='divSSWrap" + uniqueNum + "'>");
            foreach (string line in slideList)
            {
                htmlString.Append("<div>" + line + "</div>");
            }
            htmlString.Append("</div>");
            if (iconAdd) //если нужно добавить иконки слайдов внизу
            {
                htmlString.Append("<div id='divSSIconsWrap" + uniqueNum + "'></div>");
            }
            imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + @"files/slider/btnPause.png", 1);
            htmlString.Append("<img id='btnSSPlayPause" + uniqueNum + "' src='files/slider/btnPause.png' alt='pause' " +
                               "style='width:" + imageSizes[1] + "px; height:" + imageSizes[2] + "px;' " + "/>");
            htmlString.Append("</div>");

            #endregion

            var jQueryString = new StringBuilder();

            #region Формирование jQuery-кода и добавление его в конец страницы

            jQueryString.Append("<noindex><script type='text/javascript'>");
            jQueryString.Append("$(document).ready(function () { ");

            jQueryString.Append("var scrollTimeSs" + uniqueNum + " = " + scrollTime + "; ");
            jQueryString.Append("var ssIterval" + uniqueNum + " = " + interval + "; ");
            string typeOfSlideShow = "";
            if (typ == SlideShowType.Appear) { typeOfSlideShow = "appear"; }
            else if (typ == SlideShowType.ScrollLeft) { typeOfSlideShow = "scrollLeft"; }
            else if (typ == SlideShowType.ScrollRight) { typeOfSlideShow = "scrollRight"; }
            jQueryString.Append("var typeOfSs" + uniqueNum + " = '" + typeOfSlideShow + "'; ");
            jQueryString.Append("var $slideArraySs" + uniqueNum + " = $('#divSSWrap" + uniqueNum + " div'); ");
            jQueryString.Append("var clickabledSs" + uniqueNum + " = true; ");
            jQueryString.Append("var maxSlideWidthSs" + uniqueNum + " = 0; ");
            jQueryString.Append("var tempWidthSs" + uniqueNum + "; ");
            jQueryString.Append("var slideCountSs" + uniqueNum + " = $slideArraySs" + uniqueNum + ".length; ");
            jQueryString.Append("var counterSs" + uniqueNum + " = 0; ");

            jQueryString.Append("$slideArraySs" + uniqueNum + ".each(function () { ");
            jQueryString.Append("tempWidthSs" + uniqueNum + " = $(this).children().filter('img').width(); ");
            jQueryString.Append("if (tempWidthSs" + uniqueNum + " > maxSlideWidthSs" + uniqueNum + ") { maxSlideWidthSs" + uniqueNum + " = tempWidthSs" + uniqueNum + "; } ");
            jQueryString.Append("$('#divSSIconsWrap" + uniqueNum + "').append(\"<div name='\" + counterSs" + uniqueNum + " + \"'></div>\"); ");
            jQueryString.Append("counterSs" + uniqueNum + "++; ");
            jQueryString.Append("}); ");

            jQueryString.Append("var $iconArraySs" + uniqueNum + " = $('#divSSIconsWrap" + uniqueNum + " div'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[0]).css('background-color', '" + btnBckColor + "'); ");
            jQueryString.Append("$('#divSSWrap" + uniqueNum + "').width(maxSlideWidthSs" + uniqueNum + "); ");
            jQueryString.Append("$('#divSSWrap" + uniqueNum + "').height($slideArraySs" + uniqueNum + ".first().height()); ");
            jQueryString.Append("$('#divMainSSWrap" + uniqueNum + "').width($('#divSSWrap" + uniqueNum + "').outerWidth(true)); ");

            jQueryString.Append("var topWSs" + uniqueNum + " = $('#divSSWrap" + uniqueNum + "').offset().top; ");
            jQueryString.Append("var leftWSs" + uniqueNum + " = $('#divSSWrap" + uniqueNum + "').offset().left; ");
            jQueryString.Append("$slideArraySs" + uniqueNum + ".each(function () { ");
            jQueryString.Append("$(this).offset({ top: 6 + topWSs" + uniqueNum + ", left: 6 + (leftWSs" + uniqueNum + " + maxSlideWidthSs" + uniqueNum + " / 2 - $(this).width() / 2) }); ");
            jQueryString.Append("if (slideCountSs" + uniqueNum + " != 0) { ");
            jQueryString.Append("$(this).css('z-index', '2'); ");
            jQueryString.Append("slideCountSs" + uniqueNum + " = 0; ");
            jQueryString.Append("} ");
            jQueryString.Append("else { ");
            jQueryString.Append("$(this).css('z-index', '1'); ");
            jQueryString.Append("$(this).hide(); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");
            jQueryString.Append("slideCountSs" + uniqueNum + " = $slideArraySs" + uniqueNum + ".length; ");

            jQueryString.Append("var $btnPlayPause" + uniqueNum + " = $('#btnSSPlayPause" + uniqueNum + "'); ");
            jQueryString.Append("$btnPlayPause" + uniqueNum + ".offset({ top: topWSs" + uniqueNum + " + $('#divSSWrap" + uniqueNum + "').outerHeight(true) - $btnPlayPause" + uniqueNum + ".height(), left: leftWSs" + uniqueNum + " + $('#divSSWrap" + uniqueNum + "').outerWidth(true) - $btnPlayPause" + uniqueNum + ".width() }); ");

            jQueryString.Append("var visibleSlideNumSs" + uniqueNum + " = 0; ");

            jQueryString.Append("function scrollLeft" + uniqueNum + "() { ");
            jQueryString.Append("if (slideCountSs" + uniqueNum + " > 1 && clickabledSs" + uniqueNum + ") { ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = false; ");
            jQueryString.Append("var activeIndex = visibleSlideNumSs" + uniqueNum + "; ");
            jQueryString.Append("if (visibleSlideNumSs" + uniqueNum + " == slideCountSs" + uniqueNum + " - 1) { visibleSlideNumSs" + uniqueNum + " = 0; } else { visibleSlideNumSs" + uniqueNum + " += 1; }; ");
            jQueryString.Append("var $obj = $slideArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]; ");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show().animate({ opacity: 1 }, { queue: false, duration: scrollTimeSs" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArraySs" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("var topS = $($obj).offset().top; ");
            jQueryString.Append("var leftS = $($obj).offset().left; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("left: '-' + ($($obj).width() + 5) + 'px' ");
            jQueryString.Append("}, scrollTimeSs" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).offset({ top: topS, left: leftS }); ");
            jQueryString.Append("$($obj).css('z-index', '1'); ");
            jQueryString.Append("$($obj).hide(); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]).css('background-color', '" + btnBckColor + "'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[activeIndex]).css('background-color', '" + iconBackColor + "'); ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}; ");

            jQueryString.Append("function scrollRight" + uniqueNum + "() { ");
            jQueryString.Append("if (slideCountSs" + uniqueNum + " > 1 && clickabledSs" + uniqueNum + ") { ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = false; ");
            jQueryString.Append("var activeIndex = visibleSlideNumSs" + uniqueNum + "; ");
            jQueryString.Append("var preIndex = 0; ");
            jQueryString.Append("if (visibleSlideNumSs" + uniqueNum + " - 1 < 0) { ");
            jQueryString.Append("preIndex = slideCountSs" + uniqueNum + " - 1; ");
            jQueryString.Append("visibleSlideNumSs" + uniqueNum + " = slideCountSs" + uniqueNum + " - 1; ");
            jQueryString.Append("} else { ");
            jQueryString.Append("preIndex = visibleSlideNumSs" + uniqueNum + " - 1; ");
            jQueryString.Append("visibleSlideNumSs" + uniqueNum + " -= 1; ");
            jQueryString.Append("}; ");
            jQueryString.Append("var $obj = $slideArraySs" + uniqueNum + "[preIndex]; ");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show() ");
            jQueryString.Append(".css('left', '-' + ($($obj).width() + 5) + 'px').css('opacity', '1') ");
            jQueryString.Append(".animate({ left: (5 + (maxSlideWidthSs" + uniqueNum + " / 2 - $($obj).width() / 2)) + 'px' }, { queue: false, duration: scrollTimeSs" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArraySs" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("opacity: 0 ");
            jQueryString.Append("}, scrollTimeSs" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).css('z-index', '1').hide().css('opacity', '1'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]).css('background-color', '" + btnBckColor + "'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[activeIndex]).css('background-color', '" + iconBackColor + "'); ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}; ");

            jQueryString.Append("function appear" + uniqueNum + "() { ");
            jQueryString.Append("if (slideCountSs" + uniqueNum + " > 1 && clickabledSs" + uniqueNum + ") { ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = false; ");
            jQueryString.Append("var activeIndex = visibleSlideNumSs" + uniqueNum + "; ");
            jQueryString.Append("if (visibleSlideNumSs" + uniqueNum + " == slideCountSs" + uniqueNum + " - 1) { visibleSlideNumSs" + uniqueNum + " = 0; } else { visibleSlideNumSs" + uniqueNum + " += 1; } ");
            jQueryString.Append("var $obj = $slideArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]; ");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show().animate({ opacity: 1 }, { queue: false, duration: scrollTimeSs" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArraySs" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("opacity: 0 ");
            jQueryString.Append("}, scrollTimeSs" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).css('z-index', '1').hide().css('opacity', '1'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]).css('background-color', '" + btnBckColor + "'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[activeIndex]).css('background-color', '" + iconBackColor + "'); ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}; ");

            jQueryString.Append("$iconArraySs" + uniqueNum + ".each(function () { ");
            jQueryString.Append("var indexOfIcon = parseInt($(this).attr('name')); ");
            jQueryString.Append("$(this).bind('click', function () { ");
            jQueryString.Append("if (indexOfIcon != visibleSlideNumSs" + uniqueNum + ") { iconClickSs" + uniqueNum + "(indexOfIcon); }; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}); ");

            jQueryString.Append("function iconClickSs" + uniqueNum + "(index) { ");
            jQueryString.Append("if (clickabledSs" + uniqueNum + ") { ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = false; ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[index]).css('background-color', '" + btnBckColor + "'); ");
            jQueryString.Append("$($iconArraySs" + uniqueNum + "[visibleSlideNumSs" + uniqueNum + "]).css('background-color', '" + iconBackColor + "'); ");
            jQueryString.Append("var activeIndex = index; ");
            jQueryString.Append("var preIndex = visibleSlideNumSs" + uniqueNum + "; ");
            jQueryString.Append("visibleSlideNumSs" + uniqueNum + " = index; ");
            jQueryString.Append("var $obj = $slideArraySs" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show() ");
            jQueryString.Append(".animate({ opacity: 1 }, { queue: false, duration: scrollTimeSs" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArraySs" + uniqueNum + "[preIndex]; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("opacity: 0 ");
            jQueryString.Append("}, scrollTimeSs" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).css('z-index', '1').hide().css('opacity', '1'); ");
            jQueryString.Append("clickabledSs" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}; ");

            jQueryString.Append("$btnPlayPause" + uniqueNum + ".click(function () { ");
            jQueryString.Append("var pp = $btnPlayPause" + uniqueNum + ".attr('alt'); ");
            jQueryString.Append("if (pp == 'pause') { ");
            jQueryString.Append("ssStop" + uniqueNum + "(); ");
            jQueryString.Append("} ");
            jQueryString.Append("else if (pp == 'play') { ");
            jQueryString.Append("ssStart" + uniqueNum + "(); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");

            //jQueryString.Append("window.onblur = ssStop" + uniqueNum + "; ");
            //jQueryString.Append("window.onfocus = ssStart" + uniqueNum + "; ");
            jQueryString.Append("ssStart" + uniqueNum + "(); ");

            jQueryString.Append("var ssInterv" + uniqueNum + "; ");

            jQueryString.Append("function ssStart" + uniqueNum + "() { ");
            jQueryString.Append("window.clearInterval(ssInterv" + uniqueNum + "); ");
            jQueryString.Append("ssInterv" + uniqueNum + " = window.setInterval(function () { ");
            jQueryString.Append("if (typeOfSs" + uniqueNum + " == 'scrollLeft') { scrollLeft" + uniqueNum + "(); } ");
            jQueryString.Append("else if (typeOfSs" + uniqueNum + " == 'scrollRight') { scrollRight" + uniqueNum + "(); } ");
            jQueryString.Append("else if (typeOfSs" + uniqueNum + " == 'appear') { appear" + uniqueNum + "(); }; ");
            jQueryString.Append("}, ssIterval" + uniqueNum + "); ");
            jQueryString.Append("$btnPlayPause" + uniqueNum + ".attr('alt', 'pause'); ");
            jQueryString.Append("$btnPlayPause" + uniqueNum + ".attr('src', '../files/slider/btnPause.png'); ");
            jQueryString.Append("}; ");

            jQueryString.Append("function ssStop" + uniqueNum + "() { ");
            jQueryString.Append("window.clearInterval(ssInterv" + uniqueNum + "); ");
            jQueryString.Append("$btnPlayPause" + uniqueNum + ".attr('alt', 'play'); ");
            jQueryString.Append("$btnPlayPause" + uniqueNum + ".attr('src', '../files/slider/btnPlay.png'); ");
            jQueryString.Append("}; ");

            jQueryString.Append("}); ");
            jQueryString.Append("</script></noindex> ");

            _pag.Header.Controls.Add(new LiteralControl(jQueryString.ToString()));

            #endregion

            var cssString = new StringBuilder();

            #region Формирование css-кода и добавление его в шапку страницы

            cssString.Append("<style type='text/css'> ");
            cssString.Append("#divMainSSWrap" + uniqueNum + " { ");
            cssString.Append("position: relative; display: block; ");
            cssString.Append("margin: 5px auto; ");
            cssString.Append("} ");

            cssString.Append("#divSSWrap" + uniqueNum + " { ");
            cssString.Append("position: relative; display: block; ");
            cssString.Append("padding: 5px; ");
            cssString.Append("-ms-border-radius: 4px; border-radius: 4px; ");
            //cssString.Append("border: 1px #ffffff solid; ");
            //cssString.Append("background-color: #dcdcdc; ");
            //cssString.Append("-webkit-box-shadow: 0 0 12px #000000; -webkit-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            //cssString.Append("-ms-box-shadow: 0 0 12px #000000; -ms-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            //cssString.Append("box-shadow: 0 0 12px #000000; box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            cssString.Append("overflow: hidden; ");
            cssString.Append("} ");

            cssString.Append("#divSSWrap" + uniqueNum + " div { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("margin: 0; padding: 0; border: 0 transparent none; ");
            cssString.Append("} ");

            cssString.Append("#btnSSPlayPause" + uniqueNum + " { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("z-index: 4; ");
            cssString.Append("cursor: pointer; ");
            cssString.Append("-webkit-transition: all .3s; ");
            cssString.Append("-moz-transition: all .3s; ");
            cssString.Append("-o-transition: all .3s; ");
            cssString.Append("transition: all .3s; ");
            cssString.Append("} ");

            cssString.Append("#btnSSPlayPause" + uniqueNum + ":hover { ");
            cssString.Append("-ms-opacity: 1; opacity: 1; ");
            cssString.Append("} ");

            cssString.Append("#btnSSPlayPause" + uniqueNum + ":active { ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("} ");

            if (iconAdd)
            {
                cssString.Append("#divSSIconsWrap" + uniqueNum + " { ");
                cssString.Append("margin: 5px 0 0 0; ");
                cssString.Append("} ");

                cssString.Append("#divSSIconsWrap" + uniqueNum + " div { ");
                cssString.Append("position: relative; display: inline-block; ");
                cssString.Append("width: " + slideIconSize + "px; height: " + slideIconSize + "px;");
                cssString.Append("margin: 3px; ");
                cssString.Append("background-color: " + iconBackColor + "; ");
                cssString.Append("cursor: pointer; ");
                cssString.Append("-webkit-box-shadow: 2px 2px 6px #000000; -webkit-box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("-ms-box-shadow: 2px 2px 6px #000000; -ms-box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("box-shadow: 2px 2px 6px #000000; box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("-webkit-transition: all .3s; ");
                cssString.Append("-moz-transition: all .3s; ");
                cssString.Append("-o-transition: all .3s; ");
                cssString.Append("transition: all .3s; ");
                cssString.Append("} ");

                cssString.Append("#divSSIconsWrap" + uniqueNum + " div:hover { ");
                cssString.Append("background-color: " + btnBckColor + "; ");
                cssString.Append("} ");
            }

            cssString.Append("</style> ");

            _pag.Header.Controls.Add(new LiteralControl(cssString.ToString()));

            #endregion

            return new LiteralControl(htmlString.ToString());
        }

        /// <summary>Функция возвращает HTML-код слайдера; добавляет jQuery код для слайдера на страницу; добавляет стили для слайдера на страницу</summary>
        /// <param name="slideList">список строк с готовым html-кодом содержимого слайдов (обязательно должны содержать теги img)</param>
        /// <param name="scrollTime">время прокрутки слайда</param>
        /// <param name="slideIconHeight">высота иконок слайдов внизу слайдера</param>
        /// <param name="iconAdd">true - если нужно добавлять иконки слайдов внизу, false - если не надо</param>
        /// <param name="slideIncrease">true - если нужно добавить возможность просмотра полноразмерного слайда, false - если не надо</param>
        /// <param name="iconBackColor">шестнадцатиричный код цвета фона кнопок-ссылок внизу</param>
        /// <param name="uniqueNum">уникальный номер(для данной html-страницы), который будет использоваться для обозначения стилей, переменных и т.п. для одного слайда</param>
        /// <param name="slideHeight">высота картинок слайдов</param>
        /// <returns></returns>
        public LiteralControl GetSlider(List<string> slideList, int scrollTime, int slideHeight, int slideIconHeight, bool iconAdd, bool slideIncrease, string iconBackColor, int uniqueNum)
        {
            var htmlString = new StringBuilder();

            #region Формирование HTML-кода

            //сделаем объявления для определения размеры картинок для кнопок
            var slideWork = new SlideShowWork(null);
            string[] imageSizes;

            htmlString.Append("<div id='divMainWrap" + uniqueNum + "'>");
            imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + @"files/slider/btnLeft.png", 1);
            htmlString.Append("<img id='btnSlideScrollLeft" + uniqueNum + "' src='files/slider/btnLeft.png' alt='Left' " + "style='width:" + imageSizes[1] + "px; height:" + imageSizes[2] + "px;' " + "/>");
            htmlString.Append("<div id='divSlideWrap" + uniqueNum + "'>");
            foreach (string line in slideList)
            {
                htmlString.Append("<div>" + line + "</div>");
            }
            htmlString.Append("</div>");
            if (iconAdd) //если нужно добавить иконки слайдов внизу
            {
                htmlString.Append("<div id='divIconsWrap" + uniqueNum + "'></div>");
            }
            if (slideIncrease)
            {
                imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + @"files/slider/btnIncrease.png", 1);
                htmlString.Append("<img id='btnSlideIncrease" + uniqueNum + "' src='files/slider/btnIncrease.png' alt='Increase' " + "style='width:" + imageSizes[1] + "px; height:" + imageSizes[2] + "px;' " + "/>");
            }
            imageSizes = slideWork.GetImageParam(_pag.Server.MapPath("~") + @"files/slider/btnRight.png", 1);
            htmlString.Append("<img id='btnSlideScrollRight" + uniqueNum + "' src='files/slider/btnRight.png' alt='Right' " + "style='width:" + imageSizes[1] + "px; height:" + imageSizes[2] + "px;' " + "/>");
            htmlString.Append("</div>");

            #endregion

            var jQueryString = new StringBuilder();

            #region Формирование jQuery-кода и добавление его в конец страницы

            jQueryString.Append("<script type='text/javascript'>");
            jQueryString.Append("$(document).ready(function () { ");
            jQueryString.Append("var scrollTime" + uniqueNum + " = " + scrollTime + "; ");
            jQueryString.Append("var $slideArray" + uniqueNum + " = $('#divSlideWrap" + uniqueNum + " div'); ");
            jQueryString.Append("var clickabled" + uniqueNum + " = true; ");
            jQueryString.Append("var maxSlideWidth" + uniqueNum + " = 0; ");
            jQueryString.Append("var tempWidth" + uniqueNum + "; ");
            jQueryString.Append("var slideCount" + uniqueNum + " = $slideArray" + uniqueNum + ".length; ");
            if (iconAdd) { jQueryString.Append("var counter" + uniqueNum + " = 0; "); }
            jQueryString.Append("$slideArray" + uniqueNum + ".each(function () { ");
            jQueryString.Append("tempWidth" + uniqueNum + " = $(this).children().filter('img').width(); ");
            jQueryString.Append("if (tempWidth" + uniqueNum + " > maxSlideWidth" + uniqueNum + ") { maxSlideWidth" + uniqueNum + " = tempWidth" + uniqueNum + "; }; ");
            if (iconAdd)
            {
                jQueryString.Append("$('#divIconsWrap" + uniqueNum + "').append(\"<div name='\" + counter" + uniqueNum + " + \"'><img src='\" + $(this).children().filter('img').attr('src') + \"' /></div>\"); ");
                jQueryString.Append("counter" + uniqueNum + "++; ");
            }
            jQueryString.Append("}); ");
            if (iconAdd)
            {
                jQueryString.Append("var $iconArray" + uniqueNum + " = $('#divIconsWrap" + uniqueNum + " div'); ");
                jQueryString.Append("$($iconArray" + uniqueNum + "[0]).css('border', '5px #a52a2a solid'); ");
            }
            jQueryString.Append("$('#divSlideWrap" + uniqueNum + "').width(maxSlideWidth" + uniqueNum + "); ");
            jQueryString.Append("$('#divSlideWrap" + uniqueNum + "').height($slideArray" + uniqueNum + ".first().height()); ");
            jQueryString.Append("$('#divMainWrap" + uniqueNum + "').width($('#divSlideWrap" + uniqueNum + "').outerWidth(true)); ");
            jQueryString.Append("var topW" + uniqueNum + " = $('#divSlideWrap" + uniqueNum + "').offset().top; ");
            jQueryString.Append("var leftW" + uniqueNum + " = $('#divSlideWrap" + uniqueNum + "').offset().left; ");
            jQueryString.Append("$slideArray" + uniqueNum + ".each(function () { ");
            jQueryString.Append("$(this).offset({ top: 6 + topW" + uniqueNum + ", left: 6 + (leftW" + uniqueNum + " + maxSlideWidth" + uniqueNum + " / 2 - $(this).width() / 2) }); ");
            jQueryString.Append("if (slideCount" + uniqueNum + " != 0) { ");
            jQueryString.Append("$(this).css('z-index', '2'); ");
            jQueryString.Append("slideCount" + uniqueNum + " = 0; ");
            jQueryString.Append("} ");
            jQueryString.Append("else { ");
            jQueryString.Append("$(this).css('z-index', '1'); ");
            jQueryString.Append("$(this).hide(); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");
            jQueryString.Append("slideCount" + uniqueNum + " = $slideArray" + uniqueNum + ".length; ");
            jQueryString.Append("var $btnL" + uniqueNum + " = $('#btnSlideScrollLeft" + uniqueNum + "'); ");
            jQueryString.Append("var $btnR" + uniqueNum + " = $('#btnSlideScrollRight" + uniqueNum + "'); ");
            if (slideIncrease)
            {
                jQueryString.Append("var $btnIncrease" + uniqueNum + " = $('#btnSlideIncrease" + uniqueNum + "'); ");
            }
            jQueryString.Append("$btnL" + uniqueNum + ".offset({ top: topW" + uniqueNum + " + $('#divSlideWrap" + uniqueNum + "').outerHeight(true) / 2 - $btnL" + uniqueNum + ".height() / 2, left: leftW" + uniqueNum + " }); ");
            jQueryString.Append("$btnR" + uniqueNum + ".offset({ top: topW" + uniqueNum + " + $('#divSlideWrap" + uniqueNum + "').outerHeight(true) / 2 - $btnR" + uniqueNum + ".height() / 2, left: leftW" + uniqueNum + " + $('#divSlideWrap" + uniqueNum + "').outerWidth(true) - $btnR" + uniqueNum + ".width() }); ");
            if (slideIncrease)
            {
                jQueryString.Append("$btnIncrease" + uniqueNum + ".offset({ top: topW" + uniqueNum + " + $('#divSlideWrap" + uniqueNum + "').outerHeight(true) - $btnIncrease" + uniqueNum + ".height(), left: leftW" + uniqueNum + " + $('#divSlideWrap" + uniqueNum + "').outerWidth(true) - $btnIncrease" + uniqueNum + ".width() }); ");
            }
            jQueryString.Append("var visibleSlideNum" + uniqueNum + " = 0; ");
            jQueryString.Append("$('#btnSlideScrollRight" + uniqueNum + "').click(function () { ");
            jQueryString.Append("if (slideCount" + uniqueNum + " > 1 && clickabled" + uniqueNum + ") { ");
            jQueryString.Append("clickabled" + uniqueNum + " = false; ");
            jQueryString.Append("var activeIndex = visibleSlideNum" + uniqueNum + "; ");
            jQueryString.Append("if (visibleSlideNum" + uniqueNum + " == slideCount" + uniqueNum + " - 1) { visibleSlideNum" + uniqueNum + " = 0; } else { visibleSlideNum" + uniqueNum + " += 1; }; ");
            jQueryString.Append("var $obj = $slideArray" + uniqueNum + "[visibleSlideNum" + uniqueNum + "]; ");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show().animate({ opacity: 1 }, { queue: false, duration: scrollTime" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArray" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("var topS = $($obj).offset().top; ");
            jQueryString.Append("var leftS = $($obj).offset().left; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("left: '-' + ($($obj).width() + 5) + 'px' ");
            jQueryString.Append("}, scrollTime" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).offset({ top: topS, left: leftS }); ");
            jQueryString.Append("$($obj).css('z-index', '1'); ");
            jQueryString.Append("$($obj).hide(); ");
            if (iconAdd)
            {
                jQueryString.Append("$($iconArray" + uniqueNum + "[visibleSlideNum" + uniqueNum + "]).css('border', '5px #a52a2a solid'); ");
                jQueryString.Append("$($iconArray" + uniqueNum + "[activeIndex]).css('border', '5px " + iconBackColor + " solid'); ");
            }
            jQueryString.Append("clickabled" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");
            jQueryString.Append("$('#btnSlideScrollLeft" + uniqueNum + "').click(function () { ");
            jQueryString.Append("if (slideCount" + uniqueNum + " > 1 && clickabled" + uniqueNum + ") { ");
            jQueryString.Append("clickabled" + uniqueNum + " = false; ");
            jQueryString.Append("var activeIndex = visibleSlideNum" + uniqueNum + "; ");
            jQueryString.Append("var preIndex = 0; ");
            jQueryString.Append("if (visibleSlideNum" + uniqueNum + " - 1 < 0) { ");
            jQueryString.Append("preIndex = slideCount" + uniqueNum + " - 1; ");
            jQueryString.Append("visibleSlideNum" + uniqueNum + " = slideCount" + uniqueNum + " - 1; ");
            jQueryString.Append("} else { ");
            jQueryString.Append("preIndex = visibleSlideNum" + uniqueNum + " - 1;");
            jQueryString.Append("visibleSlideNum" + uniqueNum + " -= 1;");
            jQueryString.Append("};");
            jQueryString.Append("var $obj = $slideArray" + uniqueNum + "[preIndex];");
            jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show()");
            jQueryString.Append(".css('left', '-' + ($($obj).width() + 5) + 'px').css('opacity', '1')");
            jQueryString.Append(".animate({ left: (5 + (maxSlideWidth" + uniqueNum + " / 2 - $($obj).width() / 2)) + 'px' }, { queue: false, duration: scrollTime" + uniqueNum + " }); ");
            jQueryString.Append("$obj = $slideArray" + uniqueNum + "[activeIndex]; ");
            jQueryString.Append("$($obj).animate({ ");
            jQueryString.Append("opacity: 0 ");
            jQueryString.Append("}, scrollTime" + uniqueNum + ", function () { ");
            jQueryString.Append("$($obj).css('z-index', '1').hide().css('opacity', '1'); ");
            if (iconAdd)
            {
                jQueryString.Append("$($iconArray" + uniqueNum + "[visibleSlideNum" + uniqueNum + "]).css('border', '5px #a52a2a solid'); ");
                jQueryString.Append("$($iconArray" + uniqueNum + "[activeIndex]).css('border', '5px " + iconBackColor + " solid'); ");
            }
            jQueryString.Append("clickabled" + uniqueNum + " = true; ");
            jQueryString.Append("}); ");
            jQueryString.Append("}; ");
            jQueryString.Append("}); ");
            if (iconAdd)
            {
                jQueryString.Append("$iconArray" + uniqueNum + ".each(function () { ");
                jQueryString.Append("var indexOfIcon" + uniqueNum + " = parseInt($(this).attr('name')); ");
                jQueryString.Append("$(this).bind('click', function () { ");
                jQueryString.Append("if (indexOfIcon" + uniqueNum + " != visibleSlideNum" + uniqueNum + ") { iconClick" + uniqueNum + "(indexOfIcon" + uniqueNum + "); } ");
                jQueryString.Append("}); ");
                jQueryString.Append("}); ");
                jQueryString.Append("function iconClick" + uniqueNum + "(index) { ");
                jQueryString.Append("if (clickabled" + uniqueNum + ") { ");
                jQueryString.Append("clickabled" + uniqueNum + " = false; ");
                jQueryString.Append("$($iconArray" + uniqueNum + "[index]).css('border', '5px #a52a2a solid');");
                jQueryString.Append("$($iconArray" + uniqueNum + "[visibleSlideNum" + uniqueNum + "]).css('border', '5px " + iconBackColor + " solid'); ");
                jQueryString.Append("var activeIndex = index; ");
                jQueryString.Append("var preIndex = visibleSlideNum" + uniqueNum + "; ");
                jQueryString.Append("visibleSlideNum" + uniqueNum + " = index; ");
                jQueryString.Append("var $obj = $slideArray" + uniqueNum + "[activeIndex]; ");
                jQueryString.Append("$($obj).css('z-index', '2').css('opacity', '0').show() ");
                jQueryString.Append(".animate({ opacity: 1 }, { queue: false, duration: scrollTime" + uniqueNum + " }); ");
                jQueryString.Append("$obj = $slideArray" + uniqueNum + "[preIndex]; ");
                jQueryString.Append("$($obj).animate({ ");
                jQueryString.Append("opacity: 0 ");
                jQueryString.Append("}, scrollTime" + uniqueNum + ", function () { ");
                jQueryString.Append("$($obj).css('z-index', '1').hide().css('opacity', '1'); ");
                jQueryString.Append("clickabled" + uniqueNum + " = true; ");
                jQueryString.Append("}); ");
                jQueryString.Append("}; ");
                jQueryString.Append("}; ");
            }
            if (slideIncrease)
            {
                jQueryString.Append("$btnIncrease" + uniqueNum + ".bind('click', function () { ");
                jQueryString.Append("increaseSlide" + uniqueNum + "(); ");
                jQueryString.Append("}); ");
                jQueryString.Append("function increaseSlide" + uniqueNum + "() { ");
                jQueryString.Append("var pathToImg = $($slideArray" + uniqueNum + "[visibleSlideNum" + uniqueNum + "]).children().filter('img').attr('src'); ");
                jQueryString.Append("var htmlString = \"<table id='slideOpacTbl" + uniqueNum + "'><tr><td><div id='divSlideWrapOpac" + uniqueNum + "'><img id='imgSlide" + uniqueNum + "' src='\" + pathToImg + ");
                jQueryString.Append("\"' alt='Image' /><img id='btnSlideClose" + uniqueNum + "' src='../files/slider/btnClose.png' alt='Close' /></div></td></tr></table>\"; ");
                jQueryString.Append("$('body').prepend(htmlString); ");
                jQueryString.Append("var $tbl = $('#slideOpacTbl" + uniqueNum + "'); ");
                jQueryString.Append("$tbl.css('opacity', '0'); ");
                jQueryString.Append("var $btnClose = $('#btnSlideClose" + uniqueNum + "'); ");
                jQueryString.Append("$btnClose.hide(); ");
                jQueryString.Append("var $img = $('#imgSlide" + uniqueNum + "'); ");
                jQueryString.Append("var imgRealWidth = $img.width(); ");
                jQueryString.Append("var imgRealHeight = $img.height(); ");
                jQueryString.Append("$img.width(imgRealWidth / 2); ");
                jQueryString.Append("$img.height(imgRealHeight / 2); ");
                jQueryString.Append("var $imgWrap = $('#divSlideWrapOpac" + uniqueNum + "'); ");
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
                jQueryString.Append("opacClose" + uniqueNum + "(); ");
                jQueryString.Append("}); ");
                jQueryString.Append("}); ");
                jQueryString.Append("function opacClose" + uniqueNum + "() { ");
                jQueryString.Append("$btnClose.unbind('click'); ");
                jQueryString.Append("$tbl.animate({ ");
                jQueryString.Append("opacity: 0 ");
                jQueryString.Append("}, 200, function () { ");
                jQueryString.Append("$tbl.remove(); ");
                jQueryString.Append("}); ");
                jQueryString.Append("}; ");
                jQueryString.Append("}; ");
            }
            jQueryString.Append("}); ");
            jQueryString.Append("</script> ");

            _pag.Header.Controls.Add(new LiteralControl(jQueryString.ToString()));

            #endregion

            var cssString = new StringBuilder();

            #region Формирование css-кода и добавление его в шапку страницы

            cssString.Append("<style type='text/css'> ");
            cssString.Append("#divMainWrap" + uniqueNum + " { ");
            cssString.Append("position: relative; display: block; ");
            cssString.Append("margin: 5px auto; ");
            cssString.Append("} ");

            cssString.Append("#divSlideWrap" + uniqueNum + " { ");
            cssString.Append("position: relative; display: block; ");
            cssString.Append("padding: 5px; ");
            cssString.Append("-ms-border-radius: 4px; border-radius: 4px; ");
            //cssString.Append("border: 1px #ffffff solid; ");
            cssString.Append("background-color: #dcdcdc; ");
            cssString.Append("-webkit-box-shadow: 0 0 12px #000000; -webkit-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            cssString.Append("-ms-box-shadow: 0 0 12px #000000; -ms-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            cssString.Append("box-shadow: 0 0 12px #000000; box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
            cssString.Append("overflow: hidden; ");
            cssString.Append("} ");

            cssString.Append("#divSlideWrap" + uniqueNum + " div { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("margin: 0; padding: 0; border: 0 transparent none; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollLeft" + uniqueNum + " { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("z-index: 4; ");
            cssString.Append("cursor: pointer; ");
            cssString.Append("-webkit-transition: all .3s; ");
            cssString.Append("-moz-transition: all .3s; ");
            cssString.Append("-o-transition: all .3s; ");
            cssString.Append("transition: all .3s; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollLeft" + uniqueNum + ":hover { ");
            cssString.Append("-ms-opacity: 1; opacity: 1; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollLeft" + uniqueNum + ":active { ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollRight" + uniqueNum + " { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("z-index: 4; ");
            cssString.Append("cursor: pointer; ");
            cssString.Append("-webkit-transition: all .3s; ");
            cssString.Append("-moz-transition: all .3s; ");
            cssString.Append("-o-transition: all .3s; ");
            cssString.Append("transition: all .3s; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollRight" + uniqueNum + ":hover { ");
            cssString.Append("-ms-opacity: 1; opacity: 1; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideScrollRight" + uniqueNum + ":active { ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideIncrease" + uniqueNum + " { ");
            cssString.Append("position: absolute; display: inline-block; ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("z-index: 4; ");
            cssString.Append("cursor: pointer; ");
            cssString.Append("-webkit-transition: all .3s; ");
            cssString.Append("-moz-transition: all .3s; ");
            cssString.Append("-o-transition: all .3s; ");
            cssString.Append("transition: all .3s; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideIncrease" + uniqueNum + ":hover { ");
            cssString.Append("-ms-opacity: 1; opacity: 1; ");
            cssString.Append("} ");

            cssString.Append("#btnSlideIncrease" + uniqueNum + ":active { ");
            cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
            cssString.Append("} ");

            if (iconAdd)
            {
                cssString.Append("#divIconsWrap" + uniqueNum + " { ");
                cssString.Append("margin: 5px 0 0 0; ");
                cssString.Append("} ");

                cssString.Append("#divIconsWrap" + uniqueNum + " div { ");
                cssString.Append("position: relative; display: inline-block; ");
                cssString.Append("margin: 5px; ");
                cssString.Append("border: 5px " + iconBackColor + " solid; ");
                cssString.Append("cursor: pointer; ");
                cssString.Append("-webkit-box-shadow: 2px 2px 6px #000000; -webkit-box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("-ms-box-shadow: 2px 2px 6px #000000; -ms-box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("box-shadow: 2px 2px 6px #000000; box-shadow: 2px 2px 6px rgba(0,0,0,0.5); ");
                cssString.Append("-webkit-transition: all .3s; ");
                cssString.Append("-moz-transition: all .3s; ");
                cssString.Append("-o-transition: all .3s; ");
                cssString.Append("transition: all .3s; ");
                cssString.Append("} ");

                cssString.Append("#divIconsWrap" + uniqueNum + " div:hover { ");
                cssString.Append("border: 5px #a52a2a solid; ");
                cssString.Append("} ");

                cssString.Append("#divIconsWrap" + uniqueNum + " div img { ");
                cssString.Append("height: " + slideIconHeight + "px; ");
                cssString.Append("} ");
            }

            if (slideIncrease)
            {
                cssString.Append("#divSlideWrapOpac" + uniqueNum + " { ");
                cssString.Append("position: relative; display: inline-block; ");
                cssString.Append("padding: 5px; ");
                cssString.Append("-ms-border-radius: 4px; border-radius: 4px; ");
                cssString.Append("border: 1px #ffffff solid; ");
                cssString.Append("background-color: #dcdcdc; ");
                cssString.Append("-webkit-box-shadow: 0 0 12px #000000; -webkit-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append("-ms-box-shadow: 0 0 12px #000000; -ms-box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append("box-shadow: 0 0 12px #000000; box-shadow: 0 0 12px rgba(0,0,0,0.8); ");
                cssString.Append("z-index: 667; ");
                //cssString.Append("overflow: scroll; ");
                cssString.Append("} ");

                cssString.Append("#slideOpacTbl" + uniqueNum + " { ");
                cssString.Append("position: fixed; width: 100%; height: 100%; ");
                cssString.Append("top:0; left:0; ");
                cssString.Append("z-index:666; ");
                cssString.Append("border-collapse: collapse; ");
                cssString.Append("background-color: #CCC; background-color: rgba(0,0,0,.7); ");
                cssString.Append("} ");

                cssString.Append("#slideOpacTbl" + uniqueNum + " tr td { ");
                cssString.Append("text-align: center; vertical-align: middle; ");
                cssString.Append("} ");

                cssString.Append("#btnSlideClose" + uniqueNum + " { ");
                cssString.Append("position: absolute; display: inline-block; ");
                cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
                cssString.Append("z-index: 668; ");
                cssString.Append("cursor: pointer; ");
                cssString.Append("-webkit-transition: all .3s; ");
                cssString.Append("-moz-transition: all .3s; ");
                cssString.Append("-o-transition: all .3s; ");
                cssString.Append("transition: all .3s; ");
                cssString.Append("} ");

                cssString.Append("#btnSlideClose" + uniqueNum + ":hover { ");
                cssString.Append("-ms-opacity: 1; opacity: 1; ");
                cssString.Append("} ");

                cssString.Append("#btnSlideClose" + uniqueNum + ":active { ");
                cssString.Append("-ms-opacity: 0.5; opacity: 0.5; ");
                cssString.Append("} ");
            }

            cssString.Append("</style> ");

            _pag.Header.Controls.Add(new LiteralControl(cssString.ToString()));

            #endregion

            return new LiteralControl(htmlString.ToString());
        }
    }

    /// <summary>Класс, который формирует HTML-формы редактирования слайд-шоу для консоли администрирования</summary>
    public class SlideShowFormAdm
    {
        private Page _pag;

        public SlideShowFormAdm(Page pagenew) { _pag = pagenew; }

        /// <summary>функция возвращает таблицу с выбором слайд-шоу для редактирования из списка и кнопкой СОЗДАТЬ СЛАЙД-ШОУ</summary>
        /// <returns></returns>
        public Panel GetSliderChoosePanel()
        {
            var panelWrapper = new Panel();

            var sliderWork = new SlideShowWork(_pag);

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР СЛАЙД-ШОУ"; panelWrapper.Controls.Add(lbl);

            //Кнопка добавления нового слайд шоу
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "+ слайд-шоу";
            lBtn.ToolTip = "Создать новое слайд-шоу."; lBtn.Command += (lBtnCreateSlider_Command); panelWrapper.Controls.Add(lBtn);

            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //Выпадающий список с выбором слайд-шоу для редактирования
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выберите слайд-шоу для редактирования: "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            int counter = 0; int index = 0;
            foreach (string name in sliderWork.GetListOfAllSliderNames())
            {
                ddl.Items.Add(name);
                if (_pag.Session["SliderStruct"] != null)
                {
                    if (name == ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName) { index = counter; }
                }
                counter++;
            }
            ddl.SelectedIndex = index; ddl.ID = "ddlSliderSelect"; panelLine.Controls.Add(ddl);

            //Запуск редактирования выбранного в списке слайд-шоу
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "редактировать";
            lBtn.ToolTip = "Запуск редактирования выбранного в списке слайд-шоу."; lBtn.Command += (lBtnEditSlider_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetSliderChoosePanel()

        /// <summary>событие для выпадающего списка с выбором слайд-шоу для редактирования</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEditSlider_Command(object sender, CommandEventArgs e)
        {
            var sliderWork = new SlideShowWork(_pag);
            string val = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlSliderSelect")).SelectedValue;
            if (val == "") { _pag.Session["SliderStruct"] = null; }
            else { _pag.Session["SliderStruct"] = sliderWork.GetSliderStructForName(val); }

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>события нажатия на кнопку "+ слайд-шоу"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCreateSlider_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["SliderStruct"] = new SlideShowInit();
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName = "";
            ((SlideShowInit)_pag.Session["SliderStruct"]).OnOff = "on";
            var sliderWork = new SlideShowWork(_pag);
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowId = sliderWork.GetIdForNewSlider();
            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData = new List<string>();
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideHeight = "400";
            ((SlideShowInit)_pag.Session["SliderStruct"]).Interval = "2000";
            ((SlideShowInit)_pag.Session["SliderStruct"]).ScrollTime = "500";
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideIconSize = "20";
            ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIcons = "on";
            ((SlideShowInit)_pag.Session["SliderStruct"]).Typ = "scrollleft";
            ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffSliderMode = "off";
            ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIncrease = "off";

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        /// <summary>функция возвращает таблицу с редактором слайд-шоу(используется так же при создании нового слайд-шоу)</summary>
        /// <returns></returns>
        public Panel GetSliderEditPanel()
        {
            var panelWrapper = new Panel();

            //проверка на то, что временная структура с данными по слайд-шоу существует
            if (_pag.Session["SliderStruct"] == null) return panelWrapper;

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "Панель редактирования основных параметров слайд-шоу"; panelWrapper.Controls.Add(lbl);

            //имя слайд-шоу
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Имя слайд-шоу:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName;
            txtBox.ID = "txtBox_SliderName"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ID слайд-шоу
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "ID слайд-шоу: "; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowId.ToString(); txtBox.ReadOnly = true;
            txtBox.ID = "txtBox_SliderId"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Чекбокс включения или выключения слайд-шоу
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Включить слайд-шоу:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var chkBox = new CheckBox(); chkBox.ID = "slideShowOnOff";
            if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOff == "on") { chkBox.Checked = true; }
            else if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOff == "off") { chkBox.Checked = false; }
            panelLine.Controls.Add(chkBox);
            panelWrapper.Controls.Add(panelLine);

            //Чекбокс включения или выключения иконок внизу слайд-шоу
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Включить показ иконок внизу слайд-шоу:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            chkBox = new CheckBox(); chkBox.ID = "slideShowIconsOnOff";
            if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIcons == "on") { chkBox.Checked = true; }
            else if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIcons == "off") { chkBox.Checked = false; }
            panelLine.Controls.Add(chkBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка длительности показа слайда(в миллисекундах)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выбрать длительность показа слайда(в миллисекундах):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).Interval;
            txtBox.ID = "txtBox_SliderInterval"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Установка длительности прокрутки слайда(в миллисекундах)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выбрать длительность прокрутки слайда(в миллисекундах):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).ScrollTime;
            txtBox.ID = "txtBox_SliderScrollTime"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Высота слайд-шоу, в пикселях
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Высота слайд-шоу(в пикселях):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).SlideHeight;
            txtBox.ID = "txtBox_SliderHeight"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Высота и ширина иконок внизу слайд-шоу
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Высота и ширина иконок внизу слайд-шоу(в пикселях):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = ((SlideShowInit)_pag.Session["SliderStruct"]).SlideIconSize;
            txtBox.ID = "txtBox_SliderIconSize"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Выбор типа слайд-шоу
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выбрать тип анимации слайд-шоу:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            ddl.Items.Add("appear"); ddl.Items.Add("scrollleft"); ddl.Items.Add("scrollright");
            int counter = 0; int index = 0;
            foreach (ListItem item in ddl.Items)
            {
                if (((SlideShowInit)_pag.Session["SliderStruct"]).Typ == item.Value)
                {
                    index = counter;
                }
                counter++;
            }
            ddl.SelectedIndex = index; ddl.ID = "ddlSliderType"; panelLine.Controls.Add(ddl);
            panelWrapper.Controls.Add(panelLine);

            //Чекбокс включения или выключения режима слайдера
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Включить режим слайдера:"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            chkBox = new CheckBox(); chkBox.ID = "sliderOnOff";
            if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffSliderMode == "on") { chkBox.Checked = true; }
            else if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffSliderMode == "off") { chkBox.Checked = false; }
            panelLine.Controls.Add(chkBox);
            panelWrapper.Controls.Add(panelLine);

            //Чекбокс включения или выключения режима отображения кнопки увеличения слайда (только для режима слайдера)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Включить возможность увеличения слайда (только для режима слайдера):"; lbl.Width = 250; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            chkBox = new CheckBox(); chkBox.ID = "slideIncreaseOnOff";
            if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIncrease == "on") { chkBox.Checked = true; }
            else if (((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIncrease == "off") { chkBox.Checked = false; }
            panelLine.Controls.Add(chkBox);
            panelWrapper.Controls.Add(panelLine);

            //Кнопки для загрузки нового фото
            var fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fotoUpload";
            fUpload.ToolTip = "Для слайд-шоу на главной рекомендуются картинки шириной 1045 пикселей. Высота не важна, но должна быть одинаковой."; panelWrapper.Controls.Add(fUpload);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "+ фото";
            lBtn.ToolTip = "Добавить фотографию(слайд) к слайд-шоу.";
            lBtn.CommandArgument = ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowId.ToString();
            lBtn.Command += (lBtnAddFoto_Command); panelWrapper.Controls.Add(lBtn); panelWrapper.Controls.Add(new LiteralControl("<br />"));

            //Кнопки СОХРАНИТЬ И УДАЛИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "СОХРАНИТЬ СЛАЙД-ШОУ"; lBtn.ToolTip = "Сохраняются все изменения, которые были сделаны в слайд-шоу.";
            lBtn.Command += (lBtnSliderSave_Command); lBtn.ID = "btnSliderSave"; panelWrapper.Controls.Add(lBtn);
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "УДАЛИТЬ СЛАЙД-ШОУ"; lBtn.ToolTip = "Полное удаление слайд-шоу. Текстовые страницы акций не удаляются.";
            lBtn.OnClientClick = "return confirm('Слайдер будет полностью удалён. Удалить?');";
            lBtn.Command += (btnSliderDelete_Command); lBtn.ID = "btnSliderDelete"; panelWrapper.Controls.Add(lBtn);

            //Ниже добавляются элементы для редактирования каждого слайда СЛАЙД-ШОУ
            string[] lineSplit;//, strSplit;
            counter = 0;
            foreach (string line in ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData) //цикл для отображения панелей редактирования каждого слайда, входящего в слайд-шоу
            {
                lineSplit = line.Split(new[] { '|' });

                //строка с заглавием редактора слайда
                lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "Панель редактирования слайда № " + (counter + 1); panelWrapper.Controls.Add(lbl);

                //строка с картинкой слайда
                var img = new Image(); img.ImageUrl = lineSplit[1]; img.CssClass = "imgBckGrd"; panelWrapper.Controls.Add(img);

                panelLine = new Panel(); panelLine.CssClass = "panelLine";
                //строка с выбором действия при нажатии на фото слайда
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выберите то, что будет происходить при нажатии на фото слайда: "; panelLine.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
                var ddl1 = new DropDownList(); ddl1.CssClass = "txtBoxUniverse_adm";
                ddl1.Items.Add("ничего"); /*ddl1.Items.Add("переход к товару");*/ ddl1.Items.Add("переход к акции");
                //if (lineSplit[2].Contains("/")) { ddl1.SelectedIndex = 1; }
                if (lineSplit[2].Contains("action_")) { ddl1.SelectedIndex = 1; }
                else if (lineSplit[2].Contains("no_action")) { ddl1.SelectedIndex = 0; }
                ddl1.ID = "ddlSlideAction_" + counter; ddl1.AutoPostBack = true;
                ddl1.SelectedIndexChanged += (ddl_SelectedIndexChanged); panelLine.Controls.Add(ddl1);
                panelWrapper.Controls.Add(panelLine);

                //Код ниже добавляет элементы для редактирования содержимого для действия при нажатии на фото слайда

                #region ДОБАВЛЯЕМ ПАНЕЛЬ, В КОТОРОЙ НУЖНО ВВЕСТИ АРТИКУЛ ТОВАРА

                var panel = new Panel(); panel.ID = "panelArtikul_" + counter; panelWrapper.Controls.Add(panel);
                /*
                lbl = new Label(); lbl.CssClass = "POIC_lbltxt"; lbl.Text = "Введите артикул товара: "; panel.Controls.Add(lbl);
                txtBox = new TextBox(); txtBox.CssClass = "PsBC_txtBox"; txtBox.Width = 70; txtBox.TextMode = TextBoxMode.SingleLine;
                //вычисляем артикул товара для записи в текстбокс
                string fullurl = "";
                if (lineSplit[2].Contains("/"))
                {
                    fullurl = lineSplit[2].Replace("/page/prodinfo.aspx?prodart=", "");
                    int indexstart = fullurl.IndexOf("&");
                    fullurl = fullurl.Remove(indexstart); fullurl.Trim();
                }
                txtBox.Text = fullurl; txtBox.ID = "txtBoxArtikul_" + counter; panel.Controls.Add(txtBox);
                */
                #endregion

                #region ДОБАВЛЯЕМ ПАНЕЛЬ, В КОТОРОЙ НУЖНО БУДЕТ добавить название файла страницы с описанием акции

                var panel1 = new Panel(); panel1.ID = "panelAction_" + counter; panelWrapper.Controls.Add(panel1);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Введите имя файла страницы:"; panel1.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panel1.Controls.Add(lbl);
                
                txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 250; 
                txtBox.TextMode = TextBoxMode.SingleLine;
                string content = "";
                if (lineSplit[2].Trim() != "no_action")
                {
                    content = lineSplit[2].Trim();
                }
                txtBox.Text = content; txtBox.ID = "txtBoxActionContent_" + counter; panel1.Controls.Add(txtBox);
                panel1.Controls.Add(new LiteralControl("<br />"));

                #endregion

                if (ddl1.SelectedIndex == 0)         //если не требуется действий при нажатии на фото слайда
                {
                    panel.Visible = false; panel1.Visible = false;
                }
                /*else if (ddl1.SelectedIndex == 1)    //если требуется переход на продукт при нажатии на фото слайда, то..
                {
                    panel.Visible = true; panel1.Visible = false;
                }*/
                else if (ddl1.SelectedIndex == 1)    //если требуется создание страницы с описанием акции, при нажатии на фото слайда, то..
                {
                    panel.Visible = false; panel1.Visible = true;
                }

                //строка с кнопками СОХРАНИТЬ СЛАЙД, ВВЕРХ, ВНИЗ, УДАЛИТЬ СЛАЙД
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Сохранить слайд";
                lBtn.ToolTip = "Сохраняются все изменения, которые были сделаны в слайде и сохраняется всё слайд-шоу. Так же рекомендуется нажать на кнопку СОХРАНИТЬ СЛАЙД-ШОУ.";
                lBtn.Command += (lBtnOneSlidSave_Command); lBtn.ID = "btnOneSlidSave_" + counter; panelWrapper.Controls.Add(lBtn);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Вверх";
                lBtn.ToolTip = "Временно передвинуть слайд вверх в списке слайдов. Для окончательного сохранения изменений нажмите СОХРАНИТЬ СЛАЙД-ШОУ.";
                lBtn.Command += (lBtnOneSlidUp_Command); lBtn.ID = "btnOneSlidUp_" + counter; panelWrapper.Controls.Add(lBtn);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Вниз";
                lBtn.ToolTip = "Временно передвинуть слайд вниз в списке слайдов. Для окончательного сохранения изменений нажмите СОХРАНИТЬ СЛАЙД-ШОУ.";
                lBtn.Command += (lBtnOneSlidDown_Command); lBtn.ID = "btnOneSlidDown_" + counter; panelWrapper.Controls.Add(lBtn);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Удалить слайд";
                lBtn.ToolTip = "Слайд удаляется из списка слайдов, так же удаляются все данные(фото, текст) с жёсткого диска, которые касаются данного слайда." +
                              " Так же рекомендуется нажать на кнопку СОХРАНИТЬ СЛАЙД-ШОУ.";
                lBtn.OnClientClick = "return confirm('Данные, касающиеся данного слайда будут полностью удалены. Удалить?');";
                lBtn.Command += (lBtnOneSlidDelete_Command); lBtn.ID = "btnOneSlidDelete_" + counter; panelWrapper.Controls.Add(lBtn);

                counter++;
            }

            return panelWrapper;
        }

        #region События для функции GetSliderEditPanel()

        /// <summary>событие нажатия на кнопку - "+ фото"</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит SlideShowId</param>
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

                if ((extension.ToLower() == ".jpg") || (extension.ToLower() == ".jpeg") || (extension.ToLower() == ".png"))     //проверка на допустимые расширения закачиваемого файла
                {
                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 100000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        int num = 0;

                        #region КОД ОПРЕДЕЛЕНИЯ ПОСЛЕДНЕЙ ЦИФРЫ С ИМЕНИ ФАЙЛА ДЛЯ НОВОЙ ФОТОГРАФИИ

                        if (((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Count > 0)    //если для слайдера уже добавлены фотографии, то..
                        {
                            var listOfNum = new List<int>();
                            string[] lineSplit, strSplit, splitFName, splitLeftPart; string filename;
                            foreach (string line in ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData)
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
                        string savePath = _pag.Server.MapPath("~") + @"files\slider\" + newFileName;
                        try
                        {
                            fUpload.SaveAs(savePath);
                            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Add("img|../files/slider/" + newFileName + "|no_action");

                            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
                        }
                        catch { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", _pag.Master); }

                    }
                    else warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 100 килобайт.", _pag.Master);
                }
                else
                {
                    warning.ShowWarning("ВНИМАНИЕ. Доступно добавление только файлов формата 'jpg', 'jpeg' или 'png'.", _pag.Master);
                }
            }
            else warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", _pag.Master);
        }

        /// <summary>событие нажатия на кнопку - "СОХРАНИТЬ СЛАЙД-ШОУ"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSliderSave_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideShowWork(_pag);
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);

            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderName")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowId = int.Parse(((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderId")).Text);
            ((SlideShowInit)_pag.Session["SliderStruct"]).Interval = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderInterval")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideHeight = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderHeight")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).ScrollTime = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderScrollTime")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideIconSize = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderIconSize")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).Typ = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlSliderType")).SelectedValue;

            int parseToInt = StringToNum.ParseInt(((SlideShowInit)_pag.Session["SliderStruct"]).SlideHeight);
            bool checker = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$slideShowOnOff")).Checked;
            if (checker) { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOff = "on"; } else { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOff = "off"; }
            checker = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$slideShowIconsOnOff")).Checked;
            if (checker) { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIcons = "on"; } else { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIcons = "off"; }

            checker = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$sliderOnOff")).Checked;
            if (checker) { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffSliderMode = "on"; } else { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffSliderMode = "off"; }
            checker = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$slideIncreaseOnOff")).Checked;
            if (checker) { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIncrease = "on"; } else { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOffIncrease = "off"; }

            if (((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели имя для слайд-шоу.", _pag.Master); return; }
            if (((SlideShowInit)_pag.Session["SliderStruct"]).SlideHeight.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели высоту для слайд-шоу.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение высоты для слайд-шоу.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideShowInit)_pag.Session["SliderStruct"]).Interval);
            if (((SlideShowInit)_pag.Session["SliderStruct"]).Interval.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели длительность показа слайда.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение длительности показа слайда.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideShowInit)_pag.Session["SliderStruct"]).ScrollTime);
            if (((SlideShowInit)_pag.Session["SliderStruct"]).ScrollTime.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели длительность прокрутки слайда.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение длительности прокрутки слайда.", _pag.Master); return; }
            parseToInt = StringToNum.ParseInt(((SlideShowInit)_pag.Session["SliderStruct"]).SlideIconSize);
            if (((SlideShowInit)_pag.Session["SliderStruct"]).SlideIconSize.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели ширину и высоту иконок.", _pag.Master); return; }
            if (parseToInt == -1) { warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели значение ширины и высоты иконок.", _pag.Master); return; }

            if (!sliderWork.ReplaceOrDeleteSlider((SlideShowInit)_pag.Session["SliderStruct"]))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения слайд-шоу. Попробуйте повторить.", _pag.Master); }
            //else { warning.ShowWarning("УСПЕХ. Изменения успешно сохранены.", _pag.Master); }
        }

        /// <summary>событие нажатия на кнопку - "УДАЛИТЬ СЛАЙД-ШОУ" (страницы с описанием акций не удалаяются!!!)</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSliderDelete_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideShowWork(_pag);
            var warning = new WarnClass();

            if (!sliderWork.ReplaceOrDeleteSlider((SlideShowInit)_pag.Session["SliderStruct"], true))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе удаления слайд-шоу. Попробуйте повторить.", _pag.Master); }
            else
            {
                #region УДАЛЯЕМ ВСЕ ДАННЫЕ ПО СЛАЙД-ШОУ И ВСЕ ФОТОГРАФИИ, ЕСЛИ ТАКОВЫЕ ИМЕЮТСЯ

                foreach (string line in ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData)
                {
                    //удалим фотографию слайда
                    string[] slideDataSplit = line.Split(new[] { '|' });
                    try
                    {
                        File.Delete(_pag.Server.MapPath("~") + slideDataSplit[1].Replace("../", ""));
                    }
                    catch { }
                }

                #endregion

                _pag.Session["SliderStruct"] = null;
                if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
            }
        }

        /// <summary>изменение значения выпадающего списка с настройкой действий при нажатии на фото слайда</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var ddl = (DropDownList)sender;
            string[] strSplit = ddl.ID.Split(new[] { '_' });
            if (ddl.SelectedIndex == 0)          //ничего делать
            {
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelArtikul_" + strSplit[1]).Visible = false;
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelAction_" + strSplit[1]).Visible = false;
            }
            /*else if (ddl.SelectedIndex == 1)    //показать товар
            {
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelArtikul_" + strSplit[1]).Visible = true;
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelAction_" + strSplit[1]).Visible = false;
            }*/
            else if (ddl.SelectedIndex == 1)    //показать акцию
            {
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelArtikul_" + strSplit[1]).Visible = false;
                _pag.FindControl("ctl00$ContentPlaceHolder1$panelAction_" + strSplit[1]).Visible = true;
            }
        }

        /// <summary>событие нажатия на кнопку - "Сохранить слайд". Сохранение всех изменений, сделанных с одним слайдом и сохранение всего слайд шоу.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOneSlidSave_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideShowWork(_pag);
            var warning = new WarnClass();
            //получим идекс слайда
            string[] strSplit = ((LinkButton)sender).ID.Split(new[] { '_' });       //индекс будет содержаться в элементе массива strSplit[1]

            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderName")).Text;
            ((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowId = int.Parse(((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBox_SliderId")).Text);
            bool slideShowOn = ((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$slideShowOnOff")).Checked;
            if (slideShowOn) { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOff = "on"; } else { ((SlideShowInit)_pag.Session["SliderStruct"]).OnOff = "off"; }

            //проверяем, введено ли имя для слайд-шоу
            if (((SlideShowInit)_pag.Session["SliderStruct"]).SlideShowName == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели имя для слайд-шоу.", _pag.Master); return; }
            else { warning.HideWarning(_pag.Master); }

            //определим тип действия, которое будет производиться при нажатии на картинку слайда
            string actionName = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlSlideAction_" + strSplit[1])).SelectedValue;

            //в соответствии с типом действия производим необходимые действия по сохранению
            if (actionName == "ничего")
            {
                //получим, изменим и перезапишем нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
                string line = ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];
                string[] lineSplit = line.Split(new[] { '|' });
                line = lineSplit[0] + "|" + lineSplit[1] + "|no_action";
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старую строку из списка
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Insert(int.Parse(strSplit[1]), line);    //добавим новую строку в список на тоже место
            }
            else if (actionName == "переход к товару")
            {
                /*//получим артикул товара и проверим его на существование
                string prodArtikul = ((TextBox)pag.FindControl("ctl00$ctl00$ContentPlaceHolder1$ContentOfChildPages1$txtBoxArtikul_" + strSplit[1])).Text;
                var prodListClass = new ProdListClass(pag);
                var prodStruct = prodListClass.GetProdStructForArtikul(prodArtikul);
                if (prodStruct == null) { warning.ShowWarning("ВНИМАНИЕ. Товара с введённым артикулом не найдено в базе данных.", pag.Master.Master); return; }
                else { warning.HideWarning(pag.Master.Master); }
                //получим, изменим и перезапишем нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
                string line = ((SliderStruct)pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];
                string[] lineSplit = line.Split(new[] { '|' });
                line = lineSplit[0] + "|" + lineSplit[1] + "|/page/prodinfo.aspx?prodart=" + prodArtikul + "&sectid=" + prodStruct.GroupID;
                ((SliderStruct)pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старую строку из списка
                ((SliderStruct)pag.Session["SliderStruct"]).ListOfData.Insert(int.Parse(strSplit[1]), line);    //добавим новую строку в список на тоже место*/
            }
            else if (actionName == "переход к акции")
            {
                //получим нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
                string line = ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];
                string[] lineSplit = line.Split(new[] { '|' });

                //получим строку с именем текстового файла с описанием акции из текстбокса
                string filename = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxActionContent_" + strSplit[1])).Text;
                if (filename.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Вы не ввели имя текстового файла с описанием акции.", _pag.Master); return; }

                //изменим и перезапишем нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
                line = lineSplit[0] + "|" + lineSplit[1] + "|" + filename;
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старую строку из списка
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Insert(int.Parse(strSplit[1]), line);    //добавим новую строку в список на тоже место
            }

            //перезапишем все данные по слайд-шоу в файле структур слайд-шоу
            if (!sliderWork.ReplaceOrDeleteSlider((SlideShowInit)_pag.Session["SliderStruct"]))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения слайд-шоу. Попробуйте повторить.", _pag.Master); }
            //else { warning.ShowWarning("УСПЕХ. Изменения успешно сохранены.", _pag.Master); }
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

            //получим, изменим и перезапишем нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
            string line = ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];
            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старую строку из списка
            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Insert(int.Parse(strSplit[1]) - 1, line);    //добавим новую строку в список на нужное место

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

            //проверим, есть ли смысл двигать слайд вниз или двигать уже некуда
            if (int.Parse(strSplit[1]) == ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Count - 1) { return; }

            //получим, изменим и перезапишем нужную строку в списке ((SliderStruct)pag.Session["SliderStruct"]).ListOfData
            string line = ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];
            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));        //удалим старую строку из списка
            if (int.Parse(strSplit[1]) == ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Count - 1)
            {
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Add(line);    //добавим новую строку в конец списка, если слайд находился на предпоследнем месте
            }
            else
            {
                ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.Insert(int.Parse(strSplit[1]) + 1, line);    //добавим новую строку в список на нужное место
            }

            //перезагрузим страницу, чтобы увидеть изменения и перезагрузить значения ID элементов управления
            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку - "Удалить слайд". Полное удаления слайда из списка слайдов.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOneSlidDelete_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var sliderWork = new SlideShowWork(_pag);
            var warning = new WarnClass();

            //получим идекс слайда
            string[] strSplit = ((LinkButton)sender).ID.Split(new[] { '_' });       //индекс будет содержаться в элементе массива strSplit[1]

            //сохраним строку данных слайда
            string slideData = ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData[int.Parse(strSplit[1])];

            //удалим строку с данными слайда из списка
            ((SlideShowInit)_pag.Session["SliderStruct"]).ListOfData.RemoveAt(int.Parse(strSplit[1]));

            //перезапишем все данные по слайд-шоу в файле структур слайд-шоу
            if (!sliderWork.ReplaceOrDeleteSlider((SlideShowInit)_pag.Session["SliderStruct"]))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения новых данных по слайд-шоу в файл. Попробуйте повторить.", _pag.Master); return; }
            else { warning.HideWarning(_pag.Master); }

            //удалим фотографию слайда
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

    public class SlideShowWork
    {
        private Page pag;
        //private int _listIdToInsert;            //в эту переменную записывается индекс исключённого из списка элемента(исключается в функции GetAllNewsStructs)
        public string ActionId { get; set; }    //сюда записывается значение ID новой акции при вызове функции
        //private string[] _pathesToTxtPageFiles; //переменная содержит пути ко всем файлам структур текстовых страниц сайта. Инициализируется в конструкторе этого класса

        public SlideShowWork(Page pagenew) { pag = pagenew; }

        /// <summary>функция возвращает все данные по слайд-шоу(в виде структур), содержащиеся в файле /files/slider/slider, кроме структуры одного слайдера, переданного в качестве аргумента</summary>
        /// <param name="structForExclude">структура слайдера, которую нужно исключить из возвращаемого списка</param>
        /// <returns></returns>
        public List<SlideShowInit> GetAllSliderStruct(SlideShowInit structForExclude = null)
        {
            var listResult = new List<SlideShowInit>();

            #region КОД

            var structure = new SlideShowInit();
            var dataList = new List<string>();
            string[] str, strSplit;
            bool chknewslider = false;
            string excludeId = "";
            if (structForExclude != null) { excludeId = structForExclude.SlideShowId.ToString(); }

            string pathtofile = pag.Server.MapPath("~") + @"files\slider\slider";
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
                            if (strSplit[1] != excludeId)   //если не нужно исключить данные слайдера, то..
                            {
                                chknewslider = true;
                                structure = new SlideShowInit();
                                dataList = new List<string>();

                                structure.SlideShowId = int.Parse(strSplit[1]);
                                structure.SlideShowName = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.Interval = strSplit[4];
                                structure.ScrollTime = strSplit[5];
                                structure.SlideHeight = strSplit[6];
                                structure.SlideIconSize = strSplit[7];
                                structure.OnOffIcons = strSplit[8];
                                structure.Typ = strSplit[9];
                                structure.OnOffSliderMode = strSplit[10];
                                structure.OnOffIncrease = strSplit[11];
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

        /// <summary>функция возвращает сортированный по алфавиту список всех имён слайд-шоу, которые есть в файле /files/slider/slider</summary>
        /// <returns></returns>
        public List<string> GetListOfAllSliderNames()
        {
            var listresult = new List<string>();

            #region КОД

            string[] str, strSplit;
            string pathtofile = pag.Server.MapPath("~") + @"files\slider\slider";
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

        /// <summary>функция возвращает структуру данных одного слайд-шоу по переданному Id слайд-шоу</summary>
        /// <param name="sliderId">ID слайд-шоу</param>
        /// <returns></returns>
        public SlideShowInit GetSliderStructForId(int sliderId)
        {
            var structure = new SlideShowInit();
            structure.SlideShowId = -1;                 //это значение будет являться признаком того, что слайдшой с таким ID не существует.

            #region КОД

            var dataList = new List<string>();
            string[] str, strSplit;
            bool chkslider = false;

            string pathtofile = pag.Server.MapPath("~") + @"files\slider\slider";
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
                                structure = new SlideShowInit();
                                dataList = new List<string>();

                                structure.SlideShowId = int.Parse(strSplit[1]);
                                structure.SlideShowName = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.Interval = strSplit[4];
                                structure.ScrollTime = strSplit[5];
                                structure.SlideHeight = strSplit[6];
                                structure.SlideIconSize = strSplit[7];
                                structure.OnOffIcons = strSplit[8];
                                structure.Typ = strSplit[9];
                                structure.OnOffSliderMode = strSplit[10];
                                structure.OnOffIncrease = strSplit[11];
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

        /// <summary>функция возвращает структуру данных одного слайд-шоу по переданному ИМЕНИ слайд-шоу</summary>
        /// <param name="sliderName">ИМЯ слайд-шоу</param>
        /// <returns></returns>
        public SlideShowInit GetSliderStructForName(string sliderName)
        {
            var structure = new SlideShowInit();

            #region КОД


            var dataList = new List<string>();
            string[] str, strSplit;
            bool chkslider = false;

            string pathtofile = pag.Server.MapPath("~") + @"files\slider\slider";
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
                            if (strSplit[2] == sliderName)   //если это нужно нам слайд-шоу, то..
                            {
                                chkslider = true;
                                structure = new SlideShowInit();
                                dataList = new List<string>();

                                structure.SlideShowId = int.Parse(strSplit[1]);
                                structure.SlideShowName = strSplit[2];
                                structure.OnOff = strSplit[3];
                                structure.Interval = strSplit[4];
                                structure.ScrollTime = strSplit[5];
                                structure.SlideHeight = strSplit[6];
                                structure.SlideIconSize = strSplit[7];
                                structure.OnOffIcons = strSplit[8];
                                structure.Typ = strSplit[9];
                                structure.OnOffSliderMode = strSplit[10];
                                structure.OnOffIncrease = strSplit[11];
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

        /// <summary>функция возвращает очередной порядковый номер для нового слайд-шоу</summary>
        /// <returns></returns>
        public int GetIdForNewSlider()
        {
            int newid = 1;

            #region КОД

            var listOfSliderStructs = GetAllSliderStruct();
            if (listOfSliderStructs.Count > 0)
            {
                listOfSliderStructs.Sort((a, b) => a.SlideShowId.CompareTo(b.SlideShowId)); //сортировка структур по свойству - SlideShowId (от 'А до Я')
                newid = listOfSliderStructs[listOfSliderStructs.Count - 1].SlideShowId + 1;
            }

            #endregion

            return newid;
        }

        /// <summary>функция возвращает очередной порядковый номер для новой акции слайд-шоу. Этот номер добавляется к "action_" в строку вида img|~/files/slider/4.jpg|action_1</summary>
        /// <returns></returns>
        private string GetNewActionNum()
        {
            string newnum = "1";

            #region КОД

            var listOfSliderStructs = GetAllSliderStruct();
            var listOfActionNum = new List<int>();
            string[] strSplit, subStrSplit;
            if (listOfSliderStructs.Count > 0)
            {
                foreach (SlideShowInit oneStruct in listOfSliderStructs) //перебираем все структуры
                {
                    foreach (string line in oneStruct.ListOfData)       //перебираем все строки в списочном свойстве каждой структуры
                    {
                        strSplit = line.Split(new[] { '|' });
                        if (strSplit[2].Contains("action_"))
                        {
                            subStrSplit = strSplit[2].Split(new[] { '_' });
                            listOfActionNum.Add(int.Parse(subStrSplit[1]));
                        }
                    }
                }
                listOfActionNum.Sort();
                if (listOfActionNum.Count > 0)
                {
                    newnum = (listOfActionNum[listOfActionNum.Count - 1] + 1).ToString();
                }
            }

            #endregion

            return newnum;
        }

        /// <summary>функция заменяет или удаляет данные по одному слайд-шоу в файле /files/slider/slider. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных слайд-шоу, которую нужно заменить или удалить в файле</param>
        /// <param name="delete">true - удалить переданную структуру из файла, false - заменить</param>
        /// <returns></returns>
        public bool ReplaceOrDeleteSlider(SlideShowInit Struct, bool delete = false)
        {
            string pathtofile = pag.Server.MapPath("~") + @"files\slider\slider";
            string pathtotemp = pag.Server.MapPath("~") + @"files\temp\slider";

            #region КОД ЗАМЕНЫ И УДАЛЕНИЯ ДАННЫХ СЛАЙДЕРА

            var listOfStructs = GetAllSliderStruct(Struct);     //получим список структур всех слайд-шоу
            if (!delete) { listOfStructs.Add(Struct); }         //если нужно перезаписать данные слайд-шоу, то добавим в список новую переданную в функцию структуру слайд-шоу

            //преобразуем список listOfStructs в строковый список listForDBFile, который пригоден для записи в файл
            var listForDbFile = new List<string>();
            foreach (var onestruct in listOfStructs) { listForDbFile.AddRange(onestruct.GetListFromStruct()); }
            listOfStructs.Clear();

            //перезапишем файл /files/slider/slider

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

        /// <summary>Функция возвращает массив, который ввключает элементы: 0 - ширина картинки в зависимости от нужной высоты(needHeight);
        /// 1 - реальная ширина картинки; 2 - реальная высота картинки. 
        /// Функция возвращает 0,0,0, если по заданному пути картинки не нашлось.</summary>
        /// <param name="pathToFile">путь к файлу картинки</param>
        /// <param name="needHeight">требуемая высота картинки(нужна для расчёта требуемой ширины картинки)</param>
        /// <returns></returns>
        public string[] GetImageParam(string pathToFile, int needHeight)
        {
            try
            {
                System.Drawing.Image objImage = System.Drawing.Image.FromFile(pathToFile);
                float k = (float)objImage.Height / needHeight;
                int newWidth = (int)(objImage.Width / k);

                return new[] { newWidth.ToString(), objImage.Width.ToString(), objImage.Height.ToString() };
            }
            catch
            {
                return new[] { "0", "0", "0" };
            }

            return new[] { "" };
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>Класс описывающий структуру данных одного слайд-шоу или слайдера</summary>
    public class SlideShowInit
    {
        public int SlideShowId { get; set; }            //содержит ID слайд шоу
        public string SlideShowName { get; set; }       //содержит Имя слайд шоу
        public string OnOff { get; set; }               //включен или выключен слайдер (может быть on, либо off)
        public string Interval { get; set; }            //длительность показа одного слайда (в секундах)
        public string ScrollTime { get; set; }          //длительность прокрутки слайда (в секундах)
        public string SlideHeight { get; set; }         //высота слайд-шоу (в пикселях)
        public string SlideIconSize { get; set; }       //высота и ширина иконок внизу слайд-шоу (в пикселях)
        public string OnOffIcons { get; set; }          //включены или выключено отображение иконок внизу слайд-шоу (может быть on, либо off)
        public string Typ { get; set; }                 //тип слайд-шоу. Может быть appear, scrollleft, scrollright
        public List<string> ListOfData { get; set; }    //содержит список строк данных слайд-шоу, пример строки, ссылающейся на акцию: img|~/files/slider/4.jpg|pagename   . Файл страницы акций(pagename) хранbтся по пути - /files/pages/ и создаётся в РЕДАКТОРЕ СТРАНИЦ
                                                        //Если строка содержит аргумент no_action (img|~/files/slider/4.jpg|no_action), то при нажатии на картинку в слайд-шоу ничего не происходит.
        public string OnOffSliderMode { get; set; }     //включение или выключение режима слайдера (может быть on, либо off). В этом режиме картинки проматываются вручную кнопками слево вправо, так же имеется кнопка увеличения слайда и иконки-кнопки с уменьшенными картинками.
        public string OnOffIncrease { get; set; }       //включение или выключение кнопки увеличения слайда (может быть on, либо off). Эта кнопка доступна только в режиме слайдера.

        /// <summary>функция возвращает список в формате List/string/ из данной структуры слайд-шоу. 
        /// Строки в списке полностью подготовлены для записи в файл /files/slider/slider.</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();

            list.Add("sliderStart|" + SlideShowId + "|" + SlideShowName + "|" + OnOff + "|" + Interval + "|" + ScrollTime + "|" +
                                      SlideHeight + "|" + SlideIconSize + "|" + OnOffIcons + "|" + Typ + "|" + OnOffSliderMode + "|" + OnOffIncrease);
            list.AddRange(ListOfData);
            list.Add("sliderEnd|");

            return list;
        }
    }

    //перечисление для класса SlideShow - обозначает тип запускаемого слайдшоу в зависимости от способа смены слайдов
    public enum SlideShowType { ScrollLeft, ScrollRight, Appear }

    #endregion

}