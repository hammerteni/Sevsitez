using System;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitionmultimedia
{
    /// <summary>Страница отправления работы на Конкурс мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!»</summary>
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
            ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(Multimedia.self)];
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            #region Запись сессионных переменных во ViewState и обратно

            if (Session[EnumsHelper.GetSessionName(Multimedia.self)] == null)
            {
                if (ViewState["TempRequest"] != null)
                {
                    Session[EnumsHelper.GetSessionName(Multimedia.self)] = (CompetitionRequest)ViewState["TempRequest"];
                }
                else
                {
                    ViewState["TempRequest"] = new CompetitionRequest();
                    Session[EnumsHelper.GetSessionName(Multimedia.self)] = new CompetitionRequest();
                }
            }
            else
            {
                ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(Multimedia.self)];
            }

            #endregion

            Page.Title = "Регистрация на " + EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetMultimediaCode(Multimedia.self));

            PagesFormE form1 = new PagesFormE(this); p = p.Trim();
            if (p == "") form1.UniverseFillPage(addPanelText, "regmultimedia", PagesFormE.CacheContr.NoCache);
            else form1.UniverseFillPage(addPanelText, p, PagesFormE.CacheContr.NoCache);

            if (Request.QueryString["subname"] != null)
            {
                CompetitionsForm form = new CompetitionsForm(this);
                addPanel.Controls.Add(form.RequestPanel_Multimedia(Request.QueryString["subname"]));
            }
        }

        #endregion
    }
}