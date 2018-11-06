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
	internal sealed class Rectangle : ReportItem, IPageBreakOwner, IPersistable
	{
		private ReportItemCollection m_reportItems;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		private int m_linkToChild = -1;

		private bool m_keepTogether;

		private bool m_omitBorderOnPageBreak;

		private bool m_isSimple;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Rectangle.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return this.m_reportItems;
			}
			set
			{
				this.m_reportItems = value;
			}
		}

		internal int LinkToChild
		{
			get
			{
				return this.m_linkToChild;
			}
			set
			{
				this.m_linkToChild = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return this.m_keepTogether;
			}
			set
			{
				this.m_keepTogether = value;
			}
		}

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return this.m_omitBorderOnPageBreak;
			}
			set
			{
				this.m_omitBorderOnPageBreak = value;
			}
		}

		internal bool IsSimple
		{
			get
			{
				return this.m_isSimple;
			}
			set
			{
				this.m_isSimple = value;
			}
		}

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				return DataElementOutputTypes.ContentsOnly;
			}
		}

		internal ExpressionInfo PageName
		{
			get
			{
				return this.m_pageName;
			}
			set
			{
				this.m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle;
			}
		}

		string IPageBreakOwner.ObjectName
		{
			get
			{
				return base.m_name;
			}
		}

		IInstancePath IPageBreakOwner.InstancePath
		{
			get
			{
				return this;
			}
		}

		internal Rectangle(ReportItem parent)
			: base(parent)
		{
		}

		internal Rectangle(int id, int idForReportItems, ReportItem parent)
			: base(id, parent)
		{
			this.m_reportItems = new ReportItemCollection(idForReportItems, true);
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			base.CalculateSizes(width, height, context, overwrite);
			this.m_reportItems.CalculateSizes(context, false);
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.RectangleStart(base.m_name);
			this.m_isSimple = (base.m_toolTip == null && base.m_documentMapLabel == null && base.m_bookmark == null && base.m_styleClass == null && base.m_visibility == null);
			base.Initialize(context);
			context.InitializeAbsolutePosition(this);
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.Initialize(context);
			}
			if (this.m_pageName != null)
			{
				this.m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(this.m_pageName);
			}
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			bool flag = context.RegisterVisibility(base.m_visibility, this);
			context.IsTopLevelCellContents = false;
			this.m_reportItems.Initialize(context);
			if (flag)
			{
				context.UnRegisterVisibility(base.m_visibility, this);
			}
			base.ExprHostID = context.ExprHostBuilder.RectangleEnd();
			return false;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_reportItems != null)
			{
				foreach (ReportItem reportItem in this.m_reportItems)
				{
					reportItem.TraverseScopes(visitor);
				}
			}
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (this.m_reportItems != null)
			{
				this.m_reportItems.InitializeRVDirectionDependentItems(context);
			}
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (this.m_reportItems != null)
			{
				this.m_reportItems.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		internal bool ContainsDataRegionOrSubReport()
		{
			for (int i = 0; i < this.m_reportItems.Count; i++)
			{
				ReportItem reportItem = this.m_reportItems[i];
				if (reportItem.IsOrContainsDataRegionOrSubReport())
				{
					return true;
				}
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.LinkToChild, Token.Int32));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSimple, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Rectangle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportItems:
					writer.Write(this.m_reportItems);
					break;
				case MemberName.LinkToChild:
					writer.Write(this.m_linkToChild);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(this.m_omitBorderOnPageBreak);
					break;
				case MemberName.KeepTogether:
					writer.Write(this.m_keepTogether);
					break;
				case MemberName.IsSimple:
					writer.Write(this.m_isSimple);
					break;
				case MemberName.PageBreak:
					writer.Write(this.m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(this.m_pageName);
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
			reader.RegisterDeclaration(Rectangle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportItems:
					this.m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakLocation:
					this.m_pageBreak = new PageBreak();
					this.m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.LinkToChild:
					this.m_linkToChild = reader.ReadInt32();
					break;
				case MemberName.OmitBorderOnPageBreak:
					this.m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					this.m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.IsSimple:
					this.m_isSimple = reader.ReadBoolean();
					break;
				case MemberName.PageBreak:
					this.m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					this.m_pageName = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Rectangle rectangle = (Rectangle)base.PublishClone(context);
			if (this.m_reportItems != null)
			{
				rectangle.m_reportItems = (ReportItemCollection)this.m_reportItems.PublishClone(context);
			}
			if (this.m_pageBreak != null)
			{
				rectangle.m_pageBreak = (PageBreak)this.m_pageBreak.PublishClone(context);
			}
			if (this.m_pageName != null)
			{
				rectangle.m_pageName = (ExpressionInfo)this.m_pageName.PublishClone(context);
			}
			return rectangle;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.RectangleHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_pageBreak != null && this.m_exprHost.PageBreakExprHost != null)
				{
					this.m_pageBreak.SetExprHost(this.m_exprHost.PageBreakExprHost, reportObjectModel);
				}
			}
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateRectanglePageNameExpression(this, this.m_pageName, base.m_name);
		}
	}
}
