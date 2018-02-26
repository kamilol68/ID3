using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Data;

namespace SystemyUcz
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"K:\2.txt";
            string[] tekst = File.ReadAllLines(path);
            string [][] dane = new string[tekst.Length][];
            int x = 0;
           
            foreach (string s in tekst)
            {
                dane[x++] = s.Split(',');
            }

            licz(new int[]{0, 1, 2}, 3, dane, 0);

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);            
        }

        static void licz(int[] R, int C, string[][] S, int DEPTH)
        {
            string indentation = new string(' ', DEPTH);

            if (S.Length == 0)
            {                
                Console.WriteLine("Faliure");
                return;
            }

            if (Array.TrueForAll(S, element => { return String.Equals(element[C], S[0][C]); })) // porównanie z kategoria pierwszej tablicy danych
            {
                Console.WriteLine(indentation + indentation + S[0][C]); //zwraca wartość -> drugi if pseudokod
                return;
            }

            if (R.Length == 0) // trzeci if
            {
                int mv = -1; // max value
                string mk = "ERROR XD"; // max key

                foreach (KeyValuePair<string, int> KV in SlownikWystapien(S, C))
                {
                    if (KV.Value > mv)
                    {
                        mv = KV.Value;
                        mk = KV.Key;
                    }
                }

                Console.WriteLine(indentation + mk);
                return;
            }

            double mgain = -1; // max gain
            int D = -1; // kolumna z najwiekszym zyskiem
            foreach (int i in R)
            {
                double value = liczgain(i,S,C);
                if (value > mgain)
                {
                    mgain = value;
                    D = i;
                }
            }

            Dictionary<string, List<string[]>> DJSJ = new Dictionary<string, List<string[]>>();

            foreach (string[] krotka in S)
            {
                if (DJSJ.ContainsKey(krotka[D]))        // Słownik - wartość i podzestaw -> wbijać żeby szukać dalej
                {
                    DJSJ[krotka[D]].Add(krotka);
                }

                else
                {
                    DJSJ.Add(krotka[D], new List<string[]>());
                    DJSJ[krotka[D]].Add(krotka);
                }
            }

            Console.WriteLine(indentation + "D: {0}", D);

            foreach(KeyValuePair<string, List<string[]>> KV in DJSJ)
            {
                List<int> newR = R.ToList();
                newR.Remove(D);

                Console.WriteLine(indentation + "DJ: {0}", KV.Key);
                licz(newR.ToArray(), C, DJSJ[KV.Key].ToArray(), DEPTH + 4); // zamiana list na arraye i na odwrot
            }
        }

        static Dictionary<string, int> SlownikWystapien(string[][] dane, int kolumna)
        {
            Dictionary<String, int> wystapienia = new Dictionary<String, int>();

            for (int i = 0; i < dane.Length; i++)
            {
                string k = dane[i][kolumna];

                if (wystapienia.ContainsKey(k))
                {
                    wystapienia[k]++;
                }

                else
                {
                    wystapienia.Add(k, 1);
                }
            }

            return wystapienia;
        }

        static double LiczenieEntropii(string[][] dane, int kolumna) // licz entropie
        {
            Dictionary<String, int> wystapienia = new Dictionary<String, int>();

            for (int i = 0; i < dane.Length ; i++)
            {
                string k = dane[i][kolumna];

                if (wystapienia.ContainsKey(k))
                {
                    wystapienia[k]++;
                }

                else
                {
                    wystapienia.Add(k, 1);
                }
            }
            double entr = 0;
            foreach(KeyValuePair<String, int> W in wystapienia)
            {
                double p = (double) W.Value / (double) dane.Length;

                entr = entr+( p * Math.Log((p), 2));
            }
           
            return -1*entr;

        }
        
        static double Informacja_atrybutu(string[][] dane,int kolumna,int C)
        {
            double info = 0;

            Dictionary<String, int> wystapienia = new Dictionary<String, int>();
            for (int i = 0; i < dane.Length; i++)
            {
                string k = dane[i][kolumna];

                if (wystapienia.ContainsKey(k))
                {
                    wystapienia[k]++;
                }

                else
                {
                    wystapienia.Add(k, 1);
                }
               
            }

            foreach (KeyValuePair<String, int> W in wystapienia)
            {
                double p = (double)W.Value / (double)dane.Length;
                double e = EntropiaZeSlownikaWystapien(ZliczajPotwiedzenia(W.Key, kolumna,dane,C));

                info = info + p * e;
            }

            return info;
        }

        static Dictionary<String, int> ZliczajPotwiedzenia(string at, int kolumna, string[][] dane, int C)
        {
            
            Dictionary<String, int> wystapienia = new Dictionary<String, int>();
            for (int i = 0; i < dane.Length; i++)
            {
                string k = dane[i][kolumna];
                if (k.Equals(at))
                {
                    if (wystapienia.ContainsKey(dane[i][C]))
                    {
                        wystapienia[dane[i][C]]++;
                    }

                    else
                    {
                        wystapienia.Add(dane[i][C], 1);
                    }
                }
            }
           return wystapienia;
        }

        static double EntropiaZeSlownikaWystapien(Dictionary<string, int> rozklad)
        {
            double entropia = 0;
            double sumawystapien = 0; //Wymusza poprawny tryb dzielenia

            foreach (int V in rozklad.Values) sumawystapien += V;

            foreach (int V in rozklad.Values)
            {
                double p = V / sumawystapien;

                entropia =entropia +( p * Math.Log(p, 2));
            }

            return -1* entropia;
        }

        static double liczgain(int i, string[][] dane, int kategoria)
        {
            double gain = 0;
            
            gain = LiczenieEntropii(dane,kategoria) - Informacja_atrybutu(dane,i,kategoria);
            return gain;
        }
        /*static double liczgainratio(int i)
        {
            double gainratio= 0;

            gainratio = gaint[i] / entropia[i];
            return gainratio;
        }*/
    }
}
