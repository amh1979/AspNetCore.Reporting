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
	internal sealed class MapTitle : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapTitle.GetDeclaration();

		private ExpressionInfo m_text;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_textShadowOffset;

		private string m_name;

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

		internal ExpressionInfo Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
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

		internal ExpressionInfo TextShadowOffset
		{
			get
			{
				return this.m_textShadowOffset;
			}
			set
			{
				this.m_textShadowOffset = value;
			}
		}

		internal new MapTitleExprHost ExprHost
		{
			get
			{
				return (MapTitleExprHost)base.m_exprHost;
			}
		}

		internal MapTitle()
		{
		}

		internal MapTitle(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapTitleStart(this.m_name);
			base.Initialize(context);
			if (this.m_text != null)
			{
				this.m_text.Initialize("Text", context);
				context.ExprHostBuilder.MapTitleText(this.m_text);
			}
			if (this.m_angle != null)
			{
				this.m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.MapTitleAngle(this.m_angle);
			}
			if (this.m_textShadowOffset != null)
			{
				this.m_textShadowOffset.Initialize("TextShadowOffset", context);
				context.ExprHostBuilder.MapTitleTextShadowOffset(this.m_textShadowOffset);
			}
			base.m_exprHostID = context.ExprHostBuilder.MapTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapTitle mapTitle = (MapTitle)base.PublishClone(context);
			if (this.m_text != null)
			{
				mapTitle.m_text = (ExpressionInfo)this.m_text.PublishClone(context);
			}
			if (this.m_angle != null)
			{
				mapTitle.m_angle = (ExpressionInfo)this.m_angle.PublishClone(context);
			}
			if (this.m_textShadowOffset != null)
			{
				mapTitle.m_textShadowOffset = (ExpressionInfo)this.m_textShadowOffset.PublishClone(context);
			}
			return mapTitle;
		}

		internal void SetExprHost(MapTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Text, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextShadowOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapTitle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Text:
					writer.Write(this.m_text);
					break;
				case MemberName.Angle:
					writer.Write(this.m_angle);
					break;
				case MemberName.TextShadowOffset:
					writer.Write(this.m_textShadowOffset);
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
			reader.RegisterDeclaration(MapTitle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Text:
					this.m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					this.m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextShadowOffset:
					this.m_textShadowOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapTitleTextExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTitleAngleExpression(this, base.m_map.Name);
		}

		internal string EvaluateTextShadowOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTitleTextShadowOffsetExpression(this, base.m_map.Name);
		}
	}
}
