namespace AillieoUtils.AI
{
    public class Config
    {
        public readonly int batch_capacity = 2048;
        public readonly int batch_size = 128;
        public readonly int updater = 256;
        public readonly int input_size = 4;
        public readonly int hidden_size = 20;
        public readonly int output_size = 2;
        public readonly float learning_rate = 0.001f;
        public readonly float max_grad_norm = 0.5f;
        public readonly float gamma = 0.99f;
        public readonly float epsilon = 1f;
        public readonly float decay = 0.999f;
        public readonly float min_decay = 0.01f;
    }
}

