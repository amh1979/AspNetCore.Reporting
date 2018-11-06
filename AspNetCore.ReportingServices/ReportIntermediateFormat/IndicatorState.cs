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
	internal sealed class IndicatorState : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private IndicatorStateExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = IndicatorState.GetDeclaration();

		private string m_name;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_color;

		private ExpressionInfo m_scaleFactor;

		private ExpressionInfo m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

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

		internal ExpressionInfo Color
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
			}
		}

		internal ExpressionInfo ScaleFactor
		{
			get
			{
				return this.m_scaleFactor;
			}
			set
			{
				this.m_scaleFactor = value;
			}
		}

		internal ExpressionInfo IndicatorStyle
		{
			get
			{
				return this.m_indicatorStyle;
			}
			set
			{
				this.m_indicatorStyle = value;
			}
		}

		internal IndicatorImage IndicatorImage
		{
			get
			{
				return this.m_indicatorImage;
			}
			set
			{
				this.m_indicatorImage = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		internal IndicatorStateExprHost ExprHost
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

		internal IndicatorState()
		{
		}

		internal IndicatorState(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.IndicatorStateStart(this.m_name);
			if (this.m_color != null)
			{
				this.m_color.Initialize("Color", context);
				context.ExprHostBuilder.IndicatorStateColor(this.m_color);
			}
			if (this.m_scaleFactor != null)
			{
				this.m_scaleFactor.Initialize("ScaleFactor", context);
				context.ExprHostBuilder.IndicatorStateScaleFactor(this.m_scaleFactor);
			}
			if (this.m_indicatorStyle != null)
			{
				this.m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.IndicatorStateIndicatorStyle(this.m_indicatorStyle);
			}
			if (this.m_indicatorImage != null)
			{
				this.m_indicatorImage.Initialize(context);
			}
			this.m_exprHostID = context.ExprHostBuilder.IndicatorStateEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			IndicatorState indicatorState = (IndicatorState)base.MemberwiseClone();
			indicatorState.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (this.m_startValue != null)
			{
				indicatorState.m_startValue = (GaugeInputValue)this.m_startValue.PublishClone(context);
			}
			if (this.m_endValue != null)
			{
				indicatorState.m_endValue = (GaugeInputValue)this.m_endValue.PublishClone(context);
			}
			if (this.m_color != null)
			{
				indicatorState.m_color = (ExpressionInfo)this.m_color.PublishClone(context);
			}
			if (this.m_scaleFactor != null)
			{
				indicatorState.m_scaleFactor = (ExpressionInfo)this.m_scaleFactor.PublishClone(context);
			}
			if (this.m_indicatorStyle != null)
			{
				indicatorState.m_indicatorStyle = (ExpressionInfo)this.m_indicatorStyle.PublishClone(context);
			}
			if (this.m_indicatorImage != null)
			{
				indicatorState.m_indicatorImage = (IndicatorImage)this.m_indicatorImage.PublishClone(context);
			}
			return indicatorState;
		}

		internal void SetExprHost(IndicatorStateExprHost exprHost, ObjectModelImpl reportObjectModel)
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
			if (this.m_indicatorImage != null && this.ExprHost.IndicatorImageHost != null)
			{
				this.m_indicatorImage.SetExprHost(this.ExprHost.IndicatorImageHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StartValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.Color, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ScaleFactor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage));
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(IndicatorState.m_Declaration);
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
				case MemberName.Color:
					writer.Write(this.m_color);
					break;
				case MemberName.ScaleFactor:
					writer.Write(this.m_scaleFactor);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(this.m_indicatorStyle);
					break;
				case MemberName.IndicatorImage:
					writer.Write(this.m_indicatorImage);
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
			reader.RegisterDeclaration(IndicatorState.m_Declaration);
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
				case MemberName.Color:
					this.m_color = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ScaleFactor:
					this.m_scaleFactor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					this.m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorImage:
					this.m_indicatorImage = (IndicatorImage)reader.ReadRIFObject();
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
			if (memberReferencesCollection.TryGetValue(IndicatorState.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState;
		}

		internal string EvaluateColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorStateColorExpression(this, this.m_gaugePanel.Name);
		}

		internal double EvaluateScaleFactor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorStateScaleFactorExpression(this, this.m_gaugePanel.Name);
		}

		internal GaugeStateIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeStateIndicatorStyles(context.ReportRuntime.EvaluateIndicatorStateIndicatorStyleExpression(this, this.m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
