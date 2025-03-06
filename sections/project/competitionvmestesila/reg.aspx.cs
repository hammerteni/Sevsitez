using System;
using site.classes;
using site.classesHelp;

namespace site.sections.project.competitionVmesteSila
{
    /// <summary>Страница отправления работы на фестиваля-конкурса "Вместе мы сильнее!"</summary>
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
            ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(VmesteSila.self)];
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            #region Запись сессионных переменных во ViewState и обратно

            if (Session[EnumsHelper.GetSessionName(VmesteSila.self)] == null)
            {
                if (ViewState["TempRequest"] != null)
                {
                    Session[EnumsHelper.GetSessionName(VmesteSila.self)] = (CompetitionRequest)ViewState["TempRequest"];
                }
                else
                {
                    ViewState["TempRequest"] = new CompetitionRequest();
                    Session[EnumsHelper.GetSessionName(VmesteSila.self)] = new CompetitionRequest();
                }
            }
            else
            {
                ViewState["TempRequest"] = (CompetitionRequest)Session[EnumsHelper.GetSessionName(VmesteSila.self)];
            }

            #endregion

            Page.Title = "Регистрация на " + EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetVmesteSilaCode(VmesteSila.self));

            PagesFormE form1 = new PagesFormE(this); p = p.Trim();
            if (p == "") form1.UniverseFillPage(addPanelText, "regVmesteSila", PagesFormE.CacheContr.NoCache);
            else form1.UniverseFillPage(addPanelText, p, PagesFormE.CacheContr.NoCache);

            if (Request.QueryString["subname"] != null)
            {
                CompetitionsForm form = new CompetitionsForm(this);
                addPanel.Controls.Add(form.RequestPanel_VmesteSila(Request.QueryString["subname"]));
            }
        }

        #endregion
    }
}