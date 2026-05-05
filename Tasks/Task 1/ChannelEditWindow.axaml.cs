using Avalonia.Controls;
using OOP_Lab4.Models;

namespace OOP_Lab4.Tasks.Task1
{
   public partial class ChannelEditWindow : Window
   {
       private bool _forceClose = false;

       public ChannelEditWindow() { InitializeComponent(); }

       public ChannelEditWindow(ChannelModel model) : this()
       {
           var vm = new ChannelEditViewModel(model);
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
              
               // Якщо помилок немає (рядок порожній) — зберігаємо
               if (string.IsNullOrEmpty(vm.Model.GetValidationError()))
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

