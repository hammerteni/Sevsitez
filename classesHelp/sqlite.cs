
// Файл с классами для работы с данными в БД SQLite

using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace site.classesHelp
{
    #region Класс SqliteHelper

    /// <summary>Класс содержит в себе методы для работы с БД SQLite</summary>
    public class SqliteHelper
    {
        #region Поля

        private bool _checkDbFile = true;   // будет содержать false, если в методе-конструкторе произойдёт ошибка
        private string _pathToDb;
        private SQLiteConnection _connection;

        #endregion
        #region Конструктор 

        /// <summary>Конструктор. Проверяет переданный в него путь на наличие папок и самой БД. Если чего-то нет, то 
        /// оно создаётся</summary>
        /// <param name="pathToDb"></param>
        public SqliteHelper(string pathToDb)
        {
            _pathToDb = pathToDb;

            if (!File.Exists(pathToDb))
            {
                try
                {
                    Directory.CreateDirectory(_pathToDb.Replace(Path.GetFileName(_pathToDb), ""));
                }
                catch (Exception ex)
                {
                    _checkDbFile = false;
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    return;
                }
                try
                {
                    SQLiteConnection.CreateFile(_pathToDb);
                }
                catch (Exception ex)
                {
                    _checkDbFile = false;
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    return;
                }
            }

            try
            {
                _connection = new SQLiteConnection(string.Format("Data Source={0}; Pooling=true;", _pathToDb));
            }
            catch (Exception ex)
            {
                _checkDbFile = false;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        #endregion
        #region Метод int ExecuteNonQuery(string sqlQuery)

        /// <summary>Метод делает запрос на исполнение к БД. Запрос без защиты от SQL-
        /// инъекций. Запрос может использоваться для создания таблицы, к примеру.
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns>Возвращает количество добавленных или обновлённых строк в таблице. -1 - если запрос не выполнился или в случае ошибки.</returns>
        public int ExecuteNonQuery(string sqlQuery)
        {
            if (!_checkDbFile)
            {
                ConnectionClose();
                return -1;
            }

            int result = 0;
            SQLiteCommand cmd = new SQLiteCommand(sqlQuery, _connection);
            try
            {
                _connection.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _connection.Close(); _connection.Dispose();
                result = -1; // -1 - показатель ошибки
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return result;
        }

        #endregion
        #region Метод int ExecuteNonQueryParams(SQLiteCommand cmd)

        /// <summary>Метод делает запрос на исполнение к БД. Запрос с защитой от SQL-
        /// инъекций. Запрос может использоваться для вставки, обновления или удаления данных из таблицы, к примеру.
        /// </summary>
        /// <param name="cmd">Создаётся так:
        /// SQLiteCommand cmd = new SQLiteCommand();
        /// cmd.CommandText = "INSERT INTO logs (datestamp, text) VALUES (@datestamp, @text)";
        /// cmd.Parameters.Add(new SQLiteParameter("datestamp", DateTime.Now.Ticks));
        /// cmd.Parameters.Add(new SQLiteParameter("text", "какая-то строка.."));
        /// </param>
        /// <returns>Возвращает количество добавленных, удалённых или обновлённых строк в таблице. -1 - если запрос не выполнился или в случае ошибки.</returns>
        public int ExecuteNonQueryParams(SQLiteCommand cmd)
        {
            if (!_checkDbFile)
            {
                ConnectionClose();
                return -1;
            }

            int result = 0;
            try
            {
                cmd.Connection = _connection;
                _connection.Open();
                cmd.Prepare();
                //result = cmd.ExecuteNonQuery(System.Data.CommandBehavior.SequentialAccess);
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _connection.Close(); _connection.Dispose();
                result = -1;            // -1 - показатель ошибки
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return result;
        }

        #endregion
        #region Метод int ExecuteScalarParams(SQLiteCommand cmd)

        /// <summary>Метод делает запрос на исполнение к БД. Запрос с защитой от SQL-
        /// инъекций. Запрос может использоваться для запросов на подсчёт чего-нибудь. То бишь возвращается одно значение.
        /// </summary>
        /// <param name="cmd">Создаётся так:
        /// SQLiteCommand cmd = new SQLiteCommand();
        /// cmd.CommandText = "SELECT COUNT(@x) FROM logs";
        /// cmd.Parameters.Add(new SQLiteParameter("x", "*"));
        /// </param>
        /// <returns>Возвращает вычисленное значение. -1 - если запрос не выполнился или в случае ошибки или в том случае, если в таблице нет ни одной записи, а нужно получить максимальное значение
        /// по какому-либо полю..</returns>
        public int ExecuteScalarParams(SQLiteCommand cmd)
        {
            if (!_checkDbFile)
            {
                ConnectionClose();
                return -1;
            }

            int result = 0;
            try
            {
                cmd.Connection = _connection;
                _connection.Open();
                result = StringToNum.ParseInt(cmd.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                _connection.Close(); _connection.Dispose();
                result = -1;            // -1 - показатель ошибки
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return result;
        }

        #endregion
        #region Метод SQLiteDataReader ExecuteReader(SQLiteCommand cmd)

        /// <summary>Метод делает запрос к БД с возвращением объекта со значениями строк. 
        /// Запрос с защитой от SQL-инъекций. 
        /// </summary>
        /// <param name="cmd">Создаётся так:
        /// SQLiteCommand cmd = new SQLiteCommand();
        /// cmd.CommandText = "SELECT @x FROM logs";
        /// cmd.Parameters.Add(new SQLiteParameter("x", "*"));
        /// </param>
        /// <returns>Возвращает объект. null - если запрос не выполнился или в случае ошибки.</returns>
        public SQLiteDataReader ExecuteReader(SQLiteCommand cmd)
        {
            if (!_checkDbFile)
            {
                ConnectionClose();
                return null;
            }

            SQLiteDataReader result;

            try
            {
                cmd.Connection = _connection;
                _connection.Open();
                result = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                _connection.Close(); _connection.Dispose();
                result = null;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            return result;
        }

        #endregion
        #region Метод void ConnectionClose()

        /// <summary>Метод для закрытия соединения после 
        /// использования метода ExecuteReader(...)
        /// </summary>
        public void ConnectionClose()
        {
            if (_connection != null)
            {
                _connection.Close(); _connection.Dispose();
            }
        }

        #endregion

        #region Метод bool Vacuum()

        /// <summary>Метод выполняет в БД команду VACUUM</summary>
        /// <returns>возвращает false - в случае какой-либо ошибки</returns>
        public bool Vacuum()
        {
            bool result = true;

            #region Код

            int res = ExecuteNonQuery("VACUUM;");
            if (res == -1)
            {
                result = false;
            }

            #endregion

            return result;
        }

        #endregion
    }

    #endregion

    /*Получение списка таблиц БД------------------------------

    const string databaseName = @"C:\cyber.db";
    SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
    connection.Open();
    SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", connection);
    SQLiteDataReader reader = command.ExecuteReader();
    foreach (DbDataRecord record in reader)
    Console.WriteLine("Таблица: " + record["name"]);
    connection.Close();
    */

    #region Примеры запросов

    //SqliteHelper sqlite = new SqliteHelper(HttpContext.Current.Server.MapPath("~") + @"files\sqlitedb\konkurses.db");

    /*string sqlString = "CREATE TABLE IF NOT EXISTS logs(_id INTEGER PRIMARY KEY AUTOINCREMENT, datestamp INTEGER NOT NULL, text TEXT NOT NULL)";
    Response.Write(sqlite.ExecuteNonQuery(sqlString));
    Response.Write("<br>");*/

    /*SQLiteCommand cmd = new SQLiteCommand();
    cmd.CommandText = "INSERT INTO logs (datestamp, text) VALUES (@datestamp, @text)";
    cmd.Parameters.Add(new SQLiteParameter("datestamp", DateTime.Now.Ticks));
    cmd.Parameters.Add(new SQLiteParameter("text", "какая-то строка.."));
    Response.Write(sqlite.ExecuteNonQueryParams(cmd));
    Response.Write("<br>");
    cmd.Dispose();*/

    /*SQLiteCommand cmd = new SQLiteCommand();
    cmd.CommandText = "SELECT COUNT(@x) FROM logs";
    cmd.Parameters.Add(new SQLiteParameter("x", "*"));
    Response.Write(sqlite.ExecuteScalarParams(cmd));
    Response.Write("<br>");
    cmd.Dispose();*/

    /*SQLiteCommand cmd = new SQLiteCommand();
    cmd.CommandText = "SELECT * FROM logs";
    //cmd.Parameters.Add(new SQLiteParameter("x", "*"));
    SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
    DateTime dt = new DateTime();
    try
    {
        while (reader.Read())
        {
            dt = new DateTime(Int64.Parse(reader["datestamp"].ToString()));
            Response.Write(reader["_id"] + " --- " +
                           dt.ToShortDateString() + " " + dt.ToLongTimeString() + " --- " +
                           reader["text"] + "<br>");
        }
    }
    catch (Exception ex)
    {

    }
    finally
    {
        reader.Close(); reader.Dispose();
        sqlite.ConnectionClose();
    }*/

    /*SQLiteCommand cmd = new SQLiteCommand();
    cmd.CommandText = "DELETE FROM logs WHERE _id=@id";
    cmd.Parameters.Add(new SQLiteParameter("id", 1));
    Response.Write(sqlite.ExecuteNonQueryParams(cmd));
    Response.Write("<br>");
    cmd.Dispose();*/

    #endregion
}