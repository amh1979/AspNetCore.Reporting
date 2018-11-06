using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomProperty
	{
		private ReportStringProperty m_name;

		private ReportVariantProperty m_value;

		private CustomPropertyInstance m_instance;

		private RenderingContext m_renderingContext;

		private ReportElement m_reportElementOwner;

		public ReportStringProperty Name
		{
			get
			{
				return this.m_name;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ReportElement ReportElementOwner
		{
			get
			{
				return this.m_reportElementOwner;
			}
		}

		public CustomPropertyInstance Instance
		{
			get
			{
				if (this.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return this.m_instance;
			}
		}

		internal CustomProperty(ReportElement reportElementOwner, RenderingContext renderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			this.m_reportElementOwner = reportElementOwner;
			this.Init(nameExpr, valueExpr, name, value, typeCode);
			this.m_renderingContext = renderingContext;
		}

		internal CustomProperty(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo nameExpr, AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			this.m_name = new ReportStringProperty(nameExpr);
			this.m_value = new ReportVariantProperty(valueExpr);
			if (nameExpr.IsExpression || valueExpr.IsExpression)
			{
				this.m_instance = new CustomPropertyInstance(this, name, value, typeCode);
			}
			this.m_renderingContext = renderingContext;
		}

		private void Init(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr, string name, object value, TypeCode typeCode)
		{
			this.m_name = new ReportStringProperty(nameExpr);
			this.m_value = new ReportVariantProperty(valueExpr);
			if (nameExpr.IsExpression || valueExpr.IsExpression)
			{
				this.m_instance = new CustomPropertyInstance(this, name, value, typeCode);
			}
			else
			{
				this.m_instance = null;
			}
		}

		internal void Update(string name, object value, TypeCode typeCode)
		{
			if (this.m_instance != null)
			{
				this.m_instance.Update(name, value, typeCode);
			}
		}

		internal void ConstructCustomPropertyDefinition(AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue dataValueDef)
		{
			Global.Tracer.Assert(this.m_reportElementOwner != null && this.m_instance != null, "m_reportElementOwner != null && m_instance != null");
			if (this.m_instance.Name != null)
			{
				dataValueDef.Name = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(this.m_instance.Name);
			}
			else
			{
				dataValueDef.Name = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			if (this.m_instance.Value != null)
			{
				dataValueDef.Value = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((string)this.m_instance.Value);
			}
			else
			{
				dataValueDef.Value = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.Init(dataValueDef.Name, dataValueDef.Value, this.m_instance.Name, this.m_instance.Value, this.m_instance.TypeCode);
		}
	}
}
