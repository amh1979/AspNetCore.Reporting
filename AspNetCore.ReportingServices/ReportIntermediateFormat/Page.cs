using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class Page : IDOwner, IPersistable, IStyleContainer, IRIFReportScope, IInstancePath, IAggregateHolder
	{
		[NonSerialized]
		internal const int UpgradedExprHostId = 0;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		private string m_pageHeight = "11in";

		private double m_pageHeightValue;

		private string m_pageWidth = "8.5in";

		private double m_pageWidthValue;

		private string m_leftMargin = "0in";

		private double m_leftMarginValue;

		private string m_rightMargin = "0in";

		private double m_rightMarginValue;

		private string m_topMargin = "0in";

		private double m_topMarginValue;

		private string m_bottomMargin = "0in";

		private double m_bottomMarginValue;

		private string m_interactiveHeight;

		private double m_interactiveHeightValue = -1.0;

		private string m_interactiveWidth;

		private double m_interactiveWidthValue = -1.0;

		private int m_columns = 1;

		private string m_columnSpacing = "0.5in";

		private double m_columnSpacingValue;

		private int m_exprHostID = -1;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<TextBox> m_inScopeTextBoxes;

		private List<DataAggregateInfo> m_pageAggregates;

		[NonSerialized]
		private ReportSize m_columnSpacingForRendering;

		[NonSerialized]
		private ReportSize m_pageWidthForRendering;

		[NonSerialized]
		private ReportSize m_pageHeightForRendering;

		private Style m_styleClass;

		[NonSerialized]
		private ReportSize m_bottomMarginForRendering;

		[NonSerialized]
		private ReportSize m_topMarginForRendering;

		[NonSerialized]
		private ReportSize m_rightMarginForRendering;

		[NonSerialized]
		private ReportSize m_leftMarginForRendering;

		[NonSerialized]
		private ReportSize m_interactiveHeightForRendering;

		[NonSerialized]
		private ReportSize m_interactiveWidthForRendering;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Page.GetDeclaration();

		internal PageSection PageHeader
		{
			get
			{
				return this.m_pageHeader;
			}
			set
			{
				this.m_pageHeader = value;
			}
		}

		internal bool UpgradedSnapshotPageHeaderEvaluation
		{
			get
			{
				if (this.m_pageHeader == null)
				{
					return false;
				}
				return this.m_pageHeader.UpgradedSnapshotPostProcessEvaluate;
			}
		}

		internal PageSection PageFooter
		{
			get
			{
				return this.m_pageFooter;
			}
			set
			{
				this.m_pageFooter = value;
			}
		}

		internal bool UpgradedSnapshotPageFooterEvaluation
		{
			get
			{
				if (this.m_pageFooter == null)
				{
					return false;
				}
				return this.m_pageFooter.UpgradedSnapshotPostProcessEvaluate;
			}
		}

		internal string PageHeight
		{
			get
			{
				return this.m_pageHeight;
			}
			set
			{
				this.m_pageHeight = value;
			}
		}

		internal double PageHeightValue
		{
			get
			{
				return this.m_pageHeightValue;
			}
			set
			{
				this.m_pageHeightValue = value;
			}
		}

		internal string PageWidth
		{
			get
			{
				return this.m_pageWidth;
			}
			set
			{
				this.m_pageWidth = value;
			}
		}

		internal double PageWidthValue
		{
			get
			{
				return this.m_pageWidthValue;
			}
			set
			{
				this.m_pageWidthValue = value;
			}
		}

		internal string LeftMargin
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

		internal double LeftMarginValue
		{
			get
			{
				return this.m_leftMarginValue;
			}
			set
			{
				this.m_leftMarginValue = value;
			}
		}

		internal string RightMargin
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

		internal double RightMarginValue
		{
			get
			{
				return this.m_rightMarginValue;
			}
			set
			{
				this.m_rightMarginValue = value;
			}
		}

		internal string TopMargin
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

		internal double TopMarginValue
		{
			get
			{
				return this.m_topMarginValue;
			}
			set
			{
				this.m_topMarginValue = value;
			}
		}

		internal string BottomMargin
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

		internal double BottomMarginValue
		{
			get
			{
				return this.m_bottomMarginValue;
			}
			set
			{
				this.m_bottomMarginValue = value;
			}
		}

		internal ReportSize PageHeightForRendering
		{
			get
			{
				return this.m_pageHeightForRendering;
			}
			set
			{
				this.m_pageHeightForRendering = value;
			}
		}

		internal ReportSize PageWidthForRendering
		{
			get
			{
				return this.m_pageWidthForRendering;
			}
			set
			{
				this.m_pageWidthForRendering = value;
			}
		}

		internal ReportSize LeftMarginForRendering
		{
			get
			{
				return this.m_leftMarginForRendering;
			}
			set
			{
				this.m_leftMarginForRendering = value;
			}
		}

		internal ReportSize RightMarginForRendering
		{
			get
			{
				return this.m_rightMarginForRendering;
			}
			set
			{
				this.m_rightMarginForRendering = value;
			}
		}

		internal ReportSize TopMarginForRendering
		{
			get
			{
				return this.m_topMarginForRendering;
			}
			set
			{
				this.m_topMarginForRendering = value;
			}
		}

		internal ReportSize BottomMarginForRendering
		{
			get
			{
				return this.m_bottomMarginForRendering;
			}
			set
			{
				this.m_bottomMarginForRendering = value;
			}
		}

		internal string InteractiveHeight
		{
			get
			{
				return this.m_interactiveHeight;
			}
			set
			{
				this.m_interactiveHeight = value;
			}
		}

		internal double InteractiveHeightValue
		{
			get
			{
				if (!(this.m_interactiveHeightValue < 0.0))
				{
					return this.m_interactiveHeightValue;
				}
				return this.m_pageHeightValue;
			}
			set
			{
				this.m_interactiveHeightValue = value;
			}
		}

		internal string InteractiveWidth
		{
			get
			{
				return this.m_interactiveWidth;
			}
			set
			{
				this.m_interactiveWidth = value;
			}
		}

		internal double InteractiveWidthValue
		{
			get
			{
				if (!(this.m_interactiveWidthValue < 0.0))
				{
					return this.m_interactiveWidthValue;
				}
				return this.m_pageWidthValue;
			}
			set
			{
				this.m_interactiveWidthValue = value;
			}
		}

		internal int Columns
		{
			get
			{
				return this.m_columns;
			}
			set
			{
				this.m_columns = value;
			}
		}

		internal string ColumnSpacing
		{
			get
			{
				return this.m_columnSpacing;
			}
			set
			{
				this.m_columnSpacing = value;
			}
		}

		internal double ColumnSpacingValue
		{
			get
			{
				return this.m_columnSpacingValue;
			}
			set
			{
				this.m_columnSpacingValue = value;
			}
		}

		internal ReportSize ColumnSpacingForRendering
		{
			get
			{
				return this.m_columnSpacingForRendering;
			}
			set
			{
				this.m_columnSpacingForRendering = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this;
			}
		}

		public Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Page;
			}
		}

		public string Name
		{
			get
			{
				return "Page";
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal ReportSize InteractiveHeightForRendering
		{
			get
			{
				return this.m_interactiveHeightForRendering;
			}
			set
			{
				this.m_interactiveHeightForRendering = value;
			}
		}

		internal ReportSize InteractiveWidthForRendering
		{
			get
			{
				return this.m_interactiveWidthForRendering;
			}
			set
			{
				this.m_interactiveWidthForRendering = value;
			}
		}

		internal List<DataAggregateInfo> PageAggregates
		{
			get
			{
				return this.m_pageAggregates;
			}
			set
			{
				this.m_pageAggregates = value;
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

		DataScopeInfo IAggregateHolder.DataScopeInfo
		{
			get
			{
				return null;
			}
		}

		internal Page()
		{
		}

		internal Page(int id)
			: base(id)
		{
			this.m_pageAggregates = new List<DataAggregateInfo>();
		}

		internal double GetPageSectionWidth(double width)
		{
			if (this.m_columns > 1)
			{
				width += (double)(this.m_columns - 1) * (width + this.m_columnSpacingValue);
			}
			return width;
		}

		internal void Initialize(InitializationContext context)
		{
			this.m_pageHeightValue = context.ValidateSize(ref this.m_pageHeight, "PageHeight");
			if (this.m_pageHeightValue <= 0.0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "PageHeight", this.m_pageHeightValue.ToString(CultureInfo.InvariantCulture));
			}
			this.m_pageWidthValue = context.ValidateSize(ref this.m_pageWidth, "PageWidth");
			if (this.m_pageWidthValue <= 0.0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "PageWidth", this.m_pageWidthValue.ToString(CultureInfo.InvariantCulture));
			}
			if (this.m_interactiveHeight != null)
			{
				this.m_interactiveHeightValue = context.ValidateSize(ref this.m_interactiveHeight, false, "InteractiveHeight");
				if (0.0 == this.m_interactiveHeightValue)
				{
					this.m_interactiveHeightValue = 1.7976931348623157E+308;
				}
				else if (this.m_interactiveHeightValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "InteractiveHeight", this.m_interactiveHeightValue.ToString(CultureInfo.InvariantCulture));
				}
			}
			if (this.m_interactiveWidth != null)
			{
				this.m_interactiveWidthValue = context.ValidateSize(ref this.m_interactiveWidth, false, "InteractiveWidth");
				if (0.0 == this.m_interactiveWidthValue)
				{
					this.m_interactiveWidthValue = 1.7976931348623157E+308;
				}
				else if (this.m_interactiveWidthValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "InteractiveWidth", this.m_interactiveWidthValue.ToString(CultureInfo.InvariantCulture));
				}
			}
			this.m_leftMarginValue = context.ValidateSize(ref this.m_leftMargin, "LeftMargin");
			this.m_rightMarginValue = context.ValidateSize(ref this.m_rightMargin, "RightMargin");
			this.m_topMarginValue = context.ValidateSize(ref this.m_topMargin, "TopMargin");
			this.m_bottomMarginValue = context.ValidateSize(ref this.m_bottomMargin, "BottomMargin");
			this.m_columnSpacingValue = context.ValidateSize(ref this.m_columnSpacing, "ColumnSpacing");
			if (this.m_styleClass != null)
			{
				context.ExprHostBuilder.PageStart();
				this.m_styleClass.Initialize(context);
				this.ExprHostID = context.ExprHostBuilder.PageEnd();
			}
		}

		internal void PageHeaderFooterInitialize(InitializationContext context)
		{
			context.RegisterPageSectionScope(this, this.m_pageAggregates);
			if (this.m_pageHeader != null)
			{
				context.RegisterReportItems(this.m_pageHeader.ReportItems);
			}
			if (this.m_pageFooter != null)
			{
				context.RegisterReportItems(this.m_pageFooter.ReportItems);
			}
			this.m_textboxesInScope = context.GetCurrentReferencableTextboxesInSection();
			if (this.m_pageHeader != null)
			{
				this.m_pageHeader.Initialize(context);
			}
			if (this.m_pageFooter != null)
			{
				this.m_pageFooter.Initialize(context);
			}
			if (this.m_pageHeader != null)
			{
				context.UnRegisterReportItems(this.m_pageHeader.ReportItems);
			}
			if (this.m_pageFooter != null)
			{
				context.UnRegisterReportItems(this.m_pageFooter.ReportItems);
			}
			context.ValidateToggleItems();
			context.UnRegisterPageSectionScope();
		}

		internal void SetExprHost(ReportExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			if (this.m_styleClass != null && this.ExprHostID >= 0)
			{
				StyleExprHost styleExprHost = null;
				if (exprHost.PageHostsRemotable != null)
				{
					styleExprHost = exprHost.PageHostsRemotable[this.ExprHostID];
				}
				else if (this.ExprHostID == 0)
				{
					styleExprHost = exprHost.PageHost;
					if (styleExprHost == null)
					{
						return;
					}
				}
				else
				{
					Global.Tracer.Assert(false, "Missing ReportExprHost.PageHostRemotable for Page ExprHostID: {0}", this.ExprHostID);
				}
				styleExprHost.SetReportObjectModel(reportObjectModel);
				this.m_styleClass.SetStyleExprHost(styleExprHost);
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

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return this.m_pageAggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return null;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_pageAggregates, "(null != m_pageAggregates)");
			if (this.m_pageAggregates.Count == 0)
			{
				this.m_pageAggregates = null;
			}
		}

		internal void SetTextboxesInScope(byte[] items)
		{
			this.m_textboxesInScope = items;
		}

		internal void SetInScopeTextBoxes(List<TextBox> items)
		{
			this.m_inScopeTextBoxes = items;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PageHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection));
			list.Add(new MemberInfo(MemberName.PageFooter, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection));
			list.Add(new MemberInfo(MemberName.PageHeight, Token.String));
			list.Add(new MemberInfo(MemberName.PageHeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.PageWidth, Token.String));
			list.Add(new MemberInfo(MemberName.PageWidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.LeftMargin, Token.String));
			list.Add(new MemberInfo(MemberName.LeftMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.RightMargin, Token.String));
			list.Add(new MemberInfo(MemberName.RightMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.TopMargin, Token.String));
			list.Add(new MemberInfo(MemberName.TopMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.BottomMargin, Token.String));
			list.Add(new MemberInfo(MemberName.BottomMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.InteractiveHeight, Token.String));
			list.Add(new MemberInfo(MemberName.InteractiveHeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.InteractiveWidth, Token.String));
			list.Add(new MemberInfo(MemberName.InteractiveWidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Columns, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColumnSpacing, Token.String));
			list.Add(new MemberInfo(MemberName.ColumnSpacingValue, Token.Double));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.PageAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Page.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageHeader:
					writer.Write(this.m_pageHeader);
					break;
				case MemberName.PageFooter:
					writer.Write(this.m_pageFooter);
					break;
				case MemberName.PageHeight:
					writer.Write(this.m_pageHeight);
					break;
				case MemberName.PageHeightValue:
					writer.Write(this.m_pageHeightValue);
					break;
				case MemberName.PageWidth:
					writer.Write(this.m_pageWidth);
					break;
				case MemberName.PageWidthValue:
					writer.Write(this.m_pageWidthValue);
					break;
				case MemberName.LeftMargin:
					writer.Write(this.m_leftMargin);
					break;
				case MemberName.LeftMarginValue:
					writer.Write(this.m_leftMarginValue);
					break;
				case MemberName.RightMargin:
					writer.Write(this.m_rightMargin);
					break;
				case MemberName.RightMarginValue:
					writer.Write(this.m_rightMarginValue);
					break;
				case MemberName.TopMargin:
					writer.Write(this.m_topMargin);
					break;
				case MemberName.TopMarginValue:
					writer.Write(this.m_topMarginValue);
					break;
				case MemberName.BottomMargin:
					writer.Write(this.m_bottomMargin);
					break;
				case MemberName.BottomMarginValue:
					writer.Write(this.m_bottomMarginValue);
					break;
				case MemberName.InteractiveHeight:
					writer.Write(this.m_interactiveHeight);
					break;
				case MemberName.InteractiveHeightValue:
					writer.Write(this.m_interactiveHeightValue);
					break;
				case MemberName.InteractiveWidth:
					writer.Write(this.m_interactiveWidth);
					break;
				case MemberName.InteractiveWidthValue:
					writer.Write(this.m_interactiveWidthValue);
					break;
				case MemberName.Columns:
					writer.Write(this.m_columns);
					break;
				case MemberName.ColumnSpacing:
					writer.Write(this.m_columnSpacing);
					break;
				case MemberName.ColumnSpacingValue:
					writer.Write(this.m_columnSpacingValue);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(this.m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(this.m_inScopeTextBoxes);
					break;
				case MemberName.PageAggregates:
					writer.Write(this.m_pageAggregates);
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
			reader.RegisterDeclaration(Page.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageHeader:
					this.m_pageHeader = (PageSection)reader.ReadRIFObject();
					break;
				case MemberName.PageFooter:
					this.m_pageFooter = (PageSection)reader.ReadRIFObject();
					break;
				case MemberName.PageHeight:
					this.m_pageHeight = reader.ReadString();
					break;
				case MemberName.PageHeightValue:
					this.m_pageHeightValue = reader.ReadDouble();
					break;
				case MemberName.PageWidth:
					this.m_pageWidth = reader.ReadString();
					break;
				case MemberName.PageWidthValue:
					this.m_pageWidthValue = reader.ReadDouble();
					break;
				case MemberName.LeftMargin:
					this.m_leftMargin = reader.ReadString();
					break;
				case MemberName.LeftMarginValue:
					this.m_leftMarginValue = reader.ReadDouble();
					break;
				case MemberName.RightMargin:
					this.m_rightMargin = reader.ReadString();
					break;
				case MemberName.RightMarginValue:
					this.m_rightMarginValue = reader.ReadDouble();
					break;
				case MemberName.TopMargin:
					this.m_topMargin = reader.ReadString();
					break;
				case MemberName.TopMarginValue:
					this.m_topMarginValue = reader.ReadDouble();
					break;
				case MemberName.BottomMargin:
					this.m_bottomMargin = reader.ReadString();
					break;
				case MemberName.BottomMarginValue:
					this.m_bottomMarginValue = reader.ReadDouble();
					break;
				case MemberName.InteractiveHeight:
					this.m_interactiveHeight = reader.ReadString();
					break;
				case MemberName.InteractiveHeightValue:
					this.m_interactiveHeightValue = reader.ReadDouble();
					break;
				case MemberName.InteractiveWidth:
					this.m_interactiveWidth = reader.ReadString();
					break;
				case MemberName.InteractiveWidthValue:
					this.m_interactiveWidthValue = reader.ReadDouble();
					break;
				case MemberName.Columns:
					this.m_columns = reader.ReadInt32();
					break;
				case MemberName.ColumnSpacing:
					this.m_columnSpacing = reader.ReadString();
					break;
				case MemberName.ColumnSpacingValue:
					this.m_columnSpacingValue = reader.ReadDouble();
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
					this.m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.InScopeTextBoxes:
					this.m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.PageAggregates:
					this.m_pageAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
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
			if (memberReferencesCollection.TryGetValue(Page.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page;
		}
	}
}
