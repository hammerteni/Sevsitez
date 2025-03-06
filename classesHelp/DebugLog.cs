// Код для работы с событиями ошибок
// База данных - 
// ~/files/sqlitedb/errors.db

#region Using

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;

#endregion

namespace site.classesHelp
{
    #region Класс DebugLog

    /// <summary>Класс содержит метод для записи событий и ошибок в БД ошибок
    /// Пример использования:
    /// DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
    /// </summary>
    static class DebugLog
    {
        #region Метод Log(....)

        /// <summary>Метода записывает в БД логов ошибок событие ошибки</summary>
        /// <param name="type">вид события ошибки</param>
        /// <param name="class_name">имя класс, в котором произошло событие</param>
        /// <param name="method_name">имя метода, в котором произошло событие</param>
        /// <param name="text">описание события ошибки</param>
        public static void Log(ErrorEvents type, string className, string methodName, string text)
        {
            DebugLogObj obj = new DebugLogObj();
            obj.DateReg = DateTime.Now.Ticks;
            obj.Type = ErrorEventsHelper.TypeToCode(type);
            obj.ClassName = className;
            obj.MethodName = methodName;
            obj.Text = text;

            DebugLogWork work = new DebugLogWork();
            work.InsertOne(obj);
        }

        #endregion
    }

    #endregion

    #region Код формирования html-кода

    #region Класс DebugLogForm

    /// <summary>Класс формирует html-код для отображения страницы просмотра событий ошибок в консоли управления сайтом</summary>
    class DebugLogForm
    {
        #region Поля

        private Page _pag;
        private string _prefix;
        private DebugLogWork _work;
        private List<DebugLogObj> _objIdsList;   //для работы выделения галочкой строк таблицы в консоли управления

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса</summary>
        /// <param name="thisPage">объект страницы, в которой вызываются методы класса</param>
        /// <param name="prefix">уникальный префикс, используемый в названиях сессионных переменных и т.п.</param>
        public DebugLogForm(Page thisPage, string prefix)
        {
            _pag = thisPage;
            _prefix = prefix;
            _work = new DebugLogWork();
            _objIdsList = new List<DebugLogObj>();
        }

        #endregion

        #region Для консоли управления

        #region Метод ConsolePanel()

