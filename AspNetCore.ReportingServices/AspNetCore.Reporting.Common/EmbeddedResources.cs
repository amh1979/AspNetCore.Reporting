using System.IO;
using System.Reflection;

namespace AspNetCore.Reporting.Common
{
	internal static class EmbeddedResources
	{
		public static byte[] Get(ResourceList list, string name, out string mimeType)
		{
			Stream stream = EmbeddedResources.GetStream(list, name, out mimeType);
			if (stream == null)
			{
				return null;
			}
			using (stream)
			{
				int num = (int)stream.Length;
				byte[] array = new byte[num];
				stream.Read(array, 0, num);
				return array;
			}
		}

		public static Stream GetStream(ResourceList list, string name, out string mimeType)
		{
			ResourceItem resourceItem = default(ResourceItem);
			if (list.TryGetResourceItem(name, out resourceItem))
			{
				mimeType = resourceItem.MimeType;
				return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceItem.EffectiveName);
			}
			mimeType = null;
			return null;
		}
	}
}
