using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TowersOfHanoi
{
    public partial class MainWindow : Window
    {
        private List<Move> solution;
        private bool canStartSolving = false;
        private byte diskCount = 3;
        private byte pegCount = 3;
        private PuzzleSolver puzzleSolver;
        private PuzzleVisualizer puzzleVisualizer;

        public MainWindow()
        {
            InitializeComponent();
            ChangeFrameworkElementState(true, startSolveButton);
            ChangeFrameworkElementState(false, abortSolveButton, startVisualizationButton, abortVisualizationButton, resetVisualizationButton);
        }

        private void NumberTextBoxPreviewInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartSolveButton_Clicked(object sender, RoutedEventArgs e)
        {
            uint fsmovecount = FrameStewart.Hanoi(diskCount, pegCount);
            FSmoveCountLabel.Text = $"Frame Stewart: {fsmovecount} moves";
            
            Action solvingCompleted = () => {
                ChangeFrameworkElementState(true, startVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
                ChangeFrameworkElementState(false, abortSolveButton);

                Dispatcher.Invoke(() =>
                {
                    solution = new List<Move>();
                    ReconstructPath(puzzleSolver);
                    Print($"{solution.Count} moves\r{puzzleSolver.ElapsedTime} elapsed\rVisited {puzzleSolver.VisitedStates} states\r");
                    PrintMoves();
                    Print("===================\r");
                    BFSmoveCountLabel.Text = $"BFS: {solution.Count} moves";
                    string elapsedTime = puzzleSolver.ElapsedTime.ToString();
                    elapsedTimeLabel.Text = $"Elapsed time:   {elapsedTime.Substring(3, Math.Min(13, elapsedTime.Length - 3))}";
                });
            };

            Action solvingAborted = () => {
                Dispatcher.Invoke(() =>
                {
                    Print("Aborted\r");
                    Print("===================\r");
                });
            };

            puzzleSolver = new PuzzleSolverBFS(diskCount, pegCount);
            Print("Starting to solve...\r");
            puzzleSolver.Start(solvingCompleted, solvingAborted);

            ChangeFrameworkElementState(true, abortSolveButton);
            canStartSolving = false;
            ChangeFrameworkElementState(false, startSolveButton, startVisualizationButton, abortVisualizationButton, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
        }

        private void AbortSolveButton_Click(object sender, RoutedEventArgs e)
        {
            puzzleSolver.Abort();
            canStartSolving = true;
            ChangeFrameworkElementState(true, startSolveButton, diskCountInputTextBox, pegCountInputTextBox);
            ChangeFrameworkElementState(false, abortSolveButton);
        }

        private void PuzzleParametersChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                diskCount = byte.Parse(diskCountInputTextBox.Text);
                pegCount = byte.Parse(pegCountInputTextBox.Text);
                if (pegCount < 3 || diskCount < 3 || diskCount > 15)
                    throw new FormatException();
            }
            catch (FormatException)
            {
                Console.WriteLine("Couldn't get disk and peg count!");
                return;
            }
            canStartSolving = true;
            puzzleVisualizer = new PuzzleVisualizer(canvas, diskCount, pegCount);
            FSmoveCountLabel.Text = $"Frame Stewart: - moves";
            BFSmoveCountLabel.Text = $"BFS: - moves";
            elapsedTimeLabel.Text = $"Elapsed time:   -";
            ChangeFrameworkElementState(true, startSolveButton);
            ChangeFrameworkElementState(false, abortSolveButton, startVisualizationButton, abortVisualizationButton, resetVisualizationButton);
        }

        private void ReconstructPath(PuzzleSolver puzzleSolver)
        {
            HanoiNode currentNode = puzzleSolver.endNode;
            while (currentNode.parent != null)
            {
                ulong tempState1 = currentNode.state;
                ulong tempState2 = currentNode.parent != null ? currentNode.parent.state : 0;
                for (int i = 0; i < diskCount; ++i)
                {
                    if (tempState1 % 10 != tempState2 % 10)
                    {
                        solution.Add(new Move((byte)(tempState2 % 10), (byte)(tempState1 % 10)));
                    }
                    tempState1 /= 10;
                    tempState2 /= 10;
                }
                currentNode = currentNode.parent;
            }
            solution.Reverse();
        }

        private void PrintMoves()
        {
            uint counter = 1;
            Print("Solution:\r");
            solution.ForEach(move => {
                Print($"{move.from} --> {move.to}  [{counter++}]\r");
                Dispatcher.Invoke(() => { });
            });
        }

        private void StartVisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            Action visualizationCompleted = () => {
                ChangeFrameworkElementState(true, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
                ChangeFrameworkElementState(false, abortVisualizationButton);
            };
            if (puzzleVisualizer.currentState == 0)
            {
                puzzleVisualizer = new PuzzleVisualizer(canvas, diskCount, pegCount);
                puzzleVisualizer?.Start(solution, 200, visualizationCompleted);
            }
            ChangeFrameworkElementState(true, abortVisualizationButton);
            ChangeFrameworkElementState(false, startSolveButton, startVisualizationButton, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
        }

        private void AbortVisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            Action visualizationAborted = () => {
                ChangeFrameworkElementState(true, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
                ChangeFrameworkElementState(false, abortVisualizationButton);
            };
            puzzleVisualizer?.Abort(visualizationAborted);
        }

        private void ResetVisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            puzzleVisualizer = new PuzzleVisualizer(canvas, diskCount, pegCount);
            ChangeFrameworkElementState(true, startVisualizationButton);
            ChangeFrameworkElementState(false, resetVisualizationButton);
        }

        public void Print(string message)
        {
            consoleTextBox.AppendText(message);
        }

        private void ClearConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            consoleTextBox.Document.Blocks.Clear();
        }

        private void ChangeFrameworkElementState(params FrameworkElement[] frameworkElements)
        {
            Dispatcher.Invoke(() =>
            {
                Array.ForEach(frameworkElements, frameworkElement => frameworkElement.IsEnabled = !frameworkElement.IsEnabled);
            });
        }

        private void ChangeFrameworkElementState(bool state, params FrameworkElement[] frameworkElements)
        {
            Dispatcher.Invoke(() =>
            {
                Array.ForEach(frameworkElements, frameworkElement => frameworkElement.IsEnabled = state);
            });
        }
    }
}
