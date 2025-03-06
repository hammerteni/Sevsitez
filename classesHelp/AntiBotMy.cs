using System;
using System.Collections.Generic;
using System.Web;

//Файл содержит класс формирования данные для работы анти-бот системы (CAPTCHA)
namespace site.classesHelp
{
    #region Класс AntiBotMy
    /// <summary>Класс формирует данные для работы анти-бот системы</summary>
    public class AntiBotMy
    {
        #region Поля

        private static Dictionary<string, string> words;

        #endregion

        #region Метод GetCaptcha()

        /// <summary>Метод возвращает сгенерированную Captcha</summary>
        /// <returns></returns>
        public static string GetCaptcha()
        {
            string result = "";

            Random rnd = new Random();
            int choice = rnd.Next(1, 3);

            if (choice == 1)     //генерируется Каптча сложения или вычитания
            {
                #region Код

                rnd = new Random();
                int first = rnd.Next(1, 10);
                int second = rnd.Next(1, 10);
                int math = rnd.Next(1, 6);
                //Для действий вычитания делаем так, чтобы цифра, из которой вычитают, была всегда больше
                if (math >= 3 && math <= 5)
                {
                    if (first < second)
                    {
                        first = second + rnd.Next(1, 3);
                    }
                }

                string f = WordFromCifir(first);
                string s = WordFromCifir(second);
                if (math == 1)
                {
                    HttpContext.Current.Session["antiBotString"] = (first + second).ToString();
                    result = f + " прибавить " + s;
                }
                else if (math == 2)
                {
                    HttpContext.Current.Session["antiBotString"] = (first + second).ToString();
                    result = f + " плюс " + s;
                }
                else if (math == 3)
                {
                    HttpContext.Current.Session["antiBotString"] = (first - second).ToString();
                    result = f + " минус " + s;
                }
                else if (math == 4)
                {
                    HttpContext.Current.Session["antiBotString"] = (first - second).ToString();
                    result = f + " отнять " + s;
                }
                else if (math == 5)
                {
                    HttpContext.Current.Session["antiBotString"] = (first - second).ToString();
                    result = f + " вычесть " + s;
                }

                #endregion
            }
            else                //генерируется Каптча умножения
            {
                #region Код

                rnd = new Random();
                int first = rnd.Next(1, 10);
                int second = rnd.Next(1, 10);
                int math = rnd.Next(1, 3);

                string f = WordFromCifir(first);
                string s = WordFromCifir(second);
                if (math == 1)
                {
                    HttpContext.Current.Session["antiBotString"] = (first * second).ToString();
                    result = f + " умножить на " + s;
                }
                else if (math == 2)
                {
                    HttpContext.Current.Session["antiBotString"] = (first * second).ToString();
                    result = f + " помножить на " + s;
                }

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод CheckCaptcha(string captcha)

        /// <summary>Метод проверяет переданное значение на совпадение со сгеренированной ранее Captcha</summary>
        /// <param name="captcha">значение, которое нужно сравнить.</param>
        /// <returns>Возвращает 
        /// "timeout", если сессионной переменной уже не существует, 
        /// "ok", код проверки - в случае, если значения совпали,
        /// "err" - если не совпали</returns>
        public static string[] CheckCaptcha(string captcha)
        {
            string[] result = new string[2];

            if (HttpContext.Current.Session["antiBotString"] == null)
            {
                result[0] = "timeout";
            }
            else
            {
                string trueCaptcha = (string)HttpContext.Current.Session["antiBotString"];
                if (trueCaptcha == captcha)
                {
                    result[0] = "ok";
                    Random rnd = new Random();
                    HttpContext.Current.Session["antiBotCode"] = rnd.Next(100, 999).ToString();
                    result[1] = (string)HttpContext.Current.Session["antiBotCode"];
                }
                else
                {
                    result[0] = "err";
                }
            }

            return result;
        }

        #endregion
        #region Метод CheckCode(string code)

        /// <summary>Метод проверяет переданное значение кода, сформированного при проверке правильности введения человеком Captcha</summary>
        /// <param name="code">код, сформированный при проверке правильности введения человеком Captcha</param>
        /// <returns>Возвращает 
        /// true - правильный код, 
        /// false - неправильный код</returns>
        public static bool CheckCode(string code)
        {
            bool result = false;

            if(HttpContext.Current.Session["antiBotCode"] != null)
            {
                if (code == (string)HttpContext.Current.Session["antiBotCode"])
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion
        #region Метод WordFromCifir(int cifir)

        /// <summary>Метод преобразует переданную в него цифру в слово. Например - 1 == один и т.п. Обрабатывается диапозон цифр от 1 до 100 включительно.</summary>
        /// <param name="cifir"></param>
        /// <returns>возвращает словестную запись цифры или out_of_range</returns>
        private static string WordFromCifir(int cifir)
        {
            string result = "";
            string cfr = cifir.ToString();

            #region Код

            #region Словарь значений

            words = new Dictionary<string, string>();
            words.Add("1", "один");
            words.Add("2", "два");
            words.Add("3", "три");
            words.Add("4", "четыре");
            words.Add("5", "пять");
            words.Add("6", "шесть");
            words.Add("7", "семь");
            words.Add("8", "восемь");
            words.Add("9", "девять");
            words.Add("10", "десять");
            words.Add("11", "одиннадцать");
            words.Add("12", "двенадцать");
            words.Add("13", "тринадцать");
            words.Add("14", "четырнадцать");
            words.Add("15", "пятнадцать");
            words.Add("16", "шестнадцать");
            words.Add("17", "семнадцать");
            words.Add("18", "восемнадцать");
            words.Add("19", "девятнадцать");
            words.Add("20", "двадцать");
            words.Add("30", "тридцать");
            words.Add("40", "сорок");
            words.Add("50", "пятьдесят");
            words.Add("60", "шестьдесят");
            words.Add("70", "семьдесят");
            words.Add("80", "восемьдесят");
            words.Add("90", "девяносто");
            words.Add("100", "сто");

            #endregion

            if (cfr.Length == 1)
            {
                if (cfr == "0") result = "out_of_range";
                else
                {
                    result = words[cfr];
                }
            }
            else if (cfr.Length == 2)
            {
                if (words.ContainsKey(cfr)) result = words[cfr];
                else
                {
                    char[] charArr = cfr.ToCharArray();
                    result = words[charArr[0].ToString() + "0"] + " " + words[charArr[1].ToString()];
                }
            }
            else if (cfr.Length == 3)
            {
                if (words.ContainsKey(cfr)) result = words[cfr];
                else result = "out_of_range";
            }
            else
            {
                result = "out_of_range";
            }

            #endregion

            return result;
        }

        #endregion
    }
    #endregion
}