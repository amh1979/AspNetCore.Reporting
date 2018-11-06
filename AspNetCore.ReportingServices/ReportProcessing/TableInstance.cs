using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableInstance : ReportItemInstance, IShowHideContainer, IPageItem
	{
		private TableRowInstance[] m_headerRowInstances;

		private TableGroupInstanceList m_tableGroupInstances;

		private TableDetailInstanceList m_tableDetailInstances;

		private TableRowInstance[] m_footerRowInstances;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal TableRowInstance[] HeaderRowInstances
		{
			get
			{
				return this.m_headerRowInstances;
			}
			set
			{
				this.m_headerRowInstances = value;
			}
		}

		internal TableGroupInstanceList TableGroupInstances
		{
			get
			{
				return this.m_tableGroupInstances;
			}
			set
			{
				this.m_tableGroupInstances = value;
			}
		}

		internal TableDetailInstanceList TableDetailInstances
		{
			get
			{
				return this.m_tableDetailInstances;
			}
			set
			{
				this.m_tableDetailInstances = value;
			}
		}

		internal TableRowInstance[] FooterRowInstances
		{
			get
			{
				return this.m_footerRowInstances;
			}
			set
			{
				this.m_footerRowInstances = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return this.m_renderingPages;
			}
			set
			{
				this.m_renderingPages = value;
			}
		}

		internal int CurrentPage
		{
			get
			{
				return this.m_currentPage;
			}
			set
			{
				this.m_currentPage = value;
			}
		}

		internal int NumberOfChildrenOnThisPage
		{
			get
			{
				return this.m_numberOfChildrenOnThisPage;
			}
			set
			{
				this.m_numberOfChildrenOnThisPage = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		internal TableInstance(ReportProcessing.ProcessingContext pc, Table reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			this.ConstructorHelper(pc, reportItemDef);
			if (reportItemDef.TableGroups == null && reportItemDef.TableDetail != null)
			{
				this.m_tableDetailInstances = new TableDetailInstanceList();
			}
			this.m_renderingPages = new RenderingPagesRangesList();
			this.m_currentPage = reportItemDef.StartPage;
			reportItemDef.CurrentPage = reportItemDef.StartPage;
		}

		internal TableInstance(ReportProcessing.ProcessingContext pc, Table reportItemDef, TableDetailInstanceList tableDetailInstances, RenderingPagesRangesList renderingPages)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			this.ConstructorHelper(pc, reportItemDef);
			if (reportItemDef.TableGroups == null && reportItemDef.TableDetail != null)
			{
				this.m_tableDetailInstances = tableDetailInstances;
				this.m_renderingPages = renderingPages;
			}
			this.m_currentPage = reportItemDef.StartPage;
			reportItemDef.CurrentPage = reportItemDef.StartPage;
			reportItemDef.BottomInEndPage = pc.Pagination.CurrentPageHeight;
		}

		internal TableInstance()
		{
		}

		private void ConstructorHelper(ReportProcessing.ProcessingContext pc, Table reportItemDef)
		{
			base.m_instanceInfo = new TableInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			IndexedExprHost visibilityHiddenExprHost = (reportItemDef.TableExprHost != null) ? reportItemDef.TableExprHost.TableRowVisibilityHiddenExpressions : null;
			if (reportItemDef.HeaderRows != null)
			{
				this.m_headerRowInstances = new TableRowInstance[reportItemDef.HeaderRows.Count];
				for (int i = 0; i < this.m_headerRowInstances.Length; i++)
				{
					this.m_headerRowInstances[i] = new TableRowInstance(pc, reportItemDef.HeaderRows[i], reportItemDef, visibilityHiddenExprHost);
				}
			}
			if (reportItemDef.FooterRows != null)
			{
				this.m_footerRowInstances = new TableRowInstance[reportItemDef.FooterRows.Count];
				for (int j = 0; j < this.m_footerRowInstances.Length; j++)
				{
					this.m_footerRowInstances[j] = new TableRowInstance(pc, reportItemDef.FooterRows[j], reportItemDef, visibilityHiddenExprHost);
				}
			}
			if (reportItemDef.TableGroups != null)
			{
				this.m_tableGroupInstances = new TableGroupInstanceList();
			}
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			if (this.m_headerRowInstances != null)
			{
				int num = this.m_headerRowInstances.Length;
				for (int i = 0; i < num; i++)
				{
					obj = ((ISearchByUniqueName)this.m_headerRowInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (this.m_tableGroupInstances != null)
			{
				int count = this.m_tableGroupInstances.Count;
				for (int j = 0; j < count; j++)
				{
					obj = ((ISearchByUniqueName)this.m_tableGroupInstances[j]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			else if (this.m_tableDetailInstances != null)
			{
				int count2 = this.m_tableDetailInstances.Count;
				for (int k = 0; k < count2; k++)
				{
					obj = ((ISearchByUniqueName)this.m_tableDetailInstances[k]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (this.m_footerRowInstances != null)
			{
				int num2 = this.m_footerRowInstances.Length;
				for (int l = 0; l < num2; l++)
				{
					obj = ((ISearchByUniqueName)this.m_footerRowInstances[l]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			return null;
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRowInstances, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.TableGroupInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroupInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailStartUniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetailInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetailInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRowInstances, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadTableInstanceInfo((Table)base.m_reportItemDef);
		}
	}
}
