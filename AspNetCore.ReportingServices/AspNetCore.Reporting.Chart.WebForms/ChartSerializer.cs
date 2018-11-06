using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeChartSerializer_ChartSerializer")]
	[DefaultProperty("Format")]
	internal class ChartSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private Chart chart;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContents content = SerializationContents.Default;

		[SRDescription("DescriptionAttributeChartSerializer_Content")]
		[DefaultValue(typeof(SerializationContents), "Default")]
		[SRCategory("CategoryAttributeMisc")]
		public SerializationContents Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
				this.SetSerializableContentFromFlags();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(typeof(SerializationFormat), "Xml")]
		[SRDescription("DescriptionAttributeChartSerializer_Format")]
		public SerializationFormat Format
		{
			get
			{
				return this.format;
			}
			set
			{
				if (this.format != value)
				{
					this.format = value;
					SerializerBase serializerBase = null;
					serializerBase = ((this.format != SerializationFormat.Binary) ? ((SerializerBase)new XmlFormatSerializer()) : ((SerializerBase)new BinaryFormatSerializer()));
					serializerBase.IgnoreUnknownAttributes = this.serializer.IgnoreUnknownAttributes;
					serializerBase.NonSerializableContent = this.serializer.NonSerializableContent;
					serializerBase.ResetWhenLoading = this.serializer.ResetWhenLoading;
					serializerBase.SerializableContent = this.serializer.SerializableContent;
					this.serializer = serializerBase;
				}
			}
		}

		[SRDescription("DescriptionAttributeChartSerializer_ResetWhenLoading")]
		[DefaultValue(true)]
		[SRCategory("CategoryAttributeMisc")]
		public bool ResetWhenLoading
		{
			get
			{
				return this.serializer.ResetWhenLoading;
			}
			set
			{
				this.serializer.ResetWhenLoading = value;
			}
		}

		[SRDescription("DescriptionAttributeChartSerializer_IgnoreUnknownXmlAttributes")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		public bool IgnoreUnknownXmlAttributes
		{
			get
			{
				return this.serializer.IgnoreUnknownAttributes;
			}
			set
			{
				this.serializer.IgnoreUnknownAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartSerializer_TemplateMode")]
		public bool TemplateMode
		{
			get
			{
				return this.serializer.TemplateMode;
			}
			set
			{
				this.serializer.TemplateMode = value;
			}
		}

		[SRDescription("DescriptionAttributeChartSerializer_SerializableContent")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		public string SerializableContent
		{
			get
			{
				return this.serializer.SerializableContent;
			}
			set
			{
				this.serializer.SerializableContent = value;
			}
		}

		[SRDescription("DescriptionAttributeChartSerializer_NonSerializableContent")]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeMisc")]
		public string NonSerializableContent
		{
			get
			{
				return this.serializer.NonSerializableContent;
			}
			set
			{
				this.serializer.NonSerializableContent = value;
			}
		}

		private ChartSerializer()
		{
		}

		public ChartSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartSerializer))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartSerializerUnsupportedType(serviceType.ToString()));
		}

		public void Reset()
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Resetting;
			}
			this.serializer.ResetObjectProperties(this.GetChartObject());
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(string fileName)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			this.GetChartObject().ResetAutoValues();
			this.serializer.Serialize(this.GetChartObject(), fileName);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(Stream stream)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			this.GetChartObject().ResetAutoValues();
			this.serializer.Serialize(this.GetChartObject(), stream);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(XmlWriter writer)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			this.GetChartObject().ResetAutoValues();
			this.serializer.Serialize(this.GetChartObject(), writer);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(TextWriter writer)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			this.GetChartObject().ResetAutoValues();
			this.serializer.Serialize(this.GetChartObject(), writer);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(string fileName)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			this.serializer.Deserialize(this.GetChartObject(), fileName);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(Stream stream)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			this.serializer.Deserialize(this.GetChartObject(), stream);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(XmlReader reader)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			this.serializer.Deserialize(this.GetChartObject(), reader);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(TextReader reader)
		{
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = true;
				this.GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			this.serializer.Deserialize(this.GetChartObject(), reader);
			if (this.GetChartObject() != null)
			{
				this.GetChartObject().serializing = false;
				this.GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		protected void SetSerializableContentFromFlags()
		{
			this.SerializableContent = "";
			this.NonSerializableContent = "";
			Array values = Enum.GetValues(typeof(SerializationContents));
			foreach (object item in values)
			{
				if (item is SerializationContents)
				{
					SerializationContents serializationContents = (SerializationContents)item;
					if ((this.Content & serializationContents) == serializationContents && serializationContents != SerializationContents.All && this.Content != SerializationContents.All)
					{
						if (this.NonSerializableContent.Length != 0)
						{
							this.NonSerializableContent += ", ";
						}
						this.NonSerializableContent += this.GetFlagContentString(serializationContents, false);
						this.NonSerializableContent = this.NonSerializableContent.TrimStart(',');
						if (this.SerializableContent.Length != 0)
						{
							this.SerializableContent += ", ";
						}
						this.SerializableContent += this.GetFlagContentString(serializationContents, true);
						this.SerializableContent = this.SerializableContent.TrimStart(',');
					}
				}
			}
		}

		protected string GetFlagContentString(SerializationContents flag, bool serializable)
		{
			switch (flag)
			{
			case SerializationContents.All:
				return "";
			case SerializationContents.Default:
				return "";
			case SerializationContents.Data:
				if (serializable)
				{
					return "Chart.BuildNumber, Chart.Series, Series.Points, Series.Name, DataPoint.XValue, DataPoint.YValues,DataPoint.Label,DataPoint.AxisLabel,DataPoint.LabelFormat,DataPoint.Empty, Series.YValuesPerPoint, Series.XValueIndexed, Series.XValueType, Series.YValueType";
				}
				return "";
			case SerializationContents.Appearance:
				if (serializable)
				{
					return "Chart.BuildNumber, *.Name, *.Back*, *.Border*, *.Line*, *.Frame*, *.PageColor*, *.SkinStyle*, *.Palette, *.PaletteCustomColors, *.Font*, *.*Font, *.Color, *.Shadow*, *.MarkerColor, *.MarkerStyle, *.MarkerSize, *.MarkerBorderColor, *.MarkerImage, *.MarkerImageTransparentColor, *.LabelBackColor, *.LabelBorder*, *.Enable3D, *.RightAngleAxes, *.Clustered, *.Light, *.Perspective, *.XAngle, *.YAngle, *.PointDepth, *.PointGapDepth, *.WallWidth";
				}
				return "";
			default:
				throw new InvalidOperationException(SR.ExceptionChartSerializerContentFlagUnsupported);
			}
		}

		internal Chart GetChartObject()
		{
			if (this.chart == null)
			{
				this.chart = (Chart)this.serviceContainer.GetService(typeof(Chart));
			}
			return this.chart;
		}
	}
}
