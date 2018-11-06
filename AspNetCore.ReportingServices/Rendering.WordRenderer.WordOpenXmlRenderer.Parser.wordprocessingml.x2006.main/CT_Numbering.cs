using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Numbering : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DecimalNumber _numIdMacAtCleanup;

		private List<CT_AbstractNum> _abstractNum;

		private List<CT_Num> _num;

		public CT_DecimalNumber NumIdMacAtCleanup
		{
			get
			{
				return this._numIdMacAtCleanup;
			}
			set
			{
				this._numIdMacAtCleanup = value;
			}
		}

		public List<CT_AbstractNum> AbstractNum
		{
			get
			{
				return this._abstractNum;
			}
			set
			{
				this._abstractNum = value;
			}
		}

		public List<CT_Num> Num
		{
			get
			{
				return this._num;
			}
			set
			{
				this._num = value;
			}
		}

		public static string NumIdMacAtCleanupElementName
		{
			get
			{
				return "numIdMacAtCleanup";
			}
		}

		public static string AbstractNumElementName
		{
			get
			{
				return "abstractNum";
			}
		}

		public static string NumElementName
		{
			get
			{
				return "num";
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
			this._abstractNum = new List<CT_AbstractNum>();
			this._num = new List<CT_Num>();
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
			this.Write_abstractNum(s);
			this.Write_num(s);
			this.Write_numIdMacAtCleanup(s);
		}

		public void Write_numIdMacAtCleanup(TextWriter s)
		{
			if (this._numIdMacAtCleanup != null)
			{
				this._numIdMacAtCleanup.Write(s, "numIdMacAtCleanup");
			}
		}

		public void Write_abstractNum(TextWriter s)
		{
			if (this._abstractNum != null)
			{
				foreach (CT_AbstractNum item in this._abstractNum)
				{
					if (item != null)
					{
						item.Write(s, "abstractNum");
					}
				}
			}
		}

		public void Write_num(TextWriter s)
		{
			if (this._num != null)
			{
				foreach (CT_Num item in this._num)
				{
					if (item != null)
					{
						item.Write(s, "num");
					}
				}
			}
		}
	}
}
