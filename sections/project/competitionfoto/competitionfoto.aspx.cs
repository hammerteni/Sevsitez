using System;
using System.Collections.Generic;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitionfoto
{
    /// <summary>Страница Фотоконкурс «Россия-Крым-Севастополь» (подраздел - Этапы Образовательного
    /// проекта "Воссоединение Крыма с Россией")</summary>
    public partial class competitionfoto : System.Web.UI.Page
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

            #region Session["pagenumResfoto"]

            if (Session["pagenumResfoto"] == null)
            {
                if (ViewState["pagenumResfoto"] != null)
                {
                    Session["pagenumResfoto"] = (int)ViewState["pagenumResfoto"];
                }
                else
                {
                    Session["pagenumResfoto"] = 1;
                    ViewState["pagenumResfoto"] = 1;
                }
            }
            else
            {
                ViewState["pagenumResfoto"] = (int)Session["pagenumResfoto"];
            }

            #endregion

            #endregion

            if (Request.QueryString["subname"] != null)
            {
                subname = Request.QueryString["subname"];
            }
            else
            {
                subname = EnumsHelper.GetPhotoValue(Photo.self);
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
                ViewState["pagenumResfoto"] = (int)Session["pagenumResfoto"];
            }
            catch { }
        }

        #region Процедуры наполнения страницы

        private void PageFill(string p = "")
        {
            if (subname == EnumsHelper.GetPhotoValue(Photo.self))   //если это основная страница Конкурса, то..
            {
                PagesFormE form = new PagesFormE(this); p = p.Trim();
                if (p == "") form.UniverseFillPage(addPanel, "competitionFoto", PagesFormE.CacheContr.NoCache);
                else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
            }
        }

        /// <summary>Метод добавления панелей с работами для голосования</summary>
        private void AddVotingPanels()
        {
            PagesForm.NoCache(this);

            CompetitionsForm form = new CompetitionsForm(this);
            votingPanels.Controls.Add(form.VotingPanel_Photo(subname));
        }

        #endregion
    }
}