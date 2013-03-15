namespace LIMSDemo
{
    /// <summary>
    /// Interaction logic for WindowMinimalNewRun.xaml
    /// </summary>
    public partial class WindowMinimalNewRun
    {
        public WindowMinimalNewRun()
        {
            InitializeComponent();
        }

        private void ActionTextChanged(object aSender, System.Windows.Controls.TextChangedEventArgs aEventArgs)
        {
            Controller.TextChanged();
        }

        private void Button_Click(object aSender, System.Windows.RoutedEventArgs aEventArgs)
        {
            DialogResult = false;
        }

        private void btnOK_Click(object aSender, System.Windows.RoutedEventArgs aEventArgs)
        {
            DialogResult = true;
        }

        public INewRunController Controller { get; set; }
    }
}
