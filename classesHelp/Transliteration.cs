using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using site.classes;

//Файл содержит классы для транслитерации
namespace site.classesHelp
{
    #region Класс Transliteration

    /// <summary>Класс, отвечающий за транслитерацию букв</summary>
    public class Transliteration
    {
        #region Поля

        private static List<string> gostRuList = new List<string>(); //ГОСТ 16876-71
        private static List<string> gostLatList = new List<string>(); //ГОСТ 16876-71
        private static List<string> isoRuList = new List<string>(); //ISO 9-95
        private static List<string> isoLatList = new List<string>(); //ISO 9-95

        #endregion

        #region Метод Replace(string text, TransliterationType type)
        /// <summary>Главный метод запускающий преобразование букв. Метод негодится для транслитерации слов, которые состоят только из больших букв.</summary>
        /// <param name="text">текст, который нужно преобразовать</param>
        /// <param name="type">тип транслитерации</param>
        /// <returns>Возвращает изменённую строку</returns>
        public static string Replace(string text, TransliterationType type, TranslitDirection direction)
        {
            StringBuilder output = new StringBuilder();

            #region Код
            text = text.Trim();

            FillLists();
            List<string> ruList = new List<string>();
            List<string> latList = new List<string>();
            if (type == TransliterationType.Gost)
            {
                ruList = gostRuList; latList = gostLatList;
            }
            if (type == TransliterationType.ISO)
            {
                ruList = isoRuList; latList = isoLatList;
            }

            if (direction == TranslitDirection.RU_LAT)    //с русских на латинские
            {
                for (int i = 0; i <= text.Length; i++)
                {
                    for (int j = 0; j < ruList.Count; j++)
                    {
                        if (text.Contains(ruList[j]))
                        {
                            text = text.Replace(ruList[j], latList[j]);
                            break;
                        }
                    }
                }
                output.Append(text);
            }
            else if (direction == TranslitDirection.LAT_RU)    //с латинский на русские
            {
                for (int i = 0; i <= text.Length; i++)
                {
                    for (int j = 0; j < ruList.Count; j++)
                    {
                        if (latList[j] != "")
                        {
                            if (text.Contains(latList[j]))
                            {
                                text = text.Replace(latList[j], ruList[j]);
                                break;
                            }
                        }
                    }
                }
                output.Append(text);
            }
            #endregion

            return output.ToString();
        }
        #endregion

