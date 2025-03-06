using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using site.classesHelp;

/* файл с классами для работы с административными учётными записями сайта и журнала посещения сайта и консоли управления (файлы в папке files\adm\) */
namespace site.classes
{
    #region Код формирования HTML-кода     --------------------------------------------

    /// <summary>класс вывода HTML-кода для работы с данными пользователей</summary>
    public class UsersForm
    {
        #region Поля

        private Page _pag;

        #endregion

        #region Конструктор        

        public UsersForm(Page pagnew)
        {
            _pag = pagnew;
        }

        #endregion

        #region Метод GetAdmLoginPanel()

        /// <summary>Метод формирования ОКНА АВТОРИЗАЦИИ в консоль управления порталом</summary>
        public Panel GetAdmLoginPanel()
        {
            var panelWrapper = new Panel();

            #region Код добавления содержимого

            var lbl = new Label(); lbl.CssClass = "loginTitleTxt";
            lbl.Text = "Авторизация";
            panelWrapper.Controls.Add(lbl);

            lbl = new Label(); lbl.CssClass = "txtLblForTxtBox"; lbl.Text = "Имя:";
            panelWrapper.Controls.Add(lbl);

            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm txtBoxLogin"; txtBox.MaxLength = 80;
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.ID = "txtBoxLoginL";
            panelWrapper.Controls.Add(txtBox);

            lbl = new Label(); lbl.CssClass = "txtLblForTxtBox"; lbl.Text = "Пароль:";
            panelWrapper.Controls.Add(lbl);

            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm txtBoxLogin"; txtBox.MaxLength = 80;
            txtBox.TextMode = TextBoxMode.Password; txtBox.ID = "txtBoxLoginP";
            panelWrapper.Controls.Add(txtBox);

            var lBtn = new LinkButton();
            lBtn.CssClass = "buttonsHover lBtnsUniverse lBtnsLogin"; lBtn.Text = "ВОЙТИ"; lBtn.ID = "btnEnter";
            lBtn.Command += new CommandEventHandler(lBtnAdmLogin_Command);
            panelWrapper.Controls.Add(lBtn);

            #endregion

            //устанавливаем кнопку по умолчанию и фокус по умолчанию
            ((System.Web.UI.HtmlControls.HtmlForm)_pag.FindControl("form1")).DefaultButton = "btnEnter";
            ((System.Web.UI.HtmlControls.HtmlForm)_pag.FindControl("form1")).DefaultFocus = "txtBoxLoginL";

            return panelWrapper;
        }

        #region События

        #region lBtnAdmLogin_Command

