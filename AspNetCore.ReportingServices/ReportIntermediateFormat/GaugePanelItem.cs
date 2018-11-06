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
	internal class GaugePanelItem : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugePanelItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugePanelItem.GetDeclaration();

		protected string m_name;

		private ExpressionInfo m_top;

		private ExpressionInfo m_left;

		private ExpressionInfo m_height;

		private ExpressionInfo m_width;

		private ExpressionInfo m_zIndex;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_toolTip;

		private string m_parentItem;

		private int m_id;

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

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ExpressionInfo Top
		{
			get
			{
				return this.m_top;
			}
			set
			{
				this.m_top = value;
			}
		}

		internal ExpressionInfo Left
		{
			get
			{
				return this.m_left;
			}
			set
			{
				this.m_left = value;
			}
		}

		internal ExpressionInfo Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal ExpressionInfo Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
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

		internal string ParentItem
		{
			get
			{
				return this.m_parentItem;
			}
			set
			{
				this.m_parentItem = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal GaugePanelItemExprHost ExprHost
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

		internal GaugePanelItem()
		{
		}

		internal GaugePanelItem(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
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
			if (this.m_top != null)
			{
				this.m_top.Initialize("Top", context);
				context.ExprHostBuilder.GaugePanelItemTop(this.m_top);
			}
			if (this.m_left != null)
			{
				this.m_left.Initialize("Left", context);
				context.ExprHostBuilder.GaugePanelItemLeft(this.m_left);
			}
			if (this.m_height != null)
			{
				this.m_height.Initialize("Height", context);
				context.ExprHostBuilder.GaugePanelItemHeight(this.m_height);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugePanelItemWidth(this.m_width);
			}
			if (this.m_zIndex != null)
			{
				this.m_zIndex.Initialize("ZIndex", context);
				context.ExprHostBuilder.GaugePanelItemZIndex(this.m_zIndex);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugePanelItemHidden(this.m_hidden);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugePanelItemToolTip(this.m_toolTip);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanelItem gaugePanelItem = (GaugePanelItem)base.PublishClone(context);
			if (this.m_action != null)
			{
				gaugePanelItem.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_top != null)
			{
				gaugePanelItem.m_top = (ExpressionInfo)this.m_top.PublishClone(context);
			}
			if (this.m_left != null)
			{
				gaugePanelItem.m_left = (ExpressionInfo)this.m_left.PublishClone(context);
			}
			if (this.m_height != null)
			{
				gaugePanelItem.m_height = (ExpressionInfo)this.m_height.PublishClone(context);
			}
			if (this.m_width != null)
			{
				gaugePanelItem.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			if (this.m_zIndex != null)
			{
				gaugePanelItem.m_zIndex = (ExpressionInfo)this.m_zIndex.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				gaugePanelItem.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				gaugePanelItem.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			return gaugePanelItem;
		}

		internal void SetExprHost(GaugePanelItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_action != null && exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Top, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Height, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ZIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ParentItem, Token.String));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugePanelItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.ZIndex:
					writer.Write(this.m_zIndex);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.ParentItem:
					writer.Write(this.m_parentItem);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ID:
					writer.Write(this.m_id);
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
			reader.RegisterDeclaration(GaugePanelItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Top:
					this.m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					this.m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Height:
					this.m_height = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ZIndex:
					this.m_zIndex = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ParentItem:
					this.m_parentItem = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ID:
					this.m_id = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (this.m_id == 0)
			{
				this.m_id = base.m_gaugePanel.GenerateActionOwnerID();
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem;
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemTopExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemLeftExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemHeightExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal int EvaluateZIndex(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemZIndexExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemToolTipExpression(this, base.m_gaugePanel.Name);
		}
	}
}
