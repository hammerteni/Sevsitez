using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using site.classes;
using site.classesHelp;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;

namespace site
{
    /// <summary>
    /// Сводное описание для DataService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    [System.Web.Script.Services.ScriptService]
    public class DataService : WebService
    {
        private string ck = "325kjghksfdjgher9874354idshfdgkjsht348576sjefkhdsjkg";

        #region Метод EducationOrganizationData(string searchString)

        /// <summary>
        /// Метод возвращает список образовательных учреждений с привязкой к региону, областям, городам.
        /// </summary>
        /// <param name="searchString">Наименование образовательного учреждения</param>
        /// <returns>Возвращает список структур образовательных учреждений с привязкой к региону, областям, городам</returns>
        [WebMethod(EnableSession = true)]
        [HttpPost]
        public async Task<ActionResult> EducationOrganizationData(string searchString = "")
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice EducationOrganizationData")) return null;

            #region Основной код

            try
            {
                var res = await new EducationOrganizationWork().GetEducationOrganizationList(searchString.Trim());
               
                return new JsonResult() { Data = res };
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return null;

            #endregion
            
        }

        #endregion

        #region Метод RegionAreaCitiesData(string searchString)

        /// <summary>
        /// Метод возвращает список Регионов, областей, городов.
        /// </summary>
        /// <param name="searchString">Регион или Область или Город</param>
        /// <returns>Возвращает список структур Регионов, областей, городов/returns>
        [WebMethod(EnableSession = true)]
        [HttpPost]
        public async Task<ActionResult> RegionAreaCitiesData(string searchString = "")
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice RegionAreaCitiesData")) return null;

            #region Основной код

            try
            {
                var res = await new EducationOrganizationWork().GetRegionAreaCitiesList(searchString.Trim());
                return new JsonResult() { Data = res };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;

            #endregion

        }

        #endregion

        #region Метод SentRequest(FormRequest formRequest)

        /// <summary>Метод для отсылки почты через форму НАПИСАТЬ НАМ</summary>
        /// <param name="formRequest">структура данных класса FormRequest</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string SentRequest(FormRequest formRequest)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice SentRequest")) return "";

            string result = "ok";

            //Проверка значений
            if (formRequest.Name.Trim() == "") return "err";
            if (formRequest.Text.Trim() == "") return "err";
            if (formRequest.Mail.Trim() == "") return "err";
            string sPattern = "^([a-z0-9_-]+\\.)*[a-z0-9_-]+@[a-z0-9_-]+(\\.[a-z0-9_-]+)*\\.[a-z]{2,6}$";
            if (!Regex.IsMatch(formRequest.Mail, sPattern)) return "err";

            //отправляем письмо администратору сайта
            SendMailClass sendmail = new SendMailClass();
            string mailbodyAdm = "Поступило обращение на сайте." + "<br /><br />" + "Данные по обращению:" + "<br /><br />" +
                              "<table><tr><td style='padding: 0 9px 5px 9px;'>" +
                              "<span style='color:blue; font-weight:bold;'>Фамилия:</span>" +
                              "</td><td style='padding: 0 9px 5px 9px;'>" +
                              formRequest.Fam +
                              "</td></tr>" +
                              "<tr><td style='padding: 0 9px 5px 9px;'>" +
                              "<span style='color:blue; font-weight:bold;'>Имя:</span>" +
                              "</td><td style='padding: 0 9px 5px 9px;'>" +
                              formRequest.Name +
                              "</td></tr>" +
                              "<tr><td style='padding: 0 9px 5px 9px;'>" +
                              "<span style='color:blue; font-weight:bold;'>Телефон:</span>" +
                              "</td><td style='padding: 0 9px 5px 9px;'>" +
                              formRequest.Tel +
                              "</td></tr>" +
                              "<tr><td style='padding: 0 9px 5px 9px;'>" +
                              "<span style='color:blue; font-weight:bold;'>Почта:</span>" +
                              "</td><td style='padding: 0 9px 5px 9px;'>" +
                              formRequest.Mail +
                              "</td></tr>" +
                              "<tr><td style = 'padding: 0 9px 5px 9px;' > " +
                              "<span style='color:blue; font-weight:bold;'>Текст:</span>" +
                              "</td><td style='padding: 0 9px 5px 9px;'>" +
                              formRequest.Text +
                              "</td></tr></table>" +
                              "<br />Это письмо сформировано автоматически, отвечать на него не нужно.";
            var config = new ConfigFile();
            string adminMail = config.GetParam("admmail", true);
            if (adminMail == "-1") return "err";

            //если письмо не отослано, то..
            if (!sendmail.SendMail(adminMail, "Письмо о поступлении обращения на сайте sevastopolets-moskva.ru", mailbodyAdm))
            {
                result = "err";
            }


            return result;
        }

        #endregion

        #region Метод UpdateRequestData(string reqId, string newValue, string fieldName, string code)

        /// <summary>Метод для обновления данных о заявке в консоли управления.  Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestData(string reqId, string newValue, string fieldName, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestData")) return "";

