using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TowersOfHanoi
{
    abstract class PuzzleSolver
    {
        private List<Move> solution;
        public List<Move> Solution { get { return new List<Move>(solution); } }
        public TimeSpan ElapsedTime { private set; get; }
        public long VisitedStates { protected set; get; }
        private CancellationTokenSource tokenSource = null;
        private MainWindow mainWindow;

        public PuzzleSolver()
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
        }

        public async void Start(HanoiState startState, HanoiState endState, Action completedAction, Action abortedAction)
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    solution = SolveThread(startState, endState, token);
                    DateTime endTime = DateTime.Now;
                    ElapsedTime = endTime - startTime;
                    completedAction?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    abortedAction?.Invoke();
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
    }
}
