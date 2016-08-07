using System;
using System.IO;
using CefSharp.WinForms;
using EncryptThis.Service;
using EncryptThis.Service.Api;
using EncryptThis.Service.Application;
using EncryptThis.Service.Crypto;
using Ninject.Modules;

namespace EncryptThis.Binding
{
    public class ServiceBinding : NinjectModule
    {

        public override void Load()
        {
            /* The chromium web browser should point to .\frontend\index.html because there is our GUI located.
             * Need an absolute URI here just to be safe CEF handles it correctly as a local resource */
            Bind<ChromiumWebBrowser>()
                .ToConstant(new ChromiumWebBrowser(new Uri(Path.Combine(Environment.CurrentDirectory, "frontend\\index.html")).AbsoluteUri))
                .InSingletonScope();

            /* The api should always point to a fixed subdirectory (container directory) because if not, we may
             * encrypt the C drive if .\ was our container directory and we're located in C:\ (who does this?; got no better explanation) */
            Bind<IApi>()
                .To<Api>()
                .InSingletonScope()
                .WithConstructorArgument("containerDirectory", Path.Combine(Environment.CurrentDirectory, "container"));

            Bind<IApplication>()
                .To<Application>()
                .InSingletonScope();

            Bind<ICrypto>()
                .To<Crypto>()
                .InTransientScope();
        }

    }
}
