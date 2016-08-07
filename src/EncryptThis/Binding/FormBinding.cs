using CefSharp.WinForms;
using EncryptThis.Forms;
using Ninject;
using Ninject.Modules;

namespace EncryptThis.Binding
{
    public class FormBinding : NinjectModule
    {

        public override void Load()
        {
            /* Remember to add the browser as control because designer/visual studio
             * may crashes if it is there already and CEF is not initialized */
            Bind<FormWebBrowser>()
                .ToSelf()
                .InSingletonScope()
                .OnActivation((context, formWebBrowser) => formWebBrowser.SetupBrowser(context.Kernel.Get<ChromiumWebBrowser>()));
        }

    }
}
