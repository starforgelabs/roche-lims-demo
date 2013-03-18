using System.Windows;

namespace LIMSDemo
{
    /// <summary>
    /// This is the main window. 
    /// 
    /// This uses a more proper MVC paradigm, so the window delegates all window
    /// manipulation to its controller. It does not modify itself.
    /// </summary>
    public partial class WindowMain
    {
        public WindowMain()
        {
            InitializeComponent();

            // Technically the view shouldn't own the controller. 
            // This is a bit of slacking on my part because this is the main window,
            // and it's not worth rewiring Microsoft's default app startup sequence
            // to be 100% pure MVC. 
            _controller = new MainController {View = this};
        }

        private readonly IMainController _controller;

        private void Window_Closing(object aSender, System.ComponentModel.CancelEventArgs aEventArgs)
        {
            _controller.Terminate();
        }

        private void btnAbortRun_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.AbortRun();
        }

        private void btnAcquire_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.AcquireLibrary();
        }

        private void btnCloseDoor_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.CloseDoor();
        }

        private void btnConnect_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.Login();
        }

        private void btnContainerBarcode_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.GetContainerBarcode();
        }

        private void btnDisconnect_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.Logout();
        }

        private void btnExecuteQuery_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.Query();
        }

        private void btnExperimentExport_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ExportExperiment();
        }

        private void btnExperimentStatus_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ExperimentStatus();
        }

        private void btnExperimentSummary_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ExperimentSummary();
        }

        private void btnGetSensor_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.GetSensor();
        }

        private void btnOpenDoor_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.OpenDoor();
        }

        private void btnQueryClear_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ClearQueryParameters();
        }

        private void btnRelease_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ReleaseLibrary();
        }

        private void btnReserve_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ReserveInstrument();
        }

        private void btnStartRun_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.StartRun();
        }

        private void btnStatus_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.GetStatus();
        }

        private void btnToggleSensor_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.ToggleSensor();
        }

        private void btnUnreserve_Click(object aSender, RoutedEventArgs aEventArgs)
        {
            _controller.UnreserveInstrument();
        }
    }
}
