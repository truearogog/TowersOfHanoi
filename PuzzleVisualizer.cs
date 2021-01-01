using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;
using Move = System.Tuple<byte, byte>;
using System.Threading.Tasks;

namespace TowersOfHanoi
{
    class PuzzleVisualizer
    {
        private HanoiState currentState;
        private List<Move> moves;
        private Canvas canvas;
        private Dictionary<byte, Border> disks = new Dictionary<byte, Border>();
        private CancellationTokenSource tokenSource = null;

        private double margin = 20d;
        private double maxDiscWidth;
        private double minDiscWidth;
        private double diskHeight;
        private double pegOffsetX;
        private double pegOffsetY;

        public void Init(HanoiState currentState, Canvas canvas)
        {
            this.currentState = currentState;
            this.canvas = canvas;
            this.moves = null;
            CreateVisuals();
        }

        private void CreateVisuals()
        {
            canvas.Dispatcher.Invoke(() => {
                disks.Clear();
                canvas.Children.Clear();

                maxDiscWidth = canvas.ActualWidth / currentState.pegCount - margin;
                minDiscWidth = maxDiscWidth / 2;
                diskHeight = (canvas.ActualHeight / 1.3d - 100d) / currentState.diskCount;
                pegOffsetX = canvas.ActualWidth / currentState.pegCount;
                pegOffsetY = canvas.ActualHeight;

                for (byte pegIndex = 0; pegIndex < currentState.pegCount; pegIndex++)
                {
                    Peg peg = currentState[pegIndex];
                    byte diskFromTop = 1;
                    byte disksOnPeg = peg.Count();

                    //create disks
                    while (peg.CanPop())
                    {
                        byte diskValue = peg.Pop();
                        double diskWidth = Essentials.Lerp(minDiscWidth, maxDiscWidth, (double)diskValue / disksOnPeg);
                        Color fillColor = Essentials.HSVtoRGB(255 * (diskFromTop + 1) / (double)disksOnPeg, 1, 1);
                        Border diskRect = new Border() { Width = diskWidth, Height = diskHeight, Background = new SolidColorBrush(fillColor), BorderBrush = Brushes.Black, BorderThickness = new System.Windows.Thickness(3), CornerRadius = new System.Windows.CornerRadius(30d)};
                        disks.Add(diskValue, diskRect);
                        canvas.Children.Add(diskRect);
                        Canvas.SetLeft(diskRect, pegOffsetX * (pegIndex + 0.5d) - diskWidth / 2);
                        Canvas.SetTop(diskRect, pegOffsetY - diskHeight * (disksOnPeg - diskFromTop + 1) - margin / 2);
                        Panel.SetZIndex(diskRect, 1);
                        diskFromTop++;
                    }

                    //create peg
                    Rectangle pegRect = new Rectangle() { Width = 7, Height = canvas.ActualHeight / 1.5d, Fill = Brushes.Black, Stroke = Brushes.Black, StrokeThickness = 1};
                    canvas.Children.Add(pegRect);
                    Canvas.SetLeft(pegRect, pegOffsetX * (pegIndex + 0.5d) - pegRect.Width / 2);
                    Canvas.SetTop(pegRect, pegOffsetY - pegRect.Height - margin / 2);
                    Panel.SetZIndex(pegRect, 0);
                }
            });
        }

        public async void Start(List<Move> moves, Action completedAction = null)
        {
            if (moves == null)
                return;

            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await Task.Factory.StartNew(() => {
                try
                {
                    this.moves = moves;
                    VisualizationThread(token);
                    completedAction?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    
                }
                finally
                {
                    tokenSource.Dispose();
                }
            });
        }

        private void VisualizationThread(CancellationToken token)
        {
            moves.ForEach(move => {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                Thread.Sleep(300);
                if (currentState.CanMove(move.Item1, move.Item2))
                    currentState = new HanoiState(currentState, move.Item1, move.Item2);
                VisualizeCurrentState();
            });
        }

        private void VisualizeCurrentState()
        {
            canvas.Parent.Dispatcher.Invoke(() => {
                for (byte pegIndex = 0; pegIndex < currentState.pegCount; pegIndex++)
                {
                    Peg peg = currentState[pegIndex];
                    byte diskFromTop = 1;
                    byte disksOnPeg = peg.Count();
                    while (peg.CanPop())
                    {
                        byte diskValue = peg.Pop();
                        Border disk = disks[diskValue];
                        Canvas.SetLeft(disk, pegOffsetX * (pegIndex + 0.5d) - disk.Width / 2);
                        Canvas.SetTop(disk, pegOffsetY - diskHeight * (disksOnPeg - diskFromTop + 1) - margin / 2);
                        diskFromTop++;
                    }
                }
            });
        }

        public void Abort(Action completedAction = null)
        {
            tokenSource.Cancel();
            completedAction?.Invoke();
        }

        public bool StateEquals(HanoiState state)
        {
            return state.Equals(currentState);
        }
    }
}
