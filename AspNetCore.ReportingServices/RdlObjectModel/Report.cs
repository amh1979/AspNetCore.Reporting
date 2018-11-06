using AspNetCore.ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class Report : ReportObject
	{
		internal class Definition : DefinitionStore<Report, Definition.Properties>
		{
			internal enum Properties
			{
				Description,
				DescriptionLocID,
				Author,
				AutoRefresh,
				DataSources,
				DataSets,
				ReportParameters,
				CustomProperties,
				Code,
				EmbeddedImages,
				Language,
				CodeModules,
				Classes,
				Variables,
				DeferVariableEvaluation,
				ConsumeContainerWhitespace,
				DataTransform,
				DataSchema,
				DataElementName,
				DataElementStyle,
				ReportSections,
				InitialPageName,
				ReportParametersLayout,
				DefaultFontFamily,
				MustUnderstand,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public const string DefaultFontFamilyDefault = "Arial";

		[XmlAttribute]
		[DefaultValue("")]
		public string MustUnderstand
		{
			get
			{
				return (string)base.PropertyStore.GetObject(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		[DefaultValue("")]
		public string Description
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily")]
		[DefaultValue("Arial")]
		public string DefaultFontFamily
		{
			get
			{
				return (string)base.PropertyStore.GetObject(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		[DefaultValue("")]
		public string Author
		{
			get
			{
				return (string)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression<int> AutoRefresh
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression InitialPageName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[XmlElement(typeof(RdlCollection<DataSource>))]
		public IList<DataSource> DataSources
		{
			get
			{
				return (IList<DataSource>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[XmlElement(typeof(RdlCollection<DataSet>))]
		public IList<DataSet> DataSets
		{
			get
			{
				return (IList<DataSet>)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[XmlIgnore]
		public virtual Body Body
		{
			get
			{
				return this.GetFirstSection("Body").Body;
			}
			set
			{
				this.GetFirstSection("Body").Body = value;
			}
		}

		[XmlElement(typeof(RdlCollection<ReportSection>))]
		public IList<ReportSection> ReportSections
		{
			get
			{
				return (IList<ReportSection>)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportParameter>))]
		public IList<ReportParameter> ReportParameters
		{
			get
			{
				return (IList<ReportParameter>)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[XmlElement(typeof(ReportParametersLayout))]
		public ReportParametersLayout ReportParametersLayout
		{
			get
			{
				return base.PropertyStore.GetObject<ReportParametersLayout>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue("")]
		public string Code
		{
			get
			{
				return (string)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[XmlIgnore]
		public virtual ReportSize Width
		{
			get
			{
				return this.GetFirstSection("Width").Width;
			}
			set
			{
				this.GetFirstSection("Width").Width = value;
			}
		}

		[XmlIgnore]
		public virtual Page Page
		{
			get
			{
				return this.GetFirstSection("Page").Page;
			}
			set
			{
				this.GetFirstSection("Page").Page = value;
			}
		}

		[XmlElement(typeof(RdlCollection<EmbeddedImage>))]
		public IList<EmbeddedImage> EmbeddedImages
		{
			get
			{
				return (IList<EmbeddedImage>)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Language
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[XmlArrayItem("CodeModule", typeof(string))]
		[XmlElement(typeof(RdlCollection<string>))]
		public IList<string> CodeModules
		{
			get
			{
				return (IList<string>)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Class>))]
		public IList<Class> Classes
		{
			get
			{
				return (IList<Class>)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Variable>))]
		public IList<Variable> Variables
		{
			get
			{
				return (IList<Variable>)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[DefaultValue(false)]
		public bool DeferVariableEvaluation
		{
			get
			{
				return base.PropertyStore.GetBoolean(14);
			}
			set
			{
				base.PropertyStore.SetBoolean(14, value);
			}
		}

		[DefaultValue(false)]
		public bool ConsumeContainerWhitespace
		{
			get
			{
				return base.PropertyStore.GetBoolean(15);
			}
			set
			{
				base.PropertyStore.SetBoolean(15, value);
			}
		}

		[DefaultValue("")]
		public string DataTransform
		{
			get
			{
				return (string)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[DefaultValue("")]
		public string DataSchema
		{
			get
			{
				return (string)base.PropertyStore.GetObject(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[DefaultValue(DataElementStyles.Attribute)]
		[ValidEnumValues("ReportDataElementOutputTypes")]
		public DataElementStyles DataElementStyle
		{
			get
			{
                return PropertyStore.GetObject<DataElementStyles>(19);
			}
			set
			{
				((EnumProperty)DefinitionStore<Report, Definition.Properties>.GetProperty(19)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(19, (int)value);
			}
		}

		public Report()
		{
		}

		internal Report(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.ReportSections = new RdlCollection<ReportSection>();
			ReportSection item = new ReportSection();
			this.ReportSections.Add(item);
			this.DataSources = new RdlCollection<DataSource>();
			this.DataSets = new RdlCollection<DataSet>();
			this.ReportParameters = new RdlCollection<ReportParameter>();
			this.ReportParametersLayout = new ReportParametersLayout();
			this.CustomProperties = new RdlCollection<CustomProperty>();
			this.EmbeddedImages = new RdlCollection<EmbeddedImage>();
			this.CodeModules = new RdlCollection<string>();
			this.Classes = new RdlCollection<Class>();
			this.Variables = new RdlCollection<Variable>();
			this.DataElementStyle = DataElementStyles.Attribute;
		}

		private ReportSection GetFirstSection(string propertyName)
		{
			IList<ReportSection> reportSections = this.ReportSections;
			if (reportSections != null && reportSections.Count != 0)
			{
				return reportSections[0];
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot access {0} when no sections are defined", propertyName));
		}

		public DataSet GetDataSetByName(string name)
		{
			foreach (DataSet dataSet in this.DataSets)
			{
				if (StringUtil.CompareClsCompliantIdentifiers(dataSet.Name, name) == 0)
				{
					return dataSet;
				}
			}
			return null;
		}

		public DataSource GetDataSourceByName(string name)
		{
			foreach (DataSource dataSource in this.DataSources)
			{
				if (StringUtil.CompareClsCompliantIdentifiers(dataSource.Name, name) == 0)
				{
					return dataSource;
				}
			}
			return null;
		}

		public EmbeddedImage GetEmbeddedImageByName(string name)
		{
			foreach (EmbeddedImage embeddedImage in this.EmbeddedImages)
			{
				if (StringUtil.CompareClsCompliantIdentifiers(embeddedImage.Name, name) == 0)
				{
					return embeddedImage;
				}
			}
			return null;
		}

		public ReportParameter GetReportParameterByName(string name)
		{
			foreach (ReportParameter reportParameter in this.ReportParameters)
			{
				if (StringUtil.CompareClsCompliantIdentifiers(reportParameter.Name, name) == 0)
				{
					return reportParameter;
				}
			}
			return null;
		}
	}
}
