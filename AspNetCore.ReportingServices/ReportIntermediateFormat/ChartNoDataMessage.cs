using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartNoDataMessage : ChartTitle
	{
		internal ChartNoDataMessage()
		{
		}

		internal ChartNoDataMessage(Chart chart)
			: base(chart)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartNoDataMessageStart();
			base.InitializeInternal(context);
			context.ExprHostBuilder.ChartNoDataMessageEnd();
		}

		public override ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoDataMessage;
		}
	}
}
