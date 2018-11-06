using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class StyleManager
	{
		private abstract class ElementBag<T>
		{
			protected readonly IDictionary<uint, T> _values = new Dictionary<uint, T>();

			protected readonly IDictionary<T, uint> _lookup = new Dictionary<T, uint>();

			protected uint _counter;

			public uint Count
			{
				get
				{
					return (uint)this._values.Count;
				}
			}

			public ElementBag(uint startIndex)
			{
				this._counter = startIndex;
			}

			protected abstract T Freeze(T value);

			public void Add(T value, uint index)
			{
				if (this._counter <= index)
				{
					this._counter = index + 1;
				}
				this._lookup[value] = index;
				this._values[index] = value;
			}

			public uint Add(T value)
			{
				uint result = default(uint);
				if (this._lookup.TryGetValue(value, out result))
				{
					return result;
				}
				this._lookup[value] = this._counter;
				this._values[this._counter] = value;
				return this._counter++;
			}

			public void ForceAdd(T value)
			{
				if (!this._lookup.ContainsKey(value))
				{
					this._lookup[value] = this._counter;
				}
				this._values[this._counter] = value;
				this._counter += 1u;
			}

			public T Get(uint index)
			{
				T value = default(T);
				if (this._values.TryGetValue(index, out value))
				{
					return this.Freeze(value);
				}
				throw new FatalException();
			}

			public IEnumerable<KeyValuePair<uint, T>> GetPairs()
			{
				foreach (KeyValuePair<uint, T> item in (IEnumerable<KeyValuePair<uint, T>>)this._values)
				{
					yield return item;
				}
			}
		}

		private class ElementBagOfStrings : ElementBag<string>
		{
			public ElementBagOfStrings(uint startIndex)
				: base(startIndex)
			{
			}

			protected override string Freeze(string value)
			{
				return value;
			}
		}

		private class ElementBagCloneable<T> : ElementBag<T> where T : IDeepCloneable<T>
		{
			public ElementBagCloneable(uint startIndex)
				: base(startIndex)
			{
			}

			protected override T Freeze(T value)
			{
				return value.DeepClone();
			}
		}

		private readonly StyleSheetPart _part;

		private readonly CT_Stylesheet _stylesheet;

		private readonly ElementBag<string> _numberFormats = new ElementBagOfStrings(82u);

		private readonly ElementBag<XMLFontModel> _fonts = new ElementBagCloneable<XMLFontModel>(0u);

		private readonly ElementBag<XMLFillModel> _fills = new ElementBagCloneable<XMLFillModel>(0u);

		private readonly ElementBag<XMLBorderModel> _borders = new ElementBagCloneable<XMLBorderModel>(0u);

		private readonly ElementBag<XMLStyleModel> _styles = new ElementBagCloneable<XMLStyleModel>(0u);

		private readonly XMLPaletteModel _palette = new XMLPaletteModel();

		private readonly Dictionary<string, uint> _namedlookup = new Dictionary<string, uint>();

		public XMLPaletteModel Palette
		{
			get
			{
				return this._palette;
			}
		}

		public StyleManager(StyleSheetPart part)
		{
			this._part = part;
			this._stylesheet = (CT_Stylesheet)this._part.Root;
			this.Hydrate();
		}

		public static CT_Xf CreateDefaultXf()
		{
			CT_Xf cT_Xf = new CT_Xf();
			cT_Xf.NumFmtId_Attr = 0u;
			cT_Xf.FontId_Attr = 0u;
			cT_Xf.FillId_Attr = 0u;
			cT_Xf.BorderId_Attr = 0u;
			return cT_Xf;
		}

		public XMLStyleModel CreateStyle()
		{
			return new XMLStyleModel(this);
		}

		public XMLStyleModel CreateStyle(XMLStyleModel src)
		{
			return (XMLStyleModel)src.cloneStyle(true);
		}

		public XMLStyleModel CreateStyle(uint index)
		{
			if (index != 0)
			{
				return (XMLStyleModel)this.GetStyle(index).cloneStyle(true);
			}
			return new XMLStyleModel(StyleManager.CreateDefaultXf(), this);
		}

		public uint CommitStyle(XMLStyleModel style)
		{
			style.Index = this._styles.Add(style);
			return style.Index;
		}

		public XMLFontModel GetFont(uint index)
		{
			return this._fonts.Get(index);
		}

		public XMLFillModel GetFill(uint index)
		{
			return this._fills.Get(index);
		}

		public XMLBorderModel GetBorder(uint index)
		{
			return this._borders.Get(index);
		}

		public uint AddNumberFormat(string format)
		{
			return this._numberFormats.Add(format);
		}

		public uint AddFont(XMLFontModel model)
		{
			return this._fonts.Add(model);
		}

		public uint AddFill(XMLFillModel model)
		{
			return this._fills.Add(model);
		}

		public uint AddBorder(XMLBorderModel model)
		{
			return this._borders.Add(model);
		}

		public XMLStyleModel GetStyle(uint index)
		{
			return this._styles.Get(index);
		}

		private void Hydrate()
		{
			if (this._stylesheet.Borders == null)
			{
				this._stylesheet.Borders = new CT_Borders();
			}
			if (this._stylesheet.CellStyles == null)
			{
				this._stylesheet.CellStyles = new CT_CellStyles();
			}
			if (this._stylesheet.CellStyleXfs == null)
			{
				this._stylesheet.CellStyleXfs = new CT_CellStyleXfs();
			}
			if (this._stylesheet.CellXfs == null)
			{
				this._stylesheet.CellXfs = new CT_CellXfs();
			}
			if (this._stylesheet.Fills == null)
			{
				this._stylesheet.Fills = new CT_Fills();
			}
			if (this._stylesheet.Fonts == null)
			{
				this._stylesheet.Fonts = new CT_Fonts();
			}
			if (this._stylesheet.NumFmts == null)
			{
				this._stylesheet.NumFmts = new CT_NumFmts();
			}
			if (this._stylesheet.Borders.Border == null)
			{
				this._stylesheet.Borders.Border = new List<CT_Border>();
			}
			if (this._stylesheet.CellStyles.CellStyle == null)
			{
				this._stylesheet.CellStyles.CellStyle = new List<CT_CellStyle>();
			}
			if (this._stylesheet.CellStyleXfs.Xf == null)
			{
				this._stylesheet.CellStyleXfs.Xf = new List<CT_Xf>();
			}
			if (this._stylesheet.CellXfs.Xf == null)
			{
				this._stylesheet.CellXfs.Xf = new List<CT_Xf>();
			}
			if (this._stylesheet.Fills.Fill == null)
			{
				this._stylesheet.Fills.Fill = new List<CT_Fill>();
			}
			if (this._stylesheet.Fonts.Font == null)
			{
				this._stylesheet.Fonts.Font = new List<CT_Font>();
			}
			if (this._stylesheet.NumFmts.NumFmt == null)
			{
				this._stylesheet.NumFmts.NumFmt = new List<CT_NumFmt>();
			}
			foreach (CT_NumFmt item in this._stylesheet.NumFmts.NumFmt)
			{
				this._numberFormats.Add(item.FormatCode_Attr, item.NumFmtId_Attr);
			}
			for (int i = 0; i < this._stylesheet.Fonts.Font.Count; i++)
			{
				CT_Font font = this._stylesheet.Fonts.Font[i];
				this._fonts.Add(new XMLFontModel(font, this.Palette), (uint)i);
			}
			foreach (CT_Fill item2 in this._stylesheet.Fills.Fill)
			{
				this._fills.Add(new XMLFillModel(item2, this.Palette));
			}
			foreach (CT_Border item3 in this._stylesheet.Borders.Border)
			{
				this._borders.Add(new XMLBorderModel(item3, this.Palette));
			}
			foreach (CT_Xf item4 in this._stylesheet.CellXfs.Xf)
			{
				this._styles.ForceAdd(new XMLStyleModel(item4, this, false));
			}
			List<string> list = new List<string>();
			foreach (CT_CellStyle item5 in this._stylesheet.CellStyles.CellStyle)
			{
				list.Add(item5.Name_Attr);
				this._namedlookup.Add(item5.Name_Attr, item5.XfId_Attr);
			}
			this._stylesheet.NumFmts.NumFmt.Clear();
			this._stylesheet.Fonts.Font.Clear();
			this._stylesheet.Fills.Fill.Clear();
			this._stylesheet.Borders.Border.Clear();
			this._stylesheet.CellXfs.Xf.Clear();
			this._stylesheet.CellStyles.CellStyle.Clear();
		}

		public void Cleanup()
		{
			foreach (KeyValuePair<uint, XMLStyleModel> pair in this._styles.GetPairs())
			{
				pair.Value.Cleanup();
				this._stylesheet.CellXfs.Xf.Add(pair.Value.Data);
			}
			foreach (KeyValuePair<string, uint> item in this._namedlookup)
			{
				CT_CellStyle cT_CellStyle = new CT_CellStyle();
				cT_CellStyle.Name_Attr = item.Key;
				cT_CellStyle.XfId_Attr = item.Value;
				this._stylesheet.CellStyles.CellStyle.Add(cT_CellStyle);
			}
			foreach (KeyValuePair<uint, string> pair2 in this._numberFormats.GetPairs())
			{
				CT_NumFmt cT_NumFmt = new CT_NumFmt();
				cT_NumFmt.NumFmtId_Attr = pair2.Key;
				cT_NumFmt.FormatCode_Attr = pair2.Value;
				this._stylesheet.NumFmts.NumFmt.Add(cT_NumFmt);
			}
			foreach (KeyValuePair<uint, XMLFontModel> pair3 in this._fonts.GetPairs())
			{
				this._stylesheet.Fonts.Font.Add(pair3.Value.Data);
			}
			foreach (KeyValuePair<uint, XMLFillModel> pair4 in this._fills.GetPairs())
			{
				pair4.Value.Cleanup();
				this._stylesheet.Fills.Fill.Add(pair4.Value.Data);
			}
			foreach (KeyValuePair<uint, XMLBorderModel> pair5 in this._borders.GetPairs())
			{
				this._stylesheet.Borders.Border.Add(pair5.Value.Data);
			}
			if (this._palette.LegacyPaletteModified)
			{
				this._palette.WriteIndexedColors(this._stylesheet);
			}
			this._stylesheet.NumFmts.Count_Attr = this._numberFormats.Count;
			this._stylesheet.Fonts.Count_Attr = this._fonts.Count;
			this._stylesheet.Fills.Count_Attr = this._fills.Count;
			this._stylesheet.Borders.Count_Attr = this._borders.Count;
			this._stylesheet.CellXfs.Count_Attr = this._styles.Count;
			this._stylesheet.CellStyles.Count_Attr = (uint)this._namedlookup.Count;
		}
	}
}
