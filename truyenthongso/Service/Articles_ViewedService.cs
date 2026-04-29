using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.PythonAI;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class Articles_ViewedService : IArticles_ViewedService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly IAIService _aIService;
        public Articles_ViewedService(DBContext context, 
            IUserNameService userNameService,
            IAIService aIService)
        {
            _context = context;
            _userNameService = userNameService;
            _aIService = aIService;
        }
        public async Task<PayLoad<object>> Add(Articles_ViewedDTO data)
        {
            try
            {
                if(int.TryParse(_userNameService.name(),  out int userId))
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == userId && !x.deleted);
                    var checkPost = _context.posts.FirstOrDefault(x => x.id == data.post_id && !x.deleted);
                    if (checkUser == null || checkPost == null)
                        return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                    var checkCategory = _context.categories.FirstOrDefault(x => x.id == checkPost.Category_id && !x.deleted);
                    var checkData = _context.articles_Viewedss.FirstOrDefault(x => x.post_id == data.post_id && x.user_id == checkUser.id && !x.deleted);

                    AddForecastAI(checkUser, checkCategory);

                    if (checkData != null || checkCategory == null)
                        return await Task.FromResult(PayLoad<object>.Successfully(new { mwssage = Status.SUCCESS }));

                    _context.articles_Viewedss.Add(new Articles_Viewed
                    {
                        post = checkPost,
                        post_id = data.post_id,
                        user_id = checkUser.id,
                        user = checkUser
                    });

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<object>.Successfully(new { mwssage = Status.SUCCESS}));
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            } 
        }

        private async void AddForecastAI(User checkUser, Category checkCategory)
        {
            var dataAI = await _aIService.GetAi(new AIInput
            {
                UserId = checkUser.id,
                CategoryId = checkCategory.id,
                TimeOnPage = 1,
                ScrolledToBottom = false,
                Liked = false,
                TimeOfDay = 1,
                Device = "Mobile"
            });


            _context.behavioral_Analysess.Add(new Behavioral_Analysis
            {
                total = (decimal)dataAI,
                category = checkCategory,
                category_id = checkCategory.id,
                user = checkUser,
                user_id = checkUser.id
            });

            await _context.SaveChangesAsync();
        }

        public Task<PayLoad<string>> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<object>> Update(int id, Articles_ViewedDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
