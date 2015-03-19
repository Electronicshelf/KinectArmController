using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
/* 
 
 >> This program is a simple test code that send values to the RA01 robotic Arm to test the Serial Port 
 >> copyright OSAHOR Uche (2014)
 >> Department of Electronic and Electrical Engineering
 >> Obafemi Awolowo University, Ile-Ife, Osun State, Nigeria.
 >> ucheosahor@gmail.com
 
 */
using ArmController;

namespace HCIkinect
{
    

    class Program
    {


        string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        double[] raMove = new double[6] { 7, 87, 187, 97, 137, 197 };
        //int a = 10;
        static void Main(string[] args)
        {
            SerialPort port;
            armSetup RAO1 = new armSetup();

            try
            {
            
                 port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);
                 Console.ReadKey();
                port.Open();
                Console.WriteLine("Port is Open");
              
                Console.ReadKey();
                Console.WriteLine("Port open Sending first Position");
                Console.ReadKey();


                

                RAO1.armSet2(port);
                Console.ReadLine();
                
               port.Close();
                Console.ReadLine();
            
            }

            catch (Exception x)  {

                Console.WriteLine( " Dude! we have a problem with ports " , x );
                Console.ReadKey();
            }

                
        }
    }
}
