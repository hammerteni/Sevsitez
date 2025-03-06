using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактирования или добавления одной новости</summary>
    public partial class newsedit : Page
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
            PagesFormE.DisableCache(this);
            
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

            // Session["tempNewsStruct"]
            if (Session["tempNewsStruct"] == null)
            {
                if (ViewState["tempNewsStruct"] != null)
                {
                    Session["tempNewsStruct"] = (News) ViewState["tempNewsStruct"];
                }
            }
            else
            {
                ViewState["tempNewsStruct"] = (News)Session["tempNewsStruct"];
            }

            // (Session["srchStr1"]
            if (Session["srchStr1"] == null)
            {
                if (ViewState["srchStr1"] != null)
                {
                    Session["srchStr1"] = (string)ViewState["srchStr1"];
                }
                else
                {
                    Session["srchStr1"] = "";
                }
            }
            else
            {
                ViewState["srchStr1"] = (string)Session["srchStr1"];
            }

            #endregion

            #region РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) ||
                ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.newsEditor))
            {
                if (Request.QueryString["id"] != null)
                {
                    PageFilling(Request.QueryString["id"].Trim());
                }
            }

            #endregion
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                (FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Focus();
                Form.DefaultButton = "ctl00$ContentPlaceHolder1$btnEventSrch";

                ViewState["tempNewsStruct"] = (News)Session["tempNewsStruct"];
                ViewState["srchStr1"] = (string)Session["srchStr1"];
            }
            catch { }
        }

        #endregion

        private void PageFilling(string id)
        {
            NewsForm newsForm = new NewsForm(this, 0);
            addPanel.Controls.Add(newsForm.GetOneNewsEditPanel(id));
        }
    }
}