            string result = "ok";
            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            //if (newValue.Trim() == "") return "empty";
            if (fieldName == "Age")    //проверка правильности ввода даты рождения
            {
                if (!string.IsNullOrEmpty(newValue) && newValue.Contains("."))
                {
                    DateTime dtTmp;
                    if (!DateTime.TryParse(newValue, out dtTmp))
                    {
                        return "date_false";
                    }
                }
                else
                    return "date_false";
            }
            if (fieldName == "Weight")                           //проверка правильности ввода индивидуального веса
            {
                if (StringToNum.ParseInt(newValue) == -1)
                {
                    return "weight_false";
                }
            }
            if (fieldName == "AgeСategory")                     //проверка правильности ввода возрастной категории
            {
                if (newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group5) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group6) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group7_9) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group8_11) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group10_13) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group12_15) &&
                    newValue != ""
                    )
                {
                    return "agecategory_false";
                }
            }
            if (fieldName == "PartyCount")
            {
                int tmpNum = StringToNum.ParseInt(newValue);
                if (tmpNum < 1)
                {
                    return "partycount_false";
                }
            }
            if (fieldName == "Points")
            {
                int points = 0;
                int.TryParse(newValue, out points);
                if ((points < 0 || points > 46) && points != 100) { 
                    return "points_false";
                }
            }
            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            int res = work.UpdateField(reqId, fieldName, newValue);
            if (res == -1) return "err";

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDataGroup(string reqId, string newValue, string fieldName, string position, string code)

        /// <summary>Метод для обновления данных о заявке в консоли управления (для обновления списочных параметров Fios, Agies, Weights).  
        /// Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="position">позиция для вставки нового значения в списке вида - параметр|параметр|параметр|параметр</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDataGroup(string reqId, string newValue, string fieldName, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDataGroup")) return "";

            string result = "ok";
            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            int pos = StringToNum.ParseInt(position);
            if (pos == -1) return "err";
            //if (newValue.Trim() == "") return "empty";
            if (fieldName == "Fios" || fieldName == "Fios_1" || fieldName == "ChiefFios")
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return "empty_fio";
                }
                if (!CompetitonWorkCommon.IsFioOk(newValue))
                {
                    return "incorrect_fio";
                }
            }

            if (fieldName == "Agies" || fieldName == "Agies_1")    //проверка правильности ввода даты рождения
            {
                if (!string.IsNullOrEmpty(newValue) && newValue.Contains("."))
                {
                    DateTime dtTmp;
                    if (!DateTime.TryParse(newValue, out dtTmp))
                    {
                        return "date_false";
                    }
                }
                else
                    return "date_false";
            }
            if (fieldName == "Weights")                           //проверка правильности ввода индивидуального веса
            {
                if (StringToNum.ParseInt(newValue) == -1)
                {
                    return "weight_false";
                }
            }
            if (fieldName == "Schools" || fieldName == "Schools_1")
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return "empty_school";
                }
            }
            if (fieldName == "ClassRooms" || fieldName == "ClassRooms_1")
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return "empty_classroom";
                }
            }
            if (fieldName == "ChiefPositions")
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return "empty_position";
                }
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            int res = work.UpdateFieldGroup(reqId, fieldName, newValue, pos);
            if (res == -1) return "err";

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDataGroup_Ext(.....)

        /// <summary>Метод для обновления данных о заявке в консоли управления 
        /// (для обновления списочных параметров Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results).  
        /// Используется на странице редактирования заявки competitionsone.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="position">позиция для вставки нового значения в массиве вида - [значение]|[значение]|...</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDataGroup_Ext(string reqId, string newValue, string fieldName, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDataGroup_Ext")) return "";

            string result = "ok";

            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            int pos = StringToNum.ParseInt(position);
            if (pos == -1) return "err";

            if (fieldName != "Fios" && fieldName != "Agies" && fieldName != "AgeСategories" && fieldName != "Weights" &&
                fieldName != "Kvalifications" && fieldName != "Programms" && fieldName != "Results")
            {
                return "err";
            }

            if (fieldName != "Results")     // значение поля Результат может быть пустым
            {
                if (string.IsNullOrEmpty(newValue))
                    return "empty_value";
            }
            if (fieldName == "Agies")    // проверка правильности ввода даты рождения
            {
                if (!string.IsNullOrEmpty(newValue) && newValue.Contains("."))
                {
                    DateTime dtTmp;
                    if (!DateTime.TryParse(newValue, out dtTmp))
                    {
                        return "date_false";
                    }
                }
                else
                    return "date_false";
            }
            if (fieldName == "Fios")
            {
                if (!CompetitonWorkCommon.IsFioOk(newValue))
                {
                    return "incorrect_fio";
                }
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            
            var obj = work.GetOneRequest(reqId);
            if (((CompetitionRequest)obj) == null)
            {
                return "err";
            }

            if (!((CompetitionRequest)obj).UpdateFormattedStringProperty(newValue, fieldName, pos))
            {
                return "err";
            }

            long res = work.UpdateOneRequest((CompetitionRequest)obj);
            if (res == -1)
            {
                return "err";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestPublish(string reqId, string checker, string code)

        /// <summary>Метод для публикации/снятия с публикации работы.  Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="checker">0 - ещё неопубликована, 1 - уже опубликована</param>
        /// <param name="code">код авторизации</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestPublish(string reqId, string checker, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestPublish")) return "";

            string result = "err";

            #region Проверка присланных значений

            if (code != ck) return result;
            if (StringToNum.ParseInt(reqId) == -1) return result;
            if (checker != "0" && checker != "1") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            int publish = 0;
            if (checker == "0") publish = 1;
            int res = work.UpdateApproved(long.Parse(reqId), publish);
            if (res == -1 || res == 0)
            {
                return result;
            }
            else
            {
                if (checker == "0")
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }

            #endregion
        }

        #endregion
        #region Метод UpdateRequestCheckedAdmin(string reqId, string checker, string code)

        /// <summary>Метод для простановки признака проверено администратором.  Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="checker">0 - не проверен, 1 - проверен</param>
        /// <param name="code">код авторизации</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestCheckedAdmin(string reqId, string checker, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestCheckedAdmin")) return "";

            string result = "err";

            #region Проверка присланных значений

            if (code != ck) return result;
            if (StringToNum.ParseInt(reqId) == -1) return result;
            if (checker != "0" && checker != "1") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            int checkedAdmin = 0;
            if (checker == "0") checkedAdmin = 1;
            int res = work.UpdateCheckedAdmin(long.Parse(reqId), checkedAdmin);
            if (res == -1 || res == 0)
            {
                return result;
            }
            else
            {
                if (checker == "0")
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }

            #endregion
        }

        #endregion
        
        #region Метод UpdateRequestAddNewParty(...)

        /// <summary>Метод для добавления нового участника в заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО участника</param>
        /// <param name="age">дата рождения участника</param>
        /// <param name="weight">вес участника</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewParty(string reqId, string fio, string age, string weight, string cname, string code, string schools, string classRooms, string protocolType)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewParty")) return "";

            string result = "err";
            fio = fio.Trim();
            age = age.Trim();
            weight = weight.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (age == "") return "no_age";

            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return "age_err";
            }

            if (cname == EnumsHelper.GetSportCode(Sport.self))
            {
                if (weight == "") return "no_weight";
                if (StringToNum.ParseInt(weight) == -1) return "weight_err";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.AddPartyToRequest(reqId, fio, age, cname, schools, classRooms, protocolType, weight))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewPartyNoWeight(...)

        /// <summary>Метод для добавления нового участника в заявку (без веса). Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО участника</param>
        /// <param name="age">дата рождения участника</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <param name="code">код проверки</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewPartyNoWeight(string reqId, string fio, string age, string cname, string code, string schools, string classRooms, string protocolType)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewPartyNoWeight")) return "";

            string result = "err";
            fio = fio.Trim();
            age = age.Trim();
            schools = string.IsNullOrEmpty(schools) ? "" : schools.Trim();
            classRooms = string.IsNullOrEmpty(classRooms) ? "" : classRooms.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (age == "") return "no_age";

            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return "age_err";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.AddPartyToRequest(reqId, fio, age, cname, schools, classRooms, protocolType, null))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewParty_Ext(...)

        /// <summary>Метод для добавления нового участника в заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="age">дата рождения</param>
        /// <param name="ageCategory">возрастная категория</param>
        /// <param name="weight">весовая категория</param>
        /// <param name="kvalif">квалификация</param>
        /// <param name="program">программа выступления</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewParty_Ext(string reqId, string fio, string age, string ageCategory, string weight, string kvalif, string program, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewParty_Ext")) return "";

            string result = "ok";

            fio = fio.Trim();
            age = age.Trim();
            ageCategory = ageCategory.Trim();
            weight = weight.Trim();
            kvalif = kvalif.Trim();
            program = program.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";

            if (fio == "") return "empty_value";
            if (age == "") return "empty_value";
            if (ageCategory == "") return "empty_value";
            if (weight == "") return "empty_value";
            if (kvalif == "") return "empty_value";
            if (program == "") return "empty_value";

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return "incorrect_fio";
            }

            if (age.Contains("."))    // проверка правильности ввода даты рождения
            {
                DateTime dtTmp;
                if (!DateTime.TryParse(age, out dtTmp))
                {
                    return "date_false";
                }
            }
            else
            {
                return "date_false";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();

            var obj = work.GetOneRequest(reqId);
            if (((CompetitionRequest)obj) == null)
            {
                return "err";
            }

            ((CompetitionRequest)obj).AddParty(fio, age, ageCategory, weight, kvalif, program);

            long res = work.UpdateOneRequest((CompetitionRequest)obj);
            if (res == -1)
            {
                return "err";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelParty(...)

        /// <summary>Метод для удаления участника из заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelParty(string reqId, string position, string cname, string code, string protocolType)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelParty")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;
            if (cname == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.DelPartyFromRequest(reqId, position, cname, protocolType))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelParty_Ext(...)

        /// <summary>Метод для удаления участника из заявки (со списочными параметрами Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results).
        /// Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в форматированных строках данных</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelParty_Ext(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelParty_Ext")) return "";

            string result = "ok";

            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseLong(reqId) == -1) return "err";
            if (StringToNum.ParseInt(position) == -1) return "err";

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();

            var obj = work.GetOneRequest(reqId);
            if (((CompetitionRequest)obj) == null)
            {
                return "err";
            }

            ((CompetitionRequest)obj).DeleteParty(int.Parse(position));

            long res = work.UpdateOneRequest(((CompetitionRequest)obj));
            if (res == -1)
            {
                return "err";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewChief(...)

        /// <summary>Метод для добавления нового руководителя в заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="position">должность</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewChief(string reqId, string fio, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewChief")) return "";

            string result = "err";
            fio = fio.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (position == "") return "no_position";

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return "incorrect_fio";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.AddChiefToRequest(reqId, fio, position))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelChief(...)

        /// <summary>Метод для удаления участника из заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelChief(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelChief")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.DelChiefFromRequest(reqId, position))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewAthor(...)

        /// <summary>Метод для добавления нового автора коллекции в заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewAthor(string reqId, string fio, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewAthor")) return "";

            string result = "err";
            fio = fio.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";

            string sPattern = "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$";
            if (!Regex.IsMatch(fio, sPattern))
            {
                return "incorrect_fio";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.AddAthorToRequest(reqId, fio))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelAthor(...)

        /// <summary>Метод для удаления автора из заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelAthor(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelAthor")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.DelAthorFromRequest(reqId, position))
            {
                result = "ok";
            }

            #endregion

            return result;
        }

        #endregion

        #region Метод UpdateRequestData_Arch(string reqId, string newValue, string fieldName, string code)

        /// <summary>Метод для обновления данных в архивной заявке в консоли управления.  Используется на странице просмотра всех заявок - competitionsarch.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestData_Arch(string reqId, string newValue, string fieldName, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestData_Arch")) return "";

            string result = "ok";
            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            //if (newValue.Trim() == "") return "empty";
            if (fieldName == "Age" && newValue.Contains("."))    //проверка правильности ввода даты рождения
            {
                DateTime dtTmp;
                if (!DateTime.TryParse(newValue, out dtTmp))
                {
                    return "date_false";
                }
            }
            if (fieldName == "Weight")                           //проверка правильности ввода индивидуального веса
            {
                if (StringToNum.ParseInt(newValue) == -1)
                {
                    return "weight_false";
                }
            }
            if (fieldName == "AgeСategory")                     //проверка правильности ввода возрастной категории
            {
                if (newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group5) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group6) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015) &&
                    newValue != EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016) &&
                    newValue != ""
                    )
                {
                    return "agecategory_false";
                }
            }
            if (fieldName == "PartyCount")
            {
                int tmpNum = StringToNum.ParseInt(newValue);
                if (tmpNum < 1)
                {
                    return "partycount_false";
                }
            }
            if (fieldName == "Points")
            {
                int points = 0;
                int.TryParse(newValue, out points);
                if ((points < 0 || points > 36) && points != 100) 
                {
                    return "points_false";
                }
            }
            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            int res = work.UpdateField(reqId, fieldName, newValue);
            if (res == -1) return "err";

            if (HttpContext.Current.Session["authperson"] != null)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Поле - " + fieldName + ", новое значение - '" + newValue + "'. Удалил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Поле - " + fieldName + ", новое значение - '" + newValue + "'. Удалил - НЕОПРЕДЕЛЕНО");
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDataGroup_Arch(string reqId, string newValue, string fieldName, string position, string code)

        /// <summary>Метод для обновления данных в архивной заявке в консоли управления (для обновления списочных параметров Fios, Agies, Weights).  
        /// Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="position">позиция для вставки нового значения в списке вида - параметр|параметр|параметр|параметр</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDataGroup_Arch(string reqId, string newValue, string fieldName, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDataGroup_Arch")) return "";

            string result = "ok";
            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            int pos = StringToNum.ParseInt(position);
            if (pos == -1) return "err";
            //if (newValue.Trim() == "") return "empty";
            if (fieldName == "Agies" && newValue.Contains("."))    //проверка правильности ввода даты рождения
            {
                DateTime dtTmp;
                if (!DateTime.TryParse(newValue, out dtTmp))
                {
                    return "date_false";
                }
            }
            if (fieldName == "Weights")                           //проверка правильности ввода индивидуального веса
            {
                if (StringToNum.ParseInt(newValue) == -1)
                {
                    return "weight_false";
                }
            }
            if (fieldName == "ChiefFios")
            {
                if (newValue == "")
                {
                    return "empty_fio";
                }
                if (!CompetitonWorkCommon.IsFioOk(newValue))
                {
                    return "incorrect_fio";
                }
            }
            if (fieldName == "ChiefPositions")
            {
                if (newValue == "")
                {
                    return "empty_position";
                }
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            int res = work.UpdateFieldGroup(reqId, fieldName, newValue, pos);
            if (res == -1) return "err";

            if (HttpContext.Current.Session["authperson"] != null)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Поле - " + fieldName + ", новое значение - '" + newValue + "', позиция - " + position + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Поле - " + fieldName + ", новое значение - '" + newValue + "', позиция - " + position + ". Изменил - НЕОПРЕДЕЛЕНО");
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDataGroup_Ext_Arch(.....)

        /// <summary>Метод для обновления данных об архивной заявке в консоли управления 
        /// (для обновления списочных параметров Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results).  
        /// Используется на странице редактирования заявки competitionsone.aspx</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="newValue">новое значение поля</param>
        /// <param name="fieldName">имя поля</param>
        /// <param name="position">позиция для вставки нового значения в массиве вида - [значение]|[значение]|...</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDataGroup_Ext_Arch(string reqId, string newValue, string fieldName, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDataGroup_Ext_Arch")) return "";

            string result = "ok";

            newValue = newValue.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            int pos = StringToNum.ParseInt(position);
            if (pos == -1) return "err";

            if (fieldName != "Fios" && fieldName != "Agies" && fieldName != "AgeСategories" && fieldName != "Weights" &&
                fieldName != "Kvalifications" && fieldName != "Programms" && fieldName != "Results")
            {
                return "err";
            }

            if (fieldName != "Results")     // значение поля Результат может быть пустым
            {
                if (newValue == "") return "empty_value";
            }
            if (fieldName == "Agies" && newValue.Contains("."))    // проверка правильности ввода даты рождения
            {
                DateTime dtTmp;
                if (!DateTime.TryParse(newValue, out dtTmp))
                {
                    return "date_false";
                }
            }
            if (fieldName == "Fios")
            {
                if (!CompetitonWorkCommon.IsFioOk(newValue))
                {
                    return "incorrect_fio";
                }
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();

            var obj = work.GetOneRequest(reqId);
            if (obj == null)
            {
                return "err";
            }

            if (!((CompetitionRequest_Arch)obj).UpdateFormattedStringProperty(newValue, fieldName, pos))
            {
                return "err";
            }

            string[] arr = ((CompetitionRequest_Arch)obj).GetFioBirthAgecatWeightKvalifProgamResult_One(int.Parse(position));
            string tmp = "";
            if (arr.Length > 0) tmp = arr[0];

            long res = work.UpdateOneRequest((CompetitionRequest_Arch)obj);
            if (res == -1)
            {
                return "err";
            }

            #endregion

            if (HttpContext.Current.Session["authperson"] != null)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Изменено значение поля '" + fieldName + "' на новое значение '" + newValue +
                               "', для участника - '" + tmp + "'. Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Изменено значение поля '" + fieldName + "' на новое значение '" + newValue +
                               "', для участника - '" + tmp + "'. Изменил - НЕОПРЕДЕЛЕНО");
            }

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewParty_Arch(...)

        /// <summary>Метод для добавления нового участника в архивную заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО участника</param>
        /// <param name="age">дата рождения участника</param>
        /// <param name="weight">вес участника</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewParty_Arch(string reqId, string fio, string age, string weight, string cname, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewParty_Arch")) return "";

            string result = "err";
            fio = fio.Trim();
            age = age.Trim();
            weight = weight.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (age == "") return "no_age";

            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return "age_err";
            }

            if (cname == EnumsHelper.GetSportCode(Sport.self))
            {
                if (weight == "") return "no_weight";
                if (StringToNum.ParseInt(weight) == -1) return "weight_err";
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            if (work.AddPartyToRequest(reqId, fio, age, weight, cname))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник с ФИО - " + fio + ", возраст - '" + age + "', вес - " + weight + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник с ФИО - " + fio + ", возраст - '" + age + "', вес - " + weight + ". Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewPartyNoWeight_Arch(...)

        /// <summary>Метод для добавления нового участника в архивную заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО участника</param>
        /// <param name="age">дата рождения участника</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewPartyNoWeight_Arch(string reqId, string fio, string age, string cname, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewPartyNoWeight_Arch")) return "";

            string result = "err";
            fio = fio.Trim();
            age = age.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (age == "") return "no_age";

            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return "age_err";
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            if (work.AddPartyToRequest(reqId, fio, age, cname))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник с ФИО - " + fio + ", возраст - '" + age + "'. Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник с ФИО - " + fio + ", возраст - '" + age + "'. Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewParty_Ext_Arch(...)

        /// <summary>Метод для добавления нового участника в архивную заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="age">дата рождения</param>
        /// <param name="ageCategory">возрастная категория</param>
        /// <param name="weight">весовая категория</param>
        /// <param name="kvalif">квалификация</param>
        /// <param name="program">программа выступления</param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewParty_Ext_Arch(string reqId, string fio, string age, string ageCategory, string weight, string kvalif, string program, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewParty_Ext_Arch")) return "";

            string result = "ok";

            fio = fio.Trim();
            age = age.Trim();
            ageCategory = ageCategory.Trim();
            weight = weight.Trim();
            kvalif = kvalif.Trim();
            program = program.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";

            if (fio == "") return "empty_value";
            if (age == "") return "empty_value";
            if (ageCategory == "") return "empty_value";
            if (weight == "") return "empty_value";
            if (kvalif == "") return "empty_value";
            if (program == "") return "empty_value";

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return "incorrect_fio";
            }

            if (age.Contains("."))    // проверка правильности ввода даты рождения
            {
                DateTime dtTmp;
                if (!DateTime.TryParse(age, out dtTmp))
                {
                    return "date_false";
                }
            }
            else
            {
                return "date_false";
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();

            var obj = work.GetOneRequest(reqId);
            if (obj == null)
            {
                return "err";
            }

            ((CompetitionRequest_Arch)obj).AddParty(fio, age, ageCategory, weight, kvalif, program);

            long res = work.UpdateOneRequest((CompetitionRequest_Arch)obj);
            if (res == -1)
            {
                return "err";
            }

            #endregion

            if (HttpContext.Current.Session["authperson"] != null)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник - '" + fio + "'. Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен участник - '" + fio + "'. Изменил - НЕОПРЕДЕЛЕНО");
            }

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelParty_Arch(...)

        /// <summary>Метод для удаления участника из архивной заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="cname">условное наименование Конкурса</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelParty_Arch(string reqId, string position, string cname, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelParty_Arch")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();
            cname = cname.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;
            if (cname == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            if (work.DelPartyFromRequest(reqId, position, cname))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален участник с позицией - " + position + "'. Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален участник с позицией - " + position + "'. Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelParty_Ext_Arch(...)

        /// <summary>Метод для удаления участника из архивной заявки (со списочными параметрами Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results).
        /// Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в форматированных строках данных</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelParty_Ext_Arch(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelParty_Ext_Arch")) return "";

            string result = "ok";

            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return "err";
            if (StringToNum.ParseLong(reqId) == -1) return "err";
            if (StringToNum.ParseInt(position) == -1) return "err";

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();

            var obj = work.GetOneRequest(reqId);
            if (obj == null)
            {
                return "err";
            }

            string[] arr = ((CompetitionRequest_Arch)obj).GetFioBirthAgecatWeightKvalifProgamResult_One(int.Parse(position));
            string tmp = "";
            if (arr.Length > 0) tmp = arr[0];

            ((CompetitionRequest_Arch)obj).DeleteParty(int.Parse(position));

            long res = work.UpdateOneRequest((CompetitionRequest_Arch)obj);
            if (res == -1)
            {
                return "err";
            }

            #endregion

            if (HttpContext.Current.Session["authperson"] != null)
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален участник - '" + tmp + "'. Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
            }
            else
            {
                DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален участник - '" + tmp + "'. Изменил - НЕОПРЕДЕЛЕНО");
            }

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewChief_Arch(...)

        /// <summary>Метод для добавления нового руководителя в архивную заявку. Используется на странице редактирования заявки - competitiononearch.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="position">должность</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewChief_Arch(string reqId, string fio, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewChief_Arch")) return "";

            string result = "err";
            fio = fio.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";
            if (position == "") return "no_position";

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return "incorrect_fio";
            }

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            if (work.AddChiefToRequest(reqId, fio, position))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен новый руководитель с ФИО - " + fio + ", должность - " + position + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен новый руководитель с ФИО - " + fio + ", должность - " + position + ". Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelChief_Arch(...)

        /// <summary>Метод для удаления участника из архивной заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelChief_Arch(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelChief_Arch")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork_Arch work = new CompetitionsWork_Arch();
            if (work.DelChiefFromRequest(reqId, position))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален руководитель с позицией - " + position + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален руководитель с позицией - " + position + ". Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestAddNewAthor_Arch(...)

        /// <summary>Метод для добавления нового автора коллекции в заявку. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="fio">ФИО</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestAddNewAthor_Arch(string reqId, string fio, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestAddNewAthor_Arch")) return "";

            string result = "err";
            fio = fio.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (fio == "") return "no_fio";

            string sPattern = "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$";
            if (!Regex.IsMatch(fio, sPattern))
            {
                return "incorrect_fio";
            }

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.AddAthorToRequest(reqId, fio))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен новый автор с ФИО - " + fio + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Добавлен новый автор с ФИО - " + fio + ". Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод UpdateRequestDelAthor_Arch(...)

        /// <summary>Метод для удаления автора из заявки. Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -1, значит удаляется индивидуальный участник)</param>
        /// <param name="code">код проверки</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string UpdateRequestDelAthor_Arch(string reqId, string position, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestDelAthor_Arch")) return "";

            string result = "err";
            reqId = reqId.Trim();
            position = position.Trim();

            #region Проверка присланных значений

            if (code != ck) return result;
            if (reqId == "") return result;
            if (position == "") return result;

            #endregion

            #region Основной код

            CompetitionsWork work = new CompetitionsWork();
            if (work.DelAthorFromRequest(reqId, position))
            {
                result = "ok";

                if (HttpContext.Current.Session["authperson"] != null)
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален автор с позицией - " + position + ". Изменил - " + ((AdmPersonStruct)HttpContext.Current.Session["authperson"]).Name);
                }
                else
                {
                    DebugLog.Log(ErrorEvents.info, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Изменена архивная заявка с id - " + reqId + ". Удален автор с позицией - " + position + ". Изменил - НЕОПРЕДЕЛЕНО");
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region Метод ImgLazyLoad(string imgsrc)

        /// <summary>Метод для обновления данных о заявке в консоли управления.  Используется на странице просмотра всех заявок - competitions.aspx</summary>
        /// <param name="imgsrc">ссылка на картинку</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string ImgLazyLoad(string imgsrc)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice UpdateRequestData")) return "";

            string result = "ok";


            #region Основной код

            ImageForm imgForm = new ImageForm(new System.Web.UI.Page());
            result = imgForm.GetPathToLittleFoto(imgsrc, "785", "");

            #endregion

            return result;
        }

        #endregion

        #region Методы для Файлового менеджера

        #region Метод GetFiles(string pathToFlsDir, string cd)

        /// <summary> Метод получает информацию о всех файлах в одной папке</summary>
        /// <param name="pathToFlsDir">относительный путь к папке с файлами, пример - /files/pages/files_e/</param>
        /// <param name="cd">проверочный код</param>
        /// <returns>Возвращает список объектов с данными по всем файлам папки, отсортированный по дате.
        /// В случае ошибке возвращает в поле FName первого элемента списка значение - "error".
        /// В случае, если папка пустая - возвращает в поле FName первого элемента списка значение - "null".</returns>
        [WebMethod(EnableSession = true)]
        public List<FileObj> GetFiles(string pathToFlsDir, string cd)
        {
            List<FileObj> list = new List<FileObj>();
            string path = HttpContext.Current.Server.MapPath("~") + pathToFlsDir;

            #region Основной код

            #region Проверки

            if (cd != ck)
            {
                list.Add(new FileObj { FName = "error" });
                return list;
            }

            #endregion

            try
            {
                #region Код

                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] fi = di.GetFiles();
                string fType = "", extention = "";
                ImageOverlay imgWork = new ImageOverlay();

                foreach (FileInfo fileInfo in fi)
                {
                    if (fileInfo.Name.Contains("_l."))   //если это файл уменьшенного изображения, то пропускаем его
                    {
                        continue;
                    }

                    #region Определение типа файла

                    extention = Path.GetExtension(fileInfo.Name).ToLower();
                    switch (extention)
                    {
                        case ".pdf":
                            fType = "pdf";
                            break;
                        case ".txt":
                            fType = "txt";
                            break;
                        case ".rar":
                            fType = "arch";
                            break;
                        case ".zip":
                            fType = "arch";
                            break;
                        case ".jpg":
                            fType = "img";
                            break;
                        case ".jpeg":
                            fType = "img";
                            break;
                        case ".png":
                            fType = "img";
                            break;
                        case ".gif":
                            fType = "img";
                            break;
                        case ".tif":
                            fType = "img";
                            break;
                        case ".tiff":
                            fType = "img";
                            break;
                        case ".bmp":
                            fType = "img";
                            break;
                        case ".doc":
                            fType = "office";
                            break;
                        case ".docx":
                            fType = "office";
                            break;
                        case ".rtf":
                            fType = "office";
                            break;
                        case ".xls":
                            fType = "office";
                            break;
                        case ".xlsx":
                            fType = "office";
                            break;
                        case ".ppt":
                            fType = "office";
                            break;
                        case ".pptx":
                            fType = "office";
                            break;
                        default:
                            fType = "?";
                            break;
                    }

                    #endregion

                    FileObj fileObj = new FileObj()
                    {
                        FDate = fileInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm"),
                        TempDate = fileInfo.LastWriteTime.Ticks,
                        FName = fileInfo.Name,
                        FLName = Path.GetFileNameWithoutExtension(fileInfo.Name) + "_l" + Path.GetExtension(fileInfo.Name),
                        FSize = fileInfo.Length / 1024,
                        FType = fType
                    };

                    list.Add(fileObj);

                    if (fType == "img")
                    {
                        if (!File.Exists(fileInfo.DirectoryName + "/" + fileObj.FLName))   //если уменьшенного файла изображения не существует, то создаем его
                        {
                            #region Код

                            Bitmap bmp = imgWork.ResizeImageToNeed(fileInfo.DirectoryName + "/" + fileObj.FName, 50, false);

                            try
                            {
                                if (extention == ".jpg")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Jpeg);
                                }
                                else if (extention == ".jpeg")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Jpeg);
                                }
                                else if (extention == ".png")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Png);
                                }
                                else if (extention == ".gif")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Gif);
                                }
                                else if (extention == ".tif")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Tiff);
                                }
                                else if (extention == ".tiff")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Tiff);
                                }
                                else if (extention == ".bmp")
                                {
                                    bmp.Save(fileInfo.DirectoryName + "/" + fileObj.FLName, ImageFormat.Bmp);
                                }
                            }
                            catch (Exception ex)
                            {
                                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                            }
                            finally
                            {
                                bmp.Dispose();
                                bmp = null;
                            }

                            #endregion
                        }
                    }
                }

                if (list.Count > 0)
                {
                    list = list.OrderBy(a => a.TempDate * -1).ToList();
                }
                else
                {
                    list.Add(new FileObj() { FName = "null" });
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                list.Clear();
                list.Add(new FileObj() { FName = "error" });

                #endregion
            }


            #endregion

            return list;
        }

        #endregion
        #region Метод DelFile(string pathToFlsDir, string fName, string flName, string cd)

        /// <summary> Метод удаляет файл</summary>
        /// <param name="pathToFlsDir">относительный путь к папке с файлами, пример - /files/pages/files_e/</param>
        /// <param name="fName">имя файла</param>
        /// <param name="cd">проверочный код</param>
        /// <returns>Возвращает error в случае ошибки удаления файла</returns>
        [WebMethod(EnableSession = true)]
        public string DelFile(string pathToFlsDir, string fName, string flName, string cd)
        {
            string path = HttpContext.Current.Server.MapPath("~") + pathToFlsDir;

            #region Основной код

            #region Проверки

            if (cd != ck)
            {
                return "error";
            }
            else if (!Directory.Exists(path))
            {
                return "error";
            }
            else if (!File.Exists(path + fName))
            {
                return "error";
            }

            #endregion

            try
            {
                #region Код

                File.Delete(path + fName);
                File.Delete(path + flName);

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return "error";

                #endregion
            }


            #endregion

            return "";
        }

        #endregion

        #endregion

        #region Метод Voting(string reqId, string act, string code)

        /// <summary>Метод для обновления данных голосования по заявке.  Используется на странице просмотра всех заявок на сайте.</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="act"></param>
        /// <param name="code">проверочный код каптчи</param>
        /// <returns>Возвращает 'ok' в случае успеха, 'err' - в случае какой-либо ошибки</returns>
        [WebMethod(EnableSession = true)]
        public string Voting(string reqId, string act, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice Voting")) return "";

            string result = "ok";
            reqId = reqId.Trim(); act = act.Trim();

            #region Проверка присланных значений

            if (!AntiBotMy.CheckCode(code)) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";
            if (act != "+" && act != "-") return "err";

            #endregion

            #region Основной код

            CookieSession cookie = new CookieSession();
            if (!cookie.VotingCookieCheck(reqId))
            {
                CompetitionsWork work = new CompetitionsWork();
                bool like = act != "-";
                if (work.UpdateLikes(long.Parse(reqId), like) == 1) //если значение счётчика увеличилось, то..
                {
                    cookie.VotingCookieAdd(reqId);
                }
                else
                {
                    return "err";
                }
            }
            else
            {
                return "err";
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод VotingSum(string reqId, string code)

        /// <summary>Метод для обновления данных голосования по заявке (по итоговому голосованию).  Используется на странице просмотра всех заявок на сайте.</summary>
        /// <param name="reqId">id запроса на участие в конкурсе</param>
        /// <param name="code">проверочный код каптчи</param>
        /// <returns>Возвращает 'ok' в случае успеха, 'err' - в случае какой-либо ошибки</returns>
        [WebMethod(EnableSession = true)]
        public string VotingSum(string reqId, string code)
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice VotingSum")) return "";

            string result = "ok";
            reqId = reqId.Trim();

            #region Проверка присланных значений

            if (!AntiBotMy.CheckCode(code)) return "err";
            if (StringToNum.ParseInt(reqId) == -1) return "err";

            #endregion

            #region Основной код

            CookieSession cookie = new CookieSession();
            if (!cookie.VotingSumCookieCheck(reqId))
            {
                CompetitionsWork work = new CompetitionsWork();
                if (work.UpdateSummaryLikes(long.Parse(reqId)) == 1) //если значение счётчика увеличилось, то..
                {
                    cookie.VotingSumCookieAdd(reqId);
                }
                else
                {
                    return "err";
                }
            }
            else
            {
                return "err";
            }

            #endregion

            return result;
        }

        #endregion

        #region Метод GetResultData(...)

        /// <summary>Выпадающий список с результатами в консоли (верхняя строчка) + оставить возможность «ввести свой текст». Используется на странице редактирования заявки - competitionone.aspx</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public ActionResult GetResultData(string searchString = "", string reqId = "")
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice GetResultData")) return null;

            //Проверка значений
            if (string.IsNullOrEmpty(reqId)) return null;

            #region Основной код
            
            var req = CompetitonWorkCommon.GetCompetitionRequest(reqId);
            var result = CompetitionsWork.GetResultDocumentForParticipant(req, true);
            var res = new List<string>();

            if (string.IsNullOrEmpty(result) || result != searchString)
            {
                res.Add("ЛАУРЕАТ I СТЕПЕНИ");
                res.Add("ЛАУРЕАТ II СТЕПЕНИ");
                res.Add("ЛАУРЕАТ III СТЕПЕНИ");
                res.Add("ДИПЛОМАНТ");
                res.Add("ДИПЛОМАНТ I СТЕПЕНИ");
                res.Add("ДИПЛОМАНТ II СТЕПЕНИ");
                res.Add("ДИПЛОМАНТ III СТЕПЕНИ");
                res.Add("УЧАСТНИК");
                res.Add("1 МЕСТО");
                res.Add("2 МЕСТО");
                res.Add("3 МЕСТО");
                res = res.Where(x => x.ToLower().Contains(searchString.Trim().ToLower())).ToList();
            }
            else
            {
                res.Add(result);
            }

            return new JsonResult() { Data = res };

            #endregion
        }

        #endregion

        #region Метод GetClassRoomData(...)

        /// <summary>Выпадающий список с названиями классов в консоли. Используется на странице создания заявки</summary>
        /// <param name="reqId">id заявки на участие в конкурсе</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public ActionResult GetClassRoomData(string searchString = "")
        {
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice GetClassRoomData")) return null;

            var results = new List<string>();
            int[] nums = Enumerable.Range(1, 11).ToArray();
            string[] symb = Enumerable.Range('А', 32).Select((num) => ((char)num).ToString()).ToArray();
            foreach (var item in nums)
            {
                foreach (var item1 in symb)
                {
                    results.Add(item + "-" + item1);
                }
            }
            results = results.Where(x => x.ToLower().Contains(searchString.Trim().ToLower())).ToList();

            return new JsonResult() { Data = results };
        }

        #endregion

        #region CAPTCHA

        #region Метод CaptchaGet()

        /// <summary>Метод возращает проверочную строку каптчи</summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string CaptchaGet()
        {
            //Проверка на использование данного метода web-службы ботом
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice captchaget")) return "";

            return AntiBotMy.GetCaptcha();
        }

        #endregion
        #region Метод CaptchaCheck(string bot)

        /// <summary>Метод проверяет введенную человеком каптчу</summary>
        /// <param name="bot">каптча</param>
        /// <returns>возвращает массив из двух строк - "ок" и проверочный код, если каптча введена человеком правильно. Коды ошибок возвращаются в первой строке возвращаемого массива</returns>
        [WebMethod(EnableSession = true)]
        public string[] CaptchaCheck(string bot)
        {
            //Проверка на использование данного метода web-службы ботом
            if (IpWork.IsUserAgentBot(HttpContext.Current.Request, true, "dataservice captchacheck")) return new string[] { "err_ab" };

            string[] result = new string[] { };
            result = AntiBotMy.CheckCaptcha(bot);

            return result;
        }
        #endregion

        #endregion
    }

    #region Класс FormRequest

    /// <summary>Класс для объявления структуры данных для обратной связи</summary>
    public class FormRequest
    {
        public string Fam { get; set; }     // фамилия
        public string Name { get; set; }    // имя (обязательный параметр)
        public string Tel { get; set; }     // телефон
        public string Mail { get; set; }    // email (обязательный параметр)
        public string Text { get; set; }    // текст обращения (обязательный параметр)
    }

    #endregion

    #region Классы для файлового менеджера

    /// <summary>Класс для объявления структуры данных по одному файлу</summary>
    public class FileObj
    {
        private string fName = "";
        public string FName                     // имя файла (file.jpg)
        {
            get { return fName; }
            set { fName = value; }
        }

        private string flName = "";
        public string FLName                     // имя уменьшенного файла (file_l.jpg)
        {
            get { return flName; }
            set { flName = value; }
        }

        private long fSize = 0;
        public long FSize                     // размер файла в килобайтах (file.jpg)
        {
            get { return fSize; }
            set { fSize = value; }
        }

        private string fDate = "";
        public string FDate                     // дата добавления файла на сервер, пример - 22.10.2016 16:00
        {
            get { return fDate; }
            set { fDate = value; }
        }

        private string fType = "";
        public string FType                     // тип файла (все типы перечислены в методе GetFiles)
        {
            get { return fType; }
            set { fType = value; }
        }


        private long tempDate = 0;
        public long TempDate                     // дата добавления файла на сервер в тиках (для C# кода - сортировки)
        {
            get { return tempDate; }
            set { tempDate = value; }
        }
    }

    #endregion
}