        /// <summary>событие нажатия на кнопку ВОЙТИ на странице as.aspx</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAdmLogin_Command(object sender, CommandEventArgs e)
        {
            string l = ((TextBox)_pag.FindControl("txtBoxLoginL")).Text;
            string p = ((TextBox)_pag.FindControl("txtBoxLoginP")).Text;
            var warn = new WarnClass();
            if (l == "" || p == "")
            {
                warn.ShowPageWarning("ВНИМАНИЕ. Введено неправильное значение логина или пароля.. Попробуйте повторить.", _pag);
                return;
            }
            else if (l == "" && p == "")
            {
                warn.ShowPageWarning("ВНИМАНИЕ. Введено неправильное значение логина или пароля.. Попробуйте повторить.", _pag);
                return;
            }
            else
            {
                warn.HidePageWarning(_pag);
            }

            LoginWorkClass loginwork = new LoginWorkClass(_pag);
            if (loginwork.CheckAuth(l, p))   //если успешная авторизация, то..
            {
                #region COOKIE

                CookieSession cookie = new CookieSession();
                cookie.AuthAAdd(l, p);

                #endregion

                warn.HidePageWarning(_pag);
                loginwork.VizitAccounting(l, true);   //запишем событие входа в журнал
                if (_pag.Session["authperson"] != null)
                {
                    if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) ||
                        ((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.pagesEditor))
                    {
                        _pag.Response.Redirect("~/adm/pageseditor.aspx");
                    }
                    else if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.newsEditor))
                    {
                        _pag.Response.Redirect("~/adm/news.aspx");
                    }
                    else if (EnumsHelper.GetWritesCodeFromValue(((AdmPersonStruct)_pag.Session["authperson"]).Writes) != "")    //услувие для всех редакторов конкурсов
                    {
                        _pag.Response.Redirect("~/adm/competitions.aspx");
                    }
                }
            }
            else
            {
                warn.ShowPageWarning("ВНИМАНИЕ. Введено неправильное значение логина или пароля.. Попробуйте повторить.", _pag);
                loginwork.VizitAccounting(l, false);   //запишем событие отказа в журнал
            }
        }

        #endregion

        #endregion

        #endregion

        #region МЕТОДЫ, КАСАЮЩИЕСЯ ОТОБРАЖЕНИЯ РЕДАКТОРА И СПИСКА УЧЁТНЫХ ЗАПИСЕЙ

        #region Метод GetAccountTitlePanel()

        /// <summary>функция возвращает таблицу с кнопкой создания новой учётной записи</summary>
        /// <returns></returns>
        public Panel GetAccountTitlePanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР УЧЁТНЫХ ЗАПИСЕЙ КОНСОЛИ"; panelWrapper.Controls.Add(lbl);

            //Кнопка создания новой учётной записи
            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " + создать новую учётную запись ";
                lBtn.Command += (lBtnCreateAccount_Command); panelWrapper.Controls.Add(lBtn);
            }

            return panelWrapper;
        }

        #region События

        /// <summary>событие нажатия на кнопку СОЗДАТЬ НОВУЮ УЧЁТНУЮ ЗАПИСЬ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCreateAccount_Command(object sender, CommandEventArgs e)
        {
            var admPersonStruct = new AdmPersonStruct();
            admPersonStruct.Name = "";
            admPersonStruct.Pass = "";
            admPersonStruct.Writes = EnumsHelper.GetWritesValue(Writes.newsEditor);
            admPersonStruct.About = "";
            _pag.Session["TempAccount"] = admPersonStruct;

            _pag.Response.Redirect("/adm/usersedit.aspx");
        }

        #endregion

        #endregion
        #region Метод GetAccountListTbl()

        /// <summary>функция возвращает таблицу со всеми учётными записями(кроме записи tech)</summary>
        /// <returns></returns>
        public Table GetAccountListTbl()
        {
            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell();
            Label lbl;

            var loginWorkClass = new LoginWorkClass(_pag);
            var listOfStructs = loginWorkClass.GetAllAdmStructs();

            //добавим надпись - всего товара разделе/подразделе
            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                tblRow = new TableRow(); tbl.Controls.Add(tblRow);
                tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblCell.ColumnSpan = 10; tblRow.Controls.Add(tblCell);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего учётных записей: "; tblCell.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPosleslovie";
                lbl.Text = listOfStructs.Count.ToString();
                tblCell.Controls.Add(lbl);
            }

            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

            //добавляем подтаблицу
            var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

            //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ
            var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
            var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Имя(логин)"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Права"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Описание"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);

            //добавляем строки с данными по учётным записям
            foreach (AdmPersonStruct oneStruct in listOfStructs)
            {
                if (oneStruct.Name != "tech")   //отсекаем отображения учётной записи tech
                {
                    //РАЗГРАНИЧЕНИЕ ДОСТУПА
                    if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == "администратор")
                    {
                        tblRow1 = new TableRow();

                        tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //имя(логин)
                        lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Name; tblCell1.Controls.Add(lbl);

                        tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //права
                        tblCell1.Attributes.Add("style", "text-align: left;");
                        lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Writes; tblCell1.Controls.Add(lbl);

                        tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //описание
                        tblCell1.Attributes.Add("style", "text-align: justify;");
                        lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.About; tblCell1.Controls.Add(lbl);

                        //Кнопка РЕДАКТИРОВАТЬ  
                        tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                        var lBtn = new LinkButton(); lBtn.CommandArgument = oneStruct.Name; lBtn.Command += new CommandEventHandler(lBtnEditAccount_Command);
                        lBtn.ToolTip = "редактировать учётную запись";
                        var img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/edit.png";
                        lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);

                        //Кнопка УДАЛИТЬ
                        tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                        if (oneStruct.Name != "admin")  //админа удалять нельзя:-)
                        {
                            lBtn = new LinkButton(); /*lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " Удалить ";*/
                            lBtn.CommandArgument = oneStruct.Name; lBtn.Command += (lBtnDelAccount_Command);
                            lBtn.OnClientClick = "return confirm('Учётная запись будет полностью удалена из базы данных. Удалить?');";
                            lBtn.ToolTip = "удалить учётную запись";
                            img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/krestik.png";
                            lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);
                        }

                        tbl1.Controls.Add(tblRow1);
                    }
                    else
                    {
                        if (((AdmPersonStruct)_pag.Session["authperson"]).Name == oneStruct.Name)   //разрешаем вошеднему пользователю редактировать только свою учётку
                        {
                            tblRow1 = new TableRow();

                            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //имя(логин)
                            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Name; tblCell1.Controls.Add(lbl);

                            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //права
                            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Writes; tblCell1.Controls.Add(lbl);

                            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);                     //описание
                            lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.About; tblCell1.Controls.Add(lbl);

                            //Кнопка РЕДАКТИРОВАТЬ  
                            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);
                            var lBtn = new LinkButton(); lBtn.CommandArgument = oneStruct.Name; lBtn.Command += new CommandEventHandler(lBtnEditAccount_Command);
                            lBtn.ToolTip = "редактировать учётную запись";
                            var img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse"; img.ImageUrl = "~/images/edit.png";
                            lBtn.Controls.Add(img); tblCell1.Controls.Add(lBtn);

                            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);   //пустая ячейка.

                            tbl1.Controls.Add(tblRow1);
                        }
                    }
                }
            }

            return tbl;
        }

        #region СОБЫТИЯ

        /// <summary>событие нажатия на кнопку редактирования или просмотра сведений об учётной записи</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержим имя(логин) учётной записи</param>
        protected void lBtnEditAccount_Command(object sender, CommandEventArgs e)
        {
            var loginWorkClass = new LoginWorkClass(_pag);
            _pag.Session["TempAccount"] = loginWorkClass.GetOneAdmStructsForName(e.CommandArgument.ToString());

            _pag.Response.Redirect("/adm/usersedit.aspx");
        }

        /// <summary>событие нажатия на кнопку УДАЛИТЬ учётную запись</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержим имя(логин) учётной записи</param>
        protected void lBtnDelAccount_Command(object sender, CommandEventArgs e)
        {
            var loginWorkClass = new LoginWorkClass(_pag);

            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            //удалим данные учётной записи
            if (!loginWorkClass.ReplaceOrDeleteAccount(loginWorkClass.GetOneAdmStructsForName(e.CommandArgument.ToString()), true))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления учётной записи. Попробуйте повторить.", _pag.Master);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Удалена учетная запись администратора/редактора - " + e.CommandArgument.ToString() + ". Удалил - " + ((AdmPersonStruct)_pag.Session["authperson"]).Name);
                _pag.Response.Redirect("/adm/users.aspx");
            }
        }

        #endregion

        #endregion
        #region Метод GetOneAccountEditPanel()

        /// <summary>функция возвращает таблицу просмотра и редактирования одной административной учётной записи</summary>
        /// <returns></returns>
        public Panel GetOneAccountEditPanel()
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panWrap";

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР ОДНОЙ УЧЁТНОЙ ЗАПИСИ"; panelWrapper.Controls.Add(lbl);

            //имя(логин) учётной записи
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Имя учётной записи (логин)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ToolTip = "Имя учётной записи. Его изменить нельзя.";
            if (((AdmPersonStruct)_pag.Session["TempAccount"]).Name == "") { txtBox.ReadOnly = false; } else { txtBox.ReadOnly = true; }
            txtBox.Text = ((AdmPersonStruct)_pag.Session["TempAccount"]).Name; txtBox.ID = "txtBoxAccL"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //пароль
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Пароль учётной записи "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ToolTip = "Пароль для учётной записи. Оставьте это поле пустым, если хотите оставить прежний пароль. Пароль должен быть не менее 8-ми символов";
            txtBox.Text = ""; txtBox.ID = "txtBoxAccP"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //права
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Права учётной записи"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            var ddl = new DropDownList(); ddl.CssClass = "txtBoxUniverse_adm";
            ddl.ToolTip = "Выберите права, которые нужно присвоить учётной записи.";
            string actualRights = ((AdmPersonStruct)_pag.Session["TempAccount"]).Writes;
            foreach (Writes wr in Enum.GetValues(typeof(Writes)))
            {
                ddl.Items.Add(EnumsHelper.GetWritesValue(wr));
            }
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.admin));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.newsEditor));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.pagesEditor));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorPhoto));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorPhotoIzo));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorPhotoCompGraphic));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorDPI1));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorDPI2));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorLiterary));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatreHudSlovo));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatre));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatreHoreo));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatreVokal));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatreInstrumZanr));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorTheatreModa));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorKultura));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorSport));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorThekvo));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorBoks));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorKungfu));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorFootball));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorPaintball));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorShahmaty));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorShashky));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorToponim));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorRobotech));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorVmesteSila));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorClothes));
            //ddl.Items.Add(EnumsHelper.GetWritesValue(Writes.editorMultimedia));
            int counter = 0;
            foreach (ListItem nameOfRight in ddl.Items)
            {
                if (nameOfRight.Text == actualRights) { ddl.SelectedIndex = counter; break; }
                counter++;
            }
            ddl.ID = "ddlRights";
            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes != EnumsHelper.GetWritesValue(Writes.admin))
            {
                ddl.Enabled = false;
            }
            panelLine.Controls.Add(ddl);
            panelWrapper.Controls.Add(panelLine);

            //описание
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Описание учётной записи"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 250; txtBox.Height = 50; txtBox.TextMode = TextBoxMode.MultiLine;
            txtBox.ToolTip = "Описание учётной записи. Здесь можно вводить любую информацию, касающуюся учётной записи.";
            txtBox.Text = ((AdmPersonStruct)_pag.Session["TempAccount"]).About;
            txtBox.ID = "txtBoxAccDescr"; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //кнопки сохранения и удаления учётной записи
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Создаётся новая новость на основе данных этой новости(но без фото и файлов).";
            lBtn.Command += (lBtnSaveAccount_Command); lBtn.ToolTip = "Кнопка сохранения данных административной учётной записи.";
            panelLine.Controls.Add(lBtn);
            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)_pag.Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                if (((AdmPersonStruct)_pag.Session["TempAccount"]).Name != "" && ((AdmPersonStruct)_pag.Session["TempAccount"]).Name != EnumsHelper.GetWritesCode(Writes.admin))
                {
                    lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " УДАЛИТЬ ";
                    lBtn.ToolTip = "Кнопка удаления данных административной учётной записи.";
                    lBtn.OnClientClick = "return confirm('Учётная запись будет полностью удалена. Удалить?');";
                    lBtn.Command += (lBtnDeleteAccount_Command); panelLine.Controls.Add(lBtn);
                }
            }
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region СОБЫТИЯ

        #region lBtnSaveAccount_Command

        /// <summary>событие нажатия на кнопку СОХРАНИТЬ учётную запись</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSaveAccount_Command(object sender, CommandEventArgs e)
        {
            var loginWorkClass = new LoginWorkClass(_pag);

            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            string txtBoxAccL = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccL")).Text;
            string txtBoxAccP = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccP")).Text;
            string ddlRights = ((DropDownList)_pag.FindControl("ctl00$ContentPlaceHolder1$ddlRights")).SelectedValue;
            string txtBoxAccDescr = ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccDescr")).Text;

            //проверочные условия
            //в случае добавления новой учётной записи, проверим её имя на совпадения имён
            if (((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccL")).ReadOnly == false)
            {
                if (loginWorkClass.IsLoginExists(txtBoxAccL)) { warning.ShowWarning("ВНИМАНИЕ. Учётная запись с таким именем уже существует, придумайте другое имя.", _pag.Master); return; }
            }

            if (txtBoxAccL.Length < 3) { warning.ShowWarning("ВНИМАНИЕ. Имя пользователя не должно быть менее 3-х символов.", _pag.Master); return; }
            if (txtBoxAccL == EnumsHelper.GetWritesCode(Writes.admin) && ddlRights != EnumsHelper.GetWritesValue(Writes.admin)) { warning.ShowWarning("ВНИМАНИЕ. У этой учетной записи нельзя изменять права. Могут быть только права - АДМИНИСТРАТОР.", _pag.Master); return; }
            //if (txtBoxAccL != "admin" && ddlRights == "администратор") { warning.ShowWarning("ВНИМАНИЕ. Может быть только одна учётная запись с правами - АДМИНИСТРАТОР.", _pag.Master); return; }
            //если пароль не пустой, или мы создаём новую учётную запись, то проверяем длину пароля
            if (txtBoxAccP != "" || ((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccL")).ReadOnly == false)
            {
                if (txtBoxAccL == EnumsHelper.GetWritesCode(Writes.admin) && ((AdmPersonStruct)_pag.Session["authperson"]).Name != EnumsHelper.GetWritesCode(Writes.admin))
                {
                    warning.ShowWarning("ВНИМАНИЕ. Пароль для этой учётной записи может изменить только сам admin..", _pag.Master); return;
                }
                if (txtBoxAccP.Length < 6) { warning.ShowWarning("ВНИМАНИЕ. Пароль должен содержать не меньше 6-ти символов.", _pag.Master); return; }
                //условие нужно для того, чтобы никто, кроме admin, не смог изменить его пароль
            }

            //сохраним новые данные в структуру учётной записи
            //в случае добавления новой учётной записи, сохраним ещё и имя учётной записи
            if (((TextBox)_pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAccL")).ReadOnly == false)
            {
                ((AdmPersonStruct)_pag.Session["TempAccount"]).Name = txtBoxAccL;
            }
            if (txtBoxAccP != "") { ((AdmPersonStruct)_pag.Session["TempAccount"]).Pass = txtBoxAccP; }
            ((AdmPersonStruct)_pag.Session["TempAccount"]).Writes = ddlRights;
            ((AdmPersonStruct)_pag.Session["TempAccount"]).About = txtBoxAccDescr;

            //перезапишем данные учётной записи
            if (!loginWorkClass.ReplaceOrDeleteAccount((AdmPersonStruct)_pag.Session["TempAccount"]))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения данных учётной записи. Попробуйте повторить.", _pag.Master);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена учетная запись администратора/редактора - " + ((AdmPersonStruct)_pag.Session["TempAccount"]).Name + ". Изменил - " + ((AdmPersonStruct)_pag.Session["authperson"]).Name);
            }
        }

        #endregion
        #region lBtnDeleteAccount_Command

        /// <summary>событие нажатия на кнопку УДАЛИТЬ учётную запись</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnDeleteAccount_Command(object sender, CommandEventArgs e)
        {
            var loginWorkClass = new LoginWorkClass(_pag);

            var warning = new WarnClass();
            warning.HideWarning(_pag.Master);

            //удалим данные учётной записи
            if (!loginWorkClass.ReplaceOrDeleteAccount((AdmPersonStruct)_pag.Session["TempAccount"], true))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления учётной записи. Попробуйте повторить.", _pag.Master);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Удалена учетная запись администратора/редактора - " + ((AdmPersonStruct)_pag.Session["TempAccount"]).Name + ". Удалил - " + ((AdmPersonStruct)_pag.Session["authperson"]).Name);
                _pag.Response.Redirect("/adm/users.aspx");
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }

    #endregion

    #region Код работы с данными      --------------------------------------------

    /// <summary>класс предоставляет методы и функции для работы с файлом авторизации(/files/adm/lp), журналом регистрации(/files/adm/jrl/jrl_*) консоли управления
    /// структура файла авторизации:
    /// begin|  - начало данных учётки
    /// l|      - имя
    /// p|      - пароль
    /// w|      - уровень доступа (администратор, редактор страниц, редактор новостей, редактор фотоконкурса, редактор литконкурса, редактор театрконкурса)
    /// end|    - конец данных учётки
    /// все данные этого файла закодированы
    /// </summary>
    public class LoginWorkClass
    {
        private Page _pag;
        private int _listIdToInsert = -1;            //в эту переменную записывается индекс исключённого из списка элемента(исключается в функции GetAllNewsStructs)

        public LoginWorkClass(Page pagnew) { _pag = pagnew; }

        /// <summary>функция проверки логина и пароля. Возвращает true - если логин и пароль введены правильно.
        /// В этой же функции инициализируется переменная Session["authperson"], в которой содержится список объект класса структуры учётной записи(AdmPersonStruct)</summary>
        /// <param name="login">принимает значения логина</param>
        /// <param name="pass">принимает значение пароля</param>
        /// <returns></returns>
        public bool CheckAuth(string login, string pass)
        {
            bool result = false;

            #region ОСНОВНОЙ КОД

            string filePath = _pag.MapPath("~") + @"files\adm\lp";
            if (!File.Exists(filePath))     //если файла авторизации не существует, то создаём его со значениями по умолчание (учётка admin(пароль admin), учётка tech)   
            {
                //создадим новый файл для структур учётных записей c добавлением учётки админа - admin с паролем admin
                var tempList = new List<string>();
                tempList.Add("begin|");
                tempList.Add("l|497100109105110");
                tempList.Add("p|797100109105110");
                tempList.Add("w|224228236232237232241242240224242238240");
                tempList.Add("a|224228236232237232241242240224242238240732241224233242224");
                tempList.Add("end|");

                try { File.WriteAllLines(filePath, tempList, Encoding.Default); }
                catch { }
            }

            string[] str, strSplit;
            bool switcherMain = false;
            bool switcherL = false;
            bool switcherP = false;
            var encdec = new EncDecClass();
            var personStruct = new AdmPersonStruct();

            str = File.ReadAllLines(filePath, Encoding.Default);
            foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
            {
                strSplit = line.Split(new[] { '|' });
                if (strSplit.Length > 1)                    //проверка-подстраховка того, что строка содержит полезную инфу
                {
                    if (strSplit[0] == "begin")
                    {
                        switcherMain = true;
                        switcherL = false; switcherP = false;
                        personStruct = new AdmPersonStruct();
                    }
                    else if (strSplit[0] == "end")
                    {
                        switcherMain = false;
                        if (switcherL && switcherP)     //если логин и пароль правильные, то..
                        {
                            result = true;
                            _pag.Session["authperson"] = personStruct;
                            break;
                        }
                    }

                    if (switcherMain)
                    {
                        switch (strSplit[0])
                        {
                            case "l":
                                if (encdec.Dectext(strSplit[1]) == login) switcherL = true;
                                personStruct.Name = encdec.Dectext(strSplit[1]);
                                break;

                            case "p":
                                if (encdec.Dectext(strSplit[1]) == pass) switcherP = true;
                                personStruct.Pass = encdec.Dectext(strSplit[1]);
                                break;

                            case "w":
                                personStruct.Writes = encdec.Dectext(strSplit[1]);
                                break;

                            case "a":
                                personStruct.About = encdec.Dectext(strSplit[1]);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>метод записи входа(или попытки входа в консоль управления) в файл журнала (\files\adm\jrl\jrl_*)
        /// Структура строки файла журнала:
        /// дата| время| IP-адрес(с котором производится вход)| DNS-имя(NONAME - если хост не определяется)| имя админа| разрешён или запрещён вход (PERMIT или PREVENT)| информация об агенте, с помощью которого производится вход)</summary>
        /// <param name="login">передаётся логин</param>
        /// <param name="permOrPrev">передаётся true или false (разрешён был доступ или запрещён)</param>
        public void VizitAccounting(string login, bool permOrPrev)
        {
            if (login == "tech") return;

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
            string permitOrPrevent = "";
            if (permOrPrev) permitOrPrevent = "PERMIT"; else permitOrPrevent = "PREVENT";

            string vizitor = DateTime.Now.ToShortDateString() + "|" + DateTime.Now.ToLongTimeString() + "|" + HttpContext.Current.Request.UserHostAddress + "|" + hostname + "|" + login + "|" + permitOrPrevent + "|" + useragent;

            #endregion

            #region КОД ОПРЕДЕЛЕНИЯ ПОЛНОГО ПУТИ К ФАЙЛУ ЖУРНАЛА, В КОТОРЫЙ НУЖНО ДОБАВИТЬ СОБЫТИЕ, с проверкой на размер и наличие

            string pathToNeedFile = ""; string[] strSplit, strSplit1;
            string[] pathToDBfiles = Directory.GetFiles(_pag.MapPath("~") + @"files\adm\jrl", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathToDBfiles.Length == 0)      //если файла журнала ещё не существует, то записываем в pathToNeedFile полный путь к новому(первому) файлу журнала
            { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrl\jrl_1"; }
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
                var fi = new FileInfo(_pag.MapPath("~") + @"files\adm\jrl\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName);
                if (fi.Length > 500000)    //если размер последнего файла БД больше максимально допустимого, то задаём путь к новой файлу БД со следующим порядковым номером
                { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrl\" + fNameLastDB.LeftPartOfFileName + "_" + (fNameLastDB.RightPartOfFileName + 1); }
                else { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrl\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName; }
            }

            #endregion

            #region КОД ЗАПИСИ ИНФОРМАЦИИ О ПЕСЕЩЕНИИ в файл журнала учёта посещений консоли управления

            try     //пробуем записать информацию о посетителе сайта в файл журнала учёта посетителей
            {
                var sw = new StreamWriter(pathToNeedFile, true, Encoding.Default);
                sw.WriteLine(vizitor);      //записываем в конец файла строку
                sw.Close();
                sw.Dispose();
            }
            catch { }

            #endregion
        }

        /// <summary>метод записи входа на сайт(посещения) в файл журнала (\files\adm\jrl\jrl_*)
        /// Структура строки файла журнала:
        /// дата| время| IP-адрес(с котором производится вход)| DNS-имя(NONAME - если хост не определяется)| информация об агенте, с помощью которого производится вход)</summary>
        public void VizitSiteAccounting()
        {
            if (HttpContext.Current.Session["eventLog"] != null) { return; }

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

            string vizitor = DateTime.Now.ToShortDateString() + "|" + DateTime.Now.ToLongTimeString() + "|" + HttpContext.Current.Request.UserHostAddress + "|" + hostname + "|" + useragent;

            #endregion

            #region КОД ОПРЕДЕЛЕНИЯ ПОЛНОГО ПУТИ К ФАЙЛУ ЖУРНАЛА, В КОТОРЫЙ НУЖНО ДОБАВИТЬ СОБЫТИЕ, с проверкой на размер и наличие

            string pathToNeedFile = ""; string[] strSplit, strSplit1;
            string[] pathToDBfiles = Directory.GetFiles(_pag.MapPath("~") + @"files\adm\jrlsite", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathToDBfiles.Length == 0)      //если файла журнала ещё не существует, то записываем в pathToNeedFile полный путь к новому(первому) файлу журнала
            { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrlsite\jrl_1"; }
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
                var fi = new FileInfo(_pag.MapPath("~") + @"files\adm\jrlsite\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName);
                if (fi.Length > 500000)    //если размер последнего файла БД больше максимально допустимого, то задаём путь к новой файлу БД со следующим порядковым номером
                { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrlsite\" + fNameLastDB.LeftPartOfFileName + "_" + (fNameLastDB.RightPartOfFileName + 1); }
                else { pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrlsite\" + fNameLastDB.LeftPartOfFileName + "_" + fNameLastDB.RightPartOfFileName; }
            }

            #endregion

            #region КОД ЗАПИСИ ИНФОРМАЦИИ О ПЕСЕЩЕНИИ в файл журнала учёта посещений консоли управления

            try     //пробуем записать информацию о посетителе сайта в файл журнала учёта посетителей
            {
                var sw = new StreamWriter(pathToNeedFile, true, Encoding.Default);
                sw.WriteLine(vizitor);      //записываем в конец файла строку
                sw.Close();
                sw.Dispose();
            }
            catch { }

            #endregion

            HttpContext.Current.Session["eventLog"] = "ok";
        }

        /// <summary>функция возвращает заполненный объекты структуры данных по имени(логину) пользователя консоли администрирования</summary>
        /// <param name="nick">имя пользователя</param>
        /// <returns></returns>
        public AdmPersonStruct GetAdmPersonStruct(string nick)
        {
            var admpers = new AdmPersonStruct();

            #region ОСНОВНОЙ КОД

            string filePath = _pag.MapPath("~") + @"files\adm\lp";

            string[] str, strSplit;
            bool switcherMain = false;
            bool switcherFind = false;
            var encdec = new EncDecClass();

            str = File.ReadAllLines(filePath, Encoding.Default);
            foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
            {
                strSplit = line.Split(new Char[] { '|' });
                if (strSplit.Length > 1)                    //проверка-подстраховка того, что строка содержит полезную инфу
                {
                    if (strSplit[0] == "begin")
                    {
                        switcherMain = true;
                        admpers = new AdmPersonStruct();
                    }
                    else if (strSplit[0] == "end")
                    {
                        switcherMain = false;
                        if (switcherFind)     //если найдена учётка нужного нам пользователя, то выходим
                        {
                            break;
                        }
                    }

                    if (switcherMain)
                    {
                        switch (strSplit[0])
                        {
                            case "l":
                                if (encdec.Dectext(strSplit[1]) == nick) switcherFind = true;
                                admpers.Name = encdec.Dectext(strSplit[1]);
                                break;

                            case "p":
                                admpers.Pass = encdec.Dectext(strSplit[1]);
                                break;

                            case "w":
                                admpers.Writes = encdec.Dectext(strSplit[1]);
                                break;

                            case "a":
                                admpers.About = encdec.Dectext(strSplit[1]);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            #endregion

            return admpers;
        }

        /// <summary>функция возвращает список структур всех учётных записей, содержащихся в файле /files/adm/lp, кроме структуры одной учётной записи, переданной в качестве аргумента</summary>
        /// <param name="structForExclude">структура учётной записи, которую нужно исключить из возвращаемого списка</param>
        /// <returns></returns>
        public List<AdmPersonStruct> GetAllAdmStructs(AdmPersonStruct structForExclude = null)
        {
            var listResult = new List<AdmPersonStruct>();

            #region КОД

            string excludeLogin = "";
            if (structForExclude != null) { excludeLogin = structForExclude.Name; }
            string pathtofile = _pag.Server.MapPath("~") + @"files\adm\lp";
            string[] str, strSplit;
            bool startStruct = false;
            bool checkExclude = false;
            var oneStruct = new AdmPersonStruct();
            var encdec = new EncDecClass();
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                int counter = 0;
                foreach (string oneline in str)
                {
                    strSplit = oneline.Split(new[] { '|' });
                    if (strSplit[0] == "begin")
                    {
                        oneStruct = new AdmPersonStruct();
                        startStruct = true;
                        checkExclude = false;
                    }
                    else if (strSplit[0] == "end")
                    {
                        if (!checkExclude) { listResult.Add(oneStruct); }
                        startStruct = false;
                        counter++;
                    }
                    else
                    {
                        if (startStruct)
                        {
                            if (strSplit[0] == "l")
                            {
                                oneStruct.Name = encdec.Dectext(strSplit[1]);
                                if (oneStruct.Name == excludeLogin) { startStruct = false; checkExclude = true; _listIdToInsert = counter; }
                            }
                            else if (strSplit[0] == "p")
                            {
                                oneStruct.Pass = encdec.Dectext(strSplit[1]);
                            }
                            else if (strSplit[0] == "w")
                            {
                                oneStruct.Writes = encdec.Dectext(strSplit[1]);
                            }
                            else if (strSplit[0] == "a")
                            {
                                oneStruct.About = encdec.Dectext(strSplit[1]);
                            }
                        }
                    }
                }
            }
            else
            {
                //создадим новый файл для структур учётных записей c добавлением сразу двух учёток - технической учётки tech и учётки admin с паролем admin
                var tempList = new List<string>();
                tempList.Add("l|116101399104");
                tempList.Add("p|465110100114101119450755549850755756");
                tempList.Add("w|224228236232237232241242240224242238240");
                tempList.Add("a|");
                tempList.Add("l|497100109105110");
                tempList.Add("p|797100109105110");
                tempList.Add("w|224228236232237232241242240224242238240");
                tempList.Add("a|224228236232237232241242240224242238240732241224233242224");

                try { File.WriteAllLines(pathtofile, tempList, Encoding.Default); }
                catch { }

                oneStruct = new AdmPersonStruct();
                oneStruct.Name = encdec.Dectext(tempList[0].Replace("l|", ""));
                oneStruct.Pass = encdec.Dectext(tempList[1].Replace("p|", ""));
                oneStruct.Writes = encdec.Dectext(tempList[2].Replace("w|", ""));
                oneStruct.About = encdec.Dectext(tempList[3].Replace("a|", ""));
                listResult.Add(oneStruct);
                oneStruct = new AdmPersonStruct();
                oneStruct.Name = encdec.Dectext(tempList[4].Replace("l|", ""));
                oneStruct.Pass = encdec.Dectext(tempList[5].Replace("p|", ""));
                oneStruct.Writes = encdec.Dectext(tempList[6].Replace("w|", ""));
                oneStruct.About = encdec.Dectext(tempList[7].Replace("a|", ""));
                listResult.Add(oneStruct);

                return listResult;
            }

            #endregion

            return listResult;
        }

        /// <summary>функция возвращает структуру одной учётной записи по имени(логину)</summary>
        /// <param name="login">имя(логин) учётной записи, структуру которой нужно вернуть</param>
        /// <returns></returns>
        public AdmPersonStruct GetOneAdmStructsForName(string login)
        {
            var structResult = new AdmPersonStruct();

            #region КОД

            string pathtofile = _pag.Server.MapPath("~") + @"files\adm\lp";
            string[] str, strSplit;
            bool startStruct = false;
            bool checkExit = false;
            var encdec = new EncDecClass();
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string oneline in str)
                {
                    strSplit = oneline.Split(new[] { '|' });
                    if (strSplit[0] == "begin")
                    {
                        structResult = new AdmPersonStruct();
                        startStruct = true;
                        checkExit = false;
                    }
                    else if (strSplit[0] == "end")
                    {
                        if (checkExit) { break; }
                        startStruct = false;
                    }
                    else
                    {
                        if (startStruct)
                        {
                            if (strSplit[0] == "l")
                            {
                                structResult.Name = encdec.Dectext(strSplit[1]);
                                if (structResult.Name == login) { checkExit = true; } else { startStruct = false; }
                            }
                            else if (strSplit[0] == "p")
                            {
                                structResult.Pass = encdec.Dectext(strSplit[1]);
                            }
                            else if (strSplit[0] == "w")
                            {
                                structResult.Writes = encdec.Dectext(strSplit[1]);
                            }
                            else if (strSplit[0] == "a")
                            {
                                structResult.About = encdec.Dectext(strSplit[1]);
                            }
                        }
                    }
                }
            }

            #endregion

            return structResult;
        }

        /// <summary>функция заменяет(учётка помещается на то же место в списке учёток) или удаляет данные по одной учётке в файле /files/adm/lp. Возвращает true в случае успеха и false в случае ошибки во время операций</summary>
        /// <param name="Struct">структура данных учётной записи, которую нужно заменить или удалить в файле</param>
        /// <param name="delete">true - удалить переданную структуру из файла, false - заменить</param>
        /// <returns></returns>
        public bool ReplaceOrDeleteAccount(AdmPersonStruct Struct, bool delete = false)
        {
            string pathtofile = _pag.Server.MapPath("~") + @"files\adm\lp";
            string pathtotemp = _pag.Server.MapPath("~") + @"files\temp\lp";

            #region КОД ЗАМЕНЫ И УДАЛЕНИЯ ДАННЫХ ПО ОДНОЙ УЧЁТНОЙ ЗАПИСИ

            var listOfStructs = GetAllAdmStructs(Struct);     //получим список структур всех учётных записей, за исключением той учётной записи, данные по которой нужно изменить или удалить
            if (!delete)                    //если нужно перезаписать данные учётки, то добавим в список новую переданную в функцию структуру учётки на тоже место 
            {
                //вставляем структуру на то место в списке, из которого она была исключена в фукнции GetAllAdmStructs
                if (_listIdToInsert == listOfStructs.Count) { listOfStructs.Add(Struct); }
                else
                {
                    //если добавляем новую учётную запись(_listIdToInsert == -1), то добавляем структуру в конец списка
                    if (_listIdToInsert == -1) { listOfStructs.Add(Struct); } else { listOfStructs.Insert(_listIdToInsert, Struct); }
                }
            }

            //получим строковый список listForDBFile, который пригоден для записи в файл
            var listForDbFile = new List<string>();
            foreach (var onestruct in listOfStructs) { listForDbFile.AddRange(onestruct.GetListFromStruct()); }
            listOfStructs.Clear();

            //перезапишем файл /files/adm/lp

            //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла
            FileStream fs;
            try { fs = new FileStream(pathtofile, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return false; }

            var rn = new Random();
            string tempFileName = "_" + rn.Next(1, 666);

            try { File.WriteAllLines(pathtotemp + tempFileName, listForDbFile, Encoding.Default); }
            catch { return false; }
            try
            {
                try { fs.Close(); fs.Dispose(); }
                catch { }
                File.Copy(pathtotemp + tempFileName, pathtofile, true);
            }
            catch { return false; }
            try { File.Delete(pathtotemp + tempFileName); }
            catch { }

            #endregion

            return true;
        }

        /// <summary>проверка логина на уникальность. Функция возвращает true - если учётка с таким логином уже существует и false - если учётка с таким логином не существует.</summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public bool IsLoginExists(string login)
        {
            bool result = false;

            var encdec = new EncDecClass();
            string pathtofile = _pag.Server.MapPath("~") + @"files\adm\lp";
            string[] str, strSplit;
            if (File.Exists(pathtofile))
            {
                str = File.ReadAllLines(pathtofile, Encoding.Default);
                foreach (string oneline in str)
                {
                    strSplit = oneline.Split(new[] { '|' });
                    if (strSplit[0] == "l")
                    {
                        if (encdec.Dectext(strSplit[1]) == login) { result = true; break; }
                    }
                }
            }

            return result;
        }

        /// <summary>функция возвращает список структур событий посещения консоли, отфильтрованных по поисковому значению. Список начинается с последнего события.</summary>
        /// <param name="filterString">строка со строковым значением для фильтрации</param>
        /// <param name="inverseSrch">true - инверсный поиск(кроме значения, указанного в поисковой строке), false - обычный поиск по совпадению</param>
        /// <returns></returns>
        public List<EventStruct> GetAllEventStructs(string filterString, bool inverseSrch)
        {
            var eventStructList = new List<EventStruct>();

            var oneEvent = new EventStruct();
            bool checkFind = false;
            string[] str, strSplit;
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfilesArray = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrl", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathsToDBfilesArray.Length == 0) { return eventStructList; }    //если файлов журнала ещё нет, то возвращаем пустой список

            if (filterString == "")         //если фильтровать список НЕ нужно, то..
            {
                foreach (string filePath in pathsToDBfilesArray)    //перебираем каждый файл БД товаров по очереди
                {
                    str = File.ReadAllLines(filePath, Encoding.Default);
                    foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
                    {
                        if (line.Contains("|"))
                        {
                            oneEvent = new EventStruct();
                            strSplit = line.Split(new[] { '|' });
                            oneEvent.Date = strSplit[0];
                            oneEvent.Time = strSplit[1];
                            oneEvent.HostIp = strSplit[2];
                            oneEvent.HostName = strSplit[3];
                            oneEvent.Login = strSplit[4];
                            oneEvent.Access = strSplit[5];
                            oneEvent.ClientInfo = strSplit[6];
                            eventStructList.Add(oneEvent);
                        }
                    }
                }
            }
            else                            //если фильтровать список нужно, то..
            {
                foreach (string filePath in pathsToDBfilesArray)    //перебираем каждый файл БД товаров по очереди
                {
                    str = File.ReadAllLines(filePath, Encoding.Default);
                    foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
                    {
                        if (line.Contains("|"))
                        {
                            checkFind = false;
                            oneEvent = new EventStruct();
                            strSplit = line.Split(new[] { '|' });
                            //фильтрация
                            foreach (string onePart in strSplit)
                            {
                                //если хоть одно поле события содержит значение фильтрации, то отмечаем это..
                                if (onePart.ToLower().Contains(filterString.ToLower())) { checkFind = true; }
                            }

                            if (inverseSrch && !checkFind) { checkFind = true; }
                            else if (inverseSrch && checkFind) { checkFind = false; }

                            if (checkFind)
                            {
                                oneEvent.Date = strSplit[0];
                                oneEvent.Time = strSplit[1];
                                oneEvent.HostIp = strSplit[2];
                                oneEvent.HostName = strSplit[3];
                                oneEvent.Login = strSplit[4];
                                oneEvent.Access = strSplit[5];
                                oneEvent.ClientInfo = strSplit[6];
                                eventStructList.Add(oneEvent);
                            }
                        }
                    }
                }
            }

            if (eventStructList.Count > 0) { eventStructList.Reverse(); }

            return eventStructList;
        }

        /// <summary>функция возвращает список структур событий посещения консоли, отфильтрованных по поисковому значению. Список начинается с последнего события.</summary>
        /// <param name="filterString">строка со строковым значением для фильтрации</param>
        /// <param name="inverseSrch">true - инверсный поиск(кроме значения, указанного в поисковой строке), false - обычный поиск по совпадению</param>
        /// <returns></returns>
        public List<EventStruct> GetAllSiteEventStructs(string filterString, bool inverseSrch)
        {
            var eventStructList = new List<EventStruct>();

            var oneEvent = new EventStruct();
            bool checkFind = false;
            string[] str, strSplit;
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfilesArray = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrlsite", "jrl_*", SearchOption.TopDirectoryOnly);
            if (pathsToDBfilesArray.Length == 0) { return eventStructList; }    //если файлов журнала ещё нет, то возвращаем пустой список

            if (filterString == "")         //если фильтровать список НЕ нужно, то..
            {
                foreach (string filePath in pathsToDBfilesArray)    //перебираем каждый файл БД товаров по очереди
                {
                    str = File.ReadAllLines(filePath, Encoding.Default);
                    foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
                    {
                        if (line.Contains("|"))
                        {
                            oneEvent = new EventStruct();
                            strSplit = line.Split(new[] { '|' });
                            oneEvent.Date = strSplit[0];
                            oneEvent.Time = strSplit[1];
                            oneEvent.HostIp = strSplit[2];
                            oneEvent.HostName = strSplit[3];
                            //oneEvent.Login = strSplit[4];
                            //oneEvent.Access = strSplit[5];
                            oneEvent.ClientInfo = strSplit[4];
                            eventStructList.Add(oneEvent);
                        }
                    }
                }
            }
            else                            //если фильтровать список нужно, то..
            {
                foreach (string filePath in pathsToDBfilesArray)    //перебираем каждый файл БД товаров по очереди
                {
                    str = File.ReadAllLines(filePath, Encoding.Default);
                    foreach (string line in str)                    //перебираем содержимое файла, выгруженное в массив str
                    {
                        if (line.Contains("|"))
                        {
                            checkFind = false;
                            oneEvent = new EventStruct();
                            strSplit = line.Split(new[] { '|' });
                            //фильтрация
                            foreach (string onePart in strSplit)
                            {
                                //если хоть одно поле события содержит значение фильтрации, то отмечаем это..
                                if (onePart.ToLower().Contains(filterString.ToLower())) { checkFind = true; }
                            }

                            if (inverseSrch && !checkFind) { checkFind = true; }
                            else if (inverseSrch && checkFind) { checkFind = false; }

                            if (checkFind)
                            {
                                oneEvent.Date = strSplit[0];
                                oneEvent.Time = strSplit[1];
                                oneEvent.HostIp = strSplit[2];
                                oneEvent.HostName = strSplit[3];
                                //oneEvent.Login = strSplit[4];
                                //oneEvent.Access = strSplit[5];
                                oneEvent.ClientInfo = strSplit[4];
                                eventStructList.Add(oneEvent);
                            }
                        }
                    }
                }
            }

            if (eventStructList.Count > 0) { eventStructList.Reverse(); }

            return eventStructList;
        }

        /// <summary>функция очистки журнала событий посещения консоли администрирования, удаляется файл с самыми ранними событиями (allclear = false) или все файлы (allclear = true)</summary>
        /// <returns></returns>
        public bool CleanAllEvents(bool allclear)
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrl", "jrl_*", SearchOption.TopDirectoryOnly);
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
                pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrl\" + fNameFirstDB.LeftPartOfFileName + "_" + fNameFirstDB.RightPartOfFileName;


                //удалим файл с самыми ранними событиями
                try { File.Delete(pathToNeedFile); }
                catch { return false; }
            }

            return true;
        }

        /// <summary>функция очистки журнала событий посещения сайта, удаляется файл с самыми ранними событиями (allclear = false) или все файлы (allclear = true)</summary>
        /// <returns></returns>
        public bool CleanAllSiteEvents(bool allclear)
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrlsite", "jrl_*", SearchOption.TopDirectoryOnly);
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
                pathToNeedFile = _pag.MapPath("~") + @"files\adm\jrlsite\" + fNameFirstDB.LeftPartOfFileName + "_" + fNameFirstDB.RightPartOfFileName;


                //удалим файл с самыми ранними событиями
                try { File.Delete(pathToNeedFile); }
                catch { return false; }
            }

            return true;
        }

        /// <summary>функция возвращает общий размер (в килобайтах) всех файлов журнала событий посещения консоли</summary>
        /// <returns></returns>
        public string GetSizeOfAllEventFiles()
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrl", "jrl_*", SearchOption.TopDirectoryOnly);

            FileInfo fi;
            long fullLength = 0;
            foreach (string onePath in pathsToDBfiles)
            {
                fi = new FileInfo(onePath);
                fullLength += fi.Length;
            }

            return (fullLength / 1024).ToString();
        }

        /// <summary>функция возвращает общий размер (в килобайтах) всех файлов журнала событий посещения сайта</summary>
        /// <returns></returns>
        public string GetSizeOfAllSiteEventFiles()
        {
            //получаем массив полных путей к файлам журнала
            string[] pathsToDBfiles = Directory.GetFiles(_pag.Server.MapPath("~") + @"files\adm\jrlsite", "jrl_*", SearchOption.TopDirectoryOnly);

            FileInfo fi;
            long fullLength = 0;
            foreach (string onePath in pathsToDBfiles)
            {
                fi = new FileInfo(onePath);
                fullLength += fi.Length;
            }

            return (fullLength / 1024).ToString();
        }
    }

    #endregion

    #region Код с описанием структур данных (объектов)     --------------------------------------------

    /// <summary>класс, который описывает структу данных учётной записи для консоли управления</summary>
    [Serializable]
    public class AdmPersonStruct
    {
        public string Name { get; set; }            //свойство содержит имя учётной записи в раскодированном виде
        public string Pass { get; set; }            //свойство содержит пароль учётной записи в раскодированном виде
        public string Writes { get; set; }          //свойство содержит права учётной записи в раскодированном виде (администратор, редактор новостей, редактор страниц, редактор фотоконкурса, редактор литконкурса, редактор театрконкурса)
        public string About { get; set; }           //свойство содержит описание учётной записи

        public List<string> GetListFromStruct()
        {
            var resultList = new List<string>();
            var encdec = new EncDecClass();

            resultList.Add("begin|");
            resultList.Add("l|" + encdec.Enctext(Name));
            resultList.Add("p|" + encdec.Enctext(Pass));
            resultList.Add("w|" + encdec.Enctext(Writes));
            resultList.Add("a|" + encdec.Enctext(About));
            resultList.Add("end|");

            return resultList;
        }
    }

    /// <summary>класс, который описывает структу данных одного события, записываемого в журнал посещения консоли администрирования (site\files\adm\jrl\jrl_*)</summary>
    public class EventStruct
    {
        public string Date { get; set; }                //свойство содержит дату события (например, 09.11.2014)
        public string Time { get; set; }                //свойство содержит время события (например, 14:39:40)
        public string HostIp { get; set; }              //свойство содержит ip-адрес узла, с которого было обращение к консоли (например, 192.168.0.2)
        public string HostName { get; set; }            //свойство содержит имя узла, с которого было обращение к консоли (например, testcomp.test.ru)
        public string Login { get; set; }               //свойство содержит имя(логин) административной учётной записи, под которой было обращение к консоли (например, admin)
        public string Access { get; set; }              //свойство содержит значения, которые показывают, был ли разрешён доступ к консоли или запрещён. Разрешён - PERMIT, запрещён - PREVENT.
        public string ClientInfo { get; set; }          //свойство содержит информацию о клиенте, с которого было обращение к консоли (например, Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko)
    }

    /// <summary>вспомогательный класс, нужен только для удобной сортировки имён файлов БД в функции saveNewOrderToDB(OrderStruct ord) и других функциях в других классах(LoginWorkClass.cs)</summary>
    public class FileNameForSort
    {
        public string LeftPartOfFileName { get; set; }      //свойство хранит левую часть имени файла БД. Например, если всё имя файла 'dbordnew_1', то это св-во хранит только 'dbordnew'
        public int RightPartOfFileName { get; set; }      //свойство хранит правую часть имени файла БД. Например, если всё имя файла 'dbordnew_1', то это св-во хранит только '1'

        public FileNameForSort() { }
    }

    #endregion

}