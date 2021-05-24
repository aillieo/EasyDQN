using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.AI
{
    public class Adam
    {
        private static readonly float eps = 1e-8f;

        public int t = 0;
        public float beta1 = 0.9f;
        public float beta2 = 0.999f;
        public float lr = 1e-3f;

        private void Step(Matrix x, Matrix dx, Matrix m, Matrix v, int batchSize)
        {
            float mpb1t = Mathf.Pow(beta1, t);
            float mpb2t = Mathf.Pow(beta2, t);

            for (int i = 0; i < x.row; i++)
            {
                for (int j = 0; j < x.column; j++)
                {
                    float d = (float)dx[i, j] / batchSize;
                    m[i, j] = m[i, j] * beta1 + (1 - beta1) * d;
                    v[i, j] = v[i, j] * beta2 + (1 - beta2) * d * d;
                    float mb = (float)m[i, j] / (1 - mpb1t);
                    float vb = (float)v[i, j] / (1 - mpb2t);

                    x[i, j] = x[i, j] - lr * (mb / (Mathf.Sqrt(vb) + eps));
                }
            }
        }

        private void Step(FCLayer layer, int batchSize)
        {
            Step(layer.w, layer.dw, layer.mw, layer.vw, batchSize);
            Step(layer.b, layer.db, layer.mb, layer.vb, batchSize);
        }

        public void Step(Net net, float maxGradNorm, int batchSize)
        {
            var fcLayers = net.GetFCLayers();

            // clip_coef限制 防止梯度爆炸和梯度消失
            float normSum = 0f;
            foreach(var layer in fcLayers)
            {
                normSum += (float)layer.w.SqrFNorm();
                normSum += (float)layer.b.SqrFNorm();
            }

            normSum = Mathf.Sqrt(normSum);

            float clipCoef = (float)(maxGradNorm / (normSum + 1e-6));
            if (clipCoef < 1)
            {
                foreach (var layer in fcLayers)
                {
                    layer.dw *= clipCoef;
                    layer.db *= clipCoef;
                }
            }

            t++;

            // 更新layer梯度
            foreach (var layer in fcLayers)
            {
                Step(layer, batchSize);
            }
        }
    }
}
