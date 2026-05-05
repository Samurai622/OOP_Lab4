using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task1
{
   public class DeviceEditViewModel : ViewModelBase
   {
       public DeviceModel Model { get; }
       public IEnumerable<MagnitudeType> MagnitudeTypes => Enum.GetValues(typeof(MagnitudeType)).Cast<MagnitudeType>();

       public bool IsSaved { get; private set; } = false;
       public bool HasChanges { get; set; } = true;

       // ДОДАНО: Текст помилки для відображення
       private string _errorMessage = string.Empty;
       public string ErrorMessage
       {
           get => _errorMessage;
           set => SetProperty(ref _errorMessage, value);
       }

       public ICommand SaveCommand { get; }
       public ICommand CancelCommand { get; }
       public Action CloseAction { get; set; }

       public DeviceEditViewModel(DeviceModel model)
       {
           Model = model;
          
           SaveCommand = new RelayCommand(_ => {
               // ПЕРЕВІРКА ПОМИЛОК
               string error = Model.GetValidationError();
               if (!string.IsNullOrEmpty(error))
               {
                   ErrorMessage = error; // Показуємо помилку на екрані
                   return;               // Зупиняємо збереження
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
