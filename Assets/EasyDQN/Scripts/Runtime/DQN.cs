using UnityEngine;

namespace AillieoUtils.AI
{
    public class DQN
    {
        private MemoryReplay<Experience> experiences;
        private Net qNet;
        private Net qTargetNet;

        public int batchCapacity => config.batch_capacity;
        public int inputSize => config.input_size;
        public int expcount => experiences.Count;

        private float epsilon;
        private int step;
        private Config config;

        private Adam optimizer;

        // 计算q值用的 避免反复创建
        private Matrix s0Cache;
        private int[] aCache;
        private float[] rCache;
        private Matrix s1Cache;
        private float[] qTargetCache;


        public DQN(Config config)
        {
            experiences = new MemoryReplay<Experience>(config.batch_capacity);
            qNet = new Net(config.input_size, config.hidden_size, config.output_size);
            qTargetNet = new Net(config.input_size, config.hidden_size, config.output_size);
            this.config = config;
            this.epsilon = config.epsilon;

            // 复用
            int batchSize = config.batch_size;
            s0Cache = new Matrix(batchSize, inputSize);
            aCache = new int[batchSize];
            rCache = new float[batchSize];
            s1Cache = new Matrix(batchSize, inputSize);
            qTargetCache = new float[batchSize];

            // 优化器目前只有Adam
            optimizer = new Adam();
            optimizer.lr = config.learning_rate;
            float beta1 = 0.9f;
            float beta2 = 0.999f;
            optimizer.beta1 = beta1;
            optimizer.beta2 = beta2;
        }

        public int Predict(Matrix s)
        {
            // epsilon-greedy
            if (Random.Range(0f, 1f) < epsilon)
            {
                return Random.Range(0, config.output_size);
            }
            else
            {
                Matrix q = qNet.Forward(s);
                int[] act = GetAction(q);
                return act[0];
            }
        }

        public float Train(Matrix s0, int act, float r, Matrix s1, bool isDone)
        {
            experiences.Add(new Experience()
            {
                state0 = s0,
                action0 = act,
                reward0 = r,
                state1 = s1,
                isDone = isDone
            });

            if (experiences.Count < batchCapacity)
            {
                return 0;
            }

            int batchSize = config.batch_size;

            var selected = experiences.Sample(batchSize);

            // 经验存入
            for (int i = 0; i < batchSize; i++)
            {
                Experience exp = selected[i];
                rCache[i] = exp.reward0;
                aCache[i] = exp.action0;
                for (int j = 0; j < inputSize; j++)
                {
                    s0Cache[i, j] = exp.state0[0, j];
                    s1Cache[i, j] = exp.state1[0, j];
                }
            }

            Matrix qTar = qTargetNet.Forward(s1Cache);

            int[] maxQ = GetAction(qTar);

            for (int j = 0; j < batchSize; j++)
            {
                isDone = selected[j].isDone;
                if (isDone)
                {
                    qTargetCache[j] = rCache[j];
                }
                else
                {
                    qTargetCache[j] = rCache[j] + config.gamma * (float)qTar[j, maxQ[j]];
                }
            }

            Matrix q = qNet.Forward(s0Cache);
            float loss = qNet.Backward(s0Cache, aCache, q, qTargetCache);

            optimizer.Step(qNet, config.max_grad_norm, batchSize);

            if (step % config.updater == 0)
            {
                // 更新qtarget网络
                qTargetNet.SynchronizeWith(qNet);
                Debug.Log("Synchronize");
            }

            // 更新epsilon
            epsilon = Mathf.Max(epsilon * config.decay, config.min_decay);

            step++;
            return loss;
        }

        public static int[] GetAction(Matrix q)
        {
            // q.row 可能是一个batch_size
            int[] maxi = new int[q.row];
            double maxq;
            for (int i = 0; i < q.row; i++)
            {
                maxq = q[i, 0];
                for (int j = 0; j < q.column; j++)
                {
                    if (maxq < q[i, j])
                    {
                        maxq = q[i, j];
                        maxi[i] = j;
                    }
                }
            }
            return maxi;
        }
    }
}

