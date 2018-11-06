using System;
using System.ComponentModel.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class TraceManager : IServiceProvider
	{
		internal IServiceContainer serviceContainer;

		private ITraceContext traceContext;

		internal ITraceContext TraceContext
		{
			get
			{
				return this.traceContext;
			}
			set
			{
				this.traceContext = value;
			}
		}

		public bool TraceEnabled
		{
			get
			{
				if (this.TraceContext == null)
				{
					return false;
				}
				return this.TraceContext.TraceEnabled;
			}
		}

		private TraceManager()
		{
		}

		public TraceManager(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
		}

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(TraceManager))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionTraceManagerUnsupportedType(serviceType.ToString()));
		}

		public void Write(string category, string message)
		{
			if (this.TraceContext != null)
			{
				this.TraceContext.Write(category, message);
			}
		}
	}
}
