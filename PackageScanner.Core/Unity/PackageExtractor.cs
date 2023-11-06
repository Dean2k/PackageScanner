using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageScanner.Core.Unity
{
    public class PackageExtractor
    {

        public string ExtractPackage(string packagePath, string outPath = null)
        {
            outPath = GetFullOutPath(packagePath, outPath);

            string workingDir = ExtractToWorkingDirectory(packagePath, outPath);

            FixFolderStructure(outPath, workingDir);

            CleanUp(workingDir);

            return outPath;
        }

        private string GetFullOutPath(string packagePath, string outPath)
        {
            string name = Path.GetFileNameWithoutExtension(packagePath);

            outPath = Path.Combine(outPath, name);
            if (Directory.Exists(outPath))
            {
                throw new Exception($"Output path {outPath} already exists");
            }

            return outPath;
        }

        // Extract the archive using the archive's folder structure (one directory per-asset with meta data indicating where the asset should finally live)
        private string ExtractToWorkingDirectory(string packagePath, string outPath)
        {
            string workingDir = Path.Combine(outPath, ".working");

            if (Directory.Exists(workingDir))
            {
                Directory.Delete(workingDir, true);
            }
            Directory.CreateDirectory(workingDir);

            var inStream = File.OpenRead(packagePath);
            var gzipStream = new GZipInputStream(inStream);
            var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(workingDir);
            tarArchive.Close();
            gzipStream.Close();
            inStream.Close();
            return workingDir;
        }

        // Iterate over the individual assets' directories and move them to their target location from the "pathname" file
        private void FixFolderStructure(string outPath, string workingDir)
        {
            var dirs = Directory.GetDirectories(workingDir);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string dir = dirs[i];
                string assetPath = Path.Combine(dir, "asset");
                string assetPathMeta = Path.Combine(dir, "asset.meta");

                string pathnamePath = Path.Combine(dir, "pathname");
                if (!File.Exists(assetPath) || !File.Exists(pathnamePath))
                {
                    continue;
                }

                string assetTargetPathRelative;
                // Some packages have a second line containing a GUID, so just grab the path from the first line
                using (var pathnameFile = new StreamReader(pathnamePath))
                {
                    assetTargetPathRelative = pathnameFile.ReadLine();
                }
                string assetTargetPath = Path.Combine(outPath, assetTargetPathRelative);
                string assetTargetPathMeta = Path.Combine(outPath, assetTargetPathRelative + ".meta");
                string assetTargetPathDir = Path.GetDirectoryName(assetTargetPath);
                if (!Directory.Exists(assetTargetPathDir))
                {
                    Directory.CreateDirectory(assetTargetPathDir);
                }
                File.Move(assetPath, assetTargetPath);
                if (File.Exists(assetPathMeta))
                {
                    File.Move(assetPathMeta, assetTargetPathMeta);
                }
            }
        }

        // Delete the working directory when we're done
        private void CleanUp(string workingDir)
        {
            Directory.Delete(workingDir, true);
        }
    }
}
