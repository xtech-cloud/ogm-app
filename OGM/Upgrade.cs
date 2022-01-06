using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using XTC.oelUpgrade;

namespace OGM
{
    public class Upgrade
    {
        public XTC.oelMVCS.Logger logger { get; set; }

        private string version_ { get; set; }
        public class Config
        {
            public string patch_repository { get; set; }
            public string update_repository { get; set; }

        }

        public void Run()
        {
            string curDir = System.IO.Directory.GetCurrentDirectory();
            string file = Path.Combine(curDir, "upgrade.json");
            if (!File.Exists(file))
                return;

            Config config = null;
            version_ = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                string content = File.ReadAllText(file);
                config = JsonSerializer.Deserialize<Config>(content);
            }
            catch (System.Exception ex)
            {

            }

            if (null == config)
                return;

            Patch.Args patchArgs = new Patch.Args();
            patchArgs.repository = config.patch_repository;
            Patch patcher = new Patch();
            patcher.onSuccess = () =>
            {
                logger.Info("patch updrade success");
            };
            patcher.onFailure = (_err) =>
            {
                logger.Error("patch updrade failure: {0}", _err);
            };
            patcher.onStatus = (_progress, _tip) => { };
            patcher.PullRpository(patchArgs, (_repo) =>
            {
                if (null == _repo)
                    return;
                if (patcher.CompareVersion(version_, _repo.version) < 0)
                {
                    if (Patch.Strategy.Auto.ToString() == _repo.strategy)
                    {
                        // 自动开始更新
                        runUpgrader(config);
                    }
                    else if (Patch.Strategy.Auto.ToString() == _repo.strategy)
                    {
                        // 显示更新提示
                    }
                }
                else
                {
                    Update.Args updateArgs = new Update.Args();
                    updateArgs.repository = config.update_repository;
                    Update updater = new Update();
                    updater.onSuccess = () =>
                    {
                        logger.Info("updat updrade success");
                    };
                    updater.onFailure = (_err) =>
                    {
                        logger.Error("update updrade failure: {0}", _err);
                    };
                    updater.onStatus = (_progress, _tip) => { };
                    updater.PullRpository(updateArgs, (_repo) =>
                    {
                        if (null == _repo)
                            return;
                        if (!updater.Match(_repo, System.IO.Directory.GetCurrentDirectory()))
                        {
                            // 自动开始更新
                            runUpgrader(config);
                        }

                    }, (_error) =>
                     {
                     });

                }
            }, (_error) =>
             {
             });
        }

        private void runUpgrader(Config _config)
        {
            string srcdir = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "upgrade");
            string distdir = Path.Combine(System.IO.Path.GetTempPath(), "OGM-Upgrade");
            //拷贝upgrade到临时目录
            if (Directory.Exists(srcdir))
            {
                copyFolder(srcdir, distdir);
            }
            string workdir = System.IO.Directory.GetCurrentDirectory();
            string self_exe = Path.Combine(workdir, "OGM.exe");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(distdir, "upgrade/Upgrade.WPF.exe");
            string args = string.Format("--patch-repository={0} --patch-version={1} --patch-target={2} --update-repository={3} --update-target={4} --program-path={5} --program-workdir={6}", _config.patch_repository, version_, workdir, _config.update_repository, workdir, self_exe, workdir);
            psi.Arguments = args;
            psi.WorkingDirectory = workdir;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);

            Application.Current.MainWindow.Close();
        }
        private void copyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                string folderName = System.IO.Path.GetFileName(sourceFolder);
                string destfolderdir = System.IO.Path.Combine(destFolder, folderName);
                string[] filenames = System.IO.Directory.GetFileSystemEntries(sourceFolder);
                foreach (string file in filenames)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        string currentdir = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(currentdir))
                        {
                            System.IO.Directory.CreateDirectory(currentdir);
                        }
                        copyFolder(file, destfolderdir);
                    }
                    else
                    {
                        string srcfileName = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(destfolderdir))
                        {
                            System.IO.Directory.CreateDirectory(destfolderdir);
                        }
                        System.IO.File.Copy(file, srcfileName, true);
                    }
                }
            }
            catch (System.Exception e)
            {

            }
        }
    }
}
