using System;
using site.classes;
using site.classesHelp;

namespace site
{
    /// <summary>класс отображения страницы проведения работ на сайте(когда сайт выключен)</summary>
    public partial class working : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigFile cfg = new ConfigFile();
            string checkValue = cfg.GetParam("siteenable");
            if (checkValue == "true")               //если сайт включён, то уходим из этой страницы
            {
                Response.Redirect("/default.aspx");
            }
            else if (checkValue == "false")         //если сайт выключен, то загружаем эту страницу
            {
                PagesFormE form = new PagesFormE(this);
                form.UniverseFillPage(addPanel, "working", PagesFormE.CacheContr.NoCache);
            }
        }
    }
}