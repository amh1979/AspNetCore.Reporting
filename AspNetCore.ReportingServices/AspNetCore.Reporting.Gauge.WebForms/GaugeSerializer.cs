using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[DefaultProperty("Format")]
	[SRDescription("DescriptionAttributeGaugeSerializer_GaugeSerializer")]
	internal class GaugeSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private GaugeCore gaugeObject;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContent content = SerializationContent.All;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeSerializer_Content")]
		[DefaultValue(typeof(SerializationContent), "All")]
		public SerializationContent Content
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

		[SRDescription("DescriptionAttributeGaugeSerializer_Format")]
		[SRCategory("CategoryMisc")]
		[DefaultValue(typeof(SerializationFormat), "Xml")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeSerializer_ResetWhenLoading")]
		[DefaultValue(true)]
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

		[SRDescription("DescriptionAttributeGaugeSerializer_IgnoreUnknownXmlAttributes")]
		[SRCategory("CategoryMisc")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeSerializer_TemplateMode")]
		[DefaultValue(false)]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeSerializer_SerializableContent")]
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

		[SRDescription("DescriptionAttributeGaugeSerializer_NonSerializableContent")]
		[SRCategory("CategoryMisc")]
		[DefaultValue("")]
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

		private GaugeSerializer()
		{
		}

		public GaugeSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container", Utils.SRGetStr("ExceptionInvalidServiceContainer"));
			}
			this.serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(GaugeSerializer))
			{
				return this;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerMissingSerivice", serviceType.ToString()), "serviceType");
		}

		public void Reset()
		{
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.serializer.ResetObjectProperties(this.GetGaugeObject());
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
		}

		public void Save(string fileName)
		{
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.GetGaugeObject().ResetAutoValues();
			this.serializer.Serialize(this.GetGaugeObject(), fileName);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
		}

		public void Save(Stream stream)
		{
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.GetGaugeObject().ResetAutoValues();
			this.serializer.Serialize(this.GetGaugeObject(), stream);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
		}

		public void Save(XmlWriter writer)
		{
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.GetGaugeObject().ResetAutoValues();
			this.serializer.Serialize(this.GetGaugeObject(), writer);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
		}

		public void Save(TextWriter writer)
		{
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.GetGaugeObject().ResetAutoValues();
			this.serializer.Serialize(this.GetGaugeObject(), writer);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
		}

		public void Load(string fileName)
		{
			this.GetGaugeObject().BeginInit();
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetGaugeObject(), fileName);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
			this.GetGaugeObject().EndInit();
		}

		public void Load(Stream stream)
		{
			this.GetGaugeObject().BeginInit();
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetGaugeObject(), stream);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
			if (this.GetGaugeObject().Values.GetByName("DataValue") == null)
			{
				this.GetGaugeObject().Values.Add("DataValue");
			}
			this.GetGaugeObject().EndInit();
		}

		public void Load(XmlReader reader)
		{
			this.GetGaugeObject().BeginInit();
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetGaugeObject(), reader);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
			this.GetGaugeObject().EndInit();
		}

		public void Load(TextReader reader)
		{
			this.GetGaugeObject().BeginInit();
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetGaugeObject(), reader);
			if (this.GetGaugeObject() != null)
			{
				this.GetGaugeObject().Serializing = false;
			}
			this.GetGaugeObject().EndInit();
		}

		protected void SetSerializableContentFromFlags()
		{
			this.SerializableContent = "";
			this.NonSerializableContent = "";
			Array values = Enum.GetValues(typeof(SerializationContent));
			foreach (object item in values)
			{
				if (item is SerializationContent)
				{
					SerializationContent serializationContent = (SerializationContent)item;
					if ((this.Content & serializationContent) == serializationContent && serializationContent != SerializationContent.All && this.Content != SerializationContent.All)
					{
						if (this.NonSerializableContent.Length != 0)
						{
							this.NonSerializableContent += ", ";
						}
						this.NonSerializableContent += this.GetFlagContentString(serializationContent, false);
						this.NonSerializableContent = this.NonSerializableContent.TrimStart(',');
						if (this.SerializableContent.Length != 0)
						{
							this.SerializableContent += ", ";
						}
						this.SerializableContent += this.GetFlagContentString(serializationContent, true);
						this.SerializableContent = this.SerializableContent.TrimStart(',');
					}
				}
			}
		}

		internal string GetFlagContentString(SerializationContent flag, bool serializable)
		{
			switch (flag)
			{
			case SerializationContent.All:
				return "";
			case SerializationContent.Appearance:
				if (serializable)
				{
					return "GaugeCore.BuildNumber, *.Name, *.LedDimColor, *.NeedleStyle, *.Cap*, CircularPointer.Type, LinearPointer.Type, *.Shape, *.FrameShape, *.Fill*, *.BackFrame*, *.Frame*, *.Back*, *.Border*, *.SeparatorColor, *.DecimalColor, *.DigitColor, *.TextColor, *.Color, *.Shadow*, *.AntiAliasing, *.GlassEffect, *.FontColor";
				}
				return "";
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidFlag"));
			}
		}

		internal GaugeCore GetGaugeObject()
		{
			if (this.gaugeObject == null)
			{
				this.gaugeObject = (GaugeCore)this.serviceContainer.GetService(typeof(GaugeCore));
			}
			return this.gaugeObject;
		}
	}
}
