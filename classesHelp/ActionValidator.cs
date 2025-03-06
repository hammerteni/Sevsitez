using System;
using System.Web;
using System.Web.Caching;

namespace Secure
{
    /// <summary>
    /// Валидатор запросов
    /// </summary>
    public class ActionValidator
    {
        /// <summary>
        /// Информация о запросах по UserHostAddress
        /// </summary>
        private class HitInfo
        {
            public int Hits;
            public DateTime ExpiresAt
            {
                get
                {
                    return DateTime.Now.AddMinutes(Duration);
                }
            }
        }

        /// <summary>
        /// Период валидации в минутах
        /// </summary>
        private const int Duration = 10;

        /// <summary>
        /// Валидация типа запроса
        /// </summary>
        /// <param name="actionType">Тип запроса</param>
        /// <returns>Возвращает результат валидации типа запроса</returns>
        public bool IsValid(ActionType actionType)
        {
            HttpContext context = HttpContext.Current;
            if (context.Request.Browser.Crawler)
            {
                return false;
            }

            string key = actionType.ToString() + context.Request.UserHostAddress;
            HitInfo hit = (HitInfo)(context.Cache[key] ?? new HitInfo());

            if (hit.Hits > (int)actionType)
            {
                return false;
            }
            else
            {
                hit.Hits++;
            }

            if (hit.Hits == 1)
            {
                context.Cache.Add(key, hit, null, hit.ExpiresAt, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return true;
        }

        /// <summary>
        /// Проверка типа запроса
        /// </summary>
        /// <param name="actionType">Тип запроса</param>
        public void Check(ActionType actionType)
        {
            if (!IsValid(actionType))
            {
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// Первое посещение
        /// </summary>
        /// <returns></returns>
        public bool IsFirstVisit()
        {
            if (HttpContext.Current.Session["IsFirstVisit"] == null)
            {
                HttpContext.Current.Session["IsFirstVisit"] = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Инициализация валидатора
        /// </summary>
        /// <param name="isPostBack">Факт постбека</param>
        public static void Initialize(bool isPostBack)
        {
            // Создание валидатора
            var validator = new ActionValidator();

            if (!isPostBack)
            {
                // Блокировать первое обращение
                if (validator.IsFirstVisit())
                {
                    validator.Check(ActionType.FirstVisit);
                }
                else
                {
                    // Блокировать первое обращение
                    validator.Check(ActionType.ReVisit);
                }
            }
            else
            {
                // Блокировать все постбеки
                validator.Check(ActionType.Postback);
            }
        }
    }

    /// <summary>
    /// Тип запроса
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// По умолчанию
        /// </summary>
        None = 0,
        /// <summary>
        /// Первое посещение
        /// </summary>
        FirstVisit = 100,
        /// <summary>
        /// Очередное посещение
        /// </summary>
        ReVisit = 1000,
        /// <summary>
        /// Посещение при постбеке
        /// </summary>
        Postback = 5000,
        /// <summary>
        /// AddNewWidget
        /// </summary>
        AddNewWidget = 100,
        /// <summary>
        /// AddNewPage
        /// </summary>
        AddNewPage = 100,
    }
}