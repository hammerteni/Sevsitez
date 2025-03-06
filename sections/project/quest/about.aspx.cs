﻿using System;
using site.classes;

namespace site.sections.project.quest
{
    /// <summary>Страница Квест-игра (подраздел - Этапы Образовательного
    /// проекта "Воссоединение Крыма с Россией")</summary>
    public partial class about : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["p"] != null)
            {
                PageFill(Request.QueryString["p"]);
            }
            else
            {
                PageFill();
            }
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            PagesFormE form = new PagesFormE(this); p = p.Trim();
            if (p == "") form.UniverseFillPage(addPanel, "questAbout", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
        }

        #endregion
    }
}