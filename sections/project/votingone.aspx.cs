using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;
using Image = System.Web.UI.WebControls.Image;

namespace site.sections.project
{
    /// <summary>Страница разделения панелей голосования</summary>
    public partial class votingone : System.Web.UI.Page
    {
        //protected override void OnInit(EventArgs e)
        //{
        //    ControlContainer.Controls.Add(LoadControl("~/components/regCompetitionfoto.ascx"));
        //}

        private string _imgUrlPathFoto = "~/files/competitionfiles/foto/";
        private static string _pathToMainFolder = HttpContext.Current.Server.MapPath("~") + @"files\competitionfiles\";
        private string _pathToFotoFolder = _pathToMainFolder + @"foto\";
        const string emailPattern = "^[a-zA-Z0-9._%+-]+@{1}[a-zA-Z0-9.]{2,}[.]{1}[a-zA-Z]{2,6}$";
        const string phonePattern = "^((\\+7|8)+([0-9]){10})$";
        private string baseUrl;
        private string baseUrlNoPort;

        protected void Page_Load(object sender, EventArgs e)
        {

            PagesForm.DisableCache(this);

             baseUrl = "https://" + this.Request.ServerVariables["SERVER_NAME"] + ":" + this.Request.ServerVariables["SERVER_PORT"] + "/";
             baseUrlNoPort = "https://" + this.Request.ServerVariables["SERVER_NAME"];


            if (Request.QueryString["p"] != null)
            {
                PageFill(Request.QueryString["p"]);
            }
            else
            {
                PageFill();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["TempRequest"] = (CompetitionRequest)Session["TempfotoRequest"];
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            #region Запись сессионных переменных во ViewState и обратно

            if (Session["TempfotoRequest"] == null)
            {
                if (ViewState["TempRequest"] != null)
                {
                    Session["TempfotoRequest"] = (CompetitionRequest)ViewState["TempRequest"];
                }
                else
                {
                    ViewState["TempRequest"] = new CompetitionRequest();
                    Session["TempfotoRequest"] = new CompetitionRequest();
                }
            }
            else
            {
                ViewState["TempRequest"] = (CompetitionRequest)Session["TempfotoRequest"];
            }

            #endregion

            Page.Title = "Регистрация на " + EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetPhotoCode(Photo.self));

            PagesFormE form1 = new PagesFormE(this); p = p.Trim();
            if (p == "") form1.UniverseFillPage(addPanelText, "regfoto", PagesFormE.CacheContr.NoCache);
            else form1.UniverseFillPage(addPanelText, p, PagesFormE.CacheContr.NoCache);


            addPanel.Controls.Add(RequestPanel_Foto_Photo());
        }

      
        public Panel RequestPanel_Foto_Photo()
        {
            Panel panelResult = new Panel(); panelResult.CssClass = "divMainWrap_c";

            #region Получение структуры данных

            // Сохраним ссылку на структуру запроса во временную структуру для удобства использования в этом методе
            CompetitionRequest req = new CompetitionRequest();
            req = (CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"];
            req.CompetitionName = EnumsHelper.GetPhotoCode(Photo.self);
            req.SubsectionName = EnumsHelper.GetPhotoValue(Photo.photo);

            #endregion

            #region ЗАГОЛОВОК

            panelResult.Controls.Add(new LiteralControl("<h1>Форма заявки<br/>на участие в Конкурсе \"Крымский вернисаж\"</h1>"));

            #endregion

            #region Панель с ознакомительной информацией

            Panel panel = new Panel(); panel.CssClass = "panelSub_c";
            panel.Controls.Add(new LiteralControl("<p class='abzac_c'>Добрый день! </p>")); // Оргкомитет конкурса просит вас заполнить все пункты Заявки. В случае отсутствии информации, в поля для заполнения вводится слово \"НЕТ\".
            //panel.Controls.Add(new LiteralControl("<p class='abzac_c'>Прием заявок окончен.</p>")); //Дата окончания приема заявок указана в положении конкурса.
            panel.Controls.Add(new LiteralControl("<p class='abzac_c'>Поля, отмеченные звёздочкой (<span class='span_red_c'>*</span>) - <span class='span_red_c'>обязательны для заполнения</span></p>"));
            panelResult.Controls.Add(panel);

            #endregion

            #region Панель с полями для заполнения

            panel = new Panel(); panel.CssClass = "panelSub_c";
            panel.Controls.Add(new LiteralControl("<table class='table_Form_c'>"));

            #region Выпадающий список номинаций

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Номинация</td>"));
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            var ddl = new DropDownList(); ddl.CssClass = "txtBox_c placeHolderForm_c";
            ddl.Items.Add(req.SubsectionName);
            ddl.ID = "DropDown_Subsection";
            ddl.Enabled = false;
            ddl.Width = 516;
            panel.Controls.Add(ddl);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            #endregion



            TextBox txtBox = new TextBox(); LinkButton lBtn = new LinkButton();

            #region Название работ

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Название работы</td>"));                          // Название работы
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Вечерний Севастополь");
            txtBox.Text = req.WorkName; txtBox.ID = "txtBox_WorkName"; txtBox.MaxLength = 100; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            #endregion


            #region Образовательная организация

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Образовательная организация (аббревиатура полностью)</td>"));    // Образовательная организация (полностью)
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "ГБОУ СОШ №, ГБОУ ДО ДДТиМ, МАОУ Лицей №");
            txtBox.Text = req.EducationalOrganization; txtBox.ID = "txtBox_EducationalOrganization"; txtBox.MaxLength = 150; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td></td>"));                                                        // Коммент внизу поля
            panel.Controls.Add(new LiteralControl("<td></td><td class='explanation_c'>"));
            panel.Controls.Add(new LiteralControl("если вы не учащийся, то введите 'НЕТ' в этом поле</td></tr>"));

            #endregion

            #region Структурное подразделение

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Структурное подразделение</td>"));    // Структурное подразделение
            panel.Controls.Add(new LiteralControl("<td></td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Структурное подразделение (дополнительная информация)");
            txtBox.Text = req.Division; txtBox.ID = "txtBox_Division"; txtBox.MaxLength = 150; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            #endregion

            #region Адрес

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Регион</td>"));                               // Регион
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Московская область");
            txtBox.Text = req.Region; txtBox.ID = "txtBox_Region"; txtBox.MaxLength = 50; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Город</td>"));                               // Город
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            ddl = new DropDownList(); ddl.CssClass = "txtBox_c placeHolderForm_c";
            List<string> tmpList = XmlResources.GetTownsPrefList();
            if (tmpList != null)
            {
                foreach (string item in tmpList)
                {
                    ddl.Items.Add(item);
                }
            }
            ddl.ID = "DropDown_TownPrefs";
            ddl.SelectedValue = req.CityPref;
            ddl.Width = 200;
            panel.Controls.Add(ddl);

            panel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c"; txtBox.Width = 294;
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Долгопрудный");
            txtBox.Text = req.City; txtBox.ID = "txtBox_City"; txtBox.MaxLength = 50; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            #endregion

            #region Данные руководителя

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>ФИО руководителя</td>"));                     // ФИО руководителя
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Звягенцев Олег Георгиевич");
            txtBox.Text = req.ChiefFio; txtBox.ID = "txtBox_ChiefFio"; txtBox.MaxLength = 100; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td></td>"));                                                        // Коммент внизу поля
            panel.Controls.Add(new LiteralControl("<td></td><td class='explanation_c'>"));
            panel.Controls.Add(new LiteralControl("если у вас нет руководителя, то введите 'НЕТ' в этом поле</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Должность руководителя</td>"));                     // Должность руководителя
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "Заведующий по учебной части");
            txtBox.Text = req.ChiefPosition; txtBox.ID = "txtBox_ChiefPosition"; txtBox.MaxLength = 150; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td></td>"));                                                        // Коммент внизу поля
            panel.Controls.Add(new LiteralControl("<td></td><td class='explanation_c'>"));
            panel.Controls.Add(new LiteralControl("если у вас нет руководителя, то введите 'НЕТ' в этом поле</td></tr>"));

           // GetLastPartyByChiefEmail(ref panel, ref req, "TempfotoRequest");

            panel.Controls.Add(new LiteralControl("<tr><td class='td_Title_c'>Телефон руководителя (или участника)</td>"));             // Контактный телефон руководителя 
            panel.Controls.Add(new LiteralControl("<td class='td_Zvezda_c'>*</td><td class='td_Content_c'>"));
            txtBox = new TextBox(); txtBox.CssClass = "txtBox_c placeHolderForm_c";
            txtBox.TextMode = TextBoxMode.SingleLine; txtBox.Attributes.Add("placeholder", "89065556677 или +79065556677");
            txtBox.Text = req.ChiefTelephone; txtBox.ID = "txtBox_ChiefTelephone"; txtBox.MaxLength = 20; panel.Controls.Add(txtBox);
            panel.Controls.Add(new LiteralControl("</td></tr>"));

            panel.Controls.Add(new LiteralControl("<tr><td></td>"));                                                        // Коммент внизу поля
            panel.Controls.Add(new LiteralControl("<td></td><td class='explanation_c'>"));
            panel.Controls.Add(new LiteralControl("если у вас нет руководителя, то введите 'НЕТ' в этом поле</td></tr>"));

            #endregion

            #region Согласие на предоставление и обработку информации.

            //Content_Agreements_Common(ref panel, ref req);

            #endregion

            panel.Controls.Add(new LiteralControl("</table>"));

            panelResult.Controls.Add(panel);

            #endregion

            #region Панель добавления материалов работ

            panel = new Panel(); panel.CssClass = "panelSub_c";

            panel.Controls.Add(new LiteralControl("<p class='p_agreement_c'>Добавьте свою работу.</p>"));
            panel.Controls.Add(new LiteralControl("<p class='p_agreement_c'>Для этого нажмите на кнопку 'ОБЗОР', в появившемся окне выберите один файл с изображением со своего жёсткого диска. Затем нажмите на кнопку 'ДОБАВИТЬ'. " +
                                                  "Обратите внимание, что допускается добавление только одного файла изображения в формате JPG и PNG размером не более 2-х мегабайт.</p>"));
            FileUpload fUpload = new FileUpload(); fUpload.CssClass = "fUpload_c"; fUpload.ID = "foto_ImgUpload";
            fUpload.ToolTip = "Загружайте изображение в формате JPG и PNG размером не более 2-х мегабайт";
            panel.Controls.Add(fUpload);

            lBtn = new LinkButton(); lBtn.CssClass = "lBtnAddFoto_c";
            lBtn.Text = "Добавить";
            lBtn.ToolTip = "Загружайте изображение в формате JPG и PNG размером не более 2-х мегабайт";
            lBtn.Command += lBtn_AddFoto_Foto_Photo;
            lBtn.OnClientClick = "waiting('Обработка файла. Подождите..', 0); return pageReady();";       //pageReady() находится в файле site.js и предотвращает нажатие на кнопку пока страница не завершит загрузку
            panel.Controls.Add(lBtn);

            Panel panelWork = new Panel();                                      // панель со ссылкой на изображение 
            panelWork.CssClass = "divWork_c"; panelWork.ID = "panelWorks_c";

            int counter = 0;
            Image img = new Image();
            foreach (string fileName in req.Links)      // алгоритм изначально задуман на добавление множества изображений!!! Ограничение установлено в событии - lBtn_AddFoto
            {
                img = new Image(); img.CssClass = "img_c"; img.ImageUrl = _imgUrlPathFoto + fileName;
                panelWork.Controls.Add(img);
                lBtn = new LinkButton(); lBtn.CssClass = "lBtnDelFoto_c";
                lBtn.ToolTip = "удалить фотографию";
                lBtn.Text = "Удалить";
                lBtn.CommandArgument = fileName;
                lBtn.Command += lBtn_DelFile_Foto_Photo;
                lBtn.OnClientClick = "return pageReady()";       //pageReady() находится в файле site.js и предотвращает нажатие на кнопку пока страница не завершит загрузку
                panelWork.Controls.Add(lBtn);

                counter++;
            }

            panel.Controls.Add(panelWork);

            panelResult.Controls.Add(panel);

            #endregion



            #region Панель оповещения об успехе или об ошибке

            Panel panelWarning = new Panel(); panelWarning.CssClass = "divWarning_c";
            panelWarning.ID = "panelWarning_c";
            panelResult.Controls.Add(panelWarning);

            #endregion

            lBtn = new LinkButton(); lBtn.CssClass = "lBtnSendRequest_c";
            lBtn.Text = "ОТПРАВИТЬ ЗАЯВКУ";
            lBtn.Command += lBtn_SendRequest_Foto_Photo;
            panelResult.Controls.Add(lBtn);
            lBtn.OnClientClick = "return formBtnClick()";

            return panelResult;
        }

