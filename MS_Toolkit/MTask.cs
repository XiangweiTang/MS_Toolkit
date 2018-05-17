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

        public void LoadConfig(string path)
        {

        }

        protected virtual void LoadLocalConfig(XmlNode localTaskNode)
        {

        }

        public abstract void Run();
    }
}
