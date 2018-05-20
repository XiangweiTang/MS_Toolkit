using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Toolkit
{
    class Config
    {
        public string SoxPath { get; private set; } = string.Empty;
        public string FfmpegPath { get; private set; } = string.Empty;        
    }
    
    class ConfigExtractTextGrid:Config
    {
        public string InputFolder { get; private set; } = string.Empty;
        public string OutputFolder { get; private set; } = string.Empty;
    }

    class ConfigMergeAudio : Config
    {
        public string LinePath { get; private set; } = string.Empty;
        public double MergeSize { get; private set; } = 0.0;
        public string InputRootFolder { get; private set; } = string.Empty;
        public string OutputRootFolder { get; private set; } = string.Empty;
    }

    class ConfigCutAudio : Config
    {
        public string InputTransPath { get; private set; } = string.Empty;
        public string InputAudioPath { get; private set; } = string.Empty;
        public string OutputAudioRootPath { get; private set; } = string.Empty;
    }
}
