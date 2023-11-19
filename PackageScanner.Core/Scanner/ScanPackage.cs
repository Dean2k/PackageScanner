using Newtonsoft.Json;
using PackageScanner.Core.Models;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using YamlDotNet.Core.Tokens;

namespace PackageScanner.Core.Scanner
{
    public class ScanPackage
    {

        public List<HashList> _hashList;

        public void GetLatestHashes()
        {
            string apiUrl = $"https://api.avatarrecovery.com/Package/FileHashList";
            HttpClient client = new HttpClient();
            string response = client.GetStringAsync(apiUrl).Result;
            var data = JsonConvert.DeserializeObject<List<HashList>>(response);
            _hashList = data;
        }

        public string WriteLog(string logText)
        {
            string logBuilder = $"{DateTime.Now:yy/MM/dd H:mm:ss} | {logText} \n";
            File.AppendAllText("LatestLog.txt", logBuilder);
            return logBuilder;
        }


        public FileStats CheckFiles(string unityPackageExtractedLocation, bool deleteUrl, bool deleteDll, bool deleteCs)
        {
            if(_hashList == null) { Console.WriteLine("Get latest hashes first"); return null; }
            string[] csPaths = Directory.GetFiles(unityPackageExtractedLocation, "*.cs", SearchOption.AllDirectories);
            string[] dllPaths = Directory.GetFiles(unityPackageExtractedLocation, "*.dll", SearchOption.AllDirectories);

            string[] combinedStrings = csPaths.Concat(dllPaths).ToArray();

            List<string> arrayToList = new List<string>();

            foreach (var item in combinedStrings)
            {
                arrayToList.Add(item);
            }

            return CheckSafeUnsafeFiles(arrayToList, deleteUrl, deleteDll, deleteCs);
        }

        public string Sha256CheckSum(string filePath)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return BitConverter.ToString(SHA256.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
            }
        }

        public FileStats CheckSafeUnsafeFiles(List<string> imported, bool deleteUrl, bool deleteDll, bool deleteCs)
        {
            imported.Sort();
            List<(string, string)> safeFiles = new List<(string, string)>();
            List<(string, string)> badFilesShouldDelete = new List<(string, string)>();
            List<(string, string)> unknownFiles = new List<(string, string)>();
            List<(string, string)> urlDeletes = new List<(string, string)>();
            List<(string, string)> dllFiles = new List<(string, string)>();
            List<(string, string)> csFiles = new List<(string, string)>();
            int noDelete = 0;

            foreach (string file in imported)
            {
                string fileHash = Sha256CheckSum(file);
                if (deleteUrl)
                {
                    foreach (var str in File.ReadLines(file).Where(s => s.Contains("/api/webhooks/") || s.Contains("Base64String")))
                    {
                        WriteLog($"Webhook / Base64 encoding found and should be removed) {file}:Hash={fileHash}\n");
                        urlDeletes.Add((file, fileHash));
                    }
                }
                  
                HashList hashList = _hashList.SingleOrDefault(x=>x.FileHash1.Equals(fileHash));

                if (file.ToLower().EndsWith(".cs") && deleteCs)
                {
                    csFiles.Add((file, fileHash));
                }

                if (file.ToLower().EndsWith(".dll") && deleteDll)
                {
                    dllFiles.Add((file, fileHash));
                }

                if (hashList == default)
                {
                    unknownFiles.Add((file, fileHash));
                }
                else if(hashList.Malicious){
                    badFilesShouldDelete.Add((file, fileHash));
                } else
                {
                    safeFiles.Add((file, fileHash));
                }
            }

            string returnString = "";

            if (safeFiles.Count > 0)
            {
                string output = "";
                foreach (var f in safeFiles)
                {
                    output += $"{f.Item1} | Hash: {f.Item2}\n";
                }
                WriteLog($"Allowed scripts ({safeFiles.Count}):\n" + output);
            }

            if (unknownFiles.Count > 0)
            {
                string output = "";
                foreach (var f in unknownFiles)
                {
                    output += $"{f.Item1} | Hash: {f.Item2}\n";
                }
                WriteLog($"Unknown scripts ({unknownFiles.Count}):\n" + output);
            }

            if (badFilesShouldDelete.Count > 0)
            {
                string output = "";
                foreach (var f in badFilesShouldDelete)
                {
                    output += $"{f.Item1} | Hash: {f.Item2}\n";
                    bool success = DeleteFileAndMeta(f.Item1);
                    if (!success) { noDelete++; }
                }
                WriteLog($"Not allowed scripts ({badFilesShouldDelete.Count}). They will be deleted:\n" + output);
            }
            if (deleteUrl)
            {
                if (urlDeletes.Count > 0)
                {
                    string output = "";
                    foreach (var f in urlDeletes)
                    {
                        output += $"{f.Item1} | Hash: {f.Item2}\n";
                        bool success = DeleteFileAndMeta(f.Item1);
                        if (!success) { noDelete++; }
                    }
                    WriteLog($"Possible malicous files ({urlDeletes.Count}). They will be deleted:\n" + output);
                }
            }

            if (deleteDll)
            {
                if (dllFiles.Count > 0)
                {
                    string output = "";
                    foreach (var f in dllFiles)
                    {
                        output += $"{f.Item1} | Hash: {f.Item2}\n";
                        bool success = DeleteFileAndMeta(f.Item1);
                        if (!success) { noDelete++; }
                    }
                    WriteLog($"DLL files ({dllFiles.Count}). They will be deleted:\n" + output);
                }
            }

            if (deleteCs)
            {
                if (csFiles.Count > 0)
                {
                    string output = "";
                    foreach (var f in csFiles)
                    {
                        output += $"{f.Item1} | Hash: {f.Item2}\n";
                        bool success = DeleteFileAndMeta(f.Item1);
                        if (!success) { noDelete++; }
                    }
                    WriteLog($"CS files ({csFiles.Count}). They will be deleted:\n" + output);
                }
            }

            FileStats fileStats = new FileStats
            {
                SafeFiles = safeFiles.Count,
                BadFiles = badFilesShouldDelete.Count,
                UnknownFiles = unknownFiles.Count,
                UrlDelete = urlDeletes.Count,
                OtherDelete = csFiles.Count + dllFiles.Count,
                NoDelete = noDelete
            };

            return fileStats;
        }
        
        private bool DeleteFileAndMeta(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            } catch { return false; }
            try
            {
                if (File.Exists($"{path}.meta"))
                {
                    File.Delete($"{path}.meta");
                }
            }
            catch { }

            return true;
        }
    }
}
