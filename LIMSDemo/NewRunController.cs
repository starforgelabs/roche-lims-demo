using System.Windows;

namespace LIMSDemo
{
    //
    // This is the controller for getting minimal run information from a user.
    //
    // It's created by the main controller, which then calls GetMinimalNewExperiment().
    //
    public class NewRunController : INewRunController
    {
        public NewRunController(Window aMainView)
        {
            _mainView = aMainView;
        }

        private readonly Window _mainView;
        private WindowMinimalNewRun _view;

        public bool GetMinimalNewExperiment(out NewRun aNewExperiment)
        {
            aNewExperiment = new NewRun();

            _view = new WindowMinimalNewRun { Owner = _mainView, Controller = this };

            // Open the dialog box modally 
            if (!(_view.ShowDialog() ?? false)) return false;

            aNewExperiment.ExperimentName = _view.txtExperimentName.Text;
            aNewExperiment.ContainerBarcode = _view.txtContainerBarcode.Text;
            aNewExperiment.MacroName = _view.txtMacroName.Text;
            return true;
        }

        public void TextChanged()
        {
            if (_view == null) return;

            _view.btnOK.IsEnabled =
                !string.IsNullOrEmpty(_view.txtContainerBarcode.Text) &&
                !string.IsNullOrEmpty(_view.txtExperimentName.Text) &&
                !string.IsNullOrEmpty(_view.txtMacroName.Text);
        }
    }
}
