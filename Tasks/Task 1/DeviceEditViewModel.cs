using System;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task1
{
    public class DeviceEditViewModel : ViewModelBase
    {
        public DeviceModel Model { get; }
        
        public bool IsSaved { get; private set; } = false;
        public bool HasChanges { get; set; } = true;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public DeviceEditViewModel(DeviceModel model)
        {
            Model = model;
            
            SaveCommand = new RelayCommand(_ => {
                if (!Model.IsValid()) return; 
                IsSaved = true;
                HasChanges = false;
                CloseAction?.Invoke();
            });

            CancelCommand = new RelayCommand(_ => {
                HasChanges = false;
                CloseAction?.Invoke();
            });
        }
    }
}