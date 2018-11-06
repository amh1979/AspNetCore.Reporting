using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class MapperBase : IDisposable
	{
		internal class FontCache : IDisposable
		{
			internal class TestAgent
			{
				private FontCache _host;

				public Dictionary<string, FontFamily> FontFamilies
				{
					get
					{
						return this._host.m_fontFamilies;
					}
				}

				public TestAgent(FontCache fontCache)
				{
					this._host = fontCache;
				}

				public FontFamily TryCreateFont(string familyName)
				{
					return this._host.TryCreateFontFamily(familyName);
				}

				public FontFamily RegisterFontFamilyWithFallback(string familyName)
				{
					return this._host.RegisterFontFamilyWithFallback(familyName);
				}

				public FontFamily GetFontFamily(string familyName)
				{
					return this._host.GetFontFamily(familyName);
				}
			}

			private class KeyInfo
			{
				internal class EqualityComparer : IEqualityComparer<KeyInfo>
				{
					public bool Equals(KeyInfo x, KeyInfo y)
					{
						if (x.m_id == y.m_id && x.m_size == y.m_size && x.m_style == y.m_style)
						{
							return string.Compare(x.m_fontFamily, y.m_fontFamily, StringComparison.Ordinal) == 0;
						}
						return false;
					}

					public int GetHashCode(KeyInfo obj)
					{
						return obj.m_fontFamily.GetHashCode() ^ obj.m_size.GetHashCode() ^ obj.m_id.GetHashCode() ^ obj.m_style.GetHashCode();
					}
				}

				private int m_id;

				private string m_fontFamily;

				private float m_size;

				private FontStyle m_style;

				public KeyInfo(int id, string family, float size, FontStyle style)
				{
					this.m_id = id;
					this.m_fontFamily = family;
					this.m_size = size;
					this.m_style = style;
				}
			}

			private string m_defaultFontFamily;

			private static float m_defaultFontSize = 10f;

			private Dictionary<KeyInfo, Font> m_cachedFonts = new Dictionary<KeyInfo, Font>(new KeyInfo.EqualityComparer());

			private List<Font> m_fonts = new List<Font>();

			private Dictionary<string, FontFamily> m_fontFamilies = new Dictionary<string, FontFamily>();

			internal FontCache(string defaultFontFamily)
			{
				this.m_defaultFontFamily = defaultFontFamily;
			}

			public Font GetFontFromCache(int id, string familyName, float size, FontStyle style)
			{
				KeyInfo key = new KeyInfo(id, familyName, size, style);
				if (!this.m_cachedFonts.ContainsKey(key))
				{
					this.m_cachedFonts.Add(key, this.CreateSafeFont(familyName, size, style));
				}
				return this.m_cachedFonts[key];
			}

			public Font GetDefaultFontFromCache(int id)
			{
				return this.GetFontFromCache(id, this.m_defaultFontFamily, FontCache.m_defaultFontSize, MappingHelper.GetStyleFontStyle(FontStyles.Normal, FontWeights.Normal, TextDecorations.None));
			}

			public Font GetFont(string familyName, float size, FontStyle style)
			{
				Font font = null;
				try
				{
					font = this.CreateSafeFont(familyName, size, style);
					this.m_fonts.Add(font);
					return font;
				}
				catch
				{
					if (font != null && !this.m_fonts.Contains(font))
					{
						font.Dispose();
						font = null;
					}
					throw;
				}
			}

			public Font GetDefaultFont()
			{
				return this.GetFont(this.m_defaultFontFamily, FontCache.m_defaultFontSize, MappingHelper.GetStyleFontStyle(FontStyles.Normal, FontWeights.Normal, TextDecorations.None));
			}

			private FontFamily TryCreateFontFamily(string familyName)
			{
				try
				{
					return new FontFamily(familyName);
				}
				catch
				{
				}
				return null;
			}

			private FontFamily RegisterFontFamilyWithFallback(string familyName)
			{
				FontFamily fontFamily = this.TryCreateFontFamily(familyName);
				if (fontFamily == null && familyName != this.m_defaultFontFamily)
				{
					fontFamily = this.TryCreateFontFamily(this.m_defaultFontFamily);
				}
				if (fontFamily == null && this.m_defaultFontFamily != "Arial")
				{
					fontFamily = this.TryCreateFontFamily("Arial");
				}
				if (fontFamily != null)
				{
					this.m_fontFamilies.Add(familyName, fontFamily);
				}
				return fontFamily;
			}

			private FontFamily GetFontFamily(string familyName)
			{
				FontFamily result = null;
				if (!this.m_fontFamilies.TryGetValue(familyName, out result))
				{
					result = this.RegisterFontFamilyWithFallback(familyName);
				}
				return result;
			}

			private Font CreateSafeFont(string familyName, float size, FontStyle fontStyle)
			{
				FontFamily fontFamily = this.GetFontFamily(familyName);
				if (fontFamily == null)
				{
					return new Font(familyName, size, fontStyle);
				}
				familyName = fontFamily.Name;
				if (fontFamily.IsStyleAvailable(fontStyle))
				{
					return new Font(familyName, size, fontStyle);
				}
				FontStyle fontStyle2 = FontStyle.Regular;
				foreach (FontStyle value in Enum.GetValues(typeof(FontStyle)))
				{
					if (fontFamily.IsStyleAvailable(value))
					{
						fontStyle2 = value;
						break;
					}
				}
				if (fontFamily.IsStyleAvailable(fontStyle | fontStyle2))
				{
					return new Font(familyName, size, fontStyle | fontStyle2);
				}
				return new Font(familyName, size, fontStyle2);
			}

			public void Dispose()
			{
				foreach (FontFamily value in this.m_fontFamilies.Values)
				{
					value.Dispose();
				}
				foreach (Font value2 in this.m_cachedFonts.Values)
				{
					value2.Dispose();
				}
				this.m_cachedFonts.Clear();
				foreach (Font font in this.m_fonts)
				{
					font.Dispose();
				}
				this.m_fonts.Clear();
				GC.SuppressFinalize(this);
			}
		}

		private FontCache m_fontCache;

		private float m_dpiX = 96f;

		private float m_dpiY = 96f;

		private int? m_widthOverrideInPixels = null;

		private int? m_heightOverrideInPixels = null;

		public float DpiX
		{
			internal get
			{
				return this.m_dpiX;
			}
			set
			{
				this.m_dpiX = value;
			}
		}

		public float DpiY
		{
			protected get
			{
				return this.m_dpiX;
			}
			set
			{
				this.m_dpiY = value;
			}
		}

		protected int? WidthOverrideInPixels
		{
			get
			{
				return this.m_widthOverrideInPixels;
			}
		}

		protected int? HeightOverrideInPixels
		{
			get
			{
				return this.m_heightOverrideInPixels;
			}
		}

		public double? WidthOverride
		{
			set
			{
				if (value.HasValue)
				{
					MapperBase.ValidatePositiveValue(value.Value);
					int value2 = MappingHelper.ToIntPixels(value.Value, Unit.Millimeter, this.DpiX);
					MapperBase.ValidatePositiveValue(value2);
					this.m_widthOverrideInPixels = value2;
				}
				else
				{
					this.m_widthOverrideInPixels = null;
				}
			}
		}

		public double? HeightOverride
		{
			set
			{
				if (value.HasValue)
				{
					MapperBase.ValidatePositiveValue(value.Value);
					int value2 = MappingHelper.ToIntPixels(value.Value, Unit.Millimeter, this.DpiY);
					MapperBase.ValidatePositiveValue(value2);
					this.m_heightOverrideInPixels = value2;
				}
				else
				{
					this.m_heightOverrideInPixels = null;
				}
			}
		}

		internal MapperBase(string defaultFontFamily)
		{
			this.m_fontCache = new FontCache(string.IsNullOrEmpty(defaultFontFamily) ? MappingHelper.DefaultFontFamily : defaultFontFamily);
		}

		internal Font GetDefaultFont()
		{
			return this.m_fontCache.GetDefaultFont();
		}

		internal Font GetDefaultFontFromCache(int id)
		{
			return this.m_fontCache.GetDefaultFontFromCache(id);
		}

		internal Font GetFontFromCache(int id, string fontFamily, float fontSize, FontStyles fontStyle, FontWeights fontWeight, TextDecorations textDecoration)
		{
			return this.m_fontCache.GetFontFromCache(id, fontFamily, fontSize, MappingHelper.GetStyleFontStyle(fontStyle, fontWeight, textDecoration));
		}

		internal Font GetFontFromCache(int id, Style style, StyleInstance styleInstance)
		{
			return this.GetFontFromCache(id, MappingHelper.GetStyleFontFamily(style, styleInstance, this.GetDefaultFont().Name), MappingHelper.GetStyleFontSize(style, styleInstance), MappingHelper.GetStyleFontStyle(style, styleInstance), MappingHelper.GetStyleFontWeight(style, styleInstance), MappingHelper.GetStyleFontTextDecoration(style, styleInstance));
		}

		protected Font GetFont(Style style, StyleInstance styleInstance)
		{
			return this.m_fontCache.GetFont(MappingHelper.GetStyleFontFamily(style, styleInstance, this.GetDefaultFont().Name), MappingHelper.GetStyleFontSize(style, styleInstance), MappingHelper.GetStyleFontStyle(MappingHelper.GetStyleFontStyle(style, styleInstance), MappingHelper.GetStyleFontWeight(style, styleInstance), MappingHelper.GetStyleFontTextDecoration(style, styleInstance)));
		}

		private static void ValidatePositiveValue(double value)
		{
			if (!double.IsNaN(value) && !(value <= 0.0) && !(value > 1.7976931348623157E+308))
			{
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, value);
		}

		private static void ValidatePositiveValue(int value)
		{
			if (value > 0 && value <= 2147483647)
			{
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, value);
		}

		public virtual void Dispose()
		{
			this.m_fontCache.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
