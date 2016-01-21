using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    public class Platform
    {
        public float LocationX { get; set; }

        public float LocationY { get; set; }

        public int? LeftJoint { get; set; }

        public int? RightJoint { get; set; }
    }
}
