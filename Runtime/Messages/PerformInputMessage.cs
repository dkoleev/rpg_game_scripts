namespace Darkness.Runtime.Messages {
    public struct PerformInputMessage {
        public enum InputType {
            Attack,
            Roll,
            Block
        }

        public enum InputPhase {
            Started,
            Performed,
            Cancelled
        }
        
        public InputType Type;
        public InputPhase Phase;
        public bool IsSlowAttack;
    }
}