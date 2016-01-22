using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    public class Platform
    {
        private static Random RandomGenerator = new Random();
        private const int jointsCount = 3; 

        private int? leftJoint;
        private int? rightJoint;

        public int? LeftJoint
        {
            get
            {
                return this.leftJoint;
            }
        }

        public int? RightJoint
        {
            get
            {
                return this.rightJoint;
            }
        }

        public Platform(int? leftJoint, int? rightJoint)
        {
            this.leftJoint = leftJoint;
            this.rightJoint = rightJoint;
        }

        public static Platform GetRandomPlatform()
        {
            return new Platform(Platform.RandomGenerator.Next(0, jointsCount), Platform.RandomGenerator.Next(0, jointsCount));
        }

        public static Platform GetFirstPlatform()
        {
            return new Platform(null, Platform.RandomGenerator.Next(0, jointsCount));
        }

        public Platform GetNextPlatform()
        {
            return new Platform(this.rightJoint, Platform.RandomGenerator.Next(0, jointsCount));
        }

        public Platform GetNextPlatform(int? requestedJoint)
        {
            return new Platform(this.rightJoint, requestedJoint);
        }

        public Platform GetPrevPlatform()
        {
            return new Platform(Platform.RandomGenerator.Next(0, jointsCount), this.leftJoint);
        }

        public Platform GetPrevPlatform(int? requestedJoint)
        {
            return new Platform(requestedJoint, this.leftJoint);
        }
    }
}
