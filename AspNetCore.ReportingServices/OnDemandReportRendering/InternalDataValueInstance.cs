using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataValueInstance : DataValueInstance
	{
		private DataValue m_dataValueDef;

		private string m_name;

		private object m_value;

		public override string Name
		{
			get
			{
				if (this.m_name == null)
				{
					this.EvaluateNameAndValue();
				}
				return this.m_name;
			}
		}

		public override object Value
		{
			get
			{
				if (this.m_value == null)
				{
					this.EvaluateNameAndValue();
				}
				return this.m_value;
			}
		}

		internal InternalDataValueInstance(IReportScope reportScope, DataValue dataValueDef)
			: base(reportScope)
		{
			this.m_dataValueDef = dataValueDef;
		}

		private void EvaluateNameAndValue()
		{
			TypeCode typeCode = default(TypeCode);
			this.m_dataValueDef.DataValueDef.EvaluateNameAndValue((ReportElement)null, this.ReportScopeInstance, this.m_dataValueDef.InstancePath, this.m_dataValueDef.RenderingContext.OdpContext, (ObjectType)(this.m_dataValueDef.IsChart ? 16 : 28), this.m_dataValueDef.ObjectName, out this.m_name, out this.m_value, out typeCode);
		}

		protected override void ResetInstanceCache()
		{
			this.m_name = null;
			this.m_value = null;
		}
	}
}
