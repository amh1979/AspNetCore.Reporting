using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	[DefaultProperty("Format")]
	[Description("Map serializer class.")]
	internal class MapSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private MapCore mapObject;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContent content = SerializationContent.All;

		[SRDescription("DescriptionAttributeMapSerializer_Content")]
		[SRCategory("CategoryAttribute_Misc")]
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
				this.SetSerializableConentFromFlags();
			}
		}

		[DefaultValue(typeof(SerializationFormat), "Xml")]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeMapSerializer_Format")]
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

		[SRDescription("DescriptionAttributeMapSerializer_ResetWhenLoading")]
		[SRCategory("CategoryAttribute_Misc")]
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

		[DefaultValue(false)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeMapSerializer_IgnoreUnknownXmlAttributes")]
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

		[SRDescription("DescriptionAttributeMapSerializer_TemplateMode")]
		[SRCategory("CategoryAttribute_Misc")]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeMapSerializer_SerializableContent")]
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

		[SRDescription("DescriptionAttributeMapSerializer_NonSerializableContent")]
		[SRCategory("CategoryAttribute_Misc")]
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

		private MapSerializer()
		{
		}

		public MapSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("Valid Service Container object must be provided");
			}
			this.serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(MapSerializer))
			{
				return this;
			}
			throw new ArgumentException("The map serializer does not provide service of type: " + serviceType.ToString());
		}

		public void Reset()
		{
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.ResetObjectProperties(this.GetMapObject());
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
		}

		public void Save(string fileName)
		{
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Serialize(this.GetMapObject(), fileName);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
		}

		public void Save(Stream stream)
		{
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Serialize(this.GetMapObject(), stream);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
		}

		public void Save(XmlDocument document)
		{
			if (this.serializer is XmlFormatSerializer)
			{
				if (this.GetMapObject() != null)
				{
					this.GetMapObject().Serializing = true;
				}
				(this.serializer as XmlFormatSerializer).Serialize(this.GetMapObject(), document);
				if (this.GetMapObject() != null)
				{
					this.GetMapObject().Serializing = false;
				}
			}
		}

		public void Save(XmlWriter writer)
		{
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Serialize(this.GetMapObject(), writer);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
		}

		public void Save(TextWriter writer)
		{
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Serialize(this.GetMapObject(), writer);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
		}

		public void Load(string fileName)
		{
			this.GetMapObject().BeginInit();
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetMapObject(), fileName);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
			this.GetMapObject().EndInit();
		}

		public void Load(Stream stream)
		{
			this.GetMapObject().BeginInit();
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetMapObject(), stream);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
			this.GetMapObject().EndInit();
		}

		public void Load(XmlReader reader)
		{
			this.GetMapObject().BeginInit();
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetMapObject(), reader);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
			this.GetMapObject().EndInit();
		}

		public void Load(TextReader reader)
		{
			this.GetMapObject().BeginInit();
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = true;
			}
			this.serializer.Deserialize(this.GetMapObject(), reader);
			if (this.GetMapObject() != null)
			{
				this.GetMapObject().Serializing = false;
			}
			this.GetMapObject().EndInit();
		}

		protected void SetSerializableConentFromFlags()
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
						if (!string.IsNullOrEmpty(this.NonSerializableContent))
						{
							this.NonSerializableContent += ", ";
						}
						this.NonSerializableContent += this.GetFlagContentString(serializationContent, false);
						this.NonSerializableContent = this.NonSerializableContent.TrimStart(',');
						if (!string.IsNullOrEmpty(this.SerializableContent))
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
					return "MapCore.BuildNumber, *.Name, *.LedDimColor, *.NeedleStyle, *.Cap*, CircularPointer.Type, LinearPointer.Type, *.Shape, *.Fill*, *.Frame*, *.Back*, *.Border*, *.SeparatorColor, *.DecimalColor, *.DigitColor, *.TextColor, *.Color, *.Shadow*, *.AntiAliasing, *.GlassEffect, *.FontColor";
				}
				return "";
			default:
				throw new InvalidOperationException("Serializer - Unsupported serialization content flag.");
			}
		}

		internal MapCore GetMapObject()
		{
			if (this.mapObject == null)
			{
				this.mapObject = (MapCore)this.serviceContainer.GetService(typeof(MapCore));
			}
			return this.mapObject;
		}
	}
}
