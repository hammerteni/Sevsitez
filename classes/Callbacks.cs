//Данный файл содержит код для работы с ОТЗЫВАМИ посетителей. Файл данных находится по пути /files/callbacks/callbacks_*
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classesHelp;

namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>класс содержит функции и методы формирования готового HTML-кода для просмотра и редактирования отзывов.
    /// JavaScript для этого класса находится в файле - callbacksAdm.js и callbacks.js. Стили для JavaScript в файле - callbacksendAdm.css, callbacksend.css. 
    /// Стили для класса - callbacks.css</summary>
    public class CallbacksForm
    {
        Page _pag;

        public CallbacksForm(Page pag)
        {
            _pag = pag;
        }

        #region Функции для сайта

        /// <summary>Функция возвращает таблицу с отзывами для редактирования. Так же здесь добавляются кнопки перехода между страницами вывода</summary>
        /// <returns></returns>
        public Panel GetCallbacksPage()
        {
            int countOfElemInOnePage = 30;  //кол-во отзывов на одной странице
            CallbacksWork cbWork = new CallbacksWork();
            List<CallbackStruct> callbackList = cbWork.GetAllCallbacksStructs();

            Panel wrapPanel = new Panel(); wrapPanel.CssClass = "cb_divMainWrap";
            wrapPanel.Controls.Add(new LiteralControl("<h1 class='cb_mainTitle'>Гостевая книга с отзывами</h1>"));

            bool checker = false;
            foreach (CallbackStruct callbackStruct in callbackList)
            {
                if (callbackStruct.Public == "1")
                {
                    checker = true; break;
                }
            }

            //добавляем таблицу с кнопками сортировки
            if (callbackList.Count > 0 && checker)  //если список хоть что-то содержит и хоть что-то из этого опубликовано, то..
            {
                //добавим на страницу кнопки-ссылки на страницы продуктов
                PagePanelClass pageBtns = new PagePanelClass(callbackList.Count, countOfElemInOnePage);
                wrapPanel.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

                int pageCounter = 1;
                int prodCounter = 0;

                //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                foreach (CallbackStruct oneStruct in callbackList)
                {
                    prodCounter++;
                    if (prodCounter > countOfElemInOnePage)
                    {
                        prodCounter = 1;
                        pageCounter++;
                    }

                    if (pageCounter == (int)HttpContext.Current.Session["pagenum"])           //если эта та самая страница продуктов, которую нужно вывести, то выводим её
                    {
                        wrapPanel.Controls.Add(GetOneCallbackPanel(oneStruct));
                    }
                    else if (pageCounter > (int)HttpContext.Current.Session["pagenum"])
                    {
                        break;
                    }
                }

                //добавим на страницу кнопки-ссылки на страницы продуктов
                wrapPanel.Controls.Add(pageBtns.GetPageBtnsTbl("1"));
            }
            else
            {
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_noCallbacks'>Отзывы пока отсутствуют..</span>"));
            }

            wrapPanel.Controls.Add(new LiteralControl("<div class='cb_btnSendCallback' onclick='addNewGuestCallback();'>НАПИСАТЬ ОТЗЫВ</div>"));

            return wrapPanel;
        }

        /// <summary>вспомогательная функция возвращает панель со списком последних 30 отзывов посетителей</summary>
        /// <returns></returns>
        private Panel GetOneCallbackPanel(CallbackStruct oneStruct)
        {
            var wrapPanel = new Panel(); wrapPanel.CssClass = "cb_divMainOne";

            //Panel panel;
            DateTime dt;

            if (oneStruct.Role == "посетитель" && oneStruct.Public == "1")              //если нужно добавить отзыв посетителя, то..    
            {
                //текст отзыва
                wrapPanel.Controls.Add(new LiteralControl("<div class='cb_divUserTxt'>" + oneStruct.Text + "</div>"));
                //данные по отзыву (автор, дата и время)
                wrapPanel.Controls.Add(new LiteralControl("<div class='cb_divUserData'>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanPre'>автор:</span>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanData'>" + oneStruct.Name + "</span>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanPre'>дата:</span>"));
                dt = new DateTime(oneStruct.DateTimeTicks);
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanData'>" + dt.ToString("dd.MM.yyyy hh:mm") + "</span>"));
                wrapPanel.Controls.Add(new LiteralControl("</div>"));
            }
            else if (oneStruct.Role == "администратор" && oneStruct.Public == "1")      //если нужно добавить комментарий администратора, то..
            {
                //текст отзыва
                wrapPanel.Controls.Add(new LiteralControl("<div class='cb_divAdmTxt'>" + oneStruct.Text + "</div>"));
                //данные по отзыву (автор, дата и время)
                wrapPanel.Controls.Add(new LiteralControl("<div class='cb_divUserData'>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanPre'>автор:</span>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanData'>" + oneStruct.Name + "</span>"));
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanPre'>дата:</span>"));
                dt = new DateTime(oneStruct.DateTimeTicks);
                wrapPanel.Controls.Add(new LiteralControl("<span class='cb_spanData'>" + dt.ToString("dd-MM-yyyy hh:mm") + "</span>"));
                wrapPanel.Controls.Add(new LiteralControl("</div>"));
            }


            return wrapPanel;
        }

        #endregion

        #region Функции для консоли управления

        /// <summary>Метод возвращает панель с добавлением нового отзыва администратора или гостя</summary>
        /// <returns></returns>
        public Panel GetCallbackControlPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            panelWrapper.Controls.Add(new LiteralControl("<span class='lblSectionTitle'>ПАНЕЛЬ РЕДАКТИРОВАНИЯ ГОСТЕВОЙ КНИГИ С ОТЗЫВАМИ</span>"));

            //Кнопка добавления нового отзыва гостя
            panelWrapper.Controls.Add(new LiteralControl("<a class='buttonsHover lBtnsUniverse' title='добавить новый отзыв от имени посетителя сайта'" +
                                                            " href='' onclick='addNewGuestCallback(); return false;'> + отзыв посетителя </a>"));
            //Кнопка добавления нового отзыва или сообщения модератора
            panelWrapper.Controls.Add(new LiteralControl("<a class='buttonsHover lBtnsUniverse' title='добавить новый отзыв от имени модератора сайта'" +
                                                            " href='' onclick='addNewModerCallback(); return false;'> + отзыв модератора </a>"));

            return panelWrapper;
        }

        /// <summary>Функция возвращает таблицу с отзывами для редактирования. Так же здесь добавляются кнопки перехода между страницами вывода</summary>
        /// <returns></returns>
        public Table GetCallbacksEditTable()
        {
            int countOfElemInOnePage = 30;  //кол-во отзывов в таблице показываемые единовременно (на одной странице)
            CallbacksWork cbWork = new CallbacksWork();
            List<CallbackStruct> callbackList = cbWork.GetAllCallbacksStructs();

            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell(); var lbl = new Label();

            //добавляем таблицу с кнопками сортировки
            if (callbackList.Count > 0)  //если список хоть что-то содержит, то..
            {
                //добавим на страницу кнопки-ссылки на страницы продуктов
                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                var pageBtns = new PagePanelClass(callbackList.Count, countOfElemInOnePage);
                tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

                int pageCounter = 1;
                int prodCounter = 0;

                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

                //добавляем подтаблицу
                var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);
                //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ////////////////////

                var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
                var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Роль"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Имя"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Публикация"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Ответить"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Текст"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Сохранить"; tblCell1.Controls.Add(lbl);
                tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Удалить"; tblCell1.Controls.Add(lbl);

                //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                foreach (CallbackStruct oneStruct in callbackList)
                {
                    prodCounter++;
                    if (prodCounter > countOfElemInOnePage)
                    {
                        prodCounter = 1;
                        pageCounter++;
                    }

                    if (pageCounter == (int)HttpContext.Current.Session["pagenum"])           //если эта та самая страница продуктов, которую нужно вывести, то выводим её
                    {
                        tbl1.Controls.Add(GetTblRowWithOneCallback(oneStruct));
                    }
                    else if (pageCounter > (int)HttpContext.Current.Session["pagenum"])
                    {
                        break;
                    }
                }

                //добавим на страницу кнопки-ссылки на страницы продуктов
                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("1"));
            }
            else
            {
                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
                lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Отзывы посетителей для сайта отсутствуют.."; tblCell.Controls.Add(lbl);
            }

            return tbl;
        }

        /// <summary>функция возвращает строку с одним отзывом для функции GetCallbacksEditTable</summary>
        /// <returns></returns>
        private TableRow GetTblRowWithOneCallback(CallbackStruct oneStruct)
        {
            TableRow tblRow; TableCell tblCell; LinkButton lBtn; Label lbl;
            tblRow = new TableRow();

            //ДАТА
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            DateTime dt = new DateTime(oneStruct.DateTimeTicks);
            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = dt.ToString("dd-MM-yyyy hh:mm"); tblCell.Controls.Add(lbl);

            //РОЛЬ
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Role; tblCell.Controls.Add(lbl);

            //ИМЯ
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Name; tblCell.Controls.Add(lbl);

            //ПУБЛИКАЦИЯ
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lBtn = new LinkButton(); lBtn.CssClass = "lBtnsUniverse lBtnHover";
            lBtn.ToolTip = "опубликовать отзыв на сайте или скрыть его"; lBtn.ID = "lBtnPub_" + oneStruct.Id;
            lBtn.CommandArgument = oneStruct.Id.ToString(); lBtn.Command += new CommandEventHandler(lBtnPublic_Command);
            if (oneStruct.Public == "0")        //если отзыв не опубликован, то..
            {
                lBtn.Text = "опубликовать";
            }
            else if (oneStruct.Public == "1")   //если опубликован, то..
            {
                lBtn.Text = "скрыть";
            }
            tblCell.Controls.Add(lBtn);

            //ОТВЕТИТЬ
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lBtn = new LinkButton(); lBtn.CssClass = "lBtnsUniverse lBtnHover";
            lBtn.ToolTip = "добавить комментарий на этот отзыв или на комментарий"; lBtn.ID = "lBtnReply_" + oneStruct.Id;
            lBtn.Attributes.Add("onclick", "replyToCallback(" + oneStruct.Id + "); return false;");
            lBtn.Text = "+ комментарий";
            tblCell.Controls.Add(lBtn);

            //ТЕКСТ отзыва, либо комментария администратора
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 300; txtBox.Height = 40; txtBox.Text = oneStruct.Text;
            txtBox.TextMode = TextBoxMode.MultiLine; txtBox.ID = "txtBoxCallbackTxt_" + oneStruct.Id; tblCell.Controls.Add(txtBox);

            //СОХРАНИТЬ изменения в тексте отзыва либо комментария администратора
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lBtn = new LinkButton(); lBtn.CssClass = "lBtnsUniverse lBtnHover"; lBtn.Text = "сохранить";
            lBtn.ToolTip = "сохранить текст отзыва, если вы его изменили"; lBtn.CommandArgument = oneStruct.Id.ToString();
            lBtn.Command += new CommandEventHandler(lBtnSave_Command); tblCell.Controls.Add(lBtn);

            //УДАЛИТЬ
            tblCell = new TableCell(); tblCell.CssClass = "tblSubCell"; tblRow.Controls.Add(tblCell);
            lBtn = new LinkButton(); lBtn.CssClass = "lBtnsUniverse lBtnHover"; lBtn.Text = "удалить";
            lBtn.ToolTip = "удалить отзыв из базы данных полностью"; lBtn.CommandArgument = oneStruct.Id.ToString();
            lBtn.OnClientClick = "return confirm('Отзыв будет полностью удалён из базы данных. Удалить?');";
            lBtn.Command += new CommandEventHandler(lBtnDelete_Command); tblCell.Controls.Add(lBtn);

            return tblRow;
        }

        #region События для функции GetTblRowWithOneCallback(CallbacksStruct oneStruct)

        /// <summary>кнопка публикации отзыва на сайте</summary>
        /// <param name="sender"></param>
        /// <param name="e">ownerId</param>
        protected void lBtnPublic_Command(object sender, CommandEventArgs e)
        {
            string callbackId = e.CommandArgument.ToString();
            string lBtnText = ((LinkButton)sender).Text;
            if (lBtnText == "опубликовать")     //если нужно опубликовать отзыв на сайте, то..
            {
                var warning = new WarnClass();
                var callbacksWork = new CallbacksWork();
                var callbackStruct = callbacksWork.GetCallbackStruct(callbackId);
                callbackStruct.Public = "1";
                if (!callbacksWork.ReplaceCallback(callbackStruct))
                { warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить изменения в БД. Попробуйте повторить..", _pag.Master); return; }
                else { warning.HideWarning(_pag.Master); }
                ((LinkButton)_pag.FindControl("ctl00$ContentPlaceHolder1$lBtnPub_" + callbackId)).Text = "скрыть";
            }
            else if (lBtnText == "скрыть")      //если нужно скрыть отзыв с сайта, то..
            {
                var warning = new WarnClass();
                var callbacksWork = new CallbacksWork();
                var callbackStruct = callbacksWork.GetCallbackStruct(callbackId);
                callbackStruct.Public = "0";
                if (!callbacksWork.ReplaceCallback(callbackStruct))
                { warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить изменения в БД. Попробуйте повторить..", _pag.Master); return; }
                else { warning.HideWarning(_pag.Master); }
                ((LinkButton)_pag.FindControl("ctl00$ContentPlaceHolder1$lBtnPub_" + callbackId)).Text = "опубликовать";
            }
        }

        /// <summary>кнопка сохранения текста отзыва</summary>
        /// <param name="sender"></param>
        /// <param name="e">ownerId</param>
        protected void lBtnSave_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            string callbackId = e.CommandArgument.ToString();
            string callbackText = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxCallbackTxt_" + callbackId)).Text;
            if (callbackText.Trim() == "")
            { warning.ShowWarning("ВНИМАНИЕ. Текст отзыву не должен быть пустым. Добавьте текст в поле, находящееся в столбце 'Текст'", _pag.Master); return; }
            else { warning.HideWarning(_pag.Master); }

            var callbacksWork = new CallbacksWork();
            var callbackStruct = callbacksWork.GetCallbackStruct(callbackId);
            callbackStruct.Text = callbackText;
            if (!callbacksWork.ReplaceCallback(callbackStruct))
            { warning.ShowWarning("ВНИМАНИЕ. Не удалось сохранить изменения в БД. Попробуйте повторить..", _pag.Master); }
            else { warning.HideWarning(_pag.Master); }
        }

        /// <summary>кнопка полного удаления отзыва из БД</summary>
        /// <param name="sender"></param>
        /// <param name="e">ownerId</param>
        protected void lBtnDelete_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            string callbackId = e.CommandArgument.ToString();

            var callbacksWork = new CallbacksWork();
            var callbackStruct = callbacksWork.GetCallbackStruct(callbackId);
            if (!callbacksWork.DeleteCallback(callbackStruct))
            { warning.ShowWarning("ВНИМАНИЕ. Не получилось удалить отзыв из БД. Попробуйте повторить..", _pag.Master); return; }
            else { warning.HideWarning(_pag.Master); }

            //перезагрузка страницы со всеми параметрами
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>класс с функциями и методами для работы файлами данных по новостям (папка files\info\)</summary>
    public class CallbacksWork
    {
        #region ПОЛЯ КЛАССА

        private HttpContext _context;
        private FileStream _fs;                 //переменная используется для блокировки доступа к файлу, который перезаписывается
        private string _folderPath;
        private string _filePath;
        private string _tmpFolder;
        private bool _checkFolderAndPathExist;  // Переменная будет содержать false, если при инициализации класса возникнет ошибка при создании папки или файла конфигурации
        private int _callbackPosition = 0;      //переменная получает значение в функции GetAllCallbacksStructs(...), а используется в функции ReplaceDeleteOrSaveNewCallback(...)
                                                //Она содержит позицию исключённого в функции GetAllCallbacksStructs(...) отзыва.
        private string _callBackFilePath;       //переменная получает значение в функции GetAllCallbacksStructs(...), а используется в функции ReplaceDeleteOrSaveNewCallback(...)
                                                //Она содержит позицию исключённого в функции GetAllCallbacksStructs(...) отзыва.
        private string _fileName;

        #endregion

        /// <summary>Конструктор. При его вызове происходит проверка на наличие и создание при необходимости первого файла бд (callbacks_1) по пути - ~\files\callbacks</summary>
        public CallbacksWork()
        {
            // Инициализируем нужные поля класса
            _context = HttpContext.Current;
            _checkFolderAndPathExist = true;
            _fileName = "callbacks";
            _folderPath = _context.Server.MapPath("~") + @"files\callbacks";
            _filePath = _folderPath + @"\" + _fileName + "_1";
            _tmpFolder = _context.Server.MapPath("~") + @"files\temp";

            #region Проверяем наличие папки и файла и если их нет, то создаём их

            if (!Directory.Exists(_folderPath))
            {
                try
                {
                    Directory.CreateDirectory(_folderPath);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            if (!Directory.Exists(_tmpFolder))
            {
                try
                {
                    Directory.CreateDirectory(_tmpFolder);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            string[] pathsToDBfilesArray = Directory.GetFiles(_folderPath, _fileName + "_*", SearchOption.TopDirectoryOnly);
            if (pathsToDBfilesArray.Length == 0)
            {
                try
                {
                    FileStream fs = File.Create(_filePath);
                    fs.Close(); fs.Dispose();
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }

            #endregion
        }

        /// <summary>Метод возвращает список всех структур отзывов в файле БД отзывов, кроме отзыва с id переданного в качестве аргумента. 
        /// Если аргумент не задан, то возвращаются все отзывы. Если при инициализации класса возникла ошибка то возвращается null</summary>
        /// <param name="idToExclude">id отзыва, структуру которого нужно исключить</param>
        /// <returns></returns>
        public List<CallbackStruct> GetAllCallbacksStructs(string idToExclude = "")
        {
            var oneStruct = new CallbackStruct();
            var structList = new List<CallbackStruct>();
            string[] str, strSplit;
            bool switcherBegin = false;
            bool switcherExclude = false;

            if (!_checkFolderAndPathExist) return null;   //если при инициализации класса возникла ошибка то возвращаем null

            #region Код

            string[] pathsToDBfilesArray = Directory.GetFiles(_folderPath, _fileName + "_*", SearchOption.TopDirectoryOnly);

            int counter = 0;
            foreach (string filePath in pathsToDBfilesArray) //перебираем каждый файл БД товаров по очереди
            {
                counter = 0;
                str = File.ReadAllLines(filePath, Encoding.Default);
                foreach (string line in str) //перебираем содержимое файла, выгруженное в массив str
                {
                    strSplit = line.Split(new[] { '|' });
                    if (strSplit.Length > 1) //проверка-подстраховка того, что строка содержит полезную инфу
                    {
                        if (strSplit[0] == "begin")
                        {
                            switcherBegin = true;
                            switcherExclude = false;
                            oneStruct = new CallbackStruct();
                        }
                        else if (strSplit[0] == "end")
                        {
                            switcherBegin = false;
                            //если отзыв не нужно исключать из возвращаемого списка, то заполняем список
                            if (!switcherExclude)
                            {
                                structList.Add(oneStruct);
                            }
                            else
                            {
                                _callbackPosition = counter;
                                _callBackFilePath = filePath;
                            }
                            counter++;
                        }

                        if (switcherBegin)
                        {
                            switch (strSplit[0])
                            {
                                case "id":
                                    if (strSplit[1] == idToExclude)
                                    {
                                        switcherExclude = true;
                                        switcherBegin = false;
                                    }
                                    oneStruct.Id = int.Parse(strSplit[1]);
                                    break;

                                case "роль":
                                    oneStruct.Role = strSplit[1];
                                    break;

                                case "дата":
                                    try { oneStruct.DateTimeTicks = long.Parse(strSplit[1]); }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }

                                    break;

                                case "кто":
                                    oneStruct.Name = strSplit[1];
                                    break;

                                case "опубликован":
                                    oneStruct.Public = strSplit[1];
                                    break;

                                case "текст":
                                    oneStruct.Text = strSplit[1];
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            #endregion

            return structList;
        }

        /// <summary>Метод возвращает список всех структур отзывов из файла отзывов, который содержит отзыв с передаваемым ID
        /// Если аргумент не задан, то возвращаются все отзывы. Если при инициализации класса возникла ошибка то возвращается null</summary>
        /// <param name="callbackId">id отзыва</param>
        /// <param name="excludeId">исключить из возвращаемого списка отзыв с callbackId ? true - исключить, false - не исключать</param>
        /// <returns></returns>
        private List<CallbackStruct> GetCallbackStructsFromNeedFile(string callbackId, bool excludeId)
        {
            var oneStruct = new CallbackStruct();
            var structList = new List<CallbackStruct>();
            string[] str, strSplit;
            bool switcherBegin = false;
            bool switcherExclude = false;

            if (!_checkFolderAndPathExist) return null;   //если при инициализации класса возникла ошибка то возвращаем null

            #region Код

            if (GetAllCallbacksStructs(callbackId) == null)     //вызываем эту функция, чтобы определился путь к файлу БД отзывов, в котором лежит отзыв
            {
                return null;
            }
            int counter = 0;
            str = File.ReadAllLines(_callBackFilePath, Encoding.Default);
            foreach (string line in str) //перебираем содержимое файла, выгруженное в массив str
            {
                strSplit = line.Split(new[] { '|' });
                if (strSplit.Length > 1) //проверка-подстраховка того, что строка содержит полезную инфу
                {
                    if (strSplit[0] == "begin")
                    {
                        switcherBegin = true;
                        switcherExclude = false;
                        oneStruct = new CallbackStruct();
                    }
                    else if (strSplit[0] == "end")
                    {
                        switcherBegin = false;
                        //если отзыв не нужно исключать из возвращаемого списка, то заполняем список
                        if (!switcherExclude)
                        {
                            structList.Add(oneStruct);
                        }
                        counter++;
                    }

                    if (switcherBegin)
                    {
                        switch (strSplit[0])
                        {
                            case "id":
                                if (strSplit[1] == callbackId && excludeId)
                                {
                                    switcherExclude = true;
                                    switcherBegin = false;
                                    _callbackPosition = counter;
                                }
                                else if (strSplit[1] == callbackId && !excludeId)
                                {
                                    _callbackPosition = counter;
                                }
                                oneStruct.Id = int.Parse(strSplit[1]);
                                break;

                            case "роль":
                                oneStruct.Role = strSplit[1];
                                break;

                            case "дата":
                                try { oneStruct.DateTimeTicks = long.Parse(strSplit[1]); }
                                catch (Exception ex)
                                {
                                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                }
                                break;

                            case "кто":
                                oneStruct.Name = strSplit[1];
                                break;

                            case "опубликован":
                                oneStruct.Public = strSplit[1];
                                break;

                            case "текст":
                                oneStruct.Text = strSplit[1];
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            #endregion

            return structList;
        }

        /// <summary>Метод возвращает обычный или инвертированный список структур отзывов. Сортировка по дате.
        /// При инвертированном - в обратном. Возвращает null при ошибке инициализации класса</summary>
        /// <param name="invertion">перевернуть(инвертировать) список - true или нет - false</param>
        /// <returns></returns>
        public List<CallbackStruct> GetSortedCallbacksStructs(bool invertion)
        {
            List<CallbackStruct> allStructs = GetAllCallbacksStructs();
            if (allStructs == null) return null;
            List<CallbackStruct> structList =
                (invertion) ? allStructs.OrderBy(a => a.DateTimeTicks * -1).ToList() : allStructs.OrderBy(a => a.DateTimeTicks).ToList();

            return structList;
        }

        /// <summary>Метод возвращает одну структуру отзыва по id переданного в качестве аргумента. Если такого отзыва не существует в БД или была ошибка при инициализации класса, то возвращается null</summary>
        /// <param name="callbackId">id отзыва, структуру которого нужно вернуть</param>
        /// <returns></returns>
        public CallbackStruct GetCallbackStruct(string callbackId)
        {
            List<CallbackStruct> structList = GetAllCallbacksStructs();
            if (structList == null) return null;
            CallbackStruct oneStruct = new CallbackStruct();
            if (structList.Exists(x => x.Id.ToString() == callbackId))      //если отзыв с переданным ID присутствует в списке, то..
            {
                oneStruct = structList.Find(x => x.Id.ToString() == callbackId);
            }
            else { return null; }

            return oneStruct;
        }

        /// <summary>Метод добавления нового отзыва в БД. Возвращает false в случае ошибки во время добавления</summary>
        /// <param name="structNew">структура данных отзыва</param>
        /// <returns></returns>
        public bool AddNewCallback(CallbackStruct structNew)
        {
            string dbPath = GetNeedDbFilePath(_folderPath, _fileName);
            structNew.Id = GetIdForNewCallback();
            try
            {
                var sr = new StreamWriter(dbPath, true, Encoding.Default);
                var structToAdd = structNew.GetListFromStruct();
                foreach (string line in structToAdd)
                {
                    sr.WriteLine(line);
                }
                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>Метод замены данных об отзыве в БД. Возвращает false в случае ошибки во время изменения данных</summary>
        /// <param name="structNew">структура данных отзыва</param>
        /// <returns></returns>
        public bool ReplaceCallback(CallbackStruct structNew)
        {
            var newListOfStructs = GetCallbackStructsFromNeedFile(structNew.Id.ToString(), true);  //получаем список структур отзывов, содержащихся в нужном файле БД, за исключение структуры отзыва, которую мы собираемся заменить
            if (newListOfStructs == null) return false;

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла   
            try
            {
                _fs = new FileStream(_callBackFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            //При замене добавляем новую структуру отзыва в список структур на тоже место
            if (newListOfStructs.Count == 0) { newListOfStructs.Add(structNew); }
            else
            {
                if (_callbackPosition == newListOfStructs.Count)
                { newListOfStructs.Add(structNew); }
                else
                { newListOfStructs.Insert(_callbackPosition, structNew); }
            }

            //получаем финальный строковый список для перезаписи файла БД
            var listToRaplace = new List<string>();
            foreach (CallbackStruct oneStruct in newListOfStructs)
            {
                listToRaplace.AddRange(oneStruct.GetListFromStruct());
            }

            //КОД ЗАМЕНЫ ФАЙЛА БД новым содержимым из списка listToRaplace с учётом изменений в отзыве
            var rn = new Random();
            int num = rn.Next(1, 666);
            string tempFileName = "callbackstructs_" + num;
            try
            {
                File.WriteAllLines(_tmpFolder + @"\" + tempFileName, listToRaplace, Encoding.Default);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                _fs.Close();
                _fs.Dispose();
                File.Copy(_tmpFolder + @"\" + tempFileName, _callBackFilePath, true);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                File.Delete(_tmpFolder + @"\" + tempFileName);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>Метод удаления данных об отзыве из БД. Возвращает false в случае ошибки во время удаления данных</summary>
        /// <param name="structNew">структура данных отзыва</param>
        /// <returns></returns>
        public bool DeleteCallback(CallbackStruct structNew)
        {
            var newListOfStructs = GetCallbackStructsFromNeedFile(structNew.Id.ToString(), true);  //получаем список структур отзывов, содержащихся в нужном файле БД, за исключением структуры отзыва, которую мы собираемся удалить
            if (newListOfStructs == null) return false;

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла   
            try
            {
                _fs = new FileStream(_callBackFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            //получаем финальный строковый список для перезаписи файла БД
            var listToRaplace = new List<string>();
            foreach (CallbackStruct oneStruct in newListOfStructs)
            {
                listToRaplace.AddRange(oneStruct.GetListFromStruct());
            }

            //КОД ЗАМЕНЫ ФАЙЛА БД новым содержимым из списка listToRaplace с учётом изменений в отзыве
            var rn = new Random();
            int num = rn.Next(1, 666);
            string tempFileName = "callbackstructs_" + num;
            try
            {
                File.WriteAllLines(_tmpFolder + @"\" + tempFileName, listToRaplace, Encoding.Default);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                _fs.Close();
                _fs.Dispose();
                File.Copy(_tmpFolder + @"\" + tempFileName, _callBackFilePath, true);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                File.Delete(_tmpFolder + @"\" + tempFileName);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>Метод добавляет комментарий на отзыв в БД сразу же после отзыва. Возвращает false в случае возникновения ошибки.</summary>
        /// <param name="callbackId">ID отзыва, к которому нужно добавить комментарий</param>
        /// <param name="commentStruct">структура данных комментария к отзыву</param>
        /// <returns></returns>
        public bool AddCommentForCallback(string callbackId, CallbackStruct commentStruct)
        {
            var newListOfStructs = GetCallbackStructsFromNeedFile(callbackId, false);  //получаем список структур всех отзывов, содержащихся в нужном файле БД
            if (newListOfStructs == null) return false;

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла   
            try
            {
                _fs = new FileStream(_callBackFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            commentStruct.Id = GetIdForNewCallback();

            //добавляем комментарий администратора в список на место, следующее за отзывом, для которого предназначен этот комментарий
            if (newListOfStructs.Count == 0) { newListOfStructs.Add(commentStruct); }
            else
            {
                if (_callbackPosition + 1 == newListOfStructs.Count) { newListOfStructs.Add(commentStruct); }
                else { newListOfStructs.Insert(_callbackPosition + 1, commentStruct); }
            }

            //получаем финальный строковый список для перезаписи файла БД
            var listToRaplace = new List<string>();
            foreach (CallbackStruct oneStruct in newListOfStructs)
            {
                listToRaplace.AddRange(oneStruct.GetListFromStruct());
            }

            //КОД ЗАМЕНЫ ФАЙЛА БД новым содержимым из списка listToRaplace с учётом изменений в отзыве
            var rn = new Random();
            int num = rn.Next(1, 666);
            string tempFileName = "callbackstructs_" + num;
            try
            {
                File.WriteAllLines(_tmpFolder + @"\" + tempFileName, listToRaplace, Encoding.Default);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                _fs.Close();
                _fs.Dispose();
                File.Copy(_tmpFolder + @"\" + tempFileName, _callBackFilePath, true);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            try
            {
                File.Delete(_tmpFolder + @"\" + tempFileName);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>функция возвращает Id для новой добавляемой общности</summary>
        /// <returns></returns>
        public int GetIdForNewCallback()
        {
            int newId = 1;
            var listOfStructs = GetAllCallbacksStructs();
            if (listOfStructs.Count > 0)
            {
                listOfStructs.Sort((a, b) => a.Id.CompareTo(b.Id)); //сортировка от 'А до Я' по полю Id
                newId = listOfStructs[listOfStructs.Count - 1].Id + 1;
            }

            return newId;
        }

        /// <summary>Метод возвращает путь к последнему файлу БД в который нужно добавлять структуру данных</summary>
        /// <param name="pathToFolder">принимает путь к папке с файлами БД вида 'context.Server.MapPath("~") + @"files\callbacks"'</param>
        /// <param name="fileName">принимает часть имени файла БД вида 'callbacks' при полном имени, к примеру - 'callbacks_1'</param>
        /// <returns></returns>
        public string GetNeedDbFilePath(string pathToFolder, string fileName)
        {
            string pathToNeedFile = ""; string[] strSplit, strSplit1;
            string[] pathToDBfiles = Directory.GetFiles(pathToFolder, fileName + "_*", SearchOption.TopDirectoryOnly);
            if (pathToDBfiles.Length == 0)      //если файла журнала ещё не существует, то записываем в pathToNeedFile полный путь к новому(первому) файлу журнала
            {
                pathToNeedFile = pathToFolder + @"\" + fileName + "_1";
            }
            else
            {
                FileNameForSort fNameForSort, fNameLastDB;
                var fnamesForSort = new List<FileNameForSort>();
                foreach (string onepath in pathToDBfiles)
                {
                    //вычленяем имя(без пути) файла БД
                    strSplit = onepath.Split(new Char[] { '\\' });
                    strSplit1 = strSplit[strSplit.Length - 1].Split(new Char[] { '_' });
                    fNameForSort = new FileNameForSort(); fNameForSort.LeftPartOfFileName = strSplit1[0]; fNameForSort.RightPartOfFileName = int.Parse(strSplit1[1]);
                    fnamesForSort.Add(fNameForSort);
                }
                fnamesForSort.Sort((a, b) => a.RightPartOfFileName.CompareTo(b.RightPartOfFileName));  //сортировка "от А до Я"(образно) по свойству rightPartOfFileName класса fileNameForSort
                fNameLastDB = fnamesForSort[fnamesForSort.Count - 1];
                var fi = new FileInfo(pathToFolder + @"\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName);
                if (fi.Length > 500000)    //если размер последнего файла БД больше максимально допустимого, то задаём путь к новой файлу БД со следующим порядковым номером
                { pathToNeedFile = pathToFolder + @"\" + fNameLastDB.LeftPartOfFileName + "_" + (fNameLastDB.RightPartOfFileName + 1); }
                else { pathToNeedFile = pathToFolder + @"\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName; }
            }

            return pathToNeedFile;
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>класс инициализиции объекта одной новости</summary>
    public class CallbackStruct
    {
        public string Role { get; set; }                        //роль оставившего отзыв (может быть 'посетитель' или 'администратор')
        public int Id { get; set; }                             //уникальный идентификатор отзыва
        public long DateTimeTicks { get; set; }                 //метка времени с датой регистрации отзыва в базе сайта (хранится в св-ве в виде Convert.ToDateTime(string).Ticks). Обратное преобразование в дату для отображения - DateTime dt = new DateTime(long ticks);
        public string Name { get; set; }                        //имя посетителя или ник администратора, ответившего на отзыв
        public string Public { get; set; }                      //опубликован отзыв или нет ('1' или '0'). Влияет на то, будет ли он отображаться на сайте на странице отзывов или нет.
        public string Text { get; set; }                        //содержит текст отзыва или ответа администратора


        /// <summary>функция возвращает список в формате List/string/ из данной структуры отзыва. 
        /// Строки в списке полностью подготовлены для записи в файл БД отзывов. 
        /// Зашифрованы нужные данные и добавлены тэги начала и конца</summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var list = new List<string>();

            list.Add("begin|");

            list.Add("id|" + Id);
            list.Add("роль|" + Role);
            list.Add("дата|" + DateTimeTicks);
            list.Add("кто|" + Name);
            list.Add("опубликован|" + Public);
            list.Add("текст|" + Text);

            list.Add("end|");

            return list;
        }
    }

    #endregion
}