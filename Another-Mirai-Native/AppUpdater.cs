using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public static class AppUpdater
    {
        public async static Task<string?> DownloadVersion(UpdateModel.UpdateItem version)
        {
            string path = "appUpdate";
            Directory.CreateDirectory(path);
            if (version == null)
            {
                return null;
            }
            if (await Helper.DownloadFile(version.DownloadUrl, Path.GetFileName(version.DownloadUrl), path, true))
            {
                return Path.Combine(path, Path.GetFileName(version.DownloadUrl));
            }
            return null;
        }

        public async static Task<UpdateModel.UpdateItem?> GetLatestVersion()
        {
            string json = "";
            try
            {
                string url = "http://assets.hellobaka.xyz/static/AMN2/Update.json";
                json = await Helper.DownloadString(url);
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                UpdateModel updateModel = JsonConvert.DeserializeObject<UpdateModel>(json);
                if (updateModel == null)
                {
                    return null;
                }
                updateModel.Rebuild();
                return updateModel.Items.FirstOrDefault(x => x.Version == updateModel.LatestVersion);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(LogLevel.Debug, "GetLatestVersion", json);
                LogHelper.Error("获取更新版本", e);
                return null;
            }
        }
    }
}
