using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orokles
{
    class Terkep
    {
        int meretX;
        int meretY;
        float[,] magassag;

        public Terkep(int meretX, int meretY)
        {
            this.meretX = meretX;
            this.meretY = meretY;
            magassag = new float[meretX, meretY];
            VeletlenFeltoltes();
        }

        public int MeretX { 
            get { return meretX; } 
        }

        public int MeretY
        {
            get { return meretY; }
        }

        public bool TerkepenBeluliPozicio(float x, float y)
        {
            return x >= 0 && y >= 0 && x < meretX && y < meretY;
        }

        public float Magassag(float x, float y)
        {
            if (x < 0 || y < 0 || x >= meretX || y >= meretY)
                return 2;
            else
                return magassag[(int)x, (int)y];
        }

        private void VeletlenFeltoltes()
        {
            for (int x = 0; x < MeretX; x++)
                for (int y = 0; y < meretY; y++)
                    magassag[x, y] = (float)Math.Min(1.0, 
                        (Math.Sin((float)x / 5) +
                        Math.Cos((float)y / 5) +
                        Math.Cos((float)x / 3) / 2 +
                        Math.Sin((float)y / 3) / 2) / 4 + 0.3);
        }
    }

    class TerkepRajzolo
    {
        private readonly ConsoleColor[] MAGASSAG_SZINEK = {
            ConsoleColor.Blue, ConsoleColor.DarkGreen, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.DarkYellow, ConsoleColor.White, ConsoleColor.Gray
        };

        Terkep terkep;

        public TerkepRajzolo(Terkep terkep)
        {
            this.terkep = terkep;
        }

        protected virtual char MiVanItt(int x, int y)
        {
            return ' ';
        }

        public void Kirajzol()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            for (int y = 0; y < terkep.MeretY; y++)
            {
                for (int x = 0; x < terkep.MeretX; x++)
                {
                    float magassag = terkep.Magassag(x, y);
                    Console.BackgroundColor = MAGASSAG_SZINEK[Math.Min((int)Math.Ceiling(Math.Max(magassag, 0) * 5), MAGASSAG_SZINEK.Length - 1)];
                    Console.Write(MiVanItt(x, y));
                }
                Console.WriteLine();
            }
        }
    }

    class Jarmu
    {
        public char Azonosito { get; }
        public float X { get; protected set; }
        public float Y { get; protected set; }
        protected Terkep terkep;

        public Jarmu(char azonosito, float x, float y, Terkep terkep)
        {
            Azonosito = azonosito;
            X = x;
            Y = y;
            this.terkep = terkep;
        }

        public virtual bool IdeLephet(float x, float y)
        {
            return terkep.TerkepenBeluliPozicio(x, y);
        }
    }

    class TerkepEsJarmuRajzolo : TerkepRajzolo
    {
        protected Jarmu[] jarmuvek;
        private int jarmuvekN;

        public TerkepEsJarmuRajzolo(Terkep terkep, int maxJarmuSzam) : base(terkep)
        {
            jarmuvek = new Jarmu[maxJarmuSzam];
            jarmuvekN = 0;
        }

        public void JarmuFelvetel(Jarmu jarmu)
        {
            if (jarmuvekN < jarmuvek.Length)
            {
                jarmuvek[jarmuvekN] = jarmu;
                jarmuvekN++;
            }
        }

        protected override char MiVanItt(int x, int y)
        {
            // Ellenőrizzük, hogy van-e jármű a megadott koordinátán
            foreach (Jarmu jarmu in jarmuvek)
            {
                if (jarmu != null && (int)jarmu.X == x && (int)jarmu.Y == y)
                {
                    return jarmu.Azonosito;
                }
            }

            // Ha nincs jármű a koordinátán, hívjuk az ősosztály MiVanItt metódusát
            return base.MiVanItt(x, y);
        }
    }

    abstract class MozgoJarmu : Jarmu
{
    protected float iranyX;
    protected float iranyY;

    public MozgoJarmu(char azonosito, float x, float y, Terkep terkep) : base(azonosito, x, y, terkep)
    {
        iranyX = 0.0f;
        iranyY = 0.0f;
    }

    public void UjIranyVektor(float ujIranyX, float ujIranyY)
    {
        iranyX = ujIranyX;
        iranyY = ujIranyY;
    }

    public abstract void Mozog();
}

class Helikopter : MozgoJarmu
    {
        private float sebesseg;

        public Helikopter(Terkep terkep, float x, float y) : base('H', x, y, terkep)
        {
            sebesseg = 1.0f;
        }

        public void Gyorsit()
        {
            sebesseg += 0.1f;
        }

        public void Lassit()
        {
            if (sebesseg >= 0.1f)
            {
                sebesseg -= 0.1f;
            }
        }

        public override void Mozog()
        {
            float ujX = X + iranyX * sebesseg;
            float ujY = Y + iranyY * sebesseg;

            if (IdeLephet(ujX, ujY))
            {
                X = ujX;
                Y = ujY;
            }
        }
    }

    class Szimulacio : TerkepEsJarmuRajzolo
    {
        public Szimulacio(Terkep terkep, int maxJarmuSzam) : base(terkep, maxJarmuSzam)
        {
        }

        public void EgyIdoEgysegEltelt()
        {
            foreach (Jarmu jarmu in jarmuvek)
            {
                if (jarmu is MozgoJarmu)
                {
                    ((MozgoJarmu)jarmu).Mozog();
                }
            }
        }

        public void Fut()
        {
            while (true)
            {
                EgyIdoEgysegEltelt();
                Kirajzol();
                Thread.Sleep(500); // Várakozás fél másodpercig
            }
        }
    }

    class Auto : MozgoJarmu
    {
        private float sebesseg;

        public Auto(char azonosito, float x, float y, Terkep terkep) : base(azonosito, x, y, terkep)
        {
            sebesseg = 1.0f;
        }

        public override bool IdeLephet(float ujX, float ujY)
        {
            if (!base.IdeLephet(ujX, ujY))
            {
                return false; // Nem lehet vízre lépni
            }

            // Ellenőrizzük a magasságkülönbséget a jelenlegi és következő pozíció között
            float magassagAktualis = terkep.Magassag(X, Y);
            float magassagKovetkezo = terkep.Magassag(ujX, ujY);

            // Számítsuk ki az abszolút magasságkülönbséget
            float magassagKulonbseg = Math.Abs(magassagKovetkezo - magassagAktualis);

            // Mozgás sebességét az abszolút magasságkülönbség alapján módosítjuk
            float sebessegMozgatott = sebesseg / (magassagKulonbseg + 1);

            UjIranyVektor(ujX - X, ujY - Y);
            X = X + iranyX * sebessegMozgatott;
            Y = Y + iranyY * sebessegMozgatott;

            return true;
        }

        public override void Mozog()
        {
            float ujX = X + iranyX * sebesseg;
            float ujY = Y + iranyY * sebesseg;

            if (IdeLephet(ujX, ujY))
            {
                X = ujX;
                Y = ujY;
            }
        }
    }

    sealed class Tank : Auto
    {
        private float uzemanyag;

        public Tank(char azonosito, float x, float y, Terkep terkep, float uzemanyagKezdo) : base(azonosito, x, y, terkep)
        {
            uzemanyag = uzemanyagKezdo;
        }

        public override bool IdeLephet(float ujX, float ujY)
        {
            return true; // A tank bárhova léphet
        }

        public override void Mozog()
        {
            if (uzemanyag > 0)
            {
                base.Mozog();
                uzemanyag -= 10;
            }
        }
    }

}
