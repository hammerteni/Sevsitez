using System;
using System.Reflection;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактирования вкл/выкл сайта, добавления защитного изображения и добавления файлов в общий доступ</summary>
    public partial class siteenable : System.Web.UI.Page
    {
        #region События страницы

        protected void Page_PreInit(object sender, EventArgs e)
        {

        }
        protected void Page_Init(object sender, EventArgs e)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            #region ВЫДЕЛЕНИЕ КНОПКИ ДАННОГО РАЗДЕЛА НА МАСТЕР-СТРАНИЦЕ

            var s = new Style(); s.CssClass = "lBtnsActive";
            ((LinkButton)Master.FindControl("lBtnSiteEnable")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных во ViewState и обратно

            #region Session["srchStr2"]
            if (Session["srchStr2"] == null)
            {
                if (ViewState["srchStr2"] != null)
                {
                    Session["srchStr2"] = (string)ViewState["srchStr2"];
                }
                else
                {
                    ViewState["srchStr2"] = "";
                    Session["srchStr2"] = "";
                }
            }
            else
            {
                ViewState["srchStr2"] = (string)Session["srchStr2"];
            }
            #endregion

            #region Session["pagenum1"]
            if (Session["pagenum1"] == null)
            {
                if (ViewState["pagenum1"] != null)
                {
                    Session["pagenum1"] = (int)ViewState["pagenum1"];
                }
                else
                {
                    ViewState["pagenum1"] = 1;
                    Session["pagenum1"] = 1;
                }
            }
            else
            {
                ViewState["pagenum1"] = (int)Session["pagenum1"];
            }
            #endregion

            #endregion

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (Session["authperson"] != null)
            {
                if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
                {
                    PageFilling();
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                (FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Focus();
                Form.DefaultButton = "ctl00$ContentPlaceHolder1$btnEventSrch";

                #region Запись сессионных переменных во ViewState и обратно

                ViewState["srchStr2"] = (string)Session["srchStr2"];
                ViewState["pagenum1"] = (int)Session["pagenum1"];

                #endregion
            }
            catch
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "ОШИБКА при обращении к сессионной переменной Session['srchStr2'].");

                #endregion
            }
        }

        #endregion

        #region Метод PageFilling()

        private void PageFilling()
        {
            var optionsForm = new OptionsForm(this);
            addPanel.Controls.Add(optionsForm.GetSiteEnablePanel());
            addPanel.Controls.Add(optionsForm.GetOverlayImageAddPanel());   //панель добавления картинки для наложения на изображения

            SharedFilesForm form = new SharedFilesForm(this, 50);
            addPanel.Controls.Add(form.GetFilterPanel());
            addPanel.Controls.Add(form.GetListTbl());
        }

        #endregion
    }
}