using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils
{
    public class Vector
    {
        internal double[] data { get; private set; }
        public int size { get; private set; }

        public Vector(int size)
        {
            this.size = size;
            data = new double[size];
        }

        public Vector(double[] vector)
        {
            size = vector.Length;
            data = new double[size];
            Array.Copy(vector, data, size);
        }

        public double this[int index] {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public static implicit operator Matrix(Vector vector)
        {
            return new Matrix(vector.data);
        }

        public override string ToString()
        {
            return $"({string.Join(",", data)})";
        }

        public void Resize(int newSize)
        {
            if (size == newSize)
            {
                return;
            }

            if(newSize > data.Length)
            {
                double[] d = this.data;
                Array.Resize(ref d, newSize);
                this.data = d;
            }

            this.size = newSize;
        }

        public static Vector operator *(Vector matrix, double scalar)
        {
            for (int i = 0; i < matrix.size; i++)
            {
                matrix[i] = matrix[i] * scalar;
            }
            return matrix;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.size != b.size)
            {
                throw new Exception("size not match");
            }

            Vector result = new Vector(a.size);
            for (int i = 0; i < a.size; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return a + b * (-1);
        }

        public double[] ToArray()
        {
            var arr = new double[size];
            Array.Copy(data, arr, size);
            return arr;
        }

        public static Vector Join(IEnumerable<Vector> vectors)
        {
            int total = vectors.Sum(v => v.size);
            Vector vector = new Vector(total);
            int i = 0;
            foreach (var v in vectors)
            {
                for (int j = 0; j < v.size; ++j)
                {
                    vector[i++] = v[j];
                }
            }
            return vector;
        }

        public Vector Silce(int start, int end)
        {
            int count = end - start;
            Vector v = new Vector(count);
            for (int i = 0; i < count; ++ i)
            {
                v[i] = this[i + start];
            }

            return v;
        }
    }

}
