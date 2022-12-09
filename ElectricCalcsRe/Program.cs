using System.Runtime.CompilerServices;

namespace ElectricCalcs
{
    public static class Writer
    {
        public static bool Active = true;
        public static int currentId = 0;
        public static int ActionId => ++currentId;
        public static T Get<T>(this List<T> ls, int index = 0)
        {
            var val = ls[index];
            ls.RemoveAt(index);
            return val;
        }
        public static T GetLast<T>(this List<T> ls)
        {
            var val = ls.Last();
            ls.RemoveAt(ls.Count - 1);
            return val;
        }
        public static void Next(string act)
        {
            if (Active)
                Console.WriteLine($"{ActionId}) {act}");
            else
                ++currentId;
        }
    }

    public class Program
    {
        public static List<string> strmod = File.ReadAllLines("sheme2.txt").ToList().Select(i => i.Replace("\t", String.Empty)).ToList();
        public static double GetP(int n, int m)
        {
            Dictionary<string, double> repls = new Dictionary<string, double>();
            repls.Add("R1", m);
            repls.Add("R2", n);
            repls.Add("R3", m / 2);
            repls.Add("R4", n / 3);
            repls.Add("R5", m + 2);
            repls.Add("R6", 2 * n);
            repls.Add("R7", 3 * m);
            repls.Add("R8", 9 + n);
            repls.Add("R9", m + n);
            repls.Add("R10", Math.Abs(m - n));
            if (n % 2 == 0)
                repls.Add("Ua", 10 * n);
            else
                repls.Add("Ia", Math.Abs(m - n) + 1);
            ModelParcer parcer = new ModelParcer(new List<string>(strmod), repls);
            var model = parcer.ParceUniversal();
            if (repls.Last().Key != "U7")
            {
                if (repls.Last().Key == "Ua")
                    model.U = repls.Last().Value;
                else
                    model.I = repls.Last().Value;

            }
            model.CalculateValues();
            return model.P;
        }
        static void Main(string[] args)
        {
            Console.Write("Enter mode: ");
            string mode = Console.ReadLine();
            Console.WriteLine(mode);
            Console.Write("Enter N: ");
            double n = double.Parse(Console.ReadLine());
            Console.WriteLine(n);
            Console.Write("Enter M: ");
            double m = double.Parse(Console.ReadLine());
            Console.WriteLine(m);
            Dictionary<string, double> repls = new Dictionary<string, double>();
            switch (mode)
            {
                case "cap":
                    repls.Add("C1", m + n);
                    repls.Add("C2", Math.Abs(m - n));
                    repls.Add("C3", Math.Sqrt(n));
                    repls.Add("C4", Math.Pow(m, 2));
                    repls.Add("C5", m / n);
                    repls.Add("C6", m * n);
                    repls.Add("C7", 2 * m);
                    repls.Add("U7", m * n * 10);
                    break;
                case "res":
                    repls.Add("R1", m);
                    repls.Add("R2", n);
                    repls.Add("R3", m / 2);
                    repls.Add("R4", n / 3);
                    repls.Add("R5", m + 2);
                    repls.Add("R6", 2 * n);
                    repls.Add("R7", 3 * m);
                    repls.Add("R8", 9 + n);
                    repls.Add("R9", m + n);
                    repls.Add("R10", Math.Abs(m - n));
                    if (n % 2 == 0)
                        repls.Add("Ua", 10 * n);
                    else
                        repls.Add("Ia", Math.Abs(m - n) + 1);
                    break;
            }
            repls.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));
            ModelParcer parcer = new ModelParcer(mode switch { "cap" => "shemeCapLabElvin.txt", "res" => "sheme2.txt" }, repls);
            var model = parcer.ParceUniversal();
            if (repls.Last().Key != "U7")
            {
                if (repls.Last().Key == "Ua")
                    model.U = repls.Last().Value;
                else
                    model.I = repls.Last().Value;

            }
            model.CalculateValues();
            Console.ReadLine();
        }
    }
}