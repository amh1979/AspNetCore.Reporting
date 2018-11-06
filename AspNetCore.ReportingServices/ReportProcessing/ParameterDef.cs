using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterDef : ParameterBase, IParameterDef
	{
		private ParameterDataSource m_validValuesDataSource;

		private ExpressionInfoList m_validValuesValueExpressions;

		private ExpressionInfoList m_validValuesLabelExpressions;

		private ParameterDataSource m_defaultDataSource;

		private ExpressionInfoList m_defaultExpressions;

		private ParameterDefList m_dependencyList;

		private int m_exprHostID = -1;

		private string m_prompt;

		[NonSerialized]
		private ReportParamExprHost m_exprHost;

		internal ExpressionInfoList DefaultExpressions
		{
			get
			{
				return this.m_defaultExpressions;
			}
			set
			{
				this.m_defaultExpressions = value;
			}
		}

		internal ParameterDataSource ValidValuesDataSource
		{
			get
			{
				return this.m_validValuesDataSource;
			}
			set
			{
				this.m_validValuesDataSource = value;
			}
		}

		internal ExpressionInfoList ValidValuesValueExpressions
		{
			get
			{
				return this.m_validValuesValueExpressions;
			}
			set
			{
				this.m_validValuesValueExpressions = value;
			}
		}

		internal ExpressionInfoList ValidValuesLabelExpressions
		{
			get
			{
				return this.m_validValuesLabelExpressions;
			}
			set
			{
				this.m_validValuesLabelExpressions = value;
			}
		}

		internal ParameterDataSource DefaultDataSource
		{
			get
			{
				return this.m_defaultDataSource;
			}
			set
			{
				this.m_defaultDataSource = value;
			}
		}

		internal ParameterDefList DependencyList
		{
			get
			{
				return this.m_dependencyList;
			}
			set
			{
				this.m_dependencyList = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal ReportParamExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		public override string Prompt
		{
			get
			{
				return this.m_prompt;
			}
			set
			{
				this.m_prompt = value;
			}
		}

		int IParameterDef.DefaultValuesExpressionCount
		{
			get
			{
				if (this.DefaultExpressions == null)
				{
					return 0;
				}
				return this.DefaultExpressions.Count;
			}
		}

		int IParameterDef.ValidValuesValueExpressionCount
		{
			get
			{
				if (this.ValidValuesValueExpressions == null)
				{
					return 0;
				}
				return this.ValidValuesValueExpressions.Count;
			}
		}

		int IParameterDef.ValidValuesLabelExpressionCount
		{
			get
			{
				if (this.ValidValuesLabelExpressions == null)
				{
					return 0;
				}
				return this.ValidValuesLabelExpressions.Count;
			}
		}

		string IParameterDef.Name
		{
			get
			{
				return base.Name;
			}
		}

		ObjectType IParameterDef.ParameterObjectType
		{
			get
			{
				return ObjectType.ReportParameter;
			}
		}

		DataType IParameterDef.DataType
		{
			get
			{
				return base.DataType;
			}
		}

		bool IParameterDef.MultiValue
		{
			get
			{
				return base.MultiValue;
			}
		}

		IParameterDataSource IParameterDef.DefaultDataSource
		{
			get
			{
				return this.DefaultDataSource;
			}
		}

		IParameterDataSource IParameterDef.ValidValuesDataSource
		{
			get
			{
				return this.ValidValuesDataSource;
			}
		}

		bool IParameterDef.HasDefaultValuesExpressions()
		{
			return this.DefaultExpressions != null;
		}

		bool IParameterDef.HasValidValuesValueExpressions()
		{
			return this.ValidValuesValueExpressions != null;
		}

		bool IParameterDef.HasValidValuesLabelExpressions()
		{
			return this.ValidValuesLabelExpressions != null;
		}

		bool IParameterDef.HasDefaultValuesDataSource()
		{
			return this.DefaultDataSource != null;
		}

		bool IParameterDef.HasValidValuesDataSource()
		{
			return this.ValidValuesDataSource != null;
		}

		bool IParameterDef.ValidateValueForNull(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ParameterBase.ValidateValueForNull(newValue, base.Nullable, errorContext, ObjectType.ReportParameter, base.Name, parameterValueProperty);
		}

		bool IParameterDef.ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return base.ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ReportParameterStart(base.Name);
			if (this.m_defaultExpressions != null)
			{
				for (int num = this.m_defaultExpressions.Count - 1; num >= 0; num--)
				{
					context.ExprHostBuilder.ReportParameterDefaultValue(this.m_defaultExpressions[num]);
				}
			}
			if (this.m_validValuesValueExpressions != null)
			{
				context.ExprHostBuilder.ReportParameterValidValuesStart();
				for (int num2 = this.m_validValuesValueExpressions.Count - 1; num2 >= 0; num2--)
				{
					ExpressionInfo expressionInfo = this.m_validValuesValueExpressions[num2];
					if (expressionInfo != null)
					{
						context.ExprHostBuilder.ReportParameterValidValue(expressionInfo);
					}
				}
				context.ExprHostBuilder.ReportParameterValidValuesEnd();
			}
			if (this.m_validValuesLabelExpressions != null)
			{
				context.ExprHostBuilder.ReportParameterValidValueLabelsStart();
				for (int num3 = this.m_validValuesLabelExpressions.Count - 1; num3 >= 0; num3--)
				{
					ExpressionInfo expressionInfo2 = this.m_validValuesLabelExpressions[num3];
					if (expressionInfo2 != null)
					{
						context.ExprHostBuilder.ReportParameterValidValueLabel(expressionInfo2);
					}
				}
				context.ExprHostBuilder.ReportParameterValidValueLabelsEnd();
			}
			this.ExprHostID = context.ExprHostBuilder.ReportParameterEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModel reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			if (this.ExprHostID >= 0)
			{
				this.m_exprHost = reportExprHost.ReportParameterHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_exprHost.ValidValuesHost != null)
				{
					this.m_exprHost.ValidValuesHost.SetReportObjectModel(reportObjectModel);
				}
				if (this.m_exprHost.ValidValueLabelsHost != null)
				{
					this.m_exprHost.ValidValueLabelsHost.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, string prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			base.Parse(name, defaultValues, type, nullable, prompt, promptUser, allowBlank, multiValue, usedInQuery, hidden, errorContext, language);
			if (hidden)
			{
				this.m_prompt = "";
			}
			else if (prompt == null)
			{
				this.m_prompt = name + ":";
			}
			else
			{
				this.m_prompt = prompt;
			}
			if (this.m_validValuesValueExpressions == null)
			{
				return;
			}
			if (DataType.Boolean == base.DataType)
			{
				return;
			}
			int num = this.m_validValuesValueExpressions.Count - 1;
			while (true)
			{
				if (num >= 0)
				{
					ExpressionInfo expressionInfo = this.m_validValuesValueExpressions[num];
					if (expressionInfo == null && base.MultiValue)
					{
						this.m_validValuesValueExpressions.RemoveAt(num);
					}
					else if (expressionInfo != null && ExpressionInfo.Types.Constant == expressionInfo.Type)
					{
						object newValue = default(object);
						if (!ParameterBase.CastFromString(expressionInfo.Value, out newValue, base.DataType, language))
						{
							if (errorContext == null)
							{
								break;
							}
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, base.ParameterObjectType, name, "ValidValue");
						}
						else
						{
							base.ValidateValue(newValue, errorContext, base.ParameterObjectType, "ValidValue");
						}
					}
					num--;
					continue;
				}
				return;
			}
			throw new ReportParameterTypeMismatchException(name);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesDataSource, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDataSource));
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesValueExpression, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesLabelExpression, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DefaultValueDataSource, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDataSource));
			memberInfoList.Add(new MemberInfo(MemberName.ExpressionList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DependencyList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDefList));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterBase, memberInfoList);
		}
	}
}
