using System.Collections.Generic;
using LIMSClientLib;

namespace LIMSDemo
{
    public class LIMSConnection
    {
        public struct QueryParameters
        {
            public string Name;
            public string ObjectType;
            public string Owner;
            public LIMSQueryDateType DateType;
            public System.DateTime DateFrom;
            public System.DateTime DateTo;
        }

        public struct QueryResult
        {
            public string Name;
            public string Path;
            public string ObjectType;
            public System.DateTime Created;
            public System.DateTime Modified;
        }

        private LIMSClientLib.LIMSConnection _connection;
        private LIMSOperationResult _lastResult;

        public bool AbortExperiment()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.AbortExperiment();
            return IsLastResultSuccessful;
        }

        public bool CloseDoor()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.Close();
            return IsLastResultSuccessful;
        }

        public bool Connect(string aHostname, string aUsername, string aPassword)
        {
            if (!EnsureLibraryLoaded()) return false;

            _connection.Host = aHostname;
            _lastResult = _connection.Login(aUsername, aPassword);
            return IsLastResultSuccessful;
        }

        public bool Disconnect()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Logout();
            return IsLastResultSuccessful;
        }

        private bool EnsureConnected()
        {
            _lastResult = null;
            return IsConnected;
        }

        private bool EnsureLibraryLoaded()
        {
            _lastResult = null;
            return IsLibraryLoaded;
        }

        public bool ExecuteQuery(QueryParameters aParameters, 
                                 out List<QueryResult> aResults)
        {
            aResults = new List<QueryResult>();
            if (!EnsureConnected()) return false;

            _connection.Query.Name = aParameters.Name;
            _connection.Query.ObjectType = aParameters.ObjectType;
            _connection.Query.Owner = aParameters.Owner;
            _connection.Query.QueryDateType = aParameters.DateType;
            _connection.Query.FromDate = aParameters.DateFrom;
            _connection.Query.ToDate = aParameters.DateTo;

            LIMSQueryResult lQueryResult;
            _lastResult = _connection.Query.ExecuteQuery(out lQueryResult);

            for (var i = 0; i < lQueryResult.Count; i++)
            {
                var lItem = lQueryResult.GetResultData(i);
                aResults.Add(new QueryResult
                    {
                        Name=lItem.Name, 
                        Path=lItem.Path, 
                        ObjectType=lItem.ObjectType, 
                        Created=lItem.CreationDate, 
                        Modified=lItem.ModificationDate
                    });
            }

            return IsLastResultSuccessful;
        }

        public bool ExportExperiment(string aExperimentName, string aFilename)
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.ExperimentInfo.ExportExperiment(aExperimentName, aFilename);

            return IsLastResultSuccessful;
        }

        public bool GetContainerBarcode(out string aBarcode)
        {
            aBarcode = string.Empty;
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.GetContainerBarcode(out aBarcode);
            return IsLastResultSuccessful;
        }

        public bool GetContainerSensor(out bool aSensor)
        {
            aSensor = false;
            if (!EnsureConnected()) return false;

            string lResult;
            _lastResult = _connection.Instrument.GetContainerSensor(out lResult);

            if (IsLastResultSuccessful)
                aSensor = string.Equals("ON", lResult, System.StringComparison.OrdinalIgnoreCase);

            return IsLastResultSuccessful;
        }

        public bool GetExperimentStatus(string aExperimentName, out string aStatus)
        {
            aStatus = string.Empty;
            if (!EnsureConnected()) return false;

            _lastResult = _connection.ExperimentInfo.GetStatus(aExperimentName, out aStatus);
            return IsLastResultSuccessful;
        }

        public bool GetExperimentSummary(string aExperimentName, out string aSummary)
        {
            aSummary = string.Empty;
            if (!EnsureConnected()) return false;

            _lastResult = _connection.ExperimentInfo.GetCompletedExperimentSummary(aExperimentName, out aSummary);
            return IsLastResultSuccessful;
        }

        public bool GetStatus(out string aStatus)
        {
            aStatus = string.Empty;
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.GetStatus(out aStatus);
            return IsLastResultSuccessful;
        }

        public bool LoadLibrary()
        {
            if (EnsureLibraryLoaded()) return true;

            try
            {
                _connection = new LIMSClientLib.LIMSConnection();
                return true;
            }
            catch
            {
                _connection = null;
                return false;
            }
        }

        public bool OpenDoor()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.Open();
            return IsLastResultSuccessful;
        }

        public bool ReserveInstrument()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.Reserve();
            return IsLastResultSuccessful;
        }

        public bool SetContainerSensor(bool aSensor)
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.SetContainerSensor(aSensor ? "ON" : "OFF");
            return IsLastResultSuccessful;
        }

        public void UnloadLibrary()
        {
            _connection = null;
        }

        public bool UnreserveInstrument()
        {
            if (!EnsureConnected()) return false;

            _lastResult = _connection.Instrument.Unreserve();
            return IsLastResultSuccessful;
        }

        public bool HasLastResult
        {
            get { return _lastResult != null; }
        }

        public bool IsConnected
        {
            get { return IsLibraryLoaded && _connection.LoggedIn; }
        }

        public bool IsLastResultSuccessful
        {
            get { return HasLastResult && _lastResult.Successful; }
        }

        public bool IsLibraryLoaded
        {
            get { return _connection != null; }
        }

        public bool IsLibraryRegistered
        {
            get
            {
                try
                {
                    // ReSharper disable ObjectCreationAsStatement
                    new LIMSClientLib.LIMSConnection();
                    // ReSharper restore ObjectCreationAsStatement
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsReserved
        {
            get 
            { 
                var lPreserveResult = _lastResult;
                string lTemp;
                var lResult = GetStatus(out lTemp);
                _lastResult = lPreserveResult;
                return lResult;
            }
        }

        public LIMSOperationResult LastResult
        {
            get { return _lastResult; }
        }
    }
}
