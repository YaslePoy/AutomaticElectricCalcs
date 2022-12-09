using System.IO;
using ElectricCalcs;

namespace Ploting
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ElectricCalcs.Writer.Active = false;
            int n = 30, m = 30;
            double[,] zmat = new double[n, m];
            double[] xray = new double[n];
            double[] yray = new double[m];
            int i, j;
            double x, y, step, fpi;

            step = 1;
            for (i = 1; i < n; i++)
            {
                x = i * step;
                xray[i] = x;
                for (j = 1; j < m; j++)
                {

                    y = j * step;
                    yray[j] = y;
                    if (j == i)
                        zmat[i, j] = 0;
                    else
                        zmat[i, j] = ElectricCalcs.Program.GetP(i, j);
                }
            }

            dislin.scrmod("revers");
            dislin.metafl("cons");
            dislin.setpag("da5p");
            dislin.disini();
            dislin.pagera();
            dislin.hwfont();
            dislin.titlin("Shaded Surface Plot", 2);
            dislin.titlin("Total P for all groups and all students", 4);
            dislin.axspos(200, 2600);
            dislin.ax3len(30, 30, 20000);

            dislin.name("N", "X");
            dislin.name("M", "Y");
            dislin.name("Total P", "Z");

            dislin.view3d(2, 5.0, 3.0, "ABS");
            dislin.graf3d(1, n, 1, 5, 1, m, 1, 5,
                           0, 45000, 0, 5000);
            dislin.height(100);
            dislin.title();

            dislin.shdmod("smooth", "surface");
            dislin.surshd(xray, n, yray, m, zmat);
            dislin.disfin();
        }
    }
}