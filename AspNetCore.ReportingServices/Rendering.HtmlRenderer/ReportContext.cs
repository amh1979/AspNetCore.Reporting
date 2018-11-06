using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class ReportContext
	{
		private class SubreportContext : IDisposable
		{
			private readonly string m_name;

			private Action m_callback;

			internal string Name
			{
				get
				{
					return this.m_name;
				}
			}

			internal SubreportContext(string name, Action callback)
			{
				this.m_name = name;
				this.m_callback = callback;
			}

			void IDisposable.Dispose()
			{
				this.m_callback();
				this.m_callback = null;
			}
		}

		private Stack<SubreportContext> m_subreports;

		internal ReportContext()
		{
			this.m_subreports = new Stack<SubreportContext>();
		}

		internal IDisposable EnterSubreport(RPLElementPropsDef subreportDef)
		{
			RPLItemPropsDef rPLItemPropsDef = subreportDef as RPLItemPropsDef;
			SubreportContext subreportContext = new SubreportContext(rPLItemPropsDef.Name, this.PopSubreport);
			this.PushSubreport(subreportContext);
			return subreportContext;
		}

		internal IEnumerable<string> GetPath()
		{
			return from s in this.m_subreports
			select s.Name;
		}

		private void PushSubreport(SubreportContext subreport)
		{
			this.m_subreports.Push(subreport);
		}

		private void PopSubreport()
		{
			this.m_subreports.Pop();
		}
	}
}
