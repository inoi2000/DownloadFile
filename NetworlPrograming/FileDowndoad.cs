using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetworlPrograming
{
    public static class FileDowndoad
    {
        public static async Task DownloadingFileAsync(string uri, string outputPath, IProgress<double> progress, CancellationToken token)
        {
            if (string.IsNullOrEmpty(uri)) 
            { 
                throw new ArgumentException($"\"{nameof(uri)}\" не может быть неопределенным или пустым.", nameof(uri)); 
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException($"\"{nameof(outputPath)}\" не может быть неопределенным или пустым.", nameof(outputPath));
            }

            long localFileLen = 0;
            FileStream file;
            if (File.Exists(outputPath))
            {
                localFileLen = new FileInfo(outputPath).Length;
                file = File.OpenWrite(outputPath);
                file.Seek(0, SeekOrigin.End);
            }
            else
            {
                file = File.Create(outputPath);
            }

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Headers = { Range = new RangeHeaderValue(localFileLen, null) }
            };
            
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            long? contentLength = response.Content.Headers.ContentLength; 
            long totalBytesRead = 0;

            response.EnsureSuccessStatusCode();

            long? contenerLength = response.Content.Headers.ContentLength;

            await using var contentStream = await response.Content.ReadAsStreamAsync();

            byte[] buffer = new byte[8192 * 2];

            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10)); // намеренное замедление для проверки функционала прогресса и отмены

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                int bytesRead = await contentStream.ReadAsync(buffer);

                if (bytesRead == 0) { break; }

                if(bytesRead == buffer.Length) { await file.WriteAsync(buffer, token); }
                else { await file.WriteAsync(buffer[..bytesRead], token); }

                totalBytesRead += bytesRead;
                if (contentLength != null)
                {
                    progress.Report((double)totalBytesRead / contentLength.Value);
                }
            }
            file.Close();
        }
    }
}
