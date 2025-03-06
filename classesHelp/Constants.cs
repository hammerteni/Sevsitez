
using System;
using System.Security.Cryptography;
using System.Security.Policy;
/// <summary>Файл содержит консанты, перечисления и вспомогательные классы для перечислений</summary>
namespace site.classesHelp
{
    #region Класс Constants

    /// <summary>Класс с общеиспользуемыми константами</summary>
    public class Constants
    {
        #region PATH_TO_MAIN

        /// <summary>Путь к основной папке сайта</summary>
        public static readonly string PATH_TO_MAIN = System.Web.Hosting.HostingEnvironment.MapPath("~");

        #endregion
        #region PATH_TO_MAINFOLDER

        /// <summary>Путь к основной папке со всеми данными сайта (..\files)</summary>
        public static readonly string PATH_TO_MAINFOLDER = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files";

        #endregion
        #region PATH_TO_APPDATA

        ///// <summary>Путь к папке App_Data</summary>
        //public static readonly string PATH_TO_APPDATA = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"App_Data";

        #endregion
        #region ConfParamResultDocsOnOff

        /// <summary>Имя параметра вкл./выключения таблиц с кнопками скачивания документов. Сам параметр может быть 0 или 1.</summary>
        public const string ConfParamResultDocsOnOff = "ResultDocsOnOff";

        #endregion
    }

    #endregion

    #region Перечисление SqlLogic

    /// <summary>Перечисление определяет имена логических операторов SQL</summary>
    public enum SqlLogic { AND, OR }

    #endregion
    #region Перечисление Writes

    /// <summary>Перечисление прав доступа в консоль управления</summary>
    public enum Writes
    {
        admin, 
        newsEditor, 
        pagesEditor,
        editorPhoto,
        editorPhotoIzo,
        editorPhotoCompGraphic,
        editorDPI1,
        editorDPI2,
        editorLiterary,
        editorTheatre,                      //театральное искусство
        editorTheatreHudSlovo,
        editorTheatreHoreo,
        editorTheatreVokal,
        editorTheatreInstrumZanr,
        editorTheatreModa,
        editorKultura,
        editorSport,                        //простейшие единоборства
        editorThekvo,
        editorBoks,
        editorKungfu,
        editorFootball,
        editorBasketball,
        editorVolleyball,
        editorPaintball,
        editorShahmaty,
        editorShashky,
        editorToponim,
        editorRobotech,
        editorVmesteSila,
        editorVmesteSilaMakeUp,
        editorVmesteSilaShair,
        editorClothes,
        editorMultimedia,
        editorKorablik,
        editorKorablikVokal,
        editorKorablikHoreo,
        editorCrimroute,
        editorMathbattle,
        editorKosmos,
        editorScience,
        none
    }

    #endregion
     
    #region Перечисление DocumentType

    /// <summary>Перечисление типов документов, выдаваемых участникам конкурсов по результатам отбора</summary>
    public enum DocumentType
    {
        Diplom = 0, 
        Certificate = 1, 
        Blagodarnost = 2
    }

    #endregion

    #region Перечисление Новые конкурсы 2023

    /// <summary>Перечисление Новые конкурсы 2023</summary>
    public enum CompetitionsNew
    {
        sportChess,
        sportCheckers,
        sportEdinoborstva,
        sportBoks,
        sportLazertag,
        sportFootball,
        sportBasketball,
        sportVolleyball,
        theatreVokal,
        theatreHudSlovo,
        theatreInsrum,
        theatreHoreo,
        korablikHudSlovo,
        korablikHoreo,
        korablikVokal,
        modaLoskutokKuturie,
        modaTeatrModi,

        vernisazFotoGrahipcs,
        vernisazIzo,
        vernisazDpi,
        openWorld,
        kvestHraniteliIstorii,
        theatreIskustvo,
        mathBattle,
        modaModelierSedobnaya,
        vmesteSila,
        shagVBudushee,
        multimedia,
    }

    #endregion

    #region Перечисление Photo

    /// <summary>Перечисление номинаций конкурса "Крымский вернисаж" (self - условное название самого конкурса)</summary>
    public enum Photo
    {
        photo,
        izo,
        computerGraphic,                // номинация УДАЛЕНА и разделена на 'Компьютерный рисунок' и 'Коллаж, фотомонтаж' в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве.
        computer_risunok,               // номинация добавлена в 4-м Проекте (2018 год, осень)
        collazh_fotomontazh,            // номинация добавлена в 4-м Проекте (2018 год, осень)
        DPT1,
        DPT1_makrame,                   // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT1_gobelen,                   // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT1_hud_vishivka,              // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT1_hud_vyazanie,
        DPT1_bumagoplastika,            // номинация добавлена в 4-м Проекте (2018 год, осень)
        DPT1_loskut_shitie,             // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT1_avtor_igrushka,
        DPT1_voilokovalyanie,           // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT1_biseropletenie,
        DPT1_fitodisign,                // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT2,
        DPT2_keramika,
        DPT2_hud_obr_stekla,            // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT2_hud_obr_kozhi,             // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT2_narod_igrush_isglini,      // номинация УДАЛЕНА в 4-м Проекте (2018 год, осень). Оставлена, потому что заявки есть в архиве
        DPT2_batik,
        DPT2_dekupazh,
        DPT2_rospis_poderevu,
        DPT1_combitehnics,
        DPT1_plastilonografiya,
        NO,
        self
    }

    #endregion
    #region Перечисление Literary

    /// <summary>Перечисление номинаций литературного конкурса "Боевая слава Севастополя" (self - условное название самого конкурса)</summary>
    public enum Literary
    {
        stihi,
        esse,
        rasskaz,
        sochinenie,
        NO,
        self
    }

    #endregion
    #region Перечисление Theatre

    /// <summary>Перечисление номинаций конкурса "Мастер сцены" (self - условное название самого конкурса)</summary>
    public enum Theatre
    {
        vokalAkademVokal,
        vokalFolklor,
        vokalEstradVokal,
        insrumZanrFortepiano,
        insrumZanrSintezator,
        insrumZanrStrunnoSmichkovieInstrumenti,
        insrumZanrDuhovieUdarnInstrum,
        insrumZanrNarodnieInstrum,
        insrumZanrGitara,
        insrumZanrAnsambli,
        insrumZanrOrkestri,
        teatrIskusSpekt,
        teatrIskusLitMuzKom,
        teatrIskusMultiGanr,
        xoreoClassichTanets,
        xoreoNarodTanets,
        xoreoEstradTanets,
        xoreoBalniyTanets,
        xoreoStilNarodTanets,
        xoreoSovremTanets,
        xoreoCircIskus,
        xoreoKadeti,
        xoreoCherliding,
        hudSlovo,
        NO,
        self
    }

    #endregion
    #region Перечисление Sport

    /// <summary>Перечисление номинаций спортивного турнира "В единстве наша сила" (self - условное название самого конкурса)</summary>
    public enum Sport
    {
        prostEdinoborstva,
        thekvo,
        boks,
        kungfu,
        stendStrelba,   //пейнтболл //Тактический лазертаг
        football,
        shahmaty,
        shashki,
        basketball,
        volleyball,
        NO,
        self
    }

    #endregion
    #region Перечисление Kultura

    /// <summary>Перечисление номинаций конкурса творческих и исследовательских работ «Открытый мир» (self - условное название самого конкурса)</summary>
    public enum Kultura
    {
        vossoedCrimaSRossiei,
        k75letiyu,              // последнее название "Оборона Севастополя в ВОВ", до - этого было "К 75-летию начала обороны Севастополя в ВОВ", "К 76-летию начала обороны Севастополя в ВОВ"
        zashitaobsihgranits,
        kultrnNasledieCrima,
        krimskyMostVSZ,
        iSeeCrimea,
        presentaionRu,
        publishInstagram,
       

        iSeeCrimeaEn,
        presentaionEn,
        audioGaid,
        publishVkontakte,
        intellektualKviz,
        
        NO,
        self
    }

    #endregion
    #region Перечисление Toponim

    /// <summary>Перечисление номинаций конкурса «Черноморский флот Великой Отечественной войны в топонимике городов России» (self - условное название самого конкурса)</summary>
    public enum Toponim
    {
        toponimika,
        NO,
        self
    }

    #endregion
    #region Перечисление Robotech

    /// <summary>Перечисление номинаций для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» (self - условное название самого конкурса)</summary>
    public enum Robotech
    {
        robototechnika,
        robototechnikaproject,
        robototechnika3dmodel,
        tinkercad,
        programmproject,
        robotech,
        NO,
        self
    }

    #endregion
    #region Перечисление VmesteSila
    /// <summary>Перечисление номинаций конкурса «Вместе мы сильнее» (self - условное название самого конкурса)</summary>
    public enum VmesteSila
    {
        hudSlovo,
        hudSlovoPoeziya,
        hudSlovoProza,
        horeographia,
        horeographiaBalniyTanets,
        horeographiaClassichTanets,
        horeographiaEstradTanets,
        horeographiaNarodTanets,
        horeographiaSovremenTanets,
        horeographiaOstalnieGanri,

        vokal,
        vokalAkademVokal,
        vokalEstradVokal,
        vokalFolklor,
        vokalZest,
        vokalOstalnieGanri,

        instrumental,
        insrumZanrFortepiano,
        insrumZanrSintezator,
        insrumZanrStrunnoSmichkovieInstrumenti,
        insrumZanrDuhovieUdarnInstrum,
        insrumZanrNarodnieInstrum,
        insrumZanrGitara,
        insrumZanrAnsambli,
        insrumZanrOstalnieGanri,

        theatre,
        theatreSpektakl,
        theatreScenka,
        theatreLiteraturnoMusikalnaya,
        theatreDrama,

        masterMakeup,
        masterMakeupDay,
        masterMakeupNight,
        masterMakeupStsena,
        masterMakeupFantasy,

        masterShair,
        masterShairPletenie,
        masterShairDay,
        masterShairNight,
        masterShairFantasy,

        NO,
        self
    }
    #endregion
    #region Перечисление Clothes
    /// <summary>Перечисление номинаций конкурса «Индустрия моды» (self - условное название самого конкурса)</summary>
    public enum Clothes
    {
        uniyKuturie,
        uniyKuturieTkan,
        uniyKuturieNetradicMaterial,
        uniyKuturieFashion,
        uniyKuturieTechRisunok,
        uniyKuturieFoodArt,
        uniyKuturieOgorod,
        uniyKuturieBeauty,

        chudoLoskutki,
        chudoLoskutkiIgrushkiKukliTvorRisunok,
        chudoLoskutkiIgrushkiKukli,

        eskiziModelier,
        eskiziModelierTvorRisunok,
        eskiziModelierFashion,
        eskiziModelierTechRisunok,

        sedobnayaModa,
        sedobnayaModaFoodArt,
        sedobnayaModaOgorod,
        sedobnayaModaBeauty,

        tmcollectpokaz,
        tmavtorcollect,
        tmindividpokaz,
        tmnetradicmaterial,
        tmissledovproject,

        NO,
        self
    }
    #endregion
    #region Перечисление Multimedia

    /// <summary>Перечисление номинаций Конкурса мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!» (self - условное название самого конкурса)</summary>
    public enum Multimedia
    {
        uniygeroi,
        uniyzashitnik,
        characteristica,
        prichinietapi,
        rolvistorii,

        podandreevskimflagom,
        pomoryampovolnam,
        korablimorykimore,
        chernomorskomurossiiposvyashaetsa,
        kraskimorya,
        krossvord,
        metodicheskierazrabotki,
        
        morepobedkolibelsmelchakov,
        netzapyatihtolkotochki,
        vashipodvigineumrut,
        sevastopol44,
        nadevaemmitelnyashku,
        klyatvudaemsevastopolvernem,
        hranitalbomsemeinipamyat,
        ihpamaytuzivushiipoklonis,
        multimediinieizdaniya,

        yarisuupobedy,
        spesneirpobede,
        geroyamotserdca,
        plechomkplechu,
        pamyatsilneevremeni,

        NO,
        self
    }

    #endregion
    #region Перечисление Korablik
    /// <summary>Перечисление номинаций конкурса «Кораблик детства» (self - условное название самого конкурса)</summary>
    public enum Korablik
    {
        hudSlovo,

        horeographia,
        horeographiaNarodTanets,
        horeographiaEstradTanets,
        horeographiaClassichTanets,
        horeographiaBalniyTanets,
       
        vokal,
        vokalSolo,
        vokalMalieFormi,
        vokalAnsambli,

        NO,
        self
    }
    #endregion
    #region Перечисление Crimroute
    /// <summary>Перечисление номинаций конкурса проектных работ «Крымские маршруты» (self - условное название самого конкурса)</summary>
    public enum Crimroute
    {
        historyplace,
        militaryhistoryplace,
        literaturehistoryplace,
        NO,
        self
    }
    #endregion
    #region Перечисление Mathbattle

    /// <summary>Перечисление номинаций конкурса «Математический батл» (self - условное название самого конкурса)</summary>
    public enum Mathbattle
    {
        battle,
        NO,
        self
    }
    #endregion
    #region Перечисление Kosmos

    /// <summary>Перечисление номинаций конкурса «Покоряя космос» (self - условное название самого конкурса)</summary>
    public enum Kosmos
    {
        kosmos,
        NO,
        self
    }
    #endregion
    #region Перечисление Science

    /// <summary>Перечисление номинаций Конкурса научных работ "В моей лаборатории вот что... " (self - условное название самого конкурса)</summary>
    public enum Science
    {
        ekologia_ochno,
        ekologia_zaochno,
        himiya_ochno,
        himiya_zaochno,
        fizika_ochno,
        fizika_zaochno,
        biologiya_ochno,
        biologiya_zaochno,
        NO,
        self
    }

    #endregion

    #region Перечисление LocalizationType

    /// <summary>Перечисление определяет типы Локализаций, в зависимости от типа будут использоваться текстовые ресурсные xml-файлы</summary>
    public enum LocalizationType { RU }

    #endregion
    #region Перечисление StringType

    /// <summary>Перечисление для класса IsStringLatin</summary>
    public enum StringType
    {
        RUS, LAT, RUSLAT
    }

    #endregion
    #region Перечисление TransliterationType

    /// <summary>Перечисления типов транслитераций для класса Transliteration</summary>
    public enum TransliterationType
    {
        Gost, ISO
    }

    #endregion
    #region Перечисление TranslitDirection

    /// <summary>Перечисления типов направления транслитерации. RU_LAT - с латиницы на русские буквы; LAT_RU - с русских букв на латинские.
    /// Создано для класса Transliteration</summary>
    public enum TranslitDirection
    {
        RU_LAT, LAT_RU
    }

