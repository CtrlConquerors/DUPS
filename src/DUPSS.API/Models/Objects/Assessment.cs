﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUPSS.API.Models.Objects
{
    public class Assessment
    {
        [Key]
        public required string AssessmentId { get; set; }
        [Required, MaxLength(100)]
        public required string AssessmentType { get; set; }
        [Required, MaxLength(20)]
        public required string Version { get; set; } = "1.0";
        [Required, MaxLength(10)]
        public required string Language { get; set; } = "eng";
        public string? Description { get; set; }
        public List<AssessmentQuestion> Questions { get; set; } = new();
        public List<AssessmentResult> Results { get; set; } = new();
    }
}
