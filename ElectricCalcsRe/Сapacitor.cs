using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricCalcs
{
    public class Capacitor
    {
        public ResistorType Type;
        public virtual string Id => id;
        readonly string id = "noID";
        public double C, U, Q;
        public Capacitor()
        {
            C = U = Q = 0;
            Type = ResistorType.Single;
        }
        public Capacitor(string id)
        {
            C = U = Q = 0;
            Type = ResistorType.Single;
            this.id = id;
        }
        public Capacitor(double r, double u, double i, string id)
        {
            this.id = id;
            this.C = r;
            this.U = u;
            this.Q = i;
            Type = ResistorType.Single;
        }
        public virtual void UpdateRUI()
        {
           
            if (IsFull)
                return;
            if (Q != 0 && C != 0)
            {
                U = Q / C * 1000;
                Writer.Next($" U{Id} = Q{Id} / C{Id} = {Math.Round(Q, 3)}^-12 / {Math.Round(C, 3)}^-9 = {Math.Round(U, 3)} V");
                return;
            }
            if (U != 0 && C != 0)
            {
                Q = U * C / 1000;
                Writer.Next($" Q{Id} = U{Id} * C{Id} = {Math.Round(U, 3)} * {Math.Round(C, 3)}^-9 = {Math.Round(Q, 3)}  nC");
                return;
            }
            if (Q != 0 && U != 0)
            {
                C = Q / U * 1000;
                Writer.Next($" C{Id} = Q{Id} / U{Id} = {Math.Round(Q, 3)}^-12 / {Math.Round(U, 3)} = {Math.Round(C, 3)} pF");
                return;
            }
        }
        public double W => (Q * U) / 2000;
        public virtual bool IsFull => C * U * Q != 0;
        public override string ToString()
        {
            return $"Resistor[{Id}] C:{Math.Round(C, 3)} pF U:{Math.Round(U, 3)} V Q:{Math.Round(Q, 3)} nC";
        }
        public virtual List<Capacitor> GetInside() => new List<Capacitor>() { this };
    }
}
