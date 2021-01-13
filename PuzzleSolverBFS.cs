using System.Collections.Generic;
using System.Threading;

namespace TowersOfHanoi
{
    class PuzzleSolverBFS : PuzzleSolver
    {
        public PuzzleSolverBFS(byte diskCount, byte pegCount) : base(diskCount, pegCount)
        {
        }

        protected override void SolveThread(CancellationToken token)
        {
            //start Breadth-First Search
            HashSet<ulong> visitedStates = new HashSet<ulong>();
            Queue<HanoiNode> queue = new Queue<HanoiNode>();

            queue.Enqueue(new HanoiNode(startState));

            while (queue.Count > 0)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                HanoiNode node = queue.Dequeue();

                if (visitedStates.Contains(node.state))
                {
                    continue;   
                }

                visitedStates.Add(node.state);

                for (byte i = 0; i < pegCount; ++i)
                {
                    for (byte j = 0; j < pegCount; ++j)
                    {
                        if (i == j)
                            continue;
                        ulong possibleState = HanoiOperations.Move(node.state, diskCount, i, j);
                        if (node.state != possibleState)
                        {
                            if (!visitedStates.Contains(possibleState))
                            {
                                HanoiNode possibleNode = new HanoiNode(possibleState, ref node);
                                if (endState.Equals(possibleState))
                                {
                                    endNode = possibleNode;
                                    VisitedStates = visitedStates.Count;
                                    return;
                                }
                                queue.Enqueue(possibleNode);
                            }
                        }
                    }
                }
            }

            VisitedStates = visitedStates.Count;
        }
    }
}