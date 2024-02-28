using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;


namespace CorePayments.SemanticKernel
{



    internal static class RepoFiles
    {
        /// <summary>
        /// Scan the local folders from the repo, looking for "samples/plugins" folder.
        /// </summary>
        /// <returns>The full path to samples/plugins</returns>
        public static string SamplePluginsPath()
        {
            const string Parent = "CorePayments.SemanticKernel";
            const string Folder = "Plugins\\SummarizePlugin";

            bool SearchPath(string pathToFind, out string result, int maxAttempts = 10)
            {
                var currDir = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
                bool found;
                do
                {
                    result = Path.Join(currDir, pathToFind);
                    found = Directory.Exists(result);
                    currDir = Path.GetFullPath(Path.Combine(currDir, ".."));
                } while (maxAttempts-- > 0 && !found);

                return found;
            }

            if (!SearchPath(Parent + Path.DirectorySeparatorChar + Folder, out string path)
                && !SearchPath(Folder, out path))
            {
                throw new DirectoryNotFoundException("Plugins directory not found. The app needs the plugins from the repo to work.");
            }

            return path;
        }
    }

}