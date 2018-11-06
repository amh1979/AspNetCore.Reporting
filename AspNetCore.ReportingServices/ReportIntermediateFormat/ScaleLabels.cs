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
	internal sealed class ScaleLabels : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private ScaleLabelsExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ScaleLabels.GetDeclaration();

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabels;

		private ExpressionInfo m_showEndLabels;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_useFontPercent;

		internal ExpressionInfo Interval
		{
			get
			{
				return this.m_interval;
			}
			set
			{
				this.m_interval = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return this.m_intervalOffset;
			}
			set
			{
				this.m_intervalOffset = value;
			}
		}

		internal ExpressionInfo AllowUpsideDown
		{
			get
			{
				return this.m_allowUpsideDown;
			}
			set
			{
				this.m_allowUpsideDown = value;
			}
		}

		internal ExpressionInfo DistanceFromScale
		{
			get
			{
				return this.m_distanceFromScale;
			}
			set
			{
				this.m_distanceFromScale = value;
			}
		}

		internal ExpressionInfo FontAngle
		{
			get
			{
				return this.m_fontAngle;
			}
			set
			{
				this.m_fontAngle = value;
			}
		}

		internal ExpressionInfo Placement
		{
			get
			{
				return this.m_placement;
			}
			set
			{
				this.m_placement = value;
			}
		}

		internal ExpressionInfo RotateLabels
		{
			get
			{
				return this.m_rotateLabels;
			}
			set
			{
				this.m_rotateLabels = value;
			}
		}

		internal ExpressionInfo ShowEndLabels
		{
			get
			{
				return this.m_showEndLabels;
			}
			set
			{
				this.m_showEndLabels = value;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return this.m_useFontPercent;
			}
			set
			{
				this.m_useFontPercent = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal ScaleLabelsExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ScaleLabels()
		{
		}

		internal ScaleLabels(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ScaleLabelsStart();
			base.Initialize(context);
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ScaleLabelsInterval(this.m_interval);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ScaleLabelsIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_allowUpsideDown != null)
			{
				this.m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.ScaleLabelsAllowUpsideDown(this.m_allowUpsideDown);
			}
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.ScaleLabelsDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_fontAngle != null)
			{
				this.m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.ScaleLabelsFontAngle(this.m_fontAngle);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.ScaleLabelsPlacement(this.m_placement);
			}
			if (this.m_rotateLabels != null)
			{
				this.m_rotateLabels.Initialize("RotateLabels", context);
				context.ExprHostBuilder.ScaleLabelsRotateLabels(this.m_rotateLabels);
			}
			if (this.m_showEndLabels != null)
			{
				this.m_showEndLabels.Initialize("ShowEndLabels", context);
				context.ExprHostBuilder.ScaleLabelsShowEndLabels(this.m_showEndLabels);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ScaleLabelsHidden(this.m_hidden);
			}
			if (this.m_useFontPercent != null)
			{
				this.m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.ScaleLabelsUseFontPercent(this.m_useFontPercent);
			}
			context.ExprHostBuilder.ScaleLabelsEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScaleLabels scaleLabels = (ScaleLabels)base.PublishClone(context);
			if (this.m_interval != null)
			{
				scaleLabels.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				scaleLabels.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_allowUpsideDown != null)
			{
				scaleLabels.m_allowUpsideDown = (ExpressionInfo)this.m_allowUpsideDown.PublishClone(context);
			}
			if (this.m_distanceFromScale != null)
			{
				scaleLabels.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_fontAngle != null)
			{
				scaleLabels.m_fontAngle = (ExpressionInfo)this.m_fontAngle.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				scaleLabels.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_rotateLabels != null)
			{
				scaleLabels.m_rotateLabels = (ExpressionInfo)this.m_rotateLabels.PublishClone(context);
			}
			if (this.m_showEndLabels != null)
			{
				scaleLabels.m_showEndLabels = (ExpressionInfo)this.m_showEndLabels.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				scaleLabels.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_useFontPercent != null)
			{
				scaleLabels.m_useFontPercent = (ExpressionInfo)this.m_useFontPercent.PublishClone(context);
			}
			return scaleLabels;
		}

		internal void SetExprHost(ScaleLabelsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowEndLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ScaleLabels.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
					break;
				case MemberName.AllowUpsideDown:
					writer.Write(this.m_allowUpsideDown);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(this.m_distanceFromScale);
					break;
				case MemberName.FontAngle:
					writer.Write(this.m_fontAngle);
					break;
				case MemberName.Placement:
					writer.Write(this.m_placement);
					break;
				case MemberName.RotateLabels:
					writer.Write(this.m_rotateLabels);
					break;
				case MemberName.ShowEndLabels:
					writer.Write(this.m_showEndLabels);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.UseFontPercent:
					writer.Write(this.m_useFontPercent);
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
			reader.RegisterDeclaration(ScaleLabels.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowUpsideDown:
					this.m_allowUpsideDown = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					this.m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FontAngle:
					this.m_fontAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					this.m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RotateLabels:
					this.m_rotateLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowEndLabels:
					this.m_showEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					this.m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsIntervalExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsIntervalOffsetExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsAllowUpsideDownExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsFontAngleExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateScaleLabelsPlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsRotateLabelsExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateShowEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsShowEndLabelsExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsUseFontPercentExpression(this, base.m_gaugePanel.Name);
		}
	}
}
