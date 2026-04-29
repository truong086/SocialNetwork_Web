using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.PythonAI;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class TraningModelService : ITraningModelService
    {
        private readonly DBContext _context;
        private readonly IAIService _aiService;
        private readonly IUserNameService _userNameService;
        public TraningModelService(DBContext context, IAIService aIService, IUserNameService userNameService)
        {
            _context = context;
            _aiService = aIService;
            _userNameService = userNameService;
        }
        public async Task<PayLoad<object>> Add(List<TraningModel> model)
        {
            try
            {
                var list = new List<float>();
                foreach (var modelItem in model) {
                    var ai = await _aiService.GetAi(modelItem);
                    list.Add(ai);
                }
                

                return await Task.FromResult(PayLoad<object>.Successfully(new 
                {
                    data = list
                }));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> AddAiNew(List<AIInput> model)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n)){
                    var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
                    if (checkUser == null) return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                    var categoryId = model.Select(x => x.CategoryId).Distinct().ToList();
                    var categoryDataDb = _context.categories.Where(x => categoryId.Contains(x.id) && !x.deleted).ToList();

                    var categoryDic = categoryDataDb.ToDictionary(x => x.id);

                    var listFloat = new List<float>();
                    var listBehavioral_Analysis = new List<Behavioral_Analysis>();

                    var semaphore = new SemaphoreSlim(10); // Giới hạn 10 Task cùng lúc

                    /* Tạo tất cả các Task cùng lúc rồi chạy song song nhiều công việc để hiệu suất nhanh hơn, sau đó await tất cả bằng Task.WhenAll 
                        - Nếu không dùng Task thì theo cách cũ là: AI1 → xong → AI2 → xong → AI3... , 100 item = 100 × thời gian (Rất chậm)
                        - Nếu như dùng Task thì sẽ chạy song song AI1
                                                                  AI2
                                                                  AI3
                                                                  AI4
                                                                  ...
                                                                  chạy cùng lúc, 100 item ≈ thời gian của 1–2 request
                     */
                    var taskData = model.Select(async item => 
                    {
                        // Duyệt qua từng item rồi trả về Task
                        await semaphore.WaitAsync();
                        try
                        {
                            if (categoryDic.ContainsKey(item.CategoryId))
                            {
                                item.UserId = checkUser.id;

                                var ai = await _aiService.GetAi(item);


                                // Trả về kết quả
                                return new
                                {
                                    ai = ai,
                                    entry = new Behavioral_Analysis
                                    {
                                        total = Convert.ToDecimal(ai),
                                        user = checkUser,
                                        user_id = checkUser.id,
                                        category_id = item.CategoryId,
                                        category = categoryDic[item.CategoryId] // Lấy ra giá trị category trong "Dictionary"
                                    }
                                };
                            }

                            return null;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                        
                    });

                    // Chạy tất cả task, chờ tất cả task chạy xong rồi trả về kết quả: result = object[]
                    var result = await Task.WhenAll(taskData);

                    foreach(var r in result.Where(x => x != null))
                    {
                        listFloat.Add(r.ai);
                        listBehavioral_Analysis.Add(r.entry);
                    }

                    if(listBehavioral_Analysis.Count > 0 && listBehavioral_Analysis.Any())
                    {
                        _context.behavioral_Analysess.AddRange(listBehavioral_Analysis);
                        await _context.SaveChangesAsync();
                    }

                    var response = listBehavioral_Analysis.Select(x => new
                    {
                        x.total,
                        x.user.UserName,
                        x.category.Name,
                    });

                    return await Task.FromResult(PayLoad<object>.Successfully(new
                    {
                        data = listFloat,
                        behavioral_Analysis = response
                    }));
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        // Cách 1
        //public async Task<PayLoad<object>> AddAiNew(List<AIInput> model)
        //{
        //    try
        //    {
        //        var user = _userNameService.name();
        //        if(int.TryParse(_userNameService.name(), out int n))
        //        {
        //            var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
        //            if (checkUser == null)
        //                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

        //            var list = new List<float>();
        //            var listBehavioral_Analysis = new List<Behavioral_Analysis>();
        //            var categoryDistinct = model.Select(x => x.CategoryId).Distinct().ToList(); // "Distinct" là loại bỏ id bị trùng
        //            var categoryData = _context.categories.Where(x => categoryDistinct.Contains(x.id) && !x.deleted).ToList(); // Lấy hết dữ liệu category 1 lần, rồi sau đó mới bắt đầu kiểm tra, tránh gọi nhiều lần vào Database
        //            foreach (var item in model)
        //            {
        //                //var categoryModel = categoryData.FirstOrDefault(x => x.id == item.CategoryId); // Cách 1
        //                // if(categoryModel != null){}

        //                var categoryModel = categoryData.ToDictionary(x => x.id); // Cách 2: Chuyển list sang Dictionary để chạy nhanh hơn, biến list category thành "Dictionary<int, Category>" int ở đây sẽ là Id vì đã cấu hình "ToDictionary(x => x.id)"
        //                if (categoryModel.ContainsKey(item.CategoryId))
        //                {
        //                    item.UserId = checkUser.id;
        //                    var ai = await _aiService.GetAi(item);
        //                    listBehavioral_Analysis.Add(new Behavioral_Analysis
        //                    {
        //                        total = Convert.ToDecimal(ai),
        //                        category_id = item.CategoryId,
        //                        user_id = checkUser.id,
        //                        user = checkUser
        //                        //total = Math.Round((decimal)ai, 2) // Đây là ép kiểu sang "Decimal" và làm tròn 2 chữ số
        //                    });
        //                    list.Add(ai);
        //                }


        //            }

        //            if(listBehavioral_Analysis.Count > 0)
        //            {
        //                _context.behavioral_Analysess.AddRange(listBehavioral_Analysis);
        //                await _context.SaveChangesAsync();
        //            }


        //            return await Task.FromResult(PayLoad<object>.Successfully(new
        //            {
        //                data = list
        //            }));
        //        }

        //        return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

        //    }
        //    catch (Exception ex) {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}
    }
}
