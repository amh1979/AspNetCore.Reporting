using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Rectangle : ReportItem, IPageBreakItem, IIndexInto
	{
		private ReportItemCollection m_reportItems;

		private bool m_pageBreakAtEnd;

		private bool m_pageBreakAtStart;

		private int m_linkToChild = -1;

		[NonSerialized]
		private PageBreakStates m_pagebreakState;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Rectangle;
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

		internal bool PageBreakAtEnd
		{
			get
			{
				return this.m_pageBreakAtEnd;
			}
			set
			{
				this.m_pageBreakAtEnd = value;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				return this.m_pageBreakAtStart;
			}
			set
			{
				this.m_pageBreakAtStart = value;
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
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			this.m_reportItems.Initialize(context, false);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			base.ExprHostID = context.ExprHostBuilder.RectangleEnd();
			return false;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			if (DataElementOutputTypesRDL.Auto == base.m_dataElementOutputRDL)
			{
				base.m_dataElementOutputRDL = DataElementOutputTypesRDL.ContentsOnly;
			}
			base.DataRendererInitialize(context);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			if (base.m_visibility != null)
			{
				base.m_visibility.RegisterReceiver(context, true);
			}
			this.m_reportItems.RegisterReceiver(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
		}

		internal bool ContainsDataRegionOrSubReport()
		{
			for (int i = 0; i < this.m_reportItems.Count; i++)
			{
				ReportItem reportItem = this.m_reportItems[i];
				if (reportItem is DataRegion)
				{
					return true;
				}
				if (reportItem is SubReport)
				{
					return true;
				}
				if (reportItem is Rectangle && ((Rectangle)reportItem).ContainsDataRegionOrSubReport())
				{
					return true;
				}
			}
			return false;
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (this.m_pagebreakState == PageBreakStates.Unknown)
			{
				if (base.m_repeatedSibling || SharedHiddenState.Never != Visibility.GetSharedHidden(base.m_visibility))
				{
					this.m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					this.m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == this.m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		bool IPageBreakItem.HasPageBreaks(bool atStart)
		{
			if (atStart && this.m_pageBreakAtStart)
			{
				goto IL_0016;
			}
			if (!atStart && this.m_pageBreakAtEnd)
			{
				goto IL_0016;
			}
			return false;
			IL_0016:
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.RectangleHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.LinkToChild, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}

		internal object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (nonCompNames.ChildrenUniqueNames == null)
			{
				return null;
			}
			NonComputedUniqueNames nonComputedUniqueNames = null;
			int count = this.m_reportItems.Count;
			object obj = null;
			for (int i = 0; i < count; i++)
			{
				nonComputedUniqueNames = nonCompNames.ChildrenUniqueNames[i];
				obj = ((ISearchByUniqueName)this.m_reportItems[i]).Find(targetUniqueName, ref nonComputedUniqueNames, chunkManager);
				if (obj != null)
				{
					break;
				}
			}
			if (obj != null)
			{
				nonCompNames = nonComputedUniqueNames;
				return obj;
			}
			return null;
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (nonCompNames != null && nonCompNames.ChildrenUniqueNames != null)
			{
				NonComputedUniqueNames nonComputedUniqueNames = null;
				int count = this.m_reportItems.Count;
				for (int i = 0; i < count; i++)
				{
					nonComputedUniqueNames = nonCompNames.ChildrenUniqueNames[i];
					this.m_reportItems[i].ProcessDrillthroughAction(processingContext, nonComputedUniqueNames);
				}
			}
		}

		internal int ProcessNavigationChildren(ReportProcessing.NavigationInfo navigationInfo, NonComputedUniqueNames nonCompNames, int startPage)
		{
			if (nonCompNames.ChildrenUniqueNames == null)
			{
				return -1;
			}
			NonComputedUniqueNames nonComputedUniqueNames = null;
			int count = this.m_reportItems.Count;
			int result = -1;
			for (int i = 0; i < count; i++)
			{
				nonComputedUniqueNames = nonCompNames.ChildrenUniqueNames[i];
				if (i == this.m_linkToChild)
				{
					result = nonComputedUniqueNames.UniqueName;
				}
				this.m_reportItems[i].ProcessNavigationAction(navigationInfo, nonComputedUniqueNames, startPage);
			}
			return result;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			nonCompNames = null;
			if (index >= 0 && index < this.m_reportItems.Count)
			{
				return this.m_reportItems[index];
			}
			return null;
		}
	}
}
