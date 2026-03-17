using truyenthongso.Common;

namespace truyenthongso.PythonAI
{
    public interface IAIService
    {
        Task<float> GetAi(AIInput data);
        Task<List<int>> GetTopPreferredCategoriesFromAI(int userId);
    }
}
