using System;
using System.Web;
using site.classes;

namespace site.sections.project
{
    /// <summary>Страница Оргкомитет (раздел - Образовательный проект "Воссоединение крыма с Россией")</summary>
    public partial class orgcomitet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/sections/contacts/");
            
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
            if (p == "") form.UniverseFillPage(addPanel, "projectOrgcomitet", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
        }

        #endregion
    }
}