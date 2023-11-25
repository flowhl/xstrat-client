using System;
using System.Windows;
using System.Windows.Controls;
using xstrat.Core;

namespace xstrat
{
    public class StateUserControl : UserControl
    {
        public bool HasChanges { get; set; } = false;

        public bool AllowExit()
        {
            var res = CheckAllowExit();
            if (res == AllowExitResult.SaveAndExit)
            {
                Save();
                return true;
            }
            if (res == AllowExitResult.Cancel)
            {
                return false;
            }
            return true;
        }

        public virtual AllowExitResult CheckAllowExit()
        {
            if (!HasChanges) return AllowExitResult.ExitWithoutSave;
            var res = MessageBox.Show("You have unsaved changes, do you want to save first?", "Exit?", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes) return AllowExitResult.SaveAndExit;
            if (res == MessageBoxResult.No) return AllowExitResult.ExitWithoutSave;
            return AllowExitResult.Cancel;
        }

        public StateUserControl()
        {
            if (Globals.wnd != null)
            {
                Globals.wnd.Closing += MainWindow_Closing;
                Globals.wnd.CurrentView = this;
            }

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = CheckAllowExit();
            if (res == AllowExitResult.SaveAndExit)
            {
                Save();
            }
            else if (res == AllowExitResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        public void Dispose()
        {
            Globals.wnd.Closing -= MainWindow_Closing;
        }

        public virtual void Save(bool silent = false)
        {
            HasChanges = false;
        }
    }

    public enum AllowExitResult
    {
        SaveAndExit,
        ExitWithoutSave,
        Cancel
    }
}