    #endregion

    #region Перечисление ImageTypesMy

    /// <summary>Перечисление типов файлов</summary>
    public enum ImageTypesMy
    {
        JPG, JPEG, PNG, TIFF
    }

    #endregion

    #region Перечисление AgeCategories

    /// <summary>Перечисление всех типов возрастных категорий участников конкурсов</summary>
    public enum AgeCategories
    {
        baybi,
        doshkolnaya,
        mladshaya,
        srednaya,
        starshaya,
        molodezh,
        smeshannaya,
        group1,
        group2,
        group3,
        group4,
        group5,
        group6,
        profi,
        group2011,
        group2012,
        group2013,
        group2014,
        group2015,
        group2016,
        group7_9,
        group8_11,
        group10_13,
        group12_15,
        VNEKATEGORY
    }

    #endregion

    #region Перечисление PrintDocumentType

    /// <summary>Перечисление типов печатных документов</summary>
    public enum PrintDocumentType
    {
        diplomIndividual1Round,
        diplomGroup1Round,
        certificateIndividual1Round,
        certificateGroup1Round,
        diplomIndividual2Round,
        diplomGroup2Round,
        certificateIndividual2Round,
        certificateGroup2Round,
        thanksLetter
    }

    #endregion

    #region Класс помощник для определения значений перечислений

    /// <summary>Вспомогательный класс, которые закрепляет определённые строковый значения за значениями перечислений</summary>
    public class EnumsHelper
    {
        #region Метод GetCompetitionValueFromCode(string code)

        /// <summary>Метод возвращает название Конкурса по его коду</summary>
        /// <param name="code">код конкурса</param>
        /// <returns></returns>
        public static string GetCompetitionValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "foto": result = "Конкурс «Крымский вернисаж»"; break;
                case "theatre": result = "Конкурс «Мастер сцены»"; break;
                case "literary": result = "Литературный конкурс «Москва и Крым глазами современников»"; break;
                case "sport": result = "Спортивные турниры"; break;
                case "kultura": result = "Конкурс творческих и исследовательских работ «Открытый мир»"; break;
                case "toponim": result = "Конкурс «Черноморский флот Великой Отечественной войны в топонимике городов России»"; break;
                case "robotech": result = "Конкурс Робототехники и 3D моделирования «Шаг в будущее»"; break;
                case "VmesteSila": result = "Фестиваль-конкурс «Вместе мы сильнее!»"; break;
                case "Clothes": result = "Конкурс «Индустрия моды»"; break;
                case "Multimedia": result = "Конкурс мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!»"; break;
                case "Korablik": result = "Открытый конкурс «Кораблик детства»"; break;
                case "Crimroute": result = "Конкурс проектных работ «Крымские маршруты»"; break;
                case "Mathbattle": result = "Конкурс «Математический батл»"; break;
                case "Kosmos": result = "Квест-игра «Покоряя космос»"; break;
                case "Science": result = "Конкурс научных работ «В моей лаборатории вот что...»"; break;

                /*=======================================Очные / заочные мероприятия================================================*/

                case "sportChess": result = "Спортивный турнир по шахматам «В единстве наша сила»"; break;
                case "sportCheckers": result = "Спортивный турнир по шашкам «В единстве наша сила»"; break;
                case "sportEdinoborstva": result = "Спортивный фестиваль по единоборствам «В единстве наша сила»"; break;
                case "sportBoks": result = "Спортивный фестиваль по боксу «В единстве наша сила»"; break;
                case "sportLazertag": result = "Спортивный турнир по игре в тактический лазертаг «В единстве наша сила»"; break;
                case "sportFootball": result = "Спортивный турнир по футболу «В единстве наша сила»"; break;
                case "sportBasketball": result = "Спортивный турнир по баскетболу «В единстве наша сила»"; break;
                case "sportVolleyball": result = "Спортивный турнир по волейболу «В единстве наша сила»"; break;

                case "theatreVokal": result = "Конкурс Мастер сцены «Вокальный жанр»"; break;
                case "theatreHudSlovo": result = "Конкурс Мастер сцены «Художественное слово»"; break;
                case "theatreInsrum": result = "Конкурс Мастер сцены «Инструментальный жанр»"; break;
                case "theatreHoreo": result = "Конкурс Мастер сцены «Хореография»"; break;

                case "korablikHudSlovo": result = "Конкурс Кораблик детства «Художественное слово»"; break;
                case "korablikHoreo": result = "Конкурс Кораблик детства «Хореография»"; break;
                case "korablikVokal": result = "Конкурс Кораблик детства «Вокал»"; break;

                case "modaLoskutokKuturie": result = "Конкурс Индустрия моды «Чудесные лоскутки», «Юный кутюрье»"; break;
                case "modaTeatrModi": result = "Конкурс Индустрия моды «Театр моды»"; break;

                /*=======================================КОНЕЦ Очные / заочные мероприятия================================================*/

                /*=======================================Заочно-очные конкурсы================================================*/
                case "vernisazFotoGrahipcs": result = "Конкурс Крымский вернисаж «Фото», «Компьютерная графика»"; break;
                case "vernisazIzo": result = "Конкурс Крымский вернисаж «ИЗО»"; break;
                case "vernisazDpi": result = "Конкурс Крымский вернисаж «ДПИ»"; break;

                case "openWorld": result = "Конкурс творческих и исследовательских работ на английском языке «Открытый мир»"; break;

                case "kvestHraniteliIstorii": result = "Квест-Игра «Хранители истории»"; break;

                case "theatreIskustvo": result = "Конкурс Мастер сцены «Театральное искусство»"; break;

                case "mathBattle": result = "Математический батл «Крым в цифрах, фактах и задачах»"; break;
                case "modaModelierSedobnaya": result = "Конкурс «Индустрия моды»"; break;
                case "vmesteSila": result = "Конкурс для детей с ограниченными возможностями здоровья (ОВЗ) «Вместе мы сильнее!»"; break;
                case "shagVBudushee": result = "Открытый конкурс по робототехнике и моделированию 3D ручкой «Шаг в будущее»"; break;

