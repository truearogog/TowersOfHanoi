using System.Collections.Generic;
using Move = System.Tuple<byte, byte>;

namespace TowersOfHanoi
{
    class HanoiNode
    {
        public List<Move> path;
        public HanoiState state;

        public HanoiNode(HanoiState state)
        {
            path = new List<Move>();
            this.state = state;
        }

        public HanoiNode(HanoiNode node, HanoiState state)
        {
            path = new List<Move>(node.path);
            this.state = state;
        }

        public override bool Equals(object obj)
        {
            HanoiNode node = (HanoiNode)obj;

            if (!this.state.Equals(node.state))
                return false;

            if (this.path.Count != node.path.Count)
            {
                return false;
            }

            for (int i = 0; i < this.path.Count; i++)
            {
                if (!this.path[i].Equals(node.path[i]))
                    return false;
            }

            return true;
        }
    }
}
