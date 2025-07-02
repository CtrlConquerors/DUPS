using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DUPSS.API.Models.Objects
{
    public class Assessment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // ASSIST, CRAFFT, etc.
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public List<AssessmentVersion> Versions { get; set; } = new();
    }

    public class AssessmentVersion
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public Assessment? Assessment { get; set; }
        public List<AssessmentLanguage> Languages { get; set; } = new();
    }

    public class AssessmentLanguage
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string LanguageCode { get; set; } = string.Empty; // en, es, fr, etc.
        public string LanguageName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public AssessmentVersion? Version { get; set; }
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int OrderIndex { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // MultipleChoice, YesNo, Scale, Text
        public bool IsRequired { get; set; } = true;
        public AssessmentLanguage? Language { get; set; }
        public List<QuestionOption> Options { get; set; } = new();
    }

    public class QuestionOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
        public int OrderIndex { get; set; }
        public Question? Question { get; set; }
    }

    public class AssessmentResponse
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string RespondentId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        public int? TotalScore { get; set; }
        public string? RiskLevel { get; set; }
        public AssessmentLanguage? Language { get; set; }
        public List<QuestionResponse> Responses { get; set; } = new();
    }

    public class QuestionResponse
    {
        public int Id { get; set; }
        public int AssessmentResponseId { get; set; }
        public int QuestionId { get; set; }
        public string? TextResponse { get; set; }
        public int? NumericResponse { get; set; }
        public int? SelectedOptionId { get; set; }
        public AssessmentResponse? AssessmentResponse { get; set; }
        public Question? Question { get; set; }
        public QuestionOption? SelectedOption { get; set; }
    }
}
