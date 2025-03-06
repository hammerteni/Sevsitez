using System;
using System.Reflection;
using System.Linq;
using System.Web.UI;
using site.classes;
using site.classesHelp;
using Secure;

namespace site
{
    public partial class Site1 : MasterPage
    {
        /*protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ActionValidator.Initialize(base.IsPostBack);
        }*/

        protected void Page_Load(object sender, EventArgs e)
        {


            #region Запись в журнале посещения сайта

            var loginWork = new LoginWorkClass(Page);
            loginWork.VizitSiteAccounting();

            #endregion

            #region ПРОВЕРКА САЙТА НА ВКЛЮЧЁННОСТЬ ИЛИ ВЫКЛЮЧЕННОСТЬ

            ConfigFile cfg = new ConfigFile();
            string checkValue = cfg.GetParam("siteenable");
            if (checkValue == "false")         //если сайт выключен, то загружаем предупредительную страницу
            {
                Response.Redirect("~/working.aspx");
            }

            #endregion

            //плавное появление любой страницы сайта при первом её запуске с кнопки меню навигации
            /*if (Session["opacityStart"] != null)
            {
                contentSubDiv.CssClass = "contentSubDiv contentSubDivAnim";
                Session["opacityStart"] = null;
            }*/

            #region Код заполнения телефона и e-mail в шапке сайта

            /*var pageWork = new PagesWorkClass(Page);
            var mailTelStruct = pageWork.GetTelMail();
            foreach (var tel in mailTelStruct.Telephone)
            {
                tdTel.Controls.Add(new LiteralControl("<span>" + tel + "</span><br/>"));
            }
            foreach (var mail in mailTelStruct.Email)
            {
                tdMail.Controls.Add(new LiteralControl("<a href='mailto: " + mail + "' class='hLink'>" + mail + "</a><br/>"));
            }*/

            #endregion

            #region Код заполнения даты для Копирайта внизу сайта

            copyRightAdd.Text = DateTime.Now.Year + " г.";

            #endregion
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            #region FAVICON

            head.Controls.Add(new LiteralControl("<link rel = 'shortcut icon' href = 'https://" + Request.ServerVariables["HTTP_HOST"] + "/favicon.ico' />"));
            head.Controls.Add(new LiteralControl("<link rel = 'icon' type = 'image/x-icon' href = 'https://" + Request.ServerVariables["HTTP_HOST"] + "/favicon.ico'  />"));

            #endregion
        }
    }
}