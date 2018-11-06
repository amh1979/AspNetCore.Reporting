using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBox : ReportItem, IActionOwner
	{
		private ExpressionInfo m_value;

		private string m_formula;

		private bool m_canGrow;

		private bool m_canShrink;

		private string m_hideDuplicates;

		private Action m_action;

		private bool m_isToggle;

		private ExpressionInfo m_initialToggleState;

		private bool m_valueReferenced;

		private bool m_recursiveSender;

		private bool m_dataElementStyleAttribute = true;

		[Reference]
		private GroupingList m_containingScopes;

		private EndUserSort m_userSort;

		private bool m_isMatrixCellScope;

		private TypeCode m_valueType = TypeCode.String;

		private bool m_isSubReportTopLevelScope;

		[NonSerialized]
		private bool m_overrideReportDataElementStyle;

		[NonSerialized]
		private string m_textboxScope;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private bool m_valueTypeSet;

		[NonSerialized]
		private VariantResult m_oldResult;

		[NonSerialized]
		private bool m_hasOldResult;

		[NonSerialized]
		private string m_formattedValue;

		[NonSerialized]
		private TextBoxExprHost m_exprHost;

		[NonSerialized]
		private bool m_sharedFormatSettings;

		[NonSerialized]
		private bool m_calendarValidated;

		[NonSerialized]
		private Calendar m_calendar;

		[NonSerialized]
		private uint m_languageInstanceId;

		[NonSerialized]
		private int m_tableColumnPosition = -1;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Textbox;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
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

		internal TypeCode ValueType
		{
			get
			{
				return this.m_valueType;
			}
			set
			{
				this.m_valueType = value;
			}
		}

		internal VariantResult OldResult
		{
			get
			{
				return this.m_oldResult;
			}
			set
			{
				this.m_oldResult = value;
				this.m_hasOldResult = true;
			}
		}

		internal bool IsSubReportTopLevelScope
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

		internal bool HasOldResult
		{
			get
			{
				return this.m_hasOldResult;
			}
			set
			{
				this.m_hasOldResult = value;
			}
		}

		internal bool SharedFormatSettings
		{
			get
			{
				return this.m_sharedFormatSettings;
			}
			set
			{
				this.m_sharedFormatSettings = value;
			}
		}

		internal string FormattedValue
		{
			get
			{
				return this.m_formattedValue;
			}
			set
			{
				this.m_formattedValue = value;
			}
		}

		internal string Formula
		{
			get
			{
				return this.m_formula;
			}
			set
			{
				this.m_formula = value;
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

		internal bool CalendarValidated
		{
			get
			{
				return this.m_calendarValidated;
			}
			set
			{
				this.m_calendarValidated = value;
			}
		}

		internal Calendar Calendar
		{
			get
			{
				return this.m_calendar;
			}
			set
			{
				this.m_calendar = value;
			}
		}

		internal uint LanguageInstanceId
		{
			get
			{
				return this.m_languageInstanceId;
			}
			set
			{
				this.m_languageInstanceId = value;
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

		internal GroupingList ContainingScopes
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

		internal bool IsMatrixCellScope
		{
			get
			{
				return this.m_isMatrixCellScope;
			}
			set
			{
				this.m_isMatrixCellScope = value;
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

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				if (DataElementOutputTypesRDL.Auto == base.m_dataElementOutputRDL && this.m_value != null && ExpressionInfo.Types.Constant == this.m_value.Type)
				{
					return DataElementOutputTypes.NoOutput;
				}
				return DataElementOutputTypes.Output;
			}
		}

		internal string TextBoxScope
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

		internal bool IsDetailScope
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

		internal int TableColumnPosition
		{
			get
			{
				return this.m_tableColumnPosition;
			}
			set
			{
				this.m_tableColumnPosition = value;
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

		internal TextBox(ReportItem parent)
			: base(parent)
		{
		}

		internal TextBox(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.TextBoxStart(base.m_name);
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, false, false);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(this.m_value);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_initialToggleState != null)
			{
				this.m_initialToggleState.Initialize("InitialState", context);
				context.ExprHostBuilder.TextBoxToggleImageInitialState(this.m_initialToggleState);
			}
			if (this.m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(this.m_hideDuplicates, this);
			}
			context.RegisterSender(this);
			if (this.m_userSort != null)
			{
				context.RegisterSortFilterTextbox(this);
				this.m_textboxScope = context.GetCurrentScope();
				if ((LocationFlags)0 < (context.Location & LocationFlags.InMatrixCellTopLevelItem))
				{
					this.m_isMatrixCellScope = true;
				}
				if ((LocationFlags)0 < (context.Location & LocationFlags.InDetail))
				{
					this.m_isDetailScope = true;
					context.SetDataSetDetailUserSortFilter();
				}
				string sortExpressionScopeString = this.m_userSort.SortExpressionScopeString;
				if (sortExpressionScopeString == null)
				{
					context.TextboxWithDetailSortExpressionAdd(this);
				}
				else if (context.IsScope(sortExpressionScopeString))
				{
					if (context.IsCurrentScope(sortExpressionScopeString) && !this.m_isMatrixCellScope)
					{
						this.m_userSort.SortExpressionScope = context.GetSortFilterScope(sortExpressionScopeString);
						this.InitializeSortExpression(context, false);
					}
					else if (context.IsAncestorScope(sortExpressionScopeString, (LocationFlags)0 < (context.Location & LocationFlags.InMatrixGroupHeader), this.m_isMatrixCellScope))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressionScope", sortExpressionScopeString);
					}
					else
					{
						context.RegisterUserSortInnerScope(this);
					}
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressionScope", sortExpressionScopeString);
				}
				string sortTargetString = this.m_userSort.SortTargetString;
				if (sortTargetString != null)
				{
					if (context.IsScope(sortTargetString))
					{
						if (!context.IsCurrentScope(sortTargetString) && !context.IsAncestorScope(sortTargetString, (LocationFlags)0 < (context.Location & LocationFlags.InMatrixGroupHeader), false) && !context.IsPeerScope(sortTargetString))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget", sortTargetString);
						}
					}
					else
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget", sortTargetString);
					}
				}
				else if (context.IsReportTopLevelScope())
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidOmittedTargetScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget");
				}
			}
			base.ExprHostID = context.ExprHostBuilder.TextBoxEnd();
			return true;
		}

		internal void InitializeSortExpression(InitializationContext context, bool needsExplicitAggregateScope)
		{
			if (this.m_userSort != null && this.m_userSort.SortExpression != null)
			{
				bool flag = true;
				if (needsExplicitAggregateScope && this.m_userSort.SortExpression.Aggregates != null)
				{
					int count = this.m_userSort.SortExpression.Aggregates.Count;
					for (int i = 0; i < count; i++)
					{
						string text = default(string);
						if (!this.m_userSort.SortExpression.Aggregates[i].GetScope(out text))
						{
							flag = false;
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidOmittedExpressionScope, Severity.Error, ObjectType.Textbox, base.m_name, "SortExpression", "SortExpressionScope");
						}
					}
				}
				if (flag)
				{
					this.m_userSort.SortExpression.Initialize("SortExpression", context);
				}
			}
		}

		internal void AddToScopeSortFilterList()
		{
			IntList peerSortFilters = this.GetPeerSortFilters(true);
			Global.Tracer.Assert(null != peerSortFilters);
			peerSortFilters.Add(base.m_ID);
		}

		internal IntList GetPeerSortFilters(bool create)
		{
			if (this.m_userSort == null)
			{
				return null;
			}
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			IntList intList = null;
			if (this.m_containingScopes == null || this.m_containingScopes.Count == 0 || this.m_isSubReportTopLevelScope)
			{
				inScopeSortFilterHashtable = this.GetSortFiltersInScope(create, false);
			}
			else
			{
				Grouping lastEntry = this.m_containingScopes.LastEntry;
				if (lastEntry == null)
				{
					inScopeSortFilterHashtable = this.GetSortFiltersInScope(create, true);
				}
				else if (this.m_userSort.SortExpressionScope == null)
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
				int num = (this.m_userSort.SortExpressionScope == null) ? this.m_userSort.SortTarget.ID : this.m_userSort.SortExpressionScope.ID;
				intList = inScopeSortFilterHashtable[num];
				if (intList == null && create)
				{
					intList = new IntList();
					inScopeSortFilterHashtable.Add(num, intList);
				}
			}
			return intList;
		}

		private InScopeSortFilterHashtable GetSortFiltersInScope(bool create, bool inDetail)
		{
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			ReportItem parent = base.m_parent;
			if (inDetail)
			{
				while (parent != null && !(parent is DataRegion))
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
			Global.Tracer.Assert(parent is DataRegion || parent is Report);
			if (parent is Report)
			{
				Report report = (Report)parent;
				if (this.m_userSort.SortExpressionScope == null)
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
			Global.Tracer.Assert(null == this.m_userSort.SortExpressionScope);
			DataRegion dataRegion = (DataRegion)parent;
			if (dataRegion.DetailSortFiltersInScope == null && create)
			{
				dataRegion.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
			}
			return dataRegion.DetailSortFiltersInScope;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			if (!this.m_overrideReportDataElementStyle)
			{
				this.m_dataElementStyleAttribute = context.ReportDataElementStyleAttribute;
			}
		}

		internal void SetValueType(object textBoxValue)
		{
			if (textBoxValue != null && DBNull.Value != textBoxValue && (!this.m_valueTypeSet || TypeCode.Object != this.m_valueType))
			{
				Type type = textBoxValue.GetType();
				TypeCode typeCode = Type.GetTypeCode(type);
				if (this.m_valueTypeSet)
				{
					if (this.m_valueType != typeCode)
					{
						this.m_valueType = TypeCode.Object;
					}
				}
				else
				{
					this.m_valueType = typeCode;
				}
				this.m_valueTypeSet = true;
			}
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.TextBoxHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_action != null)
				{
					if (this.m_exprHost.ActionInfoHost != null)
					{
						this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
					}
					else if (this.m_exprHost.ActionHost != null)
					{
						this.m_action.SetExprHost(this.m_exprHost.ActionHost, reportObjectModel);
					}
				}
			}
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (this.m_action != null && nonCompNames != null)
			{
				this.m_action.ProcessDrillthroughAction(processingContext, nonCompNames.UniqueName);
			}
		}

		internal bool IsSimpleTextBox()
		{
			if (base.m_styleClass != null && base.m_styleClass.ExpressionList != null && 0 < base.m_styleClass.ExpressionList.Count)
			{
				return false;
			}
			if (this.m_initialToggleState == null && !this.m_isToggle && base.m_visibility == null)
			{
				if (base.m_label == null && base.m_bookmark == null && this.m_action == null)
				{
					if (base.m_toolTip != null && ExpressionInfo.Types.Constant != base.m_toolTip.Type)
					{
						return false;
					}
					if (base.m_customProperties != null)
					{
						return false;
					}
					if (this.m_hideDuplicates == null && this.m_userSort == null)
					{
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		internal bool IsSimpleTextBox(IntermediateFormatVersion intermediateFormatVersion)
		{
			Global.Tracer.Assert(null != intermediateFormatVersion);
			if (intermediateFormatVersion.IsRS2005_WithSimpleTextBoxOptimizations)
			{
				return this.IsSimpleTextBox();
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.CanGrow, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CanShrink, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.IsToggle, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InitialToggleState, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ValueType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Formula, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RecursiveSender, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ContainingScopes, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.GroupingList));
			memberInfoList.Add(new MemberInfo(MemberName.UserSort, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.EndUserSort));
			memberInfoList.Add(new MemberInfo(MemberName.IsMatrixCellScope, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IsSubReportTopLevelScope, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
