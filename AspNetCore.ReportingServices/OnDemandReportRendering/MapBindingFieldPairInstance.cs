namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPairInstance : BaseInstance
	{
		private MapBindingFieldPair m_defObject;

		private string m_fieldName;

		private object m_bindingExpression;

		public string FieldName
		{
			get
			{
				if (this.m_fieldName == null)
				{
					this.m_fieldName = this.m_defObject.MapBindingFieldPairDef.EvaluateFieldName(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_fieldName;
			}
		}

		public object BindingExpression
		{
			get
			{
				if (this.m_bindingExpression == null)
				{
					this.m_bindingExpression = this.m_defObject.MapBindingFieldPairDef.EvaluateBindingExpression(this.m_defObject.ReportScope.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_bindingExpression;
			}
		}

		internal MapBindingFieldPairInstance(MapBindingFieldPair defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_fieldName = null;
			this.m_bindingExpression = null;
		}
	}
}
