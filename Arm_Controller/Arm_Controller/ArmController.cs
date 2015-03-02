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
     
      public ArmControllerEngine()
      {
      }
      
      private static double GRIPPER_LEN = 3.0;
      private static double LOWER_ARM = 2.75;
      private static double UPPER_ARM = 2.75;
      private static double BASE_HEIGHT = 4.40;
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

      public double[] anglesINV_DH(double x, double y , double z)
      {

          return invkin_DH.calcIK(x,y,z);
      }
      

      void count(int a)
      {
          for (int e = 0; e < a; e++) ; 
              
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
          port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);

          try
          {
              port.Open();
              for (int i = 0; i < 6; i++)
              {
                  goMove[i] = Convert.ToInt32(raMove[i]);
                  newAngles[i] = goMove[i].ToString("X");
                  PositionByte[i] = byte.Parse(newAngles[i], System.Globalization.NumberStyles.HexNumber);
              }
              
              //Send Data to Serial Port
            
              int delay = 108;
              for (int j = 0; j < delay; j++)
              {
                  for (int i = 0; i < 6; i++)
                  {
                      servo = servoId[i];
                      Command = string.Format(" {0} {1} ", servo, raMove[i]);
                      //Send Control Serial Command
                      count(20);
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

      

      public double[] calcIK(double x, double y, double z)
                {

            double extent2 = (x * x) + (y * y);
            double maxExtent = GRIPPER_LEN + LOWER_ARM + UPPER_ARM;

            if (extent2 > (maxExtent * maxExtent))
            {
                return null;
            }

            // base angle and radial distance from x,y coordinates 
            double baseAngle = radTOdeg(Math.Atan2(x, y));

            double rdist = Math.Sqrt((x * x) + (y * y));
            // radial distance now treated as the y coordinate for the arm 

            // wrist position 
            double wristZ = z - BASE_HEIGHT;
            double wristY = rdist - GRIPPER_LEN;
            

            // shoulder-wrist squared distance (swDist2)
            double swDist2 = (wristZ * wristZ) + (wristY * wristY);
          

            // shoulder-wrist angle to ground 
            double swAngle1 = radTOdeg(Math.Atan2(wristZ, wristY));
           

            double triVal = (UPPER_ARM * UPPER_ARM + swDist2 - LOWER_ARM * LOWER_ARM) / (2 * UPPER_ARM * Math.Sqrt(swDist2));
            

            if (triVal > 1.0)
            {
                Console.WriteLine("Arm not long enough to reach coordinate");
                return null;
            }
            double swAngle2 = radTOdeg(Math.Acos(triVal));
            // System.out.printf("swAngles: %.2f, %.2f)\n",  Math.toDegrees(swAngle1), Math.toDegrees(swAngle2));
           

            double shoulderAngle = 30.0 - (swAngle1 + swAngle2);
            double shldr = (swAngle1 + swAngle2);
            
            // elbow angle 
            double ewAngle = radTOdeg(Math.Acos((UPPER_ARM * UPPER_ARM + LOWER_ARM * LOWER_ARM - swDist2) / (2 * UPPER_ARM * LOWER_ARM)));
            double elbowAngle = 180.0 - (ewAngle);
            

            //double wristAngle = 90.0 - (shoulderAngle + elbowAngle);
            double wristAngle = 90.0;
            //double wrstAngle = (shldr + ewAngle);
            
            // round angles to integers

            double baseAng = (double)Math.Round(baseAngle) + 165;
            double shoulderAng = checkDirection((double)Math.Round(shoulderAngle));
            double elbowAng = checkDirection((double)Math.Round(elbowAngle));
            double wristAng = checkDirection((double)Math.Round(wristAngle));

            double INIT = 7;
            double GriperDist = 160;

            double[] angles = new double[] { INIT, baseAng, shoulderAng, elbowAng, wristAng, GriperDist};
            return angles;     
      
        }  
          
      }

                 



    }
     






