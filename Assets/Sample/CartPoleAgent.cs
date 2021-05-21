using AillieoUtils;
using AillieoUtils.AI;
using UnityEngine;

namespace Sample
{
    public class CartPoleAgent : MonoBehaviour
    {
        public Rigidbody cart;
        public Rigidbody pole;

        public float forceFactor = 320f;

        private DQN brain;
        private int episodeCount;

        public void AgentAction(int action, float deltaTime)
        {
            Vector3 force = Vector3.zero;
            switch (action)
            {
                case 0:
                    force = Vector3.right;
                    break;
                case 1:
                    force = Vector3.left;
                    break;
            }

            force *= forceFactor;

            cart.AddForce(force);
        }

        private void Done()
        {
            TrainingManager.Instance.RecordScore(step);
            AgentReset();
            step = -1;
        }

        public void AgentReset()
        {
            //cart
            cart.velocity = Vector3.zero;
            cart.angularVelocity = Vector3.zero;
            cart.transform.localPosition = Vector3.zero;
            // pole
            pole.velocity = Vector3.zero;
            pole.angularVelocity = Vector3.zero;
            pole.transform.localPosition = new Vector3(0f, 2f, 0f);
            pole.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-5f, 5f));
        }

        public void Init(DQN brain)
        {
            this.brain = brain;
        }

        public void Restart()
        {
            episodeCount++;
            step = 0;
        }

        private float GetReward(out bool isDone)
        {
            float reward = 0.01f;
            isDone = false;

            bool condition = false;
            condition |= (45f < Vector3.Angle(pole.transform.up, Vector3.up));
            condition |= Mathf.Abs(cart.transform.localPosition.x) > 5f;

            if (condition)
            {
                isDone = true;
                reward = -1f;
            }

            return reward;
        }

        private Matrix GetState()
        {
            Matrix s = new Matrix(1, brain.inputSize);
            s[0, 0] = cart.transform.localPosition.x;
            s[0, 1] = cart.velocity.x;
            s[0, 2] = pole.transform.localEulerAngles.z;
            s[0, 3] = pole.angularVelocity.z;
            return s;
        }

        private Matrix lastState;
        private int lastAct;
        public void PreSimu(float deltaTime)
        {
            lastState = GetState();
            lastAct = this.brain.Predict(lastState);
            AgentAction(lastAct, deltaTime);
        }

        private int step = 0;
        private static int lastLog = -1;
        public void PostSimu(float deltaTime)
        {
            float r = GetReward(out bool isDone);

            step++;

            Matrix s1 = GetState();

            float loss = brain.Train(lastState, lastAct, r, s1, isDone);
            loss = loss * loss;

            int frameCount = Time.frameCount;
            if (frameCount != lastLog && frameCount % 30 == 0)
            {
                Debug.Log($"cap={brain.expcount}/{brain.batchCapacity}   loss={loss}   episodeCount={episodeCount}");
                lastLog = frameCount;
            }

            if (isDone)
            {
                Done();
                Restart();
            }
        }
    }
}
