using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class PictureDescriptor
	{
		internal const string BMP_MIME = "image/bmp";

		internal const string JPEG_MIME = "image/jpeg";

		internal const string PNG_MIME = "image/png";

		private const float INCH_IN_TWIPS = 1440f;

		private const float DEFAULT_DPI = 96f;

		private const ushort SHAPERECORD_ID = 61450;

		private const ushort OPTIONSRECORD_ID = 61451;

		private const ushort ANCHORRECORD_ID = 61456;

		private const ushort CONTAINERRECORD_ID = 61444;

		private const ushort BSERECORD_ID = 61447;

		private const string FILENAME = "..\\bl.jpg\0";

		internal const ushort BASE_SHAPE_ID = 1024;

		internal const ushort IDPROP_ID = 16644;

		private const ushort FILENAMEPROP_ID = 49413;

		private const ushort BLIPFLAGSPROP_ID = 262;

		private const ushort NOLINEPROP_ID = 511;

		private const int ASPECT_ID = 127;

		private const int ASPECT_NOT_LOCKED = 8388608;

		private const int RELATIVE_ID = 831;

		private const int IS_NOT_RELATIVE = 1048576;

		internal const int SIZE = 68;

		internal const int TIFF = 98;

		internal const int BMP = 99;

		internal const int ESCHER = 100;

		internal static byte[] INVALIDIMAGEDATA;

		private static readonly BitField m_fFrameEmpty;

		private static readonly BitField m_fBitmap;

		private static readonly BitField m_fDrawHatch;

		private static readonly BitField m_fError;

		private static readonly BitField m_bpp;

		private int m_totalSize;

		private short m_picSize = 68;

		private short m_mappingMode;

		private short m_xExt;

		private short m_yExt;

		private short m_hMF;

		private byte[] m_metaData = new byte[14];

		private short m_dxaGoal;

		private short m_dyaGoal;

		private short m_mX;

		private short m_mY;

		private int m_dxaCropLeft;

		private int m_dyaCropTop;

		private int m_dxaCropRight;

		private int m_dyaCropBottom;

		private short m_info;

		private int m_brcTop;

		private int m_brcLeft;

		private int m_brcBottom;

		private int m_brcRight;

		private short m_xOrigin;

		private short m_yOrigin;

		private short m_cProps;

		private byte[] m_unknownData;

		private ArrayList m_recordList;

		private short m_originalHeight;

		private short m_originalWidth;

		private EscherBSERecord m_bse;

		private EscherSpRecord m_shape;

		private EscherOptRecord m_options;

		private byte[] m_imgData;

		private byte m_blipType = 1;

		private RPLFormat.Sizings m_sizing;

		private float m_xDensity;

		private float m_yDensity;

		private EscherClientAnchorRecord m_anchorRecord;

		static PictureDescriptor()
		{
			PictureDescriptor.INVALIDIMAGEDATA = null;
			PictureDescriptor.m_fFrameEmpty = new BitField(16);
			PictureDescriptor.m_fBitmap = new BitField(32);
			PictureDescriptor.m_fDrawHatch = new BitField(64);
			PictureDescriptor.m_fError = new BitField(64);
			PictureDescriptor.m_bpp = new BitField(65280);
			ResourceManager resourceManager = new ResourceManager("AspNetCore.ReportingServices.Rendering.WordRenderer.Images", Assembly.GetExecutingAssembly());
			Bitmap bitmap = resourceManager.GetObject("InvalidImage") as Bitmap;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, bitmap.RawFormat);
				PictureDescriptor.INVALIDIMAGEDATA = memoryStream.ToArray();
			}
		}

		internal PictureDescriptor(byte[] imgData, byte[] hash, int aWidth, int aHeight, RPLFormat.Sizings sizing, int imgIndex)
		{
			this.m_mX = 1000;
			this.m_mY = 1000;
			this.m_dxaGoal = (short)aWidth;
			this.m_dyaGoal = (short)aHeight;
			this.m_originalWidth = this.m_dxaGoal;
			this.m_originalHeight = this.m_dyaGoal;
			this.m_mappingMode = 100;
			this.m_brcTop = 1073741824;
			this.m_brcLeft = 1073741824;
			this.m_brcBottom = 1073741824;
			this.m_brcRight = 1073741824;
			this.m_unknownData = new byte[0];
			this.m_sizing = sizing;
			this.m_imgData = imgData;
			if (this.m_imgData == null)
			{
				this.m_imgData = PictureDescriptor.INVALIDIMAGEDATA;
			}
			this.ParseImageData();
			this.CreateDefaultEscherRecords(hash, imgIndex);
			if (sizing == RPLFormat.Sizings.Clip)
			{
				this.m_dxaCropLeft = 0;
				this.m_dyaCropTop = 0;
				this.m_dxaCropRight = this.m_dxaGoal - aWidth;
				this.m_dyaCropBottom = this.m_dyaGoal - aHeight;
			}
		}

		internal void Serialize(Stream data)
		{
			this.m_totalSize = 68 + this.m_unknownData.Length;
			for (int i = 0; i < this.m_recordList.Count; i++)
			{
				EscherRecord escherRecord = (EscherRecord)this.m_recordList[i];
				this.m_totalSize += escherRecord.RecordSize;
			}
			BinaryWriter binaryWriter = new BinaryWriter(data);
			binaryWriter.Write(this.m_totalSize);
			binaryWriter.Write(this.m_picSize);
			binaryWriter.Write(this.m_mappingMode);
			binaryWriter.Write(this.m_xExt);
			binaryWriter.Write(this.m_yExt);
			binaryWriter.Write(this.m_hMF);
			binaryWriter.Write(this.m_metaData);
			binaryWriter.Write(this.m_dxaGoal);
			binaryWriter.Write(this.m_dyaGoal);
			binaryWriter.Write(this.m_mX);
			binaryWriter.Write(this.m_mY);
			binaryWriter.Write((short)this.m_dxaCropLeft);
			binaryWriter.Write((short)this.m_dyaCropTop);
			binaryWriter.Write((short)this.m_dxaCropRight);
			binaryWriter.Write((short)this.m_dyaCropBottom);
			binaryWriter.Write(this.m_info);
			binaryWriter.Write(this.m_brcTop);
			binaryWriter.Write(this.m_brcLeft);
			binaryWriter.Write(this.m_brcBottom);
			binaryWriter.Write(this.m_brcRight);
			binaryWriter.Write(this.m_xOrigin);
			binaryWriter.Write(this.m_yOrigin);
			binaryWriter.Write(this.m_cProps);
			if (this.m_unknownData.Length > 0)
			{
				binaryWriter.Write(this.m_unknownData);
			}
			for (int j = 0; j < this.m_recordList.Count; j++)
			{
				EscherRecord escherRecord2 = (EscherRecord)this.m_recordList[j];
				escherRecord2.Serialize(binaryWriter);
			}
			binaryWriter.Flush();
		}

		private void CreateDefaultEscherRecords(byte[] hash, int imgIndex)
		{
			this.m_recordList = new ArrayList(2);
			EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
			escherContainerRecord.SetRecordId(61444);
			escherContainerRecord.setOptions(15);
			this.m_shape = new EscherSpRecord();
			this.m_shape.ShapeId = 1025;
			this.m_shape.Flags = 2560;
			this.m_shape.setOptions(1202);
			this.m_shape.SetRecordId(61450);
			escherContainerRecord.addChildRecord(this.m_shape);
			this.m_options = new EscherOptRecord();
			this.m_options.setOptions(67);
			this.m_options.SetRecordId(61451);
			this.m_options.addEscherProperty(new EscherSimpleProperty(16644, 1));
			try
			{
				EscherComplexProperty prop = new EscherComplexProperty(49413, Encoding.GetEncoding("utf-16").GetBytes("..\\bl.jpg\0"));
				this.m_options.addEscherProperty(prop);
			}
			catch (IOException innerException)
			{
				throw new ReportRenderingException(innerException);
			}
			this.m_options.addEscherProperty(new EscherSimpleProperty(262, 2));
			this.m_options.addEscherProperty(new EscherBoolProperty(511, 524288));
			escherContainerRecord.addChildRecord(this.m_options);
			this.m_anchorRecord = new EscherClientAnchorRecord();
			this.m_anchorRecord.RemainingData = new byte[4]
			{
				0,
				0,
				0,
				8
			};
			this.m_anchorRecord.ShortRecord = true;
			this.m_anchorRecord.setOptions(0);
			this.m_anchorRecord.SetRecordId(61456);
			escherContainerRecord.addChildRecord(this.m_anchorRecord);
			this.m_recordList.Add(escherContainerRecord);
			this.m_bse = new EscherBSERecord();
			this.m_bse.Unused2 = 9;
			this.m_bse.Unused3 = 1;
			this.m_bse.Tag = 255;
			this.m_bse.Ref = 1;
			this.m_bse.SetRecordId(61447);
			this.m_bse.SubRecord = new EscherBSESubRecord();
			this.InitImage(hash, imgIndex);
			this.m_recordList.Add(this.m_bse);
		}

		private void ParseImageData()
		{
			if (this.m_imgData != null)
			{
				bool flag = false;
				try
				{
					using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(this.m_imgData)))
					{
						this.m_dxaGoal = (short)((float)image.Width / image.HorizontalResolution * 1440.0);
						this.m_dyaGoal = (short)((float)image.Height / image.VerticalResolution * 1440.0);
						this.m_xDensity = image.HorizontalResolution;
						this.m_yDensity = image.VerticalResolution;
						if (image.RawFormat == ImageFormat.Jpeg)
						{
							this.m_blipType = 5;
						}
						else if (image.RawFormat == ImageFormat.Png)
						{
							this.m_blipType = 6;
						}
						else
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								image.Save(memoryStream, ImageFormat.Png);
								this.m_imgData = memoryStream.ToArray();
							}
							this.m_blipType = 6;
						}
					}
				}
				catch (ArgumentException)
				{
					flag = true;
				}
				catch (ExternalException)
				{
					flag = true;
				}
				if (this.m_imgData != PictureDescriptor.INVALIDIMAGEDATA && flag)
				{
					this.m_imgData = PictureDescriptor.INVALIDIMAGEDATA;
					this.ParseImageData();
				}
			}
		}

		private void InitImage(byte[] aHash, int imgIndex)
		{
			this.InitSizing();
			this.m_bse.BlipTypeMacOS = this.m_blipType;
			this.m_bse.BlipTypeWin32 = this.m_blipType;
			this.m_bse.setOptions((ushort)(this.m_blipType << 4 | 2));
			this.m_bse.Uid = aHash;
			this.m_bse.Size = this.m_imgData.Length + 25;
			EscherBSESubRecord subRecord = this.m_bse.SubRecord;
			subRecord.Hash = aHash;
			ushort num = 0;
			ushort num2 = 0;
			byte blipType = this.m_blipType;
			if (blipType == 5)
			{
				num = 61469;
				num2 = 18080;
			}
			else
			{
				num = 61470;
				num2 = 28160;
			}
			subRecord.Image = this.m_imgData;
			subRecord.SetRecordId(num);
			subRecord.setOptions(num2);
			if (this.m_xDensity == 0.0)
			{
				this.m_xDensity = 96f;
			}
			if (this.m_yDensity == 0.0)
			{
				this.m_yDensity = 96f;
			}
			this.InitIndex(imgIndex);
		}

		private void InitIndex(int index)
		{
			int num = 1 + index;
			int shapeId = 1024 + num;
			EscherSpRecord shape = this.m_shape;
			shape.ShapeId = shapeId;
			EscherOptRecord options = this.m_options;
			EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)options.getEscherPropertyByID(16644);
			escherSimpleProperty.PropertyValue = num;
		}

		private void InitSizing()
		{
			switch (this.m_sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				float num = (float)this.m_originalWidth / (float)this.m_dxaGoal;
				float num2 = (float)this.m_originalHeight / (float)this.m_dyaGoal;
				if (num2 < num)
				{
					num = num2;
				}
				this.m_dyaGoal = (short)((float)this.m_dyaGoal * num);
				this.m_dxaGoal = (short)((float)this.m_dxaGoal * num);
				break;
			}
			case RPLFormat.Sizings.Fit:
				this.m_dxaGoal = this.m_originalWidth;
				this.m_dyaGoal = this.m_originalHeight;
				break;
			case RPLFormat.Sizings.Clip:
				this.m_dxaCropLeft = 0;
				this.m_dyaCropTop = 0;
				this.m_dxaCropRight = this.m_dxaGoal - this.m_originalWidth;
				this.m_dyaCropBottom = this.m_dyaGoal - this.m_originalWidth;
				break;
			}
		}
	}
}
