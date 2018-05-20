using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Toolkit
{
    abstract class Line
    {
        public int ID = 0;
        public double StartTime = 0.0;
        public double EndTime = 0.0;
        public string SpeakerId = string.Empty;
        public string SessionId = string.Empty;
        public string Text = string.Empty;
        public string AudioPath = string.Empty;
    }

    class IntervalLine:Line
    {
        public IntervalLine(string line)
        {
            var split = line.Split('\t');
            ID = int.Parse(split[0]);
            StartTime = double.Parse(split[1]);
            EndTime = double.Parse(split[2]);
            Text = split[3];
            AudioPath = split[4];
        }
    }

    class PointLine:Line
    {
        public PointLine(string line, double range)
        {
            var split = line.Split('\t');
            ID = int.Parse(split[0]);
            double midTime = double.Parse(split[1]);
            StartTime = midTime - range;
            EndTime = midTime + range;
            Text = split[2];
            AudioPath = split[3];
        }
    }
}
