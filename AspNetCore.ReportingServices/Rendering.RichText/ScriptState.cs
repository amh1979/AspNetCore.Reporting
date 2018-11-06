namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptState
	{
		private ushort m_value;

		internal int uBidiLevel;

		internal int fOverrideDirection;

		internal int fInhibitSymSwap;

		internal int fCharShape;

		internal int fDigitSubstitute;

		internal int fInhibitLigate;

		internal int fDisplayZWG;

		internal int fArabicNumContext;

		internal int fGcpClusters;

		internal int fReserved;

		internal int fEngineReserved;

		internal ScriptState()
		{
		}

		internal ScriptState(ushort value)
		{
			this.m_value = value;
			this.uBidiLevel = (this.m_value & 0x1F);
			this.fOverrideDirection = (this.m_value >> 5 & 1);
			this.fInhibitSymSwap = (this.m_value >> 6 & 1);
			this.fCharShape = (this.m_value >> 7 & 1);
			this.fDigitSubstitute = (this.m_value >> 8 & 1);
			this.fInhibitLigate = (this.m_value >> 9 & 1);
			this.fDisplayZWG = (this.m_value >> 10 & 1);
			this.fArabicNumContext = (this.m_value >> 11 & 1);
			this.fGcpClusters = (this.m_value >> 12 & 1);
			this.fReserved = (this.m_value >> 13 & 1);
			this.fEngineReserved = (this.m_value >> 14 & 3);
		}

		internal static int GetBidiLevel(ushort value)
		{
			return value & 0x1F;
		}

		internal SCRIPT_STATE GetAs_SCRIPT_STATE()
		{
			SCRIPT_STATE result = default(SCRIPT_STATE);
			result.word1 = (ushort)((this.uBidiLevel & 0x1F) | (this.fOverrideDirection & 1) << 5 | (this.fInhibitSymSwap & 1) << 6 | (this.fCharShape & 1) << 7 | (this.fDigitSubstitute & 1) << 8 | (this.fInhibitLigate & 1) << 9 | (this.fDisplayZWG & 1) << 10 | (this.fArabicNumContext & 1) << 11 | (this.fGcpClusters & 1) << 12 | (this.fReserved & 1) << 13 | (this.fEngineReserved & 3) << 14);
			return result;
		}
	}
}
