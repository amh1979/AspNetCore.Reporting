/* ===============================================
* 功能描述：AspNetCore.Reporting.RenderType
* 创 建 者：WeiGe
* 创建日期：8/20/2018 3:39:32 PM
* ===============================================*/

using System;
using System.Collections.Generic;
using System.Text;


namespace AspNetCore.Reporting
{
    /// <summary>
    /// Report Render Type
    /// </summary>
    public enum RenderType
    {
        /// <summary>
        /// word 2003-2007 .doc
        /// </summary>
        Word,
        /// <summary>
        /// word 2010-2016 .docx
        /// </summary>
        WordOpenXml,
        /// <summary>
        /// excel 2003-2007 .xls
        /// </summary>
        Excel,
        /// <summary>
        /// excel 2010-2016 .xlsx
        /// </summary>
        ExcelOpenXml,
        /// <summary>
        /// pdf file
        /// </summary>
        Pdf,
        /// <summary>
        /// image
        /// </summary>
        Image,

        /// <summary>
        /// html5
        /// </summary>
        Html,
        /// <summary>
        /// RPL
        /// </summary>
        Rpl,
    }
}
