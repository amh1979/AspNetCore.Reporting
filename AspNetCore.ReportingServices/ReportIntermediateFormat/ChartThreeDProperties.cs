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
	internal sealed class ChartThreeDProperties : IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_projectionMode;

		private ExpressionInfo m_rotation;

		private ExpressionInfo m_inclination;

		private ExpressionInfo m_perspective;

		private ExpressionInfo m_depthRatio;

		private ExpressionInfo m_shading;

		private ExpressionInfo m_gapDepth;

		private ExpressionInfo m_wallThickness;

		private ExpressionInfo m_clustered;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private Chart3DPropertiesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartThreeDProperties.GetDeclaration();

		internal ExpressionInfo Enabled
		{
			get
			{
				return this.m_enabled;
			}
			set
			{
				this.m_enabled = value;
			}
		}

		internal ExpressionInfo ProjectionMode
		{
			get
			{
				return this.m_projectionMode;
			}
			set
			{
				this.m_projectionMode = value;
			}
		}

		internal ExpressionInfo Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
			}
		}

		internal ExpressionInfo Inclination
		{
			get
			{
				return this.m_inclination;
			}
			set
			{
				this.m_inclination = value;
			}
		}

		internal ExpressionInfo Perspective
		{
			get
			{
				return this.m_perspective;
			}
			set
			{
				this.m_perspective = value;
			}
		}

		internal ExpressionInfo DepthRatio
		{
			get
			{
				return this.m_depthRatio;
			}
			set
			{
				this.m_depthRatio = value;
			}
		}

		internal ExpressionInfo Shading
		{
			get
			{
				return this.m_shading;
			}
			set
			{
				this.m_shading = value;
			}
		}

		internal ExpressionInfo GapDepth
		{
			get
			{
				return this.m_gapDepth;
			}
			set
			{
				this.m_gapDepth = value;
			}
		}

		internal ExpressionInfo WallThickness
		{
			get
			{
				return this.m_wallThickness;
			}
			set
			{
				this.m_wallThickness = value;
			}
		}

		internal ExpressionInfo Clustered
		{
			get
			{
				return this.m_clustered;
			}
			set
			{
				this.m_clustered = value;
			}
		}

		internal Chart3DPropertiesExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ChartThreeDProperties()
		{
		}

		internal ChartThreeDProperties(Chart chart)
		{
			this.m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.Chart3DPropertiesStart();
			if (this.m_enabled != null)
			{
				this.m_enabled.Initialize("Rotation", context);
				context.ExprHostBuilder.Chart3DPropertiesEnabled(this.m_enabled);
			}
			if (this.m_projectionMode != null)
			{
				this.m_projectionMode.Initialize("ProjectionMode", context);
				context.ExprHostBuilder.Chart3DPropertiesProjectionMode(this.m_projectionMode);
			}
			if (this.m_rotation != null)
			{
				this.m_rotation.Initialize("Rotation", context);
				context.ExprHostBuilder.Chart3DPropertiesRotation(this.m_rotation);
			}
			if (this.m_inclination != null)
			{
				this.m_inclination.Initialize("Inclination", context);
				context.ExprHostBuilder.Chart3DPropertiesInclination(this.m_inclination);
			}
			if (this.m_perspective != null)
			{
				this.m_perspective.Initialize("Perspective", context);
				context.ExprHostBuilder.Chart3DPropertiesPerspective(this.m_perspective);
			}
			if (this.m_depthRatio != null)
			{
				this.m_depthRatio.Initialize("DepthRatio", context);
				context.ExprHostBuilder.Chart3DPropertiesDepthRatio(this.m_depthRatio);
			}
			if (this.m_shading != null)
			{
				this.m_shading.Initialize("Shading", context);
				context.ExprHostBuilder.Chart3DPropertiesShading(this.m_shading);
			}
			if (this.m_gapDepth != null)
			{
				this.m_gapDepth.Initialize("GapDepth", context);
				context.ExprHostBuilder.Chart3DPropertiesGapDepth(this.m_gapDepth);
			}
			if (this.m_wallThickness != null)
			{
				this.m_wallThickness.Initialize("WallThickness", context);
				context.ExprHostBuilder.Chart3DPropertiesWallThickness(this.m_wallThickness);
			}
			if (this.m_clustered != null)
			{
				this.m_clustered.Initialize("Clustered", context);
				context.ExprHostBuilder.Chart3DPropertiesClustered(this.m_clustered);
			}
			context.ExprHostBuilder.Chart3DPropertiesEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartThreeDProperties chartThreeDProperties = (ChartThreeDProperties)base.MemberwiseClone();
			chartThreeDProperties.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_enabled != null)
			{
				chartThreeDProperties.m_enabled = (ExpressionInfo)this.m_enabled.PublishClone(context);
			}
			if (this.m_projectionMode != null)
			{
				chartThreeDProperties.m_projectionMode = (ExpressionInfo)this.m_projectionMode.PublishClone(context);
			}
			if (this.m_rotation != null)
			{
				chartThreeDProperties.m_rotation = (ExpressionInfo)this.m_rotation.PublishClone(context);
			}
			if (this.m_inclination != null)
			{
				chartThreeDProperties.m_inclination = (ExpressionInfo)this.m_inclination.PublishClone(context);
			}
			if (this.m_perspective != null)
			{
				chartThreeDProperties.m_perspective = (ExpressionInfo)this.m_perspective.PublishClone(context);
			}
			if (this.m_depthRatio != null)
			{
				chartThreeDProperties.m_depthRatio = (ExpressionInfo)this.m_depthRatio.PublishClone(context);
			}
			if (this.m_shading != null)
			{
				chartThreeDProperties.m_shading = (ExpressionInfo)this.m_shading.PublishClone(context);
			}
			if (this.m_gapDepth != null)
			{
				chartThreeDProperties.m_gapDepth = (ExpressionInfo)this.m_gapDepth.PublishClone(context);
			}
			if (this.m_wallThickness != null)
			{
				chartThreeDProperties.m_wallThickness = (ExpressionInfo)this.m_wallThickness.PublishClone(context);
			}
			if (this.m_clustered != null)
			{
				chartThreeDProperties.m_clustered = (ExpressionInfo)this.m_clustered.PublishClone(context);
			}
			return chartThreeDProperties;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ProjectionMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Rotation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Inclination, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Perspective, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DepthRatio, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Shading, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GapDepth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.WallThickness, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Clustered, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartThreeDProperties.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(this.m_enabled);
					break;
				case MemberName.ProjectionMode:
					writer.Write(this.m_projectionMode);
					break;
				case MemberName.Rotation:
					writer.Write(this.m_rotation);
					break;
				case MemberName.Inclination:
					writer.Write(this.m_inclination);
					break;
				case MemberName.Perspective:
					writer.Write(this.m_perspective);
					break;
				case MemberName.DepthRatio:
					writer.Write(this.m_depthRatio);
					break;
				case MemberName.Shading:
					writer.Write(this.m_shading);
					break;
				case MemberName.GapDepth:
					writer.Write(this.m_gapDepth);
					break;
				case MemberName.WallThickness:
					writer.Write(this.m_wallThickness);
					break;
				case MemberName.Clustered:
					writer.Write(this.m_clustered);
					break;
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartThreeDProperties.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					this.m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ProjectionMode:
					this.m_projectionMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Rotation:
					this.m_rotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Inclination:
					this.m_inclination = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Perspective:
					this.m_perspective = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DepthRatio:
					this.m_depthRatio = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Shading:
					this.m_shading = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GapDepth:
					this.m_gapDepth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.WallThickness:
					this.m_wallThickness = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Clustered:
					this.m_clustered = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
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
			if (memberReferencesCollection.TryGetValue(ChartThreeDProperties.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Chart)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties;
		}

		internal void SetExprHost(Chart3DPropertiesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal bool EvaluateEnabled(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesEnabledExpression(this, this.m_chart.Name, "Enabled");
		}

		internal ChartThreeDProjectionModes EvaluateProjectionMode(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return EnumTranslator.TranslateChartThreeDProjectionMode(context.ReportRuntime.EvaluateChartThreeDPropertiesProjectionModeExpression(this, this.m_chart.Name, "ProjectionMode"), context.ReportRuntime);
		}

		internal int EvaluateRotation(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesRotationExpression(this, this.m_chart.Name, "Rotation");
		}

		internal int EvaluateInclination(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesInclinationExpression(this, this.m_chart.Name, "Inclination");
		}

		internal int EvaluatePerspective(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesPerspectiveExpression(this, this.m_chart.Name, "Perspective");
		}

		internal int EvaluateDepthRatio(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesDepthRatioExpression(this, this.m_chart.Name, "DepthRatio");
		}

		internal ChartThreeDShadingTypes EvaluateShading(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return EnumTranslator.TranslateChartThreeDShading(context.ReportRuntime.EvaluateChartThreeDPropertiesShadingExpression(this, this.m_chart.Name, "Shading"), context.ReportRuntime);
		}

		internal int EvaluateGapDepth(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesGapDepthExpression(this, this.m_chart.Name, "GapDepth");
		}

		internal int EvaluateWallThickness(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesWallThicknessExpression(this, this.m_chart.Name, "WallThickness");
		}

		internal bool EvaluateClustered(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesClusteredExpression(this, this.m_chart.Name, "Clustered");
		}
	}
}
