using System;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task1
{
   public class ChannelEditViewModel : ViewModelBase
   {
       public ChannelModel Model { get; }
      
       public bool IsSaved { get; private set; } = false;
       public bool HasChanges { get; set; } = true;

       public ICommand SaveCommand { get; }
       public ICommand CancelCommand { get; }

       public Action CloseAction { get; set; }

       private string _errorMessage = string.Empty;
       public string ErrorMessage
       {
           get => _errorMessage;
           set => SetProperty(ref _errorMessage, value);
       }

       public ChannelEditViewModel(ChannelModel model)
       {
           Model = model;
          
           SaveCommand = new RelayCommand(_ => {
               string error = Model.GetValidationError();
               if (!string.IsNullOrEmpty(error))
               {
                   ErrorMessage = error;
                   return;
               }
              
               ErrorMessage = string.Empty;
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