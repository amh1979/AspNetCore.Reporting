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
	internal sealed class StateIndicator : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = StateIndicator.GetDeclaration();

		private GaugeInputValue m_gaugeInputValue;

		private ExpressionInfo m_transformationType;

		private string m_transformationScope;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ExpressionInfo m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		private ExpressionInfo m_scaleFactor;

		private List<IndicatorState> m_indicatorStates;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_angle;

		private string m_stateDataElementName;

		private DataElementOutputTypes m_stateDataElementOutput;

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return this.m_gaugeInputValue;
			}
			set
			{
				this.m_gaugeInputValue = value;
			}
		}

		internal ExpressionInfo TransformationType
		{
			get
			{
				return this.m_transformationType;
			}
			set
			{
				this.m_transformationType = value;
			}
		}

		internal string TransformationScope
		{
			get
			{
				return this.m_transformationScope;
			}
			set
			{
				this.m_transformationScope = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return this.m_maximumValue;
			}
			set
			{
				this.m_maximumValue = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return this.m_minimumValue;
			}
			set
			{
				this.m_minimumValue = value;
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

		internal List<IndicatorState> IndicatorStates
		{
			get
			{
				return this.m_indicatorStates;
			}
			set
			{
				this.m_indicatorStates = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return this.m_resizeMode;
			}
			set
			{
				this.m_resizeMode = value;
			}
		}

		internal ExpressionInfo Angle
		{
			get
			{
				return this.m_angle;
			}
			set
			{
				this.m_angle = value;
			}
		}

		internal string StateDataElementName
		{
			get
			{
				return this.m_stateDataElementName;
			}
			set
			{
				this.m_stateDataElementName = value;
			}
		}

		internal DataElementOutputTypes StateDataElementOutput
		{
			get
			{
				return this.m_stateDataElementOutput;
			}
			set
			{
				this.m_stateDataElementOutput = value;
			}
		}

		internal new StateIndicatorExprHost ExprHost
		{
			get
			{
				return (StateIndicatorExprHost)base.m_exprHost;
			}
		}

		internal StateIndicator()
		{
		}

		internal StateIndicator(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.StateIndicatorStart(base.m_name);
			base.Initialize(context);
			if (this.m_transformationType != null)
			{
				this.m_transformationType.Initialize("TransformationType", context);
				context.ExprHostBuilder.StateIndicatorTransformationType(this.m_transformationType);
			}
			if (this.m_indicatorStyle != null)
			{
				this.m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.StateIndicatorIndicatorStyle(this.m_indicatorStyle);
			}
			if (this.m_indicatorImage != null)
			{
				this.m_indicatorImage.Initialize(context);
			}
			if (this.m_scaleFactor != null)
			{
				this.m_scaleFactor.Initialize("ScaleFactor", context);
				context.ExprHostBuilder.StateIndicatorScaleFactor(this.m_scaleFactor);
			}
			if (this.m_indicatorStates != null)
			{
				for (int i = 0; i < this.m_indicatorStates.Count; i++)
				{
					this.m_indicatorStates[i].Initialize(context);
				}
			}
			if (this.m_resizeMode != null)
			{
				this.m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.StateIndicatorResizeMode(this.m_resizeMode);
			}
			if (this.m_angle != null)
			{
				this.m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.StateIndicatorAngle(this.m_angle);
			}
			base.m_exprHostID = context.ExprHostBuilder.StateIndicatorEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			StateIndicator stateIndicator = (StateIndicator)base.PublishClone(context);
			if (this.m_gaugeInputValue != null)
			{
				stateIndicator.m_gaugeInputValue = (GaugeInputValue)this.m_gaugeInputValue.PublishClone(context);
			}
			if (this.m_transformationType != null)
			{
				stateIndicator.m_transformationType = (ExpressionInfo)this.m_transformationType.PublishClone(context);
			}
			if (this.m_maximumValue != null)
			{
				stateIndicator.m_maximumValue = (GaugeInputValue)this.m_maximumValue.PublishClone(context);
			}
			if (this.m_minimumValue != null)
			{
				stateIndicator.m_minimumValue = (GaugeInputValue)this.m_minimumValue.PublishClone(context);
			}
			if (this.m_indicatorStyle != null)
			{
				stateIndicator.m_indicatorStyle = (ExpressionInfo)this.m_indicatorStyle.PublishClone(context);
			}
			if (this.m_indicatorImage != null)
			{
				stateIndicator.m_indicatorImage = (IndicatorImage)this.m_indicatorImage.PublishClone(context);
			}
			if (this.m_scaleFactor != null)
			{
				stateIndicator.m_scaleFactor = (ExpressionInfo)this.m_scaleFactor.PublishClone(context);
			}
			if (this.m_indicatorStates != null)
			{
				stateIndicator.m_indicatorStates = new List<IndicatorState>(this.m_indicatorStates.Count);
				foreach (IndicatorState indicatorState in this.m_indicatorStates)
				{
					stateIndicator.m_indicatorStates.Add((IndicatorState)indicatorState.PublishClone(context));
				}
			}
			if (this.m_resizeMode != null)
			{
				stateIndicator.m_resizeMode = (ExpressionInfo)this.m_resizeMode.PublishClone(context);
			}
			if (this.m_angle != null)
			{
				stateIndicator.m_angle = (ExpressionInfo)this.m_angle.PublishClone(context);
			}
			return stateIndicator;
		}

		internal void SetExprHost(StateIndicatorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_gaugeInputValue != null && this.ExprHost.GaugeInputValueHost != null)
			{
				this.m_gaugeInputValue.SetExprHost(this.ExprHost.GaugeInputValueHost, reportObjectModel);
			}
			if (this.m_maximumValue != null && this.ExprHost.MaximumValueHost != null)
			{
				this.m_maximumValue.SetExprHost(this.ExprHost.MaximumValueHost, reportObjectModel);
			}
			if (this.m_minimumValue != null && this.ExprHost.MinimumValueHost != null)
			{
				this.m_minimumValue.SetExprHost(this.ExprHost.MinimumValueHost, reportObjectModel);
			}
			if (this.m_indicatorImage != null && this.ExprHost.IndicatorImageHost != null)
			{
				this.m_indicatorImage.SetExprHost(this.ExprHost.IndicatorImageHost, reportObjectModel);
			}
			IList<IndicatorStateExprHost> indicatorStatesHostsRemotable = this.ExprHost.IndicatorStatesHostsRemotable;
			if (this.m_indicatorStates != null && indicatorStatesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_indicatorStates.Count; i++)
				{
					IndicatorState indicatorState = this.m_indicatorStates[i];
					if (indicatorState != null && indicatorState.ExpressionHostID > -1)
					{
						indicatorState.SetExprHost(indicatorStatesHostsRemotable[indicatorState.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeInputValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.IndicatorStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage));
			list.Add(new MemberInfo(MemberName.ScaleFactor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorStates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState));
			list.Add(new MemberInfo(MemberName.ResizeMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransformationType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransformationScope, Token.String));
			list.Add(new MemberInfo(MemberName.MaximumValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.MinimumValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.StateDataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.StateDataElementOutput, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(StateIndicator.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					writer.Write(this.m_gaugeInputValue);
					break;
				case MemberName.TransformationType:
					writer.Write(this.m_transformationType);
					break;
				case MemberName.TransformationScope:
					writer.Write(this.m_transformationScope);
					break;
				case MemberName.MaximumValue:
					writer.Write(this.m_maximumValue);
					break;
				case MemberName.MinimumValue:
					writer.Write(this.m_minimumValue);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(this.m_indicatorStyle);
					break;
				case MemberName.IndicatorImage:
					writer.Write(this.m_indicatorImage);
					break;
				case MemberName.ScaleFactor:
					writer.Write(this.m_scaleFactor);
					break;
				case MemberName.IndicatorStates:
					writer.Write(this.m_indicatorStates);
					break;
				case MemberName.ResizeMode:
					writer.Write(this.m_resizeMode);
					break;
				case MemberName.Angle:
					writer.Write(this.m_angle);
					break;
				case MemberName.StateDataElementName:
					writer.Write(this.m_stateDataElementName);
					break;
				case MemberName.StateDataElementOutput:
					writer.WriteEnum((int)this.m_stateDataElementOutput);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(StateIndicator.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					this.m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.TransformationType:
					this.m_transformationType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransformationScope:
					this.m_transformationScope = reader.ReadString();
					break;
				case MemberName.MaximumValue:
					this.m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					this.m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					this.m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorImage:
					this.m_indicatorImage = (IndicatorImage)reader.ReadRIFObject();
					break;
				case MemberName.ScaleFactor:
					this.m_scaleFactor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStates:
					this.m_indicatorStates = reader.ReadGenericListOfRIFObjects<IndicatorState>();
					break;
				case MemberName.ResizeMode:
					this.m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					this.m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StateDataElementName:
					this.m_stateDataElementName = reader.ReadString();
					break;
				case MemberName.StateDataElementOutput:
					this.m_stateDataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
				if (reader.IntermediateFormatVersion.CompareTo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.SQL16) < 0)
				{
					AttributeInfo attributeInfo = default(AttributeInfo);
					if (!base.m_styleClass.GetAttributeInfo("BorderStyle", out attributeInfo))
					{
						base.m_styleClass.AddAttribute("BorderStyle", new ExpressionInfo
						{
							StringValue = "Solid",
							ConstantType = DataType.String
						});
					}
					else
					{
						attributeInfo.IsExpression = false;
						attributeInfo.Value = "Solid";
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator;
		}

		internal GaugeTransformationType EvaluateTransformationType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeTransformationType(context.ReportRuntime.EvaluateStateIndicatorTransformationTypeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeStateIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeStateIndicatorStyles(context.ReportRuntime.EvaluateStateIndicatorIndicatorStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateScaleFactor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateStateIndicatorScaleFactorExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateStateIndicatorResizeModeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateStateIndicatorAngleExpression(this, base.m_gaugePanel.Name);
		}
	}
}
