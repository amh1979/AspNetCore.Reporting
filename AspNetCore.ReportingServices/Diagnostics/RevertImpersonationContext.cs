using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
namespace AspNetCore.ReportingServices.Diagnostics
{
	internal static class RevertImpersonationContext
	{
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public static void Run(ContextBody callback)
        {
            callback();
            return;

            //WindowsIdentity.RunImpersonated(SafeAccessTokenHandle.InvalidHandle, () => callback());
            return;
            SecurityContext.Run(SecurityContext.Capture(), delegate (object state) {
                WindowsImpersonationContext context = null;
                try
                {
                    try
                    {
                        context = WindowsIdentity.Impersonate(IntPtr.Zero);
                        callback();
                    }
                    finally
                    {
                        if (context != null)
                        {
                            context.Undo();
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }, null);

        }

        public static void RunFromRestrictedCasContext(ContextBody callback)
        {
            callback();
            return;
            SecurityContext.Run(SecurityContext.Capture(), delegate (object state) {
                new SecurityPermission(SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.UnmanagedCode).Assert();
                WindowsImpersonationContext context = null;
                try
                {
                    try
                    {
                        context = WindowsIdentity.Impersonate(IntPtr.Zero);
                        CodeAccessPermission.RevertAssert();
                        callback();
                    }
                    finally
                    {
                        if (context != null)
                        {
                            context.Undo();
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }, null);
            return;
            SecurityContext.Run(SecurityContext.Capture(), delegate (object state)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlPrincipal).Assert();
                //WindowsImpersonationContext windowsImpersonationContext = null;
                try
                {
                    try
                    {
                        WindowsIdentity.Impersonate(IntPtr.Zero);
                        //windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
                        //CodeAccessPermission.RevertAssert();
                        //WindowsIdentity.RunImpersonated(SafeAccessTokenHandle.InvalidHandle, () => callback());
                        //callback();
                    }
                    finally
                    {
                        //if (windowsImpersonationContext != null)
                        //{
                        //    windowsImpersonationContext.Undo();
                        //}
                    }
                }
                catch
                {
                    throw;
                }
            }, null);
            
        }

        internal const int SkipStackFrames = 8;
    }
}
