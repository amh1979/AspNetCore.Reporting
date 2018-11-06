using System;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBoxPropsDef : RPLItemPropsDef
	{
		private bool m_isSimple = true;

		private string m_formula;

		private bool m_isToggleParent;

		private bool m_canGrow;

		private bool m_canShrink;

		private bool m_canSort;

		private TypeCode m_typeCode = TypeCode.String;

		private bool m_formattedValueExpressionBased;

		private string m_value;

		public bool CanSort
		{
			get
			{
				return this.m_canSort;
			}
			set
			{
				this.m_canSort = value;
			}
		}

		public bool CanShrink
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

		public bool CanGrow
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

		public string Formula
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

		public bool IsToggleParent
		{
			get
			{
				return this.m_isToggleParent;
			}
			set
			{
				this.m_isToggleParent = value;
			}
		}

		public string Value
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

		public TypeCode SharedTypeCode
		{
			get
			{
				return this.m_typeCode;
			}
			set
			{
				this.m_typeCode = value;
			}
		}

		public bool FormattedValueExpressionBased
		{
			get
			{
				return this.m_formattedValueExpressionBased;
			}
			set
			{
				this.m_formattedValueExpressionBased = value;
			}
		}

		public bool IsSimple
		{
			get
			{
				return this.m_isSimple;
			}
			set
			{
				this.m_isSimple = value;
			}
		}

		internal RPLTextBoxPropsDef()
		{
		}
	}
}
