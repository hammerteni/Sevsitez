using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
using site.classesHelp;
using System.Collections.Generic;

namespace site.adm
{
    /// <summary>страница показа и редактирования АРХИВА Конкурсов</summary>
    public partial class competitionsarch : Page
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
            ((LinkButton)Master.FindControl("lBtnCompetition_Arch")).ApplyStyle(s);

            #endregion

            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion

            #region ПРОВЕРКА НА АВТОРИЗАЦИЮ

            if (Session["authperson"] == null) Response.Redirect("~/adm/as.aspx");

            #endregion

            #region Запись сессионных переменных во ViewState и обратно

            #region Session["srchStrCompet_Arch"]

            if (Session["srchStrCompet"] == null)
            {
                if (ViewState["srchStrCompet"] != null)
                {
                    Session["srchStrCompet"] = (string)ViewState["srchStrCompet"];
                }
                else
                {
                    ViewState["srchStrCompet"] = "";
                    Session["srchStrCompet"] = "";
                }
            }
            else
            {
                ViewState["srchStrCompet"] = (string)Session["srchStrCompet"];
            }

            //if (Session["srchStrCompet_Arch"] == null)
            //{
            //    if (ViewState["srchStrCompet_Arch"] != null)
            //    {
            //        Session["srchStrCompet_Arch"] = (string)ViewState["srchStrCompet_Arch"];
            //    }
            //    else
            //    {
            //        ViewState["srchStrCompet_Arch"] = "";
            //        Session["srchStrCompet_Arch"] = "";
            //    }
            //}
            //else
            //{
            //    ViewState["srchStrCompet_Arch"] = (string)Session["srchStrCompet_Arch"];
            //}

            #endregion
            #region Session["pagenum_Arch"]

            if (Session["pagenum"] == null)
            {
                if (ViewState["pagenum"] != null)
                {
                    Session["pagenum"] = (int)ViewState["pagenum"];
                }
                else
                {
                    ViewState["pagenum"] = 1;
                    Session["pagenum"] = 1;
                }
            }
            else
            {
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }

            //if (Session["pagenum_Arch"] == null)
            //{
            //    if (ViewState["pagenum_Arch"] != null)
            //    {
            //        Session["pagenum_Arch"] = (int)ViewState["pagenum_Arch"];
            //    }
            //    else
            //    {
            //        ViewState["pagenum_Arch"] = 1;
            //        Session["pagenum_Arch"] = 1;
            //    }
            //}
            //else
            //{
            //    ViewState["pagenum_Arch"] = (int)Session["pagenum_Arch"];
            //}

            #endregion

            #endregion

            #region РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (EnumsHelper.GetWritesCodeFromValue(((AdmPersonStruct)Session["authperson"]).Writes) != "")
            {
                PageFilling(((AdmPersonStruct)Session["authperson"]).Writes);
            }

            #endregion
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                (FindControl("ctl00$ContentPlaceHolder1$txtBoxSrchEvent")).Focus();
                Form.DefaultButton = "ctl00$ContentPlaceHolder1$btnEventSrch";

                //ViewState["srchStrCompet_Arch"] = (string)Session["srchStrCompet_Arch"];
                //ViewState["pagenum_Arch"] = (int)Session["pagenum_Arch"];

                ViewState["srchStrCompet"] = (string)Session["srchStrCompet"];
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }
            catch { }
        }

        #endregion

        private void PageFilling(string rights)
        {
            PagesFormE.DisableCache(this);

            CompetitionsFormCons form = new CompetitionsFormCons(this);
            addPanel.Controls.Add(form.ConsoleCompetitionPanel(rights, true));
        }
    }
}