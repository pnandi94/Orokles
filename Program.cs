using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orokles
{
    class Program
    {
        static void Teszt1()
        {
            Terkep terkep = new Terkep(80, 25);
            TerkepRajzolo rajzolo = new TerkepRajzolo(terkep);
            rajzolo.Kirajzol();
        }

        static void Main(string[] args)
        {
            Terkep terkep = new Terkep(80, 25);
            
            // Create instances of Auto, Tank, and Helikopter
            Auto auto = new Auto('A', 10, 10, terkep);
            Tank tank = new Tank('T', 20, 20, terkep, 100); // 100 units of fuel
            Helikopter helikopter = new Helikopter(terkep, 30, 30);

            // Add the vehicles to the TerkepEsJarmuRajzolo
            TerkepEsJarmuRajzolo jarmuRajzolo = new TerkepEsJarmuRajzolo(terkep, 3);
            jarmuRajzolo.JarmuFelvetel(auto);
            jarmuRajzolo.JarmuFelvetel(tank);
            jarmuRajzolo.JarmuFelvetel(helikopter);
            
            // Start the simulation
            Szimulacio szimulacio = new Szimulacio(terkep, 3);
            szimulacio.JarmuFelvetel(auto);
            szimulacio.JarmuFelvetel(tank);
            szimulacio.JarmuFelvetel(helikopter);

            // Start the simulation loop
            szimulacio.Fut();
        }
    }
}
