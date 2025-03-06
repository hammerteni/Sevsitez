using System;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактирования текстовых страниц сайта (УСТАРЕЛА И НЕ ИСПОЛЬЗУЕТСЯ!!!)</summary>
    public partial class pages : System.Web.UI.Page
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
            ((LinkButton)Master.FindControl("lBtnToPages")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных, используемых на этой странице во ViewState и обратно

            #region Session["PagesStruct"] & ViewState["PagesStruct"]

            if (Session["PagesStruct"] == null)
            {
                if (ViewState["PagesStruct"] != null)
                {
                    Session["PagesStruct"] = (PagesStruct)ViewState["PagesStruct"];
                }
            }
            else
            {
                ViewState["PagesStruct"] = (PagesStruct)Session["PagesStruct"];
            }

            #endregion

            #region Session["PagesNameStruct"] & ViewState["PagesNameStruct"]

            if (Session["PagesNameStruct"] == null)
            {
                if (ViewState["PagesNameStruct"] != null)
                {
                    Session["PagesNameStruct"] = (PagesNameStruct)ViewState["PagesNameStruct"];
                }
            }
            else
            {
                ViewState["PagesNameStruct"] = (PagesNameStruct)Session["PagesNameStruct"];
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
                ViewState["PagesStruct"] = (PagesStruct)Session["PagesStruct"];
                ViewState["PagesNameStruct"] = (PagesNameStruct)Session["PagesNameStruct"];
            }
            
        }

        #endregion

        private void PageFilling()
        {
            /*var pagesBlockClass = new PagesFormAdm(this);
            addPanel.Controls.Add(pagesBlockClass.GetPagesChoosePanel());
            addPanel.Controls.Add(pagesBlockClass.GetPagesEditPanel());
            addPanel.Controls.Add(pagesBlockClass.GetMailTelEditPanel());*/
        }

        /*/// <summary>веб-метод для сохранения состояния сессии (JavaScript в файле forAsync.js)</summary>
        /// <returns></returns>
        [WebMethod]
        public static string ReturnOk(string oky)
        {
            return oky;
        }*/
    }
}