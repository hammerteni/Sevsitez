using System;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактирования текстовых страниц сайта с помощью текстового редактора CKEditor</summary>
    public partial class pageseditor : System.Web.UI.Page
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
            ((LinkButton)Master.FindControl("lBtnToPagesE")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных, используемых на этой странице во ViewState и обратно

            #region Session["PageStructE"]

            if (Session["PageStructE"] == null)
            {
                if (ViewState["PageStructE"] != null)
                {
                    Session["PageStructE"] = (PageStructE)ViewState["PageStructE"];
                }
            }
            else
            {
                ViewState["PageStructE"] = (PageStructE)Session["PageStructE"];
            }

            #endregion

            #endregion

            #region РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (Session["authperson"] != null)
            {
                if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) ||
                    ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.pagesEditor))
                {
                    PageFilling();
                }
            }
            #endregion
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ViewState["PageStructE"] = (PageStructE)Session["PageStructE"];
            }

        }

        #endregion

        #region PageFilling()

        private void PageFilling()
        {
            PagesFormE pagesBlockClass = new PagesFormE(this);
            addPanel.Controls.Add(pagesBlockClass.GetPagesChoosePanel());
            addPanel.Controls.Add(pagesBlockClass.GetPageEditPanel());
        }

        #endregion
    }
}