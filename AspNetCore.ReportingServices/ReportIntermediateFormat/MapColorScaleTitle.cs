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
	internal sealed class MapColorScaleTitle : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapColorScaleTitleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapColorScaleTitle.GetDeclaration();

		private ExpressionInfo m_caption;

		internal ExpressionInfo Caption
		{
			get
			{
				return this.m_caption;
			}
			set
			{
				this.m_caption = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_map.Name;
			}
		}

		internal MapColorScaleTitleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapColorScaleTitle()
		{
		}

		internal MapColorScaleTitle(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorScaleTitleStart();
			base.Initialize(context);
			if (this.m_caption != null)
			{
				this.m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.MapColorScaleTitleCaption(this.m_caption);
			}
			context.ExprHostBuilder.MapColorScaleTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorScaleTitle mapColorScaleTitle = (MapColorScaleTitle)base.PublishClone(context);
			if (this.m_caption != null)
			{
				mapColorScaleTitle.m_caption = (ExpressionInfo)this.m_caption.PublishClone(context);
			}
			return mapColorScaleTitle;
		}

		internal void SetExprHost(MapColorScaleTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapColorScaleTitle.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Caption)
				{
					writer.Write(this.m_caption);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(MapColorScaleTitle.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Caption)
				{
					this.m_caption = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle;
		}

		internal string EvaluateCaption(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapColorScaleTitleCaptionExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}
	}
}
