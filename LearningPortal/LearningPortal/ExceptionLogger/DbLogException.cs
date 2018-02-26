using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.ExceptionLogger
{
    public class DbLogException : IExceptionLogger
    {
        public void LogException(Exception exception)
        {
            //Logika pro uložení do databáze
        }
    }
}
