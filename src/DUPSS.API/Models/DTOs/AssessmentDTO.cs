namespace DUPSS.API.Models.DTOs
{
    public class AssessmentForTakingDTO
    {
        public int Id { get; set; } // Language ID
        public int AssessmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public List<QuestionDTO> Questions { get; set; } = new();
    }

    public class QuestionDTO
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "YesNo", "MultipleChoice", "Scale", "Text"
        public bool IsRequired { get; set; }
        public int OrderIndex { get; set; }
        public List<QuestionOptionDTO> Options { get; set; } = new();
    }

    public class QuestionOptionDTO
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
        public int OrderIndex { get; set; }
    }

    public class AssessmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssessmentVersionDTO
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssessmentLanguageDTO
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
    }

    public class AssessmentResponseDTO
    {
        public int LanguageId { get; set; }
        public string RespondentId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        public int? TotalScore { get; set; }
        public string? RiskLevel { get; set; }
        public List<QuestionResponseDTO> Responses { get; set; } = new();
    }

    public class QuestionResponseDTO
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public int? NumericResponse { get; set; }
        public string? TextResponse { get; set; }
    }
}