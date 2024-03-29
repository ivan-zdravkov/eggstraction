﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    public class PositionTreshholds
    {
        private float horizontalTreshhold;
        private float verticalTreshhold;

        private float upperTreshhold;
        private float lowerTreshhold;
        private float leftTreshhold;
        private float rightTreshhold;

        public float HorizontalTreshhold
        {
            get
            {
                return this.horizontalTreshhold;
            }
        }

        public float VerticalTreshhold
        {
            get
            {
                return this.verticalTreshhold;
            }
        }

        public float UpperTreshhold
        {
            get
            {
                return this.upperTreshhold;
            }
        }

        public float LowerTreshhold
        {
            get
            {
                return this.lowerTreshhold;
            }
        }

        public float LeftTreshhold
        {
            get
            {
                return this.leftTreshhold;
            }
        }

        public float RightTreshhold
        {
            get
            {
                return this.rightTreshhold;
            }
        }

        public PositionTreshholds(int horizontalTreshholdPercentage, int verticalTreshholdPercentage)
        {
            if (horizontalTreshholdPercentage < 0 || horizontalTreshholdPercentage > 50 || verticalTreshholdPercentage < 0 || verticalTreshholdPercentage > 50)
            {
                throw new ArgumentException("The threshholds must be between 0 and 50.");
            }

            this.horizontalTreshhold = ((50 - horizontalTreshholdPercentage) / 100.0f);
            this.verticalTreshhold = ((50 - verticalTreshholdPercentage) / 100.0f);
        }

        public void UpdateTreshholds (float upper, float lower, float left, float right)
        {
            this.upperTreshhold = upper;
            this.lowerTreshhold = lower;
            this.leftTreshhold = left;
            this.rightTreshhold = right;
        }
    }
}
