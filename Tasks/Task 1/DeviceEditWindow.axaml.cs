using Avalonia.Controls;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Tasks.Task1
{
    public partial class DeviceEditWindow : Window
    {
        private bool _forceClose = false;

        public DeviceEditWindow()
        {
            InitializeComponent();
        }

        public DeviceEditWindow(DeviceModel model) : this()
        {
            var vm = new DeviceEditViewModel(model);
            vm.CloseAction = () => 
            {
                _forceClose = true;
                Close(vm.IsSaved ? vm.Model : null);
            };
            DataContext = vm;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (DataContext is DeviceEditViewModel vm && vm.HasChanges && !_forceClose)
            {
                e.Cancel = true; 
                
                if (vm.Model.IsValid())
                {
                    vm.SaveCommand.Execute(null);
                }
                else
                {
                    vm.CancelCommand.Execute(null);
                }
            }
            base.OnClosing(e);
        }
    }
}