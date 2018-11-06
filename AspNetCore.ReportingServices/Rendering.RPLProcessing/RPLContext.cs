using System.Collections;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLContext
	{
		private Hashtable m_sharedProps;

		private Hashtable m_sharedImages;

		private BinaryReader m_binaryReader;

		private RPLVersionEnum m_versionPicker;

		internal BinaryReader BinaryReader
		{
			get
			{
				return this.m_binaryReader;
			}
		}

		internal Hashtable SharedProps
		{
			get
			{
				return this.m_sharedProps;
			}
			set
			{
				this.m_sharedProps = value;
			}
		}

		internal Hashtable SharedImages
		{
			get
			{
				return this.m_sharedImages;
			}
			set
			{
				this.m_sharedImages = value;
			}
		}

		internal RPLVersionEnum VersionPicker
		{
			get
			{
				return this.m_versionPicker;
			}
			set
			{
				this.m_versionPicker = value;
			}
		}

		internal RPLContext(BinaryReader reader)
		{
			this.m_binaryReader = reader;
		}

		public void Release()
		{
			if (this.m_binaryReader != null)
			{
				this.m_binaryReader.Close();
				this.m_binaryReader = null;
			}
		}
	}
}
