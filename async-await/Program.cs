// See https://aka.ms/new-console-template for more information

using System.IO.Pipes;
using System.Text.Json;
using System.Threading;

using HttpClient httpClient = new HttpClient();
ApiClient apiClient = new ApiClient(httpClient);

Task<Func<string>> action = Task.FromResult<Func<string>>(() =>
{
    Console.WriteLine("Action is executed!");
    return "Action Result: Success"; // Chuỗi được trả về từ Func<string>
});

string url = "https://localhost:7133/WeatherForecast";
var result = await apiClient.CallApiAsync<object>(
                url,
                HttpMethod.Get,
                onSuccess: async (data) =>
                {
                    // Xử lý bất đồng bộ khi thành công
                    await Task.Delay(8500); // Giả lập một công việc async
                    Console.WriteLine("API call success!");
                    Console.WriteLine($"Response: {data}");

                    return "hello";
                },
                onFailure: async (error) =>
                {
                    // Xử lý bất đồng bộ khi thất bại
                    await Task.Delay(500); // Giả lập một công việc async
                    Console.WriteLine("API call failed!");
                    Console.WriteLine($"Error: {error.Message}");
                });

Console.WriteLine("Final result:");
Console.WriteLine(result);

Console.WriteLine("Hello, World!");

class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResult> CallApiAsync<TResult>(
        string url,
        HttpMethod method,
        object? requestBody = null,
        Func<TResult, Task<TResult>>? onSuccess = null,
        Func<Exception, Task>? onFailure = null)
    {
        try
        {
            // Tạo request
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            // Nếu có dữ liệu body thì serialize nó
            if (requestBody != null)
            {
                string jsonBody = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            }

            // Gửi request và lấy response
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Deserialize response body
            string responseData = await response.Content.ReadAsStringAsync();
            TResult result = JsonSerializer.Deserialize<TResult>(responseData)!;

            // Gọi action thành công (nếu có)
            if (onSuccess != null)
            {
                result = await onSuccess(result); // Hỗ trợ async/await
            }

            return result;
        }
        catch (Exception ex)
        {
            // Gọi action khi lỗi (nếu có)
            if (onFailure != null)
            {
                await onFailure(ex); // Hỗ trợ async/await
            }

            throw; // Ném lại lỗi để xử lý ở nơi khác nếu cần
        }
    }
}