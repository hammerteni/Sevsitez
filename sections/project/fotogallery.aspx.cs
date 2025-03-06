using System;
using site.classes;

namespace site.sections.project
{
    /// <summary>Страница со списком ссылок на фотогалереи различных конкурсов</summary>
    public partial class fotogallery : System.Web.UI.Page
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
            if (p != "")
            {
                panelBtnBack.Visible = true;
                panelLinks.Visible = false;
                form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
            }
            else
            {
                panelBtnBack.Visible = false;
                panelLinks.Visible = true;
            }
        }

        #endregion
    }
}