// Файл с классами для работы с ФАЙЛАМИ ДЛЯ ОБЩЕГО ДОСТУПА. Файлы располагаются так:
// ~/files/shared/; 
// База данных - 
// ~/files/sqlitedb/sharedfiles.db

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
    public class SharedFilesForm
    {
        #region Поля класса

        private HttpContext _context;
        private Page _pag;
        private int _countOfElemInOnePage;

        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        private string _imgUrlPathFile;

        #endregion

        #region Конструктор класса

        //КОНСТРУКТОР КЛАССА
        public SharedFilesForm(Page pagnew, int countOfElemInOnePagenew)
        {
            _pag = pagnew;
            _countOfElemInOnePage = countOfElemInOnePagenew;

            _context = HttpContext.Current;
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\shared\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _imgUrlPathFile = "../../../files/shared/";
        }

        #endregion

        #region Для консоли управления

        /// <summary>Метод возвращает панель с фильтром и кнопками управления списком файлов</summary>
        /// <returns></returns>
        public Panel GetFilterPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "Панель добавления файлов для общего доступа"; panelWrapper.Controls.Add(lbl);

            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = _pag.Session["srchStr2"].ToString(); txtBox.ID = "txtBoxSrchEvent";
            txtBox.ToolTip = "Введите сюда текст, по которому нужно найти данные о файле. Поиск осуществляется по всем текстовым полям данных о файле.";
            panelWrapper.Controls.Add(txtBox);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Найти "; lBtn.ToolTip = "Кнопка поиска файл по его данным. Поиск осуществляется по всем текстовым полям.";
            lBtn.Command += (lBtnSrch_Command); lBtn.ID = "btnEventSrch";
            panelWrapper.Controls.Add(lBtn);

            panelWrapper.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                                                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));

            // Кнопка очистки ненужных файлов в папке, которые не относятся к файлам, содержащимся в БД
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " - очистить лишний файлы "; lBtn.ToolTip = "Кнопка очищает все файлы, которые не относятся к файлам, содержащимся в базе данных";
            lBtn.Command += (lBtnClear_Command); panelWrapper.Controls.Add(lBtn);

            //panelWrapper.Controls.Add(new LiteralControl("<br />"));

            // Вывод размера файла БД и размера папок с файлами к новостям
            SharedFilesWork work = new SharedFilesWork();
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер БД: "; panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetDbSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер папки с файлами: "; panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetFoldersSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

            //Кнопка ДОБАВИТЬ файл
            panelWrapper.Controls.Add(new LiteralControl("<br /><br />"));
            FileUpload fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "foto_ImgUpload";
            fUpload.ToolTip = "Загружайте файлы размером не более 5-ти мегабайт";
            panelWrapper.Controls.Add(fUpload);

            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = "ДОБАВИТЬ";
            lBtn.ToolTip = "Загружайте размером не более 5-ти мегабайт";
            lBtn.Command += lBtn_AddFile; panelWrapper.Controls.Add(lBtn);

            return panelWrapper;
        }

        #region События для функции GetFilterPanel()

        /// <summary>событие нажатия на кнопку "найти" новость по поисковому запросу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSrch_Command(object sender, CommandEventArgs e)
        {
            try
            {
                _pag.Session["srchStr2"] = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Text;
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>Нажатие на кнопку ДОБАВИТЬ файл</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtn_AddFile(object sender, CommandEventArgs e)
        {
            FileUpload fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$foto_ImgUpload");
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);

            if (fUpload.HasFile)
            {
                string fileNamePath = _pag.Server.HtmlEncode(fUpload.FileName);
                string fileName = Path.GetFileName(fileNamePath);
                //string extension = Path.GetExtension(fileName);

                #region Проверка имени файла на корректность

                if (!IsStringLatin.IsLatin(fileName))
                {
                    warning.ShowWarning("Файл не добавлен. Имя файла должно быть составлено только из латинских букв", _pag.Master); return;
                }
                if (fileName.Contains(" "))
                {
                    fileName = fileName.Replace(" ", "_");
                }
                SharedFilesWork work = new SharedFilesWork();
                if (work.IsFileNameExist(fileName))
                {
                    warning.ShowWarning("Файл не добавлен. Файл с таким же именем уже существует в базе данных", _pag.Master); return;
                }

                #endregion

                //if (extension != null && ((extension.ToLower() == ".doc") || (extension.ToLower() == ".docx") ||
                //                          (extension.ToLower() == ".rar") || (extension.ToLower() == ".zip")))     //проверка на допустимые расширения закачиваемого файла
                //{
                int fileSize = fUpload.PostedFile.ContentLength;
                if (fileSize < 5000000)                              //проверка на допустимый размер закачиваемого файла
                {
                    #region Код сохранения файла на серевер

                    //сохраним файл в папку с файлами для конкурсных работ (у каждого конкурса своя папка)
                    try
                    {
                        fUpload.SaveAs(_pathToFilesFolder + fileName);
                        fUpload.Dispose();
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                        warning.ShowWarning("Не удалось сохранить файл на жёсткий диск. Попробуйте повторить..", _pag.Master);
                        return;
                    }

                    #region Добавление имени файла в БД

                    try
                    {
                        SharedFiles obj = new SharedFiles();
                        obj.FileName = fileName;
                        obj.DateReg = DateTime.Now.Ticks;
                        obj.ManagerNick = ((AdmPersonStruct) _pag.Session["authperson"]).Name;
                        if (work.InsertOneFile(obj) == -1)
                        {
                            throw new Exception("Ошибка при сохранении данных о файле с именем - " + obj.FileName + " в БД");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                        warning.ShowWarning("Не удалось сохранить файл в базу данных. Попробуйте повторить..", _pag.Master);
                        return;
                    }

                    #endregion

                    _pag.Response.Redirect(_pag.Request.RawUrl);

                    #endregion
                }
                else { warning.ShowWarning("Допускаются файлы размером не более 5 мегабайт", _pag.Master); }
                //}
                //else { lbl.Text = "Допускаются только файлы в формате DOC, DOCX, RAR, ZIP"; panelWarn.Controls.Add(lbl); }
            }
            else { warning.ShowWarning("Сначала выберите файл через кнопку 'ОБЗОР'", _pag.Master); }
        }

        /// <summary>нажатие на кнопку ОЧИСТИТЬ ЛИШНИЕ ФАЙЛЫ в папке с файлами</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnClear_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            SharedFilesWork work = new SharedFilesWork();
            if (!work.DeleteUnnecessaryFiles())
            {
                warning.ShowWarning("ВНИМАНИЕ. Не удалось очистить все лишние файлы. Попробуйте повторить.", _pag.Master);
                return;
            }
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion

        /// <summary>Метод возвращает таблицу со всеми файлами и данными по ним (возможно отфильтрованными по переменной Session["srchStr2"])</summary>
        /// <returns></returns>
        public Table GetListTbl()
        {
            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell();
            Label lbl;

            #region Получение списка структур новостей

            SharedFilesWork work = new SharedFilesWork();
            List<SharedFiles> list = work.GetSortedListOfFiles((string)_pag.Session["srchStr2"]);
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
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего файлов: "; tblCell.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = list.Count.ToString(); tblCell.Controls.Add(lbl);

            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

            //добавляем подтаблицу
            var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

            //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ
            var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
            var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата/время"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Ник админа"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Тип файла"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Имя файла"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Полная ссылка на файл"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);       //в этом столбце будут кнопки УДАЛИТЬ
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = ""; tblCell1.Controls.Add(lbl);

            //добавляем строки с данными по событиям
            foreach (SharedFiles oneStruct in list)
            {
                prodCounter++;
                if (prodCounter > _countOfElemInOnePage)   //выводим на страницу не более нужного кол-ва событий
                {
                    prodCounter = 1;
                    pageCounter++;
                }

                if (pageCounter == (int)_pag.Session["pagenum1"])           //если эта та самая страница, которую нужно вывести, то выводим её
                {
                    tblRow1 = new TableRow();

                    //ДАТА и ВРЕМЯ
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    var dt = new DateTime(oneStruct.DateReg);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = dt.ToShortDateString() + " " + dt.ToShortTimeString(); tblCell1.Controls.Add(lbl);

                    //НИК админа, добавившего файл
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.ManagerNick; tblCell1.Controls.Add(lbl);

                    //Тип файла
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl";

                    #region Код определения типа файла

                    string typeOfFile = "?";
                    var extension = Path.GetExtension(oneStruct.FileName);
                    if (extension != null)
                    {
                        string extention = extension.ToLower();
                        switch (extention)
                        {
                            case ".doc":
                                typeOfFile = "MS Word";
                                break;
                            case ".docx":
                                typeOfFile = "MS Word";
                                break;
                            case ".rtf":
                                typeOfFile = "MS Word";
                                break;
                            case ".xls":
                                typeOfFile = "MS EXCEL";
                                break;
                            case ".xlsx":
                                typeOfFile = "MS EXCEL";
                                break;
                            case ".ppt":
                                typeOfFile = "MS PowerPoint";
                                break;
                            case ".pptx":
                                typeOfFile = "MS PowerPoint";
                                break;
                            case ".txt":
                                typeOfFile = "Текстовый файл";
                                break;
                            case ".jpg":
                                typeOfFile = "Картинка";
                                break;
                            case ".jpeg":
                                typeOfFile = "Картинка";
                                break;
                            case ".png":
                                typeOfFile = "Картинка";
                                break;
                            case ".gif":
                                typeOfFile = "Картинка";
                                break;
                            case ".zip":
                                typeOfFile = "Архив";
                                break;
                            case ".rar":
                                typeOfFile = "Архив";
                                break;
                            case ".7z":
                                typeOfFile = "Архив";
                                break;
                            case ".pdf":
                                typeOfFile = "Adobe PDF";
                                break;
                            default:
                                typeOfFile = "?";
                                break;
                        }
                    }

                    #endregion

                    lbl.Text = typeOfFile;
                    tblCell1.Controls.Add(lbl);

                    //Имя файла
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.FileName; tblCell1.Controls.Add(lbl);

                    //Ссылка
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    string link = "http://" + _pag.Request.ServerVariables["SERVER_NAME"] + ":" + _pag.Request.ServerVariables["SERVER_PORT"] + 
                                    "/" + _imgUrlPathFile.Replace("../../../","") + oneStruct.FileName;
                    tblCell1.Controls.Add(new LiteralControl("<a href='" + link + "' target='_blank' class='hlinkHover lBtnsUniverse'>" + link + "</a>"));

                    //Кнопка УДАЛИТЬ
                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                    LinkButton lBtn = new LinkButton(); lBtn.CommandArgument = oneStruct.Id.ToString();
                    lBtn.Command += new CommandEventHandler(lBtnDelete_Command);
                    lBtn.ToolTip = "удалить данные о файле из БД, с жёсткого диска файл не удаляется";
                    lBtn.OnClientClick = "return confirm('Данные о файле будут удалены из базы данных. Удалить?');";
                    Image img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/krestik.png";
                    lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);

                    tbl1.Controls.Add(tblRow1);
                }
                else if (pageCounter > (int)_pag.Session["pagenum1"])
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

        #region События для функции GetListTbl()

        /// <summary>нажатие на кнопку УДАЛИТЬ файл</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит Id записи о файле</param>
        private void lBtnDelete_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            SharedFilesWork work = new SharedFilesWork();
            int res = work.DeleteOneFile(e.CommandArgument.ToString());
            if (res == 0 || res == -1)
            {
                warning.ShowWarning("ВНИМАНИЕ. Не получилось удалить новость из БД. Попробуйте повторить..", _pag.Master);
                return;
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>Класс формирования данных по файлам для класса SharedFilesForm</summary>
    public class SharedFilesWork
    {
        #region Поля

        private HttpContext _context;
        private string _pathToDb;
        private string _tableName;
        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        public bool _checkFolders;

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса. Добавляет в БД таблицу с данным по файлам, если её ещё не существует.
        /// Так же инициализирует поля класса.</summary>
        public SharedFilesWork()
        {
            _context = HttpContext.Current;
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\shared\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _checkFolders = true;

            #region Создание необходимых папок

            try
            {
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

            #region Добавление таблицы для хранения данных по файлам в БД

            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\sharedfiles.db";
            _tableName = "filestable";

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "CREATE TABLE IF NOT EXISTS " + _tableName + "(" +
                               "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "FileName TEXT NOT NULL, " +
                               "ManagerNick TEXT NOT NULL, " +
                               "DateReg INTEGER NOT NULL" +
                               ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();

            #endregion
        }

        #endregion


        #region Метод InsertOneFile(SharedFiles obj)

        /// <summary>Метод добавляет в БД одну запись с данными по файлу.</summary>
        /// <param name="obj">объект SharedFiles с данными по одному файлу</param>
        /// <returns>Метод возвращает номер(id) внесённости в БД записи или -1 в случае какой-либо ошибки</returns>
        public int InsertOneFile(SharedFiles obj)
        {
            int result = 0;
            if (!_checkFolders) return -1;

            try
            {
                #region MyRegion

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                           "FileName, " +
                                                           "ManagerNick, " +
                                                           "DateReg" +
                                                            ") " +
                                                "VALUES (" +
                                                           "@FileName, " +
                                                           "@ManagerNick, " +
                                                           "@DateReg" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@FileName", obj.FileName));
                cmd.Parameters.Add(new SQLiteParameter("@ManagerNick", obj.ManagerNick));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));


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

        #endregion

        #region Метод GetListOfFiles()

        /// <summary>Метод возвращает список структур всех данных по файлам.</summary>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<SharedFiles> GetListOfFiles()
        {
            List<SharedFiles> resultList = new List<SharedFiles>();
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

                    SharedFiles obj = new SharedFiles();
                    while (reader.Read())
                    {
                        obj = new SharedFiles();
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

        #endregion

        #region Метод GetSortedListOfFiles(string srchString = "")

        /// <summary>Метод-обёртка над методом GetListOfFiles. Сортирует список структур по дате.
        /// Так же производится фильтрация по поисковой строке</summary>
        /// <param name="srchString">строка поискового запроса</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<SharedFiles> GetSortedListOfFiles(string srchString = "")
        {
            List<SharedFiles> tempList = GetListOfFiles();
            if (!_checkFolders) return null;

            if (tempList == null) return null;
            tempList = tempList.OrderBy(x => x.DateReg * -1).ToList();

            #region Фильтрация по поисковой строке

            List<SharedFiles> resultList = new List<SharedFiles>();
            DateTime dt = new DateTime();
            if (srchString != "")
            {
                foreach (SharedFiles obj in tempList)
                {
                    dt = new DateTime(obj.DateReg);
                    if (obj.ManagerNick.ToLower().Contains(srchString.ToLower()) ||
                        obj.FileName.ToLower().Contains(srchString.ToLower()) ||
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

        #endregion

        #region Метод GetFilesCount()

        /// <summary>Метод возвращает значение кол-ва файлов, зарегистрированных в БД</summary>
        /// <returns>Возвращает кол-во файлов или -1 в случае какой-либо ошибки</returns>
        public int GetFilesCount()
        {
            int result = -1;
            if (!_checkFolders) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT Count(*) FROM " + _tableName;
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
                    return -1;
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;

                #endregion
            }


            return result;
        }

        #endregion

        #region Метод DeleteOneFile(string id)

        /// <summary>Метод удаляет данные по одному файлу из БД. Сам файл не удаляется.</summary>
        /// <param name="id">id файла</param>
        /// <returns>Метод возвращает кол-во удалённых строк из таблицы или -1 в случае какой-либо ошибки или 0 - если ни одной записи не удалено</returns>
        public int DeleteOneFile(string id)
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
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;

                #endregion
            }

            return result;
        }

        #endregion

        #region Метод DeleteUnnecessaryFiles()

        /// <summary>Метод удаляет все файлы из папки файлов, которые не принадлежат ни одной зарегистрированной в БД записи о файле</summary>
        /// <returns>Возвращает true в случае успеха, и false - в случае возникновения какой-либо ошибки</returns>
        public bool DeleteUnnecessaryFiles()
        {
            if (!_checkFolders) return false;

            try
            {
                #region Основной код

                List<SharedFiles> allList = GetListOfFiles();
                List<string> listOfNames = new List<string>();
                foreach (SharedFiles obj in allList)
                {
                    listOfNames.Add(obj.FileName);
                }

                string[] pathToFiles = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);

                string fName = "";
                bool checker = false;
                foreach (string path in pathToFiles)
                {
                    fName = Path.GetFileName(path);
                    checker = false;
                    foreach (string s in listOfNames)
                    {
                        if (fName == s)
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
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;

                #endregion
            }

            return true;
        }

        #endregion

        #region Метод GetFilesCount()

        /// <summary>Метод проверяет наличие в БД записи файла с переданным именем</summary>
        /// <param name="fileName">строка поискового запроса</param>
        /// <returns>Возвращает true если файл с таким именем уже существует в БД, false - если не существует или произошла ошибка</returns>
        public bool IsFileNameExist(string fileName)
        {
            bool result = false;
            if (!_checkFolders) return false;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT Count(FileName) FROM " + _tableName + " WHERE FileName=@FileName";
                cmd.Parameters.Add(new SQLiteParameter("FileName", fileName));

                try
                {
                    int res = sqlite.ExecuteScalarParams(cmd);
                    if (res == -1) //если ошибка, то..
                    {
                        return false;
                    }
                    else if (res == 1)  //если найдена строка с именем файла, то..
                    {
                        return true;
                    }
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    return false;
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = false;

                #endregion
            }


            return result;
        }

        #endregion

        #region Метод FillRequest(SharedFiles obj, SQLiteDataReader reader)

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа SharedFiles данными из SQLiteDataReader.
        /// Метод используется при чтении данных из БД.</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        private void FillRequest(SharedFiles obj, SQLiteDataReader reader)
        {
            obj.FileName = reader["FileName"].ToString();
            obj.ManagerNick = reader["ManagerNick"].ToString();
            obj.Id = long.Parse(reader["_id"].ToString());
            obj.DateReg = long.Parse(reader["DateReg"].ToString());
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

        #region Метод GetFoldersSize()

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к новостям</summary>
        /// <returns></returns>
        public long GetFoldersSize()
        {
            long result = 0;

            #region Основной код

            try
            {
                #region Основной код

                if (Directory.Exists(_pathToFilesFolder))
                {
                    string[] arr1 = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);
                    List<string> list = new List<string>();
                    list.AddRange(arr1);
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
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>Класс представляет структуру данных по одному файлу, выложенному в общий доступ</summary>
    [Serializable]
    public class SharedFiles
    {
        private string fileName = "";           //имя файла (без пути к нему) *
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string managerNick = "";        //Ник (логин) администратора, добавившего файл *
        public string ManagerNick
        {
            get { return managerNick; }
            set { managerNick = value; }
        }



        // поля специального назначения
        private long id = -1;                   // Номер файла (формируется в БД)
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private long dateReg = 0;                //Дата добавления файла в общий доступ *
        public long DateReg
        {
            get { return dateReg; }
            set { dateReg = value; }
        }
    }

    #endregion

}