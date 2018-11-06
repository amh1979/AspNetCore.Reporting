using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class IntermediateFormatReader
	{
		internal sealed class State
		{
			private DeclarationList m_oldDeclarations;

			private IntList[][] m_oldIndexesToSkip;

			private bool[][] m_isInOldDeclaration;

			internal static readonly State Current = new State(DeclarationList.Current);

			internal DeclarationList OldDeclarations
			{
				get
				{
					return this.m_oldDeclarations;
				}
			}

			internal IntList[][] OldIndexesToSkip
			{
				get
				{
					return this.m_oldIndexesToSkip;
				}
			}

			internal bool[][] IsInOldDeclaration
			{
				get
				{
					return this.m_isInOldDeclaration;
				}
			}

			internal State()
			{
				this.m_oldDeclarations = new DeclarationList();
				this.Initialize();
			}

			private State(DeclarationList declarations)
			{
				this.m_oldDeclarations = declarations;
				this.Initialize();
			}

			private void Initialize()
			{
				this.m_oldIndexesToSkip = new IntList[DeclarationList.Current.Count][];
				this.m_isInOldDeclaration = new bool[DeclarationList.Current.Count][];
			}
		}

		private sealed class Indexes
		{
			internal int CurrentIndex;
		}

		private sealed class ReportServerBinaryReader
		{
			private sealed class BinaryReaderWrapper : BinaryReader
			{
				internal BinaryReaderWrapper(Stream stream)
					: base(stream, Encoding.Unicode)
				{
				}

				internal new int Read7BitEncodedInt()
				{
					return base.Read7BitEncodedInt();
				}

				public override byte ReadByte()
				{
					return (byte)this.BaseStream.ReadByte();
				}
			}

			internal delegate void DeclarationCallback(ObjectType objectType, Declaration declaration);

			private BinaryReaderWrapper m_binaryReader;

			private DeclarationCallback m_declarationCallback;

			private Token m_token;

			private ObjectType m_objectType = ReportServerBinaryReader.ObjectTypeDefault;

			private int m_referenceValue = ReportServerBinaryReader.ReferenceDefault;

			private int m_enumValue = ReportServerBinaryReader.EnumDefault;

			private int m_arrayLength = ReportServerBinaryReader.ArrayLengthDefault;

			private Guid m_guidValue = ReportServerBinaryReader.GuidDefault;

			private string m_stringValue = ReportServerBinaryReader.StringDefault;

			private DateTime m_dateTimeValue = ReportServerBinaryReader.DateTimeDefault;

			private TimeSpan m_timeSpanValue = ReportServerBinaryReader.TimeSpanDefault;

			private char m_charValue = ReportServerBinaryReader.CharDefault;

			private bool m_booleanValue = ReportServerBinaryReader.BooleanDefault;

			private short m_int16Value = ReportServerBinaryReader.Int16Default;

			private int m_int32Value = ReportServerBinaryReader.Int32Default;

			private long m_int64Value = ReportServerBinaryReader.Int64Default;

			private ushort m_uint16Value = ReportServerBinaryReader.UInt16Default;

			private uint m_uint32Value = ReportServerBinaryReader.UInt32Default;

			private ulong m_uint64Value = ReportServerBinaryReader.UInt64Default;

			private byte m_byteValue = ReportServerBinaryReader.ByteDefault;

			private sbyte m_sbyteValue = ReportServerBinaryReader.SByteDefault;

			private float m_singleValue = ReportServerBinaryReader.SingleDefault;

			private double m_doubleValue = ReportServerBinaryReader.DoubleDefault;

			private decimal m_decimalValue = ReportServerBinaryReader.DecimalDefault;

			private Token m_arrayType = ReportServerBinaryReader.ArrayTypeDefault;

			private byte[] m_bytesValue = ReportServerBinaryReader.BytesDefault;

			private int[] m_int32sValue = ReportServerBinaryReader.Int32sDefault;

			private char[] m_charsValue = ReportServerBinaryReader.CharsDefault;

			private float[] m_floatsValue = ReportServerBinaryReader.FloatsDefault;

			private static readonly ObjectType ObjectTypeDefault = ObjectType.None;

			private static readonly int ReferenceDefault = 0;

			private static readonly int EnumDefault = 0;

			private static readonly int ArrayLengthDefault = 0;

			private static readonly Guid GuidDefault = Guid.Empty;

			private static readonly string StringDefault = null;

			private static readonly DateTime DateTimeDefault = new DateTime(0L);

			private static readonly TimeSpan TimeSpanDefault = new TimeSpan(0L);

			private static readonly char CharDefault = '\0';

			private static readonly bool BooleanDefault = false;

			private static readonly short Int16Default = 0;

			private static readonly int Int32Default = 0;

			private static readonly long Int64Default = 0L;

			private static readonly ushort UInt16Default = 0;

			private static readonly uint UInt32Default = 0u;

			private static readonly ulong UInt64Default = 0uL;

			private static readonly byte ByteDefault = 0;

			private static readonly sbyte SByteDefault = 0;

			private static readonly float SingleDefault = 0f;

			private static readonly double DoubleDefault = 0.0;

			private static readonly decimal DecimalDefault = 0m;

			private static readonly Token ArrayTypeDefault = Token.Byte;

			private static readonly byte[] BytesDefault = null;

			private static readonly int[] Int32sDefault = null;

			private static readonly char[] CharsDefault = null;

			private static readonly float[] FloatsDefault = null;

			internal Token Token
			{
				get
				{
					return this.m_token;
				}
			}

			internal ObjectType ObjectType
			{
				get
				{
					return this.m_objectType;
				}
			}

			internal Token ArrayType
			{
				get
				{
					return this.m_arrayType;
				}
			}

			internal int ArrayLength
			{
				get
				{
					return this.m_arrayLength;
				}
			}

			internal int ReferenceValue
			{
				get
				{
					return this.m_referenceValue;
				}
			}

			internal string StringValue
			{
				get
				{
					return this.m_stringValue;
				}
			}

			internal char CharValue
			{
				get
				{
					return this.m_charValue;
				}
			}

			internal char[] CharsValue
			{
				get
				{
					return this.m_charsValue;
				}
			}

			internal bool BooleanValue
			{
				get
				{
					return this.m_booleanValue;
				}
			}

			internal short Int16Value
			{
				get
				{
					return this.m_int16Value;
				}
			}

			internal int Int32Value
			{
				get
				{
					return this.m_int32Value;
				}
			}

			internal int[] Int32sValue
			{
				get
				{
					return this.m_int32sValue;
				}
			}

			internal long Int64Value
			{
				get
				{
					return this.m_int64Value;
				}
			}

			internal ushort UInt16Value
			{
				get
				{
					return this.m_uint16Value;
				}
			}

			internal uint UInt32Value
			{
				get
				{
					return this.m_uint32Value;
				}
			}

			internal ulong UInt64Value
			{
				get
				{
					return this.m_uint64Value;
				}
			}

			internal byte ByteValue
			{
				get
				{
					return this.m_byteValue;
				}
			}

			internal byte[] BytesValue
			{
				get
				{
					return this.m_bytesValue;
				}
			}

			internal sbyte SByteValue
			{
				get
				{
					return this.m_sbyteValue;
				}
			}

			internal float SingleValue
			{
				get
				{
					return this.m_singleValue;
				}
			}

			internal double DoubleValue
			{
				get
				{
					return this.m_doubleValue;
				}
			}

			internal decimal DecimalValue
			{
				get
				{
					return this.m_decimalValue;
				}
			}

			internal DateTime DateTimeValue
			{
				get
				{
					return this.m_dateTimeValue;
				}
			}

			internal TimeSpan TimeSpanValue
			{
				get
				{
					return this.m_timeSpanValue;
				}
			}

			internal Guid GuidValue
			{
				get
				{
					return this.m_guidValue;
				}
			}

			internal DataFieldStatus DataFieldInfo
			{
				get
				{
					return (DataFieldStatus)this.m_enumValue;
				}
			}

			internal ReportServerBinaryReader(Stream stream, DeclarationCallback declarationCallback)
			{
				IntermediateFormatReader.Assert(null != declarationCallback);
				this.m_binaryReader = new BinaryReaderWrapper(stream);
				this.m_declarationCallback = declarationCallback;
			}

			internal bool Read()
			{
				bool flag;
				for (flag = this.Advance(); flag && Token.Declaration == this.m_token; flag = this.Advance())
				{
				}
				return flag;
			}

			internal bool ReadNoTypeReference()
			{
				bool flag;
				for (flag = this.ReadNoTypeReferenceAdvance(); flag && Token.Declaration == this.m_token; flag = this.ReadNoTypeReferenceAdvance())
				{
				}
				return flag;
			}

			internal void ReadDeclaration()
			{
				IntermediateFormatReader.Assert(this.Advance());
				IntermediateFormatReader.Assert(Token.Declaration == this.m_token);
			}

			private bool Advance()
			{
				try
				{
					this.m_objectType = ReportServerBinaryReader.ObjectTypeDefault;
					this.m_token = this.UnsafeReadToken();
					switch (this.m_token)
					{
					case Token.Object:
						this.m_objectType = this.UnsafeReadObjectType();
						break;
					case Token.Reference:
						this.m_objectType = this.UnsafeReadObjectType();
						this.m_referenceValue = this.m_binaryReader.ReadInt32();
						break;
					case Token.Enum:
						this.m_enumValue = this.m_binaryReader.Read7BitEncodedInt();
						break;
					case Token.TypedArray:
					{
						this.m_arrayType = this.ReadToken();
						int num2 = this.m_binaryReader.Read7BitEncodedInt();
						if (Token.Byte == this.m_arrayType)
						{
							this.m_bytesValue = this.m_binaryReader.ReadBytes(num2);
						}
						else if (Token.Int32 == this.m_arrayType)
						{
							this.m_int32sValue = new int[num2];
							for (int j = 0; j < num2; j++)
							{
								this.m_int32sValue[j] = this.m_binaryReader.ReadInt32();
							}
						}
						else if (Token.Single == this.m_arrayType)
						{
							this.m_floatsValue = new float[num2];
							for (int k = 0; k < num2; k++)
							{
								this.m_floatsValue[k] = this.m_binaryReader.ReadSingle();
							}
						}
						else
						{
							IntermediateFormatReader.Assert(Token.Char == this.m_arrayType);
							this.m_charsValue = this.m_binaryReader.ReadChars(num2);
						}
						break;
					}
					case Token.Array:
						this.m_arrayLength = this.m_binaryReader.Read7BitEncodedInt();
						break;
					case Token.Declaration:
					{
						ObjectType objectType = this.ReadObjectType();
						ObjectType baseType = this.ReadObjectType();
						int num = this.m_binaryReader.Read7BitEncodedInt();
						MemberInfoList memberInfoList = new MemberInfoList(num);
						for (int i = 0; i < num; i++)
						{
							memberInfoList.Add(new MemberInfo(this.ReadMemberName(), this.ReadToken(), this.ReadObjectType()));
						}
						Declaration declaration = new Declaration(baseType, memberInfoList);
						this.m_declarationCallback(objectType, declaration);
						break;
					}
					case Token.Guid:
					{
						byte[] array = this.m_binaryReader.ReadBytes(16);
						IntermediateFormatReader.Assert(null != array);
						IntermediateFormatReader.Assert(16 == array.Length);
						this.m_guidValue = new Guid(array);
						break;
					}
					case Token.String:
						this.m_stringValue = this.m_binaryReader.ReadString();
						break;
					case Token.DateTime:
						this.m_dateTimeValue = new DateTime(this.m_binaryReader.ReadInt64());
						break;
					case Token.TimeSpan:
						this.m_timeSpanValue = new TimeSpan(this.m_binaryReader.ReadInt64());
						break;
					case Token.Char:
						this.m_charValue = this.m_binaryReader.ReadChar();
						break;
					case Token.Boolean:
						this.m_booleanValue = this.m_binaryReader.ReadBoolean();
						break;
					case Token.Int16:
						this.m_int16Value = this.m_binaryReader.ReadInt16();
						break;
					case Token.Int32:
						this.m_int32Value = this.m_binaryReader.ReadInt32();
						break;
					case Token.Int64:
						this.m_int64Value = this.m_binaryReader.ReadInt64();
						break;
					case Token.UInt16:
						this.m_uint16Value = this.m_binaryReader.ReadUInt16();
						break;
					case Token.UInt32:
						this.m_uint32Value = this.m_binaryReader.ReadUInt32();
						break;
					case Token.UInt64:
						this.m_uint64Value = this.m_binaryReader.ReadUInt64();
						break;
					case Token.Byte:
						this.m_byteValue = this.m_binaryReader.ReadByte();
						break;
					case Token.SByte:
						this.m_sbyteValue = this.m_binaryReader.ReadSByte();
						break;
					case Token.Single:
						this.m_singleValue = this.m_binaryReader.ReadSingle();
						break;
					case Token.Double:
						this.m_doubleValue = this.m_binaryReader.ReadDouble();
						break;
					case Token.Decimal:
						this.m_decimalValue = this.m_binaryReader.ReadDecimal();
						break;
					case Token.DataFieldInfo:
						this.m_enumValue = this.m_binaryReader.Read7BitEncodedInt();
						break;
					default:
						IntermediateFormatReader.Assert(false);
						return false;
					case Token.Null:
					case Token.EndObject:
						break;
					}
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}

			internal bool ReadNoTypeReferenceAdvance()
			{
				try
				{
					this.m_token = this.UnsafeReadToken();
					IntermediateFormatReader.Assert(Token.Reference == this.m_token || Token.Null == this.m_token);
					if (Token.Reference == this.m_token)
					{
						this.m_referenceValue = this.m_binaryReader.ReadInt32();
					}
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}

			internal byte[] ReadBytes()
			{
				IntermediateFormatReader.Assert(this.Read());
				if (this.m_token == Token.Null)
				{
					return null;
				}
				IntermediateFormatReader.Assert(Token.TypedArray == this.m_token);
				IntermediateFormatReader.Assert(Token.Byte == this.m_arrayType);
				return this.m_bytesValue;
			}

			internal int[] ReadInt32s()
			{
				IntermediateFormatReader.Assert(this.Read());
				if (this.m_token == Token.Null)
				{
					return null;
				}
				IntermediateFormatReader.Assert(Token.TypedArray == this.m_token);
				IntermediateFormatReader.Assert(Token.Int32 == this.m_arrayType);
				return this.m_int32sValue;
			}

			internal float[] ReadFloatArray()
			{
				IntermediateFormatReader.Assert(this.Read());
				if (this.m_token == Token.Null)
				{
					return null;
				}
				IntermediateFormatReader.Assert(Token.TypedArray == this.m_token);
				IntermediateFormatReader.Assert(Token.Single == this.m_arrayType);
				return this.m_floatsValue;
			}

			internal Guid ReadGuid()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Guid == this.m_token);
				return this.m_guidValue;
			}

			internal string ReadString()
			{
				IntermediateFormatReader.Assert(this.Read());
				if (this.m_token == Token.Null)
				{
					return null;
				}
				IntermediateFormatReader.Assert(Token.String == this.m_token);
				return this.m_stringValue;
			}

			internal int ReadInt32()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Int32 == this.m_token);
				return this.m_int32Value;
			}

			internal long ReadInt64()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Int64 == this.m_token);
				return this.m_int64Value;
			}

			internal double ReadDouble()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Double == this.m_token);
				return this.m_doubleValue;
			}

			internal bool ReadBoolean()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Boolean == this.m_token);
				return this.m_booleanValue;
			}

			internal DateTime ReadDateTime()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.DateTime == this.m_token);
				return this.m_dateTimeValue;
			}

			internal int ReadEnum()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Enum == this.m_token);
				return this.m_enumValue;
			}

			internal ObjectType ReadObject()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Object == this.m_token);
				return this.m_objectType;
			}

			internal void ReadEndObject()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.EndObject == this.m_token);
			}

			internal int ReadArray()
			{
				IntermediateFormatReader.Assert(this.Read());
				IntermediateFormatReader.Assert(Token.Array == this.m_token);
				return this.m_arrayLength;
			}

			private Token UnsafeReadToken()
			{
				return (Token)this.m_binaryReader.ReadByte();
			}

			private Token ReadToken()
			{
				return (Token)this.m_binaryReader.ReadByte();
			}

			private ObjectType UnsafeReadObjectType()
			{
				return (ObjectType)this.m_binaryReader.Read7BitEncodedInt();
			}

			private ObjectType ReadObjectType()
			{
				return (ObjectType)this.m_binaryReader.Read7BitEncodedInt();
			}

			private MemberName ReadMemberName()
			{
				return (MemberName)this.m_binaryReader.Read7BitEncodedInt();
			}
		}

		private ReportServerBinaryReader m_reader;

		private Hashtable m_definitionObjects;

		private Hashtable m_instanceObjects;

		private Hashtable m_parametersDef;

		private Hashtable m_parametersInfo;

		private Hashtable m_matrixHeadingInstanceObjects;

		private State m_state;

		private bool m_expectDeclarations;

		private Stack<GroupingList> m_groupingsWithHideDuplicatesStack;

		private IntermediateFormatVersion m_intermediateFormatVersion;

		private ArrayList m_textboxesWithUserSort;

		private int m_currentUniqueName = -1;

		internal IntermediateFormatVersion IntermediateFormatVersion
		{
			get
			{
				return this.m_intermediateFormatVersion;
			}
		}

		internal Hashtable DefinitionObjects
		{
			get
			{
				return this.m_definitionObjects;
			}
		}

		internal Hashtable InstanceObjects
		{
			get
			{
				return this.m_instanceObjects;
			}
		}

		internal Hashtable MatrixHeadingInstanceObjects
		{
			get
			{
				return this.m_matrixHeadingInstanceObjects;
			}
		}

		internal State ReaderState
		{
			get
			{
				return this.m_state;
			}
		}

		internal IntermediateFormatReader(Stream stream)
		{
			this.Initialize(stream);
			this.m_definitionObjects = null;
			this.m_instanceObjects = null;
			this.m_matrixHeadingInstanceObjects = null;
			this.m_state = new State();
			this.m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects)
		{
			this.Initialize(stream);
			this.m_definitionObjects = null;
			this.m_instanceObjects = instanceObjects;
			this.m_matrixHeadingInstanceObjects = null;
			this.m_state = new State();
			this.m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			this.Initialize(stream);
			this.m_intermediateFormatVersion = intermediateFormatVersion;
			this.m_definitionObjects = definitionObjects;
			this.m_instanceObjects = instanceObjects;
			this.m_matrixHeadingInstanceObjects = null;
			this.m_state = new State();
			this.m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			this.Initialize(stream);
			this.m_definitionObjects = null;
			this.m_instanceObjects = instanceObjects;
			this.m_intermediateFormatVersion = intermediateFormatVersion;
			this.m_matrixHeadingInstanceObjects = null;
			this.m_state = new State();
			this.m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, IntermediateFormatVersion intermediateFormatVersion)
		{
			this.Initialize(stream);
			this.m_definitionObjects = null;
			this.m_instanceObjects = null;
			this.m_intermediateFormatVersion = intermediateFormatVersion;
			this.m_matrixHeadingInstanceObjects = null;
			this.m_state = new State();
			this.m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, State state, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			this.Initialize(stream);
			this.m_definitionObjects = definitionObjects;
			this.m_instanceObjects = null;
			this.m_intermediateFormatVersion = intermediateFormatVersion;
			this.m_matrixHeadingInstanceObjects = null;
			if (state == null)
			{
				this.m_state = State.Current;
			}
			else
			{
				this.m_state = state;
			}
			this.m_expectDeclarations = false;
		}

		internal IntermediateFormatReader(Stream stream, State state, IntermediateFormatVersion intermediateFormatVersion)
		{
			this.Initialize(stream);
			this.m_definitionObjects = null;
			this.m_instanceObjects = null;
			this.m_intermediateFormatVersion = intermediateFormatVersion;
			this.m_matrixHeadingInstanceObjects = null;
			if (state == null)
			{
				this.m_state = State.Current;
			}
			else
			{
				this.m_state = state;
			}
			this.m_expectDeclarations = false;
		}

		private void Initialize(Stream stream)
		{
			IntermediateFormatReader.Assert(null != stream);
			this.m_reader = new ReportServerBinaryReader(stream, this.DeclarationCallback);
			IntermediateFormatReader.Assert(VersionStamp.Validate(this.m_reader.ReadBytes()));
		}

		internal IntermediateFormatVersion ReadIntermediateFormatVersion()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.IntermediateFormatVersion;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			IntermediateFormatVersion intermediateFormatVersion = new IntermediateFormatVersion();
			if (this.PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Major = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Minor = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Build = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return intermediateFormatVersion;
		}

		internal Report ReadReport(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Report;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			if (this.m_groupingsWithHideDuplicatesStack == null)
			{
				this.m_groupingsWithHideDuplicatesStack = new Stack<GroupingList>();
			}
			this.m_groupingsWithHideDuplicatesStack.Push(new GroupingList());
			if (this.m_textboxesWithUserSort == null)
			{
				this.m_textboxesWithUserSort = new ArrayList();
			}
			this.m_textboxesWithUserSort.Add(new TextBoxList());
			if (this.PreRead(objectType, indexes))
			{
				this.m_intermediateFormatVersion = this.ReadIntermediateFormatVersion();
			}
			if (this.m_intermediateFormatVersion == null)
			{
				this.m_intermediateFormatVersion = new IntermediateFormatVersion(8, 0, 673);
			}
			Guid guid = Guid.Empty;
			if (this.PreRead(objectType, indexes))
			{
				guid = this.m_reader.ReadGuid();
			}
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
			}
			Report report = new Report(parent, this.m_intermediateFormatVersion, guid);
			this.ReadReportItemBase(report);
			if (this.PreRead(objectType, indexes))
			{
				report.Author = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.AutoRefresh = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.EmbeddedImages = this.ReadEmbeddedImageHashtable();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageHeader = this.ReadPageSection(true, report);
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageFooter = this.ReadPageSection(false, report);
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ReportItems = this.ReadReportItemCollection(report);
			}
			if (this.PreRead(objectType, indexes))
			{
				report.DataSources = this.ReadDataSourceList();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageHeight = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageHeightValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageWidth = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageWidthValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.LeftMargin = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.LeftMarginValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.RightMargin = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.RightMarginValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.TopMargin = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.TopMarginValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.BottomMargin = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.BottomMarginValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.Columns = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ColumnSpacing = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ColumnSpacingValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.CompiledCode = this.m_reader.ReadBytes();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.MergeOnePass = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.PageMergeOnePass = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.SubReportMergeTransactions = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.NeedPostGroupProcessing = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasPostSortAggregates = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasReportItemReferences = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ShowHideType = this.ReadShowHideTypes();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ImageStreamNames = this.ReadImageStreamNames();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.LastID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.BodyID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.SubReports = this.ReadSubReportList();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasImageStreams = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasLabels = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasBookmarks = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.ParametersNotUsedInQuery = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.Parameters = this.ReadParameterDefList();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.OneDataSetName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.CodeModules = this.ReadStringList();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.CodeClasses = this.ReadCodeClassList();
			}
			if (this.PreRead(objectType, indexes) && this.m_intermediateFormatVersion.IsRS2000_WithSpecialRecursiveAggregates)
			{
				report.HasSpecialRecursiveAggregates = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.Language = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.DataTransform = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.DataSchema = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.DataElementStyleAttribute = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.Code = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.HasUserSortFilter = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.CompiledCodeGeneratedWithRefusedPermissions = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.InteractiveHeight = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.InteractiveHeightValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.InteractiveWidth = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.InteractiveWidthValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.NonDetailSortFiltersInScope = this.ReadInScopeSortFilterTable();
			}
			if (this.PreRead(objectType, indexes))
			{
				report.DetailSortFiltersInScope = this.ReadInScopeSortFilterTable();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			this.ResolveReportItemReferenceForGroupings(this.m_groupingsWithHideDuplicatesStack.Pop());
			this.ResolveUserSortReferenceForTextBoxes();
			return report;
		}

		private void ResolveReportItemReferenceForGroupings(GroupingList groupingsWithHideDuplicates)
		{
			if (groupingsWithHideDuplicates != null)
			{
				for (int i = 0; i < groupingsWithHideDuplicates.Count; i++)
				{
					Grouping grouping = groupingsWithHideDuplicates[i];
					IntList hideDuplicatesReportItemIDs = grouping.HideDuplicatesReportItemIDs;
					Global.Tracer.Assert(null != hideDuplicatesReportItemIDs, "(null != reportItemIDs)");
					for (int j = 0; j < hideDuplicatesReportItemIDs.Count; j++)
					{
						IDOwner definitionObject = this.GetDefinitionObject(hideDuplicatesReportItemIDs[j]);
						IntermediateFormatReader.Assert(definitionObject is ReportItem);
						grouping.AddReportItemWithHideDuplicates((ReportItem)definitionObject);
					}
					grouping.HideDuplicatesReportItemIDs = null;
				}
			}
		}

		private void ResolveUserSortReferenceForTextBoxes()
		{
			Global.Tracer.Assert(this.m_textboxesWithUserSort != null && 0 < this.m_textboxesWithUserSort.Count && null != this.m_textboxesWithUserSort[this.m_textboxesWithUserSort.Count - 1]);
			TextBoxList textBoxList = (TextBoxList)this.m_textboxesWithUserSort[this.m_textboxesWithUserSort.Count - 1];
			for (int i = 0; i < textBoxList.Count; i++)
			{
				ISortFilterScope sortFilterScope = null;
				TextBox textBox = textBoxList[i];
				if (-1 != textBox.UserSort.SortExpressionScopeID)
				{
					IDOwner definitionObject = this.GetDefinitionObject(textBox.UserSort.SortExpressionScopeID);
					sortFilterScope = (definitionObject as ISortFilterScope);
					if (sortFilterScope == null)
					{
						IntermediateFormatReader.Assert(definitionObject is ReportHierarchyNode);
						sortFilterScope = ((ReportHierarchyNode)definitionObject).Grouping;
					}
					textBox.UserSort.SortExpressionScope = sortFilterScope;
					textBox.UserSort.SortExpressionScopeID = -1;
				}
				IntList groupInSortTargetIDs = textBox.UserSort.GroupInSortTargetIDs;
				if (groupInSortTargetIDs != null)
				{
					textBox.UserSort.GroupsInSortTarget = new GroupingList(groupInSortTargetIDs.Count);
					for (int j = 0; j < groupInSortTargetIDs.Count; j++)
					{
						IDOwner definitionObject2 = this.GetDefinitionObject(groupInSortTargetIDs[j]);
						IntermediateFormatReader.Assert(definitionObject2 is ReportHierarchyNode);
						textBox.UserSort.GroupsInSortTarget.Add(((ReportHierarchyNode)definitionObject2).Grouping);
					}
					textBox.UserSort.GroupInSortTargetIDs = null;
				}
				if (-1 != textBox.UserSort.SortTargetID)
				{
					IDOwner definitionObject3 = this.GetDefinitionObject(textBox.UserSort.SortTargetID);
					sortFilterScope = (definitionObject3 as ISortFilterScope);
					if (sortFilterScope == null)
					{
						IntermediateFormatReader.Assert(definitionObject3 is ReportHierarchyNode);
						sortFilterScope = ((ReportHierarchyNode)definitionObject3).Grouping;
					}
					textBox.UserSort.SortTarget = sortFilterScope;
					textBox.UserSort.SortTargetID = -1;
				}
			}
			this.m_textboxesWithUserSort.RemoveAt(this.m_textboxesWithUserSort.Count - 1);
		}

		internal ReportSnapshot ReadReportSnapshot()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportSnapshot reportSnapshot = new ReportSnapshot();
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.ExecutionTime = this.m_reader.ReadDateTime();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.Report = this.ReadReport(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.Parameters = this.ReadParameterInfoCollection();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.ReportInstance = this.ReadReportInstance(reportSnapshot.Report);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.HasDocumentMap = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.HasShowHide = this.m_reader.ReadBoolean();
			}
			if (this.m_intermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				if (this.PreRead(objectType, indexes))
				{
					reportSnapshot.HasBookmarks = this.m_reader.ReadBoolean();
				}
			}
			else
			{
				OffsetInfo offsetInfo = this.ReadOffsetInfo();
				if (offsetInfo != null)
				{
					reportSnapshot.DocumentMapOffset = offsetInfo;
					reportSnapshot.HasDocumentMap = true;
				}
				offsetInfo = this.ReadOffsetInfo();
				if (offsetInfo != null)
				{
					reportSnapshot.ShowHideSenderInfoOffset = offsetInfo;
					reportSnapshot.HasShowHide = true;
				}
				reportSnapshot.ShowHideReceiverInfoOffset = this.ReadOffsetInfo();
				reportSnapshot.QuickFindOffset = this.ReadOffsetInfo();
				indexes.CurrentIndex++;
				reportSnapshot.HasBookmarks = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.HasImageStreams = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.RequestUserName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.ReportServerUrl = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.ReportFolder = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.Language = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.Warnings = this.ReadProcessingMessageList();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportSnapshot.PageSectionOffsets = this.ReadInt64List();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportSnapshot;
		}

		internal Report ReadReportFromSnapshot(out DateTime executionTime)
		{
			executionTime = DateTime.Now;
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Report result = null;
			if (this.PreRead(objectType, indexes))
			{
				executionTime = this.m_reader.ReadDateTime();
			}
			if (this.PreRead(objectType, indexes))
			{
				result = this.ReadReport(null);
			}
			return result;
		}

		internal ParameterInfoCollection ReadSnapshotParameters()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ParameterInfoCollection result = null;
			if (this.PreRead(objectType, indexes))
			{
				this.m_reader.ReadDateTime();
			}
			if (this.PreRead(objectType, indexes))
			{
				this.ReadReport(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				result = this.ReadParameterInfoCollection();
			}
			return result;
		}

		internal DocumentMapNode ReadDocumentMapNode()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			this.ReadInstanceInfoBase(documentMapNode);
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Id = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Page = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Children = this.ReadDocumentMapNodes();
			}
			if (this.m_intermediateFormatVersion != null && !this.m_intermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				documentMapNode.Page = this.m_reader.ReadInt32();
				indexes.CurrentIndex++;
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return documentMapNode;
		}

		internal DocumentMapNodeInfo ReadDocumentMapNodeInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			this.ReadInstanceInfoBase(documentMapNode);
			DocumentMapNodeInfo[] children = null;
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Id = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				children = this.ReadDocumentMapNodesInfo();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return new DocumentMapNodeInfo(documentMapNode, children);
		}

		internal bool FindDocumentMapNodePage(string documentMapId, ref int page)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return false;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			this.ReadInstanceInfoBase(documentMapNode);
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Id = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				documentMapNode.Page = this.m_reader.ReadInt32();
			}
			if (documentMapId.Equals(documentMapNode.Id, StringComparison.Ordinal))
			{
				page = documentMapNode.Page + 1;
				return true;
			}
			documentMapNode = null;
			bool flag = false;
			if (this.PreRead(objectType, indexes))
			{
				flag = this.FindDocumentMapNodesPage(documentMapId, ref page);
			}
			if (!flag)
			{
				this.PostRead(objectType, indexes);
				this.m_reader.ReadEndObject();
			}
			return flag;
		}

		internal TokensHashtable ReadTokensHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TokensHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			TokensHashtable tokensHashtable = new TokensHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int tokenID = this.m_reader.ReadInt32();
				object tokenValue = this.ReadVariant();
				tokensHashtable.Add(tokenID, tokenValue);
			}
			this.m_reader.ReadEndObject();
			return tokensHashtable;
		}

		internal BookmarksHashtable ReadBookmarksHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.BookmarksHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			BookmarksHashtable bookmarksHashtable = new BookmarksHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string bookmark = this.m_reader.ReadString();
				BookmarkInformation bookmarkInfo = this.ReadBookmarkInformation();
				bookmarksHashtable.Add(bookmark, bookmarkInfo);
			}
			this.m_reader.ReadEndObject();
			return bookmarksHashtable;
		}

		internal BookmarkInformation FindBookmarkIdInfo(string bookmarkId)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.BookmarksHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			for (int i = 0; i < num; i++)
			{
				string value = this.m_reader.ReadString();
				BookmarkInformation result = this.ReadBookmarkInformation();
				if (bookmarkId.Equals(value, StringComparison.Ordinal))
				{
					return result;
				}
			}
			return null;
		}

		internal DrillthroughInformation FindDrillthroughIdInfo(string drillthroughId)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			bool flag = false;
			TokensHashtable dataSetTokenIDs = null;
			if (ObjectType.ReportDrillthroughInfo == this.m_reader.ObjectType)
			{
				flag = true;
				dataSetTokenIDs = this.ReadTokensHashtable();
				IntermediateFormatReader.Assert(this.m_reader.Read());
				if (this.m_reader.Token == Token.Null)
				{
					return null;
				}
			}
			IntermediateFormatReader.Assert(ObjectType.DrillthroughHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			for (int i = 0; i < num; i++)
			{
				string value = this.m_reader.ReadString();
				DrillthroughInformation drillthroughInformation = this.ReadDrillthroughInformation(flag);
				if (drillthroughId.Equals(value, StringComparison.Ordinal))
				{
					if (flag)
					{
						drillthroughInformation.ResolveDataSetTokenIDs(dataSetTokenIDs);
					}
					return drillthroughInformation;
				}
			}
			return null;
		}

		internal SenderInformationHashtable ReadSenderInformationHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.SenderInformationHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			SenderInformationHashtable senderInformationHashtable = new SenderInformationHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = this.m_reader.ReadInt32();
				SenderInformation sender = this.ReadSenderInformation();
				senderInformationHashtable.Add(key, sender);
			}
			this.m_reader.ReadEndObject();
			return senderInformationHashtable;
		}

		internal ReceiverInformationHashtable ReadReceiverInformationHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ReceiverInformationHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ReceiverInformationHashtable receiverInformationHashtable = new ReceiverInformationHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = this.m_reader.ReadInt32();
				ReceiverInformation receiver = this.ReadReceiverInformation();
				receiverInformationHashtable.Add(key, receiver);
			}
			this.m_reader.ReadEndObject();
			return receiverInformationHashtable;
		}

		internal QuickFindHashtable ReadQuickFindHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.QuickFindHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			QuickFindHashtable quickFindHashtable = new QuickFindHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = this.m_reader.ReadInt32();
				ReportItemInstance val = this.ReadReportItemInstanceReference();
				quickFindHashtable.Add(key, val);
			}
			this.m_reader.ReadEndObject();
			return quickFindHashtable;
		}

		internal SortFilterEventInfoHashtable ReadSortFilterEventInfoHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.SortFilterEventInfoHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			SortFilterEventInfoHashtable sortFilterEventInfoHashtable = new SortFilterEventInfoHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = this.m_reader.ReadInt32();
				SortFilterEventInfo val = this.ReadSortFilterEventInfo(true);
				sortFilterEventInfoHashtable.Add(key, val);
			}
			this.m_reader.ReadEndObject();
			return sortFilterEventInfoHashtable;
		}

		internal List<PageSectionInstance> ReadPageSections(int requestedPageNumber, int startPage, PageSection headerDef, PageSection footerDef)
		{
			IntermediateFormatReader.Assert(startPage >= 0);
			List<PageSectionInstance> list = null;
			int num = (requestedPageNumber + 1) * 2;
			int num2 = 2;
			if (startPage == 0)
			{
				IntermediateFormatReader.Assert(this.m_reader.Read());
				if (this.m_reader.Token == Token.Null)
				{
					return null;
				}
				IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
				IntermediateFormatReader.Assert(ObjectType.PageSectionInstanceList == this.m_reader.ObjectType);
				num2 = this.m_reader.ReadArray();
				if (requestedPageNumber < 0)
				{
					num = num2;
				}
				IntermediateFormatReader.Assert(num2 % 2 == 0);
			}
			list = new List<PageSectionInstance>((requestedPageNumber < 0) ? num2 : 2);
			for (int i = startPage * 2; i < num; i++)
			{
				PageSection pageSectionDef = (i % 2 == 0) ? headerDef : footerDef;
				PageSectionInstance item = this.ReadPageSectionInstance(pageSectionDef);
				if (requestedPageNumber < 0)
				{
					list.Add(item);
				}
				else if (requestedPageNumber == i >> 1)
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal ActionInstance ReadActionInstance(Action action)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ActionInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ActionInstance actionInstance = new ActionInstance();
			if (this.PreRead(objectType, indexes))
			{
				ActionItemList actionItemList = null;
				if (action != null)
				{
					actionItemList = action.ActionItems;
				}
				actionInstance.ActionItemsValues = this.ReadActionItemInstanceList(actionItemList);
			}
			if (this.PreRead(objectType, indexes))
			{
				actionInstance.StyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				actionInstance.UniqueName = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return actionInstance;
		}

		private ActionItemInstanceList ReadActionItemInstanceList(ActionItemList actionItemList)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ActionItemInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ActionItemInstanceList actionItemInstanceList = new ActionItemInstanceList(num);
			ActionItem actionItem = null;
			for (int i = 0; i < num; i++)
			{
				actionItem = null;
				if (actionItemList != null)
				{
					actionItem = actionItemList[i];
				}
				actionItemInstanceList.Add(this.ReadActionItemInstance(actionItem));
			}
			this.m_reader.ReadEndObject();
			return actionItemInstanceList;
		}

		internal ActionItemInstance ReadActionItemInstance(ActionItem actionItemDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ActionItemInstance actionItemInstance = new ActionItemInstance();
			if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
			{
				ObjectType objectType = ObjectType.ActionItemInstance;
				Indexes indexes = new Indexes();
				IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
				IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.HyperLinkURL = this.m_reader.ReadString();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.BookmarkLink = this.m_reader.ReadString();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughReportName = this.m_reader.ReadString();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughParametersValues = this.ReadVariants(true, true);
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughParametersOmits = this.ReadBoolList();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItemInstance.Label = this.m_reader.ReadString();
				}
				if (actionItemDef != null && this.m_intermediateFormatVersion.IsRS2005_WithSharedDrillthroughParams)
				{
					ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
					if (drillthroughParameters != null && drillthroughParameters.Count > 0)
					{
						ExpressionInfo expressionInfo = null;
						DataSet dataSet = null;
						for (int i = 0; i < drillthroughParameters.Count; i++)
						{
							expressionInfo = drillthroughParameters[i].Value;
							if (expressionInfo != null && expressionInfo.Type == ExpressionInfo.Types.Token)
							{
								dataSet = (this.m_definitionObjects[expressionInfo.IntValue] as DataSet);
								if (dataSet != null && dataSet.Query != null)
								{
									actionItemInstance.DrillthroughParametersValues[i] = dataSet.Query.RewrittenCommandText;
								}
							}
						}
					}
				}
				this.PostRead(objectType, indexes);
			}
			else
			{
				ObjectType objectType2 = ObjectType.ActionInstance;
				IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
				IntermediateFormatReader.Assert(objectType2 == this.m_reader.ObjectType);
				actionItemInstance.HyperLinkURL = this.m_reader.ReadString();
				actionItemInstance.BookmarkLink = this.m_reader.ReadString();
				actionItemInstance.DrillthroughReportName = this.m_reader.ReadString();
				actionItemInstance.DrillthroughParametersValues = this.ReadVariants(true, true);
				actionItemInstance.DrillthroughParametersOmits = this.ReadBoolList();
			}
			this.m_reader.ReadEndObject();
			return actionItemInstance;
		}

		internal ReportItemColInstanceInfo ReadReportItemColInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemColInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportItemColInstanceInfo reportItemColInstanceInfo = new ReportItemColInstanceInfo();
			this.ReadInstanceInfoBase(reportItemColInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				reportItemColInstanceInfo.ChildrenNonComputedUniqueNames = this.ReadNonComputedUniqueNamess();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportItemColInstanceInfo;
		}

		internal ListContentInstanceInfo ReadListContentInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListContentInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ListContentInstanceInfo listContentInstanceInfo = new ListContentInstanceInfo();
			this.ReadInstanceInfoBase(listContentInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				listContentInstanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				listContentInstanceInfo.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				listContentInstanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return listContentInstanceInfo;
		}

		internal MatrixHeadingInstanceInfo ReadMatrixHeadingInstanceInfoBase()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.MatrixHeadingInstanceInfo == this.m_reader.ObjectType)
			{
				MatrixHeadingInstanceInfo matrixHeadingInstanceInfo = new MatrixHeadingInstanceInfo();
				this.ReadMatrixHeadingInstanceInfo(matrixHeadingInstanceInfo);
				return matrixHeadingInstanceInfo;
			}
			IntermediateFormatReader.Assert(ObjectType.MatrixSubtotalHeadingInstanceInfo == this.m_reader.ObjectType);
			MatrixSubtotalHeadingInstanceInfo matrixSubtotalHeadingInstanceInfo = new MatrixSubtotalHeadingInstanceInfo();
			this.ReadMatrixSubtotalHeadingInstanceInfo(matrixSubtotalHeadingInstanceInfo);
			return matrixSubtotalHeadingInstanceInfo;
		}

		internal void ReadMatrixHeadingInstanceInfo(MatrixHeadingInstanceInfo instanceInfo)
		{
			IntermediateFormatReader.Assert(Token.Null != this.m_reader.Token);
			ObjectType objectType = ObjectType.MatrixHeadingInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			this.ReadInstanceInfoBase(instanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.ContentUniqueNames = this.ReadNonComputedUniqueNames();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.HeadingCellIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.HeadingSpan = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.GroupExpressionValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				instanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			if (this.m_intermediateFormatVersion.IsRS2000_RTM_orNewer && this.m_intermediateFormatVersion.IsRS2005_IDW9_orOlder)
			{
				indexes.CurrentIndex++;
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
		}

		internal void ReadMatrixSubtotalHeadingInstanceInfo(MatrixSubtotalHeadingInstanceInfo instanceInfo)
		{
			IntermediateFormatReader.Assert(Token.Null != this.m_reader.Token);
			ObjectType objectType = ObjectType.MatrixSubtotalHeadingInstanceInfo;
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			this.ReadInstanceInfoBase(instanceInfo);
			IntermediateFormatReader.Assert(this.m_reader.Read());
			this.ReadMatrixHeadingInstanceInfo(instanceInfo);
			instanceInfo.StyleAttributeValues = this.ReadVariants();
			this.m_reader.ReadEndObject();
		}

		internal MatrixCellInstanceInfo ReadMatrixCellInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixCellInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixCellInstanceInfo matrixCellInstanceInfo = new MatrixCellInstanceInfo();
			this.ReadInstanceInfoBase(matrixCellInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.ContentUniqueNames = this.ReadNonComputedUniqueNames();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.RowIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.ColumnIndex = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixCellInstanceInfo;
		}

		internal ChartHeadingInstanceInfo ReadChartHeadingInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartHeadingInstanceInfo chartHeadingInstanceInfo = new ChartHeadingInstanceInfo();
			this.ReadInstanceInfoBase(chartHeadingInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingLabel = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingCellIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingSpan = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.GroupExpressionValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.StaticGroupingIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartHeadingInstanceInfo;
		}

		internal ChartDataPointInstanceInfo ReadChartDataPointInstanceInfo(ChartDataPointList chartDataPoints)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartDataPointInstanceInfo chartDataPointInstanceInfo = new ChartDataPointInstanceInfo();
			this.ReadInstanceInfoBase(chartDataPointInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataPointIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataLabelValue = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataLabelStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					ChartDataPoint chartDataPoint = chartDataPoints[chartDataPointInstanceInfo.DataPointIndex];
					chartDataPointInstanceInfo.Action = this.ReadActionInstance(chartDataPoint.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = this.ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						chartDataPointInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.StyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.MarkerStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartDataPointInstanceInfo;
		}

		internal TableGroupInstanceInfo ReadTableGroupInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableGroupInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableGroupInstanceInfo tableGroupInstanceInfo = new TableGroupInstanceInfo();
			this.ReadInstanceInfoBase(tableGroupInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableGroupInstanceInfo;
		}

		internal TableRowInstanceInfo ReadTableRowInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRowInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableRowInstanceInfo tableRowInstanceInfo = new TableRowInstanceInfo();
			this.ReadInstanceInfoBase(tableRowInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				tableRowInstanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableRowInstanceInfo;
		}

		internal LineInstanceInfo ReadLineInstanceInfo(Line line)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.LineInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			LineInstanceInfo lineInstanceInfo = new LineInstanceInfo(line);
			this.ReadReportItemInstanceInfoBase(lineInstanceInfo);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return lineInstanceInfo;
		}

		internal TextBoxInstanceInfo ReadTextBoxInstanceInfo(TextBox textBox)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TextBoxInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TextBoxInstanceInfo textBoxInstanceInfo = new TextBoxInstanceInfo(textBox);
			this.ReadReportItemInstanceInfoBase(textBoxInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.FormattedValue = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.OriginalValue = this.ReadVariant();
			}
			bool flag = false;
			if (this.m_intermediateFormatVersion.IsRS2000_WithUnusedFieldsOptimization)
			{
				flag = true;
			}
			if ((!flag || textBox.HideDuplicates != null) && this.PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.Duplicate = this.m_reader.ReadBoolean();
			}
			if ((!flag || textBox.Action != null) && this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					textBoxInstanceInfo.Action = this.ReadActionInstance(textBox.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = this.ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						textBoxInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if ((!flag || textBox.InitialToggleState != null) && this.PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.InitialToggleState = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return textBoxInstanceInfo;
		}

		internal SimpleTextBoxInstanceInfo ReadSimpleTextBoxInstanceInfo(TextBox textBox)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SimpleTextBoxInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo = new SimpleTextBoxInstanceInfo(textBox);
			this.ReadInstanceInfoBase(simpleTextBoxInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				simpleTextBoxInstanceInfo.FormattedValue = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				simpleTextBoxInstanceInfo.OriginalValue = this.ReadVariant();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return simpleTextBoxInstanceInfo;
		}

		internal RectangleInstanceInfo ReadRectangleInstanceInfo(Rectangle rectangle)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RectangleInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RectangleInstanceInfo rectangleInstanceInfo = new RectangleInstanceInfo(rectangle);
			this.ReadReportItemInstanceInfoBase(rectangleInstanceInfo);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return rectangleInstanceInfo;
		}

		internal CheckBoxInstanceInfo ReadCheckBoxInstanceInfo(CheckBox checkBox)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CheckBoxInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CheckBoxInstanceInfo checkBoxInstanceInfo = new CheckBoxInstanceInfo(checkBox);
			this.ReadReportItemInstanceInfoBase(checkBoxInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				checkBoxInstanceInfo.Value = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				checkBoxInstanceInfo.Duplicate = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return checkBoxInstanceInfo;
		}

		internal ImageInstanceInfo ReadImageInstanceInfo(Image image)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ImageInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ImageInstanceInfo imageInstanceInfo = new ImageInstanceInfo(image);
			this.ReadReportItemInstanceInfoBase(imageInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				imageInstanceInfo.ImageValue = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					imageInstanceInfo.Action = this.ReadActionInstance(image.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = this.ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						imageInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				imageInstanceInfo.BrokenImage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageInstanceInfo.ImageMapAreas = this.ReadImageMapAreaInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return imageInstanceInfo;
		}

		internal SubReportInstanceInfo ReadSubReportInstanceInfo(SubReport subReport)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SubReportInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SubReportInstanceInfo subReportInstanceInfo = new SubReportInstanceInfo(subReport);
			this.ReadReportItemInstanceInfoBase(subReportInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				subReportInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return subReportInstanceInfo;
		}

		internal ActiveXControlInstanceInfo ReadActiveXControlInstanceInfo(ActiveXControl activeXControl)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ActiveXControlInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ActiveXControlInstanceInfo activeXControlInstanceInfo = new ActiveXControlInstanceInfo(activeXControl);
			this.ReadReportItemInstanceInfoBase(activeXControlInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				activeXControlInstanceInfo.ParameterValues = this.ReadVariants();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return activeXControlInstanceInfo;
		}

		internal ListInstanceInfo ReadListInstanceInfo(List list)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ListInstanceInfo listInstanceInfo = new ListInstanceInfo(list);
			this.ReadReportItemInstanceInfoBase(listInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				listInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return listInstanceInfo;
		}

		internal MatrixInstanceInfo ReadMatrixInstanceInfo(Matrix matrix)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixInstanceInfo matrixInstanceInfo = new MatrixInstanceInfo(matrix);
			this.ReadReportItemInstanceInfoBase(matrixInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				matrixInstanceInfo.CornerNonComputedNames = this.ReadNonComputedUniqueNames();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixInstanceInfo;
		}

		internal TableInstanceInfo ReadTableInstanceInfo(Table table)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableInstanceInfo tableInstanceInfo = new TableInstanceInfo(table);
			this.ReadReportItemInstanceInfoBase(tableInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				tableInstanceInfo.ColumnInstances = this.ReadTableColumnInstances();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableInstanceInfo;
		}

		internal OWCChartInstanceInfo ReadOWCChartInstanceInfo(OWCChart chart)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.OWCChartInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			OWCChartInstanceInfo oWCChartInstanceInfo = new OWCChartInstanceInfo(chart);
			this.ReadReportItemInstanceInfoBase(oWCChartInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				oWCChartInstanceInfo.ChartData = this.ReadVariantLists(false);
			}
			if (this.PreRead(objectType, indexes))
			{
				oWCChartInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return oWCChartInstanceInfo;
		}

		internal ChartInstanceInfo ReadChartInstanceInfo(Chart chart)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartInstanceInfo chartInstanceInfo = new ChartInstanceInfo(chart);
			this.ReadReportItemInstanceInfoBase(chartInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.CategoryAxis = this.ReadAxisInstance();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.ValueAxis = this.ReadAxisInstance();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.Title = this.ReadChartTitleInstance();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.PlotAreaStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.LegendStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.CultureName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartInstanceInfo.NoRows = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartInstanceInfo;
		}

		internal CustomReportItemInstanceInfo ReadCustomReportItemInstanceInfo(CustomReportItem cri)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItemInstanceInfo customReportItemInstanceInfo = new CustomReportItemInstanceInfo(cri);
			this.ReadReportItemInstanceInfoBase(customReportItemInstanceInfo);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItemInstanceInfo;
		}

		internal PageSectionInstanceInfo ReadPageSectionInstanceInfo(PageSection pageSectionDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSectionInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			PageSectionInstanceInfo pageSectionInstanceInfo = new PageSectionInstanceInfo(pageSectionDef);
			this.ReadReportItemInstanceInfoBase(pageSectionInstanceInfo);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return pageSectionInstanceInfo;
		}

		internal ReportInstanceInfo ReadReportInstanceInfo(Report report)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportInstanceInfo reportInstanceInfo = new ReportInstanceInfo(report);
			this.ReadReportItemInstanceInfoBase(reportInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				reportInstanceInfo.Parameters = this.ReadParameterInfoCollection();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportInstanceInfo.ReportName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportInstanceInfo.NoRows = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportInstanceInfo.BodyUniqueName = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportInstanceInfo;
		}

		internal RecordSetInfo ReadRecordSetInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordSetInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RecordSetInfo recordSetInfo = new RecordSetInfo();
			if (this.PreRead(objectType, indexes))
			{
				recordSetInfo.ReaderExtensionsSupported = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				recordSetInfo.FieldPropertyNames = this.ReadRecordSetPropertyNamesList();
			}
			if (this.PreRead(objectType, indexes))
			{
				recordSetInfo.CompareOptions = this.ReadCompareOptions();
				recordSetInfo.ValidCompareOptions = true;
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return recordSetInfo;
		}

		private CompareOptions ReadCompareOptions()
		{
			return (CompareOptions)this.m_reader.ReadEnum();
		}

		private RecordSetPropertyNamesList ReadRecordSetPropertyNamesList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.RecordSetPropertyNamesList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			RecordSetPropertyNamesList recordSetPropertyNamesList = new RecordSetPropertyNamesList(num);
			for (int i = 0; i < num; i++)
			{
				recordSetPropertyNamesList.Add(this.ReadRecordSetPropertyNames());
			}
			this.m_reader.ReadEndObject();
			return recordSetPropertyNamesList;
		}

		internal RecordSetPropertyNames ReadRecordSetPropertyNames()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordSetPropertyNames;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RecordSetPropertyNames recordSetPropertyNames = new RecordSetPropertyNames();
			if (this.PreRead(objectType, indexes))
			{
				recordSetPropertyNames.PropertyNames = this.ReadStringList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return recordSetPropertyNames;
		}

		internal RecordRow ReadRecordRow()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordRow;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RecordRow recordRow = new RecordRow();
			if (this.PreRead(objectType, indexes))
			{
				recordRow.RecordFields = this.ReadRecordFields();
			}
			if (this.PreRead(objectType, indexes))
			{
				recordRow.IsAggregateRow = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				recordRow.AggregationFieldCount = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return recordRow;
		}

		private static void Assert(bool condition)
		{
			if (condition)
			{
				return;
			}
			Global.Tracer.Assert(false);
			throw new ReportProcessingException(ErrorCode.rsUnexpectedError);
		}

		private void RegisterParameterDef(ParameterDef paramDef)
		{
			IntermediateFormatReader.Assert(null != paramDef);
			if (this.m_parametersDef == null)
			{
				this.m_parametersDef = new Hashtable();
			}
			else
			{
				IntermediateFormatReader.Assert(!this.m_parametersDef.ContainsKey(paramDef.Name));
			}
			this.m_parametersDef.Add(paramDef.Name, paramDef);
		}

		private ParameterDef GetParameterDefObject(string name)
		{
			IntermediateFormatReader.Assert(null != this.m_parametersDef);
			ParameterDef parameterDef = (ParameterDef)this.m_parametersDef[name];
			IntermediateFormatReader.Assert(null != parameterDef);
			return parameterDef;
		}

		private void RegisterParameterInfo(ParameterInfo paramInfo)
		{
			IntermediateFormatReader.Assert(null != paramInfo);
			if (this.m_parametersInfo == null)
			{
				this.m_parametersInfo = new Hashtable();
			}
			else
			{
				IntermediateFormatReader.Assert(!this.m_parametersInfo.ContainsKey(paramInfo.Name));
			}
			this.m_parametersInfo.Add(paramInfo.Name, paramInfo);
		}

		private ParameterInfo GetParameterInfoObject(string name)
		{
			IntermediateFormatReader.Assert(null != this.m_parametersInfo);
			ParameterInfo parameterInfo = (ParameterInfo)this.m_parametersInfo[name];
			IntermediateFormatReader.Assert(null != parameterInfo);
			return parameterInfo;
		}

		private void RegisterDefinitionObject(IDOwner idOwner)
		{
			IntermediateFormatReader.Assert(null != idOwner);
			if (this.m_definitionObjects == null)
			{
				this.m_definitionObjects = new Hashtable();
			}
			else
			{
				IntermediateFormatReader.Assert(!this.m_definitionObjects.ContainsKey(idOwner.ID));
			}
			this.m_definitionObjects.Add(idOwner.ID, idOwner);
		}

		private IDOwner GetDefinitionObject(int id)
		{
			IntermediateFormatReader.Assert(null != this.m_definitionObjects);
			IDOwner iDOwner = (IDOwner)this.m_definitionObjects[id];
			IntermediateFormatReader.Assert(null != iDOwner);
			return iDOwner;
		}

		private void RegisterInstanceObject(ReportItemInstance reportItemInstance)
		{
			IntermediateFormatReader.Assert(null != reportItemInstance);
			if (this.m_instanceObjects == null)
			{
				this.m_instanceObjects = new Hashtable();
			}
			else
			{
				IntermediateFormatReader.Assert(!this.m_instanceObjects.ContainsKey(reportItemInstance.UniqueName));
			}
			this.m_instanceObjects.Add(reportItemInstance.UniqueName, reportItemInstance);
		}

		private ReportItemInstance GetInstanceObject(int uniqueName)
		{
			IntermediateFormatReader.Assert(null != this.m_instanceObjects);
			ReportItemInstance reportItemInstance = (ReportItemInstance)this.m_instanceObjects[uniqueName];
			IntermediateFormatReader.Assert(null != reportItemInstance);
			return reportItemInstance;
		}

		private void RegisterMatrixHeadingInstanceObject(MatrixHeadingInstance matrixHeadingInstance)
		{
			IntermediateFormatReader.Assert(null != matrixHeadingInstance);
			if (this.m_matrixHeadingInstanceObjects == null)
			{
				this.m_matrixHeadingInstanceObjects = new Hashtable();
			}
			else
			{
				IntermediateFormatReader.Assert(!this.m_matrixHeadingInstanceObjects.ContainsKey(matrixHeadingInstance.UniqueName));
			}
			this.m_matrixHeadingInstanceObjects.Add(matrixHeadingInstance.UniqueName, matrixHeadingInstance);
		}

		private MatrixHeadingInstance GetMatrixHeadingInstanceObject(int uniqueName)
		{
			IntermediateFormatReader.Assert(null != this.m_matrixHeadingInstanceObjects);
			MatrixHeadingInstance matrixHeadingInstance = (MatrixHeadingInstance)this.m_matrixHeadingInstanceObjects[uniqueName];
			IntermediateFormatReader.Assert(null != matrixHeadingInstance);
			return matrixHeadingInstance;
		}

		private void DeclarationCallback(ObjectType objectType, Declaration declaration)
		{
			IntermediateFormatReader.Assert(this.m_expectDeclarations);
			IntermediateFormatReader.Assert(null != declaration);
			IntermediateFormatReader.Assert(null != declaration.Members);
			bool flag = false;
			if (this.m_intermediateFormatVersion != null && !this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations && ObjectType.TableGroupInstance == objectType)
			{
				flag = true;
			}
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < declaration.Members.Count; i++)
			{
				MemberInfo memberInfo = declaration.Members[i];
				if (flag && memberInfo.MemberName == MemberName.DetailRowInstances)
				{
					memberInfo.MemberName = MemberName.TableDetailInstances;
				}
				hashtable[memberInfo.MemberName] = i;
			}
			Declaration declaration2 = DeclarationList.Current[objectType];
			IntermediateFormatReader.Assert(null != declaration2);
			IntermediateFormatReader.Assert(null != declaration2.Members);
			int lastIndex = -1;
			bool[] array = new bool[declaration2.Members.Count];
			IntList[] array2 = new IntList[declaration2.Members.Count + 1];
			for (int j = 0; j < declaration2.Members.Count; j++)
			{
				if (hashtable.ContainsKey(declaration2.Members[j].MemberName))
				{
					int num = (int)hashtable[declaration2.Members[j].MemberName];
					bool flag2 = MemberInfo.Equals(declaration2.Members[j], declaration.Members[num]);
					if (!flag2)
					{
						if (declaration2.Members[j].ObjectType == ObjectType.ExpressionInfo)
						{
							flag2 = (declaration.Members[num].ObjectType == ObjectType.None && (declaration.Members[num].Token == Token.String || declaration.Members[num].Token == Token.Boolean || declaration.Members[num].Token == Token.Int32));
						}
						if (!flag2 && objectType == ObjectType.Axis && declaration2.Members[j].ObjectType == ObjectType.ExpressionInfo)
						{
							flag2 = (declaration.Members[num].ObjectType == ObjectType.Variant);
						}
					}
					if (flag2)
					{
						array[j] = true;
						array2[j] = this.CreateOldIndexesToSkip(num, lastIndex);
						lastIndex = num;
					}
				}
			}
			array2[declaration2.Members.Count] = this.CreateOldIndexesToSkip(declaration.Members.Count, lastIndex);
			IntermediateFormatReader.Assert(null == this.m_state.OldDeclarations[objectType]);
			IntermediateFormatReader.Assert(null == this.m_state.IsInOldDeclaration[(int)objectType]);
			IntermediateFormatReader.Assert(null == this.m_state.OldIndexesToSkip[(int)objectType]);
			this.m_state.OldDeclarations[objectType] = declaration;
			this.m_state.IsInOldDeclaration[(int)objectType] = array;
			this.m_state.OldIndexesToSkip[(int)objectType] = array2;
		}

		private IntList CreateOldIndexesToSkip(int index, int lastIndex)
		{
			IntermediateFormatReader.Assert(index > lastIndex);
			IntList intList = null;
			if (index - lastIndex > 1)
			{
				intList = new IntList();
				for (int i = lastIndex + 1; i < index; i++)
				{
					intList.Add(i);
				}
			}
			return intList;
		}

		private bool PreRead(ObjectType objectType, Indexes indexes)
		{
			this.PostRead(objectType, indexes);
			bool result = this.IsInOldDeclaration(objectType, indexes);
			indexes.CurrentIndex++;
			return result;
		}

		private void PostRead(ObjectType objectType, Indexes indexes)
		{
			while (!this.m_state.OldDeclarations.ContainsKey(objectType))
			{
				this.m_reader.ReadDeclaration();
			}
			this.Skip(objectType, indexes);
		}

		private void Skip(ObjectType objectType, Indexes indexes)
		{
			IntList[] array = this.m_state.OldIndexesToSkip[(int)objectType];
			if (array != null && array.Length > indexes.CurrentIndex)
			{
				IntList intList = array[indexes.CurrentIndex];
				if (intList != null)
				{
					for (int i = 0; i < intList.Count; i++)
					{
						this.ReadRemovedItemType(this.m_state.OldDeclarations[objectType].Members[intList[i]].Token, this.m_state.OldDeclarations[objectType].Members[intList[i]].ObjectType);
					}
				}
			}
		}

		private void ReadRemovedItemType(Token token, ObjectType objectType)
		{
			switch (token)
			{
			case Token.Object:
				switch (objectType)
				{
				case ObjectType.DataAggregateInfoList:
					this.ReadDataAggregateInfoList();
					return;
				case ObjectType.TableGroup:
					Global.Tracer.Assert(!this.m_intermediateFormatVersion.IsRS2005_WithTableDetailFix);
					return;
				}
				break;
			case Token.String:
				this.m_reader.ReadString();
				return;
			case Token.Int32:
				this.m_reader.ReadInt32();
				return;
			case Token.Reference:
				switch (objectType)
				{
				case ObjectType.ReportItem:
					this.ReadReportItemReference(false);
					return;
				case ObjectType.List:
				case ObjectType.MatrixHeading:
				case ObjectType.TableGroup:
				case ObjectType.TableRow:
				case ObjectType.ReportItemCollection:
				case ObjectType.TableDetail:
					this.ReadRemovedReference();
					return;
				case ObjectType.ChartHeading:
					if (this.m_intermediateFormatVersion.IsRS2000_RTM_orOlder)
					{
						this.ReadRemovedReference();
					}
					return;
				}
				break;
			}
			IntermediateFormatReader.Assert(false);
		}

		private void ReadRemovedReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
		}

		private bool IsInOldDeclaration(ObjectType objectType, Indexes indexes)
		{
			bool[] array = this.m_state.IsInOldDeclaration[(int)objectType];
			if (array != null)
			{
				return array[indexes.CurrentIndex];
			}
			return true;
		}

		private ValidValueList ReadValidValueList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ValidValueList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ValidValueList validValueList = new ValidValueList(num);
			for (int i = 0; i < num; i++)
			{
				validValueList.Add(this.ReadValidValue());
			}
			this.m_reader.ReadEndObject();
			return validValueList;
		}

		private ParameterDefList ReadParameterDefList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ParameterDefList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ParameterDefList parameterDefList = new ParameterDefList(num);
			this.m_parametersDef = null;
			for (int i = 0; i < num; i++)
			{
				parameterDefList.Add(this.ReadParameterDef());
			}
			this.m_reader.ReadEndObject();
			return parameterDefList;
		}

		private ParameterDefList ReadParameterDefRefList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ParameterDefList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ParameterDefList parameterDefList = new ParameterDefList(num);
			for (int i = 0; i < num; i++)
			{
				parameterDefList.Add(this.GetParameterDefObject(this.m_reader.ReadString()));
			}
			this.m_reader.ReadEndObject();
			return parameterDefList;
		}

		private ParameterInfoCollection ReadParameterInfoCollection()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ParameterInfoCollection == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection(num);
			this.m_parametersInfo = null;
			for (int i = 0; i < num; i++)
			{
				parameterInfoCollection.Add(this.ReadParameterInfo());
			}
			this.m_reader.ReadEndObject();
			return parameterInfoCollection;
		}

		private ParameterInfoCollection ReadParameterInfoRefCollection()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ParameterInfoCollection == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection(num);
			for (int i = 0; i < num; i++)
			{
				parameterInfoCollection.Add(this.GetParameterInfoObject(this.m_reader.ReadString()));
			}
			this.m_reader.ReadEndObject();
			return parameterInfoCollection;
		}

		private FilterList ReadFilterList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.FilterList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			FilterList filterList = new FilterList(num);
			for (int i = 0; i < num; i++)
			{
				filterList.Add(this.ReadFilter());
			}
			this.m_reader.ReadEndObject();
			return filterList;
		}

		private DataSourceList ReadDataSourceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataSourceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataSourceList dataSourceList = new DataSourceList(num);
			for (int i = 0; i < num; i++)
			{
				dataSourceList.Add(this.ReadDataSource());
			}
			this.m_reader.ReadEndObject();
			return dataSourceList;
		}

		private DataAggregateInfoList ReadDataAggregateInfoList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataAggregateInfoList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataAggregateInfoList dataAggregateInfoList = new DataAggregateInfoList(num);
			for (int i = 0; i < num; i++)
			{
				dataAggregateInfoList.Add(this.ReadDataAggregateInfo());
			}
			this.m_reader.ReadEndObject();
			return dataAggregateInfoList;
		}

		private ReportItemList ReadReportItemList(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ReportItemList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ReportItemList reportItemList = new ReportItemList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemList.Add(this.ReadReportItem(parent));
			}
			this.m_reader.ReadEndObject();
			return reportItemList;
		}

		private ReportItemIndexerList ReadReportItemIndexerList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ReportItemIndexerList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ReportItemIndexerList reportItemIndexerList = new ReportItemIndexerList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemIndexerList.Add(this.ReadReportItemIndexer());
			}
			this.m_reader.ReadEndObject();
			return reportItemIndexerList;
		}

		private RunningValueInfoList ReadRunningValueInfoList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.RunningValueInfoList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			RunningValueInfoList runningValueInfoList = new RunningValueInfoList(num);
			for (int i = 0; i < num; i++)
			{
				runningValueInfoList.Add(this.ReadRunningValueInfo());
			}
			this.m_reader.ReadEndObject();
			return runningValueInfoList;
		}

		private StyleAttributeHashtable ReadStyleAttributeHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.StyleAttributeHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			StyleAttributeHashtable styleAttributeHashtable = new StyleAttributeHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = this.m_reader.ReadString();
				AttributeInfo value = this.ReadAttributeInfo();
				styleAttributeHashtable.Add(key, value);
			}
			this.m_reader.ReadEndObject();
			return styleAttributeHashtable;
		}

		private ImageInfo ReadImageInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ImageInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ImageInfo imageInfo = new ImageInfo();
			if (this.PreRead(objectType, indexes))
			{
				imageInfo.StreamName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageInfo.MimeType = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return imageInfo;
		}

		private DrillthroughParameters ReadDrillthroughParameters()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DrillthroughParameters == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DrillthroughParameters drillthroughParameters = new DrillthroughParameters(num);
			for (int i = 0; i < num; i++)
			{
				string key = this.m_reader.ReadString();
				object value = this.ReadMultiValue();
				drillthroughParameters.Add(key, value);
			}
			this.m_reader.ReadEndObject();
			return drillthroughParameters;
		}

		private ImageStreamNames ReadImageStreamNames()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ImageStreamNames == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ImageStreamNames imageStreamNames = new ImageStreamNames(num);
			for (int i = 0; i < num; i++)
			{
				string key = this.m_reader.ReadString();
				if (this.m_intermediateFormatVersion.IsRS2000_WithImageInfo)
				{
					ImageInfo value = this.ReadImageInfo();
					imageStreamNames.Add(key, value);
				}
				else
				{
					string streamName = this.m_reader.ReadString();
					imageStreamNames.Add(key, new ImageInfo(streamName, null));
				}
			}
			this.m_reader.ReadEndObject();
			return imageStreamNames;
		}

		private EmbeddedImageHashtable ReadEmbeddedImageHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.EmbeddedImageHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			EmbeddedImageHashtable embeddedImageHashtable = new EmbeddedImageHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = this.m_reader.ReadString();
				if (this.m_intermediateFormatVersion.IsRS2000_WithImageInfo)
				{
					ImageInfo value = this.ReadImageInfo();
					embeddedImageHashtable.Add(key, value);
				}
				else
				{
					string streamName = this.m_reader.ReadString();
					embeddedImageHashtable.Add(key, new ImageInfo(streamName, null));
				}
			}
			this.m_reader.ReadEndObject();
			return embeddedImageHashtable;
		}

		private ExpressionInfoList ReadExpressionInfoList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ExpressionInfoList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ExpressionInfoList expressionInfoList = new ExpressionInfoList(num);
			for (int i = 0; i < num; i++)
			{
				expressionInfoList.Add(this.ReadExpressionInfo());
			}
			this.m_reader.ReadEndObject();
			return expressionInfoList;
		}

		private DataSetList ReadDataSetList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataSetList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataSetList dataSetList = new DataSetList(num);
			for (int i = 0; i < num; i++)
			{
				dataSetList.Add(this.ReadDataSet());
			}
			this.m_reader.ReadEndObject();
			return dataSetList;
		}

		private ExpressionInfo[] ReadExpressionInfos()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			ExpressionInfo[] array = new ExpressionInfo[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadExpressionInfo();
			}
			return array;
		}

		private StringList ReadStringList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.StringList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			StringList stringList = new StringList(num);
			for (int i = 0; i < num; i++)
			{
				stringList.Add(this.m_reader.ReadString());
			}
			this.m_reader.ReadEndObject();
			return stringList;
		}

		private DataFieldList ReadDataFieldList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataFieldList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataFieldList dataFieldList = new DataFieldList(num);
			for (int i = 0; i < num; i++)
			{
				dataFieldList.Add(this.ReadDataField());
			}
			this.m_reader.ReadEndObject();
			return dataFieldList;
		}

		private DataRegionList ReadDataRegionList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataRegionList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataRegionList dataRegionList = new DataRegionList(num);
			for (int i = 0; i < num; i++)
			{
				dataRegionList.Add(this.ReadDataRegionReference());
			}
			this.m_reader.ReadEndObject();
			return dataRegionList;
		}

		private ParameterValueList ReadParameterValueList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ParameterValueList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ParameterValueList parameterValueList = new ParameterValueList(num);
			for (int i = 0; i < num; i++)
			{
				parameterValueList.Add(this.ReadParameterValue());
			}
			this.m_reader.ReadEndObject();
			return parameterValueList;
		}

		private CodeClassList ReadCodeClassList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CodeClassList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			CodeClassList codeClassList = new CodeClassList(num);
			for (int i = 0; i < num; i++)
			{
				codeClassList.Add(this.ReadCodeClass());
			}
			this.m_reader.ReadEndObject();
			return codeClassList;
		}

		private IntList ReadIntList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.IntList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(this.m_reader.ReadInt32());
			}
			this.m_reader.ReadEndObject();
			return intList;
		}

		private Int64List ReadInt64List()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.Int64List == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			Int64List int64List = new Int64List(num);
			for (int i = 0; i < num; i++)
			{
				int64List.Add(this.m_reader.ReadInt64());
			}
			this.m_reader.ReadEndObject();
			return int64List;
		}

		private BoolList ReadBoolList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.BoolList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			BoolList boolList = new BoolList(num);
			for (int i = 0; i < num; i++)
			{
				boolList.Add(this.m_reader.ReadBoolean());
			}
			this.m_reader.ReadEndObject();
			return boolList;
		}

		private MatrixRowList ReadMatrixRowList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixRowList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MatrixRowList matrixRowList = new MatrixRowList(num);
			for (int i = 0; i < num; i++)
			{
				matrixRowList.Add(this.ReadMatrixRow());
			}
			this.m_reader.ReadEndObject();
			return matrixRowList;
		}

		private MatrixColumnList ReadMatrixColumnList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixColumnList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MatrixColumnList matrixColumnList = new MatrixColumnList(num);
			for (int i = 0; i < num; i++)
			{
				matrixColumnList.Add(this.ReadMatrixColumn());
			}
			this.m_reader.ReadEndObject();
			return matrixColumnList;
		}

		private TableColumnList ReadTableColumnList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TableColumnList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			TableColumnList tableColumnList = new TableColumnList(num);
			for (int i = 0; i < num; i++)
			{
				tableColumnList.Add(this.ReadTableColumn());
			}
			this.m_reader.ReadEndObject();
			return tableColumnList;
		}

		private TableRowList ReadTableRowList(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TableRowList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			TableRowList tableRowList = new TableRowList(num);
			for (int i = 0; i < num; i++)
			{
				tableRowList.Add(this.ReadTableRow(parent));
			}
			this.m_reader.ReadEndObject();
			return tableRowList;
		}

		private ChartColumnList ReadChartColumnList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartColumnList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ChartColumnList chartColumnList = new ChartColumnList(num);
			for (int i = 0; i < num; i++)
			{
				chartColumnList.Add(this.ReadChartColumn());
			}
			this.m_reader.ReadEndObject();
			return chartColumnList;
		}

		private SubReportList ReadSubReportList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.SubReportList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			SubReportList subReportList = new SubReportList(num);
			for (int i = 0; i < num; i++)
			{
				subReportList.Add(this.ReadSubReportReference());
			}
			this.m_reader.ReadEndObject();
			return subReportList;
		}

		private NonComputedUniqueNames[] ReadNonComputedUniqueNamess()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			NonComputedUniqueNames[] array = new NonComputedUniqueNames[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadNonComputedUniqueNames();
			}
			return array;
		}

		private ReportItemInstanceList ReadReportItemInstanceList(ReportItemCollection reportItemsDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ReportItemInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ReportItemInstanceList reportItemInstanceList = new ReportItemInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemInstanceList.Add(this.ReadReportItemInstance(reportItemsDef.ComputedReportItems[i]));
			}
			this.m_reader.ReadEndObject();
			return reportItemInstanceList;
		}

		private RenderingPagesRangesList ReadRenderingPagesRangesList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.RenderingPagesRangesList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			RenderingPagesRangesList renderingPagesRangesList = new RenderingPagesRangesList(num);
			for (int i = 0; i < num; i++)
			{
				renderingPagesRangesList.Add(this.ReadRenderingPagesRanges());
			}
			this.m_reader.ReadEndObject();
			return renderingPagesRangesList;
		}

		private ListContentInstanceList ReadListContentInstanceList(List listDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ListContentInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ListContentInstanceList listContentInstanceList = new ListContentInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				listContentInstanceList.Add(this.ReadListContentInstance(listDef));
			}
			this.m_reader.ReadEndObject();
			return listContentInstanceList;
		}

		private MatrixHeadingInstanceList ReadMatrixHeadingInstanceList(MatrixHeading headingDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixHeadingInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MatrixHeadingInstanceList matrixHeadingInstanceList = new MatrixHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				matrixHeadingInstanceList.Add(this.ReadMatrixHeadingInstance(headingDef, i, num));
			}
			this.m_reader.ReadEndObject();
			return matrixHeadingInstanceList;
		}

		private MatrixCellInstancesList ReadMatrixCellInstancesList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixCellInstancesList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MatrixCellInstancesList matrixCellInstancesList = new MatrixCellInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				matrixCellInstancesList.Add(this.ReadMatrixCellInstanceList());
			}
			this.m_reader.ReadEndObject();
			return matrixCellInstancesList;
		}

		private MatrixCellInstanceList ReadMatrixCellInstanceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixCellInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MatrixCellInstanceList matrixCellInstanceList = new MatrixCellInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				matrixCellInstanceList.Add(this.ReadMatrixCellInstanceBase());
			}
			this.m_reader.ReadEndObject();
			return matrixCellInstanceList;
		}

		private MultiChartInstanceList ReadMultiChartInstanceList(Chart chartDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MultiChartInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			MultiChartInstanceList multiChartInstanceList = new MultiChartInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				multiChartInstanceList.Add(this.ReadMultiChartInstance(chartDef));
			}
			this.m_reader.ReadEndObject();
			return multiChartInstanceList;
		}

		private ChartHeadingInstanceList ReadChartHeadingInstanceList(ChartHeading headingDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartHeadingInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ChartHeadingInstanceList chartHeadingInstanceList = new ChartHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				chartHeadingInstanceList.Add(this.ReadChartHeadingInstance(headingDef));
			}
			this.m_reader.ReadEndObject();
			return chartHeadingInstanceList;
		}

		private ChartDataPointInstancesList ReadChartDataPointInstancesList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartDataPointInstancesList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ChartDataPointInstancesList chartDataPointInstancesList = new ChartDataPointInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointInstancesList.Add(this.ReadChartDataPointInstanceList());
			}
			this.m_reader.ReadEndObject();
			return chartDataPointInstancesList;
		}

		private ChartDataPointInstanceList ReadChartDataPointInstanceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartDataPointInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ChartDataPointInstanceList chartDataPointInstanceList = new ChartDataPointInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointInstanceList.Add(this.ReadChartDataPointInstance());
			}
			this.m_reader.ReadEndObject();
			return chartDataPointInstanceList;
		}

		private TableRowInstance[] ReadTableRowInstances(TableRowList rowDefs, int rowStartUniqueName)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			TableRowInstance[] array = new TableRowInstance[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadTableRowInstance(rowDefs, i, rowStartUniqueName);
				if (-1 != rowStartUniqueName)
				{
					rowStartUniqueName++;
					Global.Tracer.Assert(null != rowDefs, "(null != rowDefs)");
					if (rowDefs[i] != null)
					{
						ReportItemCollection reportItems = rowDefs[i].ReportItems;
						if (reportItems != null && reportItems.NonComputedReportItems != null)
						{
							rowStartUniqueName += reportItems.NonComputedReportItems.Count;
						}
					}
				}
			}
			return array;
		}

		private TableDetailInstanceList ReadTableDetailInstanceList(TableDetail detailDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TableDetailInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			TableDetailInstanceList tableDetailInstanceList = new TableDetailInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				tableDetailInstanceList.Add(this.ReadTableDetailInstance(detailDef));
			}
			this.m_reader.ReadEndObject();
			return tableDetailInstanceList;
		}

		private TableGroupInstanceList ReadTableGroupInstanceList(TableGroup groupDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TableGroupInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			TableGroupInstanceList tableGroupInstanceList = new TableGroupInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				tableGroupInstanceList.Add(this.ReadTableGroupInstance(groupDef));
			}
			this.m_reader.ReadEndObject();
			return tableGroupInstanceList;
		}

		private TableColumnInstance[] ReadTableColumnInstances()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			TableColumnInstance[] array = new TableColumnInstance[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadTableColumnInstance();
			}
			return array;
		}

		private CustomReportItemHeadingList ReadCustomReportItemHeadingList(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CustomReportItemHeadingList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			CustomReportItemHeadingList customReportItemHeadingList = new CustomReportItemHeadingList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemHeadingList.Add(this.ReadCustomReportItemHeading(parent));
			}
			this.m_reader.ReadEndObject();
			return customReportItemHeadingList;
		}

		private CustomReportItemHeadingInstanceList ReadCustomReportItemHeadingInstanceList(CustomReportItemHeadingList headingDefinitions)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CustomReportItemHeadingInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = new CustomReportItemHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemHeadingInstanceList.Add(this.ReadCustomReportItemHeadingInstance(headingDefinitions, i, num));
			}
			this.m_reader.ReadEndObject();
			return customReportItemHeadingInstanceList;
		}

		private CustomReportItemCellInstancesList ReadCustomReportItemCellInstancesList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CustomReportItemCellInstancesList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			CustomReportItemCellInstancesList customReportItemCellInstancesList = new CustomReportItemCellInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemCellInstancesList.Add(this.ReadCustomReportItemCellInstanceList());
			}
			this.m_reader.ReadEndObject();
			return customReportItemCellInstancesList;
		}

		private CustomReportItemCellInstanceList ReadCustomReportItemCellInstanceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CustomReportItemCellInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			CustomReportItemCellInstanceList customReportItemCellInstanceList = new CustomReportItemCellInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemCellInstanceList.Add(this.ReadCustomReportItemCellInstance());
			}
			this.m_reader.ReadEndObject();
			return customReportItemCellInstanceList;
		}

		private DocumentMapNode[] ReadDocumentMapNodes()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			DocumentMapNode[] array = new DocumentMapNode[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadDocumentMapNode();
			}
			return array;
		}

		private DocumentMapNodeInfo[] ReadDocumentMapNodesInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			DocumentMapNodeInfo[] array = new DocumentMapNodeInfo[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadDocumentMapNodeInfo();
			}
			return array;
		}

		private bool FindDocumentMapNodesPage(string documentMapId, ref int page)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return false;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			for (int i = 0; i < arrayLength; i++)
			{
				if (this.FindDocumentMapNodePage(documentMapId, ref page))
				{
					return true;
				}
			}
			return false;
		}

		private object[] ReadVariants()
		{
			return this.ReadVariants(false, true);
		}

		private object[] ReadVariants(bool isMultiValue, bool readNextToken)
		{
			if (readNextToken)
			{
				IntermediateFormatReader.Assert(this.m_reader.Read());
			}
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			object[] array = new object[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				if (isMultiValue)
				{
					this.ReadMultiValue(array, i);
				}
				else
				{
					array[i] = this.ReadVariant();
				}
			}
			return array;
		}

		private void ReadMultiValue(object[] parentArray, int index)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token != 0)
			{
				if (Token.Array != this.m_reader.Token)
				{
					IntermediateFormatReader.Assert(null != parentArray);
					parentArray[index] = this.ReadVariant(false);
				}
				else
				{
					IntermediateFormatReader.Assert(null != parentArray);
					parentArray[index] = this.ReadVariants(false, false);
				}
			}
		}

		private object ReadMultiValue()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			if (Token.Array != this.m_reader.Token)
			{
				return this.ReadVariant(false);
			}
			return this.ReadVariants(false, false);
		}

		private string[] ReadStrings()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			string[] array = new string[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.m_reader.ReadString();
			}
			return array;
		}

		private VariantList ReadVariantList(bool convertDBNull)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.VariantList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			VariantList variantList = new VariantList(num);
			for (int i = 0; i < num; i++)
			{
				variantList.Add(this.ReadVariant(true, convertDBNull));
			}
			this.m_reader.ReadEndObject();
			return variantList;
		}

		private VariantList[] ReadVariantLists(bool convertDBNull)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			VariantList[] array = new VariantList[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadVariantList(convertDBNull);
			}
			return array;
		}

		private ProcessingMessageList ReadProcessingMessageList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ProcessingMessageList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ProcessingMessageList processingMessageList = new ProcessingMessageList(num);
			for (int i = 0; i < num; i++)
			{
				processingMessageList.Add(this.ReadProcessingMessage());
			}
			this.m_reader.ReadEndObject();
			return processingMessageList;
		}

		private DataCellsList ReadDataCellsList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataCellsList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataCellsList dataCellsList = new DataCellsList(num);
			for (int i = 0; i < num; i++)
			{
				dataCellsList.Add(this.ReadDataCellList());
			}
			this.m_reader.ReadEndObject();
			return dataCellsList;
		}

		private DataCellList ReadDataCellList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataCellList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataCellList dataCellList = new DataCellList(num);
			for (int i = 0; i < num; i++)
			{
				dataCellList.Add(this.ReadDataValueCRIList());
			}
			this.m_reader.ReadEndObject();
			return dataCellList;
		}

		private DataValueCRIList ReadDataValueCRIList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = this.m_reader.ObjectType;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataValueCRIList == objectType);
			DataValueCRIList dataValueCRIList = new DataValueCRIList();
			this.ReadDataValueList(dataValueCRIList);
			if (this.PreRead(objectType, indexes))
			{
				dataValueCRIList.RDLRowIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataValueCRIList.RDLColumnIndex = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return dataValueCRIList;
		}

		private DataValueList ReadDataValueList()
		{
			DataValueList values = new DataValueList();
			return this.ReadDataValueList(values);
		}

		private DataValueList ReadDataValueList(DataValueList values)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataValueList == this.m_reader.ObjectType);
			int num2 = values.Capacity = this.m_reader.ReadArray();
			for (int i = 0; i < num2; i++)
			{
				values.Add(this.ReadDataValue());
			}
			this.m_reader.ReadEndObject();
			return values;
		}

		private DataValueInstanceList ReadDataValueInstanceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.DataValueInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			DataValueInstanceList dataValueInstanceList = new DataValueInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				dataValueInstanceList.Add(this.ReadDataValueInstance());
			}
			this.m_reader.ReadEndObject();
			return dataValueInstanceList;
		}

		private ImageMapAreaInstanceList ReadImageMapAreaInstanceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ImageMapAreaInstanceList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ImageMapAreaInstanceList imageMapAreaInstanceList = new ImageMapAreaInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				imageMapAreaInstanceList.Add(this.ReadImageMapAreaInstance());
			}
			this.m_reader.ReadEndObject();
			return imageMapAreaInstanceList;
		}

		private void ReadIDOwnerBase(IDOwner idOwner)
		{
			IntermediateFormatReader.Assert(null != idOwner);
			ObjectType objectType = ObjectType.IDOwner;
			Indexes indexes = new Indexes();
			if (this.PreRead(objectType, indexes))
			{
				idOwner.ID = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
		}

		private ReportItem ReadReportItem(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.Line == this.m_reader.ObjectType)
			{
				return this.ReadLineInternals(parent);
			}
			if (ObjectType.Rectangle == this.m_reader.ObjectType)
			{
				return this.ReadRectangleInternals(parent);
			}
			if (ObjectType.Image == this.m_reader.ObjectType)
			{
				return this.ReadImageInternals(parent);
			}
			if (ObjectType.CheckBox == this.m_reader.ObjectType)
			{
				return this.ReadCheckBoxInternals(parent);
			}
			if (ObjectType.TextBox == this.m_reader.ObjectType)
			{
				return this.ReadTextBoxInternals(parent);
			}
			if (ObjectType.SubReport == this.m_reader.ObjectType)
			{
				return this.ReadSubReportInternals(parent);
			}
			if (ObjectType.ActiveXControl == this.m_reader.ObjectType)
			{
				return this.ReadActiveXControlInternals(parent);
			}
			return this.ReadDataRegionInternals(parent);
		}

		private void ReadReportItemBase(ReportItem reportItem)
		{
			IntermediateFormatReader.Assert(null != reportItem);
			ObjectType objectType = ObjectType.ReportItem;
			Indexes indexes = new Indexes();
			this.ReadIDOwnerBase(reportItem);
			this.RegisterDefinitionObject(reportItem);
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Top = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.TopValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Left = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.LeftValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Height = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.HeightValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Width = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.WidthValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.ZIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Visibility = this.ReadVisibility();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.ToolTip = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Label = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Bookmark = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.Custom = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.RepeatedSibling = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.IsFullSize = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.DataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.DataElementOutput = this.ReadDataElementOutputType(reportItem.Visibility);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.DistanceFromReportTop = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.DistanceBeforeTop = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.SiblingAboveMe = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItem.CustomProperties = this.ReadDataValueList();
			}
			this.PostRead(objectType, indexes);
		}

		private ReportItem ReadReportItemReference(bool getDefinition)
		{
			if (this.m_intermediateFormatVersion.IsRS2000_WithOtherPageChunkSplit)
			{
				IntermediateFormatReader.Assert(this.m_reader.ReadNoTypeReference());
			}
			else
			{
				IntermediateFormatReader.Assert(this.m_reader.Read());
			}
			if (this.m_reader.Token != 0 && getDefinition)
			{
				IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
				IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
				IntermediateFormatReader.Assert(definitionObject is ReportItem);
				return (ReportItem)definitionObject;
			}
			return null;
		}

		private SubReport ReadSubReportReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.SubReport == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is SubReport);
			return (SubReport)definitionObject;
		}

		private PageSection ReadPageSection(bool isHeader, ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSection;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			PageSection pageSection = new PageSection(isHeader, parent);
			this.ReadReportItemBase(pageSection);
			if (this.PreRead(objectType, indexes))
			{
				pageSection.PrintOnFirstPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				pageSection.PrintOnLastPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				pageSection.ReportItems = this.ReadReportItemCollection(pageSection);
			}
			if (this.PreRead(objectType, indexes))
			{
				pageSection.PostProcessEvaluate = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return pageSection;
		}

		private ReportItemCollection ReadReportItemCollection(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemCollection;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportItemCollection reportItemCollection = new ReportItemCollection();
			this.ReadIDOwnerBase(reportItemCollection);
			this.RegisterDefinitionObject(reportItemCollection);
			if (this.PreRead(objectType, indexes))
			{
				reportItemCollection.NonComputedReportItems = this.ReadReportItemList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItemCollection.ComputedReportItems = this.ReadReportItemList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItemCollection.SortedReportItems = this.ReadReportItemIndexerList();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportItemCollection.RunningValues = this.ReadRunningValueInfoList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportItemCollection;
		}

		private Report.ShowHideTypes ReadShowHideTypes()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Report.ShowHideTypes), num));
			return (Report.ShowHideTypes)num;
		}

		private DataElementOutputTypes ReadDataElementOutputType(Visibility visibility)
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(DataElementOutputTypes), num));
			DataElementOutputTypes dataElementOutputTypes = (DataElementOutputTypes)num;
			if (dataElementOutputTypes == DataElementOutputTypes.Output && (this.m_intermediateFormatVersion == null || !this.m_intermediateFormatVersion.IsRS2005_WithXmlDataElementOutputChange) && visibility != null && visibility.Hidden != null && ExpressionInfo.Types.Constant == visibility.Hidden.Type && visibility.Hidden.BoolValue && visibility.Toggle == null)
			{
				dataElementOutputTypes = DataElementOutputTypes.NoOutput;
			}
			return dataElementOutputTypes;
		}

		private Style ReadStyle()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Style;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Style style = new Style(ConstructionPhase.Deserializing);
			if (this.PreRead(objectType, indexes))
			{
				style.StyleAttributes = this.ReadStyleAttributeHashtable();
			}
			if (this.PreRead(objectType, indexes))
			{
				style.ExpressionList = this.ReadExpressionInfoList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return style;
		}

		private Visibility ReadVisibility()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Visibility;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Visibility visibility = new Visibility();
			if (this.PreRead(objectType, indexes))
			{
				visibility.Hidden = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				visibility.Toggle = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				visibility.RecursiveReceiver = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return visibility;
		}

		private Filter ReadFilter()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Filter;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Filter filter = new Filter();
			if (this.PreRead(objectType, indexes))
			{
				filter.Expression = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				filter.Operator = this.ReadOperators();
			}
			if (this.PreRead(objectType, indexes))
			{
				filter.Values = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				filter.ExprHostID = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return filter;
		}

		private Filter.Operators ReadOperators()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Filter.Operators), num));
			return (Filter.Operators)num;
		}

		private DataSource ReadDataSource()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataSource;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DataSource dataSource = new DataSource();
			if (this.PreRead(objectType, indexes))
			{
				dataSource.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.Transaction = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.Type = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.ConnectStringExpression = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.IntegratedSecurity = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.Prompt = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.DataSourceReference = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.DataSets = this.ReadDataSetList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.ID = this.m_reader.ReadGuid();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSource.SharedDataSourceReferencePath = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return dataSource;
		}

		private DataAggregateInfo ReadDataAggregateInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.RunningValueInfo == this.m_reader.ObjectType)
			{
				return this.ReadRunningValueInfoInternals();
			}
			IntermediateFormatReader.Assert(ObjectType.DataAggregateInfo == this.m_reader.ObjectType);
			DataAggregateInfo dataAggregateInfo = new DataAggregateInfo();
			this.ReadDataAggregateInfoBase(dataAggregateInfo);
			this.m_reader.ReadEndObject();
			return dataAggregateInfo;
		}

		private void ReadDataAggregateInfoBase(DataAggregateInfo aggregate)
		{
			IntermediateFormatReader.Assert(null != aggregate);
			ObjectType objectType = ObjectType.DataAggregateInfo;
			Indexes indexes = new Indexes();
			if (this.PreRead(objectType, indexes))
			{
				aggregate.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				aggregate.AggregateType = this.ReadAggregateTypes();
			}
			if (this.PreRead(objectType, indexes))
			{
				aggregate.Expressions = this.ReadExpressionInfos();
			}
			if (this.PreRead(objectType, indexes))
			{
				aggregate.DuplicateNames = this.ReadStringList();
			}
			this.PostRead(objectType, indexes);
		}

		private ExpressionInfo ReadExpressionInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ExpressionInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token || Token.String == this.m_reader.Token || Token.Boolean == this.m_reader.Token || Token.Int32 == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType || ObjectType.None == this.m_reader.ObjectType);
			ExpressionInfo expressionInfo = new ExpressionInfo();
			if (this.m_reader.ObjectType == ObjectType.None)
			{
				expressionInfo.Type = ExpressionInfo.Types.Constant;
				switch (this.m_reader.Token)
				{
				case Token.String:
					expressionInfo.Value = this.m_reader.StringValue;
					break;
				case Token.Boolean:
					expressionInfo.BoolValue = this.m_reader.BooleanValue;
					break;
				case Token.Int32:
					expressionInfo.IntValue = this.m_reader.Int32Value;
					break;
				default:
					IntermediateFormatReader.Assert(false);
					break;
				}
			}
			else
			{
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.Type = this.ReadTypes();
				}
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.Value = this.m_reader.ReadString();
				}
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.BoolValue = this.m_reader.ReadBoolean();
				}
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.IntValue = this.m_reader.ReadInt32();
				}
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.ExprHostID = this.m_reader.ReadInt32();
				}
				if (this.PreRead(objectType, indexes))
				{
					expressionInfo.OriginalText = this.m_reader.ReadString();
				}
				this.PostRead(objectType, indexes);
				this.m_reader.ReadEndObject();
			}
			return expressionInfo;
		}

		private DataAggregateInfo.AggregateTypes ReadAggregateTypes()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(DataAggregateInfo.AggregateTypes), num));
			return (DataAggregateInfo.AggregateTypes)num;
		}

		private ExpressionInfo.Types ReadTypes()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ExpressionInfo.Types), num));
			return (ExpressionInfo.Types)num;
		}

		private ReportItemIndexer ReadReportItemIndexer()
		{
			ObjectType objectType = ObjectType.ReportItemIndexer;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(objectType == this.m_reader.ReadObject());
			ReportItemIndexer result = default(ReportItemIndexer);
			if (this.PreRead(objectType, indexes))
			{
				result.IsComputed = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				result.Index = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return result;
		}

		private RenderingPagesRanges ReadRenderingPagesRanges()
		{
			ObjectType objectType = ObjectType.RenderingPagesRanges;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(objectType == this.m_reader.ReadObject());
			RenderingPagesRanges result = default(RenderingPagesRanges);
			if (this.PreRead(objectType, indexes))
			{
				result.StartPage = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				result.EndPage = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return result;
		}

		private RunningValueInfo ReadRunningValueInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadRunningValueInfoInternals();
		}

		private RunningValueInfo ReadRunningValueInfoInternals()
		{
			ObjectType objectType = ObjectType.RunningValueInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RunningValueInfo runningValueInfo = new RunningValueInfo();
			this.ReadDataAggregateInfoBase(runningValueInfo);
			if (this.PreRead(objectType, indexes))
			{
				runningValueInfo.Scope = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return runningValueInfo;
		}

		private AttributeInfo ReadAttributeInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.AttributeInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			AttributeInfo attributeInfo = new AttributeInfo();
			if (this.PreRead(objectType, indexes))
			{
				attributeInfo.IsExpression = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				attributeInfo.Value = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				attributeInfo.BoolValue = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				attributeInfo.IntValue = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return attributeInfo;
		}

		private DataSet ReadDataSet()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataSet;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DataSet dataSet = new DataSet();
			if (this.m_intermediateFormatVersion.Is_WithUserSort)
			{
				this.ReadIDOwnerBase(dataSet);
				this.RegisterDefinitionObject(dataSet);
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Fields = this.ReadDataFieldList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Query = this.ReadReportQuery();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.CaseSensitivity = this.ReadSensitivity();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Collation = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.AccentSensitivity = this.ReadSensitivity();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.KanatypeSensitivity = this.ReadSensitivity();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.WidthSensitivity = this.ReadSensitivity();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.DataRegions = this.ReadDataRegionList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Aggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.Filters = this.ReadFilterList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.RecordSetSize = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.UsedOnlyInParameters = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.NonCalculatedFieldCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.PostSortAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.LCID = (uint)this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.HasDetailUserSortFilter = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.UserSortExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.DynamicFieldReferences = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataSet.InterpretSubtotalsAsDetails = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return dataSet;
		}

		private ReportQuery ReadReportQuery()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportQuery;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportQuery reportQuery = new ReportQuery();
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.CommandType = this.ReadCommandType();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.CommandText = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.Parameters = this.ReadParameterValueList();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.TimeOut = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.CommandTextValue = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportQuery.RewrittenCommandText = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportQuery;
		}

		private DataSet.Sensitivity ReadSensitivity()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(DataSet.Sensitivity), num));
			return (DataSet.Sensitivity)num;
		}

		private CommandType ReadCommandType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(CommandType), num));
			return (CommandType)num;
		}

		private Field ReadDataField()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Field;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Field field = new Field();
			if (this.PreRead(objectType, indexes))
			{
				field.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				field.DataField = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				field.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				field.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				field.DynamicPropertyReferences = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				field.ReferencedProperties = this.ReadFieldPropertyHashtable();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return field;
		}

		internal FieldPropertyHashtable ReadFieldPropertyHashtable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.FieldPropertyHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			FieldPropertyHashtable fieldPropertyHashtable = new FieldPropertyHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = this.m_reader.ReadString();
				fieldPropertyHashtable.Add(key);
			}
			this.m_reader.ReadEndObject();
			return fieldPropertyHashtable;
		}

		private ParameterValue ReadParameterValue()
		{
			ObjectType objectType = ObjectType.ParameterValue;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(objectType == this.m_reader.ReadObject());
			ParameterValue parameterValue = new ParameterValue();
			if (this.PreRead(objectType, indexes))
			{
				parameterValue.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterValue.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterValue.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterValue.Omit = this.ReadExpressionInfo();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return parameterValue;
		}

		private CodeClass ReadCodeClass()
		{
			ObjectType objectType = ObjectType.CodeClass;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(objectType == this.m_reader.ReadObject());
			CodeClass result = default(CodeClass);
			if (this.PreRead(objectType, indexes))
			{
				result.ClassName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				result.InstanceName = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return result;
		}

		private Action ReadAction()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Action;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Action action = new Action();
			if (this.PreRead(objectType, indexes))
			{
				action.ActionItems = this.ReadActionItemList();
			}
			if (this.PreRead(objectType, indexes))
			{
				action.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				action.ComputedActionItemsCount = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return action;
		}

		private ActionItemList ReadActionItemList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ActionItemList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ActionItemList actionItemList = new ActionItemList(num);
			for (int i = 0; i < num; i++)
			{
				actionItemList.Add(this.ReadActionItem());
			}
			this.m_reader.ReadEndObject();
			return actionItemList;
		}

		private ActionItem ReadActionItem()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ActionItem actionItem = new ActionItem();
			if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
			{
				ObjectType objectType = ObjectType.ActionItem;
				Indexes indexes = new Indexes();
				IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
				IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
				if (this.PreRead(objectType, indexes))
				{
					actionItem.HyperLinkURL = this.ReadExpressionInfo();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.DrillthroughReportName = this.ReadExpressionInfo();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.DrillthroughParameters = this.ReadParameterValueList();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.DrillthroughBookmarkLink = this.ReadExpressionInfo();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.BookmarkLink = this.ReadExpressionInfo();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.Label = this.ReadExpressionInfo();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.ExprHostID = this.m_reader.ReadInt32();
				}
				if (this.PreRead(objectType, indexes))
				{
					actionItem.ComputedIndex = this.m_reader.ReadInt32();
				}
				this.PostRead(objectType, indexes);
			}
			else
			{
				ObjectType objectType2 = ObjectType.Action;
				IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
				IntermediateFormatReader.Assert(objectType2 == this.m_reader.ObjectType);
				actionItem.ComputedIndex = 0;
				actionItem.HyperLinkURL = this.ReadExpressionInfo();
				actionItem.DrillthroughReportName = this.ReadExpressionInfo();
				actionItem.DrillthroughParameters = this.ReadParameterValueList();
				actionItem.DrillthroughBookmarkLink = this.ReadExpressionInfo();
				actionItem.BookmarkLink = this.ReadExpressionInfo();
			}
			this.m_reader.ReadEndObject();
			return actionItem;
		}

		private Line ReadLineInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Line;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Line line = new Line(parent);
			this.ReadReportItemBase(line);
			if (this.PreRead(objectType, indexes))
			{
				line.LineSlant = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return line;
		}

		private Rectangle ReadRectangleInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Rectangle;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Rectangle rectangle = new Rectangle(parent);
			this.ReadReportItemBase(rectangle);
			if (this.PreRead(objectType, indexes))
			{
				rectangle.ReportItems = this.ReadReportItemCollection(rectangle);
			}
			if (this.PreRead(objectType, indexes))
			{
				rectangle.PageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				rectangle.PageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				rectangle.LinkToChild = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return rectangle;
		}

		private Image ReadImageInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Image;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Image image = new Image(parent);
			this.ReadReportItemBase(image);
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					image.Action = this.ReadAction();
				}
				else
				{
					ActionItem actionItem = this.ReadActionItem();
					if (actionItem != null)
					{
						image.Action = new Action(actionItem, true);
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				image.Source = this.ReadSourceType();
			}
			if (this.PreRead(objectType, indexes))
			{
				image.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				image.MIMEType = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				image.Sizing = this.ReadSizings();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return image;
		}

		private ImageMapAreaInstance ReadImageMapAreaInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ImageMapAreaInstance imageMapAreaInstance = new ImageMapAreaInstance();
			ObjectType objectType = ObjectType.ImageMapAreaInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.ID = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Shape = this.ReadImageMapAreaShape();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Coordinates = this.m_reader.ReadFloatArray();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Action = this.ReadAction();
			}
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.ActionInstance = this.ReadActionInstance(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				imageMapAreaInstance.UniqueName = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return imageMapAreaInstance;
		}

		private Image.SourceType ReadSourceType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Image.SourceType), num));
			return (Image.SourceType)num;
		}

		private Image.Sizings ReadSizings()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Image.Sizings), num));
			return (Image.Sizings)num;
		}

		private ImageMapArea.ImageMapAreaShape ReadImageMapAreaShape()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ImageMapArea.ImageMapAreaShape), num));
			return (ImageMapArea.ImageMapAreaShape)num;
		}

		private CheckBox ReadCheckBoxInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CheckBox;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CheckBox checkBox = new CheckBox(parent);
			this.ReadReportItemBase(checkBox);
			if (this.PreRead(objectType, indexes))
			{
				checkBox.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				checkBox.HideDuplicates = this.m_reader.ReadString();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return checkBox;
		}

		private TextBox ReadTextBoxInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TextBox;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TextBox textBox = new TextBox(parent);
			this.ReadReportItemBase(textBox);
			if (this.PreRead(objectType, indexes))
			{
				textBox.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.CanGrow = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.CanShrink = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.HideDuplicates = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					textBox.Action = this.ReadAction();
				}
				else
				{
					ActionItem actionItem = this.ReadActionItem();
					if (actionItem != null)
					{
						textBox.Action = new Action(actionItem, true);
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.IsToggle = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.InitialToggleState = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.ValueType = this.ReadTypeCode();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.Formula = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.ValueReferenced = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.RecursiveSender = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.DataElementStyleAttribute = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.ContainingScopes = this.ReadGroupingReferenceList();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.UserSort = this.ReadEndUserSort(textBox);
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.IsMatrixCellScope = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				textBox.IsSubReportTopLevelScope = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return textBox;
		}

		private EndUserSort ReadEndUserSort(TextBox eventSource)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.EndUserSort;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			EndUserSort endUserSort = new EndUserSort();
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.DataSetID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.SortExpressionScopeID = this.ReadIDOwnerID(ObjectType.ISortFilterScope);
			}
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.GroupInSortTargetIDs = this.ReadGroupingIDList();
			}
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.SortTargetID = this.ReadIDOwnerID(ObjectType.ISortFilterScope);
			}
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.SortExpressionIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				endUserSort.DetailScopeSubReports = this.ReadSubReportList();
			}
			if (-1 != endUserSort.SortExpressionScopeID || endUserSort.GroupInSortTargetIDs != null || -1 != endUserSort.SortTargetID)
			{
				Global.Tracer.Assert(this.m_textboxesWithUserSort != null && 0 < this.m_textboxesWithUserSort.Count);
				TextBoxList textBoxList = (TextBoxList)this.m_textboxesWithUserSort[this.m_textboxesWithUserSort.Count - 1];
				Global.Tracer.Assert(null != textBoxList, "(null != textboxesWithUserSort)");
				textBoxList.Add(eventSource);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return endUserSort;
		}

		private IntList ReadGroupingIDList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.GroupingList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(this.ReadIDOwnerID(ObjectType.Grouping));
			}
			this.m_reader.ReadEndObject();
			return intList;
		}

		private GroupingList ReadGroupingReferenceList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.GroupingList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			GroupingList groupingList = new GroupingList(num);
			for (int i = 0; i < num; i++)
			{
				groupingList.Add(this.ReadGroupingReference());
			}
			this.m_reader.ReadEndObject();
			return groupingList;
		}

		private Grouping ReadGroupingReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.Grouping == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is ReportHierarchyNode);
			return ((ReportHierarchyNode)definitionObject).Grouping;
		}

		private TypeCode ReadTypeCode()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(TypeCode), num));
			return (TypeCode)num;
		}

		private SubReport ReadSubReportInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.SubReport;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SubReport subReport = new SubReport(parent);
			this.ReadReportItemBase(subReport);
			if (this.PreRead(objectType, indexes))
			{
				subReport.ReportPath = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.Parameters = this.ReadParameterValueList();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.NoRows = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.MergeTransactions = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.ContainingScopes = this.ReadGroupingReferenceList();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.IsMatrixCellScope = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.DataSetUniqueNameMap = this.ReadScopeLookupTable();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.RetrievalStatus = this.ReadStatus();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.ReportName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.Description = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.Report = this.ReadReport(subReport);
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.StringUri = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				subReport.ParametersFromCatalog = this.ReadParameterInfoCollection();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return subReport;
		}

		private ScopeLookupTable ReadScopeLookupTable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ScopeLookupTable;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ScopeLookupTable scopeLookupTable = new ScopeLookupTable();
			if (this.PreRead(objectType, indexes))
			{
				scopeLookupTable.LookupTable = this.ReadScopeTableValues();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return scopeLookupTable;
		}

		private object ReadScopeTableValues()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (Token.Int32 == this.m_reader.Token)
			{
				return this.m_reader.Int32Value;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			Hashtable hashtable = new Hashtable(arrayLength);
			for (int i = 0; i < arrayLength; i++)
			{
				object key = this.ReadVariant(true, true);
				object value = this.ReadScopeTableValues();
				hashtable.Add(key, value);
			}
			return hashtable;
		}

		private SubReport.Status ReadStatus()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(SubReport.Status), num));
			return (SubReport.Status)num;
		}

		private ActiveXControl ReadActiveXControlInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.ActiveXControl;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ActiveXControl activeXControl = new ActiveXControl(parent);
			this.ReadReportItemBase(activeXControl);
			if (this.PreRead(objectType, indexes))
			{
				activeXControl.ClassID = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				activeXControl.CodeBase = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				activeXControl.Parameters = this.ReadParameterValueList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return activeXControl;
		}

		private ParameterBase ReadParameterBase(ParameterBase parameter)
		{
			IntermediateFormatReader.Assert(null != parameter);
			ObjectType objectType = ObjectType.ParameterBase;
			Indexes indexes = new Indexes();
			if (this.PreRead(objectType, indexes))
			{
				parameter.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.DataType = this.ReadDataType();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.Nullable = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.Prompt = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.UsedInQuery = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.AllowBlank = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.MultiValue = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.DefaultValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameter.PromptUser = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			return parameter;
		}

		private ParameterDef ReadParameterDef()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterDef;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ParameterDef parameterDef = new ParameterDef();
			this.ReadParameterBase(parameterDef);
			this.RegisterParameterDef(parameterDef);
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesDataSource = this.ReadParameterDataSource();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesValueExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesLabelExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.DefaultDataSource = this.ReadParameterDataSource();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.DefaultExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.DependencyList = this.ReadParameterDefRefList();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDef.ExprHostID = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return parameterDef;
		}

		private ParameterDataSource ReadParameterDataSource()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterDataSource;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ParameterDataSource parameterDataSource = new ParameterDataSource();
			if (this.PreRead(objectType, indexes))
			{
				parameterDataSource.DataSourceIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDataSource.DataSetIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDataSource.ValueFieldIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterDataSource.LabelFieldIndex = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return parameterDataSource;
		}

		private ValidValue ReadValidValue()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ValidValue;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ValidValue validValue = new ValidValue();
			if (this.PreRead(objectType, indexes))
			{
				validValue.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				validValue.Value = this.ReadVariant();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return validValue;
		}

		private DataRegion ReadDataRegionInternals(ReportItem parent)
		{
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.List == this.m_reader.ObjectType)
			{
				return this.ReadListInternals(parent);
			}
			if (ObjectType.Matrix == this.m_reader.ObjectType)
			{
				return this.ReadMatrixInternals(parent);
			}
			if (ObjectType.Table == this.m_reader.ObjectType)
			{
				return this.ReadTableInternals(parent);
			}
			if (ObjectType.Chart == this.m_reader.ObjectType)
			{
				return this.ReadChartInternals(parent);
			}
			if (ObjectType.CustomReportItem == this.m_reader.ObjectType)
			{
				return this.ReadCustomReportItemInternals(parent);
			}
			IntermediateFormatReader.Assert(ObjectType.OWCChart == this.m_reader.ObjectType);
			return this.ReadOWCChartInternals(parent);
		}

		private void ReadDataRegionBase(DataRegion dataRegion)
		{
			IntermediateFormatReader.Assert(null != dataRegion);
			ObjectType objectType = ObjectType.DataRegion;
			Indexes indexes = new Indexes();
			this.ReadReportItemBase(dataRegion);
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.DataSetName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.NoRows = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.PageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.PageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.KeepTogether = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.RepeatSiblings = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.Filters = this.ReadFilterList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.Aggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.PostSortAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.UserSortExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataRegion.DetailSortFiltersInScope = this.ReadInScopeSortFilterTable();
			}
			this.PostRead(objectType, indexes);
		}

		private DataRegion ReadDataRegionReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.List == this.m_reader.ObjectType || ObjectType.Table == this.m_reader.ObjectType || ObjectType.Matrix == this.m_reader.ObjectType || ObjectType.Chart == this.m_reader.ObjectType || ObjectType.CustomReportItem == this.m_reader.ObjectType || ObjectType.OWCChart == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is DataRegion);
			return (DataRegion)definitionObject;
		}

		private ReportHierarchyNode ReadReportHierarchyNode(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.TableGroup == this.m_reader.ObjectType)
			{
				return this.ReadTableGroupInternals(parent);
			}
			if (ObjectType.MatrixHeading == this.m_reader.ObjectType)
			{
				return this.ReadMatrixHeadingInternals(parent);
			}
			if (ObjectType.MultiChart == this.m_reader.ObjectType)
			{
				return this.ReadMultiChartInternals(parent);
			}
			if (ObjectType.ChartHeading == this.m_reader.ObjectType)
			{
				return this.ReadChartHeadingInternals(parent);
			}
			if (ObjectType.CustomReportItemHeading == this.m_reader.ObjectType)
			{
				return this.ReadCustomReportItemHeadingInternals(parent);
			}
			IntermediateFormatReader.Assert(ObjectType.ReportHierarchyNode == this.m_reader.ObjectType);
			ReportHierarchyNode reportHierarchyNode = new ReportHierarchyNode();
			this.ReadReportHierarchyNodeBase(reportHierarchyNode, parent);
			this.m_reader.ReadEndObject();
			return reportHierarchyNode;
		}

		private void ReadReportHierarchyNodeBase(ReportHierarchyNode node, ReportItem parent)
		{
			IntermediateFormatReader.Assert(null != node);
			this.ReadIDOwnerBase(node);
			this.RegisterDefinitionObject(node);
			ObjectType objectType = ObjectType.ReportHierarchyNode;
			Indexes indexes = new Indexes();
			if (this.PreRead(objectType, indexes))
			{
				node.Grouping = this.ReadGrouping();
			}
			if (this.PreRead(objectType, indexes))
			{
				node.Sorting = this.ReadSorting();
			}
			if (this.PreRead(objectType, indexes))
			{
				node.InnerHierarchy = this.ReadReportHierarchyNode(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				node.DataRegionDef = this.ReadDataRegionReference();
			}
			this.PostRead(objectType, indexes);
		}

		private Grouping ReadGrouping()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Grouping;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Grouping grouping = new Grouping(ConstructionPhase.Deserializing);
			if (this.PreRead(objectType, indexes))
			{
				grouping.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.GroupExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.GroupLabel = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.SortDirections = this.ReadBoolList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.PageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.PageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.Custom = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.Aggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.GroupAndSort = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.Filters = this.ReadFilterList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.HideDuplicatesReportItemIDs = this.ReadReportItemIDList();
				if (grouping.HideDuplicatesReportItemIDs != null)
				{
					this.m_groupingsWithHideDuplicatesStack.Peek().Add(grouping);
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.Parent = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.RecursiveAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.PostSortAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.DataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.DataCollectionName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.DataElementOutput = this.ReadDataElementOutputType(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.CustomProperties = this.ReadDataValueList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.SaveGroupExprValues = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.UserSortExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.NonDetailSortFiltersInScope = this.ReadInScopeSortFilterTable();
			}
			if (this.PreRead(objectType, indexes))
			{
				grouping.DetailSortFiltersInScope = this.ReadInScopeSortFilterTable();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return grouping;
		}

		private InScopeSortFilterHashtable ReadInScopeSortFilterTable()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.InScopeSortFilterHashtable == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			InScopeSortFilterHashtable inScopeSortFilterHashtable = new InScopeSortFilterHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int num2 = this.m_reader.ReadInt32();
				IntList value = this.ReadIntList();
				inScopeSortFilterHashtable.Add(num2, value);
			}
			this.m_reader.ReadEndObject();
			return inScopeSortFilterHashtable;
		}

		private IntList ReadReportItemIDList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ReportItemList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(this.ReadIDOwnerID(ObjectType.TextBox));
			}
			this.m_reader.ReadEndObject();
			return intList;
		}

		private int ReadIDOwnerID(ObjectType objectType)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return -1;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			return this.m_reader.ReferenceValue;
		}

		private Sorting ReadSorting()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Sorting;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Sorting sorting = new Sorting(ConstructionPhase.Deserializing);
			if (this.PreRead(objectType, indexes))
			{
				sorting.SortExpressions = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				sorting.SortDirections = this.ReadBoolList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return sorting;
		}

		private TableGroup ReadTableGroup(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadTableGroupInternals(parent);
		}

		private TableGroup ReadTableGroupInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TableGroup;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableGroup tableGroup = new TableGroup();
			this.ReadReportHierarchyNodeBase(tableGroup, parent);
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.HeaderRows = this.ReadTableRowList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.HeaderRepeatOnNewPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.FooterRows = this.ReadTableRowList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.FooterRepeatOnNewPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.Visibility = this.ReadVisibility();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.PropagatedPageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.PropagatedPageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableGroup.HasExprHost = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableGroup;
		}

		private TableGroup ReadTableGroupReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.TableGroup == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is TableGroup);
			return (TableGroup)definitionObject;
		}

		private TableDetail ReadTableDetail(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadTableDetailInternals(parent);
		}

		private TableDetail ReadTableDetailInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TableDetail;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableDetail tableDetail = new TableDetail();
			this.ReadIDOwnerBase(tableDetail);
			this.RegisterDefinitionObject(tableDetail);
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.DetailRows = this.ReadTableRowList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.Sorting = this.ReadSorting();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.Visibility = this.ReadVisibility();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.HasExprHost = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetail.SimpleDetailRows = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableDetail;
		}

		private void ReadPivotHeadingBase(PivotHeading pivotHeading, ReportItem parent)
		{
			IntermediateFormatReader.Assert(null != pivotHeading);
			ObjectType objectType = ObjectType.PivotHeading;
			Indexes indexes = new Indexes();
			this.ReadReportHierarchyNodeBase(pivotHeading, parent);
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.Visibility = this.ReadVisibility();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.Subtotal = this.ReadSubtotal(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.Level = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.IsColumn = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.HasExprHost = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.SubtotalSpan = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivotHeading.IDs = this.ReadIntList();
			}
			this.PostRead(objectType, indexes);
		}

		private MatrixHeading ReadMatrixHeading(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadMatrixHeadingInternals(parent);
		}

		private MatrixHeading ReadMatrixHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.MatrixHeading;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixHeading matrixHeading = new MatrixHeading();
			this.ReadPivotHeadingBase(matrixHeading, parent);
			if (this.PreRead(objectType, indexes))
			{
				matrixHeading.Size = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixHeading.SizeValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixHeading.ReportItems = this.ReadReportItemCollection(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixHeading.OwcGroupExpression = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixHeading;
		}

		private MatrixHeading ReadMatrixHeadingReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.MatrixHeading == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is MatrixHeading);
			return (MatrixHeading)definitionObject;
		}

		private void ReadTablixHeadingBase(TablixHeading tablixHeading, ReportItem parent)
		{
			IntermediateFormatReader.Assert(null != tablixHeading);
			ObjectType objectType = ObjectType.TablixHeading;
			Indexes indexes = new Indexes();
			this.ReadReportHierarchyNodeBase(tablixHeading, null);
			if (this.PreRead(objectType, indexes))
			{
				tablixHeading.Subtotal = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablixHeading.IsColumn = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablixHeading.Level = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablixHeading.HasExprHost = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablixHeading.HeadingSpan = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
		}

		private CustomReportItemHeading ReadCustomReportItemHeading(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadCustomReportItemHeadingInternals(parent);
		}

		private CustomReportItemHeading ReadCustomReportItemHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CustomReportItemHeading;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItemHeading customReportItemHeading = new CustomReportItemHeading();
			this.ReadTablixHeadingBase(customReportItemHeading, parent);
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeading.Static = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeading.InnerHeadings = this.ReadCustomReportItemHeadingList(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeading.CustomProperties = this.ReadDataValueList();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeading.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeading.RunningValues = this.ReadRunningValueInfoList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItemHeading;
		}

		private CustomReportItemHeading ReadCustomReportItemHeadingReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.CustomReportItemHeading == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is CustomReportItemHeading);
			return (CustomReportItemHeading)definitionObject;
		}

		private TableRow ReadTableRow(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRow;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableRow tableRow = new TableRow();
			this.ReadIDOwnerBase(tableRow);
			this.RegisterDefinitionObject(tableRow);
			if (this.PreRead(objectType, indexes))
			{
				tableRow.ReportItems = this.ReadReportItemCollection(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				tableRow.IDs = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableRow.ColSpans = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableRow.Height = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableRow.HeightValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableRow.Visibility = this.ReadVisibility();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableRow;
		}

		private Subtotal ReadSubtotal(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Subtotal;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Subtotal subtotal = new Subtotal();
			this.ReadIDOwnerBase(subtotal);
			if (this.PreRead(objectType, indexes))
			{
				subtotal.AutoDerived = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				subtotal.ReportItems = this.ReadReportItemCollection(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				subtotal.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				subtotal.Position = this.ReadPositionType();
			}
			if (this.PreRead(objectType, indexes))
			{
				subtotal.DataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				subtotal.DataElementOutput = this.ReadDataElementOutputType(null);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return subtotal;
		}

		private Subtotal.PositionType ReadPositionType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Subtotal.PositionType), num));
			return (Subtotal.PositionType)num;
		}

		private List ReadListInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.List;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			List list = new List(parent);
			this.ReadDataRegionBase(list);
			if (this.PreRead(objectType, indexes))
			{
				list.HierarchyDef = this.ReadReportHierarchyNode(list);
			}
			if (this.PreRead(objectType, indexes))
			{
				list.ReportItems = this.ReadReportItemCollection(list);
			}
			if (this.PreRead(objectType, indexes))
			{
				list.FillPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				list.DataInstanceName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				list.DataInstanceElementOutput = this.ReadDataElementOutputType(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				list.IsListMostInner = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return list;
		}

		private void ReadPivotBase(Pivot pivot)
		{
			IntermediateFormatReader.Assert(null != pivot);
			ObjectType objectType = ObjectType.Pivot;
			Indexes indexes = new Indexes();
			this.ReadDataRegionBase(pivot);
			if (this.PreRead(objectType, indexes))
			{
				pivot.ColumnCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.RowCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.CellAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.ProcessingInnerGrouping = this.ReadProcessingInnerGrouping();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.CellPostSortAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				pivot.CellDataElementOutput = this.ReadDataElementOutputType(null);
			}
			this.PostRead(objectType, indexes);
		}

		private void ReadTablixBase(Tablix tablix)
		{
			IntermediateFormatReader.Assert(null != tablix);
			ObjectType objectType = ObjectType.Tablix;
			Indexes indexes = new Indexes();
			this.ReadDataRegionBase(tablix);
			if (this.PreRead(objectType, indexes))
			{
				tablix.ColumnCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablix.RowCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablix.CellAggregates = this.ReadDataAggregateInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablix.ProcessingInnerGrouping = this.ReadProcessingInnerGrouping();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablix.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				tablix.CellPostSortAggregates = this.ReadDataAggregateInfoList();
			}
			this.PostRead(objectType, indexes);
		}

		private CustomReportItem ReadCustomReportItemInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CustomReportItem;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItem customReportItem = new CustomReportItem(parent);
			this.ReadTablixBase(customReportItem);
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.Type = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.AltReportItem = this.ReadReportItemCollection(parent);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.Columns = this.ReadCustomReportItemHeadingList(customReportItem);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.Rows = this.ReadCustomReportItemHeadingList(customReportItem);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.DataRowCells = this.ReadDataCellsList();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.CellRunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.CellExprHostIDs = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItem.RenderReportItem = this.ReadReportItemCollection(parent);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItem;
		}

		private ChartHeading ReadChartHeading(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadChartHeadingInternals(parent);
		}

		private ChartHeading ReadChartHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.ChartHeading;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartHeading chartHeading = new ChartHeading();
			this.ReadPivotHeadingBase(chartHeading, parent);
			if (this.PreRead(objectType, indexes))
			{
				chartHeading.Labels = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeading.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeading.ChartGroupExpression = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartHeading.PlotTypesLine = this.ReadBoolList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartHeading;
		}

		private ChartHeading ReadChartHeadingReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartHeading == this.m_reader.ObjectType);
			IDOwner definitionObject = this.GetDefinitionObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(definitionObject is ChartHeading);
			return (ChartHeading)definitionObject;
		}

		private ChartDataPointList ReadChartDataPointList()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.ChartDataPointList == this.m_reader.ObjectType);
			int num = this.m_reader.ReadArray();
			ChartDataPointList chartDataPointList = new ChartDataPointList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointList.Add(this.ReadChartDataPoint());
			}
			this.m_reader.ReadEndObject();
			return chartDataPointList;
		}

		private ChartDataPoint ReadChartDataPoint()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataPoint;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartDataPoint chartDataPoint = new ChartDataPoint();
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.DataValues = this.ReadExpressionInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.DataLabel = this.ReadChartDataLabel();
			}
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					chartDataPoint.Action = this.ReadAction();
				}
				else
				{
					ActionItem actionItem = this.ReadActionItem();
					if (actionItem != null)
					{
						chartDataPoint.Action = new Action(actionItem, true);
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerType = this.ReadMarkerType();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerSize = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerStyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.DataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.DataElementOutput = this.ReadDataElementOutputType(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.ExprHostID = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataPoint.CustomProperties = this.ReadDataValueList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartDataPoint;
		}

		private ChartDataLabel ReadChartDataLabel()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataLabel;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartDataLabel chartDataLabel = new ChartDataLabel();
			if (this.PreRead(objectType, indexes))
			{
				chartDataLabel.Visible = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataLabel.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataLabel.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataLabel.Position = this.ReadDataLabelPosition();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartDataLabel.Rotation = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartDataLabel;
		}

		private MultiChart ReadMultiChart(ReportItem parent)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			return this.ReadMultiChartInternals(parent);
		}

		private MultiChart ReadMultiChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.MultiChart;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MultiChart multiChart = new MultiChart();
			this.ReadReportHierarchyNodeBase(multiChart, parent);
			if (this.PreRead(objectType, indexes))
			{
				multiChart.Layout = this.ReadMultiChartLayout();
			}
			if (this.PreRead(objectType, indexes))
			{
				multiChart.MaxCount = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				multiChart.SyncScale = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return multiChart;
		}

		private Axis ReadAxis()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Axis;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Axis axis = new Axis();
			if (this.PreRead(objectType, indexes))
			{
				axis.Visible = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Title = this.ReadChartTitle();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Margin = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MajorTickMarks = this.ReadTickMark();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MinorTickMarks = this.ReadTickMark();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MajorGridLines = this.ReadGridLines();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MinorGridLines = this.ReadGridLines();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MajorInterval = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.MinorInterval = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Reverse = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.CrossAt = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.AutoCrossAt = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Interlaced = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Scalar = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Min = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.Max = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.AutoScaleMin = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.AutoScaleMax = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.LogScale = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				axis.CustomProperties = this.ReadDataValueList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return axis;
		}

		private ChartTitle ReadChartTitle()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartTitle;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartTitle chartTitle = new ChartTitle();
			if (this.PreRead(objectType, indexes))
			{
				chartTitle.Caption = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartTitle.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartTitle.Position = this.ReadChartTitlePosition();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartTitle;
		}

		private Legend ReadLegend()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Legend;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Legend legend = new Legend();
			if (this.PreRead(objectType, indexes))
			{
				legend.Visible = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				legend.StyleClass = this.ReadStyle();
			}
			if (this.PreRead(objectType, indexes))
			{
				legend.Position = this.ReadLegendPosition();
			}
			if (this.PreRead(objectType, indexes))
			{
				legend.Layout = this.ReadLegendLayout();
			}
			if (this.PreRead(objectType, indexes))
			{
				legend.InsidePlotArea = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return legend;
		}

		private GridLines ReadGridLines()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.GridLines;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			GridLines gridLines = new GridLines();
			if (this.PreRead(objectType, indexes))
			{
				gridLines.ShowGridLines = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				gridLines.StyleClass = this.ReadStyle();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return gridLines;
		}

		private ThreeDProperties ReadThreeDProperties()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ThreeDProperties;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ThreeDProperties threeDProperties = new ThreeDProperties();
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Enabled = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.PerspectiveProjectionMode = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Rotation = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Inclination = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Perspective = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.HeightRatio = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.DepthRatio = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Shading = this.ReadShading();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.GapDepth = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.WallThickness = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.DrawingStyleCube = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				threeDProperties.Clustered = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return threeDProperties;
		}

		private PlotArea ReadPlotArea()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PlotArea;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			PlotArea plotArea = new PlotArea();
			if (this.PreRead(objectType, indexes))
			{
				plotArea.Origin = this.ReadPlotAreaOrigin();
			}
			if (this.PreRead(objectType, indexes))
			{
				plotArea.StyleClass = this.ReadStyle();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return plotArea;
		}

		private Chart ReadChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Chart;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Chart chart = new Chart(parent);
			this.ReadPivotBase(chart);
			if (this.PreRead(objectType, indexes))
			{
				chart.Columns = this.ReadChartHeading(chart);
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.Rows = this.ReadChartHeading(chart);
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.ChartDataPoints = this.ReadChartDataPointList();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.CellRunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.MultiChart = this.ReadMultiChart(chart);
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.Legend = this.ReadLegend();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.CategoryAxis = this.ReadAxis();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.ValueAxis = this.ReadAxis();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.StaticColumns = this.ReadChartHeadingReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.StaticRows = this.ReadChartHeadingReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.Type = this.ReadChartType();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.SubType = this.ReadChartSubType();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.Palette = this.ReadChartPalette();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.Title = this.ReadChartTitle();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.PointWidth = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.ThreeDProperties = this.ReadThreeDProperties();
			}
			if (this.PreRead(objectType, indexes))
			{
				chart.PlotArea = this.ReadPlotArea();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chart;
		}

		private ChartDataLabel.Positions ReadDataLabelPosition()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ChartDataLabel.Positions), num));
			return (ChartDataLabel.Positions)num;
		}

		private ChartDataPoint.MarkerTypes ReadMarkerType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ChartDataPoint.MarkerTypes), num));
			return (ChartDataPoint.MarkerTypes)num;
		}

		private MultiChart.Layouts ReadMultiChartLayout()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(MultiChart.Layouts), num));
			return (MultiChart.Layouts)num;
		}

		private Axis.TickMarks ReadTickMark()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Axis.TickMarks), num));
			return (Axis.TickMarks)num;
		}

		private ThreeDProperties.ShadingTypes ReadShading()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ThreeDProperties.ShadingTypes), num));
			return (ThreeDProperties.ShadingTypes)num;
		}

		private PlotArea.Origins ReadPlotAreaOrigin()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(PlotArea.Origins), num));
			return (PlotArea.Origins)num;
		}

		private Legend.LegendLayout ReadLegendLayout()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Legend.LegendLayout), num));
			return (Legend.LegendLayout)num;
		}

		private Legend.Positions ReadLegendPosition()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Legend.Positions), num));
			return (Legend.Positions)num;
		}

		private ChartTitle.Positions ReadChartTitlePosition()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ChartTitle.Positions), num));
			return (ChartTitle.Positions)num;
		}

		private Chart.ChartTypes ReadChartType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Chart.ChartTypes), num));
			return (Chart.ChartTypes)num;
		}

		private Chart.ChartSubTypes ReadChartSubType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Chart.ChartSubTypes), num));
			return (Chart.ChartSubTypes)num;
		}

		private Chart.ChartPalette ReadChartPalette()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Chart.ChartPalette), num));
			return (Chart.ChartPalette)num;
		}

		private Matrix ReadMatrixInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Matrix;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Matrix matrix = new Matrix(parent);
			this.ReadPivotBase(matrix);
			if (this.PreRead(objectType, indexes))
			{
				matrix.CornerReportItems = this.ReadReportItemCollection(matrix);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.Columns = this.ReadMatrixHeading(matrix);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.Rows = this.ReadMatrixHeading(matrix);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.CellReportItems = this.ReadReportItemCollection(matrix);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.CellIDs = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.PropagatedPageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.PropagatedPageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.InnerRowLevelWithPageBreak = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.MatrixRows = this.ReadMatrixRowList();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.MatrixColumns = this.ReadMatrixColumnList();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.GroupsBeforeRowHeaders = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.LayoutDirection = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.StaticColumns = this.ReadMatrixHeadingReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.StaticRows = this.ReadMatrixHeadingReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.UseOWC = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.OwcCellNames = this.ReadStringList();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.CellDataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.ColumnGroupingFixedHeader = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrix.RowGroupingFixedHeader = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrix;
		}

		private Pivot.ProcessingInnerGroupings ReadProcessingInnerGrouping()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Pivot.ProcessingInnerGroupings), num));
			return (Pivot.ProcessingInnerGroupings)num;
		}

		private MatrixRow ReadMatrixRow()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixRow;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixRow matrixRow = new MatrixRow();
			if (this.PreRead(objectType, indexes))
			{
				matrixRow.Height = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixRow.HeightValue = this.m_reader.ReadDouble();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixRow;
		}

		private MatrixColumn ReadMatrixColumn()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixColumn;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixColumn matrixColumn = new MatrixColumn();
			if (this.PreRead(objectType, indexes))
			{
				matrixColumn.Width = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixColumn.WidthValue = this.m_reader.ReadDouble();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixColumn;
		}

		private Table ReadTableInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Table;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			Table table = new Table(parent);
			this.ReadDataRegionBase(table);
			if (this.PreRead(objectType, indexes))
			{
				table.TableColumns = this.ReadTableColumnList();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.HeaderRows = this.ReadTableRowList(table);
			}
			if (this.PreRead(objectType, indexes))
			{
				table.HeaderRepeatOnNewPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.TableGroups = this.ReadTableGroup(table);
			}
			if (this.PreRead(objectType, indexes))
			{
				table.TableDetail = this.ReadTableDetail(table);
			}
			if (this.PreRead(objectType, indexes) && this.m_intermediateFormatVersion.IsRS2005_WithTableDetailFix)
			{
				table.DetailGroup = this.ReadTableGroupReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.FooterRows = this.ReadTableRowList(table);
			}
			if (this.PreRead(objectType, indexes))
			{
				table.FooterRepeatOnNewPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.PropagatedPageBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.GroupBreakAtStart = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.PropagatedPageBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.GroupBreakAtEnd = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.FillPage = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.UseOWC = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.OWCNonSharedStyles = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.RunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.DetailDataElementName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.DetailDataCollectionName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				table.DetailDataElementOutput = this.ReadDataElementOutputType(null);
			}
			if (this.PreRead(objectType, indexes))
			{
				table.FixedHeader = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return table;
		}

		private TableColumn ReadTableColumn()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableColumn;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableColumn tableColumn = new TableColumn();
			if (this.PreRead(objectType, indexes))
			{
				tableColumn.Width = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableColumn.WidthValue = this.m_reader.ReadDouble();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableColumn.Visibility = this.ReadVisibility();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableColumn.FixedHeader = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableColumn;
		}

		private OWCChart ReadOWCChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.OWCChart;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			OWCChart oWCChart = new OWCChart(parent);
			this.ReadDataRegionBase(oWCChart);
			if (this.PreRead(objectType, indexes))
			{
				oWCChart.ChartData = this.ReadChartColumnList();
			}
			if (this.PreRead(objectType, indexes))
			{
				oWCChart.ChartDefinition = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				oWCChart.DetailRunningValues = this.ReadRunningValueInfoList();
			}
			if (this.PreRead(objectType, indexes))
			{
				oWCChart.RunningValues = this.ReadRunningValueInfoList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return oWCChart;
		}

		private ChartColumn ReadChartColumn()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartColumn;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartColumn chartColumn = new ChartColumn();
			if (this.PreRead(objectType, indexes))
			{
				chartColumn.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartColumn.Value = this.ReadExpressionInfo();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartColumn;
		}

		private DataValue ReadDataValue()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataValue;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DataValue dataValue = new DataValue();
			if (this.PreRead(objectType, indexes))
			{
				dataValue.Name = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataValue.Value = this.ReadExpressionInfo();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataValue.ExprHostID = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return dataValue;
		}

		private ParameterInfo ReadParameterInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ParameterInfo parameterInfo = new ParameterInfo();
			this.ReadParameterBase(parameterInfo);
			this.RegisterParameterInfo(parameterInfo);
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.IsUserSupplied = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.Values = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.DynamicValidValues = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.DynamicDefaultValue = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.DependencyList = this.ReadParameterInfoRefCollection();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.ValidValues = this.ReadValidValueList();
			}
			if (this.PreRead(objectType, indexes))
			{
				parameterInfo.Labels = this.ReadStrings();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return parameterInfo;
		}

		private ProcessingMessage ReadProcessingMessage()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ProcessingMessage;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ProcessingMessage processingMessage = new ProcessingMessage();
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.Code = this.ReadProcessingErrorCode();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.Severity = this.ReadProcessingErrorSeverity();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.ObjectType = this.ReadProcessingErrorObjectType();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.ObjectName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.PropertyName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.Message = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.ProcessingMessages = this.ReadProcessingMessageList();
			}
			if (this.PreRead(objectType, indexes))
			{
				processingMessage.CommonCode = this.ReadCommonErrorCode();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return processingMessage;
		}

		private DataValueInstance ReadDataValueInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataValueInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DataValueInstance dataValueInstance = new DataValueInstance();
			if (this.PreRead(objectType, indexes))
			{
				dataValueInstance.Name = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				dataValueInstance.Value = this.ReadVariant();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return dataValueInstance;
		}

		private ProcessingErrorCode ReadProcessingErrorCode()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ProcessingErrorCode), num));
			return (ProcessingErrorCode)num;
		}

		private ErrorCode ReadCommonErrorCode()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(ErrorCode), num));
			return (ErrorCode)num;
		}

		private Severity ReadProcessingErrorSeverity()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(Severity), num));
			return (Severity)num;
		}

		private AspNetCore.ReportingServices.ReportProcessing.ObjectType ReadProcessingErrorObjectType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(AspNetCore.ReportingServices.ReportProcessing.ObjectType), num));
			return (AspNetCore.ReportingServices.ReportProcessing.ObjectType)num;
		}

		private DataType ReadDataType()
		{
			int num = this.m_reader.ReadEnum();
			IntermediateFormatReader.Assert(Enum.IsDefined(typeof(DataType), num));
			return (DataType)num;
		}

		private BookmarkInformation ReadBookmarkInformation()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.BookmarkInformation;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			BookmarkInformation bookmarkInformation = new BookmarkInformation();
			if (this.PreRead(objectType, indexes))
			{
				bookmarkInformation.Id = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				bookmarkInformation.Page = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return bookmarkInformation;
		}

		private DrillthroughInformation ReadDrillthroughInformation(bool hasTokensIDs)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DrillthroughInformation;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			DrillthroughInformation drillthroughInformation = new DrillthroughInformation();
			if (this.PreRead(objectType, indexes))
			{
				drillthroughInformation.ReportName = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				drillthroughInformation.ReportParameters = this.ReadDrillthroughParameters();
			}
			if (hasTokensIDs && this.PreRead(objectType, indexes))
			{
				drillthroughInformation.DataSetTokenIDs = this.ReadIntList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return drillthroughInformation;
		}

		private SenderInformation ReadSenderInformation()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SenderInformation;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SenderInformation senderInformation = new SenderInformation();
			if (this.PreRead(objectType, indexes))
			{
				senderInformation.StartHidden = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				senderInformation.ReceiverUniqueNames = this.ReadIntList();
			}
			if (this.PreRead(objectType, indexes))
			{
				senderInformation.ContainerUniqueNames = this.m_reader.ReadInt32s();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return senderInformation;
		}

		private ReceiverInformation ReadReceiverInformation()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReceiverInformation;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReceiverInformation receiverInformation = new ReceiverInformation();
			if (this.PreRead(objectType, indexes))
			{
				receiverInformation.StartHidden = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				receiverInformation.SenderUniqueName = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return receiverInformation;
		}

		private SortFilterEventInfo ReadSortFilterEventInfo(bool getDefinition)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SortFilterEventInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SortFilterEventInfo sortFilterEventInfo = new SortFilterEventInfo();
			if (this.PreRead(objectType, indexes))
			{
				sortFilterEventInfo.EventSource = (TextBox)this.ReadReportItemReference(getDefinition);
			}
			if (this.PreRead(objectType, indexes))
			{
				sortFilterEventInfo.EventSourceScopeInfo = this.ReadVariantLists(true);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return sortFilterEventInfo;
		}

		private void ReadInfoBaseBase(InfoBase infoBase)
		{
			IntermediateFormatReader.Assert(null != infoBase);
			ObjectType objectType = ObjectType.InfoBase;
			Indexes indexes = new Indexes();
			this.PostRead(objectType, indexes);
		}

		private OffsetInfo ReadSimpleOffsetInfo()
		{
			OffsetInfo offsetInfo = new OffsetInfo();
			offsetInfo.Offset = this.m_reader.ReadInt64();
			return offsetInfo;
		}

		private OffsetInfo ReadOffsetInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.OffsetInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			OffsetInfo offsetInfo = new OffsetInfo();
			this.ReadInfoBaseBase(offsetInfo);
			if (this.PreRead(objectType, indexes))
			{
				offsetInfo.Offset = this.m_reader.ReadInt64();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return offsetInfo;
		}

		private void ReadInstanceInfoBase(InstanceInfo instanceInfo)
		{
			IntermediateFormatReader.Assert(null != instanceInfo);
			ObjectType objectType = ObjectType.InstanceInfo;
			Indexes indexes = new Indexes();
			this.ReadInfoBaseBase(instanceInfo);
			this.PostRead(objectType, indexes);
		}

		private void ReadReportItemInstanceInfoBase(ReportItemInstanceInfo instanceInfo)
		{
			IntermediateFormatReader.Assert(null != instanceInfo);
			ObjectType objectType = ObjectType.ReportItemInstanceInfo;
			Indexes indexes = new Indexes();
			this.ReadInstanceInfoBase(instanceInfo);
			bool flag = false;
			ReportItem reportItemDef = instanceInfo.ReportItemDef;
			if (this.m_intermediateFormatVersion.IsRS2000_WithUnusedFieldsOptimization)
			{
				flag = true;
			}
			if ((!flag || (reportItemDef.StyleClass != null && reportItemDef.StyleClass.ExpressionList != null)) && this.PreRead(objectType, indexes))
			{
				instanceInfo.StyleAttributeValues = this.ReadVariants();
			}
			if ((!flag || reportItemDef.Visibility != null) && this.PreRead(objectType, indexes))
			{
				instanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			if ((!flag || reportItemDef.Label != null) && this.PreRead(objectType, indexes))
			{
				instanceInfo.Label = this.m_reader.ReadString();
			}
			if ((!flag || reportItemDef.Bookmark != null) && this.PreRead(objectType, indexes))
			{
				instanceInfo.Bookmark = this.m_reader.ReadString();
			}
			if ((!flag || reportItemDef.ToolTip != null) && this.PreRead(objectType, indexes))
			{
				instanceInfo.ToolTip = this.m_reader.ReadString();
			}
			if ((!flag || reportItemDef.CustomProperties != null) && this.PreRead(objectType, indexes))
			{
				instanceInfo.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
		}

		private NonComputedUniqueNames ReadNonComputedUniqueNames()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.NonComputedUniqueNames;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			NonComputedUniqueNames nonComputedUniqueNames = new NonComputedUniqueNames();
			if (this.PreRead(objectType, indexes))
			{
				nonComputedUniqueNames.UniqueName = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				nonComputedUniqueNames.ChildrenUniqueNames = this.ReadNonComputedUniqueNamess();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return nonComputedUniqueNames;
		}

		private void ReadInstanceInfoOwnerBase(InstanceInfoOwner owner)
		{
			IntermediateFormatReader.Assert(null != owner);
			ObjectType objectType = ObjectType.InstanceInfoOwner;
			Indexes indexes = new Indexes();
			if (this.PreRead(objectType, indexes))
			{
				if (this.m_intermediateFormatVersion.IsRS2000_WithOtherPageChunkSplit)
				{
					owner.OffsetInfo = this.ReadSimpleOffsetInfo();
				}
				else
				{
					owner.OffsetInfo = this.ReadOffsetInfo();
				}
			}
			this.PostRead(objectType, indexes);
		}

		private ReportItemInstance ReadReportItemInstance(ReportItem reportItemDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.LineInstance == this.m_reader.ObjectType)
			{
				return this.ReadLineInstanceInternals(reportItemDef);
			}
			if (ObjectType.RectangleInstance == this.m_reader.ObjectType)
			{
				return this.ReadRectangleInstanceInternals(reportItemDef);
			}
			if (ObjectType.ImageInstance == this.m_reader.ObjectType)
			{
				return this.ReadImageInstanceInternals(reportItemDef);
			}
			if (ObjectType.CheckBoxInstance == this.m_reader.ObjectType)
			{
				return this.ReadCheckBoxInstanceInternals(reportItemDef);
			}
			if (ObjectType.TextBoxInstance == this.m_reader.ObjectType)
			{
				return this.ReadTextBoxInstanceInternals(reportItemDef);
			}
			if (ObjectType.SubReportInstance == this.m_reader.ObjectType)
			{
				return this.ReadSubReportInstanceInternals(reportItemDef);
			}
			if (ObjectType.ActiveXControlInstance == this.m_reader.ObjectType)
			{
				return this.ReadActiveXControlInstanceInternals(reportItemDef);
			}
			if (ObjectType.ListInstance == this.m_reader.ObjectType)
			{
				return this.ReadListInstanceInternals(reportItemDef);
			}
			if (ObjectType.MatrixInstance == this.m_reader.ObjectType)
			{
				return this.ReadMatrixInstanceInternals(reportItemDef);
			}
			if (ObjectType.TableInstance == this.m_reader.ObjectType)
			{
				return this.ReadTableInstanceInternals(reportItemDef);
			}
			if (ObjectType.ChartInstance == this.m_reader.ObjectType)
			{
				return this.ReadChartInstanceInternals(reportItemDef);
			}
			if (ObjectType.CustomReportItemInstance == this.m_reader.ObjectType)
			{
				IntermediateFormatReader.Assert(reportItemDef is CustomReportItem);
				return this.ReadCustomReportItemInstanceInternals(reportItemDef as CustomReportItem);
			}
			IntermediateFormatReader.Assert(ObjectType.OWCChartInstance == this.m_reader.ObjectType);
			return this.ReadOWCChartInstanceInternals(reportItemDef);
		}

		private void ReadReportItemInstanceBase(ReportItemInstance reportItemInstance, ReportItem reportItemDef)
		{
			Global.Tracer.Assert(null != reportItemDef, "(null != reportItemDef)");
			this.ReadReportItemInstanceBase(reportItemInstance, ref reportItemDef);
		}

		private void ReadReportItemInstanceBase(ReportItemInstance reportItemInstance, ref ReportItem reportItemDef)
		{
			IntermediateFormatReader.Assert(null != reportItemInstance);
			ObjectType objectType = ObjectType.ReportItemInstance;
			Indexes indexes = new Indexes();
			this.ReadInstanceInfoOwnerBase(reportItemInstance);
			if (this.PreRead(objectType, indexes))
			{
				if (-1 == this.m_currentUniqueName)
				{
					reportItemInstance.UniqueName = this.m_reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
					reportItemInstance.UniqueName = this.m_currentUniqueName++;
				}
			}
			if (reportItemDef == null)
			{
				reportItemDef = this.ReadReportItemReference(true);
				indexes.CurrentIndex++;
			}
			Global.Tracer.Assert(null != reportItemDef, "(null != reportItemDef)");
			reportItemInstance.ReportItemDef = reportItemDef;
			this.PostRead(objectType, indexes);
		}

		private ReportItemInstance ReadReportItemInstanceReference()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Reference == this.m_reader.Token);
			IntermediateFormatReader.Assert(ObjectType.OWCChartInstance == this.m_reader.ObjectType || ObjectType.ChartInstance == this.m_reader.ObjectType);
			ReportItemInstance instanceObject = this.GetInstanceObject(this.m_reader.ReferenceValue);
			IntermediateFormatReader.Assert(instanceObject is OWCChartInstance || instanceObject is ChartInstance);
			return instanceObject;
		}

		private ReportInstance ReadReportInstance(Report reportDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportInstance reportInstance = new ReportInstance();
			this.ReadReportItemInstanceBase(reportInstance, reportDef);
			if (this.PreRead(objectType, indexes))
			{
				reportInstance.ReportItemColInstance = this.ReadReportItemColInstance(reportDef.ReportItems);
			}
			if (this.PreRead(objectType, indexes))
			{
				reportInstance.Language = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				reportInstance.NumberOfPages = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportInstance;
		}

		private ReportItemColInstance ReadReportItemColInstance(ReportItemCollection reportItemsDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemColInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ReportItemColInstance reportItemColInstance = new ReportItemColInstance();
			this.ReadInstanceInfoOwnerBase(reportItemColInstance);
			if (this.PreRead(objectType, indexes))
			{
				reportItemColInstance.ReportItemInstances = this.ReadReportItemInstanceList(reportItemsDef);
			}
			reportItemColInstance.ReportItemColDef = reportItemsDef;
			if (this.PreRead(objectType, indexes))
			{
				reportItemColInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return reportItemColInstance;
		}

		private LineInstance ReadLineInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.LineInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			LineInstance lineInstance = new LineInstance();
			this.ReadReportItemInstanceBase(lineInstance, ref reportItemDef);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return lineInstance;
		}

		private void UpdateUniqueNameForAction(Action actionDef)
		{
			if (-1 != this.m_currentUniqueName && actionDef != null)
			{
				Global.Tracer.Assert(this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
				if ((actionDef.StyleClass == null || actionDef.StyleClass.ExpressionList == null || 0 >= actionDef.StyleClass.ExpressionList.Count) && actionDef.ComputedActionItemsCount <= 0)
				{
					return;
				}
				this.m_currentUniqueName++;
			}
		}

		private TextBoxInstance ReadTextBoxInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.TextBoxInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TextBoxInstance textBoxInstance = new TextBoxInstance();
			this.ReadReportItemInstanceBase(textBoxInstance, ref reportItemDef);
			this.UpdateUniqueNameForAction(((TextBox)reportItemDef).Action);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return textBoxInstance;
		}

		private RectangleInstance ReadRectangleInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.RectangleInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RectangleInstance rectangleInstance = new RectangleInstance();
			this.ReadReportItemInstanceBase(rectangleInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				rectangleInstance.ReportItemColInstance = this.ReadReportItemColInstance(((Rectangle)reportItemDef).ReportItems);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return rectangleInstance;
		}

		private CheckBoxInstance ReadCheckBoxInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.CheckBoxInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CheckBoxInstance checkBoxInstance = new CheckBoxInstance();
			this.ReadReportItemInstanceBase(checkBoxInstance, ref reportItemDef);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return checkBoxInstance;
		}

		private ImageInstance ReadImageInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ImageInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ImageInstance imageInstance = new ImageInstance();
			this.ReadReportItemInstanceBase(imageInstance, ref reportItemDef);
			this.UpdateUniqueNameForAction(((Image)reportItemDef).Action);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return imageInstance;
		}

		private SubReportInstance ReadSubReportInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.SubReportInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			SubReportInstance subReportInstance = new SubReportInstance();
			this.ReadReportItemInstanceBase(subReportInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				subReportInstance.ReportInstance = this.ReadReportInstance(((SubReport)reportItemDef).Report);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return subReportInstance;
		}

		private ActiveXControlInstance ReadActiveXControlInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ActiveXControlInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ActiveXControlInstance activeXControlInstance = new ActiveXControlInstance();
			this.ReadReportItemInstanceBase(activeXControlInstance, ref reportItemDef);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return activeXControlInstance;
		}

		private ListInstance ReadListInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ListInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ListInstance listInstance = new ListInstance();
			this.ReadReportItemInstanceBase(listInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				listInstance.ListContents = this.ReadListContentInstanceList((List)reportItemDef);
			}
			if (this.PreRead(objectType, indexes))
			{
				listInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return listInstance;
		}

		private ListContentInstance ReadListContentInstance(List listDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListContentInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ListContentInstance listContentInstance = new ListContentInstance();
			this.ReadInstanceInfoOwnerBase(listContentInstance);
			if (this.PreRead(objectType, indexes))
			{
				listContentInstance.UniqueName = this.m_reader.ReadInt32();
			}
			listContentInstance.ListDef = listDef;
			if (this.PreRead(objectType, indexes))
			{
				listContentInstance.ReportItemColInstance = this.ReadReportItemColInstance(listDef.ReportItems);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return listContentInstance;
		}

		private MatrixInstance ReadMatrixInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.MatrixInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixInstance matrixInstance = new MatrixInstance();
			Matrix matrix = (Matrix)reportItemDef;
			this.ReadReportItemInstanceBase(matrixInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.CornerContent = this.ReadReportItemInstance(matrix.CornerReportItem);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.ColumnInstances = this.ReadMatrixHeadingInstanceList(matrix.Columns);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.RowInstances = this.ReadMatrixHeadingInstanceList(matrix.Rows);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.Cells = this.ReadMatrixCellInstancesList();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.InstanceCountOfInnerRowWithPageBreak = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return matrixInstance;
		}

		private MatrixHeadingInstance ReadMatrixHeadingInstance(MatrixHeading headingDef, int index, int totalCount)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixHeadingInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MatrixHeadingInstance matrixHeadingInstance = new MatrixHeadingInstance();
			bool flag = false;
			if (headingDef.Grouping != null && headingDef.Subtotal != null)
			{
				if (index == 0 && Subtotal.PositionType.Before == headingDef.Subtotal.Position)
				{
					goto IL_008a;
				}
				if (totalCount - 1 == index && headingDef.Subtotal.Position == Subtotal.PositionType.After)
				{
					goto IL_008a;
				}
			}
			goto IL_008c;
			IL_008c:
			this.ReadInstanceInfoOwnerBase(matrixHeadingInstance);
			if (this.PreRead(objectType, indexes))
			{
				matrixHeadingInstance.UniqueName = this.m_reader.ReadInt32();
			}
			matrixHeadingInstance.MatrixHeadingDef = headingDef;
			if (this.PreRead(objectType, indexes))
			{
				ReportItem reportItem = null;
				if (headingDef.Grouping == null)
				{
					Global.Tracer.Assert(null != headingDef.ReportItems, "(null != headingDef.ReportItems)");
					reportItem = headingDef.ReportItems[index];
				}
				else
				{
					reportItem = ((!flag) ? headingDef.ReportItem : headingDef.Subtotal.ReportItem);
				}
				matrixHeadingInstance.Content = this.ReadReportItemInstance(reportItem);
			}
			if (this.PreRead(objectType, indexes))
			{
				MatrixHeading subHeading = headingDef.SubHeading;
				if (flag)
				{
					while (subHeading != null && subHeading.Grouping != null)
					{
						subHeading = subHeading.SubHeading;
					}
				}
				matrixHeadingInstance.SubHeadingInstances = this.ReadMatrixHeadingInstanceList(subHeading);
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixHeadingInstance.IsSubtotal = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				matrixHeadingInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			if (matrixHeadingInstance.IsSubtotal && matrixHeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null)
			{
				this.RegisterMatrixHeadingInstanceObject(matrixHeadingInstance);
			}
			return matrixHeadingInstance;
			IL_008a:
			flag = true;
			goto IL_008c;
		}

		internal MatrixCellInstance ReadMatrixCellInstanceBase()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			if (ObjectType.MatrixCellInstance == this.m_reader.ObjectType)
			{
				MatrixCellInstance matrixCellInstance = new MatrixCellInstance();
				this.ReadMatrixCellInstance(matrixCellInstance);
				return matrixCellInstance;
			}
			IntermediateFormatReader.Assert(ObjectType.MatrixSubtotalCellInstance == this.m_reader.ObjectType);
			MatrixSubtotalCellInstance matrixSubtotalCellInstance = new MatrixSubtotalCellInstance();
			this.ReadMatrixSubtotalCellInstance(matrixSubtotalCellInstance);
			return matrixSubtotalCellInstance;
		}

		private void ReadMatrixCellInstance(MatrixCellInstance instance)
		{
			ObjectType objectType = ObjectType.MatrixCellInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			this.ReadInstanceInfoOwnerBase(instance);
			ReportItem reportItemDef = null;
			if (this.PreRead(objectType, indexes))
			{
				reportItemDef = this.ReadReportItemReference(true);
			}
			if (this.PreRead(objectType, indexes))
			{
				instance.Content = this.ReadReportItemInstance(reportItemDef);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
		}

		internal void ReadMatrixSubtotalCellInstance(MatrixSubtotalCellInstance instance)
		{
			ObjectType objectType = ObjectType.MatrixSubtotalCellInstance;
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			this.ReadInstanceInfoOwnerBase(instance);
			IntermediateFormatReader.Assert(this.m_reader.Read());
			this.ReadMatrixCellInstance(instance);
			int uniqueName = this.m_reader.ReadInt32();
			instance.SubtotalHeadingInstance = this.GetMatrixHeadingInstanceObject(uniqueName);
			this.m_reader.ReadEndObject();
		}

		private MultiChartInstance ReadMultiChartInstance(Chart chartDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			ObjectType objectType = ObjectType.MultiChartInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			MultiChartInstance multiChartInstance = new MultiChartInstance();
			if (this.PreRead(objectType, indexes))
			{
				multiChartInstance.ColumnInstances = this.ReadChartHeadingInstanceList(chartDef.Columns);
			}
			if (this.PreRead(objectType, indexes))
			{
				multiChartInstance.RowInstances = this.ReadChartHeadingInstanceList(chartDef.Rows);
			}
			if (this.PreRead(objectType, indexes))
			{
				multiChartInstance.DataPoints = this.ReadChartDataPointInstancesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return multiChartInstance;
		}

		private ChartInstance ReadChartInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ChartInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartInstance chartInstance = new ChartInstance();
			this.ReadReportItemInstanceBase(chartInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				chartInstance.MultiCharts = this.ReadMultiChartInstanceList((Chart)reportItemDef);
			}
			this.RegisterInstanceObject(chartInstance);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartInstance;
		}

		private ChartHeadingInstance ReadChartHeadingInstance(ChartHeading headingDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartHeadingInstance chartHeadingInstance = new ChartHeadingInstance();
			this.ReadInstanceInfoOwnerBase(chartHeadingInstance);
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstance.UniqueName = this.m_reader.ReadInt32();
			}
			chartHeadingInstance.ChartHeadingDef = headingDef;
			if (this.PreRead(objectType, indexes))
			{
				chartHeadingInstance.SubHeadingInstances = this.ReadChartHeadingInstanceList(headingDef.SubHeading);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartHeadingInstance;
		}

		private ChartDataPointInstance ReadChartDataPointInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			ObjectType objectType = ObjectType.ChartDataPointInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartDataPointInstance chartDataPointInstance = new ChartDataPointInstance();
			this.ReadInstanceInfoOwnerBase(chartDataPointInstance);
			if (this.PreRead(objectType, indexes))
			{
				chartDataPointInstance.UniqueName = this.m_reader.ReadInt32();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartDataPointInstance;
		}

		private AxisInstance ReadAxisInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.AxisInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			AxisInstance axisInstance = new AxisInstance();
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.UniqueName = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.Title = this.ReadChartTitleInstance();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.StyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MajorGridLinesStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MinorGridLinesStyleAttributeValues = this.ReadVariants();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MinValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MaxValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.CrossAtValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MajorIntervalValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.MinorIntervalValue = this.ReadVariant();
			}
			if (this.PreRead(objectType, indexes))
			{
				axisInstance.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return axisInstance;
		}

		private ChartTitleInstance ReadChartTitleInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartTitleInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			ChartTitleInstance chartTitleInstance = new ChartTitleInstance();
			if (this.PreRead(objectType, indexes))
			{
				chartTitleInstance.UniqueName = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartTitleInstance.Caption = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				chartTitleInstance.StyleAttributeValues = this.ReadVariants();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return chartTitleInstance;
		}

		private TableInstance ReadTableInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.TableInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableInstance tableInstance = new TableInstance();
			Table table = (Table)reportItemDef;
			this.ReadReportItemInstanceBase(tableInstance, ref reportItemDef);
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.HeaderRows != null))
			{
				tableInstance.HeaderRowInstances = this.ReadTableRowInstances(table.HeaderRows, -1);
			}
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.TableGroups != null))
			{
				tableInstance.TableGroupInstances = this.ReadTableGroupInstanceList(table.TableGroups);
			}
			tableInstance.TableDetailInstances = this.ReadTableDetailInstances(table, table.TableGroups, objectType, indexes);
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.FooterRows != null))
			{
				tableInstance.FooterRowInstances = this.ReadTableRowInstances(table.FooterRows, -1);
			}
			if (this.PreRead(objectType, indexes))
			{
				tableInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableInstance;
		}

		private TableDetailInstanceList ReadTableDetailInstances(Table tableDef, TableGroup tableGroup, ObjectType objectType, Indexes indexes)
		{
			TableDetailInstanceList result = null;
			bool flag = false;
			if (this.PreRead(objectType, indexes) && tableGroup == null && tableDef.TableDetail != null && tableDef.TableDetail.SimpleDetailRows)
			{
				Global.Tracer.Assert(this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
				flag = true;
				this.m_currentUniqueName = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || (tableGroup == null && tableDef.TableDetail != null)))
			{
				result = this.ReadTableDetailInstanceList(tableDef.TableDetail);
			}
			if (flag)
			{
				this.m_currentUniqueName = -1;
			}
			return result;
		}

		private TableGroupInstance ReadTableGroupInstance(TableGroup groupDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableGroupInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableGroupInstance tableGroupInstance = new TableGroupInstance();
			this.ReadInstanceInfoOwnerBase(tableGroupInstance);
			if (this.PreRead(objectType, indexes))
			{
				tableGroupInstance.UniqueName = this.m_reader.ReadInt32();
			}
			tableGroupInstance.TableGroupDef = groupDef;
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.HeaderRows != null))
			{
				tableGroupInstance.HeaderRowInstances = this.ReadTableRowInstances(groupDef.HeaderRows, -1);
			}
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.FooterRows != null))
			{
				tableGroupInstance.FooterRowInstances = this.ReadTableRowInstances(groupDef.FooterRows, -1);
			}
			if (this.PreRead(objectType, indexes) && (!this.m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.SubGroup != null))
			{
				tableGroupInstance.SubGroupInstances = this.ReadTableGroupInstanceList(groupDef.SubGroup);
			}
			tableGroupInstance.TableDetailInstances = this.ReadTableDetailInstances((Table)groupDef.DataRegionDef, groupDef.SubGroup, objectType, indexes);
			if (this.PreRead(objectType, indexes))
			{
				tableGroupInstance.ChildrenStartAndEndPages = this.ReadRenderingPagesRangesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableGroupInstance;
		}

		private TableDetailInstance ReadTableDetailInstance(TableDetail detailDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableDetailInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableDetailInstance tableDetailInstance = new TableDetailInstance();
			this.ReadInstanceInfoOwnerBase(tableDetailInstance);
			if (this.PreRead(objectType, indexes))
			{
				if (-1 == this.m_currentUniqueName)
				{
					tableDetailInstance.UniqueName = this.m_reader.ReadInt32();
				}
				else
				{
					tableDetailInstance.UniqueName = this.m_currentUniqueName++;
				}
			}
			tableDetailInstance.TableDetailDef = detailDef;
			int currentUniqueName = this.m_currentUniqueName;
			if (-1 != this.m_currentUniqueName && detailDef.DetailRows != null)
			{
				for (int i = 0; i < detailDef.DetailRows.Count; i++)
				{
					this.m_currentUniqueName++;
					if (detailDef.DetailRows[i] != null)
					{
						ReportItemCollection reportItems = detailDef.DetailRows[i].ReportItems;
						if (reportItems.NonComputedReportItems != null)
						{
							this.m_currentUniqueName += reportItems.NonComputedReportItems.Count;
						}
					}
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				tableDetailInstance.DetailRowInstances = this.ReadTableRowInstances(detailDef.DetailRows, currentUniqueName);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableDetailInstance;
		}

		internal TableDetailInstanceInfo ReadTableDetailInstanceInfo()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableDetailInstanceInfo;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableDetailInstanceInfo tableDetailInstanceInfo = new TableDetailInstanceInfo();
			this.ReadInstanceInfoBase(tableDetailInstanceInfo);
			if (this.PreRead(objectType, indexes))
			{
				tableDetailInstanceInfo.StartHidden = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableDetailInstanceInfo;
		}

		private TableRowInstance ReadTableRowInstance(TableRowList rowDefs, int index, int rowUniqueName)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRowInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableRowInstance tableRowInstance = new TableRowInstance();
			this.ReadInstanceInfoOwnerBase(tableRowInstance);
			if (this.PreRead(objectType, indexes))
			{
				if (-1 == rowUniqueName)
				{
					tableRowInstance.UniqueName = this.m_reader.ReadInt32();
				}
				else
				{
					tableRowInstance.UniqueName = rowUniqueName;
				}
			}
			tableRowInstance.TableRowDef = rowDefs[index];
			if (this.PreRead(objectType, indexes))
			{
				tableRowInstance.TableRowReportItemColInstance = this.ReadReportItemColInstance(tableRowInstance.TableRowDef.ReportItems);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableRowInstance;
		}

		private TableColumnInstance ReadTableColumnInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableColumnInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			TableColumnInstance tableColumnInstance = new TableColumnInstance();
			if (this.PreRead(objectType, indexes))
			{
				tableColumnInstance.UniqueName = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				tableColumnInstance.StartHidden = this.m_reader.ReadBoolean();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return tableColumnInstance;
		}

		private OWCChartInstance ReadOWCChartInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.OWCChartInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			OWCChartInstance oWCChartInstance = new OWCChartInstance();
			this.ReadReportItemInstanceBase(oWCChartInstance, ref reportItemDef);
			this.RegisterInstanceObject(oWCChartInstance);
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return oWCChartInstance;
		}

		private CustomReportItemInstance ReadCustomReportItemInstanceInternals(CustomReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.CustomReportItemInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItemInstance customReportItemInstance = new CustomReportItemInstance();
			this.ReadReportItemInstanceBase(customReportItemInstance, reportItemDef);
			if (this.PreRead(objectType, indexes))
			{
				if (reportItemDef.RenderReportItem != null && 1 == reportItemDef.RenderReportItem.Count)
				{
					customReportItemInstance.AltReportItemColInstance = this.ReadReportItemColInstance(reportItemDef.RenderReportItem);
				}
				else
				{
					customReportItemInstance.AltReportItemColInstance = this.ReadReportItemColInstance(reportItemDef.AltReportItem);
				}
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemInstance.ColumnInstances = this.ReadCustomReportItemHeadingInstanceList(reportItemDef.Columns);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemInstance.RowInstances = this.ReadCustomReportItemHeadingInstanceList(reportItemDef.Rows);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemInstance.Cells = this.ReadCustomReportItemCellInstancesList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItemInstance;
		}

		private CustomReportItemHeadingInstance ReadCustomReportItemHeadingInstance(CustomReportItemHeadingList headingDef, int index, int totalCount)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeadingInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItemHeadingInstance customReportItemHeadingInstance = new CustomReportItemHeadingInstance();
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.SubHeadingInstances = this.ReadCustomReportItemHeadingInstanceList(headingDef);
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingDefinition = this.ReadCustomReportItemHeadingReference();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingCellIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingSpan = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.CustomPropertyInstances = this.ReadDataValueInstanceList();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.Label = this.m_reader.ReadString();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.GroupExpressionValues = this.ReadVariantList(false);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItemHeadingInstance;
		}

		internal CustomReportItemCellInstance ReadCustomReportItemCellInstance()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemCellInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			CustomReportItemCellInstance customReportItemCellInstance = new CustomReportItemCellInstance();
			if (this.PreRead(objectType, indexes))
			{
				customReportItemCellInstance.RowIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemCellInstance.ColumnIndex = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				customReportItemCellInstance.DataValueInstances = this.ReadDataValueInstanceList();
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return customReportItemCellInstance;
		}

		private PageSectionInstance ReadPageSectionInstance(PageSection pageSectionDef)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSectionInstance;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			PageSectionInstance pageSectionInstance = new PageSectionInstance();
			this.ReadReportItemInstanceBase(pageSectionInstance, pageSectionDef);
			if (this.PreRead(objectType, indexes))
			{
				pageSectionInstance.PageNumber = this.m_reader.ReadInt32();
			}
			if (this.PreRead(objectType, indexes))
			{
				pageSectionInstance.ReportItemColInstance = this.ReadReportItemColInstance(pageSectionDef.ReportItems);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return pageSectionInstance;
		}

		private object ReadVariant()
		{
			return this.ReadVariant(true);
		}

		private object ReadVariant(bool readNextToken)
		{
			return this.ReadVariant(readNextToken, false);
		}

		private object ReadVariant(bool readNextToken, bool convertDBNull)
		{
			if (readNextToken)
			{
				IntermediateFormatReader.Assert(this.m_reader.Read());
			}
			switch (this.m_reader.Token)
			{
			case Token.Null:
				if (convertDBNull)
				{
					return DBNull.Value;
				}
				return null;
			case Token.String:
				return this.m_reader.StringValue;
			case Token.Char:
				return this.m_reader.CharValue;
			case Token.Boolean:
				return this.m_reader.BooleanValue;
			case Token.Int16:
				return this.m_reader.Int16Value;
			case Token.Int32:
				return this.m_reader.Int32Value;
			case Token.Int64:
				return this.m_reader.Int64Value;
			case Token.UInt16:
				return this.m_reader.UInt16Value;
			case Token.UInt32:
				return this.m_reader.UInt32Value;
			case Token.UInt64:
				return this.m_reader.UInt64Value;
			case Token.Byte:
				return this.m_reader.ByteValue;
			case Token.SByte:
				return this.m_reader.SByteValue;
			case Token.Single:
				return this.m_reader.SingleValue;
			case Token.Double:
				return this.m_reader.DoubleValue;
			case Token.Decimal:
				return this.m_reader.DecimalValue;
			case Token.DateTime:
				return this.m_reader.DateTimeValue;
			default:
				IntermediateFormatReader.Assert(Token.TimeSpan == this.m_reader.Token);
				return this.m_reader.TimeSpanValue;
			}
		}

		private RecordField[] ReadRecordFields()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			IntermediateFormatReader.Assert(Token.Array == this.m_reader.Token);
			int arrayLength = this.m_reader.ArrayLength;
			RecordField[] array = new RecordField[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = this.ReadRecordField();
			}
			return array;
		}

		private RecordField ReadRecordField()
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			if (this.m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordField;
			Indexes indexes = new Indexes();
			IntermediateFormatReader.Assert(Token.Object == this.m_reader.Token);
			IntermediateFormatReader.Assert(objectType == this.m_reader.ObjectType);
			RecordField recordField = new RecordField();
			if (this.PreRead(objectType, indexes))
			{
				DataFieldStatus fieldStatus = default(DataFieldStatus);
				recordField.FieldValue = this.ReadFieldValue(out fieldStatus);
				recordField.FieldStatus = fieldStatus;
			}
			if (this.PreRead(objectType, indexes))
			{
				recordField.IsAggregationField = this.m_reader.ReadBoolean();
			}
			if (this.PreRead(objectType, indexes))
			{
				recordField.FieldPropertyValues = this.ReadVariantList(false);
			}
			this.PostRead(objectType, indexes);
			this.m_reader.ReadEndObject();
			return recordField;
		}

		private object ReadFieldValue(out DataFieldStatus fieldStatus)
		{
			IntermediateFormatReader.Assert(this.m_reader.Read());
			fieldStatus = DataFieldStatus.None;
			switch (this.m_reader.Token)
			{
			case Token.Null:
				return DBNull.Value;
			case Token.String:
				return this.m_reader.StringValue;
			case Token.Char:
				return this.m_reader.CharValue;
			case Token.Boolean:
				return this.m_reader.BooleanValue;
			case Token.Int16:
				return this.m_reader.Int16Value;
			case Token.Int32:
				return this.m_reader.Int32Value;
			case Token.Int64:
				return this.m_reader.Int64Value;
			case Token.UInt16:
				return this.m_reader.UInt16Value;
			case Token.UInt32:
				return this.m_reader.UInt32Value;
			case Token.UInt64:
				return this.m_reader.UInt64Value;
			case Token.Byte:
				return this.m_reader.ByteValue;
			case Token.SByte:
				return this.m_reader.SByteValue;
			case Token.Single:
				return this.m_reader.SingleValue;
			case Token.Double:
				return this.m_reader.DoubleValue;
			case Token.Decimal:
				return this.m_reader.DecimalValue;
			case Token.DateTime:
				return this.m_reader.DateTimeValue;
			case Token.TimeSpan:
				return this.m_reader.TimeSpanValue;
			case Token.Guid:
				return this.m_reader.GuidValue;
			case Token.DataFieldInfo:
				fieldStatus = this.m_reader.DataFieldInfo;
				return null;
			default:
				IntermediateFormatReader.Assert(Token.TypedArray == this.m_reader.Token);
				if (Token.Byte == this.m_reader.ArrayType)
				{
					return this.m_reader.BytesValue;
				}
				if (Token.Int32 == this.m_reader.ArrayType)
				{
					return this.m_reader.Int32sValue;
				}
				IntermediateFormatReader.Assert(Token.Char == this.m_reader.ArrayType);
				return this.m_reader.CharsValue;
			}
		}
	}
}
