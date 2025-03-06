
namespace site.classesHelp
{
    /// <summary>класс преобразует строку в цифру, результат извлекается из свойств
    /// isint - находится ли в строке цифра (true/false)
    /// result - результат преобразования в int-переменной
    /// </summary>
    public class ParseStringToIntClass
    {
        public bool isint { get; set; }
        public int result { get; set; }

        public ParseStringToIntClass(string str)
        {
            int res;
            if (int.TryParse(str, out res))
            {
                result = res;
                isint = true;
            }
            else
            {
                isint = false;
            }
        }
    }
}