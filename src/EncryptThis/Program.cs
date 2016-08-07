using System;
using CefSharp;
using EncryptThis.Service;
using Ninject;

namespace EncryptThis
{
    public static class Program
    {
        /// <summary>
        /// The main entrypoint for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {

            // initialize CEF
            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = true;
            Cef.Initialize(settings, true, false);

            // load ninject modules from current assembly
            // btw: why am i using ninject? -> ninject is sexy, I tried spring.net before. sucks alot.
            IKernel kernel = new StandardKernel();
            kernel.Load(typeof (Program).Assembly);

            // run via IApplication service
            kernel.Get<IApplication>().Run();

        }
    }
}
