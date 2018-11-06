namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptVisAttr
	{
		private ushort m_value;

		internal int uJustification;

		internal int fClusterStart;

		internal int fDiacritic;

		internal int fZeroWidth;

		internal ScriptVisAttr(ushort value)
		{
			this.m_value = value;
			this.uJustification = (this.m_value & 0xF);
			this.fClusterStart = (this.m_value >> 4 & 1);
			this.fDiacritic = (this.m_value >> 5 & 1);
			this.fZeroWidth = (this.m_value >> 6 & 1);
		}
	}
}
