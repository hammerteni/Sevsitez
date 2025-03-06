using System;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;

namespace site.adm
{
    /// <summary>страница редактора учётных записей пользователей консоли</summary>
    public partial class portalevents : System.Web.UI.Page
    {
        private const int CountOfElemInOnePage = 50;

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
            ((LinkButton)Master.FindControl("lBtnToPortalEvents")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            //инициализация переменной для сортировки товара и переменной номера страницы, в данном случае без сортировки
            if (Session["pagenum"] == null) { Session["pagenum"] = 1; }
            if (Session["srchStrEvent"] == null) { Session["srchStrEvent"] = ""; }
            if (Session["inverseSrch"] == null) { Session["inverseSrch"] = false; }

            //РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                PageFilling();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                (FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Focus();
                Form.DefaultButton = "ctl00$ContentPlaceHolder1$btnEventSrch";
            }
            catch { }
        }

        #endregion

        private void PageFilling()
        {
            var optionForm = new OptionsForm(this);
            addPanel.Controls.Add(optionForm.GetSiteEventFilterPanel());
            addPanel.Controls.Add(optionForm.GetSiteEventListTbl(CountOfElemInOnePage));
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