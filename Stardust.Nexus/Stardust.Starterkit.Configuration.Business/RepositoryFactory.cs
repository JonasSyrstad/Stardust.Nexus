﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;
using Stardust.Core.Security;
using Stardust.Nexus.Repository;
using Stardust.Nucleus;
using Stardust.Particles;

namespace Stardust.Nexus.Business
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private static bool initialized;

        public ConfigurationContext GetRepository()
        {
            var repository= (ConfigurationContext)ContainerFactory.Current.Resolve(typeof(ConfigurationContext), Scope.Context, CreateRepository);
            if (!initialized)
            {
                var settings = repository.Settingss.SingleOrDefault();
                if (settings.IsNull())
                {
                    settings = repository.Settingss.Create();
                    var master = UniqueIdGenerator.CreateNewId(26);
                    var masterKey = master.GetByteArray();
                    var protectedKey = Convert.ToBase64String(MachineKey.Protect(masterKey));
                    settings.MasterEncryptionKey =protectedKey;
                    foreach (var configSet in repository.ConfigSets)
                    {
                       var siteCrypt= repository.SiteEncryptionss.Create();
                        siteCrypt.Site = configSet;
                        siteCrypt.Settings = settings;
                        var key = ConfigurationManagerHelper.GetValueOnKey("stardust.ConfigKey");
                        if (key.IsNullOrWhiteSpace())
                        {
                            key = UniqueIdGenerator.CreateNewId(128);
                        }
                        siteCrypt.SiteEncryptionKey = key.Encrypt(new EncryptionKeyContainer(master));
                    }
                }
                repository.SaveChanges();
                initialized = true;
            }
            
            return repository;
        }

        private static ConfigurationContext CreateRepository()
        {
            var connectionString = GetConnectionString();
            var context = new ConfigurationContext(connectionString)
                              {
                                  FilterOptimizationEnabled = true
                              };
            return context;
        }

        public static string GetConnectionString()
        {
            string connectionString;
            var postfix = "Store";
            if (ConfigurationManagerHelper.GetValueOnKey("configStoreMigrationFile").ContainsCharacters())
            {
                if (File.Exists(ConfigurationManagerHelper.GetValueOnKey("configStoreMigrationFile")))
                {
                    postfix = "";
                }
                else
                {
                }
            }
            connectionString = ConfigurationManagerHelper.GetValueOnKey("configStore");
            if (connectionString.IsNullOrWhiteSpace())
            {
                connectionString = "Type=embedded;endpoint=http://localhost:8090/brightstar;StoresDirectory=C:\\Stardust\\Stores;StoreName=configWeb";
            }
            connectionString = connectionString + postfix;
            return connectionString;
        }

        public static FileInfo Backup(string file, string tempDir)
        {
            Logging.DebugMessage("Beginning backup");
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            ZipFile.CreateFromDirectory(tempDir, file, CompressionLevel.Fastest, true);
            Logging.DebugMessage("Backup completed");
            ClearTempDir(tempDir);
            Logging.DebugMessage("Cleanup completed");
            var finfo = new FileInfo(file);
            return finfo;
        }

        private static void ClearTempDir(string tempDir)
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }

        public static string CreateTempDir(string id, string path)
        {
            var tempDir = ConfigurationManagerHelper.GetValueOnKey("stardust.StoreLocation") + "\\bck_temp\\" + id;
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);
            foreach (var enumerateFile in Directory.EnumerateFiles(path))
            {
                var finfo = new FileInfo(enumerateFile);
                finfo.CopyTo(tempDir + "\\" + finfo.Name, true);
            }
            return tempDir;
        }

        public static void Backup()
        {
            var id = GetConnectionString().Split(';').Last().Split('=').Last();
            var path = Path.Combine(ConfigurationManagerHelper.GetValueOnKey("stardust.StoreLocation"));
            var tempDir = CreateTempDir(id, path);
            var file = string.Format("{0}\\{1}_{2}.zip", ConfigurationManagerHelper.GetValueOnKey("stardust.StoreLocation"), id, DateTime.Now.ToString("yyMMdd"));
            Task.Run(() => Backup(file, tempDir));
        }
    }
}