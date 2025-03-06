using System;
using site.classes;

namespace site.sections.about
{
    /// <summary>Страница О нас</summary>
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

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            PagesFormE form = new PagesFormE(this); p = p.Trim();
            if (p == "") form.UniverseFillPage(addPanel, "about", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
        }

        #endregion
    }
}