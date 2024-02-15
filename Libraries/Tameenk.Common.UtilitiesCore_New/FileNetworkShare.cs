using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Tameenk.Common.Utilities
{
    public class FileNetworkShare
    {
        public bool SaveFileToShare(string domain, string userID, string password, string generatedReportDirPath, string filepath, byte[] pdfData, string serverIP, out string exception)
        {
            try
            {
                exception = string.Empty;
                filepath = filepath.Replace(":", "$");
                generatedReportDirPath = generatedReportDirPath.Replace(":", "$");
                FileDownloads unc = new FileDownloads(@filepath, userID, domain, password);
                if (!Directory.Exists(generatedReportDirPath))
                    Directory.CreateDirectory(generatedReportDirPath);
                File.WriteAllBytes(filepath, pdfData);
                return true;
                //int ret = 0;
                //int ret2 = 0;
                //if (ImpersonateUser(domain, userID, password, out ret, out ret2) == true)
                //{
                //    if (!Directory.Exists(generatedReportDirPath))
                //        Directory.CreateDirectory(generatedReportDirPath);

                //    File.WriteAllBytes(filepath, pdfData);
                //    undoImpersonation();
                //    return true;
                //}
                //else
                //{
                //    exception = "Could not authenticate account. Something is up.";
                //    return false;
                //}
            }
            catch (Exception exp)
            {
                exception = " generatedReportDirPath="+ generatedReportDirPath + " filepath="+ filepath+" " + exp.ToString();
                return false;
            }
        }
        public byte[] GetFileFromShare(string domain, string serverIP, string userID, string password, string filepath, out string exception)
        {
            string x = string.Empty;
            try
            {
                exception = string.Empty;
                filepath = serverIP + "\\" + filepath.Replace(":", "$");
                FileDownloads unc = new FileDownloads(@filepath, userID, domain, password);
                return File.ReadAllBytes(filepath);
                //int ret = 0;
                //int ret2 = 0;
                //if (ImpersonateUser(domain, userID, password,out ret,out ret2) == true)
                //{
                //    x += " ImpersonateUser success";
                //    filepath = serverIP + "\\" + filepath.Replace(":", "$");
                //    x += " filepath "+ filepath;
                //    x += " WindowsIdentity().Name " + WindowsIdentity.GetCurrent().Name;
                //    System.Security.Principal.WindowsImpersonationContext impersonationContext;
                //    impersonationContext =WindowsIdentity.GetCurrent().Impersonate();
                //    var file = File.ReadAllBytes(filepath);
                //    x += "  File ";
                //    undoImpersonation();
                //    return file;
                //}
                //else
                //{
                //    exception += "Could not authenticate account. Something is up. and ret is "+ret+" and ret 2 is "+ret2+" user is " + WindowsIdentity.GetCurrent().Name+" ";
                //    return null;
                //}
            }
            catch (Exception exp)
            {
                exception = x + " WindowsIdentity.GetCurrent().Name " + WindowsIdentity.GetCurrent().Name + " " + exp.ToString();
                return null;
            }
        }
        public byte[] GetFile(string filepath, out string exception)
        {
            byte[] fileData = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    exception = string.Empty;
                  
                    string URL = "https://10.101.15.180:9001/api/policy/GetFile";
                    string urlParameters = "?filePath=" + filepath;
                    client.BaseAddress = new Uri(URL);
                  
                    HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        fileData = JsonConvert.DeserializeObject<byte[]>(response.Content.ReadAsStringAsync().Result);
                       
                    }
                    return fileData;
                }
            }
            catch (Exception exp)
            {
                exception = "filedata is "+fileData+" "+exp.ToString();
                return null;
            }
        }

        public bool UploadFileToShare(string domain, string userID, string password, string generatedReportDirPath, string filepath, byte[] pdfData, string serverIP, out string exception)
        {
            exception = string.Empty;
            try
            {
                FileInfo fi = new FileInfo(filepath);
                string fileName = fi.Name;
                Uri webService = new Uri(@"https://10.101.15.180:9001/api/policy/uploadFile");
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
                requestMessage.Headers.ExpectContinue = false;

                MultipartFormDataContent formData = new MultipartFormDataContent("BCareBoundary");

                HttpContent filePathContent = new StringContent(filepath); // Le contenu du paramètre P1
                HttpContent dirPathContent = new StringContent(generatedReportDirPath);
                formData.Add(filePathContent, "filePath");
                formData.Add(dirPathContent, "directoryPath");

                ByteArrayContent byteArrayContent = new ByteArrayContent(pdfData);
                byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                formData.Add(byteArrayContent, "filebytes", fileName);
                requestMessage.Content = formData;

                HttpClient httpClient = new HttpClient();

                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;

                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;
                }
                return true;
            }
            catch (Exception exp)
            {
                exception = " generatedReportDirPath=" + generatedReportDirPath + " filepath=" + filepath + " " + exp.ToString();
                return false;
            }
        }
        /// <summary>
        /// Impersonates the given user during the session.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private bool ImpersonateUser(string domain, string userName, string password, out int ret, out int ret2)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            ret = 0;
            ret2 = 0;
            if (RevertToSelf())
            {
                int val = LogonUserA("administrator", "172.16.146.9", "J@2Ir@h-WEB02", LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token);
                ret = val;
                if (val != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            ret2 = Marshal.GetLastWin32Error();
            return false;
        }

        /// <summary>
        /// Undoes the current impersonation.
        /// </summary>
        private void undoImpersonation()
        {
            impersonationContext.Undo();
        }

        public byte[] GetFileFromNewServer(string filepath, out string exception)
        {
            byte[] fileData = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    exception = string.Empty;
                    string URL = "http://10.201.11.41:9000/api/policy/GetFile";
                    string urlParameters = "?filePath=" + filepath;
                    client.BaseAddress = new Uri(URL);

                    HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        fileData = JsonConvert.DeserializeObject<byte[]>(response.Content.ReadAsStringAsync().Result);
                    }
                    return fileData;
                }
            }
            catch (Exception exp)
            {
                exception = "filedata is " + fileData + " " + exp.ToString();
                return null;
            }
        }

        #region Impersionation global variables
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LoadUserProfile
        (IntPtr hToken, ref ProfileInfo lpProfileInfo);

        [DllImport("Userenv.dll", CallingConvention =
            CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnloadUserProfile
            (IntPtr hToken, IntPtr lpProfileInfo);
        #endregion
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileInfo
    {
        ///
        /// Specifies the size of the structure, in bytes.
        ///
        public int dwSize;

        ///
        /// This member can be one of the following flags: 
        /// PI_NOUI or PI_APPLYPOLICY
        ///
        public int dwFlags;

        ///
        /// Pointer to the name of the user.
        /// This member is used as the base name of the directory 
        /// in which to store a new profile.
        ///
        public string lpUserName;

        ///
        /// Pointer to the roaming user profile path.
        /// If the user does not have a roaming profile, this member can be NULL.
        ///
        public string lpProfilePath;

        ///
        /// Pointer to the default user profile path. This member can be NULL.
        ///
        public string lpDefaultPath;

        ///
        /// Pointer to the name of the validating domain controller, in NetBIOS format.
        /// If this member is NULL, the Windows NT 4.0-style policy will not be applied.
        ///
        public string lpServerName;

        ///
        /// Pointer to the path of the Windows NT 4.0-style policy file. 
        /// This member can be NULL.
        ///
        public string lpPolicyPath;

        ///
        /// Handle to the HKEY_CURRENT_USER registry key.
        ///
        public IntPtr hProfile;
    }

}