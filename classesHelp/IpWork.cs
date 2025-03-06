using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using site.classes;

// Файл содержим методы для работы с ip-адресами
namespace site.classesHelp
{
    #region Работа с данными

    #region Класс IpWork

    /// <summary>Класс для работы с ip-адресами</summary>
    public class IpWork
    {
        
        #region Метод GetInfoForIp(string ip_address)
        /// <summary>Метод получает данные по ip-адресу на английском языке</summary>
        /// <param name="ip_address">ip-адрес, для которого нужно получить данные</param>
        /// <returns>Возвращает объект с данными или null в случае какой-либо ошибки</returns>
        public static IpInfo GetInfoForIp(string ip_address)
        {
            IpInfo result = new IpInfo();

            #region Код
            try
            {
                //сервис возвращает данные по ip на английском языке
                //{"ip":"91.211.209.85","country_code":"RU","country_name":"Russia","region_code":"MOW","region_name":"Moscow","city":"Moscow",
                //"zip_code":"101194","time_zone":"Europe/Moscow","latitude":55.7522,"longitude":37.6156,"metro_code":0} 
                string url = "http://freegeoip.net/json/" + ip_address;
                WebRequest webReq = WebRequest.Create(url);

                using (WebResponse webRes = webReq.GetResponse())
                using (Stream stream = webRes.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);
                    result.ip = (string)obj["ip"];
                    result.country_code = (string)obj["country_code"];
                    result.country_name = (string)obj["country_name"];
                    result.region_code = (string)obj["region_code"];
                    result.region_name = (string)obj["region_name"];
                    result.city = (string)obj["city"];
                    result.zip_code = (string)obj["zip_code"];
                    result.time_zone = (string)obj["time_zone"];
                    result.latitude = (double)obj["latitude"];
                    result.longitude = (double)obj["longitude"];
                    result.metro_code = (string)obj["metro_code"];
                }
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "IpWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = null;

                #endregion
            }
            #endregion

            return result;
        }
        #endregion

        #region Метод GetClearInfoFromIp(string ip_address)

