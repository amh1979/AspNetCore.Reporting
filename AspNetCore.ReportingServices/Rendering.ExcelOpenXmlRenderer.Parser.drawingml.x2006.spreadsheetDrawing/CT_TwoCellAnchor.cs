using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_TwoCellAnchor : OoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			sp,
			grpSp,
			graphicFrame,
			cxnSp,
			pic,
			contentPart
		}

		private ST_EditAs _editAs_attr;

		private CT_Marker _from;

		private CT_Marker _to;

		private CT_Picture _pic;

		private CT_AnchorClientData _clientData;

		private ChoiceBucket_0 _choice_0;

		public ST_EditAs EditAs_Attr
		{
			get
			{
				return this._editAs_attr;
			}
			set
			{
				this._editAs_attr = value;
			}
		}

		public CT_Marker From
		{
			get
			{
				return this._from;
			}
			set
			{
				this._from = value;
			}
		}

		public CT_Marker To
		{
			get
			{
				return this._to;
			}
			set
			{
				this._to = value;
			}
		}

		public CT_Picture Pic
		{
			get
			{
				return this._pic;
			}
			set
			{
				this._pic = value;
			}
		}

		public CT_AnchorClientData ClientData
		{
			get
			{
				return this._clientData;
			}
			set
			{
				this._clientData = value;
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

		public static string FromElementName
		{
			get
			{
				return "from";
			}
		}

		public static string ToElementName
		{
			get
			{
				return "to";
			}
		}

		public static string PicElementName
		{
			get
			{
				return "pic";
			}
		}

		public static string ClientDataElementName
		{
			get
			{
				return "clientData";
			}
		}

		protected override void InitAttributes()
		{
			this._editAs_attr = ST_EditAs.twoCell;
		}

		protected override void InitElements()
		{
			this._from = new CT_Marker();
			this._to = new CT_Marker();
			this._clientData = new CT_AnchorClientData();
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, true);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, false);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			this.WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (this._editAs_attr != ST_EditAs.twoCell)
			{
				s.Write(" editAs=\"");
				OoxmlComplexType.WriteData(s, this._editAs_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_from(s, depth, namespaces);
			this.Write_to(s, depth, namespaces);
			this.Write_pic(s, depth, namespaces);
			this.Write_clientData(s, depth, namespaces);
		}

		public void Write_from(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._from != null)
			{
				this._from.Write(s, "from", depth + 1, namespaces);
			}
		}

		public void Write_to(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._to != null)
			{
				this._to.Write(s, "to", depth + 1, namespaces);
			}
		}

		public void Write_pic(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.pic && this._pic != null)
			{
				this._pic.Write(s, "pic", depth + 1, namespaces);
			}
		}

		public void Write_clientData(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._clientData != null)
			{
				this._clientData.Write(s, "clientData", depth + 1, namespaces);
			}
		}
	}
}
