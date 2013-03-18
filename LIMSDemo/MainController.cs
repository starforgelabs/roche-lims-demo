using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LIMSDemo
{
    //
    // This is the heart of the program, the main window's controller.
    //
    public class MainController : IMainController
    {
        private string _exportFileName = string.Empty;
        private readonly LIMSConnection _model = new LIMSConnection();
        private WindowMain _view;

        public void AbortRun()
        {
            if (_model.AbortExperiment())
                DisplayMessage("Experiment aborted.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        public void AcquireLibrary()
        {
            if (_model.LoadLibrary())
                DisplayMessage("COM Object Loaded.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private DateTime? BuildDate(string aDayText, int aMonthIndex, string
            aYearText)
        {
            int lYear;
            if (!Int32.TryParse(aYearText, out lYear)) return null;
            if (lYear < 0) return null;
            if (lYear < 80) lYear += 2000; else if (lYear < 100) lYear += 1900;

            int lMonth = aMonthIndex + 1;
            if (lMonth < 1 || lMonth > 12) return null;

            int lDay;
            if (!Int32.TryParse(aDayText, out lDay)) return null;
            if (lDay < 1 || lDay > 31) return null;

            if (DateTime.DaysInMonth(lYear, lMonth) < lDay) return null;

            return new DateTime(lYear, lMonth, lDay);
        }

        public void ClearQueryParameters()
        {
            if (_view == null) return;

            _view.txtQueryName.Text = "";
            _view.txtQueryOwner.Text = "";
            _view.txtQueryType.Text = "";
            _view.rbQueryDateAll.IsChecked = true;
        }

        public void CloseDoor()
        {
            if (_model.CloseDoor())
                DisplayMessage("Door closed.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void DisplayLastStatus()
        {
            if (_model.HasLastResult)
                DisplayMessage(_model.LastResult.Message + "\r\n" + _model.LastResult.UserMessage);
            else
                DisplayMessage("No last result information available.");
        }

        private void DisplayMessage(string aMessage)
        {
            DisplayMessage(aMessage, true);
        }

        private void DisplayMessage(string aMessage, bool aClearDisplay)
        {
            if (_view == null) return;

            if (aClearDisplay)
                _view.txtMessages.Text = aMessage + "\r\n";
            else
                _view.txtMessages.Text += aMessage + "\r\n";
        }

        public void ExperimentStatus()
        {
            string lStatus;
            if (_model.GetExperimentStatus(ExperimentName, out lStatus))
            {
                DisplayMessage("Experiment Status:");
                DisplayMessage(lStatus, false);
            }
            else
                DisplayLastStatus();
        }

        public void ExperimentSummary()
        {
            string lSummary;
            if (_model.GetExperimentSummary(ExperimentName, out lSummary))
            {
                DisplayMessage("Experiment Summary:");
                DisplayMessage(lSummary, false);
            }
            else
                DisplayLastStatus();
        }

        public void ExportExperiment()
        {
            string lInitialDirectory;
            try
            {
                lInitialDirectory = System.IO.Path.GetDirectoryName(_exportFileName);
            }
            catch
            {
                lInitialDirectory = Environment.GetEnvironmentVariable("CSIDL_DEFAULT_MYDOCUMENTS");
            }

            var lDialog = new OpenFileDialog
            {
                Title = "Chose Export File",
                Filter = "Object Export (*.ixo)|*.ixo|All files (*.*)|*.*",
                DefaultExt = "ixo",
                CheckPathExists = true,
                CheckFileExists = false,
                InitialDirectory = lInitialDirectory
            };

            if (lDialog.ShowDialog() == DialogResult.OK)
            {
                _exportFileName = lDialog.FileName;
                if (_model.ExportExperiment(ExperimentName, _exportFileName))
                    DisplayMessage("Experiment exported to " + _exportFileName);
                else
                    DisplayLastStatus();                    
            }
            else
                DisplayMessage("Export cancelled.");
        }

        public void GetContainerBarcode()
        {
            string lBarcode;

            if (_model.GetContainerBarcode(out lBarcode))
                DisplayMessage("Container Barcode: " + lBarcode);
            else
            {
                DisplayMessage("Failed to obtain container barcode.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        public void GetSensor()
        {
            bool lSensor;

            if (_model.GetContainerSensor(out lSensor))
                DisplayMessage("Container sensor: " + (lSensor ? "ON" : "OFF"));
            else
            {
                DisplayMessage("Failed to obtain sensor value.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        public void GetStatus()
        {
            string lStatus;

            if (_model.GetStatus(out lStatus))
                DisplayMessage("Status message: " + lStatus);
            else
            {
                DisplayMessage("Failed to obtain status message.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        public void Login()
        {
            if (_view == null) return;

            if (_model.Connect(_view.txtHostname.Text, _view.txtUsername.Text, _view.txtPassword.Password))
                DisplayMessage("Logged in.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        public void Logout()
        {
            if (_model.Disconnect()) DisplayMessage("Logged out.");
            UpdateControls();
        }

        public void OpenDoor()
        {
            if (_model.OpenDoor())
                DisplayMessage("Door opened.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        public void Query()
        {
            if (_view == null) return;

            LIMSClientLib.LIMSQueryDateType lDateType;
            DateTime? lFromDate = null;
            DateTime? lToDate = null;

            if (_view.rbQueryDateAll.IsChecked ?? false)
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtAllDateQuery;
            else if (_view.rbQueryDateCreated.IsChecked ?? false)
            {
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtCreationDateQuery;
                lFromDate = BuildDate(_view.cbDateCreatedStartDay.Text, 
                                      _view.cbDateCreatedStartMonth.SelectedIndex,
                                      _view.cbDateCreatedStartYear.Text);
                if (lFromDate == null)
                {
                    DisplayMessage("Could not parse starting date created. Query not run.");
                    return;
                }
                lToDate = BuildDate(_view.cbDateCreatedStopDay.Text, 
                                    _view.cbDateCreatedStopMonth.SelectedIndex,
                                    _view.cbDateCreatedStopYear.Text);
                if (lToDate == null)
                {
                    DisplayMessage("Could not parse ending date created. Query not run.");
                    return;
                }
            }
            else
            {
                lDateType = LIMSClientLib.LIMSQueryDateType.qdtModificationDateQuery;
                lFromDate = BuildDate(_view.cbDateModifiedStartDay.Text, 
                                      _view.cbDateModifiedStartMonth.SelectedIndex,
                                      _view.cbDateModifiedStartYear.Text);
                if (lFromDate == null)
                {
                    DisplayMessage("Could not parse starting date modified. Query not run.");
                    return;
                }
                lToDate = BuildDate(_view.cbDateModifiedStopDay.Text, 
                                    _view.cbDateModifiedStopMonth.SelectedIndex,
                                    _view.cbDateModifiedStopYear.Text);
                if (lToDate == null)
                {
                    DisplayMessage("Could not parse ending date modified. Query not run.");
                    return;
                }
            }

            List<LIMSConnection.QueryResult> lResults;

            if (_model.ExecuteQuery(
                    new LIMSConnection.QueryParameters
                    {
                        Name = _view.txtQueryName.Text,
                        ObjectType = _view.txtQueryType.Text,
                        Owner = _view.txtQueryOwner.Text,
                        DateType = lDateType,
                        DateFrom = lFromDate ?? DateTime.MaxValue,
                        DateTo = lToDate ?? DateTime.MaxValue
                    }, out lResults))
            {
                DisplayMessage("Query Successful.");

                foreach (var i in lResults)
                    DisplayMessage(string.Format("  Name: '{0}' Path: '{1}' Type: '{2}' Created: {3} Modified: {4}",
                        i.Name, i.Path, i.ObjectType, i.Created, i.Modified), false);
            }
            else
                DisplayLastStatus();
        }

        public void ReleaseLibrary()
        {
            _model.UnloadLibrary();
            DisplayMessage("COM object released.");
            UpdateControls();
        }

        public void ReserveInstrument()
        {            
            if (_model.ReserveInstrument())
                DisplayMessage("Instrument reserved.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        public void StartRun()
        {
            if (!_model.IsReserved)
            {
                DisplayMessage("Instrument must be reserved first.");
                return;
            }

            var lNewExperimentController = new NewRunController(_view);
            NewRun lNewExperiment;
            if (!lNewExperimentController.GetMinimalNewExperiment(out lNewExperiment)) return;

            if (_model.StartRun(lNewExperiment))
                DisplayMessage("Request to start a new run successful.");
            else
                DisplayLastStatus();
        }

        public void Terminate()
        {
            _model.UnreserveInstrument();
            _model.Disconnect();
            _model.UnloadLibrary();
        }

        public void ToggleSensor()
        {
            bool lSensor;

            if (_model.GetContainerSensor(out lSensor))
            {
                if (_model.SetContainerSensor(!lSensor))
                    DisplayMessage("Container sensor value toggled.");
                else
                {
                    DisplayMessage("Failed to set new container sensor value.", false);
                    DisplayLastStatus();
                }
            }
            else
            {
                DisplayMessage("Failed to read container sensor value.", false);
                DisplayLastStatus();
            }

            UpdateControls();
        }

        public void UnreserveInstrument()
        {
            if (_model.UnreserveInstrument())
                DisplayMessage("Instrument released.");
            else
                DisplayLastStatus();

            UpdateControls();
        }

        private void UpdateControls()
        {
            if (_view == null) return;
                    
            bool lIsLoaded = _model.IsLibraryLoaded;
            bool lIsConnected = _model.IsConnected;
            bool lIsResserved = _model.IsReserved;

            // Connection and login boxes
            //
            _view.btnAcquire.IsEnabled = !lIsLoaded;
            _view.btnRelease.IsEnabled = lIsLoaded && !lIsConnected;
            _view.txtHostname.IsEnabled = lIsLoaded && !lIsConnected;
            _view.txtUsername.IsEnabled = lIsLoaded && !lIsConnected;
            _view.txtPassword.IsEnabled = lIsLoaded && !lIsConnected;
            _view.btnConnect.IsEnabled = lIsLoaded && !lIsConnected;
            _view.btnDisconnect.IsEnabled = lIsConnected;

            //
            // Instrument, run &c. boxes
            //
            _view.btnReserve.IsEnabled = lIsConnected;
            _view.btnUnreserve.IsEnabled = lIsConnected;

            _view.btnOpenDoor.IsEnabled = lIsResserved;
            _view.btnCloseDoor.IsEnabled = lIsResserved;
            _view.btnStatus.IsEnabled = lIsResserved;
            _view.btnContainerBarcode.IsEnabled = lIsResserved;
            _view.btnGetSensor.IsEnabled = lIsResserved;
            _view.btnToggleSensor.IsEnabled = lIsResserved;
            _view.btnStartRun.IsEnabled = lIsResserved;
            _view.btnAbortRun.IsEnabled = lIsResserved;

            //
            // Query box
            //
            _view.txtQueryName.IsEnabled = lIsConnected;
            _view.txtQueryType.IsEnabled = lIsConnected;
            _view.txtQueryOwner.IsEnabled = lIsConnected;
            _view.rbQueryDateAll.IsEnabled = lIsConnected;
            _view.rbQueryDateModified.IsEnabled = lIsConnected;
            _view.rbQueryDateCreated.IsEnabled = lIsConnected;

            _view.cbDateCreatedStartDay.IsEnabled = lIsConnected;
            _view.cbDateCreatedStartMonth.IsEnabled = lIsConnected;
            _view.cbDateCreatedStartYear.IsEnabled = lIsConnected;
            _view.cbDateCreatedStopDay.IsEnabled = lIsConnected;
            _view.cbDateCreatedStopMonth.IsEnabled = lIsConnected;
            _view.cbDateCreatedStopYear.IsEnabled = lIsConnected;
            _view.cbDateModifiedStartDay.IsEnabled = lIsConnected;
            _view.cbDateModifiedStartMonth.IsEnabled = lIsConnected;
            _view.cbDateModifiedStartYear.IsEnabled = lIsConnected;
            _view.cbDateModifiedStopDay.IsEnabled = lIsConnected;
            _view.cbDateModifiedStopMonth.IsEnabled = lIsConnected;
            _view.cbDateModifiedStopYear.IsEnabled = lIsConnected;

            _view.btnQueryClear.IsEnabled = lIsConnected;
            _view.btnExecuteQuery.IsEnabled = lIsConnected;

            //
            // Experiment box
            //
            _view.txtExperimentName.IsEnabled = lIsConnected;
            _view.btnExperimentStatus.IsEnabled = lIsConnected;
            _view.btnExperimentSummary.IsEnabled = lIsConnected;
            _view.btnExperimentExport.IsEnabled = lIsConnected;
        }

        public string ExperimentName {
            get
            {
                if (_view == null) return string.Empty;
                return _view.txtExperimentName.Text;
            }
        }

        public WindowMain View
        {
            get { return _view; }
            set
            {
                _view = value;
                UpdateControls();
            }
        }
    }
}
