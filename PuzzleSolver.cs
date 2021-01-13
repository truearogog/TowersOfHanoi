using System;
using System.Threading;
using System.Threading.Tasks;

namespace TowersOfHanoi
{
    abstract class PuzzleSolver
    {
        public enum SearchType { BFS, AStar }
        public TimeSpan ElapsedTime { private set; get; }
        public long VisitedStates { protected set; get; }
        private CancellationTokenSource tokenSource = null;
        public HanoiNode endNode;
        protected byte diskCount;
        protected byte pegCount;
        protected ulong startState = 0;
        protected ulong endState = 0;

        public PuzzleSolver(byte diskCount, byte pegCount)
        {
            this.diskCount = diskCount;
            this.pegCount = pegCount;
            //create endState
            ulong m = 1;
            for (byte i = 0; i < diskCount; ++i)
            {
                endState += m * (byte)(pegCount - 1);
                m *= 10;
            }
        }

        public async void Start(Action completedAction, Action abortedAction)
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    SolveThread(token);
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

        protected abstract void SolveThread(CancellationToken token);

        public void Abort(Action stoppedAction = null)
        {
            tokenSource.Cancel();
            stoppedAction?.Invoke();
        }
    }
}
