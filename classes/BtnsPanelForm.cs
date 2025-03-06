using System.Web;
using System.Web.UI.WebControls;

namespace site.classes
{
    /// <summary>Универсальный класс, конструктор которого принимает общее число элементов которые будут на странице(CountOfAllElementnew),
    /// кол-во элементов на одной странице (CountOfElemInOnePagenew)
    /// Результатом работы данного класса является выведение таблицы с кнопка-ссылками перехода между страницами и изменение только одной переменной - 
    /// Session["pagenum"]. То бишь изменяется номер открываемой страницы в списке товаров.
    /// GetPageBtnsTbl(string index) - возвращает таблицу с кнопками-ссылками перехода между страницами товаров. index - нужно передать любой симвом(желательно цифру),
    /// параметр нужен для того, чтобы можно было добавить несколько таких таблиц на одну страницу. Естественно значение index для каждой таблицы должно быть разным </summary>
    public class PagePanelClass
    {
        private int _countOfAllElement;
        private int _pageNumberCurrent;
        private int _countOfElemInOnePage;
        private string _dopId = "";        // нужна для того, чтобы можно было на одной странице по несколько таких компонентов добавлять, не зависящих от одной Session["pagenum"]
        private HttpContext _context = HttpContext.Current;

        /// <summary>Конструктор класса</summary>
        /// <param name="сountOfAllElement">кол-во всех элементов в списке</param>
        /// <param name="сountOfElemInOnePage">кол-во элементов, отображаемых на одной странице</param>
        /// <param name="dopId">нужна для того, чтобы можно было на одной странице по несколько таких компонентов добавлять, не зависящих от одной Session["pagenum"]</param>
        public PagePanelClass(int сountOfAllElement, int сountOfElemInOnePage, string dopId = "")
        {
            _countOfAllElement = сountOfAllElement;
            _countOfElemInOnePage = сountOfElemInOnePage;
            _dopId = dopId;
            // инициализация переменной в случае её отсутствия
            if (_context.Session["pagenum" + _dopId] != null) 
                _pageNumberCurrent = (int)_context.Session["pagenum" + _dopId];
            else
                _context.Session["pagenum" + _dopId] = 1;
        }

