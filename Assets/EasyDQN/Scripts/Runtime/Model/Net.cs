using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AillieoUtils.AI
{
    public class Net
    {
        private List<Layer> layers = new List<Layer>();

        public FCLayer layer1;
        public ReLULayer layer2;
        public FCLayer layer3;

        public IEnumerable<Layer> GetLayers()
        {
            return layers;
        }

        public IEnumerable<Layer> GetFCLayers()
        {
            return layers.OfType<FCLayer>();
        }

        public IEnumerable<Layer> GetLayersReverse()
        {
            return ((IEnumerable<Layer>)layers).Reverse();
        }

        public IEnumerable<Layer> GetFCLayersReverse()
        {
            return layers.OfType<FCLayer>().Reverse();
        }

        public void SynchronizeWith(Net source)
        {
            layer1 = new FCLayer();
            layer2 = new ReLULayer();
            layer3 = new FCLayer();
            this.layers = new List<Layer>(){layer1,layer2,layer3};

            layer1.w = source.layer1.w.DeepCopy();
            layer1.b = source.layer1.b.DeepCopy();
            layer1.mw = source.layer1.mw.DeepCopy();
            layer1.vw = source.layer1.vw.DeepCopy();
            layer1.mb = source.layer1.mb.DeepCopy();
            layer1.vb = source.layer1.vb.DeepCopy();

            layer3.w = source.layer3.w.DeepCopy();
            layer3.b = source.layer3.b.DeepCopy();
            layer3.mw = source.layer3.mw.DeepCopy();
            layer3.vw = source.layer3.vw.DeepCopy();
            layer3.mb = source.layer3.mb.DeepCopy();
            layer3.vb = source.layer3.vb.DeepCopy();
        }

        public Net(int inCount, int hidden, int outCount)
        {
            // 临时写死 两个FC
            layer1 = new FCLayer();
            layer2 = new ReLULayer();
            layer3 = new FCLayer();
            this.layers = new List<Layer>(){layer1,layer2,layer3};

            layer1.w = Xavier(inCount, hidden);
            layer3.w = Xavier(hidden, outCount);
            layer1.b = new Matrix(1, hidden);
            layer3.b = new Matrix(1, outCount);

            layer1.mw = new Matrix(inCount, hidden);
            layer1.vw = new Matrix(inCount, hidden);
            layer3.mw = new Matrix(hidden, outCount);
            layer3.vw = new Matrix(hidden, outCount);

            layer1.mb = new Matrix(1, hidden);
            layer1.vb = new Matrix(1, hidden);
            layer3.mb = new Matrix(1, outCount);
            layer3.vb = new Matrix(1, outCount);
        }

        private Matrix Xavier(int inCount, int outCount)
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

        public Matrix Forward(Matrix s)
        {
            Matrix data = s;
            foreach (var l in layers)
            {
                data = l.Forward(data);
            }
            return data;
        }

        public float Backward(Matrix s, int[] act,  Matrix q, float[] qTarget)
        {
            int batchSize = s.row;

            Matrix cost = new Matrix(q.row, q.column);
            for (int i = 0; i < batchSize; i++)
            {
                cost[i, act[i]] = q[i, act[i]] - qTarget[i];
                cost[i, act[i]] = Mathf.Clamp((float)cost[i, act[i]], -1, 1);
            }

            Matrix m = cost;
            for(int i=2; i >=0; --i)
            {
                m = layers[i].Backward(m);
            }

            return (float)cost.FNorm() / s.row;
        }
    }
}
