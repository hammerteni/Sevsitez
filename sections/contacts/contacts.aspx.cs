using System;
using site.classes;
using System.Web.UI;

namespace site.sections.contacts
{
    /// <summary>Страница Контакты</summary>
    public partial class contacts : System.Web.UI.Page
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
            if (p == "") form.UniverseFillPage(addPanel, "contacts", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);

            //Добавление Яндекс Карты
            addPanel.Controls.Add(new LiteralControl("<div style='text-align: center;'><span><div style='position: relative; display: inline-block;'>"));
            addPanel.Controls.Add(new LiteralControl("<script type='text/javascript' charset='utf-8' src='//api-maps.yandex.ru/services/constructor/1.0/js/?sid=dXQSrAFfpDce9T-LH8tcgtda6KxSnLD8&width=500&height=400&lang=ru_RU&sourceType=constructor'></script>"));
            addPanel.Controls.Add(new LiteralControl("</div></span></div>"));
            addPanel.Controls.Add(new LiteralControl("<br/><br/><br/>"));
        }

        #endregion
    }
}