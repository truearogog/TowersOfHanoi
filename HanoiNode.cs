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
            HanoiNode node = obj as HanoiNode;

            if (!state.Equals(node.state))
                return false;

            if (path.Count != node.path.Count)
            {
                return false;
            }

            for (int i = 0; i < path.Count; i++)
            {
                if (!path[i].Equals(node.path[i]))
                    return false;
            }

            return true;
        }
    }
}
