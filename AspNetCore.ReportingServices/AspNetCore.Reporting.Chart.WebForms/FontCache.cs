using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class FontCache
	{
		private Dictionary<string, Font> fontCache = new Dictionary<string, Font>();

		internal Font GetFont(string name, int size)
		{
			string key = string.Format(CultureInfo.InvariantCulture, "Name:{0}, Size:{1}", name, size);
			if (!this.fontCache.ContainsKey(key))
			{
				this.fontCache.Add(key, new Font(name, (float)size));
			}
			return this.fontCache[key];
		}

		~FontCache()
		{
			foreach (Font value in this.fontCache.Values)
			{
				value.Dispose();
			}
		}
	}
}
