using System;
using System.Web.UI.WebControls;
using site.classes;

namespace site.sections.sitemap
{
    /// <summary>Страница с картой сайта</summary>
    public partial class sitemap : System.Web.UI.Page
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
            if (p == "") form.UniverseFillPage(addPanel, "sitemap", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
        }

        #endregion

        protected virtual void Data_Bound(Object sender, TreeNodeEventArgs e)
        {
            if (e.Node.ToolTip.Contains("Подраздел"))
            {
                e.Node.Expanded = false;
            }
        }
    }
}