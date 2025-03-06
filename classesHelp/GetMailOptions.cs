using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace site.classesHelp
{
    /// <summary>вспомогательный класс, которые достаёт параметры почтового ящика, c которого нужно отослать письмо</summary>
    public class GetMailOptions
    {
        ConfigFile config = new ConfigFile();

        /// <summary>функция, которая возвращает значение ОТ КОГО из файла options</summary>
        /// <returns></returns>
        public string GetMailFrom()
        {
            return config.GetParam("from", true);
        }

        /// <summary>функция, которая возвращает значение DNS-имени SMTP-сервера из файла options</summary>
        /// <returns></returns>
        public string GetMailDnsNameSmtp()
        {
            return config.GetParam("dnssmtp", true);
        }

        /// <summary>функция, которая возвращает значение ПОРТА SMTP-сервера из файла options</summary>
        /// <returns></returns>
        public int GetMailSmtpPort()
        {
            return Convert.ToInt32(config.GetParam("smtpport", true));
        }

        /// <summary>функция, которая возвращает значение SSL из файла options</summary>
        /// <returns></returns>
        public int GetMailSsl()
        {
            return Convert.ToInt32(config.GetParam("ssl", true)); ;
        }

        /// <summary>функция, которая возвращает значение SMTP-логина из файла options</summary>
        /// <returns></returns>
        public string GetMailSmtpL()
        {
            return config.GetParam("smtpl", true);
        }

        /// <summary>функция, которая возвращает значение SMTP-пароля из файла options</summary>
        /// <returns></returns>
        public string GetMailSmtpP()
        {
            return config.GetParam("smtpp", true);
        }
    }
}