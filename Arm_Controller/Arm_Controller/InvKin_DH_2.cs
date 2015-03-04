using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arm_Controller
{
    class InvKin_DH_2
    {

        private static double GRIPPER_LEN = 3.0;
        private static double LOWER_ARM = 2.75;
        private static double UPPER_ARM = 2.75;
        private static double BASE_HEIGHT = 4.40;

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

        public double[] calcIK(double x, double y, double z)
        /* Use (x,y,z) coordinate to calculate the IK
           angles for the base, elbow, shoulder and wrist joints. 
        */
        {

            double extent2 = (x * x) + (y * y);
            double maxExtent = GRIPPER_LEN + LOWER_ARM + UPPER_ARM;

            if (extent2 > (maxExtent * maxExtent))
            {
                Console.WriteLine("Coordinate (" + x + ", " + y + ", " + z + ") is too far away on the XY plane");
                return null;
            }

            // base angle and radial distance from x,y coordinates 
            double baseAngle = radTOdeg(Math.Atan2(x, y));

            double rdist = Math.Sqrt((x * x) + (y * y));
            // radial distance now treated as the y coordinate for the arm 

            // wrist position 
            double wristZ = z - BASE_HEIGHT;
            double wristY = rdist - GRIPPER_LEN;
            Console.WriteLine("wrist Y = {0: 0.00} , wrist Z = {1: 0.00 }\n", wristY, wristZ);

            // shoulder-wrist squared distance (swDist2)
            double swDist2 = (wristZ * wristZ) + (wristY * wristY);
            Console.WriteLine("swDist2 = {0: 0.00}\n", swDist2);

            // shoulder-wrist angle to ground 
            double swAngle1 = radTOdeg(Math.Atan2(wristZ, wristY));
            Console.WriteLine(" swAngle1 = {0: 0.00}\n", swAngle1);

            double triVal = (UPPER_ARM * UPPER_ARM + swDist2 - LOWER_ARM * LOWER_ARM) / (2 * UPPER_ARM * Math.Sqrt(swDist2));
            Console.WriteLine(" triVal = {0: 0.00}\n", triVal);

            if (triVal > 1.0)
            {
                Console.WriteLine("Arm not long enough to reach coordinate");
                return null;
            }
            double swAngle2 = radTOdeg(Math.Acos(triVal));
            // System.out.printf("swAngles: %.2f, %.2f)\n",  Math.toDegrees(swAngle1), Math.toDegrees(swAngle2));
            Console.WriteLine(" swAngle2 = {0: 0.00}\n", swAngle2);

            double shoulderAngle = 90.0 - (swAngle1 + swAngle2);
            double shldr = (swAngle1 + swAngle2);
            Console.WriteLine(" shoulderAngle = {0: 0.00}\n", shoulderAngle);
            Console.WriteLine(" shldrAngle = {0: 0.00}\n", swAngle1 + swAngle2);
            // elbow angle 
            double ewAngle = radTOdeg(Math.Acos((UPPER_ARM * UPPER_ARM + LOWER_ARM * LOWER_ARM - swDist2) / (2 * UPPER_ARM * LOWER_ARM)));
            double elbowAngle = 180.0 - (ewAngle);
            Console.WriteLine(" elbowAngle = {0: 0.00}\n", elbowAngle);
            Console.WriteLine(" ewAngle = {0: 0.00}\n", ewAngle);

            //double wristAngle = 90.0 - (shoulderAngle + elbowAngle);
            double wristAngle = 90.0;
            //double wrstAngle = (shldr + ewAngle);
            Console.WriteLine(" wristAngle = {0: 0.00}\n", ewAngle + swAngle1 + swAngle2);
            // round angles to integers

            double baseAng = (double)Math.Round(baseAngle) + 165;
            double shoulderAng = checkDirection((double)Math.Round(shoulderAngle));
            double elbowAng = checkDirection((double)Math.Round(elbowAngle));
            double wristAng = checkDirection((double)Math.Round(wristAngle));

            double INIT = 7;
            double GriperDist = 160;

            double[] angles = new double[] { INIT, baseAng, shoulderAng, elbowAng, wristAng, GriperDist };
            return angles;
        } 

    }
}
