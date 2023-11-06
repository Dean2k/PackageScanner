using Microsoft.WindowsAPICodePack.Dialogs;
using PackageScanner.Core.Models;
using PackageScanner.Core.Scanner;
using PackageScanner.Core.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackageScanner
{
    public partial class Scan : Form
    {
        public Scan()
        {
            InitializeComponent();
        }

        private string _fileLocation = "";
        private string _programLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        private void Scan_Load(object sender, EventArgs e)
        {

        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Unity Package (*.unitypackage)|*.unitypackage";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Select Unity Package";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    //Get the path of specified file
                    _fileLocation = openFileDialog.FileName;
            }
        }

        private void btnScanFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_fileLocation)) { txtLog.Text = $"Select file first!{Environment.NewLine}"; return; }
            txtLog.Text = $"Getting Lastest Hashes from API{Environment.NewLine}";
            ThreadPool.QueueUserWorkItem(delegate
            {
                DisableButtons();
                ScanPackage scanPackage = new ScanPackage();
                string fileExtract = _programLocation + "\\temp\\";
                scanPackage.GetLatestHashes();
                if (Directory.Exists(fileExtract))
                {
                    Directory.Delete(fileExtract, true);
                }
                ExtractPackage(fileExtract);
                SafeText(txtLog,$"Starting Package Scan{Environment.NewLine}");
                FileStats filestat = scanPackage.CheckFiles(fileExtract, chkDeleteWebhook.Checked, chkDeleteDll.Checked, chkDeleteCs.Checked);
                SafeText(txtLog, $"Scan Complete{Environment.NewLine}Good Files: {filestat.SafeFiles}{Environment.NewLine}Bad Files: {filestat.BadFiles}{Environment.NewLine}Unknown Files: {filestat.UnknownFiles}{Environment.NewLine}Possible Webhook Files: {filestat.UrlDelete}{Environment.NewLine}Files that couldn't be deleted: {filestat.NoDelete}{Environment.NewLine}Other files (cs/dll) deleted: {filestat.OtherDelete}{Environment.NewLine}Check logs for more details{Environment.NewLine}");

                if (filestat.BadFiles > 0 || filestat.OtherDelete > 0)
                {
                    SafeText(txtLog, $"Repacking into clean unity package{Environment.NewLine}");

                    var extensions = new List<string>()
                        {
                            "meta"
                        };

                    string fileNameWithout = Path.GetFileNameWithoutExtension(_fileLocation);
                    string extractedPath = $@"{fileExtract}{fileNameWithout}\Assets\";

                    var pack = Package.FromDirectory(extractedPath, fileNameWithout, true, extensions.ToArray(), new string[0]);
                    pack.GeneratePackage(saveLocation: fileExtract);
                    SafeText(txtLog, $"Repacking Complete find new clean package at {fileExtract}{fileNameWithout}{Environment.NewLine}");
                }
                EnableButtons();
            });
        }      

        private bool ExtractPackage(string fileOutput = "")
        {
            SafeText(txtLog, $"Starting package extract{Environment.NewLine}");
            PackageExtractor packageExtractor = new PackageExtractor();
            if (fileOutput == "")
            {
                var folderDlg = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
                var result = folderDlg.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    fileOutput = folderDlg.FileName;
                }
            }
            if (!string.IsNullOrEmpty(fileOutput))
            {
                packageExtractor.ExtractPackage(_fileLocation, fileOutput);
                SafeText(txtLog, $"Extract Success{Environment.NewLine}");
                return true;
            }
            else
            {
                SafeText(txtLog, "Please select where to extract");
                return false;
            }
        }

        private void btnExtractFile_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                DisableButtons();
                ExtractPackage();
                EnableButtons();
            });
        }

        private void DisableButtons()
        {
            SafeToggle(btnLoadFile, false);
            SafeToggle(btnScanFile, false);
            SafeToggle(btnExtractFile, false);
        }

        private void EnableButtons()
        {
            SafeToggle(btnLoadFile, true);
            SafeToggle(btnScanFile, true);
            SafeToggle(btnExtractFile, true);
        }

        private static void SafeToggle(Button button, bool enable)
        {
            if (button.InvokeRequired)
            {
                button.Invoke((MethodInvoker)delegate
                {
                    button.Enabled = enable;
                });
            }
        }

        private static void SafeText(TextBox text, string textWanted)
        {
            if (text.InvokeRequired)
            {
                text.Invoke((MethodInvoker)delegate
                {
                    text.Text += textWanted;
                });
            }
        }
    }
}
