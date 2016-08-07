using CefSharp;
using CefSharp.WinForms;
using EncryptThis.Forms;
using Ninject;
using Ninject.Syntax;

namespace EncryptThis.Service.Application
{
    public class Application : IApplication
    {

        private readonly IResolutionRoot kernel;

        public Application(IResolutionRoot kernel, ChromiumWebBrowser browser, IApi api)
        {
            this.kernel = kernel;

            // register the api object as js object in the browser window
            browser.RegisterJsObject("api", api);
        }

        public void Run()
        {
            // show the form with the webbrowser
            System.Windows.Forms.Application.Run(kernel.Get<FormWebBrowser>());
        }

    }
}
