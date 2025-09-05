using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.Runtime.InteropServices.JavaScript;

namespace MailAchiveUi
{
    public partial class FrmMain : Form
    {
        Settings settings = Settings.Instance;

        public FrmMain()
        {
            InitializeComponent();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            var emails = new List<string>();
            List<MimeMessage> aryAllMessages = new List<MimeMessage>();

            if(!Directory.Exists(".\\Temp"))
            {
                Directory.CreateDirectory(".\\Temp");
            }

            using (var client = new ImapClient())
            {
                try
                {
                    // 설정에서 서버 정보 가져오기
                    var settings = Settings.Instance;

                    await client.ConnectAsync(settings.MailServer, settings.MailServerPort, settings.MailServerUseSsl);
                    await client.AuthenticateAsync(settings.MailUser, settings.MailPassword);

                    List<string> aryAllFolders = new List<string>();


                    // 모든 폴더 탐색 및 이메일 가져오기
                    var personalNamespace = client.PersonalNamespaces[0];                    
                    var rootFolder = client.GetFolder(personalNamespace);
                    Console.WriteLine("사용 가능한 폴더 목록:");

                    int nCount = GetFolderFullNames(rootFolder, aryAllFolders);
                    progressBar1.Maximum = nCount;

                    //var rootFolder = client.GetFolder(personalNamespace);
                    foreach (string sFolderName in aryAllFolders)
                    {
                        var folder = client.GetFolder(sFolderName);
                        await FetchEmailsFromFolderAsync(folder, aryAllMessages);
                        
                    }
                    

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"IMAP4 연결 오류: {ex.Message}");
                }
            }
        }

        private int GetFolderFullNames(IMailFolder folder, List<string> folderNames)
        {
            int nCount = 0;
            if (!string.IsNullOrWhiteSpace(folder.FullName))
            {
                folderNames.Add(folder.FullName);
                //folder.OpenAsync(FolderAccess.ReadOnly).Wait();
                //nCount = folder.Count();
                
            }            

            foreach (var subFolder in folder.GetSubfolders(false))
            {
                nCount +=GetFolderFullNames(subFolder, folderNames);
            }

            return nCount;
        }


        private async Task FetchEmailsFromFolderAsync(IMailFolder folder, List<MimeMessage> emails)
        {
            // 폴더 열기
            await folder.OpenAsync(FolderAccess.ReadOnly);

            int nCount = folder.Count;

            WriteLog($"[Change Folder] :  {folder.FullName}");            
            string sBaseFolderPath = ".\\Temp\\" + folder.FullName.Replace(".", "\\");
            if(!Directory.Exists(sBaseFolderPath))
                Directory.CreateDirectory(sBaseFolderPath);

            // 현재 폴더의 이메일 가져오기
            for (int i = 0; i < nCount; i++)
            {
                try
                {
                    var message = await folder.GetMessageAsync(i);
                    string? sSubject = message.Subject;
                    WriteLog($"({i+1}/{nCount}) {sSubject}");

                    if (sSubject == null)
                        sSubject = string.Empty;
                    string fileName = $"{message.Date:yyyyMMddHHmmss}_{sSubject}";
                    fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
                    fileName = fileName.Length > 100 ? fileName.Substring(0, 50) : fileName;
                    string filePath = Path.Combine(sBaseFolderPath, fileName + ".eml");

                    if (File.Exists(filePath))
                    {
                        int count = 1;
                        string newFilePath;
                        do
                        {
                            newFilePath = Path.Combine(sBaseFolderPath, $"{fileName}_{count}.eml");
                            count++;
                        } while (File.Exists(newFilePath));
                        filePath = newFilePath;
                    }
                    message.WriteTo(filePath);
                }
                catch (Exception ex)
                {
                    WriteLog("ERROR! " + ex.Message);
                }                
            }            
        }

        private delegate void WriteLogHandler(string sMessage);

        private void WriteLog(string sMessage)
        {
            if (txtLogs.InvokeRequired)
                txtLogs.Invoke(new WriteLogHandler(WriteLog), sMessage);
            else
            {
                txtLogs.AppendText(sMessage + Environment.NewLine);
            }
        }
    }
}
