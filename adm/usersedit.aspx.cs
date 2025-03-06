using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактора учётных записей пользователей консоли</summary>
    public partial class usersedit : System.Web.UI.Page
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
            if (Session["TempAccount"] == null)
            {
                Response.Redirect("~/adm/users.aspx");
            }
            
            #region ВЫДЕЛЕНИЕ КНОПКИ ДАННОГО РАЗДЕЛА НА МАСТЕР-СТРАНИЦЕ

            var s = new Style(); s.CssClass = "lBtnsActive";
            ((LinkButton)Master.FindControl("lBtnToUsers")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных, используемых на этой странице во ViewState и обратно

            #region Session["TempAccount"] & ViewState["TempAccount"]
            if (Session["TempAccount"] == null)
            {
                if (ViewState["TempAccount"] != null)
                {
                    Session["TempAccount"] = (AdmPersonStruct)ViewState["TempAccount"];
                }
                else
                {
                    Response.Redirect("~/adm/users.aspx");
                }
            }
            else
            {
                ViewState["TempAccount"] = (AdmPersonStruct)Session["TempAccount"];
            }
            #endregion

            #endregion

            PageFilling();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #endregion

        private void PageFilling()
        {
            //Кнопка ВЕРНУТЬСЯ
            var lBtn = new LinkButton(); lBtn.CssClass = "buttonsHover lBtnsUniverse";
            lBtn.Text = " ..назад к списку всех учётных записей "; lBtn.PostBackUrl = "~/adm/users.aspx";
            addPanel.Controls.Add(lBtn);

            var usersForm = new UsersForm(this);
            addPanel.Controls.Add(usersForm.GetOneAccountEditPanel());
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