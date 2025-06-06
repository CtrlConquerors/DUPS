﻿using DUPSS.API.Models.AccessLayer.Interfaces;
using DUPSS.API.Models.DTOs;
using DUPSS.API.Models.Objects;
using Microsoft.EntityFrameworkCore;

namespace DUPSS.API.Models.AccessLayer.DAOs
{
    public class AssessmentDAO : IAssessmentDAO
    {
        private readonly AppDbContext _context;

        public AssessmentDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AssessmentDTO> CreateAsync(Assessment assessment)
        {
            _context.Assessment.Add(assessment);
            await _context.SaveChangesAsync();
            return new AssessmentDTO
            {
                AssessmentId = assessment.AssessmentId,
                AssessmentType = assessment.AssessmentType,
                Description = assessment.Description
            };
        }

        public async Task<AssessmentDTO?> GetByIdAsync(string assessmentId)
        {
            return await _context.Assessment
                .Where(a => a.AssessmentId == assessmentId)
                .Select(a => new AssessmentDTO
                {
                    AssessmentId = a.AssessmentId,
                    AssessmentType = a.AssessmentType,
                    Description = a.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<AssessmentDTO>> GetAllAsync()
        {
            return await _context.Assessment
                .Select(a => new AssessmentDTO
                {
                    AssessmentId = a.AssessmentId,
                    AssessmentType = a.AssessmentType,
                    Description = a.Description
                })
                .ToListAsync();
        }

        public async Task<AssessmentDTO> UpdateAsync(Assessment assessment)
        {
            var existingAssessment = await _context.Assessment.FindAsync(assessment.AssessmentId);
            if (existingAssessment == null)
                throw new Exception($"Assessment with ID {assessment.AssessmentId} not found.");

            existingAssessment.AssessmentType = assessment.AssessmentType;
            existingAssessment.Description = assessment.Description;

            await _context.SaveChangesAsync();
            return new AssessmentDTO
            {
                AssessmentId = existingAssessment.AssessmentId,
                AssessmentType = existingAssessment.AssessmentType,
                Description = existingAssessment.Description
            };
        }

        public async Task<bool> DeleteAsync(string assessmentId)
        {
            var assessment = await _context.Assessment.FindAsync(assessmentId);
            if (assessment == null)
                return false;

            _context.Assessment.Remove(assessment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}