using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TowersOfHanoi
{
    class PuzzleVisualizer
    {
        private List<Move> moves = null;
        private Canvas canvas;
        private Dictionary<byte, Border> disks = new Dictionary<byte, Border>();
        private CancellationTokenSource tokenSource = null;

        public ulong currentState { private set; get; } = 0;

        private byte diskCount;
        private byte pegCount;

        private double margin = 20d;
        private double maxDiscWidth;
        private double minDiscWidth;
        private double diskHeight;
        private double pegOffsetX;
        private double pegOffsetY;

        public PuzzleVisualizer(Canvas canvas, byte diskCount, byte pegCount)
        {
            this.canvas = canvas;
            this.diskCount = diskCount;
            this.pegCount = pegCount;
            CreateVisuals();
        }

        private void CreateVisuals()
        {
            canvas.Parent.Dispatcher.Invoke(() => {
                disks.Clear();
                canvas.Children.Clear();

                maxDiscWidth = canvas.ActualWidth / pegCount - margin;
                minDiscWidth = maxDiscWidth / 2;
                diskHeight = (canvas.ActualHeight / 1.3d - 100d) / diskCount;
                pegOffsetX = canvas.ActualWidth / pegCount;
                pegOffsetY = canvas.ActualHeight;

                //create disks
                for (byte diskIndex = 0; diskIndex < diskCount; ++diskIndex)
                {
                    double diskWidth = Essentials.Lerp(minDiscWidth, maxDiscWidth, (double)diskIndex / diskCount);
                    Color fillColor = Essentials.HSVtoRGB(255 * (diskIndex + 1) / (double)diskCount, 1, 1);
                    Border diskRect = new Border() { Width = diskWidth, Height = diskHeight, Background = new SolidColorBrush(fillColor), BorderBrush = Brushes.Black, BorderThickness = new System.Windows.Thickness(3), CornerRadius = new System.Windows.CornerRadius(10d)};
                    disks.Add(diskIndex, diskRect);
                    canvas.Children.Add(diskRect);
                    Canvas.SetLeft(diskRect, (pegOffsetX - diskWidth) / 2);
                    Canvas.SetTop(diskRect, pegOffsetY - diskHeight * (diskCount - diskIndex) - margin / 2);
                    Panel.SetZIndex(diskRect, 1);
                }

                //create pegs
                for (byte pegIndex = 0; pegIndex < pegCount; ++pegIndex)
                {
                    Rectangle pegRect = new Rectangle() { Width = 7, Height = canvas.ActualHeight / 1.5d, Fill = Brushes.Black, Stroke = Brushes.Black, StrokeThickness = 1 };
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
                Thread.Sleep(500);
                currentState = HanoiOperations.Move(currentState, diskCount, move.from, move.to);
                VisualizeCurrentState();
            });
        }

        private void VisualizeCurrentState()
        {
            canvas.Parent.Dispatcher.Invoke(() => {
                ulong tempState = currentState;
                byte[] disksPos = new byte[diskCount];
                for (int i = 0; i < diskCount; ++i)
                {
                    disksPos[diskCount - i - 1] = (byte)(tempState % 10);
                    tempState /= 10;
                }

                for (int pegIndex = 0; pegIndex < pegCount; ++pegIndex)
                {
                    byte diskHeight = 1;
                    for (int i = 0; i < diskCount; ++i)
                    {
                        if (disksPos[diskCount - i - 1] == pegIndex)
                        {
                            Border disk = disks[(byte)(diskCount - i - 1)];
                            Canvas.SetLeft(disk, pegOffsetX * (pegIndex + 0.5d) - disk.Width / 2);
                            Canvas.SetTop(disk, pegOffsetY - (disk.Height * diskHeight++) - margin / 2);
                        }
                    }
                }
            });
        }

        public void Abort(Action completedAction = null)
        {
            tokenSource.Cancel();
            completedAction?.Invoke();
        }
    }
}
