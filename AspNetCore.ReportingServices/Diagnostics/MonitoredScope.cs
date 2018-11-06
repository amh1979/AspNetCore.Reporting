using AspNetCore.ReportingServices.Diagnostics.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class MonitoredScope : IDisposable
	{
		private const string IndentationTag = "--";

		private readonly Stack<KeyValuePair<string, DateTime>> stack = new Stack<KeyValuePair<string, DateTime>>();

		private string indent = "--";

		private static bool? traceMonitoredScope = null;

		private static readonly MonitoredScope dummyInstance = new MonitoredScope();

		[ThreadStatic]
		private static MonitoredScope instance;

		private static bool TraceMonitoredScope
		{
			get
			{
				if (!MonitoredScope.traceMonitoredScope.HasValue)
				{
					bool value = false;
					try
					{ 
                        
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ReportServerTracing\\"))
						{
							if (registryKey != null)
							{
								object value2 = registryKey.GetValue("TraceMonitoredScope");
								value = (value2 is int && (int)value2 == 1);
							}
						}
					}
					catch (Exception)
					{
					}
					MonitoredScope.traceMonitoredScope = value;
				}
				return MonitoredScope.traceMonitoredScope.Value;
			}
		}

		private MonitoredScope()
		{
		}

		internal static MonitoredScope New(string name)
		{
			if (!MonitoredScope.TraceMonitoredScope)
			{
				return MonitoredScope.dummyInstance;
			}
			MonitoredScope monitoredScope = MonitoredScope.instance;
			if (monitoredScope == null)
			{
				monitoredScope = (MonitoredScope.instance = new MonitoredScope());
			}
			monitoredScope.Start(name);
			return monitoredScope;
		}

		internal static MonitoredScope NewFormat(string format, object arg0)
		{
			if (!MonitoredScope.TraceMonitoredScope)
			{
				return MonitoredScope.dummyInstance;
			}
			return MonitoredScope.New(string.Format(CultureInfo.InvariantCulture, format, arg0));
		}

		internal static MonitoredScope NewFormat(string format, object arg0, object arg1)
		{
			if (!MonitoredScope.TraceMonitoredScope)
			{
				return MonitoredScope.dummyInstance;
			}
			return MonitoredScope.New(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));
		}

		internal static MonitoredScope NewFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!MonitoredScope.TraceMonitoredScope)
			{
				return MonitoredScope.dummyInstance;
			}
			return MonitoredScope.New(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));
		}

		internal static MonitoredScope NewConcat(string arg0, object arg1)
		{
			if (!MonitoredScope.TraceMonitoredScope)
			{
				return MonitoredScope.dummyInstance;
			}
			if (arg1 != null)
			{
				return MonitoredScope.New(arg0 + arg1);
			}
			return MonitoredScope.New(arg0);
		}

		internal static void End(string name)
		{
			if (MonitoredScope.TraceMonitoredScope)
			{
				MonitoredScope monitoredScope = MonitoredScope.instance;
				if (!string.Equals(monitoredScope.stack.Peek().Key, name, StringComparison.InvariantCulture))
				{
					throw new Exception("MonitoredScope cannot be ended because the start and end scope names do not match!");
				}
				monitoredScope.Dispose();
			}
		}

		private void Start(string name)
		{
			if (MonitoredScope.TraceMonitoredScope)
			{
				this.stack.Push(new KeyValuePair<string, DateTime>(name, DateTime.UtcNow));
				string message = string.Format(CultureInfo.InvariantCulture, "< {0} {1}", this.indent, name);
				RSTrace.MonitoredScope.Trace(message);
				this.indent += "--";
			}
		}

		public void Dispose()
		{
			if (MonitoredScope.TraceMonitoredScope)
			{
				KeyValuePair<string, DateTime> keyValuePair = this.stack.Pop();
				this.indent = this.indent.Substring(0, this.indent.Length - 2);
				double totalMilliseconds = (DateTime.UtcNow - keyValuePair.Value).TotalMilliseconds;
				string message = string.Format(CultureInfo.InvariantCulture, "> {0} {1} - {2}", this.indent, keyValuePair.Key, totalMilliseconds);
				RSTrace.MonitoredScope.Trace(message);
			}
		}
	}
}
