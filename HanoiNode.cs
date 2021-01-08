using System.Collections.Generic;
using System.Linq;

namespace TowersOfHanoi
{
    class HanoiNode
    {
        public List<Move> path;
        public HanoiState state;

        public HanoiNode(HanoiState state, List<Move> path = null)
        {
            this.state = state;
            this.path = path != null ? new List<Move>(path) : new List<Move>();
        }

        public override bool Equals(object obj)
        {
            HanoiNode otherNode = obj as HanoiNode;

            if (!state.Equals(otherNode.state))
            {
                return false;
            }

            return Enumerable.SequenceEqual(path, otherNode.path);
        }
    }
}
