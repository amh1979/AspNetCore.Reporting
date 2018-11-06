using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTextRunInstance : TextRunInstance
	{
		private TextBoxInstance m_textBoxInstance;

		public override string UniqueName
		{
			get
			{
				if (base.m_uniqueName == null)
				{
					AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem = base.m_reportElementDef.RenderReportItem;
					base.m_uniqueName = renderReportItem.ID + 'i' + "1" + 'i' + renderReportItem.UniqueName;
				}
				return base.m_uniqueName;
			}
		}

		public override MarkupType MarkupType
		{
			get
			{
				return MarkupType.None;
			}
		}

		public override string Value
		{
			get
			{
				return this.m_textBoxInstance.Value;
			}
		}

		public override object OriginalValue
		{
			get
			{
				return this.m_textBoxInstance.OriginalValue;
			}
		}

		public override TypeCode TypeCode
		{
			get
			{
				return this.m_textBoxInstance.TypeCode;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return false;
			}
		}

		public override bool ProcessedWithError
		{
			get
			{
				if (this.OriginalValue == null && !string.IsNullOrEmpty(this.Value))
				{
					return true;
				}
				return false;
			}
		}

		internal ShimTextRunInstance(TextRun textRunDef, TextBoxInstance textBoxInstance)
			: base(textRunDef)
		{
			this.m_textBoxInstance = textBoxInstance;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
