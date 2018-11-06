using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class Filter : ReportObject
	{
		internal class Definition
		{
			internal enum Properties
			{
				FilterExpression,
				Operator,
				FilterValues
			}
		}

		public ReportExpression FilterExpression
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public Operators Operator
		{
			get
			{
				return (Operators)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[XmlArrayItem("FilterValue", typeof(ReportExpression))]
		[XmlElement(typeof(RdlCollection<ReportExpression>))]
		public IList<ReportExpression> FilterValues
		{
			get
			{
				return (IList<ReportExpression>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Filter()
		{
		}

		internal Filter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.FilterValues = new RdlCollection<ReportExpression>();
		}

		public bool Equals(Filter filter)
		{
			if (filter == null)
			{
				return false;
			}
			if (this.FilterExpression == filter.FilterExpression && this.FilterValues == filter.FilterValues && this.Operator == filter.Operator)
			{
				return base.Equals(filter);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Filter);
		}

		public override int GetHashCode()
		{
			return this.FilterExpression.GetHashCode();
		}
	}
}
