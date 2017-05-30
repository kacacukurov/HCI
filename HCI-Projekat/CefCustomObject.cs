using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CefSharp;
using CefSharp.Wpf;
using System.Windows;

namespace HCI_Projekat
{
    public class CefCustomObject
    {
        private static ChromiumWebBrowser _instanceBrowser = null;
        private static Window _instanceWindow = null;

        public CefCustomObject(ChromiumWebBrowser originalBrowser, Window mainWindow)
        {
            _instanceBrowser = originalBrowser;
            _instanceWindow = mainWindow;
        }

        public void showDevTools()
        {
            _instanceBrowser.ShowDevTools();
        }
    }
}
