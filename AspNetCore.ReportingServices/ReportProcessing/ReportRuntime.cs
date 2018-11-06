using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ReportRuntime : IErrorContext
	{
		private sealed class ExpressionHostLoader : MarshalByRefObject
		{
			private const string ExprHostRootType = "ReportExprHostImpl";

			private static readonly Hashtable ExpressionHosts;

			private static readonly byte[] ReportExpressionsDefaultEvidencePK;

			private static readonly Assembly ProcessingObjectModelAssembly;

			internal static ReportExprHost LoadExprHost(byte[] exprHostBytes, string exprHostAssemblyName, bool parametersOnly, ObjectModel objectModel, StringList codeModules, AppDomain targetAppDomain)
			{
				//Type typeFromHandle = typeof(ExpressionHostLoader);
				ExpressionHostLoader expressionHostLoader = Activator.CreateInstance<ExpressionHostLoader>();
				return expressionHostLoader.LoadExprHostRemoteEntryPoint(exprHostBytes, exprHostAssemblyName, parametersOnly, objectModel, codeModules);
			}

			internal static ReportExprHost LoadExprHostIntoCurrentAppDomain(byte[] exprHostBytes, string exprHostAssemblyName, Evidence evidence, bool parametersOnly, ObjectModel objectModel, StringList codeModules)
			{
				if (codeModules != null && 0 < codeModules.Count)
				{
                    RevertImpersonationContext.RunFromRestrictedCasContext(delegate
					{
						for (int num = codeModules.Count - 1; num >= 0; num--)
						{
							Assembly.Load(codeModules[num]);
						}
					});
				}
				Assembly assembly = ExpressionHostLoader.LoadExprHostAssembly(exprHostBytes, exprHostAssemblyName, evidence);
				Type type = assembly.GetType("ReportExprHostImpl");
				try
				{
					return (ReportExprHost)type.GetConstructors()[0].Invoke(new object[2]
					{
						parametersOnly,
						objectModel
					});
				}
				catch (Exception ex)
				{
					if (assembly.GetName().Version >= new Version(8, 0, 700))
					{
						throw ex;
					}
					return (ReportExprHost)type.GetConstructors()[0].Invoke(new object[1]
					{
						parametersOnly
					});
				}
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
				evidence.AddHost(new Zone(SecurityZone.MyComputer));
				evidence.AddHost(new StrongName(new StrongNamePublicKeyBlob(ExpressionHostLoader.ReportExpressionsDefaultEvidencePK), exprHostAssemblyName, new Version("1.0.0.0")));
				return evidence;
			}

			private ReportExprHost LoadExprHostRemoteEntryPoint(byte[] exprHostBytes, string exprHostAssemblyName, bool parametersOnly, ObjectModel objectModel, StringList codeModules)
			{
				return ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(exprHostBytes, exprHostAssemblyName, null, parametersOnly, objectModel, codeModules);
			}

			static ExpressionHostLoader()
			{
				ExpressionHostLoader.ExpressionHosts = new Hashtable();
				ExpressionHostLoader.ReportExpressionsDefaultEvidencePK = new byte[160]
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
				ExpressionHostLoader.ProcessingObjectModelAssembly = typeof(ObjectModel).Assembly;
				AppDomain.CurrentDomain.AssemblyResolve += ExpressionHostLoader.ResolveAssemblyHandler;
			}

			private static Assembly ResolveAssemblyHandler(object sender, ResolveEventArgs args)
			{
				if (args.Name != null && args.Name.StartsWith("AspNetCore.ReportingServices.Processing", StringComparison.Ordinal))
				{
					return ExpressionHostLoader.ProcessingObjectModelAssembly;
				}
				return null;
			}
		}

		private Assembly m_exprHostAssembly;

		private ReportExprHost m_reportExprHost;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private ObjectModelImpl m_reportObjectModel;

		private ErrorContext m_errorContext;

		private ReportProcessing.IScope m_currentScope;

		private ReportRuntime m_topLevelReportRuntime;

		private IActionOwner m_currentActionOwner;

		internal ReportExprHost ReportExprHost
		{
			get
			{
				return this.m_reportExprHost;
			}
		}

		internal ReportProcessing.IScope CurrentScope
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

		internal ObjectType ObjectType
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

		internal ObjectModelImpl ReportObjectModel
		{
			get
			{
				return this.m_reportObjectModel;
			}
		}

		internal IActionOwner CurrentActionOwner
		{
			get
			{
				return this.m_currentActionOwner;
			}
			set
			{
				this.m_currentActionOwner = value;
			}
		}

		internal ReportRuntime(ObjectModelImpl reportObjectModel, ErrorContext errorContext)
		{
			this.m_objectType = ObjectType.Report;
			this.m_reportObjectModel = reportObjectModel;
			this.m_errorContext = errorContext;
		}

		internal ReportRuntime(ObjectModelImpl reportObjectModel, ErrorContext errorContext, ReportExprHost copyReportExprHost, ReportRuntime topLevelReportRuntime)
			: this(reportObjectModel, errorContext)
		{
			this.m_reportExprHost = copyReportExprHost;
			this.m_topLevelReportRuntime = topLevelReportRuntime;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			this.m_errorContext.Register(code, severity, this.m_objectType, this.m_objectName, this.m_propertyName, arguments);
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			this.m_errorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
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

		internal string EvaluateReportLanguageExpression(Report report, out CultureInfo language)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(report.Language, report.ObjectType, report.Name, "Language", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(report.Language, ref result))
					{
						Global.Tracer.Assert(report.ReportExprHost != null);
						result.Value = report.ReportExprHost.ReportLanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateSpecificLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out language);
		}

		internal VariantResult EvaluateParamDefaultValue(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.DefaultExpressions != null);
			ExpressionInfo expressionInfo = paramDef.DefaultExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "DefaultValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && expressionInfo.ExprHostID >= 0);
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = ((IndexedExprHost)paramDef.ExprHost)[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(expressionInfo, ref result, true);
			return result;
		}

		internal VariantResult EvaluateParamValidValue(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesValueExpressions != null);
			ExpressionInfo expressionInfo = paramDef.ValidValuesValueExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "ValidValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValuesHost != null && expressionInfo.ExprHostID >= 0);
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValuesHost[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(expressionInfo, ref result, true);
			return result;
		}

		internal VariantResult EvaluateParamValidValueLabel(ParameterDef paramDef, int index)
		{
			Global.Tracer.Assert(paramDef.ValidValuesLabelExpressions != null);
			ExpressionInfo expressionInfo = paramDef.ValidValuesLabelExpressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, ObjectType.ReportParameter, paramDef.Name, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(paramDef.ExprHost != null && paramDef.ExprHost.ValidValueLabelsHost != null && expressionInfo.ExprHostID >= 0);
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = paramDef.UsedInQuery;
						result.Value = paramDef.ExprHost.ValidValueLabelsHost[expressionInfo.ExprHostID];
						this.m_reportObjectModel.UserImpl.IndirectQueryReference = false;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(expressionInfo, ref result, true);
			if (!result.ErrorOccurred && result.Value is object[])
			{
				try
				{
					VariantResult variantResult = default(VariantResult);
					object[] array = result.Value as object[];
					for (int i = 0; i < array.Length; i++)
					{
						variantResult.Value = array[i];
						this.ProcessLabelResult(ref variantResult);
						if (variantResult.ErrorOccurred)
						{
							result.ErrorOccurred = true;
							return result;
						}
						array[i] = variantResult.Value;
					}
					return result;
				}
				catch
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

		internal object EvaluateDataValueValueExpression(DataValue value, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value.Value, objectType, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value.Value, ref variantResult))
					{
						Global.Tracer.Assert(value.ExprHost != null);
						variantResult.Value = value.ExprHost.DataValueValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(value.Value, ref variantResult);
			return variantResult.Value;
		}

		internal string EvaluateDataValueNameExpression(DataValue value, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(value.Name, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(value.Name, ref result))
					{
						Global.Tracer.Assert(value.ExprHost != null);
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

		internal VariantResult EvaluateFilterVariantExpression(Filter filter, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(filter.Expression, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null);
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

		internal StringResult EvaluateFilterStringExpression(Filter filter, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(filter.Expression, objectType, objectName, "FilterExpression", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(filter.Expression, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null);
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

		internal VariantResult EvaluateFilterVariantValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = ((IndexedExprHost)filter.ExprHost)[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(expressionInfo, ref result, true);
			return result;
		}

		internal FloatResult EvaluateFilterIntegerOrFloatValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerOrFloatExpression(expressionInfo, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = ((IndexedExprHost)filter.ExprHost)[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerOrFloatResult(result);
		}

		internal IntegerResult EvaluateFilterIntegerValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expressionInfo, false, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = ((IndexedExprHost)filter.ExprHost)[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessIntegerResult(result);
		}

		internal StringResult EvaluateFilterStringValue(Filter filter, int index, ObjectType objectType, string objectName)
		{
			Global.Tracer.Assert(filter.Values != null);
			ExpressionInfo expressionInfo = filter.Values[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, objectType, objectName, "FilterValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(filter.ExprHost != null && expressionInfo.ExprHostID >= 0);
						result.Value = ((IndexedExprHost)filter.ExprHost)[expressionInfo.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result);
		}

		internal object EvaluateQueryParamValue(ExpressionInfo paramValue, IndexedExprHost queryParamsExprHost, ObjectType objectType, string objectName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramValue, objectType, objectName, "Value", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramValue, ref variantResult))
					{
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(queryParamsExprHost != null && paramValue.ExprHostID >= 0);
						variantResult.Value = queryParamsExprHost[paramValue.ExprHostID];
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
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

		internal StringResult EvaluateConnectString(DataSource dataSource)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataSource.ConnectStringExpression, ObjectType.DataSource, dataSource.Name, "ConnectString", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataSource.ConnectStringExpression, ref result))
					{
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(dataSource.ExprHost != null);
						result.Value = dataSource.ExprHost.ConnectStringExpr;
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
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

		internal StringResult EvaluateCommandText(DataSet dataSet)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataSet.Query.CommandText, ObjectType.Query, dataSet.Name, "CommandText", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataSet.Query.CommandText, ref result))
					{
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InQuery;
						Global.Tracer.Assert(dataSet.ExprHost != null);
						result.Value = dataSet.ExprHost.QueryCommandTextExpr;
						this.m_reportObjectModel.UserImpl.UserProfileLocation = UserProfileState.InReport;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result);
		}

		internal object EvaluateFieldValueExpression(Field field)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(field.Value, ObjectType.Field, field.Name, "Value", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(field.Value, ref variantResult))
					{
						Global.Tracer.Assert(field.ExprHost != null);
						variantResult.Value = field.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(field.Value, ref variantResult);
			return variantResult.Value;
		}

		internal VariantResult EvaluateAggregateVariantOrBinaryParamExpr(DataAggregateInfo aggregateInfo, int index, IErrorContext errorContext)
		{
			Global.Tracer.Assert(aggregateInfo.Expressions != null);
			ExpressionInfo expressionInfo = aggregateInfo.Expressions[index];
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref result))
					{
						Global.Tracer.Assert(aggregateInfo.ExpressionHosts != null && expressionInfo.ExprHostID >= 0);
						result.Value = aggregateInfo.ExpressionHosts[index].ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e, errorContext, false);
				}
			}
			this.ProcessVariantOrBinaryResult(expressionInfo, ref result, errorContext, true);
			return result;
		}

		internal bool EvaluateParamValueOmitExpression(ParameterValue paramVal, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(paramVal.Omit, true, objectType, objectName, "ParameterOmit", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramVal.Omit, ref result))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
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

		internal object EvaluateParamVariantValueExpression(ParameterValue paramVal, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramVal.Value, ref variantResult))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
						variantResult.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(paramVal.Value, ref variantResult, true);
			return variantResult.Value;
		}

		internal ParameterValueResult EvaluateParameterValueExpression(ParameterValue paramVal, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(paramVal.Value, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(paramVal.Value, ref result))
					{
						Global.Tracer.Assert(paramVal.ExprHost != null);
						result.Value = paramVal.ExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessParameterValueResult(paramVal.Value, result);
		}

		internal bool EvaluateStartHiddenExpression(Visibility visibility, IVisibilityHiddenExprHost hiddenExprHostRI, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(visibility.Hidden, true, objectType, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(visibility.Hidden, ref result))
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

		internal bool EvaluateStartHiddenExpression(Visibility visibility, IndexedExprHost hiddenExprHostIdx, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(visibility.Hidden, true, objectType, objectName, "Hidden", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(visibility.Hidden, ref result))
					{
						Global.Tracer.Assert(hiddenExprHostIdx != null && visibility.Hidden.ExprHostID >= 0);
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

		internal VariantResult EvaluateReportItemLabelExpression(ReportItem reportItem)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(reportItem.Label, reportItem.ObjectType, reportItem.Name, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(reportItem.Label, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
						result.Value = reportItem.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(reportItem.Label, ref result);
			return result;
		}

		internal string EvaluateReportItemBookmarkExpression(ReportItem reportItem)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(reportItem.Bookmark, reportItem.ObjectType, reportItem.Name, "Bookmark", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(reportItem.Bookmark, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
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

		internal string EvaluateReportItemToolTipExpression(ReportItem reportItem)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(reportItem.ToolTip, reportItem.ObjectType, reportItem.Name, "ToolTip", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(reportItem.ToolTip, ref result))
					{
						Global.Tracer.Assert(reportItem.ExprHost != null);
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

		internal string EvaluateActionLabelExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
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

		internal string EvaluateReportItemHyperlinkURLExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Hyperlink", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
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

		internal string EvaluateReportItemDrillthroughReportName(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "DrillthroughReportName", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
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

		internal string EvaluateReportItemBookmarkLinkExpression(ActionItem actionItem, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BookmarkLink", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(actionItem.ExprHost != null);
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

		internal string EvaluateImageStringValueExpression(Image image, out bool errorOccurred)
		{
			errorOccurred = false;
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(image.Value, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.Value, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
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

		internal byte[] EvaluateImageBinaryValueExpression(Image image, out bool errorOccurred)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBinaryExpression(image.Value, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.Value, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
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

		internal string EvaluateImageMIMETypeExpression(Image image)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(image.MIMEType, image.ObjectType, image.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(image.MIMEType, ref result))
					{
						Global.Tracer.Assert(image.ImageExprHost != null);
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

		internal VariantResult EvaluateTextBoxValueExpression(TextBox textBox)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(textBox.Value, textBox.ObjectType, textBox.Name, "Value", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(textBox.Value, ref result))
					{
						Global.Tracer.Assert(textBox.TextBoxExprHost != null);
						result.Value = textBox.TextBoxExprHost.ValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(textBox.Value, ref result);
			return result;
		}

		internal bool EvaluateTextBoxInitialToggleStateExpression(TextBox textBox)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateBooleanExpression(textBox.InitialToggleState, true, textBox.ObjectType, textBox.Name, "InitialState", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(textBox.InitialToggleState, ref result))
					{
						Global.Tracer.Assert(textBox.ExprHost != null);
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

		internal object EvaluateUserSortExpression(TextBox textBox)
		{
			int sortExpressionIndex = textBox.UserSort.SortExpressionIndex;
			ISortFilterScope sortTarget = textBox.UserSort.SortTarget;
			Global.Tracer.Assert(sortTarget.UserSortExpressions != null && 0 <= sortExpressionIndex && sortExpressionIndex < sortTarget.UserSortExpressions.Count);
			ExpressionInfo expressionInfo = sortTarget.UserSortExpressions[sortExpressionIndex];
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expressionInfo, textBox.ObjectType, textBox.Name, "SortExpression", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expressionInfo, ref variantResult))
					{
						Global.Tracer.Assert(sortTarget.UserSortExpressionsHost != null);
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

		internal VariantResult EvaluateGroupingLabelExpression(Grouping grouping, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(grouping.GroupLabel, objectType, objectName, "Label", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(grouping.GroupLabel, ref result))
					{
						Global.Tracer.Assert(grouping.ExprHost != null);
						result.Value = grouping.ExprHost.LabelExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			this.ProcessVariantResult(grouping.GroupLabel, ref result);
			return result;
		}

		internal object EvaluateRuntimeExpression(ReportProcessing.RuntimeExpressionInfo runtimeExpression, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(runtimeExpression.Expression, objectType, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(runtimeExpression.Expression, ref variantResult))
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
			if (variantResult.ErrorOccurred)
			{
				if (variantResult.FieldStatus != 0)
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, new string[1]
					{
						ReportRuntime.GetErrorName(variantResult.FieldStatus, variantResult.ExceptionMessage)
					});
				}
				else
				{
					((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Error, new string[0]);
				}
				throw new ReportProcessingException(this.m_errorContext.Messages);
			}
			if (variantResult.Value == null)
			{
				variantResult.Value = DBNull.Value;
			}
			return variantResult.Value;
		}

		internal object EvaluateOWCChartData(OWCChart chart, ExpressionInfo chartDataExpression)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(chartDataExpression, chart.ObjectType, chart.Name, "Value", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(chartDataExpression, ref variantResult))
					{
						Global.Tracer.Assert(chart.OWCChartExprHost != null && chart.OWCChartExprHost.OWCChartColumnHosts != null && chartDataExpression.ExprHostID >= 0);
						variantResult.Value = chart.OWCChartExprHost.OWCChartColumnHosts[chartDataExpression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(chartDataExpression, ref variantResult);
			return variantResult.Value;
		}

		internal string EvaluateSubReportNoRowsExpression(SubReport subReport, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(subReport.NoRows, ObjectType.Subreport, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(subReport.NoRows, ref result))
					{
						Global.Tracer.Assert(subReport.SubReportExprHost != null);
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

		internal string EvaluateDataRegionNoRowsExpression(DataRegion region, ObjectType objectType, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(region.NoRows, objectType, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(region.NoRows, ref result))
					{
						Global.Tracer.Assert(region.ExprHost != null);
						result.Value = ((DataRegionExprHost)region.ExprHost).NoRowsExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal object EvaluateChartDataPointDataValueExpression(ChartDataPoint dataPoint, ExpressionInfo dataPointDataValueExpression, string objectName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataPointDataValueExpression, ObjectType.Chart, objectName, "DataPoint", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataPointDataValueExpression, ref variantResult))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null && dataPointDataValueExpression.ExprHostID >= 0);
						variantResult.Value = ((IndexedExprHost)dataPoint.ExprHost)[dataPointDataValueExpression.ExprHostID];
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(dataPointDataValueExpression, ref variantResult);
			return variantResult.Value;
		}

		internal object EvaluateChartStaticHeadingLabelExpression(ChartHeading chartHeading, ExpressionInfo expression, string objectName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, "HeadingLabel", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref variantResult))
					{
						Chart chart = (Chart)chartHeading.DataRegionDef;
						IndexedExprHost indexedExprHost = null;
						if (chart.ChartExprHost != null)
						{
							indexedExprHost = ((!chartHeading.IsColumn) ? chart.ChartExprHost.StaticRowLabelsHost : chart.ChartExprHost.StaticColumnLabelsHost);
						}
						Global.Tracer.Assert(indexedExprHost != null && expression.ExprHostID >= 0);
						variantResult.Value = indexedExprHost[expression.ExprHostID];
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

		internal object EvaluateChartDynamicHeadingLabelExpression(ChartHeading chartHeading, ExpressionInfo expression, string objectName)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, "HeadingLabel", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref variantResult))
					{
						Global.Tracer.Assert(chartHeading.ExprHost != null);
						variantResult.Value = chartHeading.ExprHost.HeadingLabelExpr;
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

		internal string EvaluateChartTitleCaptionExpression(ChartTitle title, string objectName, string propertyName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(title.Caption, ObjectType.Chart, objectName, propertyName, out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(title.Caption, ref result))
					{
						Global.Tracer.Assert(title.ExprHost != null);
						result.Value = title.ExprHost.CaptionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return this.ProcessStringResult(result).Value;
		}

		internal string EvaluateChartDataLabelValueExpression(ChartDataPoint dataPoint, string objectName, object[] dataLabelStyleAttributeValues)
		{
			Global.Tracer.Assert(null != dataPoint.DataLabel);
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(dataPoint.DataLabel.Value, ObjectType.Chart, objectName, "DataLabel", out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(dataPoint.DataLabel.Value, ref variantResult))
					{
						Global.Tracer.Assert(dataPoint.ExprHost != null);
						variantResult.Value = dataPoint.ExprHost.DataLabelValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref variantResult, e);
				}
			}
			this.ProcessVariantResult(dataPoint.DataLabel.Value, ref variantResult);
			if (variantResult.Value != null)
			{
				string format = null;
				if (dataPoint.DataLabel.StyleClass != null)
				{
					AttributeInfo attributeInfo = dataPoint.DataLabel.StyleClass.StyleAttributes["Format"];
					if (attributeInfo != null)
					{
						format = ((!attributeInfo.IsExpression) ? attributeInfo.Value : ((string)dataLabelStyleAttributeValues[attributeInfo.IntValue]));
					}
				}
				string text = (string)(variantResult.Value = ((variantResult.Value is IFormattable) ? ((IFormattable)variantResult.Value).ToString(format, Thread.CurrentThread.CurrentCulture) : variantResult.Value.ToString()));
			}
			return (string)variantResult.Value;
		}

		internal object EvaluateChartAxisValueExpression(AxisExprHost exprHost, ExpressionInfo expression, string objectName, string propertyName, Axis.ExpressionType type)
		{
			VariantResult variantResult = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, ObjectType.Chart, objectName, propertyName, out variantResult))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref variantResult))
					{
						Global.Tracer.Assert(exprHost != null);
						switch (type)
						{
						case Axis.ExpressionType.Min:
							variantResult.Value = exprHost.AxisMinExpr;
							break;
						case Axis.ExpressionType.Max:
							variantResult.Value = exprHost.AxisMaxExpr;
							break;
						case Axis.ExpressionType.CrossAt:
							variantResult.Value = exprHost.AxisCrossAtExpr;
							break;
						case Axis.ExpressionType.MajorInterval:
							variantResult.Value = exprHost.AxisMajorIntervalExpr;
							break;
						case Axis.ExpressionType.MinorInterval:
							variantResult.Value = exprHost.AxisMinorIntervalExpr;
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

		internal string EvaluateStyleBorderColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderColorBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderColorBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderStyle(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(this.ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(this.ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(this.ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(this.ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderStyleBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderStyleBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderStyle(this.ProcessStringResult(result).Value, objectType, this);
		}

		internal string EvaluateStyleBorderWidth(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBorderWidthBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BorderWidth", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BorderWidthBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBorderWidth(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundGradientEndColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientEndColor", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundGradientEndColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundGradientType(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundGradientType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundGradientTypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBackgroundGradientType(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleBackgroundRepeat(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "BackgroundRepeat", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundRepeatExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateBackgroundRepeat(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontStyle(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontStyle", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontStyleExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontStyle(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontFamily(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontFamily", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
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

		internal string EvaluateStyleFontSize(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontSize", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontSizeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontSize(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFontWeight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "FontWeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.FontWeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateFontWeight(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleFormat(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Format", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
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

		internal string EvaluateStyleTextDecoration(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TextDecoration", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.TextDecorationExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateTextDecoration(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleTextAlign(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "TextAlign", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.TextAlignExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateTextAlign(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleVerticalAlign(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "VerticalAlign", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.VerticalAlignExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateVerticalAlign(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleColor(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Color", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.ColorExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateColor(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingLeft(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingLeft", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingLeftExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingRight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingRight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingRightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingTop(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingTop", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingTopExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStylePaddingBottom(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "PaddingBottom", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.PaddingBottomExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidatePadding(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLineHeight(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "LineHeight", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.LineHeightExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateLineHeight(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleDirection(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Direction", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.DirectionExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateDirection(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleWritingMode(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "WritingMode", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.WritingModeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateWritingMode(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleLanguage(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Language", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.LanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo cultureInfo = default(CultureInfo);
			return ProcessingValidator.ValidateSpecificLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out cultureInfo);
		}

		internal string EvaluateStyleUnicodeBiDi(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "UnicodeBiDi", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.UnicodeBiDiExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateUnicodeBiDi(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleCalendar(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "Calendar", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.CalendarExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateCalendar(this.ProcessStringResult(result).Value, this);
		}

		internal string EvaluateStyleNumeralLanguage(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateSimpleExpression(expression, objectType, objectName, "NumeralLanguage", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.NumeralLanguageExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			CultureInfo cultureInfo = default(CultureInfo);
			return ProcessingValidator.ValidateLanguage(this.ProcessStringResult(result).Value, (IErrorContext)this, out cultureInfo);
		}

		internal object EvaluateStyleNumeralVariant(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, true, objectType, objectName, "NumeralVariant", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.NumeralVariantExpr;
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
			return ProcessingValidator.ValidateNumeralVariant(integerResult.Value, this);
		}

		internal string EvaluateStyleBackgroundUrlImageValue(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, true, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
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

		internal string EvaluateStyleBackgroundEmbeddedImageValue(Style style, ExpressionInfo expression, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, true, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageValueExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateEmbeddedImageName(this.ProcessStringResult(result).Value, embeddedImages, this);
		}

		internal byte[] EvaluateStyleBackgroundDatabaseImageValue(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, true, objectType, objectName, "BackgroundImageValue", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
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

		internal string EvaluateStyleBackgroundImageMIMEType(Style style, ExpressionInfo expression, ObjectType objectType, string objectName)
		{
			VariantResult result = default(VariantResult);
			if (!this.EvaluateIntegerExpression(expression, true, objectType, objectName, "BackgroundImageMIMEType", out result))
			{
				try
				{
					if (!this.EvaluateComplexExpression(expression, ref result))
					{
						Global.Tracer.Assert(style.ExprHost != null);
						result.Value = style.ExprHost.BackgroundImageMIMETypeExpr;
					}
				}
				catch (Exception e)
				{
					this.RegisterRuntimeErrorInExpression(ref result, e);
				}
			}
			return ProcessingValidator.ValidateMimeType(this.ProcessStringResult(result).Value, this);
		}

		private bool EvaluateSimpleExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
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

		private bool EvaluateSimpleExpression(ExpressionInfo expression, out VariantResult result)
		{
			result = default(VariantResult);
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ExpressionInfo.Types.Constant:
					result.Value = expression.Value;
					return true;
				case ExpressionInfo.Types.Field:
					try
					{
						FieldImpl fieldImpl = this.m_reportObjectModel.FieldsImpl[expression.IntValue];
						if (fieldImpl.IsMissing)
						{
							result.Value = null;
							return true;
						}
						if (fieldImpl.FieldStatus != 0)
						{
							result.ErrorOccurred = true;
							result.FieldStatus = fieldImpl.FieldStatus;
							result.ExceptionMessage = fieldImpl.ExceptionMessage;
							result.Value = null;
							return true;
						}
						result.Value = this.m_reportObjectModel.FieldsImpl[expression.IntValue].Value;
					}
					catch (ReportProcessingException_NoRowsFieldAccess e)
					{
						this.RegisterRuntimeWarning(e, this);
						result.Value = null;
						return true;
					}
					return true;
				case ExpressionInfo.Types.Token:
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = ((DataSets)this.m_reportObjectModel.DataSetsImpl)[expression.Value];
					result.Value = dataSet.RewrittenCommandText;
					return true;
				}
				case ExpressionInfo.Types.Aggregate:
					return false;
				case ExpressionInfo.Types.Expression:
					return false;
				default:
					Global.Tracer.Assert(false);
					return true;
				}
			}
			return true;
		}

		private bool EvaluateComplexExpression(ExpressionInfo expression, ref VariantResult result)
		{
			if (expression != null)
			{
				switch (expression.Type)
				{
				case ExpressionInfo.Types.Aggregate:
					result.Value = ((Aggregates)this.m_reportObjectModel.AggregatesImpl)[expression.Value];
					return true;
				case ExpressionInfo.Types.Expression:
					return false;
				default:
					Global.Tracer.Assert(false);
					return true;
				}
			}
			return true;
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
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e)
		{
			this.RegisterRuntimeErrorInExpression(ref result, e, (IErrorContext)this, false);
		}

		private void RegisterRuntimeErrorInExpression(ref VariantResult result, Exception e, IErrorContext iErrorContext, bool isError)
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
				}
			}
		}

		private bool EvaluateBooleanExpression(ExpressionInfo expression, bool canBeConstant, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.BoolValue;
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
			return this.ProcessBooleanResult(result, false, ObjectType.Report, null);
		}

		private BooleanResult ProcessBooleanResult(VariantResult result, bool stopOnError, ObjectType objectType, string objectName)
		{
			BooleanResult result2 = default(BooleanResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				if (stopOnError && result.FieldStatus != 0)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsFieldErrorInExpression, Severity.Error, objectType, objectName, "Hidden", ReportRuntime.GetErrorName(result.FieldStatus, result.ExceptionMessage));
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
			}
			else if (result.Value is bool)
			{
				result2.Value = (bool)result.Value;
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = false;
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

		private bool EvaluateBinaryExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return this.EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private BinaryResult ProcessBinaryResult(VariantResult result)
		{
			BinaryResult result2 = default(BinaryResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is byte[])
			{
				result2.Value = (byte[])result.Value;
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = null;
			}
			else
			{
				result2.ErrorOccurred = true;
				this.RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private StringResult ProcessStringResult(VariantResult result)
		{
			StringResult result2 = default(StringResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is string)
			{
				result2.Value = (string)result.Value;
			}
			else if (result.Value is char)
			{
				result2.Value = new string((char)result.Value, 1);
			}
			else if (result.Value == null || DBNull.Value == result.Value)
			{
				result2.Value = null;
			}
			else if (result.Value is Guid)
			{
				result.Value = ((Guid)result.Value).ToString();
			}
			else
			{
				result2.ErrorOccurred = true;
				this.RegisterInvalidExpressionDataTypeWarning();
			}
			return result2;
		}

		private void ProcessLabelResult(ref VariantResult result)
		{
			if (!result.ErrorOccurred && !(result.Value is string))
			{
				if (result.Value is char)
				{
					result.Value = new string((char)result.Value, 1);
				}
				else if (result.Value == null || DBNull.Value == result.Value)
				{
					result.Value = null;
				}
				else if (result.Value is Guid)
				{
					result.Value = ((Guid)result.Value).ToString();
				}
				else
				{
					result.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
				}
			}
		}

		private bool EvaluateIntegerExpression(ExpressionInfo expression, bool canBeConstant, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = default(VariantResult);
				if (canBeConstant)
				{
					result.Value = expression.IntValue;
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

		private IntegerResult ProcessIntegerResult(VariantResult result)
		{
			IntegerResult result2 = default(IntegerResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is int)
			{
				result2.Value = (int)result.Value;
			}
			else if (result.Value is byte)
			{
				result2.Value = Convert.ToInt32((byte)result.Value);
			}
			else if (result.Value is sbyte)
			{
				result2.Value = Convert.ToInt32((sbyte)result.Value);
			}
			else if (result.Value is short)
			{
				result2.Value = Convert.ToInt32((short)result.Value);
			}
			else if (result.Value is ushort)
			{
				result2.Value = Convert.ToInt32((ushort)result.Value);
			}
			else
			{
				if (result.Value is uint)
				{
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
				}
				if (result.Value is long)
				{
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
				}
				if (result.Value is ulong)
				{
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
				}
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
			}
			return result2;
		}

		private bool EvaluateIntegerOrFloatExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			return this.EvaluateNoConstantExpression(expression, objectType, objectName, propertyName, out result);
		}

		private FloatResult ProcessIntegerOrFloatResult(VariantResult result)
		{
			FloatResult result2 = default(FloatResult);
			if (result.ErrorOccurred)
			{
				result2.ErrorOccurred = true;
				result2.FieldStatus = result.FieldStatus;
			}
			else if (result.Value is int)
			{
				result2.Value = (double)(int)result.Value;
			}
			else if (result.Value is byte)
			{
				result2.Value = (double)Convert.ToInt32((byte)result.Value);
			}
			else if (result.Value is sbyte)
			{
				result2.Value = (double)Convert.ToInt32((sbyte)result.Value);
			}
			else if (result.Value is short)
			{
				result2.Value = (double)Convert.ToInt32((short)result.Value);
			}
			else if (result.Value is ushort)
			{
				result2.Value = (double)Convert.ToInt32((ushort)result.Value);
			}
			else
			{
				if (result.Value is uint)
				{
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
				}
				if (result.Value is long)
				{
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
				}
				if (result.Value is ulong)
				{
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
				}
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
				if (result.Value is double)
				{
					result2.Value = (double)result.Value;
				}
				else if (result.Value is float)
				{
					result2.Value = Convert.ToDouble((float)result.Value);
				}
				else
				{
					if (result.Value is decimal)
					{
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
					}
					result2.ErrorOccurred = true;
					this.RegisterInvalidExpressionDataTypeWarning();
				}
			}
			return result2;
		}

		private void ProcessVariantResult(ExpressionInfo expression, ref VariantResult result)
		{
			this.ProcessVariantResult(expression, ref result, false);
		}

		private void ProcessVariantResult(ExpressionInfo expression, ref VariantResult result, bool isArrayAllowed)
		{
			if (expression != null && expression.Type != ExpressionInfo.Types.Constant && !result.ErrorOccurred && !ReportRuntime.IsVariant(result.Value))
			{
				if (result.Value == null || result.Value == DBNull.Value)
				{
					result.Value = null;
				}
				else
				{
					if (isArrayAllowed && result.Value is ICollection)
					{
						return;
					}
					if (result.Value is Guid)
					{
						result.Value = ((Guid)result.Value).ToString();
					}
					else
					{
						result.ErrorOccurred = true;
						result.Value = null;
						this.RegisterInvalidExpressionDataTypeWarning();
					}
				}
			}
		}

		private void ProcessVariantOrBinaryResult(ExpressionInfo expression, ref VariantResult result, IErrorContext iErrorContext, bool isAggregate)
		{
			if (expression != null && expression.Type != ExpressionInfo.Types.Constant && !result.ErrorOccurred && !ReportRuntime.IsVariant(result.Value) && !(result.Value is byte[]))
			{
				if (result.Value == null || result.Value == DBNull.Value)
				{
					result.Value = null;
				}
				else if (result.Value is Guid)
				{
					result.Value = ((Guid)result.Value).ToString();
				}
				else
				{
					result.ErrorOccurred = true;
					result.Value = null;
					if (!isAggregate)
					{
						this.RegisterInvalidExpressionDataTypeWarning();
					}
				}
			}
		}

		private ParameterValueResult ProcessParameterValueResult(ExpressionInfo expression, VariantResult result)
		{
			ParameterValueResult result2 = default(ParameterValueResult);
			DataAggregate.DataTypeCode dataTypeCode = DataAggregate.DataTypeCode.Null;
			if (result.Value is Guid)
			{
				result.Value = ((Guid)result.Value).ToString();
			}
			if (!(result.Value is object[]))
			{
				dataTypeCode = DataAggregate.GetTypeCode(result.Value);
			}
			if (expression != null)
			{
				if (expression.Type == ExpressionInfo.Types.Constant)
				{
					result2.Value = expression.Value;
					result2.Type = DataType.String;
				}
				else if (result.ErrorOccurred)
				{
					result2.ErrorOccurred = true;
				}
				else
				{
					switch (dataTypeCode)
					{
					case DataAggregate.DataTypeCode.Boolean:
						result2.Value = result.Value;
						result2.Type = DataType.Boolean;
						break;
					case DataAggregate.DataTypeCode.DateTime:
						result2.Value = result.Value;
						result2.Type = DataType.DateTime;
						break;
					case DataAggregate.DataTypeCode.Single:
					case DataAggregate.DataTypeCode.Double:
					case DataAggregate.DataTypeCode.Decimal:
						result2.Value = Convert.ToDouble(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.Float;
						break;
					case DataAggregate.DataTypeCode.String:
					case DataAggregate.DataTypeCode.Char:
						result2.Value = Convert.ToString(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.String;
						break;
					case DataAggregate.DataTypeCode.Int16:
					case DataAggregate.DataTypeCode.Int32:
					case DataAggregate.DataTypeCode.UInt16:
					case DataAggregate.DataTypeCode.Byte:
					case DataAggregate.DataTypeCode.SByte:
						result2.Value = Convert.ToInt32(result.Value, CultureInfo.CurrentCulture);
						result2.Type = DataType.Integer;
						break;
					case DataAggregate.DataTypeCode.TimeSpan:
						try
						{
							result2.Value = Convert.ToInt32(((TimeSpan)result.Value).Ticks);
							result2.Type = DataType.Integer;
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					case DataAggregate.DataTypeCode.Int64:
					case DataAggregate.DataTypeCode.UInt32:
					case DataAggregate.DataTypeCode.UInt64:
						try
						{
							result2.Value = Convert.ToInt32(result.Value, CultureInfo.CurrentCulture);
							result2.Type = DataType.Integer;
							return result2;
						}
						catch (OverflowException)
						{
							result2.ErrorOccurred = true;
							return result2;
						}
					default:
						if (result.Value == null || DBNull.Value == result.Value)
						{
							result2.Value = null;
							result2.Type = DataType.String;
						}
						else if (result.Value is object[])
						{
							result2.Value = result.Value;
							object[] array = result.Value as object[];
							Global.Tracer.Assert(null != array);
							result2.Type = this.GetDataType(DataAggregate.GetTypeCode(array[0]));
						}
						else
						{
							result2.ErrorOccurred = true;
							this.RegisterInvalidExpressionDataTypeWarning();
						}
						break;
					}
				}
			}
			return result2;
		}

		private DataType GetDataType(DataAggregate.DataTypeCode typecode)
		{
			switch (typecode)
			{
			case DataAggregate.DataTypeCode.Boolean:
				return DataType.Boolean;
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.Int64:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.UInt64:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
			case DataAggregate.DataTypeCode.TimeSpan:
				return DataType.Integer;
			case DataAggregate.DataTypeCode.Single:
			case DataAggregate.DataTypeCode.Double:
			case DataAggregate.DataTypeCode.Decimal:
				return DataType.Float;
			case DataAggregate.DataTypeCode.Null:
			case DataAggregate.DataTypeCode.String:
			case DataAggregate.DataTypeCode.Char:
				return DataType.String;
			case DataAggregate.DataTypeCode.DateTime:
				return DataType.DateTime;
			default:
				return DataType.String;
			}
		}

		private bool EvaluateNoConstantExpression(ExpressionInfo expression, ObjectType objectType, string objectName, string propertyName, out VariantResult result)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Constant)
			{
				result = new VariantResult(true, null);
				this.RegisterInvalidExpressionDataTypeWarning();
				return true;
			}
			return this.EvaluateSimpleExpression(expression, objectType, objectName, propertyName, out result);
		}

		internal static bool IsVariant(object o)
		{
			if (!(o is string) && !(o is int) && !(o is decimal) && !(o is DateTime) && !(o is double) && !(o is float) && !(o is short) && !(o is bool) && !(o is byte) && !(o is TimeSpan) && !(o is sbyte) && !(o is long) && !(o is ushort) && !(o is uint) && !(o is ulong))
			{
				return o is char;
			}
			return true;
		}

		private void RegisterInvalidExpressionDataTypeWarning()
		{
			((IErrorContext)this).Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning, new string[0]);
		}

		internal bool InScope(string scope)
		{
			if (this.m_currentScope == null)
			{
				return false;
			}
			return this.m_currentScope.InScope(scope);
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

		internal void LoadCompiledCode(Report report, bool parametersOnly, ObjectModelImpl reportObjectModel, ReportRuntimeSetup runtimeSetup)
		{
			Global.Tracer.Assert(report.CompiledCode != null && this.m_exprHostAssembly == null && this.m_reportExprHost == null);
			if (report.CompiledCode.Length > 0)
			{
				ProcessingErrorCode errorCode = ProcessingErrorCode.rsErrorLoadingExprHostAssembly;
				try
				{
					if (runtimeSetup.RequireExpressionHostWithRefusedPermissions && !report.CompiledCodeGeneratedWithRefusedPermissions)
					{
						if (Global.Tracer.TraceError)
						{
							Global.Tracer.Trace("Expression host generated with refused permissions is required.");
						}
						throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
					}
					if (runtimeSetup.ExprHostAppDomain == null || runtimeSetup.ExprHostAppDomain == AppDomain.CurrentDomain)
					{
						if (report.CodeModules != null)
						{
							for (int i = 0; i < report.CodeModules.Count; i++)
							{
								if (!runtimeSetup.CheckCodeModuleIsTrustedInCurrentAppDomain(report.CodeModules[i]))
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsUntrustedCodeModule, Severity.Error, ObjectType.Report, null, null, report.CodeModules[i]);
									throw new ReportProcessingException(this.m_errorContext.Messages);
								}
							}
						}
						this.m_reportExprHost = ExpressionHostLoader.LoadExprHostIntoCurrentAppDomain(report.CompiledCode, report.ExprHostAssemblyName, runtimeSetup.ExprHostEvidence, parametersOnly, reportObjectModel, report.CodeModules);
					}
					else
					{
						this.m_reportExprHost = ExpressionHostLoader.LoadExprHost(report.CompiledCode, report.ExprHostAssemblyName, parametersOnly, reportObjectModel, report.CodeModules, runtimeSetup.ExprHostAppDomain);
					}
					errorCode = ProcessingErrorCode.rsErrorInOnInit;
					this.m_reportExprHost.CustomCodeOnInit();
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception e)
				{
					this.ProcessLoadingExprHostException(e, errorCode);
				}
			}
		}

		private void ProcessLoadingExprHostException(Exception e, ProcessingErrorCode errorCode)
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
			ProcessingMessage processingMessage = this.m_errorContext.Register(errorCode, Severity.Error, ObjectType.Report, null, null, text2);
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
	}
}
