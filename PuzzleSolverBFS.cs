using System.Collections.Generic;
using System.Threading;

namespace TowersOfHanoi
{
    class PuzzleSolverBFS : PuzzleSolver
    {
        protected override List<Move> SolveThread(HanoiState startState, HanoiState endState, CancellationToken token)
        {
            //start Breadth-First Search
            List<HanoiState> visitedStates = new List<HanoiState>();
            Queue<HanoiNode> queue = new Queue<HanoiNode>();
            HanoiNode endNode = null;
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

                List<Move> possibleMoves = node.state.GetPossibleMoves();

                HanoiState possibleState;
                HanoiNode possibleNode;

                foreach (Move possibleMove in possibleMoves)
                {
                    possibleState = new HanoiState(node.state, possibleMove);
                    possibleNode = new HanoiNode(possibleState, node.path);
                    possibleNode.path.Add(possibleMove);

                    if (!visitedStates.Contains(possibleState))
                    {
                        if (endState.Equals(possibleState))
                        {
                            endNode = possibleNode;
                            goto bfsEnd;
                        }
                        queue.Enqueue(possibleNode);
                    }
                }
            }
            bfsEnd: { }
            return new List<Move>(endNode.path);
        }
    }
}
