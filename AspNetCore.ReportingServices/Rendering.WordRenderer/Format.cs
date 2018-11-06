namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal abstract class Format
	{
		protected SprmBuffer m_grpprl;

		internal Format(int initialSize, int initialOffset)
		{
			this.m_grpprl = new SprmBuffer(initialSize, initialOffset);
		}

		internal void AddSprm(ushort sprmCode, int param, byte[] varParam)
		{
			this.m_grpprl.AddSprm(sprmCode, param, varParam);
		}
	}
}
