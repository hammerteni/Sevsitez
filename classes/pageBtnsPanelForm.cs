using System.Web.UI.WebControls;

namespace site.classes
{
    /// <summary>это универсальный класс, конструктор которого принимает страницу (pagnew), общее число элементов в каком-либо списке(CountOfAllElementnew),
    /// номер открытой страницы (PageNumberCurrentnew), кол-во элементов на одной странице (CountOfElemInOnePagenew)
    /// Результатом работы данного класса является выведение таблицы с кнопка-ссылками перехода между страницами и изменение только одной переменной - 
    /// Session["pagenum"]. То бишь изменяется номер открываемой страницы в списке товаров.
    /// GetPageBtnsTbl(string index) - возвращает таблицу с кнопками-ссылками перехода между страницами товаров. index - нужно передать любой симвом(желательно цифру),
    /// параметр нужен для того, чтобы можно было добавить несколько таких таблиц на одну страницу. Естественно значение index для каждой таблицы должно быть разным </summary>
    public class PageBtnsPanelForm
    {
        private System.Web.UI.Page pag;
        private int CountOfAllElement;
        private int PageNumberCurrent;
        private int CountOfElemInOnePage;

        public PageBtnsPanelForm(System.Web.UI.Page pagnew, int CountOfAllElementnew, int PageNumberCurrentnew, int CountOfElemInOnePagenew)
        {
            pag = pagnew;
            CountOfAllElement = CountOfAllElementnew;
            PageNumberCurrent = PageNumberCurrentnew;
            CountOfElemInOnePage = CountOfElemInOnePagenew;
        }

        /// <summary>функция возвращает таблицу с панелькой, содержащей кнопки, дающие возможность перелистывать страницы</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Table GetPageBtnsTbl(string index)
        {
            #region МОДУЛЬ расчётов кол-ва страниц и т.п.
            //определим кол-во страниц - pageCount
            int pageCount = CountOfAllElement / CountOfElemInOnePage;
            if (pageCount == 0)
            {
                pageCount = 1;
            }
            else if (pageCount > 0)
            {
                if ((CountOfAllElement - (pageCount * CountOfElemInOnePage)) > 0)
                {
                    pageCount = pageCount + 1;
                }
            }
            //УСЛОВИЕ-ПОВЕРКА для того, чтобы цифра активной страницы не превышала возможное число
            //страницы для данного списка товаров(эта ситуация может возникнуть при перемещении между 
            //страницами каталога)
            if(PageNumberCurrent > pageCount)
            {
                PageNumberCurrent = 1; pag.Session["pagenum"] = 1;
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

                if (i == PageNumberCurrent)
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

            lbl = new Label(); lbl.CssClass = "spanNumPageLBtns";
            lbl.Text = PageNumberCurrent.ToString(); tblCell.Controls.Add(lbl);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вначало списка
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns lBtnHover";
            lBtn.Text = "<<";
            lBtn.CommandArgument = "1";
            lBtn.Command += new CommandEventHandler(lBtnBegin_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания влево на один модуль
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns lBtnHover";
            lBtn.Text = "<";
            if (currentBtnModuleNumber == 1)
            {
                lBtn.CommandArgument = PageNumberCurrent.ToString();
            }
            else if (currentBtnModuleNumber > 1)
            {
                lBtn.CommandArgument = ((currentBtnModuleNumber - 1) * 9 - 8).ToString();     //вычисляем номер первой страницы в предыдущем модуле, она и станет активной после перехода
            }
            lBtn.Command += new CommandEventHandler(lBtnLeft_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопками страниц
            tblRow.Controls.Add(tblCell);

            for (int i = currentBtnModuleNumber * 9 - 8; i <= ((currentBtnModuleNumber * 9) - (9 - currBtnModuleCapacity)); i++)
            {
                if (i == PageNumberCurrent)     //если нужно добавить активную кнопку, то..
                {
                    lBtn = new LinkButton();
                    lBtn.CssClass = "spanNumPageLBtns lBtnHover";
                    lBtn.Text = i.ToString();
                    lBtn.ID = "lBtn_" + i + "_" + index;
                    lBtn.CommandArgument = i.ToString();
                    lBtn.Command += new CommandEventHandler(lBtn_Command);
                    tblCell.Controls.Add(lBtn);
                }
                else                            //если нужно добавить обычную кнопку, то..
                {
                    lBtn = new LinkButton();
                    lBtn.CssClass = "spanNumPageLBtns lBtnHover";
                    lBtn.Text = i.ToString();
                    lBtn.ID = "lBtn_" + i + "_" + index;
                    lBtn.CommandArgument = i.ToString();
                    lBtn.Command += new CommandEventHandler(lBtn_Command);
                    tblCell.Controls.Add(lBtn);
                }
            }

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вправо на один модуль
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns lBtnHover";
            lBtn.Text = ">";
            if (currentBtnModuleNumber == countOfBtnModule)
            {
                lBtn.CommandArgument = PageNumberCurrent.ToString();
            }
            else if (currentBtnModuleNumber < countOfBtnModule)
            {
                lBtn.CommandArgument = ((currentBtnModuleNumber + 1) * 9 - 8).ToString();     //вычисляем номер первой страницы в следующем модуле, она и станет активной после перехода
            }
            lBtn.Command += new CommandEventHandler(lBtnRight_Command);
            tblCell.Controls.Add(lBtn);

            tblCell = new TableCell();    //ячейка с кнопкой перелистывания вконец списка
            tblRow.Controls.Add(tblCell);

            lBtn = new LinkButton();
            lBtn.CssClass = "spanArrowPageLBtns lBtnHover";
            lBtn.Text = ">>";
            lBtn.CommandArgument = pageCount.ToString();
            lBtn.Command += new CommandEventHandler(lBtnEnd_Command);
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
            lbl.CssClass = "spanNumPageLBtns";
            lbl.Text = pageCount.ToString();
            tblCell.Controls.Add(lbl);

            return tbl;
        }

        protected void lBtnBegin_Command(object sender, CommandEventArgs e)
        {
            var parsestring = new classesHelp.ParseStringToIntClass(e.CommandArgument.ToString());
            pag.Session["pagenum"] = parsestring.result;
            parsestring = null;
            //перезагрузка страницы со всеми параметрами текущего URL
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        protected void lBtnEnd_Command(object sender, CommandEventArgs e)
        {
            var parsestring = new classesHelp.ParseStringToIntClass(e.CommandArgument.ToString());
            pag.Session["pagenum"] = parsestring.result;
            parsestring = null;
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        protected void lBtnLeft_Command(object sender, CommandEventArgs e)
        {
            var parsestring = new classesHelp.ParseStringToIntClass(e.CommandArgument.ToString());
            pag.Session["pagenum"] = parsestring.result;
            parsestring = null;
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        protected void lBtnRight_Command(object sender, CommandEventArgs e)
        {
            var parsestring = new classesHelp.ParseStringToIntClass(e.CommandArgument.ToString());
            pag.Session["pagenum"] = parsestring.result;
            parsestring = null;
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        protected void lBtn_Command(object sender, CommandEventArgs e)
        {
            var parsestring = new classesHelp.ParseStringToIntClass(e.CommandArgument.ToString());
            pag.Session["pagenum"] = parsestring.result;
            parsestring = null;
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }
    }
}