using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    /// <summary>
    /// Report Execute Type
    /// </summary>
    public enum ReportExecuteType
    {
        /// <summary>
        /// show html
        /// </summary>
        Display=0,
        /// <summary>
        /// export file
        /// </summary>
        Export=1,
        /// <summary>
        /// find strings in report
        /// </summary>
        FindString=2,
        /// <summary>
        /// toggle the report for show or hide child content
        /// </summary>
        Toggle=3,
        /// <summary>
        /// Print report
        /// </summary>
        Print=4,
    }
}