        /// <summary>Метод возвращает панель со списком всех объектов событий ошибок для Консоли управления</summary>
        /// <returns></returns>
        public Panel ConsolePanel()
        {
            Panel panelWrapper = new Panel(); Label lbl;

            string srchString = _pag.Session["srchStr" + _prefix].ToString();
            int pageNum = (int)_pag.Session["pagenum" + _prefix];
            int elemInOnePage = 50;
            ConfigFile cfg = new ConfigFile();

            #region Заголовок

            panelWrapper.Controls.Add(new LiteralControl("<span class='lblSectionTitle'>ПАНЕЛЬ СООБЩЕНИЙ ОБ ОШИБКАХ В РАБОТЕ САЙТА</span>"));

            #endregion

            #region Панель с кнопками управления

            #region Поиск

            TextBox txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = srchString; txtBox.ID = "txtBoxSrchEvent";
            txtBox.ToolTip = "Введите текст для поиска. Поиск осуществляется по всем полям, содержащим текстовые данные.";
            panelWrapper.Controls.Add(txtBox);

            panelWrapper.Controls.Add(GetSymbols(2, "&nbsp;"));

            LinkButton lBtn = new LinkButton(); lBtn.CssClass = "btnSpec";
            lBtn.Text = " Найти "; lBtn.ToolTip = "Кнопка поиска. Поиск осуществляется по всем полям, содержащим текстовые данные.";
            lBtn.OnClientClick = "waiting('Идёт выполнение операции. Подождите..', 500);";
            lBtn.Command += (lBtnSrch_Command); lBtn.ID = "btnEventSrch";
            panelWrapper.Controls.Add(lBtn);

            #endregion
            #region Кнопка Удалить все события из БД

            panelWrapper.Controls.Add(GetSymbols(10, "&nbsp;"));

            lBtn = new LinkButton(); lBtn.CssClass = "btnSpec";
            lBtn.Text = "Очистить БД"; lBtn.ToolTip = "Удаление всех событий ошибок из БД";
            lBtn.OnClientClick = "if(confirm('Все события будут удалены. Продолжить?')) {waiting('Идёт удаление событий. Подождите..', 0); return true;} else {return false;};";
            lBtn.Command += (lBtnCleanDB_Command); panelWrapper.Controls.Add(lBtn);

            #endregion
            #region Кнопка Дефрагментации БД

            panelWrapper.Controls.Add(GetSymbols(4, "&nbsp;"));

            lBtn = new LinkButton(); lBtn.CssClass = "btnSpec";
            lBtn.Text = "Дефрагментировать БД"; lBtn.ToolTip = "Дефрагментация БД (выполнение команды VACUUM)";
            lBtn.OnClientClick = "if(confirm('Будет произведена дефрагментация БД. Продолжить?')) {waiting('Идёт дефрагментация. Подождите..', 0); return true;} else {return false;};";
            lBtn.Command += (lBtnDefragDB_Command); panelWrapper.Controls.Add(lBtn);

            #endregion
            #region Вывод размера файла БД

            panelWrapper.Controls.Add(GetSymbols(10, "&nbsp;"));

            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Размер БД: "; panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = _work.GetDbSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

            #endregion

            #endregion

            #region Таблица

            List<DebugLogObj> structList = _work.GetSortedList(srchString);

            Table tbl = new Table(); tbl.CssClass = "tblMain"; panelWrapper.Controls.Add(tbl);
            TableRow tblRow = new TableRow(); TableCell tblCell = new TableCell();

            if (structList == null)
            {
                #region Надпись о том, что не удалось получить данные для отображения

                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Font.Italic = true; lbl.Font.Size = 12;
                lbl.Text = "<br/>Не удалось получить данные для отображения. Обратить в техподдержку сайта."; tblCell.Controls.Add(lbl);

                #endregion

                return panelWrapper;
            }

            if (structList.Count > 0)  //если список хоть что-то содержит, то..
            {
                #region Код

                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

                #region Кнопки-ссылки на страницы

                PagePanelClass pageBtns = new PagePanelClass(structList.Count, elemInOnePage, _prefix);
                tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

                #endregion

                int pageCounter = 1;
                int prodCounter = 0;

                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего событий: "; tblCell.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = structList.Count.ToString(); tblCell.Controls.Add(lbl);

                #region Кнопка удаления всех выделенных галочками строк в таблице

                tblCell.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;"));
                lBtn = new LinkButton(); lBtn.CssClass = "btnSpec";
                lBtn.Text = "Удалить отмеченное"; lBtn.ToolTip = "Удаляются отмеченные события";
                lBtn.Command += (lBtnDeleteChecked_Command); lBtn.ID = "btnDeleteChecked";
                lBtn.OnClientClick = "if(confirm('Отмеченные события будут удалены. Сделать?')) {waiting('Идёт удаление событий. Подождите..', 500); return true;} else {return false;};";
                tblCell.Controls.Add(lBtn);
                tblCell.Controls.Add(new LiteralControl("<br/><br/>"));

                #endregion

                #region Добавляем подтаблицу

                Table tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

                #region Шапка таблицы

                var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
                var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead tblSubCellHead1"; tblRow1.Controls.Add(tblCell1);
                CheckBox chkBox = new CheckBox(); chkBox.ID = "main_chkbox"; tblCell1.Controls.Add(chkBox);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "№"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Id"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата, время"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Вид"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Класс"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Метод"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Описание"; tblCell1.Controls.Add(lbl);

                #endregion

                #region Таблица

                TableRow tblRow2; TableCell tblCell2; Label lbl2;
                DateTime dt;
                string location = "";
                string tmp = "";
                string[] tmpArr = { };
                int countofstr = 0;

                foreach (DebugLogObj oneStruct in structList)
                {
                    countofstr++;
                    prodCounter++;
                    if (prodCounter > elemInOnePage)
                    {
                        prodCounter = 1;
                        pageCounter++;
                    }

                    if (pageCounter == pageNum)           //если эта та самая страница, которую нужно вывести, то выводим её
                    {
                        #region Код добавления одной строки

                        tblRow2 = new TableRow(); tblRow2.CssClass = "tblRow_adm"; tbl1.Controls.Add(tblRow2);
                        if (ErrorEventsHelper.CodeToType(oneStruct.Type) == ErrorEvents.warn)
                        {
                            tblRow2.CssClass += " tblRow_adm_yellow";
                        }
                        else if (ErrorEventsHelper.CodeToType(oneStruct.Type) == ErrorEvents.err)
                        {
                            tblRow2.CssClass += " tblRow_adm_red";
                        }
                        dt = new DateTime(oneStruct.DateReg);
                        //location = "waiting('Страница загружается. Подождите...', 500); location.assign('/adm/answersedit.aspx?num=" + oneStruct.Id + "');";
                        location = "checkLineCheckbox('#chkbox_" + oneStruct.Id + "');";

                        #region Чекбоксы

                        tmp = "chkbox_" + oneStruct.Id;
                        tblCell2 = new TableCell(); tblCell2.CssClass = "tblSubCell"; tblRow2.Controls.Add(tblCell2);
                        chkBox = new CheckBox(); chkBox.ID = tmp; chkBox.CssClass = "allChcked";
                        tblCell2.Controls.Add(chkBox);
                        _objIdsList.Add(oneStruct);

                        #endregion
                        #region № п/п

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = countofstr.ToString();
                        tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region ID

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = oneStruct.Id.ToString();
                        tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region Дата регистрации события

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1 txtAlignLeft"; tblRow2.Controls.Add(tblCell2);
                        dt = new DateTime(oneStruct.DateReg);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = dt.ToString("dd.MM.yyyy HH:mm:ss"); tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region Вид события

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = oneStruct.Type;
                        tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region Класс

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1 txtAlignLeft"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = oneStruct.ClassName;
                        tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region Метод 

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1 txtAlignLeft"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = oneStruct.MethodName;
                        tblCell2.Controls.Add(lbl2);

                        #endregion
                        #region Описание

                        tblCell2 = new TableCell(); tblCell2.Attributes.Add("onclick", location);
                        tblCell2.CssClass = "tblSubCell tblSubCell1 txtAlignLeft"; tblRow2.Controls.Add(tblCell2);
                        lbl2 = new Label(); lbl2.CssClass = "lblTxtTbl"; lbl2.Text = oneStruct.Text;
                        tblCell2.Controls.Add(lbl2);

                        #endregion

                        #endregion
                    }
                    else if (pageCounter > pageNum)
                    {
                        break;
                    }
                }

                #endregion

                #endregion

                #region Кнопки-ссылки на страницы

                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("1"));

                #endregion

                #region Добавление JavaScript

                StringBuilder js = new StringBuilder();
                js.Append("<script type='text/javascript'> ");

                #region Добавлени Javascript для работы чекбокса выделения/снятия выделения со всех строк в таблице

                js.Append("var $mainChkBox = $('#main_chkbox'); ");
                js.Append("$mainChkBox.change(function() { ");
                js.Append("if(this.checked) { ");
                js.Append("$('.allChcked input').each(function(){ this.checked = true; }); ");
                js.Append("} else { ");
                js.Append("$('.allChcked input').each(function(){ this.checked = false; }); ");
                js.Append("} ");
                js.Append("}); ");

                js.Append("$('.allChcked input').each(function() { ");
                js.Append("$(this).change(function() { ");
                js.Append("if(!this.checked) { ");
                js.Append("$mainChkBox.prop('checked', false); ");
                js.Append("} ");
                js.Append("}); ");
                js.Append("}); ");

                #endregion
                #region Добавлени Javascript с функцией выделения чекбокса по его ID

                js.Append("function checkLineCheckbox(chkBocId) ");
                js.Append("{ ");
                js.Append("var $chkBox = $(chkBocId); ");
                js.Append("if ($chkBox.prop('checked')) ");
                js.Append("{ ");
                js.Append("$chkBox.prop('checked', false); ");
                js.Append("} ");
                js.Append("else ");
                js.Append("{ ");
                js.Append("$chkBox.prop('checked', true); ");
                js.Append("} ");
                js.Append("} ");

                #endregion

                js.Append("</script>");

                _pag.Controls.Add(new LiteralControl(js.ToString()));

                #endregion

                #endregion
            }
            else
            {
                #region Код

                if (srchString == "")
                {
                    #region Надпись об отсутствии Информационных сообщений

                    tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                    tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                    lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Font.Italic = true; lbl.Font.Size = 12;
                    lbl.Text = "<br/>События отсутствуют.."; tblCell.Controls.Add(lbl);

                    #endregion
                }
                else
                {
                    #region Надпись об отсутствии результатов поиска

                    tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                    tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                    lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Font.Italic = true; lbl.Font.Size = 12;
                    lbl.Text = "<br/>По вашему запросу не найдено событий.."; tblCell.Controls.Add(lbl);

                    #endregion
                }

                #endregion
            }

