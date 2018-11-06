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
	internal sealed class MapDistanceScale : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapDistanceScale.GetDeclaration();

		private ExpressionInfo m_scaleColor;

		private ExpressionInfo m_scaleBorderColor;

		internal ExpressionInfo ScaleColor
		{
			get
			{
				return this.m_scaleColor;
			}
			set
			{
				this.m_scaleColor = value;
			}
		}

		internal ExpressionInfo ScaleBorderColor
		{
			get
			{
				return this.m_scaleBorderColor;
			}
			set
			{
				this.m_scaleBorderColor = value;
			}
		}

		internal new MapDistanceScaleExprHost ExprHost
		{
			get
			{
				return (MapDistanceScaleExprHost)base.m_exprHost;
			}
		}

		internal MapDistanceScale()
		{
		}

		internal MapDistanceScale(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapDistanceScaleStart();
			base.Initialize(context);
			if (this.m_scaleColor != null)
			{
				this.m_scaleColor.Initialize("ScaleColor", context);
				context.ExprHostBuilder.MapDistanceScaleScaleColor(this.m_scaleColor);
			}
			if (this.m_scaleBorderColor != null)
			{
				this.m_scaleBorderColor.Initialize("ScaleBorderColor", context);
				context.ExprHostBuilder.MapDistanceScaleScaleBorderColor(this.m_scaleBorderColor);
			}
			context.ExprHostBuilder.MapDistanceScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDistanceScale mapDistanceScale = (MapDistanceScale)base.PublishClone(context);
			if (this.m_scaleColor != null)
			{
				mapDistanceScale.m_scaleColor = (ExpressionInfo)this.m_scaleColor.PublishClone(context);
			}
			if (this.m_scaleBorderColor != null)
			{
				mapDistanceScale.m_scaleBorderColor = (ExpressionInfo)this.m_scaleBorderColor.PublishClone(context);
			}
			return mapDistanceScale;
		}

		internal void SetExprHost(MapDistanceScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ScaleColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ScaleBorderColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapDistanceScale.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ScaleColor:
					writer.Write(this.m_scaleColor);
					break;
				case MemberName.ScaleBorderColor:
					writer.Write(this.m_scaleBorderColor);
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
			reader.RegisterDeclaration(MapDistanceScale.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ScaleColor:
					this.m_scaleColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ScaleBorderColor:
					this.m_scaleBorderColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale;
		}

		internal string EvaluateScaleColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDistanceScaleScaleColorExpression(this, base.m_map.Name);
		}

		internal string EvaluateScaleBorderColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDistanceScaleScaleBorderColorExpression(this, base.m_map.Name);
		}
	}
}
