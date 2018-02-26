using System;
using LearningPortal.Services;

namespace LearningPortal.ExceptionLogger
{
    public class MailLogException : IExceptionLogger
    {
        public void LogException(Exception exception)
        {
            IEmailSender emailSender = new EmailSender();
            emailSender.SendEmailAsync("vzdelavaciportal@seznam.cz", exception.Message, "Warning Exception");
        }
    }
}
