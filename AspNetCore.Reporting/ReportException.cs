using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    public class ReportException : Exception
    {
        internal ReportException()
        {

        }
        public ReportException(string message) : base(message)
        {

        }
        public ReportException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
