using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal struct ReportExpression : IExpression, IXmlSerializable, IFormattable
	{
		private string m_value;

		private DataTypes m_dataType;

		private EvaluationMode m_evaluationMode;

		private static Regex m_nonConstantRegex = new Regex("^\\s*=", RegexOptions.Compiled);

		public string Value
		{
			get
			{
				return this.m_value ?? "";
			}
			set
			{
				this.m_value = value;
			}
		}

		public DataTypes DataType
		{
			get
			{
				return this.m_dataType;
			}
			set
			{
				this.m_dataType = value;
			}
		}

		object IExpression.Value
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = (string)value;
			}
		}

		public string Expression
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		public EvaluationMode EvaluationMode
		{
			get
			{
				return this.m_evaluationMode;
			}
			set
			{
				this.m_evaluationMode = value;
			}
		}

		public bool IsExpression
		{
			get
			{
				if (this.EvaluationMode == EvaluationMode.Auto)
				{
					return ReportExpression.IsExpressionString(this.m_value);
				}
				return false;
			}
		}

		public ReportExpression(string value)
		{
			this.m_value = value;
			this.m_dataType = DataTypes.String;
			this.m_evaluationMode = EvaluationMode.Auto;
		}

		public ReportExpression(string value, EvaluationMode evaluationMode)
		{
			this.m_value = value;
			this.m_dataType = DataTypes.String;
			this.m_evaluationMode = evaluationMode;
		}

		public override string ToString()
		{
			return this.Value;
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.Value;
		}

		public void GetDependencies(IList<ReportObject> dependencies, ReportObject parent)
		{
			ReportExpressionUtils.GetDependencies(dependencies, parent, this.Expression);
		}

		public static bool IsExpressionString(string value)
		{
			if (value != null)
			{
				return ReportExpression.m_nonConstantRegex.IsMatch(value);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (value is ReportExpression)
			{
				ReportExpression reportExpression = (ReportExpression)value;
				if (this.Value == reportExpression.Value && this.IsExpression == reportExpression.IsExpression)
				{
					return this.DataType == reportExpression.DataType;
				}
				return false;
			}
			if (value is string)
			{
				return this.Equals(new ReportExpression(((string)value) ?? ""));
			}
			if (value == null)
			{
				return this.Value == "";
			}
			return false;
		}

		public static bool operator ==(ReportExpression left, ReportExpression right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(ReportExpression left, string right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(string left, ReportExpression right)
		{
			return right.Equals(left);
		}

		public static bool operator !=(ReportExpression left, ReportExpression right)
		{
			return !left.Equals(right);
		}

		public static bool operator !=(ReportExpression left, string right)
		{
			return !left.Equals(right);
		}

		public static bool operator !=(string left, ReportExpression right)
		{
			return !right.Equals(left);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static explicit operator string(ReportExpression value)
		{
			return value.Value;
		}

		public static implicit operator ReportExpression(string value)
		{
			return new ReportExpression(value);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("DataType");
			if (attribute != null)
			{
				this.DataType = (DataTypes)ReportExpression.ParseEnum(typeof(DataTypes), attribute);
			}
			string attribute2 = reader.GetAttribute("EvaluationMode");
			if (attribute2 != null)
			{
				this.EvaluationMode = (EvaluationMode)ReportExpression.ParseEnum(typeof(EvaluationMode), attribute2);
			}
			this.m_value = reader.ReadString();
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.DataType != 0)
			{
				writer.WriteAttributeString("DataType", this.DataType.ToString());
			}
			if (this.EvaluationMode != 0)
			{
				writer.WriteAttributeString("EvaluationMode", this.EvaluationMode.ToString());
			}
			if (this.Value.Length > 0)
			{
				if (this.Value.Trim().Length == 0)
				{
					writer.WriteAttributeString("xml", "space", null, "preserve");
				}
				writer.WriteString(this.Value);
			}
		}

		internal static object ParseEnum(Type type, string value)
		{
			int num = Array.IndexOf(Enum.GetNames(type), value);
			if (num < 0)
			{
				throw new ArgumentException(SRErrors.InvalidValue(value));
			}
			return Enum.GetValues(type).GetValue(num);
		}
	}
	internal struct ReportExpression<T> : IExpression, IXmlSerializable, IFormattable, IShouldSerialize where T : struct
	{
		private T m_value;

		private string m_expression;

		private static MethodInfo m_parseMethod;

		private static int m_parseMethodArgs;

		public T Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
				this.m_expression = null;
			}
		}

		object IExpression.Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = (T)value;
			}
		}

		public string Expression
		{
			get
			{
				return this.m_expression;
			}
			set
			{
				this.m_expression = value;
				this.m_value = default(T);
			}
		}

		public bool IsExpression
		{
			get
			{
				return this.m_expression != null;
			}
		}

		public ReportExpression(T value)
		{
			this.m_value = value;
			this.m_expression = null;
		}

		public ReportExpression(string value)
		{
			this = new ReportExpression<T>(value, CultureInfo.CurrentCulture);
		}

		public ReportExpression(string value, IFormatProvider provider)
		{
			this.m_value = default(T);
			this.m_expression = null;
			if (!string.IsNullOrEmpty(value))
			{
				this.Init(value, provider);
			}
		}

		private void Init(string value, IFormatProvider provider)
		{
			if (ReportExpression.IsExpressionString(value))
			{
				this.Expression = value;
			}
			else if (typeof(T).IsSubclassOf(typeof(Enum)))
			{
				this.Value = (T)ReportExpression.ParseEnum(typeof(T), value);
			}
			else if (typeof(T) == typeof(ReportSize))
			{
				this.Value = (T)(object)ReportSize.Parse(value, provider);
			}
			else if (typeof(T) == typeof(ReportColor))
			{
				this.Value = (T)(object)ReportColor.Parse(value, provider);
			}
			else
			{
				try
				{
					if (typeof(T) == typeof(bool))
					{
						this.Value = (T)(object)XmlConvert.ToBoolean(value.ToLowerInvariant());
					}
					else
					{
						this.Value = (T)this.GetParseMethod().Invoke(null, (ReportExpression<T>.m_parseMethodArgs == 1) ? new object[1]
						{
							value
						} : new object[2]
						{
							value,
							provider
						});
					}
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException != null)
					{
						throw ex.InnerException;
					}
					throw ex;
				}
			}
		}

		public static ReportExpression<T> Parse(string value, IFormatProvider provider)
		{
			return new ReportExpression<T>(value, provider);
		}

		private MethodInfo GetParseMethod()
		{
			if (ReportExpression<T>.m_parseMethodArgs == 0)
			{
				ReportExpression<T>.m_parseMethod = typeof(T).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(IFormatProvider)
				}, null);
				ReportExpression<T>.m_parseMethodArgs = 2;
				if (ReportExpression<T>.m_parseMethod == null)
				{
					ReportExpression<T>.m_parseMethod = typeof(T).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1]
					{
						typeof(string)
					}, null);
					ReportExpression<T>.m_parseMethodArgs = 1;
				}
			}
			return ReportExpression<T>.m_parseMethod;
		}

		public override string ToString()
		{
			return this.ToString(null, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			if (this.IsExpression)
			{
				return this.m_expression;
			}
			if (typeof(T) == typeof(bool) && provider == CultureInfo.InvariantCulture)
			{
				if (!true.Equals((object)this.m_value))
				{
					return "false";
				}
				return "true";
			}
			if (typeof(IFormattable).IsAssignableFrom(typeof(T)))
			{
				return ((IFormattable)(object)this.m_value).ToString(format, provider);
			}
			return this.m_value.ToString();
		}

		public override bool Equals(object value)
		{
			if (value is ReportExpression<T>)
			{
				if (this.m_value.Equals(((ReportExpression<T>)value).Value))
				{
					return this.m_expression == ((ReportExpression<T>)value).Expression;
				}
				return false;
			}
			if (this.IsExpression)
			{
				if (value is string)
				{
					return this.m_expression == (string)value;
				}
				return false;
			}
			return this.m_value.Equals(value);
		}

		public override int GetHashCode()
		{
			int num = this.m_value.GetHashCode();
			if (this.m_expression != null)
			{
				num ^= this.m_expression.GetHashCode();
			}
			return num;
		}

		public void GetDependencies(IList<ReportObject> dependencies, ReportObject parent)
		{
			ReportExpressionUtils.GetDependencies(dependencies, parent, this.Expression);
		}

		public static bool operator ==(ReportExpression<T> left, ReportExpression<T> right)
		{
			if (left.Value.Equals(right.Value))
			{
				return left.Expression == right.Expression;
			}
			return false;
		}

		public static bool operator ==(ReportExpression<T> left, T right)
		{
			if (!left.IsExpression)
			{
				return left.Value.Equals(right);
			}
			return false;
		}

		public static bool operator ==(T left, ReportExpression<T> right)
		{
			if (!right.IsExpression)
			{
				return right.Value.Equals(left);
			}
			return false;
		}

		public static bool operator ==(ReportExpression<T> left, string right)
		{
			if (left.IsExpression)
			{
				return left.Expression == right;
			}
			return false;
		}

		public static bool operator ==(string left, ReportExpression<T> right)
		{
			if (right.IsExpression)
			{
				return right.Expression == left;
			}
			return false;
		}

		public static bool operator !=(ReportExpression<T> left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static bool operator !=(ReportExpression<T> left, T right)
		{
			return !(left == right);
		}

		public static bool operator !=(T left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static bool operator !=(ReportExpression<T> left, string right)
		{
			return !(left == right);
		}

		public static bool operator !=(string left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static explicit operator T(ReportExpression<T> value)
		{
			return value.Value;
		}

		public static implicit operator ReportExpression<T>(T value)
		{
			return new ReportExpression<T>(value);
		}

		public static implicit operator ReportExpression<T>(T? value)
		{
			if (value.HasValue)
			{
				return new ReportExpression<T>(value.Value);
			}
			return new ReportExpression<T>(null, CultureInfo.InvariantCulture);
		}

		public static explicit operator string(ReportExpression<T> value)
		{
			return value.ToString();
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string value = reader.ReadString();
			this.Init(value, CultureInfo.InvariantCulture);
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.ToString(null, CultureInfo.InvariantCulture));
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			if (!this.IsExpression && typeof(IShouldSerialize).IsAssignableFrom(typeof(T)))
			{
				return ((IShouldSerialize)(object)this.m_value).ShouldSerializeThis();
			}
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
