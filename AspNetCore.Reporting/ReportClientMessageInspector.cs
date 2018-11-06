using AspNetCore.Reporting.ReportExecutionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    internal class ReportClientMessageInspector : IClientMessageInspector
    {
        ReportExecutionServiceSoapClient client;
        public ReportClientMessageInspector(ReportExecutionServiceSoapClient client)
        {
            this.client = client;
        }
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(new ReportMessageHeader(client.ExecutionHeader.ExecutionID));

            return null;
        }
    }
}
