using System;
using site.classes;

namespace site.sections.project
{
    /// <summary>Страница Карта проекта (раздел - Образовательный проект "Воссоединение крыма с Россией")</summary>
    public partial class map : System.Web.UI.Page
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

            AddYandexMap();
            AddProjParticipantsTblMap();
        }

        #region Процедура наполнения страницы

        private void PageFill(string p = "")
        {
            PagesFormE form = new PagesFormE(this); p = p.Trim();
            if (p == "") form.UniverseFillPage(addPanel, "projectMap", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);
        }

        /// <summary>Метод добавляет на страницу JavaScript-код для отрисовки Географии проекта на Яндекс Карте</summary>
        private void AddYandexMap()
        {
            CompetitionsForm form = new CompetitionsForm(this);
            form.AddGeoYandexMap();
        }

        /// <summary>Метод добавляет карту участников образовательного проекта в табличной форме</summary>
        private void AddProjParticipantsTblMap()
        {
            CompetitionsForm form = new CompetitionsForm(this);
            addParticipantsMap.Controls.Add(form.ParticipantsMapPanel());
        }


        #endregion
    }
}