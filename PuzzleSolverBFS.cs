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

                foreach (Move possibleMove in possibleMoves)
                {
                    HanoiState possibleState = new HanoiState(node.state, possibleMove);
                    HanoiNode possibleNode = new HanoiNode(possibleState, node.path);
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
            VisitedStates = visitedStates.Count;
            return new List<Move>(endNode.path);
        }
    }
}