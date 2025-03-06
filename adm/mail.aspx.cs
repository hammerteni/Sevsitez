using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактора настроек почты</summary>
    public partial class mail : System.Web.UI.Page
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
            ((LinkButton)Master.FindControl("lBtnToMail")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
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
            var optionForm = new OptionsForm(this);
            addPanel.Controls.Add(optionForm.GetMailBoxEditPanel());
            addPanel.Controls.Add(optionForm.GetAdminMailBoxEditPanel());
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