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
	internal sealed class TextBox : ReportItem, IActionOwner, IPersistable, IInScopeEventSource, IReferenceable, IGloballyReferenceable, IGlobalIDOwner
	{
		private List<Paragraph> m_paragraphs;

		private Action m_action;

		private bool m_canGrow;

		private bool m_canShrink;

		private string m_hideDuplicates;

		private bool m_isToggle;

		private ExpressionInfo m_initialToggleState;

		private bool m_valueReferenced;

		private bool m_textRunValueReferenced;

		private bool m_recursiveSender;

		private bool m_hasNonRecursiveSender;

		[Reference]
		private TablixMember m_recursiveMember;

		private bool m_dataElementStyleAttribute = true;

		private bool m_hasValue;

		private bool m_hasExpressionBasedValue;

		private bool m_keepTogether;

		private bool m_canScrollVertically;

		[Reference]
		private GroupingList m_containingScopes;

		private EndUserSort m_userSort;

		private bool m_isTablixCellScope;

		private int m_sequenceID = -1;

		private bool m_isSimple;

		[NonSerialized]
		private InitializationContext.ScopeChainInfo m_scopeChainInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TextBox.GetDeclaration();

		private bool m_isSubReportTopLevelScope;

		[NonSerialized]
		private bool m_overrideReportDataElementStyle;

		[NonSerialized]
		private string m_textboxScope;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private AspNetCore.ReportingServices.RdlExpressions.VariantResult m_oldResult;

		[NonSerialized]
		private bool m_hasOldResult;

		[NonSerialized]
		private TextBoxExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private TextBoxImpl m_textBoxImpl;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox;
			}
		}

		internal List<Paragraph> Paragraphs
		{
			get
			{
				return this.m_paragraphs;
			}
			set
			{
				this.m_paragraphs = value;
			}
		}

		internal bool CanScrollVertically
		{
			get
			{
				return this.m_canScrollVertically;
			}
			set
			{
				this.m_canScrollVertically = value;
			}
		}

		internal bool CanGrow
		{
			get
			{
				return this.m_canGrow;
			}
			set
			{
				this.m_canGrow = value;
			}
		}

		internal bool CanShrink
		{
			get
			{
				return this.m_canShrink;
			}
			set
			{
				this.m_canShrink = value;
			}
		}

		internal string HideDuplicates
		{
			get
			{
				return this.m_hideDuplicates;
			}
			set
			{
				this.m_hideDuplicates = value;
			}
		}

		internal bool IsToggle
		{
			get
			{
				return this.m_isToggle;
			}
			set
			{
				this.m_isToggle = value;
			}
		}

		internal ExpressionInfo InitialToggleState
		{
			get
			{
				return this.m_initialToggleState;
			}
			set
			{
				this.m_initialToggleState = value;
			}
		}

		internal bool RecursiveSender
		{
			get
			{
				return this.m_recursiveSender;
			}
			set
			{
				this.m_recursiveSender = value;
			}
		}

		internal bool HasNonRecursiveSender
		{
			get
			{
				return this.m_hasNonRecursiveSender;
			}
			set
			{
				this.m_hasNonRecursiveSender = value;
			}
		}

		internal TablixMember RecursiveMember
		{
			get
			{
				return this.m_recursiveMember;
			}
			set
			{
				this.m_recursiveMember = value;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return this.m_valueReferenced;
			}
			set
			{
				this.m_valueReferenced = value;
			}
		}

		internal TextBoxExprHost TextBoxExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal bool DataElementStyleAttribute
		{
			get
			{
				return this.m_dataElementStyleAttribute;
			}
			set
			{
				this.m_dataElementStyleAttribute = value;
			}
		}

		internal EndUserSort UserSort
		{
			get
			{
				return this.m_userSort;
			}
			set
			{
				this.m_userSort = value;
			}
		}

		internal bool OverrideReportDataElementStyle
		{
			get
			{
				return this.m_overrideReportDataElementStyle;
			}
			set
			{
				this.m_overrideReportDataElementStyle = value;
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

		internal bool HasExpressionBasedValue
		{
			get
			{
				return this.m_hasExpressionBasedValue;
			}
		}

		internal bool HasValue
		{
			get
			{
				return this.m_hasValue;
			}
		}

		internal bool IsSimple
		{
			get
			{
				return this.m_isSimple;
			}
		}

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				if (DataElementOutputTypes.Auto == base.m_dataElementOutput && this.HasExpressionBasedValue)
				{
					return DataElementOutputTypes.Output;
				}
				return DataElementOutputTypes.NoOutput;
			}
		}

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

		internal bool TextRunValueReferenced
		{
			get
			{
				return this.m_textRunValueReferenced;
			}
			set
			{
				this.m_textRunValueReferenced = value;
			}
		}

		internal int SequenceID
		{
			get
			{
				return this.m_sequenceID;
			}
			set
			{
				this.m_sequenceID = value;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IInScopeEventSource.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox;
			}
		}

		string IInScopeEventSource.Name
		{
			get
			{
				return base.m_name;
			}
		}

		ReportItem IInScopeEventSource.Parent
		{
			get
			{
				return base.m_parent;
			}
		}

		EndUserSort IInScopeEventSource.UserSort
		{
			get
			{
				return this.m_userSort;
			}
		}

		GroupingList IInScopeEventSource.ContainingScopes
		{
			get
			{
				return this.m_containingScopes;
			}
			set
			{
				this.m_containingScopes = value;
			}
		}

		internal GroupingList ContainingScopes
		{
			get
			{
				return this.m_containingScopes;
			}
		}

		string IInScopeEventSource.Scope
		{
			get
			{
				return this.m_textboxScope;
			}
			set
			{
				this.m_textboxScope = value;
			}
		}

		bool IInScopeEventSource.IsTablixCellScope
		{
			get
			{
				return this.m_isTablixCellScope;
			}
			set
			{
				this.m_isTablixCellScope = value;
			}
		}

		bool IInScopeEventSource.IsDetailScope
		{
			get
			{
				return this.m_isDetailScope;
			}
			set
			{
				this.m_isDetailScope = value;
			}
		}

		bool IInScopeEventSource.IsSubReportTopLevelScope
		{
			get
			{
				return this.m_isSubReportTopLevelScope;
			}
			set
			{
				this.m_isSubReportTopLevelScope = value;
			}
		}

		InitializationContext.ScopeChainInfo IInScopeEventSource.ScopeChainInfo
		{
			get
			{
				return this.m_scopeChainInfo;
			}
			set
			{
				this.m_scopeChainInfo = value;
			}
		}

		internal TextBox(ReportItem parent)
			: base(parent)
		{
		}

		internal TextBox(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_paragraphs = new List<Paragraph>();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.TextBoxStart(base.m_name);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			bool flag = context.RegisterVisibility(base.m_visibility, this);
			context.RegisterTextBoxInScope(this);
			if (this.m_paragraphs != null)
			{
				foreach (Paragraph paragraph in this.m_paragraphs)
				{
					bool flag2 = default(bool);
					this.m_hasValue |= paragraph.Initialize(context, out flag2);
					this.m_hasExpressionBasedValue |= flag2;
				}
			}
			if (this.m_paragraphs.Count == 1)
			{
				this.m_isSimple = this.m_paragraphs[0].DetermineSimplicity();
			}
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_initialToggleState != null)
			{
				this.m_initialToggleState.Initialize("InitialState", context);
				context.ExprHostBuilder.TextBoxToggleImageInitialState(this.m_initialToggleState);
			}
			if (this.m_userSort != null)
			{
				context.RegisterSortEventSource(this);
			}
			if (this.m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(this.m_hideDuplicates, this);
			}
			context.RegisterToggleItem(this);
			if (flag)
			{
				context.UnRegisterVisibility(base.m_visibility, this);
			}
			base.ExprHostID = context.ExprHostBuilder.TextBoxEnd();
			return true;
		}

		InScopeSortFilterHashtable IInScopeEventSource.GetSortFiltersInScope(bool create, bool inDetail)
		{
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			ReportItem parent = ((IInScopeEventSource)this).Parent;
			if (inDetail)
			{
				while (parent != null && !parent.IsDataRegion)
				{
					parent = parent.Parent;
				}
			}
			else
			{
				while (parent != null && !(parent is Report))
				{
					parent = parent.Parent;
				}
			}
			Global.Tracer.Assert(parent.IsDataRegion || parent is Report, "(parent.IsDataRegion || parent is Report)");
			if (parent is Report)
			{
				Report report = (Report)parent;
				if (((IInScopeEventSource)this).UserSort.SortExpressionScope == null)
				{
					if (report.DetailSortFiltersInScope == null && create)
					{
						report.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					return report.DetailSortFiltersInScope;
				}
				if (report.NonDetailSortFiltersInScope == null && create)
				{
					report.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
				}
				return report.NonDetailSortFiltersInScope;
			}
			Global.Tracer.Assert(null == ((IInScopeEventSource)this).UserSort.SortExpressionScope, "(null == eventSource.UserSort.SortExpressionScope)");
			DataRegion dataRegion = (DataRegion)parent;
			if (dataRegion.DetailSortFiltersInScope == null && create)
			{
				dataRegion.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
			}
			return dataRegion.DetailSortFiltersInScope;
		}

		List<int> IInScopeEventSource.GetPeerSortFilters(bool create)
		{
			EndUserSort userSort = ((IInScopeEventSource)this).UserSort;
			if (userSort == null)
			{
				return null;
			}
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			List<int> list = null;
			if (((IInScopeEventSource)this).ContainingScopes == null || ((IInScopeEventSource)this).ContainingScopes.Count == 0 || ((IInScopeEventSource)this).IsSubReportTopLevelScope)
			{
				inScopeSortFilterHashtable = ((IInScopeEventSource)this).GetSortFiltersInScope(create, false);
			}
			else
			{
				Grouping lastEntry = ((IInScopeEventSource)this).ContainingScopes.LastEntry;
				if (lastEntry == null)
				{
					inScopeSortFilterHashtable = ((IInScopeEventSource)this).GetSortFiltersInScope(create, true);
				}
				else if (userSort.SortExpressionScope == null)
				{
					if (lastEntry.DetailSortFiltersInScope == null && create)
					{
						lastEntry.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.DetailSortFiltersInScope;
				}
				else
				{
					if (lastEntry.NonDetailSortFiltersInScope == null && create)
					{
						lastEntry.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.NonDetailSortFiltersInScope;
				}
			}
			if (inScopeSortFilterHashtable != null)
			{
				int num = (userSort.SortExpressionScope == null) ? userSort.SortTarget.ID : userSort.SortExpressionScope.ID;
				list = inScopeSortFilterHashtable[num];
				if (list == null && create)
				{
					list = new List<int>();
					inScopeSortFilterHashtable.Add(num, list);
				}
			}
			return list;
		}

		internal string GetRecursiveUniqueName(int parentInstanceIndex)
		{
			return InstancePathItem.GenerateUniqueNameString(base.ID, this.InstancePath, parentInstanceIndex);
		}

		internal bool EvaluateIsToggle(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			bool flag = this.IsToggle;
			if (flag && this.RecursiveSender && !this.HasNonRecursiveSender)
			{
				if (this.m_recursiveMember != null)
				{
					context.SetupContext(this, romInstance);
					return this.m_recursiveMember.InstanceHasRecursiveChildren;
				}
				flag = true;
			}
			return flag;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			if (!this.m_overrideReportDataElementStyle)
			{
				this.m_dataElementStyleAttribute = context.ReportDataElementStyleAttribute;
			}
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (this.m_userSort != null)
			{
				context.ProcessSortEventSource(this);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TextBox textBox = (TextBox)base.PublishClone(context);
			textBox.m_sequenceID = context.GenerateTextboxSequenceID();
			if (this.m_paragraphs != null)
			{
				textBox.m_paragraphs = new List<Paragraph>(this.m_paragraphs.Count);
				foreach (Paragraph paragraph2 in this.m_paragraphs)
				{
					Paragraph paragraph = (Paragraph)paragraph2.PublishClone(context);
					paragraph.TextBox = textBox;
					textBox.m_paragraphs.Add(paragraph);
				}
			}
			if (this.m_hideDuplicates != null)
			{
				textBox.m_hideDuplicates = context.GetNewScopeName(this.m_hideDuplicates);
			}
			if (this.m_action != null)
			{
				textBox.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_initialToggleState != null)
			{
				textBox.m_initialToggleState = (ExpressionInfo)this.m_initialToggleState.PublishClone(context);
			}
			if (this.m_userSort != null)
			{
				textBox.m_userSort = (EndUserSort)this.m_userSort.PublishClone(context);
			}
			return textBox;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Paragraphs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph));
			list.Add(new MemberInfo(MemberName.CanGrow, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanShrink, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.IsToggle, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InitialToggleState, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RecursiveSender, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingScopes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.UserSort, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort));
			list.Add(new MemberInfo(MemberName.IsTablixCellScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSubReportTopLevelScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SequenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.RecursiveMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, Token.Reference));
			list.Add(new MemberInfo(MemberName.HasExpressionBasedValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSimple, Token.Boolean));
			list.Add(new MemberInfo(MemberName.TextRunValueReferenced, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasNonRecursiveSender, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanScrollVertically, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TextBox.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Paragraphs:
					writer.Write(this.m_paragraphs);
					break;
				case MemberName.CanScrollVertically:
					writer.Write(this.m_canScrollVertically);
					break;
				case MemberName.CanGrow:
					writer.Write(this.m_canGrow);
					break;
				case MemberName.CanShrink:
					writer.Write(this.m_canShrink);
					break;
				case MemberName.HideDuplicates:
					writer.Write(this.m_hideDuplicates);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.IsToggle:
					writer.Write(this.m_isToggle);
					break;
				case MemberName.InitialToggleState:
					writer.Write(this.m_initialToggleState);
					break;
				case MemberName.ValueReferenced:
					writer.Write(this.m_valueReferenced);
					break;
				case MemberName.TextRunValueReferenced:
					writer.Write(this.m_textRunValueReferenced);
					break;
				case MemberName.RecursiveSender:
					writer.Write(this.m_recursiveSender);
					break;
				case MemberName.DataElementStyleAttribute:
					writer.Write(this.m_dataElementStyleAttribute);
					break;
				case MemberName.ContainingScopes:
					writer.WriteListOfReferences(this.m_containingScopes);
					break;
				case MemberName.UserSort:
					writer.Write(this.m_userSort);
					break;
				case MemberName.IsTablixCellScope:
					writer.Write(this.m_isTablixCellScope);
					break;
				case MemberName.IsSubReportTopLevelScope:
					writer.Write(this.m_isSubReportTopLevelScope);
					break;
				case MemberName.RecursiveMember:
					writer.WriteReference(this.m_recursiveMember);
					break;
				case MemberName.KeepTogether:
					writer.Write(this.m_keepTogether);
					break;
				case MemberName.SequenceID:
					writer.Write(this.m_sequenceID);
					break;
				case MemberName.HasExpressionBasedValue:
					writer.Write(this.m_hasExpressionBasedValue);
					break;
				case MemberName.HasValue:
					writer.Write(this.m_hasValue);
					break;
				case MemberName.IsSimple:
					writer.Write(this.m_isSimple);
					break;
				case MemberName.HasNonRecursiveSender:
					writer.Write(this.m_hasNonRecursiveSender);
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
			reader.RegisterDeclaration(TextBox.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Paragraphs:
					this.m_paragraphs = reader.ReadGenericListOfRIFObjects<Paragraph>();
					break;
				case MemberName.Value:
				{
					TextRun orCreateSingleTextRun2 = this.GetOrCreateSingleTextRun(reader);
					ExpressionInfo expressionInfo = (ExpressionInfo)reader.ReadRIFObject();
					this.m_hasValue = true;
					this.m_hasExpressionBasedValue = expressionInfo.IsExpression;
					orCreateSingleTextRun2.Value = expressionInfo;
					if (base.m_styleClass != null)
					{
						orCreateSingleTextRun2.Paragraph.StyleClass = new ParagraphFilteredStyle(base.m_styleClass);
						orCreateSingleTextRun2.StyleClass = new TextRunFilteredStyle(base.m_styleClass);
						base.m_styleClass = new TextBoxFilteredStyle(base.m_styleClass);
					}
					break;
				}
				case MemberName.CanScrollVertically:
					this.m_canScrollVertically = reader.ReadBoolean();
					break;
				case MemberName.CanGrow:
					this.m_canGrow = reader.ReadBoolean();
					break;
				case MemberName.CanShrink:
					this.m_canShrink = reader.ReadBoolean();
					break;
				case MemberName.HideDuplicates:
					this.m_hideDuplicates = reader.ReadString();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.IsToggle:
					this.m_isToggle = reader.ReadBoolean();
					break;
				case MemberName.InitialToggleState:
					this.m_initialToggleState = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValueReferenced:
					this.m_valueReferenced = reader.ReadBoolean();
					break;
				case MemberName.TextRunValueReferenced:
					this.m_textRunValueReferenced = reader.ReadBoolean();
					break;
				case MemberName.RecursiveSender:
					this.m_recursiveSender = reader.ReadBoolean();
					break;
				case MemberName.DataElementStyleAttribute:
					this.m_dataElementStyleAttribute = reader.ReadBoolean();
					break;
				case MemberName.ContainingScopes:
					if (reader.ReadListOfReferencesNoResolution(this) == 0)
					{
						this.m_containingScopes = new GroupingList();
					}
					break;
				case MemberName.UserSort:
					this.m_userSort = (EndUserSort)reader.ReadRIFObject();
					break;
				case MemberName.IsTablixCellScope:
					this.m_isTablixCellScope = reader.ReadBoolean();
					break;
				case MemberName.IsSubReportTopLevelScope:
					this.m_isSubReportTopLevelScope = reader.ReadBoolean();
					break;
				case MemberName.DataType:
				{
					TextRun orCreateSingleTextRun = this.GetOrCreateSingleTextRun(reader);
					orCreateSingleTextRun.DataType = (DataType)reader.ReadEnum();
					break;
				}
				case MemberName.KeepTogether:
					this.m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.SequenceID:
					this.m_sequenceID = reader.ReadInt32();
					break;
				case MemberName.RecursiveMember:
					this.m_recursiveMember = reader.ReadReference<TablixMember>(this);
					break;
				case MemberName.HasExpressionBasedValue:
					this.m_hasExpressionBasedValue = reader.ReadBoolean();
					break;
				case MemberName.HasValue:
					this.m_hasValue = reader.ReadBoolean();
					break;
				case MemberName.IsSimple:
					this.m_isSimple = reader.ReadBoolean();
					break;
				case MemberName.HasNonRecursiveSender:
					this.m_hasNonRecursiveSender = reader.ReadBoolean();
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
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(TextBox.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ContainingScopes:
						if (this.m_containingScopes == null)
						{
							this.m_containingScopes = new GroupingList();
						}
						if (item.RefID != -2)
						{
							Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
							Global.Tracer.Assert(referenceableItems[item.RefID] is Grouping);
							Global.Tracer.Assert(!this.m_containingScopes.Contains((Grouping)referenceableItems[item.RefID]));
							this.m_containingScopes.Add((Grouping)referenceableItems[item.RefID]);
						}
						else
						{
							this.m_containingScopes.Add(null);
						}
						break;
					case MemberName.RecursiveMember:
					{
						IReferenceable referenceable = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable))
						{
							this.m_recursiveMember = (referenceable as TablixMember);
						}
						break;
					}
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox;
		}

		private TextRun GetOrCreateSingleTextRun(IntermediateFormatReader reader)
		{
			if (this.m_paragraphs == null)
			{
				this.m_isSimple = true;
				this.m_paragraphs = new List<Paragraph>(1);
				Paragraph paragraph = new Paragraph(this, 0, -1);
				paragraph.GlobalID = -1;
				this.m_paragraphs.Add(paragraph);
				List<TextRun> list = new List<TextRun>(1);
				TextRun textRun = new TextRun(paragraph, 0, -1);
				textRun.GlobalID = -1;
				list.Add(textRun);
				paragraph.TextRuns = list;
				return textRun;
			}
			return this.m_paragraphs[0].TextRuns[0];
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.TextBoxHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
				}
				if (this.m_paragraphs != null)
				{
					foreach (Paragraph paragraph in this.m_paragraphs)
					{
						paragraph.SetExprHost(this.m_exprHost, reportObjectModel);
					}
				}
			}
		}

		internal bool EvaluateInitialToggleState(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateTextBoxInitialToggleStateExpression(this);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return this.GetTextBoxImpl(context).GetResult(romInstance, false);
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return this.GetTextBoxImpl(context).GetFieldsUsedInValueExpression(romInstance);
		}

		internal TextBoxImpl GetTextBoxImpl(OnDemandProcessingContext context)
		{
			if (this.m_textBoxImpl == null)
			{
				AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRuntime = context.ReportRuntime;
				ReportItemsImpl reportItemsImpl = context.ReportObjectModel.ReportItemsImpl;
				this.m_textBoxImpl = (TextBoxImpl)reportItemsImpl.GetReportItem(base.m_name);
				Global.Tracer.Assert(this.m_textBoxImpl != null, "(m_textBoxImpl != null)");
			}
			return this.m_textBoxImpl;
		}

		internal void ResetTextBoxImpl(OnDemandProcessingContext context)
		{
			this.GetTextBoxImpl(context).Reset();
		}

		internal void ResetDuplicates()
		{
			this.m_hasOldResult = false;
		}

		internal bool CalculateDuplicates(AspNetCore.ReportingServices.RdlExpressions.VariantResult currentResult, OnDemandProcessingContext context)
		{
			bool flag = false;
			if (this.m_hideDuplicates != null)
			{
				if (this.m_hasOldResult)
				{
					if (currentResult.ErrorOccurred && this.m_oldResult.ErrorOccurred)
					{
						flag = true;
					}
					else if (currentResult.ErrorOccurred)
					{
						flag = false;
					}
					else if (this.m_oldResult.ErrorOccurred)
					{
						flag = false;
					}
					else if (currentResult.Value == null && this.m_oldResult.Value == null)
					{
						flag = true;
					}
					else if (currentResult.Value == null)
					{
						flag = false;
					}
					else
					{
						bool flag2 = default(bool);
						flag = (0 == AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareTo(currentResult.Value, this.m_oldResult.Value, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.DataSetDef.NullsAsBlanks : context.NullsAsBlanks, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.CompareInfo : context.CompareInfo, (context.CurrentOdpDataSetInstance != null) ? context.CurrentOdpDataSetInstance.ClrCompareOptions : context.ClrCompareOptions, false, false, out flag2));
						if (!flag2)
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					this.m_hasOldResult = true;
					this.m_oldResult = currentResult;
				}
			}
			return flag;
		}
	}
}
