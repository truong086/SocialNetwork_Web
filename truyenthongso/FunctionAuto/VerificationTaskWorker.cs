
using Microsoft.Extensions.DependencyInjection;
using truyenthongso.Service;

namespace truyenthongso.FunctionAuto
{
    public class VerificationTaskWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public VerificationTaskWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task RunOnceAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await accountService.DeleteAccountNoAction();
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var accountService = scope.ServiceProvider.GetRequiredService<IUserService>();

                        // Gọi hàm DeleteAccountNoAction
                        await accountService.DeleteAccountNoAction();
                    }

                    // Tùy chỉnh thời gian lặp lại
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi: {ex.Message}");
                }
            }
        }
    }
}
