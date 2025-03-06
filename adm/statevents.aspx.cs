using System;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница показа статистики посещения страниц сайта</summary>
    public partial class statevents : Page
    {
        #region События страницы

        protected override void OnInit(EventArgs e)
        {
            ControlContainer.Controls.Add(LoadControl("~/components/pageStatistics.ascx"));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            #region ВЫДЕЛЕНИЕ КНОПКИ ДАННОГО РАЗДЕЛА НА МАСТЕР-СТРАНИЦЕ

            var s = new Style(); s.CssClass = "lBtnsActive";
            ((LinkButton)Master.FindControl("lBtnToStatistic")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (Session["authperson"] != null)
            {
                if (((AdmPersonStruct)Session["authperson"]).Writes != EnumsHelper.GetWritesValue(Writes.admin))
                {
                    Response.Redirect("~/adm/as.aspx");
                }
            }
        }

        #endregion
    }
}