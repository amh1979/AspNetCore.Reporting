<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name="serializer_invalid_flag_error" xml:space="preserve">
    <value>Serializer - Unsupported serialization content flag.</value>
  </data>
  <data name="locate_cprovider_failed" xml:space="preserve">
    <value>Cannot locate calculated value with name "{0}"</value>
  </data>
  <data name="margin_error" xml:space="preserve">
    <value>The start margin must be less than the end margin.</value>
  </data>
  <data name="out_of_range" xml:space="preserve">
    <value>The argument is out of range ({0},{1}).</value>
  </data>
  <data name="playback_data_empty_error" xml:space="preserve">
    <value>There is no data for playback. The history is empty.</value>
  </data>
  <data name="svg_invalid_gradient_error" xml:space="preserve">
    <value>SVG Graphics object - Gradient type is not defined.</value>
  </data>
  <data name="imagemap_invalid_circle_error" xml:space="preserve">
    <value>Invalid number of coordinates for the circle map area shape. Three coordinates must be provided: the center of the circle (x,y), followed by the radius.</value>
  </data>
  <data name="serializer_hash_property_error" xml:space="preserve">
    <value>Binary serialization error. Properties "{0}" and "{1}" have the same hash code.</value>
  </data>
  <data name="serializer_invalid_constructor_error" xml:space="preserve">
    <value>Can't deserialize property. Can't find default public constructor for type "{0}".</value>
  </data>
  <data name="imagemap_invalid_polygon_error" xml:space="preserve">
    <value>Invalid number of coordinates for the polygon map area shape. "x1,y1,x2,y2...xn,yn" – Each x,y pair contains the coordinates of one vertex of the polygon.</value>
  </data>
  <data name="property_negative" xml:space="preserve">
    <value>The {0} property value cannot be less than zero.</value>
  </data>
  <data name="hot_region_error_initialize" xml:space="preserve">
    <value>The element "{0}" is not initialized.</value>
  </data>
  <data name="invalid_value_error" xml:space="preserve">
    <value>Invalid property value.</value>
  </data>
  <data name="invalid_image_missing_format_error" xml:space="preserve">
    <value>Image URL do not contain the #SEQ formatter.</value>
  </data>
  <data name="imagemap_invalid_object_error" xml:space="preserve">
    <value>Object to be inserted or added is not a MapArea object.</value>
  </data>
  <data name="serializer_missing_serivice_error" xml:space="preserve">
    <value>The gauge serializer does not provide service of type: {0}</value>
  </data>
  <data name="max_min_error" xml:space="preserve">
    <value>The maximum of the scale must be greater than the minimum.</value>
  </data>
  <data name="must_in_range" xml:space="preserve">
    <value>The argument must be in range ({0},{1}).</value>
  </data>
  <data name="bad_data_src_fields" xml:space="preserve">
    <value>The data source does not support the IEnumerable interface OR some of the provided field names are empty.</value>
  </data>
  <data name="out_of_range_min_close" xml:space="preserve">
    <value>The argument cannot be less than {0}.</value>
  </data>
  <data name="digits_decimals_error" xml:space="preserve">
    <value>The digits must be greater than the decimals.</value>
  </data>
  <data name="duplicate_object_failed" xml:space="preserve">
    <value>Invalid attempt to insert an object that already exists in collection "{0}"</value>
  </data>
  <data name="ui_type_editor_enum_error" xml:space="preserve">
    <value>UI type editor may be set for the enumerations only</value>
  </data>
  <data name="svg_invalid_size_error" xml:space="preserve">
    <value>SVG Graphics Object - Invalid SVG Picture Size.</value>
  </data>
  <data name="databind_state_error" xml:space="preserve">
    <value>Cannot perform data binding when input value state is in playback, data binding, or suspended mode.</value>
  </data>
  <data name="element_not_found" xml:space="preserve">
    <value>{0} with the name "{1}" cannot be found in the {2} collection.</value>
  </data>
  <data name="svg_invalid_output_error" xml:space="preserve">
    <value>SVG Graphics Object - SVG Output Writer is null</value>
  </data>
  <data name="negative_value_error" xml:space="preserve">
    <value>Value cannot be negative.</value>
  </data>
  <data name="hot_region_error_support" xml:space="preserve">
    <value>The element "{0}" does not support its own coordinate system.</value>
  </data>
  <data name="duration_negative" xml:space="preserve">
    <value>The duration value cannot be less than zero.</value>
  </data>
  <data name="map_duration_argument_error" xml:space="preserve">
    <value>MapToDurationType: invalid argument.</value>
  </data>
  <data name="min_log_error" xml:space="preserve">
    <value>The Minimum property cannot be less OR equal than 0 on logarithmic scales.</value>
  </data>
  <data name="decimals_negative" xml:space="preserve">
    <value>The decimals property value cannot be less than zero.</value>
  </data>
  <data name="invalid_image_dual_format_error" xml:space="preserve">
    <value>Image URL can not contain both #UID and #SEQ formatters.</value>
  </data>
  <data name="empty_name_failed" xml:space="preserve">
    <value>The name of this {0} element cannot be empty.</value>
  </data>
  <data name="image_loader_invalid_location_error" xml:space="preserve">
    <value>Can't load image from this location: {0}</value>
  </data>
  <data name="image_loader_missing_serivice_error" xml:space="preserve">
    <value> Image loader do not provide service of type: {0}</value>
  </data>
  <data name="period_negative" xml:space="preserve">
    <value>The period value cannot be less than zero.</value>
  </data>
  <data name="invalid_indexer_error" xml:space="preserve">
    <value>Invalid indexer argument.</value>
  </data>
  <data name="invalid_parent" xml:space="preserve">
    <value>Cannot locate "{0}".</value>
  </data>
  <data name="duplicate_name_failed" xml:space="preserve">
    <value>This element cannot duplicate its name with another element in the same collection.</value>
  </data>
  <data name="serializer_invalid_property_name_error" xml:space="preserve">
    <value>Invalid format of the serializable content string. Property name not set.</value>
  </data>
  <data name="serializer_invalid_object_type_error" xml:space="preserve">
    <value>Serializer do not support objects of type "{0}".</value>
  </data>
  <data name="invalid_object_type" xml:space="preserve">
    <value>The collection {0} cannot accept this object. {1} object type required.</value>
  </data>
  <data name="circular_reference" xml:space="preserve">
    <value>Circular reference detected.</value>
  </data>
  <data name="invalid_keyword_format_error" xml:space="preserve">
    <value>Invalid keyword format. "{0}"</value>
  </data>
  <data name="ui_missing_context_error" xml:space="preserve">
    <value>Context instance is not a Gauge object.</value>
  </data>
  <data name="history_cannot_null" xml:space="preserve">
    <value>The sourceHistory parameter cannot be null</value>
  </data>
  <data name="serializer_unknown_property_error" xml:space="preserve">
    <value>Can't deserialize property. Unknown property name "{0}" in object "{1}"</value>
  </data>
  <data name="gdi_noninitialized" xml:space="preserve">
    <value>Internal error. GDI engine is not initialized.</value>
  </data>
  <data name="calculated_value_provider_error" xml:space="preserve">
    <value>CalculatedValue.BaseValueName: Cannot locate value provider with name "{0} "</value>
  </data>
  <data name="svg_invalid_writer_error" xml:space="preserve">
    <value>SVG Graphics Object - Invalid Text Writer.</value>
  </data>
  <data name="column_not_found" xml:space="preserve">
    <value>A column with the name "{0}" was not found.</value>
  </data>
  <data name="imagemap_invalid_rectangle_error" xml:space="preserve">
    <value>Invalid number of coordinates for the rectangle map area shape. Four coordinates must be provided: x,y coordinates of the upper-left corner followed by x,y coordinates for the bottom-right corner.</value>
  </data>
  <data name="interval_offset_negative" xml:space="preserve">
    <value>The interval offset value cannot be less than zero.</value>
  </data>
  <data name="invalid_image_format_error" xml:space="preserve">
    <value>Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.</value>
  </data>
  <data name="period_timespan_argument_error" xml:space="preserve">
    <value>PeriodToTimeSpan : argument PeriodType cannot be NotSet.</value>
  </data>
  <data name="web_pixel_size_error" xml:space="preserve">
    <value>The height must be set in pixels.</value>
  </data>
  <data name="serializer_id_loading_error" xml:space="preserve">
    <value>Can't find property with specified ID while loading data.</value>
  </data>
  <data name="list_unique_name_error" xml:space="preserve">
    <value>{0} cannot contain duplicate elements by name.</value>
  </data>
  <data name="interval_negative" xml:space="preserve">
    <value>The interval value cannot be less than zero.</value>
  </data>
  <data name="parent_not_renderable" xml:space="preserve">
    <value>The "{0}" does not support its own coordinate system.</value>
  </data>
  <data name="invalid_service_container_error" xml:space="preserve">
    <value>Valid Service Container object must be provided</value>
  </data>
  <data name="serializer_invalid_reader_error" xml:space="preserve">
    <value>Invalid type of reader object. Can be Stream, TextReader, XmlReader or String (file name)</value>
  </data>
  <data name="locate_iprovider_failed" xml:space="preserve">
    <value>Cannot locate input value with name "{0}"</value>
  </data>
  <data name="ui_type_editor_enum_int32_error" xml:space="preserve">
    <value>UI type editor may be set for the enumerations with Int32 underlying type only.</value>
  </data>
  <data name="invalid_image_url_tag_error" xml:space="preserve">
    <value>Image URL can not be empty when rendering chart as image tag.</value>
  </data>
  <data name="serializer_invalid_binary_error" xml:space="preserve">
    <value>Deserialization error. Invalid binary format.</value>
  </data>
  <data name="out_of_range_min_open" xml:space="preserve">
    <value>The argument must be greater than {0}.</value>
  </data>
  <data name="generate_name_failed" xml:space="preserve">
    <value>Internal exception : Generating unique name failed.</value>
  </data>
  <data name="xaml_brush_not_supported_error" xml:space="preserve">
    <value>The brush {0} is not supported.</value>
  </data>
  <data name="image_loader_invalid_url_error" xml:space="preserve">
    <value>Can't load image from this URL:  {0}</value>
  </data>
  <data name="serializer_invalid_class_name_error" xml:space="preserve">
    <value>Invalid format of the serializable content string. Class name not set.</value>
  </data>
  <data name="decimals_digits_error" xml:space="preserve">
    <value>The decimals must be less than the digits.</value>
  </data>
  <data name="value_dbnull_error" xml:space="preserve">
    <value>Cannot get value because it is DBNull.</value>
  </data>
  <data name="playback_count_error" xml:space="preserve">
    <value>The length of the history is less than {0} records.</value>
  </data>
  <data name="digits_negative" xml:space="preserve">
    <value>The digits property value cannot be less than zero.</value>
  </data>
  <data name="invalid_marker_type" xml:space="preserve">
    <value>Internal error. Invalid marker type specified.</value>
  </data>
  <data name="list_out_of_range_error" xml:space="preserve">
    <value>List index is out of range</value>
  </data>
  <data name="serializer_unsupported_attribute_error" xml:space="preserve">
    <value>Binary deserialization does not support 'IgnoreUnknownAttributes' property.</value>
  </data>
  <data name="serializer_invalid_writer_error" xml:space="preserve">
    <value>Invalid type of writer object. Can be Stream, TextWriter, XmlWriter or String (file name)</value>
  </data>
  <data name="size_format_error" xml:space="preserve">
    <value>Incorrect string format. The Width, Height of the size must be specified e.g. "100,100"</value>
  </data>
  <data name="svg_maximum_gradients_error" xml:space="preserve">
    <value>SVG Graphics object - The maximum number for gradients is:{0}</value>
  </data>
  <data name="serializer_invalid_content_format_error" xml:space="preserve">
    <value>Invalid format of the serializable content string.</value>
  </data>
  <data name="xaml_geometry_not_supported_error" xml:space="preserve">
    <value>The stream geometry command {0} is not supported.</value>
  </data>
  <data name="invalid_indexer" xml:space="preserve">
    <value>Invalid indexer argument type. Valid types are int OR string</value>
  </data>
  <data name="svg_wrap_strings_error" xml:space="preserve">
    <value>SVG Rendering - Wrap String Error.</value>
  </data>
  <data name="location_format_error" xml:space="preserve">
    <value> Incorrect string format. The X, Y of the location must be specified e.g. "0,0"</value>
  </data>
  <data name="bad_data_src" xml:space="preserve">
    <value>The data source does not support the IEnumerable interface.</value>
  </data>
  <data name="min_max_error" xml:space="preserve">
    <value>The minimum of the scale must be less than the maximum.</value>
  </data>
  <data name="xaml_invalid_canvas_size_error" xml:space="preserve">
    <value>The root canvas must have its Width and Height defined.</value>
  </data>
  <data name="web_pixel_size_width_error" xml:space="preserve">
    <value>The width must be set in pixels.</value>
  </data>
  <data name="invalid_placement_type" xml:space="preserve">
    <value>Internal error. Invalid placement type specified.</value>
  </data>
  <data name="map_period_type_argument_error" xml:space="preserve">
    <value>MapToPeriodType: argument DurationType cannot be Infinity or Count.</value>
  </data>
  <data name="xaml_shape_not_supported_error" xml:space="preserve">
    <value>The shape {0} is not supported.</value>
  </data>
</root>