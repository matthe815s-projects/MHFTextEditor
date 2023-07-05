using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestEditor.Utils
{
    public class BinUtils
    {
        public static string binDirectory = "quests";
        public static string backupDirectory = "backups";

        public static void CreateIfNotExists ()
        {
            if (!Directory.Exists(BinUtils.binDirectory))
            {
                Directory.CreateDirectory(BinUtils.binDirectory);
            }

            if (!Directory.Exists(BinUtils.backupDirectory))
            {
                Directory.CreateDirectory(BinUtils.backupDirectory);
            }
        }

        public static string[] CheckForBins ()
        {
            CreateIfNotExists();

            List<string> bins = new List<string>();

            foreach (string file in Directory.GetFiles(BinUtils.binDirectory))
            {
                if (!file.EndsWith(".bin")) continue;
                bins.Add(string.Format("./{0}", file));
            }

            foreach (string file in Directory.GetFiles("./"))
            {
                if (!file.EndsWith(".bin")) continue;
                bins.Add(file);
            }

            return bins.ToArray();
        }
    }
}
