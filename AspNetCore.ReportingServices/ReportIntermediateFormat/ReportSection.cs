using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportSection : ReportItem, IPersistable, IRIFReportScope, IInstancePath
	{
		[NonSerialized]
		internal const int UpgradedExprHostId = 0;

		private ReportItemCollection m_reportItems;

		private Page m_page;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<TextBox> m_inScopeTextBoxes;

		private bool m_needsOverallTotalPages;

		private bool m_needsPageBreakTotalPages;

		private bool m_needsReportItemsOnPage;

		private bool m_layoutDirection;

		[NonSerialized]
		private int m_publishingIndexInCollection = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportSection.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportSection;
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

		internal Page Page
		{
			get
			{
				return this.m_page;
			}
			set
			{
				this.m_page = value;
			}
		}

		internal double PageSectionWidth
		{
			get
			{
				return this.m_page.GetPageSectionWidth(base.m_widthValue);
			}
		}

		internal override string DataElementNameDefault
		{
			get
			{
				return "ReportSection" + this.m_publishingIndexInCollection.ToString(CultureInfo.InvariantCulture);
			}
		}

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				return DataElementOutputTypes.ContentsOnly;
			}
		}

		internal bool NeedsOverallTotalPages
		{
			get
			{
				return this.m_needsOverallTotalPages;
			}
			set
			{
				this.m_needsOverallTotalPages = value;
			}
		}

		internal bool NeedsPageBreakTotalPages
		{
			get
			{
				return this.m_needsOverallTotalPages;
			}
			set
			{
				this.m_needsOverallTotalPages = value;
			}
		}

		internal bool NeedsReportItemsOnPage
		{
			get
			{
				return this.m_needsReportItemsOnPage;
			}
			set
			{
				this.m_needsReportItemsOnPage = value;
			}
		}

		internal bool LayoutDirection
		{
			get
			{
				return this.m_layoutDirection;
			}
			set
			{
				this.m_layoutDirection = value;
			}
		}

		public bool NeedToCacheDataRows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal ReportSection()
			: base(null)
		{
		}

		internal ReportSection(int indexInCollection)
			: this()
		{
			this.m_publishingIndexInCollection = indexInCollection;
		}

		internal ReportSection(int indexInCollection, Report report, int id, int idForReportItems)
			: base(id, report)
		{
			this.m_publishingIndexInCollection = indexInCollection;
			base.m_height = "11in";
			base.m_width = "8.5in";
			this.m_reportItems = new ReportItemCollection(idForReportItems, true);
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			ReportItemExprHost exprHost = null;
			if (base.ExprHostID >= 0)
			{
				if (reportExprHost.ReportSectionHostsRemotable != null)
				{
					exprHost = reportExprHost.ReportSectionHostsRemotable[base.ExprHostID];
				}
				else if (base.ExprHostID == 0)
				{
					exprHost = reportExprHost;
				}
				else
				{
					Global.Tracer.Assert(false, "Missing ExprHost for Body. ExprHostID: {0}", base.ExprHostID);
				}
				base.ReportItemSetExprHost(exprHost, reportObjectModel);
			}
			if (this.m_page != null)
			{
				this.m_page.SetExprHost(reportExprHost, reportObjectModel);
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = this.ObjectType;
			context.ObjectName = this.DataElementNameDefault;
			context.RegisterReportSection(this);
			context.ExprHostBuilder.ReportSectionStart();
			base.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.ReportSectionEnd();
			this.m_page.Initialize(context);
			this.BodyInitialize(context);
			context.ValidateToggleItems();
			this.m_page.PageHeaderFooterInitialize(context);
			context.UnRegisterReportSection();
			return false;
		}

		internal void BodyInitialize(InitializationContext context)
		{
			context.RegisterReportItems(this.m_reportItems);
			this.m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			this.m_variablesInScope = context.GetCurrentReferencableVariables();
			this.m_reportItems.Initialize(context);
			Report report = context.Report;
			if (report.HasUserSortFilter || report.HasSubReports)
			{
				context.InitializingUserSorts = true;
				this.m_reportItems.InitializeRVDirectionDependentItems(context);
				context.EventSourcesWithDetailSortExpressionInitialize(null);
				List<DataSource> dataSources = report.DataSources;
				if (dataSources != null)
				{
					for (int i = 0; i < dataSources.Count; i++)
					{
						List<DataSet> dataSets = dataSources[i].DataSets;
						if (dataSets != null)
						{
							for (int j = 0; j < dataSets.Count; j++)
							{
								context.ProcessUserSortScopes(dataSets[j].Name);
							}
						}
					}
				}
				context.ProcessUserSortScopes("0_ReportScope");
			}
			if (report.HasPreviousAggregates)
			{
				this.m_reportItems.DetermineGroupingExprValueCount(context, 0);
			}
			context.UnRegisterReportItems(this.m_reportItems);
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			foreach (ReportItem reportItem in this.m_reportItems)
			{
				reportItem.TraverseScopes(visitor);
			}
		}

		public bool TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_textboxesInScope, sequenceIndex, true);
		}

		public void AddInScopeTextBox(TextBox textbox)
		{
			if (this.m_inScopeTextBoxes == null)
			{
				this.m_inScopeTextBoxes = new List<TextBox>();
			}
			this.m_inScopeTextBoxes.Add(textbox);
		}

		public void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (this.m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < this.m_inScopeTextBoxes.Count; i++)
				{
					this.m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}

		public bool VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_variablesInScope, sequenceIndex, true);
		}

		public void AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			Global.Tracer.Assert(false, "Top level event sources should be registered on the Report, not ReportSection");
		}

		internal void SetTextboxesInScope(byte[] items)
		{
			this.m_textboxesInScope = items;
		}

		internal void SetInScopeTextBoxes(List<TextBox> items)
		{
			this.m_inScopeTextBoxes = items;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportSection.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Page:
					writer.Write(this.m_page);
					break;
				case MemberName.ReportItems:
					writer.Write(this.m_reportItems);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(this.m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.NeedsOverallTotalPages:
					writer.Write(this.m_needsOverallTotalPages);
					break;
				case MemberName.NeedsPageBreakTotalPages:
					writer.Write(this.m_needsPageBreakTotalPages);
					break;
				case MemberName.NeedsReportItemsOnPage:
					writer.Write(this.m_needsReportItemsOnPage);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(this.m_inScopeTextBoxes);
					break;
				case MemberName.LayoutDirection:
					writer.Write(this.m_layoutDirection);
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
			reader.RegisterDeclaration(ReportSection.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Page:
					this.m_page = (Page)reader.ReadRIFObject();
					break;
				case MemberName.ReportItems:
					this.m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.TextboxesInScope:
					this.m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.NeedsTotalPages:
				case MemberName.NeedsOverallTotalPages:
					this.m_needsOverallTotalPages = reader.ReadBoolean();
					break;
				case MemberName.NeedsPageBreakTotalPages:
					this.m_needsPageBreakTotalPages = reader.ReadBoolean();
					break;
				case MemberName.NeedsReportItemsOnPage:
					this.m_needsReportItemsOnPage = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					this.m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.LayoutDirection:
					this.m_layoutDirection = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ReportSection.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item2 in list)
				{
					MemberName memberName = item2.MemberName;
					if (memberName == MemberName.InScopeTextBoxes)
					{
						if (this.m_inScopeTextBoxes == null)
						{
							this.m_inScopeTextBoxes = new List<TextBox>();
						}
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable);
						TextBox item = (TextBox)referenceable;
						this.m_inScopeTextBoxes.Add(item);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection;
		}

		public new static Declaration GetDeclaration()
		{
			if (ReportSection.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Page, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page));
				list.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
				list.Add(new MemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
				list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
				list.Add(new ReadOnlyMemberInfo(MemberName.NeedsTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NeedsOverallTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NeedsReportItemsOnPage, Token.Boolean));
				list.Add(new MemberInfo(MemberName.InScopeTextBoxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
				list.Add(new MemberInfo(MemberName.NeedsPageBreakTotalPages, Token.Boolean));
				list.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean, Lifetime.AddedIn(200)));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
			}
			return ReportSection.m_Declaration;
		}
	}
}
