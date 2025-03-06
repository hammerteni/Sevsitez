using site.classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace site.classesHelp
{
    /// <summary>класс отсылает почту на переданный в функцию sendMail() почтовый ящик - mailaddrTo.
    /// Так же в эту функцию передаются: тема письма - mailSubj, тело письма - mailBody
    /// а так же использование протокола ssl
    /// </summary>
    public class SendMailClass
    {
        /// <summary>Метод отправки почты, возвращает true, если почта успешно отправлена</summary>
        /// <param name="mailaddrTo">кому</param>
        /// <param name="mailSubj">тема письма</param>
        /// <param name="mailBody">тело письма</param>
        /// <returns></returns>
        public bool SendMail(string mailaddrTo, string mailSubj, string mailBody, CompetitionRequest req = null)
        {
            bool result = true;

            //проверка на то, что почтовый адрес передан (на пустое значение)
            if (mailaddrTo.Trim() == "" || !CompetitonWorkCommon.IsValidEmail(mailaddrTo)) 
            { 
                return false; 
            }          

            var mailBeforeChange = "";
            if (new IsDebugging().check()) {
                mailBeforeChange = mailaddrTo;
                mailaddrTo = "web@sevastopolets-moskva.ru";
            }
            
            var getmailOpt = new GetMailOptions();

            var message = new MailMessage();
            message.To.Add(new MailAddress(mailaddrTo));                    // кому отправлять
            string mailFrom = getmailOpt.GetMailFrom();
            if (mailFrom == "-1") { return false; }            //проверка на пустое значение
            message.From = new MailAddress(mailFrom);
            message.Subject = mailSubj;                                     // тема письма
            message.IsBodyHtml = true;
            message.Body = mailBody;
            if (!string.IsNullOrEmpty(mailBeforeChange)) {
                message.Body = message.Body + "<br/><br/><p><strong>Письмо было адресовано:</strong> <a href=\"mailto:" + mailBeforeChange + "\">"+ mailBeforeChange + "</a></p>";
            }

            if (req != null && req.Links.Count > 0)
            {
                var work = new CompetitionsWork();
                var linkList = work.GetFilesList(req);
                foreach (var el in linkList)
                {
                    Attachment attach = new Attachment(el.Value, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(attach);
                }
                var linklist2 = work.GetProtocolFile(req);
                foreach (var el1 in linklist2)
                {
                    Attachment attach1 = new Attachment(el1.Value, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(attach1);
                }
            }

            string dnsName = getmailOpt.GetMailDnsNameSmtp();
            if (dnsName == "-1") { return false; }             //проверка на пустое значение
            var client = new SmtpClient(dnsName);
            int ssl = getmailOpt.GetMailSsl();
            if (ssl == -1) { return false; }                        //проверка на пустое значение
            if (ssl == 1)
            {
                client.EnableSsl = true;
            }

            int smtpPort = getmailOpt.GetMailSmtpPort();
            if (smtpPort == -1) { return false; }                   //проверка на пустое значение
            client.Port = smtpPort;                                 //указываем порт

            string l = getmailOpt.GetMailSmtpL();
            if (l == "-1") { return false; }             //проверка на пустое значение
            string p = getmailOpt.GetMailSmtpP();
            if (p == "-1") { return false; }             //проверка на пустое значение
            client.Credentials = new NetworkCredential(l, p);

            try
            {
                client.Send(message);                                       //отправить
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>Метод отправки почты, возвращает true, если почта успешно отправлена. В функцию нужно передать полностью все параметры.</summary>
        /// <param name="mailaddrTo">кому</param>
        /// <param name="ssl">0 или 1</param>
        /// <param name="mailSubj">тема письма</param>
        /// <param name="mailBody">тело письма</param>
        /// <param name="mailaddrFrom">от кого</param>
        /// <param name="dnsSmtp">dns-имя smtp-сервера</param>
        /// <param name="smtpPort">порт smtp-сервера</param>
        /// <param name="smtpl">логин</param>
        /// <param name="smtpp">пароль</param>
        /// <returns></returns>
        public bool SendTestMail(string mailaddrTo, string mailaddrFrom, string dnsSmtp, int smtpPort, string ssl, string mailSubj, string mailBody, string smtpl, string smtpp)
        {
            bool result = true;

            var message = new MailMessage();
            message.To.Add(new MailAddress(mailaddrTo));                    // кому отправлять
            message.From = new MailAddress(mailaddrFrom);
            message.Subject = mailSubj;                                     // тема письма
            message.IsBodyHtml = true;
            message.Body = mailBody;

            var client = new SmtpClient(dnsSmtp);
            if (ssl == "1")
            {
                client.EnableSsl = true;
            }
            client.Port = smtpPort;                     // указываем порт
            client.Credentials = new NetworkCredential(smtpl, smtpp);

            try
            {
                client.Send(message);                                       // отправить
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }

}