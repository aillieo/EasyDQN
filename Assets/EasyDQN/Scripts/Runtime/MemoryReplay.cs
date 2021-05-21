using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils.AI
{
    public class MemoryReplay<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection
    {
        private readonly Queue<T> queue;
        private int size;

        private static readonly int defaultCapacity = 16;

        public MemoryReplay(int size) :
            this(size, defaultCapacity)
        {
        }

        public MemoryReplay(int size, int capacity)
        {
            this.size = size;
            queue = new Queue<T>(capacity);
        }

        public int Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    if (value < size)
                    {
                        size = value;
                        Trim();
                    }
                    else
                    {
                        size = value;
                    }
                }
            }
        }

        private void Trim()
        {
            while (queue.Count > size)
            {
                queue.Dequeue();
            }
        }

        public int Count => queue.Count;

        public void Add(T item)
        {
            if (queue.Count >= size)
            {
                queue.Dequeue();
            }

            queue.Enqueue(item);
        }

        public float Sum(Func<T, float> selector)
        {
            return queue.Sum(selector);
        }

        public float Average(Func<T, float> selector)
        {
            return queue.Average(selector);
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Clear()
        {
            queue.Clear();
        }

        public bool Contains(T item)
        {
            return queue.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)queue).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public T[] Sample(int batch)
        {
            // 效率很低 受限于queue 后续优化
            int[] pool = new int[size];
            int[] result = new int[batch];
            for (int i = 0; i < size; i++)
            {
                pool[i] = i;
            }

            // 随机取batch
            for (int i = 0; i < batch; i++)
            {
                int j = UnityEngine.Random.Range(0, size - i - 1);
                result[i] = pool[j];
                // 抽取 去掉重复的 抽完换到最后一个
                pool[j] = pool[size - i - 1];
            }
            // 随机取样完成 此时result中存的是 batch个index
            
            System.Array.Sort(result);
            T[] selected = new T[batch];
            int cursorInExp = 0;
            int cursorInRes = 0;
            foreach (var e in queue)
            {
                if (result[cursorInRes] == cursorInExp)
                {
                    selected[cursorInRes] = e;
                    cursorInRes++;
                    if (cursorInRes >= selected.Length)
                    {
                        break;
                    }
                }

                cursorInExp++;
            }
            return selected;
        }
    }
}
