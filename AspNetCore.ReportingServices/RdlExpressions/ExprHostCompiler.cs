using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;

using System.Linq;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal sealed class ExprHostCompiler
	{
		private sealed class ExprCompileTimeInfo
		{
			internal AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ExpressionInfo;

			internal AspNetCore.ReportingServices.ReportProcessing.ObjectType OwnerObjectType;

			internal string OwnerObjectName;

			internal string OwnerPropertyName;

			internal int NumErrors;

			internal int NumWarnings;

			internal ExprCompileTimeInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
			{
				this.ExpressionInfo = expression;
				this.OwnerObjectType = context.ObjectType;
				this.OwnerObjectName = context.ObjectName;
				this.OwnerPropertyName = context.PropertyName;
				this.NumErrors = 0;
				this.NumWarnings = 0;
			}
		}

		private sealed class ExprCompileTimeInfoList : ArrayList
		{
			internal new ExprCompileTimeInfo this[int exprCTId]
			{
				get
				{
					return (ExprCompileTimeInfo)base[exprCTId];
				}
			}
		}

		private sealed class CodeModuleClassInstanceDeclCompileTimeInfo
		{
			internal int NumErrors;

			internal int NumWarnings;
		}

		private sealed class CodeModuleClassInstanceDeclCompileTimeInfoList : Hashtable
		{
			internal new CodeModuleClassInstanceDeclCompileTimeInfo this[object id]
			{
				get
				{
					CodeModuleClassInstanceDeclCompileTimeInfo codeModuleClassInstanceDeclCompileTimeInfo = (CodeModuleClassInstanceDeclCompileTimeInfo)base[id];
					if (codeModuleClassInstanceDeclCompileTimeInfo == null)
					{
						codeModuleClassInstanceDeclCompileTimeInfo = new CodeModuleClassInstanceDeclCompileTimeInfo();
						base.Add(id, codeModuleClassInstanceDeclCompileTimeInfo);
					}
					return codeModuleClassInstanceDeclCompileTimeInfo;
				}
			}
		}

		private ExpressionParser m_langParser;

		private ErrorContext m_errorContext;

		private ExprHostBuilder m_builder;

		private ExprCompileTimeInfoList m_ctExprList;

		private CodeModuleClassInstanceDeclCompileTimeInfoList m_ctClassInstDeclList;

		private int m_customCodeNumErrors;

		private int m_customCodeNumWarnings;

		private ArrayList m_reportLevelFieldReferences;

		private IExpressionHostAssemblyHolder m_expressionHostAssemblyHolder;

		internal ExprHostBuilder Builder
		{
			get
			{
				return this.m_builder;
			}
		}

		internal bool BodyRefersToReportItems
		{
			get
			{
				return this.m_langParser.BodyRefersToReportItems;
			}
		}

		internal bool PageSectionRefersToReportItems
		{
			get
			{
				return this.m_langParser.PageSectionRefersToReportItems;
			}
		}

		internal bool PageSectionRefersToOverallTotalPages
		{
			get
			{
				return this.m_langParser.PageSectionRefersToOverallTotalPages;
			}
		}

		internal bool PageSectionRefersToTotalPages
		{
			get
			{
				return this.m_langParser.PageSectionRefersToTotalPages;
			}
		}

		internal int NumberOfAggregates
		{
			get
			{
				return this.m_langParser.NumberOfAggregates;
			}
		}

		internal int LastAggregateID
		{
			get
			{
				return this.m_langParser.LastID;
			}
		}

		internal int LastLookupID
		{
			get
			{
				return this.m_langParser.LastLookupID;
			}
		}

		internal bool PreviousAggregateUsed
		{
			get
			{
				return this.m_langParser.PreviousAggregateUsed;
			}
		}

		internal bool AggregateOfAggregateUsed
		{
			get
			{
				return this.m_langParser.AggregateOfAggregatesUsed;
			}
		}

		internal bool AggregateOfAggregateUsedInUserSort
		{
			get
			{
				return this.m_langParser.AggregateOfAggregatesUsedInUserSort;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return this.m_langParser.ValueReferenced;
			}
		}

		internal bool ValueReferencedGlobal
		{
			get
			{
				return this.m_langParser.ValueReferencedGlobal;
			}
		}

		internal ExprHostCompiler(ExpressionParser langParser, ErrorContext errorContext)
		{
			this.m_langParser = langParser;
			this.m_errorContext = errorContext;
			this.m_builder = new ExprHostBuilder();
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionParser.EvaluationMode evaluationMode, ExpressionParser.ExpressionContext context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.m_langParser.ParseExpression(expression, context, evaluationMode);
			this.ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionParser.ExpressionContext context, ExpressionParser.EvaluationMode evaluationMode, out bool userCollectionReferenced)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.m_langParser.ParseExpression(expression, context, evaluationMode, out userCollectionReferenced);
			this.ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo CreateScopedFirstAggregate(string fieldName, string dataSetName)
		{
			return this.m_langParser.CreateScopedFirstAggregate(fieldName, dataSetName);
		}

		internal void ConvertFields2ComplexExpr()
		{
			if (this.m_reportLevelFieldReferences != null)
			{
				for (int num = this.m_reportLevelFieldReferences.Count - 1; num >= 0; num--)
				{
					ExprCompileTimeInfo exprCompileTimeInfo = (ExprCompileTimeInfo)this.m_reportLevelFieldReferences[num];
					this.m_langParser.ConvertField2ComplexExpr(ref exprCompileTimeInfo.ExpressionInfo);
					this.RegisterExpression(exprCompileTimeInfo);
				}
			}
		}

		internal void ResetValueReferencedFlag()
		{
			this.m_langParser.ResetValueReferencedFlag();
		}

		internal void ResetPageSectionRefersFlags()
		{
			this.m_langParser.ResetPageSectionRefersFlags();
		}

		internal byte[] Compile(IExpressionHostAssemblyHolder expressionHostAssemblyHolder, AppDomain compilationTempAppDomain, bool refusePermissions, PublishingVersioning versioning)
		{
            byte[] result = null;
            if (this.m_builder.HasExpressions && versioning.IsRdlFeatureRestricted(RdlFeatures.ComplexExpression))
            {
                this.m_errorContext.Register(ProcessingErrorCode.rsInvalidComplexExpressionInReport, Severity.Error, ReportProcessing.ObjectType.Report, "Report", "Body", new string[0]);
            }
            else
            {
                this.m_expressionHostAssemblyHolder = expressionHostAssemblyHolder;
                //todo: can delete?
                RevertImpersonationContext.Run(delegate
                {
                    result = this.InternalCompile(compilationTempAppDomain, refusePermissions);
                });
               // result = this.InternalCompile(compilationTempAppDomain, refusePermissions);
            }
            return result;
        }

		private void ProcessExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				this.RegisterExpression(new ExprCompileTimeInfo(expression, context));
				this.ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate)
			{
				this.ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field && context.Location == AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None)
			{
				if (this.m_reportLevelFieldReferences == null)
				{
					this.m_reportLevelFieldReferences = new ArrayList();
				}
				this.m_reportLevelFieldReferences.Add(new ExprCompileTimeInfo(expression, context));
			}
		}

		private void RegisterExpression(ExprCompileTimeInfo exprCTInfo)
		{
			if (this.m_ctExprList == null)
			{
				this.m_ctExprList = new ExprCompileTimeInfoList();
			}
			exprCTInfo.ExpressionInfo.CompileTimeID = this.m_ctExprList.Add(exprCTInfo);
		}

		private void ProcessAggregateParams(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression.Aggregates != null)
			{
				for (int num = expression.Aggregates.Count - 1; num >= 0; num--)
				{
					this.ProcessAggregateParam(expression.Aggregates[num], context);
				}
			}
			if (expression.RunningValues != null)
			{
				for (int num2 = expression.RunningValues.Count - 1; num2 >= 0; num2--)
				{
					this.ProcessAggregateParam(expression.RunningValues[num2], context);
				}
			}
		}

		private void ProcessAggregateParam(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, ExpressionParser.ExpressionContext context)
		{
			if (aggregate != null && aggregate.Expressions != null)
			{
				for (int i = 0; i < aggregate.Expressions.Length; i++)
				{
					this.ProcessAggregateParam(aggregate.Expressions[i], context);
				}
			}
		}

		private void ProcessAggregateParam(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression != null && expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				this.RegisterExpression(new ExprCompileTimeInfo(expression, context));
			}
		}

		private byte[] InternalCompile(AppDomain compilationTempAppDomain, bool refusePermissions)
		{            
			if (this.m_builder.HasExpressions)
			{                
				CompilerParameters compilerParameters = new CompilerParameters();
				compilerParameters.OutputAssembly = System.IO.Path.Combine(Path.GetTempPath(), this.m_expressionHostAssemblyHolder.ExprHostAssemblyName, "ExpressionHost.dll");
                compilerParameters.TempFiles = new TempFileCollection(Path.GetDirectoryName(compilerParameters.OutputAssembly));
                compilerParameters.GenerateExecutable = false;
				compilerParameters.GenerateInMemory = false;
				compilerParameters.IncludeDebugInformation = false;
                compilerParameters.ReferencedAssemblies.Add(typeof(AspNetCore.Reporting.InternalLocalReport).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add(AppDomain.CurrentDomain.GetAssemblies().Where(t => t.FullName.Contains("netstandard,")).FirstOrDefault().Location);
                //compilerParameters.ReferencedAssemblies.Add("System.dll");
                //compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
                //compilerParameters.ReferencedAssemblies.Add(Path.Combine(compilationTempAppDomain.BaseDirectory, "Microsoft.SqlServer.Types.dll"));                
                //compilerParameters.ReferencedAssemblies.Add(Path.Combine(compilationTempAppDomain.BaseDirectory, "Microsoft.ReportViewer.Common.dll"));
                //compilerParameters.ReferencedAssemblies.Add(Path.Combine(compilationTempAppDomain.BaseDirectory, "Microsoft.ReportViewer.ProcessingObjectModel.dll"));
                //compilerParameters.ReferencedAssemblies.Add(typeof(SqlGeography).Assembly.Location);
                //CompilerParameters compilerParameters2 = compilerParameters;
                compilerParameters.CompilerOptions += this.m_langParser.GetCompilerArguments();
				if (this.m_expressionHostAssemblyHolder.CodeModules != null)
				{
					this.ResolveAssemblylocations(this.m_expressionHostAssemblyHolder.CodeModules, compilerParameters, this.m_errorContext, compilationTempAppDomain);
				}
				CompilerResults compilerResults = null;
				try
				{
					ProcessingIntermediateFormatVersion version = new ProcessingIntermediateFormatVersion(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.Current);
					CodeCompileUnit exprHost = this.m_builder.GetExprHost(version, refusePermissions);
					this.m_expressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions = refusePermissions;
					CodeDomProvider codeCompiler = this.m_langParser.GetCodeCompiler();                    
                    compilerResults = codeCompiler.CompileAssemblyFromDom(compilerParameters, exprHost);
                    
                    if (compilerResults == null) throw new Exception("CompileAssemblyFromDom fail.");
					if (Global.Tracer.TraceVerbose)
					{
						try
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								IndentedTextWriter indentedTextWriter = new IndentedTextWriter(new StreamWriter(memoryStream), "    ");
								codeCompiler.GenerateCodeFromCompileUnit(exprHost, indentedTextWriter, new CodeGeneratorOptions());
								indentedTextWriter.Flush();
								memoryStream.Position = 0L;
								StreamReader streamReader = new StreamReader(memoryStream);
								Global.Tracer.Trace(streamReader.ReadToEnd().MarkAsPrivate());
							}
						}
						catch
						{
						}
					}
					if (compilerResults.NativeCompilerReturnValue == 0 && compilerResults.Errors.Count <= 0)
					{
						using (FileStream fileStream = File.OpenRead(compilerResults.PathToAssembly))
						{
							byte[] array = new byte[fileStream.Length];
							int num = fileStream.Read(array, 0, (int)fileStream.Length);
							Global.Tracer.Assert(num == fileStream.Length, "(read == fs.Length)");
							return array;
						}
					}
					this.ParseErrors(compilerResults);
					return new byte[0];
				}
				finally
                {
                    Directory.Delete(compilerParameters.TempFiles.TempDir, true);
				}
			}
			return new byte[0];
		}

		private void ResolveAssemblylocations(List<string> codeModules, CompilerParameters options, ErrorContext errorContext, AppDomain compilationTempAppDomain)
		{
			AssemblyLocationResolver assemblyLocationResolver = AssemblyLocationResolver.CreateResolver(compilationTempAppDomain);
			for (int num = codeModules.Count - 1; num >= 0; num--)
			{
				try
				{
					options.ReferencedAssemblies.Add(assemblyLocationResolver.LoadAssemblyAndResolveLocation(codeModules[num]));
				}
				catch (Exception ex)
				{
					ProcessingMessage processingMessage = errorContext.Register(ProcessingErrorCode.rsErrorLoadingCodeModule, Severity.Error, this.m_expressionHostAssemblyHolder.ObjectType, null, null, codeModules[num], ex.Message);
					if (Global.Tracer.TraceError && processingMessage != null)
					{
						Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + ex.ToString());
					}
				}
			}
		}

        private void ParseErrors(CompilerResults results)
        {
            int count = results.Errors.Count;
            if (results.NativeCompilerReturnValue != 0 && count == 0)
            {
                this.m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, this.m_expressionHostAssemblyHolder.ObjectType, null, null, results.NativeCompilerReturnValue.ToString(CultureInfo.InvariantCulture));
            }
            for (int i = 0; i < count; i++)
            {
                CompilerError error = results.Errors[i];
                int num = default(int);
                switch (this.m_builder.ParseErrorSource(error, out num))
                {
                    case ExprHostBuilder.ErrorSource.Expression:

                        ExprCompileTimeInfo exprCompileTimeInfo = this.m_ctExprList[num];
                        this.RegisterError(error, ref exprCompileTimeInfo.NumErrors, ref exprCompileTimeInfo.NumWarnings, exprCompileTimeInfo.OwnerObjectType, exprCompileTimeInfo.OwnerObjectName, exprCompileTimeInfo.OwnerPropertyName, ProcessingErrorCode.rsCompilerErrorInExpression);
                        break;

                    case ExprHostBuilder.ErrorSource.CustomCode:
                        this.RegisterError(error, ref this.m_customCodeNumErrors, ref this.m_customCodeNumWarnings, this.m_expressionHostAssemblyHolder.ObjectType, (string)null, (string)null, ProcessingErrorCode.rsCompilerErrorInCode);
                        break;
                    case ExprHostBuilder.ErrorSource.CodeModuleClassInstanceDecl:

                        if (this.m_ctClassInstDeclList == null)
                        {
                            this.m_ctClassInstDeclList = new CodeModuleClassInstanceDeclCompileTimeInfoList();
                        }
                        CodeModuleClassInstanceDeclCompileTimeInfo codeModuleClassInstanceDeclCompileTimeInfo = this.m_ctClassInstDeclList[num];
                        this.RegisterError(error, ref codeModuleClassInstanceDeclCompileTimeInfo.NumErrors, ref codeModuleClassInstanceDeclCompileTimeInfo.NumWarnings, AspNetCore.ReportingServices.ReportProcessing.ObjectType.CodeClass, this.m_expressionHostAssemblyHolder.CodeClasses[num].ClassName, (string)null, ProcessingErrorCode.rsCompilerErrorInClassInstanceDeclaration);
                        break;

                    default:
                        this.m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, this.m_expressionHostAssemblyHolder.ObjectType, null, null, this.FormatError(error));
                        throw new ReportProcessingException(this.m_errorContext.Messages);
                }
            }
        }

		private void RegisterError(CompilerError error, ref int numErrors, ref int numWarnings, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, ProcessingErrorCode errorCode)
		{
			if ((error.IsWarning ? numWarnings : numErrors) < 1)
			{
				bool flag = false;
				Severity severity;
				if (error.IsWarning)
				{
					flag = true;
					severity = Severity.Warning;
					numWarnings++;
				}
				else
				{
					flag = true;
					severity = Severity.Error;
					numErrors++;
				}
				if (flag)
				{
					this.m_errorContext.Register(errorCode, severity, objectType, objectName, propertyName, this.FormatError(error), error.Line.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private string FormatError(CompilerError error)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", error.ErrorNumber, error.ErrorText);
		}
	}
}
