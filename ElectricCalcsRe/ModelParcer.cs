using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCalcs
{
    public enum CalcMode
    {
        Resostor,
        Сapacitor
    }
    public class ModelParcer
    {
        public readonly string ParceType;
        public readonly Dictionary<string , double> Replacing;
        public readonly List<string> StrModel;
        public ModelParcer(string path)
        {
            var strFull = File.ReadAllLines(path).ToList().Select(i => i.Replace("\t", String.Empty)).ToList();
            ParceType = strFull.Get().Split(' ')[1];
            if (strFull.Any(i => i == "new replace"))
            {
                Replacing = new Dictionary<string, double>();
                strFull.Get();
                while (true)
                {
                    var loc = strFull.Get();
                    if (loc == "end replace")
                        break;
                    Replacing.Add(loc.Split(" ")[0], double.Parse(loc.Split(" ")[1], System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            StrModel = strFull;
        }
        public ModelParcer(List<string> model, Dictionary<string, double> replacing)
        {
            
            ParceType = model.Get().Split(' ')[1];
            StrModel = model;
            Replacing = replacing;
        }
        public ModelParcer(string path, Dictionary<string, double> replacing)
        {
            var strFull = File.ReadAllLines(path).ToList().Select(i => i.Replace("\t", String.Empty)).ToList();
            ParceType = strFull.Get().Split(' ')[1];
            StrModel = strFull;
            Replacing = replacing;
        }
        public dynamic ParceUniversal()
        {
            switch(ParceType)
            {
                case "resistor":
                    return ParceResistor();
                case "capitor":
                    return ParceCapitor();
                default:
                    return "Unknown model type";
            }
        }
        public CombinedResistor ParceResistor()
        {
            var loc = StrModel;
            loc.GetLast();
            var tp = loc.Get() switch
            {
                "new par" => ResistorType.Parallel,
                "new seq" => ResistorType.Sequential
            };
            return new CombinedResistor(ParceListRes(loc), tp);
        }
        List<Resistor> ParceListRes(List<string> list)
        {
            List<Resistor> ress = new List<Resistor>();
            var ls = list;
            while (ls.Count != 0)
            {
                Resistor rs;
                var read = ls.Get();
                switch (read)
                {
                    case "new par":
                        var locLs = new List<string>();
                        int a = 1;
                        while (a != 0)
                        {
                            var part = ls.Get();
                            locLs.Add(part);
                            if (part.StartsWith("new"))
                                a++;
                            else if (part.StartsWith("end"))
                                a--;
                        }
                        locLs.GetLast();
                        rs = new CombinedResistor(ParceListRes(locLs), ResistorType.Parallel);
                        break;
                    case "new seq":
                        var locLs1 = new List<string>();
                        int a1 = 1;
                        while (a1 != 0)
                        {
                            var part = ls.Get();
                            locLs1.Add(part);
                            if (part.StartsWith("new"))
                                a1++;
                            else if (part.StartsWith("end"))
                                a1--;
                        }
                        locLs1.GetLast();
                        rs = new CombinedResistor(ParceListRes(locLs1), ResistorType.Sequential);
                        break;
                    default:
                        var pts = read.Split(' ').ToList();
                        rs = new Resistor(pts.Get(1));
                        pts.Get();

                        while (pts.Count != 0)
                        {
                            var strProp = pts.Get(1);
                            double propValue = 0;
                            if (!Double.TryParse(strProp, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out propValue))
                                propValue = Replacing[strProp];
                            switch (pts.Get())
                            {
                                case "R":
                                    rs.R = propValue;
                                    break;
                                case "U":
                                    rs.U = propValue;
                                    break;
                                case "I":
                                    rs.I = propValue;
                                    break;
                            }
                        }
                        break;
                }
                ress.Add(rs);
            }
            return ress;
        }
        public CombinedCapacitor ParceCapitor()
        {
            var loc = StrModel;
            loc.GetLast();
            var tp = loc.Get() switch
            {
                "new par" => ResistorType.Parallel,
                "new seq" => ResistorType.Sequential
            };
            return new CombinedCapacitor(ParceListCap(loc), tp);
        }
        List<Capacitor> ParceListCap(List<string> list)
        {
            List<Capacitor> ress = new List<Capacitor>();
            var ls = list;
            while (ls.Count != 0)
            {
                Capacitor rs;
                var read = ls.Get();
                switch (read)
                {
                    case "new par":
                        var locLs = new List<string>();
                        int a = 1;
                        while (a != 0)
                        {
                            var part = ls.Get();
                            locLs.Add(part);
                            if (part.StartsWith("new"))
                                a++;
                            else if (part.StartsWith("end"))
                                a--;
                        }
                        locLs.GetLast();
                        rs = new CombinedCapacitor(ParceListCap(locLs), ResistorType.Parallel);
                        break;
                    case "new seq":
                        var locLs1 = new List<string>();
                        int a1 = 1;
                        while (a1 != 0)
                        {
                            var part = ls.Get();
                            locLs1.Add(part);
                            if (part.StartsWith("new"))
                                a1++;
                            else if (part.StartsWith("end"))
                                a1--;
                        }
                        locLs1.GetLast();
                        rs = new CombinedCapacitor(ParceListCap(locLs1), ResistorType.Sequential);
                        break;
                    default:
                        var pts = read.Split(' ').ToList();
                        rs = new Capacitor(pts.Get(1));
                        pts.Get();

                        while (pts.Count != 0)
                        {
                            var strProp = pts.Get(1);
                            double propValue = 0;
                            if (!Double.TryParse(strProp, System.Globalization.NumberStyles.Float,  System.Globalization.CultureInfo.InvariantCulture, out propValue))
                                propValue = Replacing[strProp];
                            switch (pts.Get(0))
                            {
                                case "C":
                                    rs.C = propValue;
                                    break;
                                case "U":
                                    rs.U = propValue;
                                    break;
                                case "Q":
                                    rs.Q = propValue;
                                    break;
                            }
                        }
                        break;
                }
                ress.Add(rs);
            }
            return ress;
        }
    }
}
