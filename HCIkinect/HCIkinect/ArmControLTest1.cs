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
        
       double[] raMove = new double[6] { 7, 254, 140,220, 137, 85};
       int[] goMove = new int[6];
       int a = 10;
       SerialPort port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);

       public void writeTofile(byte a)
       {

           string filename = "baseAngle.txt";
           TextWriter file = new StreamWriter(filename);
           file.WriteLine(a);
           file.Close();
       }

       public double readFile(double a)
       {
           string Filename = "baseAngle.txt";
           TextReader fileRead = new StreamReader(Filename);
           a = double.Parse(fileRead.ReadLine());
           fileRead.Close();
           return a;
       }
    
       byte[] rawBaseByteLeft = new byte[6];
       double[] rawBaseLeft = new double[6] { 7, 75, 160, 250, 165, 160 };
       
       public byte moveBaseLeft(SerialPort port)
       {
                
           for (int k = 0; k < 500; k++)
           {
               if (k == 1) { rawBaseLeft[1] = readFile(rawBaseLeft[1]); }

               for (int u = 0; u < rawBaseLeft.Length; u++)
               {
                   double c = rawBaseByteLeft[1] + 30;
                   byte d = convertToByte(c);
                   rawBaseByteLeft[u] = convertToByte(rawBaseLeft[u]);

                   if (rawBaseByteLeft[1] < d)
                   {
                       rawBaseByteLeft[1] += 1;
                       port.Write(new byte[] { rawBaseByteLeft[u] }, 0, 1);
                   }
                   Console.WriteLine(" The value sent to >>>    {0}      BASEDECREASE  ", rawBaseByteLeft[u]);
                  
               }
           }

           rawBaseByteLeft[1] += 10;
           writeTofile(rawBaseByteLeft[1]);
           return rawBaseByteLeft[1];
       }
     

       public void armSet2(SerialPort port)
       {
           setArm2(raMove, servoId,port);
       }

      
       public double forward(double fwd)
       {

           return fwd += 1;

       }
       public byte convertToByte(double b) 
            {
            int D = Convert.ToInt32(b);
            string G = D.ToString("X");
            byte byt = byte.Parse(G, System.Globalization.NumberStyles.HexNumber);
            return byt;
             }
       void count(int a) { for (a=0 ; a < 20; a++); }
      
        byte[] rawelbwByte;
        double[] rawElbow;
        public byte moveElbow(double a,double c, SerialPort port){
        
           for (int k = 0; k < 500; k++)
           {
               rawElbow = new double[6] { 7, a, 160, c, 190, 100 };
               for (int u = 0; u < rawBase.Length; u++)
               {  
                   rawelbwByte = new byte[6];
                   rawelbwByte[u] = convertToByte(rawBase[u]);
                   count(20);
                   //port.Write(new byte[] { rawelbwByte[u] }, 0, 1);
                   Console.WriteLine(" The value sent to >>> {0}  ELBOW  ", rawelbwByte[u]);
               }

           }
           return rawelbwByte[3];
       }

       double[] rawBase;
       byte[] rawBaseByte;
       public byte moveBase(double a,SerialPort port)
       {
           for (int k = 0; k < 100; k++)
           {
               rawBase = new double[6] { 7, a, 160, 165, 200, 150 };
               for (int u = 0; u < rawBase.Length; u++)
               {
                   rawBaseByte = new byte[6];
                   rawBaseByte[u] = convertToByte(rawBase[u]);
                   count(20);
                  // port.Write(new byte[] { rawBaseByte[u] }, 0, 1);
                   Console.WriteLine(" The value sent to >>>    {0}      BASE  ", rawBaseByte[u]); 
               }   
           }
           return rawBaseByte[1];
       }
       
      
       byte[] rawSldrByte;
        public byte moveShoulder(double a,double b, double c, SerialPort port)
       {
           
           for (int k= 0; k < 1000; k++)
           {  
               rawBase = new double[6] { 7, a, b, c, 200, 230 };
               for (int u = 0; u < rawBase.Length; u++)
               {
                   rawSldrByte = new byte[6];
                   rawSldrByte[u] =  convertToByte(rawBase[u]);
                   count(20);
                 //port.Write(new byte[] { rawSldrByte[u] }, 0, 1);
                 Console.WriteLine(" The value sent to >>> {0}  SHOULDER  ", rawSldrByte[u]); 
               }           
           }

           return rawSldrByte[2];
       }
      
        public void setArm2(double[] raMove , string[] servoId, SerialPort port)
        {
            
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

            moveBaseLeft(port);
           // moveBase(raMove[1], port);
            //count(200);
           // moveShoulder(raMove[1], raMove[2], raMove[3], port);
           // count(20);
           
            try
            {
               // moveElbow(raMove[1], raMove[3], port);
                
               //Send Data to Serial Port
                int delay =200;
               for (int j = 0 ; j < delay ; j++)
               {
                   int g = 6;

                   for (int i = 0; i < g; i++)
                   {
 
                       servo = servoId[i];
                      // Command = string.Format(" {0} {1} ", servo, raMove[i]);
                       goMove[i] = Convert.ToInt32(raMove[i]);
                      newAngles[i] =goMove[i].ToString("X");
                       
                       //
                      // Console.WriteLine( "  hi {0}" ,newAngles[i]);     
                       PositionByte[i] = byte.Parse(newAngles[i], System.Globalization.NumberStyles.HexNumber);
                       //Send Control Serial Command
                      // port.Write(new byte[] { PositionByte[i] }, 0, 1);
                       //Console.WriteLine(PositionByte[2]);
                      

                       Console.WriteLine(" The value sent to >>> {0}  ", servo + " >>>> " + PositionByte[i]);
                      // count();


                    
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