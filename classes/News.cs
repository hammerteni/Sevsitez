// Файл с классами для работы с НОВОСТНЫМ БЛОКОМ. Файлы располагаются так:
// Материалы для новостей - 
// ~/files/news/; 
// ~/files/news/foto; 
// ~/files/news/files; 
// База данных - 
// ~/files/sqlitedb/news.db

#region Using

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using site.classesHelp;
using Image = System.Web.UI.WebControls.Image;

#endregion

namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>Класс содержит функции и методы формирования готового HTML-кода для просмотра и редактирования новостей.
    /// CSS находится в файле - news.css</summary>
    public class NewsForm
    {
        #region Поля класса

        private HttpContext _context;
        private Page _pag;
        private int _countOfElemInOnePage;

        private string _pathToImgFolder;
        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        private string _imgUrlPathFoto;
        private string _imgUrlPathFile;

        #endregion

        #region Конструктор класса

        //КОНСТРУКТОР КЛАССА
        public NewsForm(Page pagnew, int countOfElemInOnePagenew)
        {
            _pag = pagnew;
            _countOfElemInOnePage = countOfElemInOnePagenew;

            _context = HttpContext.Current;
            _pathToImgFolder = _context.Server.MapPath("~") + @"files\news\foto\";
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\news\files\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _imgUrlPathFoto = "../../../files/news/foto/";
            _imgUrlPathFile = "../../../files/news/files/";
        }

        #endregion

        #region Для сайта

        /// <summary>Метод выводит панель с новостями.</summary>
        /// <returns></returns>
        public Panel GetNewsPanel()
        {
            Panel panelWrap = new Panel(); panelWrap.CssClass = "panelWrap_nf";

            #region Основной код

            NewsWork work = new NewsWork();
            List<News> list = work.GetSortedListOfNews();
            if (list == null) return panelWrap;
            else if (list.Count == 0) return panelWrap;
            list = list.Select(a => a).Where(a => a.Enabled).ToList();
            //Получение кол-ва выводимых новостей на одну страницу
            int newsCount = work.GetNewsCount();
            if (newsCount == -1) return panelWrap;

            #region Заголовок новостного блока и добавление панели с новостями

            panelWrap.Controls.Add(new LiteralControl("<h2 class='title_nf'>НАШИ НОВОСТИ</h2>"));
            Panel newsBlock = new Panel(); newsBlock.CssClass = "newsBlock_nf"; panelWrap.Controls.Add(newsBlock);

            #endregion

            #region Добавление самих блоков новостей

            int pageCounter = 1;
            int prodCounter = 0;
            PagePanelClass pageBtns = new PagePanelClass(list.Count, newsCount);

            foreach (News obj in list)
            {
                prodCounter++;
                if (prodCounter > newsCount)   //выводим на страницу не более нужного кол-ва событий
                {
                    prodCounter = 1;
                    pageCounter++;
                }

                if (pageCounter == (int)_pag.Session["pagenum"])           //если эта та самая страница, которую нужно вывести, то выводим её
                {
                    #region Код наполнения одной страницы со списком новостей

                    Panel oneNews = new Panel(); oneNews.CssClass = "oneNews_nf"; newsBlock.Controls.Add(oneNews);
                    oneNews.Controls.Add(new LiteralControl("<table class='tbl_nf'>"));
                    oneNews.Controls.Add(new LiteralControl("<tr>"));

                    //ячейка с картинкой новости
                    oneNews.Controls.Add(new LiteralControl("<td rowspan='3' class='tdImage_nf'><a class='aImg' href='/onenews.aspx?id=" +
                                                            obj.Id + "' target='_blank'><img src='" +
                                                            _imgUrlPathFoto + obj.LogoImgName + "' alt='" + obj.Title + "' title='" + obj.Title + "' /></a></td>"));
                    oneNews.Controls.Add(new LiteralControl("</td>"));

                    //ячейка с датой новости
                    DateTime dt = new DateTime(obj.DateReg);
                    oneNews.Controls.Add(new LiteralControl("<td class='tdDate_nf'>" + dt.ToShortDateString() + "</td>"));
                    oneNews.Controls.Add(new LiteralControl("</td>"));
                    oneNews.Controls.Add(new LiteralControl("</tr>"));
                    oneNews.Controls.Add(new LiteralControl("<tr>"));

                    //ячейка с заголовком новости
                    oneNews.Controls.Add(new LiteralControl("<td class='tdTitle_nf'><a href='/onenews.aspx?id=" + obj.Id + "' target='_blank'>" + obj.Title + "</a></td>"));
                    oneNews.Controls.Add(new LiteralControl("</tr>"));
                    oneNews.Controls.Add(new LiteralControl("<tr>"));

                    //ячейка с анонсом новости
                    string[] strSplit = obj.Anons.Split(new[] { '|' });
                    string anons = "";
                    int counter = 1;
                    foreach (string s in strSplit)
                    {
                        if (counter == strSplit.Length)
                            anons += s;
                        else
                            anons += s + "<br/>";
                        counter++;
                    }
                    oneNews.Controls.Add(new LiteralControl("<td class='tdAnons_nf'>" + anons + "</td>"));
                    oneNews.Controls.Add(new LiteralControl("</tr>"));
                    oneNews.Controls.Add(new LiteralControl("</table>"));

                    #endregion
                }
                else if (pageCounter > (int)_pag.Session["pagenum"])
                {
                    break;
                }
            }

            panelWrap.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

            #endregion

            #endregion

            return panelWrap;
        }

        /// <summary>Метод выводит панель с полной информацией по одной новости</summary>
        /// <param name="id">id новости</param>
        /// <returns></returns>
        public Panel GetOneNewsPanel(string id)
        {
            Panel panelWrap = new Panel(); panelWrap.CssClass = "panelWrap_one_nf";

            #region Основной код

            NewsWork work = new NewsWork();
            News obj = work.GetOneNews(id);
            if (obj == null) return panelWrap;

            // Заполняем title страницы
            _pag.Title = obj.Title;

            // Теперь строка с ЗАГОЛОВКОМ новости
            panelWrap.Controls.Add(new LiteralControl("<h2 class='titleOne_nf'>" + obj.Title + "</h2>"));

            // Теперь ДАТА ПУБЛИКАЦИИ новости
            DateTime dt = new DateTime(obj.DateReg);
            panelWrap.Controls.Add(new LiteralControl("<span class='spanPre_nf'>Дата публикации: </span>"));
            panelWrap.Controls.Add(new LiteralControl("<span class='spanDate_nf'>" + dt.ToString("dd MMMM yyyy") + "</span>"));

            // ОСНОВНОЕ СОДЕРЖАНИЕ НОВОСТИ
            panelWrap.Controls.Add(new LiteralControl("<div class='divTextWrap_nf'>"));
            panelWrap.Controls.Add(new LiteralControl(obj.Text));
            panelWrap.Controls.Add(new LiteralControl("</div>"));

            #endregion

            _pag.Header.Controls.Add(new LiteralControl("<noindex><style type='text/css'>" + obj.Param1 + "</style></noindex>"));
            _pag.Controls.Add(new LiteralControl("<noindex><script type='text/javascript'>" + obj.Param2 + "</script></noindex>"));

            return panelWrap;
        }

        #endregion

        #region Для консоли управления

        #region Метод GetNewsFilterPanel()
        /// <summary>Метод возвращает панель с фильтром и кнопками управления списком новостей</summary>
        /// <returns></returns>
        public Panel GetNewsFilterPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР НОВОСТНОЙ ЛЕНТЫ"; panelWrapper.Controls.Add(lbl);

            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = _pag.Session["srchStr"].ToString(); txtBox.ID = "txtBoxSrchEvent";
            txtBox.ToolTip = "Введите сюда текст, по которому нужно найти новость. Поиск осуществляется по всем текстовым полям новости.";
            panelWrapper.Controls.Add(txtBox);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Найти "; lBtn.ToolTip = "Кнопка поиска новостей. Поиск осуществляется по всем данным новостных блоков.";
            lBtn.Command += (lBtnSrch_Command); lBtn.ID = "btnEventSrch";
            panelWrapper.Controls.Add(lBtn);

            //Кнопка ДОБАВИТЬ новость
            panelWrapper.Controls.Add(new LiteralControl("<br />"));
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " + добавить новость "; lBtn.ToolTip = "Кнопка открытия формы добавления новости.";
            lBtn.Command += (lBtnAddNews_Command); panelWrapper.Controls.Add(lBtn);

            // Кнопка очистки ненужных файлов картинок и файлов в папках новостей, которые не относятся к новостям, содержащимся в БД
            // Кнопка изменения кол-ва выводимых на страницу блоков новостей
            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == "администратор")
            {
                /*lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " - очистить лишний файлы "; lBtn.ToolTip = "Кнопка очищает все файлы, которые не относятся к новостям, содержащимся в базе данных";
                lBtn.Command += (lBtnClear_Command); panelWrapper.Controls.Add(lBtn);*/

                panelWrapper.Controls.Add(new LiteralControl("<br />"));

                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Количество выводимых на страницу новостных блоков: "; panelWrapper.Controls.Add(lbl);

                txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 30; txtBox.TextMode = TextBoxMode.SingleLine;
                NewsWork work = new NewsWork();
                txtBox.Text = work.GetNewsCount().ToString(); txtBox.ID = "txtBoxNewsCount";
                txtBox.ToolTip = "Введите цифрой кол-во выводимых на страницу новостных блоков";
                panelWrapper.Controls.Add(txtBox);

                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " Сохранить "; lBtn.ToolTip = "Кнопка количество выводимых на страницу новостных блоков";
                lBtn.Command += (lBtnNewsCountSave_Command); panelWrapper.Controls.Add(lBtn);

                // Вывод размера файла БД и размера папок с файлами к новостям
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер БД: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetDbSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер папок с файлами: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetFoldersSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);
            }

            return panelWrapper;
        }

        #region События для функции GetNewsFilterPanel()

        /// <summary>событие нажатия на кнопку "найти" новость по поисковому запросу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSrch_Command(object sender, CommandEventArgs e)
        {
            try
            {
                _pag.Session["srchStr"] = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Text;
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>нажатие на кнопку ДОБАВИТЬ НОВОСТЬ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnAddNews_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["tempNewsStruct"] = null;
            _pag.Response.Redirect("~/adm/newsedit.aspx?id=");
        }

        /// <summary>нажатие на кнопку ОЧИСТИТЬ ЛИШНИЕ ФАЙЛЫ в папках новостей</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnClear_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            NewsWork work = new NewsWork();
            if (!work.DeleteUnnecessaryFiles())
            {
                warning.ShowWarning("ВНИМАНИЕ. Не удалось очистить все лишние файлы относящиеся к новостям. Попробуйте повторить.", _pag.Master);
                return;
            }
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>событие нажатия на кнопку "Сохранить" кол-ва выводимых на страницу новостных блоков</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnNewsCountSave_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            try
            {
                TextBox txtBox = (TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxNewsCount");
                string count = txtBox.Text;
                int countNew = StringToNum.ParseInt(count);
                if (countNew == -1)
                {
                    warning.ShowWarning("ВНИМАНИЕ. Вы неправильно ввели кол-во выводимых на одну страницу новостей", _pag.Master);
                    return;
                }
                NewsWork work = new NewsWork();
                if (work.UpdateNewsCount(count) == -1)
                {
                    warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить новое кол-во выводимых строк. Попробуйте повторить..", _pag.Master);
                    return;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #endregion

        #region Метод GetNewsListTbl()
        /// <summary>Метод возвращает таблицу со всеми новостями (возможно отфильтрованными по переменной Session["srchStr1"])</summary>
        /// <returns></returns>
        public Table GetNewsListTbl()
        {
            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell();
            Label lbl;

            #region Получение списка структур новостей

            NewsWork work = new NewsWork();
            List<News> list = work.GetSortedListOfNews((string)_pag.Session["srchStr"]);
            if (list == null) return tbl;

            #endregion

            int pageCounter = 1;
            int prodCounter = 0;

            //добавим на страницу кнопки-ссылки на страницы продуктов (вверху)
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            var pageBtns = new PagePanelClass(list.Count, _countOfElemInOnePage);
            tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

            //добавим надпись - всего товара разделе/подразделе
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего новостей: "; tblCell.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = list.Count.ToString(); tblCell.Controls.Add(lbl);

            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

            //добавляем подтаблицу
            var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

            //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ
            var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
            var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата время"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "№"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Заголовок"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Анонс"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Фото"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);       //в этом столбце будут кнопки ОПУБЛИКОВАТЬ    
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Публикация"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);       //в этом столбце будут кнопки РЕДАКТИРОВАТЬ    
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = ""; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);       //в этом столбце будут кнопки УДАЛИТЬ
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = ""; tblCell1.Controls.Add(lbl);

            //добавляем строки с данными по событиям
            foreach (News oneStruct in list)
            {
                prodCounter++;
                if (prodCounter > _countOfElemInOnePage)   //выводим на страницу не более нужного кол-ва событий
                {
                    prodCounter = 1;
                    pageCounter++;
                }

                if (pageCounter == (int)_pag.Session["pagenum"])           //если эта та самая страница, которую нужно вывести, то выводим её
                {
                    tblRow1 = new TableRow();

                    //ДАТА и ВРЕМЯ
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    var dt = new DateTime(oneStruct.DateReg);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = dt.ToShortDateString() + " " + dt.ToShortTimeString(); tblCell1.Controls.Add(lbl);

                    //ID
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Id.ToString(); tblCell1.Controls.Add(lbl);

                    //ЗАГЛАВИЕ
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Title; tblCell1.Controls.Add(lbl);

                    //АНОНС
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    bool first = true;
                    string[] strSplit = oneStruct.Anons.Split(new[] { '|' });
                    foreach (string line in strSplit)
                    {
                        #region Тело цикла

                        if (first)
                        {
                            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = line; tblCell1.Controls.Add(lbl);
                            first = false;
                        }
                        else
                        {
                            tblCell1.Controls.Add(new LiteralControl("<br />"));
                            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = line; tblCell1.Controls.Add(lbl);
                        }

                        #endregion
                    }

                    //ФОТО для анонса
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    var img = new Image(); img.Width = 50;
                    if (oneStruct.LogoImgName.Trim() != "") { img.ImageUrl = _imgUrlPathFoto + oneStruct.LogoImgName; } else { img.ImageUrl = "../../../images/nullnewsfoto.png"; }
                    tblCell1.Controls.Add(img);

                    //Кнопка ПУБЛИКАЦИИ новости на сайте  
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    var lBtn = new LinkButton(); lBtn.CssClass = "hlinkHover lBtnHover lBtnsUniverse";
                    string check = oneStruct.Enabled ? "1" : "0";
                    lBtn.CommandArgument = oneStruct.Id + "|" + check; lBtn.Command += new CommandEventHandler(lBtnPublic_Command);
                    if (check == "0")            //если отзыв не опубликован, то..
                    {
                        lBtn.Text = "опубликовать";
                    }
                    else if (check == "1")   //если опубликован, то..
                    {
                        lBtn.Text = "скрыть";
                    }
                    lBtn.ToolTip = "публикация новости на сайта или снятие с публикации";
                    tblCell1.Controls.Add(lBtn);


                    //Кнопка РЕДАКТИРОВАТЬ  
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lBtn = new LinkButton(); lBtn.CommandArgument = oneStruct.Id.ToString(); lBtn.Command += new CommandEventHandler(lBtnEdit_Command);
                    lBtn.ToolTip = "редактировать новость";
                    img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/edit.png";
                    lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);

                    //Кнопка УДАЛИТЬ
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lBtn = new LinkButton(); lBtn.CommandArgument = oneStruct.Id.ToString(); lBtn.Command += new CommandEventHandler(lBtnDelete_Command);
                    lBtn.ToolTip = "удалить новость";
                    lBtn.OnClientClick = "return confirm('Новость будет удалена из базы данных. Удалить?');";
                    img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/krestik.png";
                    lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);

                    tbl1.Controls.Add(tblRow1);
                }
                else if (pageCounter > (int)_pag.Session["pagenum"])
                {
                    break;
                }
            }

            //добавим на страницу кнопки-ссылки на страницы продуктов (внизу)
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("1"));

            return tbl;
        }

        #region События для функции GetNewsListTbl()

        /// <summary>кнопка публикации новости на сайте или снятия с публикации</summary>
        /// <param name="sender"></param>
        /// <param name="e">id | enabled</param>
        protected void lBtnPublic_Command(object sender, CommandEventArgs e)
        {
            try
            {
                #region Основной код

                string[] strSplit = e.CommandArgument.ToString().Split('|');
                LinkButton lBtn = (LinkButton)sender;

                WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
                NewsWork work = new NewsWork();

                int enabled = int.Parse(strSplit[1]);
                if (enabled == 1) enabled = 0; else if (enabled == 0) enabled = 1;  //заменим значение на противоположное с точки зрения логики, так как нам нужно изменить состояние
                int res = work.UpdateNewsEnabled(long.Parse(strSplit[0]), enabled);
                if (res == -1 || res == 0)
                {
                    warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить изменения в БД. Попробуйте повторить..", _pag.Master); return;
                }
                if (strSplit[1] == "0") lBtn.Text = "скрыть";
                else if (strSplit[1] == "1") lBtn.Text = "опубликовать";

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        /// <summary>нажатие на кнопку РЕДАКТИРОВАТЬ новость</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит Id новости</param>
        private void lBtnEdit_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["tempNewsStruct"] = null;
            _pag.Response.Redirect("~/adm/newsedit.aspx?id=" + e.CommandArgument);
        }

        /// <summary>нажатие на кнопку УДАЛИТЬ новость</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит Id новости</param>
        private void lBtnDelete_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            NewsWork newsWork = new NewsWork();
            int res = newsWork.DeleteOneNews(e.CommandArgument.ToString());
            if (res == 0 || res == -1)
            {
                warning.ShowWarning("ВНИМАНИЕ. Не получилось удалить новость из БД. Попробуйте повторить..", _pag.Master);
                return;
            }

            DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Удалена новость с id - " + e.CommandArgument.ToString() + 
                         ". Удалил - " + ((AdmPersonStruct)_pag.Session["authperson"]).Name);

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #endregion

        #region Метод GetOneNewsEditPanel(string newsId)
        /// <summary>Метод возвращает панель для редактирования или добавления одной новости</summary>
        /// <param name="newsId">ID новости</param>
        /// <returns></returns>
        public Panel GetOneNewsEditPanel(string newsId)
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panWrap";

            #region Кнопка НАЗАД

            panelWrapper.Controls.Add(new LiteralControl("<a class='lBtnBack' href='/adm/news.aspx'></a>"));

            #endregion
            #region Заголовок

            panelWrapper.Controls.Add(new LiteralControl("<span class='lblSectionTitle'>РЕДАКТОР ОДНОЙ НОВОСТИ</span>"));

            #endregion
            
            #region Код заполнения необходимых для работы с новостью данных

            if (_pag.Session["tempNewsStruct"] == null)
            {
                NewsWork work = new NewsWork();
                if (newsId == "")       //если нужно создать новость, то..
                {
                    _pag.Session["tempNewsStruct"] = new News();
                    ((News)_pag.Session["tempNewsStruct"]).IsNew = true;
                    ((News)_pag.Session["tempNewsStruct"]).Id = work.GetNextNewsId();
                }
                else                    //если нужно редактировать новость, то..
                {
                    _pag.Session["tempNewsStruct"] = work.GetOneNews(newsId);
                    if (_pag.Session["tempNewsStruct"] == null)
                    {
                        panelWrapper.Controls.Add(new LiteralControl("<br/> <span class='spanNull_news'>Ошибка при получении данных о новости..</span>"));
                        return panelWrapper;
                    }
                    if (((News)_pag.Session["tempNewsStruct"]).Title == "")
                    {
                        panelWrapper.Controls.Add(new LiteralControl("<br/> <span class='spanNull_news'>Новости с таким ID не существует в базе данных..</span>"));
                        return panelWrapper;
                    }
                }
            }

            //для удобства восприятия кода дальше по процедуре, запишем значение сессионный переменной в более читабельную переменную
            News obj = (News)_pag.Session["tempNewsStruct"];

            #endregion

            #region Код формирования HTML кода формы создания или редактирования новости

            //Дата и время новости
            DateTime dt = new DateTime(obj.DateReg);
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            var lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Дата и время регистрации новости:"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine; txtBox.ReadOnly = true;
            txtBox.Text = dt.ToShortDateString() + " " + dt.ToShortTimeString();
            txtBox.ToolTip = "Дата и время новости присваивается автоматически при нажатии на кнопку СОХРАНИТЬ";
            txtBox.Width = 150; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ID
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "ID новости:"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine; txtBox.ReadOnly = true;
            txtBox.ToolTip = "ID новости присваивается автоматически";
            txtBox.Width = 50; txtBox.Text = obj.Id.ToString(); panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ЗАГОЛОВОК
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Заголовок(название) новости:"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine; txtBox.ID = "txtBoxTitle";
            txtBox.Width = 900; txtBox.Text = obj.Title; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ФОТО К НОВОСТИ (только одно)
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Width = 1100; lbl.Text = "Добавление фото в анонс новости:"; panelLine.Controls.Add(lbl);

            panelLine.Controls.Add(new LiteralControl("<br/>"));

            // Кнопка открытия файлового менеджера (все стили и Javascript находятся в папке filemanager)
            // В функцию fmOpen передаются аргументы: 1-й - объект нажатой кнопки, 2-й - относительный путь к папке файлового менеджера, 3-й - относительный путь к папке с файлами
            panelLine.Controls.Add(new LiteralControl("<span id='btnFm' onclick=\"fmStart(this, '/Scripts/', '/files/news/foto/');\"></span>"));

            var fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fileFotoUpload"; panelLine.Controls.Add(fUpload);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " + добавить фото "; lBtn.ToolTip = "Добавление фото к новости. (только одно, формат JPG, JPEG или PNG, размер - не более 100 Кб, рекомендуемая ширина - 100 пикселей)";
            lBtn.CommandArgument = obj.Id.ToString();
            lBtn.Command += (lBtnAddFoto_Command);
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //фотография и кнопка удалить
            var img = new Image();
            if (obj.LogoImgName != "")
            {
                panelLine = new Panel(); panelLine.CssClass = "panelLine";
                //фото
                img = new Image(); img.ImageUrl = _imgUrlPathFoto + obj.LogoImgName;
                img.Width = 100; panelLine.Controls.Add(img);
                //кнопка УДАЛИТЬ
                lBtn = new LinkButton(); lBtn.CommandArgument = obj.LogoImgName;
                lBtn.Attributes.Add("style", "vertical-align:top;");
                lBtn.Command += new CommandEventHandler(lBtnDelFoto_Command);
                lBtn.ToolTip = "удалить фото";
                img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/krestik.png";
                lBtn.Controls.Add(img); panelLine.Controls.Add(lBtn);
                panelWrapper.Controls.Add(panelLine);
            }

            //АНОНС
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Краткое описание новости:"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine; txtBox.ID = "txtBoxAnons";
            txtBox.Width = 900; txtBox.Height = 50;
            bool first = true;
            string[] strSplit = obj.Anons.Split(new[] { '|' });
            foreach (string line in strSplit)
            {
                if (first) { txtBox.Text += line; } else { txtBox.Text += Environment.NewLine + line; }
                first = false;
            }
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //ПОЛНЫЙ ТЕКСТ НОВОСТИ
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Полный текст новости:"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);  //отступ
            panelLine.Controls.Add(new LiteralControl("<br /><br />"));

            // Абзац пояснений по добавлению ссылок файлов для текстового редактора
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp";
            lbl.Text = "Кнопка ДОБАВИТЬ ниже служит для того, чтобы можно было загрузить файл изображения или иной файл на сайт, получить ссылку на него (появится справа от кнопки). Эту ссылку можно ипользовать в текстовом редакторе, расположенном ниже.";
            panelLine.Controls.Add(lbl);
            panelLine.Controls.Add(new LiteralControl("<br /><br />"));

            // Кнопка открытия файлового менеджера (все стили и Javascript находятся в папке filemanager)
            // В функцию fmOpen передаются аргументы: 1-й - объект нажатой кнопки, 2-й - относительный путь к папке файлового менеджера, 3-й - относительный путь к папке с файлами
            panelLine.Controls.Add(new LiteralControl("<span id='btnFm' onclick=\"fmStart(this, '/Scripts/', '/files/news/files/');\"></span>"));

            // Элемент загрузки файла с диска
            fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fileUpload";
            fUpload.ToolTip = "Загружайте любые файлы размером не более 2-х мегабайт."; panelLine.Controls.Add(fUpload);

            // Чекбокс НАЛОЖЕНИЯ КАРТИНКИ
            var chkBox = new CheckBox(); chkBox.ToolTip = "поставьте здесь галочку, если нужно наложить защитное изображение на добавляемую картинку (только для JPG, JPEG и PNG)";
            chkBox.ID = "chkBoxOverlayImgAdd"; panelLine.Controls.Add(chkBox);

            // Кнопка ДОБАВИТЬ файл
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "добавить";
            lBtn.CommandArgument = obj.Id.ToString();
            lBtn.Command += (lBtnAddFile_Command);
            lBtn.ToolTip = "Загружайте любые файлы размером не более 10-ти мегабайт.";
            panelLine.Controls.Add(lBtn);

            // Элемент отображения полной ссылки на добавленный файл
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.ID = "hLinkToFile";
            lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;" + obj.HLinkToFile;
            panelLine.Controls.Add(lbl);

            panelLine.Controls.Add(new LiteralControl("<br /><br />"));

            // Текстбокс редактора
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine; txtBox.ID = "txtBoxText";
            txtBox.Text = obj.Text;
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            // Ссылка на сайт разработчиков текстового редактора
            panelWrapper.Controls.Add(new LiteralControl("<span class='lblFormPre'>Ссылка на сайт текстового редактора CKEditor: </span><a style='cursor:pointer;' href='http://ckeditor.com/' target='_blank'>перейти</a>"));

            // Добавление скрипта для редактора
            panelWrapper.Controls.Add(new LiteralControl("<script type='text/javascript'>CKEDITOR.replace(document.getElementById('txtBoxText')); " +
                                                         "CKEDITOR.config.height=400;" +
                                                         "CKEDITOR.config.language='ru';" +
                                                         "</script> "));

            #region Редактор CSS

            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Font.Bold = true;
            lbl.Text = "<br/><br/>Добавление таблиц стилей (CSS) для страницы новости:<br/>";
            panelWrapper.Controls.Add(lbl);

            // Текстбокс редактора CSS
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine;
            txtBox.Width = 1190; txtBox.Height = 150; txtBox.ID = "txtBoxCss";
            txtBox.Text = obj.Param1;
            panelWrapper.Controls.Add(txtBox);

            #endregion

            #region Редактор JavaScript

            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Font.Bold = true;
            lbl.Text = "<br/><br/>Добавление JavaScript для страницы новости:<br/>";
            panelWrapper.Controls.Add(lbl);

            // Текстбокс редактора CSS
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine;
            txtBox.Width = 1190; txtBox.Height = 150; txtBox.ID = "txtBoxJavascript";
            txtBox.Text = obj.Param2;
            panelWrapper.Controls.Add(txtBox);

            #endregion

            //Кнопки СКОПИРОВАТЬ и СОХРАНИТЬ
            panelLine = new Panel(); panelLine.CssClass = "panelLine";

            //СКОПИРОВАТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СКОПИРОВАТЬ НОВОСТЬ ";
            lBtn.ToolTip = "Копируется всё содержимое новости, кроме даты, фото для анонса и ID для создания новой новости";
            lBtn.Command += (lBtnCopy_Command);
            panelLine.Controls.Add(lBtn);

            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ ";
            lBtn.Command += (lBtnSave_Command);
            panelLine.Controls.Add(lBtn);

            panelWrapper.Controls.Add(panelLine);

            #endregion

            return panelWrapper;
        }

        #region События к функции GetOneNewsEditPanel(...)

        #region lBtnAddFoto_Command

        /// <summary>Кнопка ДОБАВИТЬ ФОТО для анонса новости</summary>
        /// <param name="sender"></param>
        /// <param name="e">ID новости</param>
        private void lBtnAddFoto_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["tempNewsStruct"] = CollectNewsStruct();   //метод сбора переменных из текстбоксов в поля переменной Session["tempNewsStruct"], нужно для сохранения изменений в текстбоксах

            FileUpload fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fileFotoUpload");
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);

            if (fUpload.HasFile)
            {
                string fileName = _pag.Server.HtmlEncode(fUpload.FileName);
                string extension = Path.GetExtension(fileName);

                if ((extension.ToLower() == ".jpg") || (extension.ToLower() == ".jpeg") || (extension.ToLower() == ".png"))     //проверка на допустимые расширения закачиваемого файла
                {
                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 100000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        string newFileName = e.CommandArgument + "_" + Guid.NewGuid().ToString().Replace("-", "") + extension;
                        string savePath = _pathToImgFolder + newFileName;
                        try
                        {
                            fUpload.SaveAs(savePath);
                            ((News)_pag.Session["tempNewsStruct"]).LogoImgName = newFileName;
                        }
                        catch (Exception ex)
                        {
                            warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", _pag.Master);
                            DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                            return;
                        }
                        _pag.Response.Redirect(_pag.Request.RawUrl);
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

        #endregion
        #region lBtnDelFoto_Command

        /// <summary>нажатие на кнопку УДАЛИТЬ ФОТО для анонса новости</summary>
        /// <param name="sender"></param>
        /// <param name="e">имя файла фотографии для анонса новости</param>
        private void lBtnDelFoto_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);

            try
            {
                File.Delete(_pathToImgFolder + ((News)_pag.Session["tempNewsStruct"]).LogoImgName);
                ((News)_pag.Session["tempNewsStruct"]).LogoImgName = "";
            }
            catch (Exception ex)
            {
                warning.ShowWarning("ВНИМАНИЕ. Не удалось удалить файл изображения. Попробуйте повторить.", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #region lBtnAddFile_Command

        /// <summary> нажатие на кнопку - ДОБАВИТЬ файл на сервер и выдать прямую ссылку на него.</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит ID новости</param>
        protected void lBtnAddFile_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["tempNewsStruct"] = CollectNewsStruct();   //метод сбора переменных из текстбоксов в поля переменной Session["tempNewsStruct"], нужно для сохранения изменений в текстбоксах

            FileUpload fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fileUpload");
            CheckBox chkBox = (CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxOverlayImgAdd");
            //Label hLinkToFile = (Label)_pag.FindControl("ctl00$ContentPlaceHolder1$hLinkToFile");
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);

            if (fUpload.HasFile)
            {
                string fileName = _pag.Server.HtmlEncode(fUpload.FileName);
                string extension = Path.GetExtension(fileName);

                int fileSize = fUpload.PostedFile.ContentLength;
                if (fileSize < 10000000)                              //проверка на допустимый размер закачиваемого файла
                {
                    string newFileName = e.CommandArgument + "_" + Guid.NewGuid().ToString().Replace("-", "") + extension;
                    string savePath = _pathToFilesFolder + newFileName;
                    try
                    {
                        fUpload.SaveAs(savePath);
                        string hlink = "http://" + _pag.Request.ServerVariables["SERVER_NAME"] + ":" + _pag.Request.ServerVariables["SERVER_PORT"] + "/" +
                                       _imgUrlPathFile.Replace("../../../", "") + newFileName;
                        ((News)_pag.Session["tempNewsStruct"]).HLinkToFile = hlink;

                        #region Код наложения защитной картинки на сохранённое изображение

                        if (chkBox.Checked && (extension.ToLower() == ".jpg") || (extension.ToLower() == ".jpeg") || (extension.ToLower() == ".png"))
                        {
                            string tempPath = _pathToTempFolder + newFileName;
                            File.Copy(savePath, tempPath);
                            string pathToOverlayImg = HttpContext.Current.Server.MapPath("~") + @"files\pages\overlayimg.png";
                            if (File.Exists(pathToOverlayImg))
                            {
                                var imgOverlay = new ImageOverlay();
                                Bitmap tempImage = new Bitmap(tempPath);
                                var newImage = imgOverlay.Overlay(tempImage, new Bitmap(pathToOverlayImg));
                                if (extension.ToLower() == ".jpg")
                                {
                                    newImage.Save(savePath, ImageFormat.Jpeg);
                                }
                                else if (extension.ToLower() == ".jpeg")
                                {
                                    newImage.Save(savePath, ImageFormat.Jpeg);
                                }
                                else if (extension.ToLower() == ".png")
                                {
                                    newImage.Save(savePath, ImageFormat.Png);
                                }
                                tempImage.Dispose();
                                newImage.Dispose();
                            }
                            File.Delete(tempPath);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", _pag.Master);
                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                        return;
                    }
                    _pag.Response.Redirect(_pag.Request.RawUrl);
                }
                else warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 2-х мегабайт.", _pag.Master);
            }
            else warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", _pag.Master);
        }

        #endregion
        #region lBtnCopy_Command

        /// <summary> нажатие на кнопку СКОПИРОВАТЬ новость</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnCopy_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            NewsWork work = new NewsWork();

            try
            {
                _pag.Session["tempNewsStruct"] = CollectNewsStruct();
                ((News)_pag.Session["tempNewsStruct"]).Id = work.GetNextNewsId();
                //((News)_pag.Session["tempNewsStruct"]).LogoImgName = "";
                ((News)_pag.Session["tempNewsStruct"]).IsNew = true;
                ((News)_pag.Session["tempNewsStruct"]).Enabled = false;
            }
            catch (Exception ex)
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе копирования новости. Попробуйте повторить или перезагрузите эту страницу.", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #region lBtnSave_Command

        /// <summary> нажатие на кнопку СОХРАНИТЬ новость</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnSave_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            _pag.Session["tempNewsStruct"] = CollectNewsStruct();
            //для удобства чтения кода запишем структуру новости в новый объект
            News obj = (News)_pag.Session["tempNewsStruct"];

            #region Проверим введённые данные на правильность

            if (obj.LogoImgName.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Нужно обязательно добавить изображение для анонса новости..", _pag.Master); return; }
            if (obj.Title.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Нужно обязательно добавить заголовок новости..", _pag.Master); return; }
            if (obj.Text.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Нужно обязательно добавить основной текст новости..", _pag.Master); return; }
            warning.HideWarning(_pag.Master);

            #endregion

            #region Сохранение новости в БД

            //Возможность делать в анонсе переносы
            string[] anonsTxt = obj.Anons.Split(Environment.NewLine.ToCharArray());
            obj.Anons = "";
            int counter = 0;
            foreach (string s in anonsTxt)
            {
                if (s != "")
                {
                    if (counter == 0)
                        obj.Anons += s;
                    else
                        obj.Anons += "|" + s;
                }
                counter++;
            }

            NewsWork work = new NewsWork();
            if (obj.IsNew) //если нужно добавить новую новость
            {
                obj.DateReg = DateTime.Now.Ticks;
                if (work.InsertOneNews(obj) == -1)
                {
                    warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить новость. Попробуйте повторить..", _pag.Master); return;
                }
                obj.IsNew = false;
            }
            else
            {
                if (work.UpdateNews(obj) == -1)
                {
                    warning.ShowWarning("ВНИМАНИЕ. Не удалось обновить новость. Попробуйте повторить..", _pag.Master);
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #endregion

        #region Метод CollectNewsStruct()
        /// <summary>Метод считывает данные в структуру данных для новой новости, находящейся временно в сессионной переменной.</summary>
        /// <returns></returns>
        private News CollectNewsStruct()
        {
            News resultStruct = new News();

            try
            {
                resultStruct = (News)_pag.Session["tempNewsStruct"];
                // ЗАГОЛОВОК
                resultStruct.Title = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxTitle")).Text;
                // АНОНС
                resultStruct.Anons = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAnons")).Text;
                // ТЕКСТ
                resultStruct.Text = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxText")).Text;
                // CSS
                resultStruct.Param1 = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxCss")).Text;
                // JavaScript
                resultStruct.Param2 = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxJavascript")).Text;
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return resultStruct;
        }
        #endregion

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>Класс формирования данных по новостям для класса NewsForm</summary>
    public class NewsWork
    {
        private HttpContext _context;
        private string _pathToDb;
        private string _tableName;
        private string _tableNewsCountName;
        private string _pathToImgFolder;
        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        private string _imgUrlPathFoto;
        private string _imgUrlPathFile;
        public bool _checkFolders;
        public bool _checkNewCountExist;

        /// <summary>Конструктор класса. Добавляет в БД таблицу новостей, если её ещё не существует.
        /// Так же инициализирует поля класса.</summary>
        public NewsWork()
        {
            _context = HttpContext.Current;
            _pathToImgFolder = _context.Server.MapPath("~") + @"files\news\foto\";
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\news\files\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _imgUrlPathFoto = "../../../files/news/foto/";
            _imgUrlPathFile = "../../../files/news/files/";
            _checkFolders = true;

            #region Создание необходимых папок

            try
            {
                Directory.CreateDirectory(_pathToImgFolder);
                Directory.CreateDirectory(_pathToFilesFolder);
                Directory.CreateDirectory(_pathToTempFolder);
            }
            catch (Exception ex)
            {
                _checkFolders = false;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            #endregion

            #region Добавление таблицы для новостей в БД

            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\news.db";
            _tableName = "newstable";

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "CREATE TABLE IF NOT EXISTS " + _tableName + "(" +
                               "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "Title TEXT NOT NULL, " +
                               "Anons TEXT NOT NULL, " +
                               "Text TEXT NOT NULL, " +
                               "LogoImgName TEXT NOT NULL, " +

                               "DateReg INTEGER NOT NULL, " +
                               "Enabled INTEGER NOT NULL, " +

                               "Param1 TEXT NOT NULL, " +
                               "Param2 TEXT NOT NULL, " +
                               "Param3 INTEGER NOT NULL, " +
                               "Param4 INTEGER NOT NULL, " +
                               "Param5 INTEGER NOT NULL, " +
                               "Param6 INTEGER NOT NULL" +
                               ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();

            #endregion

            #region Добавление таблицы для хранения одного значения с кол-вом выводимых на страницу новостных блоков в БД и добавление едиственной строки..

            _checkNewCountExist = true;
            _tableNewsCountName = "newscount";

            sqlite = new SqliteHelper(_pathToDb);
            sqlString = "CREATE TABLE IF NOT EXISTS " + _tableNewsCountName + "(" +
                               "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "NewsCount INTEGER NOT NULL" +
                               ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();

            int result = 0;
            try
            {
                #region MyRegion

                // проверяем, если ли уже нужная нам строка в таблице или нет
                sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT MAX(_id) FROM " + _tableNewsCountName;
                result = sqlite.ExecuteScalarParams(cmd);
                cmd.Dispose(); sqlite.ConnectionClose();

                if (result == 0 || result == -1)
                {
                    sqlite = new SqliteHelper(_pathToDb);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = "INSERT INTO " + _tableNewsCountName + " (NewsCount) VALUES (10)";

                    if (sqlite.ExecuteNonQueryParams(cmd) == -1) _checkNewCountExist = false;
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                result = -1;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            #endregion
        }

        /// <summary>Метод добавляет в БД одну новость.</summary>
        /// <param name="obj">объект News с данными по одной новости</param>
        /// <returns>Метод возвращает номер(id) внесённости в БД новости или -1 в случае какой-либо ошибки</returns>
        public int InsertOneNews(News obj)
        {
            int result = 0;
            if (!_checkFolders) return -1;

            try
            {
                #region MyRegion

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                           "Title, " +
                                                           "Anons, " +
                                                           "Text, " +
                                                           "LogoImgName, " +

                                                           "DateReg, " +
                                                           "Enabled, " +

                                                           "Param1, " +
                                                           "Param2, " +
                                                           "Param3, " +
                                                           "Param4, " +
                                                           "Param5, " +
                                                           "Param6" +
                                                            ") " +
                                                "VALUES (" +
                                                           "@Title, " +
                                                           "@Anons, " +
                                                           "@Text, " +
                                                           "@LogoImgName, " +

                                                           "@DateReg, " +
                                                           "@Enabled, " +

                                                           "@Param1, " +
                                                           "@Param2, " +
                                                           "@Param3, " +
                                                           "@Param4, " +
                                                           "@Param5, " +
                                                           "@Param6" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@Title", obj.Title));
                cmd.Parameters.Add(new SQLiteParameter("@Anons", obj.Anons));
                cmd.Parameters.Add(new SQLiteParameter("@Text", obj.Text));
                cmd.Parameters.Add(new SQLiteParameter("@LogoImgName", obj.LogoImgName));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Enabled", obj.Enabled ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@Param1", obj.Param1));
                cmd.Parameters.Add(new SQLiteParameter("@Param2", obj.Param2));
                cmd.Parameters.Add(new SQLiteParameter("@Param3", obj.Param3));
                cmd.Parameters.Add(new SQLiteParameter("@Param4", obj.Param4));
                cmd.Parameters.Add(new SQLiteParameter("@Param5", obj.Param5));
                cmd.Parameters.Add(new SQLiteParameter("@Param6", obj.Param6));

                if (sqlite.ExecuteNonQueryParams(cmd) == -1) return -1;
                cmd.Dispose(); sqlite.ConnectionClose();

                // определим номер последней добавленной строки, он и будет номером добавленной заявки
                sqlite = new SqliteHelper(_pathToDb);
                cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT MAX(_id) FROM " + _tableName;
                result = sqlite.ExecuteScalarParams(cmd);
                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                result = -1;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return result;
        }

        /// <summary>Метод возвращает список структур всех новостей.</summary>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<News> GetListOfNews()
        {
            List<News> resultList = new List<News>();
            if (!_checkFolders) return null;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _tableName;

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }
                try
                {
                    #region Код заполнения списка

                    News obj = new News();
                    while (reader.Read())
                    {
                        obj = new News();
                        FillRequest(obj, reader);
                        resultList.Add(obj);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                }
                finally
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                resultList = null;
            }

            return resultList;
        }

        /// <summary>Метод-обёртка над методом GetListOfNews. Сортирует список структур по дате.
        /// Так же производится фильтрация по поисковой строке</summary>
        /// <param name="srchString">строка поискового запроса</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<News> GetSortedListOfNews(string srchString = "")
        {
            List<News> tempList = GetListOfNews();
            if (!_checkFolders) return null;

            if (tempList == null) return null;
            tempList = tempList.OrderBy(x => x.DateReg * -1).ToList();

            #region Фильтрация по поисковой строке

            List<News> resultList = new List<News>();
            DateTime dt = new DateTime();
            if (srchString != "")
            {
                foreach (News obj in tempList)
                {
                    dt = new DateTime(obj.DateReg);
                    if (obj.Title.ToLower().Contains(srchString.ToLower()) ||
                        obj.Anons.ToLower().Contains(srchString.ToLower()) ||
                        obj.Text.ToLower().Contains(srchString.ToLower()) ||
                        dt.ToShortDateString().Contains(srchString)
                        )
                    {
                        resultList.Add(obj);
                    }
                }
            }
            else
            {
                resultList = tempList;
            }

            #endregion

            return resultList;
        }

        /// <summary>Метод возвращает одну новость по её номеру (id)</summary>
        /// <param name="id">номер новости</param>
        /// <returns>Возвращает null в случае ошибки</returns>
        public News GetOneNews(string id)
        {
            News obj = new News();
            if (!_checkFolders) return null;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    return null;
                }

                try
                {
                    while (reader.Read())
                    {
                        FillRequest(obj, reader);
                    }
                }
                catch (Exception ex)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    obj = null;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                obj = null;
            }

            return obj;
        }

        /// <summary>Метод возвращает следующий id для создаваемой новости</summary>
        /// <returns>Возвращает следующий id для новой создаваемой новости или -1 в случае ошибки</returns>
        public int GetNextNewsId()
        {
            int result = -1;
            if (!_checkFolders) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT MAX(_id) FROM " + _tableName;
                //cmd.Parameters.Add(new SQLiteParameter("id", id));

                try
                {
                    result = sqlite.ExecuteScalarParams(cmd);
                    if (result == -1) //если в таблице нет ни одной новости, то..
                    {
                        result = 1;
                    }
                    else
                    {
                        result += 1;
                    }
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    result = -1;
                }



                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        /// <summary>Метод возвращает значение кол-ва одновременно выводимых новостей для новостного блока</summary>
        /// <returns></returns>
        public int GetNewsCount()
        {
            int result = -1;
            if (!_checkNewCountExist) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT MAX(NewsCount) FROM " + _tableNewsCountName;
                //cmd.Parameters.Add(new SQLiteParameter("id", id));

                try
                {
                    result = sqlite.ExecuteScalarParams(cmd);
                    if (result == -1) //если ошибка, то..
                    {
                        return -1;
                    }
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    result = -1;
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }


            return result;
        }

        /// <summary>Метод обновляет количество одновременно выводимых на одну страницу блоков новостей</summary>
        /// <param name="newsCount">новое кол-ко выводимых строк</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateNewsCount(string newsCount)
        {
            int result = -1;
            if (!_checkNewCountExist) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableNewsCountName + " SET " +
                                    "NewsCount=@NewsCount " +
                                  "WHERE _id=1;";
                cmd.Parameters.Add(new SQLiteParameter("NewsCount", newsCount));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        /// <summary>Метод обновляет новость в БД. Дата новости не обновляется!</summary>
        /// <param name="obj">объект с данныйми по одной новости</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateNews(News obj)
        {
            int result = -1;
            if (!_checkFolders) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET " +
                                    "Title=@Title, " +
                                    "Anons=@Anons, " +
                                    "Text=@Text, " +
                                    "LogoImgName=@LogoImgName, " +

                                    "Enabled=@Enabled, " +

                                    "Param1=@Param1, " +
                                    "Param2=@Param2, " +
                                    "Param3=@Param3, " +
                                    "Param4=@Param4, " +
                                    "Param5=@Param5, " +
                                    "Param6=@Param6 " +
                                  "WHERE _id=@id;";

                cmd.Parameters.Add(new SQLiteParameter("id", obj.Id));
                cmd.Parameters.Add(new SQLiteParameter("Title", obj.Title));
                cmd.Parameters.Add(new SQLiteParameter("Anons", obj.Anons));
                cmd.Parameters.Add(new SQLiteParameter("Text", obj.Text));
                cmd.Parameters.Add(new SQLiteParameter("LogoImgName", obj.LogoImgName));
                cmd.Parameters.Add(new SQLiteParameter("Enabled", obj.Enabled ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("Param1", obj.Param1));
                cmd.Parameters.Add(new SQLiteParameter("Param2", obj.Param2));
                cmd.Parameters.Add(new SQLiteParameter("Param3", obj.Param3));
                cmd.Parameters.Add(new SQLiteParameter("Param4", obj.Param4));
                cmd.Parameters.Add(new SQLiteParameter("Param5", obj.Param5));
                cmd.Parameters.Add(new SQLiteParameter("Param6", obj.Param6));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        /// <summary>Метод обновляет значение публикация одной новости</summary>
        /// <param name="id">id новости</param>
        /// <param name="enabled">1 - включить новость, 0 - выключить новость</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateNewsEnabled(long id, int enabled)
        {
            int result = -1;
            if (!_checkFolders) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET Enabled=@Enabled WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("Enabled", enabled));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        /// <summary>Метод удаляет одну новость из БД. Файлы, относящиеся к новости, не удаляются.</summary>
        /// <param name="id">id заявки</param>
        /// <returns>Метод возвращает кол-во удалённых строк из таблицы или -1 в случае какой-либо ошибки или 0 - если ни одной записи не обновлено</returns>
        public int DeleteOneNews(string id)
        {
            int result = -1;
            if (!_checkFolders) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "DELETE FROM " + _tableName + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        /// <summary>Метод удаляет все файлы из папок новостей, которые не принадлежат ни одной зарегистрированной в БД новости</summary>
        /// <returns>Возвращает true в случае успеха, и false - в случае возникновения какой-либо ошибки</returns>
        public bool DeleteUnnecessaryFiles()
        {
            if (!_checkFolders) return false;

            try
            {
                #region Основной код

                List<News> allList = GetListOfNews();
                List<string> listOfNewsId = new List<string>();
                foreach (News obj in allList)
                {
                    listOfNewsId.Add(obj.Id + "_");  //формируем начало имени файлов, с которых начинаются все файлы, относящиеся к данной новости
                }

                string[] pathToFoto = Directory.GetFiles(_pathToImgFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFiles = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);

                string fName = "";
                bool checker = false;
                foreach (string path in pathToFoto)
                {
                    fName = Path.GetFileName(path);
                    checker = false;
                    foreach (string s in listOfNewsId)
                    {
                        if (fName.StartsWith(s))
                        {
                            checker = true; break;
                        }
                    }
                    if (!checker)
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFiles)
                {
                    fName = Path.GetFileName(path);
                    checker = false;
                    foreach (string s in listOfNewsId)
                    {
                        if (fName.StartsWith(s))
                        {
                            checker = true; break;
                        }
                    }
                    if (!checker)
                    {
                        File.Delete(path);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа CompetitionRequest данными из SQLiteDataReader.
        /// Метод используется при чтении данных из БД.</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        private void FillRequest(News obj, SQLiteDataReader reader)
        {
            obj.Title = reader["Title"].ToString();
            obj.Anons = reader["Anons"].ToString();
            obj.Text = reader["Text"].ToString();
            obj.LogoImgName = reader["LogoImgName"].ToString();

            obj.Id = long.Parse(reader["_id"].ToString());
            obj.DateReg = long.Parse(reader["DateReg"].ToString());
            obj.Enabled = int.Parse(reader["Enabled"].ToString()) == 1;

            obj.Param1 = reader["Param1"].ToString();
            obj.Param2 = reader["Param2"].ToString();
            obj.Param3 = int.Parse(reader["Param3"].ToString()) == 1;
            obj.Param4 = int.Parse(reader["Param4"].ToString()) == 1;
            obj.Param5 = long.Parse(reader["Param5"].ToString());
            obj.Param6 = long.Parse(reader["Param6"].ToString());
        }

        /// <summary>Метод возвращает размер файла БД в килобайтах</summary>
        /// <returns></returns>
        public long GetDbSize()
        {
            long result = 0;

            #region Основной код

            if (File.Exists(_pathToDb))
            {
                FileInfo f = new FileInfo(_pathToDb);
                result = f.Length / 1024;
            }

            #endregion

            return result;
        }

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к новостям</summary>
        /// <returns></returns>
        public long GetFoldersSize()
        {
            long result = 0;

            #region Основной код

            try
            {
                #region Основной код

                if (Directory.Exists(_pathToFilesFolder) && Directory.Exists(_pathToImgFolder))
                {
                    string[] arr1 = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr2 = Directory.GetFiles(_pathToImgFolder, "*", SearchOption.TopDirectoryOnly);
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

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = 0;

                #endregion
            }

            #endregion

            return result;
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>Класс представляет структуру данных одной НОВОСТИ,
    /// которая подходит для любого конкурса</summary>
    [Serializable]
    public class News
    {
        private string title = "";              //Заголовок *
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string anons = "";              //Текст анонса новости * 
        public string Anons
        {
            get { return anons; }
            set { anons = value; }
        }

        private string text = "";               //Текст анонса новости
        public string Text                      //Полное содержимое новости *
        {
            get { return text; }
            set { text = value; }
        }

        private string logoImgName = "";        //Ссылка на картинку для анонса новости
        public string LogoImgName               //Полное содержимое новости * (имя файла должно начинаться так '<id новости>_<GUID>.<расширение>. И все файлы, относящиеся к новости должны иметь такую структуру)
        {
            get { return logoImgName; }
            set { logoImgName = value; }
        }



        // поля специального назначения
        private long id = -1;                   // номер новости (формируется в БД)
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private long dateReg = 0;                //Дата опубликования новости на сайте *
        public long DateReg
        {
            get { return dateReg; }
            set { dateReg = value; }
        }

        private bool enabled = false;                //true - новость отображается на сайте, false - отображается *
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private string hLinkToFile = "";                //будет содержать полную ссылку на закачанный файл. Эта ссылку будет использоваться в редакторе полного текста новости. Поле используется только для временного хранения такой ссылки (в методе - GetOneNewsEditPanel(...)), в БД её нет.
        public string HLinkToFile
        {
            get { return hLinkToFile; }
            set { hLinkToFile = value; }
        }

        private bool isNew = false;                //true - новость новая, false - новость уже существует. Переменная используется только при создании или редактировании новости. В БД её нет. *
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }




        // Дополнительные параметры, которые возможно будут использованы
        private string param1 = "";
        /// <summary>Хранятся таблицы стилей для страницы с полной информацией по новости</summary>
        public string Param1
        {
            get { return param1; }
            set { param1 = value; }
        }

        private string param2 = "";
        /// <summary>Хранится JavaScript для страницы с полной информацией по новости</summary>
        public string Param2
        {
            get { return param2; }
            set { param2 = value; }
        }

        private bool param3 = false;
        public bool Param3
        {
            get { return param3; }
            set { param3 = value; }
        }

        private bool param4 = false;
        public bool Param4
        {
            get { return param4; }
            set { param4 = value; }
        }

        private long param5 = 0;
        public long Param5
        {
            get { return param5; }
            set { param5 = value; }
        }

        private long param6 = 0;
        public long Param6
        {
            get { return param6; }
            set { param6 = value; }
        }
    }

    #endregion
}