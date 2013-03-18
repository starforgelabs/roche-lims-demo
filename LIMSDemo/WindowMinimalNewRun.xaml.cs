namespace LIMSDemo
{
    /// <summary>
    /// This window is for collecting minimal information needed for a run. 
    /// 
    /// This uses a proper MVC paradigm, so the window delegates all window
    /// manipulation to its controller. It does not modify itself.
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
