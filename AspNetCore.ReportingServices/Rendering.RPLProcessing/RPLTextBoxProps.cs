using System;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBoxProps : RPLItemProps
	{
		private bool m_toggleState;

		private RPLFormat.SortOptions m_sortState;

		private bool m_isToggleParent;

		private TypeCode m_typeCode;

		private object m_originalValue;

		private string m_value;

		private RPLActionInfo m_actionInfo;

		private float m_contentHeight;

		private float m_contentOffset;

		private bool m_processedWithError;

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

		public object OriginalValue
		{
			get
			{
				return this.m_originalValue;
			}
			set
			{
				this.m_originalValue = value;
			}
		}

		public TypeCode TypeCode
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

		public bool ToggleState
		{
			get
			{
				return this.m_toggleState;
			}
			set
			{
				this.m_toggleState = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return this.m_actionInfo;
			}
			set
			{
				this.m_actionInfo = value;
			}
		}

		public RPLFormat.SortOptions SortState
		{
			get
			{
				return this.m_sortState;
			}
			set
			{
				this.m_sortState = value;
			}
		}

		public float ContentHeight
		{
			get
			{
				return this.m_contentHeight;
			}
			set
			{
				this.m_contentHeight = value;
			}
		}

		public float ContentOffset
		{
			get
			{
				return this.m_contentOffset;
			}
			set
			{
				this.m_contentOffset = value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				return this.m_processedWithError;
			}
			set
			{
				this.m_processedWithError = value;
			}
		}

		internal RPLTextBoxProps()
		{
		}

		public object Clone()
		{
			return (RPLTextBoxProps)base.MemberwiseClone();
		}
	}
}
