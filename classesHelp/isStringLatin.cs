using System;

namespace site.classesHelp
{
    public class IsStringLatin
    {
        #region IsLatin(string content)

        /// <summary>Метод проверяет строку на наличие не латинских символов.
        /// Возвращает TRUE - если строка содержит только латинские символы. FALSE - если строка содержит не только латинские символы</summary>
        /// <param name="content">проверяемая строка</param>
        /// <returns></returns>
        public static bool IsLatin(string content)
        {
            bool result = true;

            char[] letters = content.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = Convert.ToInt32(letters[i]);

                if (charValue > 128)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        #endregion

        #region IsLatinOrNo(string content)

        /// <summary>Метод проверяет строку на наличие латинских символов и русских букв.
        /// Возвращает StringType.LAT - если строка содержит только латинские символы. 
        /// StringType.RUS - если строка содержит только нелатинские символы
        /// StringType.RUSLAT - если содержит и те и другие симовлы</summary>
        /// <param name="str">проверяемая строка</param>
        /// <returns></returns>
        public static StringType IsLatinOrNo(string str)
        {
            StringType result = StringType.RUSLAT;

            char[] letters = str.ToCharArray();

            bool isLatExist = false;
            bool isRusExist = false;
            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = Convert.ToInt32(letters[i]);

                if (charValue > 128)
                {
                    isRusExist = true;
                }
                if (charValue <= 128)
                {
                    isLatExist = true;
                }
            }

            if (isLatExist && !isRusExist) result = StringType.LAT;
            else if (!isLatExist && isRusExist) result = StringType.RUS;
            else result = StringType.RUSLAT;

            return result;
        }

        #endregion

    }
}