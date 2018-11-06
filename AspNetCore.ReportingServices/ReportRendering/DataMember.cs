using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataMember : Group
	{
		private CustomReportItemHeading m_headingDef;

		private CustomReportItemHeadingInstance m_headingInstance;

		private DataGroupingCollection m_children;

		private DataMember m_parent;

		private bool m_isSubtotal;

		private int m_index;

		public override string ID
		{
			get
			{
				return this.m_headingDef.ID.ToString(CultureInfo.InvariantCulture);
			}
		}

		internal override TextBox ToggleParent
		{
			get
			{
				return null;
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				return false;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				return SharedHiddenState.Never;
			}
		}

		public override bool Hidden
		{
			get
			{
				return false;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = base.m_customProperties;
				if (base.m_customProperties == null)
				{
					if (this.m_headingDef.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((this.m_headingInstance != null) ? new CustomPropertyCollection(this.m_headingDef.CustomProperties, this.m_headingInstance.CustomPropertyInstances) : new CustomPropertyCollection(this.m_headingDef.CustomProperties, null));
					if (base.m_ownerItem.UseCache)
					{
						base.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ValueCollection GroupValues
		{
			get
			{
				if (base.m_groupingDef != null && base.m_groupingDef.GroupExpressions != null && base.m_groupingDef.GroupExpressions.Count != 0)
				{
					int count = base.m_groupingDef.GroupExpressions.Count;
					ArrayList arrayList = new ArrayList(count);
					for (int i = 0; i < count; i++)
					{
						object obj = null;
						obj = ((base.m_groupingDef.GroupExpressions[i].Type != ExpressionInfo.Types.Constant) ? ((this.m_headingInstance != null && this.m_headingInstance.GroupExpressionValues != null) ? ((ArrayList)this.m_headingInstance.GroupExpressionValues)[i] : null) : base.m_groupingDef.GroupExpressions[i].Value);
						arrayList.Add(obj);
					}
					return new ValueCollection(arrayList);
				}
				return null;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (base.m_groupingDef != null && base.m_groupingDef.GroupLabel != null)
				{
					result = ((base.m_groupingDef.GroupLabel.Type != ExpressionInfo.Types.Constant) ? ((this.m_headingInstance != null) ? this.m_headingInstance.Label : null) : base.m_groupingDef.GroupLabel.Value);
				}
				return result;
			}
		}

		public DataMember Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		public DataGroupingCollection Children
		{
			get
			{
				CustomReportItemHeadingList innerHeadings = this.m_headingDef.InnerHeadings;
				if (innerHeadings == null)
				{
					return null;
				}
				DataGroupingCollection dataGroupingCollection = this.m_children;
				if (this.m_children == null)
				{
					CustomReportItemHeadingInstanceList headingInstances = null;
					if (this.m_headingInstance == null)
					{
						return null;
					}
					if (this.m_headingInstance != null)
					{
						headingInstances = this.m_headingInstance.SubHeadingInstances;
					}
					dataGroupingCollection = new DataGroupingCollection((CustomReportItem)base.m_ownerItem, this, innerHeadings, headingInstances);
					if (base.m_ownerItem.UseCache)
					{
						this.m_children = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		public bool IsTotal
		{
			get
			{
				Global.Tracer.Assert((this.m_isSubtotal && !this.m_headingDef.Subtotal) || !this.m_isSubtotal);
				return this.m_isSubtotal;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return -1;
				}
				return this.m_headingInstance.HeadingCellIndex;
			}
		}

		public int MemberHeadingSpan
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return -1;
				}
				return this.m_headingInstance.HeadingSpan;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.m_headingDef.Grouping == null)
				{
					if (this.m_headingInstance != null && this.m_headingInstance.Label != null)
					{
						return this.m_headingInstance.Label;
					}
					if (!this.m_headingDef.IsColumn)
					{
						return "Row" + this.m_index.ToString(CultureInfo.InvariantCulture);
					}
					return "Column" + this.m_index.ToString(CultureInfo.InvariantCulture);
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.m_headingDef.Grouping == null)
				{
					return DataElementOutputTypes.Output;
				}
				return base.DataElementOutput;
			}
		}

		public bool IsStatic
		{
			get
			{
				return this.m_headingDef.Static;
			}
		}

		internal DataMember(CustomReportItem owner, DataMember parent, CustomReportItemHeading headingDef, CustomReportItemHeadingInstance headingInstance, bool isSubtotal, int index)
			: base(owner, headingDef.Grouping)
		{
			Global.Tracer.Assert(null != headingDef);
			this.m_parent = parent;
			this.m_headingDef = headingDef;
			this.m_headingInstance = headingInstance;
			this.m_index = index;
			this.m_isSubtotal = isSubtotal;
			base.m_uniqueName = -1;
		}
	}
}
