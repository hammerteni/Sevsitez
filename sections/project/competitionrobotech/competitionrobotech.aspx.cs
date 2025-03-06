using System;
using System.Collections.Generic;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitionrobotech
{
    /// <summary>Страница Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» (подраздел - Этапы образовательного проекта «Москва - Крым - Территория талантов» (Воссоединение Крыма с Россией))</summary>
    public partial class competitionrobotech : System.Web.UI.Page
    {
        #region Поля

        string subname;

        #endregion

        #region Методы страницы

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Запись сессионных переменных во ViewState и обратно

            #region Session["voiting"]

            if (Session["voiting"] == null)
            {
                if (ViewState["voiting"] != null)
                {
                    Session["voiting"] = (List<string>)ViewState["voiting"];
                }
                else
                {
                    Session["voiting"] = new List<string>();
                }
            }
            else
            {
                ViewState["voiting"] = (List<string>)Session["voiting"];
            }

            #endregion
            #region Session["voitingSum"]

            if (Session["voitingSum"] == null)
            {
                if (ViewState["voitingSum"] != null)
                {
                    Session["voitingSum"] = (List<string>)ViewState["voitingSum"];
                }
                else
                {
                    Session["voitingSum"] = new List<string>();
                }
            }
            else
            {
                ViewState["voitingSum"] = (List<string>)Session["voitingSum"];
            }

            #endregion
            #region Session["pagenum"]

            if (Session["pagenum"] == null)
            {
                if (ViewState["pagenum"] != null)
                {
                    Session["pagenum"] = (int)ViewState["pagenum"];
                }
            }
            else
            {
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }

            #endregion
            #region Session["pagenumResrobotech"]

            if (Session["pagenumResrobotech"] == null)
            {
                if (ViewState["pagenumResrobotech"] != null)
                {
                    Session["pagenumResrobotech"] = (int)ViewState["pagenumResrobotech"];
                }
                else
                {
                    Session["pagenumResrobotech"] = 1;
                    ViewState["pagenumResrobotech"] = 1;
                }
            }
            else
            {
                ViewState["pagenumResrobotech"] = (int)Session["pagenumResrobotech"];
            }

            #endregion

            #endregion

            if (Request.QueryString["subname"] != null)
            {
                subname = Request.QueryString["subname"];
            }
            else
            {
                subname = EnumsHelper.GetRobotechValue(Robotech.self);
            }

            AddVotingPanels();

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
            try
            {
                ViewState["voiting"] = (List<string>)Session["voiting"];
                ViewState["voitingSum"] = (List<string>)Session["voitingSum"];
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
                ViewState["pagenumResrobotech"] = (int)Session["pagenumResrobotech"];
            }
            catch { }
        }

        #endregion

        #region Процедуры наполнения страницы

        private void PageFill(string p = "")
        {
            if (subname == EnumsHelper.GetRobotechValue(Robotech.self))   //если это основная страница Конкурса, то..
            {
                PagesFormE form = new PagesFormE(this); p = p.Trim();
                if (p == "") form.UniverseFillPage(addPanel, "competitionRobotech", PagesFormE.CacheContr.NoCache);
                else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
            }
        }

        /// <summary>Метод добавления панелей с работами для голосования</summary>
        private void AddVotingPanels()
        {
            PagesForm.NoCache(this);

            CompetitionsForm form = new CompetitionsForm(this);
            votingPanels.Controls.Add(form.VotingPanel_Robotech(subname));
        }

        #endregion
    }
}