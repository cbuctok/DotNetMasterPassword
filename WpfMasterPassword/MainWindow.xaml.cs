using System;
using System.Diagnostics;
using System.Windows;
using WpfMasterPassword.Common;
using WpfMasterPassword.Properties;
using WpfMasterPassword.ViewModel;

namespace WpfMasterPassword
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // window placement
                Settings.Default.MainWindowPlacement = this.GetPlacement();

                // inform about closing idea (allow viewmodel to cancel closing)
                var viewModel = DataContext as DocumentViewModel;
                if (null != viewModel)
                {
                    viewModel.OnClose(e);
                }

                // save settings (we have seen problems here)
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                // suppress problem, don't interrupt the closing for that
                Trace.WriteLine(ex.ToString());
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.SetPlacement(Settings.Default.MainWindowPlacement);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var viewModel = DataContext as DocumentViewModel;
                if (null != viewModel)
                {
                    viewModel.OpenFileFromDrop(files[0]);
                }
            }
        }
    }

    public class MainWindow_DesignTimeData : DocumentViewModel
    {
        public MainWindow_DesignTimeData()
        {
            Config.UserName.Value = "John Doe";

            var site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "ebay.com";
            site.Login.Value = "jdoe@gmail.com";
            site.Counter.Value = 3;
            Config.Sites.Add(site);

            site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "ripeyesteaks.com";
            site.Login.Value = "john@gorgelmail.com";
            Config.Sites.Add(site);

            site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "othersite.com";
            site.Login.Value = "doe@john.org";
            Config.Sites.Add(site);
        }
    }
}