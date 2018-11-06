using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_ShapeProperties : OoxmlComplexType, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			custGeom,
			prstGeom
		}

		internal enum ChoiceBucket_1
		{
			noFill,
			solidFill,
			gradFill,
			blipFill,
			pattFill,
			grpFill
		}

		internal enum ChoiceBucket_2
		{
			effectLst,
			effectDag
		}

		private CT_Transform2D _xfrm;

		private CT_PresetGeometry2D _prstGeom;

		private ChoiceBucket_0 _choice_0;

		private ChoiceBucket_1 _choice_1;

		private ChoiceBucket_2 _choice_2;

		public CT_Transform2D Xfrm
		{
			get
			{
				return this._xfrm;
			}
			set
			{
				this._xfrm = value;
			}
		}

		public CT_PresetGeometry2D PrstGeom
		{
			get
			{
				return this._prstGeom;
			}
			set
			{
				this._prstGeom = value;
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

		public ChoiceBucket_1 Choice_1
		{
			get
			{
				return this._choice_1;
			}
			set
			{
				this._choice_1 = value;
			}
		}

		public ChoiceBucket_2 Choice_2
		{
			get
			{
				return this._choice_2;
			}
			set
			{
				this._choice_2 = value;
			}
		}

		public static string XfrmElementName
		{
			get
			{
				return "xfrm";
			}
		}

		public static string PrstGeomElementName
		{
			get
			{
				return "prstGeom";
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
			base.WriteOpenTag(s, tagName, "pic", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</pic:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_xfrm(s);
			this.Write_prstGeom(s);
		}

		public void Write_xfrm(TextWriter s)
		{
			if (this._xfrm != null)
			{
				this._xfrm.Write(s, "xfrm");
			}
		}

		public void Write_prstGeom(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.prstGeom && this._prstGeom != null)
			{
				this._prstGeom.Write(s, "prstGeom");
			}
		}
	}
}
