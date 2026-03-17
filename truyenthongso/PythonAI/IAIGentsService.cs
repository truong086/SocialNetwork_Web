using truyenthongso.Common;

namespace truyenthongso.PythonAI
{
    public interface IAIGentsService
    {
        Task<PayLoad<object>> AIGents(IFormFile file);
        Task<PayLoad<object>> AIGentsTiengTrung(string capdo, string type, string tu);
    }
}
