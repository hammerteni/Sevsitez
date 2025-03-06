using System;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitiontoponim
{
    /// <summary>Страница отправления работы на конкурс «Черноморский флот Великой Отечественной войны в топонимике городов России»</summary>
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
            ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(Toponim.self)];
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            #region Запись сессионных переменных во ViewState и обратно

            if (Session[EnumsHelper.GetSessionName(Toponim.self)] == null)
            {
                if (ViewState["TempRequest"] != null)
                {
                    Session[EnumsHelper.GetSessionName(Toponim.self)] = (CompetitionRequest)ViewState["TempRequest"];
                }
                else
                {
                    ViewState["TempRequest"] = new CompetitionRequest();
                    Session[EnumsHelper.GetSessionName(Toponim.self)] = new CompetitionRequest();
                }
            }
            else
            {
                ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(Toponim.self)];
            }

            #endregion

            Page.Title = "Регистрация на " + EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetToponimCode(Toponim.self));

            PagesFormE form1 = new PagesFormE(this); p = p.Trim();
            if (p == "") form1.UniverseFillPage(addPanelText, "regtoponim", PagesFormE.CacheContr.NoCache);
            else form1.UniverseFillPage(addPanelText, p, PagesFormE.CacheContr.NoCache);

            if (Request.QueryString["subname"] != null)
            {
                CompetitionsForm form = new CompetitionsForm(this);
                addPanel.Controls.Add(form.RequestPanel_Toponim(Request.QueryString["subname"]));
            }
        }

        #endregion
    }
}