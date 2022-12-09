using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCalcs
{
    public enum ResistorType
    {
        Single,
        Parallel,
        Sequential
    }
    public class CombinedResistor : Resistor
    {
        public List<Resistor> inside;
        public CombinedResistor(ResistorType type)
        {
            inside = new List<Resistor>();
            Type = type;
        }
        public CombinedResistor(List<Resistor> parts, ResistorType type)
        {
            inside = parts;
            Type = type;
        }
        public override string ToString()
        {
            return $"{Enum.GetName(Type)} resistor[{Id}] R:{Math.Round(R, 3)} U:{Math.Round(U, 3)} I:{Math.Round(I, 3)}";
        }
        public override bool IsFull => inside.All(x => x.IsFull) && base.IsFull;
        public override string Id => string.Join("", inside.Select(x => x.Id));
        public override void UpdateRUI()
        {
            if (!base.IsFull)
            {
                if (I != 0 && R != 0)
                {
                    U = I * R;
                    Writer.Next($"U{Id} = R{Id} * I{Id} = {Math.Round(R, 3)} * {Math.Round(I, 3)} = {Math.Round(U, 3)} V");
                    return;
                }
                else if (U != 0 && R != 0)
                {
                    I = U / R;
                    Writer.Next($"I{Id} = U{Id} / R{Id} = {Math.Round(U, 3)} / {Math.Round(R, 3)} = {Math.Round(I, 3)} A");
                }
            }
            inside.Where(i => i.IsFull == false).ToList().ForEach(x => x.UpdateRUI());
            if (inside.All(i => i.R != 0) && R == 0)
                switch (Type)
                {
                    case ResistorType.Parallel:
                        R = 1 / inside.Select(i => 1 / i.R).Sum();
                        Writer.Next($" R{Id} = 1 / ({String.Join(" + ", inside.Select(i => $"1 / R{i.Id}"))}) = 1 / ({String.Join(" + ", inside.Select(i => $"1 / {Math.Round(i.R, 3)}"))}) = {Math.Round(R, 3)} Om");
                        break;
                    case ResistorType.Sequential:
                        R = inside.Sum(i => i.R);
                        Writer.Next($" R{Id} = {String.Join(" + ", inside.Select(i => $"R{i.Id}"))} = {String.Join(" + ", inside.Select(i => $"{Math.Round(i.R, 3)}"))} = {Math.Round(R, 3)} Om");
                        break;
                }
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
                    if (I == 0)
                    {
                        I = inside.Select(i => i.I).Max();
                        var nn = inside.FirstOrDefault(i => i.I != 0);
                        if (nn != null)
                            Writer.Next($"I{Id} = I{nn.Id} = {U} A");
                    }
                    if (I == 0)
                        break;
                    for (int i = 0; i < inside.Count; i++)
                    {
                        if (inside[i].I == 0)
                            Writer.Next($" I{inside[i].Id} = I{Id} = {Math.Round(I, 3)} A");
                        inside[i].I = I;
                    }
                    break;
            }
        }
        public override List<Resistor> GetInside()
        {
            List<Resistor> ins = new List<Resistor>();
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
                    return;
            }
            Console.WriteLine(this);
            Console.WriteLine("----------Total----------");
            Console.WriteLine($"Total R = {Math.Round(this.R, 3)} Om");
            Console.WriteLine("----------Сurrent----------");
            var counted = this.GetInside();
            counted.ForEach(i => Console.WriteLine($"I{i.Id} = {Math.Round(i.I, 3)} A"));
            Console.WriteLine($"Total I = {Math.Round(this.I, 3)}");
            Console.WriteLine("----------Voltage----------");
            counted.ForEach(i => Console.WriteLine($"U{i.Id} = {Math.Round(i.U, 3)} V"));
            Console.WriteLine($"Total U = {Math.Round(this.U, 3)}");
            Console.WriteLine("----------Power----------");
            counted.ForEach(i => Console.WriteLine($"P{i.Id} = U{i.Id} * I{i.Id} = {Math.Round(i.U, 3)} * {Math.Round(i.I, 3)} = {Math.Round(i.P, 3)} W"));
            Console.WriteLine($"Total P = total U * total I = {Math.Round(U, 3)} * {Math.Round(I, 3)} = {Math.Round(this.P, 3)}");
        }
    }
}