        /// <summary>Метод возвращает html-код, содержащий кнопки, дающие возможность "перелистывать" страницы</summary>
        /// <param name="index">уникальный id таблицы с кнопками</param>
        /// <returns></returns>
        public Table GetPageBtnsTbl(string index)
        {
            #region МОДУЛЬ расчётов кол-ва страниц и т.п.

            //определим кол-во страниц - pageCount
            int pageCount = _countOfAllElement / _countOfElemInOnePage;
            if (pageCount == 0)
            {
                pageCount = 1;
            }
            else if (pageCount > 0)
            {
                if ((_countOfAllElement - (pageCount * _countOfElemInOnePage)) > 0)
                {
                    pageCount = pageCount + 1;
                }
            }
            //УСЛОВИЕ-ПОВЕРКА для того, чтобы цифра активной страницы не превышала возможное число
            //страницы для данного списка товаров(эта ситуация может возникнуть при перемещении между 
            //страницами каталога)
            if (_pageNumberCurrent > pageCount)
            {
                _pageNumberCurrent = 1; _context.Session["pagenum" + _dopId] = 1;
            }

            //определим кол-во модулей кнопок (пример модуля кнопок: <<  <  1 2 3 4 5 6 7 8 9  >  >>) с которыми будем далее работать
            int countOfBtnModule = pageCount / 9;       //цифра девять определяет кол-во кнопок(для перехода на другие страницы) в одном модуле кнопок(пример модуля кнопок: <<  <  1 2 3 4 5 6 7 8 9  >  >>)
            if (countOfBtnModule == 0)
            {
                countOfBtnModule = 1;
            }
            else if (countOfBtnModule > 0)
            {
                if ((pageCount - (countOfBtnModule * 9)) > 0)
                {
                    countOfBtnModule = countOfBtnModule + 1;
                }
            }
            //определим, в каком конкретно кнопочном модуле сейчас находится текущая страница
            int currentBtnModuleNumber = 1;
            int counter = 0;
            int modulenum = 1;
            for (int i = 1; i <= pageCount; i++)
            {
                counter++;
                if (counter == 10)
                {
                    modulenum++;
                    counter = 1;
                }

                if (i == _pageNumberCurrent)
                {
                    currentBtnModuleNumber = modulenum;
                    break;
                }
            }
            //определяем количество кнопок-ссылок на страницы в текущем модуле (необходимо для цикла добавления кнопок-ссылок в модуль)
            int currBtnModuleCapacity = 0;
            if (currentBtnModuleNumber < countOfBtnModule)
            {
                currBtnModuleCapacity = 9;
            }
            else if (currentBtnModuleNumber == countOfBtnModule)
            {
                currBtnModuleCapacity = pageCount - ((currentBtnModuleNumber - 1) * 9);
            }

            #endregion

            var lBtn = new LinkButton();

            var tbl = new Table(); tbl.CssClass = "tdPageLBtnsMain";

            //строка с панелькой, содержащей кнопки перелистывания страниц  
            var tblRow = new TableRow(); tbl.Controls.Add(tblRow);

            //ячейка с "Текущая страница - "    
            var tblCell = new TableCell(); tblRow.Controls.Add(tblCell);

            var lbl = new Label(); lbl.CssClass = "spanTxtPageLBtns";
            lbl.Text = "Текущая страница - "; tblCell.Controls.Add(lbl);

            //ячейка с номером текущей страницы 
            tblCell = new TableCell(); tblRow.Controls.Add(tblCell);

            lbl = new Label(); lbl.CssClass = "spanNumPage";
            lbl.Text = (_pageNumberCurrent == 0 ? 1 : _pageNumberCurrent).ToString(); tblCell.Controls.Add(lbl);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вначало списка
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns buttonsHover";
            lBtn.Text = "<<";
            lBtn.CommandArgument = "1";
            lBtn.Command += new CommandEventHandler(lBtnPage_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания влево на один модуль
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns buttonsHover";
            lBtn.Text = "<";
            if (currentBtnModuleNumber == 1)
            {
                lBtn.CommandArgument = _pageNumberCurrent.ToString();
            }
            else if (currentBtnModuleNumber > 1)
            {
                lBtn.CommandArgument = ((currentBtnModuleNumber - 1) * 9 - 8).ToString();     //вычисляем номер первой страницы в предыдущем модуле, она и станет активной после перехода
            }
            lBtn.Command += new CommandEventHandler(lBtnPage_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопками страниц
            tblRow.Controls.Add(tblCell);

            for (int i = currentBtnModuleNumber * 9 - 8; i <= ((currentBtnModuleNumber * 9) - (9 - currBtnModuleCapacity)); i++)
            {
                if (i == _pageNumberCurrent)     //если нужно добавить активную кнопку, то..
                {
                    lBtn = new LinkButton();
                    lBtn.CssClass = "spanNumPageLBtns buttonsHover hlinkHover";
                    lBtn.Text = i.ToString();
                    lBtn.ID = "lBtn_" + i + "_" + index;
                    lBtn.CommandArgument = i.ToString();
                    lBtn.Command += new CommandEventHandler(lBtnPage_Command);
                    tblCell.Controls.Add(lBtn);
                }
                else                            //если нужно добавить обычную кнопку, то..
                {
                    lBtn = new LinkButton();
                    lBtn.CssClass = "spanNumPageLBtns buttonsHover hlinkHover";
                    lBtn.Text = i.ToString();
                    lBtn.ID = "lBtn_" + i + "_" + index;
                    lBtn.CommandArgument = i.ToString();
                    lBtn.Command += new CommandEventHandler(lBtnPage_Command);
                    tblCell.Controls.Add(lBtn);
                }
            }

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вправо на один модуль
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns buttonsHover";
            lBtn.Text = ">";
            if (currentBtnModuleNumber == countOfBtnModule)
            {
                lBtn.CommandArgument = _pageNumberCurrent.ToString();
            }
            else if (currentBtnModuleNumber < countOfBtnModule)
            {
                lBtn.CommandArgument = ((currentBtnModuleNumber + 1) * 9 - 8).ToString();     //вычисляем номер первой страницы в следующем модуле, она и станет активной после перехода
            }
            lBtn.Command += new CommandEventHandler(lBtnPage_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вконец списка
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns buttonsHover";
            lBtn.Text = ">>";
            lBtn.CommandArgument = pageCount.ToString();
            lBtn.Command += new CommandEventHandler(lBtnPage_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с "Всего страниц - "
            tblRow.Controls.Add(tblCell);

            lbl = new Label();
            lbl.CssClass = "spanTxtPageLBtns";
            lbl.Text = "Всего страниц - ";
            tblCell.Controls.Add(lbl);

            tblCell = new TableCell();    //ячейка с общим кол-вом страниц
            tblRow.Controls.Add(tblCell);

            lbl = new Label();
            lbl.CssClass = "spanNumPage";
            lbl.Text = pageCount.ToString();
            tblCell.Controls.Add(lbl);

            return tbl;
        }

        /// <summary>Событие нажатия на любую кнопку</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержим номер страницы, на которую нужно перейти</param>
        protected void lBtnPage_Command(object sender, CommandEventArgs e)
        {
            _context.Session["pagenum" + _dopId] = int.Parse(e.CommandArgument.ToString());
            _context.Response.Redirect(_context.Request.RawUrl);
            //if (_context.Request.ServerVariables["QUERY_STRING"] == "") _context.Response.Redirect(_context.Request.ServerVariables["URL"]);
            //else _context.Response.Redirect(_context.Request.ServerVariables["URL"] + "?" + _context.Request.ServerVariables["QUERY_STRING"]);
        }
    }
}