        #region События

        #region lBtn_AddFoto_Foto_Photo

        /// <summary>Нажатие на кнопку ДОБАВИТЬ фотоработу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtn_AddFoto_Foto_Photo(object sender, CommandEventArgs e)
        {
            FileUpload fUpload = (FileUpload)this.FindControl("ctl00$ContentPlaceHolder1$foto_ImgUpload");
            bool err = false;

            #region Готовим панель предупреждений

            Panel panelWarn = (Panel)this.FindControl("ctl00$ContentPlaceHolder1$panelWarning_c");
            panelWarn.Controls.Clear();
            Label lbl = new Label(); lbl.Text = ""; lbl.ForeColor = Color.Red;

            #endregion

            #region Ограничение на загрузку только одного изображения

            if (((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"]) != null)
            {
                if (((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"]).Links.Count == 1)
                {
                    lbl.Text = "Вы можете загрузить только один файл!<br/>Если вы хотите заменить загруженный файл, сначала удалите его, а затем добавьте новый."; panelWarn.Controls.Add(lbl);
                    return;
                }
            }
            else
            {
                return;
            }

            #endregion

            if (fUpload.HasFile)
            {
                string fileName = this.Server.HtmlEncode(fUpload.FileName);
                string extension = Path.GetExtension(fileName);

                if (extension != null && ((extension.ToLower() == ".jpg") || (extension.ToLower() == ".png")))     //проверка на допустимые расширения закачиваемого файла
                {
                    int fileSize = fUpload.PostedFile.ContentLength;
                    if (fileSize < 2000000)                              //проверка на допустимый размер закачиваемого файла
                    {
                        #region Код сохранения файла на серевер

                        //создадим имя для файла картинки
                        Random rn = new Random();
                        string fName = DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + "_" + rn.Next(999999) + extension;

                        //сохраним файл в папку с файлами для конкурсных работ (у каждого конкурса своя папка)
                        try
                        {
                            fUpload.SaveAs(_pathToFotoFolder + fName);
                            fUpload.Dispose();
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                            lbl.Text = "Не удалось сохранить файл. Попробуйте повторить или сообщите нам!"; panelWarn.Controls.Add(lbl);
                            err = true;
                        }
                        if (err) return;

                        #region Добавление имени файла во временный список

                        try
                        {
                            ((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"]).Links.Add(fName); // добавляем в список

                            // для сохранения значений в сессии
                            FieldsValueSave((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"], this);
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                            lbl.Text = "Не удалось сохранить файл. Попробуйте повторить или сообщите нам!"; panelWarn.Controls.Add(lbl);
                            err = true;
                        }
                        if (err) return;

                        #endregion

                        this.Response.Redirect(this.Request.RawUrl);

                        #endregion
                    }
                    else { lbl.Text = "Допускаются только файлы изображений размером не более 2-х мегабайт"; panelWarn.Controls.Add(lbl); }
                }
                else { lbl.Text = "Допускаются только файлы изображений JPG и PNG"; panelWarn.Controls.Add(lbl); }
            }
            else { lbl.Text = "Сначала выберите файл через кнопку 'ОБЗОР'"; panelWarn.Controls.Add(lbl); }
        }

        #endregion
        #region lBtn_DelFile_Foto_Photo

        /// <summary>Нажатие на кнопку КРЕСТИК-УДАЛЕНИЕ файла с материалами работы</summary>
        /// <param name="sender"></param>
        /// <param name="e">содержит имя файла картинки с расширением</param>
        private void lBtn_DelFile_Foto_Photo(object sender, CommandEventArgs e)
        {
            string fName = e.CommandArgument.ToString();

            #region Готовим панель предупреждений

            Panel panelWarn = (Panel)this.FindControl("ctl00$ContentPlaceHolder1$panelWarning_c");
            panelWarn.Controls.Clear();
            Label lbl = new Label(); lbl.Text = ""; lbl.ForeColor = Color.Red;

            #endregion

            #region Удаляем имя файла из сессионной переменной

            bool err = false;
            try
            {
                ((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"]).Links.Remove(fName); // удаляем из списка 
                // для сохранения значений в сессии
                FieldsValueSave((CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"], this);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                lbl.Text = "Не удалось удалить файл. Попробуйте повторить или сообщите нам!"; panelWarn.Controls.Add(lbl);
                err = true;
            }
            if (err) return;

            #endregion

            #region Удаляем файл с диска

            try
            {
                if (File.Exists(_pathToFotoFolder + fName)) File.Delete(_pathToFotoFolder + fName);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                lbl.Text = "Не удалось удалить файл. Попробуйте повторить или сообщите нам!"; panelWarn.Controls.Add(lbl);
                err = true;
            }
            if (err) return;

            #endregion

            this.Response.Redirect(this.Request.RawUrl);
        }

        #endregion
        #region lBtn_SendRequest_Foto_Photo

        /// <summary>Нажатие на кнопку ОТПРАВИТЬ ЗАЯВКУ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lBtn_SendRequest_Foto_Photo(object sender, CommandEventArgs e)
        {
            Label lbl = new Label(); Panel panelWarn = new Panel(); TextBox txtBox = new TextBox();

            try
            {
                #region Получение ссылки на структуру данных

                CompetitionRequest req = new CompetitionRequest();
                req = (CompetitionRequest)HttpContext.Current.Session["TempfotoRequest"];

                #endregion

                #region Готовим панель предупреждений

                panelWarn = (Panel)this.FindControl("ctl00$ContentPlaceHolder1$panelWarning_c");
                panelWarn.Controls.Clear();
                lbl.Text = ""; lbl.ForeColor = Color.Red;

                #endregion

                #region Сбор переменных и привязка их к структуре

                bool checker = true;
                req.DateReg = DateTime.Now.Ticks;
                string errTxt = "Неправильное заполнение полей. Исправьте значения в выделенных красным полях и повторите попытку.";


                #region Образовательная организация (полностью) +

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_EducationalOrganization");
                if (txtBox != null && txtBox.Text.Trim().ToUpper() == "НЕТ")
                {
                    req.EducationalOrganization = txtBox.Text.Trim().ToUpper();
                    txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                }
                else
                {
                    if (txtBox.Text.Trim() == "" && txtBox.Text.Trim().Length < 3)
                    {
                        checker = false;
                        txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                    }
                    else
                    {
                        req.EducationalOrganization = txtBox.Text.Trim();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                }

                #endregion

                #region Структурное подразделение

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_Division");
                if (!string.IsNullOrEmpty(txtBox.Text))
                {
                    req.Division = txtBox.Text.Trim();
                }

                #endregion

                #region Регион

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_Region");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim() == "" && txtBox.Text.Trim().Length < 2)
                    {
                        checker = false;
                        txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                    }
                    else
                    {
                        req.Region = txtBox.Text.Trim();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                }
               

                #endregion

                #region Город

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_City");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim() == "" && txtBox.Text.Trim().Length < 2)
                    {
                        checker = false;
                        txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                    }
                    else
                    {
                        req.City = txtBox.Text.Trim();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                }

                #endregion

                #region ФИО руководителя +

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefFio");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim().ToUpper() == "НЕТ")
                    {
                        req.ChiefFio = txtBox.Text.Trim().ToUpper();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                    else
                    {
                        if (!CompetitonWorkCommon.IsFioOk(txtBox.Text))
                        {
                            checker = false;
                            txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                        }
                        else
                        {
                            req.ChiefFio = txtBox.Text.Trim();
                            txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                        }
                    }
                }

                #endregion

                #region Должность руководителя +

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefPosition");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim().ToUpper() == "НЕТ")
                    {
                        req.ChiefPosition = txtBox.Text.Trim().ToUpper();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                    else
                    {
                        if (txtBox.Text.Trim() == "" && txtBox.Text.Trim().Length < 5)
                        {
                            checker = false;
                            txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                        }
                        else
                        {
                            req.ChiefPosition = txtBox.Text.Trim();
                            txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                        }
                    }
                }
                #endregion

                #region Электронная почта руководителя +

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefEmail");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim().ToUpper() == "НЕТ")
                    {
                        req.ChiefEmail = txtBox.Text.Trim().ToUpper();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                    else
                    {
                        if (!Regex.IsMatch(txtBox.Text.Trim(), emailPattern))
                        {
                            checker = false;
                            txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                        }
                        else
                        {
                            req.ChiefEmail = txtBox.Text.Trim();
                            txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                        }
                    }
                }

                #endregion

                #region Контактный телефон руководителя +

                txtBox = (TextBox)this.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefTelephone");
                if (txtBox != null)
                {
                    if (txtBox.Text.Trim().ToUpper() == "НЕТ")
                    {
                        req.ChiefTelephone = txtBox.Text.Trim().ToUpper();
                        txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                    }
                    else
                    {
                        if (!Regex.IsMatch(txtBox.Text.Trim(), phonePattern))
                        {
                            checker = false;
                            txtBox.BorderWidth = 1; txtBox.BorderColor = Color.Red; txtBox.BackColor = Color.Bisque;
                        }
                        else
                        {
                            req.ChiefTelephone = txtBox.Text.Trim();
                            txtBox.BorderWidth = 0; txtBox.BorderColor = Color.Transparent; txtBox.BackColor = Color.White;
                        }
                    }
                }
                #endregion

               

               
                #region Работы

                if (req.Links.Count == 0)
                {
                    checker = false;
                    errTxt += "<br/>Загрузите файл(ы) работ";
                }

                #endregion



              

                
                #region Итоговые операции

                if (!checker)
                {
                    lbl.Text = errTxt; panelWarn.Controls.Add(lbl);
                    return;
                }
                else
                {
                    //Добавляем префикс только в конце, чтобы он не сохранился в сессии
                    req.City = req.CityPref + " " + req.City;
                }

                #endregion

                #endregion

                #region Сохранение заявки в базу данных

                CompetitionsWork competitionsWork = new CompetitionsWork();
                competitionsWork.InsertOneRequest(ref req);
                if (req.Id == -1)
                {
                    lbl.Text = "Не удалось сохранить вашу заявку. Попробуйте повторить или сообщите нам об этом!"; panelWarn.Controls.Add(lbl);
                    return;
                }

                #endregion

                #region Отправка уведомления администратору, ответственному за данный конкурс по почте

                string competitionName = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetPhotoCode(Photo.self)) + ". Номинация «" + req.SubsectionName + "»";
                string admMailParamName = "admmailfoto";
                string fileUrlPath = _imgUrlPathFoto.Replace("~/", "");

                string[] fios_1 = req.Fios_1.Split(new[] { '|' });
                string[] agies_1 = req.Agies_1.Split(new[] { '|' });
                string[] schools_1 = req.Schools_1.Split(new[] { '|' });
                string[] classRooms_1 = req.ClassRooms_1.Split(new[] { '|' });

                DateTime dt = new DateTime(req.DateReg);

                StringBuilder sameEmailData = new StringBuilder("<br /><br />" + "Данные по заявке:" + "<br /><br />" +
                                     "<table><tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Номер заявки и дата:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.Id.ToString() + " от " + dt.ToShortDateString() + " (поступила в " +
                                     dt.ToShortTimeString() + ")" +
                                     "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>ФИО/Дата рождения:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.Fio + " / " + req.Age +
                                     "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Место учёбы/Класс с литерой|Курс:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.Schools + " / " + req.ClassRooms +
                                     "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Название тематики:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.SubsectionName +
                                     "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Название работы:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.WorkName +
                                     "</td></tr>" +
                                     //"<tr><td style = 'padding: 0 9px 5px 9px;' > " +
                                     //"<span style='color:blue; font-weight:bold;'>Комментарий к работе:</span>" +
                                     //"</td><td style='padding: 0 9px 5px 9px;'>" +
                                     //req.WorkComment +
                                     //"</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Образовательная организация (полностью):</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.EducationalOrganization + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Структурное подразделение:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.Division + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Регион:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.Region + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Город:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.City + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>ФИО руководителя:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.ChiefFio + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Должность руководителя:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.ChiefPosition + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Электронная почта руководителя:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.ChiefEmail + "</td></tr>" +
                                     "<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>Контактный телефон руководителя:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     req.ChiefTelephone +
                                    "</td></tr>");

                sameEmailData.Append("<tr><td style='padding: 0 9px 5px 9px;' colspan='2'>" +
                                    "<span style='color:#000; font-weight:bold;'>Участники 1-го тура, не прошедшие во 2-й тур</span>" +
                                    "</td></tr>");
                for (int i = 0; i < fios_1.Length; i++)
                {
                    sameEmailData.Append("<tr><td style='padding: 0 9px 5px 9px;'>" +
                                     "<span style='color:blue; font-weight:bold;'>" + (i + 1) + ". Фамилия Имя Отчетство/Дата рождения:</span>" +
                                     "</td><td style='padding: 0 9px 5px 9px;'>" +
                                     fios_1[i] + " / " + agies_1[i] +
                                     "</td></tr>");
                    sameEmailData.Append("<tr><td style='padding: 0 9px 5px 9px;'>" +
                                    "<span style='color:blue; font-weight:bold;'>" + "   Место учёбы/Класс с литерой|Курс:</span>" +
                                    "</td><td style='padding: 0 9px 5px 9px;'>" +
                                    schools_1[i] + " / " + classRooms_1[i] +
                                    "</td></tr>");
                }
                sameEmailData.Append("</table>");

                StringBuilder mailbodyAdm = new StringBuilder();
                mailbodyAdm.Append("Здравствуйте!" + "<br /><br />" +
                                    "Поступила заявка на участие в конкурсе!<br /><br />" +
                                    "<strong>" + competitionName + "</strong><br />" +
                                     sameEmailData.ToString() +
                                     "<br />Прямые ссылки на работы заявителя:<br />");

                int counter = 1;
                foreach (string fName in req.Links)
                {
                    mailbodyAdm.Append("<a href='" + baseUrl + fileUrlPath + fName + "'>ссылка" + counter + "</a><br/>");
                    counter++;
                }
                mailbodyAdm.Append("<br /><br />Для утверждения заявки перейдите в <a href='" + baseUrl + "adm" + "'>Консоль администрирования</a>");
                mailbodyAdm.Append("<br /><br />Это письмо сформировано автоматически, отвечать на него не нужно.");

                var config = new ConfigFile();
                string adminMail = config.GetParam(admMailParamName, true);
                if (adminMail == "-1")
                {
                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Не удалось найти почтовый ящик администратора конкурса (искался ящик - " + admMailParamName + ")");
                }
                //если письмо не отослано, то..
                if (!new SendMailClass().SendMail(adminMail, "Письмо о поступлении заявки. " + competitionName, mailbodyAdm.ToString()))
                {
                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Не удалось отправить письмо о поступлении заявки (№ " + req.Id.ToString() + ") на почтовый ящик администратора конкурса (" + adminMail + ").");
                }

                #endregion

                Content_SucessRegistrationRequest(req, competitionName, sameEmailData);

                HttpContext.Current.Session["TempfotoRequest"] = null;
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                lbl.Text = "Не удалось отправить заявку. Попробуйте повторить или сообщите нам!"; panelWarn.Controls.Add(lbl);

                #endregion
            }

        }

        #endregion
        #endregion
        #endregion

        #region Метод FieldsValueSave(CompetitionRequest req, Page page)

        /// <summary>Метод пробегается по полям формы заявки и сохраняет их в переданную структуру</summary>
        /// <param name="req">структура данных заявки, в которую метод записывает данные</param>
        /// <param name="page">страница, с которой собираются данные</param>
        private void FieldsValueSave(CompetitionRequest req, Page page)
        {
            CheckBox chkBox;
            TextBox txtBox;
            DropDownList ddl;
            int tmpNum = 0;

            try
            {
                #region Номинация

                ddl = (DropDownList)page.FindControl("ctl00$ContentPlaceHolder1$DropDown_Subsection");
                if (ddl != null)
                {
                    req.SubsectionName = ddl.SelectedValue;
                }

                #endregion

                #region Возрастная категория

                ddl = (DropDownList)page.FindControl("ctl00$ContentPlaceHolder1$DropDown_AgeСategories");
                if (ddl != null)
                {
                    req.AgeСategory = ddl.SelectedValue;
                }

                #endregion

                #region Количество участников I отборочного тура

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_PartyCount");
                if (txtBox != null)
                {
                    tmpNum = StringToNum.ParseInt(txtBox.Text);
                    if (tmpNum < 0)
                    {
                        tmpNum = 0;
                    }
                    req.PartyCount = tmpNum;
                }

                #endregion

                #region ФИО

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Fio");
                if (txtBox != null)
                {
                    req.Fio = txtBox.Text;
                }

                #endregion

                #region Дата рождения

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Age");
                if (txtBox != null)
                {
                    req.Age = txtBox.Text;
                }

                #endregion

                #region Место учебы

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_School");
                if (txtBox != null)
                {
                    req.School = txtBox.Text;
                }
                #endregion

                #region Класс с литерой

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ClassRoom");
                if (txtBox != null)
                {
                    req.ClassRoom = txtBox.Text;
                }


                #endregion

                #region ФИО_1

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Fio_1");
                if (txtBox != null)
                {
                    req.Fio_1 = txtBox.Text;
                }

                #endregion

                #region Дата рождения_1

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Age_1");
                if (txtBox != null)
                {
                    req.Age_1 = txtBox.Text;
                }

                #endregion

                #region Место учебы_1

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_School_1");
                if (txtBox != null)
                {
                    req.School_1 = txtBox.Text;
                }

                #endregion

                #region Класс с литерой_1

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ClassRoom_1");
                if (txtBox != null)
                {
                    req.ClassRoom_1 = txtBox.Text;
                }

                #endregion

                #region ФИО автора

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_AthorFio");
                if (txtBox != null)
                {
                    req.AthorFio = txtBox.Text;
                }

                #endregion

                #region Хронометраж - минуты

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Timing_min");
                if (txtBox != null)
                {
                    tmpNum = StringToNum.ParseInt(txtBox.Text);
                    if (tmpNum < 0)
                    {
                        tmpNum = 0;
                    }
                    req.Timing_min = tmpNum;
                }

                #endregion

                #region Хронометраж - секунды

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Timing_sec");
                if (txtBox != null)
                {
                    tmpNum = StringToNum.ParseInt(txtBox.Text);
                    if (tmpNum < 0)
                    {
                        tmpNum = 0;
                    }
                    req.Timing_sec = tmpNum;
                }

                #endregion

                #region Вес

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Weight");
                if (txtBox != null)
                {
                    int res = StringToNum.ParseInt(txtBox.Text);
                    if (res <= -1)   //сразу проверка на ошибочное значение и на отрицательные цифры
                    {
                        req.Weight = 0;
                    }
                    else
                    {
                        req.Weight = res;
                    }
                }

                #endregion

                #region Вес_1

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Weight_1");
                if (txtBox != null)
                {
                    int res = StringToNum.ParseInt(txtBox.Text);
                    if (res <= -1)   //сразу проверка на ошибочное значение и на отрицательные цифры
                    {
                        req.Weight_1 = 0;
                    }
                    else
                    {
                        req.Weight_1 = res;
                    }
                }

                #endregion

                #region Техническое сопровождение

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_TechnicalInfo");
                if (txtBox != null)
                {
                    req.TechnicalInfo = txtBox.Text;
                }

                #endregion

                #region Название работы

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_WorkName");
                if (txtBox != null)
                {
                    req.WorkName = txtBox.Text;
                }

                #endregion

                #region Комментарий к работе

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_WorkComment");
                if (txtBox != null)
                {
                    req.WorkComment = txtBox.Text;
                }

                #endregion

                #region Образовательная организация (полностью) +

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_EducationalOrganization");
                if (txtBox != null)
                {
                    req.EducationalOrganization = txtBox.Text;
                }

                #endregion

                #region Структурное подразделение

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Division");
                if (txtBox.Text != null)
                {
                    req.Division = txtBox.Text.Trim();
                }

                #endregion

                #region Регион

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_Region");
                if (txtBox != null)
                {
                    req.Region = txtBox.Text;
                }

                #endregion

                #region Город

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_City");
                if (txtBox != null)
                {
                    req.City = txtBox.Text;
                }

                #endregion

                #region Префикс Города

                ddl = (DropDownList)page.FindControl("ctl00$ContentPlaceHolder1$DropDown_TownPrefs");
                if (ddl != null)
                {
                    req.CityPref = ddl.SelectedValue;
                }

                #endregion

                #region ФИО руководителя +

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefFio");
                if (txtBox != null)
                {
                    req.ChiefFio = txtBox.Text;
                }

                #endregion

                #region Должность руководителя +

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefPosition");
                if (txtBox != null)
                {
                    req.ChiefPosition = txtBox.Text;
                }

                #endregion

                #region Электронная почта руководителя +

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefEmail");
                if (txtBox != null)
                {
                    req.ChiefEmail = txtBox.Text;
                }

                #endregion

                #region Контактный телефон руководителя +

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ChiefTelephone");
                if (txtBox != null)
                {
                    req.ChiefTelephone = txtBox.Text;
                }

                #endregion

                #region Соглашение по обработке персданных

                chkBox = (CheckBox)page.FindControl("ctl00$ContentPlaceHolder1$chkBox_PdnProcessing");
                if (chkBox != null)
                {
                    req.PdnProcessing = chkBox.Checked;
                }

                #endregion

                #region Соглашение на публикацию работ с сохранением авторства

                chkBox = (CheckBox)page.FindControl("ctl00$ContentPlaceHolder1$chkBox_PublicAgreement");
                if (chkBox != null)
                {
                    req.PublicAgreement = chkBox.Checked;
                }

                #endregion

                #region Медицинская справка

                chkBox = (CheckBox)page.FindControl("ctl00$ContentPlaceHolder1$chkBox_ProcMedicine");
                if (chkBox != null)
                {
                    req.ProcMedicine = chkBox.Checked;
                }

                #endregion

                #region Наименование клуба

                txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ClubName");
                if (txtBox != null)
                {
                    req.ClubsName = txtBox.Text;
                }

                #endregion

                #region Количество участников I отборочного тура согласно протоколу

                //txtBox = (TextBox)page.FindControl("ctl00$ContentPlaceHolder1$txtBox_ProtocolPartyCount");
                //if (txtBox != null)
                //{
                //    int res = StringToNum.ParseInt(txtBox.Text);
                //    if (res != -1)
                //    {
                //        req.ProtocolPartyCount = res;
                //    }
                //}

                #endregion

            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion

        #region Content_SucessRegistrationRequest
        public void Content_SucessRegistrationRequest(CompetitionRequest req, string competitionName, StringBuilder sameEmailData)
        {
            #region Отправка уведомления заявителю, подавшему заявку на конкурс

            var mailbodyAdm = new StringBuilder();
            mailbodyAdm.Append("Здравствуйте!" + "<br /><br />" +
                                "Ваша заявка № " + req.Id.ToString() + " от " + new DateTime(req.DateReg).ToShortDateString() + " отправлена и находится в обработке!<br /><br />" +
                                "<strong>" + competitionName + "</strong>" +
                                sameEmailData.ToString() +
                                 "<br/><strong>После обработки Заявки Вам на почту придет подтверждение о регистрации на Конкурс.</strong>" +
                                 "<br/><strong>В случае некорректного заполнения Заявки куратор свяжется с Вами по указанному телефону.</strong>" +
                                 "<br/><strong>Заявка может быть аннулирована при невыполнении всех требований заполнения Заявки.</strong>" +
                                 "<br/><br/>C уважением,<br/>" +
                                 "Администрация Государственного бюджетного образовательного учреждения  дополнительного образования города Москвы «Дворец творчества детей и молодежи «Севастополец»." +
                                 "<br /><br />Следите за анонсами мероприятий Проекта на сайте <a href='" + baseUrlNoPort + "'>" + baseUrlNoPort + "</a>" +
                                 "<br /><br />Это письмо сформировано автоматически, отвечать на него не нужно." +
                                 "<br /><br /><img src='" + baseUrl + "files/shared/logo.png' alt='Логотип нашего учреждения' />");

            //если письмо не отослано, то..
            if (!new SendMailClass().SendMail(req.ChiefEmail, "Оповещение о регистрации заявки. " + competitionName, mailbodyAdm.ToString()))
            {
                DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Не удалось отправить письмо конкурсанту о регистрации его заявки № " + req.Id.ToString() + " от " + new DateTime(req.DateReg).ToShortDateString() + " в " + competitionName + " На почту - " + req.ChiefEmail);
            }

            #endregion

            #region Сообщение об успешном приёме заявки

            try
            {
                Panel panAll = (Panel)this.FindControl("ctl00$ContentPlaceHolder1$addPanel");
                panAll.Controls.Clear();

                panAll.Controls.Add(new LiteralControl("<table class='tblSuccess_c'><tr><td>"));
                panAll.Controls.Add(new LiteralControl("<img src='../../../images/galochka.png'>"));
                panAll.Controls.Add(new LiteralControl("<span>" + "Ваша заявка № " + req.Id.ToString() + " отправлена и находится в обработке!" + "</span>"));
                panAll.Controls.Add(new LiteralControl("</td></tr></table>"));

            }
            catch (Exception ex)
            {
                Label lbl = new Label(); Panel panelWarn = new Panel();
                lbl.Text = "Ваша заявка № " + req.Id.ToString() + " отправлена и находится в обработке!";
                lbl.ForeColor = Color.Green;
                panelWarn.Controls.Add(lbl);
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            #endregion
        }
        #endregion
    }
}