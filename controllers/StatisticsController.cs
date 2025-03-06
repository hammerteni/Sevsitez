using NameCaseLib.Core;
using site.dbContext;
using site.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace site.classes
{
    class StatisticsCouters
    {
        public string GroupTypeName { get; set; }
        public int CountAll { get; set; }
        public int CountParticipant1Round { get; set; }
        public int CountParticipant2Round { get; set; }

        public int CountParticipantAll { get; set; }
    }
    public class StatisticsController : ApiController
    {
        private EducationOrgAreasContext db;
        private CompetitionsContext dbRequests;

        public StatisticsController()
        {
            db = new EducationOrgAreasContext();
            dbRequests = new CompetitionsContext();
        }
        public object Get()
        {
            string parseError = "";
            var unknownRegionRequests = new List<CompetitionRequest>();
            var unknownMoscowRequests = new List<CompetitionRequest>();

            var result = new Dictionary<string, StatisticsCouters>()
            {
                { "Детей и молодёжи в возрасте от 5 до 21 года", new StatisticsCouters() },
                { "Федеральных округов", new StatisticsCouters() },
                { "Регионов РФ", new StatisticsCouters() },
                { "Городов РФ", new StatisticsCouters() },
                { "Участников из регионов РФ", new StatisticsCouters() },
                { "Участников из г. Москва", new StatisticsCouters() },
            };

            var fedOkrugov = dbRequests.Competitions.Select(y => y.District.Trim()).Distinct().Count();
            result["Федеральных округов"].CountAll = fedOkrugov;

            var regionov = dbRequests.Competitions.Select(y => y.Region.Trim()).Distinct().Count();
            result["Регионов РФ"].CountAll = regionov;

            var gorodov = dbRequests.Competitions.Select(y => y.City.Trim()).Distinct().Count();
            result["Городов РФ"].CountAll = gorodov;

            var childrensRegionList = dbRequests.Competitions.Where(x => x.Region != "Москва")
                    .Select(y => (!string.IsNullOrEmpty(y.Fios) ? y.Fios : y.Fio) + "|" + (string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1));
            var childrensRegionAll = string.Join("|", childrensRegionList).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();
            result["Участников из регионов РФ"].CountAll = childrensRegionAll;

            var childrensMoscowList = dbRequests.Competitions.Where(x => x.Region == "Москва")
                    .Select(y => (!string.IsNullOrEmpty(y.Fios) ? y.Fios : y.Fio) + "|" + (string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1));
            var childrensMoscowAll = string.Join("|", childrensMoscowList).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();
            result["Участников из г. Москва"].CountAll = childrensMoscowAll;

            result["Детей и молодёжи в возрасте от 5 до 21 года"].CountAll = childrensMoscowAll + childrensRegionAll;

            Dictionary<string, StatisticsCouters> groupTypes = db.EducationOrganizationType.Select(x => new StatisticsCouters() { GroupTypeName = x.Name, CountAll = 0, CountParticipantAll = 0 }).ToDictionary(x => x.GroupTypeName);

            var list = new CompetitionsWork().GetListOfRequests("", "", "");
            var edudict = new EducationOrganizationWork().EducationOrganizations();
            for (int i = 0; i < list.Count; i++)
            {
                if (edudict.ContainsKey(list[i].EducationalOrganization + list[i].EducationalOrganizationShort))
                {
                    var ttedu = edudict[list[i].EducationalOrganization + list[i].EducationalOrganizationShort];

                    list[i].TypeName = ttedu.TypeName;
                    list[i].MRSD = ttedu.MRSD;
                }
            }

            groupTypes.Add("МРСД", new StatisticsCouters());
            groupTypes.Add("нет", new StatisticsCouters());

            foreach (KeyValuePair<string, StatisticsCouters> kvp in groupTypes)
            {
                if (kvp.Key == "МРСД")
                {
                    var tempQuery = list.Where(x => x.MRSD.Contains("МРСД"));

                    var cpMRSDRound1 = tempQuery.Select(y => string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1);
                    var cpMRSDRound1All = string.Join("|", cpMRSDRound1).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();

                    var cpMRSDRound2 = tempQuery.Select(y => !string.IsNullOrEmpty(y.Fios) ? y.Fios : y.Fio);
                    var cpMRSDRound2All = string.Join("|", cpMRSDRound2).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();

                    var countAll = tempQuery.Select(y => y.EducationalOrganization).Distinct().Count();

                    kvp.Value.CountAll += countAll;
                    kvp.Value.CountParticipant1Round += cpMRSDRound1All;
                    kvp.Value.CountParticipant2Round += cpMRSDRound2All;
                    kvp.Value.CountParticipantAll += cpMRSDRound1All + cpMRSDRound2All;
                }
                else if (kvp.Key == "Индивидуальная заявка")
                {
                    var tempQuery = list.Where(x => x.EducationalOrganization == "Индивидуальная заявка");

                    var cpIndividRound1 = tempQuery.Select(y => string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1);
                    var cpIndividRound1All = string.Join("|", cpIndividRound1).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();

                    var cpIndividRound2 = tempQuery.Select(y => !string.IsNullOrEmpty(y.Fios) ? y.Fios : y.Fio);
                    var cpIndividRound2All = string.Join("|", cpIndividRound2).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();

                    var countAll = tempQuery.Select(y => y.EducationalOrganization).Distinct().Count();

                    kvp.Value.CountAll += countAll;
                    kvp.Value.CountParticipant1Round += cpIndividRound1All;
                    kvp.Value.CountParticipant2Round += cpIndividRound2All;
                    kvp.Value.CountParticipantAll += cpIndividRound1All + cpIndividRound2All;
                }
                else 
                { 
                    var tempQuery = list.Where(x => x.TypeName == kvp.Key);
                    
                    var cpRound1List = tempQuery.Select(y => string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1);
                    var cpRound1 = string.Join("|", cpRound1List).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();
                    
                    var cpRound2List = tempQuery.Select(y => !string.IsNullOrEmpty(y.Fios) ? y.Fios : y.Fio);
                    var cpRound2 = string.Join("|", cpRound2List).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Count();

                    var countAll = tempQuery.Select(y => y.EducationalOrganization).Distinct().Count();

                    kvp.Value.CountAll += countAll;
                    kvp.Value.CountParticipant1Round += cpRound1;
                    kvp.Value.CountParticipant2Round += cpRound2;
                    kvp.Value.CountParticipantAll += cpRound1 + cpRound2;
                }
            }

            return new
            {
                parseError,
                statData = result.Select(x => new { Показатель = x.Key, Количественный_показатель = x.Value.CountAll }).ToList(),
                statDataUchr = groupTypes.Select(x => new { Показатель = x.Key, Количество_учреждений = x.Value.CountAll, Количество_участников_I_тура = x.Value.CountParticipant1Round, Количество_участников_II_тура = x.Value.CountParticipant2Round, Общее_количество_участников = x.Value.CountParticipantAll }).ToList()
            };
        }

        private void old_method() {

            #region
            var work = new CompetitionsWork();
            var list = work.GetListOfRequests()
                  //Отсекаем все тестовые заявки
                  .Where(x => x.WorkName != "Проверка")
                  .ToList();

            string parseError = "";

            var result = new Dictionary<string, int>()
            {
                { "Детей и молодёжи в возрасте от 5 до 21 года", 0},
                { "Регионов РФ", 0},
                { "Участников из регионов РФ", 0},
                { "Московских образовательных организаций", 0 },
                { "Учреждений дополнительного образования", 0 },
                { "Школ", 0 },
                { "Колледжей и техникумов", 0 },
                { "Учреждений культуры", 0 },
                { "Муниципальных учреждений", 0 },
                { "Региональных образовательных организаций", 0 },
            };

            var regions = list.GroupBy(x => (string.IsNullOrEmpty(x.Region) ? x.Region.Trim() : x.Region)).Select(x => new { Region = x.Key, ReqList = x.Select(y => y).ToList() }).ToList();
            var regionsResult = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "Москва", new List<CompetitionRequest>()},
                { "Московская область", new List<CompetitionRequest>() },
                { "Курганская область", new List<CompetitionRequest>() },
                { "Рязанская область", new List<CompetitionRequest>()},
                { "Пермский край", new List<CompetitionRequest>()},
                { "Вологодская область", new List<CompetitionRequest>() },
                { "Краснодарский край", new List<CompetitionRequest>() },
                { "Республика Крым", new List<CompetitionRequest>() },
                { "Архангельская область", new List<CompetitionRequest>() },
                { "Курская область", new List<CompetitionRequest>() },
                { "Ленинградская область", new List<CompetitionRequest>() },
                { "Ярославская область", new List<CompetitionRequest>() },
                { "Саратовская область", new List<CompetitionRequest>() },
                { "Ямало-Ненецкий автономный округ", new List<CompetitionRequest>() },
                { "Ивановская область", new List<CompetitionRequest>() },
                { "Калининградская область", new List<CompetitionRequest>() },
                { "Брянская область", new List<CompetitionRequest>() },
                { "Нижегородская область", new List<CompetitionRequest>()},
                { "Липецкая область", new List<CompetitionRequest>()},
                { "Тюменская область", new List<CompetitionRequest>()},
                { "Ханты-Мансийский автономный округ", new List<CompetitionRequest>()},
                { "Смоленская область", new List<CompetitionRequest>()},
                { "Белгородская область", new List<CompetitionRequest>() },
                { "Свердловская область", new List<CompetitionRequest>() },
                { "Калужская область", new List<CompetitionRequest>() },
                { "Челябинская область", new List<CompetitionRequest>() },
                { "Самарская область", new List<CompetitionRequest>() },
                { "Оренбургская область", new List<CompetitionRequest>() },
                { "Пензенская область", new List<CompetitionRequest>() },
                { "Мурманская область", new List<CompetitionRequest>() },
                { "Ростовская область", new List<CompetitionRequest>() },
                { "Кабардино-Балкарская Республика", new List<CompetitionRequest>() },
                { "Воронежская область", new List<CompetitionRequest>() },
                { "Тверская область", new List<CompetitionRequest>() },
                { "Волгоградская область", new List<CompetitionRequest>() },
                { "Нижегородская облать", new List<CompetitionRequest>() },
                { "Владимирская область", new List<CompetitionRequest>() },
                { "Магаданская область", new List<CompetitionRequest>() },
                { "Алтайский край", new List<CompetitionRequest>() },
                { "Тульская область", new List<CompetitionRequest>() },
                { "Амурская область", new List<CompetitionRequest>() },
                { "Республики Саха (Якутия)", new List<CompetitionRequest>() },
             };
            var unknownRegionRequests = new List<CompetitionRequest>();

            try
            {

                for (var i = 0; i < regions.Count; i++)
                {
                    var req = regions[i];
                    //пустые регионы
                    if (string.IsNullOrEmpty(req.Region))
                    {
                        for (var j = 0; j < req.ReqList.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(req.ReqList[j].City))
                            {
                                if (req.ReqList[j].City.ToLower().Contains("москва")
                                    || req.ReqList[j].City.ToLower().Contains("моска")
                                    || req.ReqList[j].City.ToLower().Contains("щербинка")
                                    || req.ReqList[j].City.ToLower().Contains("млсква")
                                    || req.ReqList[j].City.ToLower().Contains("город город")
                                    || req.ReqList[j].City.ToLower().Contains("moscow")
                                    || req.ReqList[j].City.ToLower().Contains("звенигород")
                                    || req.ReqList[j].City.ToLower().Contains("москвв")
                                    || req.ReqList[j].City.ToLower().Contains("первомайское")
                                    || req.ReqList[j].City.ToLower().Contains("троицк")
                                    || req.ReqList[j].City.ToLower().Contains("моссква")
                                    || req.ReqList[j].EducationalOrganization.ToLower().Replace(" ", "").Contains("гбоушкола")
                                    )
                                {
                                    var temp = regionsResult["Москва"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Москва"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("крым")
                                    || req.ReqList[j].City.ToLower().Contains("ялта")
                                    || req.ReqList[j].City.ToLower().Contains("керчь"))
                                {
                                    var temp = regionsResult["Республика Крым"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Республика Крым"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("подольск")
                                    || req.ReqList[j].City.ToLower().Contains("московский") || req.ReqList[j].City.ToLower().Contains("зеленоград")
                                    || req.ReqList[j].City.ToLower().Contains("пушкино") || req.ReqList[j].City.ToLower().Contains("сергиев посад")
                                    || req.ReqList[j].City.ToLower().Contains("красногорск") || req.ReqList[j].City.ToLower().Contains("химки")
                                    || req.ReqList[j].City.ToLower().Contains("ивантеевка")
                                    || req.ReqList[j].City.ToLower().Contains("поселок Васильевское")
                                    || req.ReqList[j].City.ToLower().Contains("кокошкино")
                                    || req.ReqList[j].City.ToLower().Contains("назарьево")
                                    || req.ReqList[j].City.ToLower().Contains("деревня суханово")
                                    || req.ReqList[j].City.ToLower().Contains("деревня лопатино")
                                    || req.ReqList[j].City.ToLower().Contains("раменское")

                                    )
                                {
                                    var temp = regionsResult["Московская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Московская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("саратов"))
                                {
                                    var temp = regionsResult["Саратовская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Саратовская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("калуга")
                                    || req.ReqList[j].City.ToLower().Contains("калужская"))
                                {
                                    var temp = regionsResult["Калужская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Калужская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("новая ладога"))
                                {
                                    var temp = regionsResult["Ленинградская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Ленинградская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("пермь"))
                                {
                                    var temp = regionsResult["Пермский край"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Пермский край"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("челябинск") || req.ReqList[j].City.ToLower().Contains("златоуст"))
                                {
                                    var temp = regionsResult["Челябинская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Челябинская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("чапаевск"))
                                {
                                    var temp = regionsResult["Самарская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Самарская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("белгород"))
                                {
                                    var temp = regionsResult["Белгородская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Белгородская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("оренбург"))
                                {
                                    var temp = regionsResult["Оренбургская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Оренбургская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("смоленск"))
                                {
                                    var temp = regionsResult["Смоленская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Смоленская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("пенза"))
                                {
                                    var temp = regionsResult["Пензенская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Пензенская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("самара"))
                                {
                                    var temp = regionsResult["Самарская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Самарская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("саров") || req.ReqList[j].City.ToLower().Contains("кондрово"))
                                {
                                    var temp = regionsResult["Нижегородская облать"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Нижегородская облать"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("село черницыно"))
                                {
                                    var temp = regionsResult["Курская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Курская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("село большая глушица"))
                                {
                                    var temp = regionsResult["Самарская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Самарская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("магадан"))
                                {
                                    var temp = regionsResult["Магаданская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Магаданская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("никологоры"))
                                {
                                    var temp = regionsResult["Владимирская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Владимирская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("великий устюг") ||
                                    req.ReqList[j].City.ToLower().Contains("поселок васильевское"))
                                {
                                    var temp = regionsResult["Вологодская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Вологодская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("барнаул"))
                                {
                                    var temp = regionsResult["Алтайский край"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Алтайский край"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("слобода советка"))
                                {
                                    var temp = regionsResult["Ростовская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Ростовская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("слобода советка"))
                                {
                                    var temp = regionsResult["Ростовская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Ростовская область"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().Contains("якутск"))
                                {
                                    var temp = regionsResult["Республики Саха(Якутия)"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Республики Саха(Якутия)"] = temp;
                                }
                                else if (req.ReqList[j].City.ToLower().IndexOf("новомосковск") != -1)
                                {
                                    var temp = regionsResult["Тульская область"];
                                    temp.Add(req.ReqList[j]);
                                    regionsResult["Тульская область"] = temp;
                                }

                                else
                                {
                                    var temp = unknownRegionRequests;
                                    temp.Add(req.ReqList[j]);
                                    unknownRegionRequests = temp;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(req.ReqList[j].EducationalOrganization))
                                {
                                    if (req.ReqList[j].EducationalOrganization.ToLower().Contains("севастополец")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("москва")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("москвы")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("кадетская школа 1784")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("гкоу скоши № 52")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("неоткрытые острова")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("мос квы")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("центр внешкольной работы синегория")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("эврика-бутово")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("на таганке")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("государственное бюджетное общеобразовательное учреждение школа №2075")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа 193")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("цдт замоскворечье")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа №1285")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа №1358")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа №1747")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа 1747")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа №1564")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("школа 1564")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("средняя общеообразовательная школа №2007")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("гбу цтдс хорошее настроение")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("гбу  сдц  \"хорошее настроение\"")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("№1798 феникс")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Contains("пресня")
                                        || req.ReqList[j].EducationalOrganization.ToLower().Replace(" ", "").Contains("гбоушкола")
                                        )
                                    {
                                        var temp = regionsResult["Москва"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Москва"] = temp;
                                    }
                                    else if (req.ReqList[j].EducationalOrganization.ToLower().Contains("ооо \"школа развития маяк\""))
                                    {
                                        var temp = regionsResult["Московская область"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Московская область"] = temp;
                                    }
                                    else if (req.ReqList[j].EducationalOrganization.ToLower().Contains("богандинская средняя общеобразовательная школа № 1 тюменского муниципального района"))
                                    {
                                        var temp = regionsResult["Тюменская область"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Тюменская область"] = temp;
                                    }
                                    else if (req.ReqList[j].EducationalOrganization.ToLower().Contains("города сарова") || req.ReqList[j].EducationalOrganization.ToLower().Contains("г.саров"))
                                    {
                                        var temp = regionsResult["Нижегородская облать"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Нижегородская облать"] = temp;
                                    }
                                    else
                                    {
                                        var temp = unknownRegionRequests;
                                        temp.Add(req.ReqList[j]);
                                        unknownRegionRequests = temp;
                                    }
                                }
                                else
                                {
                                    var temp = unknownRegionRequests;
                                    temp.Add(req.ReqList[j]);
                                    unknownRegionRequests = temp;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (req.Region == "ЮАО" || req.Region == "г. Москва" || req.Region == "г.Москва" ||
                            req.Region == "город Москва" || req.Region == "мосвка" || req.Region == "СЗАО Москвы" || req.Region == "москва" ||
                            req.Region == "ТиНАО" || req.Region == "Moscow" || req.Region == "ВАО" || req.Region == "ЮВАО" || req.Region == "СВАО" ||
                            req.Region == "Москва СВАО" || req.Region == "Москвы" || req.Region == "МОСКВА" || req.Region == "ЦФО" || req.Region == "Москва город" ||
                            req.Region == "Город Москва" || req.Region == "ЮЗАО" || req.Region == "ЦАО" || req.Region.ToLower().Contains("москва") ||
                            req.Region.ToLower().Contains("moscow") || req.Region.ToLower().Contains("мосва") || req.Region.ToLower().Contains("мосвка") ||
                            req.Region.Contains("77"))
                        {
                            var temp = regionsResult["Москва"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Москва"] = temp;
                        }
                        else if (req.Region == "Московская область" || req.Region == "Московская обл." || req.Region == "Московская область " ||
                            req.Region == "Московская облачть" || req.Region == "Московский" || req.Region == "Московская" ||
                            req.Region == "московская область" || req.Region == "Московская обл" || req.Region == "Москоская область" ||
                            req.Region == "Москвовская область")
                        {
                            var temp = regionsResult["Московская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Московская область"] = temp;
                        }
                        else if (req.Region == "Курганская")
                        {
                            var temp = regionsResult["Курганская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Курганская область"] = temp;
                        }
                        else if (req.Region.ToLower() == "крым" || req.Region == "Севастополь" || req.Region == "Крымская область")
                        {
                            var temp = regionsResult["Республика Крым"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Республика Крым"] = temp;
                        }
                        else if (req.Region == "Свердловская область Байкаловский район")
                        {
                            var temp = regionsResult["Свердловская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Свердловская область"] = temp;
                        }
                        else if (req.Region == "Санкт-Петербург")
                        {
                            var temp = regionsResult["Ленинградская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Ленинградская область"] = temp;
                        }
                        else if (req.Region == "Ямало-Ненtцкий автономный округ")
                        {
                            var temp = regionsResult["Ямало-Ненецкий автономный округ"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Ямало-Ненецкий автономный округ"] = temp;
                        }
                        else if (req.Region == "Пермский" || req.Region == "пермский край")
                        {
                            var temp = regionsResult["Пермский край"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Пермский край"] = temp;
                        }
                        else if (req.Region == "Смоленская обл")
                        {
                            var temp = regionsResult["Смоленская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Смоленская область"] = temp;
                        }
                        else if (req.Region == "Белогородская область" || req.Region == "Белгородская обл")
                        {
                            var temp = regionsResult["Белгородская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Белгородская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("кабардино-балкарская"))
                        {
                            var temp = regionsResult["Кабардино-Балкарская Республика"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Кабардино-Балкарская Республика"] = temp;
                        }
                        else if (req.Region == "Ивановский")
                        {
                            var temp = regionsResult["Ивановская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Ивановская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("калужская"))
                        {
                            var temp = regionsResult["Калужская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Калужская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("мурманская"))
                        {
                            var temp = regionsResult["Мурманская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Мурманская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("калининградская"))
                        {
                            var temp = regionsResult["Калининградская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Калининградская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("тверская"))
                        {
                            var temp = regionsResult["Тверская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Тверская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("собинский"))
                        {
                            var temp = regionsResult["Владимирская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Владимирская область"] = temp;
                        }
                        else if (req.Region.ToLower().Contains("вологодская"))
                        {
                            var temp = regionsResult["Вологодская область"];
                            temp.AddRange(req.ReqList);
                            regionsResult["Вологодская область"] = temp;
                        }
                        else if (req.Region == "РФ" || req.Region == "нет" || req.Region == "Россия" ||
                            req.Region == "ЦФО РФ" || req.Region == "Выберите" || req.Region == "1611" || req.Region == "Рф"
                            || req.Region == "moiseeva911@yandex.ru" || req.Region == "-" || req.Region == "Центральный" || req.Region == "п.Киевский" || req.Region == "п. Киевский"
                            || req.Region == "Российская Федерация" || req.Region == "Куркино" || req.Region == "Moskva" || req.Region == "Центральный округ"
                            || req.Region == "Московская Область" || req.Region == "МО" || req.Region == "Московский регион")
                        {
                            for (var j = 0; j < req.ReqList.Count; j++)
                            {
                                if (!string.IsNullOrEmpty(req.ReqList[j].City))
                                {
                                    if (req.ReqList[j].City.ToLower().Contains("москва") || req.ReqList[j].City.ToLower().Contains("moscow"))
                                    {
                                        var temp = regionsResult["Москва"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Москва"] = temp;
                                    }
                                    else if (req.ReqList[j].City.ToLower().Contains("крым"))
                                    {
                                        var temp = regionsResult["Республика Крым"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Республика Крым"] = temp;
                                    }
                                    else if (req.ReqList[j].City.ToLower().Contains("санкт-петербург"))
                                    {
                                        var temp = regionsResult["Ленинградская область"];
                                        temp.Add(req.ReqList[j]);
                                        regionsResult["Ленинградская область"] = temp;
                                    }
                                    else
                                    {
                                        var temp = unknownRegionRequests;
                                        temp.Add(req.ReqList[j]);
                                        unknownRegionRequests = temp;
                                    }
                                }
                                else
                                {
                                    var temp = unknownRegionRequests;
                                    temp.Add(req.ReqList[j]);
                                    unknownRegionRequests = temp;
                                }
                            }
                        }
                        else
                        {
                            if (regionsResult.ContainsKey(req.Region))
                            {
                                var temp = regionsResult[req.Region];
                                temp.AddRange(req.ReqList);
                                regionsResult[req.Region] = temp;
                            }
                            else
                            {
                                if (req.ReqList[0].EducationalOrganization.ToLower().Replace(" ", "").Contains("гбоушкола"))
                                {
                                    var temp = regionsResult["Москва"];
                                    temp.Add(req.ReqList[0]);
                                    regionsResult["Москва"] = temp;
                                }
                                else
                                {
                                    var temp = unknownRegionRequests;
                                    temp.AddRange(req.ReqList);
                                    unknownRegionRequests = temp;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                parseError = ex.Message;
            }

            result["Регионов РФ"] = regionsResult.Where(x => x.Value.Count > 0).Count();

            var childrensAllList = regionsResult.SelectMany(x => x.Value.Select(y => (string.IsNullOrEmpty(y.Fio) ? "" : y.Fio) + "|" + (string.IsNullOrEmpty(y.Fios) ? "" : y.Fios) + "|" + (string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1)));
            var childrensAllGroup = string.Join("|", childrensAllList).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim()));//.Distinct().OrderBy(x => x);

            result["Детей и молодёжи в возрасте от 5 до 21 года"] = childrensAllGroup.Count();

            var childrensAllListRegions = regionsResult.Where(x => (!new string[] { "Москва" }.Contains(x.Key)) && x.Value.Count > 0).SelectMany(x => x.Value.Select(y => (string.IsNullOrEmpty(y.Fio) ? "" : y.Fio) + "|" + (string.IsNullOrEmpty(y.Fios) ? "" : y.Fios) + "|" + (string.IsNullOrEmpty(y.Fios_1) ? "" : y.Fios_1)));
            var childrensAllGroupRegions = string.Join("|", childrensAllListRegions).Split('|').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().OrderBy(x => x);

            result["Участников из регионов РФ"] = childrensAllGroupRegions.Count();

            var dictSchools = new Dictionary<string, string>();
            var regionslistRequests = regionsResult.Where(x => !new string[] { "Москва" }.Contains(x.Key) && x.Value.Count > 0);
            foreach (var regKey in regionslistRequests)
            {
                foreach (var regReq in regKey.Value)
                {
                    if (!string.IsNullOrEmpty(((CompetitionRequest)regReq).Schools))
                    {
                        var sc = ((CompetitionRequest)regReq).Schools.Split('|');
                        foreach (var s in sc)
                        {
                            if (!dictSchools.ContainsKey(s))
                            {
                                dictSchools.Add(s, s);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(((CompetitionRequest)regReq).Schools_1))
                    {
                        var sc = ((CompetitionRequest)regReq).Schools_1.Split('|');
                        foreach (var s in sc)
                        {
                            if (!dictSchools.ContainsKey(s))
                            {
                                dictSchools.Add(s, s);
                            }

                        }
                    }
                }
            }

            result["Региональных образовательных организаций"] = dictSchools.Where(x => x.Key.ToLower() != "нет").Count();

            var moscowRequests = regionsResult.Where(x => x.Key == "Москва").SelectMany(x => x.Value).ToList();
            var moscowSchool = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "193", new List<CompetitionRequest>()},
                { "1467", new List<CompetitionRequest>() },
                { "1981", new List<CompetitionRequest>()},
                { "1582", new List<CompetitionRequest>() },
                { "1586", new List<CompetitionRequest>() },
                { "1784", new List<CompetitionRequest>()},
                { "52", new List<CompetitionRequest>()},
                { "1357", new List<CompetitionRequest>() },
                { "1569", new List<CompetitionRequest>() },
                { "1539", new List<CompetitionRequest>() },
                { "1034", new List<CompetitionRequest>() },
                { "1329", new List<CompetitionRequest>() },
                { "2089", new List<CompetitionRequest>() },
                { "1265", new List<CompetitionRequest>() },
                { "1002", new List<CompetitionRequest>() },
                { "1279", new List<CompetitionRequest>() },
                { "626", new List<CompetitionRequest>() },
                { "319", new List<CompetitionRequest>() },
                { "1080", new List<CompetitionRequest>() },
                { "771", new List<CompetitionRequest>()},
                { "1795", new List<CompetitionRequest>()},
                { "2115", new List<CompetitionRequest>()},
                { "1371", new List<CompetitionRequest>()},
                { "1547", new List<CompetitionRequest>()},
                { "1883", new List<CompetitionRequest>() },
                { "1450", new List<CompetitionRequest>() },
                { "1212", new List<CompetitionRequest>() },
                { "1748", new List<CompetitionRequest>() },
                { "883", new List<CompetitionRequest>() },
                { "1502", new List<CompetitionRequest>() },
                { "1794", new List<CompetitionRequest>() },
                { "814", new List<CompetitionRequest>() },
                { "2120", new List<CompetitionRequest>() },
                { "281", new List<CompetitionRequest>() },
                { "1282", new List<CompetitionRequest>() },
                { "1191", new List<CompetitionRequest>() },
                { "1492", new List<CompetitionRequest>() },
                { "1273", new List<CompetitionRequest>() },
                { "1694", new List<CompetitionRequest>() },
                { "2098", new List<CompetitionRequest>() },
                { "1542", new List<CompetitionRequest>() },
                { "2101", new List<CompetitionRequest>() },
                { "1103", new List<CompetitionRequest>() },
                { "1296", new List<CompetitionRequest>() },
                { "1590", new List<CompetitionRequest>() },
                { "2070", new List<CompetitionRequest>() },
                { "1533", new List<CompetitionRequest>() },
                { "2114", new List<CompetitionRequest>() },
                { "324", new List<CompetitionRequest>() },
                { "1708", new List<CompetitionRequest>() },
                { "887", new List<CompetitionRequest>() },
                { "1368", new List<CompetitionRequest>() },
                { "Судакова", new List<CompetitionRequest>() },
                { "201", new List<CompetitionRequest>() },
                { "1613", new List<CompetitionRequest>() },
                { "2051", new List<CompetitionRequest>() },
                { "1770", new List<CompetitionRequest>() },
                { "1541", new List<CompetitionRequest>() },
                { "935", new List<CompetitionRequest>() },
                { "1948", new List<CompetitionRequest>() },
                { "1354", new List<CompetitionRequest>() },
                { "1370", new List<CompetitionRequest>() },
                { "902", new List<CompetitionRequest>() },
                { "1519", new List<CompetitionRequest>() },
                { "1359", new List<CompetitionRequest>() },
                { "Марьино", new List<CompetitionRequest>() },
                { "2057", new List<CompetitionRequest>() },
                { "Марьина Роща", new List<CompetitionRequest>() },
                { "1430", new List<CompetitionRequest>() },
                { "97", new List<CompetitionRequest>() },
                { "138", new List<CompetitionRequest>() },
                { "1363", new List<CompetitionRequest>() },
                { "1811", new List<CompetitionRequest>() },
                { "2026", new List<CompetitionRequest>() },
                { "1409", new List<CompetitionRequest>() },
                { "2086", new List<CompetitionRequest>() },
                { "949", new List<CompetitionRequest>() },
                { "2073", new List<CompetitionRequest>() },
                { "1293", new List<CompetitionRequest>() },
                { "536", new List<CompetitionRequest>() },
                { "1798", new List<CompetitionRequest>() },
                { "1208", new List<CompetitionRequest>() },
                { "962", new List<CompetitionRequest>() },
                { "1566", new List<CompetitionRequest>() },
                { "544", new List<CompetitionRequest>() },
                { "1362", new List<CompetitionRequest>() },
                { "939", new List<CompetitionRequest>() },
                { "1253", new List<CompetitionRequest>() },
                { "1468", new List<CompetitionRequest>() },
                { "998", new List<CompetitionRequest>() },
                { "587", new List<CompetitionRequest>() },
                { "2009", new List<CompetitionRequest>() },
                { "Окуджавы", new List<CompetitionRequest>() },
                { "538", new List<CompetitionRequest>() },
                { "625", new List<CompetitionRequest>() },
                { "118", new List<CompetitionRequest>() },
                { "Твардовского", new List<CompetitionRequest>() },
                { "Мильграма", new List<CompetitionRequest>() },
                { "Достоевского", new List<CompetitionRequest>() },
                { "1358", new List<CompetitionRequest>() },
                { "1360", new List<CompetitionRequest>() },
                { "Тропарево", new List<CompetitionRequest>() },
                { "1280", new List<CompetitionRequest>() },
                { "1554", new List<CompetitionRequest>() },
                { "1530", new List<CompetitionRequest>() },
                { "1537", new List<CompetitionRequest>() },
                { "1985", new List<CompetitionRequest>() },
                { "830", new List<CompetitionRequest>() },
                { "2129", new List<CompetitionRequest>() },
                { "709", new List<CompetitionRequest>() },
                { "867", new List<CompetitionRequest>() },
                { "1391", new List<CompetitionRequest>() },
                { "460", new List<CompetitionRequest>() },
                { "1560", new List<CompetitionRequest>() },
                { "657", new List<CompetitionRequest>() },
                { "1557", new List<CompetitionRequest>() },
                { "2065", new List<CompetitionRequest>() },
                { "1955", new List<CompetitionRequest>() },
                { "1580", new List<CompetitionRequest>() },
                { "1466", new List<CompetitionRequest>() },
                { "1583", new List<CompetitionRequest>() },
                { "1429", new List<CompetitionRequest>() },
                { "2122", new List<CompetitionRequest>() },
                { "Соколиная гора", new List<CompetitionRequest>() },
                { "1400", new List<CompetitionRequest>() },
                { "1164", new List<CompetitionRequest>() },
                { "1576", new List<CompetitionRequest>() },
                { "1501", new List<CompetitionRequest>() },
                { "1272", new List<CompetitionRequest>() },
                { "1234", new List<CompetitionRequest>() },
                { "1257", new List<CompetitionRequest>() },
                { "1284", new List<CompetitionRequest>() },
                { "1150", new List<CompetitionRequest>() },
                { "996", new List<CompetitionRequest>() },
                { "1420", new List<CompetitionRequest>() },
                { "2054", new List<CompetitionRequest>() },
                { "1394", new List<CompetitionRequest>() },
                { "760", new List<CompetitionRequest>() },
                { "1862", new List<CompetitionRequest>() },
                { "1392", new List<CompetitionRequest>() },
                { "1151", new List<CompetitionRequest>() },
                { "947", new List<CompetitionRequest>() },
                { "Ларикова", new List<CompetitionRequest>() },
                { "1056", new List<CompetitionRequest>() },
                { "2053", new List<CompetitionRequest>() },
                { "2000", new List<CompetitionRequest>() },
                { "338", new List<CompetitionRequest>() },
                { "222", new List<CompetitionRequest>() },
                { "1287", new List<CompetitionRequest>() },
                { "2006", new List<CompetitionRequest>() },
                { "Таганская кадетская школа", new List<CompetitionRequest>() },
                { "Школа №158", new List<CompetitionRequest>() },
                { "Школа №108", new List<CompetitionRequest>() },
                { "Школа №17", new List<CompetitionRequest>() },
                { "Школа №90", new List<CompetitionRequest>() },
                { "Школа №15", new List<CompetitionRequest>() },
                { "1190", new List<CompetitionRequest>() },
                { "2103", new List<CompetitionRequest>() },
                { "1290", new List<CompetitionRequest>() },
                { "1413", new List<CompetitionRequest>() },
                { "878", new List<CompetitionRequest>() },
                { "827", new List<CompetitionRequest>() },
                { "1158", new List<CompetitionRequest>() },
                { "2075", new List<CompetitionRequest>() },
                { "1499", new List<CompetitionRequest>() },
                { "2127", new List<CompetitionRequest>() },
                { "1482", new List<CompetitionRequest>() },
                { "507", new List<CompetitionRequest>() },
                { "843", new List<CompetitionRequest>() },
                { "654", new List<CompetitionRequest>() },
                { "1373", new List<CompetitionRequest>() },
                { "305", new List<CompetitionRequest>() },
                { "1236", new List<CompetitionRequest>() },
                { "1375", new List<CompetitionRequest>() },
                { "1173", new List<CompetitionRequest>() },
                { "763", new List<CompetitionRequest>() },
                { "2001", new List<CompetitionRequest>() },
                { "Спектр", new List<CompetitionRequest>() },
                { "Протон", new List<CompetitionRequest>() },
                { "1245", new List<CompetitionRequest>() },
                { "285", new List<CompetitionRequest>() },
                { "1619", new List<CompetitionRequest>() },
                { "2107", new List<CompetitionRequest>() },
                { "2083", new List<CompetitionRequest>() },
                { "2030", new List<CompetitionRequest>() },
                { "1355", new List<CompetitionRequest>() },
                { "1465", new List<CompetitionRequest>() },
                { "1161", new List<CompetitionRequest>() },
                { "851", new List<CompetitionRequest>() },
                { "Московская международная школа", new List<CompetitionRequest>()},
                { "Глория", new List<CompetitionRequest>()},
                { "Карамзина", new List<CompetitionRequest>() },
                { "3108", new List<CompetitionRequest>() },
                { "956", new List<CompetitionRequest>() },
                { "656", new List<CompetitionRequest>() },
                { "117", new List<CompetitionRequest>() },
                { "2007", new List<CompetitionRequest>() },
                { "1286", new List<CompetitionRequest>() },
                { "1095", new List<CompetitionRequest>() },
                { "121", new List<CompetitionRequest>() },
                { "113", new List<CompetitionRequest>() },
                { "556", new List<CompetitionRequest>() },
                { "1449", new List<CompetitionRequest>() },
                { "224", new List<CompetitionRequest>() },
                { "630", new List<CompetitionRequest>() },
                { "64", new List<CompetitionRequest>() },
                { "21", new List<CompetitionRequest>() },
                { "42", new List<CompetitionRequest>() },
                { "49", new List<CompetitionRequest>() },
                { "в Капотне", new List<CompetitionRequest>() },
                { "Живоносный источник", new List<CompetitionRequest>() },
                { "1159", new List<CompetitionRequest>()},
                { "1246", new List<CompetitionRequest>()},
                { "1411", new List<CompetitionRequest>()},
                { "2033", new List<CompetitionRequest>()},
                { "368", new List<CompetitionRequest>()},
                { "2087", new List<CompetitionRequest>()},
                { "1440", new List<CompetitionRequest>()},
                { "480", new List<CompetitionRequest>()},
                { "825", new List<CompetitionRequest>()},
                { "1412", new List<CompetitionRequest>()},
                { "на Яузе", new List<CompetitionRequest>()},
                { "Перспектива", new List<CompetitionRequest>()},
                { "1194", new List<CompetitionRequest>()},
                { "766", new List<CompetitionRequest>()},
                { "1995", new List<CompetitionRequest>()},
                { "2072", new List<CompetitionRequest>()},
                { "Покровский квартал", new List<CompetitionRequest>()},
                { "ГБОУ Школа №46", new List<CompetitionRequest>()},
                { "1980", new List<CompetitionRequest>()},
                { "1944", new List<CompetitionRequest>()},
                { "Полбина", new List<CompetitionRequest>()},
                { "1601", new List<CompetitionRequest>()},
                { "ПМКК", new List<CompetitionRequest>()},
                { "Южный", new List<CompetitionRequest>()},
                { "1636", new List<CompetitionRequest>()},
                { "Первый Московский кадетский корпус", new List<CompetitionRequest>()},
            };

            var moscowDOP = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "Радость", new List<CompetitionRequest>()},
                { "Севастополец", new List<CompetitionRequest>() },
                { "Гайдара", new List<CompetitionRequest>() },
                { "Личность", new List<CompetitionRequest>() },
                { "Неоткрытые острова", new List<CompetitionRequest>() },
                { "На Стопани", new List<CompetitionRequest>() },
                { "Синегория", new List<CompetitionRequest>() },
                { "Преображенский", new List<CompetitionRequest>() },
                { "На Сумском", new List<CompetitionRequest>() },
                { "Воробьевы горы", new List<CompetitionRequest>() },
                { "Алексеевский", new List<CompetitionRequest>() },
                { "Косарева", new List<CompetitionRequest>() },
                { "Пресня", new List<CompetitionRequest>() },
                { "Глазунова", new List<CompetitionRequest>() },
                { "На Таганке", new List<CompetitionRequest>() },
                { "Строгино", new List<CompetitionRequest>() },
                { "Гилельса", new List<CompetitionRequest>() },
                { "Центр педагогического мастерства", new List<CompetitionRequest>() },
                { "Логос", new List<CompetitionRequest>() },
                { "Хорошево", new List<CompetitionRequest>() },
                { "Свиблово", new List<CompetitionRequest>() },
                { "Зеленоградский", new List<CompetitionRequest>() },
                { "Бибирево", new List<CompetitionRequest>() },
                { "Гермес", new List<CompetitionRequest>() },
                { "Каравелла", new List<CompetitionRequest>() },
                { "Фили-Давыдково", new List<CompetitionRequest>() },
                { "Благо", new List<CompetitionRequest>() },
                { "Виктория", new List<CompetitionRequest>() },
                { "Ника", new List<CompetitionRequest>() },
                { "Соц-ин", new List<CompetitionRequest>() },
                { "Маяк", new List<CompetitionRequest>() },
                { "Донской", new List<CompetitionRequest>() },
                { "Щербинка", new List<CompetitionRequest>() },
                { "Миуссах", new List<CompetitionRequest>() },
                { "Богородское", new List<CompetitionRequest>() },
                { "Юность", new List<CompetitionRequest>() },
                { "Чайковского", new List<CompetitionRequest>() },
                { "Мясковского", new List<CompetitionRequest>() },
                { "ТемоЦентр", new List<CompetitionRequest>() },
                { "Восточный", new List<CompetitionRequest>() },
                { "Московский центр технологической модернизации образования", new List<CompetitionRequest>() },
                { "Рыжий кит", new List<CompetitionRequest>() },
                { "Чудо-шашки", new List<CompetitionRequest>() },
                { "Замоскворечье", new List<CompetitionRequest>() },
                { "Шаляпина", new List<CompetitionRequest>() },
                { "PROговори", new List<CompetitionRequest>() },
                { "Ново-Переделкино", new List<CompetitionRequest>() },
                { "Детская школа искусств \"Центр\"", new List<CompetitionRequest>() },
                { "Хорошее настроение", new List<CompetitionRequest>() },
                { "Неотктрытые острова", new List<CompetitionRequest>() },
                { "Светланова", new List<CompetitionRequest>() },
                { "Юго-Запад", new List<CompetitionRequest>() },
                { "Щербинки", new List<CompetitionRequest>() },
                { "Откровение", new List<CompetitionRequest>() },
                { "Планета кидс", new List<CompetitionRequest>() },
                { "ГБОУДО Центр эстетического воспитания детей", new List<CompetitionRequest>() },
                { "ГБОУ ДО ЦДТ Срогино", new List<CompetitionRequest>() },
                { "Детская школа искусств «Центр»", new List<CompetitionRequest>() },
                { "ГБУДО Детская музыкальная школа №62", new List<CompetitionRequest>() },
                { "ГБОУДО ЦЭВД", new List<CompetitionRequest>() },
                { "ОКЦ\"СВАО\"", new List<CompetitionRequest>() },
                { "Гармония", new List<CompetitionRequest>() },
                { "ГБУДО Детская Музыкальная Школа №4", new List<CompetitionRequest>() },
                { "Дунаевского", new List<CompetitionRequest>() },
                { "Соловьева- Седого", new List<CompetitionRequest>() },
            };

            var moscowColleges = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "Экономико-технологический колледж №22", new List<CompetitionRequest>()},
                { "Колледж связи №54", new List<CompetitionRequest>()},
                { "Колледж предпринимательства №11", new List<CompetitionRequest>()},
                { "Педагогический колледж №18", new List<CompetitionRequest>()},
                { "Инженерно-техническая школа", new List<CompetitionRequest>()},
                { "Ипполитова-Иванова", new List<CompetitionRequest>()},
                { "РосНОУ", new List<CompetitionRequest>()},
                { "Колледж сферы услуг", new List<CompetitionRequest>()},
                { "ГБПОУ КСУ №3", new List<CompetitionRequest>()},
                { "Макарова", new List<CompetitionRequest>()},
                { "Максимчука", new List<CompetitionRequest>()},
                { "26 Кадр", new List<CompetitionRequest>()},
                { "ТК 21", new List<CompetitionRequest>()},
                { "1-Московский образовательный комплекс", new List<CompetitionRequest>()},
                { "Царицыно", new List<CompetitionRequest>()},
                { "Колледж градостроительства, транспорта и технологий №41", new List<CompetitionRequest>()},
                { "Юридический колледж", new List<CompetitionRequest>()},
                { "Росинка", new List<CompetitionRequest>()},
                { "Олимп-Плюс", new List<CompetitionRequest>()},
                { "Фаберже", new List<CompetitionRequest>()},
                { "ТСиТ № 29", new List<CompetitionRequest>()},
                { "Технологический колледж № 24", new List<CompetitionRequest>()},
                { "Панова", new List<CompetitionRequest>()},
                { "Шолохова", new List<CompetitionRequest>()},
                { "АНО ВО", new List<CompetitionRequest>()},
                { "Пансион воспитанниц МО РФ", new List<CompetitionRequest>()},
                { "Вострухина", new List<CompetitionRequest>()},
                { "КЖГТ", new List<CompetitionRequest>()},
                { "Технологический колледж № 34", new List<CompetitionRequest>()},
                { "МССВУ МО РФ", new List<CompetitionRequest>()},
                { "Колледж Архитектуры, Дизайна и Реинжиниринга №26", new List<CompetitionRequest>()},
            };

            var moscowKultura = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "Эврика-Бутово", new List<CompetitionRequest>()},
                { "Печатники", new List<CompetitionRequest>()},
                { "Сцена", new List<CompetitionRequest>()},
                { "Москвич", new List<CompetitionRequest>()},
                { "Бригантина", new List<CompetitionRequest>()},
                { "На Вешняковской", new List<CompetitionRequest>()},
                { "Лидер", new List<CompetitionRequest>()},
                { "Гагаринец", new List<CompetitionRequest>()},
                { "Танеева", new List<CompetitionRequest>()},
                { "Ратмир", new List<CompetitionRequest>()},
            };

            var moscowMunitsip = new Dictionary<string, List<CompetitionRequest>>()
            {
                { "Реабилитационно-образовательный центр №105", new List<CompetitionRequest>()},
                { "школа-интернат VIII", new List<CompetitionRequest>()},
                { "школа-интернат №73", new List<CompetitionRequest>()},
                { "школа-интернат №65", new List<CompetitionRequest>()},
                { "ЦИО \"Южный\"", new List<CompetitionRequest>()},
                { "Федеральное государственное казенное общеобразовательное учреждение", new List<CompetitionRequest>()},
                { "ВГИК", new List<CompetitionRequest>()},
                { "МГАВТ", new List<CompetitionRequest>()},
                { "ГКОУ КШИ №1", new List<CompetitionRequest>()},
                { "571", new List<CompetitionRequest>()},
                { "школа-интернат № 31", new List<CompetitionRequest>()},
                { "Каховские ромашки", new List<CompetitionRequest>()},
                { "школа-интернат № 102", new List<CompetitionRequest>()},


            };

            var unknownMoscowRequests = new List<CompetitionRequest>();

            try
            {
                for (int i = 0; i < moscowRequests.Count(); i++)
                {
                    var req = moscowRequests[i];
                    var find = false;

                    //Школ
                    foreach (KeyValuePair<string, List<CompetitionRequest>> ms in moscowSchool.ToArray())
                    {
                        if (req.EducationalOrganization.ToLower().Contains(ms.Key.ToLower()))
                        {
                            var temp = moscowSchool[ms.Key];
                            temp.Add(req);
                            moscowSchool[ms.Key] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа№158".ToLower()) ||
                            req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа158".ToLower()) ||
                            req.EducationalOrganization.ToLower().Replace(" ", "").Contains("№158".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowSchool["Школа №158"];
                            temp.Add(req);
                            moscowSchool["Школа №158"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа№108".ToLower()) ||
                            req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа108".ToLower()))
                        {
                            var temp = moscowSchool["Школа №108"];
                            temp.Add(req);
                            moscowSchool["Школа №108"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа№17".ToLower()) ||
                            req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа17".ToLower()))
                        {
                            var temp = moscowSchool["Школа №17"];
                            temp.Add(req);
                            moscowSchool["Школа №17"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа№90".ToLower()) ||
                           req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа90".ToLower()))
                        {
                            var temp = moscowSchool["Школа №90"];
                            temp.Add(req);
                            moscowSchool["Школа №90"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа№15".ToLower()) ||
                           req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Школа15".ToLower()))
                        {
                            var temp = moscowSchool["Школа №15"];
                            temp.Add(req);
                            moscowSchool["Школа №15"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("НЕТ".ToLower()) && req.ChiefEmail.ToLower().Contains("anikeychik_anna@mail.ru".ToLower()))
                        {
                            var temp = moscowSchool["Карамзина"];
                            temp.Add(req);
                            moscowSchool["Карамзина"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("НЕТ".ToLower()) && req.ChiefEmail.ToLower().Contains("b-elena1@yandex.ru".ToLower()))
                        {
                            var temp = moscowSchool["Судакова"];
                            temp.Add(req);
                            moscowSchool["Судакова"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("marina.kuznecova@mail.ru".ToLower()))
                        {
                            var temp = moscowSchool["1590"];
                            temp.Add(req);
                            moscowSchool["1590"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("salikovael2065@gmail.com".ToLower()))
                        {
                            var temp = moscowSchool["2065"];
                            temp.Add(req);
                            moscowSchool["2065"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("malena1609@yandex.ru".ToLower()))
                        {
                            var temp = moscowSchool["1985"];
                            temp.Add(req);
                            moscowSchool["1985"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && (req.ChiefEmail.ToLower().Contains("chubova.galina@gmail.com".ToLower()) || req.ChiefEmail.ToLower().Contains("seryscheva.natasha2012@yandex.ru".ToLower())))
                        {
                            var temp = moscowSchool["Судакова"];
                            temp.Add(req);
                            moscowSchool["Судакова"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("olyabe@bk.ru".ToLower()))
                        {
                            var temp = moscowSchool["626"];
                            temp.Add(req);
                            moscowSchool["626"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("3108".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowSchool["3108"];
                            temp.Add(req);
                            moscowSchool["3108"] = temp;
                            find = true;
                            break;
                        }
                    }

                    //Учреждений дополнительного образования
                    foreach (KeyValuePair<string, List<CompetitionRequest>> ms in moscowDOP.ToArray())
                    {
                        if (req.EducationalOrganization.ToLower().Replace("ё", "e").Contains(ms.Key.ToLower()))
                        {
                            var temp = moscowDOP[ms.Key];
                            temp.Add(req);
                            moscowDOP[ms.Key] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Косорева".ToLower()))
                        {
                            var temp = moscowDOP["Косарева"];
                            temp.Add(req);
                            moscowDOP["Косарева"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Детско-Юношескийцентр".ToLower()))
                        {
                            var temp = moscowDOP["Щербинка"];
                            temp.Add(req);
                            moscowDOP["Щербинка"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Воробьёвы горы".ToLower())
                            || (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("lev1976@yandex.ru".ToLower())))
                        {
                            var temp = moscowDOP["Воробьевы горы"];
                            temp.Add(req);
                            moscowDOP["Воробьевы горы"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("ЗДТДиМ".ToLower()))
                        {
                            var temp = moscowDOP["Зеленоградский"];
                            temp.Add(req);
                            moscowDOP["Зеленоградский"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("haritonov.f@gmail.com".ToLower()))
                        {
                            var temp = moscowDOP["Чудо-шашки"];
                            temp.Add(req);
                            moscowDOP["Чудо-шашки"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Замоскворечье".ToLower()))
                        {
                            var temp = moscowDOP["Замоскворечье"];
                            temp.Add(req);
                            moscowDOP["Замоскворечье"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Миусах".ToLower()))
                        {
                            var temp = moscowDOP["Миуссах"];
                            temp.Add(req);
                            moscowDOP["Миуссах"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("планета кидс".ToLower()))
                        {
                            var temp = moscowDOP["Планета кидс"];
                            temp.Add(req);
                            moscowDOP["Планета кидс"] = temp;
                            find = true;
                            break;
                        }
                    }

                    //Колледжей и техникумов
                    foreach (KeyValuePair<string, List<CompetitionRequest>> ms in moscowColleges.ToArray())
                    {
                        if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains(ms.Key.Replace(" ", "").ToLower()))
                        {
                            var temp = moscowColleges[ms.Key];
                            temp.Add(req);
                            moscowColleges[ms.Key] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Колледж связи 54".Replace(" ", "").ToLower()))
                        {
                            var temp = moscowColleges["Колледж связи №54"];
                            temp.Add(req);
                            moscowColleges["Колледж связи №54"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("тК № 21".Replace(" ", "").ToLower())
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Технологический колледж № 21".Replace(" ", "").ToLower()))
                        {
                            var temp = moscowColleges["ТК 21"];
                            temp.Add(req);
                            moscowColleges["ТК 21"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Технологический колледж № 24".Replace(" ", "").ToLower()))
                        {
                            var temp = moscowColleges["Технологический колледж № 24"];
                            temp.Add(req);
                            moscowColleges["Технологический колледж № 24"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("1-МОК".Replace(" ", "").ToLower())
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("1-й МОК".Replace(" ", "").ToLower())
                            || (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Московский государственный образовательный комплекс".Replace(" ", "").ToLower()))
                            || (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Московский образовательный комплекс".Replace(" ", "").ToLower()))
                            )
                        {
                            var temp = moscowColleges["1-Московский образовательный комплекс"];
                            temp.Add(req);
                            moscowColleges["1-Московский образовательный комплекс"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Пансион воспитанниц".Replace(" ", "").ToLower()))
                        {
                            var temp = moscowColleges["Пансион воспитанниц МО РФ"];
                            temp.Add(req);
                            moscowColleges["Пансион воспитанниц МО РФ"] = temp;
                            find = true;
                            break;
                        }

                        else if (req.EducationalOrganization.ToLower().Contains("ГБПОУ КСУ №32".ToLower())
                            || req.EducationalOrganization.ToLower().Contains("КСУ№32".ToLower()))
                        {
                            var temp = moscowColleges["Колледж сферы услуг"];
                            temp.Add(req);
                            moscowColleges["Колледж сферы услуг"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("ГБПОУ ЭТК № 22".ToLower())
                            || req.EducationalOrganization.ToLower().Contains("КСУ№22".ToLower()))
                        {
                            var temp = moscowColleges["Экономико-технологический колледж №22"];
                            temp.Add(req);
                            moscowColleges["Экономико-технологический колледж №22"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("вострухина".ToLower())
                            || req.EducationalOrganization.ToLower().Contains("кс54".ToLower()))
                        {
                            var temp = moscowColleges["Вострухина"];
                            temp.Add(req);
                            moscowColleges["Вострухина"] = temp;
                            find = true;
                            break;
                        }
                    }

                    //Учреждений культуры
                    foreach (KeyValuePair<string, List<CompetitionRequest>> ms in moscowKultura.ToArray())
                    {
                        if (req.EducationalOrganization.ToLower().Contains(ms.Key.ToLower()))
                        {
                            var temp = moscowKultura[ms.Key];
                            temp.Add(req);
                            moscowKultura[ms.Key] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Нет".ToLower()) && req.ChiefEmail.ToLower().Contains("leufegs@ya.ru".ToLower()))
                        {
                            var temp = moscowSchool["На Вешняковской"];
                            temp.Add(req);
                            moscowSchool["На Вешняковской"] = temp;
                            find = true;
                            break;
                        }
                    }

                    //Муниципальных учереждений
                    foreach (KeyValuePair<string, List<CompetitionRequest>> ms in moscowMunitsip.ToArray())
                    {
                        if (req.EducationalOrganization.ToLower().Contains(ms.Key.ToLower()))
                        {
                            var temp = moscowMunitsip[ms.Key];
                            temp.Add(req);
                            moscowMunitsip[ms.Key] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Реабилитационно-образовательный центр №105".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["Реабилитационно-образовательный центр №105"];
                            temp.Add(req);
                            moscowMunitsip["Реабилитационно-образовательный центр №105"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("СКОШИ 73".ToLower().Replace(" ", ""))
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("СКОШИ № 73".ToLower().Replace(" ", ""))
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("школа-интернат №73".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["школа-интернат №73"];
                            temp.Add(req);
                            moscowMunitsip["школа-интернат №73"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("школа-интернат №65".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["школа-интернат №65"];
                            temp.Add(req);
                            moscowMunitsip["школа-интернат №65"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("школа VIII".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["школа-интернат VIII"];
                            temp.Add(req);
                            moscowMunitsip["школа-интернат VIII"] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Contains("Москва".ToLower()) && req.ChiefEmail.ToLower().Contains("botega1@yandex.ru".ToLower())
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Центр Инклюзивного Образования \"Южный\"".ToLower().Replace(" ", ""))
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("ЦИО\"Южный\"".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["ЦИО \"Южный\""];
                            temp.Add(req);
                            moscowMunitsip["ЦИО \"Южный\""] = temp;
                            find = true;
                            break;
                        }
                        else if (req.EducationalOrganization.ToLower().Replace(" ", "").Contains("Кадетская школа-интернат №1".ToLower().Replace(" ", ""))
                            || req.EducationalOrganization.ToLower().Replace(" ", "").Contains("КШИ №1".ToLower().Replace(" ", "")))
                        {
                            var temp = moscowMunitsip["ГКОУ КШИ №1"];
                            temp.Add(req);
                            moscowMunitsip["ГКОУ КШИ №1"] = temp;
                            find = true;
                            break;
                        }

                    }

                    if (!find)
                    {
                        unknownMoscowRequests.Add(req);
                    }
                }
            }
            catch (Exception ex)
            {
                parseError = ex.Message;
            }

            result["Московских образовательных организаций"] = moscowSchool.Where(x => x.Value.Count > 0).Count() + moscowDOP.Where(x => x.Value.Count > 0).Count() + moscowColleges.Where(x => x.Value.Count > 0).Count() + moscowKultura.Where(x => x.Value.Count > 0).Count() + moscowMunitsip.Where(x => x.Value.Count > 0).Count(); ;
            result["Учреждений дополнительного образования"] = moscowDOP.Where(x => x.Value.Count > 0).Count();
            result["Школ"] = moscowSchool.Where(x => x.Value.Count > 0).Count();
            result["Колледжей и техникумов"] = moscowColleges.Where(x => x.Value.Count > 0).Count();
            result["Учреждений культуры"] = moscowKultura.Where(x => x.Value.Count > 0).Count();
            result["Муниципальных учреждений"] = moscowMunitsip.Where(x => x.Value.Count > 0).Count();

            #endregion
        }
    }
}