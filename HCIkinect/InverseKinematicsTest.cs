using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Development of the Inverse Kinematics Algorithm for the RA-01 robotic Arm to determine the four joint angles
with only values of x,y&z; code is developed in C# (2014) Adapted from  Matlab By Olawale B. Akinwale(2009)
*/

namespace consoleInverseKinematics
{
   public class InvKin
    {

        
    double  L0 = 2.20;
    double  L1 = 2.20;
    double L2 = 2.75;
    double L3 = 2.75;
    double L4 = 3.0;
    

//Math.Cos(10);
//Math.Acos(4);
    public void invKin(double X, double Y, double Z ){

    

        Z -= (L0 + L1);

    double angB = 0, angS1, angS2, angE1 ,angE2, angW = 0;
  //Determine Base Angle
        double  O1;
    O1 = Math.Atan(Y/X); 
    O1 = angB;

    //Determine Elbow angle
    double a,a2,a3,a4,O3,o3;
    a  = (((X*X) + (Y*Y) + (Z*Z)) - ((L2*L2)+(L3*L3)+(L4*L4)) );
    a2 =  ((4 * (L2*L2) *(L4*L4)))+((4 * (L2*L2) *(L3*L3)));
    a3 = (4*a*L2*L4);
    a4 = ((a*a) - ((4 * (L2*L2) *(L3*L3))));

    O3 = ( a3  + Math.Sqrt( ((a3)*(a3)) - (4*a2*a4) ) );
    o3 = ( 0 -(a3) - Math.Sqrt( ((a3)*(a3)) - (4*a2*a4) ) );
    angE1 = Math.Asin(O3);
    angE2 = Math.Asin(o3);

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
    
    O2 = ( nK  + Math.Sqrt( ((nK)*(nK)) - (4*dK*lK) ) );
    o2 = ( 0 -(nK) - Math.Sqrt( ((nK)*(nK)) - (4*dK*lK) ) );
    angS1 = Math.Asin(O2);
    angS2 = Math.Asin(o2);
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
    
    }
    
    
        }
    }
    