namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldNameInstance : BaseInstance
	{
		private MapFieldName m_defObject;

		private string m_name;

		public string Name
		{
			get
			{
				if (this.m_name == null)
				{
					this.m_name = this.m_defObject.MapFieldNameDef.EvaluateName(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_name;
			}
		}

		internal MapFieldNameInstance(MapFieldName defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_name = null;
		}
	}
}