            #endregion

            return panelWrapper;
        }

        #region События

        #region lBtnSrch_Command

        /// <summary>Событие нажатия на кнопку НАЙТИ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSrch_Command(object sender, CommandEventArgs e)
        {
            bool err = false;

            try
            {
                TextBox txtBox = (TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent");
                _pag.Session["srchStr" + _prefix] = txtBox.Text;
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message);
            }

            if (!err)
            {
                _pag.Response.Redirect(_pag.Request.RawUrl);
            }
        }

        #endregion
        #region lBtnCleanDB_Command

        /// <summary>Событие нажатия на кнопку удаления всех событий ошибок из БД</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCleanDB_Command(object sender, CommandEventArgs e)
        {
            _work.DropTable();
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #region lBtnDefragDB_Command

        /// <summary>Событие нажатия на кнопку дефрагментации БД</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDefragDB_Command(object sender, CommandEventArgs e)
        {
            _work.VacuumDb();
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion
        #region lBtnDeleteChecked_Command

        /// <summary>Событие кнопки удаления всех выделенных Ответов</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDeleteChecked_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            #region Получаем список объектов, которые отмечены галочкой

            List<DebugLogObj> listToRemove = new List<DebugLogObj>();
            foreach (DebugLogObj obj in _objIdsList)
            {
                if (((CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$chkbox_" + obj.Id)).Checked)
                {
                    listToRemove.Add(obj);
                }
            }

            #endregion
            #region Если ничего не отмечено галочкой, то..

            if (listToRemove.Count == 0)
            {
                warning.ShowWarning("Ничего не отмечено для удаления...", _pag.Master);
                return;
            }

            #endregion
            #region Удаляем отмеченное

            int res;
            foreach (DebugLogObj obj in listToRemove)
            {
                res = _work.DeleteOne(obj.Id.ToString());
                if (res == 0 || res == -1)
                {
                    DebugLog.Log(ErrorEvents.warn, this.GetType().Name, MethodBase.GetCurrentMethod().Name,
                        "Не удалось удалить событие ошибки с Id " + obj.Id + " из базы данных. Пытался удалить - " + ((AdmPersonStruct)_pag.Session["authperson"]).Name);
                }
            }

            #endregion

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion

        #endregion

        #endregion

        #region Метод GetSymbols(...)

        /// <summary>Метод формирует литеральный элемент с нужным количеством символов</summary>
        /// <param name="count">кол-во неразрывных пробелов</param>
        /// <param name="symbol">символ, который нужно повторить count кол-во раз</param>
        /// <returns></returns>
        private LiteralControl GetSymbols(int count, string symbol)
        {
            LiteralControl result = new LiteralControl();

            symbol = symbol.Trim();
            if (count > 0 && symbol != "")
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < count; i++)
                {
                    sb.Append(symbol);
                }
                result = new LiteralControl(sb.ToString());
            }

            return result;
        }

        #endregion

        #endregion
    }

    #endregion

    #endregion

    #region Код работы с данными

    #region Класс DebugLogWork

    /// <summary>Класс формирования данных по событиям ошибок</summary>
    class DebugLogWork
    {
        #region Поля

        private string _pathToDb;
        private string _table;

        #endregion

        #region Конструктор

        /// <summary>Конструктор класса. Добавляет в БД нужные таблицы, если они ещё не существуют. Так же инициализирует поля.</summary>
        public DebugLogWork()
        {
            _pathToDb = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\sqlitedb\errors.db";
            _table = "errors_table";

            #region Добавление таблицы

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "CREATE TABLE IF NOT EXISTS " + _table + "(" +
                               "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "DateReg INTEGER NOT NULL DEFAULT (0), " +
                               "Type TEXT NOT NULL DEFAULT (''), " +
                               "ClassName TEXT NOT NULL DEFAULT (''), " +
                               "MethodName TEXT NOT NULL DEFAULT (''), " +
                               "Text TEXT NOT NULL DEFAULT ('')" +
                               ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();

            #endregion
        }

        #endregion

        #region Методы

        #region Метод InsertOne(.)

        /// <summary>Метод добавляет в БД данные по одному объекту</summary>
        /// <param name="obj">объект, который нужно добавить в БД</param>
        /// <returns>Метод возвращает Id добавленного объекта или -1 в случае какой-либо ошибки</returns>
        public int InsertOne(DebugLogObj obj)
        {
            int result = 0;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _table + " (" +
                                                           "DateReg, " +
                                                           "Type, " +
                                                           "ClassName, " +
                                                           "MethodName, " +
                                                           "Text" +
                                                            ") " +
                                                "VALUES (" +
                                                           "@DateReg, " +
                                                           "@Type, " +
                                                           "@ClassName, " +
                                                           "@MethodName, " +
                                                           "@Text" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Type", obj.Type));
                cmd.Parameters.Add(new SQLiteParameter("@ClassName", obj.ClassName));
                cmd.Parameters.Add(new SQLiteParameter("@MethodName", obj.MethodName));
                cmd.Parameters.Add(new SQLiteParameter("@Text", obj.Text));

                if (sqlite.ExecuteNonQueryParams(cmd) == -1)
                {
                    result = -1;
                    cmd.Dispose(); sqlite.ConnectionClose();
                }
                else
                {
                    // Определим номер последней добавленной строки, он и будет номером добавленного объекта
                    sqlite = new SqliteHelper(_pathToDb);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = "SELECT MAX(_id) FROM " + _table;
                    result = sqlite.ExecuteScalarParams(cmd);
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch
            {
                #region Код

                result = -1;

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод UpdateOne(.)

        /// <summary>Метод обновляет в БД данные по одному объекту.</summary>
        /// <param name="obj"></param>
        /// <returns>Метод кол-во обновлённых строк или -1 в случае какой-либо ошибки</returns>
        public int UpdateOne(DebugLogObj obj)
        {
            int result = 0;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _table + " SET " +
                                                                "DateReg=@DateReg, " +
                                                                "Type=@Type, " +
                                                                "ClassName=@ClassName, " +
                                                                "MethodName=@MethodName, " +
                                                                "Text=@Text" +
                                                           " WHERE _id=@Id";

                cmd.Parameters.Add(new SQLiteParameter("@Id", obj.Id));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Type", obj.Type));
                cmd.Parameters.Add(new SQLiteParameter("@ClassName", obj.ClassName));
                cmd.Parameters.Add(new SQLiteParameter("@MethodName", obj.MethodName));
                cmd.Parameters.Add(new SQLiteParameter("@Text", obj.Text));


                result = sqlite.ExecuteNonQueryParams(cmd);
                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch
            {
                #region Код

                result = -1;

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод GetList()

        /// <summary>Метод возвращает готовый список объектов</summary>
        /// <returns>Возвращает список структур или null - в случае возникновения какой-либо ошибки во время запроса</returns>
        public List<DebugLogObj> GetList()
        {
            List<DebugLogObj> resultList = new List<DebugLogObj>();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _table;

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    resultList = null;
                }

                if (resultList != null)
                {
                    DebugLogObj obj;
                    while (reader.Read())
                    {
                        obj = new DebugLogObj();
                        FillOne(obj, reader);
                        resultList.Add(obj);
                    }

                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch
            {
                #region Код

                resultList = null;

                #endregion
            }

            return resultList;
        }

        #endregion
        #region Метод SelectWhere(....)

        /// <summary>Метод возвращает готовый список структур, выбирая нужные поля и по заданным условиям</summary>
        /// <param name="fieldsNamesSel">имена полей, которые нужно выбрать. Если указать null, будут выбраны все поля</param>
        /// <param name="fieldsNamesWhere">имена полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет</param>
        /// <param name="fieldsValueWhere">значения полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет. Количество аргументов должно совпадать с кол-вом аргументов в fielsNamesWhere</param>
        /// <param name="sqlLogic">логический оператор, по которому будут складываться фильтрация по именам и значениям фильтрующих полей</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<DebugLogObj> SelectWhere(string[] fieldsNamesSel, string[] fieldsNamesWhere, string[] fieldsValueWhere, SqlLogic sqlLogic = SqlLogic.AND)
        {
            List<DebugLogObj> resultList = new List<DebugLogObj>();
            string sqlLog = EnumsHelper.GetSqlLogic(sqlLogic);
            if (sqlLog == "no result") return resultList;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            #region Формирование строки запроса

            cmd.CommandText = "SELECT ";
            if (fieldsNamesSel == null)
            {
                cmd.CommandText += "* ";
            }
            else
            {
                for (int i = 0; i < fieldsNamesSel.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesSel[i];
                    else cmd.CommandText += ", " + fieldsNamesSel[i];
                }
            }
            cmd.CommandText += " FROM " + _table;
            if (fieldsNamesWhere != null && fieldsNamesWhere != null)
            {
                cmd.CommandText += " WHERE ";
                for (int i = 0; i < fieldsNamesWhere.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    else cmd.CommandText += " " + sqlLog + " " + fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    cmd.Parameters.Add(new SQLiteParameter("@" + fieldsNamesWhere[i], fieldsValueWhere[i]));
                }
            }

            #endregion

            try
            {
                #region Код

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    resultList = null;
                }

                if (resultList != null)
                {
                    DebugLogObj obj;
                    while (reader.Read())
                    {
                        obj = new DebugLogObj();
                        FillOneSel(obj, reader, fieldsNamesSel);
                        resultList.Add(obj);
                    }
                }

                reader.Dispose();
                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch
            {
                #region Код

                resultList = null;

                #endregion
            }

            return resultList;
        }

        #endregion
        #region Метод GetSortedList(.)

        /// <summary>Метод формирует список объектов, отсортированный по дате добавления и отфильтрованный по поисковой строке</summary>
        /// <param name="srchString">поисковая строка</param>
        /// <returns>Возвращает список структур или null в случае какой-либо ошибки</returns>
        public List<DebugLogObj> GetSortedList(string srchString = "")
        {
            List<DebugLogObj> tempList = GetList();
            if (tempList == null) return null;
            tempList = tempList.OrderByDescending(x => x.DateReg).ToList();

            #region Фильтрация по поисковой строке

            List<DebugLogObj> resultList = new List<DebugLogObj>();
            List<DebugLogObj> mainMatchesList = new List<DebugLogObj>();  //совпадения содержимого поля
            List<DebugLogObj> fullMatchesList = new List<DebugLogObj>();  //полные совпадения всего содержимого поля
            srchString = srchString.Trim().ToLower();
            DateTime dt;

            if (srchString != "")
            {
                foreach (DebugLogObj obj in tempList)
                {
                    #region Код

                    #region Код проверки полных совпадений всего содержимого поля

                    dt = new DateTime(obj.DateReg);

                    if (
                            obj.Id.ToString() == srchString ||
                            dt.ToShortDateString() == srchString ||
                            obj.Type.ToLower() == srchString ||
                            obj.ClassName.ToLower() == srchString ||
                            obj.MethodName.ToLower() == srchString ||
                            obj.Text.ToLower() == srchString
                       )
                    {
                        fullMatchesList.Add(obj);
                    }

                    #endregion

                    if (srchString.Contains(" "))
                    {
                        #region Код поиска в случае поискового запроса, состоящего из нескольких слов, разделённых пробелами

                        string[] srchArr = srchString.Split(new[] { ' ' });
                        dt = new DateTime(obj.DateReg);
                        bool checker = false;

                        #region Сначала проверим вхождение целой фразы в полях

                        if (
                            obj.Id.ToString().Contains(srchString) ||
                            dt.ToShortDateString().Contains(srchString) ||
                            obj.Type.ToLower().Contains(srchString) ||
                            obj.ClassName.ToLower().Contains(srchString) ||
                            obj.MethodName.ToLower().Contains(srchString) ||
                            obj.Text.ToLower().Contains(srchString)
                            )
                        {
                            mainMatchesList.Add(obj);
                            checker = true;
                        }

                        #endregion

                        #region Потом проверим отдельно вхождение каждого слова в поисковой строке

                        if (!checker)   //если вхождение поисковой фразы целиком не нашлось, то ищем вхождение слова из фразы
                        {
                            foreach (string srch in srchArr)
                            {
                                if (
                                    obj.Id.ToString().Contains(srch) ||
                                    dt.ToShortDateString().Contains(srch) ||
                                    obj.Type.ToLower().Contains(srch) ||
                                    obj.ClassName.ToLower().Contains(srch) ||
                                    obj.MethodName.ToLower().Contains(srch) ||
                                    obj.Text.ToLower().Contains(srch)
                                    )
                                {
                                    resultList.Add(obj);
                                    break;
                                }
                            }
                        }

                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Код поиска в случае отсутствия пробелов в поисковом запросе

                        dt = new DateTime(obj.DateReg);

                        if (
                            obj.Id.ToString().Contains(srchString) ||
                            dt.ToShortDateString().Contains(srchString) ||
                            obj.Type.ToLower().Contains(srchString) ||
                            obj.ClassName.ToLower().Contains(srchString) ||
                            obj.MethodName.ToLower().Contains(srchString) ||
                            obj.Text.ToLower().Contains(srchString)
                            )
                        {
                            mainMatchesList.Add(obj);
                        }

                        #endregion
                    }

                    #endregion
                }

                #region Выставление точных совпадений по поиску на первое место

                fullMatchesList.AddRange(mainMatchesList);

                if (fullMatchesList.Count > 0)
                {
                    resultList.InsertRange(0, fullMatchesList);
                }

                //Удаление повторяющихся заявок
                resultList = resultList.Distinct().ToList();

                #endregion
            }
            else
            {
                resultList = tempList;
            }

            #endregion

            return resultList;
        }

        #endregion
        #region Метод GetOne(.)

        /// <summary>Метод возвращает объект по его id</summary>
        /// <param name="id">id</param>
        /// <returns>Возвращает null в случае отсутствия объекта с таким id или в случае ошибки</returns>
        public DebugLogObj GetOne(string id)
        {
            DebugLogObj obj = new DebugLogObj();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _table + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    obj = null;
                }
                if (!reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    obj = null;
                }

                if (obj != null)
                {
                    while (reader.Read())
                    {
                        FillOne(obj, reader);
                    }

                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch
            {
                #region Код

                obj = null;

                #endregion
            }

            return obj;
        }

        #endregion
        #region Метод GetListForFields(.)

        /// <summary>Метод возвращает список объектов по одному или нескольким полям</summary>
        /// <param name="fieldNames">имена полей</param>
        /// <param name="fieldValues">значения полей</param>
        /// <returns>Возвращает список только если все значения полей совпали.
        /// Возвращает null в случае какой-либо ошибки</returns>
        public List<DebugLogObj> GetListForFields(string[] fieldNames, string[] fieldValues)
        {
            List<DebugLogObj> resultList = new List<DebugLogObj>();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _table + " WHERE ";
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (i == 0)
                    {
                        cmd.CommandText += fieldNames[i] + "=@Val" + i;
                    }
                    else
                    {
                        cmd.CommandText += " AND " + fieldNames[i] + "=@Val" + i;
                    }
                }
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (fieldNames[i] == "_id" ||
                        fieldNames[i] == "DateReg")
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Val" + i, StringToNum.ParseLong(fieldValues[i])));
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Val" + i, fieldValues[i]));
                    }
                }

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    resultList = null;
                }

                if (resultList != null)
                {
                    DebugLogObj obj;
                    while (reader.Read())
                    {
                        obj = new DebugLogObj();
                        FillOne(obj, reader);
                        resultList.Add(obj);
                    }

                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch
            {
                #region Код

                resultList = null;

                #endregion
            }

            return resultList;
        }

        #endregion
        #region Метод GetNextObjId()

        /// <summary>Метод возвращает следующий id для нового создаваемого Вопроса</summary>
        /// <returns></returns>
        public int GetNextObjId()
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "SELECT MAX(_id) FROM " + _table;

                result = sqlite.ExecuteScalarParams(cmd);
                if (result == -1) //если в таблице нет ни одной структуры, то..
                {
                    result = 1;
                }
                else
                {
                    result += 1;
                }

                sqlite.ConnectionClose();

                #endregion
            }
            catch
            {
                #region Код

                result = -1;

                #endregion
            }
            finally
            {
                #region Код

                cmd.Dispose();

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод UpdateFields(...)

        /// <summary>Метод обновляет значение многих полей в одной строке таблицы</summary>
        /// <param name="id">id объекта</param>
        /// <param name="fieldNames">названия полей</param>
        /// <param name="fieldValues">значения полей</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateFields(string id, string[] fieldNames, string[] fieldValues)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "";
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    cmd.CommandText += "UPDATE " + _table + " SET " + fieldNames[i] + "=@value" + i + " WHERE _id=@id; ";
                }
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    cmd.Parameters.Add(new SQLiteParameter("value" + i, fieldValues[i]));
                }

                result = sqlite.ExecuteNonQueryParams(cmd);
                sqlite.ConnectionClose();

                #endregion
            }
            catch
            {
                #region Код

                result = -1;

                #endregion
            }
            finally
            {
                #region Код

                cmd.Dispose();

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод DeleteOne(.)

        /// <summary>Метод удаляет одну строку из БД по ее id</summary>
        /// <param name="id">id</param>
        /// <returns>Возвращает кол-во удалённых строк или -1 в случае возникновения какой-либо ошибки</returns>
        public int DeleteOne(string id)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "DELETE FROM " + _table + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                result = sqlite.ExecuteNonQueryParams(cmd);
                sqlite.ConnectionClose();

                #endregion
            }
            catch
            {
                #region Код

                result = -1;

                #endregion
            }
            finally
            {
                #region Код

                cmd.Dispose();

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод FillOne(..)

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект данными из SQLiteDataReader</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        private void FillOne(DebugLogObj obj, SQLiteDataReader reader)
        {
            obj.Id = long.Parse(reader["_id"].ToString());
            obj.DateReg = long.Parse(reader["DateReg"].ToString());
            obj.Type = reader["Type"].ToString();
            obj.ClassName = reader["ClassName"].ToString();
            obj.MethodName = reader["MethodName"].ToString();
            obj.Text = reader["Text"].ToString();
        }

        #endregion
        #region Метод FillOneSel(...)

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект данными из SQLiteDataReader</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        /// <param name="fieldsNamesSel">массив имен полей, которые присутствуют в возвращаемом запросе</param>
        private void FillOneSel(DebugLogObj obj, SQLiteDataReader reader, string[] fieldsNamesSel)
        {
            if (fieldsNamesSel.Contains("_id")) obj.Id = long.Parse(reader["_id"].ToString());
            if (fieldsNamesSel.Contains("DateReg")) obj.DateReg = long.Parse(reader["DateReg"].ToString());
            if (fieldsNamesSel.Contains("Type")) obj.Type = reader["Type"].ToString();
            if (fieldsNamesSel.Contains("ClassName")) obj.Type = reader["ClassName"].ToString();
            if (fieldsNamesSel.Contains("MethodName")) obj.Type = reader["MethodName"].ToString();
            if (fieldsNamesSel.Contains("Text")) obj.Type = reader["Text"].ToString();
        }

        #endregion
        #region Метод GetDbSize()

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

        #endregion
        #region Метод DropTable(.)

        /// <summary>Метод удаляет таблицу и выполняет команду VACUUM</summary>
        public void DropTable()
        {
            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "";
            sqlString = "BEGIN; DROP TABLE IF EXISTS " + _table + "; COMMIT; VACUUM;";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();
        }

        #endregion
        #region Метод VacuumDb(.)

        /// <summary>Метод выполняет команду VACUUM, тем самым "дефрагментирует" БД</summary>
        public void VacuumDb()
        {
            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "";
            sqlString = "VACUUM;";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();
        }

        #endregion

        #endregion
    }

    #endregion

    #endregion

    #region Классы с описанием структур данных

    #region Класс DebugLogObj

    /// <summary>Класс представляет структуру данных одного События ошибки</summary>
    class DebugLogObj
    {
        #region Поля

        private long id = -1;                           // идентификатор (присваивается в БД)
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private long dateReg = 0;                       // дата регистрации события ошибки
        public long DateReg
        {
            get { return dateReg; }
            set { dateReg = value; }
        }

        private string type = "";                       // строковый код вида события ошибки
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string className = "";                  // имя класса, в котором произошла ошибка
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        private string methodName = "";                 // имя метода, в котором произошла ошибка
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        private string text = "";                       // текст события ошибки
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        #endregion

        #region Методы



        #endregion
    }

    #endregion
    #region Перечисление с видами событий ошибок

    enum ErrorEvents { info, warn, err }

    /// <summary>Вспомогательный класс для различных преобразований со значениями перечисления ErrorEvents</summary>
    class ErrorEventsHelper
    {
        #region Метод TypeToCode(.)

        /// <summary>Метод преобразует значение перечисления в соответствующее ему строковое значение (строковый код)</summary>
        /// <param name="type">вид события ошибки</param>
        /// <returns></returns>
        public static string TypeToCode(ErrorEvents type)
        {
            string result = "";

            switch (type)
            {
                case ErrorEvents.info:
                    result = "info";
                    break;
                case ErrorEvents.warn:
                    result = "warn";
                    break;
                case ErrorEvents.err:
                    result = "err";
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion
        #region Метод CodeToType(.)

        /// <summary>Метод преобразует строковое значение (строковый код) в соответствующее ему значение перечисления</summary>
        /// <param name="type">вид события ошибки</param>
        /// <returns></returns>
        public static ErrorEvents CodeToType(string code)
        {
            ErrorEvents result = ErrorEvents.info;

            switch (code)
            {
                case "info":
                    result = ErrorEvents.info;
                    break;
                case "warn":
                    result = ErrorEvents.warn;
                    break;
                case "err":
                    result = ErrorEvents.err;
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion
    }

    #endregion

    #endregion
}