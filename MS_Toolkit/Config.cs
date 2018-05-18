using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MS_Toolkit
{
    class Config
    {
        string TaskName = string.Empty;
        public void LoadConfig(string configPath)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configPath);
            TaskName = xDoc["Root"].Attributes["TaskName"].Value;
            var taskNode = xDoc[TaskName];
            LoadTask(taskNode);            
        }

        protected virtual void LoadTask(XmlNode taskNode)
        {
            //Do nothing.
        }
    }
    
    class ConfigExtractTextGrid:Config
    {
        public string InputFolder { get; private set; } = string.Empty;
        public string OutputFolder { get; private set; } = string.Empty;
    }
}
