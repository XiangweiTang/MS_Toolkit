using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Toolkit
{
    class Config
    {

    }
    
    class ConfigExtractTextGrid:Config
    {
        public string InputFolder { get; private set; } = string.Empty;
        public string OutputFolder { get; private set; } = string.Empty;
    }
}
