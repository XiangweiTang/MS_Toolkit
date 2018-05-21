using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Toolkit
{
    abstract class MTask
    {
        public MTask() { }

        public abstract void LoadConfig(string configPath);

        public abstract void Run();
    }
}
