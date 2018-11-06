using System;
using System.Collections.Generic;

namespace AspNetCore.Reporting
{
    [Serializable]
    internal sealed class ReportHierarchy : Stack<ReportInfo>, IDisposable
    {
        public ReportHierarchy()
        {
            //ServerModeSession serverSession = new ServerModeSession(serverReport);
            LocalModeSession localSession = new LocalModeSession();
            ReportInfo item = new ReportInfo(localSession);
            base.Push(item);
        }

        public void Dispose()
        {
            foreach (ReportInfo reportInfo in this)
            {
                reportInfo.Dispose();
            }
        }

        public void DisposeNonSessionResources()
        {
            foreach (ReportInfo reportInfo in this)
            {
                reportInfo.DisposeNonSessionResources();
            }
        }

        public ReportInfo MainReport
        {
            get
            {
                ReportInfo[] array = base.ToArray();
                return array[array.Length - 1];
            }
        }

       

        public void ConnectChangeEvents(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
        {
            foreach (ReportInfo reportInfo in this)
            {
                reportInfo.ConnectChangeEvent(changeHandler, dataInitializationHandler);
            }
        }

        public void DisconnectChangeEvents(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
        {
            foreach (ReportInfo reportInfo in this)
            {
                reportInfo.DisconnectChangeEvent(changeHandler, dataInitializationHandler, true);
            }
        }

        public void LoadViewState(object viewStateObj)
        {
            object[] array = (object[])viewStateObj;
            this.SyncToClientPage(array.Length);
            int num = 0;
            foreach (ReportInfo reportInfo in this)
            {
                reportInfo.LoadViewState(array[num++]);
            }
        }

        public object SaveViewState(bool includeReport)
        {
            object[] array = new object[base.Count];
            int num = 0;
            foreach (ReportInfo reportInfo in this)
            {
                array[num++] = reportInfo.SaveViewState(includeReport);
            }
            return array;
        }

        public void SyncToClientPage(int clientStackSize)
        {
            if (clientStackSize < 1)
            {
                throw new ArgumentOutOfRangeException("clientStackSize");
            }
            for (int i = base.Count; i > clientStackSize; i--)
            {
                base.Pop();
            }
        }
    }
}
