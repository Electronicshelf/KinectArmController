using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using  consoleInverseKinematicsCalc;
    


namespace consoleInverseKinematicsCalc
{
    
    
    class Program
    {
        
        static void Main(string[] args)
        {
            InvKin iKin = new InvKin();
            InKIN inki =new InKIN();
           double [] Angles ;
            double a , b  , c ;
                Console.WriteLine("Enter a = " );
             a =  double.Parse(Console.ReadLine());
             Console.WriteLine("Enter b = " );
             b =  double.Parse(Console.ReadLine());
             Console.WriteLine("Enter c = " );
             c =  double.Parse(Console.ReadLine());


             Angles = inki.calcIK(a, b, c);
          
                // Angles = iKin.invKin(a, b, c);

             Console.WriteLine(" Angles are =  {0} >> {1} >> {2} >> {3} >> {4} >> {5}  ", Angles[0], Angles[1], Angles[2], Angles[3], Angles[4], Angles[5]);
                 //Console.WriteLine(" Angles are =  {0}", iKin.invKin(a, b, c));
             
            Console.ReadKey();
                    }
    }
}
