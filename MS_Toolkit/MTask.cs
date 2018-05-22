using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MS_Toolkit
{
    abstract class MTask
    {
        public MTask() { }

        public void LoadConfig(string configPath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            using(XmlReader reader = XmlReader.Create(configPath, settings))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(reader);
                string taskName = xDoc["Root"].Attributes["TaskName"].Value;
                var taskNode = xDoc["Root"][taskName];
                LoadLocalConfig(taskNode);
            }
        }

        protected virtual void LoadLocalConfig(XmlNode taskNode)
        {
            //Do nothing
        }

        public abstract void Run();
    }
}
