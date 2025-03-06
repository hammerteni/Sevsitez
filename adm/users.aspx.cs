using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактора учётных записей пользователей консоли</summary>
    public partial class users : System.Web.UI.Page
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
            ((LinkButton)Master.FindControl("lBtnToUsers")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            //РАЗГРАНИЧЕНИЕ ДОСТУПА
            if (Session["authperson"] != null)
            {
                if (((AdmPersonStruct)Session["authperson"]).Writes != EnumsHelper.GetWritesValue(Writes.admin))
                {
                    Response.Redirect("~/adm/as.aspx");
                }
            }

            #endregion

            PageFilling();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #endregion

        private void PageFilling()
        {
            var usersForm = new UsersForm(this);
            addPanel.Controls.Add(usersForm.GetAccountTitlePanel());
            addPanel.Controls.Add(usersForm.GetAccountListTbl());
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