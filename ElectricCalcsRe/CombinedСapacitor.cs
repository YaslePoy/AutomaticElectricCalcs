using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCalcs
{
    public class CombinedCapacitor : Capacitor
    {
        public List<Capacitor> inside;
        public CombinedCapacitor(ResistorType type)
        {
            inside = new List<Capacitor>();
            Type = type;
        }
        public CombinedCapacitor(List<Capacitor> parts, ResistorType type)
        {
            inside = parts;
            Type = type;
        }
        public override string ToString()
        {

            return $"{Enum.GetName(Type)} capacitor[{Id}] C:{Math.Round(C, 3)} pF U:{Math.Round(U, 3)} V Q:{Math.Round(Q, 3)} nC";
        }
        public override bool IsFull => inside.All(x => x.IsFull) && base.IsFull;
        public override string Id => string.Join("", inside.Select(x => x.Id));
        public override void UpdateRUI()
        {
            if (inside.All(Q => Q.C != 0) && C == 0)
            {
                switch (Type)
                {
                    case ResistorType.Sequential:
                        C = 1 / inside.Select(Q => 1 / Q.C).Sum();
                        Writer.Next($" C{Id} = 1 / ({String.Join(" + ", inside.Select(Q => $"(1 / C{Q.Id})"))}) = 1 / ({String.Join(" + ", inside.Select(Q => $"(1 / {Math.Round(Q.C, 3)}^-12)"))} = {Math.Round(C, 3)} pF");
                        break;
                    case ResistorType.Parallel:
                        C = inside.Sum(Q => Q.C);
                        Writer.Next($" C{Id} = {String.Join(" + ", inside.Select(Q => $"C{Q.Id}"))} = {String.Join(" + ", inside.Select(Q => $"{Math.Round(Q.C, 3)}"))} = {Math.Round(C, 3)} pF");
                        break;
                }
                return;
            }

            if (!base.IsFull)
            {
                if (Q != 0 && C != 0)
                {
                    U = Q / C * 1000;
                    Writer.Next($"U{Id} = Q{Id} / C{Id} = {Math.Round(Q, 3)}^-12 / {Math.Round(C, 3)}^-9 = {Math.Round(U, 3)} V");
                    return;
                }
                else if (U != 0 && C != 0)
                {
                    Q = U * C / 1000;
                    Writer.Next($"Q{Id} = U{Id} * C{Id} = {Math.Round(U, 3)} * {Math.Round(C, 3)}^-12 = {Math.Round(Q, 3)} nC");
                }
            }
            inside.Where(Q => Q.IsFull == false).ToList().ForEach(x => x.UpdateRUI());

            UprateConstansParam();
        }
        public void UprateConstansParam()
        {
            switch (Type)
            {
                case ResistorType.Parallel:
                    if (U == 0)
                    {
                        U = inside.Select(i => i.U).Max();
                        var nn = inside.FirstOrDefault(i => i.U != 0);
                        if (nn != null)
                            Writer.Next($"U{Id} = U{nn.Id} = {U} V");
                    }
                    if (U == 0)
                        break;
                    for (int i = 0; i < inside.Count; i++)
                    {
                        if (inside[i].U == 0)
                            Writer.Next($" U{inside[i].Id} = U{Id} = {Math.Round(U, 3)} V");
                        inside[i].U = U;
                    }

                    break;
                case ResistorType.Sequential:
                    if (Q == 0)
                    {
                        Q = inside.Select(i => i.Q).Max();
                        var nn = inside.FirstOrDefault(i => i.Q != 0);
                        if (nn != null)
                            Writer.Next($"Q{Id} = Q{nn.Id} = {Q} nC");
                    }
                    if (Q == 0)
                        break;
                    for (int i = 0; i < inside.Count; i++)
                    {
                        if (inside[i].Q == 0)
                            Writer.Next($" Q{inside[i].Id} = Q{Id} = {Math.Round((decimal)Q, 3)} nC");
                        inside[i].Q = Q;
                    }
                    break;
            }
        }
        public override List<Capacitor> GetInside()
        {
            List<Capacitor> ins = new List<Capacitor>();
            foreach (var lc in inside)
                ins.AddRange(lc.GetInside());
            return ins;
        }
        public void CalculateValues()
        {
            while (IsFull == false)
            {
                int logIndex = Writer.currentId;
                UpdateRUI();
                if (logIndex == Writer.currentId)
                    throw new Exception("Imposible to calulate all values in this model");
            }
            Console.WriteLine(this);
            Console.WriteLine("----------Total----------");
            Console.WriteLine($"Total C = {Math.Round(this.C, 3)} nC");
            Console.WriteLine("----------Сharge----------");
            var countedC = this.GetInside();
            countedC.ForEach(i => Console.WriteLine($"Q{i.Id} = {Math.Round(i.Q, 3)} pF"));
            Console.WriteLine($"Total Q = {Math.Round(this.Q, 3)} pF");
            Console.WriteLine("----------Voltage----------");
            countedC.ForEach(i => Console.WriteLine($"U{i.Id} = {Math.Round(i.U, 3)} V"));
            Console.WriteLine($"Total U = {Math.Round(this.U, 3)} V");
            Console.WriteLine("----------Energy----------");
            countedC.ForEach(i => Console.WriteLine($"W{i.Id} = (Q{i.Id} * U{i.Id}) / 2 = ({Math.Round(i.Q, 3)}^-9 * {Math.Round(i.U, 3)}) / 2 = {Math.Round(i.W, 3)} mkJ"));
            Console.WriteLine($"Total W = {Math.Round(this.W, 3)} mkJ");
        }
    }
}
