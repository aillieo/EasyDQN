using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AillieoUtils.AI
{
    [System.Serializable]
    public class FCLayer : Layer
    {
        // 第1层weight
        public Matrix w;
        // 第1层bias
        public Matrix b;
        // loss对b的梯度
        public Matrix db;
        // loss对w的梯度
        public Matrix dw;
        // w一阶动量
        public Matrix mw;
        // w二阶动量
        public Matrix vw;
        // b一阶动量
        public Matrix mb;
        // b二阶动量
        public Matrix vb;

        public FCLayer(int inCount, int outCount)
        {
            w = Xavier(inCount, outCount);
            b = new Matrix(1, outCount);
            dw = new Matrix(inCount, outCount);
            db = new Matrix(1, outCount);
            mw = new Matrix(inCount, outCount);
            vw = new Matrix(inCount, outCount);
            mb = new Matrix(1, outCount);
            vb = new Matrix(1, outCount);
        }

        private FCLayer()
        { }

        private Matrix input;

        private static Matrix Xavier(int inCount, int outCount)
        {
            Matrix u = new Matrix(inCount, outCount);
            float un = Mathf.Sqrt(1.0f / (inCount * outCount));

            for (int i = 0; i < inCount; i++)
            {
                for (int j = 0; j < outCount; j++)
                {
                    u[i, j] = Random.Range(-un, un);
                }
            }
            return u;
        }

        public override Matrix Forward(Matrix data)
        {
            input = data;
            //return data * w + b;
            // 这个地方做一些特殊处理
            // input 可能是[1,inSize] 或者是[batchSize,inSize]
            // 为了兼容 使用一下RowExtend
            return data * w + RowExtend(b, data.row);
        }

        public override Matrix Backward(Matrix chained)
        {
            db = Sum(chained);
            dw = input.Transpose() * chained;
            return chained * w.Transpose();
        }

        private static Matrix RowExtend(Matrix bias, int rowCount)
        {
            if (bias.row != 1)
            {
                throw new Exception();
            }
            Matrix mat = new Matrix(rowCount, bias.column);
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < bias.column; ++j)
                {
                    mat[i, j] = bias[0, j];
                }
            }
            return mat;
        }

        private static Matrix Sum(Matrix g)
        {
            Matrix mat = new Matrix(1, g.row);
            for (int j = 0; j < g.column; j++)
            {
                for (int i = 0; i < g.row; i++)
                {
                    mat[0, j] += g[i, j];
                }
            }
            return mat;
        }

        public override Layer DeepCopy()
        {
            FCLayer layer = new FCLayer();
            layer.w = this.w.DeepCopy();
            layer.b = this.b.DeepCopy();
            layer.dw = this.dw.DeepCopy();
            layer.dw = this.dw.DeepCopy();
            layer.mw = this.mw.DeepCopy();
            layer.vw = this.vw.DeepCopy();
            layer.mb = this.mb.DeepCopy();
            layer.vb = this.vb.DeepCopy();
            return layer;
        }
    }

}

