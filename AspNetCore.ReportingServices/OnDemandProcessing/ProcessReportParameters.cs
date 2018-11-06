using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class ProcessReportParameters
	{
		protected const int UnrestrictedStringResultLength = -1;

		private IInternalProcessingContext m_processingContext;

		private ReportParameterDataSetCache m_paramDataSetCache;

		private Dictionary<string, bool> m_dependenciesSubmitted;

		private int m_lastDynamicParam;

		private ParameterInfoCollection m_parameters;

		protected int m_maxStringResultLength = -1;

		internal virtual bool IsReportParameterProcessing
		{
			get
			{
				return true;
			}
		}

		internal IInternalProcessingContext ProcessingContext
		{
			get
			{
				return this.m_processingContext;
			}
		}

		public ProcessReportParameters(IInternalProcessingContext aContext)
		{
			this.m_processingContext = aContext;
		}

		private void ProcessParameter(ParameterInfoCollection aParameters, int aParamIndex)
		{
			ParameterInfo parameterInfo = aParameters[aParamIndex];
			parameterInfo.MissingUpstreamDataSourcePrompt = false;
			IParameterDef parameterDef = null;
			bool flag = aParameters.UserProfileState != UserProfileState.None;
			if (this.m_processingContext.SnapshotProcessing && parameterInfo.UsedInQuery)
			{
				parameterInfo.State = ReportParameterState.HasValidValue;
				parameterInfo.StoreLabels();
			}
			else
			{
				if (parameterInfo.DynamicDefaultValue || parameterInfo.DynamicValidValues || parameterInfo.DynamicPrompt)
				{
					this.UpdateParametersContext(aParameters, this.m_lastDynamicParam, aParamIndex);
					this.m_lastDynamicParam = aParamIndex;
					parameterDef = this.GetParameterDef(aParamIndex);
					Global.Tracer.Assert(null != parameterDef, "null != paramDef, parameter {0}", parameterInfo.Name.MarkAsPrivate());
					Global.Tracer.Assert(parameterInfo.DataType == parameterDef.DataType, "paramInfo.DataType == paramDef.DataType, parameter {0}", parameterInfo.Name.MarkAsPrivate());
					this.AssertAreSameParameterByName(parameterInfo, parameterDef);
				}
				bool flag2 = this.m_dependenciesSubmitted.ContainsKey(parameterInfo.Name);
				if (parameterInfo.DynamicPrompt && (flag2 || !parameterInfo.IsUserSupplied || flag))
				{
					this.SetupExprHost(parameterDef);
					string text = this.EvaluatePromptExpr(parameterInfo, parameterDef);
					if (text == null || text.Equals(string.Empty))
					{
						text = parameterInfo.Name;
					}
					parameterInfo.Prompt = text;
				}
				switch (parameterInfo.CalculateDependencyStatus())
				{
				case ReportParameterDependencyState.HasOutstandingDependencies:
					parameterInfo.State = ReportParameterState.HasOutstandingDependencies;
					parameterInfo.Values = null;
					if (parameterInfo.DynamicDefaultValue)
					{
						parameterInfo.DefaultValues = null;
					}
					if (parameterInfo.DynamicValidValues)
					{
						parameterInfo.ValidValues = null;
					}
					return;
				case ReportParameterDependencyState.MissingUpstreamDataSourcePrompt:
					parameterInfo.MissingUpstreamDataSourcePrompt = true;
					parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
					return;
				default:
					Global.Tracer.Assert(false, "Unexpected dependency state.");
					break;
				case ReportParameterDependencyState.AllDependenciesSpecified:
					break;
				}
				bool flag3 = parameterInfo.DynamicDefaultValue && (parameterInfo.Values == null || (parameterInfo.Values != null && !parameterInfo.IsUserSupplied)) && ((this.m_processingContext.SnapshotProcessing && parameterDef.HasDefaultValuesExpressions() && (flag || (parameterInfo.DependencyList != null && (parameterInfo.Values == null || (!parameterInfo.IsUserSupplied && flag2))))) || (!this.m_processingContext.SnapshotProcessing && (flag2 || parameterInfo.Values == null)));
				if (parameterInfo.DynamicValidValues && ((this.m_processingContext.SnapshotProcessing && parameterDef.HasValidValuesValueExpressions() && (parameterInfo.DependencyList != null || (flag && flag3))) || (!this.m_processingContext.SnapshotProcessing && ((parameterInfo.ValidValues != null && flag2) || parameterInfo.ValidValues == null))) && !this.ProcessValidValues(parameterInfo, parameterDef, flag3))
				{
					parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
				}
				else if (!flag3 && parameterInfo.Values != null)
				{
					if (parameterInfo.ValueIsValid())
					{
						parameterInfo.State = ReportParameterState.HasValidValue;
						parameterInfo.StoreLabels();
					}
					else
					{
						parameterInfo.State = ReportParameterState.InvalidValueProvided;
						parameterInfo.Values = null;
						parameterInfo.EnsureLabelsAreGenerated();
					}
				}
				else
				{
					parameterInfo.Values = null;
					parameterInfo.State = ReportParameterState.MissingValidValue;
					if (flag3 && !this.ProcessDefaultValue(parameterInfo, parameterDef))
					{
						parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
					}
					else
					{
						if (parameterInfo.DefaultValues != null)
						{
							parameterInfo.Values = parameterInfo.DefaultValues;
							if (!parameterInfo.ValueIsValid())
							{
								parameterInfo.Values = null;
								parameterInfo.State = ReportParameterState.DefaultValueInvalid;
								parameterInfo.EnsureLabelsAreGenerated();
							}
							else
							{
								parameterInfo.State = ReportParameterState.HasValidValue;
								parameterInfo.StoreLabels();
							}
						}
						this.m_paramDataSetCache = null;
					}
				}
			}
		}

		protected virtual void AssertAreSameParameterByName(ParameterInfo paramInfo, IParameterDef paramDef)
		{
			Global.Tracer.Assert(0 == string.Compare(paramInfo.Name, paramDef.Name, StringComparison.OrdinalIgnoreCase), "paramInfo.Name == paramDef.Name, parameter {0}", paramInfo.Name.MarkAsPrivate());
		}

		public ProcessingMessageList Process(ParameterInfoCollection aParameters)
		{
			this.m_parameters = aParameters;
			try
			{
				if (this.m_parameters.IsAnyParameterDynamic)
				{
					this.InitParametersContext(this.m_parameters);
				}
				this.m_dependenciesSubmitted = ProcessReportParameters.BuildSubmittedDependencyList(this.m_parameters);
				for (int i = 0; i < aParameters.Count; i++)
				{
					this.ProcessParameter(this.m_parameters, i);
				}
				this.m_parameters.Validated = true;
				return this.m_processingContext.ErrorContext.Messages;
			}
			finally
			{
				this.Cleanup();
			}
		}

		internal static Dictionary<string, bool> BuildSubmittedDependencyList(ParameterInfoCollection parameters)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.DependencyList != null)
				{
					for (int j = 0; j < parameterInfo.DependencyList.Count; j++)
					{
						ParameterInfo parameterInfo2 = parameterInfo.DependencyList[j];
						if (parameterInfo2.IsUserSupplied && parameterInfo2.ValuesChanged)
						{
							goto IL_004d;
						}
						if (dictionary.ContainsKey(parameterInfo2.Name))
						{
							goto IL_004d;
						}
						continue;
						IL_004d:
						dictionary.Add(parameterInfo.Name, true);
						break;
					}
				}
			}
			return dictionary;
		}

		internal abstract IParameterDef GetParameterDef(int aParamIndex);

		internal abstract void InitParametersContext(ParameterInfoCollection parameters);

		internal abstract void Cleanup();

		internal abstract void AddToRuntime(ParameterInfo aParamInfo);

		internal abstract void SetupExprHost(IParameterDef aParamDef);

		internal abstract object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex);

		internal abstract object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex);

		internal abstract object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex);

		internal abstract bool NeedPrompt(IParameterDataSource paramDS);

		internal abstract void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName);

		internal abstract string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef);

		internal abstract ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues);

		protected abstract string ApplySandboxStringRestriction(string value, string paramName, string propertyName);

		internal bool ValidateValue(object newValue, IParameterDef paramDef, string parameterValueProperty)
		{
			if (paramDef.ValidateValueForNull(newValue, this.m_processingContext.ErrorContext, parameterValueProperty) && paramDef.ValidateValueForBlank(newValue, this.m_processingContext.ErrorContext, parameterValueProperty))
			{
				return true;
			}
			return false;
		}

		internal object ConvertValue(object o, IParameterDef paramDef, bool isDefaultValue)
		{
			if (o != null && DBNull.Value != o)
			{
				bool flag = false;
				object obj = null;
				try
				{
					DataType dataType = paramDef.DataType;
					if (dataType <= DataType.Integer)
					{
						switch (dataType)
						{
						case DataType.Object:
							obj = o;
							return obj;
						case DataType.Boolean:
							obj = (bool)o;
							return obj;
						case DataType.Integer:
							obj = Convert.ToInt32(o, Thread.CurrentThread.CurrentCulture);
							return obj;
						default:
							return obj;
						case (DataType)2:
							return obj;
						}
					}
					switch (dataType)
					{
					case DataType.DateTime:
						if (o is DateTimeOffset)
						{
							obj = (DateTimeOffset)o;
							return obj;
						}
						obj = (DateTime)o;
						return obj;
					case DataType.Float:
						obj = Convert.ToDouble(o, Thread.CurrentThread.CurrentCulture);
						return obj;
					case DataType.String:
						obj = Convert.ToString(o, Thread.CurrentThread.CurrentCulture);
						obj = this.ApplySandboxStringRestriction((string)obj, paramDef.Name, isDefaultValue ? "DefaultValue" : "ValidValue");
						return obj;
					default:
						return obj;
					case (DataType)17:
						return obj;
					}
				}
				catch (InvalidCastException)
				{
					flag = true;
					return obj;
				}
				catch (OverflowException)
				{
					flag = true;
					return obj;
				}
				catch (FormatException)
				{
					flag = true;
					return obj;
				}
				finally
				{
					if (flag)
					{
						string propertyName = (!isDefaultValue) ? "ValidValues" : "DefaultValue";
						this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, paramDef.ParameterObjectType, paramDef.Name, propertyName);
						throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
					}
				}
			}
			return null;
		}

		internal void UpdateParametersContext(ParameterInfoCollection parameters, int lastIndex, int currentIndex)
		{
			for (int i = lastIndex; i < currentIndex; i++)
			{
				ParameterInfo aParamInfo = parameters[i];
				this.AddToRuntime(aParamInfo);
			}
		}

		internal bool ProcessDefaultValue(ParameterInfo parameter, IParameterDef paramDef)
		{
			if (parameter != null && paramDef != null)
			{
				object obj = null;
				if (paramDef.HasDefaultValuesExpressions())
				{
					int num = paramDef.DefaultValuesExpressionCount;
					Global.Tracer.Assert(0 != num, "(0 != count)");
					if (!paramDef.MultiValue)
					{
						num = 1;
					}
					this.SetupExprHost(paramDef);
					ArrayList arrayList = new ArrayList(num);
					for (int i = 0; i < num; i++)
					{
						obj = this.EvaluateDefaultValueExpr(paramDef, i);
						if (obj is object[])
						{
							object[] array = obj as object[];
							foreach (object o in array)
							{
								object obj2 = this.ConvertValue(o, paramDef, true);
								if (!this.ValidateValue(obj2, paramDef, "DefaultValue"))
								{
									return true;
								}
								arrayList.Add(obj2);
							}
							continue;
						}
						obj = this.ConvertValue(obj, paramDef, true);
						if (!this.ValidateValue(obj, paramDef, "DefaultValue"))
						{
							return true;
						}
						arrayList.Add(obj);
					}
					Global.Tracer.Assert(null != arrayList, "(null != defaultValues)");
					if (paramDef.MultiValue)
					{
						parameter.DefaultValues = new object[arrayList.Count];
						arrayList.CopyTo(parameter.DefaultValues);
					}
					else if (arrayList.Count > 0)
					{
						parameter.DefaultValues = new object[1];
						parameter.DefaultValues[0] = arrayList[0];
					}
					else
					{
						parameter.DefaultValues = new object[0];
					}
				}
				else if (paramDef.HasDefaultValuesDataSource() && this.m_processingContext.EnableDataBackedParameters)
				{
					IParameterDataSource defaultDataSource = paramDef.DefaultDataSource;
					IParameterDataSource validValuesDataSource = paramDef.ValidValuesDataSource;
					List<object> list = null;
					if (this.m_paramDataSetCache != null && validValuesDataSource != null && defaultDataSource.DataSourceIndex == validValuesDataSource.DataSourceIndex && defaultDataSource.DataSetIndex == validValuesDataSource.DataSetIndex)
					{
						list = this.m_paramDataSetCache.DefaultValues;
					}
					else
					{
						if (this.NeedPrompt(defaultDataSource))
						{
							parameter.MissingUpstreamDataSourcePrompt = true;
							return false;
						}
						ReportParameterDataSetCache reportParameterDataSetCache = this.ProcessReportParameterDataSet(parameter, paramDef, defaultDataSource, false, true);
						list = reportParameterDataSetCache.DefaultValues;
						if (Global.Tracer.TraceVerbose && (list == null || list.Count == 0))
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' default value list does not contain any values.", parameter.Name.MarkAsPrivate());
						}
					}
					if (list != null)
					{
						int count = list.Count;
						parameter.DefaultValues = new object[count];
						int num2 = 0;
						while (num2 < count)
						{
							obj = list[num2];
							if (this.ValidateValue(obj, paramDef, "DefaultValue"))
							{
								parameter.DefaultValues[num2] = obj;
								num2++;
								continue;
							}
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' has a default value '{1}' which is not a valid value.", parameter.Name.MarkAsPrivate(), obj.ToString().MarkAsPrivate());
							}
							parameter.DefaultValues = null;
							return true;
						}
					}
				}
				return true;
			}
			return true;
		}

		internal bool ProcessValidValues(ParameterInfo parameter, IParameterDef paramDef, bool aEvaluateDefaultValues)
		{
			if (parameter != null && paramDef != null)
			{
				IParameterDataSource validValuesDataSource = paramDef.ValidValuesDataSource;
				if (paramDef.HasValidValuesDataSource())
				{
					if (this.m_processingContext.EnableDataBackedParameters)
					{
						if (this.NeedPrompt(validValuesDataSource))
						{
							parameter.MissingUpstreamDataSourcePrompt = true;
							return false;
						}
						IParameterDataSource defaultDataSource = paramDef.DefaultDataSource;
						bool aRetrievalDefaultValues = aEvaluateDefaultValues && defaultDataSource != null && defaultDataSource.DataSourceIndex == validValuesDataSource.DataSourceIndex && defaultDataSource.DataSetIndex == validValuesDataSource.DataSetIndex;
						this.m_paramDataSetCache = this.ProcessReportParameterDataSet(parameter, paramDef, validValuesDataSource, true, aRetrievalDefaultValues);
						if (Global.Tracer.TraceVerbose && parameter.ValidValues != null && parameter.ValidValues.Count == 0)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Parameter '{0}' dynamic valid value list does not contain any values.", parameter.Name.MarkAsPrivate());
						}
					}
				}
				else if (paramDef.HasValidValuesValueExpressions())
				{
					int validValuesValueExpressionCount = paramDef.ValidValuesValueExpressionCount;
					Global.Tracer.Assert(0 != validValuesValueExpressionCount, "(0 != count)");
					Global.Tracer.Assert(paramDef.HasValidValuesLabelExpressions() && validValuesValueExpressionCount == paramDef.ValidValuesLabelExpressionCount);
					this.SetupExprHost(paramDef);
					parameter.ValidValues = new ValidValueList(validValuesValueExpressionCount);
					for (int i = 0; i < validValuesValueExpressionCount; i++)
					{
						object obj = this.EvaluateValidValueExpr(paramDef, i);
						object obj2 = this.EvaluateValidValueLabelExpr(paramDef, i);
						bool flag = obj is object[];
						bool flag2 = obj2 is object[];
						if (flag && (flag2 || obj2 == null))
						{
							object[] array = obj as object[];
							object[] array2 = obj2 as object[];
							if (array2 != null && array.Length != array2.Length)
							{
								this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValueList, Severity.Error, ObjectType.ReportParameter, paramDef.Name, "ValidValues");
								throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
							}
							int num = array.Length;
							for (int j = 0; j < num; j++)
							{
								obj2 = ((array2 == null) ? null : array2[j]);
								this.ConvertAndAddValidValue(parameter, paramDef, array[j], obj2);
							}
							continue;
						}
						if (!flag && (!flag2 || obj2 == null))
						{
							this.ConvertAndAddValidValue(parameter, paramDef, obj, obj2);
							continue;
						}
						this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValueList, Severity.Error, ObjectType.ReportParameter, paramDef.Name, "ValidValues");
						throw new ReportProcessingException(this.m_processingContext.ErrorContext.Messages);
					}
				}
				return true;
			}
			return true;
		}

		internal void ConvertAndAddValidValue(ParameterInfo parameter, IParameterDef paramDef, object value, object label)
		{
			value = this.ConvertValue(value, paramDef, false);
			string value2 = label as string;
			value2 = this.ApplySandboxStringRestriction(value2, paramDef.Name, "Label");
			if (this.ValidateValue(value, paramDef, "ValidValues"))
			{
				parameter.AddValidValue(value, value2);
			}
		}

		protected static string ApplySandboxRestriction(ref string value, string paramName, string propertyName, OnDemandProcessingContext odpContext, int maxStringResultLength)
		{
			if (maxStringResultLength != -1 && value != null && value.Length > maxStringResultLength)
			{
				value = null;
				odpContext.ErrorContext.Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, ObjectType.ReportParameter, paramName, propertyName);
			}
			return value;
		}
	}
}
