namespace LIMSDemo
{
    //
    // This is to provide a callback that the main form can use to communicate back to the controller. 
    //
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

        void StartRun();

        void Terminate();

        void ToggleSensor();

        void UnreserveInstrument();

        WindowMain View { get; set; }
    }
}
