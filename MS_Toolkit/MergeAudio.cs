using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace MS_Toolkit
{
    class MergeAudio:MTask
    {
        ConfigMergeAudio Cfg = new ConfigMergeAudio();
        public MergeAudio() { }
        public override void Run()
        {
            MergeAudioFolder(Cfg.InputRootFolder, Cfg.OutputRootFolder, Cfg.LinePath);
        }

        protected override void LoadLocalConfig(XmlNode localTaskNode)
        {
            
        }

        private void MergeAudioFolder(string inputRootFolder, string outputRootFolder, string linePath)
        {
            var list = File.ReadAllLines(linePath).Select(x => new IntervalLine(x));
            var groups = list.GroupBy(x => x.SessionId);
            foreach(var group in groups)
            {
                string outputFolder = Path.Combine(outputRootFolder, group.Key);
                List<Line> subList = new List<Line>();
                int count = 0;
                foreach(Line line in group)
                {
                    subList.Add(line);
                    if (subList.Sum(x => x.EndTime - x.StartTime) >= Cfg.MergeSize)
                    {
                        MergeAudioByFfMpeg(subList, outputFolder, count);
                        count++;
                        subList.Clear();
                    }
                }
                if (subList.Count > 0)
                    MergeAudioByFfMpeg(subList, outputFolder, count);
            }
        }

        private void MergeAudioByFfMpeg(IEnumerable<Line> list, string outputFolder, int count)
        {
            var transList = list.Select(x => $"file '{x.AudioPath}'");
            string filePath = Path.Combine(outputFolder, count.ToString("000000") + ".txt");
            File.WriteAllLines(filePath, transList);
            string args = $"{0}{1}";

            Common.RunFile(Cfg.FfmpegPath, args, string.Empty, false);
        }
    }
}
