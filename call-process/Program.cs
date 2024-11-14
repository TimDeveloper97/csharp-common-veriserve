// See https://aka.ms/new-console-template for more information

using System.IO.Pipes;
using System.Threading;

string PipeName = "PipeName";
CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

await Task.Run(async () =>
{
    while (!_cancellationTokenSource.Token.IsCancellationRequested)
    {
        using (var server = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
        {
            try
            {
                // Đợi kết nối từ Client
                await server.WaitForConnectionAsync(_cancellationTokenSource.Token);

                using (var reader = new StreamReader(server))
                {
                    string receivedData = await reader.ReadToEndAsync();

                    // Xử lý dữ liệu nhận được
                    if (!string.IsNullOrWhiteSpace(receivedData))
                    {
                        Console.WriteLine("Data Received", receivedData, "OK");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Kết thúc vòng lặp khi bị hủy
                break;
            }
            catch (Exception ex)
            {
                // Log hoặc xử lý lỗi khác nếu cần
                Console.WriteLine("Error", ex.Message, "OK");
            }
        }
    }
});


Console.WriteLine("Hello, World!");
