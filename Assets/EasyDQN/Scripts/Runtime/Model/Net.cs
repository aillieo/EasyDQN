using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AillieoUtils.AI
{
    [System.Serializable]
    public class Net
    {
        private List<Layer> layers = new List<Layer>();

        public IEnumerable<Layer> GetLayers()
        {
            return layers;
        }

        public IEnumerable<FCLayer> GetFCLayers()
        {
            return layers.OfType<FCLayer>();
        }

        public IEnumerable<Layer> GetLayersReverse()
        {
            return ((IEnumerable<Layer>)layers).Reverse();
        }

        public IEnumerable<FCLayer> GetFCLayersReverse()
        {
            return layers.OfType<FCLayer>().Reverse();
        }

        public void SynchronizeWith(Net source)
        {
            this.layers = source.layers.Select(l => l.DeepCopy()).ToList();
        }

        public Net(int inCount, int hidden, int outCount)
        {
            // 临时写死 两个FC
            this.layers = new List<Layer>(){
                new FCLayer(inCount, hidden),
                new ReLULayer(),
                new FCLayer(hidden, outCount)
            };
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
