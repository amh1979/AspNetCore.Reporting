using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Drawing : OoxmlComplexType, IEG_RunInnerContent, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			anchor,
			inline
		}

		private CT_Inline _inline;

		private ChoiceBucket_0 _choice_0;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_Drawing;
			}
		}

		public CT_Inline Inline
		{
			get
			{
				return this._inline;
			}
			set
			{
				this._inline = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return this._choice_0;
			}
			set
			{
				this._choice_0 = value;
			}
		}

		public static string InlineElementName
		{
			get
			{
				return "inline";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_inline(s);
		}

		public void Write_inline(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.inline && this._inline != null)
			{
				this._inline.Write(s, "inline");
			}
		}
	}
}
