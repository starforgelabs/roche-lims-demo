using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIMSDemo
{
    public interface IMainController
    {
        void AbortRun();

        void AcquireLibrary();

        void ClearQueryParameters();

        void CloseDoor();

        void ExperimentStatus();

        void ExperimentSummary();

        void ExportExperiment();

        void GetContainerBarcode();

        void GetSensor();

        void GetStatus();

        void Login();

        void Logout();

        void OpenDoor();

        void Query();

        void ReleaseLibrary();

        void ReserveInstrument();

        void Terminate();

        void ToggleSensor();

        void UnreserveInstrument();

        WindowMain View { get; set; }
    }
}
