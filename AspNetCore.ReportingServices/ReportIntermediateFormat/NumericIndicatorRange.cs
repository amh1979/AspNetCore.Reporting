using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class NumericIndicatorRange : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private NumericIndicatorRangeExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = NumericIndicatorRange.GetDeclaration();

		private string m_name;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_decimalDigitColor;

		private ExpressionInfo m_digitColor;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal GaugeInputValue StartValue
		{
			get
			{
				return this.m_startValue;
			}
			set
			{
				this.m_startValue = value;
			}
		}

		internal GaugeInputValue EndValue
		{
			get
			{
				return this.m_endValue;
			}
			set
			{
				this.m_endValue = value;
			}
		}

		internal ExpressionInfo DecimalDigitColor
		{
			get
			{
				return this.m_decimalDigitColor;
			}
			set
			{
				this.m_decimalDigitColor = value;
			}
		}

		internal ExpressionInfo DigitColor
		{
			get
			{
				return this.m_digitColor;
			}
			set
			{
				this.m_digitColor = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		internal NumericIndicatorRangeExprHost ExprHost
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

		internal NumericIndicatorRange()
		{
		}

		internal NumericIndicatorRange(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.NumericIndicatorRangeStart(this.m_name);
			if (this.m_decimalDigitColor != null)
			{
				this.m_decimalDigitColor.Initialize("DecimalDigitColor", context);
				context.ExprHostBuilder.NumericIndicatorRangeDecimalDigitColor(this.m_decimalDigitColor);
			}
			if (this.m_digitColor != null)
			{
				this.m_digitColor.Initialize("DigitColor", context);
				context.ExprHostBuilder.NumericIndicatorRangeDigitColor(this.m_digitColor);
			}
			this.m_exprHostID = context.ExprHostBuilder.NumericIndicatorRangeEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			NumericIndicatorRange numericIndicatorRange = (NumericIndicatorRange)base.MemberwiseClone();
			numericIndicatorRange.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (this.m_startValue != null)
			{
				numericIndicatorRange.m_startValue = (GaugeInputValue)this.m_startValue.PublishClone(context);
			}
			if (this.m_endValue != null)
			{
				numericIndicatorRange.m_endValue = (GaugeInputValue)this.m_endValue.PublishClone(context);
			}
			if (this.m_decimalDigitColor != null)
			{
				numericIndicatorRange.m_decimalDigitColor = (ExpressionInfo)this.m_decimalDigitColor.PublishClone(context);
			}
			if (this.m_digitColor != null)
			{
				numericIndicatorRange.m_digitColor = (ExpressionInfo)this.m_digitColor.PublishClone(context);
			}
			return numericIndicatorRange;
		}

		internal void SetExprHost(NumericIndicatorRangeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_startValue != null && this.ExprHost.StartValueHost != null)
			{
				this.m_startValue.SetExprHost(this.ExprHost.StartValueHost, reportObjectModel);
			}
			if (this.m_endValue != null && this.ExprHost.EndValueHost != null)
			{
				this.m_endValue.SetExprHost(this.ExprHost.EndValueHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StartValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.DecimalDigitColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DigitColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicatorRange, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(NumericIndicatorRange.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(this.m_gaugePanel);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.StartValue:
					writer.Write(this.m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(this.m_endValue);
					break;
				case MemberName.DecimalDigitColor:
					writer.Write(this.m_decimalDigitColor);
					break;
				case MemberName.DigitColor:
					writer.Write(this.m_digitColor);
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
			reader.RegisterDeclaration(NumericIndicatorRange.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					this.m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.StartValue:
					this.m_startValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					this.m_endValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.DecimalDigitColor:
					this.m_decimalDigitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DigitColor:
					this.m_digitColor = (ExpressionInfo)reader.ReadRIFObject();
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

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(NumericIndicatorRange.m_Declaration.ObjectType, out list))
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

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicatorRange;
		}

		internal string EvaluateDecimalDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorRangeDecimalDigitColorExpression(this, this.m_gaugePanel.Name);
		}

		internal string EvaluateDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorRangeDigitColorExpression(this, this.m_gaugePanel.Name);
		}
	}
}
