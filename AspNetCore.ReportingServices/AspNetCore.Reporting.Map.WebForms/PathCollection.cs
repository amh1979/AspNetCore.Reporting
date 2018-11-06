using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PathCollection : NamedCollection
	{
		private Path this[int index]
		{
			get
			{
				return (Path)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Path this[string name]
		{
			get
			{
				return (Path)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public Path this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal PathCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(Path);
		}

		public Path Add(string name)
		{
			Path path = new Path();
			path.Name = name;
			this.Add(path);
			return path;
		}

		public int Add(Path value)
		{
			return base.List.Add(value);
		}

		public void Remove(Path value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common != null && base.Common.MapCore != null && base.Common.MapCore.PathFields != null)
			{
				if (ignoreCase)
				{
					searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
				}
				FieldCollection pathFields = base.Common.MapCore.PathFields;
				{
					foreach (Path item in this)
					{
						string text = ignoreCase ? item.Name.ToUpper(CultureInfo.CurrentCulture) : item.Name;
						if (exactSearch)
						{
							if (text == searchFor)
							{
								arrayList.Add(item);
								continue;
							}
						}
						else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
						{
							arrayList.Add(item);
							continue;
						}
						foreach (Field item2 in pathFields)
						{
							if (!uniqueOnlyFields || item2.UniqueIdentifier)
							{
								try
								{
									if (!base.Common.MapCore.IsDesignMode() || !item2.IsTemporary)
									{
										object obj = item[item2.Name];
										if (obj != null)
										{
											if (item2.Type == typeof(string))
											{
												string text2 = ignoreCase ? ((string)obj).ToUpper(CultureInfo.CurrentCulture) : ((string)obj);
												if (exactSearch)
												{
													if (text2 == searchFor)
													{
														arrayList.Add(item);
														goto IL_01c4;
													}
												}
												else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
												{
													arrayList.Add(item);
													goto IL_01c4;
												}
											}
											else
											{
												object obj2 = item2.Parse(searchFor);
												if (obj2 != null && obj2.Equals(obj))
												{
													arrayList.Add(item);
													goto IL_01c4;
												}
											}
										}
									}
								}
								catch
								{
								}
							}
						}
						IL_01c4:;
					}
					return arrayList;
				}
			}
			return arrayList;
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Path1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Path{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			Path path = (Path)value;
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
				base.Common.MapCore.InvalidateCachedBounds();
			}
			base.Invalidate();
		}
	}
}
