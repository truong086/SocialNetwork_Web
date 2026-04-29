using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.PythonAI
{
    public interface IAIService
    {
        Task<float> GetAi(AIInput data);
        Task<float> GetAi(TraningModel data);
        Task<List<int>> GetTopPreferredCategoriesFromAI(int userId);
    }
}
