using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OOP_Lab4.Models
{
    public class ParticipantDto
    {
        [JsonPropertyName("id")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("birthDate")] public string BirthDate { get; set; } = string.Empty;
    }

    public class PerformanceDto
    {
        [JsonPropertyName("id")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        
        [JsonPropertyName("isTeam")] public bool IsTeam { get; set; }
        [JsonPropertyName("resultScore")] public double ResultScore { get; set; }
        
        [JsonPropertyName("CompetitionId")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CompetitionId { get; set; }
        
        [JsonPropertyName("ParticipantId")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ParticipantId { get; set; }
        
        [JsonPropertyName("Participant")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ParticipantDto? Participant { get; set; }
    }

    public class CompetitionDto
    {
        [JsonPropertyName("id")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        
        // Не відправляємо порожні поля
        [JsonPropertyName("shortInfo")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ShortInfo { get; set; }
        
        [JsonPropertyName("performances")] 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PerformanceDto>? Performances { get; set; }
    }

    public class ParticipantModel
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTimeOffset _birthDate = DateTimeOffset.Now.AddYears(-18);

        public string FirstName { get => _firstName; set => _firstName = value; }
        public string LastName { get => _lastName; set => _lastName = value; }
        public DateTimeOffset BirthDate { get => _birthDate; set => _birthDate = value; }

        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return "Ім'я та прізвище не можуть бути порожніми.";
            if (BirthDate > DateTimeOffset.Now)
                return "Дата народження не може бути в майбутньому.";
            return string.Empty;
        }
    }

    public class PerformanceModel
    {
        public int Id { get; set; }

        private ParticipantModel _participant = new();
        private bool _isTeam;
        private double _resultScore;

        private static int _totalPerformances = 0;
        private int _sequenceNumber;

        public PerformanceModel()
        {
            _totalPerformances++;
            _sequenceNumber = _totalPerformances;
        }

        public PerformanceModel(int existingSeqNumber)
        {
            _sequenceNumber = existingSeqNumber;
        }

        public ParticipantModel Participant { get => _participant; set => _participant = value; }
        public bool IsTeam { get => _isTeam; set => _isTeam = value; }
        public double ResultScore { get => _resultScore; set => _resultScore = value; }
        public int SequenceNumber => _sequenceNumber;

        public static void SetTotalCount(int count) => _totalPerformances = count;

        public string GetValidationError()
        {
            if (ResultScore < 0) return "Результат (бали) не може бути від'ємним.";
            return Participant.GetValidationError();
        }
    }

    public class CompetitionModel
    {
        public int Id { get; set; }
        private string _name = string.Empty;
        public string Name { get => _name; set => _name = value; }

        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(Name)) return "Назва змагання не може бути порожньою.";
            return string.Empty;
        }

        public static string ToShortString(CompetitionDto dto)
        {
            string winnerName = "Немає виступів";
            if (dto.Performances != null && dto.Performances.Count > 0)
            {
                var bestPerformance = dto.Performances.OrderByDescending(p => p.ResultScore).First();
                winnerName = bestPerformance.Participant != null ? bestPerformance.Participant.LastName : "Невідомий";
            }
            return $"{dto.Name} | Переможець: {winnerName}";
        }
    }
}