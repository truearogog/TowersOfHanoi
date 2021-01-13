using System;

namespace TowersOfHanoi
{
    class HanoiNodeValued : HanoiNode
    {
        public int value = 0;

        public HanoiNodeValued(ulong state) : base(state)
        {
        }

        public HanoiNodeValued(ulong state, ref HanoiNode parent) : base(state, ref parent)
        {
        }

        public int F(int cost, byte diskCount, byte pegCount)
        {
            value = G(cost) + H(diskCount, pegCount);
            return value;
        }

        private int G(int cost)
        {
            return cost + 1;
        }

        private int H(byte diskCount, byte pegCount)
        {
            ulong tempState = state;
            int h = 0;
            while (tempState > 0)
            {
                h += ((tempState % 10) == (byte)(pegCount - 1)) ? 1 : 0; // diskCount-- : 0;
                tempState /= 10;
            }
            return h;
        }

        public override bool Equals(object obj)
        {
            HanoiNodeValued otherNode = obj as HanoiNodeValued;
            return state == otherNode.state;
        }
    }
}