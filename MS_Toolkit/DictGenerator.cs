using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MS_Toolkit
{
    class DictGenerator:MTask
    {
        public override void Run()
        {
            throw new NotImplementedException();
        }
        protected override void LoadLocalConfig(XmlNode localTaskNode)
        {
            base.LoadLocalConfig(localTaskNode);
        }

        List<string> MismatchList = new List<string>();

        private IEnumerable<Tuple<char,string>> DictMatch(IEnumerable<Line> ccList, IEnumerable<Line> sylList)
        {
            foreach(Line ccLine in ccList)
            {
                double start = ccLine.StartTime;
                double end = ccLine.EndTime;
                string cleanLine = CleanUp(ccLine.Text);
                var sylSubList = sylList.Where(x => x.StartTime >= start && x.EndTime <= end && x.Text != "sil").Select(x=>x.Text);
                if (cleanLine.Count() != sylSubList.Count())
                    MismatchList.Add(cleanLine + "\t" + string.Join(" ", sylSubList));
                else
                {
                    foreach (var item in cleanLine.Zip(sylSubList, (x, y) => new Tuple<char, string>(x, y)))
                        yield return item;
                }
            }
        }

        private string CleanUp(string trans)
        {
            return new string(trans.Where(x => !char.IsPunctuation(x)).ToArray());
        }
    }
}
