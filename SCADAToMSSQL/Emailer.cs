/****************************** Module Header ******************************\
Module Name:    Emailer [static]
Project:        SCADAToMSSQL
Summary:        A static utility class for sending emails out.
Author[s]:      Ryan Cooper
Email[s]:       rcooper@edwardsaquifer.org
\***************************************************************************/

using System.Net;
using System.Net.Mail;

namespace SCADAToMSSQL
{
    public static class Emailer
    {
        #region Public Methods

        /// <summary>
        /// Sends an email continaing a subect and message
        /// </summary>
        /// <param name="subject">The subject of the email</param>
        /// <param name="message">The content of the email</param>
        public static void Send(string subject, string message)
        {
            var mail = new MailMessage(, );//need to add credentials and smtp host info
            var client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = ,
                Credentials = ,
                EnableSsl = true,
            };

            mail.Subject = subject;
            mail.Body = message;
            client.Send(mail);
        }

        /// <summary>
        /// Sends an email containing a subject, message, and attachment
        /// </summary>
        /// <param name="subject">The subject of the email</param>
        /// <param name="message">The content of the email</param>
        /// <param name="filename">The attachment path</param>
        public static void Send(string subject, string message, string filename)
        {
            var mail = new MailMessage(, );//need to add credentials and smtp host info
            var client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = ,
                Credentials = ,
                EnableSsl = true,
            };

            mail.Subject = subject;
            mail.Body = message;
            mail.Attachments.Add(new Attachment(filename));
            client.Send(mail);
        }

        #endregion Public Methods
    }
}