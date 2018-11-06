using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal abstract class DataRegion : ReportItem
	{
		public virtual bool PageBreakAtEnd
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtEnd;
			}
		}

		public virtual bool PageBreakAtStart
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtStart;
			}
		}

		public virtual bool KeepTogether
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).KeepTogether;
			}
		}

		public virtual bool NoRows
		{
			get
			{
				return false;
			}
		}

		public string NoRowMessage
		{
			get
			{
				ExpressionInfo noRows = ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).NoRows;
				if (noRows != null)
				{
					if (ExpressionInfo.Types.Constant == noRows.Type)
					{
						return noRows.Value;
					}
					return this.InstanceInfoNoRowMessage;
				}
				return null;
			}
		}

		internal abstract string InstanceInfoNoRowMessage
		{
			get;
		}

		public string DataSetName
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).DataSetName;
			}
		}

		internal DataRegion(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext)
			: base(null, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public int[] GetRepeatSiblings()
		{
			AspNetCore.ReportingServices.ReportProcessing.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef;
			if (dataRegion.RepeatSiblings == null)
			{
				return new int[0];
			}
			int[] array = new int[dataRegion.RepeatSiblings.Count];
			dataRegion.RepeatSiblings.CopyTo(array);
			return array;
		}
	}
}
