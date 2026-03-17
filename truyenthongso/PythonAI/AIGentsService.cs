using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.Protocols;
using System.Text;
using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.PythonAI
{
    public class AIGentsService : IAIGentsService
    {
        public async Task<PayLoad<object>> AIGents(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    ms.Position = 0;

                    var fileContent = new ByteArrayContent(ms.ToArray());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

                    content.Add(fileContent, "image", file.FileName);

                    var response = await client.PostAsync("http://127.0.0.1:5000/analyze-image", content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode) 
                    {
                        string jsonResponse = result;

                        // 1. Deserialize bước đầu để lấy "data" dưới dạng string
                        var apiResponse = JsonConvert.DeserializeObject<AIJson>(jsonResponse);

                        return await Task.FromResult(PayLoad<object>.Successfully(new
                        {
                            data = apiResponse
                        }));
                    }
                    else
                    {
                        return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                    }
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> AIGentsTiengTrung(string capdo, string type, string tu)
        {
            try
            {
                if (string.IsNullOrEmpty(capdo) || string.IsNullOrEmpty(type))
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                using(var client = new HttpClient())
                {
                    var body = new { capdo, type, tu };
                    var json = JsonConvert.SerializeObject(body);
                    var converString = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://127.0.0.1:5000/tiengtrung", converString);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = result;

                        var apiResponse = JsonConvert.DeserializeObject<tiengtrung>(jsonResponse);

                        return await Task.FromResult(PayLoad<object>.Successfully(new
                        {
                            data = apiResponse
                        }));

                    }
                    else
                    {
                        return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                    }
                }
               
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}