namespace AspNetCore.Reporting.Chart.WebForms.Svg
{
	internal struct SvgOpenParameters
	{
		private bool m_toolTipsEnabled;

		private bool m_resizable;

		private bool m_preserveAspectRatio;

		public bool ToolTipsEnabled
		{
			get
			{
				return this.m_toolTipsEnabled;
			}
			set
			{
				this.m_toolTipsEnabled = value;
			}
		}

		public bool Resizable
		{
			get
			{
				return this.m_resizable;
			}
			set
			{
				this.m_resizable = value;
			}
		}

		public bool PreserveAspectRatio
		{
			get
			{
				return this.m_preserveAspectRatio;
			}
			set
			{
				this.m_preserveAspectRatio = value;
			}
		}

		public SvgOpenParameters(bool toolTipsEnabled, bool resizable, bool preserveAspectRatio)
		{
			this.m_toolTipsEnabled = toolTipsEnabled;
			this.m_resizable = resizable;
			this.m_preserveAspectRatio = preserveAspectRatio;
		}

		public override int GetHashCode()
		{
			return this.m_preserveAspectRatio.GetHashCode() ^ this.m_resizable.GetHashCode() ^ this.m_toolTipsEnabled.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SvgOpenParameters))
			{
				return false;
			}
			return this.Equals((SvgOpenParameters)obj);
		}

		public bool Equals(SvgOpenParameters other)
		{
			if (this.m_preserveAspectRatio != other.m_preserveAspectRatio)
			{
				return false;
			}
			if (this.m_resizable != other.m_resizable)
			{
				return false;
			}
			if (this.m_toolTipsEnabled != other.m_toolTipsEnabled)
			{
				return false;
			}
			return true;
		}

		public static bool operator ==(SvgOpenParameters value1, SvgOpenParameters value2)
		{
			return value2.Equals(value2);
		}

		public static bool operator !=(SvgOpenParameters value1, SvgOpenParameters value2)
		{
			return !value2.Equals(value2);
		}
	}
}
