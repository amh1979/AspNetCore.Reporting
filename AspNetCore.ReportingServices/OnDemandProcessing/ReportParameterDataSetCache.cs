using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class ReportParameterDataSetCache
	{
		private ProcessReportParameters m_paramProcessor;

		private ParameterInfo m_parameter;

		private IParameterDef m_parameterDef;

		private List<object> m_defaultValues;

		private bool m_processValidValues;

		private bool m_processDefaultValues;

		internal List<object> DefaultValues
		{
			get
			{
				return this.m_defaultValues;
			}
		}

		internal ReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, IParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
		{
			this.m_paramProcessor = aParamProcessor;
			this.m_parameter = aParameter;
			this.m_parameterDef = aParamDef;
			this.m_processDefaultValues = aProcessDefaultValues;
			this.m_processValidValues = aProcessValidValues;
			if (this.m_processDefaultValues)
			{
				this.m_defaultValues = new List<object>();
			}
			if (this.m_processValidValues)
			{
				this.m_parameter.ValidValues = new ValidValueList();
			}
		}

		internal void NextRow(object aRow)
		{
			if (this.m_processValidValues)
			{
				IParameterDataSource validValuesDataSource = this.m_parameterDef.ValidValuesDataSource;
				object obj = null;
				object value = null;
				string label = null;
				bool flag = false;
				try
				{
					flag = false;
					obj = this.GetFieldValue(aRow, validValuesDataSource.ValueFieldIndex);
					if (validValuesDataSource.LabelFieldIndex >= 0)
					{
						flag = true;
						value = this.GetFieldValue(aRow, validValuesDataSource.LabelFieldIndex);
					}
					if (!AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.ProcessObjectToString(value, true, out label))
					{
						this.m_paramProcessor.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Warning, ObjectType.ReportParameter, this.m_parameterDef.Name, "Label");
					}
					this.m_paramProcessor.ConvertAndAddValidValue(this.m_parameter, this.m_parameterDef, obj, label);
				}
				catch (ReportProcessingException_FieldError aError)
				{
					int aFieldIndex = flag ? validValuesDataSource.LabelFieldIndex : validValuesDataSource.ValueFieldIndex;
					this.m_paramProcessor.ThrowExceptionForQueryBackedParameter(aError, this.m_parameterDef.Name, validValuesDataSource.DataSourceIndex, validValuesDataSource.DataSetIndex, aFieldIndex, "ValidValue");
				}
			}
			if (this.m_processDefaultValues)
			{
				IParameterDataSource defaultDataSource = this.m_parameterDef.DefaultDataSource;
				try
				{
					if (this.m_parameterDef.MultiValue || this.m_defaultValues.Count == 0)
					{
						object fieldValue = this.GetFieldValue(aRow, defaultDataSource.ValueFieldIndex);
						fieldValue = this.m_paramProcessor.ConvertValue(fieldValue, this.m_parameterDef, true);
						this.m_defaultValues.Add(fieldValue);
					}
				}
				catch (ReportProcessingException_FieldError aError2)
				{
					this.m_paramProcessor.ThrowExceptionForQueryBackedParameter(aError2, this.m_parameterDef.Name, defaultDataSource.DataSourceIndex, defaultDataSource.DataSetIndex, defaultDataSource.ValueFieldIndex, "DefaultValue");
				}
			}
		}

		internal abstract object GetFieldValue(object aRow, int col);
	}
}
