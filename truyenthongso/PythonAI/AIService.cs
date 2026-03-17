using Microsoft.EntityFrameworkCore;
using truyenthongso.Models;

namespace truyenthongso.PythonAI
{
    public class AIService : IAIService
    {
        private readonly DBContext _context;
        public AIService(DBContext context)
        {
            _context = context;
        }
        public async Task<float> GetAi(AIInput data)
        {
            try { 
                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync("http://127.0.0.1:5001/predict", data);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("AI service failed");
                }

                var result = await response.Content.ReadFromJsonAsync<AIResult>();
                return result.score;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Lỗi xảy ra: {ex.Message}");
                return await Task.FromResult(-1f);
            }
        }

        public async Task<List<int>> GetTopPreferredCategoriesFromAI(int userId)
        {
            try
            {
                var allCategories = await _context.categories.ToListAsync();
                var timeOfDay = DateTime.Now.Hour;
                var results = new List<(int categoryId, float score)>();

                foreach (var cat in allCategories)
                {
                    var input = new AIInput
                    {
                        UserId = userId,
                        CategoryId = cat.id,
                        TimeOnPage = 10,
                        ScrolledToBottom = true,
                        Liked = false,
                        TimeOfDay = timeOfDay,
                        Device = "Mobile"
                    };

                    var score = await GetAi(input);
                    results.Add((cat.id, score));
                }

                return results
                    .OrderByDescending(x => x.score)
                    .Where(x => x.score > 0.6)
                    .Select(x => x.categoryId)
                    .Take(3)
                    .ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return await Task.FromResult(new List<int>()
                {
                    -1
                });
            }
        }
    }
}
