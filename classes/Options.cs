using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using site.classesHelp;
using Image = System.Web.UI.WebControls.Image;

/* файл с классами для работы с дополнительными опциями сайта (файлы в папке files\options\) */
namespace site.classes
{
    #region Код формирования HTML-кода

    /// <summary>класс предназначен для отображения различных редакторов и просмотрщиков в разделе </summary>
    public class OptionsForm
    {
        private Page pag;

        public OptionsForm(Page pagenew) { pag = pagenew; }

        #region МЕТОДЫ, КАСАЮЩИЕСЯ ОТОБРАЖЕНИЯ РЕДАКТОРА ОСНОВНЫХ ПАРАМЕТРОВ САЙТА

        /// <summary>функция возвращает таблицу с возможностью включения или выключения сайта</summary>
        /// <returns></returns>
        public Panel GetSiteEnablePanel()
        {
            var panelWrapper = new Panel();

            ConfigFile cfg = new ConfigFile();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР ПАРАМЕТРОВ САЙТА"; panelWrapper.Controls.Add(lbl);

            //Кнопка вкл/выкл сайта
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Нажмите на кнопку, если нужно: "; panelLine.Controls.Add(lbl);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            string optionData = cfg.GetParam("siteenable");
            if (optionData == "-1")
            {
                cfg.AddParam("siteenable", "true");
                optionData = "true";
            }
            if (optionData == "true") { lBtn.Text = " выключить сайт "; }
            else if (optionData == "false") { lBtn.Text = " включить сайт "; }
            lBtn.ToolTip = "кнопка включения или выключения сайта";
            lBtn.CommandArgument = optionData; lBtn.Command += (lBtnSiteEnable_Command);
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetSiteEnablePanel()

        /// <summary>событие нажатия на кнопку включения или выключения сайта</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSiteEnable_Command(object sender, CommandEventArgs e)
        {
            ConfigFile cfg = new ConfigFile();

            if (e.CommandArgument.ToString() == "true")             //если сайт включён, то выключаем его
            {
                cfg.AddParam("siteenable", "false");
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Выключение сайта. Выключил - " + ((AdmPersonStruct)pag.Session["authperson"]).Name);
            }
            else if (e.CommandArgument.ToString() == "false")       //если сайт выключен, то включаем его
            {
                cfg.AddParam("siteenable", "true");
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Включение сайта. Включил - " + ((AdmPersonStruct)pag.Session["authperson"]).Name);
            }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        /// <summary>функция возвращает панель с возможностью добавления файла картинки для наложения на изображения.
        /// Картинка добавляется в папку /files/pages/images/ имя файла - overlayimg.png</summary>
        /// <returns></returns>
        public Panel GetOverlayImageAddPanel()
        {
            var panelWrapper = new Panel();

            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            var lbl = new Label(); lbl.CssClass = "lblFormPre"; lbl.Text = "Добавить картинку, используемую для наложения на изображения: "; panelLine.Controls.Add(lbl);
            var fUpload = new FileUpload(); fUpload.CssClass = "txtBoxUniverse_adm"; fUpload.Width = 200; fUpload.ID = "OverlayImgUpload"; panelLine.Controls.Add(fUpload);
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " добавить ";
            lBtn.ToolTip = "кнопка добавления изображения, используемого для наложения на изображения (защита от копирования). Только PNG-формат.";
            lBtn.Command += (lBtnOverlayImageAdd_Command);
            panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //фотография и кнопка удалить
            var img = new Image();
            string pathToOverlayImg = HttpContext.Current.Server.MapPath("~") + @"files\pages\overlayimg.png";
            if (File.Exists(pathToOverlayImg))
            {
                panelLine = new Panel(); panelLine.CssClass = "panelLine";
                //фото
                img = new Image(); img.Height = 150; img.ImageUrl = "../files/pages/overlayimg.png"; panelLine.Controls.Add(img);
                //кнопка УДАЛИТЬ
                lBtn = new LinkButton(); lBtn.Command += new CommandEventHandler(lBtnDelOverlayImg_Command);
                lBtn.ToolTip = "удалить картинку для наложения";
                img = new Image(); img.CssClass = "buttonsHover lBtnsUniverse moveUp"; img.ImageUrl = "~/images/krestik.png";
                lBtn.Controls.Add(img); panelLine.Controls.Add(lBtn);
                panelWrapper.Controls.Add(panelLine);
            }

            return panelWrapper;
        }

        #region События для функции GetOverlayImageAddPanel()

        /// <summary>событие нажатия на кнопку ДОБАВИТЬ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnOverlayImageAdd_Command(object sender, CommandEventArgs e)
        {
            var fUpload = (FileUpload)pag.FindControl("ctl00$ContentPlaceHolder1$OverlayImgUpload");
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            if (fUpload.HasFile)
            {
                warning.HideWarning(pag.Master);

                string fileName = pag.Server.HtmlEncode(fUpload.FileName);
                string extension = Path.GetExtension(fileName);

                if (extension != null && extension.ToLower() == ".png")     //проверка на допустимые расширения закачиваемого файла
                {
                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 100000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        string savePath = HttpContext.Current.Server.MapPath("~") + @"files\pages\overlayimg.png";
                        try
                        {
                            fUpload.SaveAs(savePath);
                            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
                            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
                        }
                        catch { warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе сохранения файла на сервер. Попробуйте повторить.", pag.Master); }

                    }
                    else warning.ShowWarning("ВНИМАНИЕ. Доступно добавление файлов размером не более 100 килобайт.", pag.Master);
                }
                else
                {
                    warning.ShowWarning("ВНИМАНИЕ. Доступно добавление только файлов формата PNG.", pag.Master);
                }
            }
            else warning.ShowWarning("ВНИМАНИЕ. Загрузите для начала файл через кнопку ОБЗОР.", pag.Master);
        }

        /// <summary>нажатие на кнопку УДАЛИТЬ КАРТИНКУ ДЛЯ НАЛОЖЕНИЯ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtnDelOverlayImg_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            string pathToOverlayImg = HttpContext.Current.Server.MapPath("~") + @"files\pages\overlayimg.png";
            try
            {
                File.Delete(pathToOverlayImg);
            }
            catch
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка в процессе удаления файла картинки. Попробуйте повторить.", pag.Master);
                return;
            }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        #endregion

        #region МЕТОДЫ, КАСАЮЩИЕСЯ ОТОБРАЖЕНИЯ РЕДАКТОРА ПОЧТОВОГО ЯЩИКА ДЛЯ РАССЫЛОК И АДМИНИСТРАТИВНОГО ПОЧТОВОГО ЯЩИКА

        #region Метод GetMailBoxEditPanel()

        /// <summary>функция возвращает таблицу с редактором параметров почтового ящика для рассылок</summary>
        /// <returns></returns>
        public Panel GetMailBoxEditPanel()
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panWrap";

            ConfigFile config = new ConfigFile();
            MailBoxStruct mailBoxStruct = new MailBoxStruct();
            string tempStr = config.GetParam("from", true);
            mailBoxStruct.From = tempStr == "-1" ? "" : tempStr;
            tempStr = config.GetParam("dnssmtp", true);
            mailBoxStruct.SmtpServer = tempStr == "-1" ? "" : tempStr;
            tempStr = config.GetParam("smtpport", true);
            mailBoxStruct.ServerPort = tempStr == "-1" ? "" : tempStr;
            tempStr = config.GetParam("ssl", true);
            mailBoxStruct.Ssl = tempStr == "-1" ? "" : tempStr;
            tempStr = config.GetParam("smtpl", true);
            mailBoxStruct.Login = tempStr == "-1" ? "" : tempStr;
            tempStr = config.GetParam("smtpp", true);
            mailBoxStruct.Password = tempStr == "-1" ? "" : tempStr;

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "РЕДАКТОР НАСТРОЕК ПОЧТОВОГО ЯЩИКА ДЛЯ РАССЫЛОК"; panelWrapper.Controls.Add(lbl);

            //от кого
            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "От кого "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxFrom"; txtBox.Width = 400; txtBox.Text = mailBoxStruct.From; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //DNS-имя SMTP-сервера
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "DNS-имя SMTP-сервера "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxSmtpServer"; txtBox.Width = 400; txtBox.Text = mailBoxStruct.SmtpServer; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //порт SMTP-сервера
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Порт SMTP-сервера "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxServerPort"; txtBox.Width = 50; txtBox.Text = mailBoxStruct.ServerPort; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //SSL
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "SSL "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = ""; panelLine.Controls.Add(lbl);
            var chkBox = new CheckBox(); chkBox.CssClass = "PsBC_txtBox"; chkBox.ID = "chkBoxSsl";
            if (mailBoxStruct.Ssl == "0") { chkBox.Checked = false; } else if (mailBoxStruct.Ssl == "1") { chkBox.Checked = true; }
            panelLine.Controls.Add(chkBox); panelWrapper.Controls.Add(panelLine);

            //логин
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Логин "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxL"; txtBox.Width = 400; txtBox.Text = mailBoxStruct.Login; panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //пароль
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Пароль "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxP"; txtBox.Width = 400; txtBox.Text = ""; txtBox.ToolTip = "Оставьте это поле пустым, если не хотите менять пароль.";
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённых параметров(отсылается пробное письмо).";
            lBtn.CommandArgument = mailBoxStruct.Password; lBtn.Command += (lBtnTestMailBox_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения параметров почтового ящика для рассылок.";
            lBtn.CommandArgument = mailBoxStruct.Password; lBtn.Command += (lBtnSaveMailBox_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            return panelWrapper;
        }

        #region События для функции GetMailBoxEditPanel()

        /// <summary>событие нажатия на кнопку "ПРОВЕРИТЬ". Проверяются настройки почтового ящика отсылкой на него пробного письма</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит пароль к почтовому ящику</param>
        protected void lBtnTestMailBox_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            string txtBoxFrom = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxFrom")).Text;
            string txtBoxSmtpServer = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSmtpServer")).Text;
            string txtBoxServerPort = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxServerPort")).Text;
            string chkBoxSsl = "";
            if (((CheckBox)pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxSsl")).Checked) { chkBoxSsl = "1"; } else { chkBoxSsl = "0"; }
            string txtBoxL = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxL")).Text;
            string password = e.CommandArgument.ToString();

            int servPort = StringToNum.ParseInt(txtBoxServerPort);
            //проверочные условия
            if (txtBoxFrom.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'От кого' не должно быть пустым.", pag.Master); return; }
            if (txtBoxSmtpServer.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'DNS-имя SMTP-сервера' не должно быть пустым.", pag.Master); return; }
            if (servPort == -1) { warning.ShowWarning("ВНИМАНИЕ. В поле 'Порт SMTP-сервера' введено неправильное значение.", pag.Master); return; }
            if (txtBoxL.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'Логин' не должно быть пустым.", pag.Master); return; }

            //отправим тестовое письмо на почтовый ящик, который настраивается
            var sendMailClass = new SendMailClass();
            if (!sendMailClass.SendTestMail(txtBoxFrom, txtBoxFrom, txtBoxSmtpServer, servPort, chkBoxSsl, "проверка связи", "", txtBoxL, password))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибки при передаче проверочного письма на почтовый ящик. Возможно вы неправильно ввели параметры почтового ящика.", pag.Master);
            }
            /*else
            {
                warning.ShowWarning("УСПЕХ. Проверочное письмо успешно отправлено на почтовый ящик. Проверьте его. Затем нажмите СОХРАНИТЬ.", pag.Master);
            }*/
        }

        /// <summary>событие нажатия на кнопку "сохранить"</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит пароль к почтовому ящику</param>
        protected void lBtnSaveMailBox_Command(object sender, CommandEventArgs e)
        {
            var mailBoxStruct = new MailBoxStruct();

            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            mailBoxStruct.From = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxFrom")).Text;
            mailBoxStruct.SmtpServer = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSmtpServer")).Text;
            mailBoxStruct.ServerPort = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxServerPort")).Text;
            if (((CheckBox)pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxSsl")).Checked) { mailBoxStruct.Ssl = "1"; } else { mailBoxStruct.Ssl = "0"; }
            mailBoxStruct.Login = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxL")).Text;
            //если в поле ПАРОЛЬ ничего не введено, записываем в структуру почтового ящика для сохранения прежнее значение пароля
            if (((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxP")).Text == "") { mailBoxStruct.Password = e.CommandArgument.ToString(); }
            else { mailBoxStruct.Password = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxP")).Text; }

            int servPort = StringToNum.ParseInt(mailBoxStruct.ServerPort);
            //проверочные условия
            if (mailBoxStruct.From.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'От кого' не должно быть пустым.", pag.Master); return; }
            if (mailBoxStruct.SmtpServer.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'DNS-имя SMTP-сервера' не должно быть пустым.", pag.Master); return; }
            if (servPort == -1) { warning.ShowWarning("ВНИМАНИЕ. В поле 'Порт SMTP-сервера' введено неправильное значение.", pag.Master); return; }
            if (mailBoxStruct.Login.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'Логин' не должно быть пустым.", pag.Master); return; }

            //сохраним новую структуру в файл
            var config = new ConfigFile();
            if (!config.AddParam("from", mailBoxStruct.From, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }
            if (!config.AddParam("dnssmtp", mailBoxStruct.SmtpServer, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }
            if (!config.AddParam("smtpport", mailBoxStruct.ServerPort, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }
            if (!config.AddParam("ssl", mailBoxStruct.Ssl, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }
            if (!config.AddParam("smtpl", mailBoxStruct.Login, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }
            if (!config.AddParam("smtpp", mailBoxStruct.Password, true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения новых параметров почтового ящика. Попробуйте повторить.", pag.Master); }

            DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменение данных почтового ящика для рассылок. Удалил - " + ((AdmPersonStruct)pag.Session["authperson"]).Name);
        }

        #endregion

        #endregion
        #region Метод GetAdminMailBoxEditPanel()

        /// <summary>функция возвращает таблицу с редактором параметров почтового ящика для административных уведомлений</summary>
        /// <returns></returns>
        public Panel GetAdminMailBoxEditPanel()
        {
            var panelWrapper = new Panel(); panelWrapper.CssClass = "panWrap";

            ConfigFile config = new ConfigFile();

            #region Админский почтовый ящик

            var panelLine = new Panel(); panelLine.CssClass = "panelLine";
            var lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "АДМИНИСТРАТИВНЫЙ "; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBox"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о публикации новости или объявления..";
            txtBox.Text = config.GetParam("admmail", true) == "-1" ? "" : config.GetParam("admmail", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmail";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmail";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Почтовый ящик для рассылок (Номинация Фотография)

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Крымский Вернисаж - Фотография"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxFoto"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailfoto", true) == "-1" ? "" : config.GetParam("admmailfoto", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailfoto";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailfoto";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Почтовый ящик для рассылок (Номинация ИЗО)

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Крымский Вернисаж - ИЗО)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxIso"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailiso", true) == "-1" ? "" : config.GetParam("admmailiso", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailiso";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailiso";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Почтовый ящик для рассылок (Номинация Графика)

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Крымский Вернисаж - Компьютерная графика)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxGrafics"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailgrafics", true) == "-1" ? "" : config.GetParam("admmailgrafics", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailgrafics";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailgrafics";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Почтовый ящик для рассылок (Номинации Декоративно-прикладное творчество: Макраме, Гобелен,..)

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Крымский Вернисаж - ДПТ: Макраме, Гобелен,..)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxDpt1"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmaildpt1", true) == "-1" ? "" : config.GetParam("admmaildpt1", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmaildpt1";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmaildpt1";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Почтовый ящик для рассылок (Номинации Декоративно-прикладное творчество: Керамика, Батик,..)

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Крымский Вернисаж - ДПТ: Керамика, Батик,..)"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxDpt2"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmaildpt2", true) == "-1" ? "" : config.GetParam("admmaildpt2", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmaildpt2";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmaildpt2";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            
            #region  Название почтового ящика для Литературного конкурса

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Литературный конкурс"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxLiterary"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailliterary", true) == "-1" ? "" : config.GetParam("admmailliterary", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailliterary";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailliterary";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Театральный конкурс - Театральное искусство

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Театральное искусство"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxTheatre"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по театральному конкурсу..";
            txtBox.Text = config.GetParam("admmailtheatre", true) == "-1" ? "" : config.GetParam("admmailtheatre", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailtheatre";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailtheatre";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Театральный конкурс - Художественное слово

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Художественное слово"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxSlovo"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по театральному конкурсу..";
            txtBox.Text = config.GetParam("admmailslovo", true) == "-1" ? "" : config.GetParam("admmailslovo", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailslovo";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailslovo";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Театральный конкурс - Хореография

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Хореография"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKhoreografia"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по театральному конкурсу..";
            txtBox.Text = config.GetParam("admmailkhoreografia", true) == "-1" ? "" : config.GetParam("admmailkhoreografia", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkhoreografia";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkhoreografia";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Театральный конкурс - Инструментал

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Инструментал"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxEnsemble"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по театральному конкурсу..";
            txtBox.Text = config.GetParam("admmailensemble", true) == "-1" ? "" : config.GetParam("admmailensemble", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailensemble";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailensemble";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Театральный конкурс - Вокал

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Вокал"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxVoice"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по театральному конкурсу..";
            txtBox.Text = config.GetParam("admmailvoice", true) == "-1" ? "" : config.GetParam("admmailvoice", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailvoice";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailvoice";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Театральный конкурс - Театр моды

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Театральный конкурс - Театр моды"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxModa"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailmoda", true) == "-1" ? "" : config.GetParam("admmailmoda", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailmoda";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailmoda";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Простейшие единоборства

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Простейшие единоборства"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxProedinob"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailsport", true) == "-1" ? "" : config.GetParam("admmailsport", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailsport";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailsport";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Тхэквондо

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Тхэквондо"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxThekvo"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailthekvo", true) == "-1" ? "" : config.GetParam("admmailthekvo", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailthekvo";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailthekvo";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Бокс

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Бокс"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxBoks"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailboks", true) == "-1" ? "" : config.GetParam("admmailboks", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailboks";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailboks";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Кунг-фу УИ

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Кунг-фу УИ"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKungfu"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailkungfu", true) == "-1" ? "" : config.GetParam("admmailkungfu", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkungfu";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkungfu";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Футбол

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Футбол"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxFootball"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailfootball", true) == "-1" ? "" : config.GetParam("admmailfootball", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailfootball";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailfootball";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Баскетбол

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Баскетбол"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxBasketball"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailbasketball", true) == "-1" ? "" : config.GetParam("admmailbasketball", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailbasketball";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailbasketball";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Волейбол

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Волейбол"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxVolleyball"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailvolleyball", true) == "-1" ? "" : config.GetParam("admmailvolleyball", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailvolleyball";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailvolleyball";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Тактический лазертаг

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Тактический лазертаг"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxPaintball"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailpaintball", true) == "-1" ? "" : config.GetParam("admmailpaintball", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailpaintball";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailpaintball";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Шахматы

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Шахматы"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxShahmaty"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailshahmaty", true) == "-1" ? "" : config.GetParam("admmailshahmaty", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailshahmaty";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailshahmaty";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion
            #region Шашки

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Шашки"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxShashky"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailshashky", true) == "-1" ? "" : config.GetParam("admmailshashky", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailshashky";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailshashky";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Открытый мир

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Открытый мир"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKultura"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailkultura", true) == "-1" ? "" : config.GetParam("admmailkultura", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkultura";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkultura";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Топонимика Москвы и Севастополя

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Топонимика Москвы и Севастополя"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxToponim"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailtoponim", true) == "-1" ? "" : config.GetParam("admmailtoponim", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailtoponim";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailtoponim";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Робототехника

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Робототехника 'Крымский мост -два берега России'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxRobotech"; txtBox.Width = 400;
            txtBox.Text = config.GetParam("admmailrobotech", true) == "-1" ? "" : config.GetParam("admmailrobotech", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailrobotech";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailrobotech";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Вместе мы сила

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Вместе мы сила"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxVmesteSila"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по фестивалю Вместе мы сила..";
            txtBox.Text = config.GetParam("admmailvmestesila", true) == "-1" ? "" : config.GetParam("admmailvmestesila", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailvmestesila";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailvmestesila";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Вместе мы сила - мастер макияжа

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Вместе мы сила - мастер макияжа"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxVmesteSilaMakeUp"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по фестивалю Вместе мы сила - мастер макияжа";
            txtBox.Text = config.GetParam("admmailvmestesilamakeup", true) == "-1" ? "" : config.GetParam("admmailvmestesilamakeup", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailvmestesilamakeup";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailvmestesilamakeup";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Вместе мы сила - парикмахерское искусство

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Вместе мы сила - парикмахерское искусство"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxVmesteSilaShair"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по фестивалю Вместе мы сила - парикмахерское искусство";
            txtBox.Text = config.GetParam("admmailvmestesilashair", true) == "-1" ? "" : config.GetParam("admmailvmestesilashair", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailvmestesilashair";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailvmestesilashair";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Индустрия моды

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Конкурс шитья"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxClothes"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по конкурсу Индустрия моды";
            txtBox.Text = config.GetParam("admmailclothes", true) == "-1" ? "" : config.GetParam("admmailclothes", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailclothes";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailclothes";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Юные защитники 1-й и 2-й обороны Севастополя"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxMultimedia"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по конкурсу мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя";
            txtBox.Text = config.GetParam("admmailmultimedia", true) == "-1" ? "" : config.GetParam("admmailmultimedia", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailmultimedia";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailmultimedia";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс "Кораблик детства"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Творческий конкурс 'Кораблик детства'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKorablik"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по творческому конкурсу 'Кораблик детства'";
            txtBox.Text = config.GetParam("admmailkorablik", true) == "-1" ? "" : config.GetParam("admmailkorablik", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkorablik";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkorablik";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс "Кораблик детства (Вокал)"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Творческий конкурс 'Кораблик детства (Вокал)'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKorablikVokal"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по творческому конкурсу 'Кораблик детства (Вокал)'";
            txtBox.Text = config.GetParam("admmailkorablikvokal", true) == "-1" ? "" : config.GetParam("admmailkorablikvokal", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkorablikvokal";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkorablikvokal";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс "Кораблик детства (Хореография)"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Творческий конкурс 'Кораблик детства (Хореография)'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKorablikHoreo"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по творческому конкурсу 'Кораблик детства (Хореография)'";
            txtBox.Text = config.GetParam("admmailkorablikhoreo", true) == "-1" ? "" : config.GetParam("admmailkorablikhoreo", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkorablikhoreo";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkorablikhoreo";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс "Крымские маршруты"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Конкурс 'Крымские маршруты'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxCrimroute"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по конкурсу проектов 'Крымские маршруты'";
            txtBox.Text = config.GetParam("admmailcrimroute", true) == "-1" ? "" : config.GetParam("admmailcrimroute", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailcrimroute";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailcrimroute";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс "Математический батл"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Конкурс 'Математический батл'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxMathbattle"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по конкурсу проектов 'Математический батл'";
            txtBox.Text = config.GetParam("admmailmathbattle", true) == "-1" ? "" : config.GetParam("admmailmathbattle", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailmathbattle";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailmathbattle";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Квест-игра "Покоряя космос"

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Квест-игра 'Покоряя космос'"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxKosmos"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по Квест-игре 'Покоряя космос'";
            txtBox.Text = config.GetParam("admmailkosmos", true) == "-1" ? "" : config.GetParam("admmailkosmos", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailkosmos";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailkosmos";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            #region Конкурс научных работ "В моей лаборатории вот что... "

            panelLine = new Panel(); panelLine.CssClass = "panelLine";
            lbl = new Label(); lbl.CssClass = "lblFormPre moveUp"; lbl.Width = 250; lbl.Text = "Конкурс научных работ 'В моей лаборатории вот что... '"; panelLine.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblFormZvezd moveUp"; lbl.Width = 20; lbl.Text = "*"; panelLine.Controls.Add(lbl);
            txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.ID = "txtBoxAdmMailBoxScience"; txtBox.Width = 400;
            txtBox.ToolTip = "Этот почтовый ящик используется для рассылки уведомлений о поступлении заявок по Конкурсу научных работ 'В моей лаборатории вот что... '";
            txtBox.Text = config.GetParam("admmailscience", true) == "-1" ? "" : config.GetParam("admmailscience", true);
            panelLine.Controls.Add(txtBox);
            panelWrapper.Controls.Add(panelLine);

            //строка с кнопками ПРОВЕРИТЬ и СОХРАНИТЬ
            //panelLine = new Panel(); panelLine.CssClass = "panelLine";
            //ПРОВЕРИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " ПРОВЕРИТЬ "; lBtn.ToolTip = "Проверить правильность введённого названия почтового ящика(отсылается пробное письмо).";
            lBtn.CommandArgument = "admmailscience";
            lBtn.Command += (lBtnTestMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);
            //СОХРАНИТЬ
            lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse"; lBtn.Text = " СОХРАНИТЬ "; lBtn.ToolTip = "Кнопка сохранения нового названия административного почтового ящика.";
            lBtn.CommandArgument = "admmailscience";
            lBtn.Command += (lBtnSaveMailBox1_Command); panelLine.Controls.Add(lBtn);
            panelWrapper.Controls.Add(panelLine);

            #endregion

            return panelWrapper;
        }

        #region События

        #region lBtnTestMailBox1_Command

        /// <summary>событие нажатия на кнопку "ПРОВЕРИТЬ". Проверяются настройки почтового ящика отсылкой на него пробного письма</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnTestMailBox1_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            string txtBoxAdmMailBox = e.CommandArgument.ToString().Trim();

            #region Проверка значения параметра.

            if (txtBoxAdmMailBox == "admmail")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBox")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailfoto")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxFoto")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailiso")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxIso")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailgrafics")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxGrafics")).Text;
            }
            else if (txtBoxAdmMailBox == "admmaildpt1")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxDpt1")).Text;
            }
            else if (txtBoxAdmMailBox == "admmaildpt2")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxDpt2")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailliterary")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxLiterary")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailtheatre")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxTheatre")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailslovo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxSlovo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkhoreografia")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKhoreografia")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvoice")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVoice")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailensemble")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxEnsemble")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmoda")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxModa")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailpaintball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxPaintball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailshahmaty")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxShahmaty")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailshashky")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxShashky")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailsport")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxProedinob")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailboks")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxBoks")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkungfu")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKungfu")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailthekvo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxThekvo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailfootball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxFootball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailbasketball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxBasketball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvolleyball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVolleyball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkultura")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKultura")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailtoponim")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxToponim")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailrobotech")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxRobotech")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesila")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSila")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesilamakeup")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSilaMakeUp")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesilashair")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSilaShair")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailclothes")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxClothes")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmultimedia")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxMultimedia")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablik")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablik")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablikvokal")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablikVokal")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablikhoreo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablikHoreo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailcrimroute")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxCrimroute")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmathbattle")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxMathbattle")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkosmos")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKosmos")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailscience")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxScience")).Text;
            }
            #endregion

            //проверочные условия
            if (txtBoxAdmMailBox.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'Название почтового ящика' не должно быть пустым.", pag.Master); return; }

            //отправим тестовое письмо на административный почтовый ящик
            var sendMailClass = new SendMailClass();
            if (!sendMailClass.SendMail(txtBoxAdmMailBox, "ПРОВЕРКА СВЯЗИ", ""))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибки при передаче проверочного письма на почтовый ящик. Возможно вы неправильно ввели имя почтового ящика.", pag.Master);
            }
        }

        #endregion
        #region lBtnSaveMailBox1_Command

        /// <summary>событие нажатия на кнопку "сохранить"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSaveMailBox1_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            string txtBoxAdmMailBox = e.CommandArgument.ToString().Trim();
            string param = e.CommandArgument.ToString().Trim();

            #region Сохранение значения параметра.

            if (txtBoxAdmMailBox == "admmail")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBox")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailfoto")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxFoto")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailiso")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxIso")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailgrafics")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxGrafics")).Text;
            }
            else if (txtBoxAdmMailBox == "admmaildpt1")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxDpt1")).Text;
            }
            else if (txtBoxAdmMailBox == "admmaildpt2")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxDpt2")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailliterary")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxLiterary")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailtheatre")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxTheatre")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailslovo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxSlovo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkhoreografia")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKhoreografia")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvoice")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVoice")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailensemble")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxEnsemble")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmoda")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxModa")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailpaintball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxPaintball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailshahmaty")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxShahmaty")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailshashky")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxShashky")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailsport")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxProedinob")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailboks")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxBoks")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkungfu")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxkungfu")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailthekvo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxThekvo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailfootball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxFootball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailbasketball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxBasketball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvolleyball")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVolleyball")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkultura")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKultura")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailtoponim")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxToponim")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailrobotech")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxRobotech")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesila")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSila")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesilamakeup")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSilaMakeUp")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailvmestesilashair")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxVmesteSilaShair")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailclothes")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxClothes")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmultimedia")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxMultimedia")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablik")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablik")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablikvokal")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablikVokal")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkorablikhoreo")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKorablikHoreo")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailcrimroute")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxCrimroute")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailmathbattle")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxMathbattle")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailkosmos")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxKosmos")).Text;
            }
            else if (txtBoxAdmMailBox == "admmailscience")
            {
                txtBoxAdmMailBox = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxAdmMailBoxScience")).Text;
            }
            
            #endregion

            //проверочные условия
            if (txtBoxAdmMailBox.Trim() == "") { warning.ShowWarning("ВНИМАНИЕ. Поле 'Название почтового ящика' не должно быть пустым.", pag.Master); return; }

            //сохраним новое название административного почтового ящика
            var config = new ConfigFile();

            if (!config.AddParam(param, txtBoxAdmMailBox, true))
            {
                warning.ShowWarning("ВНИМАНИЕ. Ошибка во время сохранения нового названия административного почтового ящика. Попробуйте повторить.", pag.Master);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменение данных почтового ящика Конкурса (" + txtBoxAdmMailBox + "). Удалил - " + ((AdmPersonStruct)pag.Session["authperson"]).Name);
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region МЕТОДЫ, КАСАЮЩИЕСЯ ОТОБРАЖЕНИЯ СОБЫТИЙ

        /// <summary>функция возвращает таблицу с фильтром событий для журнала консоли</summary>
        /// <returns></returns>
        public Panel GetEventFilterPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "ПРОСМОТР СОБЫТИЙ ВХОДА В КОНСОЛЬ УПРАВЛЕНИЯ"; panelWrapper.Controls.Add(lbl);

            //Поле и кнопка поиска
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = pag.Session["srchStrEvent"].ToString(); txtBox.ID = "txtBoxSrchEvent";
            txtBox.ToolTip = "Введите сюда текст, который нужно найти. Поиск осуществляется по всем данным события.";
            panelWrapper.Controls.Add(txtBox);
            panelWrapper.Controls.Add(new LiteralControl("&nbsp;"));
            var chkBox = new CheckBox(); chkBox.ID = "chkBoxInverse"; chkBox.ToolTip = "поставьте галочку, чтобы найти все строки, не содержащие искомый текст (инверсный поиск)";
            if ((bool)pag.Session["inverseSrch"]) { chkBox.Checked = true; } else { chkBox.Checked = false; }
            panelWrapper.Controls.Add(chkBox);
            panelWrapper.Controls.Add(new LiteralControl("&nbsp;"));
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " найти "; lBtn.ToolTip = "Кнопка поиска события в журнале. Поиск осуществляется по всем данным события.";
            lBtn.Command += (lBtnEventSrch_Command); lBtn.ID = "btnEventSrch"; panelWrapper.Controls.Add(lBtn);

            //Только для администратора с именем "админ"
            if (((AdmPersonStruct)pag.Session["authperson"]).Name == EnumsHelper.GetWritesCode(Writes.admin))
            {
                //Кнопка очистки журнала(файла с самыми ранними событиями)
                panelWrapper.Controls.Add(new LiteralControl("<br />"));
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " очистить журнал "; lBtn.ToolTip = "Кнопка удаления файла с наиболее ранними событиями журнала. Если файл журнал один, то события удалены не будут. (Размер одного файла журнала - 500 килобайт.)";
                lBtn.Command += (lBtnEventClean_Command); panelWrapper.Controls.Add(lBtn);
                var loginWorkClass = new LoginWorkClass(pag);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " (Размер файлов журнала: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = loginWorkClass.GetSizeOfAllEventFiles(); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " килобайт)"; panelWrapper.Controls.Add(lbl);

                //Кнопка очистки журнала(удаляются все файлы событий)
                panelWrapper.Controls.Add(new LiteralControl("<br />"));
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " очистить весь журнал "; lBtn.ToolTip = "Кнопка удаления всех событий журнала.";
                lBtn.Command += (lBtnAllEventClean_Command); panelWrapper.Controls.Add(lBtn);
            }

            return panelWrapper;
        }

        #region События для функции GetEventFilterPanel()

        /// <summary>событие нажатия на кнопку "найти" событие по поисковому запросу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEventSrch_Command(object sender, CommandEventArgs e)
        {
            pag.Session["srchStrEvent"] = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Text;
            if (((string)pag.Session["srchStrEvent"]).Trim() == "") { pag.Session["inverseSrch"] = false; }
            else { pag.Session["inverseSrch"] = ((CheckBox)pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxInverse")).Checked; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "очистить журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            //удаляем все файлы событий
            var loginWorkClass = new LoginWorkClass(pag);
            if (!loginWorkClass.CleanAllEvents(false)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "очистить весь журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAllEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            //удаляем все файлы событий
            var loginWorkClass = new LoginWorkClass(pag);
            if (!loginWorkClass.CleanAllEvents(true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        /// <summary>функция возвращает таблицу со всеми событиями журнала консоли</summary>
        /// <param name="countOfElemInOnePage">кол-во выводимых событий на одну страницу</param>
        /// <returns></returns>
        public Table GetEventListTbl(int countOfElemInOnePage)
        {
            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell();
            Label lbl;

            var loginWorkClass = new LoginWorkClass(pag);
            var listOfStructs = loginWorkClass.GetAllEventStructs((string)pag.Session["srchStrEvent"], (bool)pag.Session["inverseSrch"]);

            int pageCounter = 1;
            int prodCounter = 0;

            //добавим на страницу кнопки-ссылки на страницы продуктов (вверху)
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            var pageBtns = new PagePanelClass(listOfStructs.Count, countOfElemInOnePage);
            tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

            //добавим надпись - всего товара разделе/подразделе
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего событий по данному фильтру: "; tblCell.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = listOfStructs.Count.ToString(); tblCell.Controls.Add(lbl);

            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

            //добавляем подтаблицу
            var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

            //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ
            var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
            var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Время"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "IP источника"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Имя источника"; lbl.ToolTip = "Если имя источника определить не удаётся, то в этом поле записывается - NONAME."; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Логин"; lbl.ToolTip = "В этом поле отображается имя(логин), которое использовалось для входа в консоль."; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Доступ"; lbl.ToolTip = "В этом поле отображается PERMIT, если доступ в консоль был разрешён и PREVENT - если запрещён."; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Информация о клиенте"; lbl.ToolTip = "В этом поле отображается информация о браузере, через которой заходили в консоль."; tblCell1.Controls.Add(lbl);


            //добавляем строки с данными по событиям
            foreach (EventStruct oneStruct in listOfStructs)
            {
                prodCounter++;
                if (prodCounter > countOfElemInOnePage)   //выводим на страницу не более нужного кол-ва событий
                {
                    prodCounter = 1;
                    pageCounter++;
                }

                if (pageCounter == (int)pag.Session["pagenum"])           //если эта та самая страница продуктов, которую нужно вывести, то выводим её
                {
                    tblRow1 = new TableRow();

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);           //дата
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Date; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);           //время
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Time; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //IP-адрес источника
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.HostIp; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //Имя источника
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.HostName; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //Логин
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Login; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);           //Доступ
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Access;
                    if (oneStruct.Access == "PERMIT") { lbl.ForeColor = Color.Green; }
                    else if (oneStruct.Access == "PREVENT") { lbl.ForeColor = Color.Red; }
                    tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //Информация о клиенте
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl";
                    /*if (oneStruct.ClientInfo.Length > 110)
                    {
                        lbl.Text = oneStruct.ClientInfo.Substring(0, 110) + " ...";
                        lbl.ToolTip = oneStruct.ClientInfo;
                    }
                    else
                    {*/
                    lbl.Text = oneStruct.ClientInfo;
                    //}
                    tblCell1.Controls.Add(lbl);

                    tbl1.Controls.Add(tblRow1);
                }
                else if (pageCounter > (int)pag.Session["pagenum"])
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

        /// <summary>функция возвращает таблицу с фильтром событий для журнала сайта</summary>
        /// <returns></returns>
        public Panel GetSiteEventFilterPanel()
        {
            var panelWrapper = new Panel();

            //ЗАГЛАВИЕ
            var lbl = new Label(); lbl.CssClass = "lblSectionTitle"; lbl.Text = "ПРОСМОТР СОБЫТИЙ ПОСЕЩЕНИЯ ПОРТАЛА"; panelWrapper.Controls.Add(lbl);

            //Поле, чекбокс и кнопка поиска в журнале событий
            var txtBox = new TextBox(); txtBox.CssClass = "txtBoxUniverse_adm"; txtBox.Width = 150; txtBox.TextMode = TextBoxMode.SingleLine;
            txtBox.Text = pag.Session["srchStrEvent"].ToString(); txtBox.ID = "txtBoxSrchEvent";
            txtBox.ToolTip = "Введите сюда текст, который нужно найти. Поиск осуществляется по всем данным события.";
            panelWrapper.Controls.Add(txtBox);
            panelWrapper.Controls.Add(new LiteralControl("&nbsp;"));
            var chkBox = new CheckBox(); chkBox.ID = "chkBoxInverse"; chkBox.ToolTip = "поставьте галочку, чтобы найти все строки, не содержащие искомый текст (инверсный поиск)";
            if ((bool)pag.Session["inverseSrch"]) { chkBox.Checked = true; } else { chkBox.Checked = false; }
            panelWrapper.Controls.Add(chkBox);
            panelWrapper.Controls.Add(new LiteralControl("&nbsp;"));
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " найти "; lBtn.ToolTip = "Кнопка поиска события в журнале. Поиск осуществляется по всем данным события.";
            lBtn.Command += (lBtnSiteEventSrch_Command); lBtn.ID = "btnEventSrch"; panelWrapper.Controls.Add(lBtn);

            //Только для администратора с именем "админ"
            if (((AdmPersonStruct)pag.Session["authperson"]).Name == EnumsHelper.GetWritesCode(Writes.admin))
            {
                //Кнопка очистки наиболее старого файла журнала
                panelWrapper.Controls.Add(new LiteralControl("<br />"));
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " очистить журнал "; lBtn.ToolTip = "Кнопка удаления файла с наиболее ранними событиями журнала. Если файл журнал один, то события удалены не будут. (Размер одного файла журнала - 500 килобайт.)";
                lBtn.Command += (lBtnSiteEventClean_Command); panelWrapper.Controls.Add(lBtn);
                var loginWorkClass = new LoginWorkClass(pag);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " (Размер файлов журнала: "; panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = loginWorkClass.GetSizeOfAllSiteEventFiles(); panelWrapper.Controls.Add(lbl);
                lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = " килобайт)"; panelWrapper.Controls.Add(lbl);

                //Кнопка очистки журнала(удаляются все файлы событий)
                panelWrapper.Controls.Add(new LiteralControl("<br />"));
                lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
                lBtn.Text = " очистить весь журнал "; lBtn.ToolTip = "Кнопка удаления всех событий журнала.";
                lBtn.Command += (lBtnAllSiteEventClean_Command); panelWrapper.Controls.Add(lBtn);
            }

            return panelWrapper;
        }

        #region События для функции GetSiteEventFilterPanel()

        /// <summary>событие нажатия на кнопку "найти" событие по поисковому запросу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSiteEventSrch_Command(object sender, CommandEventArgs e)
        {
            pag.Session["srchStrEvent"] = ((TextBox)pag.FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Text;
            if (((string)pag.Session["srchStrEvent"]).Trim() == "") { pag.Session["inverseSrch"] = false; }
            else { pag.Session["inverseSrch"] = ((CheckBox)pag.FindControl("ctl00$ContentPlaceHolder1$chkBoxInverse")).Checked; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "очистить журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSiteEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            //удаляем все файлы событий
            var loginWorkClass = new LoginWorkClass(pag);
            if (!loginWorkClass.CleanAllSiteEvents(false)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        /// <summary>событие нажатия на кнопку "очистить весь журнал"</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnAllSiteEventClean_Command(object sender, CommandEventArgs e)
        {
            var warning = new WarnClass();
            warning.HideWarning(pag.Master);

            //удаляем все файлы событий
            var loginWorkClass = new LoginWorkClass(pag);
            if (!loginWorkClass.CleanAllSiteEvents(true)) { warning.ShowWarning("ВНИМАНИЕ. Ошибка во время удаления одного из файлов журнала. Попробуйте повторить.", pag.Master); return; }

            //перезагрузка страницы с теми же параметрами URL-строки
            if (pag.Request.ServerVariables["QUERY_STRING"] == "") pag.Response.Redirect(pag.Request.ServerVariables["URL"]);
            else pag.Response.Redirect(pag.Request.ServerVariables["URL"] + "?" + pag.Request.ServerVariables["QUERY_STRING"]);
        }

        #endregion

        /// <summary>функция возвращает таблицу со всеми событиями журнала консоли</summary>
        /// <param name="countOfElemInOnePage">кол-во выводимых событий на одну страницу</param>
        /// <returns></returns>
        public Table GetSiteEventListTbl(int countOfElemInOnePage)
        {
            var tbl = new Table(); tbl.CssClass = "tblMain";
            var tblRow = new TableRow(); var tblCell = new TableCell();
            Label lbl;

            var loginWorkClass = new LoginWorkClass(pag);
            var listOfStructs = loginWorkClass.GetAllSiteEventStructs((string)pag.Session["srchStrEvent"], (bool)pag.Session["inverseSrch"]);

            int pageCounter = 1;
            int prodCounter = 0;

            //добавим на страницу кнопки-ссылки на страницы продуктов (вверху)
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            var pageBtns = new PagePanelClass(listOfStructs.Count, countOfElemInOnePage);
            tblCell.Controls.Add(pageBtns.GetPageBtnsTbl("0"));

            //добавим надпись - всего товара разделе/подразделе
            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);
            lbl = new Label(); lbl.CssClass = "lblPredislovie1"; lbl.Text = "Всего событий по данному фильтру: "; tblCell.Controls.Add(lbl);
            lbl = new Label(); lbl.CssClass = "lblPosleslovie"; lbl.Text = listOfStructs.Count.ToString(); tblCell.Controls.Add(lbl);

            tblRow = new TableRow(); tbl.Controls.Add(tblRow);
            tblCell = new TableCell(); tblCell.CssClass = "tblMainCell"; tblRow.Controls.Add(tblCell);

            //добавляем подтаблицу
            var tbl1 = new Table(); tbl1.CssClass = "tblSub"; tblCell.Controls.Add(tbl1);

            //ДОБАВЛЯЕМ ШАПКУ ПОДТАБЛИЦЫ
            var tblRow1 = new TableRow(); tbl1.Controls.Add(tblRow1);
            var tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Дата"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Время"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "IP источника"; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Имя источника"; lbl.ToolTip = "Если имя источника определить не удаётся, то в этом поле записывается - NONAME."; tblCell1.Controls.Add(lbl);
            tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCellHead"; tblRow1.Controls.Add(tblCell1);
            lbl = new Label(); lbl.CssClass = "lblTxtTblHead"; lbl.Text = "Информация о клиенте"; lbl.ToolTip = "В этом поле отображается информация о браузере, через которой заходили в консоль."; tblCell1.Controls.Add(lbl);


            //добавляем строки с данными по событиям
            foreach (EventStruct oneStruct in listOfStructs)
            {
                prodCounter++;
                if (prodCounter > countOfElemInOnePage)   //выводим на страницу не более нужного кол-ва событий
                {
                    prodCounter = 1;
                    pageCounter++;
                }

                if (pageCounter == (int)pag.Session["pagenum"])           //если эта та самая страница продуктов, которую нужно вывести, то выводим её
                {
                    tblRow1 = new TableRow();

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);           //дата
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Date; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell"; tblRow1.Controls.Add(tblCell1);           //время
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.Time; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //IP-адрес источника
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.HostIp; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //Имя источника
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl"; lbl.Text = oneStruct.HostName; tblCell1.Controls.Add(lbl);

                    tblCell1 = new TableCell(); tblCell1.CssClass = "tblSubCell txtAlignLeft"; tblRow1.Controls.Add(tblCell1);           //Информация о клиенте
                    lbl = new Label(); lbl.CssClass = "lblTxtTbl";
                    /*if (oneStruct.ClientInfo.Length > 110)
                    {
                        lbl.Text = oneStruct.ClientInfo.Substring(0, 110) + " ...";
                        lbl.ToolTip = oneStruct.ClientInfo;
                    }
                    else
                    {*/
                    lbl.Text = oneStruct.ClientInfo;
                    //}
                    tblCell1.Controls.Add(lbl);

                    tbl1.Controls.Add(tblRow1);
                }
                else if (pageCounter > (int)pag.Session["pagenum"])
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

        #endregion
    }

    #endregion

    #region Код с описанием структур данных (объектов)

    /// <summary>класс структуры одной опции из файла files\options\options</summary>
    public class OptionsStruct
    {
        public string Name { get; set; }                    //название опции
        public string Data { get; set; }                    //данные опции

        /// <summary>функция возвращает готовую для записи в файл строку из данной структуры товара. 
        /// Строка полностью подготовлена для записи в файл опций. </summary>
        /// <returns></returns>
        public string GetStringFromStruct()
        {
            return Name + "|" + Data;
        }
    }

    /// <summary>перечисление, которое указывает на конкретную опцию из файла /files/options/options
    /// Usdrur - курс доллара по отношению к рублю
    /// SiteEnable - включение или выключение сайта (значение - true или false)</summary>
    public enum OptionName { Usdrur, SiteEnable, AdminMailBox }

    /// <summary>класс содержит данные почтового ящика для рассылок в своих свойствах. Данные из файла files\mail\options</summary>
    public class MailBoxStruct
    {
        public string From { get; set; }                    //от кого
        public string SmtpServer { get; set; }              //DNS-имя SMTP-сервера
        public string ServerPort { get; set; }              //номер порта SMTP-сервера
        public string Ssl { get; set; }                     //ssl-протокол(включён или выключен, 1 или 0 соответственно)
        public string Login { get; set; }                   //логин для учётки почтового ящика
        public string Password { get; set; }                //пароль для учётки почтового ящика

        //в файле все поля хранятся в кодированном виде

        /// <summary>функция возвращает готовую для записи в файл строку из данной структуры товара. 
        /// Строка полностью подготовлена для записи в файл данных почтового ящика. </summary>
        /// <returns></returns>
        public List<string> GetListFromStruct()
        {
            var resultList = new List<string>();
            var encdec = new EncDecClass();

            resultList.Add("from|" + encdec.Enctext(From));
            resultList.Add("dnssmtp|" + encdec.Enctext(SmtpServer));
            resultList.Add("smtpport|" + encdec.Enctext(ServerPort));
            resultList.Add("ssl|" + encdec.Enctext(Ssl));
            resultList.Add("smtpl|" + encdec.Enctext(Login));
            resultList.Add("smtpp|" + encdec.Enctext(Password));

            return resultList;
        }
    }

    #endregion

}