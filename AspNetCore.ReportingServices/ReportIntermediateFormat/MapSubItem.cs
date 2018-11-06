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
	internal class MapSubItem : MapStyleContainer, IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapSubItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSubItem.GetDeclaration();

		private MapLocation m_mapLocation;

		private MapSize m_mapSize;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_zIndex;

		internal MapLocation MapLocation
		{
			get
			{
				return this.m_mapLocation;
			}
			set
			{
				this.m_mapLocation = value;
			}
		}

		internal MapSize MapSize
		{
			get
			{
				return this.m_mapSize;
			}
			set
			{
				this.m_mapSize = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return this.m_leftMargin;
			}
			set
			{
				this.m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return this.m_rightMargin;
			}
			set
			{
				this.m_rightMargin = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return this.m_topMargin;
			}
			set
			{
				this.m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return this.m_bottomMargin;
			}
			set
			{
				this.m_bottomMargin = value;
			}
		}

		internal ExpressionInfo ZIndex
		{
			get
			{
				return this.m_zIndex;
			}
			set
			{
				this.m_zIndex = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_map.Name;
			}
		}

		internal MapSubItemExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal MapSubItem()
		{
		}

		internal MapSubItem(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_mapLocation != null)
			{
				this.m_mapLocation.Initialize(context);
			}
			if (this.m_mapSize != null)
			{
				this.m_mapSize.Initialize(context);
			}
			if (this.m_leftMargin != null)
			{
				this.m_leftMargin.Initialize("LeftMargin", context);
				context.ExprHostBuilder.MapSubItemLeftMargin(this.m_leftMargin);
			}
			if (this.m_rightMargin != null)
			{
				this.m_rightMargin.Initialize("RightMargin", context);
				context.ExprHostBuilder.MapSubItemRightMargin(this.m_rightMargin);
			}
			if (this.m_topMargin != null)
			{
				this.m_topMargin.Initialize("TopMargin", context);
				context.ExprHostBuilder.MapSubItemTopMargin(this.m_topMargin);
			}
			if (this.m_bottomMargin != null)
			{
				this.m_bottomMargin.Initialize("BottomMargin", context);
				context.ExprHostBuilder.MapSubItemBottomMargin(this.m_bottomMargin);
			}
			if (this.m_zIndex != null)
			{
				this.m_zIndex.Initialize("ZIndex", context);
				context.ExprHostBuilder.MapSubItemZIndex(this.m_zIndex);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSubItem mapSubItem = (MapSubItem)base.PublishClone(context);
			if (this.m_mapLocation != null)
			{
				mapSubItem.m_mapLocation = (MapLocation)this.m_mapLocation.PublishClone(context);
			}
			if (this.m_mapSize != null)
			{
				mapSubItem.m_mapSize = (MapSize)this.m_mapSize.PublishClone(context);
			}
			if (this.m_leftMargin != null)
			{
				mapSubItem.m_leftMargin = (ExpressionInfo)this.m_leftMargin.PublishClone(context);
			}
			if (this.m_rightMargin != null)
			{
				mapSubItem.m_rightMargin = (ExpressionInfo)this.m_rightMargin.PublishClone(context);
			}
			if (this.m_topMargin != null)
			{
				mapSubItem.m_topMargin = (ExpressionInfo)this.m_topMargin.PublishClone(context);
			}
			if (this.m_bottomMargin != null)
			{
				mapSubItem.m_bottomMargin = (ExpressionInfo)this.m_bottomMargin.PublishClone(context);
			}
			if (this.m_zIndex != null)
			{
				mapSubItem.m_zIndex = (ExpressionInfo)this.m_zIndex.PublishClone(context);
			}
			return mapSubItem;
		}

		internal void SetExprHost(MapSubItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapLocation != null && this.ExprHost.MapLocationHost != null)
			{
				this.m_mapLocation.SetExprHost(this.ExprHost.MapLocationHost, reportObjectModel);
			}
			if (this.m_mapSize != null && this.ExprHost.MapSizeHost != null)
			{
				this.m_mapSize.SetExprHost(this.ExprHost.MapSizeHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapLocation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLocation));
			list.Add(new MemberInfo(MemberName.MapSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSize));
			list.Add(new MemberInfo(MemberName.LeftMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ZIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapSubItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapLocation:
					writer.Write(this.m_mapLocation);
					break;
				case MemberName.MapSize:
					writer.Write(this.m_mapSize);
					break;
				case MemberName.LeftMargin:
					writer.Write(this.m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(this.m_rightMargin);
					break;
				case MemberName.TopMargin:
					writer.Write(this.m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(this.m_bottomMargin);
					break;
				case MemberName.ZIndex:
					writer.Write(this.m_zIndex);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
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
			reader.RegisterDeclaration(MapSubItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapLocation:
					this.m_mapLocation = (MapLocation)reader.ReadRIFObject();
					break;
				case MemberName.MapSize:
					this.m_mapSize = (MapSize)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					this.m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					this.m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopMargin:
					this.m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					this.m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ZIndex:
					this.m_zIndex = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem;
		}

		internal string EvaluateLeftMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemLeftMarginExpression(this, base.m_map.Name);
		}

		internal string EvaluateRightMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemRightMarginExpression(this, base.m_map.Name);
		}

		internal string EvaluateTopMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemTopMarginExpression(this, base.m_map.Name);
		}

		internal string EvaluateBottomMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemBottomMarginExpression(this, base.m_map.Name);
		}

		internal int EvaluateZIndex(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSubItemZIndexExpression(this, base.m_map.Name);
		}
	}
}
