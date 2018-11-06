using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal abstract class Group
	{
		protected ReportItem m_ownerItem;

		internal Grouping m_groupingDef;

		internal Visibility m_visibilityDef;

		protected int m_uniqueName;

		protected CustomPropertyCollection m_customProperties;

		public string Name
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return null;
				}
				return this.m_groupingDef.Name;
			}
		}

		public abstract string ID
		{
			get;
		}

		public string UniqueName
		{
			get
			{
				if (this.m_uniqueName == 0)
				{
					return null;
				}
				return this.m_uniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public abstract string Label
		{
			get;
		}

		public virtual bool PageBreakAtEnd
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return false;
				}
				return this.m_groupingDef.PageBreakAtEnd;
			}
		}

		public virtual bool PageBreakAtStart
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return false;
				}
				return this.m_groupingDef.PageBreakAtStart;
			}
		}

		public string Custom
		{
			get
			{
				if (this.m_groupingDef != null)
				{
					string text = this.m_groupingDef.Custom;
					if (text == null && this.CustomProperties != null)
					{
						CustomProperty customProperty = this.CustomProperties["Custom"];
						if (customProperty != null && customProperty.Value != null)
						{
							text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
						}
					}
					return text;
				}
				return null;
			}
		}

		public abstract CustomPropertyCollection CustomProperties
		{
			get;
		}

		public abstract bool Hidden
		{
			get;
		}

		public virtual bool HasToggle
		{
			get
			{
				return Visibility.HasToggle(this.m_visibilityDef);
			}
		}

		public virtual string ToggleItem
		{
			get
			{
				if (this.m_visibilityDef == null)
				{
					return null;
				}
				return this.m_visibilityDef.Toggle;
			}
		}

		internal virtual TextBox ToggleParent
		{
			get
			{
				if (!this.HasToggle)
				{
					return null;
				}
				Global.Tracer.Assert(null != this.OwnerDataRegion);
				return this.OwnerDataRegion.RenderingContext.GetToggleParent(this.m_uniqueName);
			}
		}

		public virtual SharedHiddenState SharedHidden
		{
			get
			{
				return Visibility.GetSharedHidden(this.m_visibilityDef);
			}
		}

		public virtual bool IsToggleChild
		{
			get
			{
				Global.Tracer.Assert(null != this.OwnerDataRegion);
				return this.OwnerDataRegion.RenderingContext.IsToggleChild(this.m_uniqueName);
			}
		}

		public virtual string DataElementName
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return null;
				}
				return this.m_groupingDef.DataElementName;
			}
		}

		public virtual string DataCollectionName
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return null;
				}
				return this.m_groupingDef.DataCollectionName;
			}
		}

		public virtual DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.m_groupingDef == null)
				{
					return DataElementOutputTypes.Output;
				}
				return this.m_groupingDef.DataElementOutput;
			}
		}

		internal DataRegion OwnerDataRegion
		{
			get
			{
				return this.m_ownerItem as DataRegion;
			}
		}

		internal Group(CustomReportItem owner, Grouping groupingDef)
		{
			this.m_ownerItem = owner;
			this.m_groupingDef = groupingDef;
		}

		internal Group(DataRegion owner, Grouping groupingDef, Visibility visibilityDef)
		{
			this.m_ownerItem = owner;
			this.m_groupingDef = groupingDef;
			this.m_visibilityDef = visibilityDef;
		}
	}
}
