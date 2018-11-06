using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TextBox : ReportItem
	{
		private SimpleTextBoxInstanceInfo m_simpleInstanceInfo;

		private string m_value;

		private ActionInfo m_actionInfo;

		private object m_originalValue;

		internal SimpleTextBoxInstanceInfo SimpleInstanceInfo
		{
			get
			{
				if (base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (base.ReportItemInstance == null)
				{
					return null;
				}
				if (this.m_simpleInstanceInfo == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.TextBoxInstance textBoxInstance = (AspNetCore.ReportingServices.ReportProcessing.TextBoxInstance)base.ReportItemInstance;
					this.m_simpleInstanceInfo = textBoxInstance.GetSimpleInstanceInfo(base.RenderingContext.ChunkManager, base.RenderingContext.InPageSection);
				}
				return this.m_simpleInstanceInfo;
			}
		}

		public Report.DataElementStyles DataElementStyle
		{
			get
			{
				if (!((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).DataElementStyleAttribute)
				{
					return Report.DataElementStyles.ElementNormal;
				}
				return Report.DataElementStyles.AttributeNormal;
			}
		}

		public bool CanGrow
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).CanGrow;
			}
		}

		public bool CanShrink
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).CanShrink;
			}
		}

		public string Value
		{
			get
			{
				RenderingContext renderingContext = base.RenderingContext;
				string text = this.m_value;
				if (this.m_value == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.TextBox textBox = (AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef;
					if (textBox.Value.Type == ExpressionInfo.Types.Constant)
					{
						text = textBox.Value.Value;
					}
					else if (base.ReportItemInstance == null)
					{
						text = null;
					}
					else if (textBox.IsSimpleTextBox(base.RenderingContext.IntermediateFormatVersion))
					{
						text = this.SimpleInstanceInfo.FormattedValue;
						if (text == null)
						{
							text = (this.SimpleInstanceInfo.OriginalValue as string);
						}
					}
					else
					{
						TextBoxInstanceInfo textBoxInstanceInfo = (TextBoxInstanceInfo)base.InstanceInfo;
						text = textBoxInstanceInfo.FormattedValue;
						if (text == null)
						{
							text = (textBoxInstanceInfo.OriginalValue as string);
						}
					}
					if (base.RenderingContext.CacheState)
					{
						this.m_value = text;
					}
				}
				return text;
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].HyperLinkURL;
				}
				return null;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughReport;
				}
				return null;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughParameters;
				}
				return null;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].BookmarkLink;
				}
				return null;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.Action action = ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).Action;
					if (action != null)
					{
						AspNetCore.ReportingServices.ReportProcessing.ActionInstance actionInstance = null;
						string ownerUniqueName = base.UniqueName;
						if (base.ReportItemInstance != null)
						{
							actionInstance = ((TextBoxInstanceInfo)base.InstanceInfo).Action;
							if (base.RenderingContext.InPageSection)
							{
								ownerUniqueName = base.ReportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
						}
						else if (base.RenderingContext.InPageSection && base.m_intUniqueName != 0)
						{
							ownerUniqueName = base.m_intUniqueName.ToString(CultureInfo.InvariantCulture);
						}
						actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, base.RenderingContext);
						if (base.RenderingContext.CacheState)
						{
							this.m_actionInfo = actionInfo;
						}
					}
				}
				return actionInfo;
			}
		}

		public bool Duplicate
		{
			get
			{
				if (!this.HideDuplicates)
				{
					return false;
				}
				if (base.ReportItemInstance != null)
				{
					return ((TextBoxInstanceInfo)base.InstanceInfo).Duplicate;
				}
				return false;
			}
		}

		public bool HideDuplicates
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).HideDuplicates != null;
			}
		}

		public string Formula
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).Formula;
			}
		}

		public object OriginalValue
		{
			get
			{
				object obj = this.m_originalValue;
				if (this.m_originalValue == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.TextBox textBox = (AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef;
					obj = ((textBox.Value.Type != ExpressionInfo.Types.Constant) ? ((base.ReportItemInstance != null) ? ((!textBox.IsSimpleTextBox(base.RenderingContext.IntermediateFormatVersion)) ? ((TextBoxInstanceInfo)base.InstanceInfo).OriginalValue : this.SimpleInstanceInfo.OriginalValue) : null) : textBox.Value.Value);
					if (base.RenderingContext.CacheState)
					{
						this.m_originalValue = obj;
					}
				}
				return obj;
			}
		}

		public TypeCode SharedTypeCode
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).ValueType;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(base.ReportItemDef.Visibility);
				}
				if (base.ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (base.ReportItemDef.Visibility.Toggle != null)
				{
					return base.RenderingContext.IsItemHidden(base.ReportItemInstance.UniqueName, true);
				}
				return base.InstanceInfo.StartHidden;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return false;
				}
				if (this.IsSharedToggleParent)
				{
					return base.RenderingContext.IsToggleParent(base.ReportItemInstance.UniqueName);
				}
				return false;
			}
		}

		public bool IsSharedToggleParent
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).IsToggle;
			}
		}

		public bool ToggleState
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return false;
				}
				if (this.IsSharedToggleParent)
				{
					if (base.RenderingContext.IsToggleStateNegated(base.ReportItemInstance.UniqueName))
					{
						return !((TextBoxInstanceInfo)base.InstanceInfo).InitialToggleState;
					}
					return ((TextBoxInstanceInfo)base.InstanceInfo).InitialToggleState;
				}
				return false;
			}
		}

		public bool CanSort
		{
			get
			{
				return null != ((AspNetCore.ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).UserSort;
			}
		}

		public SortOptions SortState
		{
			get
			{
				if (base.IsCustomControl)
				{
					return SortOptions.None;
				}
				return base.RenderingContext.GetSortState(base.m_intUniqueName);
			}
		}

		internal TextBox(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.TextBox reportItemDef, AspNetCore.ReportingServices.ReportProcessing.TextBoxInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			return this.SearchTextBox(searchContext.FindValue);
		}

		private bool SearchTextBox(string findValue)
		{
			string value = this.Value;
			if (value != null)
			{
				int num = value.IndexOf(findValue, 0, StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
