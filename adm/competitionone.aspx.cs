using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница показа полной информации по одной заявке</summary>
    public partial class competitionone : Page
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
            ((LinkButton)Master.FindControl("lBtnCompetition")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            PagesFormE.DisableCache(this);

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (
               (((AdmPersonStruct)Session["authperson"]).Writes != EnumsHelper.GetWritesValue(Writes.newsEditor)) &&
               (((AdmPersonStruct)Session["authperson"]).Writes != EnumsHelper.GetWritesValue(Writes.pagesEditor)))
            {
                if(Request.QueryString["num"] != null)
                    PageFilling(Request.QueryString["num"], ((AdmPersonStruct)Session["authperson"]).Writes);
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #endregion

        private void PageFilling(string requestNum, string rights)
        {
            Page.Title += requestNum;
            CompetitionsFormCons form = new CompetitionsFormCons(this);
            addPanel.Controls.Add(form.ConsoleCompetitionOnePanel(requestNum, rights));
        }
    }
}