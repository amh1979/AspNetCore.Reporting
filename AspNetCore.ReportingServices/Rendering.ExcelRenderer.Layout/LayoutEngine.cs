using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Util;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class LayoutEngine : ALayout
	{
		internal sealed class RowInfo : IStorable, IPersistable
		{
			private int m_height;

			private List<IRowItemStruct> m_items;

			[NonSerialized]
			private static Declaration m_declaration = RowInfo.GetDeclaration();

			internal int Height
			{
				get
				{
					return this.m_height;
				}
				set
				{
					this.m_height = value;
				}
			}

			internal List<IRowItemStruct> Items
			{
				get
				{
					if (this.m_items == null)
					{
						this.m_items = new List<IRowItemStruct>();
					}
					return this.m_items;
				}
			}

			public int Size
			{
				get
				{
					return 4 + ItemSizes.SizeOf(this.m_items);
				}
			}

			internal RowInfo()
			{
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(RowInfo.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Height:
						writer.Write(this.m_height);
						break;
					case MemberName.Items:
						writer.Write(this.m_items);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(RowInfo.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Height:
						this.m_height = reader.ReadInt32();
						break;
					case MemberName.Items:
						this.m_items = reader.ReadGenericListOfRIFObjects<IRowItemStruct>();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.ExcelRowInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (RowInfo.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.Items, ObjectType.RIFObjectList, ObjectType.IRowItemStruct));
					return new Declaration(ObjectType.ExcelRowInfo, ObjectType.None, list);
				}
				return RowInfo.m_declaration;
			}
		}

		internal sealed class ColumnInfo
		{
			private const byte m_outlineLevelMask = 15;

			private const byte m_collapsedMask = 16;

			private int m_width;

			private Stack<ItemInfo> m_styleStack = new Stack<ItemInfo>();

			private List<TablixMemberInfo> m_tablixStructures;

			private TablixMemberInfo m_lastColumnHeader;

			private IBlockerInfo m_columnBlocker;

			private Stack<TablixBlockerInfo> m_tablixBlockers;

			private byte m_flags;

			internal int Width
			{
				get
				{
					return this.m_width;
				}
			}

			internal Stack<ItemInfo> Stack
			{
				get
				{
					return this.m_styleStack;
				}
			}

			internal List<TablixMemberInfo> TablixStructs
			{
				get
				{
					return this.m_tablixStructures;
				}
				set
				{
					this.m_tablixStructures = value;
				}
			}

			internal IBlockerInfo Blocker
			{
				get
				{
					return this.m_columnBlocker;
				}
				set
				{
					this.m_columnBlocker = value;
				}
			}

			internal Stack<TablixBlockerInfo> TablixBlockers
			{
				get
				{
					return this.m_tablixBlockers;
				}
				set
				{
					this.m_tablixBlockers = value;
				}
			}

			internal TablixMemberInfo LastColumnHeader
			{
				get
				{
					return this.m_lastColumnHeader;
				}
				set
				{
					this.m_lastColumnHeader = value;
				}
			}

			internal byte OutlineLevel
			{
				get
				{
					return (byte)(this.m_flags & 0xF);
				}
				set
				{
					if (value < 8)
					{
						this.m_flags &= 240;
						this.m_flags |= (byte)(value & 0xF);
					}
				}
			}

			internal bool Collapsed
			{
				get
				{
					return (this.m_flags & 0x10) != 0;
				}
				set
				{
					if (value)
					{
						this.m_flags |= 16;
					}
					else
					{
						this.m_flags &= 239;
					}
				}
			}

			internal ColumnInfo()
			{
			}

			internal ColumnInfo(int width)
			{
				this.m_width = width;
			}
		}

		internal interface IBlockerInfo
		{
			int TopRow
			{
				get;
			}

			int RightColumn
			{
				get;
			}

			int BottomRow
			{
				get;
			}

			int LeftColumn
			{
				get;
			}
		}

		internal class ItemInfo : IBlockerInfo
		{
			private const ushort m_togglePositionMask = 15;

			private const ushort m_typeMask = 4080;

			private const ushort m_isHiddenMask = 4096;

			private const ushort m_isMergedMask = 8192;

			private int m_rowTop;

			private int m_rowBottom;

			private int m_colLeft;

			private int m_colRight;

			private IColor m_backgroundColor;

			private BorderProperties m_leftBorder;

			private BorderProperties m_topBorder;

			private BorderProperties m_rightBorder;

			private BorderProperties m_bottomBorder;

			private BorderProperties m_diagonal;

			private ushort m_flags;

			public int TopRow
			{
				get
				{
					return this.m_rowTop;
				}
			}

			public int RightColumn
			{
				get
				{
					return this.m_colRight;
				}
			}

			public int BottomRow
			{
				get
				{
					return this.m_rowBottom;
				}
			}

			public int LeftColumn
			{
				get
				{
					return this.m_colLeft;
				}
			}

			internal IColor BackgroundColor
			{
				get
				{
					return this.m_backgroundColor;
				}
			}

			public bool IsHidden
			{
				get
				{
					return (this.m_flags & 0x1000) != 0;
				}
				set
				{
					if (value)
					{
						this.m_flags |= 4096;
					}
					else
					{
						this.m_flags &= 61439;
					}
				}
			}

			public TogglePosition TogglePosition
			{
				get
				{
					return (TogglePosition)(byte)(this.m_flags & 0xF);
				}
				set
				{
					this.m_flags &= 65520;
					this.m_flags |= (ushort)(value & (TogglePosition)15);
				}
			}

			internal bool IsMerged
			{
				get
				{
					return (this.m_flags & 0x2000) != 0;
				}
			}

			internal byte Type
			{
				get
				{
					return (byte)((this.m_flags & 0xFF0) >> 4);
				}
			}

			internal ItemInfo()
			{
			}

			internal ItemInfo(int top, int bottom, int colLeft, int colRight, bool isMerged, BorderInfo borders, byte type, bool isHidden, TogglePosition togglePosition)
			{
				this.m_rowTop = top;
				this.m_rowBottom = bottom;
				this.m_colLeft = colLeft;
				this.m_colRight = colRight;
				this.m_flags = (ushort)(togglePosition & (TogglePosition)15);
				this.m_flags |= (ushort)(type << 4 & 0xFF0);
				if (isMerged)
				{
					this.m_flags |= 8192;
				}
				if (isHidden)
				{
					this.m_flags |= 4096;
				}
				this.FillBorders(borders);
			}

			private void FillBorders(BorderInfo borders)
			{
				if (borders != null)
				{
					if (borders.BackgroundColor != null)
					{
						this.m_backgroundColor = borders.BackgroundColor;
					}
					if (borders.TopBorder != null && !borders.OmitBorderTop)
					{
						this.m_topBorder = borders.TopBorder;
					}
					if (borders.BottomBorder != null && !borders.OmitBorderBottom)
					{
						this.m_bottomBorder = borders.BottomBorder;
					}
					if (borders.LeftBorder != null)
					{
						this.m_leftBorder = borders.LeftBorder;
					}
					if (borders.RightBorder != null)
					{
						this.m_rightBorder = borders.RightBorder;
					}
					if (borders.Diagonal != null)
					{
						this.m_diagonal = borders.Diagonal;
					}
				}
			}

			internal void FillBorders(RPLStyleProps style, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
			{
				BorderInfo.FillAllBorders(style, ref this.m_leftBorder, ref this.m_rightBorder, ref this.m_topBorder, ref this.m_bottomBorder, ref this.m_backgroundColor, excel);
				if (omitBorderTop)
				{
					this.m_topBorder = null;
				}
				if (omitBorderBottom)
				{
					this.m_bottomBorder = null;
				}
			}

			internal void RenderBackground(IStyle style)
			{
				if (this.m_backgroundColor != null)
				{
					style.BackgroundColor = this.m_backgroundColor;
				}
			}

			internal void RenderBorders(IExcelGenerator excel, int currentRow, int currentColumn)
			{
				if (this.m_diagonal != null)
				{
					this.m_diagonal.Render(excel.GetCellStyle());
				}
				if (this.m_topBorder != null && currentRow == this.m_rowTop)
				{
					this.m_topBorder.Render(excel.GetCellStyle());
				}
				if (this.m_bottomBorder != null && currentRow == this.m_rowBottom)
				{
					this.m_bottomBorder.Render(excel.GetCellStyle());
				}
				if (this.m_leftBorder != null && currentColumn == this.m_colLeft)
				{
					this.m_leftBorder.Render(excel.GetCellStyle());
				}
				if (this.m_rightBorder != null && currentColumn == this.m_colRight)
				{
					this.m_rightBorder.Render(excel.GetCellStyle());
				}
			}
		}

		internal class TextBoxItemInfo : ItemInfo
		{
			private bool m_canGrow;

			private bool m_canShrink;

			internal bool CanGrow
			{
				get
				{
					return this.m_canGrow;
				}
			}

			internal bool CanShrink
			{
				get
				{
					return this.m_canShrink;
				}
			}

			internal TextBoxItemInfo()
			{
			}

			internal TextBoxItemInfo(int top, int bottom, int colLeft, int colRight, bool isMerged, bool canGrow, bool canShrink, BorderInfo borders, byte type, bool isHidden, TogglePosition togglePosition)
				: base(top, bottom, colLeft, colRight, isMerged, borders, type, isHidden, togglePosition)
			{
				this.m_canGrow = canGrow;
				this.m_canShrink = canShrink;
			}
		}

		internal class TablixItemInfo : IBlockerInfo
		{
			private int m_rowTop;

			private int m_rowBottom;

			private int m_colLeft;

			private int m_colRight;

			public int TopRow
			{
				get
				{
					return this.m_rowTop;
				}
			}

			public int RightColumn
			{
				get
				{
					return this.m_colRight;
				}
			}

			public int BottomRow
			{
				get
				{
					return this.m_rowBottom;
				}
			}

			public int LeftColumn
			{
				get
				{
					return this.m_colLeft;
				}
			}

			internal TablixItemInfo()
			{
			}

			public TablixItemInfo(int top, int bottom, int colLeft, int colRight)
			{
				this.m_rowTop = top;
				this.m_rowBottom = bottom;
				this.m_colLeft = colLeft;
				this.m_colRight = colRight;
			}
		}

		internal class TablixBlockerInfo : TablixItemInfo
		{
			private BlockOutlines m_blockOutlines;

			private int m_bodyTop;

			private int m_bodyLeft;

			private int m_bodyRight;

			public BlockOutlines BlockOutlines
			{
				get
				{
					return this.m_blockOutlines;
				}
				set
				{
					this.m_blockOutlines = value;
				}
			}

			public int BodyTopRow
			{
				get
				{
					return this.m_bodyTop;
				}
			}

			public int BodyBottomRow
			{
				get
				{
					return base.BottomRow;
				}
			}

			public int BodyLeftColumn
			{
				get
				{
					return this.m_bodyLeft;
				}
			}

			public int BodyRightColumn
			{
				get
				{
					return this.m_bodyRight;
				}
			}

			internal TablixBlockerInfo()
			{
			}

			public TablixBlockerInfo(int top, int bottom, int colLeft, int colRight, int bodyTop, int bodyLeft, int bodyRight)
				: base(top, bottom, colLeft, colRight)
			{
				this.m_bodyTop = bodyTop;
				this.m_bodyLeft = bodyLeft;
				this.m_bodyRight = bodyRight;
			}
		}

		internal class TablixMemberInfo : TablixItemInfo
		{
			private const byte m_togglePositionMask = 15;

			private const byte m_hasToggleMask = 16;

			private const byte m_isHiddenMask = 32;

			private byte m_flags;

			private int m_recursiveToggleLevel;

			public bool IsHidden
			{
				get
				{
					return (this.m_flags & 0x20) != 0;
				}
			}

			public TogglePosition TogglePosition
			{
				get
				{
					return (TogglePosition)(byte)(this.m_flags & 0xF);
				}
			}

			public int RecursiveToggleLevel
			{
				get
				{
					return this.m_recursiveToggleLevel;
				}
			}

			public bool HasToggle
			{
				get
				{
					return (this.m_flags & 0x10) != 0;
				}
				set
				{
					if (value)
					{
						this.m_flags |= 16;
					}
					else
					{
						this.m_flags &= 239;
					}
				}
			}

			internal TablixMemberInfo()
			{
			}

			public TablixMemberInfo(int top, int bottom, int colLeft, int colRight, TablixMemberStruct tablixMember)
				: base(top, bottom, colLeft, colRight)
			{
				this.m_flags = (byte)(tablixMember.TogglePosition & (TogglePosition)15);
				if (tablixMember.HasToggle)
				{
					this.m_flags |= 16;
				}
				if (tablixMember.IsHidden)
				{
					this.m_flags |= 32;
				}
				this.m_recursiveToggleLevel = tablixMember.RecursiveToggleLevel;
			}
		}

		internal interface IRowItemStruct : IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			object RPLSource
			{
				get;
			}

			int Left
			{
				get;
			}

			int Width
			{
				get;
			}

			int Height
			{
				get;
			}

			int GenerationIndex
			{
				get;
			}

			byte State
			{
				get;
			}
		}

		internal class RowItemStruct : IRowItemStruct, IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			[StaticReference]
			private object m_rplSource;

			private int m_left;

			private int m_width;

			private int m_height;

			private int m_generationIndex;

			private byte m_state;

			private bool m_isDefaultLine = true;

			private string m_subreportLanguage;

			private Dictionary<string, ToggleParent> m_toggleParents;

			private bool m_useRPLStream;

			[NonSerialized]
			private static Declaration m_declaration = RowItemStruct.GetDeclaration();

			public object RPLSource
			{
				get
				{
					return this.m_rplSource;
				}
			}

			public int Left
			{
				get
				{
					return this.m_left;
				}
			}

			public int Width
			{
				get
				{
					return this.m_width;
				}
			}

			public int Height
			{
				get
				{
					return this.m_height;
				}
			}

			public int GenerationIndex
			{
				get
				{
					return this.m_generationIndex;
				}
			}

			public byte State
			{
				get
				{
					return this.m_state;
				}
			}

			public Dictionary<string, ToggleParent> ToggleParents
			{
				get
				{
					return this.m_toggleParents;
				}
			}

			internal bool IsDefaultLine
			{
				get
				{
					return this.m_isDefaultLine;
				}
				set
				{
					this.m_isDefaultLine = value;
				}
			}

			internal bool UseRPLStream
			{
				get
				{
					return this.m_useRPLStream;
				}
			}

			internal string SubreportLanguage
			{
				get
				{
					return this.m_subreportLanguage;
				}
			}

			public int Size
			{
				get
				{
					return 27 + ItemSizes.SizeOf(this.m_subreportLanguage) + ItemSizes.SizeOf(this.m_toggleParents);
				}
			}

			internal RowItemStruct()
			{
			}

			internal RowItemStruct(object rplSource, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
				: this(rplSource, left, width, height, generationIndex, state, subreportLanguage, true, toggleParents)
			{
			}

			internal RowItemStruct(object rplSource, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, bool isDefaultLine, Dictionary<string, ToggleParent> toggleParents)
			{
				this.m_rplSource = rplSource;
				this.m_left = left;
				this.m_width = width;
				this.m_height = height;
				this.m_generationIndex = generationIndex;
				this.m_state = state;
				this.m_isDefaultLine = isDefaultLine;
				this.m_subreportLanguage = subreportLanguage;
				this.m_toggleParents = toggleParents;
				this.m_useRPLStream = (this.m_rplSource is long);
			}

			public int CompareTo(object obj)
			{
				return this.CompareTo((IRowItemStruct)obj);
			}

			public int CompareTo(IRowItemStruct other)
			{
				return this.m_left.CompareTo(other.Left);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(RowItemStruct.m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.UseRPLStream:
						writer.Write(this.m_useRPLStream);
						break;
					case MemberName.RPLSource:
						if (this.m_useRPLStream)
						{
							long num = -1L;
							try
							{
								num = (long)this.m_rplSource;
							}
							catch (InvalidCastException)
							{
								RSTrace.ExcelRendererTracer.Assert(false, "The RPL source object is corrupt");
							}
							if (num >= 0)
							{
								writer.Write(num);
							}
							else
							{
								RSTrace.ExcelRendererTracer.Assert(false, "The RPL source object is corrupt");
							}
						}
						else
						{
							int num2 = scalabilityCache.StoreStaticReference(this.m_rplSource);
							writer.Write((long)num2);
						}
						break;
					case MemberName.Left:
						writer.Write(this.m_left);
						break;
					case MemberName.Width:
						writer.Write(this.m_width);
						break;
					case MemberName.Height:
						writer.Write(this.m_height);
						break;
					case MemberName.GenerationIndex:
						writer.Write(this.m_generationIndex);
						break;
					case MemberName.State:
						writer.Write(this.m_state);
						break;
					case MemberName.IsDefaultLine:
						writer.Write(this.m_isDefaultLine);
						break;
					case MemberName.Language:
						writer.Write(this.m_subreportLanguage);
						break;
					case MemberName.ToggleParent:
						writer.WriteStringRIFObjectDictionary(this.m_toggleParents);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(RowItemStruct.m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.UseRPLStream:
						this.m_useRPLStream = reader.ReadBoolean();
						break;
					case MemberName.RPLSource:
						if (this.m_useRPLStream)
						{
							long num = reader.ReadInt64();
							if (num >= 0)
							{
								this.m_rplSource = num;
							}
							else
							{
								RSTrace.ExcelRendererTracer.Assert(false, "The RPL source object is corrupt");
							}
						}
						else
						{
							long num2 = reader.ReadInt64();
							this.m_rplSource = scalabilityCache.FetchStaticReference((int)num2);
						}
						break;
					case MemberName.Left:
						this.m_left = reader.ReadInt32();
						break;
					case MemberName.Width:
						this.m_width = reader.ReadInt32();
						break;
					case MemberName.Height:
						this.m_height = reader.ReadInt32();
						break;
					case MemberName.GenerationIndex:
						this.m_generationIndex = reader.ReadInt32();
						break;
					case MemberName.State:
						this.m_state = reader.ReadByte();
						break;
					case MemberName.IsDefaultLine:
						this.m_isDefaultLine = reader.ReadBoolean();
						break;
					case MemberName.Language:
						this.m_subreportLanguage = reader.ReadString();
						break;
					case MemberName.ToggleParent:
						this.m_toggleParents = reader.ReadStringRIFObjectDictionary<ToggleParent>();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowItemStruct;
			}

			internal static Declaration GetDeclaration()
			{
				if (RowItemStruct.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.UseRPLStream, Token.Boolean));
					list.Add(new MemberInfo(MemberName.RPLSource, Token.Int64));
					list.Add(new MemberInfo(MemberName.Left, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Int32));
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.GenerationIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.IsDefaultLine, Token.Boolean));
					list.Add(new MemberInfo(MemberName.Language, Token.String));
					list.Add(new MemberInfo(MemberName.ToggleParent, ObjectType.StringRIFObjectDictionary, ObjectType.ToggleParent));
					return new Declaration(ObjectType.RowItemStruct, ObjectType.None, list);
				}
				return RowItemStruct.m_declaration;
			}
		}

		internal abstract class TablixItemStruct : IRowItemStruct, IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			protected int m_left;

			protected int m_width;

			protected int m_height;

			protected int m_generationIndex;

			[NonSerialized]
			private static Declaration m_declaration = TablixItemStruct.GetDeclaration();

			public object RPLSource
			{
				get
				{
					return null;
				}
			}

			public int Left
			{
				get
				{
					return this.m_left;
				}
			}

			public int Width
			{
				get
				{
					return this.m_width;
				}
			}

			public int Height
			{
				get
				{
					return this.m_height;
				}
			}

			public int GenerationIndex
			{
				get
				{
					return this.m_generationIndex;
				}
			}

			public byte State
			{
				get
				{
					return 0;
				}
			}

			public virtual bool IsHidden
			{
				get
				{
					return false;
				}
			}

			public virtual TogglePosition TogglePosition
			{
				get
				{
					return TogglePosition.None;
				}
				set
				{
				}
			}

			public virtual int Size
			{
				get
				{
					return 16;
				}
			}

			protected TablixItemStruct(int left, int width, int height, int generationIndex)
			{
				this.m_left = left;
				this.m_width = width;
				this.m_height = height;
				this.m_generationIndex = generationIndex;
			}

			public int CompareTo(object obj)
			{
				return this.CompareTo((IRowItemStruct)obj);
			}

			public int CompareTo(IRowItemStruct other)
			{
				return this.m_left.CompareTo(other.Left);
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(TablixItemStruct.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Left:
						writer.Write(this.m_left);
						break;
					case MemberName.Width:
						writer.Write(this.m_width);
						break;
					case MemberName.Height:
						writer.Write(this.m_height);
						break;
					case MemberName.GenerationIndex:
						writer.Write(this.m_generationIndex);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(TablixItemStruct.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Left:
						this.m_left = reader.ReadInt32();
						break;
					case MemberName.Width:
						this.m_width = reader.ReadInt32();
						break;
					case MemberName.Height:
						this.m_height = reader.ReadInt32();
						break;
					case MemberName.GenerationIndex:
						this.m_generationIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public virtual void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.TablixItemStruct;
			}

			internal static Declaration GetDeclaration()
			{
				if (TablixItemStruct.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Left, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Int32));
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.GenerationIndex, Token.Int32));
					return new Declaration(ObjectType.TablixItemStruct, ObjectType.None, list);
				}
				return TablixItemStruct.m_declaration;
			}
		}

		internal class TablixStruct : TablixItemStruct
		{
			private int m_rowHeaderWidth;

			private int m_columnHeaderHeight;

			private bool m_rtl;

			[NonSerialized]
			private static Declaration m_declaration = TablixStruct.GetDeclaration();

			public int ColumnHeaderHeight
			{
				get
				{
					return this.m_columnHeaderHeight;
				}
			}

			public int RowHeaderWidth
			{
				get
				{
					return this.m_rowHeaderWidth;
				}
			}

			public bool RTL
			{
				get
				{
					return this.m_rtl;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 8 + 1;
				}
			}

			internal TablixStruct()
				: base(0, 0, 0, 0)
			{
			}

			internal TablixStruct(int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
				: base(left, width, height, generationIndex)
			{
				this.m_rowHeaderWidth = rowHeaderWidth;
				this.m_columnHeaderHeight = columnHeaderHeight;
				this.m_rtl = rtl;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(TablixStruct.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.RowHeaderWidth:
						writer.Write(this.m_rowHeaderWidth);
						break;
					case MemberName.ColumnHeaderHeight:
						writer.Write(this.m_columnHeaderHeight);
						break;
					case MemberName.RTL:
						writer.Write(this.m_rtl);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(TablixStruct.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.RowHeaderWidth:
						this.m_rowHeaderWidth = reader.ReadInt32();
						break;
					case MemberName.ColumnHeaderHeight:
						this.m_columnHeaderHeight = reader.ReadInt32();
						break;
					case MemberName.RTL:
						this.m_rtl = reader.ReadBoolean();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public override void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.TablixStruct;
			}

			internal new static Declaration GetDeclaration()
			{
				if (TablixStruct.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowHeaderWidth, Token.Int32));
					list.Add(new MemberInfo(MemberName.ColumnHeaderHeight, Token.Int32));
					list.Add(new MemberInfo(MemberName.RTL, Token.Boolean));
					return new Declaration(ObjectType.TablixStruct, ObjectType.None, list);
				}
				return TablixStruct.m_declaration;
			}
		}

		internal class TablixMemberStruct : TablixItemStruct
		{
			[NonSerialized]
			private const byte m_togglePositionMask = 15;

			[NonSerialized]
			private const byte m_hasToggleMask = 16;

			[NonSerialized]
			private const byte m_isHiddenMask = 32;

			[NonSerialized]
			private const byte m_hasLabelMask = 64;

			private byte m_flags;

			private string m_uniqueName;

			private int m_recursiveToggleLevel;

			[NonSerialized]
			private static Declaration m_declaration = TablixMemberStruct.GetDeclaration();

			public bool HasToggle
			{
				get
				{
					return (this.m_flags & 0x10) != 0;
				}
			}

			public override bool IsHidden
			{
				get
				{
					return (this.m_flags & 0x20) != 0;
				}
			}

			public bool HasLabel
			{
				get
				{
					return (this.m_flags & 0x40) != 0;
				}
			}

			public override TogglePosition TogglePosition
			{
				get
				{
					return (TogglePosition)(byte)(this.m_flags & 0xF);
				}
			}

			public string UniqueName
			{
				get
				{
					return this.m_uniqueName;
				}
			}

			public int RecursiveToggleLevel
			{
				get
				{
					return this.m_recursiveToggleLevel;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 1 + 4 + ItemSizes.SizeOf(this.m_uniqueName);
				}
			}

			internal TablixMemberStruct()
				: base(0, 0, 0, 0)
			{
			}

			internal TablixMemberStruct(int left, int width, int height, int generationIndex, bool hasToggle, bool isHidden, TogglePosition togglePosition, bool hasLabel, string uniqueName, int recursiveToggleLevel)
				: base(left, width, height, generationIndex)
			{
				this.m_flags = (byte)(togglePosition & (TogglePosition)15);
				if (hasToggle)
				{
					this.m_flags |= 16;
				}
				if (isHidden)
				{
					this.m_flags |= 32;
				}
				if (hasLabel)
				{
					this.m_flags |= 64;
				}
				this.m_uniqueName = uniqueName;
				this.m_recursiveToggleLevel = recursiveToggleLevel;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(TablixMemberStruct.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Flags:
						writer.Write(this.m_flags);
						break;
					case MemberName.UniqueName:
						writer.Write(this.m_uniqueName);
						break;
					case MemberName.RecursiveLevel:
						writer.Write(this.m_recursiveToggleLevel);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(TablixMemberStruct.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Flags:
						this.m_flags = reader.ReadByte();
						break;
					case MemberName.UniqueName:
						this.m_uniqueName = reader.ReadString();
						break;
					case MemberName.RecursiveLevel:
						this.m_recursiveToggleLevel = reader.ReadInt32();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(false);
						break;
					}
				}
			}

			public override void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.TablixMemberStruct;
			}

			internal new static Declaration GetDeclaration()
			{
				if (TablixMemberStruct.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Flags, Token.Byte));
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
					return new Declaration(ObjectType.TablixMemberStruct, ObjectType.None, list);
				}
				return TablixMemberStruct.m_declaration;
			}
		}

		private const int MAX_ROW_HEIGHT = 8180;

		private const string InvalidImage = "InvalidImage";

		private const double MinimumRowPrecentage = 0.004;

		private const double MinimumColumnPrecentage = 0.001;

		private static readonly char[] BulletChars = new char[3]
		{
			'•',
			'◦',
			'▪'
		};

		private static readonly char[,] RomanNumerals = new char[3, 3]
		{
			{
				'i',
				'v',
				'x'
			},
			{
				'x',
				'l',
				'c'
			},
			{
				'c',
				'd',
				'm'
			}
		};

		private string m_reportLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

		private bool m_renderFooterInBody;

		private bool m_renderHeaderInBody = true;

		private HeaderFooterLayout m_headerLayout;

		private HeaderFooterLayout m_footerLayout;

		private int m_headerHeight;

		private AspNetCore.ReportingServices.Rendering.ExcelRenderer.Util.HashSet<int> m_columnEdges;

		private ColumnInfo[] m_columns;

		private Dictionary<int, int> m_columnEdgesMap;

		private ScalableDictionary<int, RowInfo> m_rows;

		private ScalableList<RowInfo> m_rowInfo;

		private Dictionary<int, int> m_rowEdgesMap;

		private bool? m_summaryRowAfter = null;

		private bool? m_summaryColumnAfter = null;

		private Stack<IBlockerInfo> m_rowBlockers = new Stack<IBlockerInfo>();

		private IScalabilityCache m_scalabilityCache;

		private bool m_onFirstSection = true;

		private bool m_onLastSection;

		internal override bool HeaderInBody
		{
			get
			{
				if (!this.m_renderHeaderInBody)
				{
					return !this.m_onFirstSection;
				}
				return true;
			}
		}

		internal override bool FooterInBody
		{
			get
			{
				if (!this.m_renderFooterInBody)
				{
					return !this.m_onLastSection;
				}
				return true;
			}
		}

		internal override bool? SummaryRowAfter
		{
			get
			{
				return this.m_summaryRowAfter;
			}
			set
			{
				this.m_summaryRowAfter = value;
			}
		}

		internal override bool? SummaryColumnAfter
		{
			get
			{
				return this.m_summaryColumnAfter;
			}
			set
			{
				this.m_summaryColumnAfter = value;
			}
		}

		internal IScalabilityCache ScalabilityCache
		{
			get
			{
				return this.m_scalabilityCache;
			}
		}

		internal LayoutEngine(RPLReport report, bool headerInBody, CreateAndRegisterStream streamDelegate)
			: base(report)
		{
			this.m_renderHeaderInBody = headerInBody;
			this.InitCache(streamDelegate);
			this.m_columnEdges = new AspNetCore.ReportingServices.Rendering.ExcelRenderer.Util.HashSet<int>();
			this.m_rows = new ScalableDictionary<int, RowInfo>(1, this.ScalabilityCache, 113, 13);
		}

		private void AddColumnEdge(int edge)
		{
			this.m_columnEdges.Add(edge);
		}

		private void AddRowEdge(int edge)
		{
			IDisposable disposable = null;
			this.AddRowEdge(edge, false, out disposable);
		}

		private RowInfo AddRowEdge(int edge, bool returnRowInfo, out IDisposable rowRef)
		{
			RowInfo rowInfo = null;
			rowRef = null;
			if (this.m_rows.TryGetAndPin(edge, out rowInfo, out rowRef))
			{
				if (returnRowInfo)
				{
					return rowInfo;
				}
				if (rowRef != null)
				{
					rowRef.Dispose();
					rowRef = null;
				}
				return null;
			}
			rowInfo = new RowInfo();
			if (returnRowInfo)
			{
				rowRef = this.m_rows.AddAndPin(edge, rowInfo);
				return rowInfo;
			}
			this.m_rows.Add(edge, rowInfo);
			return null;
		}

		private void AddRowItemStruct(int top, IRowItemStruct rowItem)
		{
			IDisposable disposable = null;
			int edge = top + rowItem.Height;
			int edge2 = rowItem.Left + rowItem.Width;
			RowInfo rowInfo = this.AddRowEdge(top, true, out disposable);
			if (disposable != null)
			{
				rowInfo.Items.Add(rowItem);
				disposable.Dispose();
			}
			this.AddRowEdge(edge);
			this.AddColumnEdge(rowItem.Left);
			this.AddColumnEdge(edge2);
		}

		internal override void AddReportItem(object rplSource, int top, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
		{
			RowItemStruct rowItem = new RowItemStruct(rplSource, left, width, height, generationIndex, state, subreportLanguage, toggleParents);
			this.AddRowItemStruct(top, rowItem);
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, bool isToggglable, int generationIndex, RPLTablixMemberCell member, TogglePosition togglePosition)
		{
			this.AddRowItemStruct(top, new TablixMemberStruct(left, width, height, ALayout.TablixStructStart + ALayout.TablixStructGenerationOffset * generationIndex - member.TablixMemberDef.Level, isToggglable, member.ToggleCollapse, togglePosition, member.GroupLabel != null, member.UniqueName, member.RecursiveToggleLevel));
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
		{
			this.AddRowItemStruct(top, new TablixStruct(left, width, height, generationIndex, rowHeaderWidth, columnHeaderHeight, rtl));
		}

		internal override void SetIsLastSection(bool isLastSection)
		{
			this.m_onLastSection = isLastSection;
		}

		internal override ALayout GetPageHeaderLayout(float width, float height)
		{
			if (!this.HeaderInBody)
			{
				if (this.m_headerLayout == null)
				{
					this.m_headerLayout = new HeaderFooterLayout(base.m_report, width, height);
				}
				return this.m_headerLayout;
			}
			if (this.m_onFirstSection)
			{
				this.m_headerHeight = LayoutConvert.ConvertMMTo20thPoints((double)height);
			}
			return this;
		}

		internal override ALayout GetPageFooterLayout(float width, float height)
		{
			if (!this.FooterInBody)
			{
				if (this.m_footerLayout == null)
				{
					this.m_footerLayout = new HeaderFooterLayout(base.m_report, width, height);
				}
				return this.m_footerLayout;
			}
			return this;
		}

		internal override void CompleteSection()
		{
			this.m_onFirstSection = false;
		}

		internal override void CompletePage()
		{
			this.m_columnEdges.Add(0);
			List<int> list = new List<int>(this.m_columnEdges);
			list.Sort();
			int num = list.Count - 1;
			this.m_columns = new ColumnInfo[num];
			this.m_columnEdgesMap = new Dictionary<int, int>(num + 1);
			int num2 = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				ColumnInfo columnInfo = new ColumnInfo(list[i] - num2);
				this.m_columns[i - 1] = columnInfo;
				this.m_columnEdgesMap.Add(num2, i - 1);
				num2 = list[i];
			}
			this.m_columnEdgesMap.Add(num2, list.Count - 1);
			this.m_columnEdges = null;
			this.AddRowEdge(0);
			List<int> list2 = new List<int>(this.m_rows.Keys);
			list2.Sort();
			int num3 = list2.Count - 1;
			this.m_rowInfo = new ScalableList<RowInfo>(0, this.ScalabilityCache, 100);
			this.m_rowEdgesMap = new Dictionary<int, int>(num3 + 1);
			int num4 = list2[0];
			int num5 = 0;
			for (int j = 1; j < list2.Count; j++)
			{
				int num6 = list2[j];
				int num7 = num6 - num4;
				RowInfo rowInfo = null;
				using (this.m_rows.GetAndPin(num4, out rowInfo))
				{
					if (rowInfo != null)
					{
						rowInfo.Height = Math.Min(num7, 8180);
					}
					else
					{
						RSTrace.ExcelRendererTracer.Assert(rowInfo != null, "Null row info object");
					}
				}
				this.m_rowInfo.Add(rowInfo);
				this.m_rows.Remove(num4);
				this.m_rowEdgesMap.Add(num4, num5);
				num4 += rowInfo.Height;
				num5++;
				while (num7 > 8180)
				{
					num7 -= 8180;
					RowInfo rowInfo2 = new RowInfo();
					rowInfo2.Height = Math.Min(num7, 8180);
					this.m_rowInfo.Add(rowInfo2);
					this.m_rowEdgesMap.Add(num4, num5);
					num4 += rowInfo2.Height;
					num5++;
				}
			}
			this.m_rowEdgesMap.Add(num4, num5);
			RowInfo rowInfo3 = null;
			using (this.m_rows.GetAndPin(num4, out rowInfo3))
			{
				if (rowInfo3 != null)
				{
					if (rowInfo3.Items != null && rowInfo3.Items.Count > 0)
					{
						foreach (IRowItemStruct item in rowInfo3.Items)
						{
							if (this.m_rowInfo.Count > 0 && (item.Height == 0 || item.Width == 0))
							{
								RowItemStruct rowItemStruct = item as RowItemStruct;
								RSTrace.ExcelRendererTracer.Assert(null != rowItemStruct, "The row item structure object corresponding to a line cannot be null");
								rowItemStruct.IsDefaultLine = false;
								RowInfo rowInfo4 = null;
								using (this.m_rowInfo.GetAndPin(this.m_rowInfo.Count - 1, out rowInfo4))
								{
									if (rowInfo4 != null)
									{
										rowInfo4.Items.Add(rowItemStruct);
									}
									else
									{
										RSTrace.ExcelRendererTracer.Assert(rowInfo4 != null, "Null row info object");
									}
								}
							}
						}
					}
				}
				else
				{
					RSTrace.ExcelRendererTracer.Assert(rowInfo3 != null, "Null row info object");
				}
			}
			if (this.m_rows != null)
			{
				this.m_rows.Clear();
				this.m_rows = null;
			}
			if (this.m_columns.Length == 0)
			{
				this.m_columns = new ColumnInfo[1];
				this.m_columns[0] = new ColumnInfo(0);
			}
			if (this.m_rowInfo.Count == 0)
			{
				this.m_rowInfo.Add(new RowInfo());
			}
		}

		internal void RenderPageToExcel(IExcelGenerator excel, string key, Dictionary<string, BorderInfo> sharedBorderCache, Dictionary<string, ImageInformation> sharedImageCache)
		{
			if (this.m_columns.Length > excel.MaxColumns)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxColExceededInSheet(this.m_columns.Length.ToString(CultureInfo.InvariantCulture), excel.MaxColumns.ToString(CultureInfo.InvariantCulture)));
			}
			if (this.m_rowInfo.Count > excel.MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(this.m_rowInfo.Count.ToString(CultureInfo.InvariantCulture), excel.MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			this.m_reportLanguage = base.m_report.Language;
			RPLPageContent rPLPageContent = base.m_report.RPLPaginatedPages[0];
			bool isPortrait = default(bool);
			int pageSizeIndex = PageSizeIndex.GetPageSizeIndex(rPLPageContent.PageLayout.PageWidth, rPLPageContent.PageLayout.PageHeight, out isPortrait);
			float num = 0f;
			if (this.m_headerLayout != null)
			{
				num = this.m_headerLayout.Height;
			}
			float num2 = 0f;
			if (this.m_footerLayout != null)
			{
				num2 = this.m_footerLayout.Height;
			}
			excel.SetPageContraints(pageSizeIndex, isPortrait, LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginTop)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginBottom)));
			excel.SetMargins(LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginTop + num)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginBottom + num2)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginLeft)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginRight)));
			int num3 = 0;
			excel.SetColumnExtents(0, this.m_columns.Length - 1);
			if (!this.m_renderHeaderInBody)
			{
				if (this.m_headerLayout != null)
				{
					string left = default(string);
					string center = default(string);
					string right = default(string);
					this.m_headerLayout.RenderStrings(base.m_report, excel, out left, out center, out right);
					excel.AddHeader(left, center, right);
				}
			}
			else if (this.m_headerHeight > 0)
			{
				excel.AddPrintTitle(0, this.m_rowEdgesMap[this.m_headerHeight] - 1);
			}
			if (!this.m_renderFooterInBody && this.m_footerLayout != null)
			{
				string left2 = default(string);
				string center2 = default(string);
				string right2 = default(string);
				this.m_footerLayout.RenderStrings(base.m_report, excel, out left2, out center2, out right2);
				excel.AddFooter(left2, center2, right2);
			}
			if (this.m_headerHeight > 0)
			{
				excel.AddFreezePane(this.m_rowEdgesMap[this.m_headerHeight], 0);
			}
			int num4 = 0;
			int num5 = this.m_rowInfo.Count;
			if (1 == num5 && (this.m_rowInfo[0].Items == null || this.m_rowInfo[0].Items.Count == 0))
			{
				this.RenderEmptyPageToExcel(excel, rPLPageContent.PageLayout, sharedBorderCache);
			}
			else
			{
				int num6 = 0;
				while (num5 > 0)
				{
					int num7 = Math.Min(excel.RowBlockSize, num5);
					for (int i = 0; i < num7; i++)
					{
						int rowIndex = num6 + i;
						excel.AddRow(rowIndex);
					}
					for (int j = 0; j < num7; j++)
					{
						int num8 = num6 + j;
						bool flag = false;
						bool flag2 = false;
						bool rowCanGrow = false;
						bool rowCanShrink = false;
						bool collapsed = false;
						byte b = 0;
						RowInfo rowInfo = null;
						IEnumerator<IRowItemStruct> enumerator = null;
						using (this.m_rowInfo.GetAndPin(num8, out rowInfo))
						{
							if (rowInfo != null)
							{
								rowInfo.Items.Sort();
								enumerator = (IEnumerator<IRowItemStruct>)(object)rowInfo.Items.GetEnumerator();
							}
							else
							{
								RSTrace.ExcelRendererTracer.Assert(false, "Missing expected scalable list item with index");
							}
						}
						int num9 = -1;
						if (enumerator.MoveNext())
						{
							num9 = enumerator.Current.Left;
						}
						num3 = 0;
						excel.SetRowContext(num8);
						List<IRowItemStruct> list = new List<IRowItemStruct>();
						for (int k = 0; k < this.m_columns.Length; k++)
						{
							ColumnInfo columnInfo = this.m_columns[k];
							excel.SetColumnContext(k);
							if (num9 == num3)
							{
								list.Add(enumerator.Current);
								while (enumerator.MoveNext())
								{
									num9 = enumerator.Current.Left;
									if (num9 != num3)
									{
										break;
									}
									list.Add(enumerator.Current);
								}
								if (num3 == num9)
								{
									num9 = -1;
								}
								list.Sort(LayoutEngine.CompareGenerationIndex);
								bool flag3 = false;
								bool flag4 = false;
								foreach (IRowItemStruct item in list)
								{
									this.RenderNewItem(item, num4, num8, excel, key, sharedBorderCache, sharedImageCache, ref flag3, ref flag4);
								}
								list.Clear();
								flag |= (flag3 || flag4);
							}
							if (columnInfo.Stack.Count > 0)
							{
								ItemInfo itemInfo = null;
								foreach (ItemInfo item2 in columnInfo.Stack)
								{
									if (item2.TogglePosition != 0)
									{
										switch (item2.TogglePosition)
										{
										case TogglePosition.Above:
										case TogglePosition.Below:
											if (item2.LeftColumn == k)
											{
												b = (byte)(b + 1);
												if (item2.IsHidden)
												{
													collapsed = true;
												}
											}
											break;
										case TogglePosition.Left:
										case TogglePosition.Right:
											if (item2.TopRow == num8)
											{
												columnInfo.OutlineLevel++;
												if (item2.IsHidden)
												{
													columnInfo.Collapsed = true;
												}
											}
											break;
										}
									}
									if ((!item2.IsMerged || item2.LeftColumn == k) && item2.BackgroundColor != null && itemInfo == null)
									{
										itemInfo = item2;
									}
									item2.RenderBorders(excel, num8, k);
									if (item2.Type == 9 || item2.Type == 11 || item2.Type == 14 || item2.Type == 21)
									{
										flag2 = true;
									}
									if (!flag2 && item2.Type == 7)
									{
										this.CalculateRowFlagsFromTextBoxes(item2, num8, ref rowCanGrow, ref rowCanShrink, ref flag);
									}
								}
								if (itemInfo != null)
								{
									itemInfo.RenderBackground(excel.GetCellStyle());
								}
								else if (columnInfo.Stack.Count > 0)
								{
									excel.GetCellStyle().BackgroundColor = null;
								}
							}
							while (columnInfo.Stack.Count > 0 && columnInfo.Stack.Peek().BottomRow <= num8)
							{
								columnInfo.Stack.Pop();
							}
							if (columnInfo.TablixStructs != null)
							{
								foreach (TablixMemberInfo tablixStruct in columnInfo.TablixStructs)
								{
									if (tablixStruct.HasToggle)
									{
										this.HandleTablixOutline(tablixStruct, num8, k, ref b, ref collapsed);
									}
								}
							}
							num3 += columnInfo.Width;
						}
						ColumnInfo[] columns = this.m_columns;
						foreach (ColumnInfo columnInfo2 in columns)
						{
							if (columnInfo2.TablixStructs != null)
							{
								bool flag5 = false;
								for (int num10 = columnInfo2.TablixStructs.Count - 1; num10 >= 0; num10--)
								{
									TablixMemberInfo tablixMemberInfo = columnInfo2.TablixStructs[num10];
									if (tablixMemberInfo.BottomRow == num8)
									{
										columnInfo2.TablixStructs.RemoveAt(num10);
										if (!flag5 && (tablixMemberInfo.TogglePosition == TogglePosition.Above || tablixMemberInfo.TogglePosition == TogglePosition.Below))
										{
											flag5 = true;
											columnInfo2.LastColumnHeader = tablixMemberInfo;
										}
									}
								}
							}
							if (columnInfo2.TablixBlockers != null)
							{
								while (columnInfo2.TablixBlockers.Count > 0 && columnInfo2.TablixBlockers.Peek().BottomRow == num8)
								{
									columnInfo2.TablixBlockers.Pop();
								}
								if (columnInfo2.TablixBlockers.Count == 0)
								{
									columnInfo2.TablixBlockers = null;
								}
							}
						}
						while (this.m_rowBlockers.Count > 0 && this.m_rowBlockers.Peek().BottomRow == num8)
						{
							this.m_rowBlockers.Pop();
						}
						this.CalculateAutoSizeFlag(rowCanGrow, rowCanShrink, this.m_rowInfo[num8].Height, ref flag);
						this.m_rowEdgesMap.Remove(num4);
						num4 += rowInfo.Height;
						excel.SetRowProperties(num8, this.m_rowInfo[num8].Height, b, collapsed, flag && !flag2);
						this.m_rowInfo[num8] = null;
					}
					num6 += num7;
					num5 -= num7;
				}
				excel.SetSummaryRowAfter(this.m_summaryRowAfter ?? true);
				excel.SetSummaryColumnToRight(this.m_summaryColumnAfter ?? true);
				for (int m = 0; m < this.m_columns.Length; m++)
				{
					ColumnInfo columnInfo3 = this.m_columns[m];
					double widthInPoints = (double)columnInfo3.Width / 20.0;
					excel.SetColumnProperties(m, widthInPoints, columnInfo3.OutlineLevel, columnInfo3.Collapsed);
				}
			}
		}

		internal void RenderEmptyPageToExcel(IExcelGenerator excel, RPLPageLayout pageLayout, Dictionary<string, BorderInfo> sharedBorderCache)
		{
			if (pageLayout != null)
			{
				excel.AddRow(0);
				excel.SetRowContext(0);
				excel.SetColumnContext(0);
				BorderInfo borderDefinitionFromCache = this.GetBorderDefinitionFromCache(string.Empty, sharedBorderCache, pageLayout.Style.SharedProperties, false, false, excel);
				ItemInfo itemInfo = new ItemInfo(0, 0, 0, 0, false, borderDefinitionFromCache, 3, false, TogglePosition.None);
				if (pageLayout.Style.NonSharedProperties != null)
				{
					itemInfo.FillBorders(pageLayout.Style.NonSharedProperties, false, false, excel);
				}
				ItemInfo itemInfo2 = null;
				if (itemInfo.BackgroundColor != null)
				{
					itemInfo2 = itemInfo;
				}
				itemInfo.RenderBorders(excel, 0, 0);
				if (itemInfo2 != null)
				{
					itemInfo2.RenderBackground(excel.GetCellStyle());
				}
				this.m_rowEdgesMap.Remove(0);
				int heightIn20thPoints = LayoutConvert.ConvertMMTo20thPoints((double)pageLayout.PageHeight);
				excel.SetRowProperties(0, heightIn20thPoints, 0, false, false);
				this.m_rowInfo[0] = null;
				double widthInPoints = LayoutConvert.ConvertMMToPoints((double)pageLayout.PageWidth);
				excel.SetColumnProperties(0, widthInPoints, 0, false);
				this.m_columns[0] = null;
			}
		}

		private void RenderNewItem(IRowItemStruct item, int top, int topRow, IExcelGenerator excel, string pageContentKey, Dictionary<string, BorderInfo> sharedBorderCache, Dictionary<string, ImageInformation> sharedImageCache, ref bool autosizableGrow, ref bool autosizableShrink)
		{
			bool flag = false;
			int num;
			if (item.Height > 0)
			{
				num = this.m_rowEdgesMap[top + item.Height] - 1;
				if (num > topRow && this.m_rowEdgesMap.ContainsKey(top + item.Height - 1) && !(item.RPLSource is RPLImageProps))
				{
					num--;
					flag = true;
				}
			}
			else
			{
				num = topRow;
			}
			int num2 = this.m_columnEdgesMap[item.Left];
			int num3;
			if (item.Width > 0)
			{
				num3 = this.m_columnEdgesMap[item.Left + item.Width] - 1;
				if (num3 > num2 && this.m_columnEdgesMap.ContainsKey(item.Left + item.Width - 1) && !(item.RPLSource is RPLImageProps))
				{
					num3--;
				}
			}
			else
			{
				num3 = num2;
			}
			int width = item.Width;
			bool flag2 = false;
			if (item is TablixMemberStruct)
			{
				TablixMemberStruct tablixMemberStruct = (TablixMemberStruct)item;
				if (tablixMemberStruct.HasLabel)
				{
					excel.AddBookmarkTarget(tablixMemberStruct.UniqueName);
				}
				TablixMemberInfo tablixMemberInfo = new TablixMemberInfo(topRow, num, num2, num3, tablixMemberStruct);
				if (this.m_columns[num2].TablixBlockers != null && this.m_columns[num2].TablixBlockers.Count > 0)
				{
					TablixBlockerInfo tablixBlockerInfo = this.m_columns[num2].TablixBlockers.Peek();
					if (tablixBlockerInfo != null && tablixBlockerInfo.BlockOutlines != 0)
					{
						switch (tablixMemberStruct.TogglePosition)
						{
						case TogglePosition.Above:
						case TogglePosition.Below:
							if ((tablixBlockerInfo.BlockOutlines & BlockOutlines.Columns) != 0)
							{
								tablixMemberInfo.HasToggle = false;
							}
							break;
						case TogglePosition.Left:
						case TogglePosition.Right:
							if ((tablixBlockerInfo.BlockOutlines & BlockOutlines.Rows) != 0)
							{
								tablixMemberInfo.HasToggle = false;
							}
							break;
						}
					}
				}
				for (int i = num2; i <= num3; i++)
				{
					if (this.m_columns[i].TablixStructs == null)
					{
						this.m_columns[i].TablixStructs = new List<TablixMemberInfo>();
					}
					this.m_columns[i].TablixStructs.Add(tablixMemberInfo);
				}
			}
			else if (item is TablixStruct)
			{
				TablixStruct tablixStruct = (TablixStruct)item;
				int bodyTop = this.m_rowEdgesMap[top + tablixStruct.ColumnHeaderHeight];
				int bodyLeft = tablixStruct.RTL ? this.m_columnEdgesMap[tablixStruct.Left] : (this.m_columnEdgesMap[tablixStruct.Left + tablixStruct.RowHeaderWidth] - 1);
				int bodyRight = tablixStruct.RTL ? (this.m_columnEdgesMap[tablixStruct.Left + tablixStruct.Width + tablixStruct.RowHeaderWidth] - 1) : (this.m_columnEdgesMap[tablixStruct.Left + tablixStruct.Width] - 1);
				TablixBlockerInfo tablixBlockerInfo2 = new TablixBlockerInfo(topRow, num, num2, num3, bodyTop, bodyLeft, bodyRight);
				BlockOutlines blockOutlines = BlockOutlines.None;
				if (this.HandleBlocking((IBlockerInfo)tablixBlockerInfo2, TogglePosition.Left, out flag2))
				{
					for (int j = num2; j <= num3; j++)
					{
						this.m_columns[j].Blocker = tablixBlockerInfo2;
					}
				}
				else
				{
					blockOutlines = BlockOutlines.Columns;
				}
				if (this.HandleBlocking((IBlockerInfo)tablixBlockerInfo2, TogglePosition.Above, out flag2))
				{
					this.m_rowBlockers.Push(tablixBlockerInfo2);
				}
				else
				{
					blockOutlines |= BlockOutlines.Rows;
				}
				tablixBlockerInfo2.BlockOutlines = blockOutlines;
				for (int k = num2; k <= num3; k++)
				{
					if (this.m_columns[k].TablixBlockers == null)
					{
						this.m_columns[k].TablixBlockers = new Stack<TablixBlockerInfo>();
					}
					this.m_columns[k].TablixBlockers.Push(tablixBlockerInfo2);
				}
			}
			else
			{
				ItemInfo itemInfo = null;
				byte b = 0;
				BorderInfo borderInfo = null;
				RPLPageLayout rPLPageLayout = item.RPLSource as RPLPageLayout;
				RowItemStruct rowItemStruct = item as RowItemStruct;
				RSTrace.ExcelRendererTracer.Assert(rowItemStruct != null, "The row item cannot be null");
				if (rPLPageLayout == null)
				{
					RPLTextBox rPLTextBox = item.RPLSource as RPLTextBox;
					RPLItemProps rPLItemProps;
					if (rPLTextBox != null)
					{
						if (rPLTextBox.StartOffset > 0)
						{
							rPLItemProps = base.m_report.GetItemProps(rPLTextBox.StartOffset, out b);
						}
						else
						{
							rPLItemProps = (RPLItemProps)rPLTextBox.ElementProps;
							b = 7;
						}
					}
					else
					{
						rPLItemProps = base.m_report.GetItemProps(item.RPLSource, out b);
					}
					TogglePosition togglePosition = this.GetTogglePosition(rowItemStruct, (rPLItemProps.Definition as RPLItemPropsDef).ToggleItem, top);
					bool flag3 = b == 7 || b == 11 || b == 14 || b == 21 || b == 9 || b == 8;
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					if (flag3)
					{
						flag5 = (num3 != num2);
						flag6 = (num != topRow);
						if (b == 8)
						{
							if (flag5 && flag6)
							{
								excel.AddMergeCell(topRow, num2, num, num3);
								flag4 = true;
							}
							else if (item.Height != 0 && item.Width != 0 && (flag5 || flag6))
							{
								excel.AddMergeCell(topRow, num2, num, num3);
								flag4 = true;
							}
						}
						else if (flag5 || flag6)
						{
							excel.AddMergeCell(topRow, num2, num, num3);
							flag4 = true;
						}
					}
					if (b == 8)
					{
						BorderInfo borders = new BorderInfo(rPLItemProps.Style, width, item.Height, ((RPLLinePropsDef)rPLItemProps.Definition).Slant, false, false, rowItemStruct.IsDefaultLine, excel);
						itemInfo = new ItemInfo(topRow, num, num2, num3, flag4, borders, b, (rowItemStruct.State & 0x20) != 0, togglePosition);
					}
					else
					{
						string text = rPLItemProps.Definition.ID;
						if (text == null)
						{
							text = string.Empty;
						}
						borderInfo = this.GetBorderDefinitionFromCache(text, sharedBorderCache, rPLItemProps.Definition.SharedStyle, (rowItemStruct.State & 1) != 0, (rowItemStruct.State & 2) != 0, excel);
						if (b == 7)
						{
							RPLTextBoxPropsDef rPLTextBoxPropsDef = rPLItemProps.Definition as RPLTextBoxPropsDef;
							itemInfo = new TextBoxItemInfo(topRow, num, num2, num3, flag4, rPLTextBoxPropsDef.CanGrow, rPLTextBoxPropsDef.CanShrink, borderInfo, b, (rowItemStruct.State & 0x20) != 0, togglePosition);
						}
						else
						{
							itemInfo = new ItemInfo(topRow, num, num2, num3, flag4, borderInfo, b, (rowItemStruct.State & 0x20) != 0, togglePosition);
						}
						if (rPLItemProps.NonSharedStyle != null)
						{
							itemInfo.FillBorders(rPLItemProps.NonSharedStyle, (rowItemStruct.State & 1) != 0, (rowItemStruct.State & 2) != 0, excel);
						}
					}
					if (this.HandleBlocking((IBlockerInfo)itemInfo, itemInfo.TogglePosition, out flag2))
					{
						if (!flag2)
						{
							this.m_rowBlockers.Push(itemInfo);
						}
					}
					else
					{
						itemInfo.TogglePosition = TogglePosition.None;
					}
					this.RenderItem(excel, sharedImageCache, (RPLElementProps)rPLItemProps, b, topRow, num, num2, num3, flag4, ref autosizableGrow, ref autosizableShrink, borderInfo, item);
				}
				else
				{
					bool omitBorderTop = topRow > 0;
					bool omitBorderBottom = this.m_rowInfo.Count > num + ((!flag) ? 1 : 2);
					borderInfo = this.GetBorderDefinitionFromCache(string.Empty, sharedBorderCache, rPLPageLayout.Style.SharedProperties, omitBorderTop, omitBorderBottom, excel);
					itemInfo = new ItemInfo(topRow, num, num2, num3, false, borderInfo, 3, false, TogglePosition.None);
					if (rPLPageLayout.Style.NonSharedProperties != null)
					{
						itemInfo.FillBorders(rPLPageLayout.Style.NonSharedProperties, omitBorderTop, omitBorderBottom, excel);
					}
				}
				for (int l = num2; l <= num3; l++)
				{
					this.m_columns[l].Stack.Push(itemInfo);
					if (flag2)
					{
						this.m_columns[l].Blocker = itemInfo;
					}
				}
			}
		}

		private void RenderItem(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps props, byte type, int topRow, int bottomRow, int leftColumn, int rightColumn, bool verticallyMerged, ref bool autosizableGrow, ref bool autosizableShrink, BorderInfo borderDef, IRowItemStruct item)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			if (rPLItemProps.Bookmark != null)
			{
				excel.AddBookmarkTarget(rPLItemProps.Bookmark);
			}
			else if (rPLItemPropsDef.Bookmark != null)
			{
				excel.AddBookmarkTarget(rPLItemPropsDef.Bookmark);
			}
			if (rPLItemProps.Label != null || rPLItemPropsDef.Label != null)
			{
				excel.AddBookmarkTarget(rPLItemProps.UniqueName);
			}
			switch (type)
			{
			case 7:
			{
				RPLTextBoxProps rPLTextBoxProps = rPLItemProps as RPLTextBoxProps;
				RPLTextBoxPropsDef rPLTextBoxPropsDef = rPLTextBoxProps.Definition as RPLTextBoxPropsDef;
				RPLStyleProps sharedStyle = rPLTextBoxPropsDef.SharedStyle;
				if (!verticallyMerged)
				{
					if (rPLTextBoxPropsDef.CanGrow)
					{
						autosizableGrow = true;
					}
					if (rPLTextBoxPropsDef.CanShrink)
					{
						autosizableShrink = true;
					}
				}
				if (!rPLTextBoxPropsDef.IsSimple)
				{
					this.RenderRichTextBox(excel, rPLTextBoxProps, rPLTextBoxPropsDef, sharedStyle, borderDef, item, excel.GetCellRichTextInfo());
				}
				else
				{
					this.RenderSimpleTextBox(excel, rPLTextBoxProps, rPLTextBoxPropsDef, sharedStyle, borderDef, item);
				}
				break;
			}
			case 9:
				this.RenderImage(excel, sharedImageCache, rPLItemProps, topRow, bottomRow, leftColumn, rightColumn, item);
				break;
			case 11:
			case 14:
			case 21:
				this.RenderDynamicImage(excel, sharedImageCache, rPLItemProps, topRow, bottomRow, leftColumn, rightColumn, item);
				break;
			}
		}

		private void RenderDynamicImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps itemProps, int topRow, int bottomRow, int leftColumn, int rightColumn, IRowItemStruct item)
		{
			Stream stream = null;
			bool flag = false;
			bool flag2 = false;
			RPLDynamicImageProps rPLDynamicImageProps = itemProps as RPLDynamicImageProps;
			ImageInformation imageInformation = new ImageInformation();
			if (rPLDynamicImageProps.DynamicImageContent != null)
			{
				stream = excel.CreateStream(rPLDynamicImageProps.UniqueName);
				byte[] array = new byte[4096];
				rPLDynamicImageProps.DynamicImageContent.Position = 0L;
				int num2;
				for (int num = (int)rPLDynamicImageProps.DynamicImageContent.Length; num > 0; num -= num2)
				{
					num2 = rPLDynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
				}
			}
			else if (rPLDynamicImageProps.DynamicImageContentOffset > 0)
			{
				stream = excel.CreateStream(rPLDynamicImageProps.UniqueName);
				base.m_report.GetImage(rPLDynamicImageProps.DynamicImageContentOffset, stream);
			}
			else
			{
				imageInformation = this.GetInvalidImage(excel, sharedImageCache, ref flag);
				flag2 = true;
			}
			if (!flag2)
			{
				imageInformation.ImageData = stream;
				imageInformation.ImageName = rPLDynamicImageProps.UniqueName;
				imageInformation.Sizings = RPLFormat.Sizings.Fit;
				imageInformation.ImageFormat = ImageFormat.Png;
			}
			this.RenderImage(imageInformation, item, excel, sharedImageCache, flag2, topRow, leftColumn, bottomRow, rightColumn);
		}

		private void RenderImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps itemProps, int topRow, int bottomRow, int leftColumn, int rightColumn, IRowItemStruct item)
		{
			ImageInformation imageInformation = null;
			bool invalidImage = false;
			bool flag = false;
			string text = null;
			RPLImageProps rPLImageProps = itemProps as RPLImageProps;
			if (rPLImageProps.Image != null)
			{
				text = rPLImageProps.Image.ImageName;
				if (text == null)
				{
					text = rPLImageProps.UniqueName;
					imageInformation = new ImageInformation();
					imageInformation.ReadImage(excel, rPLImageProps.Image, text, base.m_report);
					if (imageInformation.ImageData == null || 0 == imageInformation.ImageData.Length)
					{
						imageInformation = this.GetInvalidImage(excel, sharedImageCache, ref flag);
						invalidImage = true;
					}
				}
				else
				{
					flag = true;
					if (sharedImageCache.ContainsKey(text))
					{
						imageInformation = sharedImageCache[text];
					}
					else
					{
						imageInformation = new ImageInformation();
						imageInformation.ReadImage(excel, rPLImageProps.Image, text, base.m_report);
						if (imageInformation.ImageData == null || 0 == imageInformation.ImageData.Length)
						{
							imageInformation = this.GetInvalidImage(excel, sharedImageCache, ref flag);
							invalidImage = true;
						}
						else
						{
							sharedImageCache.Add(text, imageInformation);
						}
					}
				}
			}
			else
			{
				imageInformation = this.GetInvalidImage(excel, sharedImageCache, ref flag);
				invalidImage = true;
			}
			imageInformation.Sizings = (itemProps.Definition as RPLImagePropsDef).Sizing;
			if (rPLImageProps.ActionInfo != null)
			{
				RPLAction[] actions = rPLImageProps.ActionInfo.Actions;
				foreach (RPLAction rPLAction in actions)
				{
					if (rPLAction.BookmarkLink != null)
					{
						imageInformation.HyperlinkURL = rPLAction.BookmarkLink;
						imageInformation.HyperlinkIsBookmark = true;
					}
					else if (rPLAction.DrillthroughUrl != null)
					{
						imageInformation.HyperlinkURL = rPLAction.DrillthroughUrl;
						imageInformation.HyperlinkIsBookmark = false;
					}
					else if (rPLAction.Hyperlink != null)
					{
						imageInformation.HyperlinkURL = rPLAction.Hyperlink;
						imageInformation.HyperlinkIsBookmark = false;
					}
				}
			}
			imageInformation.Paddings = this.GetImagePaddings(rPLImageProps.Style);
			this.RenderImage(imageInformation, item, excel, sharedImageCache, invalidImage, topRow, leftColumn, bottomRow, rightColumn);
		}

		private void RenderSimpleTextBox(IExcelGenerator excel, RPLTextBoxProps textBox, RPLTextBoxPropsDef textBoxDef, RPLStyleProps defStyle, BorderInfo borderDef, IRowItemStruct item)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			object obj = null;
			object obj2 = null;
			if (textBox.ProcessedWithError)
			{
				excel.SetCellError(ExcelErrorCode.ValueError);
			}
			else
			{
				text = (string)defStyle[23];
				text2 = (string)defStyle[32];
				text3 = (string)defStyle[36];
				obj = defStyle[38];
				obj2 = defStyle[37];
				if (textBoxDef.Value != null)
				{
					excel.SetCellValue(textBoxDef.Value, TypeCode.String);
				}
				if (textBox.TypeCode != 0)
				{
					this.RenderTextBoxValue(excel, textBox.Value, textBox.OriginalValue, textBox.TypeCode);
				}
				else
				{
					this.RenderTextBoxValue(excel, textBox.Value, textBox.OriginalValue, textBoxDef.SharedTypeCode);
				}
			}
			this.RenderActions(excel, textBox.ActionInfo);
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
			VerticalAlignment verticalAlign = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			if (!excel.UseCachedStyle(textBoxDef.ID))
			{
				excel.DefineCachedStyle(textBoxDef.ID);
				LayoutEngine.RenderTextBoxStyle(excel, defStyle, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
				borderDef.RenderBorders(excel);
				if (!textBoxDef.FormattedValueExpressionBased && !textBox.ProcessedWithError)
				{
					this.RenderFormat(excel, text, null, null, textBoxDef.SharedTypeCode, text2, text3, obj, obj2, ref horizontalAlign);
				}
				excel.EndCachedStyle();
				excel.UseCachedStyle(textBoxDef.ID);
			}
			else
			{
				textOrientation = this.GetTextOrientation(defStyle);
				LayoutEngine.UpdateHorizontalAlign(excel, defStyle, ref horizontalAlign);
				LayoutEngine.UpdateVerticalAlign(defStyle, ref verticalAlign);
				LayoutEngine.UpdateDirection(defStyle, ref textDirection);
			}
			if (textBox.NonSharedStyle != null)
			{
				LayoutEngine.RenderTextBoxStyle(excel, textBox.NonSharedStyle, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
			}
			excel.SetModifiedRotationForEastAsianChars(textOrientation == TextOrientation.Rotate90);
			bool flag = false;
			if (!textBox.ProcessedWithError)
			{
				TypeCode typeCode = (textBox.TypeCode == TypeCode.Empty) ? textBoxDef.SharedTypeCode : textBox.TypeCode;
				flag = FormatHandler.IsExcelNumberDataType(typeCode);
				if (flag)
				{
					string text4 = null;
					string text5 = null;
					string text6 = null;
					object obj3 = null;
					object obj4 = null;
					bool flag2 = false;
					if (textBox.NonSharedStyle != null)
					{
						text4 = (string)textBox.NonSharedStyle[23];
						if (text4 == null)
						{
							text4 = text;
						}
						else
						{
							flag2 = true;
						}
						text5 = (string)textBox.NonSharedStyle[32];
						if (text5 == null)
						{
							text5 = text2;
						}
						else
						{
							flag2 = true;
						}
						text6 = (string)textBox.NonSharedStyle[36];
						if (text6 == null)
						{
							text6 = text3;
						}
						else
						{
							flag2 = true;
						}
						obj3 = textBox.NonSharedStyle[38];
						if (obj3 == null)
						{
							obj3 = obj;
						}
						else
						{
							flag2 = true;
						}
						obj4 = textBox.NonSharedStyle[37];
						if (obj4 == null)
						{
							obj4 = obj2;
						}
						else
						{
							flag2 = true;
						}
						if (textBox.NonSharedStyle[25] != null)
						{
							flag2 = true;
						}
					}
					if (text5 == null)
					{
						string subreportLanguage = ((RowItemStruct)item).SubreportLanguage;
						if (!string.IsNullOrEmpty(subreportLanguage))
						{
							text5 = subreportLanguage;
							flag2 = true;
						}
					}
					if (textBoxDef.FormattedValueExpressionBased)
					{
						this.RenderFormat(excel, (text4 != null) ? text4 : text, textBox.OriginalValue, textBox.Value, typeCode, (text5 != null) ? text5 : text2, (text6 != null) ? text6 : text3, (obj3 != null) ? obj3 : obj, (obj4 != null) ? obj4 : obj2, ref horizontalAlign);
					}
					else if (flag2)
					{
						this.RenderFormat(excel, text4, textBox.OriginalValue, textBox.Value, typeCode, text5, text6, obj3, obj4, ref horizontalAlign);
					}
				}
			}
			this.FixupAlignments(excel, flag, textOrientation, horizontalAlign, verticalAlign, textDirection);
		}

		private void FixupAlignments(IExcelGenerator excel, bool isNumberType, TextOrientation textOrientation, HorizontalAlignment horizontalAlign, VerticalAlignment verticalAlign, TextDirection textDirection)
		{
			if (textOrientation == TextOrientation.Horizontal)
			{
				if (horizontalAlign == HorizontalAlignment.General && textDirection == TextDirection.RightToLeft)
				{
					horizontalAlign = this.ResolveGeneralAlignment(textDirection, isNumberType);
					excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
				}
			}
			else
			{
				if (horizontalAlign == HorizontalAlignment.General)
				{
					horizontalAlign = this.ResolveGeneralAlignment(textDirection, isNumberType);
				}
				bool isClockwise = textOrientation == TextOrientation.Rotate90;
				excel.GetCellStyle().HorizontalAlignment = LayoutConvert.RotateVerticalToHorizontalAlign(verticalAlign, isClockwise);
				excel.GetCellStyle().VerticalAlignment = LayoutConvert.RotateHorizontalToVerticalAlign(horizontalAlign, isClockwise);
			}
		}

		private HorizontalAlignment ResolveGeneralAlignment(TextDirection textDirection, bool isNumberType)
		{
			bool flag = textDirection == TextDirection.RightToLeft;
			if (isNumberType)
			{
				if (flag)
				{
					return HorizontalAlignment.Left;
				}
				return HorizontalAlignment.Right;
			}
			if (flag)
			{
				return HorizontalAlignment.Right;
			}
			return HorizontalAlignment.Left;
		}

		private void RenderRichTextBox(IExcelGenerator excel, RPLTextBoxProps textBox, RPLTextBoxPropsDef textBoxDef, RPLStyleProps defStyle, BorderInfo borderDef, IRowItemStruct item, IRichTextInfo richTextInfo)
		{
			RPLActionInfo actionInfo = textBox.ActionInfo;
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
			VerticalAlignment verticalAlign = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			if (!excel.UseCachedStyle(textBoxDef.ID))
			{
				excel.DefineCachedStyle(textBoxDef.ID);
				LayoutEngine.RenderTextBoxStyle(excel, defStyle, null, false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
				borderDef.RenderBorders(excel);
				excel.EndCachedStyle();
				excel.UseCachedStyle(textBoxDef.ID);
			}
			else
			{
				textOrientation = this.GetTextOrientation(defStyle);
				LayoutEngine.UpdateVerticalAlign(defStyle, ref verticalAlign);
				LayoutEngine.UpdateDirection(defStyle, ref textDirection);
			}
			if (textBox.NonSharedStyle != null)
			{
				LayoutEngine.RenderTextBoxStyle(excel, textBox.NonSharedStyle, null, false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
			}
			excel.GetCellStyle().Size = 10.0;
			richTextInfo.CheckForRotatedFarEastChars = (textOrientation == TextOrientation.Rotate90);
			RPLTextBox rplTextbox = (RPLTextBox)item.RPLSource;
			actionInfo = LayoutEngine.RenderRichText(excel, rplTextbox, richTextInfo, false, actionInfo, textDirection != TextDirection.RightToLeft, ref horizontalAlign);
			this.RenderActions(excel, actionInfo);
			this.FixupAlignments(excel, false, textOrientation, horizontalAlign, verticalAlign, textDirection);
		}

		internal static RPLActionInfo RenderRichText(IExcelGenerator excel, RPLTextBox rplTextbox, IRichTextInfo richTextInfo, bool inHeaderAndFooter, RPLActionInfo actions, bool renderListPrefixes, ref HorizontalAlignment horizontalAlign)
		{
			List<int> list = null;
			bool flag = true;
			RPLParagraph nextParagraph;
			while ((nextParagraph = rplTextbox.GetNextParagraph()) != null)
			{
				RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				RPLReportSize leftIndent;
				if (flag)
				{
					flag = false;
					if (!inHeaderAndFooter)
					{
						if (LayoutEngine.UpdateHorizontalAlign(excel, rPLParagraphProps.Style[25], ref horizontalAlign))
						{
							excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
						}
						if (horizontalAlign == HorizontalAlignment.Left || horizontalAlign == HorizontalAlignment.General)
						{
							leftIndent = rPLParagraphProps.LeftIndent;
							if (leftIndent == null)
							{
								leftIndent = rPLParagraphPropsDef.LeftIndent;
							}
							if (leftIndent != null)
							{
								int num = (int)(leftIndent.ToInches() * 96.0);
								excel.GetCellStyle().IndentLevel = (num + 6) / 12;
							}
						}
						else if (horizontalAlign == HorizontalAlignment.Right)
						{
							leftIndent = rPLParagraphProps.RightIndent;
							if (leftIndent == null)
							{
								leftIndent = rPLParagraphPropsDef.RightIndent;
							}
							if (leftIndent != null)
							{
								int num2 = (int)(leftIndent.ToInches() * 96.0);
								excel.GetCellStyle().IndentLevel = (num2 + 6) / 12;
							}
						}
					}
				}
				else
				{
					richTextInfo.AppendText("\n", false);
				}
				leftIndent = rPLParagraphProps.SpaceBefore;
				if (leftIndent == null)
				{
					leftIndent = rPLParagraphPropsDef.SpaceBefore;
				}
				if (leftIndent != null && leftIndent.ToMillimeters() >= 0.18)
				{
					IFont font = richTextInfo.AppendTextRun(" \n", false);
					font.Size = leftIndent.ToPoints();
				}
				if (renderListPrefixes)
				{
					int num3 = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
					if (num3 > 0)
					{
						IFont font;
						switch (rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle)
						{
						case RPLFormat.ListStyles.Bulleted:
							font = richTextInfo.AppendTextRun(new string(' ', num3 * 4 - 2), false);
							richTextInfo.AppendText(LayoutEngine.BulletChars[(num3 - 1) % 3]);
							if (list != null && list.Count > num3)
							{
								list.RemoveRange(num3, list.Count - num3);
							}
							break;
						case RPLFormat.ListStyles.Numbered:
						{
							if (list == null)
							{
								list = new List<int>();
							}
							int value = 1;
							int num4 = num3 - list.Count + 1;
							if (num4 > 0)
							{
								for (int i = 0; i < num4; i++)
								{
									list.Add(1);
								}
							}
							else
							{
								value = ++list[num3];
							}
							string text = null;
							switch (num3 % 3)
							{
							case 1:
								text = LayoutEngine.GetAsDecimalString(value);
								break;
							case 2:
								text = LayoutEngine.GetAsRomanNumeralString(value);
								break;
							case 0:
								text = LayoutEngine.GetAsLatinAlphaString(value);
								break;
							}
							int num5 = num3 * 4 - 2;
							if (text.Length > num5)
							{
								font = richTextInfo.AppendTextRun(text.Substring(text.Length - num5), false);
							}
							else
							{
								font = richTextInfo.AppendTextRun(new string(' ', num5 - text.Length), false);
								richTextInfo.AppendText(text, false);
							}
							if (list != null && list.Count > num3 + 1)
							{
								list.RemoveRange(num3 + 1, list.Count - num3 - 1);
							}
							richTextInfo.AppendText('.');
							break;
						}
						default:
							font = richTextInfo.AppendTextRun(new string(' ', num3 * 4 - 1), false);
							if (list != null && list.Count > num3)
							{
								list.RemoveRange(num3, list.Count - num3);
							}
							break;
						}
						font.Size = 10.0;
						font.Name = "Courier New";
						font.Underline = Underline.None;
						font.Strikethrough = false;
						font.Bold = 400;
						font.Italic = false;
						richTextInfo.AppendText(' ');
					}
				}
				RPLTextRun nextTextRun;
				while ((nextTextRun = nextParagraph.GetNextTextRun()) != null)
				{
					RPLTextRunProps rPLTextRunProps = nextTextRun.ElementProps as RPLTextRunProps;
					RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
					RPLStyleProps sharedStyle = rPLTextRunPropsDef.SharedStyle;
					RPLStyleProps nonSharedStyle = rPLTextRunProps.NonSharedStyle;
					string text2 = null;
					bool replaceInvalidWhitespace = true;
					if (inHeaderAndFooter && !string.IsNullOrEmpty(rPLTextRunPropsDef.Formula) && rPLTextRunPropsDef.Markup != RPLFormat.MarkupStyles.HTML && rPLTextRunProps.Markup != RPLFormat.MarkupStyles.HTML)
					{
						string text3 = rPLTextRunPropsDef.Formula;
						replaceInvalidWhitespace = false;
						if (text3.StartsWith("=", StringComparison.Ordinal))
						{
							text3 = text3.Remove(0, 1);
						}
						text2 = FormulaHandler.ProcessHeaderFooterFormula(text3);
					}
					else
					{
						text2 = ((rPLTextRunProps.Value == null) ? rPLTextRunPropsDef.Value : rPLTextRunProps.Value);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						IFont font = richTextInfo.AppendTextRun(text2, replaceInvalidWhitespace);
						LayoutEngine.RenderTextBoxStyle(excel, sharedStyle, font, true);
						if (nonSharedStyle != null)
						{
							LayoutEngine.RenderTextBoxStyle(excel, nonSharedStyle, font, true);
						}
						if (!inHeaderAndFooter && actions == null)
						{
							actions = rPLTextRunProps.ActionInfo;
						}
					}
				}
				leftIndent = rPLParagraphProps.SpaceAfter;
				if (leftIndent == null)
				{
					leftIndent = rPLParagraphPropsDef.SpaceAfter;
				}
				if (leftIndent != null && leftIndent.ToMillimeters() >= 0.18)
				{
					IFont font = richTextInfo.AppendTextRun("\n ", false);
					font.Size = leftIndent.ToPoints();
				}
			}
			return actions;
		}

		private void CalculateRowFlagsFromTextBoxes(ItemInfo itemInfo, int row, ref bool rowCanGrow, ref bool rowCanShrink, ref bool autoSize)
		{
			TextBoxItemInfo textBoxItemInfo = itemInfo as TextBoxItemInfo;
			if (textBoxItemInfo.CanGrow)
			{
				if (!autoSize && textBoxItemInfo.TopRow <= row && textBoxItemInfo.BottomRow >= row && textBoxItemInfo.TopRow != textBoxItemInfo.BottomRow)
				{
					autoSize = true;
				}
				rowCanGrow = true;
			}
			if (textBoxItemInfo.CanShrink)
			{
				rowCanShrink = true;
			}
		}

		private void CalculateAutoSizeFlag(bool rowCanGrow, bool rowCanShrink, int rowHeightIn20thPoints, ref bool autoSize)
		{
			if (!autoSize && !rowCanGrow && rowCanShrink)
			{
				autoSize = true;
			}
			if (autoSize && rowHeightIn20thPoints >= 8180)
			{
				autoSize = false;
			}
		}

		private static string GetAsDecimalString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (value > 0)
			{
				stringBuilder.Append((char)(ushort)(48 + value % 10));
				value /= 10;
			}
			int num = stringBuilder.Length / 2;
			for (int i = 0; i < num; i++)
			{
				char value2 = stringBuilder[i];
				stringBuilder[i] = stringBuilder[stringBuilder.Length - i - 1];
				stringBuilder[stringBuilder.Length - i - 1] = value2;
			}
			return stringBuilder.ToString();
		}

		private static string GetAsLatinAlphaString(int value)
		{
			value--;
			return new string((char)(ushort)(97 + value % 26), value / 26 + 1);
		}

		private static string GetAsRomanNumeralString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (value >= 1000)
			{
				stringBuilder.Append('m', value / 1000);
				value %= 1000;
			}
			for (int num = 2; num >= 0; num--)
			{
				int num2 = (int)Math.Pow(10.0, (double)num);
				int num3 = 9 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 0]);
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 2]);
					value -= num3;
				}
				num3 = 5 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 1]);
					value -= num3;
				}
				num3 = 4 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 0]);
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 1]);
					value -= num3;
				}
				if (value >= num2)
				{
					stringBuilder.Append(LayoutEngine.RomanNumerals[num, 0], value / num2);
					value %= num2;
				}
			}
			return stringBuilder.ToString();
		}

		private void RenderTextBoxValue(IExcelGenerator excel, string value, object originalValue, TypeCode type)
		{
			if (originalValue != null)
			{
				excel.SetCellValue(originalValue, type);
			}
			else if (value != null)
			{
				excel.SetCellValue(value, TypeCode.String);
			}
		}

		private void RenderActions(IExcelGenerator excel, RPLActionInfo actions)
		{
			if (actions != null)
			{
				RPLAction[] actions2 = actions.Actions;
				foreach (RPLAction rPLAction in actions2)
				{
					if (rPLAction.BookmarkLink != null)
					{
						excel.AddBookmarkLink(rPLAction.Label, rPLAction.BookmarkLink);
					}
					else if (rPLAction.DrillthroughUrl != null)
					{
						excel.AddHyperlink(rPLAction.Label, rPLAction.DrillthroughUrl);
					}
					else if (rPLAction.Hyperlink != null)
					{
						excel.AddHyperlink(rPLAction.Label, rPLAction.Hyperlink);
					}
				}
			}
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, ref TextOrientation textOrientation, ref HorizontalAlignment horizontalAlign, ref VerticalAlignment verticalAlign, ref TextDirection textDirection)
		{
			LayoutEngine.RenderTextBoxStyle(excel, style, (IFont)excel.GetCellStyle(), false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, IFont font, bool fontOnly)
		{
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.General;
			VerticalAlignment verticalAlignment = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			LayoutEngine.RenderTextBoxStyle(excel, style, font, fontOnly, ref textOrientation, ref horizontalAlignment, ref verticalAlignment, ref textDirection);
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, IFont font, bool fontOnly, ref TextOrientation textOrientation, ref HorizontalAlignment horizontalAlign, ref VerticalAlignment verticalAlign, ref TextDirection textDirection)
		{
			object obj = null;
			if (font != null)
			{
				obj = style[22];
				if (obj != null)
				{
					int num = LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj);
					if (num == 600)
					{
						num = 700;
					}
					font.Bold = num;
				}
				if (excel != null)
				{
					obj = style[27];
					if (obj != null && !style[27].Equals("Transparent"))
					{
						font.Color = excel.AddColor((string)obj);
					}
				}
				obj = style[19];
				if (obj != null)
				{
					font.Italic = ((RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic);
				}
				obj = style[20];
				if (obj != null)
				{
					font.Name = (string)obj;
				}
				obj = style[21];
				if (obj != null)
				{
					font.Size = LayoutConvert.ToPoints((string)obj);
				}
				obj = style[24];
				if (obj != null)
				{
					RPLFormat.TextDecorations textDecorations = (RPLFormat.TextDecorations)obj;
					font.Strikethrough = (textDecorations == RPLFormat.TextDecorations.LineThrough);
					font.Underline = (Underline)((textDecorations == RPLFormat.TextDecorations.Underline) ? 1 : 0);
				}
			}
			if (!fontOnly)
			{
				if (LayoutEngine.UpdateHorizontalAlign(excel, style, ref horizontalAlign))
				{
					excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
				}
				if (LayoutEngine.UpdateVerticalAlign(style, ref verticalAlign))
				{
					excel.GetCellStyle().VerticalAlignment = verticalAlign;
				}
				obj = style[30];
				if (obj != null)
				{
					switch ((RPLFormat.WritingModes)obj)
					{
					case RPLFormat.WritingModes.Vertical:
						textOrientation = TextOrientation.Rotate90;
						excel.GetCellStyle().Orientation = Orientation.Rotate90ClockWise;
						break;
					case RPLFormat.WritingModes.Rotate270:
						textOrientation = TextOrientation.Rotate270;
						excel.GetCellStyle().Orientation = Orientation.Rotate90CounterClockWise;
						break;
					default:
						textOrientation = TextOrientation.Horizontal;
						excel.GetCellStyle().Orientation = Orientation.Horizontal;
						break;
					}
				}
				if (LayoutEngine.UpdateDirection(style, ref textDirection))
				{
					excel.GetCellStyle().TextDirection = textDirection;
				}
			}
		}

		private static bool UpdateHorizontalAlign(IExcelGenerator excel, RPLStyleProps style, ref HorizontalAlignment horizontalAlign)
		{
			object value = style[25];
			return LayoutEngine.UpdateHorizontalAlign(excel, value, ref horizontalAlign);
		}

		private static bool UpdateHorizontalAlign(IExcelGenerator excel, object value, ref HorizontalAlignment horizontalAlign)
		{
			if (value != null)
			{
				RPLFormat.TextAlignments textAlignments = (RPLFormat.TextAlignments)value;
				if (excel.GetCellValueType() == TypeCode.Boolean && textAlignments == RPLFormat.TextAlignments.General)
				{
					textAlignments = RPLFormat.TextAlignments.Left;
				}
				horizontalAlign = LayoutConvert.ToHorizontalAlignEnum(textAlignments);
				return true;
			}
			return false;
		}

		private static bool UpdateVerticalAlign(RPLStyleProps style, ref VerticalAlignment verticalAlign)
		{
			object obj = style[26];
			if (obj != null)
			{
				verticalAlign = LayoutConvert.ToVerticalAlignEnum((RPLFormat.VerticalAlignments)obj);
				return true;
			}
			return false;
		}

		private static bool UpdateDirection(RPLStyleProps style, ref TextDirection textDirection)
		{
			object obj = style[29];
			if (obj != null)
			{
				textDirection = (TextDirection)(((RPLFormat.Directions)obj != RPLFormat.Directions.RTL) ? 1 : 2);
				return true;
			}
			return false;
		}

		private TextOrientation GetTextOrientation(RPLStyleProps style)
		{
			TextOrientation result = TextOrientation.Horizontal;
			object obj = style[30];
			if (obj != null)
			{
				switch ((RPLFormat.WritingModes)obj)
				{
				case RPLFormat.WritingModes.Vertical:
					result = TextOrientation.Rotate90;
					break;
				case RPLFormat.WritingModes.Rotate270:
					result = TextOrientation.Rotate270;
					break;
				}
			}
			return result;
		}

		private void RenderFormat(IExcelGenerator excel, string format, object originalValue, object value, TypeCode typeCode, string language, string numeralLanguage, object rplCalendar, object numeralVariant, ref HorizontalAlignment textAlign)
		{
			if (FormatHandler.IsExcelNumberDataType(typeCode))
			{
				string text = null;
				if (string.IsNullOrEmpty(language))
				{
					language = this.m_reportLanguage;
				}
				if (numeralLanguage == null)
				{
					numeralLanguage = language;
				}
				if (rplCalendar == null)
				{
					rplCalendar = RPLFormat.Calendars.Gregorian;
				}
				bool flag = default(bool);
				string excelNumberFormat = FormatHandler.GetExcelNumberFormat(format, language, (RPLFormat.Calendars)rplCalendar, numeralLanguage, (numeralVariant != null) ? ((int)numeralVariant) : 0, typeCode, originalValue, out text, out flag);
				if (flag)
				{
					excel.SetCellError(ExcelErrorCode.ValueError);
				}
				else if (text == null)
				{
					excel.GetCellStyle().NumberFormat = excelNumberFormat;
				}
				else if (value != null)
				{
					excel.SetCellValue(value, TypeCode.String);
					if (textAlign == HorizontalAlignment.General)
					{
						textAlign = HorizontalAlignment.Right;
					}
					excel.GetCellStyle().HorizontalAlignment = textAlign;
				}
				else
				{
					excel.GetCellStyle().NumberFormat = excelNumberFormat;
				}
			}
		}

		private void RenderImage(ImageInformation imageInfo, IRowItemStruct item, IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, bool invalidImage, int rowTop, int columnLeft, int rowBottom, int columnRight)
		{
			ImageInformation imageInformation;
			int num;
			int num2;
			int num3;
			int num4;
			int num9;
			int num10;
			int num11;
			int num12;
			int num13;
			int num14;
			double num17;
			double num7;
			double num8;
			double num15;
			double num16;
			bool flag;
			double num5;
			double num6;
			if (imageInfo != null && imageInfo.ImageData != null && 0 != imageInfo.ImageData.Length)
			{
				imageInformation = imageInfo;
				num = 0;
				num2 = 0;
				num3 = 0;
				num4 = 0;
				num5 = 1.0;
				num6 = 1.0;
				num7 = 1.0;
				num8 = 1.0;
				flag = false;
				num9 = 0;
				num10 = 0;
				if (imageInfo.Paddings != null)
				{
					num = imageInfo.Paddings.PaddingTop;
					num2 = imageInfo.Paddings.PaddingLeft;
					num3 = imageInfo.Paddings.PaddingBottom;
					num4 = imageInfo.Paddings.PaddingRight;
				}
				byte b = default(byte);
				RPLElementProps itemProps = base.m_report.GetItemProps(item.RPLSource, out b);
				object obj = itemProps.Style[5];
				RPLFormat.BorderStyles defaultBorderStyle = RPLFormat.BorderStyles.None;
				if (obj != null)
				{
					defaultBorderStyle = (RPLFormat.BorderStyles)obj;
				}
				object obj2 = itemProps.Style[10];
				double defaultBorderWidthInPts = 0.0;
				if (obj2 != null)
				{
					defaultBorderWidthInPts = LayoutConvert.ToPoints((string)obj2);
				}
				int borderWidth = this.GetBorderWidth(itemProps.Style, 6, 11, defaultBorderStyle, defaultBorderWidthInPts, false);
				if (num2 < borderWidth)
				{
					num2 = borderWidth;
				}
				borderWidth = this.GetBorderWidth(itemProps.Style, 7, 12, defaultBorderStyle, defaultBorderWidthInPts, true);
				if (num4 < borderWidth)
				{
					num4 = borderWidth;
				}
				borderWidth = this.GetBorderWidth(itemProps.Style, 8, 13, defaultBorderStyle, defaultBorderWidthInPts, false);
				if (num < borderWidth)
				{
					num = borderWidth;
				}
				borderWidth = this.GetBorderWidth(itemProps.Style, 9, 14, defaultBorderStyle, defaultBorderWidthInPts, true);
				if (num3 < borderWidth)
				{
					num3 = borderWidth;
				}
				if (RPLFormat.Sizings.Fit == imageInformation.ImageSizings)
				{
					num11 = rowTop;
					num9 = this.m_rowInfo[num11].Height;
					if (!this.ProcessVerticalPaddings(ref num11, rowBottom, ref num, true, ref num9))
					{
						num5 = (double)num / (double)num9;
						num5 = ((num <= 0 || !(num5 < 0.004)) ? Math.Round(num5, 3) : 0.004);
						num12 = rowBottom;
						num9 = this.m_rowInfo[num12].Height;
						if (!this.ProcessVerticalPaddings(ref num12, num11, ref num3, false, ref num9))
						{
							num6 = (double)(num9 - num3) / (double)num9;
							num6 = ((num3 <= 0 || !(num6 < 0.004)) ? Math.Round(num6, 3) : 0.004);
							num13 = columnLeft;
							num10 = this.m_columns[num13].Width;
							if (!this.ProcessHorizontalPaddings(ref num13, columnRight, ref num2, true, ref num10))
							{
								num7 = (double)num2 / (double)num10;
								num7 = ((num2 <= 0 || !(num7 < 0.001)) ? Math.Round(num7, 3) : 0.001);
								num14 = columnRight;
								num10 = this.m_columns[num14].Width;
								if (!this.ProcessHorizontalPaddings(ref num14, num13, ref num4, false, ref num10))
								{
									num8 = (double)(num10 - num4) / (double)num10;
									num8 = ((num4 <= 0 || !(num8 < 0.001)) ? Math.Round(num8, 3) : 0.001);
									goto IL_06f0;
								}
							}
						}
					}
				}
				else
				{
					num15 = 0.0;
					num16 = 0.0;
					try
					{
						num15 = (double)((float)imageInformation.Height / imageInformation.VerticalResolution) * 25.4;
						num16 = (double)((float)imageInformation.Width / imageInformation.HorizontalResolution) * 25.4;
					}
					catch
					{
						bool flag2 = false;
						imageInformation = this.GetInvalidImage(excel, sharedImageCache, ref flag2);
						invalidImage = true;
						num15 = (double)((float)imageInformation.Height / imageInformation.VerticalResolution) * 25.4;
						num16 = (double)((float)imageInformation.Width / imageInformation.HorizontalResolution) * 25.4;
					}
					if (invalidImage)
					{
						num17 = 1.0;
						goto IL_041c;
					}
					int num18 = Math.Max(item.Height - num - num3, 0);
					int num19 = Math.Max(item.Width - num2 - num4, 0);
					if (num18 != 0 && num19 != 0)
					{
						double num20 = LayoutConvert.ConvertPointsToMM((double)num18 / 20.0);
						double num21 = LayoutConvert.ConvertPointsToMM((double)num19 / 20.0);
						double num22 = num20 / num15;
						double num23 = num21 / num16;
						num17 = ((num22 <= num23) ? num22 : num23);
						goto IL_041c;
					}
				}
			}
			return;
			IL_05a2:
			num13 = columnLeft;
			num10 = this.m_columns[num13].Width;
			flag = this.ProcessHorizontalPaddings(ref num13, columnRight, ref num2, true, ref num10);
			int num24;
			if (!flag)
			{
				num7 = (double)num2 / (double)num10;
				num7 = ((num2 <= 0 || !(num7 < 0.001)) ? Math.Round(num7, 3) : 0.001);
				num14 = num13;
				if (num24 <= num10 - num2 - num4)
				{
					num8 = (double)(num24 + num2) / (double)num10;
					goto IL_06f0;
				}
				num10 = this.m_columns[num14].Width - num2;
				while (!flag && num24 >= num10)
				{
					num24 -= num10;
					if (num24 <= 0)
					{
						break;
					}
					num14++;
					if (num14 <= columnRight)
					{
						num10 = this.m_columns[num14].Width;
					}
					else
					{
						flag = true;
					}
				}
				if (!flag)
				{
					int num25;
					if (num24 != 0)
					{
						num8 = (double)num24 / (double)num10;
						num8 = ((num4 <= 0 || !(num8 < 0.001)) ? Math.Round(num8, 3) : 0.001);
						num25 = num14;
					}
					else
					{
						num8 = 1.0;
						num25 = num14 + 1;
					}
					if (num25 <= columnRight)
					{
						num10 = this.m_columns[num25].Width - num24;
						if (this.ProcessHorizontalPaddings(ref num25, columnRight, ref num4, true, ref num10) && num4 != 0)
						{
							return;
						}
					}
					else if (num4 != 0)
					{
						return;
					}
					goto IL_06f0;
				}
			}
			return;
			IL_041c:
			num15 = Math.Round(num15 * num17, 2);
			num16 = Math.Round(num16 * num17, 2);
			int num26 = LayoutConvert.ConvertMMTo20thPoints(num15);
			num24 = LayoutConvert.ConvertMMTo20thPoints(num16);
			num11 = rowTop;
			num9 = this.m_rowInfo[num11].Height;
			flag = this.ProcessVerticalPaddings(ref num11, rowBottom, ref num, true, ref num9);
			if (!flag)
			{
				num5 = (double)num / (double)num9;
				num5 = ((num <= 0 || !(num5 < 0.004)) ? Math.Round(num5, 3) : 0.004);
				num12 = num11;
				if (num26 <= num9 - num - num3)
				{
					num6 = (double)(num26 + num) / (double)num9;
					goto IL_05a2;
				}
				num9 = this.m_rowInfo[num12].Height - num;
				while (!flag && num26 >= num9)
				{
					num26 -= num9;
					if (num26 <= 0)
					{
						break;
					}
					num12++;
					if (num12 <= rowBottom)
					{
						num9 = this.m_rowInfo[num12].Height;
					}
					else
					{
						flag = true;
					}
				}
				if (!flag)
				{
					int num27;
					if (num26 != 0)
					{
						num6 = (double)num26 / (double)num9;
						num6 = ((num3 <= 0 || !(num6 < 0.004)) ? Math.Round(num6, 3) : 0.004);
						num27 = num12;
					}
					else
					{
						num6 = 1.0;
						num27 = num12 + 1;
					}
					if (num27 <= rowBottom)
					{
						num9 = this.m_rowInfo[num27].Height - num26;
						if (this.ProcessVerticalPaddings(ref num27, rowBottom, ref num3, true, ref num9) && num3 != 0)
						{
							return;
						}
					}
					else if (num3 != 0)
					{
						return;
					}
					goto IL_05a2;
				}
			}
			return;
			IL_06f0:
			excel.AddImage(imageInformation.ImageName, imageInformation.ImageData, imageInformation.ImageFormat, num11, num5, num13, num7, num12, num6, num14, num8, imageInformation.HyperlinkURL, imageInformation.HyperlinkIsBookmark);
		}

		private int GetBorderWidth(RPLElementStyle style, byte borderStyleItem, byte boderWidthItem, RPLFormat.BorderStyles defaultBorderStyle, double defaultBorderWidthInPts, bool rightOrBottom)
		{
			object obj = style[borderStyleItem];
			object obj2 = style[boderWidthItem];
			double borderWidthInPts = defaultBorderWidthInPts;
			RPLFormat.BorderStyles borderStyles = defaultBorderStyle;
			if (obj != null)
			{
				borderStyles = (RPLFormat.BorderStyles)obj;
			}
			if (borderStyles != 0)
			{
				if (obj2 != null)
				{
					borderWidthInPts = LayoutConvert.ToPoints((string)obj2);
				}
				return LayoutConvert.GetBorderWidth(LayoutConvert.ToBorderLineStyle(borderStyles), borderWidthInPts, rightOrBottom);
			}
			return 0;
		}

		private bool HandleBlocking(IBlockerInfo item, TogglePosition togglePosition, out bool isColumnBlocker)
		{
			isColumnBlocker = false;
			switch (togglePosition)
			{
			case TogglePosition.Above:
			case TogglePosition.Below:
				if (this.m_rowBlockers.Count > 0 && (item.LeftColumn < this.m_rowBlockers.Peek().LeftColumn || item.LeftColumn > this.m_rowBlockers.Peek().RightColumn))
				{
					return false;
				}
				if (!this.m_summaryRowAfter.HasValue)
				{
					this.m_summaryRowAfter = (togglePosition == TogglePosition.Below);
				}
				return true;
			case TogglePosition.Left:
			case TogglePosition.Right:
				for (int i = item.LeftColumn; i <= item.RightColumn; i++)
				{
					if (this.m_columns[i].Blocker != null && item.TopRow > this.m_columns[i].Blocker.BottomRow)
					{
						return false;
					}
				}
				if (!this.m_summaryColumnAfter.HasValue)
				{
					this.m_summaryColumnAfter = (togglePosition == TogglePosition.Right);
				}
				isColumnBlocker = true;
				return true;
			default:
				return false;
			}
		}

		private TablixMemberInfo FindVerticalParentTablixMember(TablixMemberInfo member, int currentColumn)
		{
			int count = this.m_columns[currentColumn].TablixStructs.Count;
			if (count > 1)
			{
				for (int num = this.m_columns[currentColumn].TablixStructs.IndexOf(member) - 1; num >= 0; num--)
				{
					TablixMemberInfo tablixMemberInfo = this.m_columns[currentColumn].TablixStructs[num];
					if (tablixMemberInfo.TogglePosition == TogglePosition.Above || tablixMemberInfo.TogglePosition == TogglePosition.Below)
					{
						return tablixMemberInfo;
					}
				}
			}
			return this.m_columns[currentColumn].LastColumnHeader;
		}

		private TablixMemberInfo FindHorizontalParentTablixMember(TablixMemberInfo member, int currentColumn)
		{
			int count = this.m_columns[currentColumn].TablixStructs.Count;
			if (count > 1)
			{
				for (int num = this.m_columns[currentColumn].TablixStructs.IndexOf(member) - 1; num >= 0; num--)
				{
					TablixMemberInfo tablixMemberInfo = this.m_columns[currentColumn].TablixStructs[num];
					if (tablixMemberInfo.TogglePosition == TogglePosition.Left || tablixMemberInfo.TogglePosition == TogglePosition.Right)
					{
						return tablixMemberInfo;
					}
				}
			}
			switch (member.TogglePosition)
			{
			case TogglePosition.Left:
				if (member.LeftColumn != 0 && member.LeftColumn == currentColumn)
				{
					int num3 = currentColumn - 1;
					if (num3 >= 0 && this.m_columns[num3].TablixStructs != null && this.m_columns[num3].TablixStructs.Count > 1)
					{
						return this.m_columns[num3].TablixStructs[this.m_columns[num3].TablixStructs.Count - 1];
					}
					return null;
				}
				return null;
			case TogglePosition.Right:
				if (member.LeftColumn == currentColumn && member.LeftColumn != this.m_columns.Length - 1)
				{
					int num2 = member.RightColumn + 1;
					if (num2 < this.m_columns.Length && this.m_columns[num2].TablixStructs != null && this.m_columns[num2].TablixStructs.Count > 1)
					{
						return this.m_columns[num2].TablixStructs[this.m_columns[num2].TablixStructs.Count - 1];
					}
					return null;
				}
				return null;
			default:
				return null;
			}
		}

		private void HandleTablixOutline(TablixMemberInfo member, int currentRow, int currentColumn, ref byte rowOutlineLevel, ref bool rowCollapsed)
		{
			switch (member.TogglePosition)
			{
			case TogglePosition.None:
				break;
			case TogglePosition.Left:
			case TogglePosition.Right:
				if (member.LeftColumn == currentColumn)
				{
					TablixMemberInfo tablixMemberInfo2 = this.FindHorizontalParentTablixMember(member, currentColumn);
					if (this.m_summaryRowAfter ?? true)
					{
						if (tablixMemberInfo2 != null)
						{
							if (member.BottomRow != tablixMemberInfo2.BottomRow)
							{
								this.BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
							}
						}
						else if (member.BottomRow != this.m_columns[currentColumn].TablixBlockers.Peek().BodyBottomRow)
						{
							this.BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
						}
					}
					else if (tablixMemberInfo2 != null)
					{
						if (member.TopRow != tablixMemberInfo2.TopRow)
						{
							this.BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
						}
					}
					else if (member.TopRow != this.m_columns[currentColumn].TablixBlockers.Peek().BodyTopRow)
					{
						this.BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
					}
				}
				break;
			case TogglePosition.Above:
			case TogglePosition.Below:
				if (member.TopRow == currentRow)
				{
					TablixMemberInfo tablixMemberInfo = this.FindVerticalParentTablixMember(member, currentColumn);
					if (this.m_summaryColumnAfter ?? true)
					{
						if (tablixMemberInfo != null)
						{
							if (member.RightColumn != tablixMemberInfo.RightColumn)
							{
								this.BumpColOutlineLevel(member, currentColumn);
							}
						}
						else if (member.RightColumn != this.m_columns[currentColumn].TablixBlockers.Peek().BodyRightColumn)
						{
							this.BumpColOutlineLevel(member, currentColumn);
						}
					}
					else if (tablixMemberInfo != null)
					{
						if (member.LeftColumn != tablixMemberInfo.LeftColumn)
						{
							this.BumpColOutlineLevel(member, currentColumn);
						}
					}
					else if (member.LeftColumn != this.m_columns[currentColumn].TablixBlockers.Peek().BodyLeftColumn)
					{
						this.BumpColOutlineLevel(member, currentColumn);
					}
				}
				break;
			}
		}

		private void BumpRowOutlineLevel(TablixMemberInfo member, ref byte rowOutlineLevel, ref bool rowCollapsed)
		{
			bool flag = default(bool);
			rowOutlineLevel += this.GetOutlineLevelIncrementValue(member, out flag);
			if (flag)
			{
				rowCollapsed = true;
			}
		}

		private void BumpColOutlineLevel(TablixMemberInfo member, int currentColumn)
		{
			ColumnInfo columnInfo = this.m_columns[currentColumn];
			bool flag = default(bool);
			columnInfo.OutlineLevel += this.GetOutlineLevelIncrementValue(member, out flag);
			if (flag)
			{
				columnInfo.Collapsed = true;
			}
		}

		private byte GetOutlineLevelIncrementValue(TablixMemberInfo member, out bool isCollapsed)
		{
			isCollapsed = member.IsHidden;
			int recursiveToggleLevel = member.RecursiveToggleLevel;
			if (recursiveToggleLevel < 0)
			{
				return 1;
			}
			if (member.RecursiveToggleLevel > 0)
			{
				return (byte)recursiveToggleLevel;
			}
			isCollapsed = false;
			return 0;
		}

		private TogglePosition GetTogglePosition(RowItemStruct rowItem, string toggleParentName, int top)
		{
			ToggleParent toggleParent = default(ToggleParent);
			if (toggleParentName != null && rowItem != null && rowItem.ToggleParents.TryGetValue(toggleParentName, out toggleParent))
			{
				if (toggleParent.Left + toggleParent.Width < rowItem.Left)
				{
					if (toggleParent.Top + toggleParent.Height < top)
					{
						return TogglePosition.Above;
					}
					if (toggleParent.Top > top + rowItem.Height)
					{
						return TogglePosition.Below;
					}
					return TogglePosition.Left;
				}
				if (toggleParent.Left < rowItem.Left + rowItem.Width)
				{
					if (toggleParent.Top < top)
					{
						return TogglePosition.Above;
					}
					return TogglePosition.Below;
				}
				if (toggleParent.Top + toggleParent.Height < top)
				{
					return TogglePosition.Above;
				}
				if (toggleParent.Top > top + rowItem.Height)
				{
					return TogglePosition.Below;
				}
				return TogglePosition.Right;
			}
			return TogglePosition.None;
		}

		private ImageInformation GetInvalidImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, ref bool isShared)
		{
			RSTrace.ExcelRendererTracer.Assert(null != excel, "The excel generator cannot be null");
			RSTrace.ExcelRendererTracer.Assert(null != sharedImageCache, "The shared image collection cannot be null");
			ImageInformation imageInformation = null;
			if (sharedImageCache.ContainsKey("InvalidImage"))
			{
				imageInformation = sharedImageCache["InvalidImage"];
				isShared = true;
			}
			else
			{
				imageInformation = new ImageInformation();
				imageInformation.ImageName = "InvalidImage";
				System.Drawing.Image image = null;
				if (ExcelRenderer.ExcelResourceManager != null)
				{
					image = (System.Drawing.Image)ExcelRenderer.ExcelResourceManager.GetObject("InvalidImage");
				}
				if (image != null)
				{
					try
					{
						Stream stream = excel.CreateStream("InvalidImage");
						image.Save(stream, ImageFormat.Bmp);
						imageInformation.ImageData = stream;
						imageInformation.ImageSizings = RPLFormat.Sizings.FitProportional;
						sharedImageCache.Add("InvalidImage", imageInformation);
						isShared = true;
						return imageInformation;
					}
					finally
					{
						image.Dispose();
						image = null;
					}
				}
				isShared = false;
			}
			return imageInformation;
		}

		private PaddingInformation GetImagePaddings(RPLElementStyle style)
		{
			object obj = null;
			int paddingLeft = 0;
			int paddingRight = 0;
			int paddingTop = 0;
			int paddingBottom = 0;
			if (style == null)
			{
				return null;
			}
			obj = style[17];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding top object should be a string");
				paddingTop = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[18];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding bottom object should be a string");
				paddingBottom = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[15];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding left object should be a string");
				paddingLeft = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[16];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding right object should be a string");
				paddingRight = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			return new PaddingInformation(paddingLeft, paddingRight, paddingTop, paddingBottom);
		}

		private BorderInfo GetBorderDefinitionFromCache(string key, Dictionary<string, BorderInfo> sharedBorderCache, RPLStyleProps styleProps, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
		{
			RSTrace.ExcelRendererTracer.Assert(key != null, "The key cannot be null");
			RSTrace.ExcelRendererTracer.Assert(sharedBorderCache != null, "The shared border collection cannot be null");
			RSTrace.ExcelRendererTracer.Assert(excel != null, "The excel generator cannot be null");
			BorderInfo borderInfo = null;
			if (sharedBorderCache.ContainsKey(key))
			{
				borderInfo = sharedBorderCache[key];
				borderInfo.OmitBorderTop = omitBorderTop;
				borderInfo.OmitBorderBottom = omitBorderBottom;
			}
			else
			{
				borderInfo = ((styleProps == null) ? new BorderInfo() : new BorderInfo(styleProps, omitBorderTop, omitBorderBottom, excel));
				sharedBorderCache.Add(key, borderInfo);
			}
			return borderInfo;
		}

		private bool ProcessVerticalPaddings(ref int rowStart, int rowEnd, ref int padding, bool topToBottom, ref int rowHeight)
		{
			bool flag = false;
			if (padding == 0)
			{
				return flag;
			}
			if (topToBottom)
			{
				if (rowStart > rowEnd)
				{
					flag = true;
				}
			}
			else if (rowStart < rowEnd)
			{
				flag = true;
			}
			while (!flag && padding - rowHeight >= 0)
			{
				padding -= rowHeight;
				bool flag2;
				if (topToBottom)
				{
					rowStart++;
					flag2 = (rowStart <= rowEnd);
				}
				else
				{
					rowStart--;
					flag2 = (rowStart >= rowEnd);
				}
				if (flag2)
				{
					rowHeight = this.m_rowInfo[rowStart].Height;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		private bool ProcessHorizontalPaddings(ref int columnStart, int columnEnd, ref int padding, bool leftToRight, ref int columnWidth)
		{
			bool flag = false;
			if (padding == 0)
			{
				return flag;
			}
			if (leftToRight)
			{
				if (columnStart > columnEnd)
				{
					flag = true;
				}
			}
			else if (columnStart < columnEnd)
			{
				flag = true;
			}
			while (!flag && padding - columnWidth >= 0)
			{
				padding -= columnWidth;
				bool flag2;
				if (leftToRight)
				{
					columnStart++;
					flag2 = (columnStart <= columnEnd);
				}
				else
				{
					columnStart--;
					flag2 = (columnStart >= columnEnd);
				}
				if (flag2)
				{
					columnWidth = this.m_columns[columnStart].Width;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		private static int CompareGenerationIndex(IRowItemStruct left, IRowItemStruct right)
		{
			return left.GenerationIndex.CompareTo(right.GenerationIndex);
		}

		internal void InitCache(CreateAndRegisterStream streamDelegate)
		{
			if (this.m_scalabilityCache == null)
			{
				this.m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(streamDelegate, "Excel", StorageObjectCreator.Instance, ExcelReferenceCreator.Instance, ComponentType.Rendering, 1);
			}
		}

		internal void Dispose()
		{
			if (this.m_scalabilityCache != null)
			{
				this.m_scalabilityCache.Dispose();
				this.m_scalabilityCache = null;
			}
		}
	}
}
