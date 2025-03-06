using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using site.classesHelp;

/* файл с классами для работы со страницами, содержащими текстовые и графические данные (файлы в папке files\pages\) */
namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>универсальный статический класс, метод GetPage() которого возвращает таблицу, наполненную разной информацией - форматированным текстом, картинками, гиперссылками, яндекс картами и т.п.</summary>
    public class PagesForm
    {
        private Page _pag;

        public PagesForm(Page pagnew = null) { _pag = pagnew; }

        /// <summary>функция возвращает панель с различной информацией для текстовой страницы</summary>
        /// <param name="pathToStructFile">путь к файлу со структурой данных страницы</param>
        /// <param name="tagList">список тегированных строк</param>
        /// <param name="siteBckgrd">true - включить фон, как на сайте</param>
        /// <returns></returns>
        public Panel GetPage(string pathToStructFile = "", List<string> tagList = null, bool siteBckgrd = false)
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panelTxtPageWrapper";

            if (siteBckgrd) { panelWrapper.CssClass = "panelTxtPageWrapper contentDiv"; }

            #region Блок записи тегированных строк в массив

            string[] str = { };
            if (pathToStructFile != "" && tagList == null)
            {
                if (File.Exists(pathToStructFile))
                {
                    str = File.ReadAllLines(pathToStructFile, Encoding.Default);
                }
            }
            else if (pathToStructFile == "" && tagList != null)
            {
                str = new string[tagList.Count];
                tagList.CopyTo(str);
            }

            #endregion

            #region Блок заполнения панели

            string style = "";
            string[] strSplit;
            int counter = 0;
            var img = new ImageForm(_pag);

            if (str.Length > 0)     //условие-подстраховка
            {
                int index = 0; var strBuilder = new StringBuilder();
                foreach (string item in str)
                {
                    strSplit = item.Split(new[] { '|' });
                    switch (strSplit[0])
                    {
                        case "ТЕКСТ":

                            #region Код

                            //шаблон - ТЕКСТ|<размер>|<стиль(жкпч - жирный курсив подчёркнутый перечёркнутый)>|<цвет(#000000)>|<h1-6>|<noindex(true/false)>|<текст>
                            if (strSplit.Length == 7) //проверка на правильность длины тега
                            {
                                string size = "";
                                if (strSplit[1].Trim() != "") { size = "font-size: " + strSplit[1] + "px; "; }
                                string zkp = "";
                                if (strSplit[2].Trim() != "")
                                {
                                    string str1 = strSplit[2].Trim();
                                    string teg = "";
                                    for (int i = 0; i < str1.Length; i++)
                                    {
                                        teg = str1.Substring(i, 1);
                                        if (teg == "ж") { zkp += " font-weight: bold;"; }
                                        else if (teg == "к") { zkp += " font-style: italic;"; }
                                        else if (teg == "п") { zkp += " text-decoration: underline;"; }
                                        else if (teg == "ч") { zkp += " text-decoration: line-through;"; }
                                    }
                                }
                                string color = "";
                                if (strSplit[3].Trim() != "") { color = "color: " + strSplit[3] + "; "; }

                                style = "style='" + size + zkp + color + "' ";
                                if (strSplit[5].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                if (strSplit[4].Trim() == "")
                                {
                                    panelWrapper.Controls.Add(new LiteralControl("<span class='txt' " + style + ">" + strSplit[6] + "</span>"));
                                }
                                else
                                {
                                    switch (strSplit[4].Trim())
                                    {
                                        case "h1":
                                            panelWrapper.Controls.Add(new LiteralControl("<h1 class='txt' " + style + ">" + strSplit[6] + "</h1>"));
                                            break;
                                        case "h2":
                                            panelWrapper.Controls.Add(new LiteralControl("<h2 class='txt' " + style + ">" + strSplit[6] + "</h2>"));
                                            break;
                                        case "h3":
                                            panelWrapper.Controls.Add(new LiteralControl("<h3 class='txt' " + style + ">" + strSplit[6] + "</h3>"));
                                            break;
                                        case "h4":
                                            panelWrapper.Controls.Add(new LiteralControl("<h4 class='txt' " + style + ">" + strSplit[6] + "</h4>"));
                                            break;
                                        case "h5":
                                            panelWrapper.Controls.Add(new LiteralControl("<h5 class='txt' " + style + ">" + strSplit[6] + "</h5>"));
                                            break;
                                        case "h6":
                                            panelWrapper.Controls.Add(new LiteralControl("<h6 class='txt' " + style + ">" + strSplit[6] + "</h6>"));
                                            break;
                                    }
                                }

                                if (strSplit[5].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ТЕКСТ НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;
                        #endregion

                        case "АБЗАЦН":

                            #region Код

                            //шаблон - АБЗАЦН|<высота шрифта в пикселях>|<стиль(жкп – жирный/курсив/ подчёркнутый)>|<цвет(#000)>|<отступ первой строки абзаца в пикселях>|<отступ перед после абзаца в пикселях>|<noindex>
                            if (strSplit.Length == 7) //проверка на правильность длины тега
                            {
                                string size = "";
                                if (strSplit[1].Trim() != "") { size = "font-size: " + strSplit[1] + "px; "; }
                                string zkp = "";
                                if (strSplit[2].Trim() != "")
                                {
                                    string str1 = strSplit[2].Trim();
                                    string teg = "";
                                    for (int i = 0; i < str1.Length; i++)
                                    {
                                        teg = str1.Substring(i, 1);
                                        if (teg == "ж") { zkp += " font-weight: bold;"; }
                                        else if (teg == "к") { zkp += " font-style: italic;"; }
                                        else if (teg == "п") { zkp += " text-decoration: underline;"; }
                                    }
                                }
                                string color = "";
                                if (strSplit[3].Trim() != "") { color = "color: " + strSplit[3] + "; "; }
                                string indent = "";
                                if (strSplit[4].Trim() != "") { indent = "text-indent: " + strSplit[4] + "px; "; }
                                string margin = "";
                                if (strSplit[5].Trim() != "") { margin = "margin: " + strSplit[5] + "px 0; "; }

                                style = "style='" + size + zkp + color + indent + margin + "' ";

                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                panelWrapper.Controls.Add(new LiteralControl("<p class='txtAbz' " + style + ">"));
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ТЕКСТ НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "АБЗАЦК":

                            #region Код

                            //шаблон - АБЗАЦК|
                            panelWrapper.Controls.Add(new LiteralControl("</p>"));
                            break;

                        #endregion

                        case "ОТСТУП":    //установка неразрывного пробела, 

                            #region Код

                            //шаблон - ОТСТУП|<кол-во неразрывных пробелов>
                            if (strSplit.Length == 2) //проверка на правильность длины тега
                            {
                                if (strSplit[1].Trim() != "")
                                {
                                    try
                                    {
                                        for (int i = 0; i < int.Parse(strSplit[1]); i++)
                                        { panelWrapper.Controls.Add(new LiteralControl("&nbsp;")); }
                                    }
                                    catch (Exception) { }
                                }
                                else
                                {
                                    panelWrapper.Controls.Add(new LiteralControl("&nbsp;"));
                                }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>НЕРАЗРЫВНЫЙ ПРОБЕЛ НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "НАЧНУМСПИСКА":

                            #region Код

                            //шаблон - НАЧНУМСПИСКА|<размер шрифта>|<цвет шрифта>
                            if (strSplit.Length == 3) //проверка на правильность длины тега
                            {
                                string size1 = "";
                                if (strSplit[1].Trim() != "") { size1 = "font-size: " + strSplit[1] + "px; "; }
                                string color1 = "";
                                if (strSplit[2].Trim() != "") { color1 = "color: " + strSplit[2] + "; "; }

                                style = "style='" + size1 + color1 + "' ";
                                panelWrapper.Controls.Add(new LiteralControl("<ol class='numericalListOl' " + style + ">"));
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>НАЧАЛО НУМЕРОВАННОГО СПИСКА НЕ ДОБАВЛЕНО, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "СТРНУМСПИСКАН":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<li class='numericalListLi'>"));
                            break;

                        #endregion

                        case "СТРНУМСПИСКАК":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</li>"));
                            break;

                        #endregion

                        case "КОННУМСПИСКА":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</ol>"));
                            break;

                        #endregion

                        case "НАЧДЕФСПИСКА":

                            #region Код

                            //шаблон - НАЧДЕФСПИСКА|<размер шрифта>|<цвет шрифта>
                            if (strSplit.Length == 3) //проверка на правильность длины тега
                            {
                                string size2 = "";
                                if (strSplit[1].Trim() != "") { size2 = "font-size: " + strSplit[1] + "px; "; }
                                string color2 = "";
                                if (strSplit[2].Trim() != "") { color2 = "color: " + strSplit[2] + "; "; }

                                style = "style='" + size2 + color2 + "' ";
                                panelWrapper.Controls.Add(new LiteralControl("<ul class='defisListUl' " + style + ">"));
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>НАЧАЛО ДЕФИСНОГО СПИСКА НЕ ДОБАВЛЕНО, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "СТРДЕФСПИСКАН":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<li class='defisListLi'>"));
                            break;

                        #endregion

                        case "СТРДЕФСПИСКАК":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</li>"));
                            break;

                        #endregion

                        case "КОНДЕФСПИСКА":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</ul>"));
                            break;

                        #endregion

                        case "ПЕРЕНОС":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<br />"));
                            break;

                        #endregion

                        case "картинка":

                            #region Код

                            //шаблон - картинка|<ширина>|<высота>|<тень(on/off)>|<увеличение(on/off)>|<путь к файлу>|<noindex(true/false)>|<alt>|<title>
                            if (strSplit.Length == 9) //проверка на правильность длины тега
                            {
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                try
                                {
                                    var imageF = img.GetImage(strSplit[1], strSplit[2], strSplit[3], strSplit[4], strSplit[5], strSplit[7], strSplit[8], counter.ToString());
                                    if (imageF != null) panelWrapper.Controls.Add(imageF);
                                }
                                catch (Exception ex)
                                {
                                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                                }
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>КАРТИНКА НЕ ДОБАВЛЕНА, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "гиперссылка":

                            #region Код

                            //шаблон новый - гиперссылка|<высота шрифта в пикселах>|<стиль(жк–жирный/курсив>|<цвет(#000)>|<текст>|<полный URL ресурса>|<noindex>|<nofollow>
                            if (strSplit.Length == 8)
                            {
                                string size = "";
                                if (strSplit[1].Trim() != "") { size = "font-size: " + strSplit[1] + "px; "; }
                                string zkp = "";
                                if (strSplit[2].Trim() != "")
                                {
                                    string str1 = strSplit[2].Trim();
                                    string teg = "";
                                    for (int i = 0; i < str1.Length; i++)
                                    {
                                        teg = str1.Substring(i, 1);
                                        if (teg == "ж") { zkp += " font-weight: bold;"; }
                                        else if (teg == "к") { zkp += " font-style: italic;"; }
                                    }
                                }
                                string color = "";
                                if (strSplit[3].Trim() != "") { color = "color: " + strSplit[3] + "; "; }

                                style = "style='" + size + zkp + color + "' ";

                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                string nofollow = "";
                                if (strSplit[7].Trim() == "true") { nofollow = " rel='nofollow' "; }
                                panelWrapper.Controls.Add(new LiteralControl("<a class='hyperLink' " + style + " href='" + strSplit[5] + "' target='_blank'" + nofollow + ">" + strSplit[4] + "</a>"));
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ГИПЕРССЫЛКА НЕ ДОБАВЛЕНА, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "ЛИТЕРАЛ":

                            #region Код

                            //шаблон - ЛИТЕРАЛ|<содержимое>
                            if (strSplit.Length == 2)
                            {
                                panelWrapper.Controls.Add(new LiteralControl(strSplit[1] + "&nbsp;"));
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ЛИТЕРАЛ НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "гиперссылкафайл":

                            #region Код

                            //шаблон - гиперссылка|<высота шрифта в пикселах>|<стиль(жк–жирный/курсив>|<цвет(#000)>|<текст>|<полный URL ресурса>|<noindex>|<nofollow>
                            if (strSplit.Length == 8)
                            {
                                string size = "";
                                if (strSplit[1].Trim() != "") { size = "font-size: " + strSplit[1] + "px; "; }
                                string zkp = "";
                                if (strSplit[2].Trim() != "")
                                {
                                    string str1 = strSplit[2].Trim();
                                    string teg = "";
                                    for (int i = 0; i < str1.Length; i++)
                                    {
                                        teg = str1.Substring(i, 1);
                                        if (teg == "ж") { zkp += " font-weight: bold;"; }
                                        else if (teg == "к") { zkp += " font-style: italic;"; }
                                    }
                                }
                                string color = "";
                                if (strSplit[3].Trim() != "") { color = "color: " + strSplit[3] + "; "; }

                                style = "style='" + size + zkp + color + "' ";
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                string nofollow = "";
                                if (strSplit[7].Trim() == "true") { nofollow = " rel='nofollow' "; }
                                panelWrapper.Controls.Add(new LiteralControl("<a class='hyperLink' " + style + " href='" + strSplit[5] + "'" + nofollow + " >" + strSplit[4] + "</a>"));
                                if (strSplit[6].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ГИПЕРССЫЛКА НА ФАЙЛ НЕ ДОБАВЛЕНА, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "карта":

                            #region Код

                            if (strSplit.Length == 3) //проверка на правильность длины тега
                            {
                                if (strSplit[1].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                panelWrapper.Controls.Add(new LiteralControl("<div class='divVideoMain'><span><div class='divVideo'><script type='text/javascript' charset='utf-8' src='" + strSplit[2] + "'></script></div></span></div>"));
                                if (strSplit[1].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>КАРТА НЕ ДОБАВЛЕНА, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "видеоизвне":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<div class='divVideoMain'><span><div class='divVideo'>" + strSplit[1] + "</div></span></div>"));
                            break;

                        #endregion

                        case "таблначало":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<table class='tblTxtPage'>"));
                            break;

                        #endregion

                        case "таблСтрокаН":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<tr>"));
                            break;

                        #endregion

                        case "таблСтрокаК":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</tr>"));
                            break;

                        #endregion

                        case "таблЯчейкаН":

                            #region Код

                            //шаблон - таблЯчейкаН|<цвет фона>|<толщина бордюра>|<цвет бордюра>|<отступ сверху/снизу>|<отступ справа/слева>|<colspan>|<rowspan>|
                            //<выравнивание по горизонтали(л/п/с/ш)>|<выравнивание по вертикали(в/н/с)|<ширина ячейки>|<высота ячейки>
                            if (strSplit.Length == 12) //проверка на правильность длины тега
                            {
                                string bckgrdcolor = "";
                                if (strSplit[1].Trim() != "") { bckgrdcolor = "background-color: " + strSplit[1].Trim() + "; "; }
                                string border = "border: " + strSplit[2].Trim() + "px " + strSplit[3].Trim() + " solid; ";
                                string padding = "padding: " + strSplit[4].Trim() + "px " + strSplit[5].Trim() + "px; ";
                                string colspan = "colspan='" + strSplit[6].Trim() + "' ";
                                string rowspan = "rowspan='" + strSplit[7].Trim() + "' ";
                                string txtalign = "";
                                if (strSplit[8].Trim() == "л") { txtalign = "text-align: left; "; }
                                else if (strSplit[8].Trim() == "п") { txtalign = "text-align: right; "; }
                                else if (strSplit[8].Trim() == "с") { txtalign = "text-align: center; "; }
                                else if (strSplit[8].Trim() == "ш") { txtalign = "text-align: justify; "; }
                                string vertalign = "";
                                if (strSplit[9].Trim() == "в") { vertalign = "vertical-align: top; "; }
                                else if (strSplit[9].Trim() == "н") { vertalign = "vertical-align: bottom; "; }
                                else if (strSplit[9].Trim() == "с") { vertalign = "vertical-align: middle; "; }
                                string width = "";
                                if (strSplit[10].Trim() != "") { width = "width: " + strSplit[10].Trim() + "px; "; }
                                string height = "";
                                if (strSplit[11].Trim() != "") { height = "height: " + strSplit[11].Trim() + "px; "; }

                                string resultStr = colspan + rowspan + "style='" + bckgrdcolor + border + padding + txtalign + vertalign + width + height + "' ";
                                panelWrapper.Controls.Add(new LiteralControl("<td " + resultStr + ">"));
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>НАЧАЛО ЯЧЕЙКИ ТАБЛИЦЫ НЕ ДОБАВЛЕНО, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }
                            break;

                        #endregion

                        case "таблЯчейкаК":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</td>"));
                            break;

                        #endregion

                        case "таблконец":

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</table>"));
                            break;

                        #endregion

                        case "раскрпанельшапкаНач":         //выпадающая панель шапка, начало

                            #region Код

                            index++;
                            panelWrapper.Controls.Add(new LiteralControl("<div class='header' onclick='showhide" + index + "();'>"));   //добавляем начало DIV шапки раскрывающейся панели
                            break;

                        #endregion

                        case "раскрпанельшапкаКон":         //выпадающая панель шапка, конец

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</div>"));        //добавляем конец DIV с содержимым шапки раздвигающейся панели
                            break;

                        #endregion

                        case "раскрпанельсодержНач":        //выпадающая панель содержание, начало

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("<div id='content" + index + "' class='content'>"));      //добавляем начало DIV с содержимым раздвигающейся панели
                            break;

                        #endregion

                        case "раскрпанельсодержКон":        //выпадающая панель содержание, начало

                            #region Код

                            panelWrapper.Controls.Add(new LiteralControl("</div>"));   //добавляем конец DIV с содержимым раздвигающейся панели

                            #region Добавление скрипта раскрывающейся панели на страницу

                            strBuilder = new StringBuilder();
                            strBuilder.AppendLine("<noindex><script language='javascript' type='text/javascript'>");
                            strBuilder.Append("var contentPanelId" + index + " = 'content" + index + "';");
                            strBuilder.Append("var showOnStart" + index + " = 0;");
                            strBuilder.Append("$(document).ready(function () {");
                            strBuilder.Append("var obj = $('#' + contentPanelId" + index + ");");
                            strBuilder.Append("if (showOnStart" + index + " == 0) { obj.hide(); }");
                            strBuilder.Append("else if (showOnStart" + index + " == 1) { obj.show(); }");
                            strBuilder.Append("});");
                            strBuilder.Append("function showhide" + index + "() {");
                            strBuilder.Append("var obj = $('#' + contentPanelId" + index + ");");
                            strBuilder.Append("if (showOnStart" + index + " == 0) {");
                            strBuilder.Append("obj.show(500);");
                            strBuilder.Append("showOnStart" + index + " = 1;");
                            strBuilder.Append("}");
                            strBuilder.Append("else if (showOnStart" + index + " == 1) {");
                            strBuilder.Append("obj.hide(500);");
                            strBuilder.Append("showOnStart" + index + " = 0;");
                            strBuilder.Append("};");
                            strBuilder.AppendLine("};");
                            strBuilder.AppendLine("</script></noindex>");
                            _pag.Controls.Add(new LiteralControl(strBuilder.ToString()));

                            #endregion

                            break;

                        #endregion

                        case "слайдшоу":        //добавляем слайд-шоу с ID, указанном в тэге

                            #region Код

                            var sliderForm = new SlideShowForm(_pag, strSplit[1]);
                            panelWrapper.Controls.Add(sliderForm.GetSlideShow());

                            break;

                        #endregion

                        case "СПИСОКФОТО":        //добавляем анимированный список изображений с ID, указанном в тэге

                            #region Код

                            //Шаблон - СПИСОКФОТО|<uniqid>|<noindex>
                            if (strSplit.Length == 3) //проверка на правильность длины тега
                            {
                                var sliderScroll = new SlideScrollForm(_pag);
                                if (strSplit[2].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                panelWrapper.Controls.Add(sliderScroll.GetSlideScroll(strSplit[1]));
                                if (strSplit[2].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>АНИМИРОВАННЫЙ СПИСОК ИЗОБРАЖЕНИЙ НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }

                            break;

                        #endregion

                        case "БЛОКПАССН":       //Добавляется начало блока, при наведении на который мышкой будет отображаться блок БЛОКПОЯВЛН

                            #region Код

                            //шаблон - БЛОКПАССН|<uniqid>
                            if (strSplit.Length == 2) //проверка на правильность длины тега
                            {
                                if (strSplit[1].Trim() != "")
                                {
                                    panelWrapper.Controls.Add(new LiteralControl("<div id='hoverBlock_" + strSplit[1] + "' class='hoverBlock'>"));
                                }
                                else
                                {
                                    panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ПАССИВНЫЙ БЛОК НЕ ДОБАВЛЕН(id не должен быть пустым), ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                                }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ПАССИВНЫЙ БЛОК НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }

                            break;

                        #endregion

                        case "БЛОКПАССК":       //Добавляется конец блока, при наведении на который мышкой будет отображаться блок БЛОКПОЯВЛН

                            #region Код

                            //шаблон - БЛОКПАССК|
                            panelWrapper.Controls.Add(new LiteralControl("</div>"));

                            break;

                        #endregion

                        case "БЛОКПОЯВЛН":       //Добавляется начало блока, который появляется при наведении мышкой на блок БЛОКПАССН

                            #region Код

                            //шаблон - БЛОКПОЯВЛН|<uniqid>|<ширина>|<высота>|<noindex>
                            if (strSplit.Length == 5) //проверка на правильность длины тега
                            {
                                if (strSplit[1].Trim() != "")
                                {
                                    string width = "";
                                    if (strSplit[2].Trim() != "") { width = "width: " + strSplit[2].Trim() + "px; "; }
                                    string height = "";
                                    if (strSplit[3].Trim() != "") { height = "height: " + strSplit[3].Trim() + "px; "; }

                                    string resultStr = "style='" + width + height + "' ";

                                    if (strSplit[4].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("<noindex>")); }
                                    panelWrapper.Controls.Add(new LiteralControl("<div id='popupBlock_" + strSplit[1] + "' class='popupBlock' " + resultStr + ">"));
                                    if (strSplit[4].Trim() == "true") { panelWrapper.Controls.Add(new LiteralControl("</noindex>")); }

                                    #region Добавление скрипта для всплывающего блока на страницу

                                    strBuilder = new StringBuilder();
                                    strBuilder.AppendLine("<noindex><script language='javascript' type='text/javascript'>");
                                    strBuilder.Append("$(document).ready(function () {");

                                    strBuilder.Append("var $hoverObj" + strSplit[1] + " = $('#hoverBlock_" + strSplit[1] + "');");
                                    strBuilder.Append("var $popupObj" + strSplit[1] + " = $('#popupBlock_" + strSplit[1] + "');");
                                    strBuilder.Append("var sTimeOut" + strSplit[1] + ";");

                                    //Мышка вошла в область элемента
                                    strBuilder.Append("$hoverObj" + strSplit[1] + ".mouseenter(function () {");
                                    strBuilder.Append("sTimeOut" + strSplit[1] + " = window.setTimeout(function () {");
                                    strBuilder.Append("var popupHeight" + strSplit[1] + " = $popupObj" + strSplit[1] + ".height();");
                                    //strBuilder.Append("$popupObj" + strSplit[1] + ".children().width($popupObj" + strSplit[1] + ".width());");
                                    //strBuilder.Append("$popupObj" + strSplit[1] + ".children().height(popupHeight" + strSplit[1] + ");");
                                    strBuilder.Append("$popupObj" + strSplit[1] + ".show(300).offset({ top: $hoverObj" + strSplit[1] + ".offset().top - popupHeight" + strSplit[1] + " / 2, left: $hoverObj" + strSplit[1] + ".offset().left + $hoverObj" + strSplit[1] + ".innerWidth() + 15 });");
                                    strBuilder.Append("}, 300);");
                                    strBuilder.Append("});");

                                    //Мышка вышла из области элемента
                                    strBuilder.Append("$hoverObj" + strSplit[1] + ".mouseleave(function () {");
                                    strBuilder.Append("window.clearTimeout(sTimeOut" + strSplit[1] + ");");
                                    strBuilder.Append("$popupObj" + strSplit[1] + ".hide(300);");
                                    strBuilder.Append("});");

                                    strBuilder.AppendLine("});");
                                    strBuilder.AppendLine("</script></noindex>");
                                    _pag.Controls.Add(new LiteralControl(strBuilder.ToString()));

                                    #endregion

                                    //СТИЛИ С БЛОКАМ НАХОДЯТСЯ В ФАЙЛЕ txtpages.css
                                }
                                else
                                {
                                    panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ПОЯВЛЯЮЩИЙСЯ БЛОК НЕ ДОБАВЛЕН(id не должен быть пустым), ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                                }
                            }
                            else
                            {
                                panelWrapper.Controls.Add(new LiteralControl("<span class='txtErr'>ПОЯВЛЯЮЩИЙСЯ БЛОК НЕ ДОБАВЛЕН, ТРЕБУЕТСЯ КОРРЕКТИРОВКА</span>"));
                            }

                            break;

                        #endregion

                        case "БЛОКПОЯВЛК":       //Добавляется конец блока, который появляется при наведении мышкой на блок БЛОКПАССН

                            #region Код

                            //шаблон - БЛОКПОЯВЛК|
                            panelWrapper.Controls.Add(new LiteralControl("</div>"));

                            break;

                        #endregion

                        default:
                            break;
                    }
                    counter++;
                }
            }

            #endregion

            return panelWrapper;
        }

        /// <summary>Метод добавляет на страницу содержимое тега title, мета-тегов description и keywords</summary>
        /// <param name="pageFileName"></param>
        /// <param name="cacheContr">установка управления cache браузера</param>
        public void AddSeo(string pageFileName, PagesFormE.CacheContr cacheContr)
        {
            var pageWork = new PagesWorkClass(_pag);
            var pageNameStruct = pageWork.GetPagesNameStruct("", pageFileName);
            if (pageNameStruct != null)
            {
                //Добавление заголовка о модификации страницы - например: Last-Modified: Sun, 09 Aug 2015 13:44:12 GMT
                var fInfo = new FileInfo(_pag.Server.MapPath("~") + @"files/pages/" + pageFileName);
                _pag.Response.Cache.SetLastModified(fInfo.LastWriteTime);

                //Добавляем TITLE, DESCRIPTION, KEYWORDS
                _pag.Title = pageNameStruct.Title;
                _pag.Header.Controls.Add(new LiteralControl("<meta name='description' content='" + pageNameStruct.Description + "' />"));
                _pag.Header.Controls.Add(new LiteralControl("<meta name='keywords' content='" + pageNameStruct.Keywords + "' />"));

                //CACHE
                if (cacheContr == PagesFormE.CacheContr.Cash)
                {
                    // здесь не нужен код
                }
                else if (cacheContr == PagesFormE.CacheContr.DisableCash)
                {
                    _pag.Response.Cache.SetCacheability(HttpCacheability.Private);
                    _pag.Response.Cache.SetMaxAge(TimeSpan.Zero);
                }
                else if (cacheContr == PagesFormE.CacheContr.NoCache)
                {
                    _pag.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
            }
        }

        /// <summary>Полное отключение кэширования на странице</summary>
        public static void DisableCache(Page page)
        {
            page.Response.Cache.SetCacheability(HttpCacheability.Private);
            page.Response.Cache.SetMaxAge(TimeSpan.Zero);
        }

        /// <summary>Отключение кэширования на странице при новом запуске браузера. Во время работы браузера кэширование работает.</summary>
        public static void NoCache(Page page)
        {
            page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }

    /// <summary>Класс предназначен для отображения редакторов текстовых страниц.
    /// Данный класс берёт данные для редакторов через класс PagesWorkClass.cs</summary>
    public class PagesFormAdm
    {
        private Page _pag;

        public PagesFormAdm(Page pagenew) { _pag = pagenew; }

        /// <summary>функция возвращает панель с выбором страницы для редактирования из списка</summary>
        /// <returns></returns>
        public Panel GetPagesChoosePanel()
        {
            Panel panelWrapper = new Panel();
            PagesWorkClass pagesWorkClass = new PagesWorkClass(_pag);

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР ТЕКСТОВЫХ СТРАНИЦ"; panelWrapper.Controls.Add(lbl);

            //Панель добавления новой страницы
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm placeHolder"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxPageName"; txtBox.Width = 200;
            txtBox.Text = ""; txtBox.Attributes.Add("placeholder", "имя страницы");
            panelLine.Controls.Add(txtBox);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm placeHolder"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxPageFileName"; txtBox.Width = 250;
            txtBox.Text = ""; txtBox.Attributes.Add("placeholder", "имя файла страницы - на латинице");
            panelLine.Controls.Add(txtBox);

            //Кнопка добавления новой страницы
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Создать страницу ";
            lBtn.ToolTip = "Создание новой страницы";
            lBtn.Command += (lBtnAddNewPage_Command);
            panelLine.Controls.Add(lBtn);

            panelWrapper.Controls.Add(panelLine);

            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == "администратор")
            {
                //кнопка удаления лишних фотографий
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " Удалить лишние фотографии ";
                lBtn.ToolTip = "В процессе создания текстовых страниц могут оставаться лишние фотографии. Данная кнопка предназначена для их удаления и очистки временной папки.";
                lBtn.Command += (lBtnDelOldPagesFoto_Command); panelWrapper.Controls.Add(lBtn);

                // Вывод размера файлов "БД" и размера папок с файлами к страницам
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер файлов БД: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = pagesWorkClass.GetDbSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер папок с файлами: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = pagesWorkClass.GetFoldersSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);
            }
            
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //выпадающий список с выбором текстовой страницы для редактирования для редактирования
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выберите текстовую страницу для редактирования: "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            foreach (PagesNameStruct oneStruct in pagesWorkClass._namesAndPathesToTxtFiles)
            {
                ddl.Items.Add(oneStruct.Name + "|" + Path.GetFileName(oneStruct.FileName));
            }
            if (_pag.Session["PagesStruct"] != null)
            {
                int counter = 0;
                foreach (ListItem name in ddl.Items)
                {
                    if (name.Text == ((PagesStruct)_pag.Session["PagesStruct"]).NameForSite) { ddl.SelectedIndex = counter; break; }
                    counter++;
                }
            }
            ddl.ID = "ddlPagesSelect"; panelLine.Controls.Add(ddl);

            //запуск редактирования выбранной в списке новости
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Редактировать ";
            lBtn.ToolTip = "Запуск редактирования выбранной в списке текстовой страницы.";
            lBtn.Command += (lBtnEditPage_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            //очистка содержимого страницы
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Очистить ";
            lBtn.ToolTip = "Очистка всего содержимого выбранной для редактирования текстовой страницы.";
            lBtn.Command += (lBtnCleanPage_Command); lBtn.OnClientClick = "return confirm('Содержимое страницы будет полностью удалено. Сделать?');";
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            //удаление текстовой страницы и данных о ней из файла данных о всех текстовых страницах
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Удалить ";
            lBtn.ToolTip = "Удаление всех данных о выбранной для редактирования текстовой страницы.";
            lBtn.Command += (lBtnDeletePage_Command); lBtn.OnClientClick = "return confirm('Страница будет полностью удалена. Сделать?');";
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetPagesChoosePanel()

        /// <summary>событие нажатия на кнопку "редактировать" выбранную в выпадающем списке текстовую страницу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEditPage_Command(object sender, CommandEventArgs e)
        {
            var pagesWorkClass = new PagesWorkClass(_pag);
            string val = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlPagesSelect")).SelectedValue;
            if (val == "") { _pag.Session["PagesStruct"] = null; }
            else
            {
                string[] strSplit = val.Split(new[] { '|' });
                _pag.Session["PagesStruct"] = pagesWorkClass.GetPageStruct(val);
                _pag.Session["PagesNameStruct"] = pagesWorkClass.GetPagesNameStruct("", strSplit[1]);
            }

            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>события нажатия на кнопку "удалить лишние фотографии"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDelOldPagesFoto_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var pagesWorkClass = new PagesWorkClass(_pag);
            pagesWorkClass.DeleteOldPageImages();

            //var warning = new WarnClass();
            //warning.ShowWarning("УСПЕХ. Лишние файлы картинок для всех текстовых страниц удалены.", _pag.Master);
        }

        /// <summary>события нажатия на кнопку "очистить"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCleanPage_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var pagesWorkClass = new PagesWorkClass(_pag);
            if (_pag.Session["PagesNameStruct"] != null)
            {
                var warning = new WarnClass(); warning.HideWarning(_pag.Master);
                if (!pagesWorkClass.CleanPageStructFile(((PagesNameStruct)_pag.Session["PagesNameStruct"])))
                {
                    warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе очистки данных страницы. Попробуйте повторить.", _pag.Master);
                }
                else
                {
                    ((PagesStruct)_pag.Session["PagesStruct"]).ListOfTagTxt = new List<string>();
                    if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                    else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
                }
            }
        }

        /// <summary>события нажатия на кнопку "удалить"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDeletePage_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var pagesWorkClass = new PagesWorkClass(_pag);
            if (_pag.Session["PagesNameStruct"] != null)
            {
                var warning = new WarnClass(); warning.HideWarning(_pag.Master);
                if (!pagesWorkClass.DeletePageStructFile(((PagesNameStruct)_pag.Session["PagesNameStruct"])))
                {
                    warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе удаления данных о странице. Попробуйте повторить.", _pag.Master);
                }
                else
                {
                    _pag.Session["PagesStruct"] = null;
                    if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                    else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
                }
            }
        }

        /// <summary>события нажатия на кнопку "создать страницу"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAddNewPage_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var pagesWorkClass = new PagesWorkClass(_pag);
            var warning = new WarnClass(); warning.HideWarning(_pag.Master);

            string name = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageName")).Text;
            string filename = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageFileName")).Text;
            var pageNameStruct = new PagesNameStruct();

            //ПРОВЕРКА ЗНАЧЕНИЙ (на повтор, на пустые, на то, что имя файла введено латиницей)
            #region Код

            if (name.Trim() == "")
            {
                warning.ShowWarning("ВНИМАНИЕ. Имя страницы не должно быть пустым.", _pag.Master);
                return;
            }
            if (filename.Trim() == "")
            {
                warning.ShowWarning("ВНИМАНИЕ. Имя файла страницы не должно быть пустым.", _pag.Master);
                return;
            }
            if (!IsStringLatin.IsLatin(filename))
            {
                warning.ShowWarning("ВНИМАНИЕ. Имя файла страницы должно содержать только латинские символы.", _pag.Master);
                return;
            }
            if (pagesWorkClass.GetPagesNameStruct(name, "") != null)
            {
                warning.ShowWarning("ВНИМАНИЕ. Страница с таким именем уже существует. Придумайте другое название.", _pag.Master);
                return;
            }
            if (pagesWorkClass.GetPagesNameStruct("", filename) != null)
            {
                warning.ShowWarning("ВНИМАНИЕ. Страница с таким именем файла уже существует. Придумайте другое название.", _pag.Master);
                return;
            }

            #endregion

            //Заполняем свойства объекта
            pageNameStruct.Name = name;
            pageNameStruct.FileName = filename;
            pageNameStruct.Title = "";
            pageNameStruct.Description = "";
            pageNameStruct.Keywords = "";

            if (!pagesWorkClass.AddNewPageNameStruct(pageNameStruct))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе создания новой страницы. Попробуйте повторить.", _pag.Master);
            }
            else
            {
                if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
                else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
            }
        }

        #endregion

        /// <summary>функция возвращает таблицу с редактором одной текстовой страницы</summary>
        /// <returns></returns>
        public Panel GetPagesEditPanel()
        {
            var panelWrapper = new Panel();

            //проверка на то, что временная структура с данными по странице существует
            if (_pag.Session["PagesStruct"] == null) return panelWrapper;

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle";
            lbl.Text = "Панель редактирования содержимого текстовой страницы"; panelWrapper.Controls.Add(lbl);

            #region ДОБАВЛЯЕМ ПАНЕЛЬ, В КОТОРОЙ НУЖНО БУДЕТ СОСТАВИТЬ страницу с полным описанием текстовой страницы сайта

            var panel1 = new Panel(); panel1.ID = "panelLine"; panelWrapper.Controls.Add(panel1);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Добавить в описание страницы: "; panel1.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panel1.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm"; ddl.ToolTip = "Выбрать добавляемые на страницу тэги.";
            ddl.Items.Add("АБЗАЦ");
            ddl.Items.Add("видеоизвне");
            ddl.Items.Add("гиперссылка");
            ddl.Items.Add("гиперссылкафайл");
            ddl.Items.Add("карта");
            ddl.Items.Add("картинка");
            ddl.Items.Add("ЛИТЕРАЛ");
            ddl.Items.Add("ОТСТУП");
            ddl.Items.Add("ПЕРЕНОС");
            ddl.Items.Add("раскрпанель");
            ddl.Items.Add("ПОЯВЛБЛОК");
            ddl.Items.Add("таблица");
            ddl.Items.Add("ТЕКСТ");
            ddl.Items.Add("нумерованныйСписок");
            ddl.Items.Add("дефисныйСписок");
            ddl.Items.Add("слайдшоу");
            ddl.Items.Add("СПИСОКФОТО");
            ddl.ID = "ddlPageTags";
            panel1.Controls.Add(ddl);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panel1.Controls.Add(lbl);  //отступ

            var fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fotoImgUpload";
            fUpload.ToolTip = "Загружайте картинки размером не более 500 килобайт.";
            panel1.Controls.Add(fUpload);

            var chkBox = new CheckBox(); chkBox.ToolTip = "поставьте здесь галочку, если нужно наложить защитное изображение на добавляемую картинку";
            chkBox.ID = "chkBoxOverlayImgAdd"; panel1.Controls.Add(chkBox);

            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Добавить ";
            lBtn.ToolTip = "В поле ниже добавляется тег, выбранный в выпадающем списке слева.";
            string[] strSplit = ((PagesStruct)_pag.Session["PagesStruct"]).NameForSite.Split(new[] { '|' });
            lBtn.CommandArgument = strSplit[0].Replace(" ", "_");
            lBtn.Command += (lBtnAddPageTag_Command); lBtn.ID = "btnAddPageTag"; panel1.Controls.Add(lBtn);

            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 1193; txtBox.Height = 600;
            txtBox.TextMode = TextBoxMode.MultiLine;
            string content = "";
            foreach (string line in ((PagesStruct)_pag.Session["PagesStruct"]).ListOfTagTxt)
            {
                content += line + Environment.NewLine;
            }
            txtBox.Text = content; txtBox.ID = "txtBoxPageContent"; panel1.Controls.Add(txtBox);
            panel1.Controls.Add(new LiteralControl("<br />"));

            #region Поля для добавления строки для тега <title>, мета тегов description и keywords

            //Заголовок страницы - Title
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Заголовок страницы(title)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxTitle"; txtBox.Width = 800;
            txtBox.Text = "";
            if (_pag.Session["PagesNameStruct"] != null) { txtBox.Text = ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Title; }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Содержимое значения content мета-тега description
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Описание страницы(description)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxDescription"; txtBox.Width = 800;
            txtBox.Text = "";
            if (_pag.Session["PagesNameStruct"] != null) { txtBox.Text = ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Description; }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Содержимое значения content мета-тега keywords
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Ключевые слова страницы(keywords)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxKeywords"; txtBox.Width = 800;
            txtBox.Text = "";
            if (_pag.Session["PagesNameStruct"] != null) { txtBox.Text = ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Keywords; }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Посмотреть результат ";
            lBtn.ToolTip = "Посмотреть как будет выглядеть текстовая страница.";
            lBtn.Command += (lBtnLookResult2_Command); lBtn.ID = "btnLookPageResult"; panel1.Controls.Add(lBtn);

            #endregion

            //Кнопка СОХРАНИТЬ текстовую страницу сайта
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Сохранить ";
            lBtn.ToolTip = "Сохраняются все изменения, которые были сделаны в структуре страницы.";
            lBtn.Command += (lBtnOnePageSave_Command); lBtn.ID = "btnOnePageSave"; panelWrapper.Controls.Add(lBtn);

            return panelWrapper;
        }

        #region События для функции GetPagesEditPanel()

        /// <summary>событие нажатия на кнопку - "добавить" тег в TextBox ввода данных о содержимом страницы</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит название страницы (пробелы заменены на символ '_')</param>
        protected void lBtnAddPageTag_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            string value = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlPageTags")).SelectedValue;
            var txtBoxActionContent = (TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageContent");
            bool adder = false;
            //подправим на правильные значения, если нужно
            if (value == "гиперссылка") { value = "гиперссылка|14|жк|#000|текст|URL|false"; }
            else if (value == "ТЕКСТ")
            {
                adder = true;

                value = "ТЕКСТ|14|жкп|#000000||false|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "карта")
            {
                adder = true;

                value = "карта|false|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "картинка")   //если нужно добавить картинку, то нужно её для начала закачать на сервер..
            {
                adder = true;

                #region Код загрузки файла картинки на сервер

                var fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fotoImgUpload");
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
                            #region Код сохранения файла на серевер

                            //сохраним путь к папке картинок для новостей
                            string pathToDir = _pag.Server.MapPath("~") + @"files\pages\images";

                            //создадим имя для файла картинки
                            string fName = ""; string tempPath = "";
                            string randomCifir = "";
                            bool checker = true;
                            Random rn;
                            do
                            {
                                rn = new Random();
                                randomCifir = rn.Next(1, 6666).ToString();
                                fName = e.CommandArgument + "_" + randomCifir + extension;
                                if (!File.Exists(pathToDir + @"\" + fName)) { checker = false; }
                            } while (checker);

                            //сохраним файл в папку с картинками для текстовых страниц
                            try
                            {
                                tempPath = HttpContext.Current.Server.MapPath("~") + @"files\temp\";
                                tempPath += "tempimg_" + randomCifir + extension;
                                fUpload.SaveAs(tempPath);
                                fUpload.Dispose();

                                #region Код наложения защитной картинки на сохранённое изображение

                                if (((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxOverlayImgAdd")).Checked)
                                {
                                    string pathToOverlayImg = HttpContext.Current.Server.MapPath("~") + @"files\pages\overlayimg.png";
                                    if (File.Exists(pathToOverlayImg))
                                    {
                                        var imgOverlay = new ImageOverlay();
                                        Bitmap tempImage = new Bitmap(tempPath);
                                        var newImage = imgOverlay.Overlay(tempImage, new Bitmap(pathToOverlayImg));
                                        if (extension.ToLower() == ".jpg")
                                        {
                                            newImage.Save(pathToDir + @"\" + fName, ImageFormat.Jpeg);
                                        }
                                        else if (extension.ToLower() == ".jpeg")
                                        {
                                            newImage.Save(pathToDir + @"\" + fName, ImageFormat.Jpeg);
                                        }
                                        else if (extension.ToLower() == ".png")
                                        {
                                            newImage.Save(pathToDir + @"\" + fName, ImageFormat.Png);
                                        }
                                        tempImage.Dispose();
                                        newImage.Dispose();
                                    }
                                    File.Delete(tempPath);
                                }
                                else
                                {
                                    if (File.Exists(pathToDir + @"\" + fName))
                                    {
                                        File.Delete(pathToDir + @"\" + fName);
                                    }
                                    File.Move(tempPath, pathToDir + @"\" + fName);
                                }

                                #endregion
                            }
                            catch { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла картинки на сервер. Попробуйте повторить.", _pag.Master); return; }

                            //сохраним путь к файлу в переменную
                            value = value + "|200||on|on|../files/pages/images/" + fName + "|false|изображение|подсказка";

                            //добавим тег в текстбокс
                            txtBoxActionContent.Text += Environment.NewLine + value;

                            #endregion
                        }
                        else { warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 500 килобайт.", _pag.Master); return; }
                    }
                    else { warning.ShowWarning("ВНИМАНИЕ. Доступно добавление только файлов формата 'jpg', 'jpeg' или 'png'.", _pag.Master); return; }
                }
                else { warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", _pag.Master); return; }

                #endregion
            }
            else if (value == "гиперссылкафайл")   //если нужно файл, то нужно её для начала закачать на сервер..
            {
                adder = true;

                #region Код загрузки файла на сервер

                var fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fotoImgUpload");
                if (fUpload.HasFile)
                {
                    warning.HideWarning(_pag.Master);

                    string fileName = _pag.Server.HtmlEncode(fUpload.FileName);
                    string extension = Path.GetExtension(fileName);

                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 95000000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        #region Код сохранения файла на серевер

                        //сохраним путь к папке картинок для новостей
                        string pathToDir = _pag.Server.MapPath("~") + @"files\pages\files";

                        //создадим имя для файла картинки
                        string fName = "";
                        bool checker = true;
                        Random rn;
                        do
                        {
                            rn = new Random();
                            fName = e.CommandArgument + "__" + rn.Next(1, 6666) + extension;
                            if (!File.Exists(pathToDir + @"\" + fName)) { checker = false; }
                        } while (checker);

                        //сохраним файл в папку с картинками для новостей
                        try { fUpload.SaveAs(pathToDir + @"\" + fName); }
                        catch { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", _pag.Master); return; }

                        //сохраним путь к файлу в переменную
                        value = "гиперссылкафайл|14|жк|#000|текст|../files/pages/files/" + fName + "|false|false";

                        //добавим тег в текстбокс
                        txtBoxActionContent.Text += Environment.NewLine + value;

                        #endregion
                    }
                    else { warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 95 мегабайт.", _pag.Master); return; }
                }
                else { warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", _pag.Master); return; }

                #endregion
            }
            else if (value == "АБЗАЦ")            //если нужно добавить таблицу
            {
                adder = true;
                value = "АБЗАЦН|14|жкп|#000|25|5|false" + Environment.NewLine + Environment.NewLine + "АБЗАЦК|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "таблица")            //если нужно добавить таблицу
            {
                adder = true;
                value = "таблначало|" + Environment.NewLine + "таблСтрокаН|" + Environment.NewLine + "таблЯчейкаН|#ffffff|1|#cccccc|3|9|1|1|с|с||" + Environment.NewLine + Environment.NewLine +
                        "таблЯчейкаК|" + Environment.NewLine + "таблСтрокаК|" + Environment.NewLine + "таблконец|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "нумерованныйСписок")            //если нужно добавить нумерованный список
            {
                adder = true;
                value = "НАЧНУМСПИСКА|14|#000000" + Environment.NewLine + "СТРНУМСПИСКАН|" + Environment.NewLine + Environment.NewLine + "СТРНУМСПИСКАК|" + Environment.NewLine + "КОННУМСПИСКА|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "дефисныйСписок")            //если нужно добавить дефисный список
            {
                adder = true;
                value = "НАЧДЕФСПИСКА|14|#000000" + Environment.NewLine + "СТРДЕФСПИСКАН|" + Environment.NewLine + Environment.NewLine + "СТРДЕФСПИСКАК|" + Environment.NewLine + "КОНДЕФСПИСКА|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "раскрпанель")            //если нужно добавить раскрывающуюся панель
            {
                adder = true;
                value = "раскрпанельшапкаНач|" + Environment.NewLine + Environment.NewLine + "раскрпанельшапкаКон|" + Environment.NewLine + "раскрпанельсодержНач|" + Environment.NewLine + Environment.NewLine + "раскрпанельсодержКон|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "ПОЯВЛБЛОК")            //если нужно добавить блок, появляющийся при наведении на элемент 
            {
                adder = true;
                value = "БЛОКПАССН|<uniqid>" + Environment.NewLine + Environment.NewLine + "БЛОКПАССК|" + Environment.NewLine + "БЛОКПОЯВЛН|<uniqid>|<ширина>|<высота>|false" + Environment.NewLine + Environment.NewLine + "БЛОКПОЯВЛК|";
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }
            else if (value == "СПИСОКФОТО")            //если нужно анимированный список фотографий 
            {
                adder = true;
                value = "СПИСОКФОТО|<uniqid>|<noindex>" + Environment.NewLine;
                //добавим тег в текстбокс
                txtBoxActionContent.Text += Environment.NewLine + value;
            }

            //условие для добавления в тектсбокс нового тега(любого, кроме картинки и гиперссылки на файл)
            if (!adder) { txtBoxActionContent.Text += Environment.NewLine + value + "|"; }
        }

        /// <summary>событие нажатия на кнопку - "Посмотреть результат"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnLookResult2_Command(object sender, CommandEventArgs e)
        {
            //получим список тегированных строк из содержимого тексбокса
            string txtAll = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageContent")).Text;

            //преобразуем тегированную строку в тегированный список
            var pageWorks = new PagesWorkClass(_pag);
            var tagList = pageWorks.GetTagListFromTagString(txtAll);

            //покажем результат отображения страницы
            var pageForm = new PagesForm(_pag);
            _pag.Master.FindControl("divWarnWrapper").Visible = false;
            _pag.Master.FindControl("divWindowWrapper1").Visible = true;
            _pag.Master.FindControl("divWindowContent1").Controls.Clear();
            _pag.Master.FindControl("divWindowContent1").Controls.Add(pageForm.GetPage("", tagList, true));
        }

        /// <summary>событие нажатия на кнопку - "Сохранить". Сохранение всех изменений, сделанных со структурой страницы в нужный файл.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOnePageSave_Command(object sender, CommandEventArgs e)
        {
            _pag.Master.FindControl("divWindowWrapper1").Visible = false;

            var warning = new WarnClass();
            var pagesWorkClass = new PagesWorkClass(_pag);
            warning.HideWarning(_pag.Master);

            //перезапишем свойство в структуре текстовой страницы
            var txtBoxActionContent = (TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageContent");
            var pageWorks = new PagesWorkClass(_pag);
            ((PagesStruct)_pag.Session["PagesStruct"]).ListOfTagTxt = pageWorks.GetTagListFromTagString(txtBoxActionContent.Text);

            //перезапишем структуру страницы
            if (!pagesWorkClass.ReplacePageData((PagesStruct)_pag.Session["PagesStruct"], ((PagesStruct)_pag.Session["PagesStruct"]).FullPathToStructFile))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения структуры страницы. Попробуйте повторить.", _pag.Master); }
            //else { warning.ShowWarning("УСПЕХ. Данные успешно сохранены.", _pag.Master); }

            //перезапишем данные по странице (заголовок, значения метатегов) в файл данных всех страниц
            ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Title = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxTitle")).Text;
            ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Description = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxDescription")).Text;
            ((PagesNameStruct)_pag.Session["PagesNameStruct"]).Keywords = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxKeywords")).Text;
            if (!pagesWorkClass.ReplacePageStructFile(((PagesNameStruct)_pag.Session["PagesNameStruct"])))
            { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения данных страницы. Попробуйте повторить.", _pag.Master); }
        }

        #endregion

        /// <summary>функция возвращает таблицу с редактором контактных данных в шапке, чердаке или т.п.</summary>
        /// <returns></returns>
        public Panel GetMailTelEditPanel()
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panWrap";

            var pageWork = new PagesWorkClass(_pag);
            var mailTelStruct = pageWork.GetTelMail();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР КОНТАКТНЫХ ДАННЫХ"; panelWrapper.Controls.Add(lbl);

            //ТЕЛЕФОН
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Телефон"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxTel"; txtBox.Width = 400;
            txtBox.Text = "";
            foreach (var tel in mailTelStruct.Telephone)
            {
                if (txtBox.Text == "") { txtBox.Text = tel; } else { txtBox.Text += ", " + tel; }
            }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ПОЧТОВЫЙ ЯЩИК
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Почта"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxMail"; txtBox.Width = 400;
            txtBox.Text = "";
            foreach (var mail in mailTelStruct.Email)
            {
                if (txtBox.Text == "") { txtBox.Text = mail; } else { txtBox.Text += ", " + mail; }
            }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопкой СОХРАНИТЬ
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //СОХРАНИТЬ
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения контактных данных.";
            lBtn.Command += (lBtnSaveMailTel_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetMailBoxEditPanel()

        /// <summary>событие нажатия на кнопку "сохранить"</summary>
        /// <param name="sender"></param>
        protected void lBtnSaveMailTel_Command(object sender, CommandEventArgs e)
        {
            var pageWork = new PagesWorkClass(_pag);
            var mailTelStruct = new MailTelStruct();

            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            string[] strSplit = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxTel")).Text.Trim().Split(new[] { ',' });
            mailTelStruct.Telephone = new List<string>();
            foreach (var tel in strSplit)
            {
                mailTelStruct.Telephone.Add(tel);
            }
            strSplit = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxMail")).Text.Replace(" ", "").Trim().Split(new[] { ',' });
            mailTelStruct.Email = new List<string>();
            foreach (var mail in strSplit)
            {
                mailTelStruct.Email.Add(mail);
            }

            //сохраним новую структуру в файл
            if (!pageWork.ReplaceMailTelData(mailTelStruct)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения контактных данных. Попробуйте повторить.", _pag.Master); }
        }

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>класс предназначен для работы с данными, касающимися текстовых страниц, новостного блока(часть функционала уже есть в файле класса NewsBlockClass)
    /// Так же этот класс работает с данными, касающимися слайд-шоу на главной (/files/slider/slider)</summary>
    public class PagesWorkClass
    {
        private Page _pag;
        //private int _listIdToInsert;            //в эту переменную записывается индекс исключённого из списка элемента(исключается в функции GetAllNewsStructs)
        public string ActionId { get; set; }    //сюда записывается значение ID новой акции при вызове функции
        public List<PagesNameStruct> _namesAndPathesToTxtFiles; //переменная содержит названия и пути ко всем файлам структур текстовых страниц сайта. Инициализируется в конструкторе этого класса

        string dirpath = HttpContext.Current.Server.MapPath("~") + @"files\pages\";

        public PagesWorkClass(Page pagenew)
        {
            _pag = pagenew;
            //получение списка с условными именами и полными путями к файлам структур текстовых страниц сайта.
            _namesAndPathesToTxtFiles = GetAllPagesNameStructs();
        }

        #region Функции и методы, касающиеся работы с текстовыми страницами

        /// <summary>функция возвращает структуру одной текстовой страницы, содержащейся в файле /files/pages/ФАЙЛ СТРАНИЦЫ. Возвращает null, если файл пустой.</summary>
        /// <param name="listValue">строка вида - Главная|default</param>
        /// <returns></returns>
        public PagesStruct GetPageStruct(string listValue)
        {
            var resultStruct = new PagesStruct();

            #region КОД

            string[] strSplit = listValue.Split(new[] { '|' });
            resultStruct.FullPathToStructFile = dirpath + strSplit[1]; resultStruct.NameForSite = listValue;

            string[] str;
            var listOfTags = new List<string>();
            if (File.Exists(resultStruct.FullPathToStructFile))
            {
                str = File.ReadAllLines(resultStruct.FullPathToStructFile, Encoding.Default);
                foreach (string oneline in str)
                {
                    if (oneline.Contains("|")) { listOfTags.Add(oneline); }
                }
            }
            else
            {
                //создадим новый файл для структуры текстовой страницы
                try { File.WriteAllLines(resultStruct.FullPathToStructFile, new List<string>(), Encoding.Default); }
                catch { }
            }

            resultStruct.ListOfTagTxt = listOfTags;

            #endregion

            return resultStruct;
        }

        /// <summary>функция заменяет данные по одной текстовой странице. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных новости, которую нужно заменить или удалить в файле</param>
        /// <param name="pathToFile">полный путь к файлу структуры текстовой страницы сайта</param>
        /// <returns></returns>
        public bool ReplacePageData(PagesStruct Struct, string pathToFile)
        {
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\page";

            #region КОД ЗАМЕНЫ ДАННЫХ ОДНОЙ ТЕКСТОВОЙ СТРАНИЦЫ

            //получим список тегированных строк для записи в файл текстовой страницы
            var listForDbFile = Struct.GetListFromStruct();

            //перезапишем нужный файл структуры текстовой страницы

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try { fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return false; }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try { File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default); }
            catch { return false; }
            try
            {
                try { fs.Close(); fs.Dispose(); }
                catch { }
                File.Copy(pathtotemp + tempFileName, pathToFile, true);
            }
            catch { return false; }
            try { File.Delete(pathtotemp + tempFileName); }
            catch { }

            #endregion

            return true;
        }

        /// <summary>процедура удаляет лишние фотографии текстовой страницы, которые по разным причинам могут скапливаться в папке files/pages/ПАПКА СТАРНИЦЫ/images.
        /// Так же очищается содержимое папки files\temp</summary>
        public void DeleteOldPageImages()
        {
            //получим пути ко всем существующим файлам в папке files/pages/images и в папке files/pages/files
            string[] pathToExistImgFiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\pages\images", "*", SearchOption.TopDirectoryOnly);
            string[] pathToExistImgFiles1 = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\pages\files", "*", SearchOption.TopDirectoryOnly);
            //из массива путей получим список имён существующих файлов
            var listOfExistImgFileNames = new List<string>();
            foreach (string onePath in pathToExistImgFiles)
            {
                listOfExistImgFileNames.Add(Path.GetFileName(onePath));
            }
            foreach (string onePath in pathToExistImgFiles1)
            {
                listOfExistImgFileNames.Add(Path.GetFileName(onePath));
            }
            if (listOfExistImgFileNames.Count == 0) return;

            //получим список всех тегированный строк всех файлов текстовых страниц, на которые есть ссылки в списке _pathesToTxtPageFiles
            var listOfAllTxtFilesTag = new List<string>();
            string[] str;
            List<string> listOfTags;
            foreach (PagesNameStruct oneStruct in _namesAndPathesToTxtFiles)
            {
                listOfTags = new List<string>();
                if (File.Exists(oneStruct.FileName))
                {
                    str = File.ReadAllLines(oneStruct.FileName, Encoding.Default);
                    foreach (string oneline in str)
                    {
                        if (oneline.Contains("|")) { listOfTags.Add(oneline); }
                    }
                }
                else
                {
                    //создадим новый файл для структуры текстовой страницы
                    try { File.WriteAllLines(oneStruct.FileName, new List<string>(), Encoding.Default); }
                    catch { }
                }

                listOfAllTxtFilesTag.AddRange(listOfTags);
            }

            var listOfNeedFileNames = new List<string>();
            string[] strSplit;
            foreach (string oneLine in listOfAllTxtFilesTag)
            {
                if (oneLine.Contains("картинка|"))
                {
                    strSplit = oneLine.Split(new[] { '|' });
                    listOfNeedFileNames.Add(Path.GetFileName(strSplit[5].Replace("../", "~/")));
                }
                if (oneLine.Contains("гиперссылкафайл|"))
                {
                    strSplit = oneLine.Split(new[] { '|' });
                    listOfNeedFileNames.Add(Path.GetFileName(strSplit[5].Replace("../", "~/")));
                }
            }

            //удалим те файлы картинок, на которые нет ссылок в структурах данных текстовых страниц
            if (listOfNeedFileNames.Count == 0)  //если список имён нужных файлов пуст, то удаляем все файлы, которые есть в списке существующих файлов, потому что они явно лишние 
            {
                foreach (string name in listOfExistImgFileNames)
                {
                    try { File.Delete(_pag.Server.MapPath("~") + @"files\pages\images\" + name); }
                    catch { }
                    try { File.Delete(_pag.Server.MapPath("~") + @"files\pages\files\" + name); }
                    catch { }
                }
            }
            else
            {
                var listOfFilePathesToDelete = new List<string>();
                bool checker = false;
                string dirPath = _pag.Server.MapPath("~") + @"files\pages\images\";
                string dirPath1 = _pag.Server.MapPath("~") + @"files\pages\files\";
                foreach (string existFileName in listOfExistImgFileNames)    //перебираем имена существующих в папке файлов картинок
                {
                    checker = false;
                    foreach (string needFileName in listOfNeedFileNames)     //перебираем имена нужных файлов(на которые есть ссылки в структурах данных текстовых страниц)
                    {
                        if (existFileName == needFileName) { checker = true; }
                    }
                    if (!checker)
                    {
                        if (existFileName.Contains("__"))   //если имя файла принадлежит гиперссылке на файл (только путь к таким файлам содержит два символа '_')
                        { listOfFilePathesToDelete.Add(dirPath1 + existFileName); }
                        else
                        { listOfFilePathesToDelete.Add(dirPath + existFileName); }
                    }
                }
                foreach (string pathToDelFile in listOfFilePathesToDelete)
                {
                    try { File.Delete(pathToDelFile); }
                    catch { }
                }
            }

            #region Очистка папки files\temp

            string[] pathToTempFiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\temp", "*", SearchOption.TopDirectoryOnly);
            foreach (string pathToFile in pathToTempFiles)
            {
                try
                {
                    File.Delete(pathToFile);
                }
                catch { }
            }

            #endregion
        }

        /// <summary>Функция возвращает объект с контактными данными.
        /// Данные берутся из файла files\pages\contactMaster</summary>
        /// <returns></returns>
        public MailTelStruct GetTelMail()
        {
            var mailTelStruct = new MailTelStruct();
            mailTelStruct.Email = new List<string>(); mailTelStruct.Telephone = new List<string>();

            string path = _pag.Server.MapPath("~") + @"files\pages\contactMaster";
            string[] str, strSplit;
            if (File.Exists(path))
            {
                str = File.ReadAllLines(path, Encoding.Default);
                foreach (string oneline in str)
                {
                    if (oneline.Contains("|"))
                    {
                        strSplit = oneline.Split(new[] { '|' });
                        if (strSplit[0] == "telephone") mailTelStruct.Telephone.Add(strSplit[1]);
                        if (strSplit[0] == "email") mailTelStruct.Email.Add(strSplit[1]);
                    }
                }
            }

            return mailTelStruct;
        }

        /// <summary>функция заменяет контактные данные в файле данных. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных контактов, которую нужно заменить или удалить в файле</param>
        /// <returns></returns>
        public bool ReplaceMailTelData(MailTelStruct Struct)
        {
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\mailtel";
            string pathToFile = _pag.Server.MapPath("~") + @"files\pages\contactMaster";

            #region КОД ЗАМЕНЫ ДАННЫХ ОДНОЙ ТЕКСТОВОЙ СТРАНИЦЫ

            //получим список тегированных строк для записи в файл текстовой страницы
            var listForDbFile = Struct.GetListFromStruct();

            //перезапишем нужный файл структуры текстовой страницы

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try { fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return false; }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try { File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default); }
            catch { return false; }
            try
            {
                try { fs.Close(); fs.Dispose(); }
                catch { }
                File.Copy(pathtotemp + tempFileName, pathToFile, true);
            }
            catch { return false; }
            try { File.Delete(pathtotemp + tempFileName); }
            catch { }

            #endregion

            return true;
        }

        /// <summary>Функция возвращает структуры с названиями всех существующих текстовых страниц. 
        /// Если файла с названиями ещё нет, то она создаёт его(пустым) и возвращает пустой список.</summary>
        /// <returns></returns>
        private List<PagesNameStruct> GetAllPagesNameStructs()
        {
            var structList = new List<PagesNameStruct>();
            var oneStruct = new PagesNameStruct();

            string path = _pag.Server.MapPath("~") + @"files\pages\pagenames";
            if (File.Exists(path))
            {
                string[] str = File.ReadAllLines(path, Encoding.Default);
                string[] strSplit;
                foreach (string oneline in str)
                {
                    if (oneline.Contains("|"))
                    {
                        strSplit = oneline.Split(new[] { '|' });
                        switch (strSplit[0])
                        {
                            case "start":
                                oneStruct = new PagesNameStruct();
                                break;
                            case "end":
                                structList.Add(oneStruct);
                                break;
                            case "pagename":
                                oneStruct.Name = strSplit[1];
                                break;
                            case "filename":
                                oneStruct.FileName = dirpath + strSplit[1];
                                break;
                            case "title":
                                oneStruct.Title = strSplit[1];
                                break;
                            case "description":
                                oneStruct.Description = strSplit[1];
                                break;
                            case "keywords":
                                oneStruct.Keywords = strSplit[1];
                                break;
                        }
                    }
                }
                //отсортируем полученный список по свойству Name структуры данных
                structList.Sort((a, b) => a.Name.CompareTo(b.Name));  //сортировка "от А до Я" по свойству Name объекта
            }
            else
            {
                try { File.WriteAllText(path, ""); }        //создаём новый пустой файл
                catch (Exception) { }
            }


            return structList;
        }

        /// <summary>Функция возвращает структуру с названиями для файла текстовой страницы по названию страницы или(!) имени файла страницы.
        /// В любом другом случае возвращает null.</summary>
        /// <param name="name">название страницы</param>
        /// <param name="filename">имя файла со структурой страницы</param>
        /// <returns></returns>
        public PagesNameStruct GetPagesNameStruct(string name, string filename)
        {
            var oneStruct = new PagesNameStruct();
            oneStruct.Name = ""; oneStruct.FileName = "";

            if (name.Trim() == "" && filename.Trim() == "") { return null; }
            if (name.Trim() != "" && filename.Trim() != "") { return null; }
            var allStruct = _namesAndPathesToTxtFiles;
            if (allStruct.Count == 0) { return null; }

            if (name.Trim() != "" && filename.Trim() == "")
            {
                foreach (PagesNameStruct oneStr in allStruct)
                {
                    if (name.Trim() == oneStr.Name) { oneStruct = oneStr; break; }
                }
            }
            if (name.Trim() == "" && filename.Trim() != "")
            {
                foreach (PagesNameStruct oneStr in allStruct)
                {
                    if (filename.Trim() == Path.GetFileName(oneStr.FileName)) { oneStruct = oneStr; break; }
                }
            }
            if (oneStruct.Name == "") { return null; }

            return oneStruct;
        }

        /// <summary>Функция добавляет файл данных новой страницы и записывает данные по странице в файл с данными по всем страницам.
        /// Возвращает true в случае успеха. False - в случае ошибки.</summary>
        /// <param name="Struct"></param>
        /// <returns></returns>
        public bool AddNewPageNameStruct(PagesNameStruct Struct)
        {
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\page";

            string path = _pag.Server.MapPath("~") + @"files\pages\pagenames";
            if (!File.Exists(path))
            {
                try { File.WriteAllText(path, ""); }        //создаём новый пустой файл
                catch (Exception) { return false; }
            }

            //получим список тегированных строк для записи в файл названий страниц
            var listForDbFile = Struct.GetListFromStruct();

            //Создадим файл для данных текстовой страницы
            try { File.WriteAllText(dirpath + Struct.FileName, ""); }        //создаём новый пустой файл
            catch (Exception) { return false; }

            //Допишем данные по новой странице в конец файла с названиями для всех текстовых страниц
            try     //пробуем записать информацию о посетителе сайта в файл журнала учёта посетителей
            {
                var sw = new StreamWriter(path, true, Encoding.Default);
                foreach (string line in listForDbFile)
                {
                    sw.WriteLine(line);      //записываем в конец файла строку
                }
                sw.Close();
                sw.Dispose();
            }
            catch { return false; }

            return true;
        }

        /// <summary>Функция удаляет файл текстовой страницы. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных контактов, которую нужно заменить или удалить в файле</param>
        /// <returns></returns>
        public bool DeletePageStructFile(PagesNameStruct Struct)
        {
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\pagenames";
            string pathToFile = _pag.Server.MapPath("~") + @"files\pages\pagenames";

            #region КОД ЗАМЕНЫ ДАННЫХ ОДНОЙ ТЕКСТОВОЙ СТРАНИЦЫ

            //получим список тегированных строк для записи в файл текстовой страницы
            var listForDbFile = new List<string>();
            var newStruct = new PagesNameStruct();
            string pathToDel = "";
            foreach (PagesNameStruct oneStruct in _namesAndPathesToTxtFiles)
            {
                if (oneStruct.Name != Struct.Name)
                {
                    newStruct = new PagesNameStruct();
                    newStruct.Name = oneStruct.Name;
                    newStruct.FileName = Path.GetFileName(oneStruct.FileName);
                    newStruct.Title = oneStruct.Title;
                    newStruct.Description = oneStruct.Description;
                    newStruct.Keywords = oneStruct.Keywords;
                    listForDbFile.AddRange(newStruct.GetListFromStruct());
                }
                else
                {
                    pathToDel = oneStruct.FileName;
                }
            }

            //перезапишем нужный файл структуры текстовой страницы
            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try
            {
                fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch
            {
                return false;
            }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try
            {
                File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default);
            }
            catch
            {
                return false;
            }
            try
            {
                try
                {
                    fs.Close();
                    fs.Dispose();
                }
                catch
                {
                }
                File.Copy(pathtotemp + tempFileName, pathToFile, true);
            }
            catch
            {
                return false;
            }
            try
            {
                File.Delete(pathtotemp + tempFileName);
            }
            catch { }

            //удаляем файл структуры данных текстовой страницы
            try
            {
                File.Delete(pathToDel);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message);
            }

            #endregion

            return true;
        }

        /// <summary>Функция заменяем данные в файле данных текстовых страниц. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных текстовой страницы, которую нужно заменить в файле</param>
        /// <returns></returns>
        public bool ReplacePageStructFile(PagesNameStruct Struct)
        {
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\pagenames";
            string pathToFile = _pag.Server.MapPath("~") + @"files\pages\pagenames";

            #region КОД ЗАМЕНЫ ДАННЫХ ОДНОЙ ТЕКСТОВОЙ СТРАНИЦЫ

            //получим список тегированных строк для записи в файл текстовой страницы
            var listForDbFile = new List<string>();
            var newStruct = new PagesNameStruct();
            foreach (PagesNameStruct oneStruct in _namesAndPathesToTxtFiles)
            {
                if (oneStruct.Name != Struct.Name)
                {
                    newStruct = new PagesNameStruct();
                    newStruct.Name = oneStruct.Name;
                    newStruct.FileName = Path.GetFileName(oneStruct.FileName);
                    newStruct.Title = oneStruct.Title;
                    newStruct.Description = oneStruct.Description;
                    newStruct.Keywords = oneStruct.Keywords;
                    listForDbFile.AddRange(newStruct.GetListFromStruct());
                }
                else
                {
                    newStruct = new PagesNameStruct();
                    newStruct.Name = Struct.Name;
                    newStruct.FileName = Path.GetFileName(Struct.FileName);
                    newStruct.Title = Struct.Title;
                    newStruct.Description = Struct.Description;
                    newStruct.Keywords = Struct.Keywords;
                    listForDbFile.AddRange(newStruct.GetListFromStruct());
                }
            }

            //перезапишем нужный файл структуры текстовой страницы
            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try { fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return false; }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try { File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default); }
            catch { return false; }
            try
            {
                try { fs.Close(); fs.Dispose(); }
                catch { }
                File.Copy(pathtotemp + tempFileName, pathToFile, true);
            }
            catch { return false; }
            try { File.Delete(pathtotemp + tempFileName); }
            catch { }

            #endregion

            return true;
        }

        /// <summary>Функция очищает файл данных текстовой страницы.
        /// Возвращает true в случае успеха. False - в случае ошибки.</summary>
        /// <param name="Struct"></param>
        /// <returns></returns>
        public bool CleanPageStructFile(PagesNameStruct Struct)
        {
            try { File.WriteAllText(Struct.FileName, ""); }        //создаём новый пустой файл
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>Метод возвращает размер файлов с данными по страницам (БД) в килобайтах</summary>
        /// <returns></returns>
        public long GetDbSize()
        {
            long result = 0;

            #region Основной код

            if (Directory.Exists(dirpath))
            {
                string[] arr = Directory.GetFiles(dirpath, "*", SearchOption.TopDirectoryOnly);

                FileInfo f;
                foreach (string path in arr)
                {
                    f = new FileInfo(path);
                    result += f.Length;
                }
                result = result / 1024;
            }

            #endregion

            return result;
        }

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к страницам</summary>
        /// <returns></returns>
        public long GetFoldersSize()
        {
            long result = 0;

            #region Основной код

            try
            {
                #region Основной код

                if (Directory.Exists(dirpath + "files") && Directory.Exists(dirpath + "images"))
                {
                    string[] arr1 = Directory.GetFiles(dirpath + "files", "*", SearchOption.TopDirectoryOnly);
                    string[] arr2 = Directory.GetFiles(dirpath + "images", "*", SearchOption.TopDirectoryOnly);
                    List<string> list = new List<string>();
                    list.AddRange(arr1); list.AddRange(arr2);

                    FileInfo f;
                    foreach (string path in list)
                    {
                        f = new FileInfo(path);
                        result += f.Length;
                    }
                    result = result / 1024;
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message);
                result = 0;

                #endregion
            }

            #endregion

            return result;
        }

        #endregion


        /// <summary>функция преобразования строки, содержащей теги(теги, разработанные под PageInfoBlockClass), в список строк с тегами. Каждая строка списка содержит по одному тегу(и конечно же его данным).</summary>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public List<string> GetTagListFromTagString(string tagString)
        {
            var tagList = new List<string>();

            #region СПИСОК ШАБЛОНОВ ДЛЯ ПОИСКА

            var tagTemplateList = new List<string>();

            tagTemplateList.Add("АБЗАЦН|");
            tagTemplateList.Add("АБЗАЦК|");
            tagTemplateList.Add("ТЕКСТ|");
            tagTemplateList.Add("ЛИТЕРАЛ|");
            tagTemplateList.Add("ОТСТУП|");
            tagTemplateList.Add("НАЧНУМСПИСКА|");
            tagTemplateList.Add("СТРНУМСПИСКАН|");
            tagTemplateList.Add("СТРНУМСПИСКАК|");
            tagTemplateList.Add("КОННУМСПИСКА|");
            tagTemplateList.Add("НАЧДЕФСПИСКА|");
            tagTemplateList.Add("СТРДЕФСПИСКАН|");
            tagTemplateList.Add("СТРДЕФСПИСКАК|");
            tagTemplateList.Add("КОНДЕФСПИСКА|");
            tagTemplateList.Add("таблначало|");
            tagTemplateList.Add("таблСтрокаН|");
            tagTemplateList.Add("таблСтрокаК|");
            tagTemplateList.Add("таблЯчейкаН|");        //у это тега 11 вертикальных черт (описание - таблЯчейкаН|<цвет фона>|<толщина бордюра>|<цвет бордюра>|<отступ сверху/снизу>|<отступ справа/слева>|<colspan>|<rowspan>|
                                                        //<выравнивание по горизонтали(л/п/с/ш)>|<выравнивание по вертикали(в/н/с)|<ширина ячейки>|<высота ячейки>)
            tagTemplateList.Add("таблЯчейкаК|");
            tagTemplateList.Add("таблконец|");
            tagTemplateList.Add("гиперссылка|");        //у этого тега две вертикальные черты - || (пример - гиперссылка|ЯНДЕКС|http://www.yandex.ru)
            tagTemplateList.Add("гиперссылкафайл|");    //у этого тега две вертикальные черты
            tagTemplateList.Add("карта|");
            tagTemplateList.Add("видеоизвне|");
            tagTemplateList.Add("картинка|");           //у этого тега 4 вертикальные черты (описание - картинка|400|on|on|../images/logo.jpg)
            tagTemplateList.Add("ПЕРЕНОС|");
            tagTemplateList.Add("раскрпанельшапкаНач|");
            tagTemplateList.Add("раскрпанельшапкаКон|");
            tagTemplateList.Add("раскрпанельсодержНач|");
            tagTemplateList.Add("раскрпанельсодержКон|");
            tagTemplateList.Add("БЛОКПАССН|");
            tagTemplateList.Add("БЛОКПАССК|");
            tagTemplateList.Add("БЛОКПОЯВЛН|");
            tagTemplateList.Add("БЛОКПОЯВЛК|");
            tagTemplateList.Add("слайдшоу|");
            tagTemplateList.Add("СПИСОКФОТО|");

            #endregion

            bool checker = true;
            int firstIndex = 0; int lastIndex = 0; int findIndex = 0;
            var indexList = new List<int>();
            string tempString, tag = "";
            string[] tempStringSplit;
            List<string> tempStringList;
            do
            {
                indexList = new List<int>();
                foreach (string line in tagTemplateList)
                {
                    findIndex = tagString.IndexOf(line);
                    if (findIndex != -1) { indexList.Add(findIndex); }  //если тег найден в строке, то вносим в список индекс первого его найденного положения в строке
                }
                indexList.Sort();

                if (indexList.Count > 1)        //если нужно выбрать очередную часть строки txtAll для добавления в список tagList
                {
                    firstIndex = indexList[0]; lastIndex = indexList[1];

                    #region алгоритм анализа(и разделения) строки tagString между firstIndex и lastIndex, там могут быть повторяющиеся теги (если в tagString подряд идут 2 или более повторяющихся тега, то предыдущая часть алгоритма соединяет их в одну строку)

                    //данных алгоритм разделяет строку на несколько, если в строка состоит из нескольких одинаковых тегов
                    tempString = tagString.Substring(firstIndex, lastIndex - firstIndex);
                    foreach (string line in tagTemplateList)                    //определяем значение тега, который есть в этой строке и сохраняем его в переменную tag
                    {
                        if (tempString.Contains(line)) { tag = line; break; }
                    }
                    tempStringSplit = tempString.Split(new[] { tag }, StringSplitOptions.None);     //разделим строку по значению тега
                    tempStringList = new List<string>();
                    for (int i = 0; i < tempStringSplit.Length; i++)                                //получим список одиночных тегов с их значениями
                    {
                        if (i > 0)
                        {
                            tempStringList.Add(tag + tempStringSplit[i].Trim());
                        }
                    }

                    #endregion

                    tagList.AddRange(tempStringList);                                           //добавляем в список очередной тег(и) со значением
                    tagString = tagString.Remove(firstIndex, lastIndex - firstIndex);           //удаляем из исходной строки этот тег вместе с его значением
                }
                else if (indexList.Count == 1)  //если осталось выбрать последнее значение
                {
                    firstIndex = indexList[0]; lastIndex = tagString.Length;

                    #region алгоритм анализа(и разделения) строки tagString между firstIndex и lastIndex, там могут быть повторяющиеся теги (если в tagString подряд идут 2 или более повторяющихся тега, то предыдущая часть алгоритма соединяет их в одну строку)

                    //данных алгоритм разделяет строку на несколько, если в строка состоит из нескольких одинаковых тегов
                    tempString = tagString.Substring(firstIndex, lastIndex - firstIndex);
                    foreach (string line in tagTemplateList)                    //определяем значение тега, который есть в этой строке и сохраняем его в переменную tag
                    {
                        if (tempString.Contains(line)) { tag = line; break; }
                    }
                    tempStringSplit = tempString.Split(new[] { tag }, StringSplitOptions.None);     //разделим строку по значению тега
                    tempStringList = new List<string>();
                    for (int i = 0; i < tempStringSplit.Length; i++)                                //получим список одиночных тегов с их значениями
                    {
                        if (i > 0)
                        {
                            tempStringList.Add(tag + tempStringSplit[i].Trim());
                        }
                    }

                    #endregion

                    tagList.AddRange(tempStringList);                                           //добавляем в список очередной тег со значением
                    tagString = "";                                                             //очищаем исходную строку, так как это уже был последний тег со значением
                }
                else if (indexList.Count == 0)  //если из строки txtAll выбрано всё
                {
                    checker = false;
                }

            } while (checker);

            return tagList;
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>класс описывающий структуру данных одной текстовой страницы</summary>
    [Serializable]
    public class PagesStruct
    {
        public List<string> ListOfTagTxt { get; set; }      //содержит список тегированных строк данных для страницы.
        public string FullPathToStructFile { get; set; }    //содержит полный путь к файлу структуры данных текстовой страницы
        public string NameForSite { get; set; }             //содержит краткое имя страницы, как на сайте

        /// <summary>функция возвращает список в формате List/string/ из этой структуры текстовой страницы. 
        /// Строки в списке полностью подготовлены для записи в файл текстовой страницы (все текстовые страницы находятся в папке /files/pages/)</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();

            list.AddRange(ListOfTagTxt);

            return list;
        }
    }

    /// <summary>класс описывающий структуру данных для контактной информации, которая нужна в шапке, подвале сайта и т.п.</summary>
    [Serializable]
    public class MailTelStruct
    {
        public List<string> Email { get; set; }           //содержит e-mail'ы
        public List<string> Telephone { get; set; }       //содержит телефоны

        /// <summary>функция возвращает список в формате List/string/ из этой структуры контактных данных. 
        /// Строки в списке полностью подготовлены для записи в файл данных (файл с данными - files\pages\contactMaster)</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();

            foreach (var tel in Telephone)
            {
                list.Add("telephone|" + tel);
            }
            foreach (var mail in Email)
            {
                list.Add("email|" + mail);
            }

            return list;
        }
    }

    /// <summary>класс описывающий структуру данных для файла с именами всех текстовых страниц (файл - files\pages\pagenames)</summary>
    [Serializable]
    public class PagesNameStruct
    {
        public string Name { get; set; }           //содержит условное наименование файла. Например - Главная
        public string FileName { get; set; }       //содержит реальное имя файла в папке files\pages\. Например - default
        public string Title { get; set; }          //содержит текст для тега <title></title>
        public string Description { get; set; }    //содержит текст для тега <meta name='description' content='' />
        public string Keywords { get; set; }       //содержит текст для тега <meta name='keywords' content='' />

        /// <summary>функция возвращает список в формате List/string/ из этой структуры данных. 
        /// Строки в списке полностью подготовлены для записи в файл данных (файл с данными - files\pages\pagenames)</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();
            list.Add("start|");
            list.Add("pagename|" + Name);
            list.Add("filename|" + FileName);
            list.Add("title|" + Title);
            list.Add("description|" + Description);
            list.Add("keywords|" + Keywords);
            list.Add("end|");
            return list;
        }
    }

    #endregion

}