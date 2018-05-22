using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MS_Toolkit
{
    static class Common
    {
        public static void RunFile(string filePath, string args, string workingDirectory, bool useShell)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = filePath;
            info.Arguments = args;
            if (!string.IsNullOrWhiteSpace(workingDirectory))
                info.WorkingDirectory = workingDirectory;
            info.UseShellExecute = useShell;

            using(Process proc=new Process())
            {
                proc.StartInfo = info;
                proc.Start();
                proc.WaitForExit();
            }
        }

        public static byte[] ReadBytes(string path, int n)
        {
            using(FileStream fs=new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    return br.ReadBytes(n);
                }
            }
        }
    }
}
