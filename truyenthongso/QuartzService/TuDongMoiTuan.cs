using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Quartz;
using truyenthongso.Clouds;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.QuartzService
{
    public class TuDongMoiTuan : IJob
    {
        private readonly DBContext _dbContext;
        private readonly Cloud _cloud;
        public TuDongMoiTuan(DBContext context, IOptions<Cloud> cloud)
        {
            _dbContext = context;
            _cloud = cloud.Value;
        }
        public Task Execute(IJobExecutionContext context)
        {

            return Task.CompletedTask;
        }
    }
}
