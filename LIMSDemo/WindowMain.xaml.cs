using System.Collections.Generic;
using System.Windows;

namespace LIMSDemo
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain
    {
        public WindowMain()
        {
            InitializeComponent();
            UpdateControls();
        }

        private readonly LIMSConnection _lims = new LIMSConnection();

        private void Display(string aMessage)
        {
            Display(aMessage, true);
        }

        private void Display(string aMessage, bool aClearDisplay)
        {
            if(aClearDisplay)
                txtMessages.Text  = aMessage + "\r\n";
            else
                txtMessages.Text += aMessage + "\r\n";
        }

        private void DisplayLastStatus()
        {
            if (_lims.HasLastResult)
                Display(_lims.LastResult.Message + "\r\n" + _lims.LastResult.UserMessage);
        }

        private void UpdateControls()
        {
            bool lIsLoaded = _lims.IsLibraryLoaded;
            bool lIsConnected = _lims.IsConnected;
            bool lIsResserved = _lims.IsReserved;

            btnAcquire.IsEnabled = !lIsLoaded;
            btnRelease.IsEnabled = lIsLoaded && !lIsConnected;
            txtHostname.IsEnabled = lIsLoaded && !lIsConnected;
            txtUsername.IsEnabled = lIsLoaded && !lIsConnected;
            txtPassword.IsEnabled = lIsLoaded && !lIsConnected;
            btnConnect.IsEnabled = lIsLoaded && !lIsConnected;
            btnDisconnect.IsEnabled = lIsConnected;

            btnReserve.IsEnabled = lIsConnected;
            btnUnreserve.IsEnabled = lIsConnected;

            btnOpenDoor.IsEnabled = lIsResserved;
            btnCloseDoor.IsEnabled = lIsResserved;
            btnStatus.IsEnabled = lIsResserved;
            btnContainerBarcode.IsEnabled = lIsResserved;
            btnGetSensor.IsEnabled = lIsResserved;
            btnToggleSensor.IsEnabled = lIsResserved;
            btnStartRun.IsEnabled = lIsResserved;
            btnAbortRun.IsEnabled = lIsResserved;

            txtQueryName.IsEnabled = lIsConnected;
            txtQueryType.IsEnabled = lIsConnected;
            txtQueryOwner.IsEnabled = lIsConnected;
            rbQueryDateAll.IsEnabled = lIsConnected;
            rbQueryDateModified.IsEnabled = lIsConnected;
            rbQueryDateCreated.IsEnabled = lIsConnected;
            btnQueryClear.IsEnabled = lIsConnected;
            btnExecuteQuery.IsEnabled = lIsConnected;

            txtExperimentName.IsEnabled = lIsConnected;
            btnExperimentStatus.IsEnabled = lIsConnected;
            btnExperimentSummary.IsEnabled = lIsConnected;
            btnExperimentExport.IsEnabled = lIsConnected;
            /*
                        btnExperimentWizard.IsEnabled = lIsConnected;
                        btnExperimentResults.IsEnabled = lIsConnected;
            */
        }

        private void btnAbortRun_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.AbortExperiment())
                Display("Experiment aborted.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnAcquire_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.LoadLibrary())
                Display("COM Object Loaded.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnCloseDoor_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.CloseDoor())
                Display("Door closed.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.Connect(txtHostname.Text, txtUsername.Text, txtPassword.Password))
                Display("Logged in.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnContainerBarcode_Click(object sender, RoutedEventArgs e)
        {
            string lBarcode;

            if (_lims.GetContainerBarcode(out lBarcode))
                Display("Container Barcode: " + lBarcode);
            else
            {
                Display("Failed to obtain container barcode.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if(_lims.Disconnect()) Display("Logged out.");
            UpdateControls();
        }

        private void btnExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            LIMSClientLib.LIMSQueryDateType lDateType;
            System.DateTime lFromDate = System.DateTime.MinValue;;
            System.DateTime lToDate = System.DateTime.MaxValue;

            if(rbQueryDateAll.IsChecked ?? false)
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtAllDateQuery;
            else if (rbQueryDateCreated.IsChecked ?? false)
            {
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtCreationDateQuery;
                // lFromDate
                // lToDate
            }
            else
            {
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtModificationDateQuery;
                // lFromDate
                // lToDate
            }

            List<LIMSConnection.QueryResult> lResults;

            if (_lims.ExecuteQuery(
                    new LIMSConnection.QueryParameters
                    {
                        Name = txtQueryName.Text,
                        ObjectType = txtQueryType.Text,
                        Owner = txtQueryOwner.Text,
                        DateType = lDateType,
                        DateFrom = lFromDate,
                        DateTo = lToDate
                    }, out lResults))
            {
                Display("Query Successful.");

                foreach (var i in lResults)
                    Display(string.Format("  Name: '{0}' Path: '{1}' Type: '{2}' Created: {3} Modified: {4}",
                        i.Name, i.Path, i.ObjectType, i.Created, i.Modified), false);
            }
            else
                DisplayLastStatus();
        }

        private void btnExperimentStatus_Click(object sender, RoutedEventArgs e)
        {
            string lStatus;
            if (_lims.GetExperimentStatus(txtExperimentName.Text, out lStatus))
            {
                Display("Experiment Status:");
                Display(lStatus, false);
            }
            else
                DisplayLastStatus();
        }

        private void btnExperimentSummary_Click(object sender, RoutedEventArgs e)
        {
            string lSummary;
            if (_lims.GetExperimentSummary(txtExperimentName.Text, out lSummary))
            {
                Display("Experiment Summary:");
                Display(lSummary, false);
            }
            else
                DisplayLastStatus();
        }

        private void btnGetSensor_Click(object sender, RoutedEventArgs e)
        {
            bool lSensor;

            if (_lims.GetContainerSensor(out lSensor))
                Display("Container sensor: " + (lSensor?"ON":"OFF"));
            else
            {
                Display("Failed to obtain sensor value.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        private void btnOpenDoor_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.OpenDoor())
                Display("Door opened.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnQueryClear_Click(object sender, RoutedEventArgs e)
        {
            txtQueryName.Text = "";
            txtQueryOwner.Text = "";
            txtQueryType.Text = "";
            rbQueryDateAll.IsChecked = true;
        }

        private void btnRelease_Click(object sender, RoutedEventArgs e)
        {
            _lims.UnloadLibrary();
            Display("COM object released.");
            UpdateControls();
        }

        private void btnReserve_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.ReserveInstrument())
                Display("Instrument reserved.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            string lStatus;

            if (_lims.GetStatus(out lStatus))
                Display("Status message: " + lStatus);
            else
            {
                Display("Failed to obtain status message.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        private void btnToggleSensor_Click(object sender, RoutedEventArgs e)
        {
            bool lSensor;

            if (_lims.GetContainerSensor(out lSensor))
            {
                if (_lims.SetContainerSensor(!lSensor))
                    Display("Container sensor value toggled.");
                else
                {
                    Display("Failed to set new container sensor value.", false);
                    DisplayLastStatus();
                }
            }
            else
            {
                Display("Failed to read container sensor value.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        private void btnUnreserve_Click(object sender, RoutedEventArgs e)
        {
            if (_lims.UnreserveInstrument())
                Display("Instrument released.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void btnExperimentExport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