        /// <summary>Метод возвращает структуру данных по IP-адресу. Необходимые данные переведены на русский язык</summary>
        /// <param name="ip_address">ip-адрес, для которого нужно получить данные</param>
        /// <returns>Возвращает объект с данными или null в случае какой-либо ошибки</returns>
        public static IpInfo GetClearInfoFromIp(string ip_address)
        {
            IpInfo result = new IpInfo();

            #region Код
            try
            {
                result = GetInfoForIp(ip_address.Trim());
                if (result != null)
                {

                    //Перевод на русский
                    result.city_ru = YandexTranslate.Translate(result.city);
                    result.region_name_ru = YandexTranslate.Translate(result.region_name);
                    result.country_name_ru = YandexTranslate.Translate(result.country_name);

                    //Транслитерация, если требуется
                    if (IsStringLatin.IsLatinOrNo(result.city_ru) == StringType.LAT)
                    {
                        result.city_ru = Transliteration.Replace(result.city_ru, TransliterationType.ISO, TranslitDirection.LAT_RU);
                    }
                    else if (IsStringLatin.IsLatinOrNo(result.region_name_ru) == StringType.LAT)
                    {
                        result.region_name_ru = Transliteration.Replace(result.region_name_ru, TransliterationType.ISO, TranslitDirection.LAT_RU);
                    }
                    else if (IsStringLatin.IsLatinOrNo(result.country_name_ru) == StringType.LAT)
                    {
                        result.country_name_ru = Transliteration.Replace(result.country_name_ru, TransliterationType.ISO, TranslitDirection.LAT_RU);
                    }

                    //Приведение названия города к грамматически правильному виду, если требуется
                    List<string> towns = XmlResources.GetTownsList();
                    if (towns != null)
                    {
                        List<string> similars = new List<string>();
                        foreach (string town in towns)
                        {
                            if (result.city_ru.Contains(town))
                            {
                                similars.Add(town);
                            }
                        }
                        if (similars.Count > 0) {
                            similars = similars.OrderBy(a => a.Length).ToList();    //сортировка списка названий городов по длине названия
                            result.city_ru = similars[similars.Count - 1];          //самое длинное из названий будет самым предпочтительным
                        }
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "IpWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = null;

                #endregion
            }
            #endregion

            return result;
        }

        #endregion
        
        #region Метод IsUserAgentBot(string userAgentInfo)

        /// <summary>Метод проверяет полученную информацию об агенте, который делает запрос, на предмет определения - бот или человек</summary>
        /// <param name="request">объект HttpRequest с данным по входу</param>
        /// <param name="loggingNotBot">логировать или нет входы ботов</param>
        /// <param name="tag">тэг, добавляемый к данным по входу при включённом логировании</param>
        /// <returns>true - если это бот, false - если человек заходит через браузер</returns>
        public static bool IsUserAgentBot(HttpRequest request, bool loggingBot = false, string tag = "")
        {
            bool result = false;
            string userAgentInfo = request.UserAgent.Trim();

            #region Код

            if (userAgentInfo == "") return true;

            string botWords = XmlResources.GetString("bots_words");
            if(botWords == "no resource")
            {
                DebugLog.Log(ErrorEvents.warn, "IpWork", MethodBase.GetCurrentMethod().Name, "Текст: не удалось получить текстовый ресурс с именем bots_words");
                result = false;
            }
            else
            {
                string[] botWordsArr = botWords.Split(new[] { ',' });
                foreach (string word in botWordsArr)
                {
                    if (userAgentInfo.Contains(word))
                    {
                        result = true;
                        break;
                    }
                }

                if (result && loggingBot)    //все выявленные, как Бот, входы, записываем в журнал посещения сайта со своим тегом
                {
                    DebugLog.Log(ErrorEvents.warn, "IpWork", MethodBase.GetCurrentMethod().Name, "Текст: Зафиксирована активность бота. Тэг - " + tag + ". UserAgent - " + request.UserAgent + " / ip - " + request.UserHostAddress);
                }
            }

            #endregion

            return result;
        }
        #endregion
    }

    #endregion

    #endregion

    #region Структуры данных

    #region Класс IpInfo
    [Serializable]
    public class IpInfo
    {
        #region ip
        private string _ip = "";
        /// <summary>ip адрес</summary>
        public string ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
        #endregion

        #region country_code
        private string _countrycode = "";
        /// <summary>код страны, которой принадлежит ip</summary>
        public string country_code
        {
            get { return _countrycode; }
            set { _countrycode = value; }
        }
        #endregion

        #region country_name
        private string _countryname = "";
        /// <summary>название страны, которой принадлежит ip</summary>
        public string country_name
        {
            get { return _countryname; }
            set { _countryname = value; }
        }
        #endregion

        #region country_name_ru
        private string _countryname_ru = "";
        /// <summary>название страны, которой принадлежит ip, на русском</summary>
        public string country_name_ru
        {
            get { return _countryname_ru; }
            set { _countryname_ru = value; }
        }
        #endregion

        #region region_code
        private string _region_code = "";
        /// <summary>код региона</summary>
        public string region_code
        {
            get { return _region_code; }
            set { _region_code = value; }
        }
        #endregion

        #region region_name
        private string _region_name = "";
        /// <summary>название региона</summary>
        public string region_name
        {
            get { return _region_name; }
            set { _region_name = value; }
        }
        #endregion

        #region region_name_ru
        private string _region_name_ru = "";
        /// <summary>название региона, на русском</summary>
        public string region_name_ru
        {
            get { return _region_name_ru; }
            set { _region_name_ru = value; }
        }
        #endregion

        #region city
        private string _city = "";
        /// <summary>город, которому принадлежит ip</summary>
        public string city
        {
            get { return _city; }
            set { _city = value; }
        }
        #endregion

        #region city_ru
        private string _city_ru = "";
        /// <summary>город, которому принадлежит ip, на русском</summary>
        public string city_ru
        {
            get { return _city_ru; }
            set { _city_ru = value; }
        }
        #endregion

        #region zip_code
        private string _zip_code = "";
        /// <summary>индекс</summary>
        public string zip_code
        {
            get { return _zip_code; }
            set { _zip_code = value; }
        }
        #endregion

        #region time_zone
        private string _time_zone = "";
        /// <summary>временная зона</summary>
        public string time_zone
        {
            get { return _time_zone; }
            set { _time_zone = value; }
        }
        #endregion

        #region latitude
        private double _latitude = 0;
        /// <summary>широта</summary>
        public double latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        #endregion

        #region longitude
        private double _longitude = 0;
        /// <summary>долгота</summary>
        public double longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        #endregion

        #region metro_code
        private string _metro_code = "";
        /// <summary>долгота</summary>
        public string metro_code
        {
            get { return _metro_code; }
            set { _metro_code = value; }
        }
        #endregion
    }
    #endregion

    #endregion
}