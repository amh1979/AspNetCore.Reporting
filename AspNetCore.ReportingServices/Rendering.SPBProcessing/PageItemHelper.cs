using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemHelper
	{
		private PaginationInfoItems m_type;

		private ItemSizes m_itemPageSizes;

		private PageItem.State m_state;

		private List<int> m_pageItemsAbove;

		private List<int> m_pageItemsLeft;

		private double m_defLeftValue;

		private double m_prevPageEnd;

		private PageItemHelper m_childPage;

		private int m_bodyIndex = -1;

		internal PaginationInfoItems Type
		{
			get
			{
				return this.m_type;
			}
		}

		internal ItemSizes ItemPageSizes
		{
			get
			{
				return this.m_itemPageSizes;
			}
			set
			{
				this.m_itemPageSizes = value;
			}
		}

		internal PageItem.State State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		internal List<int> PageItemsAbove
		{
			get
			{
				return this.m_pageItemsAbove;
			}
			set
			{
				this.m_pageItemsAbove = value;
			}
		}

		internal List<int> PageItemsLeft
		{
			get
			{
				return this.m_pageItemsLeft;
			}
			set
			{
				this.m_pageItemsLeft = value;
			}
		}

		internal double PrevPageEnd
		{
			get
			{
				return this.m_prevPageEnd;
			}
			set
			{
				this.m_prevPageEnd = value;
			}
		}

		internal PageItemHelper ChildPage
		{
			get
			{
				return this.m_childPage;
			}
			set
			{
				this.m_childPage = value;
			}
		}

		internal double DefLeftValue
		{
			get
			{
				return this.m_defLeftValue;
			}
			set
			{
				this.m_defLeftValue = value;
			}
		}

		internal int BodyIndex
		{
			get
			{
				return this.m_bodyIndex;
			}
			set
			{
				this.m_bodyIndex = value;
			}
		}

		internal PageItemHelper(byte type)
		{
			this.m_type = (PaginationInfoItems)type;
		}

		internal static PageItemHelper ReadItems(BinaryReader reader, long offsetEndPage)
		{
			if (reader != null && offsetEndPage > 0)
			{
				long position = reader.BaseStream.Position;
				PageItemContainerHelper pageItemContainerHelper = null;
				byte b = reader.ReadByte();
				if (b == 7)
				{
					pageItemContainerHelper = new PageItemContainerHelper(b);
					PageItemHelper.ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
				}
				else
				{
					reader.BaseStream.Position -= 1L;
				}
				if (reader.BaseStream.Position > offsetEndPage)
				{
					throw new InvalidDataException(SPBRes.InvalidPaginationStream);
				}
				return pageItemContainerHelper;
			}
			return null;
		}

		private static void ReadRepeatWithItemProperties(PageItemRepeatWithHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != 255 && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 12:
					item.RelativeTop = reader.ReadDouble();
					break;
				case 13:
					item.RelativeBottom = reader.ReadDouble();
					break;
				case 14:
					item.RelativeTopToBottom = reader.ReadDouble();
					break;
				case 15:
					item.DataRegionIndex = reader.ReadInt32();
					break;
				case 1:
					item.RenderItemSize = new ItemSizes();
					if (item.RenderItemSize.ReadPaginationInfo(reader, offsetEndPage) == 0)
					{
						break;
					}
					throw new InvalidDataException(SPBRes.InvalidPaginationStream);
				case 2:
					item.RenderItemSize = new PaddItemSizes();
					if (item.RenderItemSize.ReadPaginationInfo(reader, offsetEndPage) == 0)
					{
						break;
					}
					throw new InvalidDataException(SPBRes.InvalidPaginationStream);
				case 19:
				{
					byte b2 = reader.ReadByte();
					PageItemHelper pageItemHelper = null;
					switch (b2)
					{
					case 5:
					case 6:
					{
						PageItemContainerHelper pageItemContainerHelper = new PageItemContainerHelper(b2);
						pageItemHelper = pageItemContainerHelper;
						PageItemHelper.ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
						break;
					}
					case 1:
					case 8:
					case 9:
					case 12:
						pageItemHelper = new PageItemHelper(b2);
						PageItemHelper.ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
						break;
					default:
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					item.ChildPage = pageItemHelper;
					break;
				}
				default:
					throw new InvalidDataException(SPBRes.InvalidTokenPaginationProperties(b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private static void ReadPageItemContainerProperties(PageItemContainerHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != 255 && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 6:
					item.ItemsCreated = reader.ReadBoolean();
					break;
				case 11:
					item.PrevPageEnd = reader.ReadDouble();
					break;
				case 9:
				{
					byte b3 = reader.ReadByte();
					if (b3 != 3)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
					PageItemHelper pageItemHelper = new PageItemHelper(3);
					PageItemHelper.ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
					item.RightEdgeItem = pageItemHelper;
					break;
				}
				case 7:
				{
					int num3 = reader.ReadInt32();
					int[] array3 = new int[num3];
					for (int k = 0; k < num3; k++)
					{
						array3[k] = reader.ReadInt32();
					}
					item.IndexesLeftToRight = array3;
					break;
				}
				case 20:
				{
					int num4 = reader.ReadInt32();
					int[] array4 = new int[num4];
					for (int l = 0; l < num4; l++)
					{
						array4[l] = reader.ReadInt32();
					}
					item.IndexesTopToBottom = array4;
					break;
				}
				case 10:
				{
					int num2 = reader.ReadInt32();
					PageItemHelper[] array2 = new PageItemHelper[num2];
					for (int j = 0; j < num2; j++)
					{
						byte b4 = reader.ReadByte();
						switch (b4)
						{
						case 5:
						case 6:
							PageItemHelper.ReadPageItemContainerProperties((PageItemContainerHelper)(array2[j] = new PageItemContainerHelper(b4)), reader, offsetEndPage);
							break;
						case 1:
						case 2:
						case 3:
						case 8:
						case 9:
						case 10:
						case 12:
						case 15:
						case 17:
							array2[j] = new PageItemHelper(b4);
							PageItemHelper.ReadPageItemProperties(array2[j], reader, offsetEndPage);
							break;
						case 4:
							array2[j] = new PageItemHelper(b4);
							PageItemHelper.ReadSubReportProperties(array2[j], reader, offsetEndPage);
							break;
						case 11:
							PageItemHelper.ReadTablixProperties((PageTablixHelper)(array2[j] = new PageTablixHelper(b4)), reader, offsetEndPage);
							break;
						case 14:
							reader.ReadByte();
							array2[j] = null;
							break;
						default:
							throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b4.ToString("x", CultureInfo.InvariantCulture)));
						}
					}
					item.Children = array2;
					break;
				}
				case 8:
				{
					int num = reader.ReadInt32();
					PageItemRepeatWithHelper[] array = new PageItemRepeatWithHelper[num];
					for (int i = 0; i < num; i++)
					{
						byte b2 = reader.ReadByte();
						array[i] = new PageItemRepeatWithHelper(b2);
						if (b2 != 14)
						{
							PageItemHelper.ReadRepeatWithItemProperties(array[i], reader, offsetEndPage);
						}
						else
						{
							reader.ReadByte();
							array[i] = null;
						}
					}
					item.RepeatWithItems = array;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private static void ReadPageItemProperties(PageItemHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != 255 && reader.BaseStream.Position <= offsetEndPage)
			{
				item.ProcessPageItemToken(b, reader, offsetEndPage);
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private static void ReadSubReportProperties(PageItemHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != 255 && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 23:
					item.BodyIndex = reader.ReadInt32();
					break;
				case 11:
					item.PrevPageEnd = reader.ReadDouble();
					break;
				case 19:
				{
					byte b2 = reader.ReadByte();
					if (b2 != 7)
					{
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					PageItemContainerHelper pageItemContainerHelper = new PageItemContainerHelper(b2);
					PageItemHelper.ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
					item.ChildPage = pageItemContainerHelper;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private static void ReadTablixProperties(PageTablixHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != 255 && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 16:
					item.LevelForRepeat = reader.ReadInt32();
					break;
				case 22:
					item.IgnoreTotalsOnLastLevel = reader.ReadBoolean();
					break;
				case 17:
					item.TablixCreateState = PageItemHelper.ReadIntList(reader, offsetEndPage);
					break;
				case 18:
					item.MembersInstanceIndex = PageItemHelper.ReadIntList(reader, offsetEndPage);
					break;
				case 19:
				{
					byte b2 = reader.ReadByte();
					PageItemHelper pageItemHelper = null;
					switch (b2)
					{
					case 5:
					case 6:
					{
						PageItemContainerHelper pageItemContainerHelper = new PageItemContainerHelper(b2);
						pageItemHelper = pageItemContainerHelper;
						PageItemHelper.ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
						break;
					}
					case 1:
					case 2:
					case 3:
					case 8:
					case 9:
					case 10:
					case 12:
					case 15:
					case 17:
						pageItemHelper = new PageItemHelper(b2);
						PageItemHelper.ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
						break;
					case 4:
						pageItemHelper = new PageItemHelper(b2);
						PageItemHelper.ReadSubReportProperties(pageItemHelper, reader, offsetEndPage);
						break;
					case 11:
					{
						PageTablixHelper pageTablixHelper = new PageTablixHelper(b2);
						pageItemHelper = pageTablixHelper;
						PageItemHelper.ReadTablixProperties(pageTablixHelper, reader, offsetEndPage);
						break;
					}
					default:
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					item.ChildPage = pageItemHelper;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private void ProcessPageItemToken(byte token, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			switch (token)
			{
			case 1:
				this.m_itemPageSizes = new ItemSizes();
				if (this.m_itemPageSizes.ReadPaginationInfo(reader, offsetEndPage) == 0)
				{
					break;
				}
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			case 2:
				this.m_itemPageSizes = new PaddItemSizes();
				if (this.m_itemPageSizes.ReadPaginationInfo(reader, offsetEndPage) == 0)
				{
					break;
				}
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			case 3:
				this.m_state = (PageItem.State)reader.ReadByte();
				break;
			case 21:
				this.m_defLeftValue = reader.ReadDouble();
				break;
			case 4:
				this.m_pageItemsAbove = PageItemHelper.ReadIntList(reader, offsetEndPage);
				break;
			case 5:
				this.m_pageItemsLeft = PageItemHelper.ReadIntList(reader, offsetEndPage);
				break;
			default:
				throw new InvalidDataException(SPBRes.InvalidTokenPaginationProperties(token.ToString(CultureInfo.InvariantCulture)));
			}
			if (reader.BaseStream.Position <= offsetEndPage)
			{
				return;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private static List<int> ReadIntList(BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			List<int> list = null;
			int num = reader.ReadInt32();
			if (num <= 0)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			list = new List<int>(num);
			for (int i = 0; i < num; i++)
			{
				int item = reader.ReadInt32();
				list.Add(item);
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			return list;
		}
	}
}
