using System.Windows;
using System.Windows.Controls;
using WpfMasterPassword.ViewModel;

namespace WpfMasterPassword.UserControls
{
    /// <summary>
    /// Interaction logic for ConfigurationUserControl.xaml
    /// </summary>
    public partial class ConfigurationUserControl : UserControl
    {
        // trick to reset password box
        public bool ResetPassword
        {
            get => (bool)GetValue(ResetPasswordProperty);
            set => SetValue(ResetPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResetPasswordProperty =
            DependencyProperty.Register("ResetPassword", typeof(bool), typeof(ConfigurationUserControl), new PropertyMetadata(false, ResetPasswordChangedCallback));

        private static void ResetPasswordChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ConfigurationUserControl;

            if (null == control)
            {
                return;
            }

            if (!true.Equals(e.NewValue))
            {
                return;
            }

            control.passwordBox.Clear();
        }

        public ConfigurationUserControl()
        {
            InitializeComponent();
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // forward to view model, we can't bind it directly
            var viewModel = DataContext as ConfigurationViewModel;
            if (null != viewModel)
            {
                viewModel.CurrentMasterPassword.Value = passwordBox.SecurePassword;
            }
        }
    }

    public class ConfigurationUserControl_DesignTimeData : ConfigurationViewModel
    {
        public ConfigurationUserControl_DesignTimeData()
        {
            UserName.Value = "John Doe";

            var site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "ebay.com";
            site.Login.Value = "jdoe@gmail.com";
            site.TypeOfPassword.Value = MasterPassword.Core.PasswordType.LongPassword;
            site.Counter.Value = 3;
            Sites.Add(site);

            site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "ripeyesteaks.com";
            site.Login.Value = "john@gorgelmail.com";
            site.TypeOfPassword.Value = MasterPassword.Core.PasswordType.PIN;
            Sites.Add(site);

            SelectedItem.Value = site;

            site = new ConfigurationSiteViewModel();
            site.SiteName.Value = "othersite.com";
            site.Login.Value = "doe@john.org";
            site.TypeOfPassword.Value = MasterPassword.Core.PasswordType.MaximumSecurityPassword;
            Sites.Add(site);
        }
    }
}