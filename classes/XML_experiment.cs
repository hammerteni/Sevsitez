
// файл с классами для работы с данными, предназначенными для оповещения людей о появлении товаров в магазине. Эти товары люди помечают для напоминания через кнопку
// УВЕДОМИТЬ О ПОСТАВКЕ, если товар отсутствует в наличии
// XML-файл находится по пути /files/availmailing/availmailing.xml
// Классы используются в web-службе, в методе - AvailabilityMailing(...) и в 
// методе SaveChangeToTempStruct() класса ProdOneInfoClass (файл productForm) 

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using site.classesHelp;

namespace site.classes
{
    #region Классы работы с данными

    /// <summary>Класс для работы с данными из файла /files/availmailing/availmailing.xml</summary>
    public class AvailProdMailingWork
    {
        #region ПОЛЯ КЛАССА

        private HttpContext _context = HttpContext.Current;
        private string _folderPath;
        private string _fileName;
        private string _filePath;
        private bool _checkFolderAndPathExist;  // Переменная будет содержать false, если при инициализации класса возникнет ошибка при создании папки или XML-файла данных

        #endregion

        /// <summary>Конструктор. При его вызове происходит проверка на наличие и создание при необходимости
        /// папки /files/availmailing и файла /files/availmailing/availmailing.xml с начальной его структурой</summary>
        public AvailProdMailingWork()
        {
            #region Инициализируем поля класса

            _checkFolderAndPathExist = true;
            _fileName = "availmailing.xml";
            _folderPath = _context.Server.MapPath("~") + @"files\availmailing";
            _filePath = _folderPath + @"\" + _fileName;

            #endregion

            #region Проверяем наличие папки и файла и если их нет, то создаём их

            if (!Directory.Exists(_folderPath))
            {
                try
                {
                    Directory.CreateDirectory(_folderPath);
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    _checkFolderAndPathExist = false;
                }
            }

            if (!File.Exists(_filePath) && _checkFolderAndPathExist)
            {
                //Основная структура документа будет такой
                // <?xml version="1.0" encoding="utf-8"?>
                // <requests>
                //      <request>
                //          <artikul>63452344</artikul>
                //          <mail>mail@yandex.ru<mail>
                //      </request>
                // </requests>

                #region Создаём начальную структуру XML-файла

                try
                {
                    XmlTextWriter textWriter = new XmlTextWriter(_filePath, Encoding.UTF8);
                    textWriter.WriteStartDocument();
                    textWriter.WriteStartElement("requests");
                    textWriter.WriteEndElement();
                    textWriter.Close();
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    _checkFolderAndPathExist = false;
                }

                #endregion
            }

            #endregion
        }

        /// <summary>Метод добавляет данные по новое заявке на оповещение в XML-файл данных</summary>
        /// <param name="oneStruct">структура данных одного напоминания(заявки) о наличии товара</param>
        /// <returns>Возвращает true в лучае успешного добавления и false в случае неудачи</returns>
        public bool AddElement(AvailProdMailingStruct oneStruct)
        {
            //проверка на наличие в XML-файле такой заявки
            if (CheckRequestExist(oneStruct)) return true;

            try
            {
                XDocument xdoc = XDocument.Load(_filePath);
                XElement root = xdoc.Element("requests");
                root.Add(new XElement("request",
                                        new XElement("artikul", oneStruct.Artikul),
                                        new XElement("mail", oneStruct.Mail)));
                xdoc.Save(_filePath);

            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false;
            }

            return true;
        }

        /// <summary>Метод проверяет наличие заявки в XML-файле по переданной в него структуре</summary>
        /// <param name="oneStruct">структура данных одного напоминания(заявки) о наличии товара</param>
        /// <returns>Возвращает true если в файле уже есть такая заявка и false если нет такой заявки</returns>
        private bool CheckRequestExist(AvailProdMailingStruct oneStruct)
        {
            bool result = false;

            XDocument xdoc = XDocument.Load(_filePath);
            XElement root = xdoc.Element("requests");
            try
            {
                foreach (XElement xElement in root.Elements("request").ToList())
                {
                    if (xElement.Element("artikul").Value == oneStruct.Artikul &&
                        xElement.Element("mail").Value == oneStruct.Mail)
                    {
                        result = true; break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }


            return result;
        }

        /// <summary>Метод проверяет наличие заявки в XML-файле по переданному в него артикулу товара</summary>
        /// <param name="artikul">артикул товара</param>
        /// <returns>Возвращает true если в файле есть заявка с таким артикулом и false если нет заявки с таким артикулом</returns>
        public bool CheckRequestExist(string artikul)
        {
            bool result = false;

            XDocument xdoc = XDocument.Load(_filePath);
            XElement root = xdoc.Element("requests");
            try
            {
                foreach (XElement xElement in root.Elements("request").ToList())
                {
                    if (xElement.Element("artikul").Value == artikul)
                    {
                        result = true; break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }


            return result;
        }

        /// <summary>Метод получает список адресов по артикулу товара из XML-файла</summary>
        /// <param name="artikul"></param>
        /// <returns>возвращает список e-mail. Если для такого артикула не найдено адресов, то возвращается пустой список.</returns>
        private List<string> GetMails(string artikul)
        {
            List<string> result = new List<string>();

            XDocument xdoc = XDocument.Load(_filePath);
            XElement root = xdoc.Element("requests");
            try
            {
                foreach (XElement xElement in root.Elements("request").ToList())
                {
                    if (xElement.Element("artikul").Value == artikul)
                    {
                        result.Add(xElement.Element("mail").Value);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }


            return result;
        }

        /// <summary>Метод удаляет из XML-файла данные по заявкам на оповещение, согласно списку адресов и артикулу товара</summary>
        /// <param name="mailList">список адресов получателей оповещения</param>
        /// <param name="artikul">артикул товара, по которому производится оповещение</param>
        /// <returns></returns>
        private bool DeleteRequests(List<string> mailList, string artikul)
        {
            bool result = true;

            XDocument xdoc = XDocument.Load(_filePath);
            XElement root = xdoc.Element("requests");
            try
            {
                foreach (XElement xElement in root.Elements("request").ToList())
                {
                    if (xElement.Element("artikul").Value == artikul)
                    {
                        if (mailList.Contains(xElement.Element("mail").Value))
                        {
                            xElement.Remove();
                        }
                    }
                }
                xdoc.Save(_filePath);
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = false;
            }

            return result;
        }

    }

    #endregion

    #region Классы структур данных

    /// <summary>Структура данных одного напоминания о товаре</summary>
    public class AvailProdMailingStruct
    {
        public string Artikul { get; set; }     //артикул товара, которого в данный момент нет в наличии
        public string Mail { get; set; }        //почта человека, которого нужно оповестить с случае появления товара в магазине
    }

    #endregion

}