                case "multimedia": result = "Конкурс мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!», посвященный 80-й годовщине Победы в Великой Отечественной войне"; break;
                /*=======================================КОНЕЦ Заочно-очные конкурсы================================================*/
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetCompetitionsNewCode(Sport fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления новых конкурсов</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetCompetitionsNewCode(CompetitionsNew fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case CompetitionsNew.sportChess: result = "sportChess"; break;
                case CompetitionsNew.sportCheckers: result = "sportCheckers"; break;
                case CompetitionsNew.sportEdinoborstva: result = "sportEdinoborstva"; break;
                case CompetitionsNew.sportBoks: result = "sportBoks"; break;
                case CompetitionsNew.sportLazertag: result = "sportLazertag"; break;
                case CompetitionsNew.sportFootball: result = "sportFootball"; break;
                case CompetitionsNew.sportBasketball: result = "sportBasketball"; break;
                case CompetitionsNew.sportVolleyball: result = "sportVolleyball"; break;

                case CompetitionsNew.theatreVokal: result = "theatreVokal"; break;
                case CompetitionsNew.theatreHudSlovo: result = "theatreHudSlovo"; break;
                case CompetitionsNew.theatreInsrum: result = "theatreInsrum"; break;
                case CompetitionsNew.theatreHoreo: result = "theatreHoreo"; break;
                case CompetitionsNew.korablikHudSlovo: result = "korablikHudSlovo"; break;
                case CompetitionsNew.korablikHoreo: result = "korablikHoreo"; break;
                case CompetitionsNew.korablikVokal: result = "korablikVokal"; break;
                case CompetitionsNew.modaLoskutokKuturie: result = "modaLoskutokKuturie"; break;
                case CompetitionsNew.modaTeatrModi: result = "modaTeatrModi"; break;

                case CompetitionsNew.vernisazFotoGrahipcs: result = "vernisazFotoGrahipcs"; break;
                case CompetitionsNew.vernisazIzo: result = "vernisazIzo"; break;
                case CompetitionsNew.vernisazDpi: result = "vernisazDpi"; break;
                case CompetitionsNew.openWorld: result = "openWorld"; break;
                case CompetitionsNew.kvestHraniteliIstorii: result = "kvestHraniteliIstorii"; break;
                case CompetitionsNew.theatreIskustvo: result = "theatreIskustvo"; break;
                case CompetitionsNew.mathBattle: result = "mathBattle"; break;
                case CompetitionsNew.modaModelierSedobnaya: result = "modaModelierSedobnaya"; break;
                case CompetitionsNew.vmesteSila: result = "vmesteSila"; break;
                case CompetitionsNew.shagVBudushee: result = "shagVBudushee"; break;
                case CompetitionsNew.multimedia: result = "multimedia"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region GetCompetitionValueFromCodeForDocument

        /// <summary>Метод возвращает название Конкурса по его коду для документов</summary>
        /// <param name="code">код конкурса</param>
        /// <returns></returns>
        public static string GetCompetitionValueFromCodeForDocument(string code)
        {
            string result = "";

            switch (code)
            {
                case "foto": result = "Конкурса «<b>Крымский Вернисаж</b>»"; break;
                case "theatre": result = "Конкурса «<b>Мастер сцены</b>»"; break;
                case "literary": result = "Литературного конкурса<br/>«<b>Москва и Крым глазами современников</b>»"; break;
                case "sport": result = "Спортивного турнира<br/>«<b>В единстве наша сила</b>»"; break;
                case "kultura": result = "Конкурса творческих и исследовательских работ<br/>«<b>Открытый мир</b>»"; break;
                case "toponim": result = "Конкурса «<b>Черноморский флот Великой Отечественной войны в топонимике городов России</b>»"; break;
                case "robotech": result = "Конкурса Робототехники и 3D моделирования «<b>Шаг в будущее</b>»"; break;
                case "VmesteSila": result = "Фестиваль-конкурса «<b>Вместе мы сильнее!</b>»"; break;
                case "Clothes": result = "Конкурса «<b>Индустрия моды</b>»"; break;
                case "Multimedia": result = "Конкурса мультимедийных проектов «<b>Герой войны, достойный Славы, любимый город Севастополь!</b>»"; break;
                case "Korablik": result = "Открытого конкурса «<b>Кораблик детства</b>»"; break;
                case "Crimroute": result = "Конкурса проектных работ «<b>Крымские маршруты</b>»"; break;
                case "Mathbattle": result = "Конкурса «<b>Математический батл</b>»"; break;
                case "Kosmos": result = "Квест-игры «<b>Покоряя космос</b>»"; break;
                case "Science": result = "Конкурса научных работ «В моей лаборатории вот что...»"; break;
                default: break;
            }

            return result;
        }
        #endregion

        #region Метод GetSessionFromCompetitionCode(string code)

        /// <summary>Метод возвращает сессию по названию Конкурса</summary>
        /// <param name="code">код конкурса</param>
        /// <returns></returns>
        public static string GetSessionFromCompetitionCode(string code)
        {
            return String.Concat("Temp", code, "Request");
        }

        #endregion

        #region GetSessionName(Enum T)
        public static string GetSessionName(Enum T)
        {
            string sessionName = "";

            switch (T)
            {
                case (Photo.self): sessionName = GetSessionFromCompetitionCode(GetPhotoCode(Photo.self)); break;
                case (Literary.self): sessionName = GetSessionFromCompetitionCode(GetLiteraryCode(Literary.self)); break;
                case (Theatre.self): sessionName = GetSessionFromCompetitionCode(GetTheatreCode(Theatre.self)); break;
                case (Sport.self): sessionName = GetSessionFromCompetitionCode(GetSportCode(Sport.self)); break;
                case (Toponim.self): sessionName = GetSessionFromCompetitionCode(GetToponimCode(Toponim.self)); break;
                case (Robotech.self): sessionName = GetSessionFromCompetitionCode(GetRobotechCode(Robotech.self)); break;
                case (Kultura.self): sessionName = GetSessionFromCompetitionCode(GetKulturaCode(Kultura.self)); break;
                case (VmesteSila.self): sessionName = GetSessionFromCompetitionCode(GetVmesteSilaCode(VmesteSila.self)); break;
                case (Clothes.self): sessionName = GetSessionFromCompetitionCode(GetClothesCode(Clothes.self)); break;
                case (Multimedia.self): sessionName = GetSessionFromCompetitionCode(GetMultimediaCode(Multimedia.self)); break;
                case (Korablik.self): sessionName = GetSessionFromCompetitionCode(GetKorablikCode(Korablik.self)); break;
                case (Crimroute.self): sessionName = GetSessionFromCompetitionCode(GetCrimrouteCode(Crimroute.self)); break;
                case (Mathbattle.self): sessionName = GetSessionFromCompetitionCode(GetMathbattleCode(Mathbattle.self)); break;
                case (Kosmos.self): sessionName = GetSessionFromCompetitionCode(GetKosmosCode(Kosmos.self)); break;
                case (Science.self): sessionName = GetSessionFromCompetitionCode(GetScienceCode(Science.self)); break;
                default: break;
            }

            return sessionName;
        }
        #endregion

        #region Метод GetTheatreValue(Theatre fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями театрального конкурса и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetTheatreValue(Theatre fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Theatre.hudSlovo: result = "Художественное слово"; break;
                
                case Theatre.insrumZanrAnsambli: result = "Инструментальный жанр: ансамбли"; break;
                case Theatre.insrumZanrDuhovieUdarnInstrum: result = "Инструментальный жанр: духовые и ударные инструменты"; break;
                case Theatre.insrumZanrFortepiano: result = "Инструментальный жанр: фортепиано"; break;
                case Theatre.insrumZanrGitara: result = "Инструментальный жанр: гитара"; break;
                case Theatre.insrumZanrNarodnieInstrum: result = "Инструментальный жанр: народные инструменты"; break;
                case Theatre.insrumZanrOrkestri: result = "Инструментальный жанр: оркестры"; break;
                case Theatre.insrumZanrSintezator: result = "Инструментальный жанр: синтезатор"; break;
                case Theatre.insrumZanrStrunnoSmichkovieInstrumenti: result = "Инструментальный жанр: струнно - смычковые инструменты"; break;
                
                case Theatre.teatrIskusLitMuzKom: result = "Театральное искусство: Литературно - музыкальная композиция"; break;
                case Theatre.teatrIskusSpekt: result = "Театральное искусство: Спектакль"; break;
                case Theatre.teatrIskusMultiGanr: result = "Театральное искусство: Мультижанр"; break;

                case Theatre.vokalAkademVokal: result = "Вокал: академический вокал"; break;
                case Theatre.vokalEstradVokal: result = "Вокал: эстрадный вокал"; break;
                case Theatre.vokalFolklor: result = "Вокал: фольклор"; break;

                case Theatre.xoreoBalniyTanets: result = "Хореография: бальный танец"; break;
                case Theatre.xoreoClassichTanets: result = "Хореография: классический танец"; break;
                case Theatre.xoreoEstradTanets: result = "Хореография: эстрадный танец"; break;
                case Theatre.xoreoNarodTanets: result = "Хореография: народный танец"; break;
                case Theatre.xoreoStilNarodTanets: result = "Хореография: стилизованный народный танец"; break;
                case Theatre.xoreoSovremTanets: result = "Хореография: современный танец"; break;
                case Theatre.xoreoCircIskus: result = "Хореография: цирковое искусство"; break;
                case Theatre.xoreoKadeti: result = "Хореография: кадеты"; break;
                case Theatre.xoreoCherliding: result = "Хореография: черлидинг"; break;

                case Theatre.NO: result = "НЕТ"; break;
                case Theatre.self: result = "theatre"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetTheatreCode(Theatre fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями театрального конкурса</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetTheatreCode(Theatre fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Theatre.hudSlovo: result = "hudslovo"; break;
                case Theatre.insrumZanrAnsambli: result = "insrumzanransambli"; break;
                case Theatre.insrumZanrDuhovieUdarnInstrum: result = "insrumzanrduhovieudarninstrum"; break;
                case Theatre.insrumZanrFortepiano: result = "insrumzanrfortepiano"; break;
                case Theatre.insrumZanrGitara: result = "insrumzanrgitara"; break;
                case Theatre.insrumZanrNarodnieInstrum: result = "insrumzanrnarodnieinstrum"; break;
                case Theatre.insrumZanrOrkestri: result = "insrumzanrorkestri"; break;
                case Theatre.insrumZanrSintezator: result = "insrumzanrsintezator"; break;
                case Theatre.insrumZanrStrunnoSmichkovieInstrumenti: result = "insrumzanrstrunnosmichkovieinstrumenti"; break;
                case Theatre.teatrIskusLitMuzKom: result = "teatriskuslitmuzkom"; break;
                case Theatre.teatrIskusSpekt: result = "teatriskusspekt"; break;
                case Theatre.teatrIskusMultiGanr: result = "teatriskusmultiganr"; break;
                case Theatre.vokalAkademVokal: result = "vokalakademvokal"; break;
                case Theatre.vokalEstradVokal: result = "vokalestradvokal"; break;
                case Theatre.vokalFolklor: result = "vokalfolklor"; break;
                case Theatre.xoreoBalniyTanets: result = "xoreobalniytanets"; break;
                case Theatre.xoreoClassichTanets: result = "xoreoclassichtanets"; break;
                case Theatre.xoreoEstradTanets: result = "xoreoestradtanets"; break;
                case Theatre.xoreoNarodTanets: result = "xoreonarodtanets"; break;
                case Theatre.xoreoStilNarodTanets: result = "xoreostilnarodtanets"; break;
                case Theatre.xoreoSovremTanets: result = "xoreosovremtanets"; break;
                case Theatre.xoreoCircIskus: result = "xoreocirciskus"; break;
                case Theatre.xoreoKadeti: result = "xoreokadeti"; break;
                case Theatre.xoreoCherliding: result = "xoreocherliding"; break;

                case Theatre.NO: result = "NO"; break;
                case Theatre.self: result = "theatre"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetTheatreValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса "Мастер сцены" по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetTheatreValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "hudslovo": result = "Художественное слово"; break;
                case "insrumzanransambli": result = "Инструментальный жанр: ансамбли"; break;
                case "insrumzanrduhovieudarninstrum": result = "Инструментальный жанр: духовые и ударные инструменты"; break;
                case "insrumzanrfortepiano": result = "Инструментальный жанр: фортепиано"; break;
                case "insrumzanrgitara": result = "Инструментальный жанр: гитара"; break;
                case "insrumzanrnarodnieinstrum": result = "Инструментальный жанр: народные инструменты"; break;
                case "insrumzanrorkestri": result = "Инструментальный жанр: оркестры"; break;
                case "insrumzanrsintezator": result = "Инструментальный жанр: синтезатор"; break;
                case "insrumzanrstrunnosmichkovieinstrumenti": result = "Инструментальный жанр: струнно - смычковые инструменты"; break;
                case "teatriskuslitmuzkom": result = "Театральное искусство: Литературно - музыкальная композиция"; break;
                case "teatriskusspekt": result = "Театральное искусство: Спектакль"; break;
                case "teatriskusmultiganr": result = "Театральное искусство: Мультижанр"; break;
       
                case "vokalakademvokal": result = "Вокал: академический вокал"; break;
                case "vokalestradvokal": result = "Вокал: эстрадный вокал"; break;
                case "vokalfolklor": result = "Вокал: фольклор"; break;
                case "xoreobalniytanets": result = "Хореография: бальный танец"; break;
                case "xoreoclassichtanets": result = "Хореография: классический танец"; break;
                case "xoreoestradtanets": result = "Хореография: эстрадный танец"; break;
                case "xoreonarodtanets": result = "Хореография: народный танец"; break;
                case "xoreostilnarodtanets": result = "Хореография: стилизованный народный танец"; break;
                case "xoreosovremtanets": result = "Хореография: современный танец"; break;
                case "xoreokadeti": result = "Хореография: кадеты"; break;
                case "xoreocherliding": result = "Хореография: черлидинг"; break;
                case "xoreocirciskus": result = "Хореография: цирковое искусство"; break;
                    
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetTheatreCodeFromValue(string code)

        /// <summary>Метод код номинации конкурса "Мастер сцены" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetTheatreCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Художественное слово": result = "hudslovo"; break;
                case "Инструментальный жанр: ансамбли": result = "insrumzanransambli"; break;
                case "Инструментальный жанр: духовые и ударные инструменты": result = "insrumzanrduhovieudarninstrum"; break;
                case "Инструментальный жанр: фортепиано": result = "insrumzanrfortepiano"; break;
                case "Инструментальный жанр: гитара": result = "insrumzanrgitara"; break;
                case "Инструментальный жанр: народные инструменты": result = "insrumzanrnarodnieinstrum"; break;
                case "Инструментальный жанр: оркестры": result = "insrumzanrorkestri"; break;
                case "Инструментальный жанр: синтезатор": result = "insrumzanrsintezator"; break;
                case "Инструментальный жанр: струнно - смычковые инструменты": result = "insrumzanrstrunnosmichkovieinstrumenti"; break;
                case "Театральное искусство: Литературно - музыкальная композиция": result = "teatriskuslitmuzkom"; break;
                case "Театральное искусство: Спектакль": result = "teatriskusspekt"; break;
                case "Театральное искусство: Мультижанр": result = "teatriskusmultiganr"; break;
                    
                case "Вокал: академический вокал": result = "vokalakademvokal"; break;
                case "Вокал: эстрадный вокал": result = "vokalestradvokal"; break;
                case "Вокал: фольклор": result = "vokalfolklor"; break;
                case "Хореография: бальный танец": result = "xoreobalniytanets"; break;
                case "Хореография: классический танец": result = "xoreoclassichtanets"; break;
                case "Хореография: эстрадный танец": result = "xoreoestradtanets"; break;
                case "Хореография: народный танец": result = "xoreonarodtanets"; break;
                case "Хореография: стилизованный народный танец": result = "xoreostilnarodtanets"; break;
                case "Хореография: современный танец": result = "xoreosovremtanets"; break;
                case "Хореография: кадеты": result = "xoreokadeti"; break;
                case "Хореография: черлидинг": result = "xoreocherliding"; break;
                case "Хореография: цирковое искусство": result = "xoreocirciskus"; break;
               
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetPhotoValue(Photo fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями фотоконкурса "Крымский вернисаж" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetPhotoValue(Photo fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Photo.photo: result = "Фотография"; break;
                case Photo.izo: result = "Изобразительное творчество"; break;
                case Photo.computerGraphic: result = "Компьютерная графика"; break;
                case Photo.computer_risunok: result = "Компьютерный рисунок"; break;
                case Photo.collazh_fotomontazh: result = "Коллаж, фотомонтаж"; break;
                case Photo.DPT1_makrame: result = "Макраме"; break;
                case Photo.DPT1_gobelen: result = "Гобелен"; break;
                case Photo.DPT1_hud_vishivka: result = "Художественная вышивка"; break;
                case Photo.DPT1_hud_vyazanie: result = "Вязание (одежда, салфетки, игрушка, макраме)"; break;
                case Photo.DPT1_bumagoplastika: result = "Бумагопластика"; break;
                case Photo.DPT1_loskut_shitie: result = "Лоскутное шитье"; break;
                case Photo.DPT1_avtor_igrushka: result = "Текстиль (тильды, шитье, вышивка)"; break;
                case Photo.DPT1_voilokovalyanie: result = "Валяние (картины, игрушка)"; break;
                case Photo.DPT1_biseropletenie: result = "Бисероплетение"; break;
                case Photo.DPT1_fitodisign: result = "Фитодизайн"; break;
                case Photo.DPT2_keramika: result = "Керамика"; break;
                case Photo.DPT2_hud_obr_stekla: result = "Художественная обработка стекла"; break;
                case Photo.DPT2_hud_obr_kozhi: result = "Художественная обработка кожи"; break;
                case Photo.DPT2_narod_igrush_isglini: result = "Народная игрушка из глины"; break;
                case Photo.DPT2_batik: result = "Батик"; break;
                case Photo.DPT2_dekupazh: result = "Декупаж"; break;
                case Photo.DPT2_rospis_poderevu: result = "Роспись по дереву"; break;
                case Photo.DPT1_combitehnics: result = "Комбинированные техники"; break;
                case Photo.DPT1_plastilonografiya: result = "Пластилинография"; break;
                case Photo.NO: result = "НЕТ"; break;
                case Photo.self: result = "foto"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetPhotoCode(Photo fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями фотоконкурса "Крымский вернисаж"</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetPhotoCode(Photo fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Photo.photo: result = "photo"; break;
                case Photo.izo: result = "izo"; break;
                case Photo.computerGraphic: result = "computergraphic"; break;
                case Photo.computer_risunok: result = "computer_risunok"; break;
                case Photo.collazh_fotomontazh: result = "collazh_fotomontazh"; break;
                case Photo.DPT1: result = "dpt1"; break;
                case Photo.DPT1_makrame: result = "dpt1_makrame"; break;
                case Photo.DPT1_gobelen: result = "dpt1_gobelen"; break;
                case Photo.DPT1_hud_vishivka: result = "dpt1_hud_vishivka"; break;
                case Photo.DPT1_hud_vyazanie: result = "dpt1_hud_vyazanie"; break;
                case Photo.DPT1_bumagoplastika: result = "dpt1_bumagoplastika"; break;
                case Photo.DPT1_loskut_shitie: result = "dpt1_loskut_shitie"; break;
                case Photo.DPT1_avtor_igrushka: result = "dpt1_avtor_igrushka"; break;
                case Photo.DPT1_voilokovalyanie: result = "dpt1_voilokovalyanie"; break;
                case Photo.DPT1_biseropletenie: result = "dpt1_biseropletenie"; break;
                case Photo.DPT1_fitodisign: result = "dpt1_fitodisign"; break;
                case Photo.DPT2: result = "dpt2"; break;
                case Photo.DPT2_keramika: result = "dpt2_keramika"; break;
                case Photo.DPT2_hud_obr_stekla: result = "dpt2_hud_obr_stekla"; break;
                case Photo.DPT2_hud_obr_kozhi: result = "dpt2_hud_obr_kozhi"; break;
                case Photo.DPT2_narod_igrush_isglini: result = "dpt2_narod_igrush_isglini"; break;
                case Photo.DPT2_batik: result = "dpt2_batik"; break;
                case Photo.DPT2_dekupazh: result = "dpt2_dekupazh"; break;
                case Photo.DPT2_rospis_poderevu: result = "dpt2_rospis_poderevu"; break;
                case Photo.DPT1_combitehnics: result = "dpt1_combitehnics"; break;
                case Photo.DPT1_plastilonografiya: result = "dpt1_plastilonografiya"; break;
                case Photo.NO: result = "NO"; break;
                case Photo.self: result = "foto"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetPhotoValueFromCode(string code)

        /// <summary>Метод возвращает название номинации фотоконкурса "Крымский вернисаж" по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetPhotoValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "photo": result = "Фотография"; break;
                case "izo": result = "Изобразительное творчество"; break;
                case "computergraphic": result = "Компьютерная графика"; break;
                case "computer_risunok": result = "Компьютерный рисунок"; break;
                case "collazh_fotomontazh": result = "Коллаж, фотомонтаж"; break;
                case "dpt1_makrame": result = "Макраме"; break;
                case "dpt1_gobelen": result = "Гобелен"; break;
                case "dpt1_hud_vishivka": result = "Художественная вышивка"; break;
                case "dpt1_hud_vyazanie": result = "Вязание (одежда, салфетки, игрушка, макраме)"; break;
                case "dpt1_bumagoplastika": result = "Бумагопластика"; break;
                case "dpt1_loskut_shitie": result = "Лоскутное шитье"; break;
                case "dpt1_avtor_igrushka": result = "Текстиль (тильды, шитье, вышивка)"; break;
                case "dpt1_voilokovalyanie": result = "Валяние (картины, игрушка)"; break;
                case "dpt1_biseropletenie": result = "Бисероплетение"; break;
                case "dpt1_fitodisign": result = "Фитодизайн"; break;
                case "dpt1_combitehnics": result = "Комбинированные техники"; break;
                case "dpt1_plastilonografiya": result = "Пластилинография"; break;
                case "dpt2_keramika": result = "Керамика"; break;
                case "dpt2_hud_obr_stekla": result = "Художественная обработка стекла"; break;
                case "dpt2_hud_obr_kozhi": result = "Художественная обработка кожи"; break;
                case "dpt2_narod_igrush_isglini": result = "Народная игрушка из глины"; break;
                case "dpt2_batik": result = "Батик"; break;
                case "dpt2_dekupazh": result = "Декупаж"; break;
                case "dpt2_rospis_poderevu": result = "Роспись по дереву"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetPhotoCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации фотоконкурса "Крымский вернисаж" по её названию</summary>
        /// <param name="value">код номинации</param>
        /// <returns></returns>
        public static string GetPhotoCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Фотография": result = "photo"; break;
                case "Изобразительное творчество": result = "izo"; break;
                case "Компьютерная графика": result = "computergraphic"; break;
                case "Компьютерный рисунок": result = "computer_risunok"; break;
                case "Коллаж, фотомонтаж": result = "collazh_fotomontazh"; break;
                case "Макраме": result = "dpt1_makrame"; break;
                case "Гобелен": result = "dpt1_gobelen"; break;
                case "Художественная вышивка": result = "dpt1_hud_vishivka"; break;
                case "Вязание (одежда, салфетки, игрушка, макраме)": result = "dpt1_hud_vyazanie"; break;
                case "Бумагопластика": result = "dpt1_bumagoplastika"; break;
                case "Лоскутное шитье": result = "dpt1_loskut_shitie"; break;
                case "Текстиль (тильды, шитье, вышивка)": result = "dpt1_avtor_igrushka"; break;
                case "Валяние (картины, игрушка)": result = "dpt1_voilokovalyanie"; break;
                case "Бисероплетение": result = "dpt1_biseropletenie"; break;
                case "Фитодизайн": result = "dpt1_fitodisign"; break;
                case "Керамика": result = "dpt2_keramika"; break;
                case "Художественная обработка стекла": result = "dpt2_hud_obr_stekla"; break;
                case "Художественная обработка кожи": result = "dpt2_hud_obr_kozhi"; break;
                case "Народная игрушка из глины": result = "dpt2_narod_igrush_isglini"; break;
                case "Батик": result = "dpt2_batik"; break;
                case "Декупаж": result = "dpt2_dekupazh"; break;
                case "Роспись по дереву": result = "dpt2_rospis_poderevu"; break;
                case "Комбинированные техники": result = "dpt1_combitehnics"; break;
                case "Пластилинография": result = "dpt1_plastilonografiya"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetLiteraryValue(Literary fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями литературного конкурса "Боевая слава Севастополя" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetLiteraryValue(Literary fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Literary.stihi: result = "Стихи"; break;
                case Literary.esse: result = "Эссе"; break;
                case Literary.rasskaz: result = "Рассказ"; break;
                case Literary.sochinenie: result = "Сочинение"; break;
                case Literary.NO: result = "НЕТ"; break;
                case Literary.self: result = "literary"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetLiteraryCode(Literary fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями литературного конкурса "Боевая слава Севастополя"</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetLiteraryCode(Literary fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Literary.stihi: result = "stihi"; break;
                case Literary.esse: result = "esse"; break;
                case Literary.rasskaz: result = "rasskaz"; break;
                case Literary.sochinenie: result = "sochinenie"; break;
                case Literary.NO: result = "NO"; break;
                case Literary.self: result = "literary"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetLiteraryValueFromCode(string code)

        /// <summary>Метод возвращает название номинации литературного конкурса "Боевая слава Севастополя" по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetLiteraryValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "stihi": result = "Стихи"; break;
                case "esse": result = "Эссе"; break;
                case "rasskaz": result = "Рассказ"; break;
                case "sochinenie": result = "Сочинение"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetLiteraryCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации литературного конкурса "Боевая слава Севастополя" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetLiteraryCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Стихи": result = "stihi"; break;
                case "Эссе": result = "esse"; break;
                case "Рассказ": result = "rasskaz"; break;
                case "Сочинение": result = "sochinenie"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetSportValue(Sport fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями спортивного турнира "В единстве наша сила" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetSportValue(Sport fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Sport.prostEdinoborstva: result = "Простейшие единоборства"; break;
                case Sport.boks: result = "Бокс"; break;
                case Sport.kungfu: result = "Кунг-фу УИ"; break;
                case Sport.thekvo: result = "Тхэквондо (ИТФ) среди мальчиков и девочек"; break;
                case Sport.stendStrelba: result = "Тактический лазертаг"; break;
                case Sport.football: result = "Футбол"; break;
                case Sport.shahmaty: result = "Шахматы"; break;
                case Sport.shashki: result = "Шашки"; break;
                case Sport.basketball: result = "Баскетбол"; break;
                case Sport.volleyball: result = "Волейбол"; break;
                case Sport.NO: result = "НЕТ"; break;
                case Sport.self: result = "sport"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetSportCode(Sport fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями спортивного турнира "В единстве наша сила"</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetSportCode(Sport fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Sport.prostEdinoborstva: result = "prostedinoborstva"; break;
                case Sport.boks: result = "boks"; break;
                case Sport.kungfu: result = "kungfu"; break;
                case Sport.thekvo: result = "thekvo"; break;
                case Sport.stendStrelba: result = "stendstrelba"; break;
                case Sport.football: result = "football"; break;
                case Sport.shahmaty: result = "shahmaty"; break;
                case Sport.shashki: result = "shashki"; break;
                case Sport.basketball: result = "basketball"; break;
                case Sport.volleyball: result = "volleyball"; break;
                case Sport.NO: result = "NO"; break;
                case Sport.self: result = "sport"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetSportValueFromCode(string code)

        /// <summary>Метод возвращает название номинации спортивного турнира "В единстве наша сила" по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetSportValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "prostedinoborstva": result = "Простейшие единоборства"; break;
                case "boks": result = "Бокс"; break;
                case "kungfu": result = "Кунг-фу УИ"; break;
                case "thekvo": result = "Тхэквондо (ИТФ) среди мальчиков и девочек"; break;
                case "stendstrelba": result = "Тактический лазертаг"; break;
                case "football": result = "Футбол"; break;
                case "shahmaty": result = "Шахматы"; break;
                case "shashki": result = "Шашки"; break;
                case "basketball": result = "Баскетбол"; break;
                case "volleyball": result = "Волейбол"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetSportCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации спортивного турнира "В единстве наша сила" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetSportCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Простейшие единоборства": result = "prostedinoborstva"; break;
                case "Бокс": result = "boks"; break;
                case "Кунг-фу УИ": result = "kungfu"; break;
                case "Тхэквондо (ИТФ) среди мальчиков и девочек": result = "thekvo"; break;
                case "Тактический лазертаг": result = "stendstrelba"; break;
                case "Футбол": result = "football"; break;
                case "Шахматы": result = "shahmaty"; break;
                case "Шашки": result = "shashki"; break;
                case "Баскетбол": result = "basketball"; break;
                case "Волейбол": result = "volleyball"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetKulturaValue(Kultura fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса творческих и исследовательских работ «Открытый мир» и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKulturaValue(Kultura fromEnum)
        {
            string result = "";
            
            switch (fromEnum)
            {
                case Kultura.vossoedCrimaSRossiei: result = "Крым с нами!"; break;
                case Kultura.k75letiyu: result = "75-я годовщина Победы в Великой Отечественной Войне"; break;
                case Kultura.zashitaobsihgranits: result = "На защите общих границ"; break;
                case Kultura.kultrnNasledieCrima: result = "Наследие Крыма и города Севастополя"; break;
                case Kultura.krimskyMostVSZ: result = "\"Крымский мост. Вчера. Сегодня. Завтра\""; break;
                case Kultura.iSeeCrimea: result = "\"I see Crimea...\" (Я вижу Крым...)"; break;
                case Kultura.publishInstagram: result = "Конкурс публикаций на английском языке в \"Instagram\" с хештегом #IseeCrimea"; break;
                case Kultura.presentaionRu: result = "Конкурс презентаций на русском языке \"Москва – Крым\""; break;

                case Kultura.iSeeCrimeaEn: result = "Конкурс эссе \"I see Crimea...\" (Я вижу Крым...) на английском языке"; break;
                case Kultura.presentaionEn: result = "Конкурс презентаций на английском языке \"Москва - Крым\""; break;
                case Kultura.audioGaid: result = "Конкурс аудиогидов по туристическим маршрутам на английском языке \"Москва-Крым\""; break;
                case Kultura.publishVkontakte: result = "Конкурс публикаций на английском языке в \"Vkontakte\" с хештегом #IseeCrimea"; break;
                case Kultura.intellektualKviz: result = "Интеллектуальный квиз \"I see Crimea...\""; break;

                case Kultura.NO: result = "НЕТ"; break;
                case Kultura.self: result = "kultura"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKulturaCode(Kultura fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса творческих и исследовательских работ «Открытый мир»</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKulturaCode(Kultura fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Kultura.vossoedCrimaSRossiei: result = "vossoedCrimaSRossiei"; break;
                case Kultura.k75letiyu: result = "k75letiyu"; break;
                case Kultura.zashitaobsihgranits: result = "zashitaobsihgranits"; break;
                case Kultura.kultrnNasledieCrima: result = "kultrnNasledieCrima"; break;
                case Kultura.krimskyMostVSZ: result = "krimskyMostVSZ"; break;
                case Kultura.iSeeCrimea: result = "iSeeCrimea"; break;
                case Kultura.presentaionRu: result = "presentaionRu"; break;
                case Kultura.publishInstagram: result = "publishInstagram"; break;

                case Kultura.iSeeCrimeaEn: result = "iSeeCrimeaEn"; break;
                case Kultura.presentaionEn: result = "presentaionEn"; break;
                case Kultura.publishVkontakte: result = "publishVkontakte"; break;
                case Kultura.audioGaid: result = "audioGaid"; break;
                case Kultura.intellektualKviz: result = "intellektualKviz"; break;

                case Kultura.NO: result = "NO"; break;
                case Kultura.self: result = "kultura"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKulturaValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса творческих и исследовательских работ «Открытый мир» по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetKulturaValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "vossoedCrimaSRossiei": result = "Крым с нами!"; break;
                case "k75letiyu": result = "75-я годовщина Победы в Великой Отечественной Войне"; break;
                case "zashitaobsihgranits": result = "На защите общих границ"; break;
                case "kultrnNasledieCrima": result = "Наследие Крыма и города Севастополя"; break;
                case "krimskyMostVSZ": result = "\"Крымский мост. Вчера. Сегодня. Завтра\""; break;
                case "iSeeCrimea": result = "\"I see Crimea...\" (Я вижу Крым...)"; break;
                case "presentaionRu": result = "Конкурс презентаций на русском языке \"Москва – Крым\""; break;
                case "publishInstagram": result = "Конкурс публикаций на английском языке в \"Instagram\" с хештегом #IseeCrimea"; break;

                case "iSeeCrimeaEn": result = "Конкурс эссе \"I see Crimea...\" (Я вижу Крым...) на английском языке"; break;
                case "presentaionEn": result = "Конкурс презентаций на английском языке \"Москва - Крым\""; break;
                case "publishVkontakte": result = "Конкурс публикаций на английском языке в \"Vkontakte\" с хештегом #IseeCrimea"; break;
                case "audioGaid": result = "Конкурс аудиогидов по туристическим маршрутам на английском языке \"Москва-Крым\""; break;
                case "intellektualKviz": result = "Интеллектуальный квиз \"I see Crimea...\""; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKulturaCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса творческих и исследовательских работ «Открытый мир» по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetKulturaCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Крым с нами!": result = "vossoedCrimaSRossiei"; break;
                case "75-я годовщина Победы в Великой Отечественной Войне": result = "k75letiyu"; break;
                case "На защите общих границ": result = "zashitaobsihgranits"; break;
                case "Наследие Крыма и города Севастополя": result = "kultrnNasledieCrima"; break;
                case "\"Крымский мост. Вчера. Сегодня. Завтра\"": result = "krimskyMostVSZ"; break;
                case "\"I see Crimea...\" (Я вижу Крым...)": result = "iSeeCrimea"; break;
                case "Конкурс презентаций на русском языке \"Москва – Крым\"": result = "presentaionRu"; break;
                case "Конкурс публикаций на английском языке в \"Instagram\" с хештегом #IseeCrimea": result = "publishInstagram"; break;
                
                case "Конкурс эссе \"I see Crimea...\" (Я вижу Крым...) на английском языке": result = "iSeeCrimeaEn"; break;
                case "Конкурс презентаций на английском языке \"Москва - Крым\"": result = "presentaionEn"; break;
                case "Конкурс публикаций на английском языке в \"Vkontakte\" с хештегом #IseeCrimea": result = "publishVkontakte"; break;
                case "Конкурс аудиогидов по туристическим маршрутам на английском языке \"Москва-Крым\"": result = "audioGaid"; break;
                case "Интеллектуальный квиз \"I see Crimea...\"": result = "intellektualKviz"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetToponimValue(Toponim fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса «Черноморский флот Великой Отечественной войны в топонимике городов России» и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetToponimValue(Toponim fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Toponim.toponimika: result = "Черноморский флот Великой Отечественной войны в топонимике городов России"; break;
                case Toponim.NO: result = "НЕТ"; break;
                case Toponim.self: result = "toponim"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetToponimCode(Toponim fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса «Черноморский флот Великой Отечественной войны в топонимике городов России»</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetToponimCode(Toponim fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Toponim.toponimika: result = "toponimika"; break;
                case Toponim.NO: result = "NO"; break;
                case Toponim.self: result = "toponim"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetToponimValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса «Черноморский флот Великой Отечественной войны в топонимике городов России» по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetToponimValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "toponimika": result = "Черноморский флот Великой Отечественной войны в топонимике городов России"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetToponimCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса «Черноморский флот Великой Отечественной войны в топонимике городов России» по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetToponimCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Черноморский флот Великой Отечественной войны в топонимике городов России": result = "toponimika"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetRobotechValue(Robotech fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetRobotechValue(Robotech fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Robotech.robototechnika: result = "Моделирование и конструирование"; break;
                case Robotech.robototechnikaproject: result = "Роботехнический проект"; break;
                case Robotech.robototechnika3dmodel: result = "Создание объемных моделей 3D ручкой"; break;
                case Robotech.tinkercad: result = "Создание объемных моделей в Tinkercad"; break;
                case Robotech.programmproject: result = "Программный проект"; break;
                case Robotech.NO: result = "НЕТ"; break;
                case Robotech.self: result = "robotech"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetRobotechCode(Robotech fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее»</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetRobotechCode(Robotech fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Robotech.robototechnika: result = "robototechnika"; break;
                case Robotech.robototechnikaproject: result = "robototechnikaproject"; break;
                case Robotech.robototechnika3dmodel: result = "robototechnika3dmodel"; break;
                case Robotech.tinkercad: result = "tinkercad"; break;
                case Robotech.programmproject: result = "programmproject"; break;
                case Robotech.NO: result = "NO"; break;
                case Robotech.self: result = "robotech"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetRobotechValueFromCode(string code)

        /// <summary>Метод возвращает название номинации для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetRobotechValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "robototechnika": result = "Моделирование и конструирование"; break;
                case "robototechnikaproject": result = "Роботехнический проект"; break;
                case "robototechnika3dmodel": result = "Создание объемных моделей 3D ручкой"; break;
                case "tinkercad": result = "Создание объемных моделей в Tinkercad"; break;
                case "programmproject": result = "Программный проект"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetRobotechCodeFromValue(string code)

        /// <summary>Метод возвращает код номинации для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetRobotechCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Моделирование и конструирование": result = "robototechnika"; break;
                case "Роботехнический проект": result = "robototechnikaproject"; break;
                case "Создание объемных моделей 3D ручкой": result = "robototechnika3dmodel"; break;
                case "Создание объемных моделей в Tinkercad": result = "tinkercad"; break;
                case "Программный проект": result = "programmproject"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetVmesteSilaValue(VmesteSila fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями фестиваля "Вместе мы сильнее" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetVmesteSilaValue(VmesteSila fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case VmesteSila.hudSlovo: result = "Художественное слово"; break;
                case VmesteSila.hudSlovoPoeziya: result = "Художественное слово: Поэзия"; break;
                case VmesteSila.hudSlovoProza: result = "Художественное слово: Проза"; break;

                case VmesteSila.horeographia: result = "Хореография"; break;
                case VmesteSila.horeographiaBalniyTanets: result = "Хореография: бальный танец"; break;
                case VmesteSila.horeographiaClassichTanets: result = "Хореография: классический танец"; break;
                case VmesteSila.horeographiaEstradTanets: result = "Хореография: эстрадный танец"; break;
                case VmesteSila.horeographiaNarodTanets: result = "Хореография: народный танец"; break;
                case VmesteSila.horeographiaSovremenTanets: result = "Хореография: современный танец"; break;
                case VmesteSila.horeographiaOstalnieGanri: result = "Хореография: остальные жанры"; break;

                case VmesteSila.vokal: result = "Вокал"; break;
                case VmesteSila.vokalAkademVokal: result = "Вокал: академический вокал"; break;
                case VmesteSila.vokalEstradVokal: result = "Вокал: эстрадный вокал"; break;
                case VmesteSila.vokalFolklor: result = "Вокал: фольклор"; break;
                case VmesteSila.vokalZest: result = "Вокал: жестовое пение"; break;
                case VmesteSila.vokalOstalnieGanri: result = "Вокал: остальные жанры"; break;

                case VmesteSila.instrumental: result = "Инструментальный жанр"; break;
                case VmesteSila.insrumZanrAnsambli: result = "Инструментальный жанр: ансамбли"; break;
                case VmesteSila.insrumZanrDuhovieUdarnInstrum: result = "Инструментальный жанр: духовые и ударные инструменты"; break;
                case VmesteSila.insrumZanrFortepiano: result = "Инструментальный жанр: фортепиано"; break;
                case VmesteSila.insrumZanrGitara: result = "Инструментальный жанр: гитара"; break;
                case VmesteSila.insrumZanrNarodnieInstrum: result = "Инструментальный жанр: народные инструменты"; break;
                case VmesteSila.insrumZanrSintezator: result = "Инструментальный жанр: синтезатор"; break;
                case VmesteSila.insrumZanrStrunnoSmichkovieInstrumenti: result = "Инструментальный жанр: струнно - смычковые инструменты"; break;
                case VmesteSila.insrumZanrOstalnieGanri: result = "Инструментальный жанр: остальные жанры"; break;
                    
                case VmesteSila.theatre: result = "Театральный жанр"; break;
                case VmesteSila.theatreSpektakl: result = "Театральный жанр: Спекталь"; break;
                case VmesteSila.theatreScenka: result = "Театральный жанр: Сценка"; break;
                case VmesteSila.theatreLiteraturnoMusikalnaya: result = "Театральный жанр: Литературно-музыкальная композиция"; break;
                case VmesteSila.theatreDrama: result = "Театральный жанр: Драматический спектакль"; break;

                case VmesteSila.masterMakeup: result = "Мастер макияжа"; break;
                case VmesteSila.masterMakeupDay: result = "Мастер макияжа: Дневной макияж"; break;
                case VmesteSila.masterMakeupNight: result = "Мастер макияжа: Вечерний макияж"; break;
                case VmesteSila.masterMakeupStsena: result = "Мастер макияжа: Сценический макияж"; break;
                case VmesteSila.masterMakeupFantasy: result = "Мастер макияжа: Фантазийный макияж"; break;

                case VmesteSila.masterShair: result = "Парикмахерское искусство"; break;
                case VmesteSila.masterShairPletenie: result = "Парикмахерское искусство: Плетение"; break;
                case VmesteSila.masterShairDay: result = "Парикмахерское искусство: Дневная причёска"; break;
                case VmesteSila.masterShairNight: result = "Парикмахерское искусство: Вечерняя причёска"; break;
                case VmesteSila.masterShairFantasy: result = "Парикмахерское искусство: Фантазийная причёска"; break;

                case VmesteSila.NO: result = "НЕТ"; break;
                case VmesteSila.self: result = "VmesteSila"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetVmesteSilaCode(VmesteSila fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями фестиваля "Вместе мы сильнее"</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetVmesteSilaCode(VmesteSila fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case VmesteSila.hudSlovo: result = "hudslovo"; break;
                //case VmesteSila.hudSlovoPoeziya: result = "hudSlovoPoeziya"; break;
                //case VmesteSila.hudSlovoProza: result = "hudSlovoProza"; break;

                case VmesteSila.horeographia: result = "horeographia"; break;
                case VmesteSila.vokal: result = "vokal"; break;
                case VmesteSila.instrumental: result = "instrumental"; break;
                case VmesteSila.theatre: result = "theatre"; break;
                case VmesteSila.masterMakeup: result = "masterMakeup"; break;
                case VmesteSila.masterShair: result = "masterShair"; break;
                case VmesteSila.NO: result = "NO"; break;
                case VmesteSila.self: result = "VmesteSila"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetVmesteSilaValueFromCode(string code)

        /// <summary>Метод возвращает название номинации фестиваля "Вместе мы сильнее"</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetVmesteSilaValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "hudslovo": result = "Художественное слово"; break;
                //case "hudSlovoPoeziya": result = "Художественное слово: Поэзия"; break;
                //case "hudSlovoPeoza": result = "Художественное слово: Проза"; break;
                case "horeographia": result = "Хореография"; break;
                case "vokal": result = "Вокал"; break;
                case "instrumental": result = "Инструментальный жанр"; break;
                case "theatre": result = "Театральный жанр"; break;
                case "masterMakeup": result = "Мастер макияжа"; break;
                case "masterShair": result = "Парикмахерское искусство"; break;
 
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetVmesteSilaCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации фестиваля "Вместе мы сильнее" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetVmesteSilaCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Художественное слово": result = "hudslovo"; break;
                //case "Художественное слово: Поэзия": result = "hudSlovoPoeziya"; break;
                //case "Художественное слово: Проза": result = "hudSlovoProza"; break;
                case "Хореография": result = "horeographia"; break;
                case "Вокал": result = "vokal"; break;
                case "Инструментальный жанр": result = "instrumental"; break;
                case "Театральный жанр": result = "theatre"; break;
                case "Мастер макияжа": result = "masterMakeup"; break;
                case "Парикмахерское искусство": result = "masterShair"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetClothesValue(Clothes fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса «Индустрия моды» и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetClothesValue(Clothes fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Clothes.uniyKuturie: result = "Юный кутюрье"; break;
                case Clothes.uniyKuturieTkan: result = "Юный кутюрье. Ткань"; break;
                case Clothes.uniyKuturieNetradicMaterial: result = "Юный кутюрье. Нетрадиционные материалы"; break;
                case Clothes.uniyKuturieFashion: result = "Юный кутюрье. Fashion иллюстрация"; break;
                case Clothes.uniyKuturieTechRisunok: result = "Юный кутюрье. Технический рисунок"; break;
                case Clothes.uniyKuturieFoodArt: result = "Юный кутюрье. Фуд-арт и fashion-дизайн"; break;
                case Clothes.uniyKuturieOgorod: result = "Юный кутюрье. Модный огород на подоконнике"; break;
                case Clothes.uniyKuturieBeauty: result = "Юный кутюрье. Beauty-меню: блюда с микрозеленью"; break;

                case Clothes.chudoLoskutki: result = "Чудесные лоскутки"; break;
                case Clothes.chudoLoskutkiIgrushkiKukliTvorRisunok: result = "Чудесные лоскутки: Добрые игрушки и куклы. Творческий рисунок"; break;
                case Clothes.chudoLoskutkiIgrushkiKukli: result = "Чудесные лоскутки: Добрые игрушки и куклы"; break;

                case Clothes.eskiziModelier: result = "Эскизы для модельера"; break;
                case Clothes.eskiziModelierTvorRisunok: result = "Эскизы для модельера. Творческий рисунок"; break;
                case Clothes.eskiziModelierFashion: result = "Эскизы для модельера. Fashion иллюстрация"; break;
                case Clothes.eskiziModelierTechRisunok: result = "Эскизы для модельера. Технический рисунок"; break;
                
                case Clothes.sedobnayaModa: result = "Съедобная мода"; break;
                case Clothes.sedobnayaModaFoodArt: result = "Съедобная мода. Фуд-арт и fashion-дизайн"; break;
                case Clothes.sedobnayaModaOgorod: result = "Съедобная мода. Модный огород на подоконнике"; break;
                case Clothes.sedobnayaModaBeauty: result = "Съедобная мода. Beauty-меню: блюда с микрозеленью"; break;

                case Clothes.tmcollectpokaz: result = "Театр моды: Коллективный показ"; break;
                case Clothes.tmavtorcollect: result = "Театр моды: Авторская коллекция"; break;
                case Clothes.tmindividpokaz: result = "Театр моды: Индивидуальный показ"; break;
                case Clothes.tmnetradicmaterial: result = "Театр моды: Коллекция из нетрадиционных материалов"; break;
                case Clothes.tmissledovproject: result = "Театр моды: Исследовательский проект"; break;

                case Clothes.NO: result = "НЕТ"; break;
                case Clothes.self: result = "Clothes"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetClothesCode(Clothes fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса «Индустрия моды»</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetClothesCode(Clothes fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Clothes.uniyKuturie: result = "uniyKuturie"; break;
                case Clothes.uniyKuturieTkan: result = "uniyKuturieTkan"; break;
                case Clothes.uniyKuturieNetradicMaterial: result = "uniyKuturieNetradicMaterial"; break;
                case Clothes.uniyKuturieFashion: result = "uniyKuturieFashion"; break;
                case Clothes.uniyKuturieTechRisunok: result = "uniyKuturieTechRisunok"; break;
                case Clothes.uniyKuturieFoodArt: result = "uniyKuturieFoodArt"; break;
                case Clothes.uniyKuturieOgorod: result = "uniyKuturieOgorod"; break;
                case Clothes.uniyKuturieBeauty: result = "uniyKuturieBeauty"; break;

                case Clothes.chudoLoskutki: result = "chudoLoskutki"; break;
                case Clothes.chudoLoskutkiIgrushkiKukli: result = "chudoLoskutkiIgrushkiKukli"; break;
                case Clothes.chudoLoskutkiIgrushkiKukliTvorRisunok: result = "chudoLoskutkiIgrushkiKukliTvorRisunok"; break;
  
                case Clothes.eskiziModelier: result = "eskiziModelier"; break;
                case Clothes.eskiziModelierTvorRisunok: result = "eskiziModelierTvorRisunok"; break;
                case Clothes.eskiziModelierFashion: result = "eskiziModelierFashion"; break;
                case Clothes.eskiziModelierTechRisunok: result = "eskiziModelierTechRisunok"; break;

                case Clothes.sedobnayaModa: result = "sedobnayaModa"; break;
                case Clothes.sedobnayaModaFoodArt: result = "sedobnayaModaFoodArt"; break;
                case Clothes.sedobnayaModaOgorod: result = "sedobnayaModaOgorod"; break;
                case Clothes.sedobnayaModaBeauty: result = "sedobnayaModaBeauty"; break;

                case Clothes.tmcollectpokaz: result = "tmcollectpokaz"; break;
                case Clothes.tmavtorcollect: result = "tmavtorcollect"; break;
                case Clothes.tmindividpokaz: result = "tmindividpokaz"; break;
                case Clothes.tmnetradicmaterial: result = "tmnetradicmaterial"; break;
                case Clothes.tmissledovproject: result = "tmissledovproject"; break;

                case Clothes.NO: result = "NO"; break;
                case Clothes.self: result = "Clothes"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetClothesValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса «Индустрия моды» по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetClothesValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "uniyKuturie": result = "Юный кутюрье"; break;
                case "uniyKuturieTkan": result = "Юный кутюрье. Ткань"; break;
                case "uniyKuturieNetradicMaterial": result = "Юный кутюрье. Нетрадиционные материалы"; break;
                case "uniyKuturieFashion": result = "Юный кутюрье. Fashion иллюстрация"; break;
                case "uniyKuturieTechRisunok": result = "Юный кутюрье. Технический рисунок"; break;
                case "uniyKuturieFoodArt": result = "Юный кутюрье. Фуд-арт и fashion-дизайн"; break;
                case "uniyKuturieOgorod": result = "Юный кутюрье. Модный огород на подоконнике"; break;
                case "uniyKuturieBeauty": result = "Юный кутюрье. Beauty-меню: блюда с микрозеленью"; break;

                case "chudoLoskutki": result = "Чудесные лоскутки"; break;
                case "chudoLoskutkiIgrushkiKukli": result = "Чудесные лоскутки: Добрые игрушки и куклы"; break;
                case "chudoLoskutkiIgrushkiKukliTvorRisunok": result = "Чудесные лоскутки: Добрые игрушки и куклы. Творческий рисунок"; break;

                case "eskiziModelier": result = "Эскизы для модельера"; break;
                case "eskiziModelierTvorRisunok": result = "Эскизы для модельера. Творческий рисунок"; break;
                case "eskiziModelierFashion": result = "Эскизы для модельера. Fashion иллюстрация"; break;
                case "eskiziModelierTechRisunok": result = "Эскизы для модельера. Технический рисунок"; break;

                case "sedobnayaModa": result = "Съедобная мода"; break;
                case "sedobnayaModaFoodArt": result = "Съедобная мода. Фуд-арт и fashion-дизайн"; break;
                case "sedobnayaModaOgorod": result = "Съедобная мода. Модный огород на подоконнике"; break;
                case "sedobnayaModaBeauty": result = "Съедобная мода. Beauty-меню: блюда с микрозеленью"; break;

                case "tmcollectpokaz": result = "Театр моды: Коллективный показ"; break;
                case "tmavtorcollect": result = "Театр моды: Авторская коллекция"; break;
                case "tmindividpokaz": result = "Театр моды: Индивидуальный показ"; break;
                case "tmnetradicmaterial": result = "Театр моды: Коллекция из нетрадиционных материалов"; break;
                case "tmissledovproject": result = "Театр моды: Исследовательский проект"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetClothesCodeFromValue(string code)

        /// <summary>Метод возвращает код номинации конкурса «Индустрия моды» по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetClothesCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Юный кутюрье": result = "uniyKuturie"; break;
                case "Юный кутюрье. Ткань": result = "uniyKuturieTkan"; break;
                case "Юный кутюрье. Нетрадиционные материалы": result = "uniyKuturieNetradicMaterial"; break;
                case "Юный кутюрье. Fashion иллюстрация": result = "uniyKuturieFashion"; break;
                case "Юный кутюрье. Технический рисунок": result = "uniyKuturieTechRisunok"; break;
                case "Юный кутюрье. Фуд-арт и fashion-дизайн": result = "uniyKuturieFoodArt"; break;
                case "Юный кутюрье. Модный огород на подоконнике": result = "uniyKuturieOgorod"; break;
                case "Юный кутюрье. Beauty-меню: блюда с микрозеленью": result = "uniyKuturieBeauty"; break;

                case "Чудесные лоскутки": result = "chudoLoskutki"; break;
                case "Чудесные лоскутки: Добрые игрушки и куклы": result = "chudoLoskutkiIgrushkiKukli"; break;
                case "Чудесные лоскутки: Добрые игрушки и куклы. Творческий рисунок":result = "chudoLoskutkiIgrushkiKukliTvorRisunok"; break;

                case "Эскизы для модельера": result = "eskiziModelier"; break;
                case "Эскизы для модельера. Творческий рисунок": result = "eskiziModelierTvorRisunok"; break;
                case "Эскизы для модельера. Fashion иллюстрация": result = "eskiziModelierFashion"; break;
                case "Эскизы для модельера. Технический рисунок": result = "eskiziModelierTechRisunok"; break;

                case "Съедобная мода": result = "sedobnayaModa"; break;
                case "Съедобная мода. Фуд-арт и fashion-дизайн": result = "sedobnayaModaFoodArt"; break;
                case "Съедобная мода. Модный огород на подоконнике": result = "sedobnayaModaOgorod"; break;
                case "Съедобная мода. Beauty-меню: блюда с микрозеленью": result = "sedobnayaModaBeauty"; break;

                case "Театр моды: Коллективный показ": result = "tmcollectpokaz"; break;
                case "Театр моды: Авторская коллекция": result = "tmavtorcollect"; break;
                case "Театр моды: Индивидуальный показ": result = "tmindividpokaz"; break;
                case "Театр моды: Коллекция из нетрадиционных материалов": result = "tmnetradicmaterial"; break;
                case "Театр моды: Исследовательский проект": result = "tmissledovproject"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetMultimediaValue(Multimedia fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее» и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetMultimediaValue(Multimedia fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Multimedia.uniygeroi: result = "Юный герой - пример для подражания"; break;
                case Multimedia.uniyzashitnik: result = "Юные защитники 1-й и 2-й обороны Севастополя"; break;
                case Multimedia.characteristica: result = "Характеристика личности Нахимова, причины её становления и формирования"; break;
                case Multimedia.prichinietapi: result = "Причины и этапы становления личности Нахимова как флотоводца и адмирала"; break;
                case Multimedia.rolvistorii: result = "Роль Нахимова в мировой и отечественной истории"; break;

                //2022
                case Multimedia.podandreevskimflagom: result = "Под Адреевским флагом..."; break;
                case Multimedia.pomoryampovolnam: result = "По морям, по волнам..."; break;
                case Multimedia.korablimorykimore: result = "Корабли, моряки и море..."; break;
                case Multimedia.kraskimorya: result = "Краски моря"; break;
                case Multimedia.krossvord: result = "Кроссворд"; break;
                case Multimedia.chernomorskomurossiiposvyashaetsa: result = "Черноморскому России посвящается..."; break;
                case Multimedia.metodicheskierazrabotki: result = "Методические разработки"; break;

                //2023
                case Multimedia.morepobedkolibelsmelchakov: result = "Море побед - колыбель смельчаков"; break;
                case Multimedia.netzapyatihtolkotochki: result = "Нет запятых, только чёрные точки"; break;
                case Multimedia.vashipodvigineumrut: result = "Ваши подвиги в легендах не умрут"; break;
                case Multimedia.sevastopol44: result = "Севастополь. Год сорок четвёртый. Вместо улиц камни да зола"; break;
                case Multimedia.nadevaemmitelnyashku: result = "Надеваем мы тельняшку!"; break;
                case Multimedia.klyatvudaemsevastopolvernem: result = "Клятву даём: Севастополь вернём!"; break;
                case Multimedia.hranitalbomsemeinipamyat: result = "Хранит альбом семейный память"; break;
                case Multimedia.ihpamaytuzivushiipoklonis: result = "Их памяти, живущий, поклонись"; break;
                case Multimedia.multimediinieizdaniya: result = "Мультимедийные издания"; break;

                //2024
                case Multimedia.yarisuupobedy: result = "Я рисую Победу"; break;
                case Multimedia.spesneirpobede: result = "С песней к Победе"; break;
                case Multimedia.geroyamotserdca: result = "Героям - от сердца"; break;
                case Multimedia.plechomkplechu: result = "В одном строю, плечом к плечу мы шли к Победе!"; break;
                case Multimedia.pamyatsilneevremeni: result = "Память сильнее времени"; break;

                case Multimedia.NO: result = "НЕТ"; break;
                case Multimedia.self: result = "Multimedia"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMultimediaCode(Multimedia fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями для Конкурса по Робототехнике и моделированию 3D ручкой «Шаг в будущее»</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetMultimediaCode(Multimedia fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Multimedia.uniygeroi: result = "uniygeroi"; break;
                case Multimedia.uniyzashitnik: result = "uniyzashitnik"; break;
                case Multimedia.characteristica: result = "characteristica"; break;
                case Multimedia.prichinietapi: result = "prichinietapi"; break;
                case Multimedia.rolvistorii: result = "rolvistorii"; break;

                //2022
                case Multimedia.podandreevskimflagom: result = "podandreevskimflagom"; break;
                case Multimedia.pomoryampovolnam: result = "pomoryampovolnam"; break;
                case Multimedia.korablimorykimore: result = "korablimorykimore"; break;
                case Multimedia.kraskimorya: result = "kraskimorya"; break;
                case Multimedia.krossvord: result = "krossvord"; break;
                case Multimedia.chernomorskomurossiiposvyashaetsa: result = "chernomorskomurossiiposvyashaetsa"; break;
                case Multimedia.metodicheskierazrabotki: result = "metodicheskierazrabotki"; break;

                //2023
                case Multimedia.morepobedkolibelsmelchakov: result = "morepobedkolibelsmelchakov"; break;
                case Multimedia.netzapyatihtolkotochki: result = "netzapyatihtolkotochki"; break;
                case Multimedia.vashipodvigineumrut: result = "vashipodvigineumrut"; break;
                case Multimedia.sevastopol44: result = "sevastopol44"; break;
                case Multimedia.nadevaemmitelnyashku: result = "nadevaemmitelnyashku"; break;
                case Multimedia.klyatvudaemsevastopolvernem: result = "klyatvudaemsevastopolvernem"; break;
                case Multimedia.hranitalbomsemeinipamyat: result = "hranitalbomsemeinipamyat"; break;
                case Multimedia.ihpamaytuzivushiipoklonis: result = "ihpamaytuzivushiipoklonis"; break;
                case Multimedia.multimediinieizdaniya: result = "multimediinieizdaniya"; break;

                //2024
                case Multimedia.yarisuupobedy: result = "yarisuupobedy"; break;
                case Multimedia.spesneirpobede: result = "spesneirpobede"; break;
                case Multimedia.geroyamotserdca: result = "geroyamotserdca"; break;
                case Multimedia.plechomkplechu: result = "plechomkplechu"; break;
                case Multimedia.pamyatsilneevremeni: result = "pamyatsilneevremeni"; break;

                case Multimedia.NO: result = "NO"; break;
                case Multimedia.self: result = "Multimedia"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMultimediaValueFromCode(string code)

        /// <summary>Метод возвращает название номинации Конкурса мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!» по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetMultimediaValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "uniygeroi": result = "Юный герой - пример для подражания"; break;
                case "uniyzashitnik": result = "Юные защитники 1-й и 2-й обороны Севастополя"; break;
                case "characteristica": result = "Характеристика личности Нахимова, причины её становления и формирования"; break;
                case "prichinietapi": result = "Причины и этапы становления личности Нахимова как флотоводца и адмирала"; break;
                case "rolvistorii": result = "Роль Нахимова в мировой и отечественной истории"; break;

                //2022
                case "podandreevskimflagom": result = "Под Адреевским флагом..."; break;
                case "pomoryampovolnam": result = "По морям, по волнам..."; break;
                case "korablimorykimore": result = "Корабли, моряки и море..."; break;
                case "chernomorskomurossiiposvyashaetsa": result = "Черноморскому России посвящается..."; break;
                case "kraskimorya": result = "Краски моря"; break;
                case "krossvord": result = "Кроссворд"; break;
                case "metodicheskierazrabotki": result = "Методические разработки"; break;
                
                //2023
                case "morepobedkolibelsmelchakov": result = "Море побед - колыбель смельчаков"; break;
                case "netzapyatihtolkotochki": result = "Нет запятых, только чёрные точки"; break;
                case "vashipodvigineumrut": result = "Ваши подвиги в легендах не умрут"; break;
                case "sevastopol44": result = "Севастополь. Год сорок четвёртый. Вместо улиц камни да зола"; break;
                case "nadevaemmitelnyashku": result = "Надеваем мы тельняшку!"; break;
                case "klyatvudaemsevastopolvernem": result = "Клятву даём: Севастополь вернём!"; break;
                case "hranitalbomsemeinipamyat": result = "Хранит альбом семейный память"; break;
                case "ihpamaytuzivushiipoklonis": result = "Их памяти, живущий, поклонись"; break;
                case "multimediinieizdaniya": result = "Мультимедийные издания"; break;

                //2024
                case "yarisuupobedy": result = "Я рисую Победу"; break;
                case "spesneirpobede": result = "С песней к Победе"; break;
                case "geroyamotserdca": result = "Героям - от сердца"; break;
                case "plechomkplechu": result = "В одном строю, плечом к плечу мы шли к Победе!"; break;
                case "pamyatsilneevremeni": result = "Память сильнее времени"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMultimediaCodeFromValue(string code)

        /// <summary>Метод возвращает код номинации Конкурса мультимедийных проектов «Герой войны, достойный Славы, любимый город Севастополь!»</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetMultimediaCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Юный герой - пример для подражания": result = "uniygeroi"; break;
                case "Юные защитники 1-й и 2-й обороны Севастополя": result = "uniyzashitnik"; break;
                case "Характеристика личности Нахимова, причины её становления и формирования": result = "characteristica"; break;
                case "Причины и этапы становления личности Нахимова как флотоводца и адмирала": result = "prichinietapi"; break;
                case "Роль Нахимова в мировой и отечественной истории": result = "rolvistorii"; break;

                //2022
                case "Под Адреевским флагом...": result = "podandreevskimflagom"; break;
                case "По морям, по волнам...": result = "pomoryampovolnam"; break;
                case "Корабли, моряки и море...": result = "korablimorykimore"; break;
                case "Черноморскому России посвящается...": result = "chernomorskomurossiiposvyashaetsa"; break;
                case "Краски моря": result = "kraskimorya"; break;
                case "Кроссворд": result = "krossvord"; break;
                case "Методические разработки": result = "metodicheskierazrabotki"; break;

                //2023
                case "Море побед - колыбель смельчаков": result = "morepobedkolibelsmelchakov"; break;
                case "Нет запятых, только чёрные точки": result = "netzapyatihtolkotochki"; break;
                case "Ваши подвиги в легендах не умрут": result = "vashipodvigineumrut"; break;
                case "Севастополь. Год сорок четвёртый. Вместо улиц камни да зола": result = "sevastopol44"; break;
                case "Надеваем мы тельняшку!": result = "nadevaemmitelnyashku"; break;
                case "Клятву даём: Севастополь вернём!": result = "klyatvudaemsevastopolvernem"; break;
                case "Хранит альбом семейный память": result = "hranitalbomsemeinipamyat"; break;
                case "Их памяти, живущий, поклонись": result = "ihpamaytuzivushiipoklonis"; break;
                case "Мультимедийные издания": result = "multimediinieizdaniya"; break;

                //2024
                case "Я рисую Победу": result = "yarisuupobedy"; break;
                case "С песней к Победе": result = "spesneirpobede"; break;
                case "Героям - от сердца": result = "geroyamotserdca"; break;
                case "В одном строю, плечом к плечу мы шли к Победе!": result = "plechomkplechu"; break;
                case "Память сильнее времени": result = "pamyatsilneevremeni"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetKorablikValue(Korablik fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса "Кораблик детства" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKorablikValue(Korablik fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Korablik.hudSlovo: result = "Художественное слово"; break;

                case Korablik.horeographia: result = "Хореография"; break;
                case Korablik.horeographiaBalniyTanets: result = "Хореография: бальный танец"; break;
                case Korablik.horeographiaClassichTanets: result = "Хореография: классический танец"; break;
                case Korablik.horeographiaEstradTanets: result = "Хореография: эстрадный танец"; break;
                case Korablik.horeographiaNarodTanets: result = "Хореография: народный танец"; break;

                case Korablik.vokal: result = "Вокал"; break;
                case Korablik.vokalSolo: result = "Вокал: соло"; break;
                case Korablik.vokalMalieFormi: result = "Вокал: малые формы (дуэт и трио)"; break;
                case Korablik.vokalAnsambli: result = "Вокал: ансамбли"; break;

                case Korablik.NO: result = "НЕТ"; break;
                case Korablik.self: result = "Korablik"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKorablikCode(Korablik fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса "Кораблик детства"</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKorablikCode(Korablik fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Korablik.hudSlovo: result = "hudslovo"; break;
                case Korablik.horeographia: result = "horeographia"; break;
                case Korablik.vokal: result = "vokal"; break;
                case Korablik.NO: result = "NO"; break;
                case Korablik.self: result = "Korablik"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKorablikValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса "Кораблик детства"</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetKorablikValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "hudslovo": result = "Художественное слово"; break;
                case "horeographia": result = "Хореография"; break;
                case "vokal": result = "Вокал"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKorablikCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса "Кораблик детства" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetKorablikCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Художественное слово": result = "hudslovo"; break;
                case "Хореография": result = "horeographia"; break;
                case "Вокал": result = "vokal"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetCrimrouteValue(Crimroute fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса проектных работ "Крымские маршруты" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetCrimrouteValue(Crimroute fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Crimroute.historyplace: result = "Исторический Крым"; break;
                case Crimroute.militaryhistoryplace: result = "Героический Крым"; break;
                case Crimroute.literaturehistoryplace: result = "Литературный Крым"; break;
                case Crimroute.NO: result = "НЕТ"; break;
                case Crimroute.self: result = "Crimroute"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetCrimrouteCode(Crimroute fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса проектных работ "Крымские маршруты" </summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetCrimrouteCode(Crimroute fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Crimroute.historyplace: result = "historyplace"; break;
                case Crimroute.militaryhistoryplace: result = "militaryhistoryplace"; break;
                case Crimroute.literaturehistoryplace: result = "literaturehistoryplace"; break;
                case Crimroute.NO: result = "NO"; break;
                case Crimroute.self: result = "Crimroute"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetCrimrouteValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса проектных работ "Крымские маршруты" </summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetCrimrouteValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "historyplace": result = "Исторический Крым"; break;
                case "militaryhistoryplace": result = "Героический Крым"; break;
                case "literaturehistoryplace": result = "Литературный Крым"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetCrimrouteCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса проектных работ "Крымские маршруты" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetCrimrouteCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Исторический Крым": result = "historyplace"; break;
                case "Героический Крым": result = "militaryhistoryplace"; break;
                case "Литературный Крым": result = "literaturehistoryplace"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetMathbattleValue(Mathbattle fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса проектных работ "Математический батл" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetMathbattleValue(Mathbattle fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Mathbattle.battle: result = "Математический батл"; break;
                case Mathbattle.NO: result = "НЕТ"; break;
                case Mathbattle.self: result = "Mathbattle"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMathbattleCode(Mathbattle fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса проектных работ "Математический батл" </summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetMathbattleCode(Mathbattle fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Mathbattle.battle: result = "battle"; break;
                case Mathbattle.NO: result = "NO"; break;
                case Mathbattle.self: result = "Mathbattle"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMathbattleValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса проектных работ "Математический батл" </summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetMathbattleValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "battle": result = "Математический батл"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetMathbattleFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса проектных работ "Математический батл" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetMathbattleCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Математический батл": result = "battle"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetKosmosValue(Kosmos fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями конкурса проектных работ "Покоряя космос" и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKosmosValue(Kosmos fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Kosmos.kosmos: result = "Покоряя космос"; break;
                case Kosmos.NO: result = "НЕТ"; break;
                case Kosmos.self: result = "Kosmos"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKosmosCode(Kosmos fromEnum)

        /// <summary>Метод возвращает условное наименование номенации, соответствующей значению перечисления с номинациями конкурса проектных работ "Покоряя космос" </summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetKosmosCode(Kosmos fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Kosmos.kosmos: result = "kosmos"; break;
                case Kosmos.NO: result = "NO"; break;
                case Kosmos.self: result = "Kosmos"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKosmosValueFromCode(string code)

        /// <summary>Метод возвращает название номинации конкурса проектных работ "Покоряя космос" </summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetKosmosValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "kosmos": result = "Покоряя космос"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetKosmosFromValue(string value)

        /// <summary>Метод возвращает код номинации конкурса проектных работ "Покоряя космос" по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetKosmosCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Покоряя космос": result = "kosmos"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetScienceValue(Science fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями Конкурса научных работ "В моей лаборатории вот что... " и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetScienceValue(Science fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Science.ekologia_ochno: result = "Экология (очно)"; break;
                case Science.ekologia_zaochno: result = "Экология (заочно)"; break;
                case Science.himiya_ochno: result = "Химия (очно)"; break;
                case Science.himiya_zaochno: result = "Химия (заочно)"; break;
                case Science.fizika_ochno: result = "Физика (очно)"; break;
                case Science.fizika_zaochno: result = "Физика (заочно)"; break;
                case Science.biologiya_ochno: result = "Биология (очно)"; break;
                case Science.biologiya_zaochno: result = "Биология (заочно)"; break;
                case Science.NO: result = "НЕТ"; break;
                case Science.self: result = "Science"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetScienceCode(Science fromEnum)

        /// <summary>Метод возвращает условное наименование номинации, соответствующей значению перечисления с номинациями Конкурса научных работ "В моей лаборатории вот что... "</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetScienceCode(Science fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Science.ekologia_ochno: result = "ekologia_ochno"; break;
                case Science.ekologia_zaochno: result = "ekologia_zaochno"; break;
                case Science.himiya_ochno: result = "himiya_ochno"; break;
                case Science.himiya_zaochno: result = "himiya_zaochno"; break;
                case Science.fizika_ochno: result = "fizika_ochno"; break;
                case Science.fizika_zaochno: result = "fizika_zaochno"; break;
                case Science.biologiya_ochno: result = "biologiya_ochno"; break;
                case Science.biologiya_zaochno: result = "biologiya_zaochno"; break;
                case Science.NO: result = "NO"; break;
                case Science.self: result = "Science"; break;

                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetScienceValueFromCode(string code)

        /// <summary>Метод возвращает название номинации Конкурса научных работ "В моей лаборатории вот что... " по её коду</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetScienceValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "ekologia_ochno": result = "Экология (очно)"; break;
                case "ekologia_zaochno": result = "Экология (заочно)"; break;
                case "himiya_ochno": result = "Химия (очно)"; break;
                case "himiya_zaochno": result = "Химия (заочно)"; break;
                case "fizika_ochno": result = "Физика (очно)"; break;
                case "fizika_zaochno": result = "Физика (заочно)"; break;
                case "biologiya_ochno": result = "Биология (очно)"; break;
                case "biologiya_zaochno": result = "Биология (заочно)"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetScienceCodeFromValue(string value)

        /// <summary>Метод возвращает код номинации Конкурса научных работ "В моей лаборатории вот что... " по её названию</summary>
        /// <param name="value">название номинации</param>
        /// <returns></returns>
        public static string GetScienceCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "Экология (очно)": result = "ekologia_ochno"; break;
                case "Экология (заочно)": result = "ekologia_zaochno"; break;
                case "Химия (очно)": result = "himiya_ochno"; break;
                case "Химия (заочно)": result = "himiya_zaochno"; break;
                case "Физика (очно)": result = "fizika_ochno"; break;
                case "Физика (заочно)": result = "fizika_zaochno"; break;
                case "Биология (очно)": result = "biologiya_ochno"; break;
                case "Биология (заочно)": result = "biologiya_zaochno"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetWritesValue(Writes fromEnum)

        /// <summary>Метод получает значение перечисления с правами доступа в консоль управления и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetWritesValue(Writes fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Writes.admin: result = "администратор"; break;
                case Writes.newsEditor: result = "редактор новостей"; break;
                case Writes.pagesEditor: result = "редактор страниц"; break;
                case Writes.editorPhoto: result = "редактор фотоконкурса: Фотография"; break;
                case Writes.editorPhotoIzo: result = "редактор фотоконкурса: ИЗО"; break;
                case Writes.editorPhotoCompGraphic: result = "редактор фотоконкурса: Компьютерная графика"; break;
                case Writes.editorDPI1: result = "редактор фотоконкурса: ДПИ (макраме..)"; break;
                case Writes.editorDPI2: result = "редактор фотоконкурса: ДПИ (керамика..)"; break;
                case Writes.editorLiterary: result = "редактор Литературного конкурса"; break;
                case Writes.editorTheatre: result = "редактор театрконкурса: Театральное искусство"; break;
                case Writes.editorTheatreHudSlovo: result = "редактор театрконкурса: Художественное слово"; break;
                case Writes.editorTheatreHoreo: result = "редактор театрконкурса: Хореография"; break;
                case Writes.editorTheatreVokal: result = "редактор театрконкурса: Вокал"; break;
                case Writes.editorTheatreInstrumZanr: result = "редактор театрконкурса: Инструментальный жанр"; break;
                case Writes.editorTheatreModa: result = "редактор театрконкурса: Театр моды"; break;
                case Writes.editorKultura: result = "редактор конкурса Открытый мир"; break;
                case Writes.editorSport: result = "редактор спорт: Простейшие единоборства"; break;
                case Writes.editorThekvo: result = "редактор спорт: Тхэквондо"; break;
                case Writes.editorBoks: result = "редактор спорт: Бокс"; break;
                case Writes.editorKungfu: result = "редактор спорт: Кунг-фу УИ"; break;
                case Writes.editorFootball: result = "редактор спорт: Футбол"; break;
                case Writes.editorBasketball: result = "редактор спорт: Баскетбол"; break;
                case Writes.editorVolleyball: result = "редактор спорт: Волейбол"; break;
                case Writes.editorPaintball: result = "редактор спорт: Тактический лазертаг"; break;
                case Writes.editorShahmaty: result = "редактор спорт: Шахматы"; break;
                case Writes.editorShashky: result = "редактор спорт: Шашки"; break;
                case Writes.editorToponim: result = "редактор топонимики: Топонимика Москвы и Севастополя"; break;
                case Writes.editorRobotech: result = "редактор робототехники: Шаг в будущее"; break;
                case Writes.editorVmesteSila: result = "редактор конкурса: Вместе мы сильнее"; break;
                case Writes.editorVmesteSilaMakeUp: result = "редактор конкурса: Вместе мы сильнее (макияж)"; break;
                case Writes.editorVmesteSilaShair: result = "редактор конкурса: Вместе мы сильнее (парикмахер)"; break;
                case Writes.editorClothes: result = "редактор конкурса: Индустрия моды"; break;
                case Writes.editorMultimedia: result = "редактор конкурса мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя"; break;
                case Writes.editorKorablik: result = "редактор конкурса: Кораблик детства"; break;
                case Writes.editorKorablikVokal: result = "редактор конкурса: Кораблик детства (вокал)"; break;
                case Writes.editorKorablikHoreo: result = "редактор конкурса: Кораблик детства (хореография)"; break;
                case Writes.editorCrimroute: result = "редактор конкурса: Крымские маршруты"; break;
                case Writes.editorMathbattle: result = "редактор конкурса: Математический батл"; break;
                case Writes.editorKosmos: result = "редактор квест-игры: Покоряя космос"; break;
                case Writes.editorScience: result = "редактор конкурса научных работ В моей лаборатории вот что..."; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetWritesCode(Writes fromEnum)

        /// <summary>Метод возвращает условное наименование прав доступа, соответствующей значению перечисления с с правами доступа в консоль управления</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetWritesCode(Writes fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case Writes.admin: result = "admin"; break;
                case Writes.newsEditor: result = "newsEditor"; break;
                case Writes.pagesEditor: result = "pagesEditor"; break;
                case Writes.editorPhoto: result = "editorPhoto"; break;
                case Writes.editorPhotoIzo: result = "editorPhotoIzo"; break;
                case Writes.editorPhotoCompGraphic: result = "editorPhotoCompGraphic"; break;
                case Writes.editorDPI1: result = "editorDPI1"; break;
                case Writes.editorDPI2: result = "editorDPI2"; break;
                case Writes.editorLiterary: result = "editorLiterary"; break;
                case Writes.editorTheatre: result = "editorTheatre"; break;
                case Writes.editorTheatreHudSlovo: result = "editorTheatreHudSlovo"; break;
                case Writes.editorTheatreHoreo: result = "editorTheatreHoreo"; break;
                case Writes.editorTheatreVokal: result = "editorTheatreVokal"; break;
                case Writes.editorTheatreInstrumZanr: result = "editorTheatreInstrumZanr"; break;
                case Writes.editorTheatreModa: result = "editorTheatreModa"; break;
                case Writes.editorKultura: result = "editorKultura"; break;
                case Writes.editorSport: result = "editorSport"; break;
                case Writes.editorThekvo: result = "editorThekvo"; break;
                case Writes.editorBoks: result = "editorBoks"; break;
                case Writes.editorKungfu: result = "editorKungfu"; break;
                case Writes.editorFootball: result = "editorFootball"; break;
                case Writes.editorBasketball: result = "editorBasketball"; break;
                case Writes.editorVolleyball: result = "editorVolleyball"; break;
                case Writes.editorPaintball: result = "editorPaintball"; break;
                case Writes.editorShahmaty: result = "editorShahmaty"; break;
                case Writes.editorShashky: result = "editorShashky"; break;
                case Writes.editorToponim: result = "editorToponim"; break;
                case Writes.editorRobotech: result = "editorRobotech"; break;
                case Writes.editorVmesteSila: result = "editorVmesteSila"; break;
                case Writes.editorVmesteSilaMakeUp: result = "editorVmesteSilaMakeUp"; break;
                case Writes.editorVmesteSilaShair: result = "editorVmesteSilaShair"; break;
                case Writes.editorClothes: result = "editorClothes"; break;
                case Writes.editorMultimedia: result = "editorMultimedia"; break;
                case Writes.editorKorablik: result = "editorKorablik"; break;
                case Writes.editorKorablikHoreo: result = "editorKorablikHoreo"; break;
                case Writes.editorKorablikVokal: result = "editorKorablikVokal"; break;
                case Writes.editorCrimroute: result = "editorCrimroute"; break;
                case Writes.editorMathbattle: result = "editorMathbattle"; break;
                case Writes.editorKosmos: result = "editorKosmos"; break;
                case Writes.editorScience: result = "editorScience"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetWritesValueFromCode(string code)

        /// <summary>Метод возвращает название прав доступа в консоль управления по их условному наименованию (коду)</summary>
        /// <param name="code">код номинации</param>
        /// <returns></returns>
        public static string GetWritesValueFromCode(string code)
        {
            string result = "";

            switch (code)
            {
                case "admin": result = "администратор"; break;
                case "newsEditor": result = "редактор новостей"; break;
                case "pagesEditor": result = "редактор страниц"; break;
                case "editorPhoto": result = "редактор фотоконкурса: Фотография"; break;
                case "editorPhotoIzo": result = "редактор фотоконкурса: ИЗО"; break;
                case "editorPhotoCompGraphic": result = "редактор фотоконкурса: Компьютерная графика"; break;
                case "editorDPI1": result = "редактор фотоконкурса: ДПИ (макраме..)"; break;
                case "editorDPI2": result = "редактор фотоконкурса: ДПИ (керамика..)"; break;
                case "editorLiterary": result = "редактор Литературного конкурса"; break;
                case "editorTheatre": result = "редактор театрконкурса: Театральное искусство"; break;
                case "editorTheatreHudSlovo": result = "редактор театрконкурса: Художественное слово"; break;
                case "editorTheatreHoreo": result = "редактор театрконкурса: Хореография"; break;
                case "editorTheatreVokal": result = "редактор театрконкурса: Вокал"; break;
                case "editorTheatreInstrumZanr": result = "редактор театрконкурса: Инструментальный жанр"; break;
                case "editorTheatreModa": result = "редактор театрконкурса: Театр моды"; break;
                case "editorKultura": result = "редактор конкурса Открытый мир"; break;
                case "editorSport": result = "редактор спорт: Простейшие единоборства"; break;
                case "editorThekvo": result = "редактор спорт: Тхэквондо"; break;
                case "editorBoks": result = "редактор спорт: Бокс"; break;
                case "editorKungfu": result = "редактор спорт: Кунг-фу УИ"; break;
                case "editorFootball": result = "редактор спорт: Футбол"; break;
                case "editorBasketball": result = "редактор спорт: Баскетбол"; break;
                case "editorVolleyball": result = "редактор спорт: Волейбол"; break;
                case "editorPaintball": result = "редактор спорт: Тактический лазертаг"; break;
                case "editorShahmaty": result = "редактор спорт: Шахматы"; break;
                case "editorShashky": result = "редактор спорт: Шашки"; break;
                case "editorToponim": result = "редактор топонимики: Топонимика Москвы и Севастополя"; break;
                case "editorRobotech": result = "редактор робототехники: Шаг в будущее"; break;
                case "editorVmesteSila": result = "редактор конкурса: Вместе мы сильнее"; break;
                case "editorVmesteSilaMakeUp": result = "редактор конкурса: Вместе мы сильнее (макияж)"; break;
                case "editorVmesteSilaShair": result = "редактор конкурса: Вместе мы сильнее (парикмахер)"; break;
                case "editorClothes": result = "редактор конкурса: Индустрия моды"; break;
                case "editorMultimedia": result = "редактор конкурса мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя"; break;
                case "editorKorablik": result = "редактор конкурса: Кораблик детства"; break;
                case "editorKorablikVokal": result = "редактор конкурса: Кораблик детства (вокал)"; break;
                case "editorKorablikHoreo": result = "редактор конкурса: Кораблик детства (хореография)"; break;
                case "editorCrimroute": result = "редактор конкурса: Крымские маршруты"; break;
                case "editorMathbattle": result = "редактор конкурса: Математический батл"; break;
                case "editorKosmos": result = "редактор квест-игры: Покоряя космос"; break;
                case "editorScience": result = "редактор конкурса научных работ В моей лаборатории вот что..."; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetWritesCodeFromValue(string value)

        /// <summary>Метод возвращает код прав доступа по их названию</summary>
        /// <param name="value">название прав</param>
        /// <returns></returns>
        public static string GetWritesCodeFromValue(string value)
        {
            string result = "";

            switch (value)
            {
                case "администратор": result = "admin"; break;
                case "редактор новостей": result = "newsEditor"; break;
                case "редактор страниц": result = "pagesEditor"; break;
                case "редактор фотоконкурса: Фотография": result = "editorPhoto"; break;
                case "редактор фотоконкурса: ИЗО": result = "editorPhotoIzo"; break;
                case "редактор фотоконкурса: Компьютерная графика": result = "editorPhotoCompGraphic"; break;
                case "редактор фотоконкурса: ДПИ (макраме..)": result = "editorDPI1"; break;
                case "редактор фотоконкурса: ДПИ (керамика..)": result = "editorDPI2"; break;
                case "редактор Литературного конкурса": result = "editorLiterary"; break;
                case "редактор театрконкурса: Театральное искусство": result = "editorTheatre"; break;
                case "редактор театрконкурса: Художественное слово": result = "editorTheatreHudSlovo"; break;
                case "редактор театрконкурса: Хореография": result = "editorTheatreHoreo"; break;
                case "редактор театрконкурса: Вокал": result = "editorTheatreVokal"; break;
                case "редактор театрконкурса: Инструментальный жанр": result = "editorTheatreInstrumZanr"; break;
                case "редактор театрконкурса: Театр моды": result = "editorTheatreModa"; break;
                case "редактор конкурса Открытый мир": result = "editorKultura"; break;
                case "редактор спорт: Простейшие единоборства": result = "editorSport"; break;
                case "редактор спорт: Бокс": result = "editorBoks"; break;
                case "редактор спорт: Кунг-фу УИ": result = "editorKungfu"; break;
                case "редактор спорт: Тхэквондо": result = "editorThekvo"; break;
                case "редактор спорт: Футбол": result = "editorFootball"; break;
                case "редактор спорт: Баскетбол": result = "editorBasketball"; break;
                case "редактор спорт: Волейбол": result = "editorVolleyball"; break;
                case "редактор спорт: Тактический лазертаг": result = "editorPaintball"; break;
                case "редактор спорт: Шахматы": result = "editorShahmaty"; break;
                case "редактор спорт: Шашки": result = "editorShashky"; break;
                case "редактор топонимики: Топонимика Москвы и Севастополя": result = "editorToponim"; break;
                case "редактор робототехники: Шаг в будущее": result = "editorRobotech"; break;
                case "редактор конкурса: Вместе мы сильнее": result = "editorVmesteSila"; break;
                case "редактор конкурса: Вместе мы сильнее (макияж)": result = "editorVmesteSilaMakeUp"; break;
                case "редактор конкурса: Вместе мы сильнее (парикмахер)": result = "editorVmesteSilaShair"; break;
                case "редактор конкурса: Индустрия моды": result = "editorClothes"; break;
                case "редактор конкурса мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя": result = "editorMultimedia"; break;
                case "редактор конкурса: Кораблик детства": result = "editorKorablik"; break;
                case "редактор конкурса: Кораблик детства (вокал)": result = "editorKorablikVokal"; break;
                case "редактор конкурса: Кораблик детства (хореография)": result = "editorKorablikHoreo"; break;
                case "редактор конкурса: Крымские маршруты": result = "editorCrimroute"; break;
                case "редактор конкурса: Математический батл": result = "editorMathbattle"; break;
                case "редактор квест-игры: Покоряя космос": result = "editorKosmos"; break;
                case "редактор конкурса научных работ В моей лаборатории вот что...": result = "editorScience"; break;
                default: break;
            }

            return result;
        }

        #endregion
        #region Метод GetWritesCodeFromValue(string value)

        /// <summary>Метод возвращает код прав доступа по их названию</summary>
        /// <param name="value">название прав</param>
        /// <returns></returns>
        public static Writes GetWritesEnumCodeFromValue(string value)
        {
            Writes result = Writes.none;

            switch (value)
            {
                case "администратор": result = Writes.admin; break;
                case "редактор новостей": result = Writes.newsEditor; break;
                case "редактор страниц": result = Writes.pagesEditor; break;
                case "редактор фотоконкурса: Фотография": result = Writes.editorPhoto; break;
                case "редактор фотоконкурса: ИЗО": result = Writes.editorPhotoIzo; break;
                case "редактор фотоконкурса: Компьютерная графика": result = Writes.editorPhotoCompGraphic; break;
                case "редактор фотоконкурса: ДПИ (макраме..)": result = Writes.editorDPI1; break;
                case "редактор фотоконкурса: ДПИ (керамика..)": result = Writes.editorDPI2; break;
                case "редактор Литературного конкурса": result = Writes.editorLiterary; break;
                case "редактор театрконкурса: Театральное искусство": result = Writes.editorTheatre; break;
                case "редактор театрконкурса: Художественное слово": result = Writes.editorTheatreHudSlovo; break;
                case "редактор театрконкурса: Хореография": result = Writes.editorTheatreHoreo; break;
                case "редактор театрконкурса: Вокал": result = Writes.editorTheatreVokal; break;
                case "редактор театрконкурса: Инструментальный жанр": result = Writes.editorTheatreInstrumZanr; break;
                case "редактор театрконкурса: Театр моды": result = Writes.editorTheatreModa; break;
                case "редактор конкурса Открытый мир": result = Writes.editorKultura; break;
                case "редактор спорт: Простейшие единоборства": result = Writes.editorSport; break;
                case "редактор спорт: Бокс": result = Writes.editorBoks; break;
                case "редактор спорт: Кунг-фу УИ": result = Writes.editorKungfu; break;
                case "редактор спорт: Тхэквондо": result = Writes.editorThekvo; break;
                case "редактор спорт: Футбол": result = Writes.editorFootball; break;
                case "редактор спорт: Баскетбол": result = Writes.editorBasketball; break;
                case "редактор спорт: Волейбол": result = Writes.editorVolleyball; break;
                case "редактор спорт: Тактический лазертаг": result = Writes.editorPaintball; break;
                case "редактор спорт: Шахматы": result = Writes.editorShahmaty; break;
                case "редактор спорт: Шашки": result = Writes.editorShashky; break;
                case "редактор топонимики: Топонимика Москвы и Севастополя": result = Writes.editorToponim; break;
                case "редактор робототехники: Шаг в будущее": result = Writes.editorRobotech; break;
                case "редактор конкурса: Вместе мы сильнее": result = Writes.editorVmesteSila; break;
                case "редактор конкурса: Вместе мы сильнее (макияж)": result = Writes.editorVmesteSilaMakeUp; break;
                case "редактор конкурса: Вместе мы сильнее (парикмахер)": result = Writes.editorVmesteSilaShair; break;
                case "редактор конкурса: Индустрия моды": result = Writes.editorClothes; break;
                case "редактор конкурса мультимедиа-проектов Юные защитники 1-й и 2-й обороны Севастополя": result = Writes.editorMultimedia; break;
                case "редактор конкурса: Кораблик детства": result = Writes.editorKorablik; break;
                case "редактор конкурса: Кораблик детства (вокал)": result = Writes.editorKorablikVokal; break;
                case "редактор конкурса: Кораблик детства (хореография)": result = Writes.editorKorablikHoreo; break;
                case "редактор конкурса: Крымские маршруты": result = Writes.editorCrimroute; break;
                case "редактор конкурса: Математический батл": result = Writes.editorMathbattle; break;
                case "редактор квест-игры: Покоряя космос": result = Writes.editorKosmos; break;
                case "редактор конкурса научных работ В моей лаборатории вот что...": result = Writes.editorScience; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetAgeCategoriesValue(AgeCategories fromEnum)

        /// <summary>Метод получает значение перечисления типа возрастной категорий участников конкурсов и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetAgeCategoriesValue(AgeCategories fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case AgeCategories.baybi: result = "Бэби"; break;
                case AgeCategories.doshkolnaya: result = "Дошкольная"; break;
                case AgeCategories.mladshaya: result = "Младшая"; break;
                case AgeCategories.srednaya: result = "Средняя"; break;
                case AgeCategories.starshaya: result = "Старшая"; break;
                case AgeCategories.molodezh: result = "Молодежь"; break;
                case AgeCategories.smeshannaya: result = "Смешанная"; break;
                case AgeCategories.group1: result = "1 группа"; break;
                case AgeCategories.group2: result = "2 группа"; break;
                case AgeCategories.group3: result = "3 группа"; break;
                case AgeCategories.group4: result = "4 группа"; break;
                case AgeCategories.group5: result = "5 группа"; break;
                case AgeCategories.group6: result = "6 группа"; break;
                case AgeCategories.profi: result = "Профи"; break;
                case AgeCategories.group2011: result = "группа 2011"; break;
                case AgeCategories.group2012: result = "группа 2012"; break;
                case AgeCategories.group2013: result = "группа 2013"; break;
                case AgeCategories.group2014: result = "группа 2014"; break;
                case AgeCategories.group2015: result = "группа 2015"; break;
                case AgeCategories.group2016: result = "группа 2016"; break;
                case AgeCategories.group7_9: result = "7-9 лет"; break;
                case AgeCategories.group8_11: result = "8-11 лет"; break;
                case AgeCategories.group10_13: result = "10-13 лет"; break;
                case AgeCategories.group12_15: result = "12-15 лет"; break;
                case AgeCategories.VNEKATEGORY: result = "Вне категории"; break;

                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetPrintDocumentType(PrintDocumentType fromEnum)

        /// <summary>Метод получает значение перечисления с номинациями театрального конкурса и возвращает соответствующее ему строковое значение</summary>
        /// <param name="fromEnum">значение перечисления</param>
        /// <returns></returns>
        public static string GetPrintDocumentType(PrintDocumentType fromEnum)
        {
            string result = "";

            switch (fromEnum)
            {
                case PrintDocumentType.diplomIndividual1Round: result = "ДИПЛОМЫ для каждого участника первого тура"; break;
                case PrintDocumentType.diplomGroup1Round: result = "Коллективный ДИПЛОМ участников первого тура"; break;
                case PrintDocumentType.certificateIndividual1Round: result = "СЕРТИФИКАТЫ для каждого участника первого тура"; break;
                case PrintDocumentType.certificateGroup1Round: result = "Коллективный СЕРТИФИКАТ участников первого тура"; break;
                case PrintDocumentType.diplomIndividual2Round: result = "ДИПЛОМЫ для каждого участника второго тура"; break;
                case PrintDocumentType.diplomGroup2Round: result = "Коллективный ДИПЛОМ участников второго тура"; break;
                case PrintDocumentType.certificateIndividual2Round: result = "СЕРТИФИКАТЫ для каждого участника второго тура"; break;
                case PrintDocumentType.certificateGroup2Round: result = "Коллективный СЕРТИФИКАТ участников второго тура"; break;
                case PrintDocumentType.thanksLetter: result = "Благодарственное письмо педагогу"; break;
                default: break;
            }

            return result;
        }

        #endregion

        #region Метод GetSqlLogic(SqlLogic sqlLogic)

        /// <summary>Метод возвращает сроковое значение, соответствующее значению в перечислении SqlLogic</summary>
        /// <param name="sqlLogic">значение перечисления</param>
        /// <returns>Возвращает "no result" в крайнем случае:-)</returns>
        public static string GetSqlLogic(SqlLogic sqlLogic)
        {
            string result = "no result";

            #region Код
            if (sqlLogic == SqlLogic.AND) result = "AND";
            else if (sqlLogic == SqlLogic.OR) result = "OR";
            #endregion

            return result;
        }


        #endregion
    }

    #endregion
}