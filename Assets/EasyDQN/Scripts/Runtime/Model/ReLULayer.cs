using System;

namespace AillieoUtils.AI
{
    public class ReLULayer : Layer
    {
        private Matrix output;

        public override Matrix Forward(Matrix data)
        {
            this.output = Relu(data);
            return this.output;
        }

        public override Matrix Backward(Matrix chained)
        {
            return Matrix.Hadamard(DRelu(chained, output), chained);
        }

        private static Matrix Relu(Matrix a)
        {
            Matrix b = new Matrix(a.row, a.column);
            for (int i = 0; i < a.row; i++)
            {
                for (int j = 0; j < a.column; j++)
                {
                    b[i, j] = Math.Max(a[i, j], 0);
                }
            }
            return b;
        }

        private static Matrix DRelu(Matrix a, Matrix z)
        {
            Matrix r = new Matrix(a.row, a.column);
            for (int i = 0; i < a.row; i++)
            {
                for (int j = 0; j < a.column; j++)
                {
                    if (z[i, j] <= 0)
                    {
                        r[i, j] = 0;
                    }
                    else
                    {
                        r[i, j] = 1;
                    }
                }
            }
            return r;
        }

        public override Layer DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
}

