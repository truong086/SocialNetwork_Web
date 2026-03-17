using MailKit.Security;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace truyenthongso.EmailConfig
{
    public class SendEmais
    {
        public EmailSetting _emaiSetting;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public SendEmais(IOptions<EmailSetting> emailSetting,
            IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider
            , IServiceProvider serviceProvider)
        {
            _emaiSetting = emailSetting.Value;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;

        }

        public async Task SendEmai(string emai, string tieude, string body)
        {

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emaiSetting.SenderName, _emaiSetting.SenderEmail));
            message.To.Add(new MailboxAddress("", emai));
            message.Subject = tieude;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emaiSetting.SmtpServer, _emaiSetting.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emaiSetting.Username, _emaiSetting.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            string message = "";
            using (var scope = _serviceProvider.CreateScope())
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData.Model = model;

                var actionContext = new ActionContext(
                    new DefaultHttpContext { RequestServices = scope.ServiceProvider },
                new RouteData(),
                    new ActionDescriptor()
                );

                using (var writers = new StringWriter())
                {
                    try
                    {
                        var viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);
                        var viewContext = new ViewContext(actionContext, viewResult.View, viewData, new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), writers, new HtmlHelperOptions());

                        await viewResult.View.RenderAsync(viewContext);
                        return await Task.FromResult(writers.ToString());
                        //await SendEmai(email, title);
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi, ví dụ: log lỗi hoặc thông báo cho người dùng
                        message = $"{ex.Message}";
                    }
                }
            }
            return await Task.FromResult(message);
        }
    }
}
