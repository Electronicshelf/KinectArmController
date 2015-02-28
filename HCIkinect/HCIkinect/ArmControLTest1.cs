using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;

/* This program is a simple test code that send values to the RA01 robotic Arm to test the Serial Port
 >> copyright OSAHOR Uche (2014)
 >> Department of Electronic and Electrical Engineering
 >> Obafemi Awolowo University, Ile-Ife, Osun State, Nigeria.
 >> ucheosahor@gmail.com
 */

namespace ArmController
{
    class armSetup
    {
       string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        
       double[] raMove = new double[6] { 7, 160, 165, 160, 165,160 };
       int[] goMove = new int[6];
       int a = 10;
      
       
      // public void armSet( SerialPort port)
      // {
      //     setArm(raMove, port , servoId);
      // }
        
       public void armSet2()
       {
           setArm2(raMove, servoId);
       }

       void count() { for (int e = 0; e < 5; e++)
           Console.WriteLine(e);
       }
       public double forward(double fwd)
       {

           return fwd += 1;

       }
			
        public void setArm2(double[] raMove , string[] servoId)
        {    
             //SerialPort port, 
           // string Command;
            string servo;
            //raMove = new byte[6];
            string[] newAngles = new string[6]; 
            byte[] PositionByte = new byte[6];
            byte b;

		    //sets all the servos of the arm 
            //goes through each motor Value and  converts it to hex
            string c;
            c = a.ToString("X");
            b = byte.Parse(c, System.Globalization.NumberStyles.HexNumber);

            try
            {
                
               //Send Data to Serial Port
                int delay = 15;
               for (int j = 0 ; j < delay ; j++)
               {
                   int g = 6;
                   for (int i = 0; i < g; i++)
                   {
 
                       servo = servoId[i];
                      // Command = string.Format(" {0} {1} ", servo, raMove[i]);
                       goMove[i] = Convert.ToInt32(raMove[i]);
                       newAngles[i] =goMove[i].ToString("X");
                       
                       // Console.WriteLine( "  hi {0}" ,newAngles[i]);     
                       PositionByte[i] = byte.Parse(newAngles[i], System.Globalization.NumberStyles.HexNumber);
                       //Send Control Serial Command
                       //port.Write(new byte[] { PositionByte[i] }, 0, 1);
                       //Console.WriteLine(PositionByte[2]);

                       Console.WriteLine(" The value sent to >>> {0}  ", servo + " >>>> " + PositionByte[i]);
                       count();
                       if (i == 1 && raMove[1] < 254.0)
                       {
                           count();
                          // raMove[1] += 1;
                           forward(raMove[1]);

                       }
                       if (i == 2 && raMove[2] < 253.0)
                       {
                           raMove[2] += 1;
                           count();
                       }

                       if (i == 3 && raMove[3] < 253.0)
                       {
                           raMove[3] += 1;
                           count();
                       }
                   }
               }
               
            }
            catch (Exception eX)
            {
                Console.WriteLine(eX);
            }
            finally
            {

                Console.WriteLine("---# Command  Stream Sent ---#");
            }
  }


        //byte Converter
        public byte toByte(int b) {
            string G = b.ToString("X");
            byte byt = byte.Parse(G, System.Globalization.NumberStyles.HexNumber);
            return byt;
           }

   
        


    }

}