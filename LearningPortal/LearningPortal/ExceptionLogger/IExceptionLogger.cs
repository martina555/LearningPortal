using System;

namespace LearningPortal.ExceptionLogger
{
    interface IExceptionLogger
    {
        void LogException(Exception exception);
    }
}
