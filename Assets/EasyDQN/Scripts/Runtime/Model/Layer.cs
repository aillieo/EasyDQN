using System;
using System.Collections.Generic;

namespace AillieoUtils
{
    [System.Serializable]
    public abstract class Layer
    {
        public abstract Matrix Forward(Matrix data);

        public abstract Matrix Backward(Matrix chained);

        public abstract Layer DeepCopy();
    }
}
