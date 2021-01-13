namespace TowersOfHanoi
{
    class HanoiNode
    {
        public ulong state;
        public HanoiNode parent = null;

        public HanoiNode(ulong state)
        {
            this.state = state;
        }

        public HanoiNode(ulong state, ref HanoiNode parent)
        {
            this.state = state;
            this.parent = parent;
        }

        public override bool Equals(object obj)
        {
            HanoiNode otherNode = obj as HanoiNode;
            return state == otherNode.state;
        }
    }
}
