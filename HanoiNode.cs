using System.Collections.Generic;

namespace TowersOfHanoi
{
    class HanoiNode
    {
        public List<Move> path;
        public HanoiState state;

        public HanoiNode(HanoiState state, List<Move> path = null)
        {
            this.state = state;
            this.path = path != null ? path : new List<Move>();
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
