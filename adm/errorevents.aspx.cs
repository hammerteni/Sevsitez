using System;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница отображения ПРЕДУПРЕЖДЕНИЙ И ОШИБОК</summary>
    public partial class errorevents : System.Web.UI.Page
    {
        #region Поля

        private string _prefix = "Errors";

        #endregion

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
            ((LinkButton)Master.FindControl("lBtnToErrEvents")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных во ViewState и обратно

            #region Session["srchStr*"]

            if (Session["srchStr" + _prefix] == null)
            {
                if (ViewState["srchStr" + _prefix] != null)
                {
                    Session["srchStr" + _prefix] = (string)ViewState["srchStr" + _prefix];
                }
                else
                {
                    Session["srchStr" + _prefix] = "";
                }
            }
            else
            {
                ViewState["srchStr" + _prefix] = (string)Session["srchStr" + _prefix];
            }
            #endregion
            #region Session["pagenum*"]

            if (Session["pagenum" + _prefix] == null)
            {
                if (ViewState["pagenum" + _prefix] != null)
                {
                    Session["pagenum" + _prefix] = (int)ViewState["pagenum" + _prefix];
                }
                else
                {
                    Session["pagenum" + _prefix] = 1;
                }
            }
            else
            {
                ViewState["pagenum" + _prefix] = (int)Session["pagenum" + _prefix];
            }

            #endregion

            #endregion

            #region РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                PageFilling();
            }

            #endregion
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                (FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Focus();
                Form.DefaultButton = "ctl00$ContentPlaceHolder1$btnEventSrch";

                ViewState["srchStr" + _prefix] = (string)Session["srchStr" + _prefix];
                ViewState["pagenum" + _prefix] = (int)Session["pagenum" + _prefix];
            }
            catch { }
        }

        #endregion

        private void PageFilling()
        {
            DebugLogForm form = new DebugLogForm(this, _prefix);
            addPanel.Controls.Add(form.ConsolePanel());
        }
    }
}