using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.AI
{
    public class Experience
    {
        public Matrix state0;
        public int action0;
        public float reward0;
        public Matrix state1;
        public bool isDone;
    }
}
