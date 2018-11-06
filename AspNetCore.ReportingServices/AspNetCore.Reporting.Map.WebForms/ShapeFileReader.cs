using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeFileReader
	{
		private string fileName = string.Empty;

		private Stream shpStream;

		private Stream dbfStream;

		private int fileCode = 9994;

		private int fileLength;

		private int version = 1000;

		private ShapeType shapeType;

		private double xMin;

		private double xMax;

		private double yMin;

		private double yMax;

		private double zMin;

		private double zMax;

		private double mMin;

		private double mMax;

		private ArrayList points = new ArrayList();

		private ArrayList multiPoints = new ArrayList();

		private ArrayList polyLines = new ArrayList();

		private ArrayList polygons = new ArrayList();

		private DataTable table;

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public Stream ShpStream
		{
			get
			{
				return this.shpStream;
			}
			set
			{
				this.shpStream = value;
			}
		}

		public Stream DbfStream
		{
			get
			{
				return this.dbfStream;
			}
			set
			{
				this.dbfStream = value;
			}
		}

		public int FileCode
		{
			get
			{
				return this.fileCode;
			}
			set
			{
				this.fileCode = value;
			}
		}

		public int FileLength
		{
			get
			{
				return this.fileLength;
			}
			set
			{
				this.fileLength = value;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public ShapeType ShapeType
		{
			get
			{
				return this.shapeType;
			}
			set
			{
				this.shapeType = value;
			}
		}

		public double XMin
		{
			get
			{
				return this.xMin;
			}
			set
			{
				this.xMin = value;
			}
		}

		public double XMax
		{
			get
			{
				return this.xMax;
			}
			set
			{
				this.xMax = value;
			}
		}

		public double YMin
		{
			get
			{
				return this.yMin;
			}
			set
			{
				this.yMin = value;
			}
		}

		public double YMax
		{
			get
			{
				return this.yMax;
			}
			set
			{
				this.yMax = value;
			}
		}

		public double ZMin
		{
			get
			{
				return this.zMin;
			}
			set
			{
				this.zMin = value;
			}
		}

		public double ZMax
		{
			get
			{
				return this.zMax;
			}
			set
			{
				this.zMax = value;
			}
		}

		public double MMin
		{
			get
			{
				return this.mMin;
			}
			set
			{
				this.mMin = value;
			}
		}

		public double MMax
		{
			get
			{
				return this.mMax;
			}
			set
			{
				this.mMax = value;
			}
		}

		public ArrayList Points
		{
			get
			{
				return this.points;
			}
		}

		public ArrayList MultiPoints
		{
			get
			{
				return this.multiPoints;
			}
		}

		public ArrayList PolyLines
		{
			get
			{
				return this.polyLines;
			}
		}

		public ArrayList Polygons
		{
			get
			{
				return this.polygons;
			}
		}

		public DataTable Table
		{
			get
			{
				if (this.table == null)
				{
					this.table = new DataTable();
					this.table.Locale = CultureInfo.CurrentCulture;
				}
				return this.table;
			}
			set
			{
				this.table = value;
			}
		}

		public static SqlBytes File2SqlBytes(string fileName)
		{
			FileStream fileStream = null;
			byte[] array = null;
			SqlBytes @null = SqlBytes.Null;
			if (!File.Exists(fileName))
			{
				return @null;
			}
			using (fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				if (fileStream.Length == 0)
				{
					return @null;
				}
				if (fileStream.Length > 2147483647)
				{
					throw new InvalidDataException(SR.FileToLarge);
				}
				int num = (int)fileStream.Length;
				array = new byte[num];
				if (array == null)
				{
					throw new OutOfMemoryException(SR.UnableToAllocateMemoryForSqlBinary);
				}
				if (num != fileStream.Read(array, 0, num))
				{
					throw new IOException(SR.UnableToReadWholeFileToSqlBinary);
				}
			}
			return new SqlBytes(array);
		}

		public void LoadHeader()
		{
			bool flag = false;
			Stream stream = this.ShpStream;
			if (stream == null)
			{
				stream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read);
				flag = true;
			}
			BinaryReader binaryReader = new BinaryReader(stream);
			this.ReadHeader(binaryReader);
			binaryReader.Close();
			if (flag)
			{
				stream.Close();
			}
		}

		public void Load()
		{
			if (this.ShpStream != null)
			{
				BinaryReader binaryReader = new BinaryReader(this.ShpStream);
				try
				{
					this.ReadHeader(binaryReader);
					this.ReadShapes(binaryReader);
				}
				finally
				{
					binaryReader.Close();
				}
				if (this.DbfStream != null)
				{
					SqlBytes data = new SqlBytes(this.dbfStream);
					DBF dBF = new DBF(data);
					this.Table = dBF.GetDataTable();
				}
			}
			else
			{
				using (FileStream input = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
				{
					BinaryReader binaryReader2 = new BinaryReader(input);
					try
					{
						this.ReadHeader(binaryReader2);
						this.ReadShapes(binaryReader2);
					}
					finally
					{
						binaryReader2.Close();
					}
				}
				string path = this.FileName.Substring(0, this.FileName.LastIndexOf('.')) + ".dbf";
				if (File.Exists(path))
				{
					SqlBytes data2 = ShapeFileReader.File2SqlBytes(path);
					DBF dBF2 = new DBF(data2);
					this.Table = dBF2.GetDataTable();
				}
			}
		}

		internal void ReadHeader(BinaryReader reader)
		{
			this.FileCode = this.SwapBytes(reader.ReadInt32());
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			this.FileLength = this.SwapBytes(reader.ReadInt32());
			this.Version = reader.ReadInt32();
			this.ShapeType = (ShapeType)reader.ReadInt32();
			this.XMin = reader.ReadDouble();
			this.YMin = reader.ReadDouble();
			this.XMax = reader.ReadDouble();
			this.YMax = reader.ReadDouble();
			this.ZMin = reader.ReadDouble();
			this.ZMax = reader.ReadDouble();
			this.MMin = reader.ReadDouble();
			this.MMax = reader.ReadDouble();
		}

		internal void ReadShapes(BinaryReader reader)
		{
			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				this.SwapBytes(reader.ReadInt32());
				this.SwapBytes(reader.ReadInt32());
				switch (reader.ReadInt32())
				{
				case 1:
				{
					ShapePoint shapePoint = new ShapePoint();
					shapePoint.Read(reader);
					this.Points.Add(shapePoint);
					break;
				}
				case 8:
				{
					MultiPoint multiPoint = new MultiPoint();
					multiPoint.Read(reader);
					this.MultiPoints.Add(multiPoint);
					break;
				}
				case 3:
				{
					PolyLine polyLine2 = new PolyLine();
					polyLine2.Read(reader);
					this.PolyLines.Add(polyLine2);
					break;
				}
				case 5:
				{
					PolyLine polyLine = new PolyLine();
					polyLine.Read(reader);
					this.Polygons.Add(polyLine);
					break;
				}
				}
			}
		}

		public static BasicMapElements? DetermineMapElementsFromShapeFile(string fileName, out ShapeType? unsupportedShapeType)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.FileName = fileName;
			return shapeFileReader.DetermineMapElements(out unsupportedShapeType);
		}

		public static BasicMapElements? DetermineMapElementsFromShapeFile(Stream shpStream, out ShapeType? unsupportedShapeType)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.ShpStream = shpStream;
			return shapeFileReader.DetermineMapElements(out unsupportedShapeType);
		}

		private BasicMapElements? DetermineMapElements(out ShapeType? unsupportedShapeType)
		{
			this.LoadHeader();
			unsupportedShapeType = null;
			if (this.ShapeType == ShapeType.Polygon)
			{
				return BasicMapElements.Shapes;
			}
			if (this.ShapeType == ShapeType.PolyLine)
			{
				return BasicMapElements.Paths;
			}
			if (this.ShapeType != ShapeType.Point && this.ShapeType != ShapeType.MultiPoint)
			{
				unsupportedShapeType = this.ShapeType;
				return null;
			}
			return BasicMapElements.Symbols;
		}

		public static string GetShortFileName(string fullPath, string fileName)
		{
			string result = "";
			try
			{
				Assembly assembly = Assembly.LoadWithPartialName("System.Web");
				Type type = assembly.GetType("System.Web.Util.FindFileData");
				object[] array = new object[2]
				{
					fullPath,
					null
				};
				type.InvokeMember("FindFile", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, array, CultureInfo.InvariantCulture);
				if (type.GetField("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField) != null)
				{
					result = (string)type.InvokeMember("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, array[1], null, CultureInfo.InvariantCulture);
					return result;
				}
				if (type.GetProperty("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty) != null)
				{
					result = (string)type.InvokeMember("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, array[1], null, CultureInfo.InvariantCulture);
					return result;
				}
				result = fileName.Substring(0, 6) + "~1";
				return result;
			}
			catch
			{
				return result;
			}
		}

		public static DataTable ReadDBFThroughOLEDB(string fullPath)
		{
			int num = fullPath.LastIndexOf("\\", StringComparison.Ordinal);
			string str = fullPath.Substring(0, num + 1);
			string text = fullPath.Substring(num + 1);
			text = text.Substring(0, text.LastIndexOf('.'));
			if (text.Length > 8)
			{
				text = ShapeFileReader.GetShortFileName(fullPath, text);
				text = text.Substring(0, text.LastIndexOf('.'));
			}
			string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + str + "; Extended Properties=dBase 5.0;";
			string sql = "SELECT * FROM [" + text + "#DBF]";
			DataSet dataSet = ShapeFileReader.GetDataSet(connectionString, sql);
			return dataSet.Tables[0];
		}

		public static DataSet GetDataSet(string connectionString, string sql)
		{
            throw new NotSupportedException("oledb");
		}

		public static bool IsDataSchemaIdentical(DataTable table1, DataTable table2)
		{
			foreach (DataColumn column in table1.Columns)
			{
				if (!table2.Columns.Contains(column.ColumnName) || table2.Columns[column.ColumnName].DataType != column.DataType)
				{
					return false;
				}
			}
			foreach (DataColumn column2 in table2.Columns)
			{
				if (!table1.Columns.Contains(column2.ColumnName) || table1.Columns[column2.ColumnName].DataType != column2.DataType)
				{
					return false;
				}
			}
			return true;
		}

		internal int SwapBytes(int inputValue)
		{
			long num = inputValue & 4278190080u;
			num >>= 24;
			long num2 = inputValue & 0xFF0000;
			num2 >>= 8;
			long num3 = inputValue & 0xFF00;
			num3 <<= 8;
			long num4 = inputValue & 0xFF;
			num4 <<= 24;
			return (int)(num | num2 | num3 | num4);
		}
	}
}