        #region Метод FillLists()
        /// <summary>Метод наполняет списки транслитерации</summary>
        static void FillLists()
        {
            gostRuList.Add("Ё"); gostLatList.Add("Jo");
            gostRuList.Add("Ж"); gostLatList.Add("Zh");
            gostRuList.Add("Й"); gostLatList.Add("Jj");
            gostRuList.Add("Х"); gostLatList.Add("Kh");
            gostRuList.Add("Ч"); gostLatList.Add("Ch");
            gostRuList.Add("Ш"); gostLatList.Add("Sh");
            gostRuList.Add("Щ"); gostLatList.Add("Shh");
            gostRuList.Add("Э"); gostLatList.Add("Eh");
            gostRuList.Add("Ю"); gostLatList.Add("Yu");
            gostRuList.Add("Я"); gostLatList.Add("Ya");

            gostRuList.Add("ё"); gostLatList.Add("jo");
            gostRuList.Add("ж"); gostLatList.Add("zh");
            gostRuList.Add("й"); gostLatList.Add("jj");
            gostRuList.Add("х"); gostLatList.Add("kh");
            gostRuList.Add("ч"); gostLatList.Add("ch");
            gostRuList.Add("ш"); gostLatList.Add("sh");
            gostRuList.Add("щ"); gostLatList.Add("shh");
            gostRuList.Add("э"); gostLatList.Add("eh");
            gostRuList.Add("ю"); gostLatList.Add("yu");
            gostRuList.Add("я"); gostLatList.Add("ya");

            gostRuList.Add("А"); gostLatList.Add("A");
            gostRuList.Add("Б"); gostLatList.Add("B");
            gostRuList.Add("В"); gostLatList.Add("V");
            gostRuList.Add("Г"); gostLatList.Add("G");
            gostRuList.Add("Д"); gostLatList.Add("D");
            gostRuList.Add("Е"); gostLatList.Add("E");
            gostRuList.Add("З"); gostLatList.Add("Z");
            gostRuList.Add("И"); gostLatList.Add("I");
            gostRuList.Add("К"); gostLatList.Add("K");
            gostRuList.Add("Л"); gostLatList.Add("L");
            gostRuList.Add("М"); gostLatList.Add("M");
            gostRuList.Add("Н"); gostLatList.Add("N");
            gostRuList.Add("О"); gostLatList.Add("O");
            gostRuList.Add("П"); gostLatList.Add("P");
            gostRuList.Add("Р"); gostLatList.Add("R");
            gostRuList.Add("С"); gostLatList.Add("S");
            gostRuList.Add("Т"); gostLatList.Add("T");
            gostRuList.Add("У"); gostLatList.Add("U");
            gostRuList.Add("Ф"); gostLatList.Add("F");
            gostRuList.Add("Ц"); gostLatList.Add("C");
            gostRuList.Add("Ъ"); gostLatList.Add("'");
            gostRuList.Add("Ы"); gostLatList.Add("Y");
            gostRuList.Add("Ь"); gostLatList.Add("");

            gostRuList.Add("а"); gostLatList.Add("a");
            gostRuList.Add("б"); gostLatList.Add("b");
            gostRuList.Add("в"); gostLatList.Add("v");
            gostRuList.Add("г"); gostLatList.Add("g");
            gostRuList.Add("д"); gostLatList.Add("d");
            gostRuList.Add("е"); gostLatList.Add("e");
            gostRuList.Add("з"); gostLatList.Add("z");
            gostRuList.Add("и"); gostLatList.Add("i");
            gostRuList.Add("к"); gostLatList.Add("k");
            gostRuList.Add("л"); gostLatList.Add("l");
            gostRuList.Add("м"); gostLatList.Add("m");
            gostRuList.Add("н"); gostLatList.Add("n");
            gostRuList.Add("о"); gostLatList.Add("o");
            gostRuList.Add("п"); gostLatList.Add("p");
            gostRuList.Add("р"); gostLatList.Add("r");
            gostRuList.Add("с"); gostLatList.Add("s");
            gostRuList.Add("т"); gostLatList.Add("t");
            gostRuList.Add("у"); gostLatList.Add("u");
            gostRuList.Add("ф"); gostLatList.Add("f");
            gostRuList.Add("ц"); gostLatList.Add("c");
            gostRuList.Add("ъ"); gostLatList.Add("");
            gostRuList.Add("ы"); gostLatList.Add("y");
            gostRuList.Add("ь"); gostLatList.Add("");


            isoRuList.Add("Ё"); isoLatList.Add("Yo");
            isoRuList.Add("Ж"); isoLatList.Add("Zh");
            isoRuList.Add("Ч"); isoLatList.Add("Ch");
            isoRuList.Add("Ш"); isoLatList.Add("Sh");
            isoRuList.Add("Щ"); isoLatList.Add("Shh");
            isoRuList.Add("Ю"); isoLatList.Add("Yu");
            isoRuList.Add("Я"); isoLatList.Add("Ya");

            isoRuList.Add("ё"); isoLatList.Add("yo");
            isoRuList.Add("ж"); isoLatList.Add("zh");
            isoRuList.Add("ч"); isoLatList.Add("ch");
            isoRuList.Add("ш"); isoLatList.Add("sh");
            isoRuList.Add("щ"); isoLatList.Add("shh");
            isoRuList.Add("ю"); isoLatList.Add("yu");
            isoRuList.Add("я"); isoLatList.Add("ya");

            isoRuList.Add("А"); isoLatList.Add("A");
            isoRuList.Add("Б"); isoLatList.Add("B");
            isoRuList.Add("В"); isoLatList.Add("V");
            isoRuList.Add("Г"); isoLatList.Add("G");
            isoRuList.Add("Д"); isoLatList.Add("D");
            isoRuList.Add("Е"); isoLatList.Add("E");
            isoRuList.Add("З"); isoLatList.Add("Z");
            isoRuList.Add("И"); isoLatList.Add("I");
            isoRuList.Add("Й"); isoLatList.Add("J");
            isoRuList.Add("К"); isoLatList.Add("K");
            isoRuList.Add("Л"); isoLatList.Add("L");
            isoRuList.Add("М"); isoLatList.Add("M");
            isoRuList.Add("Н"); isoLatList.Add("N");
            isoRuList.Add("О"); isoLatList.Add("O");
            isoRuList.Add("П"); isoLatList.Add("P");
            isoRuList.Add("Р"); isoLatList.Add("R");
            isoRuList.Add("С"); isoLatList.Add("S");
            isoRuList.Add("Т"); isoLatList.Add("T");
            isoRuList.Add("У"); isoLatList.Add("U");
            isoRuList.Add("Ф"); isoLatList.Add("F");
            isoRuList.Add("Х"); isoLatList.Add("X");
            isoRuList.Add("Ц"); isoLatList.Add("C");
            isoRuList.Add("Ъ"); isoLatList.Add("'");
            isoRuList.Add("Ы"); isoLatList.Add("Y");
            isoRuList.Add("Ь"); isoLatList.Add("");
            isoRuList.Add("Э"); isoLatList.Add("E");

            isoRuList.Add("а"); isoLatList.Add("a");
            isoRuList.Add("б"); isoLatList.Add("b");
            isoRuList.Add("в"); isoLatList.Add("v");
            isoRuList.Add("г"); isoLatList.Add("g");
            isoRuList.Add("д"); isoLatList.Add("d");
            isoRuList.Add("е"); isoLatList.Add("e");
            isoRuList.Add("з"); isoLatList.Add("z");
            isoRuList.Add("и"); isoLatList.Add("i");
            isoRuList.Add("й"); isoLatList.Add("j");
            isoRuList.Add("к"); isoLatList.Add("k");
            isoRuList.Add("л"); isoLatList.Add("l");
            isoRuList.Add("м"); isoLatList.Add("m");
            isoRuList.Add("н"); isoLatList.Add("n");
            isoRuList.Add("о"); isoLatList.Add("o");
            isoRuList.Add("п"); isoLatList.Add("p");
            isoRuList.Add("р"); isoLatList.Add("r");
            isoRuList.Add("с"); isoLatList.Add("s");
            isoRuList.Add("т"); isoLatList.Add("t");
            isoRuList.Add("у"); isoLatList.Add("u");
            isoRuList.Add("ф"); isoLatList.Add("f");
            isoRuList.Add("х"); isoLatList.Add("x");
            isoRuList.Add("ц"); isoLatList.Add("c");
            isoRuList.Add("ъ"); isoLatList.Add("");
            isoRuList.Add("ы"); isoLatList.Add("y");
            isoRuList.Add("ь"); isoLatList.Add("");
            isoRuList.Add("э"); isoLatList.Add("e");
        }
        #endregion
    }

    #endregion
}