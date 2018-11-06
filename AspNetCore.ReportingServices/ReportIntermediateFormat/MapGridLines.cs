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
	internal sealed class MapGridLines : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapGridLinesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapGridLines.GetDeclaration();

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_showLabels;

		private ExpressionInfo m_labelPosition;

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

		internal ExpressionInfo ShowLabels
		{
			get
			{
				return this.m_showLabels;
			}
			set
			{
				this.m_showLabels = value;
			}
		}

		internal ExpressionInfo LabelPosition
		{
			get
			{
				return this.m_labelPosition;
			}
			set
			{
				this.m_labelPosition = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_map.Name;
			}
		}

		internal MapGridLinesExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapGridLines()
		{
		}

		internal MapGridLines(Map map)
			: base(map)
		{
		}

		internal void Initialize(InitializationContext context, bool isMeridian)
		{
			context.ExprHostBuilder.MapGridLinesStart(isMeridian);
			base.Initialize(context);
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapGridLinesHidden(this.m_hidden);
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.MapGridLinesInterval(this.m_interval);
			}
			if (this.m_showLabels != null)
			{
				this.m_showLabels.Initialize("ShowLabels", context);
				context.ExprHostBuilder.MapGridLinesShowLabels(this.m_showLabels);
			}
			if (this.m_labelPosition != null)
			{
				this.m_labelPosition.Initialize("LabelPosition", context);
				context.ExprHostBuilder.MapGridLinesLabelPosition(this.m_labelPosition);
			}
			context.ExprHostBuilder.MapGridLinesEnd(isMeridian);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapGridLines mapGridLines = (MapGridLines)base.PublishClone(context);
			if (this.m_hidden != null)
			{
				mapGridLines.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_interval != null)
			{
				mapGridLines.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_showLabels != null)
			{
				mapGridLines.m_showLabels = (ExpressionInfo)this.m_showLabels.PublishClone(context);
			}
			if (this.m_labelPosition != null)
			{
				mapGridLines.m_labelPosition = (ExpressionInfo)this.m_labelPosition.PublishClone(context);
			}
			return mapGridLines;
		}

		internal void SetExprHost(MapGridLinesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapGridLines.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.ShowLabels:
					writer.Write(this.m_showLabels);
					break;
				case MemberName.LabelPosition:
					writer.Write(this.m_labelPosition);
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
			reader.RegisterDeclaration(MapGridLines.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowLabels:
					this.m_showLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPosition:
					this.m_labelPosition = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesHiddenExpression(this, base.m_map.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesIntervalExpression(this, base.m_map.Name);
		}

		internal bool EvaluateShowLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapGridLinesShowLabelsExpression(this, base.m_map.Name);
		}

		internal MapLabelPosition EvaluateLabelPosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelPosition(context.ReportRuntime.EvaluateMapGridLinesLabelPositionExpression(this, base.m_map.Name), context.ReportRuntime);
		}
	}
}
