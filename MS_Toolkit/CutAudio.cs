using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace MS_Toolkit
{
    class CutAudio:MTask
    {
        ConfigCutAudio Cfg = new ConfigCutAudio();
        public override void Run()
        {
            var list = Directory.EnumerateFiles(Cfg.InputAudioPath, ".wav");
            foreach(string audioPath in list)
            {
                string transPath = Path.Combine(Cfg.InputTransPath, audioPath.Split('\\').Last().Replace(".wav", ".txt"));
                CutSingleAudio(audioPath, transPath);
            }
        }

        protected override void LoadLocalConfig(XmlNode localTaskNode)
        {
        }

        private void CutSingleAudio(string audioPath, string lineListPath)
        {
            var list = File.ReadLines(lineListPath).Select(x => new IntervalLine(x));
            foreach(Line line in list)
            {
                string inputAudioPath = line.AudioPath;
                string outputAudioPath = Path.Combine(Cfg.OutputAudioRootPath, line.SpeakerId, line.SessionId);
                double duration = line.EndTime - line.StartTime;
                CutAudioBySox(inputAudioPath, outputAudioPath, line.StartTime, duration);
            }
        }

        private void CutAudioBySox(string inputAudioPath, string outputAudioPath, double startTime, double duration)
        {
            string args = $"{inputAudioPath} {outputAudioPath} trim {startTime} {duration}";
            Common.RunFile(Cfg.SoxPath, args, string.Empty, false);
        }
    }
}
