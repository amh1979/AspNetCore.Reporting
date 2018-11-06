using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal static class MsoDrawingGroup
	{
		private const int MaxRecordLength = 8224;

		private const int BSEBlipLength = 69;

		private const int ShapePropertyLength = 50;

		private const int BSELength = 44;

		private static byte[] FillRecordWithBlips(Escher.DrawingGroupContainer dggContainer, ArrayList blipList, Hashtable bseList, Stream stream, ref int currentBlip, ref int currentLength, ref Stream currentImageStream)
		{
			byte[] array = null;
			if (currentBlip >= blipList.Count)
			{
				return null;
			}
			do
			{
				byte[] checkSum = ((Escher.Blip)blipList[currentBlip]).CheckSum;
				string @string = Encoding.ASCII.GetString(checkSum);
				if (bseList.ContainsKey(@string))
				{
					byte[] array2 = new byte[69];
					((Escher.BlipStoreEntry)bseList[@string]).GetData().CopyTo(array2, 0);
					((Escher.Blip)blipList[currentBlip]).GetHeaderData().CopyTo(array2, 44);
					int num = currentLength + 69;
					currentImageStream = dggContainer.StreamList[currentBlip].Stream;
					currentImageStream.Seek(dggContainer.StreamList[currentBlip].Offset, SeekOrigin.Begin);
					if ((int)(currentImageStream.Length - currentImageStream.Position + num) > 8224)
					{
						int num2 = 8224 - currentLength;
						if (num2 <= 69)
						{
							stream.Write(array2, 0, num2);
							if (num2 < 69)
							{
								array = new byte[69 - num2];
								Array.Copy(array2, num2, array, 0, array.Length);
							}
							currentLength += num2;
						}
						else
						{
							stream.Write(array2, 0, 69);
							int num3 = num2 - 69;
							byte[] array3 = new byte[num3];
							currentImageStream.Read(array3, 0, num3);
							stream.Write(array3, 0, array3.Length);
							currentLength += num2;
							dggContainer.StreamList[currentBlip].Offset = (int)currentImageStream.Position;
						}
					}
					else
					{
						stream.Write(array2, 0, array2.Length);
						int num4 = (int)(currentImageStream.Length - currentImageStream.Position);
						byte[] array4 = new byte[num4];
						currentImageStream.Read(array4, 0, num4);
						stream.Write(array4, 0, array4.Length);
						currentLength += num4 + 69;
						dggContainer.StreamList[currentBlip].Offset = -1;
						currentImageStream.Close();
						currentImageStream = null;
						currentBlip++;
					}
				}
			}
			while (currentLength < 8224 && currentBlip < blipList.Count);
			return array;
		}

		private static void WriteDGHeader(BinaryWriter output, byte[] buffer, ref int currentLength, ref int numberOfContinues, uint lastContinueLength)
		{
			int num = 0;
			int num2 = buffer.Length;
			int size = 8224;
			RSTrace.ExcelRendererTracer.Assert(currentLength < 8224, "Current stream position cannot exceed the Max Record Length");
			do
			{
				if (num2 + currentLength < 8224)
				{
					output.BaseStream.Write(buffer, num, num2);
					currentLength += num2;
					num2 = 0;
				}
				else
				{
					output.BaseStream.Write(buffer, num, 8224 - currentLength);
					num = 8224 - currentLength;
					num2 -= num;
					numberOfContinues--;
					if (numberOfContinues == 0)
					{
						size = (ushort)lastContinueLength;
					}
					RecordFactory.WriteHeader(output, 60, size);
					currentLength = 0;
				}
			}
			while (num2 > 0);
		}

		internal static void WriteToStream(BinaryWriter output, Escher.DrawingGroupContainer dggContainer)
		{
			if (dggContainer != null)
			{
				int num = 0;
				uint num2 = dggContainer.Length + 8;
				uint num3 = 0u;
				int size;
				if (num2 > 8224)
				{
					size = 8224;
					num = (int)num2 / 8224;
					num3 = (uint)(num2 - num * 8224);
				}
				else
				{
					size = (ushort)num2;
				}
				RecordFactory.WriteHeader(output, 235, size);
				byte[] drawingGroupContainerData = dggContainer.DrawingGroupContainerData;
				int num4 = 0;
				MsoDrawingGroup.WriteDGHeader(output, drawingGroupContainerData, ref num4, ref num, num3);
				drawingGroupContainerData = dggContainer.DrawingGroupData;
				MsoDrawingGroup.WriteDGHeader(output, drawingGroupContainerData, ref num4, ref num, num3);
				drawingGroupContainerData = dggContainer.BStoreContainerData;
				MsoDrawingGroup.WriteDGHeader(output, drawingGroupContainerData, ref num4, ref num, num3);
				Hashtable bSEList = dggContainer.BSEList;
				ArrayList blipList = dggContainer.BlipList;
				int num5 = 0;
				Stream stream = null;
				int num6 = 0;
				byte[] array = MsoDrawingGroup.FillRecordWithBlips(dggContainer, blipList, bSEList, output.BaseStream, ref num5, ref num4, ref stream);
				if (array == null && num4 < 8224 && num == 1)
				{
					MsoDrawingGroup.WriteShapeProperties(dggContainer, output.BaseStream, ref num6, num4);
				}
				for (int i = 0; i < num; i++)
				{
					int size2 = (i != num - 1) ? 8224 : ((ushort)num3);
					RecordFactory.WriteHeader(output, 60, size2);
					num4 = 0;
					if (array != null)
					{
						output.BaseStream.Write(array, 0, array.Length);
						num4 += array.Length;
						array = null;
					}
					if (stream == null)
					{
						array = MsoDrawingGroup.FillRecordWithBlips(dggContainer, blipList, bSEList, output.BaseStream, ref num5, ref num4, ref stream);
						if (num4 < 8224)
						{
							MsoDrawingGroup.WriteShapeProperties(dggContainer, output.BaseStream, ref num6, num4);
						}
					}
					else
					{
						int num7 = 8224 - num4;
						int num8 = (int)(stream.Length - stream.Position);
						if (num8 <= num7)
						{
							if (num8 > 0)
							{
								byte[] array2 = new byte[num8];
								stream.Read(array2, 0, array2.Length);
								output.BaseStream.Write(array2, 0, array2.Length);
								num4 += array2.Length;
							}
							dggContainer.StreamList[num5].Offset = -1;
							stream.Close();
							stream = null;
							num5++;
							array = MsoDrawingGroup.FillRecordWithBlips(dggContainer, blipList, bSEList, output.BaseStream, ref num5, ref num4, ref stream);
							if (num4 < 8224)
							{
								MsoDrawingGroup.WriteShapeProperties(dggContainer, output.BaseStream, ref num6, num4);
							}
						}
						else
						{
							byte[] buffer = new byte[num7];
							stream.Read(buffer, 0, num7);
							output.BaseStream.Write(buffer, 0, num7);
							dggContainer.StreamList[num5].Offset = (int)stream.Position;
						}
					}
				}
				output.BaseStream.Write(dggContainer.ShapePropertyData, num6, 50 - num6);
			}
		}

		private static void WriteShapeProperties(Escher.DrawingGroupContainer aDggContainer, Stream stream, ref int shapePropertiesIndex, int currentLength)
		{
			if (shapePropertiesIndex + 8224 - currentLength < 50)
			{
				stream.Write(aDggContainer.ShapePropertyData, shapePropertiesIndex, 8224 - currentLength);
				shapePropertiesIndex += 8224 - currentLength;
			}
			else
			{
				stream.Write(aDggContainer.ShapePropertyData, shapePropertiesIndex, 50 - shapePropertiesIndex);
				shapePropertiesIndex = 50;
			}
		}
	}
}
