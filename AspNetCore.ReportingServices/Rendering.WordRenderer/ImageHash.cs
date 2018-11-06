using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class ImageHash
	{
		private byte[] m_md4;

		private RPLFormat.Sizings m_sizing;

		private int m_width;

		private int m_height;

		internal ImageHash(byte[] md4, RPLFormat.Sizings sizing, int width, int height)
		{
			this.m_md4 = md4;
			this.m_sizing = sizing;
			this.m_width = width;
			this.m_height = height;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.m_md4.Length; i++)
			{
				num += this.m_md4[i];
			}
			num += this.m_width;
			num += this.m_height;
			return num + (int)this.m_sizing;
		}

		public override bool Equals(object obj)
		{
			ImageHash imageHash = (ImageHash)obj;
			if (this.m_sizing == imageHash.m_sizing && this.m_width == imageHash.m_width && this.m_height == imageHash.m_height && this.m_md4.Length == imageHash.m_md4.Length)
			{
				for (int i = 0; i < this.m_md4.Length; i++)
				{
					if (this.m_md4[i] != imageHash.m_md4[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
