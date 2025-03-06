using System;
using site.classes;
using site.classesHelp;

namespace site.sections.project
{
    /// <summary>Страница Этапы (раздел - Образовательный проект "Воссоединение крыма с Россией")</summary>
    public partial class stages : System.Web.UI.Page
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
            if (p == "") form.UniverseFillPage(addPanel, "projectStages", PagesFormE.CacheContr.NoCache);
            else form.UniverseFillPage(addPanel, p, PagesFormE.CacheContr.NoCache);

            #region Наполнение кнопок-ссылками на Конкурсы атрибутами

            lBtn_Sport1.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportChess));
            lBtn_Sport1.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportChess");

            lBtn_Sport2.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportCheckers));
            lBtn_Sport2.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportCheckers");

            lBtn_Sport3.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportEdinoborstva));
            lBtn_Sport3.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportEdinoborstva");

            lBtn_Sport4.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportBoks));
            lBtn_Sport4.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportBoks");

            lBtn_Sport5.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportLazertag));
            lBtn_Sport5.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportLazertag");

            lBtn_Sport6.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportFootball));
            lBtn_Sport6.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportFootball");

            //lBtn_Sport7.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportBasketball));
            //lBtn_Sport7.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportBasketball");

            //lBtn_Sport8.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.sportVolleyball));
            //lBtn_Sport8.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx?comp=sportVolleyball");


            lBtn_Theatre1.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.theatreVokal));
            lBtn_Theatre1.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx?comp=theatreVokal");

            lBtn_Theatre2.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.theatreHudSlovo));
            lBtn_Theatre2.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx?comp=theatreHudSlovo");

            lBtn_Theatre3.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.theatreInsrum));
            lBtn_Theatre3.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx?comp=theatreInsrum");

            lBtn_Theatre4.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.theatreHoreo));
            lBtn_Theatre4.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx?comp=theatreHoreo");

            lBtn_Theatre5.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.theatreIskustvo));
            lBtn_Theatre5.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx?comp=theatreIskustvo");



            lBtn_Korablik1.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.korablikHudSlovo));
            lBtn_Korablik1.Attributes.Add("href", "../../sections/project/competitionkorablik/competitionkorablik.aspx?comp=korablikHudSlovo");

            lBtn_Korablik2.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.korablikHoreo));
            lBtn_Korablik2.Attributes.Add("href", "../../sections/project/competitionkorablik/competitionkorablik.aspx?comp=korablikHoreo");

            lBtn_Korablik3.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.korablikVokal));
            lBtn_Korablik3.Attributes.Add("href", "../../sections/project/competitionkorablik/competitionkorablik.aspx?comp=korablikVokal");



            lBtn_Moda1.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.modaLoskutokKuturie));
            lBtn_Moda1.Attributes.Add("href", "../../sections/project/competitionclothes/competitionclothes.aspx?comp=modaLoskutokKuturie");

            lBtn_Moda2.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.modaTeatrModi));
            lBtn_Moda2.Attributes.Add("href", "../../sections/project/competitionclothes/competitionclothes.aspx?comp=modaTeatrModi");

            lBtn_Moda3.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.modaModelierSedobnaya));
            lBtn_Moda3.Attributes.Add("href", "../../sections/project/competitionclothes/competitionclothes.aspx?comp=modaModelierSedobnaya");


            lBtn_Photo1.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.vernisazFotoGrahipcs));
            lBtn_Photo1.Attributes.Add("href", "../../sections/project/competitionfoto/competitionfoto.aspx?comp=vernisazFotoGrahipcs");

            lBtn_Photo2.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.vernisazIzo));
            lBtn_Photo2.Attributes.Add("href", "../../sections/project/competitionfoto/competitionfoto.aspx?comp=vernisazIzo");

            lBtn_Photo3.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.vernisazDpi));
            lBtn_Photo3.Attributes.Add("href", "../../sections/project/competitionfoto/competitionfoto.aspx?comp=vernisazDpi");


            lBtn_Kultura.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.openWorld));
            lBtn_Kultura.Attributes.Add("href", "../../sections/project/competitionkultura/competitionkultura.aspx?comp=openWorld");

            lBtn_Quest.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.kvestHraniteliIstorii));
            lBtn_Quest.Attributes.Add("href", "http://quest.sevastopolets-moskva.ru");
           


            lBtn_Mathbattle.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.mathBattle));
            lBtn_Mathbattle.Attributes.Add("href", "../../sections/project/competitionmathbattle/competitionmathbattle.aspx?comp=mathBattle");



            lBtn_VmesteSila.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.vmesteSila));
            lBtn_VmesteSila.Attributes.Add("href", "../../sections/project/competitionvmestesila/competitionvmestesila.aspx?comp=vmesteSila");

            lBtn_Robotech.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.shagVBudushee));
            lBtn_Robotech.Attributes.Add("href", "../../sections/project/competitionrobotech/competitionrobotech.aspx?comp=shagVBudushee");

            lBtn_Multimedia.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCompetitionsNewCode(CompetitionsNew.multimedia));
            lBtn_Multimedia.Attributes.Add("href", "../../sections/project/competitionmultimedia/competitionmultimedia.aspx?comp=multimedia");


            //lBtn_Photo.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetPhotoCode(Photo.self));
            //lBtn_Photo.Attributes.Add("href", "../../sections/project/competitionfoto/competitionfoto.aspx");

            //lBtn_Literary.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetLiteraryCode(Literary.self));
            //lBtn_Literary.Attributes.Add("href", "../../sections/project/competitionliterary/competitionliterary.aspx");

            //lBtn_Theatre.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetTheatreCode(Theatre.self));
            //lBtn_Theatre.Attributes.Add("href", "../../sections/project/competitiontheatre/competitiontheatre.aspx");

            //lBtn_Sport.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetSportCode(Sport.self));
            //lBtn_Sport.Attributes.Add("href", "../../sections/project/competitionsport/competitionsport.aspx");

   
            //lBtn_Robotech.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetRobotechCode(Robotech.self));
            //lBtn_Robotech.Attributes.Add("href", "../../sections/project/competitionrobotech/competitionrobotech.aspx");

            //lBtn_Kultura.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetKulturaCode(Kultura.self));
            //lBtn_Kultura.Attributes.Add("href", "../../sections/project/competitionkultura/competitionkultura.aspx");

            //lBtn_Multimedia.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetMultimediaCode(Multimedia.self));
            //lBtn_Multimedia.Attributes.Add("href", "../../sections/project/competitionmultimedia/competitionmultimedia.aspx");

            //lBtn_Toponim.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetToponimCode(Toponim.self));
            //lBtn_Toponim.Attributes.Add("href", "../../sections/project/competitiontoponim/competitiontoponim.aspx");

            //lBtn_Math.Text = "Олимпиада «Объединенная математика»";
            //lBtn_Math.Attributes.Add("href", "http://ommsk.sevastopolets-moskva.ru/");

            //lBtn_Clothes.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetClothesCode(Clothes.self));
            //lBtn_Clothes.Attributes.Add("href", "../../sections/project/competitionclothes/competitionclothes.aspx");

            //lBtn_VmesteSila.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetVmesteSilaCode(VmesteSila.self));
            //lBtn_VmesteSila.Attributes.Add("href", "../../sections/project/competitionvmestesila/competitionvmestesila.aspx");

            //lBtn_Korablik.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetKorablikCode(Korablik.self));
            //lBtn_Korablik.Attributes.Add("href", "../../sections/project/competitionkorablik/competitionkorablik.aspx");

            //lBtn_Crimroute.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetCrimrouteCode(Crimroute.self));
            //lBtn_Crimroute.Attributes.Add("href", "../../sections/project/competitioncrimroute/competitioncrimroute.aspx");

            //lBtn_Mathbattle.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetMathbattleCode(Mathbattle.self));
            //lBtn_Mathbattle.Attributes.Add("href", "../../sections/project/competitionmathbattle/competitionmathbattle.aspx");

            //lBtn_Kosmos.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetKosmosCode(Kosmos.self));
            //lBtn_Kosmos.Attributes.Add("href", "../../sections/project/competitionkosmos/competitionkosmos.aspx");

            //lBtn_Science.Text = EnumsHelper.GetCompetitionValueFromCode(EnumsHelper.GetScienceCode(Science.self));
            //lBtn_Science.Attributes.Add("href", "../../sections/project/competitionscience/competitionscience.aspx");

            #endregion
        }

        #endregion
    }
}