using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OOP_Lab4.Models
{
   public enum MagnitudeType
   {
       Mm = 0,             // мм
       DegreeAngle = 1,    // градус (кут)
       DegreeTemp = 2,     // градус (температура)
       Second = 3,         // секунда
       Percent = 4         // відсоток
   }

   public class SensorDto
   {
       [JsonPropertyName("id")]
       [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // Не відправляти ID, якщо він 0
       public int Id { get; set; }
      
       [JsonPropertyName("magnitudeType")] public int MagnitudeType { get; set; }
       [JsonPropertyName("minRange")] public double MinRange { get; set; }
       [JsonPropertyName("maxRange")] public double MaxRange { get; set; }
       [JsonPropertyName("currentValue")] public double CurrentValue { get; set; }

       [JsonIgnore]
       public string MagnitudeTypeName
        {
            get
            {
                return MagnitudeType switch
                {
                    0 => "мм",
                    1 => "градус (кут)",
                    2 => "градус (температура)",
                    3 => "секунда",
                    4 => "відсоток",
                    _ => "невідомо"
                };
            }
        }
   }

   public class DeviceDto
   {
       [JsonPropertyName("id")]
       [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
       public int Id { get; set; }
      
       [JsonPropertyName("locationNumber")] public int LocationNumber { get; set; }
       [JsonPropertyName("calibrationDate")] public string CalibrationDate { get; set; }
      
       [JsonPropertyName("ChannelId")] public int? ChannelId { get; set; }
       [JsonPropertyName("SensorId")] public int? SensorId { get; set; }
      
       [JsonPropertyName("Sensor")] public SensorDto? Sensor { get; set; }
   }

   public class ChannelDto
   {
       [JsonPropertyName("id")]
       [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
       public int Id { get; set; }
      
       [JsonPropertyName("name")] public string Name { get; set; }
       [JsonPropertyName("devicesCount")] public int DevicesCount { get; set; }
       [JsonPropertyName("shortInfo")] public string ShortInfo { get; set; }
       [JsonPropertyName("devices")] public List<DeviceDto> Devices { get; set; }
   }

   // Клас для роботи у самій програмі Avalonia (з валідацією)
   // ========================================================
   // «не-GUI» КЛАСИ ЗА ВАРІАНТОМ 2 (ІЗ ПРИВАТНИМИ ПОЛЯМИ)
   // ========================================================

   public class SensorModel
   {
       // ЗАКРИТІ ПОЛЯ (за вимогами варіанту)
       private MagnitudeType _magnitudeType = MagnitudeType.Mm;
       private double _minRange;
       private double _maxRange;
       private double _currentValue;

       // Властивості для XAML та валідації
       public MagnitudeType MagType
       {
           get => _magnitudeType;
           set => _magnitudeType = value;
       }
      
       [Required]
       public double MinRange
       {
           get => _minRange;
           set => _minRange = value;
       }
      
       [Required]
       public double MaxRange
       {
           get => _maxRange;
           set => _maxRange = value;
       }
      
       [Required]
       public double CurrentValue
       {
           get => _currentValue;
           set => _currentValue = value;
       }

        public string GetValidationError()
       {
           if (MinRange >= MaxRange)
               return "Мінімальна межа має бути строго меншою за максимальну.";
              
           if (CurrentValue < MinRange || CurrentValue > MaxRange)
               return $"Поточне значення має бути в проміжку від {MinRange} до {MaxRange}.";
              
           return string.Empty; // Помилок немає
       }
   }

   public class DeviceModel
   {
       public int Id { get; set; } // ID для БД 
       // ЗАКРИТІ ПОЛЯ 
       private SensorModel _sensor;         // ПОЛЕ ТИПУ ДАТЧИК
       private int _locationNumber;         // Ціле типу з номером місця кріплення
       private DateTimeOffset _calibrationDate; // Типу Date з датою калібрування

       public DeviceModel()
       {
           _sensor = new SensorModel();
           _calibrationDate = DateTimeOffset.Now;
       }

       // Властивості-обгортки для XAML
       public SensorModel Sensor
       {
           get => _sensor;
           set => _sensor = value;
       }

       [Required]
       [Range(1, 1000, ErrorMessage = "Номер місця має бути від 1 до 1000")]
       public int LocationNumber
       {
           get => _locationNumber;
           set => _locationNumber = value;
       }
      
       [Required]
       public DateTimeOffset CalibrationDate
       {
           get => _calibrationDate;
           set => _calibrationDate = value;
       }

       public string GetValidationError()
       {
            if (LocationNumber <= 0)
                return "Номер місця кріплення має бути додатнім числом (більше 0).";
            if (CalibrationDate > DateTimeOffset.Now)
                return "Дата калібрування не може бути в майбутньому.";
              
           // Перевіряємо також вкладений датчик
           return Sensor.GetValidationError();
       }
   }

   public class ChannelModel
   {
       public int Id { get; set; } // Для БД

       // статичне поле з загальною кількістю
       private static int _totalChannelsCreated = 0;

       // закрите поле з порядковим номером
       private int _orderNumber;

       private string _name;

       // Конструктор 1: Викликається при створенні НОВОГО каналу (Збільшує лічильник)
       public ChannelModel()
       {
           _totalChannelsCreated++;
           _orderNumber = _totalChannelsCreated;
       }

       // Конструктор 2: Викликається при РЕДАГУВАННІ (НЕ збільшує лічильник)
       public ChannelModel(int existingOrderNumber)
       {
           _orderNumber = existingOrderNumber;
       }

       public string Name
       {
           get => _name;
           set => _name = value;
       }

       public int OrderNumber => _orderNumber;

       // Встановлення лічильника (для синхронізації з БД)
       public static void SetTotalCount(int count)
       {
           _totalChannelsCreated = count;
       }

       public string GetValidationError()
       {
           if (string.IsNullOrWhiteSpace(Name))
               return "Назва каналу не може бути порожньою.";
              
           return string.Empty; // Помилок немає
       }
   }
}
