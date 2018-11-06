using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes
{
	internal class CT_Variant : OoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			variant,
			vector,
			array,
			blob,
			oblob,
			empty,
			_null,
			i1,
			i2,
			i4,
			i8,
			_int,
			ui1,
			ui2,
			ui4,
			ui8,
			_uint,
			r4,
			r8,
			_decimal,
			lpstr,
			lpwstr,
			bstr,
			date,
			filetime,
			_bool,
			cy,
			error,
			stream,
			ostream,
			storage,
			ostorage,
			vstream,
			clsid,
			cf
		}

		private CT_Variant _variant;

		private CT_Vector _vector;

		private string _blob;

		private string _oblob;

		private sbyte _i1;

		private short _i2;

		private int _i4;

		private long _i8;

		private int _int;

		private byte _ui1;

		private ushort _ui2;

		private uint _ui4;

		private ulong _ui8;

		private uint _uint;

		private double _r4;

		private double _r8;

		private double _decimal;

		private string _lpstr;

		private string _lpwstr;

		private string _bstr;

		private DateTime _date;

		private DateTime _filetime;

		private OoxmlBool _bool;

		private string _cy;

		private string _error;

		private string _stream;

		private string _ostream;

		private string _storage;

		private string _ostorage;

		private string _clsid;

		private ChoiceBucket_0 _choice_0;

		public CT_Variant Variant
		{
			get
			{
				return this._variant;
			}
			set
			{
				this._variant = value;
			}
		}

		public CT_Vector Vector
		{
			get
			{
				return this._vector;
			}
			set
			{
				this._vector = value;
			}
		}

		public string Blob
		{
			get
			{
				return this._blob;
			}
			set
			{
				this._blob = value;
			}
		}

		public string Oblob
		{
			get
			{
				return this._oblob;
			}
			set
			{
				this._oblob = value;
			}
		}

		public sbyte I1
		{
			get
			{
				return this._i1;
			}
			set
			{
				this._i1 = value;
			}
		}

		public short I2
		{
			get
			{
				return this._i2;
			}
			set
			{
				this._i2 = value;
			}
		}

		public int I4
		{
			get
			{
				return this._i4;
			}
			set
			{
				this._i4 = value;
			}
		}

		public long I8
		{
			get
			{
				return this._i8;
			}
			set
			{
				this._i8 = value;
			}
		}

		public int Int
		{
			get
			{
				return this._int;
			}
			set
			{
				this._int = value;
			}
		}

		public byte Ui1
		{
			get
			{
				return this._ui1;
			}
			set
			{
				this._ui1 = value;
			}
		}

		public ushort Ui2
		{
			get
			{
				return this._ui2;
			}
			set
			{
				this._ui2 = value;
			}
		}

		public uint Ui4
		{
			get
			{
				return this._ui4;
			}
			set
			{
				this._ui4 = value;
			}
		}

		public ulong Ui8
		{
			get
			{
				return this._ui8;
			}
			set
			{
				this._ui8 = value;
			}
		}

		public uint Uint
		{
			get
			{
				return this._uint;
			}
			set
			{
				this._uint = value;
			}
		}

		public double R4
		{
			get
			{
				return this._r4;
			}
			set
			{
				this._r4 = value;
			}
		}

		public double R8
		{
			get
			{
				return this._r8;
			}
			set
			{
				this._r8 = value;
			}
		}

		public double Decimal
		{
			get
			{
				return this._decimal;
			}
			set
			{
				this._decimal = value;
			}
		}

		public string Lpstr
		{
			get
			{
				return this._lpstr;
			}
			set
			{
				this._lpstr = value;
			}
		}

		public string Lpwstr
		{
			get
			{
				return this._lpwstr;
			}
			set
			{
				this._lpwstr = value;
			}
		}

		public string Bstr
		{
			get
			{
				return this._bstr;
			}
			set
			{
				this._bstr = value;
			}
		}

		public DateTime Date
		{
			get
			{
				return this._date;
			}
			set
			{
				this._date = value;
			}
		}

		public DateTime Filetime
		{
			get
			{
				return this._filetime;
			}
			set
			{
				this._filetime = value;
			}
		}

		public OoxmlBool Bool
		{
			get
			{
				return this._bool;
			}
			set
			{
				this._bool = value;
			}
		}

		public string Cy
		{
			get
			{
				return this._cy;
			}
			set
			{
				this._cy = value;
			}
		}

		public string Error
		{
			get
			{
				return this._error;
			}
			set
			{
				this._error = value;
			}
		}

		public string Stream
		{
			get
			{
				return this._stream;
			}
			set
			{
				this._stream = value;
			}
		}

		public string Ostream
		{
			get
			{
				return this._ostream;
			}
			set
			{
				this._ostream = value;
			}
		}

		public string Storage
		{
			get
			{
				return this._storage;
			}
			set
			{
				this._storage = value;
			}
		}

		public string Ostorage
		{
			get
			{
				return this._ostorage;
			}
			set
			{
				this._ostorage = value;
			}
		}

		public string Clsid
		{
			get
			{
				return this._clsid;
			}
			set
			{
				this._clsid = value;
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

		public static string VariantElementName
		{
			get
			{
				return "variant";
			}
		}

		public static string VectorElementName
		{
			get
			{
				return "vector";
			}
		}

		public static string I1ElementName
		{
			get
			{
				return "i1";
			}
		}

		public static string I2ElementName
		{
			get
			{
				return "i2";
			}
		}

		public static string I4ElementName
		{
			get
			{
				return "i4";
			}
		}

		public static string I8ElementName
		{
			get
			{
				return "i8";
			}
		}

		public static string IntElementName
		{
			get
			{
				return "int";
			}
		}

		public static string Ui1ElementName
		{
			get
			{
				return "ui1";
			}
		}

		public static string Ui2ElementName
		{
			get
			{
				return "ui2";
			}
		}

		public static string Ui4ElementName
		{
			get
			{
				return "ui4";
			}
		}

		public static string Ui8ElementName
		{
			get
			{
				return "ui8";
			}
		}

		public static string UintElementName
		{
			get
			{
				return "uint";
			}
		}

		public static string R4ElementName
		{
			get
			{
				return "r4";
			}
		}

		public static string R8ElementName
		{
			get
			{
				return "r8";
			}
		}

		public static string DecimalElementName
		{
			get
			{
				return "decimal";
			}
		}

		public static string DateElementName
		{
			get
			{
				return "date";
			}
		}

		public static string FiletimeElementName
		{
			get
			{
				return "filetime";
			}
		}

		public static string BoolElementName
		{
			get
			{
				return "bool";
			}
		}

		public static string BlobElementName
		{
			get
			{
				return "blob";
			}
		}

		public static string OblobElementName
		{
			get
			{
				return "oblob";
			}
		}

		public static string LpstrElementName
		{
			get
			{
				return "lpstr";
			}
		}

		public static string LpwstrElementName
		{
			get
			{
				return "lpwstr";
			}
		}

		public static string BstrElementName
		{
			get
			{
				return "bstr";
			}
		}

		public static string CyElementName
		{
			get
			{
				return "cy";
			}
		}

		public static string ErrorElementName
		{
			get
			{
				return "error";
			}
		}

		public static string StreamElementName
		{
			get
			{
				return "stream";
			}
		}

		public static string OstreamElementName
		{
			get
			{
				return "ostream";
			}
		}

		public static string StorageElementName
		{
			get
			{
				return "storage";
			}
		}

		public static string OstorageElementName
		{
			get
			{
				return "ostorage";
			}
		}

		public static string ClsidElementName
		{
			get
			{
				return "clsid";
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_variant(s, depth, namespaces);
			this.Write_vector(s, depth, namespaces);
			this.Write_blob(s, depth, namespaces);
			this.Write_oblob(s, depth, namespaces);
			this.Write_i1(s, depth, namespaces);
			this.Write_i2(s, depth, namespaces);
			this.Write_i4(s, depth, namespaces);
			this.Write_i8(s, depth, namespaces);
			this.Write_int(s, depth, namespaces);
			this.Write_ui1(s, depth, namespaces);
			this.Write_ui2(s, depth, namespaces);
			this.Write_ui4(s, depth, namespaces);
			this.Write_ui8(s, depth, namespaces);
			this.Write_uint(s, depth, namespaces);
			this.Write_r4(s, depth, namespaces);
			this.Write_r8(s, depth, namespaces);
			this.Write_decimal(s, depth, namespaces);
			this.Write_lpstr(s, depth, namespaces);
			this.Write_lpwstr(s, depth, namespaces);
			this.Write_bstr(s, depth, namespaces);
			this.Write_date(s, depth, namespaces);
			this.Write_filetime(s, depth, namespaces);
			this.Write_bool(s, depth, namespaces);
			this.Write_cy(s, depth, namespaces);
			this.Write_error(s, depth, namespaces);
			this.Write_stream(s, depth, namespaces);
			this.Write_ostream(s, depth, namespaces);
			this.Write_storage(s, depth, namespaces);
			this.Write_ostorage(s, depth, namespaces);
			this.Write_clsid(s, depth, namespaces);
		}

		public void Write_variant(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.variant && this._variant != null)
			{
				this._variant.Write(s, "variant", depth + 1, namespaces);
			}
		}

		public void Write_vector(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.vector && this._vector != null)
			{
				this._vector.Write(s, "vector", depth + 1, namespaces);
			}
		}

		public void Write_i1(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.i1)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i1", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._i1);
			}
		}

		public void Write_i2(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.i2)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i2", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._i2);
			}
		}

		public void Write_i4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.i4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._i4);
			}
		}

		public void Write_i8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.i8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._i8);
			}
		}

		public void Write_int(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0._int)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "int", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._int);
			}
		}

		public void Write_ui1(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ui1)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui1", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ui1);
			}
		}

		public void Write_ui2(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ui2)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui2", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ui2);
			}
		}

		public void Write_ui4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ui4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ui4);
			}
		}

		public void Write_ui8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ui8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ui8);
			}
		}

		public void Write_uint(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0._uint)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "uint", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._uint);
			}
		}

		public void Write_r4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.r4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "r4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._r4);
			}
		}

		public void Write_r8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.r8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "r8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._r8);
			}
		}

		public void Write_decimal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0._decimal)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "decimal", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._decimal);
			}
		}

		public void Write_date(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.date)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "date", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._date);
			}
		}

		public void Write_filetime(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.filetime)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "filetime", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._filetime);
			}
		}

		public void Write_bool(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0._bool)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "bool", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._bool);
			}
		}

		public void Write_blob(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.blob && this._blob != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "blob", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._blob);
			}
		}

		public void Write_oblob(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.oblob && this._oblob != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oblob", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._oblob);
			}
		}

		public void Write_lpstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.lpstr && this._lpstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "lpstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._lpstr);
			}
		}

		public void Write_lpwstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.lpwstr && this._lpwstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "lpwstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._lpwstr);
			}
		}

		public void Write_bstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.bstr && this._bstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "bstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._bstr);
			}
		}

		public void Write_cy(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.cy && this._cy != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "cy", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._cy);
			}
		}

		public void Write_error(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.error && this._error != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "error", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._error);
			}
		}

		public void Write_stream(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.stream && this._stream != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "stream", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._stream);
			}
		}

		public void Write_ostream(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ostream && this._ostream != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ostream", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ostream);
			}
		}

		public void Write_storage(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.storage && this._storage != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "storage", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._storage);
			}
		}

		public void Write_ostorage(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.ostorage && this._ostorage != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ostorage", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._ostorage);
			}
		}

		public void Write_clsid(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.clsid && this._clsid != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "clsid", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", this._clsid);
			}
		}
	}
}
