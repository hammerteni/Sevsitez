using System;
using System.Collections.Generic;
using System.Text;

namespace site.classesHelp
{
    /// <summary>класс кодирования информации</summary>
    public class EncDecClass
    {
        /// <summary>функция кодирования текста</summary>
        /// <param name="sourcetext">в эту переменную передаётся открытый текст</param>
        /// <returns></returns>
        public string Enctext(string sourcetext)
        {
            var charstring = new StringBuilder();
            if (!string.IsNullOrEmpty(sourcetext))
            {
                Random rn = new Random();
                byte[] charOfsourceText = Encoding.Default.GetBytes(sourcetext);
                foreach (byte item in charOfsourceText)
                {
                    if (item.ToString().Length == 2)        //если длина кода символа - 2 знака, то добавляем случайную цифру слева от 3-х до 9-ти
                    {
                        charstring.Append(rn.Next(3, 9));
                        charstring.Append(item);
                    }
                    else if (item.ToString().Length == 3)   //если длина кода символа - 3 знака, то ничего не добавляем, а просто дописываем в строку код символа
                    {
                        charstring.Append(item);
                    }
                }
                rn = null; charOfsourceText = null;
            }
            if (charstring != null)
            {
                return charstring.ToString();
            }
            return "";
        }

        /// <summary>функция раскодирования текста</summary>
        /// <param name="codetext">в эту переменную передаётся кодированный текст</param>
        /// <returns></returns>
        public string Dectext(string codetext)
        {
            if (!string.IsNullOrEmpty(codetext))
            {
                string tempString;
                List<byte> byteList = new List<byte>();
                while (true)
                {
                    if (codetext.Length != 0)   //если в строке остались символы, то продолжаем..
                    {
                        tempString = codetext.Substring(0, 3);
                        codetext = codetext.Remove(0, 3);
                        if (Convert.ToInt32(tempString) > 256)          //если полученные три цифры превышают значение 256, значит код символа на самом деле содержится в последних двух цифрах
                        {
                            tempString = tempString.Substring(1, 2);
                        }
                        byteList.Add(Convert.ToByte(tempString));       //добавляем в список битов получившуюся сконвертированную строку
                    }
                    else                        //иначе выходим из цикла
                    {
                        break;
                    }
                }
                byte[] decodeBytesArray = new byte[byteList.Count];     //создаём новый массив битов
                byteList.CopyTo(decodeBytesArray);                      //копируем список битов в этот массив

                codetext = ""; tempString = ""; byteList = null;

                return Encoding.GetEncoding(1251).GetString(decodeBytesArray, 0, decodeBytesArray.Length);     //и наконец преобразовываем коды символов в символы и возвращаем их
            }
            return "";
        }
    }
}