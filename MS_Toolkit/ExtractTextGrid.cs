using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace MS_Toolkit
{
    class ExtractTextGrid:MTask
    {
        ConfigExtractTextGrid Cfg = new ConfigExtractTextGrid();
        #region Regs

        const string BlockRegTemplate = "{0}\\s*\\[([0-9]+)\\]";
        const string ITEM = "item";
        const string INTERVALS = "intervals";
        const string POINTS = "points";

        static Regex ItemReg = CreateReg(BlockRegTemplate, ITEM);
        static Regex IntervalReg = CreateReg(BlockRegTemplate, INTERVALS);
        static Regex PointReg = CreateReg(BlockRegTemplate, POINTS);

        const string NumValueTemplate = "{0}\\s*=\\s*(.*)";
        const string StrValueTemplate = "{0}\\s*=\\s*\"(.*)\"";
        const string XMIN = "xmin";
        const string XMAX = "xmax";
        const string TEXT = "text";
        const string NUMBER = "number";
        const string MARK = "mark";
        const string NAME = "name";

        static Regex XminReg = CreateReg(NumValueTemplate, XMIN);
        static Regex XmaxReg = CreateReg(NumValueTemplate, XMAX);
        static Regex TextReg = CreateReg(StrValueTemplate, TEXT);
        static Regex NumberReg = CreateReg(NumValueTemplate, NUMBER);
        static Regex MarkReg = CreateReg(StrValueTemplate, MARK);
        static Regex NameReg = CreateReg(StrValueTemplate, NAME);

        #endregion

        public ExtractTextGrid() : base()
        {

        }

        public override void Run()
        {
            ProcessFolder(Cfg.InputFolder, Cfg.OutputFolder);
        }

        protected override void LoadLocalConfig(XmlNode taskNode)
        {
            
        }

        static Regex CreateReg(string template, string value)
        {
            return new Regex(string.Format(template, value));
        }

        private void ProcessFolder(string inputFolder, string outputFolder)
        {
            foreach(string textGridFilePath in Directory.EnumerateFiles(inputFolder, "*.textgrid"))
            {
                string nameCore = textGridFilePath.Split('\\').Last().Split('.')[0];
                var list = File.ReadLines(textGridFilePath);
                var tgList = GetTextGrids(list);
                SplitTextGrids(tgList, outputFolder, nameCore);
            }
        }

        private void SplitTextGrids(IEnumerable<TextGrid> tgList, string outputFolder, string nameCore)
        {
            const string FileNameTemplate = "{0}_{1}_{2}.txt";
            var groups = tgList.GroupBy(x => x.ItemId).Select(x => x.OrderBy(y => y.Id));
            foreach(var group in groups)
            {
                string fileName = string.Format(FileNameTemplate, nameCore, group.First().ItemId.ToString("00"), group.First().ItemName);
                string filePath = Path.Combine(outputFolder, fileName);
                var list = group.Select(x => x.Output());
                File.WriteAllLines(filePath, list);
            }
        }

        private IEnumerable<TextGrid> GetTextGrids(IEnumerable<string> list)
        {
            int itemId = 0;
            string itemName = string.Empty;
            TextGrid tg = new TextGrid();
            foreach(string line in list)
            {
                if (ItemReg.IsMatch(line))
                {
                    itemId = int.Parse(ItemReg.Match(line).Groups[1].Value);
                    tg = new TextGrid();                    
                    continue;
                }
                if (NameReg.IsMatch(line))
                {
                    itemName = NameReg.Match(line).Groups[1].Value;
                    continue;
                }

                if (PointReg.IsMatch(line))
                {
                    tg.Id = int.Parse(PointReg.Match(line).Groups[1].Value);
                    continue;
                }
                if (NumberReg.IsMatch(line))
                {
                    tg.NumberStr = NumberReg.Match(line).Groups[1].Value;
                    continue;
                }
                if (MarkReg.IsMatch(line))
                {
                    tg.Value = MarkReg.Match(line).Groups[1].Value;
                    tg.ItemId = itemId;
                    tg.ItemName = itemName;
                    yield return tg;
                    continue;
                }

                if (IntervalReg.IsMatch(line))
                {
                    tg.Id = int.Parse(IntervalReg.Match(line).Groups[1].Value);
                    continue;
                }
                if (XminReg.IsMatch(line))
                {
                    tg.XMinStr = XminReg.Match(line).Groups[1].Value;
                    continue;
                }
                if (XmaxReg.IsMatch(line))
                {
                    tg.XMaxStr = XmaxReg.Match(line).Groups[1].Value;
                    continue;
                }
                if (TextReg.IsMatch(line))
                {
                    tg.Value = TextReg.Match(line).Groups[1].Value;
                    tg.ItemId = itemId;
                    tg.ItemName = itemName;
                    yield return tg;
                    continue;
                }
            }
        }
    }

    class TextGrid
    {
        public int ItemId = 0;
        public string ItemName = string.Empty;

        public string XMinStr = string.Empty;
        public string XMaxStr = string.Empty;

        public string NumberStr = string.Empty;

        public string Value = string.Empty;
        public int Id = 0;
        public string Output()
        {
            return string.IsNullOrWhiteSpace(NumberStr) 
                ? string.Join("\t", OutputIntervals()) 
                : string.Join("\t", OutputPoints());
        }

        private IEnumerable<string> OutputIntervals()
        {
            yield return Id.ToString();
            yield return XMinStr;
            yield return XMaxStr;
            yield return Value;
        }

        private IEnumerable<string> OutputPoints()
        {
            yield return Id.ToString();
            yield return NumberStr;
            yield return Value;
        }
    }
}
