using System;
using System.Collections.Generic;
using site.classes;

namespace site
{
    /// <summary>код Главной страницы</summary>
    public partial class _default : System.Web.UI.Page
    {
        #region События страницы

        protected override void OnInit(EventArgs e)
        {
            
        }
         
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Запись сессионных переменных во ViewState и обратно

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

            #endregion

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
                ViewState["pagenum"] = (Session["pagenum"] != null ? (int)Session["pagenum"] : 1);
            }
            catch { }

            
        }

        #endregion

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            PagesFormE form = new PagesFormE(this); p = p.Trim();
            if (p == "") form.UniverseFillPage(addPanel, "main", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);

            NewsForm form1 = new NewsForm(this, 0);
            addPanel.Controls.Add(form1.GetNewsPanel());

            SlideScrollForm sliderScroll = new SlideScrollForm(this);
            addAnimFoto.Controls.Add(sliderScroll.GetSlideScroll("1"));
        }

        #endregion
    }
}