﻿using System.ComponentModel.DataAnnotations;

namespace DUPSS.API.Models.Objects
{
    public class CourseEnroll
    {
        [Key]
        public required string EnrollId { get; set; }
        [Required]
        public required string MemberId { get; set; }
        [Required]
        public required string CourseId { get; set; }
        [Required, MaxLength(50)]
        public required string Status { get; set; }
        [Required]
        public DateOnly EnrollDate { get; set; }
        public DateOnly? CompleteDate { get; set; }
        public User? Member { get; set; }
        public Course? Course { get; set; }
    }
}
