// Файл с классами для работы с ТЕКСТОВЫМИ СТРАНИЦАМИ. Файлы располагаются так:
// ~/files/pages/; 
// ~/files/pages/files_e; 
// База данных - 
// ~/files/sqlitedb/pages.db

#region Using

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

#endregion

namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>Класс формирует HTML-код страниц для сайта и панели редактирования страниц для консоли</summary>
    public class PagesFormE
    {
        #region Поля

        private Page _pag;
        private HttpContext _context;
        private string _pathToDb;
        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        private string _imgUrlPathFile;

        #endregion

        #region Конструктор класса

        public PagesFormE(Page pagnew = null)
        {
            _pag = pagnew;

            _context = HttpContext.Current;
            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\pages.db";
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\pages\files_e\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _imgUrlPathFile = "../../../files/pages/files_e/";
        }

        #endregion

        #region Для сайта

        /// <summary>Метод возвращает панель с различной информацией для текстовой страницы</summary>
        /// <param name="addPanel">ссылка на объект панели, в которую нужно добавить содержимое страницы</param>
        /// <param name="pageUName">условное имя страницы в БД, из которой нужно взять содержимое</param>
        /// <param name="cacheContr">установка управления cache браузера</param>
        /// <returns></returns>
        public Panel FillPage(Panel addPanel, string pageUName, CacheContr cacheContr)
        {
            Panel panelWrapper = new Panel();

            #region Основной код

            #region Получение структуры данных страницы

            PagesWorkClassE work = new PagesWorkClassE();
            PageStructE obj = work.GetOnePage(pageUName);
            if (obj == null) return panelWrapper;
            if (obj.Name == "") return panelWrapper;

            #endregion

            #region Добавление контента на страницу

            addPanel.Controls.Add(new LiteralControl(obj.Content));
            addPanel.Page.Header.Controls.Add(new LiteralControl("<noindex><style type='text/css'>" + obj.Css + "</style></noindex>"));
            addPanel.Page.Controls.Add(new LiteralControl("<noindex><script type='text/javascript'>" + obj.Javascript + "</script></noindex>"));

            #endregion

            #region SEO

            //Заголовок о модификации страницы - например: Last-Modified: Sun, 09 Aug 2015 13:44:12 GMT
            FileInfo fInfo = new FileInfo(_pathToDb);
            _pag.Response.Cache.SetLastModified(fInfo.LastWriteTime);

            //TITLE, DESCRIPTION, KEYWORDS
            _pag.Title = obj.Title;
            _pag.Header.Controls.Add(new LiteralControl("<meta name='description' content='" + obj.Description + "' />"));
            _pag.Header.Controls.Add(new LiteralControl("<meta name='keywords' content='" + obj.Keywords + "' />"));

            //CACHE
            if (cacheContr == CacheContr.Cash)
            {
                // здесь не нужен код
            }
            else if (cacheContr == CacheContr.DisableCash)
            {
                _pag.Response.Cache.SetCacheability(HttpCacheability.Private);
                _pag.Response.Cache.SetMaxAge(TimeSpan.Zero);
            }
            else if (cacheContr == CacheContr.NoCache)
            {
                _pag.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }

            #endregion

            #endregion

            return panelWrapper;
        }

        /// <summary>Метод возвращает панель с различной информацией для текстовой страницы. Он так же использует текстовые страницы,
        /// созданные в редакторе, создаваемом классом PagesFormAdm. Если в обоих редакторах есть страницы с одним и тем же
        /// условным наименованием, то используется страница, созданная в редакторе, формируемом классом PagesFormE (новым редактором)</summary>
        /// <param name="addPanel">ссылка на объект панели, в которую нужно добавить содержимое страницы</param>
        /// <param name="pageUName">условное имя страницы в БД, из которой нужно взять содержимое</param>
        /// <param name="cacheContr">установка управления cache браузера</param>
        /// <returns></returns>
        public Panel UniverseFillPage(Panel addPanel, string pageUName, CacheContr cacheContr)
        {
            Panel panelWrapper = new Panel();

            #region Основной код

            PagesWorkClassE work = new PagesWorkClassE();
            if (work.IsPageUnameExist(pageUName)) // если существует страница в БД нового редактора, то используем её
            {
                FillPage(addPanel, pageUName, cacheContr);
            }
            else
            {
                PagesForm form = new PagesForm(_pag);
                addPanel.Controls.Add(form.GetPage(_context.Server.MapPath("~") + @"files/pages/" + pageUName));
                form.AddSeo(pageUName, cacheContr);
            }

            #endregion

            return panelWrapper;
        }

        #region Статические методы для управление cache браузера

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

        #endregion

        #region Перечисление для управления Cache браузера

        /// <summary>Перечисление для управления Cache браузера</summary>
        public enum CacheContr
        {
            Cash,           // включить полное кэширование (на усмотрение браузера)
            NoCache,        // включить в браузере режим NoCache
            DisableCash     // полное отключение кэширования в браузере
        };

        #endregion

        #endregion

        #region Для консоли

        /// <summary>Метод возвращает панель с выбором страницы для редактирования из списка</summary>
        /// <returns></returns>
        public Panel GetPagesChoosePanel()
        {
            Panel panelWrapper = new Panel();
            PagesWorkClassE work = new PagesWorkClassE();

            //ЗАГЛАВИЕ
            Label lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР ТЕКСТОВЫХ СТРАНИЦ"; panelWrapper.Controls.Add(lbl);

            //Панель добавления новой страницы
            Panel panelLine = new Panel(); panelLine.CssClass = "panelLine";
            TextBox txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm placeHolder"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxPageName"; txtBox.Width = 200;
            txtBox.Text = ""; txtBox.Attributes.Add("placeholder", "имя страницы");
            panelLine.Controls.Add(txtBox);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 10; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm placeHolder"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxPageFileName"; txtBox.Width = 250;
            txtBox.Text = ""; txtBox.Attributes.Add("placeholder", "условное имя страницы - на латинице");
            panelLine.Controls.Add(txtBox);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 10; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            //Кнопка добавления новой страницы
            LinkButton lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Создать страницу ";
            lBtn.ToolTip = "Создание новой страницы";
            lBtn.Command += (lBtnAddNewPage_Command);
            panelLine.Controls.Add(lBtn);

            panelWrapper.Controls.Add(panelLine);

            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == "администратор")
            {
                //кнопка удаления лишних фотографий
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " Удалить лишние файлы "; lBtn.ToolTip = "Удаляются файлы удалённых страниц";
                lBtn.Command += (lBtnDelOldPagesFoto_Command); panelWrapper.Controls.Add(lBtn);

                // Вывод размера файла БД и размера папки с файлами к страницам
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер БД: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetDbSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);

                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Размер папки с файлами: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = work.GetFoldersSize().ToString(); lbl.Font.Bold = true; lbl.ForeColor = Color.FromArgb(40, 85, 191); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = " Kb"; panelWrapper.Controls.Add(lbl);
            }

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //выпадающий список с выбором текстовой страницы для редактирования для редактирования
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Выберите текстовую страницу для редактирования: "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            DropDownList ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            List<PageStructE> list = work.GetSortedListOfPagesL();
            if (list != null)
            {
                #region Заполнение выпадающего списка

                foreach (PageStructE obj in list)
                {
                    ddl.Items.Add(obj.Name + "|" + obj.Uname);
                }

                if (_pag.Session["PageStructE"] != null)
                {
                    int counter = 0;
                    string itemText = ((PageStructE)_pag.Session["PageStructE"]).Name + "|" +
                                      ((PageStructE)_pag.Session["PageStructE"]).Uname;
                    foreach (ListItem name in ddl.Items)
                    {
                        if (name.Text == itemText)
                        {
                            ddl.SelectedIndex = counter;
                            break;
                        }
                        counter++;
                    }
                }

                #endregion
            }
            ddl.ID = "ddlPagesSelect"; panelLine.Controls.Add(ddl);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            //запуск редактирования выбранной в списке новости
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Редактировать ";
            lBtn.ToolTip = "Запуск редактирования выбранной в списке текстовой страницы.";
            lBtn.Command += (lBtnEditPage_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            //очистка содержимого страницы
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Очистить ";
            lBtn.ToolTip = "Очистка содержимого выбранной для редактирования текстовой страницы. Из базы данных содержимое не удаляется, поэтому для окончательного удаления нажмите на кнопку СОХРАНИТЬ.";
            lBtn.Command += (lBtnCleanPage_Command); lBtn.OnClientClick = "return confirm('Очистить содержимое страницы. Сделать?');";
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            //удаление текстовой страницы и данных о ней из файла данных о всех текстовых страницах
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " Удалить ";
            lBtn.ToolTip = "Удаление данных выбранной для редактирования текстовой страницы. (файлы, относящиеся к странице не удаляются)";
            lBtn.Command += (lBtnDeletePage_Command); lBtn.OnClientClick = "return confirm('Страница будет удалена. Сделать?');";
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для метода GetPagesChoosePanel()

        /// <summary>Нажатие на кнопку "Редактировать" выбранную в выпадающем списке текстовую страницу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEditPage_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            try
            {
                #region Основной код

                string val = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlPagesSelect")).SelectedValue;
                if (val == "")
                {
                    _pag.Session["PageStructE"] = null;
                }
                else
                {
                    PagesWorkClassE work = new PagesWorkClassE();
                    string[] strSplit = val.Split(new[] { '|' });
                    _pag.Session["PageStructE"] = work.GetOnePage(strSplit[1]);
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                warning.ShowWarning("ВНИМАНИЕ. Ошибка при загрузке данных страницы. Попробуйте повторить..", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;

                #endregion
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>события нажатия на кнопку "Удалить лишние фотографии"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDelOldPagesFoto_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            PagesWorkClassE work = new PagesWorkClassE();
            if (!work.DeleteUnnecessaryFiles())
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка при удалении файлов. Попробуйте повторить..", _pag.Master);
                return;
            }
            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>события нажатия на кнопку "Очистить"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCleanPage_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            try
            {
                #region Основной код

                if (_pag.Session["PageStructE"] != null)
                    ((PageStructE)_pag.Session["PageStructE"]).Content = "";

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                warning.ShowWarning("ВНИМАНИЕ. Не удалось очистить содержимое старницы. Попробуйте повторить..", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;

                #endregion
            }

            if (_pag.Session["PageStructE"] != null)
                _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>события нажатия на кнопку "Удалить"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDeletePage_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            bool err = false;

            try
            {
                #region Основной код

                if (_pag.Session["PageStructE"] != null)
                {
                    PagesWorkClassE work = new PagesWorkClassE();
                    int res = work.DeleteOnePage(((PageStructE)_pag.Session["PageStructE"]).Uname);
                    if (res == 0)
                    {
                        throw new Exception("в базе данных не удалилось ни одной строки для страницы с uname - " + ((PageStructE)_pag.Session["PageStructE"]).Uname);
                    }
                    else if (res == -1)
                    {
                        throw new Exception("в базе данных произошла ошибка при удалении строки данных для страницы с uname - " +
                            ((PageStructE)_pag.Session["PageStructE"]).Uname);
                    }
                    else
                    {
                        DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Удалена страница - " + 
                            ((PageStructE)_pag.Session["PageStructE"]).Name + " / " + ((PageStructE)_pag.Session["PageStructE"]).Uname + ". Удалил - " +
                            ((AdmPersonStruct)_pag.Session["authperson"]).Name);
                        _pag.Session["PageStructE"] = null;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Основной код

                warning.ShowWarning("ВНИМАНИЕ. Не удалось удалить страницу. Попробуйте повторить..", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                err = true;

                #endregion
            }

            if (err) return;

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        /// <summary>события нажатия на кнопку "Создать страницу"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAddNewPage_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            try
            {
                #region Основной код

                PagesWorkClassE work = new PagesWorkClassE();

                string name = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageName")).Text;
                string uname = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxPageFileName")).Text;

                #region Проверка значений на правильность

                if (name.Trim() == "")
                {
                    warning.ShowWarning("ВНИМАНИЕ. Имя страницы не должно быть пустым.", _pag.Master); return;
                }
                if (uname.Trim() == "")
                {
                    warning.ShowWarning("ВНИМАНИЕ. Условное имя файла страницы не должно быть пустым.", _pag.Master); return;
                }
                if (!IsStringLatin.IsLatin(uname))
                {
                    warning.ShowWarning("ВНИМАНИЕ. Условное имя файла страницы должно содержать только латинские символы.", _pag.Master); return;
                }
                if (work.IsPageUnameExist(uname))
                {
                    warning.ShowWarning("ВНИМАНИЕ. Страница с таким условным именем уже существует. Придумайте уникальное название.", _pag.Master); return;
                }

                #endregion

                #region Заполнение свойств объекта

                PageStructE obj = new PageStructE();
                obj.Name = name;
                obj.Uname = uname;

                #endregion

                #region Сохранение объекта страницы в БД

                int res = work.InsertOnePage(obj);
                if (res == -1)
                {
                    throw new Exception("ошибка при создании новой страницы с uname - " + obj.Uname + " и с name -" + obj.Name);
                }

                #endregion

                _pag.Session["PageStructE"] = obj;

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                warning.ShowWarning("ВНИМАНИЕ. Не удалось добавить новую страницу. Попробуйте повторить..", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;

                #endregion
            }

            _pag.Response.Redirect(_pag.Request.RawUrl);
        }

        #endregion

        /// <summary>Метод возвращает таблицу с редактором одной текстовой страницы</summary>
        /// <returns></returns>
        public Panel GetPageEditPanel()
        {
            Panel panelWrapper = new Panel();

            #region Код заполнения необходимых для работы с новостью данных

            //проверка на то, что временная структура с данными по странице существует
            if (_pag.Session["PageStructE"] == null) return panelWrapper;
            PageStructE obj = (PageStructE)_pag.Session["PageStructE"];

            #endregion

            // Заголовок
            panelWrapper.Controls.Add(new LiteralControl("<span class='lblSectionTitle'>Панель редактирования содержимого текстовой страницы</span>"));

            #region Код формирования HTML кода формы создания или редактирования новости

            // Абзац пояснений по добавлению ссылок файлов для текстового редактора
            Panel panelLine = new Panel(); panelLine.CssClass = "panelLine";
            Label lbl = new Label(); lbl.CssClass = "lblFormPre moveUp";
            lbl.Text = "Кнопка ДОБАВИТЬ ниже служит для того, чтобы можно было загрузить файл изображения или иной файл на сайт, получить ссылку на него (появится справа от кнопки). Эту ссылку можно ипользовать в текстовом редакторе, расположенном ниже.";
            panelLine.Controls.Add(lbl);
            panelLine.Controls.Add(new LiteralControl("<br /><br />"));

            // Кнопка открытия файлового менеджера (все стили и Javascript находятся в папке filemanager)
            // В функцию fmOpen передаются аргументы: 1-й - объект нажатой кнопки, 2-й - относительный путь к папке файлового менеджера, 3-й - относительный путь к папке с файлами
            panelLine.Controls.Add(new LiteralControl("<span id='btnFm' onclick=\"fmStart(this, '/Scripts/', '/files/pages/files_e/');\"></span>"));

            // Элемент загрузки файла с диска
            FileUpload fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.ID = "fileUpload";
            fUpload.ToolTip = "Загружайте любые файлы размером не более 2-х мегабайт."; panelLine.Controls.Add(fUpload);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 10; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            // Чекбокс НАЛОЖЕНИЯ КАРТИНКИ
            var chkBox = new CheckBox(); chkBox.ToolTip = "поставьте здесь галочку, если нужно наложить защитное изображение на добавляемую картинку (только для JPG, JPEG и PNG)";
            chkBox.ID = "chkBoxOverlayImgAdd"; panelLine.Controls.Add(chkBox);

            lbl = new Label(); lbl.CssClass = "lblFormZvezd"; lbl.Width = 10; lbl.Text = ""; panelLine.Controls.Add(lbl);       //просто для пропуска расстояния..

            // Кнопка ДОБАВИТЬ файл
            LinkButton lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = "Добавить";
            lBtn.CommandArgument = obj.Id.ToString();
            lBtn.Command += (lBtnAddFile_Command);
            lBtn.ToolTip = "Загружайте любой файл размером не более 10-ти мегабайт.";
            panelLine.Controls.Add(lBtn);

            // Элемент отображения полной ссылки на добавленный файл
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.ID = "hLinkToFile";
            lbl.Text = "&nbsp;&nbsp;&nbsp;&nbsp;" + obj.HLinkToFile;
            panelLine.Controls.Add(lbl);

            panelLine.Controls.Add(new LiteralControl("<br /><br />"));

            // Текстбокс редактора
            TextBox txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine; txtBox.ID = "txtBoxText";
            txtBox.Text = obj.Content;
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            // Ссылка на сайт разработчиков текстового редактора
            panelWrapper.Controls.Add(new LiteralControl("<span class='lblFormPre'>Ссылка на сайт текстового редактора CKEditor: </span><a style='cursor:pointer;' href='http://ckeditor.com/' target='_blank'>перейти</a><br/><br/>"));

            // Добавление скрипта для редактора
            panelWrapper.Controls.Add(new LiteralControl("<script type='text/javascript'>CKEDITOR.replace(document.getElementById('txtBoxText')); " +
                                                         "CKEDITOR.config.height=400;" +
                                                         "CKEDITOR.config.language='ru';" +
                                                         "</script> "));

            #region Редактор CSS

            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Font.Bold = true;
            lbl.Text = "Добавление таблиц стилей (CSS) для страницы:<br/>";
            panelWrapper.Controls.Add(lbl);

            // Текстбокс редактора CSS
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine;
            txtBox.Width = 1190; txtBox.Height = 150; txtBox.ID = "txtBoxCss";
            txtBox.Text = obj.Css;
            panelWrapper.Controls.Add(txtBox);

            #endregion

            #region Редактор JavaScript

            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Font.Bold = true;
            lbl.Text = "<br/><br/>Добавление JavaScript для страницы:<br/>";
            panelWrapper.Controls.Add(lbl);

            // Текстбокс редактора CSS
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.MultiLine;
            txtBox.Width = 1190; txtBox.Height = 150; txtBox.ID = "txtBoxJavascript";
            txtBox.Text = obj.Javascript;
            panelWrapper.Controls.Add(txtBox);

            #endregion

            #region Поля для добавления строки для тега <title>, мета тегов description и keywords

            //Заголовок страницы - Title
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Заголовок страницы(title)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxTitle"; txtBox.Width = 800; txtBox.Text = obj.Title;
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Содержимое значения content мета-тега description
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Описание страницы(description)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxDescription"; txtBox.Width = 800; txtBox.Text = obj.Description;
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //Содержимое значения content мета-тега keywords
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Ключевые слова страницы(keywords)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxKeywords"; txtBox.Width = 800; txtBox.Text = obj.Keywords;
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            //Кнопка СОХРАНИТЬ
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ ";
            lBtn.ToolTip = "Все изменения сохраняются в базу данных";
            lBtn.Command += (lBtnSave_Command);
            panelLine.Controls.Add(lBtn);

            panelWrapper.Controls.Add(panelLine);

            #endregion

            return panelWrapper;
        }

        #region События к методу GetPageEditPanel(...)

        /// <summary> нажатие на кнопку - ДОБАВИТЬ файл на сервер и выдать прямую ссылку на него.</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит ID новости</param>
        protected void lBtnAddFile_Command(object sender, CommandEventArgs e)
        {
            _pag.Session["PageStructE"] = CollectStructData();   // Метод все актуальные данные из текстбоксов во временную структуру данных страницы

            FileUpload fUpload = (FileUpload)_pag.FindControl("ctl00$ContentPlaceHolder1$fileUpload");
            CheckBox chkBox = (CheckBox)_pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxOverlayImgAdd");
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
                        ((PageStructE)_pag.Session["PageStructE"]).HLinkToFile = hlink;

                        #region Код наложения защитной картинки на сохранённое изображение

                        if (chkBox.Checked && ((extension.ToLower() == ".jpg") || (extension.ToLower() == ".jpeg") || (extension.ToLower() == ".png")))
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

        /// <summary> нажатие на кнопку СОХРАНИТЬ страницу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnSave_Command(object sender, CommandEventArgs e)
        {
            WarnClass warning = new WarnClass(); warning.HideWarning(_pag.Master);
            try
            {
                #region Основной код

                _pag.Session["PageStructE"] = CollectStructData();
                PageStructE obj = (PageStructE)_pag.Session["PageStructE"];
                PagesWorkClassE work = new PagesWorkClassE();

                if (work.IsPageUnameExist(obj.Uname))       // если страница уже существует в БД нужно обновить существующую запись
                {
                    #region Код

                    int res = work.UpdatePage(obj);
                    if (res == -1)
                    {
                        throw new Exception(
                            "в базе данных произошла ошибка при обновлении данных для страницы с uname - " + obj.Uname);
                    }
                    if (res == 0)
                    {
                        throw new Exception("в базе данных не обновились данные для страницы с uname - " + obj.Uname);
                    }

                    #endregion
                }
                else                                        // если страницы нет в БД нужно добавить новую запись
                {
                    #region Код

                    int res = work.InsertOnePage(obj);
                    if (res == -1)
                    {
                        throw new Exception("в базе данных произошла ошибка при добавлении данных для страницы с uname - " + obj.Uname);
                    }
                    if (res == 0)
                    {
                        throw new Exception("в базу данных не добавились данные для страницы с uname - " + obj.Uname);
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                warning.ShowWarning("ВНИМАНИЕ. Не удалось добавить новую страницу. Попробуйте повторить..", _pag.Master);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }
        }

        #endregion

        /// <summary>Метод считывает данные со страницы в структуру данных одной страницы, находящейся временно в сессионной переменной.</summary>
        /// <returns></returns>
        private PageStructE CollectStructData()
        {
            PageStructE obj = new PageStructE();

            try
            {
                #region Основной код

                obj = (PageStructE)_pag.Session["PageStructE"];
                // Содержимое страницы
                obj.Content = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxText")).Text;
                obj.Css = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxCss")).Text;
                obj.Javascript = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxJavascript")).Text;
                obj.Title = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxTitle")).Text;
                obj.Description = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxDescription")).Text;
                obj.Keywords = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxKeywords")).Text;

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return obj;
        }

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>Класс предназначен для работы с данными, касающимися текстовых страниц.</summary>
    public class PagesWorkClassE
    {
        #region Поля

        private HttpContext _context;
        private string _pathToDb;
        private string _tableName;
        private string _pathToFilesFolder;
        private string _pathToTempFolder;
        private string _imgUrlPathFile;
        public bool _checker;

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса. Добавляет в БД таблицу страниц, если её ещё не существует.
        /// Так же инициализирует поля класса.</summary>
        public PagesWorkClassE()
        {
            _context = HttpContext.Current;
            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\pages.db";
            _tableName = "pagetable";
            _pathToFilesFolder = _context.Server.MapPath("~") + @"files\pages\files_e\";
            _pathToTempFolder = _context.Server.MapPath("~") + @"files\temp\";
            _imgUrlPathFile = "../../../files/pages/files_e/";
            _checker = true;

            #region Создание необходимых папок

            try
            {
                Directory.CreateDirectory(_pathToFilesFolder);
                Directory.CreateDirectory(_pathToTempFolder);
            }
            catch (Exception ex)
            {
                _checker = false;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return;
            }

            #endregion

            #region Добавление таблицы для страниц в БД

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "CREATE TABLE IF NOT EXISTS " + _tableName + "(" +
                               "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "Name TEXT NOT NULL, " +
                               "Uname TEXT NOT NULL, " +
                               "Title TEXT NOT NULL, " +
                               "Description TEXT NOT NULL, " +
                               "Keywords TEXT NOT NULL, " +
                               "Content TEXT NOT NULL, " +
                               "Css TEXT NOT NULL, " +
                               "Javascript TEXT NOT NULL" +
                               ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();

            #endregion
        }

        #endregion

        #region Методы класса

        /// <summary>Метод добавляет в БД одну страницу.</summary>
        /// <param name="obj">объект PageStructE с данными по одной странице</param>
        /// <returns>Метод возвращает 1 в случае успешного добавления строки данных или -1 в случае какой-либо ошибки</returns>
        public int InsertOnePage(PageStructE obj)
        {
            int result = 0;
            if (!_checker) return -1;

            try
            {
                #region MyRegion

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                           "Name, " +
                                                           "Uname, " +
                                                           "Title, " +
                                                           "Description, " +
                                                           "Keywords, " +
                                                           "Content, " +
                                                           "Css, " +
                                                           "Javascript" +
                                                            ") " +
                                                "VALUES (" +
                                                           "@Name, " +
                                                           "@Uname, " +
                                                           "@Title, " +
                                                           "@Description, " +
                                                           "@Keywords, " +
                                                           "@Content," +
                                                           "@Css, " +
                                                           "@Javascript" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@Name", obj.Name));
                cmd.Parameters.Add(new SQLiteParameter("@Uname", obj.Uname));
                cmd.Parameters.Add(new SQLiteParameter("@Title", obj.Title));
                cmd.Parameters.Add(new SQLiteParameter("@Description", obj.Description));
                cmd.Parameters.Add(new SQLiteParameter("@Keywords", obj.Keywords));
                cmd.Parameters.Add(new SQLiteParameter("@Content", obj.Content));
                cmd.Parameters.Add(new SQLiteParameter("@Css", obj.Css));
                cmd.Parameters.Add(new SQLiteParameter("@Javascript", obj.Javascript));

                if (sqlite.ExecuteNonQueryParams(cmd) == -1) return -1;
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

        /// <summary>Метод возвращает список структур всех страниц. В структурах заполняется только поле Id, Name, Uname</summary>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<PageStructE> GetListOfPagesL()
        {
            List<PageStructE> resultList = new List<PageStructE>();
            if (!_checker) return null;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT _id, Name, Uname FROM " + _tableName;

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }
                try
                {
                    #region Код заполнения списка

                    PageStructE obj = new PageStructE();
                    while (reader.Read())
                    {
                        obj = new PageStructE();
                        FillObjL(obj, reader);
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
                return null;
            }

            return resultList;
        }

        /// <summary>Метод возвращает список структур всех страниц.</summary>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<PageStructE> GetListOfPages()
        {
            List<PageStructE> resultList = new List<PageStructE>();
            if (!_checker) return null;

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

                    PageStructE obj = new PageStructE();
                    while (reader.Read())
                    {
                        obj = new PageStructE();
                        FillObj(obj, reader);
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
                return null;
            }

            return resultList;
        }

        /// <summary>Метод-обёртка над методом GetListOfPagesL. Сортирует список структур по полю Name.</summary>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<PageStructE> GetSortedListOfPagesL()
        {
            List<PageStructE> tempList = GetListOfPagesL();
            if (!_checker) return null;

            if (tempList == null) return null;
            tempList = tempList.OrderBy(x => x.Name).ToList();

            return tempList;
        }

        /// <summary>Метод возвращает одну полную структуру страницы по её условному названию (поле - Uname)</summary>
        /// <param name="uname">условное название страницы, например - mainpage</param>
        /// <returns>Возвращает null в случае ошибки</returns>
        public PageStructE GetOnePage(string uname)
        {
            PageStructE obj = new PageStructE();
            if (!_checker) return null;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE Uname=@Uname";
                cmd.Parameters.Add(new SQLiteParameter("Uname", uname));

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
                        FillObj(obj, reader);
                    }
                }
                catch (Exception ex)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    return null;
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

        /// <summary>Метод обновляет данные по странице в таблице БД.</summary>
        /// <param name="obj">объект с данныйми по одной странице</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdatePage(PageStructE obj)
        {
            int result = -1;
            if (!_checker) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET " +
                                    "Name=@Name, " +
                                    "Uname=@Uname, " +
                                    "Title=@Title, " +
                                    "Description=@Description, " +
                                    "Keywords=@Keywords, " +
                                    "Content=@Content, " +
                                    "Css=@Css, " +
                                    "Javascript=@Javascript " +
                                  "WHERE Uname=@Uname;";

                cmd.Parameters.Add(new SQLiteParameter("Name", obj.Name));
                cmd.Parameters.Add(new SQLiteParameter("Uname", obj.Uname));
                cmd.Parameters.Add(new SQLiteParameter("Title", obj.Title));
                cmd.Parameters.Add(new SQLiteParameter("Description", obj.Description));
                cmd.Parameters.Add(new SQLiteParameter("Keywords", obj.Keywords));
                cmd.Parameters.Add(new SQLiteParameter("Content", obj.Content));
                cmd.Parameters.Add(new SQLiteParameter("Css", obj.Css));
                cmd.Parameters.Add(new SQLiteParameter("Javascript", obj.Javascript));

                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return -1;
            }

            return result;
        }

        /// <summary>Проверка условного наименования страницы на уникальность</summary>
        /// <param name="uname">условное название страницы, например - mainpage</param>
        /// <returns>Возвращает true в случае, если страница с таким uname уже существует в таблице, false - если не существует</returns>
        public bool IsPageUnameExist(string uname)
        {
            #region Основной код

            List<PageStructE> list = GetListOfPagesL();
            if (list == null) return false;
            if (list.Any(a => a.Uname == uname))    //если такое условное наименование страницы уже существует, то..
            {
                return true;
            }

            #endregion

            return false;
        }

        /// <summary>Метод удаляет данные по одной странице из БД. Файлы, относящиеся к странице, не удаляются. Имеются ввиду файлы, 
        /// загруженные на сервер для использования на этой странице</summary>
        /// <param name="uname">условное название страницы, например - mainpage</param>
        /// <returns>Метод возвращает кол-во удалённых строк из таблицы или -1 в случае какой-либо ошибки или 0 - если ни одной записи не удалено</returns>
        public int DeleteOnePage(string uname)
        {
            int result = -1;
            if (!_checker) return -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "DELETE FROM " + _tableName + " WHERE Uname=@Uname";
                cmd.Parameters.Add(new SQLiteParameter("Uname", uname));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message);
                return -1;
            }

            return result;
        }

        /// <summary>Метод удаляет все файлы из папки с файлами страниц (~/files/pages/files_e), которые не принадлежат ни одной зарегистрированной в БД странице</summary>
        /// <returns>Возвращает true в случае успеха, и false - в случае возникновения какой-либо ошибки</returns>
        public bool DeleteUnnecessaryFiles()
        {
            if (!_checker) return false;

            try
            {
                #region Основной код

                List<PageStructE> allList = GetListOfPages();

                string[] pathToFiles = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);

                string fName = "";
                bool checker = false;

                foreach (string path in pathToFiles)
                {
                    fName = Path.GetFileName(path);
                    checker = false;
                    foreach (PageStructE obj in allList)
                    {
                        if (fName != null && obj.Content.Contains(fName))
                        {
                            checker = true; break;
                        }
                        else if (fName != null && obj.Css.Contains(fName))
                        {
                            checker = true; break;
                        }
                        else if (fName != null && obj.Javascript.Contains(fName))
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

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа PageStructE данными из SQLiteDataReader.
        /// Метод используется при чтении данных из БД.</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        private void FillObj(PageStructE obj, SQLiteDataReader reader)
        {
            obj.Name = reader["Name"].ToString();
            obj.Uname = reader["Uname"].ToString();
            obj.Title = reader["Title"].ToString();
            obj.Description = reader["Description"].ToString();
            obj.Keywords = reader["Keywords"].ToString();
            obj.Content = reader["Content"].ToString();
            obj.Css = reader["Css"].ToString();
            obj.Javascript = reader["Javascript"].ToString();

            obj.Id = long.Parse(reader["_id"].ToString());
        }

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа PageStructE данными из SQLiteDataReader.
        /// Метод заполняет не все поля объекта.
        /// Метод используется при чтении данных из БД.</summary>
        /// <param name="obj">объект</param>
        /// <param name="reader">объект</param>
        private void FillObjL(PageStructE obj, SQLiteDataReader reader)
        {
            obj.Name = reader["Name"].ToString();
            obj.Uname = reader["Uname"].ToString();
            obj.Id = long.Parse(reader["_id"].ToString());
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

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к конкурсам</summary>
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
                    string[] arr = Directory.GetFiles(_pathToFilesFolder, "*", SearchOption.TopDirectoryOnly);

                    FileInfo f;
                    foreach (string path in arr)
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
                return 0;

                #endregion
            }

            #endregion

            return result;
        }

        #endregion
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>класс описывающий структуру данных одной текстовой страницы</summary>
    [Serializable]
    public class PageStructE
    {
        private string name = "";                   //содержит название страницы. Например - Главная
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string uname = "";                   //содержит условное название страницы. Например - mainpage (должно быть уникальным)
        public string Uname
        {
            get { return uname; }
            set { uname = value; }
        }

        private string title = "";                   //содержит текст для тега <title></title>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string description = "";             //содержит текст для тега <meta name='description' content='' />
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string keywords = "";               //содержит текст для тега <meta name='keywords' content='' />
        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        private string content = "";                //содержит весь контент страницы
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        private string css = "";                //содержит таблицы стилей к странице
        public string Css
        {
            get { return css; }
            set { css = value; }
        }

        private string javascript = "";                //содержит JavaScript к странице
        public string Javascript
        {
            get { return javascript; }
            set { javascript = value; }
        }




        // поля специального назначения :-)
        private long id = 0;                        // id страницы в БД
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private string hLinkToFile = "";                  // переменная будет временно хранить ссылку на закачанный для использования в редакторе файл. В БД не используется
        public string HLinkToFile
        {
            get { return hLinkToFile; }
            set { hLinkToFile = value; }
        }
    }

    #endregion
}