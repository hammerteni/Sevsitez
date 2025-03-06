using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace site.classesHelp
{
    #region Класс ConfigFile

    /// <summary>Класс для создания, записи и извлечения данных из конфигурационных файлов</summary>
    public class ConfigFile
    {
        #region Свойства

        private string _cfgFolderPath;
        private string _cfgFilePath;
        private string _tmpFolder;
        private bool _checkFolderAndPathExist = true;   // Переменная будет содержать false, если при инициализации класса возникнет ошибка при создании папки или файла конфигурации
        private FileStream _fs; //переменная используется для блокировки доступа к файлу, который перезаписывается

        #endregion

        #region Конструкторы

        /// <summary>Конструктор. При его вызове происходит проверка на наличие и создание при необходимости конфигурационного файла (cfg.ini) по пути - ~\files\cfg</summary>
        public ConfigFile()
        {
            // Определяем пути к папке и файлу конфигурации
            _cfgFolderPath = HttpContext.Current.Server.MapPath("~") + @"files\cfg";
            _cfgFilePath = _cfgFolderPath + @"\cfg.ini";
            _tmpFolder = HttpContext.Current.Server.MapPath("~") + @"files\temp";

            // Проверяем наличие папки и файла и если их нет, то создаём их
            if (!Directory.Exists(_cfgFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(_cfgFolderPath);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            if (!Directory.Exists(_tmpFolder))
            {
                try
                {
                    Directory.CreateDirectory(_tmpFolder);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            if (!File.Exists(_cfgFilePath))
            {
                try
                {
                    FileStream fs = File.Create(_cfgFilePath);
                    fs.Close();
                    fs.Dispose();
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
        }

        /// <summary>Перегрузка конструктора, которая принимает путь к файлу конфигурации. При его вызове происходит проверка на наличие и создание при необходимости конфигурационного файла (cfg.ini)</summary>
        public ConfigFile(string pathToCfgFile)
        {
            // Определяем пути к папке и файлу конфигурации
            _cfgFolderPath = Path.GetDirectoryName(pathToCfgFile);
            _cfgFilePath = pathToCfgFile;
            _tmpFolder = HttpContext.Current.Server.MapPath("~") + @"files\temp";

            // Проверяем наличие папки и файла и если их нет, то создаём их
            if (!Directory.Exists(_cfgFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(_cfgFolderPath);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            if (!Directory.Exists(_tmpFolder))
            {
                try
                {
                    Directory.CreateDirectory(_tmpFolder);
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
            if (!File.Exists(_cfgFilePath))
            {
                try
                {
                    FileStream fs = File.Create(_cfgFilePath);
                    fs.Close();
                    fs.Dispose();
                }
                catch
                {
                    _checkFolderAndPathExist = false;
                }
            }
        }

        #endregion

        #region Метод AddParam(...)

        /// <summary>Метод добавляет новые или изменяет существующий в файле конфигурации параметр.</summary>
        /// <param name="name">имя параметра</param>
        /// <param name="val">значение параметра</param>
        /// <param name="encrypt">если true - значение параметра кодируется, если false - не кодируется</param>
        /// <returns>возвращает true в случае успеха или false в случае ошибки записи данных</returns>
        public bool AddParam(string name, string val, bool encrypt = false)
        {
            if (_checkFolderAndPathExist)
            {
                #region Основной код

                EncDecClass encdec = new EncDecClass();
                List<OneParam> paramList = GetAllParams();
                OneParam oneParam = new OneParam();

                //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла для других процессов 
                try
                {
                    _fs = new FileStream(_cfgFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch
                {
                    return false;
                }

                bool chkReplace = false;
                int index = 0;
                foreach (OneParam oneparam in paramList)
                {
                    if (oneparam.Name == name)
                    {
                        chkReplace = true;
                        break;
                    }
                    index++;
                }

                if (chkReplace) // если нужно заменить значение параметра, то..
                {
                    if (encrypt)
                    {
                        paramList[index].Value = encdec.Enctext(val);
                    }
                    else
                    {
                        paramList[index].Value = val;
                    }
                }
                else // если нужно добавить новый параметр, то..
                {
                    oneParam.Name = name;
                    if (encrypt)
                    {
                        oneParam.Value = encdec.Enctext(val);
                    }
                    else
                    {
                        oneParam.Value = val;
                    }
                    paramList.Add(oneParam);
                }

                // Теперь перезапишем файл конфигурации с учётом проделанных изменений
                var listToRaplace = new List<string>();
                foreach (OneParam oneStruct in paramList)
                    //получаем финальный строковый список для перезаписи файла конфигурации 
                {
                    listToRaplace.Add(oneStruct.GetListFromStruct());
                }

                //Код замены файла конфигурации новым содержимым из списка listToRaplace
                var rn = new Random();
                int num = rn.Next(1, 666);
                string tempFilePath = _tmpFolder + @"\cfg_" + num;
                try
                {
                    File.WriteAllLines(tempFilePath, listToRaplace, Encoding.Default);
                    _fs.Close();
                    _fs.Dispose();
                    File.Copy(tempFilePath, _cfgFilePath, true);
                }
                catch
                {
                    return false;
                }
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                }

                #endregion
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
        #region Метод GetParam(...)

        /// <summary>Метод возвращает значение параметра из файла конфигурации по его имени.</summary>
        /// <param name="name">имя параметра</param>
        /// <param name="decrypt">если true - значение параметра кодируется, если false - не кодируется</param>
        /// <returns>возвращает значение параметра из файла конфигурации по его имени. Если параметра с таким именем нет, то возвращается '-1'</returns>
        public string GetParam(string name, bool decrypt = false)
        {
            string result = "-1";
            EncDecClass encdec = new EncDecClass();

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                List<OneParam> paramList = GetAllParams();
                foreach (OneParam oneparam in paramList)
                {
                    if (oneparam.Name == name)
                    {
                        if (decrypt) //если нужно раскодировать значение параметра, то..
                        {
                            result = encdec.Dectext(oneparam.Value);
                        }
                        else
                        {
                            result = oneparam.Value;
                        }
                        break;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region DelParam(...)

        /// <summary>Метод удаляет параметр из файла конфигурации</summary>
        /// <param name="name">имя параметра для удаления</param>
        /// <returns>возвращает true в случае успешного удаления параметра</returns>
        public bool DelParam(string name)
        {
            if (_checkFolderAndPathExist)
            {
                #region Основной код

                List<OneParam> paramList = GetAllParams();
                List<OneParam> paramListNew = new List<OneParam>();

                //строка блокировки доступа к изменяемому файлу, разрешено только чтение из файла для других процессов 
                try
                {
                    _fs = new FileStream(_cfgFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch
                {
                    return false;
                }

                foreach (OneParam oneparam in paramList)
                {
                    if (oneparam.Name != name)
                    {
                        paramListNew.Add(oneparam);
                    }
                }

                // Теперь перезапишем файл конфигурации с учётом проделанных изменений
                var listToRaplace = new List<string>();
                foreach (OneParam oneStruct in paramListNew)
                    //получаем финальный строковый список для перезаписи файла конфигурации 
                {
                    listToRaplace.Add(oneStruct.GetListFromStruct());
                }

                //Код замены файла конфигурации новым содержимым из списка listToRaplace
                var rn = new Random();
                int num = rn.Next(1, 666);
                string tempFilePath = _tmpFolder + @"\cfg_" + num;
                try
                {
                    File.WriteAllLines(tempFilePath, listToRaplace.ToArray(), Encoding.Default);
                    _fs.Close();
                    _fs.Dispose();
                    File.Copy(tempFilePath, _cfgFilePath, true);
                }
                catch
                {
                    return false;
                }
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                }

                #endregion
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Метод GetAllParams()

        /// <summary>Метод возвращает список объектов всех параметров, присутствующих в файле конфигурации</summary>
        /// <returns></returns>
        private List<OneParam> GetAllParams()
        {
            List<OneParam> resultList = new List<OneParam>();

            #region Основной код

            String[] strSplit;
            OneParam oneStruct = new OneParam();
            String[] str = File.ReadAllLines(_cfgFilePath, Encoding.Default);
            foreach (string line in str) //перебираем содержимое файла, выгруженное в массив str
            {
                if (line.Contains("="))
                {
                    strSplit = line.Split(new Char[] {'='});
                    oneStruct = new OneParam();
                    oneStruct.Name = strSplit[0];
                    oneStruct.Value = strSplit[1];
                    resultList.Add(oneStruct);
                }
            }

            #endregion

            return resultList;
        }

        #endregion

        #region Метод IsReqGettingOn(...)

        /// <summary>Метод проверяет значение параметра переключения отображения кнопки ОТПРАВИТЬ ЗАЯВКУ в номинациях. Метод создает параметр, если его еще не существует.
        /// Короче учитывает все форсмажорные варианты</summary>
        /// <param name="code">кодовое имя номинации</param>
        /// <returns>true - если прием заявок открыт, false - если закрыт или параметра еще нет (и если не удалось создать этот параметр)</returns>
        public bool IsReqGettingOn(string code)
        {
            bool result = false;
            string suffix = "_onoff";
            string on = "on";
            string off = "off";

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                string val = GetParam(code + suffix);
                if(val == "-1")
                {
                    AddParam(code + suffix, off);
                }
                else
                {
                    if(val == on)
                    {
                        result = true;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод IsVoitingPanelOn(...)

        /// <summary>Метод проверяет значение параметра переключения отображения панели голосования в номинациях. Метод создает параметр, если его еще не существует.
        /// Короче учитывает все форсмажорные варианты</summary>
        /// <param name="code">кодовое имя номинации</param>
        /// <returns>true - если отображение панели голосования включено, false - если выключено или параметра еще нет (и если не удалось создать этот параметр)</returns>
        public bool IsVoitingPanelOn(string code)
        {
            bool result = false;
            string suffix = "_voiting_onoff";
            string on = "on";
            string off = "off";

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                string val = GetParam(code + suffix);
                if (val == "-1")
                {
                    AddParam(code + suffix, off);
                }
                else
                {
                    if (val == on)
                    {
                        result = true;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод IsResultPanelOn(...)

        /// <summary>Метод проверяет значение параметра переключения отображения панели результатов в номинациях. Метод создает параметр, если его еще не существует.
        /// Короче учитывает все форсмажорные варианты</summary>
        /// <param name="code">кодовое имя номинации</param>
        /// <returns>true - если панель результатов нужно отображать, false - если не нужно или параметра еще нет (и если не удалось создать этот параметр)</returns>
        public bool IsResultPanelOn(string code)
        {
            bool result = false;
            string suffix = "_results_onoff";
            string on = "on";
            string off = "off";

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                string val = GetParam(code + suffix);
                if (val == "-1")
                {
                    AddParam(code + suffix, off);
                }
                else
                {
                    if (val == on)
                    {
                        result = true;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод IsResultPanelCertificateFirstRoundOn(...)

        /// <summary>Метод проверяет значение параметра переключения отображения включения вывода участников первого тура и сертификатов по ним. Метод создает параметр, если его еще не существует.
        /// Короче учитывает все форсмажорные варианты</summary>
        /// <param name="code">кодовое имя номинации</param>
        /// <returns>true - если панель результатов нужно отображать, false - если не нужно или параметра еще нет (и если не удалось создать этот параметр)</returns>
        public bool IsResultPanelFirstRoundOn(string code)
        {
            bool result = false;
            string suffix = "_results_first_round_onoff";
            string on = "on";
            string off = "off";

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                string val = GetParam(code + suffix);
                if (val == "-1")
                {
                    AddParam(code + suffix, off);
                }
                else
                {
                    if (val == on)
                    {
                        result = true;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод IsVoitingBtnsOn(...)

        /// <summary>Метод проверяет значение параметра отображения кнопок голосования в номинациях. Метод создает параметр, если его еще не существует.
        /// Короче учитывает все форсмажорные варианты</summary>
        /// <param name="code">кодовое имя номинации</param>
        /// <returns>true - если кнопки нужно отображать, false - если не нужно или параметра еще нет (и если не удалось создать этот параметр)</returns>
        public bool IsVoitingBtnsOn(string code)
        {
            bool result = false;
            string suffix = "_voitbtns_onoff";
            string on = "on";
            string off = "off";

            if (_checkFolderAndPathExist)
            {
                #region Основной код

                string val = GetParam(code + suffix);
                if (val == "-1")
                {
                    AddParam(code + suffix, off);
                }
                else
                {
                    if (val == on)
                    {
                        result = true;
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion
    }

    #endregion

    #region Класс OneParam

    /// <summary>класс описывающий структуру данных одной новости</summary>
    public class OneParam
    {
        public string Name { get; set; }
        //содержит URL-ссылку на основную картинку новости вида - ~/files/news/images/1.jpg
        public string Value { get; set; } //содержит дату новости в формате - 11.09.2014

        /// <summary>функция возвращает строку из данной структуры, подготовленную для записи в файл конфигурации</summary>
        /// <returns></returns>
        public String GetListFromStruct()
        {
            return Name + "=" + Value;
        }
    }

    #endregion
}