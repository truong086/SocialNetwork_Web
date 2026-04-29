using truyenthongso.Common;
using truyenthongso.PythonAI;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ITraningModelService
    {
        Task<PayLoad<object>> Add (List<TraningModel> model);
        Task<PayLoad<object>> AddAiNew (List<AIInput> model);
    }
}
