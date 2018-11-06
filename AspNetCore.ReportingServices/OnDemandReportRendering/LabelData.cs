using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.ObjectModel;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LabelData
	{
		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.LabelData m_labelData;

		private ReadOnlyCollection<string> m_keyFields;

		public string DataSetName
		{
			get
			{
				return this.m_labelData.DataSetName;
			}
		}

		[Obsolete("Use KeyFields instead.")]
		public string Key
		{
			get
			{
				return this.KeyFields[0];
			}
		}

		public ReadOnlyCollection<string> KeyFields
		{
			get
			{
				if (this.m_keyFields == null)
				{
					this.m_keyFields = new ReadOnlyCollection<string>(this.m_labelData.KeyFields);
				}
				return this.m_keyFields;
			}
		}

		public string Label
		{
			get
			{
				return this.m_labelData.Label;
			}
		}

		internal LabelData(AspNetCore.ReportingServices.ReportIntermediateFormat.LabelData labelData)
		{
			this.m_labelData = labelData;
		}
	}
}
