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
	internal sealed class ScalePin : TickMarkStyle, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = ScalePin.GetDeclaration();

		private ExpressionInfo m_location;

		private ExpressionInfo m_enable;

		private PinLabel m_pinLabel;

		internal ExpressionInfo Location
		{
			get
			{
				return this.m_location;
			}
			set
			{
				this.m_location = value;
			}
		}

		internal ExpressionInfo Enable
		{
			get
			{
				return this.m_enable;
			}
			set
			{
				this.m_enable = value;
			}
		}

		internal PinLabel PinLabel
		{
			get
			{
				return this.m_pinLabel;
			}
			set
			{
				this.m_pinLabel = value;
			}
		}

		internal ScalePin()
		{
		}

		internal ScalePin(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal void Initialize(InitializationContext context, bool isMaximum)
		{
			context.ExprHostBuilder.ScalePinStart(isMaximum);
			base.InitializeInternal(context);
			if (this.m_location != null)
			{
				this.m_location.Initialize("Location", context);
				context.ExprHostBuilder.ScalePinLocation(this.m_location);
			}
			if (this.m_enable != null)
			{
				this.m_enable.Initialize("Enable", context);
				context.ExprHostBuilder.ScalePinEnable(this.m_enable);
			}
			if (this.m_pinLabel != null)
			{
				this.m_pinLabel.Initialize(context);
			}
			context.ExprHostBuilder.ScalePinEnd(isMaximum);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScalePin scalePin = (ScalePin)base.PublishClone(context);
			if (this.m_location != null)
			{
				scalePin.m_location = (ExpressionInfo)this.m_location.PublishClone(context);
			}
			if (this.m_enable != null)
			{
				scalePin.m_enable = (ExpressionInfo)this.m_enable.PublishClone(context);
			}
			if (this.m_pinLabel != null)
			{
				scalePin.m_pinLabel = (PinLabel)this.m_pinLabel.PublishClone(context);
			}
			return scalePin;
		}

		internal void SetExprHost(ScalePinExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			if (this.m_pinLabel != null && ((ScalePinExprHost)base.m_exprHost).PinLabelHost != null)
			{
				this.m_pinLabel.SetExprHost(((ScalePinExprHost)base.m_exprHost).PinLabelHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Location, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Enable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PinLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ScalePin.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Location:
					writer.Write(this.m_location);
					break;
				case MemberName.Enable:
					writer.Write(this.m_enable);
					break;
				case MemberName.PinLabel:
					writer.Write(this.m_pinLabel);
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
			reader.RegisterDeclaration(ScalePin.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Location:
					this.m_location = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Enable:
					this.m_enable = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PinLabel:
					this.m_pinLabel = (PinLabel)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin;
		}

		internal double EvaluateLocation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScalePinLocationExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateEnable(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScalePinEnableExpression(this, base.m_gaugePanel.Name);
		}
	}
}
