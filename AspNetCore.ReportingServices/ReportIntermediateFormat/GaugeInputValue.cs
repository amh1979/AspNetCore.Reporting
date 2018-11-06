using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class GaugeInputValue : IPersistable
	{
		[NonSerialized]
		private GaugeInputValueExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeInputValue.GetDeclaration();

		private ExpressionInfo m_value;

		private ExpressionInfo m_formula;

		private ExpressionInfo m_minPercent;

		private ExpressionInfo m_maxPercent;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_addConstant;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private int m_exprHostID = -1;

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal ExpressionInfo Formula
		{
			get
			{
				return this.m_formula;
			}
			set
			{
				this.m_formula = value;
			}
		}

		internal ExpressionInfo MinPercent
		{
			get
			{
				return this.m_minPercent;
			}
			set
			{
				this.m_minPercent = value;
			}
		}

		internal ExpressionInfo MaxPercent
		{
			get
			{
				return this.m_maxPercent;
			}
			set
			{
				this.m_maxPercent = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return this.m_multiplier;
			}
			set
			{
				this.m_multiplier = value;
			}
		}

		internal ExpressionInfo AddConstant
		{
			get
			{
				return this.m_addConstant;
			}
			set
			{
				this.m_addConstant = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		internal GaugeInputValueExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal GaugeInputValue()
		{
		}

		internal GaugeInputValue(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.GaugeInputValueStart(index);
			if (this.m_value != null)
			{
				this.InitializeValue(context);
			}
			if (this.m_formula != null)
			{
				this.m_formula.Initialize("Formula", context);
				context.ExprHostBuilder.GaugeInputValueFormula(this.m_formula);
			}
			if (this.m_minPercent != null)
			{
				this.m_minPercent.Initialize("MinPercent", context);
				context.ExprHostBuilder.GaugeInputValueMinPercent(this.m_minPercent);
			}
			if (this.m_maxPercent != null)
			{
				this.m_maxPercent.Initialize("MaxPercent", context);
				context.ExprHostBuilder.GaugeInputValueMaxPercent(this.m_maxPercent);
			}
			if (this.m_multiplier != null)
			{
				this.m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.GaugeInputValueMultiplier(this.m_multiplier);
			}
			if (this.m_addConstant != null)
			{
				this.m_addConstant.Initialize("AddConstant", context);
				context.ExprHostBuilder.GaugeInputValueAddConstant(this.m_addConstant);
			}
			this.m_exprHostID = context.ExprHostBuilder.GaugeInputValueEnd();
		}

		protected virtual void InitializeValue(InitializationContext context)
		{
			this.m_value.Initialize("Value", context);
			context.ExprHostBuilder.GaugeInputValueValue(this.m_value);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeInputValue gaugeInputValue = (GaugeInputValue)base.MemberwiseClone();
			gaugeInputValue.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (this.m_value != null)
			{
				gaugeInputValue.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_formula != null)
			{
				gaugeInputValue.m_formula = (ExpressionInfo)this.m_formula.PublishClone(context);
			}
			if (this.m_minPercent != null)
			{
				gaugeInputValue.m_minPercent = (ExpressionInfo)this.m_minPercent.PublishClone(context);
			}
			if (this.m_maxPercent != null)
			{
				gaugeInputValue.m_maxPercent = (ExpressionInfo)this.m_maxPercent.PublishClone(context);
			}
			if (this.m_multiplier != null)
			{
				gaugeInputValue.m_multiplier = (ExpressionInfo)this.m_multiplier.PublishClone(context);
			}
			if (this.m_addConstant != null)
			{
				gaugeInputValue.m_addConstant = (ExpressionInfo)this.m_addConstant.PublishClone(context);
			}
			return gaugeInputValue;
		}

		internal void SetExprHost(GaugeInputValueExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Formula, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Multiplier, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AddConstant, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GaugeInputValue.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(this.m_gaugePanel);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.Formula:
					writer.Write(this.m_formula);
					break;
				case MemberName.MinPercent:
					writer.Write(this.m_minPercent);
					break;
				case MemberName.MaxPercent:
					writer.Write(this.m_maxPercent);
					break;
				case MemberName.Multiplier:
					writer.Write(this.m_multiplier);
					break;
				case MemberName.AddConstant:
					writer.Write(this.m_addConstant);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GaugeInputValue.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					this.m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Formula:
					this.m_formula = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinPercent:
					this.m_minPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxPercent:
					this.m_maxPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					this.m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AddConstant:
					this.m_addConstant = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue;
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(GaugeInputValue.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.GaugePanel)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueValueExpression(this, this.m_gaugePanel.Name);
		}

		internal GaugeInputValueFormulas EvaluateFormula(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return EnumTranslator.TranslateGaugeInputValueFormulas(context.ReportRuntime.EvaluateGaugeInputValueFormulaExpression(this, this.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateMinPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMinPercentExpression(this, this.m_gaugePanel.Name);
		}

		internal double EvaluateMaxPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMaxPercentExpression(this, this.m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMultiplierExpression(this, this.m_gaugePanel.Name);
		}

		internal double EvaluateAddConstant(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(this.m_gaugePanel.GaugeRow != null && this.m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(this.m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueAddConstantExpression(this, this.m_gaugePanel.Name);
		}
	}
}
