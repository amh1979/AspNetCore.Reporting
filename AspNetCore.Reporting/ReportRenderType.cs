using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    public enum ReportRenderType
    {
        /// <summary>
        /// NULL
        /// </summary>
        Null=0,
        /// <summary>
        /// word 2003-2007 .doc
        /// </summary>
        Word=1,
        /// <summary>
        /// word 2010-2016 .docx
        /// </summary>
        WordOpenXml=2,
        /// <summary>
        /// excel 2003-2007 .xls
        /// </summary>
        Excel=3,
        /// <summary>
        /// excel 2010-2016 .xlsx
        /// </summary>
        ExcelOpenXml=4,
        /// <summary>
        /// power point 2010-2016 .pptx
        /// </summary>
        Pptx=5,
        /// <summary>
        /// pdf file
        /// </summary>
        Pdf=6,
        /// <summary>
        /// image
        /// </summary>
        Image=7,
        /// <summary>
        /// Mhtml
        /// </summary>
        Mhtml=8,
        /// <summary>
        /// CSV
        /// </summary>
        Csv=9,
        /// <summary>
        /// XML
        /// </summary>
        Xml,
        /// <summary>
        /// Atom
        /// </summary>
        Atom,
        /// <summary>
        /// html4.0
        /// </summary>
        Html4_0,
        /// <summary>
        /// html5
        /// </summary>
        Html5,
        /// <summary>
        /// RPL
        /// </summary>
        Rpl,
    }
}
