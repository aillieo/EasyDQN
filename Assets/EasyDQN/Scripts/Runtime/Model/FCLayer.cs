using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.AI
{

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



        private Matrix input;

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
            throw new NotImplementedException();
        }
    }

}

