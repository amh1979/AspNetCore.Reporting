using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal static class RPLReader
	{
		internal static long ReadOffset(BinaryReader reader)
		{
			return reader.ReadInt64();
		}

		internal static byte ReadItemType(long startOffset, BinaryReader reader)
		{
			reader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
			return reader.ReadByte();
		}

		internal static long ResolveReportItemEnd(long offsetEnd, BinaryReader reader, ref byte itemType)
		{
			reader.BaseStream.Seek(offsetEnd, SeekOrigin.Begin);
			reader.ReadByte();
			long num = RPLReader.ReadOffset(reader);
			reader.BaseStream.Seek(num, SeekOrigin.Begin);
			itemType = reader.ReadByte();
			if (itemType == 16 || itemType == 17 || itemType == 18 || itemType == 18)
			{
				num = RPLReader.ReadOffset(reader);
			}
			return num;
		}

		internal static RPLMeasurement[] ReadMeasurements(RPLContext context, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num > 0)
			{
				RPLMeasurement[] array = new RPLMeasurement[num];
				RPLMeasurement rPLMeasurement = null;
				for (int i = 0; i < num; i++)
				{
					rPLMeasurement = new RPLMeasurement();
					rPLMeasurement.Left = reader.ReadSingle();
					rPLMeasurement.Top = reader.ReadSingle();
					rPLMeasurement.Width = reader.ReadSingle();
					rPLMeasurement.Height = reader.ReadSingle();
					rPLMeasurement.ZIndex = reader.ReadInt32();
					rPLMeasurement.State = reader.ReadByte();
					rPLMeasurement.SetOffset(RPLReader.ReadOffset(reader), context);
					array[i] = rPLMeasurement;
				}
				return array;
			}
			return null;
		}

		internal static RPLItemMeasurement[] ReadItemMeasurements(RPLContext context, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num > 0)
			{
				RPLItemMeasurement[] array = new RPLItemMeasurement[num];
				RPLItemMeasurement rPLItemMeasurement = null;
				for (int i = 0; i < num; i++)
				{
					rPLItemMeasurement = new RPLItemMeasurement();
					rPLItemMeasurement.Left = reader.ReadSingle();
					rPLItemMeasurement.Top = reader.ReadSingle();
					rPLItemMeasurement.Width = reader.ReadSingle();
					rPLItemMeasurement.Height = reader.ReadSingle();
					rPLItemMeasurement.ZIndex = reader.ReadInt32();
					rPLItemMeasurement.State = reader.ReadByte();
					rPLItemMeasurement.SetOffset(RPLReader.ReadOffset(reader), context);
					array[i] = rPLItemMeasurement;
				}
				return array;
			}
			return null;
		}

		private static RPLStyleProps ReadStyle(RPLContext context, BinaryReader reader)
		{
			switch (reader.ReadByte())
			{
			case 0:
				return RPLReader.ReadStyleProps(context, reader);
			case 1:
				return RPLReader.ReadStyleProps(context, reader);
			default:
				return null;
			}
		}

		private static RPLStyleProps ReadStyleProps(RPLContext context, BinaryReader reader)
		{
			byte b = reader.ReadByte();
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			while (true)
			{
				switch (b)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 20:
				case 21:
				case 23:
				case 27:
				case 28:
				case 32:
				case 34:
				case 36:
					rPLStyleProps.Add(b, reader.ReadString());
					break;
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 19:
				case 22:
				case 24:
				case 25:
				case 26:
				case 29:
				case 30:
				case 31:
				case 35:
				case 38:
					rPLStyleProps.Add(b, reader.ReadByte());
					break;
				case 33:
					rPLStyleProps.Add(b, RPLReader.ReadImage(context, true, reader));
					break;
				case 37:
					rPLStyleProps.Add(b, reader.ReadInt32());
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Style properties", b.ToString("x", CultureInfo.InvariantCulture)));
				case 254:
				case 255:
					if (rPLStyleProps.Count == 0)
					{
						return null;
					}
					return rPLStyleProps;
				}
				b = reader.ReadByte();
			}
		}

		internal static void ReadReport(RPLReport report, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			Version version = null;
			RPLVersionEnum rPLVersionEnum = RPLReader.ReadAndVerifyRPLVersion(binaryReader, out version);
			switch (rPLVersionEnum)
			{
			case RPLVersionEnum.RPL2008:
			case RPLVersionEnum.RPL2008WithImageConsolidation:
			case RPLVersionEnum.RPLAccess:
			case RPLVersionEnum.RPLMap:
			case RPLVersionEnum.RPL2009:
				context.VersionPicker = rPLVersionEnum;
				report.RPLVersion = version;
				RPLReader.ReadVersionedReport(report, context);
				break;
			default:
				throw new ArgumentException(RPLRes.UnsupportedRPLVersion(version.ToString(3), "10.6"));
			}
		}

		private static void ReadVersionedReport(RPLReport report, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			byte b = 0;
			long offsetEnd = binaryReader.BaseStream.Length - 16;
			long offset = RPLReader.ResolveReportItemEnd(offsetEnd, binaryReader, ref b);
			int num = binaryReader.ReadInt32();
			report.RPLPaginatedPages = new RPLPageContent[num];
			for (int i = 0; i < num; i++)
			{
				report.RPLPaginatedPages[i] = new RPLPageContent(RPLReader.ReadOffset(binaryReader), context, report.RPLVersion);
			}
			binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			binaryReader.ReadByte();
			RPLReader.ReadReportProps(report, binaryReader);
		}

		private static void ReadReportProps(RPLReport report, BinaryReader reader)
		{
			reader.ReadByte();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 9:
					report.Description = reader.ReadString();
					break;
				case 10:
					report.Location = reader.ReadString();
					break;
				case 11:
					report.Language = reader.ReadString();
					break;
				case 12:
					report.ExecutionTime = RPLReader.ReadDateTimeFromBinary(reader.ReadInt64());
					break;
				case 13:
					report.Author = reader.ReadString();
					break;
				case 14:
					report.AutoRefresh = reader.ReadInt32();
					break;
				case 15:
					report.ReportName = reader.ReadString();
					break;
				case 50:
					report.ConsumeContainerWhitespace = reader.ReadBoolean();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Report properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		internal static DateTime ReadDateTimeFromBinary(long dateTime)
		{
			return DateTime.FromBinary(dateTime);
		}

		private static RPLVersionEnum ReadAndVerifyRPLVersion(BinaryReader reader, out Version rplVersion)
		{
			reader.BaseStream.Seek(0L, SeekOrigin.Begin);
			reader.ReadString();
			Version version = new Version(reader.ReadByte(), reader.ReadByte(), reader.ReadInt32());
			RPLVersionEnum rPLVersionEnum = RPLReader.CompareRPLVersions(version);
			reader.BaseStream.Seek(-6L, SeekOrigin.End);
			Version version2 = new Version(reader.ReadByte(), reader.ReadByte(), reader.ReadInt32());
			RPLVersionEnum rPLVersionEnum2 = RPLReader.CompareRPLVersions(version2);
			if (rPLVersionEnum != rPLVersionEnum2)
			{
				throw new ArgumentException(RPLRes.MismatchRPLVersion(version.ToString(3), version2.ToString(3)));
			}
			rplVersion = version;
			return rPLVersionEnum;
		}

		internal static RPLVersionEnum CompareRPLVersions(Version clientRPLVersion)
		{
			int major = clientRPLVersion.Major;
			int minor = clientRPLVersion.Minor;
			if (major == 10)
			{
				switch (minor)
				{
				case 6:
					return RPLVersionEnum.RPL2009;
				case 4:
					return RPLVersionEnum.RPLAccess;
				case 5:
					return RPLVersionEnum.RPLMap;
				case 3:
					if (clientRPLVersion.Build != 1)
					{
						break;
					}
					return RPLVersionEnum.RPL2008WithImageConsolidation;
				}
				if (minor < 6)
				{
					return RPLVersionEnum.RPL2008;
				}
				throw new ArgumentException(RPLRes.UnsupportedRPLVersion(clientRPLVersion.ToString(3), "10.6"));
			}
			if (major < 10)
			{
				return RPLVersionEnum.RPL2008;
			}
			throw new ArgumentException(RPLRes.UnsupportedRPLVersion(clientRPLVersion.ToString(3), "10.6"));
		}

		internal static void ReadPageContent(RPLPageContent pageContent, long endOffset, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			byte b = 0;
			long offset = RPLReader.ResolveReportItemEnd(endOffset, binaryReader, ref b);
			RPLMeasurement[] array = RPLReader.ReadMeasurements(context, binaryReader);
			long position = binaryReader.BaseStream.Position;
			byte b2 = binaryReader.ReadByte();
			pageContent.ReportSectionSizes = new RPLSizes[array.Length];
			long[] array2 = new long[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				pageContent.ReportSectionSizes[i] = new RPLSizes(array[i].Top, array[i].Left, array[i].Height, array[i].Width);
				array2[i] = array[i].OffsetInfo.EndOffset;
			}
			pageContent.SectionOffsets = array2;
			pageContent.SectionCount = array2.Length;
			binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			binaryReader.ReadByte();
			if (pageContent.PageLayout == null)
			{
				pageContent.PageLayout = new RPLPageLayout();
			}
			RPLReader.ReadPageLayoutProps(pageContent.PageLayout, context, binaryReader);
			if (b2 == 3)
			{
				binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
				RPLReader.ReadPageLayoutProps(pageContent.PageLayout, context, binaryReader);
			}
		}

		internal static void ReadPageContent2008(RPLPageContent pageContent, long endOffset, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			byte b = 0;
			long num = RPLReader.ResolveReportItemEnd(endOffset, binaryReader, ref b);
			RPLItemMeasurement[] array = RPLReader.ReadItemMeasurements(context, binaryReader);
			RPLItemMeasurement[] columns = null;
			RPLItemMeasurement rPLItemMeasurement = null;
			RPLItemMeasurement rPLItemMeasurement2 = null;
			string iD = null;
			int columns2 = 0;
			float columnSpacing = 0f;
			float top = 0f;
			float left = 0f;
			float num2 = 0f;
			float num3 = 0f;
			long num4 = -1L;
			if (array != null)
			{
				OffsetInfo offsetInfo = null;
				for (int i = 0; i < array.Length; i++)
				{
					offsetInfo = array[i].OffsetInfo;
					num = RPLReader.ResolveReportItemEnd(offsetInfo.EndOffset, binaryReader, ref b);
					b = RPLReader.ReadItemType(num, binaryReader);
					switch (b)
					{
					case 4:
					{
						long endOffset4 = offsetInfo.EndOffset;
						rPLItemMeasurement = new RPLItemMeasurement(array[i]);
						rPLItemMeasurement.SetOffset(endOffset4, context);
						break;
					}
					case 5:
					{
						long endOffset3 = offsetInfo.EndOffset;
						rPLItemMeasurement2 = new RPLItemMeasurement(array[i]);
						rPLItemMeasurement2.SetOffset(endOffset3, context);
						break;
					}
					default:
					{
						top = array[i].Top;
						left = array[i].Left;
						num3 = array[i].Width;
						num2 = array[i].Height;
						long endOffset2 = offsetInfo.EndOffset;
						num4 = endOffset2;
						binaryReader.BaseStream.Seek(endOffset2, SeekOrigin.Begin);
						binaryReader.ReadByte();
						RPLReader.ReadOffset(binaryReader);
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						pageContent.PageLayout = new RPLPageLayout();
						RPLReader.ReadPageLayoutProps2008(pageContent.PageLayout, context, binaryReader, out iD, out columns2, out columnSpacing);
						RPLReader.ResolveReportItemEnd(endOffset2, binaryReader, ref b);
						columns = RPLReader.ReadItemMeasurements(context, binaryReader);
						break;
					}
					}
				}
			}
			float num5 = num2;
			float num6 = num3;
			if (rPLItemMeasurement != null)
			{
				num5 += rPLItemMeasurement.Height;
				num6 = Math.Max(num6, rPLItemMeasurement.Width);
			}
			if (rPLItemMeasurement2 != null)
			{
				num5 += rPLItemMeasurement2.Height;
				num6 = Math.Max(num6, rPLItemMeasurement2.Width);
			}
			pageContent.ReportSectionSizes = new RPLSizes[1];
			pageContent.ReportSectionSizes[0] = new RPLSizes(top, left, num5, num6);
			pageContent.SectionOffsets = new long[1]
			{
				num4
			};
			pageContent.SectionCount = 0;
			RPLReportSection rPLReportSection = new RPLReportSection(columns2);
			rPLReportSection.ID = iD;
			rPLReportSection.Header = rPLItemMeasurement;
			rPLReportSection.Footer = rPLItemMeasurement2;
			rPLReportSection.Columns = columns;
			rPLReportSection.ColumnSpacing = columnSpacing;
			rPLReportSection.BodyArea = new RPLSizes(top, left, num2, num3);
			pageContent.AddReportSection(rPLReportSection);
		}

		internal static void ReadPageLayoutProps(RPLPageLayout pageLayout, RPLContext context, BinaryReader reader)
		{
			reader.ReadByte();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 48:
					pageLayout.PageName = reader.ReadString();
					break;
				case 18:
					pageLayout.MarginTop = reader.ReadSingle();
					break;
				case 20:
					pageLayout.MarginBottom = reader.ReadSingle();
					break;
				case 19:
					pageLayout.MarginLeft = reader.ReadSingle();
					break;
				case 21:
					pageLayout.MarginRight = reader.ReadSingle();
					break;
				case 16:
					pageLayout.PageHeight = reader.ReadSingle();
					break;
				case 17:
					pageLayout.PageWidth = reader.ReadSingle();
					break;
				case 6:
					pageLayout.Style = RPLReader.ReadPageStyle(context, reader);
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Page properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadPageLayoutProps2008(RPLPageLayout pageLayout, RPLContext context, BinaryReader reader, out string id, out int columnCount, out float columnSpacing)
		{
			columnCount = 0;
			columnSpacing = 0f;
			id = null;
			reader.ReadByte();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 1:
					id = reader.ReadString();
					break;
				case 0:
					reader.ReadString();
					break;
				case 16:
					pageLayout.PageHeight = reader.ReadSingle();
					break;
				case 17:
					pageLayout.PageWidth = reader.ReadSingle();
					break;
				case 20:
					pageLayout.MarginBottom = reader.ReadSingle();
					break;
				case 19:
					pageLayout.MarginLeft = reader.ReadSingle();
					break;
				case 21:
					pageLayout.MarginRight = reader.ReadSingle();
					break;
				case 18:
					pageLayout.MarginTop = reader.ReadSingle();
					break;
				case 23:
					columnCount = reader.ReadInt32();
					break;
				case 22:
					columnSpacing = reader.ReadSingle();
					break;
				case 6:
					pageLayout.Style = RPLReader.ReadPageStyle(context, reader);
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Page properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		internal static RPLReportSection ReadReportSection(long endOffset, RPLContext context)
		{
			RPLReportSection rPLReportSection = null;
			byte b = 0;
			long num = -1L;
			BinaryReader binaryReader = context.BinaryReader;
			long offset = RPLReader.ResolveReportItemEnd(endOffset, binaryReader, ref b);
			RPLMeasurement[] array = RPLReader.ReadMeasurements(context, binaryReader);
			if (array != null)
			{
				OffsetInfo offsetInfo = null;
				for (int i = 0; i < array.Length; i++)
				{
					offsetInfo = array[i].OffsetInfo;
					long startOffset = RPLReader.ResolveReportItemEnd(offsetInfo.EndOffset, binaryReader, ref b);
					b = RPLReader.ReadItemType(startOffset, binaryReader);
					switch (b)
					{
					case 20:
						if (rPLReportSection == null)
						{
							rPLReportSection = new RPLReportSection(endOffset);
						}
						rPLReportSection.BodyArea = new RPLSizes(array[i].Top, array[i].Left, array[i].Height, array[i].Width);
						num = array[i].OffsetInfo.EndOffset;
						break;
					case 4:
					{
						long endOffset3 = array[i].OffsetInfo.EndOffset;
						RPLItemMeasurement rPLItemMeasurement2 = new RPLItemMeasurement(array[i]);
						rPLItemMeasurement2.SetOffset(endOffset3, context);
						if (rPLReportSection == null)
						{
							rPLReportSection = new RPLReportSection(endOffset);
						}
						rPLReportSection.Header = rPLItemMeasurement2;
						break;
					}
					case 5:
					{
						long endOffset2 = array[i].OffsetInfo.EndOffset;
						RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement(array[i]);
						rPLItemMeasurement.SetOffset(endOffset2, context);
						if (rPLReportSection == null)
						{
							rPLReportSection = new RPLReportSection(endOffset);
						}
						rPLReportSection.Footer = rPLItemMeasurement;
						break;
					}
					}
				}
			}
			binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			binaryReader.ReadByte();
			RPLReader.ReadReportSectionProps(rPLReportSection, binaryReader);
			if (rPLReportSection.BodyArea != null && num != -1)
			{
				offset = RPLReader.ResolveReportItemEnd(num, binaryReader, ref b);
				RPLItemMeasurement[] array2 = RPLReader.ReadItemMeasurements(context, binaryReader);
				if (array2 != null)
				{
					rPLReportSection.Columns = array2;
				}
				else
				{
					rPLReportSection.Columns = new RPLItemMeasurement[0];
				}
			}
			return rPLReportSection;
		}

		internal static void ReadReportSectionProps(RPLReportSection section, BinaryReader reader)
		{
			reader.ReadByte();
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 254:
				case 255:
					return;
				case 0:
					section.ID = reader.ReadString();
					break;
				case 1:
					section.ColumnCount = reader.ReadInt32();
					break;
				case 2:
					section.ColumnSpacing = reader.ReadSingle();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Section properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		private static RPLElementStyle ReadPageStyle(RPLContext context, BinaryReader reader)
		{
			RPLStyleProps rPLStyleProps = null;
			RPLStyleProps rPLStyleProps2 = null;
			byte b = reader.ReadByte();
			if (b == 0)
			{
				rPLStyleProps = RPLReader.ReadStyleProps(context, reader);
				b = reader.ReadByte();
				if (b != 1)
				{
					reader.BaseStream.Position -= 1L;
				}
			}
			if (b == 1)
			{
				rPLStyleProps2 = RPLReader.ReadStyleProps(context, reader);
			}
			if (rPLStyleProps2 == null && rPLStyleProps == null)
			{
				return null;
			}
			return new RPLElementStyle(rPLStyleProps2, rPLStyleProps);
		}

		internal static RPLItemProps ReadElementProps(long startOffset, RPLContext context)
		{
			byte b = 0;
			return RPLReader.ReadElementProps(startOffset, context, out b);
		}

		internal static RPLItemProps ReadElementProps(long startOffset, RPLContext context, out byte elementType)
		{
			BinaryReader binaryReader = context.BinaryReader;
			binaryReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
			elementType = binaryReader.ReadByte();
			RPLItemProps rPLItemProps = null;
			switch (elementType)
			{
			case 7:
				rPLItemProps = new RPLTextBoxProps();
				break;
			case 9:
				rPLItemProps = new RPLImageProps();
				break;
			case 11:
				rPLItemProps = new RPLChartProps();
				break;
			case 14:
				rPLItemProps = new RPLGaugePanelProps();
				break;
			case 21:
				rPLItemProps = new RPLMapProps();
				break;
			case 8:
				rPLItemProps = new RPLLineProps();
				break;
			case 12:
				rPLItemProps = new RPLSubReportProps();
				break;
			default:
				rPLItemProps = new RPLItemProps();
				break;
			}
			RPLReader.ReadItemProps(rPLItemProps, elementType, context, binaryReader);
			return rPLItemProps;
		}

		internal static RPLItemPropsDef ReadElementPropsDef(long startOffset, RPLContext context)
		{
			byte b = 0;
			return RPLReader.ReadElementPropsDef(startOffset, context, out b);
		}

		internal static RPLItemPropsDef ReadElementPropsDef(long startOffset, RPLContext context, out byte elementType)
		{
			BinaryReader binaryReader = context.BinaryReader;
			binaryReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
			elementType = binaryReader.ReadByte();
			return RPLReader.ReadItemPropsDef(elementType, context, binaryReader);
		}

		private static void ReadItemProps(RPLItemProps element, byte code, RPLContext context, BinaryReader reader)
		{
			long position = reader.BaseStream.Position;
			reader.ReadByte();
			switch (reader.ReadByte())
			{
			case 2:
			{
				long num = RPLReader.ReadOffset(reader);
				RPLItemPropsDef rPLItemPropsDef2 = null;
				if (context.SharedProps != null)
				{
					rPLItemPropsDef2 = (RPLItemPropsDef)context.SharedProps[num];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLItemPropsDef2 == null)
				{
					long position2 = reader.BaseStream.Position;
					reader.BaseStream.Seek(num, SeekOrigin.Begin);
					rPLItemPropsDef2 = RPLReader.ReadDefinition(code, context, reader);
					reader.BaseStream.Seek(position2, SeekOrigin.Begin);
					context.SharedProps.Add(num, rPLItemPropsDef2);
				}
				element.Definition = rPLItemPropsDef2;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return;
			}
			case 0:
			{
				RPLItemPropsDef rPLItemPropsDef = null;
				if (context.SharedProps != null)
				{
					rPLItemPropsDef = (RPLItemPropsDef)context.SharedProps[position];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLItemPropsDef != null)
				{
					RPLReader.ReadDefinition(code, context, reader);
				}
				else
				{
					rPLItemPropsDef = RPLReader.ReadDefinition(code, context, reader);
					context.SharedProps.Add(position, rPLItemPropsDef);
				}
				element.Definition = rPLItemPropsDef;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return;
			}
			}
			RPLReader.ReadInstance(element, code, context, reader);
		}

		private static RPLItemPropsDef ReadItemPropsDef(byte code, RPLContext context, BinaryReader reader)
		{
			RPLItemPropsDef rPLItemPropsDef = null;
			long position = reader.BaseStream.Position;
			reader.ReadByte();
			switch (reader.ReadByte())
			{
			case 2:
			{
				long num = RPLReader.ReadOffset(reader);
				if (context.SharedProps != null)
				{
					rPLItemPropsDef = (RPLItemPropsDef)context.SharedProps[num];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLItemPropsDef == null)
				{
					long position2 = reader.BaseStream.Position;
					reader.BaseStream.Seek(num, SeekOrigin.Begin);
					rPLItemPropsDef = RPLReader.ReadDefinition(code, context, reader);
					reader.BaseStream.Seek(position2, SeekOrigin.Begin);
					context.SharedProps.Add(num, rPLItemPropsDef);
				}
				break;
			}
			case 0:
				if (context.SharedProps != null)
				{
					rPLItemPropsDef = (RPLItemPropsDef)context.SharedProps[position];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLItemPropsDef == null)
				{
					rPLItemPropsDef = RPLReader.ReadDefinition(code, context, reader);
					context.SharedProps.Add(position, rPLItemPropsDef);
				}
				break;
			}
			return rPLItemPropsDef;
		}

		private static RPLImageData ReadImage(RPLContext context, bool backgroundImage, BinaryReader reader)
		{
			long num = reader.BaseStream.Position;
			if (backgroundImage)
			{
				reader.ReadByte();
			}
			else
			{
				num--;
			}
			byte b = reader.ReadByte();
			RPLImageData rPLImageData = null;
			switch (b)
			{
			case 1:
				return RPLReader.ReadImageProperties(reader);
			case 2:
			{
				long num2 = RPLReader.ReadOffset(reader);
				if (context.SharedImages != null)
				{
					rPLImageData = (RPLImageData)context.SharedImages[num2];
					if (rPLImageData != null)
					{
						return rPLImageData;
					}
				}
				else
				{
					context.SharedImages = new Hashtable();
				}
				long position = reader.BaseStream.Position;
				reader.BaseStream.Seek(num2 + 2, SeekOrigin.Begin);
				rPLImageData = RPLReader.ReadImageProperties(reader);
				reader.BaseStream.Seek(position, SeekOrigin.Begin);
				rPLImageData.IsShared = true;
				context.SharedImages.Add(num2, rPLImageData);
				break;
			}
			default:
				if (context.SharedImages != null)
				{
					rPLImageData = (RPLImageData)context.SharedImages[num];
				}
				else
				{
					context.SharedImages = new Hashtable();
				}
				if (rPLImageData == null)
				{
					rPLImageData = RPLReader.ReadImageProperties(reader);
					context.SharedImages.Add(num, rPLImageData);
				}
				else
				{
					RPLReader.ReadImageProperties(reader);
				}
				rPLImageData.IsShared = true;
				break;
			}
			return rPLImageData;
		}

		private static RPLImageData ReadImageProperties(BinaryReader reader)
		{
			RPLImageData rPLImageData = new RPLImageData();
			for (byte b = reader.ReadByte(); b != 255; b = reader.ReadByte())
			{
				switch (b)
				{
				case 0:
					rPLImageData.ImageMimeType = reader.ReadString();
					break;
				case 1:
					rPLImageData.ImageName = reader.ReadString();
					break;
				case 2:
					rPLImageData.ImageDataOffset = reader.BaseStream.Position;
					reader.BaseStream.Seek(reader.ReadInt32(), SeekOrigin.Current);
					break;
				case 3:
					if (rPLImageData.GDIImageProps == null)
					{
						rPLImageData.GDIImageProps = new GDIImageProps();
					}
					rPLImageData.GDIImageProps.Width = reader.ReadInt32();
					break;
				case 4:
					if (rPLImageData.GDIImageProps == null)
					{
						rPLImageData.GDIImageProps = new GDIImageProps();
					}
					rPLImageData.GDIImageProps.Height = reader.ReadInt32();
					break;
				case 5:
					if (rPLImageData.GDIImageProps == null)
					{
						rPLImageData.GDIImageProps = new GDIImageProps();
					}
					rPLImageData.GDIImageProps.HorizontalResolution = reader.ReadSingle();
					break;
				case 6:
					if (rPLImageData.GDIImageProps == null)
					{
						rPLImageData.GDIImageProps = new GDIImageProps();
					}
					rPLImageData.GDIImageProps.VerticalResolution = reader.ReadSingle();
					break;
				case 49:
				{
					int num = reader.ReadInt32();
					int num2 = reader.ReadInt32();
					int num3 = reader.ReadInt32();
					int num4 = reader.ReadInt32();
					if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
					{
						break;
					}
					Rectangle rectangle2 = rPLImageData.ImageConsolidationOffsets = new Rectangle(num, num2, num3, num4);
					break;
				}
				case 7:
					if (rPLImageData.GDIImageProps == null)
					{
						rPLImageData.GDIImageProps = new GDIImageProps();
					}
					switch (reader.ReadByte())
					{
					case 0:
						rPLImageData.GDIImageProps.RawFormat = ImageFormat.Bmp;
						break;
					case 2:
						rPLImageData.GDIImageProps.RawFormat = ImageFormat.Gif;
						break;
					case 1:
						rPLImageData.GDIImageProps.RawFormat = ImageFormat.Jpeg;
						break;
					case 3:
						rPLImageData.GDIImageProps.RawFormat = ImageFormat.Png;
						break;
					}
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Image properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
			}
			return rPLImageData;
		}

		internal static RPLTextBox ReadTextBoxStructure(long startOffset, RPLContext m_context)
		{
			RPLTextBox rPLTextBox = new RPLTextBox(startOffset, m_context);
			rPLTextBox.ParagraphCount = m_context.BinaryReader.ReadInt32();
			rPLTextBox.ParagraphOffsets = m_context.BinaryReader.BaseStream.Position;
			return rPLTextBox;
		}

		internal static RPLParagraph ReadParagraph(long paragraphOffset, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			binaryReader.BaseStream.Seek(paragraphOffset, SeekOrigin.Begin);
			if (binaryReader.ReadByte() != 19)
			{
				return null;
			}
			RPLSizes contentSizes = default(RPLSizes);
			RPLParagraphProps elementProps = RPLReader.ReadParagraphProps(context, binaryReader, out contentSizes);
			int textRunCount = binaryReader.ReadInt32();
			RPLParagraph rPLParagraph = new RPLParagraph(binaryReader.BaseStream.Position, context);
			rPLParagraph.TextRunCount = textRunCount;
			rPLParagraph.ContentSizes = contentSizes;
			rPLParagraph.ElementProps = elementProps;
			return rPLParagraph;
		}

		private static RPLParagraphProps ReadParagraphProps(RPLContext context, BinaryReader reader, out RPLSizes sizes)
		{
			sizes = null;
			RPLParagraphProps rPLParagraphProps = new RPLParagraphProps();
			long position = reader.BaseStream.Position;
			reader.ReadByte();
			switch (reader.ReadByte())
			{
			case 2:
			{
				long num = RPLReader.ReadOffset(reader);
				RPLParagraphPropsDef rPLParagraphPropsDef2 = null;
				if (context.SharedProps != null)
				{
					rPLParagraphPropsDef2 = (RPLParagraphPropsDef)context.SharedProps[num];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLParagraphPropsDef2 == null)
				{
					long position2 = reader.BaseStream.Position;
					reader.BaseStream.Seek(num, SeekOrigin.Begin);
					rPLParagraphPropsDef2 = RPLReader.ReadParagraphDefinition(context, reader);
					reader.BaseStream.Seek(position2, SeekOrigin.Begin);
					context.SharedProps.Add(num, rPLParagraphPropsDef2);
				}
				rPLParagraphProps.Definition = rPLParagraphPropsDef2;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return rPLParagraphProps;
			}
			case 0:
			{
				RPLParagraphPropsDef rPLParagraphPropsDef = null;
				if (context.SharedProps != null)
				{
					rPLParagraphPropsDef = (RPLParagraphPropsDef)context.SharedProps[position];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLParagraphPropsDef != null)
				{
					RPLReader.ReadParagraphDefinitionProps(context, reader);
				}
				else
				{
					rPLParagraphPropsDef = RPLReader.ReadParagraphDefinitionProps(context, reader);
					context.SharedProps.Add(position, rPLParagraphPropsDef);
				}
				rPLParagraphProps.Definition = rPLParagraphPropsDef;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return rPLParagraphProps;
			}
			}
			RPLReader.ReadParagraphInstance(rPLParagraphProps, context, reader, out sizes);
			reader.ReadByte();
			return rPLParagraphProps;
		}

		internal static RPLTextRun ReadTextRun(long textRunOffset, RPLContext context)
		{
			BinaryReader binaryReader = context.BinaryReader;
			binaryReader.BaseStream.Seek(textRunOffset, SeekOrigin.Begin);
			if (binaryReader.ReadByte() != 20)
			{
				return null;
			}
			RPLSizes contentSizes = default(RPLSizes);
			RPLTextRunProps rplElementProps = RPLReader.ReadTextRunProps(context, binaryReader, out contentSizes);
			RPLTextRun rPLTextRun = new RPLTextRun(rplElementProps);
			rPLTextRun.ContentSizes = contentSizes;
			return rPLTextRun;
		}

		private static RPLTextRunProps ReadTextRunProps(RPLContext context, BinaryReader reader, out RPLSizes sizes)
		{
			sizes = null;
			RPLTextRunProps rPLTextRunProps = new RPLTextRunProps();
			long position = reader.BaseStream.Position;
			reader.ReadByte();
			switch (reader.ReadByte())
			{
			case 2:
			{
				long num = RPLReader.ReadOffset(reader);
				RPLTextRunPropsDef rPLTextRunPropsDef2 = null;
				if (context.SharedProps != null)
				{
					rPLTextRunPropsDef2 = (RPLTextRunPropsDef)context.SharedProps[num];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLTextRunPropsDef2 == null)
				{
					long position2 = reader.BaseStream.Position;
					reader.BaseStream.Seek(num, SeekOrigin.Begin);
					rPLTextRunPropsDef2 = RPLReader.ReadTextRunDefinition(context, reader);
					reader.BaseStream.Seek(position2, SeekOrigin.Begin);
					context.SharedProps.Add(num, rPLTextRunPropsDef2);
				}
				rPLTextRunProps.Definition = rPLTextRunPropsDef2;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return rPLTextRunProps;
			}
			case 0:
			{
				RPLTextRunPropsDef rPLTextRunPropsDef = null;
				if (context.SharedProps != null)
				{
					rPLTextRunPropsDef = (RPLTextRunPropsDef)context.SharedProps[position];
				}
				else
				{
					context.SharedProps = new Hashtable();
				}
				if (rPLTextRunPropsDef != null)
				{
					RPLReader.ReadTextRunDefinitionProps(context, reader);
				}
				else
				{
					rPLTextRunPropsDef = RPLReader.ReadTextRunDefinitionProps(context, reader);
					context.SharedProps.Add(position, rPLTextRunPropsDef);
				}
				rPLTextRunProps.Definition = rPLTextRunPropsDef;
				if (reader.ReadByte() == 1)
				{
					break;
				}
				return rPLTextRunProps;
			}
			}
			RPLReader.ReadTextRunInstance(rPLTextRunProps, context, reader, out sizes);
			reader.ReadByte();
			return rPLTextRunProps;
		}

		internal static void ReadTablixStructure(RPLTablix tablix, RPLContext context, BinaryReader reader)
		{
			int num = 0;
			bool flag = false;
			byte b = reader.ReadByte();
			while (b != 255 && !flag)
			{
				switch (b)
				{
				case 0:
					tablix.ColumnHeaderRows = reader.ReadInt32();
					break;
				case 1:
					tablix.RowHeaderColumns = reader.ReadInt32();
					break;
				case 7:
					tablix.ContentLeft = reader.ReadSingle();
					break;
				case 6:
					tablix.ContentTop = reader.ReadSingle();
					break;
				case 2:
					tablix.ColsBeforeRowHeaders = reader.ReadInt32();
					break;
				case 3:
					tablix.LayoutDirection = (RPLFormat.Directions)reader.ReadByte();
					break;
				case 4:
					num = reader.ReadInt32();
					if (num > 0)
					{
						tablix.ColumnWidths = new float[num];
						tablix.FixedColumns = new bool[num];
						for (int l = 0; l < num; l++)
						{
							tablix.ColumnWidths[l] = reader.ReadSingle();
							tablix.FixedColumns[l] = reader.ReadBoolean();
						}
					}
					break;
				case 5:
					num = reader.ReadInt32();
					if (num > 0)
					{
						tablix.RowHeights = new float[num];
						tablix.RowsState = new byte[num];
						for (int k = 0; k < num; k++)
						{
							tablix.RowHeights[k] = reader.ReadSingle();
							tablix.RowsState[k] = reader.ReadByte();
						}
					}
					break;
				case 8:
					tablix.NextRowStart = reader.BaseStream.Position - 1;
					flag = true;
					break;
				case 14:
					num = reader.ReadInt32();
					if (num > 0)
					{
						tablix.TablixRowMembersDef = new RPLTablixMemberDef[num];
						for (int j = 0; j < num; j++)
						{
							tablix.TablixRowMembersDef[j] = RPLReader.ReadTablixMemberDef(reader);
						}
					}
					break;
				case 15:
					num = reader.ReadInt32();
					if (num > 0)
					{
						tablix.TablixColMembersDef = new RPLTablixMemberDef[num];
						for (int i = 0; i < num; i++)
						{
							tablix.TablixColMembersDef[i] = RPLReader.ReadTablixMemberDef(reader);
						}
					}
					break;
				}
				b = reader.ReadByte();
			}
		}

		internal static RPLTablixMemberDef ReadTablixMemberDef(BinaryReader reader)
		{
			reader.ReadByte();
			RPLTablixMemberDef rPLTablixMemberDef = new RPLTablixMemberDef();
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 0:
					rPLTablixMemberDef.DefinitionPath = reader.ReadString();
					break;
				case 1:
					rPLTablixMemberDef.Level = reader.ReadInt32();
					break;
				case 2:
					rPLTablixMemberDef.MemberCellIndex = reader.ReadInt32();
					break;
				case 3:
					rPLTablixMemberDef.State = reader.ReadByte();
					break;
				case 255:
					return rPLTablixMemberDef;
				}
				b = reader.ReadByte();
			}
		}

		internal static RPLTablixRow ReadTablixRow(long rowOffset, RPLContext context, RPLTablixMemberDef[] rowMembersDef, RPLTablixMemberDef[] colMembersDef, ref long nextRowStart)
		{
			BinaryReader binaryReader = context.BinaryReader;
			binaryReader.BaseStream.Seek(rowOffset, SeekOrigin.Begin);
			if (binaryReader.ReadByte() != 8)
			{
				return null;
			}
			List<RPLTablixCell> list = new List<RPLTablixCell>();
			List<RPLTablixMemberCell> list2 = null;
			bool flag = true;
			RPLTablixCell rPLTablixCell = null;
			int num = -1;
			int bodyStart = -1;
			for (byte b = binaryReader.ReadByte(); b != 255; b = binaryReader.ReadByte())
			{
				byte b2 = b;
				if (b2 == 9)
				{
					bodyStart = list.Count;
					long offset = RPLReader.ReadOffset(binaryReader);
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
					binaryReader.ReadByte();
					int rowIndex = binaryReader.ReadInt32();
					for (byte b3 = binaryReader.ReadByte(); b3 != 255; b3 = binaryReader.ReadByte())
					{
						rPLTablixCell = RPLReader.ReadTablixCell(b3, context, binaryReader, null, null);
						rPLTablixCell.RowIndex = rowIndex;
						list.Add(rPLTablixCell);
					}
					binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
				}
				else
				{
					rPLTablixCell = RPLReader.ReadTablixCell(b, context, binaryReader, rowMembersDef, colMembersDef);
					if (rPLTablixCell.ColSpan == 0 || rPLTablixCell.RowSpan == 0)
					{
						if (flag && b == 12)
						{
							flag = false;
						}
						if (list2 == null)
						{
							list2 = new List<RPLTablixMemberCell>();
						}
						list2.Add((RPLTablixMemberCell)rPLTablixCell);
					}
					else
					{
						if (num == -1)
						{
							num = list.Count;
						}
						list.Add(rPLTablixCell);
					}
				}
			}
			nextRowStart = binaryReader.BaseStream.Position;
			if (num >= 0)
			{
				return new RPLTablixFullRow(list, list2, num, bodyStart);
			}
			if (list2 != null)
			{
				if (flag)
				{
					return new RPLTablixOmittedRow(list2);
				}
				return new RPLTablixFullRow(list, list2, num, bodyStart);
			}
			return new RPLTablixRow(list);
		}

		private static RPLTablixCell ReadTablixCell(byte type, RPLContext context, BinaryReader reader, RPLTablixMemberDef[] rowMembersDef, RPLTablixMemberDef[] colMembersDef)
		{
			RPLTablixCell rPLTablixCell = null;
			RPLTablixMemberCell rPLTablixMemberCell = null;
			RPLSizes rPLSizes = null;
			switch (type)
			{
			case 10:
				rPLTablixCell = new RPLTablixCornerCell();
				break;
			case 13:
				rPLTablixCell = new RPLTablixCell();
				break;
			default:
				rPLTablixMemberCell = new RPLTablixMemberCell();
				rPLTablixCell = rPLTablixMemberCell;
				break;
			}
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 4:
					rPLTablixCell.SetOffset(RPLReader.ReadOffset(reader), context);
					break;
				case 5:
					rPLTablixCell.ColSpan = reader.ReadInt32();
					break;
				case 6:
					rPLTablixCell.RowSpan = reader.ReadInt32();
					break;
				case 8:
					rPLTablixCell.ColIndex = reader.ReadInt32();
					break;
				case 9:
					rPLTablixCell.RowIndex = reader.ReadInt32();
					break;
				case 10:
					rPLTablixMemberCell.GroupLabel = reader.ReadString();
					break;
				case 14:
					rPLTablixMemberCell.RecursiveToggleLevel = reader.ReadInt32();
					break;
				case 11:
					rPLTablixMemberCell.UniqueName = reader.ReadString();
					break;
				case 12:
					rPLTablixMemberCell.State = reader.ReadByte();
					break;
				case 13:
					rPLTablixCell.ElementState = reader.ReadByte();
					break;
				case 7:
				{
					int num = reader.ReadInt32();
					if (type == 12)
					{
						rPLTablixMemberCell.TablixMemberDef = rowMembersDef[num];
					}
					else
					{
						rPLTablixMemberCell.TablixMemberDef = colMembersDef[num];
					}
					break;
				}
				case 1:
					if (rPLSizes == null)
					{
						rPLSizes = new RPLSizes();
					}
					rPLSizes.Left = reader.ReadSingle();
					break;
				case 0:
					if (rPLSizes == null)
					{
						rPLSizes = new RPLSizes();
					}
					rPLSizes.Top = reader.ReadSingle();
					break;
				case 2:
					if (rPLSizes == null)
					{
						rPLSizes = new RPLSizes();
					}
					rPLSizes.Width = reader.ReadSingle();
					break;
				case 3:
					if (rPLSizes == null)
					{
						rPLSizes = new RPLSizes();
					}
					rPLSizes.Height = reader.ReadSingle();
					break;
				case 255:
					rPLTablixCell.ContentSizes = rPLSizes;
					return rPLTablixCell;
				}
				b = reader.ReadByte();
			}
		}

		private static RPLItemPropsDef ReadDefinition(byte elementType, RPLContext context, BinaryReader reader)
		{
			switch (elementType)
			{
			case 8:
				return RPLReader.ReadLineDefinition(context, reader);
			case 10:
				return RPLReader.ReadRectangleDefinition(context, reader);
			case 9:
				return RPLReader.ReadImageDefinition(context, reader);
			case 4:
				return RPLReader.ReadHeaderFooterDefinition(context, reader);
			case 5:
				return RPLReader.ReadHeaderFooterDefinition(context, reader);
			case 12:
				return RPLReader.ReadSubReportDefinition(context, reader);
			case 7:
				return RPLReader.ReadTextBoxDefinition(context, reader);
			default:
				return RPLReader.ReadElementDefinition(context, reader);
			}
		}

		private static void ReadBasedDefinitionProps(RPLItemPropsDef props, RPLContext context, BinaryReader reader, byte token)
		{
			switch (token)
			{
			case 7:
				break;
			case 1:
				props.ID = reader.ReadString();
				break;
			case 2:
				props.Name = reader.ReadString();
				break;
			case 5:
				props.ToolTip = reader.ReadString();
				break;
			case 4:
				props.Bookmark = reader.ReadString();
				break;
			case 3:
				props.Label = reader.ReadString();
				break;
			case 8:
				props.ToggleItem = reader.ReadString();
				break;
			case 6:
				props.SharedStyle = RPLReader.ReadStyle(context, reader);
				break;
			}
		}

		private static RPLItemPropsDef ReadElementDefinition(RPLContext context, BinaryReader reader)
		{
			RPLItemPropsDef rPLItemPropsDef = new RPLItemPropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				RPLReader.ReadBasedDefinitionProps(rPLItemPropsDef, context, reader, b);
				b = reader.ReadByte();
			}
			return rPLItemPropsDef;
		}

		private static RPLItemPropsDef ReadRectangleDefinition(RPLContext context, BinaryReader reader)
		{
			RPLRectanglePropsDef rPLRectanglePropsDef = new RPLRectanglePropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				byte b2 = b;
				if (b2 == 43)
				{
					rPLRectanglePropsDef.LinkToChildId = reader.ReadString();
				}
				else
				{
					RPLReader.ReadBasedDefinitionProps(rPLRectanglePropsDef, context, reader, b);
				}
				b = reader.ReadByte();
			}
			return rPLRectanglePropsDef;
		}

		private static RPLItemPropsDef ReadHeaderFooterDefinition(RPLContext context, BinaryReader reader)
		{
			RPLHeaderFooterPropsDef rPLHeaderFooterPropsDef = new RPLHeaderFooterPropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 44:
					rPLHeaderFooterPropsDef.PrintOnFirstPage = reader.ReadBoolean();
					break;
				case 47:
					rPLHeaderFooterPropsDef.PrintBetweenSections = reader.ReadBoolean();
					break;
				default:
					RPLReader.ReadBasedDefinitionProps(rPLHeaderFooterPropsDef, context, reader, b);
					break;
				}
				b = reader.ReadByte();
			}
			return rPLHeaderFooterPropsDef;
		}

		private static RPLSubReportPropsDef ReadSubReportDefinition(RPLContext context, BinaryReader reader)
		{
			RPLSubReportPropsDef rPLSubReportPropsDef = new RPLSubReportPropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				byte b2 = b;
				if (b2 == 15)
				{
					rPLSubReportPropsDef.ReportName = reader.ReadString();
				}
				else
				{
					RPLReader.ReadBasedDefinitionProps(rPLSubReportPropsDef, context, reader, b);
				}
				b = reader.ReadByte();
			}
			return rPLSubReportPropsDef;
		}

		private static RPLLinePropsDef ReadLineDefinition(RPLContext context, BinaryReader reader)
		{
			RPLLinePropsDef rPLLinePropsDef = new RPLLinePropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				byte b2 = b;
				if (b2 == 24)
				{
					rPLLinePropsDef.Slant = reader.ReadBoolean();
				}
				else
				{
					RPLReader.ReadBasedDefinitionProps(rPLLinePropsDef, context, reader, b);
				}
				b = reader.ReadByte();
			}
			return rPLLinePropsDef;
		}

		private static RPLImagePropsDef ReadImageDefinition(RPLContext context, BinaryReader reader)
		{
			RPLImagePropsDef rPLImagePropsDef = new RPLImagePropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				byte b2 = b;
				if (b2 == 41)
				{
					rPLImagePropsDef.Sizing = (RPLFormat.Sizings)reader.ReadByte();
				}
				else
				{
					RPLReader.ReadBasedDefinitionProps(rPLImagePropsDef, context, reader, b);
				}
				b = reader.ReadByte();
			}
			return rPLImagePropsDef;
		}

		private static RPLTextBoxPropsDef ReadTextBoxDefinition(RPLContext context, BinaryReader reader)
		{
			RPLTextBoxPropsDef rPLTextBoxPropsDef = new RPLTextBoxPropsDef();
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 32:
					rPLTextBoxPropsDef.IsToggleParent = reader.ReadBoolean();
					break;
				case 25:
					rPLTextBoxPropsDef.CanGrow = reader.ReadBoolean();
					break;
				case 26:
					rPLTextBoxPropsDef.CanShrink = reader.ReadBoolean();
					break;
				case 29:
					rPLTextBoxPropsDef.CanSort = reader.ReadBoolean();
					break;
				case 31:
					rPLTextBoxPropsDef.Formula = reader.ReadString();
					break;
				case 27:
					rPLTextBoxPropsDef.Value = reader.ReadString();
					break;
				case 33:
					rPLTextBoxPropsDef.SharedTypeCode = (TypeCode)reader.ReadByte();
					break;
				case 35:
					rPLTextBoxPropsDef.IsSimple = reader.ReadBoolean();
					break;
				case 45:
					rPLTextBoxPropsDef.FormattedValueExpressionBased = reader.ReadBoolean();
					break;
				default:
					RPLReader.ReadBasedDefinitionProps(rPLTextBoxPropsDef, context, reader, b);
					break;
				}
				b = reader.ReadByte();
			}
			return rPLTextBoxPropsDef;
		}

		private static RPLParagraphPropsDef ReadParagraphDefinition(RPLContext context, BinaryReader reader)
		{
			reader.ReadByte();
			reader.ReadByte();
			return RPLReader.ReadParagraphDefinitionProps(context, reader);
		}

		private static RPLParagraphPropsDef ReadParagraphDefinitionProps(RPLContext context, BinaryReader reader)
		{
			RPLParagraphPropsDef rPLParagraphPropsDef = new RPLParagraphPropsDef();
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 9:
					rPLParagraphPropsDef.LeftIndent = new RPLReportSize(reader.ReadString());
					break;
				case 10:
					rPLParagraphPropsDef.RightIndent = new RPLReportSize(reader.ReadString());
					break;
				case 11:
					rPLParagraphPropsDef.HangingIndent = new RPLReportSize(reader.ReadString());
					break;
				case 7:
					rPLParagraphPropsDef.ListStyle = (RPLFormat.ListStyles)reader.ReadByte();
					break;
				case 8:
					rPLParagraphPropsDef.ListLevel = reader.ReadInt32();
					break;
				case 12:
					rPLParagraphPropsDef.SpaceBefore = new RPLReportSize(reader.ReadString());
					break;
				case 13:
					rPLParagraphPropsDef.SpaceAfter = new RPLReportSize(reader.ReadString());
					break;
				case 5:
					rPLParagraphPropsDef.ID = reader.ReadString();
					break;
				case 6:
					rPLParagraphPropsDef.SharedStyle = RPLReader.ReadStyle(context, reader);
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Unknown token while reading TextBox.Paragraph", b.ToString("x", CultureInfo.InvariantCulture)));
				case 255:
					return rPLParagraphPropsDef;
				}
				b = reader.ReadByte();
			}
		}

		private static RPLTextRunPropsDef ReadTextRunDefinition(RPLContext context, BinaryReader reader)
		{
			reader.ReadByte();
			reader.ReadByte();
			return RPLReader.ReadTextRunDefinitionProps(context, reader);
		}

		private static RPLTextRunPropsDef ReadTextRunDefinitionProps(RPLContext context, BinaryReader reader)
		{
			RPLTextRunPropsDef rPLTextRunPropsDef = new RPLTextRunPropsDef();
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 7:
					rPLTextRunPropsDef.Markup = (RPLFormat.MarkupStyles)reader.ReadByte();
					break;
				case 8:
					rPLTextRunPropsDef.Label = reader.ReadString();
					break;
				case 9:
					rPLTextRunPropsDef.ToolTip = reader.ReadString();
					break;
				case 12:
					rPLTextRunPropsDef.Formula = reader.ReadString();
					break;
				case 5:
					rPLTextRunPropsDef.ID = reader.ReadString();
					break;
				case 6:
					rPLTextRunPropsDef.SharedStyle = RPLReader.ReadStyle(context, reader);
					break;
				case 10:
					rPLTextRunPropsDef.Value = reader.ReadString();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Unknown token while reading TextBox.Paragraph.TextRun", b.ToString("x", CultureInfo.InvariantCulture)));
				case 255:
					return rPLTextRunPropsDef;
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadInstance(RPLItemProps instance, byte code, RPLContext context, BinaryReader reader)
		{
			switch (code)
			{
			case 7:
				RPLReader.ReadTextBoxInstance((RPLTextBoxProps)instance, context, reader);
				break;
			case 11:
			case 14:
			case 21:
				RPLReader.ReadDynamicImageInstance((RPLDynamicImageProps)instance, context, reader);
				break;
			case 9:
				RPLReader.ReadImageInstance((RPLImageProps)instance, context, reader);
				break;
			case 12:
				RPLReader.ReadSubReportInstance((RPLSubReportProps)instance, context, reader);
				break;
			default:
				RPLReader.ReadElementInstance(instance, context, reader);
				break;
			}
		}

		private static void ReadBasedInstanceProps(RPLItemProps props, RPLContext context, BinaryReader reader, byte token)
		{
			switch (token)
			{
			case 1:
			case 2:
				break;
			case 0:
				props.UniqueName = reader.ReadString();
				break;
			case 3:
				props.Label = reader.ReadString();
				break;
			case 4:
				props.Bookmark = reader.ReadString();
				break;
			case 5:
				props.ToolTip = reader.ReadString();
				break;
			case 6:
				props.NonSharedStyle = RPLReader.ReadStyle(context, reader);
				break;
			}
		}

		private static void ReadElementInstance(RPLItemProps props, RPLContext context, BinaryReader reader)
		{
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				RPLReader.ReadBasedInstanceProps(props, context, reader, b);
				b = reader.ReadByte();
			}
		}

		private static void ReadSubReportInstance(RPLSubReportProps props, RPLContext context, BinaryReader reader)
		{
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				byte b2 = b;
				if (b2 == 11)
				{
					props.Language = reader.ReadString();
				}
				else
				{
					RPLReader.ReadBasedInstanceProps(props, context, reader, b);
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadImageInstance(RPLImageProps props, RPLContext context, BinaryReader reader)
		{
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 7:
					props.ActionInfo = RPLReader.ReadActionInfo(reader);
					break;
				case 38:
					props.ActionImageMapAreas = RPLReader.ReadActionImageMapAreas(reader);
					break;
				case 42:
					props.Image = RPLReader.ReadImage(context, false, reader);
					break;
				default:
					RPLReader.ReadBasedInstanceProps(props, context, reader, b);
					break;
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadDynamicImageInstance(RPLDynamicImageProps dynamicImage, RPLContext context, BinaryReader reader)
		{
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 38:
					dynamicImage.ActionImageMapAreas = RPLReader.ReadActionImageMapAreas(reader);
					break;
				case 39:
					dynamicImage.DynamicImageContentOffset = reader.BaseStream.Position;
					reader.BaseStream.Seek(reader.ReadInt32(), SeekOrigin.Current);
					break;
				case 40:
					dynamicImage.StreamName = reader.ReadString();
					break;
				case 47:
				case 49:
				{
					int num = reader.ReadInt32();
					int num2 = reader.ReadInt32();
					int num3 = reader.ReadInt32();
					int num4 = reader.ReadInt32();
					if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
					{
						break;
					}
					Rectangle rectangle2 = dynamicImage.ImageConsolidationOffsets = new Rectangle(num, num2, num3, num4);
					break;
				}
				default:
					RPLReader.ReadBasedInstanceProps(dynamicImage, context, reader, b);
					break;
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadTextBoxInstance(RPLTextBoxProps props, RPLContext context, BinaryReader reader)
		{
			if (props.Definition != null)
			{
				props.TypeCode = ((RPLTextBoxPropsDef)props.Definition).SharedTypeCode;
			}
			byte b = reader.ReadByte();
			while (b != 255 && b != 254)
			{
				switch (b)
				{
				case 7:
					props.ActionInfo = RPLReader.ReadActionInfo(reader);
					break;
				case 27:
					props.Value = reader.ReadString();
					break;
				case 28:
					props.ToggleState = reader.ReadBoolean();
					break;
				case 30:
					props.SortState = (RPLFormat.SortOptions)reader.ReadByte();
					break;
				case 32:
					props.IsToggleParent = reader.ReadBoolean();
					break;
				case 36:
					props.ContentHeight = reader.ReadSingle();
					break;
				case 37:
					props.ContentOffset = reader.ReadSingle();
					break;
				case 46:
					props.ProcessedWithError = reader.ReadBoolean();
					break;
				case 33:
					props.TypeCode = (TypeCode)reader.ReadByte();
					break;
				case 34:
					switch (props.TypeCode)
					{
					case TypeCode.Char:
						props.OriginalValue = reader.ReadChar();
						break;
					case TypeCode.String:
						props.OriginalValue = reader.ReadString();
						break;
					case TypeCode.Boolean:
						props.OriginalValue = reader.ReadBoolean();
						break;
					case TypeCode.Byte:
						props.OriginalValue = reader.ReadByte();
						break;
					case TypeCode.Int16:
						props.OriginalValue = reader.ReadInt16();
						break;
					case TypeCode.Int32:
						props.OriginalValue = reader.ReadInt32();
						break;
					case TypeCode.Int64:
						props.OriginalValue = reader.ReadInt64();
						break;
					case TypeCode.Single:
						props.OriginalValue = reader.ReadSingle();
						break;
					case TypeCode.Decimal:
						props.OriginalValue = reader.ReadDecimal();
						break;
					case TypeCode.Double:
						props.OriginalValue = reader.ReadDouble();
						break;
					case TypeCode.DateTime:
						props.OriginalValue = DateTime.FromBinary(reader.ReadInt64());
						break;
					default:
						props.TypeCode = TypeCode.String;
						props.OriginalValue = reader.ReadString();
						break;
					}
					break;
				default:
					RPLReader.ReadBasedInstanceProps(props, context, reader, b);
					break;
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadParagraphInstance(RPLParagraphProps props, RPLContext context, BinaryReader reader, out RPLSizes size)
		{
			size = null;
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 255:
					return;
				case 9:
					props.LeftIndent = new RPLReportSize(reader.ReadString());
					break;
				case 10:
					props.RightIndent = new RPLReportSize(reader.ReadString());
					break;
				case 11:
					props.HangingIndent = new RPLReportSize(reader.ReadString());
					break;
				case 7:
					props.ListStyle = (RPLFormat.ListStyles)reader.ReadByte();
					break;
				case 8:
					props.ListLevel = reader.ReadInt32();
					break;
				case 14:
					props.ParagraphNumber = reader.ReadInt32();
					break;
				case 12:
					props.SpaceBefore = new RPLReportSize(reader.ReadString());
					break;
				case 13:
					props.SpaceAfter = new RPLReportSize(reader.ReadString());
					break;
				case 15:
					props.FirstLine = reader.ReadBoolean();
					break;
				case 4:
					props.UniqueName = reader.ReadString();
					break;
				case 6:
					props.NonSharedStyle = RPLReader.ReadStyle(context, reader);
					break;
				case 1:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Left = reader.ReadSingle();
					break;
				case 0:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Top = reader.ReadSingle();
					break;
				case 2:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Width = reader.ReadSingle();
					break;
				case 3:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Height = reader.ReadSingle();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Unknown token while reading TextBox.Paragraph", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		private static void ReadTextRunInstance(RPLTextRunProps props, RPLContext context, BinaryReader reader, out RPLSizes size)
		{
			size = null;
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 255:
					return;
				case 7:
					props.Markup = (RPLFormat.MarkupStyles)reader.ReadByte();
					break;
				case 10:
					props.Value = reader.ReadString();
					break;
				case 9:
					props.ToolTip = reader.ReadString();
					break;
				case 4:
					props.UniqueName = reader.ReadString();
					break;
				case 11:
				{
					RPLActionInfo rPLActionInfo2 = props.ActionInfo = RPLReader.ReadActionInfo(reader);
					break;
				}
				case 6:
					props.NonSharedStyle = RPLReader.ReadStyle(context, reader);
					break;
				case 13:
					props.ProcessedWithError = reader.ReadBoolean();
					break;
				case 1:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Left = reader.ReadSingle();
					break;
				case 0:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Top = reader.ReadSingle();
					break;
				case 2:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Width = reader.ReadSingle();
					break;
				case 3:
					if (size == null)
					{
						size = new RPLSizes();
					}
					size.Height = reader.ReadSingle();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Unknown token while reading TextBox.Paragraph.TextRun", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
		}

		private static RPLActionInfo ReadActionInfo(BinaryReader reader)
		{
			RPLActionInfo rPLActionInfo = new RPLActionInfo();
			byte b = reader.ReadByte();
			while (b != 255)
			{
				byte b2 = b;
				if (b2 == 2)
				{
					int num = reader.ReadInt32();
					RPLAction[] array = new RPLAction[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = RPLReader.ReadAction(reader);
					}
					rPLActionInfo.Actions = array;
					b = reader.ReadByte();
					continue;
				}
				throw new Exception(RPLRes.InvalidRPLToken("Action info properties", b.ToString("x", CultureInfo.InvariantCulture)));
			}
			return rPLActionInfo;
		}

		private static RPLActionInfoWithImageMap ReadActionInfoWithMaps(BinaryReader reader)
		{
			reader.ReadByte();
			byte b = reader.ReadByte();
			if (b == 255)
			{
				return null;
			}
			RPLActionInfoWithImageMap rPLActionInfoWithImageMap = new RPLActionInfoWithImageMap();
			while (b != 255)
			{
				switch (b)
				{
				case 2:
				{
					int num = reader.ReadInt32();
					RPLAction[] array = new RPLAction[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = RPLReader.ReadAction(reader);
					}
					rPLActionInfoWithImageMap.Actions = array;
					break;
				}
				case 10:
					rPLActionInfoWithImageMap.ImageMaps = RPLReader.ReadImageMapAreas(reader);
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Action info with image map properties", b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
			return rPLActionInfoWithImageMap;
		}

		private static RPLAction ReadAction(BinaryReader reader)
		{
			RPLAction rPLAction = new RPLAction();
			reader.ReadByte();
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 4:
					rPLAction.Label = reader.ReadString();
					break;
				case 6:
					rPLAction.Hyperlink = reader.ReadString();
					break;
				case 7:
					rPLAction.BookmarkLink = reader.ReadString();
					break;
				case 8:
					rPLAction.DrillthroughId = reader.ReadString();
					break;
				case 9:
					rPLAction.DrillthroughUrl = reader.ReadString();
					break;
				default:
					throw new Exception(RPLRes.InvalidRPLToken("Action properties", b.ToString("x", CultureInfo.InvariantCulture)));
				case 255:
					return rPLAction;
				}
				b = reader.ReadByte();
			}
		}

		private static RPLActionInfoWithImageMap[] ReadActionImageMapAreas(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				return null;
			}
			RPLActionInfoWithImageMap[] array = new RPLActionInfoWithImageMap[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = RPLReader.ReadActionInfoWithMaps(reader);
			}
			return array;
		}

		private static RPLImageMapCollection ReadImageMapAreas(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				return null;
			}
			RPLImageMap[] array = new RPLImageMap[num];
			RPLImageMap rPLImageMap = null;
			for (int i = 0; i < num; i++)
			{
				rPLImageMap = new RPLImageMap();
				rPLImageMap.Shape = (RPLFormat.ShapeType)reader.ReadByte();
				int num2 = reader.ReadInt32();
				if (num2 > 0)
				{
					float[] array2 = new float[num2];
					for (int j = 0; j < num2; j++)
					{
						array2[j] = reader.ReadSingle();
					}
					rPLImageMap.Coordinates = array2;
				}
				byte b = reader.ReadByte();
				if (b == 5)
				{
					rPLImageMap.ToolTip = reader.ReadString();
				}
				array[i] = rPLImageMap;
			}
			return new RPLImageMapCollection(array);
		}
	}
}
