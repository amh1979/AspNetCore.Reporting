using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class MapLine : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLine.GetDeclaration();

		private ExpressionInfo m_useCustomLineTemplate;

		private MapLineTemplate m_mapLineTemplate;

		internal ExpressionInfo UseCustomLineTemplate
		{
			get
			{
				return this.m_useCustomLineTemplate;
			}
			set
			{
				this.m_useCustomLineTemplate = value;
			}
		}

		internal MapLineTemplate MapLineTemplate
		{
			get
			{
				return this.m_mapLineTemplate;
			}
			set
			{
				this.m_mapLineTemplate = value;
			}
		}

		internal new MapLineExprHost ExprHost
		{
			get
			{
				return (MapLineExprHost)base.m_exprHost;
			}
		}

		internal MapLine()
		{
		}

		internal MapLine(MapLineLayer mapLineLayer, Map map)
			: base(mapLineLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapLineStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (this.m_useCustomLineTemplate != null)
			{
				this.m_useCustomLineTemplate.Initialize("UseCustomLineTemplate", context);
				context.ExprHostBuilder.MapLineUseCustomLineTemplate(this.m_useCustomLineTemplate);
			}
			if (this.m_mapLineTemplate != null)
			{
				this.m_mapLineTemplate.Initialize(context);
			}
			base.m_exprHostID = context.ExprHostBuilder.MapLineEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLine mapLine = (MapLine)base.PublishClone(context);
			if (this.m_useCustomLineTemplate != null)
			{
				mapLine.m_useCustomLineTemplate = (ExpressionInfo)this.m_useCustomLineTemplate.PublishClone(context);
			}
			if (this.m_mapLineTemplate != null)
			{
				mapLine.m_mapLineTemplate = (MapLineTemplate)this.m_mapLineTemplate.PublishClone(context);
			}
			return mapLine;
		}

		internal void SetExprHost(MapLineExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapLineTemplate != null && this.ExprHost.MapLineTemplateHost != null)
			{
				this.m_mapLineTemplate.SetExprHost(this.ExprHost.MapLineTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomLineTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapLineTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapLine.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomLineTemplate:
					writer.Write(this.m_useCustomLineTemplate);
					break;
				case MemberName.MapLineTemplate:
					writer.Write(this.m_mapLineTemplate);
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
			reader.RegisterDeclaration(MapLine.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.UseCustomLineTemplate:
					this.m_useCustomLineTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapLineTemplate:
					this.m_mapLineTemplate = (MapLineTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine;
		}

		internal bool EvaluateUseCustomLineTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLineUseCustomLineTemplateExpression(this, base.m_map.Name);
		}
	}
}
