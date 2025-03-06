using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

//Файл содержим классы для работы с Яндекс-Переводчиком
// Ссылка на API - https://tech.yandex.ru/translate/
namespace site.classesHelp
{
    #region Класс YandexTranslate
    /// <summary>Класс обращается к бесплатному api Яндекс-Переводчика для перевода текста</summary>
    public class YandexTranslate
    {
        #region Поля
        private static string key = "trnsl.1.1.20160808T133827Z.fee20f42135738ef.e857ff87b4dc86ad0a31adc262142ee758f1679f";
        #endregion

        #region Метод Translate(string text, string translateDirection = "en-ru")
        /// <summary>Метод переводит переданный в него текст и возвращает его</summary>
        /// <param name="text">текст, который нужно перевести</param>
        /// <param name="translateDirection">направление перевода, по умолчанию с английского на русский</param>
        /// <returns></returns>
        public static string Translate(string text, string translateDirection = "en-ru")
        {
            string result = "";

            #region Код
            try
            {
                //сервис возвращает данные по ip на английском языке
                //{"code":200,"lang":"en-ru","text":["Москва"]}
                text = HttpUtility.UrlEncode(text);
                string url = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + key +
                             "&text=" + text + "&lang=" + translateDirection + "&format=plain";
                WebRequest webReq = WebRequest.Create(url);

                using (WebResponse webRes = webReq.GetResponse())
                using (Stream stream = webRes.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);
                    string code = (string)obj["code"];
                    switch (code)
                    {
                        case "200":
                            JArray tmpArr = (JArray)obj["text"];
                            if(tmpArr.Count > 0) result = tmpArr[0].ToString();
                            break;
                        case "401":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        case "402":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        case "404":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        case "413":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        case "422":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        case "501":
                            DebugLog.Log(ErrorEvents.warn, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст: " + XmlResources.GetString("warn_translate_" + code));
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "YandexTranslate", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }
            #endregion

            return result;
        }
        #endregion
    }
    #endregion
}