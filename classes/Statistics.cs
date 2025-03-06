using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
/* Файл содержит классы для сбора и обработки различной статистической информации по посещению страниц сайта */
using System.Web.UI.WebControls;
using site.classesHelp;

namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    public class StatisticsForm
    {
        private Page _pag;
        private int _countOfElemInOnePage;

        /// <summary>Конструктор класса</summary>
        /// <param name="pagenew">передаётся объект страницы, на которой производится работа</param>
        /// <param name="countOfElemInOnePage">количество статистических строк, выводимых на одной странице</param>
        public StatisticsForm(Page pagenew, int countOfElemInOnePage)
        {
            _pag = pagenew;
            _countOfElemInOnePage = countOfElemInOnePage;
        }

        /// <summary>функция возвращает таблицу с панель управления отображением статистических данных</summary>
        /// <returns>возвращает панель для консоли управления</returns>
        public Panel GetStatControlPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "ПРОСМОТР СТАТИСТИКИ ПОСЕЩЕНИЙ СТРАНИЦ САЙТА"; panelWrapper.Controls.Add(lbl);

            //Кнопки очистки статистики
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " очистить раннюю статистику "; lBtn.ToolTip = "Кнопка удаления файла с наиболее ранними статистическими событиями журнала. Если файл журнал один, то события удалены не будут. (Размер одного файла журнала - 500 килобайт.)";
            lBtn.Command += (lBtnStatEventClean_Command); panelWrapper.Controls.Add(lBtn);
            var statisticsWork = new StatisticsWork();
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " (Размер файлов журнала: "; panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = statisticsWork.GetSizeOfAllSiteEventFiles(); panelWrapper.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " килобайт)"; panelWrapper.Controls.Add(lbl);
            //Кнопка очистки журнала(удаляются все файлы событий)
            panelWrapper.Controls.Add(new LiteralControl("<br />"));
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " очистить всю статистику "; lBtn.ToolTip = "Кнопка удаления всех статистических событий журнала.";
            lBtn.Command += (lBtnAllStatEventClean_Command);
            panelWrapper.Controls.Add(lBtn);

            //Кнопка очистки журнала(удаляются все файлы событий)
            panelWrapper.Controls.Add(new LiteralControl("<br />"));
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " СТАТИСТИКА ПОСЕЩЕНИЯ СТРАНИЦ НОВОСТЕЙ "; lBtn.ToolTip = "Кнопка отображение статистики посещения новостных страниц.";
            lBtn.Command += (lBtnNewsStat_Command);
            panelWrapper.Controls.Add(lBtn);

            return panelWrapper;
        }

        #region События для функции GetStatControlPanel()

        /// <summary>событие нажатия на кнопку "очистить журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnStatEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            //удаляем все файлы событий
            var statisticsWork = new StatisticsWork();
            if (!statisticsWork.CleanStatEvents(false)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", _pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "очистить весь журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAllStatEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            //удаляем все файлы событий
            var statisticsWork = new StatisticsWork();
            if (!statisticsWork.CleanStatEvents(true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", _pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (_pag.Request.ServerVariables["QUERY_STRING"] == "") _pag.Response.Redirect(_pag.Request.ServerVariables["URL"]);
            else _pag.Response.Redirect(_pag.Request.ServerVariables["URL"] + "?" + _pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "СТАТИСТИКА ПОСЕЩЕНИЯ СТРАНИЦ НОВОСТЕЙ"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnNewsStat_Command(object sender, CommandEventArgs e)
        {
            //Очищаем статистику со страницы
            _pag.FindControl("ctl00$ContentPlaceHolder1$addStat").Controls.Clear();
            //Добавляем статистику на страницу
            //_pag.FindControl("ctl00$ContentPlaceHolder1$addStat").Controls.Add(GetNewsStatEventListTbl());
            //Заносим значение в сессонную переменную для отображения статистики при перезагрузках страницы
            HttpContext.Current.Session["showStatistic"] = "newsstat";
        }

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>Класс для работы с данными по посещению страниц сайта</summary>
    public class StatisticsWork
    {
        private FileStream _fs;  //переменная используется для блокировки доступа к файлу, который перезаписывается

        /// <summary>Конструктор класса</summary>
        public StatisticsWork() { }

        /// <summary>Метод сохраняет необходимые данные в файл статистики. Регистрируется только первый переход на конкретную страницу, остальные переходы на эту же 
        /// страницу не регистрируются.
        /// Записывается строка формата: ticks|rawurl|ip-адрес|dns-имя хоста|информация об агенте посетителя</summary>
        public void SaveStatistic()
        {
            #region Код определения нужности записи статистики по данному переходу

            if (HttpContext.Current.Session["statisticLog"] == null)
            {
                HttpContext.Current.Session["statisticLog"] = new List<string>();
            }
            string rawurl = HttpContext.Current.Request.RawUrl;     //RawUrl - это url вида - /infoone.aspx?newsid=8
            //Проверим, записывалась ли уже статистика по данной странице или нет. И если записывалась, то выходим из метода.
            bool checker = false;
            foreach (string oneraw in (List<string>)HttpContext.Current.Session["statisticLog"])
            {
                if (rawurl == oneraw) { checker = true; break; }
            }
            if (checker)    //если посетитель уже переходил на данную страницу, то не собираем для неё статистику
            {
                return;
            }
            else
            {
                ((List<string>)HttpContext.Current.Session["statisticLog"]).Add(rawurl);
            }

            #endregion

            #region КОД ФОРМИРОВАНИЯ СТРОКИ ДЛЯ ЗАПИСИ В ЖУРНАЛ

            //определяем имя хоста по IP-адресу
            var ipaddress = IPAddress.Parse(HttpContext.Current.Request.UserHostAddress);
            string hostname = "";
            try
            {
                var hostentry = Dns.GetHostEntry(ipaddress);
                hostname = hostentry.HostName;
            }
            catch
            {
                hostname = "NONAME";
            }

            string useragent = "";
            if (HttpContext.Current.Request.UserAgent != null) useragent = HttpContext.Current.Request.UserAgent;
            long ticky = DateTime.Now.Ticks;
            var dateTime = new DateTime(ticky);
            string vizitor = ticky + "|" + rawurl + "|" + HttpContext.Current.Request.UserHostAddress + "|" + hostname + "|" + useragent;

            #endregion

            #region КОД ЗАПИСИ ИНФОРМАЦИИ О ПЕСЕЩЕНИИ в файл журнала учёта посещений консоли управления

            try     //пробуем записать информацию о посетителе сайта в файл журнала учёта посетителей
            {
                var sw = new StreamWriter(GetNeeedDbFilePath(), true, Encoding.Default);
                sw.WriteLine(vizitor);      //записываем в конец файла строку
                sw.Close();
                sw.Dispose();
            }
            catch { }

            #endregion
        }

        /// <summary>Функция возвращает путь к файлу БД в который нужно добавлять статистику</summary>
        /// <returns></returns>
        private string GetNeeedDbFilePath()
        {
            string pathToNeedFile = ""; string[] strSplit, strSplit1;
            string[] pathToDBfiles = Directory.GetFiles(HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathToDBfiles.Length == 0)      //если файла журнала ещё не существует, то записываем в pathToNeedFile полный путь к новому(первому) файлу журнала
            {
                pathToNeedFile = HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics\jrl_1";
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
                var fi = new FileInfo(HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName);
                if (fi.Length > 500000)    //если размер последнего файла БД больше максимально допустимого, то задаём путь к новой файлу БД со следующим порядковым номером
                { pathToNeedFile = HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics\" + fNameLastDB.LeftPartOfFileName + "_" + (fNameLastDB.RightPartOfFileName + 1); }
                else { pathToNeedFile = HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName; }
            }

            return pathToNeedFile;
        }

        /// <summary>Функция возвращает список структур статистических событий посещения страниц сайта</summary>
        /// <returns>возвращает список структур всех статистических событий. Возвращает пустой список, если файлов с событиями ещё нет.</returns>
        private List<StatStruct> GetAllStatEventStructs()
        {
            var eventStructList = new List<StatStruct>();

            var oneEvent = new StatStruct();
            string[] str, strSplit;
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfilesArray = Directory.GetFiles(HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathsToDBfilesArray.Length == 0) { return eventStructList; }    //если файлов журнала ещё нет, то возвращаем пустой список

            foreach (string filePath in pathsToDBfilesArray)    //перебираем каждый файл БД товаров по очереди
            {
                str = File.ReadAllLines(filePath, Encoding.Default);
                foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
                {
                    if (line.Contains("|"))
                    {
                        oneEvent = new StatStruct();
                        strSplit = line.Split(new[] { '|' });
                        oneEvent.Ticks = Convert.ToInt64(strSplit[0]);
                        oneEvent.RawUrl = strSplit[1];
                        oneEvent.HostIp = strSplit[2];
                        oneEvent.HostName = strSplit[3];
                        oneEvent.AgentInfo = strSplit[4];
                        eventStructList.Add(oneEvent);
                    }
                }
            }

            return eventStructList;
        }

        /// <summary>функция очистки журнала статистических событий посещения сайта, удаляется файл с самыми ранними событиями (allclear = false) или все файлы (allclear = true)</summary>
        /// <returns></returns>
        public bool CleanStatEvents(bool allclear)
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathsToDBfiles.Length < 2 && !allclear) { return true; }

            if (allclear)   //если нужно удалить все события, то..
            {
                foreach (string onepath in pathsToDBfiles)
                {
                    try { File.Delete(onepath); }
                    catch { return false; }
                }
            }
            else            //если нужно удалить только файл журнала с самыми ранними событиями, то..
            {
                //определим полный путь к файлу журнала с самыми ранними событиями
                FileNameForSort fNameForSort, fNameFirstDB;
                string pathToNeedFile = ""; string[] strSplit, strSplit1;
                var fnamesForSort = new List<FileNameForSort>();
                foreach (string onepath in pathsToDBfiles)
                {
                    //вычленяем имя(без пути) файла БД
                    strSplit = onepath.Split(new[] { '\\' });
                    strSplit1 = strSplit[strSplit.Length - 1].Split(new[] { '_' });
                    fNameForSort = new FileNameForSort(); fNameForSort.LeftPartOfFileName = strSplit1[0]; fNameForSort.RightPartOfFileName = int.Parse(strSplit1[1]);
                    fnamesForSort.Add(fNameForSort);
                }
                fnamesForSort.Sort((a, b) => a.RightPartOfFileName.CompareTo(b.RightPartOfFileName));  //сортировка "от А до Я"(образно) по свойству rightPartOfFileName класса fileNameForSort
                fNameFirstDB = fnamesForSort[0];
                pathToNeedFile = HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics\" + fNameFirstDB.LeftPartOfFileName + "_" + fNameFirstDB.RightPartOfFileName;

                //удалим файл с самыми ранними событиями
                try { File.Delete(pathToNeedFile); }
                catch { return false; }
            }

            return true;
        }

        /// <summary>функция возвращает общий размер (в килобайтах) всех файлов журналов статистических событий посещения страниц сайта</summary>
        /// <returns></returns>
        public string GetSizeOfAllSiteEventFiles()
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(HttpContext.Current.Server.MapPath("~") + @"files\adm\statistics", "jrl_*", SearchOption.TopDirectoryOnly);

            FileInfo fi;
            long fullLength = 0;
            foreach (string onePath in pathsToDBfiles)
            {
                fi = new FileInfo(onePath);
                fullLength += fi.Length;
            }

            return (fullLength / 1024).ToString();
        }

        #region Методы для работы с файлом хранения данных по опросам (лайки)

        /// <summary>Метод возвращает список всех структур опросов. Если файл пуст, то возвращается пустой список</summary>
        /// <returns>список всех структур опросов - LikesStruct</returns>
        private List<LikesStruct> GetListOfAllLikesStructs()
        {
            var listAllOptions = new List<LikesStruct>();

            string pathtofile = "";
            pathtofile = HttpContext.Current.Server.MapPath("~") + @"files\adm\likes\likes";

            string[] str, strSplit;
            LikesStruct oneStruct;

            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string line in str)
                {
                    if (line.Contains("survey|"))  //проверка-подстраховка
                    {
                        strSplit = line.Split(new[] { '|' });
                        oneStruct = new LikesStruct();
                        oneStruct.likeName = strSplit[1];
                        oneStruct.likeCount = int.Parse(strSplit[2]);
                        oneStruct.nolikeCount = int.Parse(strSplit[3]);
                        listAllOptions.Add(oneStruct);
                    }
                }
            }
            else
            {
                try { File.WriteAllLines(pathtofile, new List<string>(), Encoding.Default); }
                catch { }
            }

            return listAllOptions;
        }

        /// <summary>Метод возвращает структуру данных ОПРОСА по его имени. Если такой структуры (с таким именем) не найдено, то метод возвращает "0|0"</summary>
        /// /// <param name="surveyName">имя ОПРОСА, для которого нужно получить данные</param>
        /// <returns>данные по опросу - likecount|nolikecount</returns>
        public string GetLikesStructCountersForName(string surveyName)
        {
            string resultString = "0|0";

            var structList = GetListOfAllLikesStructs();
            if (structList.Count > 0)
            {
                foreach (LikesStruct likesStruct in structList)
                {
                    if (likesStruct.likeName == surveyName)
                    {
                        resultString = likesStruct.likeCount + "|" + likesStruct.nolikeCount;
                        break;
                    }
                }
            }

            return resultString;
        }

        /// <summary>Метод добавляет значение для опроса в зависимости от переданного логического значения. Если нужно добавить like, то в метод передаётся - true, если nolike - то false</summary>
        /// <param name="surveyName">имя ОПРОСА, для которого нужно изменить данные</param>
        /// <param name="like">true - если нужно добавить для опроса положительный отзыв (like), false - если отзыв отказа</param>
        /// <returns>цифровое значение по добавленному отзыву. В случае ошибки при добавлении возвращается -1</returns>
        public string SaveLike(string surveyName, bool like)
        {
            var listOfStructs = GetListOfAllLikesStructs();
            string resultCount = "";
            string pathtoDb = HttpContext.Current.Server.MapPath("~") + @"files\adm\likes\likes";
            try { _fs = new FileStream(pathtoDb, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return "-1"; }

            if (listOfStructs.Count == 0)       //если файл данных по ОПРОСАМ пуст, то..
            {
                var newStruct = new LikesStruct();
                newStruct.likeName = surveyName;
                newStruct.likeCount = 0;
                newStruct.nolikeCount = 0;
                if (like)
                {
                    newStruct.likeCount++; 
                    resultCount = newStruct.likeCount.ToString();
                } 
                else
                {
                    newStruct.nolikeCount++; 
                    resultCount = newStruct.nolikeCount.ToString();
                }

                listOfStructs.Add(newStruct);
            }
            else if (listOfStructs.Count > 0)    //если нет, то..
            {
                //заменим данные в нужной структуре
                int counter = 0; bool checker = false;
                foreach (LikesStruct oneStruct in listOfStructs)
                {
                    if (oneStruct.likeName == surveyName)
                    {
                        checker = true;
                        if (like)
                        {
                            listOfStructs[counter].likeCount++; 
                            resultCount = listOfStructs[counter].likeCount.ToString(); 
                            break;
                        }
                        else
                        {
                            listOfStructs[counter].nolikeCount++;
                            resultCount = listOfStructs[counter].nolikeCount.ToString();
                            break;
                        }
                    }
                    counter++;
                }

                if(!checker)    //если в списке не было найдено ОПРОСА с нужным именем, то создаём новый ОПРОС и добавляем его в список
                {
                    var newStruct = new LikesStruct();
                    newStruct.likeName = surveyName;
                    newStruct.likeCount = 0;
                    newStruct.nolikeCount = 0;
                    if (like)
                    {
                        newStruct.likeCount++;
                        resultCount = newStruct.likeCount.ToString();
                    }
                    else
                    {
                        newStruct.nolikeCount++;
                        resultCount = newStruct.nolikeCount.ToString();
                    }

                    listOfStructs.Add(newStruct);
                }
            }

            //преобразуем список структур в список строк для перезаписи файла /files/options/options
            var newDataList = new List<string>();
            foreach (LikesStruct oneStruct in listOfStructs)
            {
                newDataList.Add(oneStruct.GetStringFromStruct());
            }

            //перезапишем файл /files/options/options
            var rn = new Random();
            int num = rn.Next(1, 666);
            string tempFileName = "likes_" + num;
            try
            {
                File.WriteAllLines(HttpContext.Current.Server.MapPath("~") + @"files\temp\" + tempFileName, newDataList, Encoding.Default);
            }
            catch { try { _fs.Close(); _fs.Dispose(); } catch { return "-1"; } return "-1"; }
            try
            {
                try { _fs.Close(); _fs.Dispose(); }
                catch { return "-1"; }
                File.Copy(HttpContext.Current.Server.MapPath("~") + @"files\temp\" + tempFileName, pathtoDb, true);
            }
            catch { return "-1"; }
            try
            {
                File.Delete(HttpContext.Current.Server.MapPath("~") + @"files\temp\" + tempFileName);
            }
            catch { }

            return resultCount;
        }

        #endregion
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>класс, который описывает структу данных одного статистического события, записываемого в журнал посещения консоли администрирования (site\files\adm\jrl\jrl_*)</summary>
    public class StatStruct
    {
        public long Ticks { get; set; }                 //свойство содержит тики, по которым можно вычислить дату и время записи события статистики
        public string RawUrl { get; set; }              //свойство содержит часть URL страницы сайта, на который был переход. URL вида - /infoone.aspx?newsid=8
        public string HostIp { get; set; }              //свойство содержит ip-адрес узла, с которого было обращение к странице (например, 192.168.0.2)
        public string HostName { get; set; }            //свойство содержит имя узла, с которого было обращение к странице (например, testcomp.test.ru)
        public string AgentInfo { get; set; }           //свойство содержит информацию о клиенте, с которого было обращение к странице (например, Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko)
    }

    /// <summary>Класс опысывает структу данных по статистике для одной новостной страницы. Статистика вида ЗАГОЛОВОК И ССЫЛКА НА НОВОСТЬ - КОЛ-ВО ПЕРЕХОДОВ НА СТРАНИЦУ</summary>
    public class NewsStatStruct
    {
        public string NewsTitle { get; set; }           //заголовок новости
        public string RawUrl { get; set; }              //ссылка на страницу новости вида  - /infoone.aspx?newsid=8
        public int TransitionsNum { get; set; }         //количество переходов на страницу новости
    }
    /// <summary>Класс опысывает вспомогательную структу данных по статистике для одной новостной страницы. Статистика вида ЗАГОЛОВОК И ССЫЛКА НА НОВОСТЬ - КОЛ-ВО ПЕРЕХОДОВ НА СТРАНИЦУ</summary>
    public class NewsStatStructTemp
    {
        public int NewsId { get; set; }                 //id новости
        public int TransitionsNum { get; set; }         //количество переходов на страницу новости
    }

    /// <summary>Класс описывает структуру данных по лайкам для опроса. У опроса есть своё имя.</summary>
    public class LikesStruct
    {
        public string likeName { get; set; }            //имя опроса
        public int likeCount { get; set; }              //количество голос - НРАВИТСЯ
        public int nolikeCount { get; set; }            //количество голосов - НЕ НРАВИТСЯ

        /// <summary>функция возвращает список в формате String из этой структуры опроса. 
        /// Строка в списке полностью подготовлена для записи в файл данных по опросам (файл likes находится в папке /files/adm/likes/)</summary>
        /// <returns>возвращает строку в формате - survey|/имя опроса/|/кол-во нравится/|/кол-во ненравится/</returns>
        public String GetStringFromStruct()
        {
            return "survey|" + likeName + "|" + likeCount + "|" + nolikeCount;
        }
    }

    #endregion

}