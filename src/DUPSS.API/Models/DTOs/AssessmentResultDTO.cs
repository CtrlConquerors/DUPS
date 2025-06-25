﻿using DUPSS.API.Models.Objects;
using System.ComponentModel.DataAnnotations;

namespace DUPSS.API.Models.DTOs
{
    public class AssessmentResultDTO
    {
        public required string ResultId { get; set; }
        public required string AssessmentId { get; set; }
        public required string MemberId { get; set; }
        public int? TotalScore { get; set; }
        public string? ScoreDetails { get; set; } // For complex results
        public string? Recommendation { get; set; }
        public DateOnly? CompletedOn { get; set; }

        public UserDTO? Member { get; set; }
        public AssessmentDTO? Assessment { get; set; }
    }
}
