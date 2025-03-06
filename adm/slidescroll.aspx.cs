using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактирования анимированных списков фотографий, которые можно добавлять на страницы</summary>
    public partial class slidescroll : System.Web.UI.Page
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
            ((LinkButton)Master.FindControl("lBtnSlideScroll")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) ||
                ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.pagesEditor))
            {
                PageFilling();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #endregion

        private void PageFilling()
        {
            var slideShow = new SlideScrollFormAdm(this);
            addPanel.Controls.Add(slideShow.GetSlideScrollChoosePanel());
            addPanel.Controls.Add(slideShow.GetSlideScrollEditPanel());
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