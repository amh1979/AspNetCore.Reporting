using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class VariableImpl : Variable
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Variable m_variableDef;

		private IndexedExprHost m_exprHost;

		private ObjectType m_parentObjectType;

		private string m_parentObjectName;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IScope m_scope;

		private object m_value;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private int m_indexInCollection;

		public override object Value
		{
			get
			{
				if (!this.m_isValueReady)
				{
					if (!this.m_reportRT.ReportObjectModel.OdpContext.IsTablixProcessingMode && !this.m_reportRT.VariableReferenceMode)
					{
						return null;
					}
					return this.GetResult(true);
				}
				if (!this.VariableInScope)
				{
					return null;
				}
				return this.m_value;
			}
			set
			{
				this.SetValue(value, false);
			}
		}

		public override bool Writable
		{
			get
			{
				return this.m_variableDef.Writable;
			}
		}

		internal IScope Scope
		{
			set
			{
				this.m_scope = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_variableDef.Name;
			}
			set
			{
				this.m_variableDef.Name = value;
			}
		}

		private bool VariableInScope
		{
			get
			{
				if (!this.IsReportVariable)
				{
					IRIFReportScope iRIFReportScope = null;
					if (this.m_reportRT.ReportObjectModel.OdpContext.IsTablixProcessingMode || this.m_reportRT.VariableReferenceMode)
					{
						if (this.m_reportRT.CurrentScope != null)
						{
							iRIFReportScope = this.m_reportRT.CurrentScope.RIFReportScope;
						}
					}
					else
					{
						IReportScope currentReportScope = this.m_reportRT.ReportObjectModel.OdpContext.CurrentReportScope;
						if (currentReportScope != null)
						{
							iRIFReportScope = currentReportScope.RIFReportScope;
						}
					}
					if (iRIFReportScope != null && iRIFReportScope.VariableInScope(this.m_variableDef.SequenceID))
					{
						goto IL_0086;
					}
					return false;
				}
				goto IL_0086;
				IL_0086:
				return true;
			}
		}

		private bool IsReportVariable
		{
			get
			{
				return this.m_parentObjectType == ObjectType.Report;
			}
		}

		internal VariableImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.Variable variable, IndexedExprHost variableValuesHost, ObjectType parentObjectType, string parentObjectName, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, int indexInCollection)
		{
			Global.Tracer.Assert(reportRT != null && null != variable, "(null != reportRT && null != variable)");
			this.m_variableDef = variable;
			this.m_exprHost = variableValuesHost;
			this.m_parentObjectType = parentObjectType;
			this.m_parentObjectName = parentObjectName;
			this.m_reportRT = reportRT;
			this.m_indexInCollection = indexInCollection;
		}

		internal void SetResult(AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			this.m_result = result;
			this.m_isValueReady = true;
		}

		public override bool SetValue(object value)
		{
			bool result = default(bool);
			this.SetValue(value, false, out result);
			return result;
		}

		internal void SetValue(object value, bool internalSet)
		{
			bool flag = default(bool);
			this.SetValue(value, internalSet, out flag);
		}

		private void SetValue(object value, bool internalSet, out bool succeeded)
		{
			succeeded = false;
			if (!internalSet)
			{
				if (this.IsReportVariable && this.m_variableDef.Writable)
				{
					this.m_result = new AspNetCore.ReportingServices.RdlExpressions.VariantResult(false, value);
					bool flag = this.m_reportRT.ProcessSerializableResult(true, ref this.m_result);
					if (this.m_result.ErrorOccurred)
					{
						if (flag)
						{
							((IErrorContext)this.m_reportRT).Register(ProcessingErrorCode.rsVariableTypeNotSerializable, Severity.Error, this.m_parentObjectType, this.m_parentObjectName, this.m_variableDef.GetPropertyName(), new string[0]);
						}
					}
					else
					{
						this.m_reportRT.ReportObjectModel.OdpContext.StoreUpdatedVariableValue(this.m_indexInCollection, value);
						succeeded = true;
						this.m_value = value;
						this.m_isValueReady = true;
					}
				}
			}
			else
			{
				succeeded = true;
				this.m_value = value;
				this.m_isValueReady = true;
			}
		}

		internal void Reset()
		{
			this.m_isValueReady = false;
		}

		internal object GetResult()
		{
			return this.GetResult(false);
		}

		private object GetResult(bool fromValue)
		{
			if (fromValue && !this.VariableInScope)
			{
				return null;
			}
			if (!this.m_isValueReady)
			{
				if (this.m_isVisited)
				{
					ProcessingErrorCode code = (ProcessingErrorCode)(this.IsReportVariable ? 346 : 347);
					((IErrorContext)this.m_reportRT).Register(code, Severity.Error, this.m_parentObjectType, this.m_parentObjectName, this.m_variableDef.GetPropertyName(), new string[0]);
					throw new ReportProcessingException(this.m_reportRT.RuntimeErrorContext.Messages);
				}
				this.m_isVisited = true;
				bool variableReferenceMode = this.m_reportRT.VariableReferenceMode;
				ObjectType objectType = this.m_reportRT.ObjectType;
				string objectName = this.m_reportRT.ObjectName;
				string propertyName = this.m_reportRT.PropertyName;
				bool unfulfilledDependency = this.m_reportRT.UnfulfilledDependency;
				IScope currentScope = this.m_reportRT.CurrentScope;
				this.m_reportRT.VariableReferenceMode = true;
				this.m_reportRT.UnfulfilledDependency = false;
				this.m_result = this.m_reportRT.EvaluateVariableValueExpression(this.m_variableDef, this.m_exprHost, this.m_parentObjectType, this.m_parentObjectName, this.IsReportVariable);
				bool unfulfilledDependency2 = this.m_reportRT.UnfulfilledDependency;
				this.m_reportRT.UnfulfilledDependency |= unfulfilledDependency;
				this.m_reportRT.VariableReferenceMode = variableReferenceMode;
				this.m_reportRT.CurrentScope = currentScope;
				this.m_reportRT.ObjectType = objectType;
				this.m_reportRT.ObjectName = objectName;
				this.m_reportRT.PropertyName = propertyName;
				if (this.m_result.ErrorOccurred)
				{
					throw new ReportProcessingException(this.m_reportRT.RuntimeErrorContext.Messages);
				}
				if (unfulfilledDependency2 && fromValue)
				{
					this.m_value = null;
					this.m_isValueReady = false;
				}
				else
				{
					this.m_value = this.m_result.Value;
					this.m_isValueReady = true;
				}
				this.m_isVisited = false;
			}
			return this.m_value;
		}
	}
}
