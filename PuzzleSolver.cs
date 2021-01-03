using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Move = System.Tuple<byte, byte>;

namespace TowersOfHanoi
{
    abstract class PuzzleSolver
    {
        private List<Move> solution;
        private CancellationTokenSource tokenSource = null;

        public async void Start(HanoiState startState, HanoiState endState, Action completedAction = null)
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Starting to solve...");
                    DateTime startTime = DateTime.Now;

                    solution = SolveThread(startState, endState, token);

                    DateTime endTime = DateTime.Now;
                    Console.WriteLine($"{solution.Count} moves");
                    Console.WriteLine($"{endTime - startTime} elapsed");
                    Console.WriteLine("------------------------------");

                    completedAction?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Aborted");
                    Console.WriteLine("------------------------------");
                }
                finally
                {
                    tokenSource.Dispose();
                }
            });
        }

        protected abstract List<Move> SolveThread(HanoiState startState, HanoiState endState, CancellationToken token);

        public void Abort(Action stoppedAction = null)
        {
            tokenSource.Cancel();
            stoppedAction?.Invoke();
        }

        public List<Move> GetSolution()
        {
            return new List<Move>(solution);
        }
    }
}
