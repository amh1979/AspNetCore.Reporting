using AspNetCore.Reporting.ReportExecutionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    internal class ReportEndpointBehavior : IEndpointBehavior
    {

        ReportClientMessageInspector inspector;
        public ReportEndpointBehavior(ReportExecutionServiceSoapClient client)
        {
            inspector = new ReportClientMessageInspector(client);
        }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
