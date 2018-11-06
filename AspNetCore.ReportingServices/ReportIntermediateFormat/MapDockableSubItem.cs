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
	internal class MapDockableSubItem : MapSubItem, IPersistable, IActionOwner
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapDockableSubItem.GetDeclaration();

		private Action m_action;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		private ExpressionInfo m_position;

		private ExpressionInfo m_dockOutsideViewport;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_toolTip;

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal ExpressionInfo DockOutsideViewport
		{
			get
			{
				return this.m_dockOutsideViewport;
			}
			set
			{
				this.m_dockOutsideViewport = value;
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

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		internal new MapDockableSubItemExprHost ExprHost
		{
			get
			{
				return (MapDockableSubItemExprHost)base.m_exprHost;
			}
		}

		internal MapDockableSubItem()
		{
		}

		internal MapDockableSubItem(Map map, int id)
			: base(map)
		{
			this.m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.MapDockableSubItemPosition(this.m_position);
			}
			if (this.m_dockOutsideViewport != null)
			{
				this.m_dockOutsideViewport.Initialize("DockOutsideViewport", context);
				context.ExprHostBuilder.MapDockableSubItemDockOutsideViewport(this.m_dockOutsideViewport);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapDockableSubItemHidden(this.m_hidden);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.MapDockableSubItemToolTip(this.m_toolTip);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDockableSubItem mapDockableSubItem = (MapDockableSubItem)base.PublishClone(context);
			if (this.m_action != null)
			{
				mapDockableSubItem.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_position != null)
			{
				mapDockableSubItem.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_dockOutsideViewport != null)
			{
				mapDockableSubItem.m_dockOutsideViewport = (ExpressionInfo)this.m_dockOutsideViewport.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				mapDockableSubItem.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				mapDockableSubItem.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			return mapDockableSubItem;
		}

		internal void SetExprHost(MapDockableSubItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_action != null && this.ExprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.ExprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockOutsideViewport, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapDockableSubItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.ID:
					writer.Write(this.m_id);
					break;
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.DockOutsideViewport:
					writer.Write(this.m_dockOutsideViewport);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
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
			reader.RegisterDeclaration(MapDockableSubItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ID:
					this.m_id = reader.ReadInt32();
					break;
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockOutsideViewport:
					this.m_dockOutsideViewport = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem;
		}

		internal MapPosition EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapPosition(context.ReportRuntime.EvaluateMapDockableSubItemPositionExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideViewport(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDockableSubItemDockOutsideViewportExpression(this, base.m_map.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDockableSubItemHiddenExpression(this, base.m_map.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapDockableSubItemToolTipExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}
	}
}
