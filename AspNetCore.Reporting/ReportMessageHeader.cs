using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace AspNetCore.Reporting
{
    internal class ReportMessageHeader : MessageHeader
    {
        readonly string _value;
        public ReportMessageHeader(string value)
        {
            _value = value;
        }
        const string _name = "ExecutionHeader";
        const string _nameSpace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices";
        public override string Name
        {
            get { return _name; }
        }

        public override string Namespace
        {
            get
            {
                return _nameSpace;
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteElementString("ExecutionID", _value);
        }
    }
}
