using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal sealed class ReportRuntime : IErrorContext, IStaticReferenceable
	{
		private delegate object EvalulateDataPoint(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint);

		internal sealed class TextRunExprHostWrapper : TextRunExprHost
		{
			private TextBoxExprHost m_textBoxExprHost;

			public override object ValueExpr
			{
				get
				{
					return this.m_textBoxExprHost.ValueExpr;
				}
			}

			internal TextRunExprHostWrapper(TextBoxExprHost textBoxExprHost)
			{
				this.m_textBoxExprHost = textBoxExprHost;
			}
		}

		private enum NormalizationCode
		{
			Success,
			InvalidType,
			StringLengthViolation,
			ArrayLengthViolation
		}

		private sealed class ExpressionHostLoader : MarshalByRefObject
		{
			private const string ExprHostRootType = "ReportExprHostImpl";

			private static readonly Hashtable ExpressionHosts = new Hashtable();

			private static readonly byte[] ReportExpressionsDefaultEvidencePK = new byte[160]
			{
				0,
				36,
				0,
				0,
				4,
				128,
				0,
				0,
				148,
				0,
				0,
				0,
				6,
				2,
				0,
				0,
				0,
				36,
				0,
				0,
				82,
				83,
				65,
				49,
				0,
				4,
				0,
				0,
				1,
				0,
				1,
				0,
				81,
				44,
				142,
				135,
				46,
				40,
				86,
				158,
				115,
				59,
				203,
				18,
				55,
				148,
				218,
				181,
				81,
				17,
				160,
				87,
				11,
				59,
				61,
				77,
				227,
				121,
				65,
				83,
				222,
				165,
				239,
				183,
				195,
				254,
				169,
				242,
				216,
				35,
				108,
				255,
				50,
				12,
				79,
				208,
				234,
				213,
				246,
				119,
				136,
				11,
				246,
				193,
				129,
				242,
				150,
				199,
				81,
				197,
				246,
				230,
				91,
				4,
				211,
				131,
				76,
				2,
				247,
				146,
				254,
				224,
				254,
				69,
				41,
				21,
				212,
				74,
				254,
				116,
				160,
				194,
				126,
				13,
				142,
				75,
				141,
				4,
				236,
				82,
				168,
				226,
				129,
				224,
				31,
				244,
				126,
				125,
				105,
				78,
				108,
				114,
				117,
				160,
				154,
				252,
				191,
				216,
				204,
				130,
				112,
				90,
				6,
				178,
				15,
				214,
				239,
				97,
				235,
				186,
				104,
				115,
				226,
				156,
				140,
				15,
				44,
				174,
				221,
				162
			};

			internal static ReportExprHost LoadExprHost(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules, AppDomain targetAppDomain)
			{
				Type expressionHostLoaderType = typeof(ExpressionHostLoader);
                ExpressionHostLoader remoteEHL = null;// Activator.CreateInstance<ExpressionHostLoader>();
                
                //todo: can delete?
                RevertImpersonationContext.RunFromRestrictedCasContext(delegate
				{
					remoteEHL = Activator.CreateInstance<ExpressionHostLoader>();
				});
				return remoteEHL.LoadExprHostRemoteEntryPoint(exprHostBytes, exprHostAssemblyName, includeParameters, parametersOnly, objectModel, codeModules);
			}

			internal static ReportExprHost LoadExprHostIntoCurrentAppDomain(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules)
			{
				if (codeModules != null && 0 < codeModules.Count)
				{
                    //for (int num = codeModules.Count - 1; num >= 0; num--)
                    //{
                    //    Assembly.Load(codeModules[num]);
                    //}
                    //todo: can delete?
                    RevertImpersonationContext.RunFromRestrictedCasContext(delegate
					{
						for (int num = codeModules.Count - 1; num >= 0; num--)
						{
							Assembly.Load(codeModules[num]);
						}
					});
				}
				Assembly assembly = ExpressionHostLoader.LoadExprHostAssembly(exprHostBytes, exprHostAssemblyName, evidence);
				Type type = assembly.GetType("ExpressionHost.ReportExprHostImpl");
				return (ReportExprHost)type.GetConstructors()[0].Invoke(new object[3]
				{
					includeParameters,
					parametersOnly,
					objectModel
				});
			}

			private static Assembly LoadExprHostAssembly(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence)
			{
				lock (ExpressionHostLoader.ExpressionHosts.SyncRoot)
				{
					Assembly assembly = (Assembly)ExpressionHostLoader.ExpressionHosts[exprHostAssemblyName];
					if (assembly == null)
					{
						if (evidence == null)
						{
							evidence = ExpressionHostLoader.CreateDefaultExpressionHostEvidence(exprHostAssemblyName);
						}
						try
						{
							new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
							assembly = Assembly.Load(exprHostBytes);
						}
						finally
						{
							CodeAccessPermission.RevertAssert();
						}
						ExpressionHostLoader.ExpressionHosts.Add(exprHostAssemblyName, assembly);
					}
					return assembly;
				}
			}

			private static Evidence CreateDefaultExpressionHostEvidence(string exprHostAssemblyName)
			{
				Evidence evidence = new Evidence();
				evidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));
				evidence.AddHostEvidence(new StrongName(new StrongNamePublicKeyBlob(ExpressionHostLoader.ReportExpressionsDefaultEvidencePK), exprHostAssemblyName, new Version("1.0.0.0")));
				return evidence;
			}

			private ReportExprHost LoadExprHostRemoteEntryPoint(byte[] exprHostBytes, string exprHostAssemblyName, bool includeParameters, bool parametersOnly, OnDemandObjectModel objectModel, List<string> codeModules)
			{
				return ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(exprHostBytes, exprHostAssemblyName, null, includeParameters, parametersOnly, objectModel, codeModules);
			}
		}

		private const int UnrestrictedStringResultLength = -1;

		private const int UnrestrictedArrayResultLength = -1;

		private bool m_exprHostInSandboxAppDomain;

		private ReportExprHost m_reportExprHost;

		private AspNetCore.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl m_reportObjectModel;

		private ErrorContext m_errorContext;

		private IErrorContext m_delayedErrorContext;

		private bool m_contextUpdated;

		private IScope m_currentScope;

		private bool m_variableReferenceMode;

		private bool m_unfulfilledDependency;

		private ReportRuntime m_topLevelReportRuntime;

		private List<string> m_fieldsUsedInCurrentActionOwnerValue;

		private int m_id = -2147483648;

		private int m_maxStringResultLength = -1;

		private int m_maxArrayResultLength = -1;

		private bool m_rdlSandboxingEnabled;

		private bool m_isSerializableValuesProhibited;

		internal ReportExprHost ReportExprHost
		{
			get
			{
				return this.m_reportExprHost;
			}
		}

		internal bool VariableReferenceMode
		{
			get
			{
				return this.m_variableReferenceMode;
			}
			set
			{
				this.m_variableReferenceMode = value;
			}
		}

		internal bool UnfulfilledDependency
		{
			get
			{
				return this.m_unfulfilledDependency;
			}
			set
			{
				this.m_unfulfilledDependency = value;
			}
		}

		internal bool ContextUpdated
		{
			get
			{
				return this.m_contextUpdated;
			}
			set
			{
				this.m_contextUpdated = value;
			}
		}

		internal IScope CurrentScope
		{
			get
			{
				return this.m_currentScope;
			}
			set
			{
				this.m_currentScope = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
			set
			{
				this.m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return this.m_objectName;
			}
			set
			{
				this.m_objectName = value;
			}
		}

		internal string PropertyName
		{
			get
			{
				return this.m_propertyName;
			}
			set
			{
				this.m_propertyName = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl ReportObjectModel
		{
			get
			{
				return this.m_reportObjectModel;
			}
		}

		internal ErrorContext RuntimeErrorContext
		{
			get
			{
				return this.m_errorContext;
			}
		}

		internal List<string> FieldsUsedInCurrentActionOwnerValue
		{
			get
			{
				return this.m_fieldsUsedInCurrentActionOwnerValue;
			}
			set
			{
				this.m_fieldsUsedInCurrentActionOwnerValue = value;
			}
		}

		public int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ReportRuntime(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext)
		{
			this.m_objectType = objectType;
			this.m_reportObjectModel = reportObjectModel;
			this.m_errorContext = errorContext;
			if (reportObjectModel.OdpContext.IsRdlSandboxingEnabled())
			{
				this.m_rdlSandboxingEnabled = true;
				IRdlSandboxConfig rdlSandboxing = reportObjectModel.OdpContext.Configuration.RdlSandboxing;
				this.m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
				this.m_maxArrayResultLength = rdlSandboxing.MaxArrayResultLength;
			}
			if (reportObjectModel.OdpContext.Configuration != null)
			{
				this.m_isSerializableValuesProhibited = reportObjectModel.OdpContext.Configuration.ProhibitSerializableValues;
			}
		}

		internal ReportRuntime(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ErrorContext errorContext, ReportExprHost copyReportExprHost, ReportRuntime topLevelReportRuntime)
			: this(objectType, reportObjectModel, errorContext)
		{
			this.m_reportExprHost = copyReportExprHost;
			this.m_topLevelReportRuntime = topLevelReportRuntime;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (this.m_delayedErrorContext == null)
			{
				this.m_errorContext.Register(code, severity, this.m_objectType, this.m_objectName, this.m_propertyName, arguments);
			}
			else
			{
				this.m_delayedErrorContext.Register(code, severity, arguments);
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (this.m_delayedErrorContext == null)
			{
				this.m_errorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
			else
			{
				this.m_delayedErrorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
			}
		}

		internal static string GetErrorName(DataFieldStatus status, string exceptionMessage)
		{
			if (exceptionMessage != null)
			{
				return exceptionMessage;
			}
			switch (status)
			{
			case DataFieldStatus.Overflow:
				return "OverflowException.";
			case DataFieldStatus.UnSupportedDataType:
				return "UnsupportedDatatypeException.";
			case DataFieldStatus.IsError:
				return "FieldValueException.";
			default:
				return null;
			}
		}

		internal string EvaluateReportLanguageExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, out CultureInfo language)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(report.Language, report.ObjectType, report.Name, "Language", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(report.Language, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.ReportLanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out language);
		}

		internal int EvaluateReportAutoRefreshExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(report.AutoRefreshExpression, report.ObjectType, report.Name, "AutoRefresh", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(report.AutoRefreshExpression, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.AutoRefreshExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateInitialPageNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(report.InitialPageName, report.ObjectType, report.Name, "InitialPageName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(report.InitialPageName, ref result, report.ReportExprHost))
					{
						Global.Tracer.Assert(report.ReportExprHost != null, "(report.ReportExprHost != null)");
						result.Value = report.ReportExprHost.InitialPageNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result, true).Value;
		}

		internal string EvaluateParamPrompt(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramDef.PromptExpression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Prompt", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramDef.PromptExpression, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.PromptExpression != null, "(paramDef.PromptExpression != null)");
						result.Value = paramDef.ExprHost.PromptExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessAutocastStringResult(result).Value;
		}

		internal VariantResult EvaluateParamDefaultValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.DefaultExpressions != null, "(paramDef.DefaultExpressions != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.DefaultExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "DefaultValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && expressionInfo.ExprHostID >= 0, "(paramDef.ExprHost != null && defaultExpression.ExprHostID >= 0)");
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = ((IndexedExprHost)paramDef.ExprHost)[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e, this, true);
				}
			}
			this.ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesValueExpressions != null, "(paramDef.ValidValuesValueExpressions != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesValueExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "ValidValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValuesHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValuesHost != null && expressionInfo.ExprHostID >= 0);
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValuesHost[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e, this, true);
				}
			}
			this.ProcessReportParameterVariantResult(expressionInfo, ref result);
			return result;
		}

		internal VariantResult EvaluateParamValidValueLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesLabelExpressions != null, "(paramDef.ValidValuesLabelExpressions != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = paramDef.ValidValuesLabelExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramDef.Name, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result, paramDef.ExprHost.ValidValueLabelsHost))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValueLabelsHost != null && expressionInfo.ExprHostID >= 0);
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValueLabelsHost[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e, this, true);
				}
			}
			this.ProcessReportParameterVariantResult(expressionInfo, ref result);
			if (!result.ErrorOccurred && (result.Value is object[] || result.Value is IList))
			{
				object[] asObjectArray = ReportRuntime.GetAsObjectArray(ref result);
				try
				{
					VariantResult variantResult = default(VariantResult);
					for (int i = 0; i < asObjectArray.Length; i++)
					{
						variantResult.Value = asObjectArray[i];
						this.ProcessLabelResult(ref variantResult);
						if (variantResult.ErrorOccurred)
						{
							result.ErrorOccurred = true;
							return result;
						}
						asObjectArray[i] = variantResult.Value;
					}
					result.Value = asObjectArray;
					return result;
				}
				catch (Exception)
				{
					this.RegisterInvalidExpressionDataTypeWarning();
					result.ErrorOccurred = true;
					return result;
				}
			}
			if (!result.ErrorOccurred)
			{
				this.ProcessLabelResult(ref result);
			}
			return result;
		}

		internal object EvaluateDataValueValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue value, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out TypeCode typeCode)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value.Value, objectType, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value.Value, ref variantResult, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
						variantResult.Value = value.ExprHost.DataValueValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(value.Value, ref variantResult);
			typeCode = variantResult.TypeCode;
			return variantResult.Value;
		}

		internal string EvaluateDataValueNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue value, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value.Name, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value.Name, ref result, value.ExprHost))
					{
						Global.Tracer.Assert(value.ExprHost != null, "(value.ExprHost != null)");
						result.Value = value.ExprHost.DataValueNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateFilterVariantExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(filter.Expression, ref result, false);
			return result;
		}

		internal StringResult EvaluateFilterStringExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(filter.Expression, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null, "(filter.ExprHost != null)");
						result.Value = filter.ExprHost.FilterExpressionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result);
		}

		internal VariantResult EvaluateFilterVariantValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int index, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = ((IndexedExprHost)filter.ExprHost)[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(value, ref result, true);
			return result;
		}

		internal FloatResult EvaluateFilterIntegerOrFloatValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int index, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerOrFloatExpression(value, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = ((IndexedExprHost)filter.ExprHost)[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result);
		}

		internal IntegerResult EvaluateFilterIntegerValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int index, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(value, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = ((IndexedExprHost)filter.ExprHost)[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result);
		}

		internal StringResult EvaluateFilterStringValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int index, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null, "(filter.Values != null)");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = filter.Values[index].Value;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value, ref result, filter.ExprHost))
					{
						Global.Tracer.Assert(filter.ExprHost != null && value.ExprHostID >= 0, "(filter.ExprHost != null && valueExpression.ExprHostID >= 0)");
						result.Value = ((IndexedExprHost)filter.ExprHost)[value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result);
		}

		internal object EvaluateQueryParamValue(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo paramValue, IndexedExprHost queryParamsExprHost, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramValue, objectType, objectName, "Value", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramValue, ref variantResult, queryParamsExprHost))
					{
						AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.UserImpl userImpl = this.m_reportObjectModel.UserImpl;
						using (userImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(queryParamsExprHost != null && paramValue.ExprHostID >= 0, "(queryParamsExprHost != null && paramValue.ExprHostID >= 0)");
							variantResult.Value = queryParamsExprHost[paramValue.ExprHostID];
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpressionAndStop(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(paramValue, ref variantResult, true);
			return variantResult.Value;
		}

		internal StringResult EvaluateConnectString(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataSource.ConnectStringExpression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSource, dataSource.Name, "ConnectString", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataSource.ConnectStringExpression, ref result, dataSource.ExprHost))
					{
						AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.UserImpl userImpl = this.m_reportObjectModel.UserImpl;
						using (userImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSource.ExprHost != null, "(dataSource.ExprHost != null)");
							result.Value = dataSource.ExprHost.ConnectStringExpr;
						}
					}
				}
				catch (Exception ex)
				{
					if (ex is ReportProcessingException_NonExistingParameterReference)
					{
						this.RegisterRuntimeErrorInExpression(ref result, ex);
					}
					else
					{
						result = new VariantResult(true, null);
					}
				}
			}
			return this.ProcessStringResult(result);
		}

		internal StringResult EvaluateCommandText(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataSet.Query.CommandText, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Query, dataSet.Name, "CommandText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataSet.Query.CommandText, ref result, dataSet.ExprHost))
					{
						AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.UserImpl userImpl = this.m_reportObjectModel.UserImpl;
						using (userImpl.UpdateUserProfileLocation(UserProfileState.InQuery))
						{
							Global.Tracer.Assert(dataSet.ExprHost != null, "(dataSet.ExprHost != null)");
							result.Value = dataSet.ExprHost.QueryCommandTextExpr;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result);
		}

		internal VariantResult EvaluateFieldValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Field field)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(field.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field, field.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(field.Value, ref result, field.ExprHost))
					{
						Global.Tracer.Assert(field.ExprHost != null, "(field.ExprHost != null)");
						field.EnsureExprHostReportObjectModelBinding(this.m_reportObjectModel);
						result.Value = field.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(field.Value, ref result);
			return result;
		}

		internal VariantResult EvaluateAggregateVariantOrBinaryParamExpr(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateInfo, int index, IErrorContext errorContext)
		{
			IErrorContext delayedErrorContext = this.m_delayedErrorContext;
			try
			{
				this.m_delayedErrorContext = errorContext;
				Global.Tracer.Assert(aggregateInfo.Expressions != null, "(aggregateInfo.Expressions != null)");
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = aggregateInfo.Expressions[index];
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expressionInfo, out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expressionInfo, ref result, null))
						{
							Global.Tracer.Assert(aggregateInfo.ExpressionHosts != null && expressionInfo.ExprHostID >= 0, "(aggregateInfo.ExpressionHosts != null && aggParamExpression.ExprHostID >= 0)");
							if (this.m_exprHostInSandboxAppDomain)
							{
								aggregateInfo.ExpressionHosts[index].SetReportObjectModel(this.m_reportObjectModel);
							}
							result.Value = aggregateInfo.ExpressionHosts[index].ValueExpr;
						}
					}
					catch (ReportProcessingException_MissingAggregateDependency)
					{
						throw;
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e, errorContext, false);
					}
				}
				this.ProcessVariantOrBinaryResult(expressionInfo, ref result, true, false);
				return result;
			}
			finally
			{
				this.m_delayedErrorContext = delayedErrorContext;
			}
		}

		internal VariantResult EvaluateLookupDestExpression(LookupDestinationInfo lookupDestInfo, IErrorContext errorContext)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo destinationExpr = lookupDestInfo.DestinationExpr;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(destinationExpr, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(destinationExpr, ref result, lookupDestInfo.ExprHost))
					{
						Global.Tracer.Assert(lookupDestInfo.ExprHost != null, "(lookupDestInfo.ExprHost != null)");
						result.Value = lookupDestInfo.ExprHost.DestExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e, errorContext, false);
				}
			}
			this.ProcessLookupVariantResult(destinationExpr, errorContext, false, false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupSourceExpression(LookupInfo lookupInfo)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo sourceExpr = lookupInfo.SourceExpr;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(sourceExpr, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(sourceExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(this.m_reportObjectModel);
						result.Value = lookupInfo.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessLookupVariantResult(sourceExpr, this, lookupInfo.LookupType == LookupType.MultiLookup, false, ref result);
			return result;
		}

		internal VariantResult EvaluateLookupResultExpression(LookupInfo lookupInfo)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo resultExpr = lookupInfo.ResultExpr;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(resultExpr, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(resultExpr, ref result, lookupInfo.ExprHost))
					{
						LookupExprHost exprHost = lookupInfo.ExprHost;
						Global.Tracer.Assert(exprHost != null, "(lookupExprHost != null)");
						exprHost.SetReportObjectModel(this.m_reportObjectModel);
						result.Value = exprHost.ResultExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessLookupVariantResult(resultExpr, this, false, true, ref result);
			return result;
		}

		internal bool EvaluateParamValueOmitExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(paramVal.Omit, true, objectType, objectName, "ParameterOmit", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramVal.Omit, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
						result.Value = paramVal.ExprHost.OmitExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal ParameterValueResult EvaluateParameterValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue paramVal, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramVal.Value, ref result, paramVal.ExprHost))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null, "(paramVal.ExprHost != null)");
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessParameterValueResult(paramVal.Value, paramVal.Name, result);
		}

		internal bool EvaluateStartHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility, IVisibilityHiddenExprHost hiddenExprHostRI, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateBooleanExpression(visibility.Hidden, true, objectType, objectName, "Hidden", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(visibility.Hidden, ref result, (ReportObjectModelProxy)hiddenExprHostRI))
						{
							result.Value = hiddenExprHostRI.VisibilityHiddenExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpressionAndStop(ref result, e);
					}
				}
				return this.ProcessBooleanResult(result, true, objectType, objectName).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluateStartHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility, IndexedExprHost hiddenExprHostIdx, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateBooleanExpression(visibility.Hidden, true, objectType, objectName, "Hidden", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(visibility.Hidden, ref result, hiddenExprHostIdx))
						{
							Global.Tracer.Assert(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0, "(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0)");
							result.Value = hiddenExprHostIdx[visibility.Hidden.ExprHostID];
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpressionAndStop(ref result, e);
					}
				}
				return this.ProcessBooleanResult(result, true, objectType, objectName).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemDocumentMapLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			UserProfileState newLocation = this.m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(reportItem.DocumentMapLabel, reportItem.ObjectType, reportItem.Name, "Label", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(reportItem.DocumentMapLabel, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
							result.Value = reportItem.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				this.m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemBookmarkExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(reportItem.Bookmark, reportItem.ObjectType, reportItem.Name, "Bookmark", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(reportItem.Bookmark, ref result, reportItem.ExprHost))
						{
							Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
							result.Value = reportItem.ExprHost.BookmarkExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessStringResult(result).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateReportItemToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(reportItem.ToolTip, reportItem.ObjectType, reportItem.Name, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(reportItem.ToolTip, ref result, reportItem.ExprHost))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null, "(reportItem.ExprHost != null)");
						result.Value = reportItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateActionLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemHyperlinkURLExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Hyperlink", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.HyperlinkExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemDrillthroughReportName(AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "DrillthroughReportName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.DrillThroughReportNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateReportItemBookmarkLinkExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BookmarkLink", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, actionItem.ExprHost))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null, "(actionItem.ExprHost != null)");
						result.Value = actionItem.ExprHost.BookmarkLinkExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateImageStringValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			errorOccurred = false;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(image.Value, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = this.ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal VariantResult EvaluateImageTagExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Image image, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo tag)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tag, image.ObjectType, image.Name, "Tag", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tag, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						Global.Tracer.Assert(image.Tags.Count == 1, "Only a single Tag expression host is allowed from old snapshots");
						result.Value = image.ImageExprHost.TagExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(tag, ref result);
			return result;
		}

		internal byte[] EvaluateImageBinaryValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Image image, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBinaryExpression(image.Value, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.Value, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = this.ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateImageMIMETypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Image image)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(image.MIMEType, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.MIMEType, ref result, image.ImageExprHost))
					{
						Global.Tracer.Assert(image.ImageExprHost != null, "(image.ImageExprHost != null)");
						result.Value = image.ImageExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateTextBoxInitialToggleStateExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textBox)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(textBox.InitialToggleState, true, textBox.ObjectType, textBox.Name, "InitialState", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(textBox.InitialToggleState, ref result, textBox.ExprHost))
					{
						Global.Tracer.Assert(textBox.ExprHost != null, "(textBox.ExprHost != null)");
						result.Value = textBox.TextBoxExprHost.ToggleImageInitialStateExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal object EvaluateUserSortExpression(IInScopeEventSource eventSource)
		{
			int sortExpressionIndex = eventSource.UserSort.SortExpressionIndex;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
			Global.Tracer.Assert(sortTarget.UserSortExpressions != null && 0 <= sortExpressionIndex && sortExpressionIndex < sortTarget.UserSortExpressions.Count);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = sortTarget.UserSortExpressions[sortExpressionIndex];
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, eventSource.ObjectType, eventSource.Name, "SortExpression", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref variantResult, sortTarget.UserSortExpressionsHost))
					{
						Global.Tracer.Assert(sortTarget.UserSortExpressionsHost != null, "(sortTarget.UserSortExpressionsHost != null)");
						variantResult.Value = sortTarget.UserSortExpressionsHost[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpressionAndStop(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(expressionInfo, ref variantResult);
			if (variantResult.Value == null)
			{
				variantResult.Value = DBNull.Value;
			}
			return variantResult.Value;
		}

		internal string EvaluateGroupingLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			UserProfileState newLocation = this.m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(grouping.GroupLabel, objectType, objectName, "Label", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(grouping.GroupLabel, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.LabelExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessAutocastStringResult(result).Value;
			}
			finally
			{
				this.m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(newLocation);
			}
		}

		internal string EvaluateGroupingPageNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, objectName, "PageName", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, grouping.ExprHost))
						{
							Global.Tracer.Assert(grouping.ExprHost != null, "(grouping.ExprHost != null)");
							result.Value = grouping.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessStringResult(result, true).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal object EvaluateRuntimeExpression(RuntimeExpressionInfo runtimeExpression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(runtimeExpression.Expression, objectType, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(runtimeExpression.Expression, ref variantResult, runtimeExpression.ExpressionsHost))
					{
						Global.Tracer.Assert(runtimeExpression.ExpressionsHost != null && runtimeExpression.Expression.ExprHostID >= 0);
						variantResult.Value = runtimeExpression.ExpressionsHost[runtimeExpression.Expression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpressionAndStop(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(runtimeExpression.Expression, ref variantResult);
			this.VerifyVariantResultAndStopOnError(ref variantResult);
			return variantResult.Value;
		}

		internal VariantResult EvaluateVariableValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Variable variable, IndexedExprHost variableValuesHost, AspNetCore.ReportingServices.ReportProcessing.ObjectType parentObjectType, string parentObjectName, bool isReportScope)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(variable.Value, parentObjectType, parentObjectName, variable.GetPropertyName(), out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(variable.Value, ref result, variableValuesHost))
					{
						Global.Tracer.Assert(variableValuesHost != null && variable.Value.ExprHostID >= 0);
						result.Value = variableValuesHost[variable.Value.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpressionAndStop(ref result, e);
				}
			}
			this.ProcessSerializableResult(variable.Value, isReportScope, ref result);
			return result;
		}

		internal string EvaluateSubReportNoRowsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(subReport.NoRowsMessage, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(subReport.NoRowsMessage, ref result, subReport.SubReportExprHost))
					{
						Global.Tracer.Assert(subReport.SubReportExprHost != null, "(subReport.SubReportExprHost != null)");
						result.Value = subReport.SubReportExprHost.NoRowsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionNoRowsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion region, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(region.NoRowsMessage, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(region.NoRowsMessage, ref result, region.ExprHost))
					{
						Global.Tracer.Assert(region.ExprHost != null, "(region.ExprHost != null)");
						result.Value = region.EvaluateNoRowsMessageExpression();
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateDataRegionPageNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PageName", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, dataRegion.ExprHost))
						{
							Global.Tracer.Assert(dataRegion.ExprHost != null, "(dataRegion.ExprHost != null)");
							result.Value = dataRegion.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessStringResult(result, true).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateTablixMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition marginPosition)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression = null;
			switch (marginPosition)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
				expression = tablix.TopMargin;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
				expression = tablix.BottomMargin;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
				expression = tablix.LeftMargin;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
				expression = tablix.RightMargin;
				break;
			}
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, tablix.ObjectType, tablix.Name, marginPosition.ToString(), out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, tablix.ExprHost))
					{
						Global.Tracer.Assert(tablix.TablixExprHost != null, "(tablix.TablixExprHost != null)");
						switch (marginPosition)
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin:
							result.Value = tablix.TablixExprHost.TopMarginExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin:
							result.Value = tablix.TablixExprHost.BottomMarginExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin:
							result.Value = tablix.TablixExprHost.LeftMarginExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin:
							result.Value = tablix.TablixExprHost.RightMarginExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartDynamicSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expr, string propertyName, bool isDynamicWidth)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expr, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expr, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						if (isDynamicWidth)
						{
							result.Value = chart.ChartExprHost.DynamicWidthExpr;
						}
						else
						{
							result.Value = chart.ChartExprHost.DynamicHeightExpr;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateChartDynamicMemberLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember chartMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DocumentMapLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, chartMember.ExprHost))
					{
						Global.Tracer.Assert(chartMember.ExprHost != null, "(chartMember.ExprHost != null)");
						result.Value = chartMember.ExprHost.MemberLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(expression, ref result);
			return result;
		}

		internal string EvaluateChartPaletteExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chart.Palette, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Palette", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chart.Palette, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "(chart.ExprHost != null)");
						result.Value = chart.ChartExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartPaletteHatchBehaviorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chart.PaletteHatchBehavior, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PaletteHatchBehavior", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chart.PaletteHatchBehavior, ref result, chart.ExprHost))
					{
						Global.Tracer.Assert(chart.ExprHost != null, "chart.ExprHost != null");
						result.Value = chart.ChartExprHost.PaletteHatchBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartTitleCaptionExpression(ChartTitleBase title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Caption, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = title.ExprHost.CaptionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(title.Caption, ref result);
			return result;
		}

		internal bool EvaluateEvaluateChartTitleHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Hidden, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartTitleDockingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Docking, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitlePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartTitleDockOutsideChartAreaExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.DockOutsideChartArea, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal int EvaluateChartTitleDockOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.DockOffset, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).DockingOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartTitleToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.ToolTip, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)title.ExprHost).ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTitleTextOrientationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTitle.TextOrientation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTitle.TextOrientation, ref result, chartTitle.ExprHost))
					{
						Global.Tracer.Assert(chartTitle.ExprHost != null, "(chartTitle.ExprHost != null)");
						result.Value = ((ChartTitleExprHost)chartTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitlePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Position, ref result, title.ExprHost))
					{
						Global.Tracer.Assert(title.ExprHost != null, "(title.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)title.ExprHost).ChartTitlePositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisTitleTextOrientationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisTitle.TextOrientation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisTitle.TextOrientation, ref result, chartAxisTitle.ExprHost))
					{
						Global.Tracer.Assert(chartAxisTitle.ExprHost != null, "(chartAxisTitle.ExprHost != null)");
						result.Value = ((ChartAxisTitleExprHost)chartAxisTitle.ExprHost).TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendTitleTitleSeparatorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendTitle.TitleSeparator, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartTitleSeparator", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendTitle.TitleSeparator, ref result, chartLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(chartLegendTitle.ExprHost != null, "(chartLegendTitle.ExprHost != null)");
						result.Value = ((ChartLegendTitleExprHost)chartLegendTitle.ExprHost).TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.Label, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.Label, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(chartDataLabel.Label, ref result);
			return result;
		}

		internal string EvaluateChartDataLabePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Position", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.Position, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.ChartDataLabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartDataLabelRotationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.Rotation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Rotation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.Rotation, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartDataLabelUseValueAsLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.UseValueAsLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UseValueAsLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.UseValueAsLabel, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.UseValueAsLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartDataLabelVisibleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.Visible, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.Visible, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartDataLabelToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataLabel.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataLabel.ToolTip, ref result, chartDataLabel.ExprHost))
					{
						Global.Tracer.Assert(chartDataLabel.ExprHost != null, "(chartDataLabel.ExprHost != null)");
						result.Value = chartDataLabel.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.X, objectName, "X", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesXExpr);
		}

		internal string EvaluateChartTickMarksEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.Enabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.Enabled, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartTickMarksTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.Type, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.Type, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksLengthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.Length, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Length", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.Length, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.Interval, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.IntervalType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.IntervalType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartTickMarksIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.IntervalOffset, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartTickMarksIntervalOffsetTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartTickMarks.IntervalOffsetType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartTickMarks.IntervalOffsetType, ref result, chartTickMarks.ExprHost))
					{
						Global.Tracer.Assert(chartTickMarks.ExprHost != null, "(chartTickMarks.ExprHost != null)");
						result.Value = chartTickMarks.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartItemInLegendLegendTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartItemInLegend.LegendText, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartItemInLegend.LegendText, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						result.Value = chartItemInLegend.ExprHost.LegendTextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartItemInLegendToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartItemInLegend.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartItemInLegend.ToolTip, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						result.Value = chartItemInLegend.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal bool EvaluateChartItemInLegendHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartItemInLegend.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartItemInLegend.Hidden, ref result, chartItemInLegend.ExprHost))
					{
						Global.Tracer.Assert(chartItemInLegend.ExprHost != null, "(chartItemInLegend.ExprHost != null)");
						if (chartItemInLegend.ExprHost != null)
						{
							result.Value = chartItemInLegend.ExprHost.HiddenExpr;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateChartEmptyPointsAxisLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartEmptyPoints.AxisLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartEmptyPoints.AxisLabel, ref result, chartEmptyPoints.ExprHost))
					{
						Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
						result.Value = chartEmptyPoints.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(chartEmptyPoints.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartEmptyPointsToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartEmptyPoints.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartEmptyPoints.ToolTip, ref result, chartEmptyPoints.ExprHost))
					{
						Global.Tracer.Assert(chartEmptyPoints.ExprHost != null, "(chartEmptyPoints.ExprHost != null)");
						result.Value = chartEmptyPoints.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartFormulaParameterValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartFormulaParameter.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartFormulaParameter.Value, ref result, chartFormulaParameter.ExprHost))
					{
						Global.Tracer.Assert(chartFormulaParameter.ExprHost != null, "(chartFormulaParameter.ExprHost != null)");
						result.Value = chartFormulaParameter.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(chartFormulaParameter.Value, ref result);
			return result;
		}

		internal double EvaluateChartElementPositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string propertyName, ChartElementPositionExprHost exprHost, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position position, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (position)
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Top:
							result.Value = exprHost.TopExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Left:
							result.Value = exprHost.LeftExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Height:
							result.Value = exprHost.HeightExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition.Position.Width:
							result.Value = exprHost.WidthExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartSmartLabelAllowOutSidePlotAreaExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.AllowOutSidePlotArea, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowOutSidePlotArea", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.AllowOutSidePlotArea, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.AllowOutSidePlotAreaExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutBackColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutBackColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutBackColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutBackColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutBackColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateChartSmartLabelCalloutLineAnchorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutLineAnchor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineAnchor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutLineAnchor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineAnchorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutLineColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutLineColor, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateChartSmartLabelCalloutLineStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutLineStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutLineStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSmartLabelCalloutLineWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutLineWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutLineWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutLineWidth, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutLineWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelCalloutStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.CalloutStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CalloutStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.CalloutStyle, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.CalloutStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelShowOverlappedExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.ShowOverlapped, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ShowOverlapped", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.ShowOverlapped, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.ShowOverlappedExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelMarkerOverlappingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.MarkerOverlapping, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarkerOverlapping", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.MarkerOverlapping, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MarkerOverlappingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSmartLabelDisabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.Disabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Disabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.Disabled, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.DisabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartSmartLabelMaxMovingDistanceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.MaxMovingDistance, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxMovingDistance", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.MaxMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MaxMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartSmartLabelMinMovingDistanceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSmartLabel.MinMovingDistance, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinMovingDistance", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSmartLabel.MinMovingDistance, ref result, chartSmartLabel.ExprHost))
					{
						Global.Tracer.Assert(chartSmartLabel.ExprHost != null, "(chartSmartLabel.ExprHost != null)");
						result.Value = chartSmartLabel.ExprHost.MinMovingDistanceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartLegendHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.Hidden, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendPositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.Position, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ChartLegendPositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendLayoutExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.Layout, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.Layout, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartLegendDockOutsideChartAreaExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.DockOutsideChartArea, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.DockOutsideChartArea, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.DockOutsideChartAreaExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartLegendAutoFitTextDisabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.AutoFitTextDisabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.AutoFitTextDisabled, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendMinFontSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.MinFontSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.MinFontSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendHeaderSeparatorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.HeaderSeparator, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.HeaderSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendHeaderSeparatorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.HeaderSeparatorColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.HeaderSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.HeaderSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateChartLegendColumnSeparatorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.ColumnSeparator, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.ColumnSeparator, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnSeparatorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.ColumnSeparatorColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.ColumnSeparatorColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal int EvaluateChartLegendColumnSpacingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.ColumnSpacing, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.ColumnSpacing, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ColumnSpacingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartLegendInterlacedRowsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.InterlacedRows, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.InterlacedRows, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendInterlacedRowsColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.InterlacedRowsColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.InterlacedRowsColor, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal bool EvaluateChartLegendEquallySpacedItemsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.EquallySpacedItems, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.EquallySpacedItems, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartLegendReversedExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.Reversed, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reversed", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.Reversed, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendMaxAutoSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.MaxAutoSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.MaxAutoSize, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.MaxAutoSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendTextWrapThresholdExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegend.TextWrapThreshold, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegend.TextWrapThreshold, ref result, chartLegend.ExprHost))
					{
						Global.Tracer.Assert(chartLegend.ExprHost != null, "(chartLegend.ExprHost != null)");
						result.Value = chartLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendColumnHeaderValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeader, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumnHeader.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumnHeader.Value, ref result, chartLegendColumnHeader.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumnHeader.ExprHost != null, "(chartLegendColumnHeader.ExprHost != null)");
						result.Value = chartLegendColumnHeader.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnColumnTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.ColumnType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ColumnType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.ColumnType, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ColumnTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.Value, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.ToolTip, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendColumnMinimumWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.MinimumWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinimumWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.MinimumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MinimumWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartLegendColumnMaximumWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.MaximumWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaximumWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.MaximumWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.MaximumWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal int EvaluateChartLegendColumnSeriesSymbolWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.SeriesSymbolWidth, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendColumnSeriesSymbolHeightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendColumn.SeriesSymbolHeight, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeriesSymbolHeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendColumn.SeriesSymbolHeight, ref result, chartLegendColumn.ExprHost))
					{
						Global.Tracer.Assert(chartLegendColumn.ExprHost != null, "(chartLegendColumn.ExprHost != null)");
						result.Value = chartLegendColumn.ExprHost.SeriesSymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellCellTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.CellType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.CellType, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.Text, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Text", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.Text, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellCellSpanExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.CellSpan, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CellSpan", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.CellSpan, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.CellSpanExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.ToolTip, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.ImageWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.ImageWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellImageHeightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.ImageHeight, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ImageHeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.ImageHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.ImageHeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolHeightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolHeight, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolHeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.SymbolHeight, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolHeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellSymbolWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.SymbolWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SymbolWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.SymbolWidth, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.SymbolWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemCellAlignmentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.Alignment, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Alignment", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.Alignment, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.AlignmentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellTopMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.TopMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TopMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.TopMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellBottomMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.BottomMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BottomMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.BottomMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellLeftMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.LeftMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LeftMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.LeftMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartLegendCustomItemCellRightMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItemCell.RightMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "RightMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItemCell.RightMargin, ref result, chartLegendCustomItemCell.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItemCell.ExprHost != null, "(chartLegendCustomItemCell.ExprHost != null)");
						result.Value = chartLegendCustomItemCell.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.Up, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Up", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.Up, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.Down, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Down", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.Down, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsLeftExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.Left, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Left", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.Left, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsRightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.Right, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Right", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.Right, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.RightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpLeftExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.UpLeft, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpLeft", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.UpLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsUpRightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.UpRight, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "UpRight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.UpRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.UpRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownLeftExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.DownLeft, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownLeft", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.DownLeft, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartNoMoveDirectionsDownRightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartNoMoveDirections.DownRight, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "DownRight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartNoMoveDirections.DownRight, ref result, chartNoMoveDirections.ExprHost))
					{
						Global.Tracer.Assert(chartNoMoveDirections.ExprHost != null, "(chartNoMoveDirections.ExprHost != null)");
						result.Value = chartNoMoveDirections.ExprHost.DownRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartStripLineTitleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.Title, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Title", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.Title, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartStripLineTitleAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.TitleAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TitleAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.TitleAngle, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TitleAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartStripLineTextOrientationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.TextOrientation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "TextOrientation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.TextOrientation, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.TextOrientationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartStripLineToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.ToolTip, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.Interval, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.IntervalType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.IntervalType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.IntervalOffset, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineIntervalOffsetTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.IntervalOffsetType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.IntervalOffsetType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartStripLineStripWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.StripWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.StripWidth, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartStripLineStripWidthTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartStripLine.StripWidthType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "StripWidthType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartStripLine.StripWidthType, ref result, chartStripLine.ExprHost))
					{
						Global.Tracer.Assert(chartStripLine.ExprHost != null, "(chartStripLine.ExprHost != null)");
						result.Value = chartStripLine.ExprHost.StripWidthTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItem.Separator, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Separator", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItem.Separator, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartLegendCustomItemSeparatorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItem.SeparatorColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "SeparatorColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItem.SeparatorColor, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateChartLegendCustomItemToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartLegendCustomItem.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartLegendCustomItem.ToolTip, ref result, chartLegendCustomItem.ExprHost))
					{
						Global.Tracer.Assert(chartLegendCustomItem.ExprHost != null, "(chartLegendCustomItem.ExprHost != null)");
						result.Value = chartLegendCustomItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.Type, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.Type, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesSubtypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.Subtype, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Subtype", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.Subtype, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.SubtypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesLegendNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.LegendName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.LegendName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.LegendNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesLegendTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.LegendText, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LegendText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.LegendText, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.LegendTextExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesChartAreaNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.ChartAreaName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartAreaName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.ChartAreaName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ChartAreaNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartSeriesValueAxisNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.ValueAxisName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ValueAxisName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.ValueAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ValueAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateChartSeriesToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.ToolTip, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartSeriesCategoryAxisNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.CategoryAxisName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CategoryAxisName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.CategoryAxisName, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.CategoryAxisNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartSeriesHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.Hidden, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartSeriesHideInLegendExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartSeries.HideInLegend, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideInLegend", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartSeries.HideInLegend, ref result, chartSeries.ExprHost))
					{
						Global.Tracer.Assert(chartSeries.ExprHost != null, "(chartSeries.ExprHost != null)");
						result.Value = chartSeries.ExprHost.HideInLegendExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartBorderSkinBorderSkinTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkin, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartBorderSkin.BorderSkinType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ChartBorderSkinType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartBorderSkin.BorderSkinType, ref result, chartBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(chartBorderSkin.ExprHost != null, "(chartBorderSkin.ExprHost != null)");
						result.Value = chartBorderSkin.ExprHost.BorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisScaleBreakEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.Enabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.Enabled, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakBreakLineTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.BreakLineType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "BreakLineType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.BreakLineType, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.BreakLineTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result, true).Value;
		}

		internal int EvaluateChartAxisScaleBreakCollapsibleSpaceThresholdExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "CollapsibleSpaceThreshold", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.CollapsibleSpaceThreshold, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.CollapsibleSpaceThresholdExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartAxisScaleBreakMaxNumberOfBreaksExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.MaxNumberOfBreaks, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxNumberOfBreaks", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.MaxNumberOfBreaks, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.MaxNumberOfBreaksExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal double EvaluateChartAxisScaleBreakSpacingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.Spacing, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Spacing", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.Spacing, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.SpacingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisScaleBreakIncludeZeroExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxisScaleBreak.IncludeZero, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxisScaleBreak.IncludeZero, ref result, chartAxisScaleBreak.ExprHost))
					{
						Global.Tracer.Assert(chartAxisScaleBreak.ExprHost != null, "(chartAxisScaleBreak.ExprHost != null)");
						result.Value = chartAxisScaleBreak.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartCustomPaletteColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor customPaletteColor, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customPaletteColor.Color, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Color", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customPaletteColor.Color, ref result, customPaletteColor.ExprHost))
					{
						Global.Tracer.Assert(customPaletteColor.ExprHost != null, "(customPaletteColor.ExprHost != null)");
						result.Value = customPaletteColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal VariantResult EvaluateChartDataPointAxisLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataPoint.AxisLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AxisLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataPoint.AxisLabel, ref result, chartDataPoint.ExprHost))
					{
						Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
						result.Value = chartDataPoint.ExprHost.AxisLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(chartDataPoint.AxisLabel, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataPoint.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataPoint.ToolTip, ref result, chartDataPoint.ExprHost))
					{
						Global.Tracer.Assert(chartDataPoint.ExprHost != null, "(chartDataPoint.ExprHost != null)");
						result.Value = chartDataPoint.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Y, objectName, "Y", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesYExpr);
		}

		internal VariantResult EvaluateChartDataPointValueSizesExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Size, objectName, "Size", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesSizeExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataPoint.DataPointValues.High, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "High", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataPoint.DataPointValues.High, ref result, dataPoint.ExprHost))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
						result.Value = dataPoint.ExprHost.DataPointValuesHighExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(dataPoint.DataPointValues.High, ref result);
			return result;
		}

		internal VariantResult EvaluateChartDataPointValuesLowExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Low, objectName, "Low", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesLowExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesStartExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Start, objectName, "Start", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesStartExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesEndExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.End, objectName, "End", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesEndExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMeanExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Mean, objectName, "Mean", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMeanExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesMedianExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.Median, objectName, "Median", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesMedianExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightX, objectName, "HighlightX", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightXExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightY, objectName, "HighlightY", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightYExpr);
		}

		internal VariantResult EvaluateChartDataPointValuesHighlightSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsVariant(dataPoint, dataPoint.DataPointValues.HighlightSize, objectName, "HighlightSize", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesHighlightSizeExpr);
		}

		internal string EvaluateChartDataPointValuesFormatXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatX, objectName, "FormatX", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatXExpr);
		}

		internal string EvaluateChartDataPointValuesFormatYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatY, objectName, "FormatY", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatYExpr);
		}

		internal string EvaluateChartDataPointValuesFormatSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.FormatSize, objectName, "FormatSize", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesFormatSizeExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageX, objectName, "CurrencyLanguageX", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageXExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageY, objectName, "CurrencyLanguageY", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageYExpr);
		}

		internal string EvaluateChartDataPointValuesCurrencyLanguageSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, string objectName)
		{
			return this.EvaluateChartDataPointValuesExpressionAsString(dataPoint, dataPoint.DataPointValues.CurrencyLanguageSize, objectName, "CurrencyLanguageSize", (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dp) => dp.ExprHost.DataPointValuesCurrencyLanguageSizeExpr);
		}

		private VariantResult EvaluateChartDataPointValuesExpressionAsVariant(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = this.EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			this.ProcessVariantResult(expressionInfo, ref result);
			return result;
		}

		private string EvaluateChartDataPointValuesExpressionAsString(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = this.EvaluateChartDataPointValuesExpression(dataPoint, expressionInfo, objectName, propertyName, expressionFunction);
			return this.ProcessStringResult(result).Value;
		}

		private VariantResult EvaluateChartDataPointValuesExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, string objectName, string propertyName, EvalulateDataPoint expressionFunction)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result, dataPoint.ExprHost))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null, "(dataPoint.ExprHost != null)");
						result.Value = expressionFunction(dataPoint);
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateChartMarkerSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartMarker.Size, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Size", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartMarker.Size, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartMarkerType(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartMarker.Type, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Type", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartMarker.Type, ref result, chartMarker.ExprHost))
					{
						Global.Tracer.Assert(chartMarker.ExprHost != null, "(chartMarker.ExprHost != null)");
						result.Value = chartMarker.ExprHost.TypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisVisibleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Visible, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Visible", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Visible, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VisibleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartAxisMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Margin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Margin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Margin, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Interval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.IntervalType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.IntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.IntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisIntervalOffsetTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.IntervalOffsetType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IntervalOffsetType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.IntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisMarksAlwaysAtPlotEdgeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.MarksAlwaysAtPlotEdge, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MarksAlwaysAtPlotEdge", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.MarksAlwaysAtPlotEdge, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MarksAlwaysAtPlotEdgeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisReverseExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Reverse, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Reverse", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Reverse, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ReverseExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisLocationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Location, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Location", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Location, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LocationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisInterlacedExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Interlaced, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Interlaced", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Interlaced, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisInterlacedColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.InterlacedColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "InterlacedColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.InterlacedColor, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.InterlacedColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal bool EvaluateChartAxisLogScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LogScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LogScale, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLogBaseExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LogBase, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LogBase", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LogBase, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LogBaseExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateChartAxisHideLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.HideLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.HideLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Angle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Angle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Angle, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisArrowsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.Arrows, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Arrows", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.Arrows, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.ArrowsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontShrinkExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.PreventFontShrink, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontShrink", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.PreventFontShrink, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontShrinkExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventFontGrowExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.PreventFontGrow, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventFontGrow", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.PreventFontGrow, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventFontGrowExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventLabelOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.PreventLabelOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventLabelOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.PreventLabelOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventLabelOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisPreventWordWrapExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.PreventWordWrap, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "PreventWordWrap", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.PreventWordWrap, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.PreventWordWrapExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisAllowLabelRotationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.AllowLabelRotation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "AllowLabelRotation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.AllowLabelRotation, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.AllowLabelRotationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAxisIncludeZeroExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.IncludeZero, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "IncludeZero", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.IncludeZero, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.IncludeZeroExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisLabelsAutoFitDisabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LabelsAutoFitDisabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelsAutoFitDisabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LabelsAutoFitDisabled, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelsAutoFitDisabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAxisMinFontSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.MinFontSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MinFontSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.MinFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateChartAxisMaxFontSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.MaxFontSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "MaxFontSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.MaxFontSize, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.MaxFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateChartAxisOffsetLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.OffsetLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "OffsetLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.OffsetLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.OffsetLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisHideEndLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.HideEndLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "HideEndLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.HideEndLabels, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAxisVariableAutoIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.VariableAutoInterval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "VariableAutoInterval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.VariableAutoInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.VariableAutoIntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LabelInterval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelInterval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LabelInterval, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LabelIntervalType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LabelIntervalType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartAxisLabelIntervalOffsetsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LabelIntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LabelIntervalOffset, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartAxisLabelIntervalOffsetTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAxis.LabelIntervalOffsetType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "LabelIntervalOffsetType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAxis.LabelIntervalOffsetType, ref result, chartAxis.ExprHost))
					{
						Global.Tracer.Assert(chartAxis.ExprHost != null, "(chartAxis.ExprHost != null)");
						result.Value = chartAxis.ExprHost.LabelIntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal object EvaluateChartAxisValueExpression(ChartAxisExprHost exprHost, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName, string propertyName, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType type)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref variantResult, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						switch (type)
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Min:
							variantResult.Value = exprHost.AxisMinExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.Max:
							variantResult.Value = exprHost.AxisMaxExpr;
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.ExpressionType.CrossAt:
							variantResult.Value = exprHost.AxisCrossAtExpr;
							break;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(expression, ref variantResult);
			return variantResult.Value;
		}

		internal bool EvaluateChartAreaEquallySizedAxesFontExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartArea.EquallySizedAxesFont, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartArea.EquallySizedAxesFont, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.EquallySizedAxesFontExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartAreaAlignOrientationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartArea.AlignOrientation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartArea.AlignOrientation, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.AlignOrientationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartAreaHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartArea, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartArea.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartArea.Hidden, ref result, chartArea.ExprHost))
					{
						Global.Tracer.Assert(chartArea.ExprHost != null, "(chartArea.ExprHost != null)");
						result.Value = chartArea.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeAxesViewExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAlignType.AxesView, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAlignType.AxesView, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.AxesViewExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeCursorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAlignType.Cursor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAlignType.Cursor, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.CursorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAlignType.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAlignType.Position, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.ChartAlignTypePositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateChartAlignTypeInnerPlotPositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartAlignType.InnerPlotPosition, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartAlignType.InnerPlotPosition, ref result, chartAlignType.ExprHost))
					{
						Global.Tracer.Assert(chartAlignType.ExprHost != null, "(chartAlignType.ExprHost != null)");
						result.Value = chartAlignType.ExprHost.InnerPlotPositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartGridLinesEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartGridLines.Enabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, "Enabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartGridLines.Enabled, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartGridLines.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartGridLines.Interval, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartGridLines.IntervalType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartGridLines.IntervalType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateChartGridLinesIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartGridLines.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartGridLines.IntervalOffset, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateChartGridLinesIntervalOffsetTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartGridLines.IntervalOffsetType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartGridLines.IntervalOffsetType, ref result, chartGridLines.ExprHost))
					{
						Global.Tracer.Assert(chartGridLines.ExprHost != null, "(chartGridLines.ExprHost != null)");
						result.Value = chartGridLines.ExprHost.IntervalOffsetTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Enabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Enabled, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.EnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesProjectionModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.ProjectionMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.ProjectionMode, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ProjectionModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesRotationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Rotation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Rotation, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.RotationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesInclinationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Inclination, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Inclination, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.InclinationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesPerspectiveExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Perspective, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Perspective, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.PerspectiveExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesDepthRatioExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.DepthRatio, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.DepthRatio, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.DepthRatioExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateChartThreeDPropertiesShadingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Shading, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Shading, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ShadingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesGapDepthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.GapDepth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.GapDepth, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.GapDepthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateChartThreeDPropertiesWallThicknessExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.WallThickness, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.WallThickness, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.WallThicknessExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateChartThreeDPropertiesClusteredExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartThreeDProperties.Clustered, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartThreeDProperties.Clustered, ref result, chartThreeDProperties.ExprHost))
					{
						Global.Tracer.Assert(chartThreeDProperties.ExprHost != null, "(chartThreeDProperties.ExprHost != null)");
						result.Value = chartThreeDProperties.ExprHost.ClusteredExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateRectanglePageNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle, objectName, "PageName", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, rectangle.ExprHost))
						{
							Global.Tracer.Assert(rectangle.ExprHost != null, "(rectangle.ExprHost != null)");
							result.Value = rectangle.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessStringResult(result, true).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal string EvaluateStyleBorderColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorRight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorTop(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBorderColorBottom(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderColorBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BorderStyles EvaluateStyleBorderStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(this.ProcessStringResult(result).Value, (BorderStyles)((AspNetCore.ReportingServices.ReportProcessing.ObjectType.Line != objectType) ? 1 : 4), this);
		}

		internal BorderStyles EvaluateStyleBorderStyleLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(this.ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleRight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(this.ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleTop(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(this.ProcessStringResult(result).Value, this);
		}

		internal BorderStyles EvaluateStyleBorderStyleBottom(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderStyleBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBorderStyle(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidth(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthRight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthTop(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthBottom(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BorderWidthBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleBackgroundGradientEndColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientEndColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientEndColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal BackgroundGradients EvaluateStyleBackgroundGradientType(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundGradientType(this.ProcessStringResult(result).Value, this);
		}

		internal BackgroundRepeatTypes EvaluateStyleBackgroundRepeat(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundRepeat", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundRepeatExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateBackgroundRepeat(this.ProcessStringResult(result).Value, this, objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal FontStyles EvaluateStyleFontStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontStyle(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontFamily(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontFamily", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontFamilyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleFontSize(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateFontSize(this.ProcessStringResult(result).Value, this);
		}

		internal FontWeights EvaluateStyleFontWeight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontWeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FontWeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateFontWeight(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFormat(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Format", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.FormatExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal TextDecorations EvaluateStyleTextDecoration(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TextDecoration", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextDecorationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextDecoration(this.ProcessStringResult(result).Value, this);
		}

		internal TextAlignments EvaluateStyleTextAlign(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TextAlign", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextAlignExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateTextAlign(this.ProcessStringResult(result).Value, this);
		}

		internal VerticalAlignments EvaluateStyleVerticalAlign(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "VerticalAlign", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.VerticalAlignExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateVerticalAlign(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Color", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStylePaddingLeft(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingLeft", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingRight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingRight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingTop(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingTop", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingBottom(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingBottom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PaddingBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLineHeight(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "LineHeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LineHeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateLineHeight(this.ProcessStringResult(result).Value, this);
		}

		internal Directions EvaluateStyleDirection(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Direction", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.DirectionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateDirection(this.ProcessStringResult(result).Value, this);
		}

		internal WritingModes EvaluateStyleWritingMode(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "WritingMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.WritingModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateWritingMode(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Language", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.LanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo cultureInfo = default(CultureInfo);
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSpecificLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out cultureInfo);
		}

		internal UnicodeBiDiTypes EvaluateStyleUnicodeBiDi(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "UnicodeBiDi", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.UnicodeBiDiExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateUnicodeBiDi(this.ProcessStringResult(result).Value, this);
		}

		internal Calendars EvaluateStyleCalendar(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Calendar", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.CalendarExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslateCalendar(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleCurrencyLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "CurrencyLanguage", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(false, "(style.ExprHost should not be invoked for CurrencyLanguage.)");
						result.Value = null;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleNumeralLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "NumeralLanguage", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralLanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo cultureInfo = default(CultureInfo);
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out cultureInfo);
		}

		internal object EvaluateStyleNumeralVariant(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, objectType, objectName, "NumeralVariant", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.NumeralVariantExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			if (result.Value == null)
			{
				return null;
			}
			IntegerResult integerResult = this.ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateNumeralVariant(integerResult.Value, this);
		}

		internal object EvaluateTransparentColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TransparentColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal object EvaluatePosition(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, objectType, objectName, "Position", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.PositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return StyleTranslator.TranslatePosition(this.ProcessStringResult(result).Value, this, objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart);
		}

		internal string EvaluateStyleBackgroundUrlImageValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateStyleBackgroundEmbeddedImageValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateEmbeddedImageName(this.ProcessStringResult(result).Value, embeddedImages, this);
		}

		internal byte[] EvaluateStyleBackgroundDatabaseImageValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBinaryResult(result).Value;
		}

		internal string EvaluateStyleBackgroundImageMIMEType(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundImageMIMEType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundImageMIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateMimeType(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleTextEffect(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TextEffect", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.TextEffectExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextEffect(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleShadowColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "ShadowColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(objectType));
		}

		internal string EvaluateStyleShadowOffset(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "ShadowOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.ShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundHatchType(AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundHatchType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result, style.ExprHost))
					{
						Global.Tracer.Assert(style.ExprHost != null, "(style.ExprHost != null)");
						result.Value = style.ExprHost.BackgroundHatchTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateBackgroundHatchType(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphLeftIndentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.LeftIndent, paragraph.ObjectType, paragraph.Name, "LeftIndent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.LeftIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.LeftIndentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphRightIndentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.RightIndent, paragraph.ObjectType, paragraph.Name, "RightIndent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.RightIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.RightIndentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphHangingIndentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.HangingIndent, paragraph.ObjectType, paragraph.Name, "HangingIndent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.HangingIndent, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.HangingIndentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, true, this);
		}

		internal string EvaluateParagraphSpaceBeforeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.SpaceBefore, paragraph.ObjectType, paragraph.Name, "SpaceBefore", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.SpaceBefore, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceBeforeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateParagraphSpaceAfterExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.SpaceAfter, paragraph.ObjectType, paragraph.Name, "SpaceAfter", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.SpaceAfter, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.SpaceAfterExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal int? EvaluateParagraphListLevelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.ListLevel, paragraph.ObjectType, paragraph.Name, "ListLevel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.ListLevel, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListLevelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			IntegerResult integerResult = this.ProcessIntegerResult(result);
			if (integerResult.ErrorOccurred)
			{
				return null;
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListLevel(integerResult.Value, this);
		}

		internal string EvaluateParagraphListStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paragraph.ListStyle, paragraph.ObjectType, paragraph.Name, "ListStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paragraph.ListStyle, ref result, paragraph.ExprHost))
					{
						Global.Tracer.Assert(paragraph.ExprHost != null);
						result.Value = paragraph.ExprHost.ListStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateParagraphListStyle(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateTextRunToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(textRun.ToolTip, textRun.ObjectType, textRun.Name, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(textRun.ToolTip, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateTextRunMarkupTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(textRun.MarkupType, textRun.ObjectType, textRun.Name, "MarkupType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(textRun.MarkupType, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.MarkupTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateTextRunMarkupType(this.ProcessStringResult(result).Value, this);
		}

		internal VariantResult EvaluateTextRunValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = textRun.Value;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value, textRun.ObjectType, textRun.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value, ref result, textRun.ExprHost))
					{
						Global.Tracer.Assert(textRun.ExprHost != null);
						result.Value = textRun.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(value, ref result);
			return result;
		}

		internal bool EvaluatePageBreakDisabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Disabled", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.DisabledExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessBooleanResult(result).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal bool EvaluatePageBreakResetPageNumberExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "ResetPageNumber", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, pageBreak.ExprHost))
						{
							Global.Tracer.Assert(pageBreak.ExprHost != null, "(pageBreak.ExprHost != null)");
							result.Value = pageBreak.ExprHost.ResetPageNumberExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessBooleanResult(result).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}

		internal VariantResult EvaluateJoinConditionForeignKeyExpression(Relationship.JoinCondition joinCondition)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo foreignKeyExpression = joinCondition.ForeignKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(foreignKeyExpression, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(foreignKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.ForeignKeyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(foreignKeyExpression, ref result);
			this.VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		internal VariantResult EvaluateJoinConditionPrimaryKeyExpression(Relationship.JoinCondition joinCondition)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo primaryKeyExpression = joinCondition.PrimaryKeyExpression;
			JoinConditionExprHost exprHost = joinCondition.ExprHost;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(primaryKeyExpression, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(primaryKeyExpression, ref result, exprHost))
					{
						Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
						result.Value = exprHost.PrimaryKeyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(primaryKeyExpression, ref result);
			this.VerifyVariantResultAndStopOnError(ref result);
			return result;
		}

		private bool EvaluateSimpleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_propertyName = propertyName;
			if (this.m_topLevelReportRuntime != null)
			{
				this.m_topLevelReportRuntime.ObjectType = objectType;
				this.m_topLevelReportRuntime.ObjectName = objectName;
				this.m_topLevelReportRuntime.PropertyName = propertyName;
			}
			return this.EvaluateSimpleExpression(expression, out result);
		}

		private bool EvaluateSimpleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, out VariantResult result)
		{
			result = default(VariantResult);
			if (expression != null)
			{
				switch (expression.Type)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant:
					result.Value = expression.Value;
					result.TypeCode = expression.ConstantTypeCode;
					return true;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field:
					this.EvaluateSimpleFieldReference(expression.IntValue, ref result);
					return true;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token:
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = ((DataSets)this.m_reportObjectModel.DataSetsImpl)[expression.StringValue];
					result.Value = dataSet.RewrittenCommandText;
					return true;
				}
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					return false;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					return false;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
					return false;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					return false;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.RdlFunction:
					this.EvaluateRdlFunction(expression, ref result);
					return true;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Literal:
					result.Value = expression.LiteralInfo.Value;
					return true;
				default:
					Global.Tracer.Assert(false);
					return true;
				}
			}
			return true;
		}

		internal void EvaluateSimpleFieldReference(int fieldIndex, ref VariantResult result)
		{
			try
			{
				AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.FieldImpl fieldImpl = this.m_reportObjectModel.FieldsImpl[fieldIndex];
				if (fieldImpl.IsMissing)
				{
					result.Value = null;
				}
				else if (fieldImpl.FieldStatus != 0)
				{
					result.ErrorOccurred = true;
					result.FieldStatus = fieldImpl.FieldStatus;
					result.ExceptionMessage = fieldImpl.ExceptionMessage;
					result.Value = null;
				}
				else
				{
					result.Value = fieldImpl.Value;
				}
			}
			catch (ReportProcessingException_NoRowsFieldAccess e)
			{
				this.RegisterRuntimeWarning(e, this);
				result.Value = null;
			}
			catch (ReportProcessingException_InvalidOperationException)
			{
				result.Value = null;
				result.ErrorOccurred = true;
			}
		}

		private bool EvaluateComplexExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, ReportObjectModelProxy exprHost)
		{
			if (expression != null)
			{
				switch (expression.Type)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					result.Value = ((Aggregates)this.m_reportObjectModel.AggregatesImpl)[expression.StringValue];
					return true;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				{
					Lookup lookup2 = ((Lookups)this.m_reportObjectModel.LookupsImpl)[expression.StringValue];
					result.Value = lookup2.Value;
					return true;
				}
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
				{
					Lookup lookup = ((Lookups)this.m_reportObjectModel.LookupsImpl)[expression.StringValue];
					result.Value = lookup.Values;
					return true;
				}
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.ScopedFieldReference:
					try
					{
						this.m_reportObjectModel.OdpContext.StateManager.EvaluateScopedFieldReference(expression.StringValue, expression.ScopedFieldInfo.FieldIndex, ref result);
					}
					catch (ReportProcessingException_NonExistingScopeReference reportProcessingException_NonExistingScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_NonExistingScopeReference.Message;
					}
					catch (ReportProcessingException_InvalidScopeReference reportProcessingException_InvalidScopeReference)
					{
						result.Value = null;
						result.ErrorOccurred = true;
						result.ExceptionMessage = reportProcessingException_InvalidScopeReference.Message;
					}
					return true;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression:
					if (this.m_exprHostInSandboxAppDomain && exprHost != null)
					{
						exprHost.SetReportObjectModel(this.m_reportObjectModel);
					}
					return false;
				default:
					Global.Tracer.Assert(false);
					return true;
				}
			}
			return true;
		}

		private void EvaluateRdlFunction(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions = expression.RdlFunctionInfo.Expressions;
			object[] array = new object[expressions.Count];
			for (int i = 0; i < expressions.Count; i++)
			{
				VariantResult variantResult = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expressions[i], out variantResult) && !this.EvaluateComplexExpression(expressions[i], ref variantResult, null))
				{
					Global.Tracer.Assert(false, "Rdl function argument is complex.");
				}
				array[i] = variantResult.Value;
				if (variantResult.ErrorOccurred)
				{
					result = variantResult;
					return;
				}
			}
			switch (expression.RdlFunctionInfo.FunctionType)
			{
			case RdlFunctionInfo.RdlFunctionType.MinValue:
				result.Value = this.MinValue(array);
				break;
			case RdlFunctionInfo.RdlFunctionType.MaxValue:
				result.Value = this.MaxValue(array);
				break;
			default:
				Global.Tracer.Assert(false, "No case for: " + expression.RdlFunctionInfo.FunctionType.ToString());
				break;
			}
		}

		private void RegisterRuntimeWarning(Exception e, IErrorContext iErrorContext)
		{
			if (e is ReportProcessingException_NoRowsFieldAccess)
			{
				iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, Severity.Warning, e.Message);
				return;
			}
			if (Global.Tracer.TraceError)
			{
				Global.Tracer.Trace("Caught unexpected exception inside of RegisterRuntimeWarning.");
			}
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		private void RegisterRuntimeErrorInExpressionAndStop(ref VariantResult result, Exception e)
		{
			this.RegisterRuntimeErrorInExpression(ref result, e, (IErrorContext)this, true);
			if (!result.ErrorOccurred)
			{
				return;
			}
			ProcessingMessageList messages = this.m_errorContext.Messages;
			if (messages != null && messages.Count > 0)
			{
				throw new ReportProcessingException(messages[0].Message, ErrorCode.rsProcessingError);
			}
			throw new ReportProcessingException(messages);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e)
		{
			this.RegisterRuntimeErrorInExpression(ref result, e, (IErrorContext)this, false);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e, IErrorContext iErrorContext, bool isError)
		{
			if (!(e is RSException) && !AsynchronousExceptionDetection.IsStoppingException(e))
			{
				if (e is ReportProcessingException_FieldError)
				{
					result.FieldStatus = ((ReportProcessingException_FieldError)e).Status;
					if (DataFieldStatus.IsMissing == result.FieldStatus)
					{
						result = new VariantResult(false, null);
					}
					else
					{
						result = new VariantResult(true, null);
					}
				}
				else if (e is ReportProcessingException_InvalidOperationException)
				{
					result = new VariantResult(true, null);
				}
				else
				{
					if (e is ReportProcessingException_UserProfilesDependencies)
					{
						iErrorContext.Register(ProcessingErrorCode.rsRuntimeUserProfileDependency, Severity.Error, (string[])null);
						throw new ReportProcessingException(this.m_errorContext.Messages);
					}
					string text;
					if (e != null)
					{
						try
						{
							text = ((e.InnerException == null) ? e.Message : e.InnerException.Message);
						}
						catch
						{
							text = RPRes.NonClsCompliantException;
						}
					}
					else
					{
						text = RPRes.NonClsCompliantException;
					}
					iErrorContext.Register(ProcessingErrorCode.rsRuntimeErrorInExpression, (Severity)((!isError) ? 1 : 0), text);
					if (e is ReportProcessingException_NoRowsFieldAccess)
					{
						result = new VariantResult(false, null);
					}
					else
					{
						if (isError)
						{
							throw new ReportProcessingException(this.m_errorContext.Messages);
						}
						result = new VariantResult(true, null);
						StringBuilder stringBuilder = new StringBuilder();
						for (Exception ex = e; ex != null; ex = ex.InnerException)
						{
							if (ex.Message != null)
							{
								stringBuilder.Append(ex.Message);
								stringBuilder.Append(";");
							}
							if (ex.Source != null)
							{
								stringBuilder.Append(" Source: ");
								stringBuilder.Append(ex.Source);
								stringBuilder.Append(";");
							}
						}
						result.ExceptionMessage = stringBuilder.ToString();
					}
				}
				return;
			}
			throw new ReportProcessingException(e.GetType().FullName + ": " + e.Message, ErrorCode.rsProcessingError);
		}

		private bool EvaluateBooleanExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool canBeConstant, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.BoolValue;
					result.TypeCode = TypeCode.Boolean;
				}
				else
				{
					result.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return this.EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result)
		{
			return this.ProcessBooleanResult(result, false, this.m_objectType, null);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result, bool stopOnError, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			BooleanResult result2 = default(BooleanResult);
			bool value = default(bool);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				if (stopOnError && result.FieldStatus != 0)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, objectType, objectName, "Hidden", ReportRuntime.GetErrorName(result.FieldStatus, result.ExceptionMessage));
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
			}
			else if (ReportRuntime.TryProcessObjectToBoolean(result.Value, out value))
			{
				result2.Value = value;
			}
			else
			{
				result2.ErrorOccurred = true;
				if (stopOnError)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, objectType, objectName, "Hidden");
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
				this.RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		internal static bool TryProcessObjectToBoolean(object value, out bool processedValue)
		{
			if (value is bool)
			{
				processedValue = (bool)value;
				return true;
			}
			if (value != null && DBNull.Value != value)
			{
				processedValue = false;
				return false;
			}
			processedValue = false;
			return true;
		}

		private bool EvaluateBinaryExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return this.EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BinaryResult ProcessBinaryResult(VariantResult result)
		{
			BinaryResult result2 = default(BinaryResult);
			object value = result.Value;
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (value is byte[])
			{
				byte[] array = (byte[])value;
				if (this.ViolatesMaxArrayResultLength(array.Length))
				{
					result2.ErrorOccurred = true;
					result2.Value = null;
					this.RegisterSandboxMaxArrayLengthWarning();
				}
				else
				{
					result2.Value = array;
				}
			}
			else if (value == null || DBNull.Value == value)
			{
				result2.Value = null;
			}
			else
			{
				if (value is string)
				{
					try
					{
						string text = (string)value;
						if (this.ViolatesMaxStringResultLength(text))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							this.RegisterSandboxMaxStringLengthWarning();
							return result2;
						}
						byte[] array2 = Convert.FromBase64String(text);
						if (array2 != null && this.ViolatesMaxArrayResultLength(array2.Length))
						{
							result2.ErrorOccurred = true;
							result2.Value = null;
							this.RegisterSandboxMaxArrayLengthWarning();
							return result2;
						}
						result2.Value = array2;
						return result2;
					}
					catch (FormatException)
					{
						result2.ErrorOccurred = true;
						this.RegisterInvalidExpressionDataTypeWarning();
						return result2;
					}
				}
				result2.ErrorOccurred = true;
				this.RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private StringResult ProcessAutocastStringResult(VariantResult result)
		{
			return this.ProcessStringResult(result, true);
		}

		private StringResult ProcessStringResult(VariantResult result)
		{
			return this.ProcessStringResult(result, false);
		}

		private StringResult ProcessStringResult(VariantResult result, bool autocast)
		{
			StringResult result2 = default(StringResult);
			bool errorOccurred = default(bool);
			result2.Value = this.ProcessVariantResultToString(result, autocast, Severity.Warning, out errorOccurred);
			result2.ErrorOccurred = errorOccurred;
			result2.FieldStatus = result.FieldStatus;
			return result2;
		}

		private void ProcessLabelResult(ref VariantResult result)
		{
			bool flag = default(bool);
			result.Value = this.ProcessVariantResultToString(result, true, Severity.Error, out flag);
			result.ErrorOccurred = flag;
			if (!flag)
			{
				return;
			}
			throw new ReportProcessingException(this.m_errorContext.Messages);
		}

		private string ProcessVariantResultToString(VariantResult result, bool autocast, Severity severity, out bool errorOccured)
		{
			string text = null;
			if (result.ErrorOccurred)
			{
				errorOccured = true;
			}
			else
			{
				errorOccured = !ReportRuntime.ProcessObjectToString(result.Value, autocast, out text);
				if (errorOccured)
				{
					this.RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, severity);
				}
				else if (this.ViolatesMaxStringResultLength(text))
				{
					this.RegisterSandboxMaxStringLengthWarning();
				}
			}
			return text;
		}

		internal static bool ProcessObjectToString(object value, bool autocast, out string output)
		{
			output = null;
			bool flag = false;
			if (value == null || DBNull.Value == value)
			{
				output = null;
			}
			else if (value is string)
			{
				output = (string)value;
			}
			else if (value is char)
			{
				output = Convert.ToString((char)value, CultureInfo.CurrentCulture);
			}
			else if (value is Guid)
			{
				output = ((Guid)value).ToString();
			}
			else if (autocast)
			{
				output = value.ToString();
			}
			else
			{
				flag = true;
			}
			return !flag;
		}

		private bool EvaluateIntegerExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					this.RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return this.EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private IntegerResult ProcessIntegerResult(VariantResult result)
		{
			IntegerResult result2 = default(IntegerResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
				return result2;
			}
			if (result.Value != null && result.Value != DBNull.Value)
			{
				if (!ReportRuntime.SetVariantType(ref result))
				{
					result2.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
					return result2;
				}
				if (result.TypeCode == TypeCode.Object)
				{
					this.ConvertFromSqlTypes(ref result);
				}
				switch (result.TypeCode)
				{
				case TypeCode.Int32:
					result2.Value = (int)result.Value;
					break;
				case TypeCode.Byte:
					result2.Value = Convert.ToInt32((byte)result.Value);
					break;
				case TypeCode.SByte:
					result2.Value = Convert.ToInt32((sbyte)result.Value);
					break;
				case TypeCode.Int16:
					result2.Value = Convert.ToInt32((short)result.Value);
					break;
				case TypeCode.UInt16:
					result2.Value = Convert.ToInt32((ushort)result.Value);
					break;
				case TypeCode.UInt32:
					try
					{
						result2.Value = Convert.ToInt32((uint)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Int64:
					try
					{
						result2.Value = Convert.ToInt32((long)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.UInt64:
					try
					{
						result2.Value = Convert.ToInt32((ulong)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Double:
					try
					{
						result2.Value = Convert.ToInt32((double)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Single:
					try
					{
						result2.Value = Convert.ToInt32((float)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Decimal:
					try
					{
						result2.Value = Convert.ToInt32((decimal)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				default:
					if (result.Value is TimeSpan)
					{
						try
						{
							result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					}
					result2.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
					break;
				}
				return result2;
			}
			return result2;
		}

		private bool EvaluateIntegerOrFloatExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (expression.ConstantType == DataType.Integer)
				{
					result.Value = expression.IntValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else if (expression.ConstantType == DataType.Float)
				{
					result.Value = expression.FloatValue;
					result.TypeCode = expression.ConstantTypeCode;
				}
				else
				{
					result = default(VariantResult);
					result.ErrorOccurred = true;
					result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					this.RegisterInvalidExpressionDataTypeWarning();
				}
				return true;
			}
			return this.EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		private FloatResult ProcessIntegerOrFloatResult(VariantResult result)
		{
			FloatResult result2 = default(FloatResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
				return result2;
			}
			if (result.Value != null && result.Value != DBNull.Value)
			{
				if (!ReportRuntime.SetVariantType(ref result))
				{
					result2.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
					return result2;
				}
				if (result.TypeCode == TypeCode.Object)
				{
					this.ConvertFromSqlTypes(ref result);
				}
				switch (result.TypeCode)
				{
				case TypeCode.Int32:
					result2.Value = (double)(int)result.Value;
					break;
				case TypeCode.Byte:
					result2.Value = (double)Convert.ToInt32((byte)result.Value);
					break;
				case TypeCode.SByte:
					result2.Value = (double)Convert.ToInt32((sbyte)result.Value);
					break;
				case TypeCode.Int16:
					result2.Value = (double)Convert.ToInt32((short)result.Value);
					break;
				case TypeCode.UInt16:
					result2.Value = (double)Convert.ToInt32((ushort)result.Value);
					break;
				case TypeCode.Double:
					result2.Value = (double)result.Value;
					break;
				case TypeCode.Single:
					result2.Value = Convert.ToDouble((float)result.Value);
					break;
				case TypeCode.UInt32:
					try
					{
						result2.Value = (double)Convert.ToInt32((uint)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Int64:
					try
					{
						result2.Value = (double)Convert.ToInt32((long)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.UInt64:
					try
					{
						result2.Value = (double)Convert.ToInt32((ulong)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				case TypeCode.Decimal:
					try
					{
						result2.Value = Convert.ToDouble((decimal)result.Value);
						return result2;
					}
					catch (OverflowException)
					{
						result2.ErrorOccurred = true;
						return result2;
					}
				default:
					if (result.Value is TimeSpan)
					{
						try
						{
							result2.Value = (double)Convert.ToInt32(((TimeSpan)result.Value).Ticks);
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					}
					result2.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
					break;
				}
				return result2;
			}
			return result2;
		}

		private void ProcessLookupVariantResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, IErrorContext errorContext, bool isArrayRequired, bool normalizeDBNullToNull, ref VariantResult result)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				object value = default(object);
				TypeCode typeCode = default(TypeCode);
				NormalizationCode normalizationCode = default(NormalizationCode);
				if (!this.NormalizeVariantValue(result, isArrayRequired, isArrayRequired, false, normalizeDBNullToNull, out value, out typeCode, out normalizationCode))
				{
					result.ErrorOccurred = true;
					switch (normalizationCode)
					{
					case NormalizationCode.InvalidType:
						result.FieldStatus = DataFieldStatus.UnSupportedDataType;
						errorContext.Register(ProcessingErrorCode.rsLookupOfInvalidExpressionDataType, Severity.Warning);
						break;
					case NormalizationCode.StringLengthViolation:
						errorContext.Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, Convert.ToString(this.m_maxStringResultLength, CultureInfo.InvariantCulture));
						break;
					case NormalizationCode.ArrayLengthViolation:
						errorContext.Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, Convert.ToString(this.m_maxArrayResultLength, CultureInfo.InvariantCulture));
						break;
					}
				}
				result.Value = value;
				result.TypeCode = typeCode;
			}
		}

		private bool NormalizeVariantValue(VariantResult result, bool isArrayAllowed, bool isArrayRequired, bool isByteArrayAllowed, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			if (!isArrayRequired && ReportRuntime.GetVariantTypeCode(result.Value, out typeCode))
			{
				if (typeCode == TypeCode.String && this.ViolatesMaxStringResultLength((string)result.Value))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.StringLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
				goto IL_01e7;
			}
			if (!isArrayRequired && (result.Value == null || result.Value == DBNull.Value))
			{
				if (normalizeDBNullToNull)
				{
					normalizedValue = null;
				}
				else
				{
					normalizedValue = DBNull.Value;
				}
				typeCode = TypeCode.Empty;
				goto IL_01e7;
			}
			if (isArrayAllowed && result.Value is object[])
			{
				object[] array = (object[])result.Value;
				if (this.ViolatesMaxArrayResultLength(array.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				if (!this.NormalizeVariantArray(array.GetEnumerator(), array, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
				goto IL_01e7;
			}
			if (!isArrayRequired && isByteArrayAllowed && result.Value is byte[])
			{
				byte[] array2 = (byte[])result.Value;
				if (this.ViolatesMaxArrayResultLength(array2.Length))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				normalizedValue = result.Value;
				typeCode = TypeCode.Object;
				goto IL_01e7;
			}
			if (isArrayAllowed && result.Value is IList)
			{
				IList list = (IList)result.Value;
				if (this.ViolatesMaxArrayResultLength(list.Count))
				{
					normalizedValue = null;
					typeCode = TypeCode.Empty;
					normalCode = NormalizationCode.ArrayLengthViolation;
					return false;
				}
				object[] destArr = new object[list.Count];
				if (!this.NormalizeVariantArray(list.GetEnumerator(), destArr, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
				goto IL_01e7;
			}
			if (!isArrayRequired && result.Value is Guid)
			{
				normalizedValue = ((Guid)result.Value).ToString();
				typeCode = TypeCode.String;
				goto IL_01e7;
			}
			if (result.Value != null && this.ConvertFromSqlTypes(ref result))
			{
				typeCode = TypeCode.Object;
				normalizedValue = result.Value;
				goto IL_01e7;
			}
			typeCode = TypeCode.Empty;
			normalizedValue = null;
			normalCode = NormalizationCode.InvalidType;
			return false;
			IL_01e7:
			normalCode = NormalizationCode.Success;
			return true;
		}

		private bool NormalizeVariantArray(IEnumerator source, object[] destArr, bool normalizeDBNullToNull, out object normalizedValue, out TypeCode typeCode, out NormalizationCode normalCode)
		{
			int num = 0;
			while (source.MoveNext())
			{
				VariantResult result = default(VariantResult);
				result.Value = source.Current;
				if (!this.NormalizeVariantValue(result, false, false, false, normalizeDBNullToNull, out normalizedValue, out typeCode, out normalCode))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			typeCode = TypeCode.Object;
			normalCode = NormalizationCode.Success;
			return true;
		}

		private void ProcessReportParameterVariantResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				throw new ReportProcessingException(this.m_errorContext.Messages);
			}
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				object value = default(object);
				TypeCode typeCode = default(TypeCode);
				NormalizationCode normalizationCode = default(NormalizationCode);
				if (!this.NormalizeVariantValue(result, true, false, false, true, out value, out typeCode, out normalizationCode))
				{
					result.ErrorOccurred = true;
					switch (normalizationCode)
					{
					case NormalizationCode.InvalidType:
						this.RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error);
						break;
					case NormalizationCode.StringLengthViolation:
						this.RegisterSandboxMaxStringLengthWarning();
						break;
					case NormalizationCode.ArrayLengthViolation:
						this.RegisterSandboxMaxArrayLengthWarning();
						break;
					}
				}
				result.Value = value;
				result.TypeCode = typeCode;
			}
		}

		private bool ViolatesMaxStringResultLength(string value)
		{
			if (this.m_maxStringResultLength != -1 && value != null)
			{
				return value.Length > this.m_maxStringResultLength;
			}
			return false;
		}

		private bool ViolatesMaxArrayResultLength(int count)
		{
			if (this.m_maxArrayResultLength != -1)
			{
				return count > this.m_maxArrayResultLength;
			}
			return false;
		}

		private void ProcessSerializableResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool isReportScope, ref VariantResult result)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				this.ProcessSerializableResult(isReportScope, ref result);
			}
		}

		internal bool ProcessSerializableResult(bool isReportScope, ref VariantResult result)
		{
			bool result2 = false;
			object value = default(object);
			TypeCode typeCode = default(TypeCode);
			NormalizationCode normalizationCode = default(NormalizationCode);
			if (!this.NormalizeVariantValue(result, true, false, !this.m_rdlSandboxingEnabled, true, out value, out typeCode, out normalizationCode))
			{
				result.ErrorOccurred = true;
				switch (normalizationCode)
				{
				case NormalizationCode.InvalidType:
					if (isReportScope)
					{
						try
						{
							if (result.Value is ISerializable || (result.Value.GetType().Attributes & TypeAttributes.Serializable) != 0)
							{
								result.TypeCode = TypeCode.Object;
								if (this.m_isSerializableValuesProhibited)
								{
									((IErrorContext)this).Register(ProcessingErrorCode.rsSerializableTypeNotSupported, Severity.Error, new string[2]
									{
										this.m_objectType.ToString(),
										this.m_objectName
									});
									result.Value = null;
									return false;
								}
								if (!this.m_rdlSandboxingEnabled)
								{
									result.ErrorOccurred = false;
									return false;
								}
							}
							else
							{
								result2 = true;
								result.Value = null;
							}
						}
						catch (Exception ex)
						{
							((IErrorContext)this).Register(ProcessingErrorCode.rsUnexpectedSerializationError, Severity.Warning, new string[1]
							{
								ex.Message
							});
							result.Value = null;
						}
					}
					else
					{
						result.Value = null;
					}
					this.RegisterInvalidExpressionDataTypeWarning();
					break;
				case NormalizationCode.StringLengthViolation:
					this.RegisterSandboxMaxStringLengthWarning();
					break;
				case NormalizationCode.ArrayLengthViolation:
					this.RegisterSandboxMaxArrayLengthWarning();
					break;
				}
			}
			else
			{
				result.Value = value;
				result.TypeCode = typeCode;
			}
			return result2;
		}

		private void ProcessVariantResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result)
		{
			this.ProcessVariantResult(expression, ref result, false);
		}

		private void ProcessVariantResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isArrayAllowed)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				object value = default(object);
				TypeCode typeCode = default(TypeCode);
				NormalizationCode normalizationCode = default(NormalizationCode);
				if (!this.NormalizeVariantValue(result, isArrayAllowed, false, false, true, out value, out typeCode, out normalizationCode))
				{
					result.ErrorOccurred = true;
					switch (normalizationCode)
					{
					case NormalizationCode.InvalidType:
						this.RegisterInvalidExpressionDataTypeWarning();
						break;
					case NormalizationCode.StringLengthViolation:
						this.RegisterSandboxMaxStringLengthWarning();
						break;
					case NormalizationCode.ArrayLengthViolation:
						this.RegisterSandboxMaxArrayLengthWarning();
						break;
					}
				}
				result.Value = value;
				result.TypeCode = typeCode;
			}
		}

		private void VerifyVariantResultAndStopOnError(ref VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				if (result.FieldStatus != 0)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, new string[1]
					{
						ReportRuntime.GetErrorName(result.FieldStatus, result.ExceptionMessage)
					});
				}
				else
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, new string[0]);
				}
				throw new ReportProcessingException(this.m_errorContext.Messages);
			}
			if (result.Value == null)
			{
				result.Value = DBNull.Value;
			}
		}

		private void ProcessVariantOrBinaryResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ref VariantResult result, bool isAggregate, bool allowArray)
		{
			if (expression != null && expression.IsExpression && !result.ErrorOccurred)
			{
				object value = default(object);
				TypeCode typeCode = default(TypeCode);
				NormalizationCode normalizationCode = default(NormalizationCode);
				if (!this.NormalizeVariantValue(result, allowArray, false, true, true, out value, out typeCode, out normalizationCode))
				{
					result.ErrorOccurred = true;
					if (isAggregate)
					{
						result.FieldStatus = DataFieldStatus.UnSupportedDataType;
					}
					else
					{
						switch (normalizationCode)
						{
						case NormalizationCode.InvalidType:
							this.RegisterInvalidExpressionDataTypeWarning();
							break;
						case NormalizationCode.StringLengthViolation:
							this.RegisterSandboxMaxStringLengthWarning();
							break;
						case NormalizationCode.ArrayLengthViolation:
							this.RegisterSandboxMaxArrayLengthWarning();
							break;
						}
					}
				}
				result.Value = value;
				result.TypeCode = typeCode;
			}
		}

		private ParameterValueResult ProcessParameterValueResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string paramName, VariantResult result)
		{
			ParameterValueResult result2 = default(ParameterValueResult);
			if (expression != null)
			{
				if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					result2.Value = result.Value;
					result2.Type = expression.ConstantType;
				}
				else if (result.ErrorOccurred)
				{
					result2.ErrorOccurred = true;
				}
				else
				{
					object value = default(object);
					DataType type = default(DataType);
					if (!this.NormalizeParameterVariantValue(result.Value, paramName, out value, out type))
					{
						Type type2 = result.Value.GetType();
						TypeCode typeCode = Type.GetTypeCode(type2);
						if (typeCode == TypeCode.Object && !this.ConvertFromSqlTypes(ref result))
						{
							result2.ErrorOccurred = true;
						}
					}
					result2.Value = value;
					result2.Type = type;
				}
			}
			return result2;
		}

		private bool NormalizeParameterVariantValue(object value, string paramName, out object normalizedValue, out DataType dataType)
		{
			if (value == null || value == DBNull.Value)
			{
				normalizedValue = null;
				dataType = DataType.String;
			}
			else if (value is bool)
			{
				normalizedValue = value;
				dataType = DataType.Boolean;
			}
			else if (value is DateTime)
			{
				normalizedValue = value;
				dataType = DataType.DateTime;
			}
			else if (value is double || value is float || value is decimal)
			{
				normalizedValue = Convert.ToDouble(value, CultureInfo.CurrentCulture);
				dataType = DataType.Float;
			}
			else if (value is string)
			{
				string text = (string)value;
				if (this.ViolatesMaxStringResultLength(text))
				{
					dataType = DataType.String;
					normalizedValue = null;
					this.RegisterSandboxMaxStringLengthWarning();
					return false;
				}
				normalizedValue = text;
				dataType = DataType.String;
			}
			else if (value is char)
			{
				normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
				dataType = DataType.String;
			}
			else if (value is int || value is short || value is byte || value is sbyte || value is ushort)
			{
				normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
				dataType = DataType.Integer;
			}
			else
			{
				if (!(value is uint) && !(value is long) && !(value is ulong))
				{
					if (value is TimeSpan)
					{
						try
						{
							normalizedValue = Convert.ToString(value, CultureInfo.CurrentCulture);
							dataType = DataType.String;
						}
						catch (FormatException)
						{
							this.RegisterInvalidExpressionDataTypeWarning();
							normalizedValue = null;
							dataType = DataType.String;
							return false;
						}
						goto IL_0284;
					}
					if (value is DateTimeOffset)
					{
						normalizedValue = value;
						dataType = DataType.DateTime;
						goto IL_0284;
					}
					if (value is Guid)
					{
						normalizedValue = ((Guid)value).ToString();
						dataType = DataType.String;
						goto IL_0284;
					}
					if (value is object[])
					{
						object[] array = (object[])value;
						if (this.ViolatesMaxArrayResultLength(array.Length))
						{
							normalizedValue = null;
							dataType = DataType.String;
							return false;
						}
						if (!this.NormalizeParameterVariantArray(array.GetEnumerator(), array, paramName, out normalizedValue, out dataType))
						{
							return false;
						}
						goto IL_0284;
					}
					if (value is IList)
					{
						IList list = (IList)value;
						if (this.ViolatesMaxArrayResultLength(list.Count))
						{
							normalizedValue = null;
							dataType = DataType.String;
							return false;
						}
						object[] destArr = new object[list.Count];
						if (!this.NormalizeParameterVariantArray(list.GetEnumerator(), destArr, paramName, out normalizedValue, out dataType))
						{
							return false;
						}
						goto IL_0284;
					}
					this.RegisterInvalidExpressionDataTypeWarning();
					normalizedValue = null;
					dataType = DataType.String;
					return false;
				}
				try
				{
					normalizedValue = Convert.ToInt32(value, CultureInfo.CurrentCulture);
					dataType = DataType.Integer;
				}
				catch (OverflowException)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsParameterValueCastFailure, Severity.Warning, this.m_objectType, this.m_objectName, this.m_propertyName, new string[1]
					{
						paramName
					});
					normalizedValue = value;
					dataType = DataType.Integer;
					return false;
				}
			}
			goto IL_0284;
			IL_0284:
			return true;
		}

		private bool NormalizeParameterVariantArray(IEnumerator source, object[] destArr, string paramName, out object normalizedValue, out DataType dataType)
		{
			dataType = DataType.String;
			int num = 0;
			while (source.MoveNext())
			{
				if (!this.NormalizeParameterVariantValue(source.Current, paramName, out normalizedValue, out dataType))
				{
					return false;
				}
				destArr[num] = normalizedValue;
				num++;
			}
			normalizedValue = destArr;
			return true;
		}

		private static object[] GetAsObjectArray(ref VariantResult result)
		{
			object[] array = result.Value as object[];
			if (array == null)
			{
				IList list = result.Value as IList;
				if (list != null)
				{
					array = new object[list.Count];
					list.CopyTo(array, 0);
				}
			}
			return array;
		}

		private DataType GetDataType(object obj)
		{
			TypeCode typeCode = TypeCode.Empty;
			if (obj != null)
			{
				Type type = obj.GetType();
				typeCode = Type.GetTypeCode(type);
			}
			switch (typeCode)
			{
			case TypeCode.Boolean:
				return DataType.Boolean;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return DataType.Float;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return DataType.Integer;
			case TypeCode.DateTime:
				return DataType.DateTime;
			case TypeCode.Empty:
			case TypeCode.DBNull:
			case TypeCode.Char:
			case TypeCode.String:
				return DataType.String;
			default:
				if (obj is TimeSpan)
				{
					return DataType.Integer;
				}
				if (obj is DateTimeOffset)
				{
					return DataType.DateTime;
				}
				return DataType.String;
			}
		}

		private void SetNullResult(ref VariantResult result)
		{
			result.Value = null;
			result.TypeCode = TypeCode.Empty;
		}

		private void SetGuidResult(ref VariantResult result)
		{
			result.Value = ((Guid)result.Value).ToString();
			result.TypeCode = TypeCode.String;
		}

		private bool ConvertFromSqlTypes(ref VariantResult result)
		{
			if (result.Value is SqlInt32)
			{
				if (((SqlInt32)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int32;
					result.Value = ((SqlInt32)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlInt16)
			{
				if (((SqlInt16)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int16;
					result.Value = ((SqlInt16)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlInt64)
			{
				if (((SqlInt64)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Int64;
					result.Value = ((SqlInt64)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlBoolean)
			{
				if (((SqlBoolean)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Boolean;
					result.Value = ((SqlBoolean)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlDecimal)
			{
				if (((SqlDecimal)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlDecimal)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlDouble)
			{
				if (((SqlDouble)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Double;
					result.Value = ((SqlDouble)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlDateTime)
			{
				if (((SqlDateTime)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.DateTime;
					result.Value = ((SqlDateTime)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlMoney)
			{
				if (((SqlMoney)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Decimal;
					result.Value = ((SqlMoney)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlSingle)
			{
				if (((SqlSingle)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Single;
					result.Value = ((SqlSingle)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlByte)
			{
				if (((SqlByte)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.Byte;
					result.Value = ((SqlByte)result.Value).Value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlString)
			{
				if (((SqlString)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					result.TypeCode = TypeCode.String;
					string value = ((SqlString)result.Value).Value;
					if (this.ViolatesMaxStringResultLength(value))
					{
						value = null;
						this.RegisterSandboxMaxStringLengthWarning();
					}
					result.Value = value;
				}
				goto IL_0422;
			}
			if (result.Value is SqlGuid)
			{
				if (((SqlGuid)result.Value).IsNull)
				{
					this.SetNullResult(ref result);
				}
				else
				{
					this.SetGuidResult(ref result);
				}
				goto IL_0422;
			}
			return false;
			IL_0422:
			return true;
		}

		private bool EvaluateNoConstantExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
			{
				result = new VariantResult(true, null);
				this.RegisterInvalidExpressionDataTypeWarning();
				return true;
			}
			return this.EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		internal static bool GetVariantTypeCode(object o, out TypeCode typeCode)
		{
            if (o == null)
            {
                typeCode = TypeCode.Empty;
            }
            else
            {
                Type type = o.GetType();
                typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        return false;
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        return true;
                }
                if (o is TimeSpan || o is DateTimeOffset)
                {
                    return true;
                }
                //if (o is SqlGeography || o is SqlGeometry)
                //{
                //    return true;
                //}
            }
            return false;
        }

		internal static bool SetVariantType(ref VariantResult result)
		{
			TypeCode typeCode = default(TypeCode);
			if (ReportRuntime.GetVariantTypeCode(result.Value, out typeCode))
			{
				result.TypeCode = typeCode;
				return true;
			}
			return false;
		}

		internal static string ConvertToStringFallBack(object value)
		{
			try
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}", value);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void RegisterInvalidExpressionDataTypeWarning()
		{
			this.RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning);
		}

		private void RegisterInvalidExpressionDataTypeWarning(ProcessingErrorCode errorCode, Severity severity)
		{
			((IErrorContext)this).Register(errorCode, severity, new string[0]);
		}

		private void RegisterSandboxMaxStringLengthWarning()
		{
			this.ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(this.m_maxStringResultLength, CultureInfo.InvariantCulture)
			});
		}

		private void RegisterSandboxMaxArrayLengthWarning()
		{
			this.ReportObjectModel.OdpContext.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength);
			((IErrorContext)this).Register(ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength, Severity.Warning, new string[1]
			{
				Convert.ToString(this.m_maxArrayResultLength, CultureInfo.InvariantCulture)
			});
		}

		internal object MinValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (this.CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) > 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		internal object MaxValue(params object[] arguments)
		{
			if (arguments != null)
			{
				object obj = arguments[0];
				for (int i = 1; i < arguments.Length; i++)
				{
					if (this.CompareWithExtendedTypesAndStopOnError(obj, arguments[i]) < 0)
					{
						obj = arguments[i];
					}
				}
				return obj;
			}
			return null;
		}

		private int CompareWithExtendedTypesAndStopOnError(object x, object y)
		{
			return this.m_reportObjectModel.OdpContext.CompareAndStopOnError(x, y, this.m_objectType, this.m_objectName, this.m_propertyName, true);
		}

		internal int RecursiveLevel(string scope)
		{
			if (this.m_currentScope == null)
			{
				return 0;
			}
			int num = this.m_currentScope.RecursiveLevel(scope);
			if (-1 == num)
			{
				return 0;
			}
			return num;
		}

		internal void LoadCompiledCode(IExpressionHostAssemblyHolder expressionHostAssemblyHolder, bool includeParameters, bool parametersOnly, AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel, ReportRuntimeSetup runtimeSetup)
		{
			if (expressionHostAssemblyHolder.CompiledCode != null && expressionHostAssemblyHolder.CompiledCode.Length > 0)
			{
				try
				{
					if (runtimeSetup.RequireExpressionHostWithRefusedPermissions && !expressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions)
					{
						if (Global.Tracer.TraceError)
						{
							Global.Tracer.Trace("Expression host generated with refused permissions is required.");
						}
						throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
					}
					if (runtimeSetup.ExprHostAppDomain == null || runtimeSetup.ExprHostAppDomain == AppDomain.CurrentDomain)
					{
						this.m_exprHostInSandboxAppDomain = false;
						if (expressionHostAssemblyHolder.CodeModules != null)
						{
							for (int i = 0; i < expressionHostAssemblyHolder.CodeModules.Count; i++)
							{
								if (!runtimeSetup.CheckCodeModuleIsTrustedInCurrentAppDomain(expressionHostAssemblyHolder.CodeModules[i]))
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsUntrustedCodeModule, Severity.Error, expressionHostAssemblyHolder.ObjectType, null, null, expressionHostAssemblyHolder.CodeModules[i]);
									throw new ReportProcessingException(this.m_errorContext.Messages);
								}
							}
						}
						this.m_reportExprHost = ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(expressionHostAssemblyHolder.CompiledCode, "ExpressionHost", runtimeSetup.ExprHostEvidence, includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules);
					}
					else
					{
						this.m_exprHostInSandboxAppDomain = true;
						this.m_reportExprHost = ExpressionHostLoader.LoadExprHost(expressionHostAssemblyHolder.CompiledCode, "ExpressionHost", includeParameters, parametersOnly, reportObjectModel, expressionHostAssemblyHolder.CodeModules, runtimeSetup.ExprHostAppDomain);
					}
				}
				catch (ReportProcessingException)
				{
					throw;
				}
				catch (Exception e)
				{
					this.ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorLoadingExprHostAssembly);
				}
			}
		}

		internal void CustomCodeOnInit(IExpressionHostAssemblyHolder expressionHostAssemblyHolder)
		{
			if (expressionHostAssemblyHolder.CompiledCode.Length > 0)
			{
				try
				{
					this.m_reportExprHost.CustomCodeOnInit();
				}
				catch (ReportProcessingException)
				{
					throw;
				}
				catch (Exception e)
				{
					this.ProcessLoadingExprHostException(expressionHostAssemblyHolder.ObjectType, e, ProcessingErrorCode.rsErrorInOnInit);
				}
			}
		}

		private void ProcessLoadingExprHostException(AspNetCore.ReportingServices.ReportProcessing.ObjectType assemblyHolderObjectType, Exception e, ProcessingErrorCode errorCode)
		{
			if (e != null && e is TargetInvocationException && e.InnerException != null)
			{
				e = e.InnerException;
			}
			string text = null;
			string text2;
			if (e != null)
			{
				try
				{
					text2 = e.Message;
					text = e.ToString();
				}
				catch
				{
					text2 = RPRes.NonClsCompliantException;
				}
			}
			else
			{
				text2 = RPRes.NonClsCompliantException;
			}
			ProcessingMessage processingMessage = this.m_errorContext.Register(errorCode, Severity.Error, assemblyHolderObjectType, null, null, text2);
			if (Global.Tracer.TraceError && processingMessage != null)
			{
				Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + (text ?? ""));
			}
			throw new ReportProcessingException(this.m_errorContext.Messages);
		}

		internal void Close()
		{
			this.m_reportExprHost = null;
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportRuntime;
		}

		public void SetID(int id)
		{
			this.m_id = id;
		}

		internal string EvaluateBaseGaugeImageSourceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(baseGaugeImage.Source, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(baseGaugeImage.Source, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageStringValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(baseGaugeImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = this.ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal byte[] EvaluateBaseGaugeImageBinaryValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBinaryExpression(baseGaugeImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(baseGaugeImage.Value, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = this.ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateBaseGaugeImageMIMETypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(baseGaugeImage.MIMEType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MIMEType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(baseGaugeImage.MIMEType, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateBaseGaugeImageTransparentColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage baseGaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(baseGaugeImage.TransparentColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(baseGaugeImage.TransparentColor, ref result, baseGaugeImage.ExprHost))
					{
						Global.Tracer.Assert(baseGaugeImage.ExprHost != null, "(baseGaugeImage.ExprHost != null)");
						result.Value = baseGaugeImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateGaugeImageSourceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeImage.Source, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Source", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeImage.Source, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).SourceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateGaugeImageValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeImage.Value, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(gaugeImage.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeImageTransparentColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeImage.TransparentColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransparentColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeImage.TransparentColor, ref result, gaugeImage.ExprHost))
					{
						Global.Tracer.Assert(gaugeImage.ExprHost != null, "(gaugeImage.ExprHost != null)");
						result.Value = ((GaugeImageExprHost)gaugeImage.ExprHost).TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateCapImageHueColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(capImage.HueColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(capImage.HueColor, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateCapImageOffsetXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(capImage.OffsetX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(capImage.OffsetX, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateCapImageOffsetYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage capImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(capImage.OffsetY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(capImage.OffsetY, ref result, capImage.ExprHost))
					{
						Global.Tracer.Assert(capImage.ExprHost != null, "(capImage.ExprHost != null)");
						result.Value = ((CapImageExprHost)capImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateFrameImageHueColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(frameImage.HueColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(frameImage.HueColor, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal double EvaluateFrameImageTransparencyExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(frameImage.Transparency, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(frameImage.Transparency, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateFrameImageClipImageExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage frameImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(frameImage.ClipImage, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipImage", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(frameImage.ClipImage, ref result, frameImage.ExprHost))
					{
						Global.Tracer.Assert(frameImage.ExprHost != null, "(frameImage.ExprHost != null)");
						result.Value = ((FrameImageExprHost)frameImage.ExprHost).ClipImageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateTopImageHueColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage topImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(topImage.HueColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(topImage.HueColor, ref result, topImage.ExprHost))
					{
						Global.Tracer.Assert(topImage.ExprHost != null, "(topImage.ExprHost != null)");
						result.Value = ((TopImageExprHost)topImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateBackFrameFrameStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(backFrame.FrameStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(backFrame.FrameStyle, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateBackFrameFrameShapeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(backFrame.FrameShape, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameShape", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(backFrame.FrameShape, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameShapeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateBackFrameFrameWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(backFrame.FrameWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FrameWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(backFrame.FrameWidth, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.FrameWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateBackFrameGlassEffectExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame backFrame, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(backFrame.GlassEffect, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GlassEffect", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(backFrame.GlassEffect, ref result, backFrame.ExprHost))
					{
						Global.Tracer.Assert(backFrame.ExprHost != null, "(backFrame.ExprHost != null)");
						result.Value = backFrame.ExprHost.GlassEffectExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePanelAntiAliasingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanel.AntiAliasing, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AntiAliasing", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanel.AntiAliasing, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePanelAutoLayoutExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanel.AutoLayout, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AutoLayout", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanel.AutoLayout, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.AutoLayoutExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePanelShadowIntensityExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanel.ShadowIntensity, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShadowIntensity", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanel.ShadowIntensity, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePanelTextAntiAliasingQualityExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanel.TextAntiAliasingQuality, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextAntiAliasingQuality", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanel.TextAntiAliasingQuality, ref result, gaugePanel.GaugePanelExprHost))
					{
						Global.Tracer.Assert(gaugePanel.GaugePanelExprHost != null, "(gaugePanel.GaugePanelExprHost != null)");
						result.Value = gaugePanel.GaugePanelExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePanelItemTopExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.Top, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Top", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.Top, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemLeftExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.Left, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Left", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.Left, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemHeightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.Height, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Height", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.Height, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePanelItemWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.Width, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal int EvaluateGaugePanelItemZIndexExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.ZIndex, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ZIndex", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.ZIndex, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal bool EvaluateGaugePanelItemHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.Hidden, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugePanelItemToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem gaugePanelItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePanelItem.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePanelItem.ToolTip, ref result, gaugePanelItem.ExprHost))
					{
						Global.Tracer.Assert(gaugePanelItem.ExprHost != null, "(gaugePanelItem.ExprHost != null)");
						result.Value = gaugePanelItem.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerBarStartExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.BarStart, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BarStart", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.BarStart, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.BarStartExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugePointerDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.DistanceFromScale, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugePointerMarkerLengthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.MarkerLength, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerLength", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.MarkerLength, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerLengthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerMarkerStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.MarkerStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MarkerStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.MarkerStyle, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.MarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugePointerPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.Placement, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerSnappingEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.SnappingEnabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.SnappingEnabled, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerSnappingIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.SnappingInterval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.SnappingInterval, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugePointerToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.ToolTip, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugePointerHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.Hidden, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugePointerWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer gaugePointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugePointer.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugePointer.Width, ref result, gaugePointer.ExprHost))
					{
						Global.Tracer.Assert(gaugePointer.ExprHost != null, "(gaugePointer.ExprHost != null)");
						result.Value = gaugePointer.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Interval, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.IntervalOffset, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleLogarithmicExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Logarithmic, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Logarithmic", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Logarithmic, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleLogarithmicBaseExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.LogarithmicBase, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LogarithmicBase", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.LogarithmicBase, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.LogarithmicBaseExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeScaleMultiplierExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Multiplier, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Multiplier, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateGaugeScaleReversedExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Reversed, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reversed", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Reversed, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ReversedExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeScaleTickMarksOnTopExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.TickMarksOnTop, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TickMarksOnTop", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.TickMarksOnTop, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.TickMarksOnTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateGaugeScaleToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.ToolTip, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateGaugeScaleHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Hidden, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeScaleWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale gaugeScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeScale.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeScale.Width, ref result, gaugeScale.ExprHost))
					{
						Global.Tracer.Assert(gaugeScale.ExprHost != null, "(gaugeScale.ExprHost != null)");
						result.Value = gaugeScale.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeTickMarks.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeTickMarks.Interval, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeTickMarksIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeTickMarks.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeTickMarks.IntervalOffset, ref result, gaugeTickMarks.ExprHost))
					{
						Global.Tracer.Assert(gaugeTickMarks.ExprHost != null, "(gaugeTickMarks.ExprHost != null)");
						result.Value = ((GaugeTickMarksExprHost)gaugeTickMarks.ExprHost).IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateLinearGaugeOrientationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(linearGauge.Orientation, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Orientation", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(linearGauge.Orientation, ref result, linearGauge.ExprHost))
					{
						Global.Tracer.Assert(linearGauge.ExprHost != null, "(linearGauge.ExprHost != null)");
						result.Value = ((LinearGaugeExprHost)linearGauge.ExprHost).OrientationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateLinearPointerTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(linearPointer.Type, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(linearPointer.Type, ref result, linearPointer.ExprHost))
					{
						Global.Tracer.Assert(linearPointer.ExprHost != null, "(linearPointer.ExprHost != null)");
						result.Value = ((LinearPointerExprHost)linearPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateLinearScaleStartMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(linearScale.StartMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(linearScale.StartMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).StartMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScaleEndMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(linearScale.EndMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(linearScale.EndMargin, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).EndMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateLinearScalePositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale linearScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(linearScale.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Position", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(linearScale.Position, ref result, linearScale.ExprHost))
					{
						Global.Tracer.Assert(linearScale.ExprHost != null, "(linearScale.ExprHost != null)");
						result.Value = ((LinearScaleExprHost)linearScale.ExprHost).PositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.Text, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.Text, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelAllowUpsideDownExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.AllowUpsideDown, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.AllowUpsideDown, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePinLabelDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.DistanceFromScale, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluatePinLabelFontAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.FontAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.FontAngle, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePinLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.Placement, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluatePinLabelRotateLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.RotateLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.RotateLabel, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePinLabelUseFontPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pinLabel.UseFontPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pinLabel.UseFontPercent, ref result, pinLabel.ExprHost))
					{
						Global.Tracer.Assert(pinLabel.ExprHost != null, "(pinLabel.ExprHost != null)");
						result.Value = pinLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapOnTopExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerCap.OnTop, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OnTop", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerCap.OnTop, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.OnTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluatePointerCapReflectionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerCap.Reflection, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Reflection", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerCap.Reflection, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.ReflectionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluatePointerCapCapStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerCap.CapStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "CapStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerCap.CapStyle, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.CapStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluatePointerCapHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerCap.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerCap.Hidden, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluatePointerCapWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerCap.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerCap.Width, ref result, pointerCap.ExprHost))
					{
						Global.Tracer.Assert(pointerCap.ExprHost != null, "(pointerCap.ExprHost != null)");
						result.Value = pointerCap.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageHueColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerImage.HueColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerImage.HueColor, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).HueColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal double EvaluatePointerImageTransparencyExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerImage.Transparency, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerImage.Transparency, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluatePointerImageOffsetXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerImage.OffsetX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerImage.OffsetX, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluatePointerImageOffsetYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(pointerImage.OffsetY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OffsetY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(pointerImage.OffsetY, ref result, pointerImage.ExprHost))
					{
						Global.Tracer.Assert(pointerImage.ExprHost != null, "(pointerImage.ExprHost != null)");
						result.Value = ((PointerImageExprHost)pointerImage.ExprHost).OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal double EvaluateRadialGaugePivotXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialGauge.PivotX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialGauge.PivotX, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialGaugePivotYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialGauge.PivotY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "PivotY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialGauge.PivotY, ref result, radialGauge.ExprHost))
					{
						Global.Tracer.Assert(radialGauge.ExprHost != null, "(radialGauge.ExprHost != null)");
						result.Value = ((RadialGaugeExprHost)radialGauge.ExprHost).PivotYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateRadialPointerTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialPointer.Type, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Type", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialPointer.Type, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).TypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateRadialPointerNeedleStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialPointer.NeedleStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NeedleStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialPointer.NeedleStyle, ref result, radialPointer.ExprHost))
					{
						Global.Tracer.Assert(radialPointer.ExprHost != null, "(radialPointer.ExprHost != null)");
						result.Value = ((RadialPointerExprHost)radialPointer.ExprHost).NeedleStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateRadialScaleRadiusExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialScale.Radius, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Radius", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialScale.Radius, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).RadiusExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleStartAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialScale.StartAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialScale.StartAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).StartAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateRadialScaleSweepAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale radialScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(radialScale.SweepAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SweepAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(radialScale.SweepAngle, ref result, radialScale.ExprHost))
					{
						Global.Tracer.Assert(radialScale.ExprHost != null, "(radialScale.ExprHost != null)");
						result.Value = ((RadialScaleExprHost)radialScale.ExprHost).SweepAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.Interval, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsIntervalOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.IntervalOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IntervalOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.IntervalOffset, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.IntervalOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScaleLabelsAllowUpsideDownExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.AllowUpsideDown, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.AllowUpsideDown, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleLabelsDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.DistanceFromScale, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleLabelsFontAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.FontAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.FontAngle, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleLabelsPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.Placement, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleLabelsRotateLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.RotateLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.RotateLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.RotateLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsShowEndLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.ShowEndLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowEndLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.ShowEndLabels, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.ShowEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.Hidden, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateScaleLabelsUseFontPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleLabels.UseFontPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleLabels.UseFontPercent, ref result, scaleLabels.ExprHost))
					{
						Global.Tracer.Assert(scaleLabels.ExprHost != null, "(scaleLabels.ExprHost != null)");
						result.Value = scaleLabels.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScalePinLocationExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scalePin.Location, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Location", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scalePin.Location, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).LocationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateScalePinEnableExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin scalePin, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scalePin.Enable, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Enable", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scalePin.Enable, ref result, scalePin.ExprHost))
					{
						Global.Tracer.Assert(scalePin.ExprHost != null, "(scalePin.ExprHost != null)");
						result.Value = ((ScalePinExprHost)scalePin.ExprHost).EnableExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateScaleRangeDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.DistanceFromScale, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeStartWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.StartWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "StartWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.StartWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.StartWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateScaleRangeEndWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.EndWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EndWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.EndWidth, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.EndWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateScaleRangeInRangeBarPointerColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.InRangeBarPointerColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeBarPointerColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.InRangeBarPointerColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeBarPointerColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateScaleRangeInRangeLabelColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.InRangeLabelColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeLabelColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.InRangeLabelColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeLabelColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateScaleRangeInRangeTickMarksColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.InRangeTickMarksColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "InRangeTickMarksColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.InRangeTickMarksColor, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.InRangeTickMarksColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateScaleRangePlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.Placement, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateScaleRangeToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.ToolTip, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.ToolTipExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateScaleRangeHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.Hidden, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateScaleRangeBackgroundGradientTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(scaleRange.BackgroundGradientType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BackgroundGradientType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(scaleRange.BackgroundGradientType, ref result, scaleRange.ExprHost))
					{
						Global.Tracer.Assert(scaleRange.ExprHost != null, "(scaleRange.ExprHost != null)");
						result.Value = scaleRange.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateThermometerBulbOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(thermometer.BulbOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(thermometer.BulbOffset, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateThermometerBulbSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(thermometer.BulbSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "BulbSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(thermometer.BulbSize, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.BulbSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateThermometerThermometerStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer thermometer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(thermometer.ThermometerStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ThermometerStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(thermometer.ThermometerStyle, ref result, thermometer.ExprHost))
					{
						Global.Tracer.Assert(thermometer.ExprHost != null, "(thermometer.ExprHost != null)");
						result.Value = thermometer.ExprHost.ThermometerStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateTickMarkStyleDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.DistanceFromScale, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStylePlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.Placement, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleEnableGradientExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.EnableGradient, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "EnableGradient", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.EnableGradient, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.EnableGradientExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateTickMarkStyleGradientDensityExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.GradientDensity, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "GradientDensity", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.GradientDensity, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.GradientDensityExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleLengthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.Length, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Length", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.Length, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.LengthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateTickMarkStyleWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.Width, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateTickMarkStyleShapeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.Shape, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Shape", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.Shape, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.ShapeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateTickMarkStyleHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(tickMarkStyle.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(tickMarkStyle.Hidden, ref result, tickMarkStyle.ExprHost))
					{
						Global.Tracer.Assert(tickMarkStyle.ExprHost != null, "(tickMarkStyle.ExprHost != null)");
						result.Value = tickMarkStyle.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateCustomLabelTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.Text, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.Text, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.TextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(customLabel.Text, ref result);
			return result;
		}

		internal bool EvaluateCustomLabelAllowUpsideDownExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.AllowUpsideDown, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AllowUpsideDown", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.AllowUpsideDown, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.AllowUpsideDownExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelDistanceFromScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.DistanceFromScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DistanceFromScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.DistanceFromScale, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.DistanceFromScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateCustomLabelFontAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.FontAngle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "FontAngle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.FontAngle, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.FontAngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateCustomLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.Placement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Placement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.Placement, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.PlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateCustomLabelRotateLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.RotateLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "RotateLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.RotateLabel, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.RotateLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateCustomLabelValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.Value, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateCustomLabelHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.Hidden, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateCustomLabelUseFontPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(customLabel.UseFontPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(customLabel.UseFontPercent, ref result, customLabel.ExprHost))
					{
						Global.Tracer.Assert(customLabel.ExprHost != null, "(customLabel.ExprHost != null)");
						result.Value = customLabel.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateGaugeClipContentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gauge.ClipContent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ClipContent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gauge.ClipContent, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).ClipContentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateGaugeAspectRatioExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge gauge, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gauge.AspectRatio, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AspectRatio", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gauge.AspectRatio, ref result, gauge.ExprHost))
					{
						Global.Tracer.Assert(gauge.ExprHost != null, "(gauge.ExprHost != null)");
						result.Value = ((GaugeExprHost)gauge.ExprHost).AspectRatioExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeInputValueValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.Value, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(gaugeInputValue.Value, ref result);
			return result;
		}

		internal string EvaluateGaugeInputValueFormulaExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.Formula, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Formula", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.Formula, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.FormulaExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMinPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.MinPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MinPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.MinPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MinPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMaxPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.MaxPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "MaxPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.MaxPercent, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MaxPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueMultiplierExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.Multiplier, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.Multiplier, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateGaugeInputValueAddConstantExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeInputValue.AddConstant, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "AddConstant", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeInputValue.AddConstant, ref result, gaugeInputValue.ExprHost))
					{
						Global.Tracer.Assert(gaugeInputValue.ExprHost != null, "(gaugeInputValue.ExprHost != null)");
						result.Value = gaugeInputValue.ExprHost.AddConstantExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateGaugeLabelTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeLabel.Text, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Text", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeLabel.Text, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(gaugeLabel.Text, ref result);
			return result;
		}

		internal double EvaluateGaugeLabelAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeLabel.Angle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeLabel.Angle, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).AngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateGaugeLabelResizeModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeLabel.ResizeMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeLabel.ResizeMode, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateGaugeLabelTextShadowOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeLabel.TextShadowOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TextShadowOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeLabel.TextShadowOffset, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateGaugeLabelUseFontPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(gaugeLabel.UseFontPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(gaugeLabel.UseFontPercent, ref result, gaugeLabel.ExprHost))
					{
						Global.Tracer.Assert(gaugeLabel.ExprHost != null, "(gaugeLabel.ExprHost != null)");
						result.Value = ((GaugeLabelExprHost)gaugeLabel.ExprHost).UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorDecimalDigitColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.DecimalDigitColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.DecimalDigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateNumericIndicatorDigitColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.DigitColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.DigitColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal bool EvaluateNumericIndicatorUseFontPercentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.UseFontPercent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "UseFontPercent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.UseFontPercent, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.UseFontPercentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDecimalDigitsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.DecimalDigits, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigits", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.DecimalDigits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DecimalDigitsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal int EvaluateNumericIndicatorDigitsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.Digits, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Digits", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.Digits, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.DigitsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal double EvaluateNumericIndicatorMultiplierExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.Multiplier, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Multiplier", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.Multiplier, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.MultiplierExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorNonNumericStringExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.NonNumericString, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "NonNumericString", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.NonNumericString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.NonNumericStringExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorOutOfRangeStringExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.OutOfRangeString, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "OutOfRangeString", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.OutOfRangeString, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.OutOfRangeStringExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorResizeModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.ResizeMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.ResizeMode, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowDecimalPointExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.ShowDecimalPoint, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowDecimalPoint", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.ShowDecimalPoint, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowDecimalPointExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorShowLeadingZerosExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.ShowLeadingZeros, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowLeadingZeros", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.ShowLeadingZeros, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowLeadingZerosExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateNumericIndicatorIndicatorStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.IndicatorStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.IndicatorStyle, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateNumericIndicatorShowSignExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.ShowSign, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ShowSign", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.ShowSign, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.ShowSignExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateNumericIndicatorSnappingEnabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.SnappingEnabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingEnabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.SnappingEnabled, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingEnabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateNumericIndicatorSnappingIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.SnappingInterval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SnappingInterval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.SnappingInterval, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SnappingIntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorLedDimColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.LedDimColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "LedDimColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.LedDimColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.LedDimColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal double EvaluateNumericIndicatorSeparatorWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.SeparatorWidth, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.SeparatorWidth, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateNumericIndicatorSeparatorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicator.SeparatorColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "SeparatorColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicator.SeparatorColor, ref result, numericIndicator.ExprHost))
					{
						Global.Tracer.Assert(numericIndicator.ExprHost != null, "(numericIndicator.ExprHost != null)");
						result.Value = numericIndicator.ExprHost.SeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateNumericIndicatorRangeDecimalDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicatorRange.DecimalDigitColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DecimalDigitColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicatorRange.DecimalDigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DecimalDigitColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateNumericIndicatorRangeDigitColorExpression(NumericIndicatorRange numericIndicatorRange, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(numericIndicatorRange.DigitColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "DigitColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(numericIndicatorRange.DigitColor, ref result, numericIndicatorRange.ExprHost))
					{
						Global.Tracer.Assert(numericIndicatorRange.ExprHost != null, "(numericIndicatorRange.ExprHost != null)");
						result.Value = numericIndicatorRange.ExprHost.DigitColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateIndicatorImageHueColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(indicatorImage.HueColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "HueColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(indicatorImage.HueColor, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.HueColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal double EvaluateIndicatorImageTransparencyExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(indicatorImage.Transparency, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Transparency", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(indicatorImage.Transparency, ref result, indicatorImage.ExprHost))
					{
						Global.Tracer.Assert(indicatorImage.ExprHost != null, "(indicatorImage.ExprHost != null)");
						result.Value = indicatorImage.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorTransformationTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(stateIndicator.TransformationType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "TransformationType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(stateIndicator.TransformationType, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.TransformationTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateStateIndicatorIndicatorStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(stateIndicator.IndicatorStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(stateIndicator.IndicatorStyle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorScaleFactorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(stateIndicator.ScaleFactor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(stateIndicator.ScaleFactor, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateStateIndicatorResizeModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(stateIndicator.ResizeMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ResizeMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(stateIndicator.ResizeMode, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateStateIndicatorAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(stateIndicator.Angle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Angle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(stateIndicator.Angle, ref result, stateIndicator.ExprHost))
					{
						Global.Tracer.Assert(stateIndicator.ExprHost != null, "(stateIndicator.ExprHost != null)");
						result.Value = stateIndicator.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(indicatorState.Color, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "Color", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(indicatorState.Color, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal double EvaluateIndicatorStateScaleFactorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(indicatorState.ScaleFactor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "ScaleFactor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(indicatorState.ScaleFactor, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateIndicatorStateIndicatorStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(indicatorState.IndicatorStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, objectName, "IndicatorStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(indicatorState.IndicatorStyle, ref result, indicatorState.ExprHost))
					{
						Global.Tracer.Assert(indicatorState.ExprHost != null, "(indicatorState.ExprHost != null)");
						result.Value = indicatorState.ExprHost.IndicatorStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLocationLeftExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLocation.Left, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Left", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLocation.Left, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.LeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLocationTopExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLocation.Top, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Top", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLocation.Top, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.TopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapLocationUnitExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLocation.Unit, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLocation.Unit, ref result, mapLocation.ExprHost))
					{
						Global.Tracer.Assert(mapLocation.ExprHost != null, "(mapLocation.ExprHost != null)");
						result.Value = mapLocation.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapSizeWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSize.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSize.Width, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSizeHeightExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSize.Height, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Height", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSize.Height, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.HeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapSizeUnitExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize mapSize, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSize.Unit, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Unit", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSize.Unit, ref result, mapSize.ExprHost))
					{
						Global.Tracer.Assert(mapSize.ExprHost != null, "(mapSize.ExprHost != null)");
						result.Value = mapSize.ExprHost.UnitExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapGridLinesHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapGridLines.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapGridLines.Hidden, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapGridLinesIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapGridLines.Interval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Interval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapGridLines.Interval, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.IntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapGridLinesShowLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapGridLines.ShowLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapGridLines.ShowLabels, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.ShowLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapGridLinesLabelPositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapGridLines.LabelPosition, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPosition", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapGridLines.LabelPosition, ref result, mapGridLines.ExprHost))
					{
						Global.Tracer.Assert(mapGridLines.ExprHost != null, "(mapGridLines.ExprHost != null)");
						result.Value = mapGridLines.ExprHost.LabelPositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapDockableSubItemPositionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDockableSubItem.Position, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Position", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDockableSubItem.Position, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.MapPositionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemDockOutsideViewportExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDockableSubItem.DockOutsideViewport, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DockOutsideViewport", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDockableSubItem.DockOutsideViewport, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.DockOutsideViewportExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapDockableSubItemHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDockableSubItem.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDockableSubItem.Hidden, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal VariantResult EvaluateMapDockableSubItemToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDockableSubItem.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDockableSubItem.ToolTip, ref result, mapDockableSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapDockableSubItem.ExprHost != null, "(mapDockableSubItem.ExprHost != null)");
						result.Value = mapDockableSubItem.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapSubItemLeftMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSubItem.LeftMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LeftMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSubItem.LeftMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.LeftMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemRightMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSubItem.RightMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RightMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSubItem.RightMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.RightMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemTopMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSubItem.TopMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TopMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSubItem.TopMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.TopMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSubItemBottomMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSubItem.BottomMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BottomMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSubItem.BottomMargin, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.BottomMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal int EvaluateMapSubItemZIndexExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSubItem.ZIndex, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ZIndex", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSubItem.ZIndex, ref result, mapSubItem.ExprHost))
					{
						Global.Tracer.Assert(mapSubItem.ExprHost != null, "(mapSubItem.ExprHost != null)");
						result.Value = mapSubItem.ExprHost.ZIndexExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapBindingFieldPairFieldNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapBindingFieldPair.FieldName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "FieldName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapBindingFieldPair.FieldName, ref result, mapBindingFieldPair.ExprHost))
					{
						Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
						result.Value = mapBindingFieldPair.ExprHost.FieldNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapBindingFieldPairBindingExpressionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapBindingFieldPair.BindingExpression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BindingExpression", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapBindingFieldPair.BindingExpression, ref result, null))
					{
						if (mapBindingFieldPair.InElementView)
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHost != null, "(mapBindingFieldPair.ExprHost != null)");
							if (this.m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHost.SetReportObjectModel(this.m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHost.BindingExpressionExpr;
						}
						else
						{
							Global.Tracer.Assert(mapBindingFieldPair.ExprHostMapMember != null, "(mapBindingFieldPair.ExprHostMapMember != null)");
							if (this.m_exprHostInSandboxAppDomain)
							{
								mapBindingFieldPair.ExprHostMapMember.SetReportObjectModel(this.m_reportObjectModel);
							}
							result.Value = mapBindingFieldPair.ExprHostMapMember.BindingExpressionExpr;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapBindingFieldPair.BindingExpression, ref result);
			return result;
		}

		internal string EvaluateMapViewportMapCoordinateSystemExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.MapCoordinateSystem, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapCoordinateSystem", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.MapCoordinateSystem, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapCoordinateSystemExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapViewportMapProjectionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.MapProjection, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapProjection", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.MapProjection, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MapProjectionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.ProjectionCenterX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.ProjectionCenterX, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportProjectionCenterYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.ProjectionCenterY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ProjectionCenterY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.ProjectionCenterY, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ProjectionCenterYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMaximumZoomExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.MaximumZoom, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.MaximumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportMinimumZoomExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.MinimumZoom, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.MinimumZoom, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapViewportSimplificationResolutionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.SimplificationResolution, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SimplificationResolution", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.SimplificationResolution, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.SimplificationResolutionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapViewportContentMarginExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.ContentMargin, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ContentMargin", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.ContentMargin, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.ContentMarginExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapViewportGridUnderContentExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapViewport.GridUnderContent, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "GridUnderContent", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapViewport.GridUnderContent, ref result, mapViewport.ExprHost))
					{
						Global.Tracer.Assert(mapViewport.ExprHost != null, "(mapViewport.ExprHost != null)");
						result.Value = mapViewport.ExprHost.GridUnderContentExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLimits.MinimumX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLimits.MinimumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMinimumYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLimits.MinimumY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLimits.MinimumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MinimumYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLimits.MaximumX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLimits.MaximumX, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLimitsMaximumYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLimits.MaximumY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLimits.MaximumY, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.MaximumYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal bool EvaluateMapLimitsLimitToDataExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLimits.LimitToData, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LimitToData", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLimits.LimitToData, ref result, mapLimits.ExprHost))
					{
						Global.Tracer.Assert(mapLimits.ExprHost != null, "(mapLimits.ExprHost != null)");
						result.Value = mapLimits.ExprHost.LimitToDataExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleTickMarkLengthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.TickMarkLength, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TickMarkLength", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.TickMarkLength, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.TickMarkLengthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapColorScaleColorBarBorderColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.ColorBarBorderColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ColorBarBorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.ColorBarBorderColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.ColorBarBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal int EvaluateMapColorScaleLabelIntervalExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.LabelInterval, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelInterval", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.LabelInterval, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelIntervalExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelFormatExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.LabelFormat, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelFormat", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.LabelFormat, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelFormatExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.LabelPlacement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.LabelPlacement, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorScaleLabelBehaviorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.LabelBehavior, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelBehavior", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.LabelBehavior, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.LabelBehaviorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapColorScaleHideEndLabelsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.HideEndLabels, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "HideEndLabels", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.HideEndLabels, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.HideEndLabelsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapColorScaleRangeGapColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.RangeGapColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "RangeGapColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.RangeGapColor, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.RangeGapColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapColorScaleNoDataTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScale.NoDataText, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "NoDataText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScale.NoDataText, ref result, mapColorScale.ExprHost))
					{
						Global.Tracer.Assert(mapColorScale.ExprHost != null, "(mapColorScale.ExprHost != null)");
						result.Value = mapColorScale.ExprHost.NoDataTextExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapColorScaleTitleCaptionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorScaleTitle.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorScaleTitle.Caption, ref result, mapColorScaleTitle.ExprHost))
					{
						Global.Tracer.Assert(mapColorScaleTitle.ExprHost != null, "(mapColorScaleTitle.ExprHost != null)");
						result.Value = mapColorScaleTitle.ExprHost.CaptionExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapDistanceScaleScaleColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDistanceScale.ScaleColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDistanceScale.ScaleColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapDistanceScaleScaleBorderColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapDistanceScale.ScaleBorderColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleBorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapDistanceScale.ScaleBorderColor, ref result, mapDistanceScale.ExprHost))
					{
						Global.Tracer.Assert(mapDistanceScale.ExprHost != null, "(mapDistanceScale.ExprHost != null)");
						result.Value = mapDistanceScale.ExprHost.ScaleBorderColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal VariantResult EvaluateMapTitleTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTitle.Text, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Text", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTitle.Text, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.TextExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal double EvaluateMapTitleAngleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTitle.Angle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Angle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTitle.Angle, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.AngleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTitleTextShadowOffsetExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTitle.TextShadowOffset, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextShadowOffset", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTitle.TextShadowOffset, ref result, mapTitle.ExprHost))
					{
						Global.Tracer.Assert(mapTitle.ExprHost != null, "(mapTitle.ExprHost != null)");
						result.Value = mapTitle.ExprHost.TextShadowOffsetExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLegendLayoutExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.Layout, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Layout", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.Layout, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.LayoutExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLegendAutoFitTextDisabledExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.AutoFitTextDisabled, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AutoFitTextDisabled", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.AutoFitTextDisabled, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.AutoFitTextDisabledExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendMinFontSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.MinFontSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinFontSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.MinFontSize, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.MinFontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal bool EvaluateMapLegendInterlacedRowsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.InterlacedRows, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRows", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.InterlacedRows, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapLegendInterlacedRowsColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.InterlacedRowsColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "InterlacedRowsColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.InterlacedRowsColor, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.InterlacedRowsColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal bool EvaluateMapLegendEquallySpacedItemsExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.EquallySpacedItems, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EquallySpacedItems", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.EquallySpacedItems, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.EquallySpacedItemsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal int EvaluateMapLegendTextWrapThresholdExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegend.TextWrapThreshold, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextWrapThreshold", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegend.TextWrapThreshold, ref result, mapLegend.ExprHost))
					{
						Global.Tracer.Assert(mapLegend.ExprHost != null, "(mapLegend.ExprHost != null)");
						result.Value = mapLegend.ExprHost.TextWrapThresholdExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapLegendTitleCaptionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegendTitle.Caption, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Caption", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegendTitle.Caption, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.CaptionExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegendTitle.TitleSeparator, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparator", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegendTitle.TitleSeparator, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLegendTitleTitleSeparatorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLegendTitle.TitleSeparatorColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TitleSeparatorColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLegendTitle.TitleSeparatorColor, ref result, mapLegendTitle.ExprHost))
					{
						Global.Tracer.Assert(mapLegendTitle.ExprHost != null, "(mapLegendTitle.ExprHost != null)");
						result.Value = mapLegendTitle.ExprHost.TitleSeparatorColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal VariantResult EvaluateMapAppearanceRuleDataValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.DataValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.DataValue, ref result, mapAppearanceRule.ExprHostMapMember))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHostMapMember != null || mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHostMapMember != null) || (mapAppearanceRule.ExprHost != null)");
						if (mapAppearanceRule.ExprHostMapMember != null)
						{
							result.Value = mapAppearanceRule.ExprHostMapMember.DataValueExpr;
						}
						else
						{
							result.Value = mapAppearanceRule.ExprHost.DataValueExpr;
						}
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapAppearanceRule.DataValue, ref result);
			return result;
		}

		internal string EvaluateMapAppearanceRuleDistributionTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.DistributionType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DistributionType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.DistributionType, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.DistributionTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal int EvaluateMapAppearanceRuleBucketCountExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.BucketCount, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "BucketCount", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.BucketCount, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.BucketCountExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result).Value;
		}

		internal VariantResult EvaluateMapAppearanceRuleStartValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.StartValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.StartValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapAppearanceRule.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleEndValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.EndValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.EndValue, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapAppearanceRule.EndValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapAppearanceRuleLegendTextExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapAppearanceRule.LegendText, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LegendText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapAppearanceRule.LegendText, ref result, mapAppearanceRule.ExprHost))
					{
						Global.Tracer.Assert(mapAppearanceRule.ExprHost != null, "(mapAppearanceRule.ExprHost != null)");
						result.Value = mapAppearanceRule.ExprHost.LegendTextExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapBucketStartValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapBucket.StartValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapBucket.StartValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.StartValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapBucket.StartValue, ref result);
			return result;
		}

		internal VariantResult EvaluateMapBucketEndValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapBucket.EndValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapBucket.EndValue, ref result, mapBucket.ExprHost))
					{
						Global.Tracer.Assert(mapBucket.ExprHost != null, "(mapBucket.ExprHost != null)");
						result.Value = mapBucket.ExprHost.EndValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapBucket.EndValue, ref result);
			return result;
		}

		internal string EvaluateMapColorPaletteRulePaletteExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorPaletteRule.Palette, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Palette", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorPaletteRule.Palette, ref result, mapColorPaletteRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorPaletteRule.ExprHost != null, "(mapColorPaletteRule.ExprHost != null)");
						result.Value = mapColorPaletteRule.ExprHost.PaletteExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapColorRangeRuleStartColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorRangeRule.StartColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorRangeRule.StartColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.StartColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapColorRangeRuleMiddleColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorRangeRule.MiddleColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MiddleColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorRangeRule.MiddleColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.MiddleColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapColorRangeRuleEndColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorRangeRule.EndColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorRangeRule.EndColor, ref result, mapColorRangeRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRangeRule.ExprHost != null, "(mapColorRangeRule.ExprHost != null)");
						result.Value = mapColorRangeRule.ExprHost.EndColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal bool EvaluateMapColorRuleShowInColorScaleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapColorRule.ShowInColorScale, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowInColorScale", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapColorRule.ShowInColorScale, ref result, mapColorRule.ExprHost))
					{
						Global.Tracer.Assert(mapColorRule.ExprHost != null, "(mapColorRule.ExprHost != null)");
						result.Value = mapColorRule.ExprHost.ShowInColorScaleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapSizeRuleStartSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSizeRule.StartSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "StartSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSizeRule.StartSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.StartSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapSizeRuleEndSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSizeRule.EndSize, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "EndSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSizeRule.EndSize, ref result, mapSizeRule.ExprHost))
					{
						Global.Tracer.Assert(mapSizeRule.ExprHost != null, "(mapSizeRule.ExprHost != null)");
						result.Value = mapSizeRule.ExprHost.EndSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapMarkerImageSourceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.Source, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.Source, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapMarkerImageValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapMarkerImage.Value, ref result);
			return result;
		}

		internal string EvaluateMapMarkerImageStringValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			StringResult stringResult = this.ProcessStringResult(result);
			errorOccurred = stringResult.ErrorOccurred;
			return stringResult.Value;
		}

		internal byte[] EvaluateMapMarkerImageBinaryValueExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBinaryExpression(mapMarkerImage.Value, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.Value, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			BinaryResult binaryResult = this.ProcessBinaryResult(result);
			errorOccurred = binaryResult.ErrorOccurred;
			return binaryResult.Value;
		}

		internal string EvaluateMapMarkerImageMIMETypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.MIMEType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MIMEType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.MIMEType, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.MIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerImageTransparentColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.TransparentColor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TransparentColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.TransparentColor, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.TransparentColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapMarkerImageResizeModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarkerImage.ResizeMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ResizeMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarkerImage.ResizeMode, ref result, mapMarkerImage.ExprHost))
					{
						Global.Tracer.Assert(mapMarkerImage.ExprHost != null, "(mapMarkerImage.ExprHost != null)");
						result.Value = mapMarkerImage.ExprHost.ResizeModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapMarkerMapMarkerStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapMarker.MapMarkerStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapMarkerStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapMarker.MapMarkerStyle, ref result, mapMarker.ExprHost))
					{
						Global.Tracer.Assert(mapMarker.ExprHost != null, "(mapMarker.ExprHost != null)");
						result.Value = mapMarker.ExprHost.MapMarkerStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapCustomColorColorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapCustomColor.Color, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Color", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapCustomColor.Color, ref result, mapCustomColor.ExprHost))
					{
						Global.Tracer.Assert(mapCustomColor.ExprHost != null, "(mapCustomColor.ExprHost != null)");
						result.Value = mapCustomColor.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this, true);
		}

		internal string EvaluateMapLineTemplateWidthExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLineTemplate.Width, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Width", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLineTemplate.Width, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.WidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapLineTemplateLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLineTemplate.LabelPlacement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLineTemplate.LabelPlacement, ref result, mapLineTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapLineTemplate.ExprHost != null, "(mapLineTemplate.ExprHost != null)");
						result.Value = mapLineTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateScaleFactorExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygonTemplate.ScaleFactor, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ScaleFactor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygonTemplate.ScaleFactor, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ScaleFactorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetX, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapPolygonTemplateCenterPointOffsetYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygonTemplate.CenterPointOffsetY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterPointOffsetY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygonTemplate.CenterPointOffsetY, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.CenterPointOffsetYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateShowLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygonTemplate.ShowLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShowLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygonTemplate.ShowLabel, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapPolygonTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.ShowLabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapPolygonTemplateLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygonTemplate.LabelPlacement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygonTemplate.LabelPlacement, ref result, mapPolygonTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPolygonTemplate.ExprHost != null, "(mapLabelTemplate.ExprHost != null)");
						result.Value = mapPolygonTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapSpatialElementTemplateHiddenExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.Hidden, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.Hidden, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.HiddenExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.OffsetX, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapSpatialElementTemplateOffsetYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.OffsetY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "OffsetY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.OffsetY, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.OffsetYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.Label, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.Label, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.LabelExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateDataElementLabelExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.DataElementLabel, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataElementLabel", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.DataElementLabel, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.DataElementLabelExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal VariantResult EvaluateMapSpatialElementTemplateToolTipExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialElementTemplate.ToolTip, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialElementTemplate.ToolTip, ref result, mapSpatialElementTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialElementTemplate.ExprHost != null, "(mapSpatialElementTemplate.ExprHost != null)");
						result.Value = mapSpatialElementTemplate.ExprHost.ToolTipExpr;
						return result;
					}
					return result;
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
					return result;
				}
			}
			return result;
		}

		internal string EvaluateMapPointTemplateSizeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPointTemplate.Size, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Size", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPointTemplate.Size, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.SizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateMapPointTemplateLabelPlacementExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPointTemplate.LabelPlacement, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LabelPlacement", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPointTemplate.LabelPlacement, ref result, mapPointTemplate.ExprHost))
					{
						Global.Tracer.Assert(mapPointTemplate.ExprHost != null, "(mapPointTemplate.ExprHost != null)");
						result.Value = mapPointTemplate.ExprHost.LabelPlacementExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapLineUseCustomLineTemplateExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine mapLine, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLine.UseCustomLineTemplate, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomLineTemplate", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLine.UseCustomLineTemplate, ref result, mapLine.ExprHost))
					{
						Global.Tracer.Assert(mapLine.ExprHost != null, "(mapLine.ExprHost != null)");
						result.Value = mapLine.ExprHost.UseCustomLineTemplateExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPolygonTemplateExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygon.UseCustomPolygonTemplate, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPolygonTemplate", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygon.UseCustomPolygonTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPolygonTemplateExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPolygonUseCustomPointTemplateExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPolygon.UseCustomCenterPointTemplate, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomCenterPointTemplate", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPolygon.UseCustomCenterPointTemplate, ref result, mapPolygon.ExprHost))
					{
						Global.Tracer.Assert(mapPolygon.ExprHost != null, "(mapPolygon.ExprHost != null)");
						result.Value = mapPolygon.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal bool EvaluateMapPointUseCustomPointTemplateExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapPoint.UseCustomPointTemplate, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseCustomPointTemplate", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapPoint.UseCustomPointTemplate, ref result, mapPoint.ExprHost))
					{
						Global.Tracer.Assert(mapPoint.ExprHost != null, "(mapPoint.ExprHost != null)");
						result.Value = mapPoint.ExprHost.UseCustomPointTemplateExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapFieldNameNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName mapFieldName, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapFieldName.Name, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Name", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapFieldName.Name, ref result, mapFieldName.ExprHost))
					{
						Global.Tracer.Assert(mapFieldName.ExprHost != null, "(mapFieldName.ExprHost != null)");
						result.Value = mapFieldName.ExprHost.NameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapLayerVisibilityModeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLayer.VisibilityMode, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VisibilityMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLayer.VisibilityMode, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.VisibilityModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapLayerMinimumZoomExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLayer.MinimumZoom, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MinimumZoom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLayer.MinimumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MinimumZoomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerMaximumZoomExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLayer.MaximumZoom, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MaximumZoom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLayer.MaximumZoom, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.MaximumZoomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapLayerTransparencyExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapLayer.Transparency, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Transparency", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapLayer.Transparency, ref result, mapLayer.ExprHost))
					{
						Global.Tracer.Assert(mapLayer.ExprHost != null, "(mapLayer.ExprHost != null)");
						result.Value = mapLayer.ExprHost.TransparencyExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapShapefileSourceExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapShapefile.Source, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Source", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapShapefile.Source, ref result, mapShapefile.ExprHost))
					{
						Global.Tracer.Assert(mapShapefile.ExprHost != null, "(mapShapefile.ExprHost != null)");
						result.Value = mapShapefile.ExprHost.SourceExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal VariantResult EvaluateMapSpatialDataRegionVectorDataExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialDataRegion.VectorData, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "VectorData", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialDataRegion.VectorData, ref result, mapSpatialDataRegion.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataRegion.ExprHost != null, "(mapSpatialDataRegion.ExprHost != null)");
						result.Value = mapSpatialDataRegion.ExprHost.VectorDataExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(mapSpatialDataRegion.VectorData, ref result);
			return result;
		}

		internal string EvaluateMapSpatialDataSetDataSetNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialDataSet.DataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "DataSetName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialDataSet.DataSetName, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.DataSetNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapSpatialDataSetSpatialFieldExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapSpatialDataSet.SpatialField, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "SpatialField", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapSpatialDataSet.SpatialField, ref result, mapSpatialDataSet.ExprHost))
					{
						Global.Tracer.Assert(mapSpatialDataSet.ExprHost != null, "(mapSpatialDataSet.ExprHost != null)");
						result.Value = mapSpatialDataSet.ExprHost.SpatialFieldExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerServiceUrlExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTileLayer.ServiceUrl, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ServiceUrl", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTileLayer.ServiceUrl, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.ServiceUrlExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTileLayerTileStyleExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTileLayer.TileStyle, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTileLayer.TileStyle, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.TileStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal bool EvaluateMapTileLayerUseSecureConnectionExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapTileLayer.UseSecureConnection, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "UseSecureConnection", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapTileLayer.UseSecureConnection, ref result, mapTileLayer.ExprHost))
					{
						Global.Tracer.Assert(mapTileLayer.ExprHost != null, "(mapTileLayer.ExprHost != null)");
						result.Value = mapTileLayer.ExprHost.UseSecureConnectionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessBooleanResult(result).Value;
		}

		internal string EvaluateMapAntiAliasingExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(map.AntiAliasing, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "AntiAliasing", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(map.AntiAliasing, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.AntiAliasingExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapTextAntiAliasingQualityExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(map.TextAntiAliasingQuality, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TextAntiAliasingQuality", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(map.TextAntiAliasingQuality, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TextAntiAliasingQualityExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapShadowIntensityExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(map.ShadowIntensity, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "ShadowIntensity", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(map.ShadowIntensity, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.ShadowIntensityExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapTileLanguageExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(map.TileLanguage, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "TileLanguage", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(map.TileLanguage, ref result, map.MapExprHost))
					{
						Global.Tracer.Assert(map.MapExprHost != null, "(map.MapExprHost != null)");
						result.Value = map.MapExprHost.TileLanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateMapBorderSkinMapBorderSkinTypeExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapBorderSkin.MapBorderSkinType, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "MapBorderSkinType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapBorderSkin.MapBorderSkinType, ref result, mapBorderSkin.ExprHost))
					{
						Global.Tracer.Assert(mapBorderSkin.ExprHost != null, "(mapBorderSkin.ExprHost != null)");
						result.Value = mapBorderSkin.ExprHost.MapBorderSkinTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterXExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapCustomView.CenterX, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterX", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapCustomView.CenterX, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterXExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal double EvaluateMapCustomViewCenterYExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapCustomView.CenterY, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "CenterY", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapCustomView.CenterY, ref result, mapCustomView.ExprHost))
					{
						Global.Tracer.Assert(mapCustomView.ExprHost != null, "(mapCustomView.ExprHost != null)");
						result.Value = mapCustomView.ExprHost.CenterYExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapElementViewLayerNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapElementView.LayerName, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "LayerName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapElementView.LayerName, ref result, mapElementView.ExprHost))
					{
						Global.Tracer.Assert(mapElementView.ExprHost != null, "(mapElementView.ExprHost != null)");
						result.Value = mapElementView.ExprHost.LayerNameExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal double EvaluateMapViewZoomExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.MapView mapView, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(mapView.Zoom, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "Zoom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(mapView.Zoom, ref result, mapView.ExprHost))
					{
						Global.Tracer.Assert(mapView.ExprHost != null, "(mapView.ExprHost != null)");
						result.Value = mapView.ExprHost.ZoomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result).Value;
		}

		internal string EvaluateMapPageNameExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, string objectName)
		{
			bool isUnrestrictedRenderFormatReferenceMode = this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode;
			this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = false;
			try
			{
				VariantResult result = default(VariantResult);
				if (!this.EvaluateSimpleExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, objectName, "PageName", out result))
				{
					try
					{
						if (!this.EvaluateComplexExpression(expression, ref result, map.ExprHost))
						{
							Global.Tracer.Assert(map.ExprHost != null, "(map.ExprHost != null)");
							result.Value = map.ExprHost.PageNameExpr;
						}
					}
					catch (Exception e)
					{
						this.RegisterRuntimeErrorInExpression(ref result, e);
					}
				}
				return this.ProcessStringResult(result, true).Value;
			}
			finally
			{
				this.m_reportObjectModel.OdpContext.IsUnrestrictedRenderFormatReferenceMode = isUnrestrictedRenderFormatReferenceMode;
			}
		}
	}
}
