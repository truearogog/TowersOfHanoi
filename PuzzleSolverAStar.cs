using System.Collections.Generic;
using System.Threading;

namespace TowersOfHanoi
{
    class PuzzleSolverAStar : PuzzleSolver
    {
        private List<HanoiNodeValued> openList = new List<HanoiNodeValued>();
        private HashSet<ulong> closedList = new HashSet<ulong>();

        public PuzzleSolverAStar(byte diskCount, byte pegCount) : base(diskCount, pegCount)
        {
        }

        private HanoiNodeValued SelectNextNode(int cost)
        {
            int minorF = int.MaxValue;
            int minorIndex = 0;
            for (int i = 0; i < openList.Count; ++i)
            {
                int nodeF = openList[i].F(cost, diskCount, pegCount);
                if (nodeF < minorF)
                {
                    minorF = nodeF;
                    minorIndex = i;
                }
            }
            HanoiNodeValued selectedNode = openList[minorIndex];
            openList.RemoveAt(minorIndex);
            return selectedNode;
        }

        protected override void SolveThread(CancellationToken token)
        {
            int cost = -1;
            openList.Add(new HanoiNodeValued(startState));
            while (openList.Count > 0)
            {
                HanoiNode node = SelectNextNode(cost);
                cost += 1;
                closedList.Add(node.state);
                if (node.state == endState)
                {
                    //end
                    endNode = node;
                    VisitedStates = openList.Count + closedList.Count;
                    return;
                }
                else
                {
                    for (byte i = 0; i < pegCount; ++i)
                    {
                        for (byte j = 0; j < pegCount; ++j)
                        {
                            if (i == j)
                                continue;
                            ulong possibleState = HanoiOperations.Move(node.state, diskCount, i, j);
                            if (node.state != possibleState)
                            {
                                HanoiNodeValued possibleNode = new HanoiNodeValued(possibleState, ref node);
                                bool newState = !closedList.Contains(possibleState);
                                if (newState)
                                {
                                    for (int c = 0; c < openList.Count; ++c)
                                    {
                                        if (openList[c].state == possibleState)
                                        {
                                            newState = false;
                                            if (openList[c].value > possibleNode.F(cost, diskCount, pegCount))
                                            {
                                                openList.RemoveAt(c);
                                                openList.Add(possibleNode);
                                            }
                                        }
                                    }
                                    if (newState)
                                    {
                                        openList.Add(possibleNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
