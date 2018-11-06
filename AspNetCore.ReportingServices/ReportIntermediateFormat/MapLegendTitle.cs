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
	internal sealed class MapLegendTitle : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapLegendTitleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLegendTitle.GetDeclaration();

		private ExpressionInfo m_caption;

		private ExpressionInfo m_titleSeparator;

		private ExpressionInfo m_titleSeparatorColor;

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

		internal ExpressionInfo TitleSeparator
		{
			get
			{
				return this.m_titleSeparator;
			}
			set
			{
				this.m_titleSeparator = value;
			}
		}

		internal ExpressionInfo TitleSeparatorColor
		{
			get
			{
				return this.m_titleSeparatorColor;
			}
			set
			{
				this.m_titleSeparatorColor = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_map.Name;
			}
		}

		internal MapLegendTitleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapLegendTitle()
		{
		}

		internal MapLegendTitle(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLegendTitleStart();
			base.Initialize(context);
			if (this.m_caption != null)
			{
				this.m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.MapLegendTitleCaption(this.m_caption);
			}
			if (this.m_titleSeparator != null)
			{
				this.m_titleSeparator.Initialize("TitleSeparator", context);
				context.ExprHostBuilder.MapLegendTitleTitleSeparator(this.m_titleSeparator);
			}
			if (this.m_titleSeparatorColor != null)
			{
				this.m_titleSeparatorColor.Initialize("TitleSeparatorColor", context);
				context.ExprHostBuilder.MapLegendTitleTitleSeparatorColor(this.m_titleSeparatorColor);
			}
			context.ExprHostBuilder.MapLegendTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLegendTitle mapLegendTitle = (MapLegendTitle)base.PublishClone(context);
			if (this.m_caption != null)
			{
				mapLegendTitle.m_caption = (ExpressionInfo)this.m_caption.PublishClone(context);
			}
			if (this.m_titleSeparator != null)
			{
				mapLegendTitle.m_titleSeparator = (ExpressionInfo)this.m_titleSeparator.PublishClone(context);
			}
			if (this.m_titleSeparatorColor != null)
			{
				mapLegendTitle.m_titleSeparatorColor = (ExpressionInfo)this.m_titleSeparatorColor.PublishClone(context);
			}
			return mapLegendTitle;
		}

		internal void SetExprHost(MapLegendTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleSeparator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleSeparatorColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapLegendTitle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Caption:
					writer.Write(this.m_caption);
					break;
				case MemberName.TitleSeparator:
					writer.Write(this.m_titleSeparator);
					break;
				case MemberName.TitleSeparatorColor:
					writer.Write(this.m_titleSeparatorColor);
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
			reader.RegisterDeclaration(MapLegendTitle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Caption:
					this.m_caption = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleSeparator:
					this.m_titleSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleSeparatorColor:
					this.m_titleSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle;
		}

		internal string EvaluateCaption(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapLegendTitleCaptionExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}

		internal MapLegendTitleSeparator EvaluateTitleSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapLegendTitleSeparator(context.ReportRuntime.EvaluateMapLegendTitleTitleSeparatorExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal string EvaluateTitleSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendTitleTitleSeparatorColorExpression(this, base.m_map.Name);
		}
	}
}
