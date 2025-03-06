
using System;

namespace site.classesHelp
{
    #region Класс StringToInt

    /// <summary>Класс преобразует строку в цифру с помощью статического метода Parse
    /// </summary>
    public class StringToNum
    {
        #region Метод ParseInt(string s)

        /// <summary>Метод преобразует строку в цифру(int). Возвращает -1 если строку не удаётся привести к цифре.</summary>
        /// <param name="s">строка</param>
        /// <returns>возвращает цифру в любом случае, если строку не удаётся преобразовать в цифру, то метод
        /// возвращает -1</returns>
        public static int ParseInt(string s)
        {
            int result = 0;

            if(!int.TryParse(s, out result))
            {
                result = -1;
            }

            return result;
        }

        #endregion
        #region Метод ParseLong(string s)

        /// <summary>Метод преобразует строку в цифру(long). Возвращает -1 если строку не удаётся привести к цифре.</summary>
        /// <param name="s">строка</param>
        /// <returns>возвращает цифру в любом случае, если строку не удаётся преобразовать в цифру, то метод
        /// возвращает -1</returns>
        public static long ParseLong(string s)
        {
            long result = 0;

            if (!long.TryParse(s, out result))
            {
                result = -1;
            }

            return result;
        }

        #endregion
    }

    #endregion
}