using System;
using site.classes;

namespace site
{
    /// <summary>код Главной страницы</summary>
    public partial class onenews : System.Web.UI.Page
    {
        #region События страницы

        protected void Page_Init(object sender, EventArgs e)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                PageFill(Request.QueryString["id"]);
            }
            else
            {
                Response.Redirect("~/default.aspx");
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Процедура наполнения страницы

        private void PageFill(string id = "")
        {
            PagesFormE.NoCache(this);

            NewsForm form = new NewsForm(this,0);
            addPanel.Controls.Add(form.GetOneNewsPanel(id));
        }

        #endregion
    }
}