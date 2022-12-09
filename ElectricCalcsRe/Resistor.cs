using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCalcs
{
    public class Resistor
    {
        public ResistorType Type;
        public virtual string Id => id;
        readonly string id = "noID";
        public double R, U, I;
        public Resistor()
        {
            R = U = I = 0;
            Type = ResistorType.Single;
        }
        public Resistor(string id)
        {
            R = U = I = 0;
            Type = ResistorType.Single;
            this.id = id;
        }
        public Resistor(double r, double u, double i, string id)
        {
            this.id = id;
            this.R = r;
            this.U = u;
            this.I = i;
            Type = ResistorType.Single;
        }
        public virtual void UpdateRUI()
        {
            if (IsFull)
                return;
            if (I != 0 && R != 0)
            {
                U = I * R;
                Writer.Next($" U{Id} = R{Id} * I{Id} = {Math.Round(R, 3)} * {Math.Round(I, 3)} = {Math.Round(U, 3)} V");
                return;
            }
            if (U != 0 && R != 0)
            {
                I = U / R;
                Writer.Next($" I{Id} = U{Id} / R{Id} = {Math.Round(U, 3)} / {Math.Round(R, 3)} = {Math.Round(I, 3)} A");
                return;
            }
            if (I != 0 && U != 0)
            {
                R = U / I;
                Writer.Next($" R{Id} = U{Id} / I{Id} = {Math.Round(U, 3)} / {Math.Round(I, 3)} = {Math.Round(R, 3)} Om");
                return;
            }
        }
        public double P => U * I;
        public virtual bool IsFull => R * U * I != 0;
        public override string ToString()
        {
            return $"Resistor[{Id}] R:{Math.Round(R, 3)} Om  U:{Math.Round(U, 3)} V I:{Math.Round(I, 3)} A";
        }
        public virtual List<Resistor> GetInside() => new List<Resistor>(){ this };
    }
}
