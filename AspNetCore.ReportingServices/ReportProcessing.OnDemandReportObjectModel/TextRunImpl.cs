using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class TextRunImpl : TextRun
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun m_textRunDef;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private IScope m_scope;

		private List<string> m_fieldsUsedInValueExpression;

		public override object Value
		{
			get
			{
				this.GetResult(null);
				return this.m_result.Value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun TextRunDef
		{
			get
			{
				return this.m_textRunDef;
			}
		}

		internal TextRunImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textBoxDef, AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRunDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			this.m_textBoxDef = textBoxDef;
			this.m_textRunDef = textRunDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = iErrorContext;
			this.m_scope = scope;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult GetResult(IReportScopeInstance romInstance)
		{
			if (!this.m_isValueReady)
			{
				if (this.m_isVisited)
				{
					this.m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, this.m_textRunDef.ObjectType, this.m_textRunDef.Name, "Value");
					throw new ReportProcessingException_InvalidOperationException();
				}
				this.m_isVisited = true;
				ObjectType objectType = this.m_reportRT.ObjectType;
				string objectName = this.m_reportRT.ObjectName;
				string propertyName = this.m_reportRT.PropertyName;
				IScope currentScope = this.m_reportRT.CurrentScope;
				this.m_reportRT.CurrentScope = this.m_scope;
				OnDemandProcessingContext odpContext = this.m_reportRT.ReportObjectModel.OdpContext;
				ObjectModelImpl reportObjectModel = this.m_reportRT.ReportObjectModel;
				try
				{
					odpContext.SetupContext(this.m_textBoxDef, romInstance);
					bool flag = (this.m_textRunDef.Action != null && this.m_textRunDef.Action.TrackFieldsUsedInValueExpression) || (this.m_textBoxDef != null && this.m_textBoxDef.Action != null && this.m_textBoxDef.Action.TrackFieldsUsedInValueExpression);
					if (flag)
					{
						reportObjectModel.ResetFieldsUsedInExpression();
					}
					this.m_result = this.m_reportRT.EvaluateTextRunValueExpression(this.m_textRunDef);
					if (flag)
					{
						this.m_fieldsUsedInValueExpression = new List<string>();
						reportObjectModel.AddFieldsUsedInExpression(this.m_fieldsUsedInValueExpression);
					}
				}
				finally
				{
					this.m_reportRT.CurrentScope = currentScope;
					this.m_reportRT.ObjectType = objectType;
					this.m_reportRT.ObjectName = objectName;
					this.m_reportRT.PropertyName = propertyName;
					this.m_isVisited = false;
					this.m_isValueReady = true;
				}
			}
			return this.m_result;
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance)
		{
			if (!this.m_isValueReady)
			{
				this.GetResult(romInstance);
			}
			return this.m_fieldsUsedInValueExpression;
		}

		internal void MergeFieldsUsedInValueExpression(Dictionary<string, bool> usedFields)
		{
			if (this.m_fieldsUsedInValueExpression != null)
			{
				for (int i = 0; i < this.m_fieldsUsedInValueExpression.Count; i++)
				{
					string text = this.m_fieldsUsedInValueExpression[i];
					if (text != null)
					{
						usedFields[text] = true;
					}
				}
			}
		}

		internal void Reset()
		{
			if (this.m_isValueReady && this.m_textRunDef.Value.IsExpression)
			{
				this.m_isValueReady = false;
			}
		}
	}
}
