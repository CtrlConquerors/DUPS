using DUPSS.API.Models.AccessLayer;
using DUPSS.API.Models.DTOs;
using DUPSS.API.Models.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DUPSS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssessmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/assessments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssessmentDTO>>> GetAssessments()
        {
            var assessments = await _context.Assessments
                .Select(a => new AssessmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Type = a.Type,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(assessments);
        }

        // GET: api/assessments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDTO>> GetAssessment(int id)
        {
            var assessment = await _context.Assessments
                .Where(a => a.Id == id)
                .Select(a => new AssessmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Type = a.Type,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (assessment == null)
            {
                return NotFound();
            }

            return Ok(assessment);
        }

        // GET: api/assessments/{id}/versions
        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<AssessmentVersionDTO>>> GetAssessmentVersions(int id)
        {
            var versions = await _context.AssessmentVersions
                .Where(v => v.AssessmentId == id)
                .Select(v => new AssessmentVersionDTO
                {
                    Id = v.Id,
                    AssessmentId = v.AssessmentId,
                    Version = v.Version,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync();

            return Ok(versions);
        }

        // GET: api/assessments/versions/{versionId}/languages
        [HttpGet("versions/{versionId}/languages")]
        public async Task<ActionResult<IEnumerable<AssessmentLanguageDTO>>> GetLanguagesForVersion(int versionId)
        {
            var languages = await _context.AssessmentLanguages
                .Where(l => l.VersionId == versionId)
                .Select(l => new AssessmentLanguageDTO
                {
                    Id = l.Id,
                    VersionId = l.VersionId,
                    LanguageCode = l.LanguageCode,
                    LanguageName = l.LanguageName
                })
                .ToListAsync();

            return Ok(languages);
        }

        // GET: api/assessments/{id}/for-taking
        [HttpGet("{id}/for-taking")]
        public async Task<ActionResult<AssessmentForTakingDTO>> GetAssessmentForTaking(int id, [FromQuery] string languageCode = "eng")
        {
            var language = await _context.AssessmentLanguages
                .Include(l => l.Version)
                .ThenInclude(v => v.Assessment)
                .Include(l => l.Questions)
                .ThenInclude(q => q.Options)
                .Where(l => l.Id == id && l.IsActive && l.Version.IsActive && l.Version.Assessment.IsActive && l.LanguageCode == languageCode)
                .FirstOrDefaultAsync();

            if (language == null)
            {
                language = await _context.AssessmentLanguages
                    .Include(l => l.Version)
                    .ThenInclude(v => v.Assessment)
                    .Include(l => l.Questions)
                    .ThenInclude(q => q.Options)
                    .Where(l => l.Id == id && l.IsActive && l.Version.IsActive && l.Version.Assessment.IsActive)
                    .FirstOrDefaultAsync();

                if (language == null)
                {
                    return NotFound("Assessment language not found.");
                }
            }

            var assessmentForTaking = new AssessmentForTakingDTO
            {
                Id = language.Id,
                AssessmentId = language.Version.AssessmentId,
                Name = language.Version.Assessment.Name,
                Description = language.Version.Assessment.Description,
                Type = language.Version.Assessment.Type,
                Version = language.Version.Version,
                LanguageCode = language.LanguageCode,
                LanguageName = language.LanguageName,
                Questions = language.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        LanguageId = q.LanguageId,
                        Text = q.Text,
                        Type = q.Type,
                        IsRequired = q.IsRequired,
                        OrderIndex = q.OrderIndex,
                        Options = q.Options
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new QuestionOptionDTO
                            {
                                Id = o.Id,
                                Text = o.Text,
                                Value = o.Value,
                                OrderIndex = o.OrderIndex
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return Ok(assessmentForTaking);
        }

        // GET: api/assessments/languages/{languageId}/questions
        [HttpGet("languages/{languageId}/questions")]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestionsForLanguage(int languageId)
        {
            var language = await _context.AssessmentLanguages
                .Where(l => l.Id == languageId && l.IsActive)
                .FirstOrDefaultAsync();

            if (language == null)
            {
                return NotFound("Assessment language not found.");
            }

            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.LanguageId == languageId)
                .OrderBy(q => q.OrderIndex)
                .Select(q => new QuestionDTO
                {
                    Id = q.Id,
                    LanguageId = q.LanguageId,
                    Text = q.Text,
                    Type = q.Type,
                    IsRequired = q.IsRequired,
                    OrderIndex = q.OrderIndex,
                    Options = q.Options
                        .OrderBy(o => o.OrderIndex)
                        .Select(o => new QuestionOptionDTO
                        {
                            Id = o.Id,
                            Text = o.Text,
                            Value = o.Value,
                            OrderIndex = o.OrderIndex
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(questions);
        }

        // POST: api/assessments
        [HttpPost]
        public async Task<ActionResult<AssessmentDTO>> CreateAssessment(AssessmentDTO assessmentDTO)
        {
            if (string.IsNullOrWhiteSpace(assessmentDTO.Name) || string.IsNullOrWhiteSpace(assessmentDTO.Type))
            {
                return BadRequest("Name and Type are required.");
            }

            var assessment = new Assessment
            {
                Name = assessmentDTO.Name,
                Description = assessmentDTO.Description,
                Type = assessmentDTO.Type,
                IsActive = assessmentDTO.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            assessmentDTO.Id = assessment.Id;
            assessmentDTO.CreatedAt = assessment.CreatedAt;

            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.Id }, assessmentDTO);
        }

        // PUT: api/assessments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssessment(int id, AssessmentDTO assessmentDTO)
        {
            if (id != assessmentDTO.Id)
            {
                return BadRequest("Assessment ID mismatch.");
            }

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }

            assessment.Name = assessmentDTO.Name;
            assessment.Description = assessmentDTO.Description;
            assessment.Type = assessmentDTO.Type;
            assessment.IsActive = assessmentDTO.IsActive;

            _context.Entry(assessment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/assessments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/assessments/versions/{id}
        [HttpGet("versions/{id}")]
        public async Task<ActionResult<AssessmentVersionDTO>> GetVersion(int id)
        {
            var version = await _context.AssessmentVersions
                .Where(v => v.Id == id)
                .Select(v => new AssessmentVersionDTO
                {
                    Id = v.Id,
                    AssessmentId = v.AssessmentId,
                    Version = v.Version,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (version == null)
            {
                return NotFound();
            }

            return Ok(version);
        }

        // POST: api/assessments/versions
        [HttpPost("versions")]
        public async Task<ActionResult<AssessmentVersionDTO>> CreateVersion(AssessmentVersionDTO versionDTO)
        {
            var assessment = await _context.Assessments.FindAsync(versionDTO.AssessmentId);
            if (assessment == null)
            {
                return NotFound("Assessment not found.");
            }

            var version = new AssessmentVersion
            {
                AssessmentId = versionDTO.AssessmentId,
                Version = versionDTO.Version,
                IsActive = versionDTO.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.AssessmentVersions.Add(version);
            await _context.SaveChangesAsync();

            versionDTO.Id = version.Id;
            versionDTO.CreatedAt = version.CreatedAt;

            return CreatedAtAction(nameof(GetVersion), new { id = version.Id }, versionDTO);
        }

        // PUT: api/assessments/versions/{id}
        [HttpPut("versions/{id}")]
        public async Task<IActionResult> UpdateVersion(int id, AssessmentVersionDTO versionDTO)
        {
            if (id != versionDTO.Id)
            {
                return BadRequest("Version ID mismatch.");
            }

            var version = await _context.AssessmentVersions.FindAsync(id);
            if (version == null)
            {
                return NotFound();
            }

            version.Version = versionDTO.Version;
            version.IsActive = versionDTO.IsActive;

            _context.Entry(version).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/assessments/versions/{id}
        [HttpDelete("versions/{id}")]
        public async Task<IActionResult> DeleteVersion(int id)
        {
            var version = await _context.AssessmentVersions.FindAsync(id);
            if (version == null)
            {
                return NotFound();
            }

            _context.AssessmentVersions.Remove(version);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/assessments/languages/{id}
        [HttpGet("languages/{id}")]
        public async Task<ActionResult<AssessmentLanguageDTO>> GetAssessmentLanguage(int id)
        {
            var language = await _context.AssessmentLanguages
                .Where(l => l.Id == id)
                .Select(l => new AssessmentLanguageDTO
                {
                    Id = l.Id,
                    VersionId = l.VersionId,
                    LanguageCode = l.LanguageCode,
                    LanguageName = l.LanguageName
                })
                .FirstOrDefaultAsync();

            if (language == null)
            {
                return NotFound();
            }

            return Ok(language);
        }

        // POST: api/assessments/languages
        [HttpPost("languages")]
        public async Task<ActionResult<AssessmentLanguageDTO>> CreateLanguage(AssessmentLanguageDTO languageDTO)
        {
            var version = await _context.AssessmentVersions.FindAsync(languageDTO.VersionId);
            if (version == null)
            {
                return NotFound("Version not found.");
            }

            var language = new AssessmentLanguage
            {
                VersionId = languageDTO.VersionId,
                LanguageCode = languageDTO.LanguageCode,
                LanguageName = languageDTO.LanguageName,
                IsActive = true
            };

            _context.AssessmentLanguages.Add(language);
            await _context.SaveChangesAsync();

            languageDTO.Id = language.Id;

            return CreatedAtAction(nameof(GetAssessmentLanguage), new { id = language.Id }, languageDTO);
        }

        // PUT: api/assessments/languages/{id}
        [HttpPut("languages/{id}")]
        public async Task<IActionResult> UpdateLanguage(int id, AssessmentLanguageDTO languageDTO)
        {
            if (id != languageDTO.Id)
            {
                return BadRequest("Language ID mismatch.");
            }

            var language = await _context.AssessmentLanguages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            language.LanguageCode = languageDTO.LanguageCode;
            language.LanguageName = languageDTO.LanguageName;

            _context.Entry(language).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/assessments/languages/{id}
        [HttpDelete("languages/{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _context.AssessmentLanguages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            _context.AssessmentLanguages.Remove(language);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/assessments/questions
        [HttpPost("questions")]
        public async Task<ActionResult<QuestionDTO>> CreateQuestion(QuestionDTO questionDTO)
        {
            var language = await _context.AssessmentLanguages.FindAsync(questionDTO.LanguageId);
            if (language == null)
            {
                return NotFound("Language not found.");
            }

            var question = new Question
            {
                LanguageId = questionDTO.LanguageId,
                Text = questionDTO.Text,
                Type = questionDTO.Type,
                IsRequired = questionDTO.IsRequired,
                OrderIndex = questionDTO.OrderIndex,
                Options = questionDTO.Options.Select(o => new QuestionOption
                {
                    Text = o.Text,
                    Value = o.Value,
                    OrderIndex = o.OrderIndex
                }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            questionDTO.Id = question.Id;
            for (int i = 0; i < questionDTO.Options.Count; i++)
            {
                questionDTO.Options[i].Id = question.Options[i].Id;
            }

            return CreatedAtAction(nameof(GetQuestionsForLanguage), new { languageId = question.LanguageId }, questionDTO);
        }

        // PUT: api/assessments/questions/{id}
        [HttpPut("questions/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, QuestionDTO questionDTO)
        {
            if (id != questionDTO.Id)
            {
                return BadRequest("Question ID mismatch.");
            }

            var question = await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            question.Text = questionDTO.Text;
            question.Type = questionDTO.Type;
            question.IsRequired = questionDTO.IsRequired;
            question.OrderIndex = questionDTO.OrderIndex;

            _context.QuestionOptions.RemoveRange(question.Options);
            question.Options = questionDTO.Options.Select(o => new QuestionOption
            {
                QuestionId = id,
                Text = o.Text,
                Value = o.Value,
                OrderIndex = o.OrderIndex
            }).ToList();

            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/assessments/questions/{id}
        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/assessments/responses
        [HttpPost("responses")]
        public async Task<ActionResult<AssessmentResponseDTO>> SubmitResponse(AssessmentResponseDTO responseDTO)
        {
            try
            {
                // Validate LanguageId
                var language = await _context.AssessmentLanguages
                    .Include(l => l.Version)
                    .ThenInclude(v => v.Assessment)
                    .FirstOrDefaultAsync(l => l.Id == responseDTO.LanguageId && l.IsActive);

                if (language == null)
                {
                    return NotFound("Assessment language not found or inactive.");
                }

                if (language.Version == null || language.Version.Assessment == null)
                {
                    return BadRequest("Invalid assessment version or assessment data.");
                }

                // Validate QuestionIds and SelectedOptionIds
                var questionIds = responseDTO.Responses.Select(r => r.QuestionId).ToList();
                var validQuestionIds = await _context.Questions
                    .Where(q => q.LanguageId == responseDTO.LanguageId)
                    .Select(q => q.Id)
                    .ToListAsync();

                var invalidQuestionIds = questionIds.Except(validQuestionIds).ToList();
                if (invalidQuestionIds.Any())
                {
                    return BadRequest($"Invalid Question IDs: {string.Join(", ", invalidQuestionIds)}");
                }

                foreach (var response in responseDTO.Responses)
                {
                    if (response.SelectedOptionId.HasValue)
                    {
                        var optionExists = await _context.QuestionOptions
                            .AnyAsync(o => o.Id == response.SelectedOptionId && o.QuestionId == response.QuestionId);
                        if (!optionExists)
                        {
                            return BadRequest($"Invalid SelectedOptionId {response.SelectedOptionId} for QuestionId {response.QuestionId}");
                        }
                    }
                }

                var assessmentResponse = new AssessmentResponse
                {
                    LanguageId = responseDTO.LanguageId,
                    RespondentId = responseDTO.RespondentId,
                    StartedAt = responseDTO.StartedAt,
                    CompletedAt = responseDTO.CompletedAt,
                    IsCompleted = responseDTO.IsCompleted,
                    Responses = responseDTO.Responses.Select(r => new QuestionResponse
                    {
                        QuestionId = r.QuestionId,
                        TextResponse = r.TextResponse,
                        NumericResponse = r.NumericResponse,
                        SelectedOptionId = r.SelectedOptionId
                    }).ToList()
                };

                int totalScore = 0;
                foreach (var response in assessmentResponse.Responses)
                {
                    if (response.NumericResponse.HasValue)
                    {
                        totalScore += response.NumericResponse.Value;
                    }
                }
                assessmentResponse.TotalScore = totalScore;
                assessmentResponse.RiskLevel = CalculateRiskLevel(totalScore, language.Version.Assessment.Type);

                _context.AssessmentResponses.Add(assessmentResponse);
                await _context.SaveChangesAsync();

                responseDTO.TotalScore = assessmentResponse.TotalScore;
                responseDTO.RiskLevel = assessmentResponse.RiskLevel;
                responseDTO.Responses.ForEach(r => 
                {
                    var savedResponse = assessmentResponse.Responses.FirstOrDefault(qr => qr.QuestionId == r.QuestionId);
                    r.QuestionId = savedResponse?.Id ?? 0;
                });

                return Ok(responseDTO);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/assessments/languages/{languageId}/responses
        [HttpGet("languages/{languageId}/responses")]
        public async Task<ActionResult<IEnumerable<AssessmentResponseDTO>>> GetResponses(int languageId)
        {
            var language = await _context.AssessmentLanguages.FindAsync(languageId);
            if (language == null)
            {
                return NotFound("Assessment language not found.");
            }

            var responses = await _context.AssessmentResponses
                .Include(r => r.Responses)
                .Where(r => r.LanguageId == languageId)
                .Select(r => new AssessmentResponseDTO
                {
                    LanguageId = r.LanguageId,
                    RespondentId = r.RespondentId,
                    StartedAt = r.StartedAt,
                    CompletedAt = r.CompletedAt,
                    IsCompleted = r.IsCompleted,
                    TotalScore = r.TotalScore,
                    RiskLevel = r.RiskLevel,
                    Responses = r.Responses.Select(qr => new QuestionResponseDTO
                    {
                        QuestionId = qr.QuestionId,
                        TextResponse = qr.TextResponse,
                        NumericResponse = qr.NumericResponse,
                        SelectedOptionId = qr.SelectedOptionId
                    }).ToList()
                })
                .ToListAsync();

            return Ok(responses);
        }

        // GET: api/assessments/responses/{id}
        [HttpGet("responses/{id}")]
        public async Task<ActionResult<AssessmentResponseDTO>> GetResponse(int id)
        {
            var response = await _context.AssessmentResponses
                .Include(r => r.Responses)
                .Where(r => r.Id == id)
                .Select(r => new AssessmentResponseDTO
                {
                    LanguageId = r.LanguageId,
                    RespondentId = r.RespondentId,
                    StartedAt = r.StartedAt,
                    CompletedAt = r.CompletedAt,
                    IsCompleted = r.IsCompleted,
                    TotalScore = r.TotalScore,
                    RiskLevel = r.RiskLevel,
                    Responses = r.Responses.Select(qr => new QuestionResponseDTO
                    {
                        QuestionId = qr.QuestionId,
                        TextResponse = qr.TextResponse,
                        NumericResponse = qr.NumericResponse,
                        SelectedOptionId = qr.SelectedOptionId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        // DELETE: api/assessments/responses/{id}
        [HttpDelete("responses/{id}")]
        public async Task<IActionResult> DeleteResponse(int id)
        {
            var response = await _context.AssessmentResponses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            _context.AssessmentResponses.Remove(response);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string CalculateRiskLevel(int totalScore, string assessmentType)
        {
            if (assessmentType == "CRAFFT")
            {
                if (totalScore >= 4) return "High";
                if (totalScore >= 2) return "Medium";
                return "Low";
            }
            else if (assessmentType == "ASSIST")
            {
                if (totalScore >= 27) return "High";
                if (totalScore >= 11) return "Medium";
                return "Low";
            }
            return "Unknown";
        }
    }
}