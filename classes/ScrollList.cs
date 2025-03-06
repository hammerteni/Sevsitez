using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace site.classes
{
    /*ПРИМЕР ДОБАВЛЕНИЯ КОДА БЕГУЩЕЙ СТРОКИ НА СТРАНИЦУ
     var list = new List<string>();
     list.Add("<span class='actionText'>МЕГА скидка на монтаж под ключ!</span>");
     list.Add("<span class='actionText'>Только сейчас, супер акция на Станции ЮНИЛОС, Септик Дочиста, Станции Дочиста!</span>");
     var scrollList = new ScrollList(this);
     Master.FindControl("scrollString").Controls.Add(scrollList.GetScrollList(list, 1000, 25, 10, 1));
     */

    /// <summary>Класс, который формирует html-код, jquery-код и стили для бегущей строки с любым содержимым.
    /// Можно добавлять сколько угодно бегущих строк на страницу
    /// Так же для работы должна быть подключена библиотека jquery.li-scroller.1.0.js</summary>
    public class ScrollList
    {
        private Page _pag;

        //Конструктор класса
        public ScrollList(Page pag)
        {
            _pag = pag;
        }

        /// <summary>Функция возвращает html-код бегущей строки. Добавляет JavaScript код на страницу и стили.</summary>
        /// <param name="elemList">список строк с содержимым элементов списка</param>
        /// <param name="scrollerWidth">ширина бегущей строки в пикселях</param>
        /// <param name="scrollerHeigt">высота</param>
        /// <param name="scrollSpeed">скорость бегущей строки. Рекомендуется значение 16. Чем меньше значение - тем быстрее.</param>
        /// <param name="uniqNum">уникальный идентификатор бегущей строки</param>
        /// <returns></returns>
        public LiteralControl GetScrollList(List<string> elemList, int scrollerWidth, int scrollerHeigt, int scrollSpeed, int uniqNum)
        {
            var htmlString = new StringBuilder();

            if (elemList.Count == 0) { return new LiteralControl(""); }

            #region Формирование HTML-кода

            htmlString.Append("<div id='scrollStringWrapper_" + uniqNum + "'>");
            htmlString.Append("<div id='scrollDiv" + uniqNum + "'>");
            foreach (string line in elemList)
            {
                htmlString.Append("<div class='oneScrollElem_" + uniqNum + "'>" + line + "</div>");
            }
            htmlString.Append("</div>");
            htmlString.Append("</div>");

            #endregion

            var jQueryString = new StringBuilder();

            #region Формирование jQuery-кода и добавление его в конец страницы

            jQueryString.AppendLine("<noindex><script type='text/javascript'>");
            jQueryString.Append("$(document).ready(function () { ");

            //Вычисляем ширину, складывающуюся из ширины всех элементов списка
            jQueryString.Append("var $elemArray" + uniqNum + " = $('#scrollStringWrapper_" + uniqNum + " div div');");
            jQueryString.Append("var widthAll" + uniqNum + " = 0;");
            jQueryString.Append("$elemArray" + uniqNum + ".each(function () {");
            jQueryString.Append("widthAll" + uniqNum + " += $(this).outerWidth(true);");
            jQueryString.Append("});");

            //Устанавливаем ширину для слоя со всеми элементами
            jQueryString.Append("var $scrollDiv" + uniqNum + " = $('#scrollDiv" + uniqNum + "');");
            jQueryString.Append("$scrollDiv" + uniqNum + ".width(widthAll" + uniqNum + " + ($elemArray" + uniqNum + ".length * 5));");

            //Устанавливаем слой со всеми элементами в начальное положение
            jQueryString.Append("var $mainWrap" + uniqNum + " = $('#scrollStringWrapper_" + uniqNum + "');");
            jQueryString.Append("$scrollDiv" + uniqNum + ".offset({ top: $scrollDiv" + uniqNum + ".offset().top, left: $scrollDiv" + uniqNum + ".offset().left + $mainWrap" + uniqNum + ".width() });");

            //Запускаем бегущую строку
            //Функции запуска и остановки слайд-шоу, а также начальный запуск и остановка при событиях окна
            //window.onblur = ssStop1;
            //window.onfocus = ssStart1;
            jQueryString.Append("ssStart" + uniqNum + "();");
            jQueryString.Append("var ssInterv" + uniqNum + ";");
            jQueryString.Append("function ssStart" + uniqNum + "() {");
            jQueryString.Append("window.clearInterval(ssInterv" + uniqNum + ");");
            jQueryString.Append("ssInterv" + uniqNum + " = window.setInterval(function () {");
            jQueryString.Append("if (($scrollDiv" + uniqNum + ".offset().left + $scrollDiv" + uniqNum + ".width()) < $mainWrap" + uniqNum + ".offset().left) {");
            jQueryString.Append("$scrollDiv" + uniqNum + ".offset({ top: $scrollDiv" + uniqNum + ".offset().top, left: $mainWrap" + uniqNum + ".offset().left + $mainWrap" + uniqNum + ".width() });");
            jQueryString.Append("}");
            jQueryString.Append("$scrollDiv" + uniqNum + ".offset({ top: $scrollDiv" + uniqNum + ".offset().top, left: $scrollDiv" + uniqNum + ".offset().left - 1 });");
            jQueryString.Append("}, " + scrollSpeed + ");");
            jQueryString.Append("};");
            jQueryString.Append("function ssStop" + uniqNum + "() {");
            jQueryString.Append("window.clearInterval(ssInterv" + uniqNum + ");");
            jQueryString.Append("};");

            //События наведения мышкой
            jQueryString.Append("$scrollDiv" + uniqNum + ".on('mouseover', function () {");
            jQueryString.Append("ssStop" + uniqNum + "();");
            jQueryString.Append("});");
            jQueryString.Append("$scrollDiv" + uniqNum + ".on('mouseout', function () {");
            jQueryString.Append("ssStart" + uniqNum + "();");
            jQueryString.Append("});");

            jQueryString.Append("}); ");
            jQueryString.AppendLine("</script></noindex> ");

            _pag.Controls.Add(new LiteralControl(jQueryString.ToString()));

            #endregion

            var cssString = new StringBuilder();

            #region Формирование css-кода и добавление его в шапку страницы

            cssString.AppendLine("<style type='text/css'> ");

            cssString.AppendLine("#scrollStringWrapper_" + uniqNum + " { ");
            cssString.Append("position: relative; ");
            cssString.Append("width: " + scrollerWidth + "px; ");
            cssString.Append("height: " + scrollerHeigt + "px; ");
            cssString.Append("margin: 5px auto 5px auto; ");
            cssString.Append("overflow: hidden; ");
            cssString.Append("cursor: default; ");
            //cssString.Append("border: 1px #000 solid; ");
            cssString.Append("} ");

            cssString.AppendLine(".oneScrollElem_" + uniqNum + " { ");
            cssString.Append("position: relative; display: inline; ");
            cssString.Append("padding: 0 5px 0 5px; ");
            cssString.Append("} ");

            cssString.AppendLine("</style> ");

            _pag.Header.Controls.Add(new LiteralControl(cssString.ToString()));

            #endregion

            return new LiteralControl(htmlString.ToString());
        }
    }
}