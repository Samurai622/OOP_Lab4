using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Tasks.Task1
{
    public partial class Task1View : UserControl
    {
        public Task1View()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is Task1ViewModel vm)
            {
                vm.OpenEditDialogAsync = OpenDialog;
            }
        }

        private async Task<DeviceModel> OpenDialog(DeviceModel model)
        {
            var dialog = new DeviceEditWindow(model);
            
            if (this.GetVisualRoot() is Window mainWindow)
            {
                var result = await dialog.ShowDialog<DeviceModel>(mainWindow);
                return result;
            }
            return null;
        }
    }
}