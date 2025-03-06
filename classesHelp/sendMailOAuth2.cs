using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using site.classes;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace site.classesHelp
{
    /// <summary>класс отсылает почту: тема письма - mailSubj, тело письма - mailBody
    /// а так же использование по протоколу OAuth2
    /// </summary>
    public class SendMailOAuth2
    {

        private string googleClientId = "233792567616-odsef9rddu9nhr9f0p4p8h4kf5hrv119.apps.googleusercontent.com";
        private string googleClientSecretKey = "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";
        private string[] scopes = new string[] { GmailService.Scope.GmailSend };

        private UserCredential credential;

        public SendMailOAuth2()
        {
            credential = Login(googleClientId, googleClientSecretKey, scopes);
        }

        // Encoding method
        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private static UserCredential Login(string googleClientId, string googleClientSecretKey, string[] scopes)
        {
            var secrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecretKey
            };
            return GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, "user", CancellationToken.None).Result;
        }

        /// <summary>Метод отправки почты, возвращает true, если почта успешно отправлена</summary>
        /// <param name="mailaddrTo">кому</param>
        /// <param name="mailSubj">тема письма</param>
        /// <param name="mailBody">тело письма</param>
        /// <returns></returns>
        public bool SendMail(string mailaddrTo, string mailSubj, string mailBody)
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

            var mm = MimeMessage.CreateFromMailMessage(message);
            byte[] rawMimeData;
            using (var memory = new MemoryStream())
            {
                mm.WriteTo(memory);
                rawMimeData = memory.ToArray();
            }

            var encodedMessage = Base64UrlEncode(rawMimeData);

            var gmailMessage = new Message()
            {
                Raw = encodedMessage
            };

            try
            {
                using (var gmailService = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential }))
                {
                    gmailService.Users.Messages.Send(gmailMessage, "me").Execute();
                }
            }
            catch (GoogleApiException gex)
            {
                result = false;
            }
            catch (Exception ex)
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

            var mm = MimeMessage.CreateFromMailMessage(message);
            byte[] rawMimeData;
            using (var memory = new MemoryStream())
            {
                mm.WriteTo(memory);
                rawMimeData = memory.ToArray();
            }

            var encodedMessage = Base64UrlEncode(rawMimeData);

            var gmailMessage = new Message()
            {
                Raw = encodedMessage
            };

            try
            {
                using (var gmailService = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential }))
                {
                    gmailService.Users.Messages.Send(gmailMessage, "me").Execute();
                }
            }
            catch (GoogleApiException gex)
            {
                result = false;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
       

    
    
}