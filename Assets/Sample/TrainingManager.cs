using System;
using System.Collections.Generic;
using AillieoUtils;
using AillieoUtils.AI;
using UnityEditor;
using UnityEngine;

namespace Sample
{
    public class TrainingManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DisableAutoSimulation()
        {
            Physics.autoSimulation = false;
        }

        private static TrainingManager ins;

        public static TrainingManager Instance
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<TrainingManager>();
                }
                return ins;
            }
        }

        public CartPoleAgent agentTemplate;
        public Vector2Int amount = new Vector2Int(5, 5);
        public Vector2 distance = new Vector2(5, 5);

        [NonSerialized]
        public List<CartPoleAgent> agents = new List<CartPoleAgent>();

        private DQN dqn;

        private void Awake()
        {

            for (int i = 0; i < amount.x; ++i)
            {
                for (int j = 0; j < amount.y; ++j)
                {
                    CartPoleAgent newAgent = GameObject.Instantiate(agentTemplate, this.transform);
                    agents.Add(newAgent);
                    newAgent.transform.localPosition = new Vector3(i * distance.x, 0, j * distance.y);
                }
            }

            dqn = new DQN(new Config());

            foreach (var a in agents)
            {
                a.Init(dqn);
                a.Restart();
            }
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            foreach (var a in agents)
            {
                a.PreSimu(dt);
            }

            if (!Physics.autoSimulation)
            {
                Physics.Simulate(dt);
            }

            foreach (var a in agents)
            {
                a.PostSimu(dt);
            }
        }

        // 临时蹭用一下
        private MemoryReplay<int> timeRecord = new MemoryReplay<int>(100);
        public void RecordScore(int step)
        {
            timeRecord.Add(step);
            Debug.Log($"step={step}  avg={timeRecord.Average(i => (float)i)}  timepassed={Time.timeSinceLevelLoad}");
        }

        public void Save(string path)
        {
            SerializeHelper.SerializeDataToBytes(dqn, path);
        }

        public void Load(string path)
        {
            SerializeHelper.DeserializeBytesToData(path, out DQN loaded);
            if(loaded != null)
            {
                this.dqn = loaded;
                foreach (var a in agents)
                {
                    a.Init(dqn);
                }
            }
        }
    }
}
