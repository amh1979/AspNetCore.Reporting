using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ParameterDef : ParameterBase, IPersistable, IParameterDef, IReferenceable
	{
		private ParameterDataSource m_validValuesDataSource;

		private List<ExpressionInfo> m_validValuesValueExpressions;

		private List<ExpressionInfo> m_validValuesLabelExpressions;

		private ParameterDataSource m_defaultDataSource;

		private List<ExpressionInfo> m_defaultExpressions;

		[Reference]
		private List<ParameterDef> m_dependencyList;

		private int m_exprHostID = -1;

		private ExpressionInfo m_prompt;

		private int m_referenceId = -2;

		[NonSerialized]
		private ReportParamExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ParameterDef.GetDeclaration();

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

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IParameterDef.ParameterObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter;
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

		public override string Prompt
		{
			get
			{
				return this.m_prompt.StringValue;
			}
			set
			{
				this.m_prompt = ExpressionInfo.CreateConstExpression(value);
			}
		}

		public ExpressionInfo PromptExpression
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

		internal List<ExpressionInfo> DefaultExpressions
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

		internal List<ExpressionInfo> ValidValuesValueExpressions
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

		internal List<ExpressionInfo> ValidValuesLabelExpressions
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

		internal List<ParameterDef> DependencyList
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

		public int ID
		{
			get
			{
				return this.m_referenceId;
			}
		}

		internal ParameterDef()
		{
		}

		internal ParameterDef(int referenceId)
		{
			this.m_referenceId = referenceId;
		}

		bool IParameterDef.HasDefaultValuesExpressions()
		{
			return this.DefaultExpressions != null;
		}

		bool IParameterDef.HasValidValuesLabelExpressions()
		{
			return this.ValidValuesLabelExpressions != null;
		}

		bool IParameterDef.HasValidValuesValueExpressions()
		{
			return this.ValidValuesValueExpressions != null;
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
			return ParameterBase.ValidateValueForNull(newValue, base.Nullable, errorContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, base.Name, parameterValueProperty);
		}

		bool IParameterDef.ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return base.ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal void Initialize(InitializationContext context)
		{
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InParameter;
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
			if (this.m_prompt != null)
			{
				context.ExprHostBuilder.ReportParameterPromptExpression(this.m_prompt);
			}
			this.ExprHostID = context.ExprHostBuilder.ReportParameterEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, OnDemandObjectModel reportObjectModel)
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

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, ExpressionInfo prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			base.Parse(name, defaultValues, type, nullable, prompt, promptUser, allowBlank, multiValue, usedInQuery, hidden, errorContext, language);
			if (hidden)
			{
				this.m_prompt = ExpressionInfo.CreateConstExpression("");
			}
			else if (prompt == null)
			{
				this.m_prompt = ExpressionInfo.CreateConstExpression(name + ":");
			}
			else
			{
				this.m_prompt = prompt;
			}
			this.ValidateExpressionDataTypes(this.m_validValuesValueExpressions, errorContext, name, "ValidValue", true, language);
			this.ValidateExpressionDataTypes(this.m_defaultExpressions, errorContext, name, "DefaultValue", false, language);
		}

		private void ValidateExpressionDataTypes(List<ExpressionInfo> expressions, ErrorContext errorContext, string paramName, string memberName, bool fromValidValues, CultureInfo language)
		{
			if (expressions != null)
			{
				int num = expressions.Count - 1;
				while (true)
				{
					if (num >= 0)
					{
						ExpressionInfo expressionInfo = expressions[num];
						if (fromValidValues && expressionInfo == null && base.MultiValue)
						{
							this.m_validValuesValueExpressions.RemoveAt(num);
						}
						else if (expressionInfo != null && ExpressionInfo.Types.Constant == expressionInfo.Type)
						{
							object obj = default(object);
							if (!ParameterBase.CastFromString(expressionInfo.StringValue, out obj, base.DataType, language))
							{
								if (errorContext == null)
								{
									break;
								}
								errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramName, memberName);
							}
							else
							{
								base.ValidateValue(obj, errorContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, memberName);
								if (obj != null && base.DataType != DataType.String)
								{
									ExpressionInfo expressionInfo2 = new ExpressionInfo();
									expressionInfo2.Type = ExpressionInfo.Types.Constant;
									expressionInfo2.OriginalText = expressionInfo.OriginalText;
									expressionInfo2.ConstantType = base.DataType;
									expressions[num] = expressionInfo2;
									switch (base.DataType)
									{
									case DataType.Boolean:
										expressionInfo2.BoolValue = (bool)obj;
										break;
									case DataType.DateTime:
										if (obj is DateTimeOffset)
										{
											expressionInfo2.SetDateTimeValue((DateTimeOffset)obj);
										}
										else
										{
											expressionInfo2.SetDateTimeValue((DateTime)obj);
										}
										break;
									case DataType.Float:
										expressionInfo2.FloatValue = (double)obj;
										break;
									case DataType.Integer:
										expressionInfo2.IntValue = (int)obj;
										break;
									}
								}
							}
						}
						num--;
						continue;
					}
					return;
				}
				throw new ReportParameterTypeMismatchException(paramName);
			}
		}

		[SkipMemberStaticValidation(MemberName.DependencyList)]
		private new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ValidValuesDataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource));
			list.Add(new MemberInfo(MemberName.ValidValuesValueExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValidValuesLabelExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DefaultValueDataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource));
			list.Add(new MemberInfo(MemberName.ExpressionList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DependencyList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.DependencyRefList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Prompt, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ReferenceID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, list);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ParameterDef.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Prompt:
					writer.Write(this.m_prompt);
					break;
				case MemberName.ValidValuesDataSource:
					writer.Write(this.m_validValuesDataSource);
					break;
				case MemberName.ValidValuesValueExpression:
					writer.Write(this.m_validValuesValueExpressions);
					break;
				case MemberName.ValidValuesLabelExpression:
					writer.Write(this.m_validValuesLabelExpressions);
					break;
				case MemberName.DefaultValueDataSource:
					writer.Write(this.m_defaultDataSource);
					break;
				case MemberName.ExpressionList:
					writer.Write(this.m_defaultExpressions);
					break;
				case MemberName.DependencyList:
					writer.Write((List<ParameterDef>)null);
					break;
				case MemberName.DependencyRefList:
					writer.WriteListOfReferences(this.m_dependencyList);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ReferenceID:
					writer.Write(this.m_referenceId);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ParameterDef.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Prompt:
					this.m_prompt = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValidValuesDataSource:
					this.m_validValuesDataSource = (ParameterDataSource)reader.ReadRIFObject();
					break;
				case MemberName.ValidValuesValueExpression:
					this.m_validValuesValueExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.ValidValuesLabelExpression:
					this.m_validValuesLabelExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DefaultValueDataSource:
					this.m_defaultDataSource = (ParameterDataSource)reader.ReadRIFObject();
					break;
				case MemberName.ExpressionList:
					this.m_defaultExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DependencyList:
				{
					List<ParameterDef> list = reader.ReadGenericListOfRIFObjects<ParameterDef>();
					if (list != null)
					{
						this.m_dependencyList = list;
					}
					break;
				}
				case MemberName.DependencyRefList:
					this.m_dependencyList = reader.ReadGenericListOfReferences<ParameterDef>(this);
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ReferenceID:
					this.m_referenceId = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ParameterDef.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item2 in list)
				{
					MemberName memberName = item2.MemberName;
					if (memberName == MemberName.DependencyRefList)
					{
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable);
						ParameterDef item = referenceable as ParameterDef;
						if (this.m_dependencyList == null)
						{
							this.m_dependencyList = new List<ParameterDef>();
						}
						this.m_dependencyList.Add(item);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef;
		}
	}
}
