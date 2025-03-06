using System;
using site.classes;
using site.classesHelp;
using Secure;

namespace site.adm
{
    /// <summary>мастер страница консоли администрирования</summary>
    public partial class Adm : System.Web.UI.MasterPage
    {
        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ActionValidator.Initialize(base.IsPostBack);
        }

        #endregion
        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            #region РАЗГРАНИЧЕНИЕ ДОСТУПА

            if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.newsEditor))
            {
                lBtnToPagesE.Enabled = false; lBtnToPagesE.Visible = false;
                //lBtnSlideShow.Enabled = false; lBtnSlideShow.Visible = false;
                //lBtnSlideScroll.Enabled = false; lBtnSlideScroll.Visible = false;
                lBtnCompetition.Enabled = false; lBtnCompetition.Visible = false;
                lBtnCompetition_Arch.Enabled = false; lBtnCompetition_Arch.Visible = false;
                lBtnToConsEvents.Enabled = false; lBtnToConsEvents.Visible = false;
                lBtnToPortalEvents.Enabled = false; lBtnToPortalEvents.Visible = false;
                lBtnToErrEvents.Enabled = false; lBtnToErrEvents.Visible = false;
                lBtnToMail.Enabled = false; lBtnToMail.Visible = false;
                lBtnSiteEnable.Enabled = false; lBtnSiteEnable.Visible = false;
                lBtnToStatistic.Enabled = false; lBtnToStatistic.Visible = false;
                lBtnToUsers.Enabled = false; lBtnToUsers.Visible = false;
                lBtnToEduOrg.Enabled = false; lBtnToEduOrg.Visible = false;
                lBtnCompetitionAdmin.Enabled = false; lBtnCompetitionAdmin.Visible = false;
            }
            else if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.pagesEditor))
            {
                lBtnToNews.Enabled = false; lBtnToNews.Visible = false;
                lBtnCompetition.Enabled = false; lBtnCompetition.Visible = false;
                lBtnCompetition_Arch.Enabled = false; lBtnCompetition_Arch.Visible = false;
                lBtnToConsEvents.Enabled = false; lBtnToConsEvents.Visible = false;
                lBtnToPortalEvents.Enabled = false; lBtnToPortalEvents.Visible = false;
                lBtnToErrEvents.Enabled = false; lBtnToErrEvents.Visible = false;
                lBtnToMail.Enabled = false; lBtnToMail.Visible = false;
                lBtnSiteEnable.Enabled = false; lBtnSiteEnable.Visible = false;
                lBtnToStatistic.Enabled = false; lBtnToStatistic.Visible = false;
                lBtnToUsers.Enabled = false; lBtnToUsers.Visible = false;
                lBtnToEduOrg.Enabled = false; lBtnToEduOrg.Visible = false;
                lBtnCompetitionAdmin.Enabled = false; lBtnCompetitionAdmin.Visible = false;
            }
            else if(((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin))
            {
                lBtnCompetitionAdmin.Enabled = false; lBtnCompetitionAdmin.Visible = false;
            }
            else if (EnumsHelper.GetWritesCodeFromValue(((AdmPersonStruct)Session["authperson"]).Writes) != "") //условие для редакторов Конкурсов
            {
                lBtnToPagesE.Enabled = false; lBtnToPagesE.Visible = false;
                //lBtnSlideShow.Enabled = false; lBtnSlideShow.Visible = false;
                //lBtnSlideScroll.Enabled = false; lBtnSlideScroll.Visible = false;
                lBtnToNews.Enabled = false; lBtnToNews.Visible = false;
                lBtnToMail.Enabled = false; lBtnToMail.Visible = false;
                lBtnSiteEnable.Enabled = false; lBtnSiteEnable.Visible = false;
                lBtnToConsEvents.Enabled = false; lBtnToConsEvents.Visible = false;
                lBtnToPortalEvents.Enabled = false; lBtnToPortalEvents.Visible = false;
                lBtnToErrEvents.Enabled = false; lBtnToErrEvents.Visible = false;
                
                lBtnToStatistic.Enabled = false; lBtnToStatistic.Visible = false;
                lBtnToUsers.Enabled = false; lBtnToUsers.Visible = false;

                if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.editorTheatreVokal) || 
                    ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.editorTheatreInstrumZanr))
                {
                    lBtnToEduOrg.Enabled = true; lBtnToEduOrg.Visible = true;
                    lBtnCompetitionAdmin.Enabled = true; lBtnCompetitionAdmin.Visible = true;
                }
                else
                {
                    lBtnToEduOrg.Enabled = false; lBtnToEduOrg.Visible = false;
                    lBtnCompetitionAdmin.Enabled = false; lBtnCompetitionAdmin.Visible = false;
                }
                
            }

            #endregion

            //применение стиля для выпадания верхнего меню при старте сессии (только один раз)
            if (Session["menuAdmTopShow"] == null)
            {
                headerDiv.CssClass = "headerDiv headerDivAnim";
                Session["menuAdmTopShow"] = "ok";
            }
        }

        #endregion
        #region Page_PreRender

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                whoami.Text = ((AdmPersonStruct)Session["authperson"]).Name;
            }
            catch { }
        }

        #endregion

        /// <summary>кнопка перехода на страницу редактирования текстовых страниц с редактором CKEditor</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToPagesE_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/pageseditor.aspx");
        }   

        /*/// <summary>кнопка перехода на страницу редактирования слайд-шоу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSlideShow_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/slideshow.aspx");
        }*/

        //
        /*/// <summary>кнопка перехода на страницу редактирования слайд-шоу</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSlideScroll_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/slidescroll.aspx");
        }*/

        /// <summary>кнопка перехода на страницу новостей</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToNews_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/news.aspx");
        }
        
        /// <summary>кнопка перехода на страницу редактирования конкурсов</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCompetition_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/competitions.aspx");
        }

        protected void lBtnCompetitionAdmin_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/competitionsadmin.aspx");
        }
        

        /// <summary>кнопка перехода на страницу редактирования архивных конкурсов</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnCompetition_Arch_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/competitionsarch.aspx");
        }

        /// <summary>кнопка перехода на страницу настройки пользователей консоли</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToUsers_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/users.aspx");
        }

        /// <summary>кнопка перехода на страницу настройки пользователей консоли</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToMail_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/mail.aspx");
        }   
        
        /// <summary>кнопка перехода на страницу вкл/выкл сайта</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnSiteEnable_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/siteenable.aspx");
        }

        /// <summary>кнопка перехода на страницу просмотра событий консоли</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToConsEvents_OnClick(object sender, EventArgs e)
        {
            Session["srchStrEvent"] = null;
            Session["inverseSrch"] = null;
            Response.Redirect("~/adm/consevents.aspx");
        }

        /// <summary>кнопка перехода на страницу просмотра событий сайта</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToPortalEvents_OnClick(object sender, EventArgs e)
        {
            Session["srchStrEvent"] = null;
            Session["inverseSrch"] = null;
            Response.Redirect("~/adm/portalevents.aspx");
        }

        /// <summary>кнопка перехода на страницу просмотра ошибок при работе сайта или портала</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToErrEvents_OnClick(object sender, EventArgs e)
        {
            Session["pagenum"] = 1;
            Session["srchStrErr"] = null;
            Session["inverseSrchErr"] = null;
            Response.Redirect("~/adm/errorevents.aspx");
        }

        /// <summary>кнопка перехода на страницу статистики</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToStatistic_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/statevents.aspx");
        }

        /// <summary>кнопка перехода на страницу редактирования образовательных учреждений</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToEduOrg_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adm/educationalorg.aspx");
        }


        /// <summary>кнопка перехода на сайт</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnToPortal_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/default.aspx");
        }

        /// <summary>кнопка завершения сессии</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lBtnExit_OnClick(object sender, EventArgs e)
        {
            Session.Clear(); Session.Abandon();
            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthADel();

            #endregion
            Response.Redirect("~/adm/as.aspx");
        }

        
    }
}