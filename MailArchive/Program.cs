using MailKit.Net.Pop3;
using MimeKit;
using SharpSevenZip;
using System.Text;

namespace MailArchive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 추가 인코딩(ks_c_5601-1987 등)을 사용하기 위해 공급자를 등록합니다.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!Settings.Instance.IsSucess)
            {
                MakeSetting();
                return;
            }

            DownloadEmails();
        }


        static void DownloadEmails()
        {
            // 파서 옵션을 생성하고, 문자 집합이 명시되지 않은 경우를 위한
            // 기본(fallback) 문자 집합을 "ks_c_5601-1987"로 설정합니다.
            var parserOptions = new ParserOptions();
            parserOptions.CharsetEncoding = Encoding.GetEncoding("ks_c_5601-1987");
           

            Console.WriteLine("Connecting to POP3 server...");
            try
            {
                using (var client = new Pop3Client())
                {
                    client.Connect(Settings.Instance.Pop3Server, Settings.Instance.Pop3Port, Settings.Instance.Pop3UseSsl);
                    client.Authenticate(Settings.Instance.Pop3User, Settings.Instance.Pop3Password);

                    int messageCount = client.Count;
                    Console.WriteLine($"{messageCount} messages found.");

                    if (messageCount == 0)
                    {
                        Console.WriteLine("No new messages to download.");
                        return;
                    }

                    string sStartDate = string.Empty;
                    string sEndDate = string.Empty;

                    string emlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mails");
                    if (!Directory.Exists(emlPath))              
                        Directory.CreateDirectory(emlPath);

                    for (int i = 0; i < messageCount; i++)
                    {
                        MimeMessage? message = null;

                        // client.GetMessage(i) 대신, 스트림을 직접 가져와 파서 옵션을 적용하여 메시지를 로드합니다.
                        // 이렇게 해야 fallback 인코딩 설정이 적용됩니다.
                        using (var stream = client.GetStream(i))
                        {
                            message = MimeMessage.Load(parserOptions, stream);
                        }
                        if (message == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(sStartDate))
                        {
                            sStartDate = message.Date.ToString("yyyyMMddHHmmss");
                        }

                        string? sSubject = message.Subject;
                        if (sSubject == null)
                            sSubject = string.Empty;

                        string filePath = string.Empty;
                        string sPostFix = string.Empty;

                        string fileName = $"{message.Date:yyyyMMddHHmmss}_{sSubject}{sPostFix}";
                        fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
                        fileName = fileName.Length > 100 ? fileName.Substring(0, 50) : fileName;
                        filePath = Path.Combine(emlPath, fileName + ".eml");
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine($"Skip. Already Downloaded message {i + 1}/{messageCount}: {message.Subject}");
                            continue;
                        }

                        sEndDate = message.Date.ToString("yyyyMMddHHmmss");
                        Console.WriteLine($"Downloading message {i + 1}/{messageCount}: {message.Subject}");
                        message.WriteTo(filePath);
                    }
                    Console.WriteLine("All messages have been downloaded successfully.");
                    CompressMailFiles(emlPath, sStartDate, sEndDate);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        
        static void CompressMailFiles(string sourceDirectory, string startDate, string endDate)
        {
            if (!Directory.Exists(sourceDirectory) || !Directory.EnumerateFiles(sourceDirectory).Any())
            {
                Console.WriteLine("No files to compress.");
                return;
            }

            string archiveFileName = $"Mails_{startDate}_{endDate}.7z";
            string archivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archiveFileName);

            try
            {
                Console.WriteLine($"Compressing files to {archivePath}...");

                // SharpSevenZip을 사용하여 압축
                var compressor = new SharpSevenZipCompressor
                {
                    ArchiveFormat = OutArchiveFormat.SevenZip,
                    CompressionLevel = CompressionLevel.Normal                    
                };
                
                compressor.CompressDirectory(sourceDirectory, archivePath);
                Console.WriteLine("Compression completed successfully.");

                // 원본 파일 및 폴더 삭제
                Directory.Delete(sourceDirectory, true);
                Console.WriteLine("Original mail files and directory have been deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during compression: {ex.Message}");
            }
        }

        static void MakeSetting()
        {
            string? sInputValue = string.Empty;

            Console.WriteLine("Settings are not configured properly. Let's begin the setup now.");
            Console.WriteLine();

            sInputValue = string.Empty;
            Console.Write("POP3 Server Host name(e.g., pop.example.com):");
            while (string.IsNullOrEmpty(sInputValue))
            {
                sInputValue = Console.ReadLine();
                if (!string.IsNullOrEmpty(sInputValue))
                {
                    Settings.Instance.Pop3Server = sInputValue.Trim();
                }
                else
                {
                    Console.WriteLine("Please enter a valid POP3 server host name.");
                }
            }

            sInputValue = string.Empty;
            Console.Write("POP3 Server Port(e.g., 995):");
            while (string.IsNullOrEmpty(sInputValue))
            {
                int nPort = 0;
                sInputValue = Console.ReadLine();
                if (int.TryParse(sInputValue, out nPort))
                {
                    Settings.Instance.Pop3Port = nPort;
                }
                else
                {
                    Console.WriteLine("Please enter a valid POP3 server port number.");
                    sInputValue = string.Empty;
                }
            }

            sInputValue = string.Empty;
            Console.Write("POP3 Server Use SSL (Y/N):");
            sInputValue = Console.ReadLine()?.Trim().ToUpper();
            Settings.Instance.Pop3UseSsl = !(sInputValue == "N");


            sInputValue = string.Empty;
            Console.Write("POP3 Server User Name:");
            while (string.IsNullOrEmpty(sInputValue))
            {
                sInputValue = Console.ReadLine();
                if (!string.IsNullOrEmpty(sInputValue))
                {
                    Settings.Instance.Pop3User = sInputValue;
                }
                else
                {
                    Console.WriteLine("Please enter a valid POP3 server user name.");
                }
            }

            sInputValue = string.Empty;
            Console.Write("POP3 Server User Password:");
            while (string.IsNullOrEmpty(sInputValue))
            {
                sInputValue = Console.ReadLine();
                if (!string.IsNullOrEmpty(sInputValue))
                {
                    Settings.Instance.Pop3Password = sInputValue;
                }
                else
                {
                    Console.WriteLine("Please enter a valid POP3 server user password.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Setting values...");
            Console.WriteLine(" - POP3 Server: {0}", Settings.Instance.Pop3Server);
            Console.WriteLine(" - POP3 Port: {0}", Settings.Instance.Pop3Port);
            Console.WriteLine(" - POP3 Use SSL: {0}", Settings.Instance.Pop3UseSsl ? "Yes" : "No");
            Console.WriteLine(" - POP3 User: {0}", Settings.Instance.Pop3User);
            Console.WriteLine(" - POP3 Password: {0}", Settings.Instance.Pop3Password);

            Console.WriteLine();

            sInputValue = string.Empty;
            Console.WriteLine("Do you save settings? (Y/N):");
            sInputValue = Console.ReadLine()?.Trim().ToUpper();
            if (sInputValue == "Y")
            {
                Console.WriteLine("Saving settings...");
                Settings.Instance.Save();
            }
            return;
        }
    }
}
