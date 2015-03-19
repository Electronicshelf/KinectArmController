using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.IO.Ports;


namespace Arm_Controller
{
  public  class ArmControllerEngine
    {
      InvKin_DH_2 invkin_DH = new InvKin_DH_2();
      inKinGeometry_1 inkin_G = new inKinGeometry_1();
      baseControl baseDynamics = new baseControl();
      SerialPort port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);
      public ArmControllerEngine()
      {
      }

    
      private static int [] goMove = new int[6];

      public double radTOdeg(double angle)
      {
          return (angle * (180 / Math.PI));
      }
      public double checkDirection(double AngleTiter)
      {
          if (AngleTiter > 0)
          //converting to RA01 rotation convention for poistive angles
          { AngleTiter = (AngleTiter - 90 + 165); }
          //
          else AngleTiter = (AngleTiter + 90 - 165);
          return AngleTiter;
      }

      public double[] anglesINV_G(double x, double y , double z)
      {

          return inkin_G.calcIK(x,y,z);
      }
      public void baseDynamicLeft(double[] raMove, SerialPort port, string[] servoId) 
      {

          baseDynamics.movLEFT(raMove,port,servoId);
      
      }
      public void baseDynamicRight(double[] raMove, SerialPort port, string[] servoId)
      {

          baseDynamics.movRIGHT(raMove, port, servoId);

      }

      void count(int a)
      {
          for (int e = 0; e < a; e++) ; 
              
      }
      public byte convertToByte(double b)
      {
          int D = Convert.ToInt32(b);
          string G = D.ToString("X");
          byte byt = byte.Parse(G, System.Globalization.NumberStyles.HexNumber);
          return byt;
      }


      double[] rawBase;
      byte[] rawBaseByte;
      public byte moveBase(double a, double b, double c, SerialPort port)
      {
          for (int k = 0; k < 20; k++)
          {
              rawBase = new double[6] { 7, a, b, c, 100, 165 };
              for (int u = 0; u < rawBase.Length; u++)
              {
                  rawBaseByte = new byte[6];
                  rawBaseByte[u] = convertToByte(rawBase[u]);

                   port.Write(new byte[] { rawBaseByte[u] }, 0, 1);
                  Console.WriteLine(" The value sent to >>>    {0}      BASE  ", rawBaseByte[u]);
              }
          }
          return rawBaseByte[1];
      }
      
      byte[] rawelbwByte;
      public byte moveElbow(double c, SerialPort port)
      {

          rawBase = new double[6] { 7, 165, 160, c, 150, 165 };
          for (int k = 0; k < 40; k++)
          {

              for (int u = 0; u < rawBase.Length; u++)
              {
                  rawelbwByte = new byte[6];
                  rawelbwByte[u] = convertToByte(rawBase[u]);

                   port.Write(new byte[] { rawelbwByte[u] }, 0, 1);
                  Console.WriteLine(" The value sent to >>> {0}  ELBOW  ", rawelbwByte[u]);
              }

          }
          return rawelbwByte[3];
      }

      byte[] rawSldrByte;
      public byte moveShoulder(double b, double c, SerialPort port)
      {

          for (int k = 0; k < 30; k++)
          {
              rawBase = new double[6] { 7, 165, b, c, 200, 165 };
              for (int u = 0; u < rawBase.Length; u++)
              {
                  rawSldrByte = new byte[6];
                  rawSldrByte[u] = convertToByte(rawBase[u]);

                  port.Write(new byte[] { rawSldrByte[u] }, 0, 1);
                  Console.WriteLine(" The value sent to >>> {0}  SHOULDER  ", rawSldrByte[u]);
              }
          }

          return rawSldrByte[2];
      }

    
      public void setArm(double[] raMove, SerialPort port, string[] servoId)
      {
          string Command;
          string servo;
          string[] newAngles = new string[6];
          byte[] PositionByte = new byte[6];
          byte b;
          int a = 10;

          //sets all the servos of the arm 
          //goes through each motor Value and  converts it to hex
          string c = a.ToString("X");
          b = byte.Parse(c, System.Globalization.NumberStyles.HexNumber);
          //port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);

          try
          {
             port.Open();
              count(10);
             //moveBaseLeft(port);
             // moveElbow(raMove[3], port);
             // moveShoulder(raMove[2], raMove[3], port);
             // moveBase(raMove[1], raMove[2], raMove[3], port);
              for (int i = 0; i < 6; i++)
              {
                  goMove[i] = Convert.ToInt32(raMove[i]);
                  newAngles[i] = goMove[i].ToString("X");
                  PositionByte[i] = byte.Parse(newAngles[i], System.Globalization.NumberStyles.HexNumber);
              }
              
              //Send Data to Serial Port
            
             int delay = 200;
              for (int j= 0; j < delay;j++ )
              {
                  for (int i = 0; i < 6; i++)
                  {
                      servo = servoId[i];
                      Command = string.Format(" {0} {1} ", servo, raMove[i]);
                      //Send Control Serial Command
                      //count(20);
                      port.Write(new byte[] { PositionByte[i] }, 0, 1);
                      //Console.WriteLine(" The value sent to >>> {0} ", servo + " >>>> " + PositionByte[i]);
                 }
     
                    }
          }

          finally
          {
              // Console.WriteLine("---# Command  Stream Sent ---#");
              port.Close();
          }
      }

      

  
          
      }

                 



    }
     






