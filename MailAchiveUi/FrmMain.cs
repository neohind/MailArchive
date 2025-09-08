using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.Runtime.InteropServices.JavaScript;

namespace MailAchiveUi
{
    public partial class FrmMain : Form
    {
        private Settings settings = Settings.Instance;
        private int m_nCount = 0;

        public FrmMain()
        {
            InitializeComponent();

            txtUrl.Text = settings.MailServer;
            txtPort.Text = settings.MailServerPort.ToString();
            txtId.Text = settings.MailUser;
            txtPassword.Text = settings.MailPassword;
            chkUseSSL.Checked = settings.MailServerUseSsl;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            txtResultPath.Text = settings.ResultPath;

            timer1.Start();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            Settings.Instance.MailServer = txtUrl.Text.Trim();
            Settings.Instance.MailServerPort = int.TryParse(txtPort.Text.Trim(), out int port) ? port : 0;
            Settings.Instance.MailServerUseSsl = chkUseSSL.Checked;
            Settings.Instance.MailUser = txtId.Text.Trim();
            Settings.Instance.MailPassword = txtPassword.Text.Trim();
            Settings.Instance.Save();

            if(txtUrl.Text.Trim() == string.Empty ||
               txtPort.Text.Trim() == string.Empty ||
               txtId.Text.Trim() == string.Empty ||
               txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("모든 필드를 입력해주세요.");
                return;
            }


            var emails = new List<string>();
            List<MimeMessage> aryAllMessages = new List<MimeMessage>();

            if (!Directory.Exists(settings.ResultPath))            
                Directory.CreateDirectory(settings.ResultPath);
            

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
                    WriteLog($"IMAP4 연결 오류: {ex.Message}");
                }
            }
            timer1.Stop();
            progressBar1.Value = m_nCount;
        }

        private int GetFolderFullNames(IMailFolder folder, List<string> folderNames)
        {
            int nCount = 0;
            if (!string.IsNullOrWhiteSpace(folder.FullName))
            {
                folderNames.Add(folder.FullName);
                folder.OpenAsync(FolderAccess.ReadOnly).Wait();
                nCount = folder.Count;
            }
            progressBar1.Maximum = nCount;

            foreach (var subFolder in folder.GetSubfolders(false))
            {
                nCount += GetFolderFullNames(subFolder, folderNames);
            }

            return nCount;
        }


        private async Task FetchEmailsFromFolderAsync(IMailFolder folder, List<MimeMessage> emails)
        {
            // 폴더 열기
            await folder.OpenAsync(FolderAccess.ReadOnly);

            int nCount = folder.Count;

            WriteLog($"[Change Folder] :  {folder.FullName}");
            string sBaseFolderPath = Path.Combine(settings.ResultPath, folder.FullName.Replace(".", "\\"));
            if (!Directory.Exists(sBaseFolderPath))
                Directory.CreateDirectory(sBaseFolderPath);

            // 현재 폴더의 이메일 가져오기
            for (int i = 0; i < nCount; i++)
            {
                m_nCount++;
                try
                {
                    var message = await folder.GetMessageAsync(i);
                    string? sSubject = message.Subject;
                    WriteLog($"({i + 1}/{nCount}) {sSubject}");

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = m_nCount;
        }
    }
}
