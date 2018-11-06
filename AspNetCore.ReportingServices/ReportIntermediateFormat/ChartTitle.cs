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
	internal class ChartTitle : ChartTitleBase, IPersistable, IActionOwner
	{
		private string m_name;

		private ExpressionInfo m_position;

		protected int m_exprHostID;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_docking;

		private string m_dockToChartArea;

		private ExpressionInfo m_dockOutsideChartArea;

		private ExpressionInfo m_dockOffset;

		private ExpressionInfo m_toolTip;

		private Action m_action;

		private ExpressionInfo m_textOrientation;

		private ChartElementPosition m_chartElementPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartTitle.GetDeclaration();

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
				return null;
			}
			set
			{
			}
		}

		internal string TitleName
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

		internal ExpressionInfo Docking
		{
			get
			{
				return this.m_docking;
			}
			set
			{
				this.m_docking = value;
			}
		}

		internal string DockToChartArea
		{
			get
			{
				return this.m_dockToChartArea;
			}
			set
			{
				this.m_dockToChartArea = value;
			}
		}

		internal ExpressionInfo DockOutsideChartArea
		{
			get
			{
				return this.m_dockOutsideChartArea;
			}
			set
			{
				this.m_dockOutsideChartArea = value;
			}
		}

		internal ExpressionInfo DockOffset
		{
			get
			{
				return this.m_dockOffset;
			}
			set
			{
				this.m_dockOffset = value;
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

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return this.m_textOrientation;
			}
			set
			{
				this.m_textOrientation = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return this.m_chartElementPosition;
			}
			set
			{
				this.m_chartElementPosition = value;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal ChartTitle()
		{
		}

		internal ChartTitle(Chart chart)
			: base(chart)
		{
			base.m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartTitleStart(this.m_name);
			this.InitializeInternal(context);
			this.m_exprHostID = context.ExprHostBuilder.ChartTitleEnd();
		}

		protected void InitializeInternal(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartTitlePosition(this.m_position);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartTitleHidden(this.m_hidden);
			}
			if (this.m_docking != null)
			{
				this.m_docking.Initialize("Docking", context);
				context.ExprHostBuilder.ChartTitleDocking(this.m_docking);
			}
			string dockToChartArea = this.m_dockToChartArea;
			if (this.m_dockOutsideChartArea != null)
			{
				this.m_dockOutsideChartArea.Initialize("DockOutsideChartArea", context);
				context.ExprHostBuilder.ChartTitleDockOutsideChartArea(this.m_dockOutsideChartArea);
			}
			if (this.m_dockOffset != null)
			{
				this.m_dockOffset.Initialize("DockOffset", context);
				context.ExprHostBuilder.ChartTitleDockOffset(this.m_dockOffset);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartTitleToolTip(this.m_toolTip);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_textOrientation != null)
			{
				this.m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartTitleTextOrientation(this.m_textOrientation);
			}
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.Initialize(context);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTitle chartTitle = (ChartTitle)base.PublishClone(context);
			if (this.m_position != null)
			{
				chartTitle.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				chartTitle.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_docking != null)
			{
				chartTitle.m_docking = (ExpressionInfo)this.m_docking.PublishClone(context);
			}
			if (this.m_dockToChartArea != null)
			{
				chartTitle.m_dockToChartArea = (string)this.m_dockToChartArea.Clone();
			}
			if (this.m_dockOutsideChartArea != null)
			{
				chartTitle.m_dockOutsideChartArea = (ExpressionInfo)this.m_dockOutsideChartArea.PublishClone(context);
			}
			if (this.m_dockOffset != null)
			{
				chartTitle.m_dockOffset = (ExpressionInfo)this.m_dockOffset.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartTitle.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_action != null)
			{
				chartTitle.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_textOrientation != null)
			{
				chartTitle.m_textOrientation = (ExpressionInfo)this.m_textOrientation.PublishClone(context);
			}
			if (this.m_chartElementPosition != null)
			{
				chartTitle.m_chartElementPosition = (ChartElementPosition)this.m_chartElementPosition.PublishClone(context);
			}
			return chartTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Docking, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockToChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.DockOutsideChartArea, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.TextOrientation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartTitle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Docking:
					writer.Write(this.m_docking);
					break;
				case MemberName.DockToChartArea:
					writer.Write(this.m_dockToChartArea);
					break;
				case MemberName.DockOutsideChartArea:
					writer.Write(this.m_dockOutsideChartArea);
					break;
				case MemberName.DockOffset:
					writer.Write(this.m_dockOffset);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.TextOrientation:
					writer.Write(this.m_textOrientation);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(this.m_chartElementPosition);
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
			reader.RegisterDeclaration(ChartTitle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Docking:
					this.m_docking = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockToChartArea:
					this.m_dockToChartArea = reader.ReadString();
					break;
				case MemberName.DockOutsideChartArea:
					this.m_dockOutsideChartArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockOffset:
					this.m_dockOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					this.m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartElementPosition:
					this.m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
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
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle;
		}

		internal override void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_action != null && ((ChartTitleExprHost)exprHost).ActionInfoHost != null)
			{
				this.m_action.SetExprHost(((ChartTitleExprHost)exprHost).ActionInfoHost, reportObjectModel);
			}
			if (this.m_chartElementPosition != null && ((ChartTitleExprHost)exprHost).ChartElementPositionHost != null)
			{
				this.m_chartElementPosition.SetExprHost(((ChartTitleExprHost)exprHost).ChartElementPositionHost, reportObjectModel);
			}
		}

		internal bool EvaluateHidden(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateEvaluateChartTitleHiddenExpression(this, base.Name, "Hidden");
		}

		internal ChartTitleDockings EvaluateDocking(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartTitleDocking(context.ReportRuntime.EvaluateChartTitleDockingExpression(this, base.Name, "Docking"), context.ReportRuntime);
		}

		internal ChartTitlePositions EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartTitlePosition(context.ReportRuntime.EvaluateChartTitlePositionExpression(this, base.Name, "Position"), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideChartArea(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleDockOutsideChartAreaExpression(this, base.Name, "DockOutsideChartArea");
		}

		internal int EvaluateDockOffset(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleDockOffsetExpression(this, base.Name, "DockOffset");
		}

		internal string EvaluateToolTip(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleToolTipExpression(this, base.Name, "ToolTip");
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartTitleTextOrientationExpression(this, base.m_chart.Name), context.ReportRuntime);
		}
	}
}
