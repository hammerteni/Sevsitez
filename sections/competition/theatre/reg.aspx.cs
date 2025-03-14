﻿using System;
using site.classes;

namespace site.sections.project.competitiontheatre
{
    /// <summary>Страница Конкурс творческого мастерства «Мастер сцены» (подраздел - Этапы Образовательного
    /// проекта "Воссоединение Крыма с Россией")</summary>
    public partial class reg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PagesForm.DisableCache(this);

            if (Request.QueryString["p"] != null)
            {
                PageFill(Request.QueryString["p"]);
            }
            else
            {
                PageFill();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["TempRequest"] = (CompetitionRequest)Session["TempTheatreRequest"];
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            #region Запись сессионных переменных во ViewState и обратно

            if (Session["TempTheatreRequest"] == null)
            {
                if (ViewState["TempRequest"] != null)
                {
                    Session["TempTheatreRequest"] = (CompetitionRequest)ViewState["TempRequest"];
                }
                else
                {
                    ViewState["TempRequest"] = new CompetitionRequest();
                    Session["TempTheatreRequest"] = new CompetitionRequest();
                }
            }
            else
            {
                ViewState["TempRequest"] = (CompetitionRequest)Session["TempTheatreRequest"];
            }

            #endregion

            PagesFormE form1 = new PagesFormE(this); p = p.Trim();
            if (p == "") form1.UniverseFillPage(addPanelText, "regtheatre", PagesFormE.CacheContr.NoCache);
            else form1.UniverseFillPage(addPanelText, p, PagesFormE.CacheContr.NoCache);

            CompetitionsForm form = new CompetitionsForm(this);
            addPanel.Controls.Add(form.RequestPanel("theatre"));
        }

        #endregion
    }
}