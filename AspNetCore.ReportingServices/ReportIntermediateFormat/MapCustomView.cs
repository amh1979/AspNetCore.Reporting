using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapCustomView : MapView, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapCustomView.GetDeclaration();

		private ExpressionInfo m_centerX;

		private ExpressionInfo m_centerY;

		internal ExpressionInfo CenterX
		{
			get
			{
				return this.m_centerX;
			}
			set
			{
				this.m_centerX = value;
			}
		}

		internal ExpressionInfo CenterY
		{
			get
			{
				return this.m_centerY;
			}
			set
			{
				this.m_centerY = value;
			}
		}

		internal new MapCustomViewExprHost ExprHost
		{
			get
			{
				return (MapCustomViewExprHost)base.m_exprHost;
			}
		}

		internal MapCustomView()
		{
		}

		internal MapCustomView(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomViewStart();
			base.Initialize(context);
			if (this.m_centerX != null)
			{
				this.m_centerX.Initialize("CenterX", context);
				context.ExprHostBuilder.MapCustomViewCenterX(this.m_centerX);
			}
			if (this.m_centerY != null)
			{
				this.m_centerY.Initialize("CenterY", context);
				context.ExprHostBuilder.MapCustomViewCenterY(this.m_centerY);
			}
			context.ExprHostBuilder.MapCustomViewEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapCustomView mapCustomView = (MapCustomView)base.PublishClone(context);
			if (this.m_centerX != null)
			{
				mapCustomView.m_centerX = (ExpressionInfo)this.m_centerX.PublishClone(context);
			}
			if (this.m_centerY != null)
			{
				mapCustomView.m_centerY = (ExpressionInfo)this.m_centerY.PublishClone(context);
			}
			return mapCustomView;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CenterX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CenterY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomView, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapCustomView.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CenterX:
					writer.Write(this.m_centerX);
					break;
				case MemberName.CenterY:
					writer.Write(this.m_centerY);
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
			reader.RegisterDeclaration(MapCustomView.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CenterX:
					this.m_centerX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CenterY:
					this.m_centerY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomView;
		}

		internal double EvaluateCenterX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapCustomViewCenterXExpression(this, base.m_map.Name);
		}

		internal double EvaluateCenterY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapCustomViewCenterYExpression(this, base.m_map.Name);
		}
	}
}
