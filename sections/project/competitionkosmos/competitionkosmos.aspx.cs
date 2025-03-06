using System;
using System.Collections.Generic;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitionkosmos
{
    /// <summary>Страница Квест-игры «Покоряя космос» (подраздел - Этапы Образовательного
    /// проекта "Воссоединение Крыма с Россией")</summary>
    public partial class competitionkosmos : System.Web.UI.Page
    {
        string subname;

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

            #region Session["pagenumResKosmos"]

            if (Session["pagenumResKosmos"] == null)
            {
                if (ViewState["pagenumResKosmos"] != null)
                {
                    Session["pagenumResKosmos"] = (int)ViewState["pagenumResKosmos"];
                }
                else
                {
                    Session["pagenumResKosmos"] = 1;
                    ViewState["pagenumResKosmos"] = 1;
                }
            }
            else
            {
                ViewState["pagenumResKosmos"] = (int)Session["pagenumResKosmos"];
            }

            #endregion

            #endregion

            if (Request.QueryString["subname"] != null)
            {
                subname = Request.QueryString["subname"];
            }
            else
            {
                subname = EnumsHelper.GetKosmosValue(Kosmos.self);
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
                ViewState["voitingSum"] = (List<string>)Session["voiting"];
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
                ViewState["pagenumResKosmos"] = (int)Session["pagenumResKosmos"];
            }
            catch { }
        }

        #region Процедуры наполнения страницы

        private void PageFill(string p = "")
        {
            if (subname == EnumsHelper.GetKosmosValue(Kosmos.self))   //если это основная страница Конкурса, то..
            {
                PagesFormE form = new PagesFormE(this); p = p.Trim();
                if (p == "") form.UniverseFillPage(addPanel, "competitionKosmos", PagesFormE.CacheContr.NoCache);
                else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
            }
        }

        /// <summary>Метод добавления панелей с работами для голосования</summary>
        private void AddVotingPanels()
        {
            PagesForm.NoCache(this);

            CompetitionsForm form = new CompetitionsForm(this);
            votingPanels.Controls.Add(form.VotingPanel_Kosmos(subname));
        }

        #endregion
    }
}