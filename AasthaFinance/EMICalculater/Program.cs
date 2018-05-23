using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMICalculater
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Simple interest
            double p = 100;
            double r = 18;
            double t = 100;

            //Fourmulae of SI
            double totalSimpleInterest = (p * r * t) / 100;


            Console.WriteLine(" Amount : {0} , rate : {1}% and time : {2}", p, r, t);

            double simpleInterest_D = (p * r * t) / (100 * 365);
            Console.WriteLine("Total Simple Interest for {0} Day : {1}", t, Math.Round(simpleInterest_D, 2));

            //Month Formulae is correct
            double simpleInterest_M = ((p * r * t) / (100 * 12));

            Console.WriteLine("Total Simple Interest for {0}  Month: {1}", t, Math.Round(simpleInterest_M, 2));

            //Year Formulae is also perfect
            double simpleInterest_Y = (p * r * t) / 100;

            Console.WriteLine("Total Simple Interest for {0}  year: {1}", t, Math.Round(simpleInterest_Y, 2));

            #endregion


            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(DateTime.Now.AddDays(i));
            }


            Console.ReadKey();

        }
    }

    public enum Freequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
