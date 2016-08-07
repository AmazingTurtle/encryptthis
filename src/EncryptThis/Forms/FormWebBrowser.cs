using System.Drawing;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace EncryptThis.Forms
{
    public partial class FormWebBrowser : Form
    {
        public FormWebBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configures the browser control and adds it to controls
        /// </summary>
        /// <param name="browser">The browser to be added as control</param>
        public void SetupBrowser(ChromiumWebBrowser browser)
        {
            browser.Location = new Point(0, 0);
            browser.Name = "chromiumWebBrowser";
            browser.Dock = DockStyle.Fill;
            Controls.Add(browser);
        }

    }
}
