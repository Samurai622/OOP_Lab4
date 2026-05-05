using Avalonia.Controls;
using OOP_Lab4.Models;

namespace OOP_Lab4.Tasks.Task2
{
    public partial class PerformanceEditWindow : Window
    {
        private bool _forceClose = false;
        public PerformanceEditWindow() { InitializeComponent(); }
        public PerformanceEditWindow(PerformanceModel model) : this()
        {
            var vm = new PerformanceEditViewModel(model);
            vm.CloseAction = () => { _forceClose = true; Close(vm.IsSaved ? vm.Model : null); };
            DataContext = vm;
        }
        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (DataContext is PerformanceEditViewModel vm && vm.HasChanges && !_forceClose)
            {
                e.Cancel = true; 
                if (string.IsNullOrEmpty(vm.Model.GetValidationError())) vm.SaveCommand.Execute(null);
                else vm.CancelCommand.Execute(null);
            }
            base.OnClosing(e);
        }
    }
}