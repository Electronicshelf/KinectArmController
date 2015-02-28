using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/* Development of the Inverse Kinematics Algorithm for the RA-01 robotic Arm to determine the four joint angles
with only values of x,y&z; code is developed in C# (2014) Adapted from  Matlab By Olawale B. Akinwale(2009)
 * developed by Osahor Uche ucheosahor@gmail.com
*/

namespace consoleInverseKinematicsCalc
{
    class InvKin
    {



//Math.Cos(10);
//Math.Acos(4);
   
   public double[] invKin (double X, double Y, double Z ){

    double L2,L3,L4;
    
   // L0 = 2.20;
   // L1 = 2.20;
    L2 = 2.75;
    L3 = 2.75;
    L4 = 3.0;
   



            double angB , angS1, angS2, angE1 ,angE2, angW = 0;
          //Determine Base Angle
                double  O1;
                double Q1 =(Y/X);
                
            O1 = Math.Atan(Q1);
           
            angB = Math.Round(O1) ;
            
            //Determine Elbow angle
            double a,a2,a3,a4,O3,o3;
            a  = (((X*X) + (Y*Y) + (Z*Z)) + ((L2*L2)+(L3*L3)+(L4*L4)) );
            a2 =  ((4 * (L2*L2) *(L3*L3)))+((4 * (L2*L2) * (L4*L4)));
            a3 =  (2*a*L2*L4);
            a4 = ((a*a) - ((4 * (L2*L2) *(L3*L3))));
            Console.WriteLine(" values =  {0}, {1} ,{2}, {3},  {4} , {5}  ", Q1,a4,a,a3,a2,angB);
            
            //determine the values for real and imaginary values
            O3 =  ((a3) + Math.Sqrt((Math.Abs(((a3)*(a3)) - (4*a2*a4)))  / (2*a2)));
            Console.WriteLine(" O3 {0} ", O3);
            o3 = ((a3) - Math.Sqrt((Math.Abs(((a3) * (a3)) - (4 * a2 * a4))) / (2 * a2)));
        
            angE1 = Math.Asin(O3) - 90;
            angE2 = Math.Asin(o3) - 90;
        
               //determine real values of angE1 and angE2 and the mantissa(significannt figures)
               //because of the complex number analysis
               
               //Determine the Shoulder Angle
               //declear Variables
                   double k1,k2,dK,nK,lK,O2,o2;
                   k1 = ((L4*Math.Sin(O3)) + (L3*Math.Cos(O3))+ L2);
                   k2 = ((L4*Math.Cos(O3)) - (L3*Math.Sin(O3)));
                   dK = ((k1*k1) + (k2*k2));
                   nK = (2*Z*k2);
                   lK = ((Z*Z)-(k1*k1));
                   Console.WriteLine(" values =  {0} {1} {2} {3} {4}   " , k1, k2, dK, nK,lK);
                   O2 = Math.Abs (nK + Math.Sqrt(((nK) * (nK)) - (4 * dK * lK)));
                   o2 = Math.Abs (nK - Math.Sqrt(((nK) * (nK)) - (4 * dK * lK)));
                   Console.WriteLine("  O2 =  {0} {1}", O2,o2);
                   angS1 = Math.Asin(O2);
                   angS2 = Math.Asin(o2);
                   Console.WriteLine("  O2 =  {0}  angs1 = {1} , angs2 =  {2} ", O2,angS1,angS2);
               //determine real values of angS1 and angS2 and the mantissa(significannt figures)
               //because of the complex number analysis
               
               //Determine Gripper Centre
                   double O4,o4, _z,e ;
                   double GRP = 1;
                   o4 = 160;
                   //from forward Kinematics
                   e =( Math.Sin(O2)* Math.Sin(O3));
                   _z =( e - ((Math.Cos(O2)*Math.Cos(O3))*GRP*(Math.Sin(o4)) + Z ));
                   O4 = Math.Asin((1/GRP)* ((Z - _z)/ e));
                   angW = O4 ;
                  
                      double[] Angles = new double[] {angB, angS1,  angS2, angE1,angE2, angW };
               
                   return Angles;
                    
                   }
                   
    
        }
    }
    