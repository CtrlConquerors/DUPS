using DUPSS.API.Models.DTOs;

namespace DUPSS.Web.Components.Service
{
    public interface IAssessmentService
    {
        Task<List<AssessmentDTO>> GetAssessmentsAsync();
        Task<AssessmentDTO?> GetAssessmentAsync(int id);
        Task<AssessmentForTakingDTO?> GetAssessmentForTakingAsync(int id, string languageCode = "eng");
        Task<AssessmentDTO> CreateAssessmentAsync(AssessmentDTO dto);
        Task UpdateAssessmentAsync(int id, AssessmentDTO dto);
        Task DeleteAssessmentAsync(int id);

        Task<List<AssessmentVersionDTO>> GetAssessmentVersionsAsync(int assessmentId);
        Task<AssessmentVersionDTO?> GetVersionAsync(int id);
        Task<AssessmentVersionDTO> CreateVersionAsync(AssessmentVersionDTO dto);
        Task UpdateVersionAsync(int id, AssessmentVersionDTO dto);
        Task DeleteVersionAsync(int id);

        Task<List<AssessmentLanguageDTO>> GetLanguagesForVersionAsync(int versionId);
        Task<AssessmentLanguageDTO?> GetAssessmentLanguageAsync(int id);
        Task<AssessmentLanguageDTO> CreateLanguageAsync(AssessmentLanguageDTO dto);
        Task UpdateLanguageAsync(int id, AssessmentLanguageDTO dto);
        Task DeleteLanguageAsync(int id);

        Task<List<QuestionDTO>> GetQuestionsForLanguageAsync(int languageId);
        Task<QuestionDTO> CreateQuestionAsync(QuestionDTO dto);
        Task UpdateQuestionAsync(int id, QuestionDTO dto);
        Task DeleteQuestionAsync(int id);

        Task<AssessmentResponseDTO> SubmitResponseAsync(AssessmentResponseDTO dto);
        Task<List<AssessmentResponseDTO>> GetResponsesAsync(int languageId);
        Task<AssessmentResponseDTO?> GetResponseAsync(int id);
        Task DeleteResponseAsync(int id);
    }

    public class AssessmentApiService : IAssessmentService
    {
        private readonly HttpClient _httpClient;

        public AssessmentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AssessmentDTO>> GetAssessmentsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<AssessmentDTO>>("api/assessments");
            return response ?? new List<AssessmentDTO>();
        }

        public async Task<AssessmentDTO?> GetAssessmentAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AssessmentDTO>($"api/assessments/{id}");
        }

        public async Task<List<AssessmentVersionDTO>> GetAssessmentVersionsAsync(int assessmentId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<AssessmentVersionDTO>>($"api/assessments/{assessmentId}/versions");
            return response ?? new List<AssessmentVersionDTO>();
        }

        public async Task<AssessmentForTakingDTO?> GetAssessmentForTakingAsync(int id, string languageCode = "eng")
        {
            return await _httpClient.GetFromJsonAsync<AssessmentForTakingDTO>($"api/assessments/{id}/for-taking?languageCode={languageCode}");
        }

        public async Task<AssessmentDTO> CreateAssessmentAsync(AssessmentDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assessments", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AssessmentDTO>() ?? new AssessmentDTO();
        }

        public async Task UpdateAssessmentAsync(int id, AssessmentDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/assessments/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAssessmentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/assessments/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<AssessmentVersionDTO?> GetVersionAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AssessmentVersionDTO>($"api/assessments/versions/{id}");
        }

        public async Task<AssessmentVersionDTO> CreateVersionAsync(AssessmentVersionDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assessments/versions", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AssessmentVersionDTO>() ?? new AssessmentVersionDTO();
        }

        public async Task UpdateVersionAsync(int id, AssessmentVersionDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/assessments/versions/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVersionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/assessments/versions/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<AssessmentLanguageDTO>> GetLanguagesForVersionAsync(int versionId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<AssessmentLanguageDTO>>($"api/assessments/versions/{versionId}/languages");
            return response ?? new List<AssessmentLanguageDTO>();
        }

        public async Task<AssessmentLanguageDTO?> GetAssessmentLanguageAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AssessmentLanguageDTO>($"api/assessments/languages/{id}");
        }

        public async Task<AssessmentLanguageDTO> CreateLanguageAsync(AssessmentLanguageDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assessments/languages", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AssessmentLanguageDTO>() ?? new AssessmentLanguageDTO();
        }

        public async Task UpdateLanguageAsync(int id, AssessmentLanguageDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/assessments/languages/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLanguageAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/assessments/languages/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<QuestionDTO>> GetQuestionsForLanguageAsync(int languageId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<QuestionDTO>>($"api/assessments/languages/{languageId}/questions");
            return response ?? new List<QuestionDTO>();
        }

        public async Task<QuestionDTO> CreateQuestionAsync(QuestionDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assessments/questions", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestionDTO>() ?? new QuestionDTO();
        }

        public async Task UpdateQuestionAsync(int id, QuestionDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/assessments/questions/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/assessments/questions/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<AssessmentResponseDTO> SubmitResponseAsync(AssessmentResponseDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assessments/responses", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AssessmentResponseDTO>() ?? new AssessmentResponseDTO();
        }

        public async Task<List<AssessmentResponseDTO>> GetResponsesAsync(int languageId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<AssessmentResponseDTO>>($"api/assessments/languages/{languageId}/responses");
            return response ?? new List<AssessmentResponseDTO>();
        }

        public async Task<AssessmentResponseDTO?> GetResponseAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AssessmentResponseDTO>($"api/assessments/responses/{id}");
        }

        public async Task DeleteResponseAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/assessments/responses/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}