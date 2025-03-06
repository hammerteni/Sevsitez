using System;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактора новостной ленты</summary>
    public partial class news : System.Web.UI.Page
    {
        private const int CountOfElemInOnePage = 37;

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
            ((LinkButton)Master.FindControl("lBtnToNews")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) { Response.Redirect("~/adm/as.aspx"); }

            #endregion

            #region Запись сессионных переменных во ViewState и обратно

            // Session["srchStr"]
            if (Session["srchStr"] == null)
            {
                if (ViewState["srchStr"] != null)
                {
                    Session["srchStr"] = (string)ViewState["srchStr"];
                }
                else
                {
                    Session["srchStr"] = "";
                }
            }
            else
            {
                ViewState["srchStr"] = (string)Session["srchStr"];
            }

            // Session["pagenum"]
            if (Session["pagenum"] == null)
            {
                if (ViewState["pagenum"] != null)
                {
                    Session["pagenum"] = (int)ViewState["pagenum"];
                }
            }
            else
            {
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }

            #endregion

            #region РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) ||
                ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.newsEditor))
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

                ViewState["srchStr"] = (string)Session["srchStr"];
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }
            catch { }
        }

        #endregion

        private void PageFilling()
        {
            NewsForm newsForm = new NewsForm(this, CountOfElemInOnePage);
            addPanel.Controls.Add(newsForm.GetNewsFilterPanel());
            addPanel.Controls.Add(newsForm.GetNewsListTbl());
        }
    }
}