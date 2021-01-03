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
        private byte diskCount;
        private byte pegCount;
        private HanoiState startState;
        private HanoiState endState;
        private PuzzleSolver puzzleSolver;
        private PuzzleVisualizer puzzleVisualizer = new PuzzleVisualizer();

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
            if (startState == null || endState == null)
                return;

            uint fsmovecount = FrameStewart.Hanoi(startState.diskCount, startState.pegCount);
            FSmoveCountLabel.Content = $"{fsmovecount} moves";
            puzzleSolver = new PuzzleSolverBFS();

            Action solvingCompleted = () => {
                ChangeFrameworkElementState(true, startVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
                ChangeFrameworkElementState(false, abortSolveButton);

                Dispatcher.Invoke(() =>
                {
                    BFSmoveCountLabel.Content = $"{puzzleSolver.GetSolution().Count} moves";
                });
            };

            puzzleSolver.Start(startState, endState, solvingCompleted);

            ChangeFrameworkElementState(true, abortSolveButton);
            ChangeFrameworkElementState(false, startSolveButton, startVisualizationButton, abortVisualizationButton, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
        }

        private void AbortSolveButton_Click(object sender, RoutedEventArgs e)
        {
            puzzleSolver.Abort();
            ChangeFrameworkElementState(true, startSolveButton, diskCountInputTextBox, pegCountInputTextBox);
            ChangeFrameworkElementState(false, abortSolveButton);
        }

        private void PuzzleParametersChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                diskCount = byte.Parse(diskCountInputTextBox.Text);
                pegCount = byte.Parse(pegCountInputTextBox.Text);
            }
            catch (FormatException)
            {
                Console.WriteLine("Couldn't get disk and peg count!");
                return;
            }
            startState = new HanoiState(diskCount, pegCount, 0);
            endState = new HanoiState(diskCount, pegCount, (byte)(pegCount - 1));
            puzzleVisualizer.Init(startState, canvas);
            BFSmoveCountLabel.Content = $"- moves";
            FSmoveCountLabel.Content = $"- moves";
            ChangeFrameworkElementState(true, startSolveButton);
            ChangeFrameworkElementState(false, abortSolveButton, startVisualizationButton, abortVisualizationButton, resetVisualizationButton);
        }

        private void PrintMoves(PuzzleSolver puzzleSolver)
        {
            List<Move> solutionPath = puzzleSolver.GetSolution();
            byte counter = 1;
            Console.WriteLine("Solution : ");
            solutionPath.ForEach(move => { Console.WriteLine($"{move.from} --> {move.to}  [{counter++}]"); });
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

        private void StartVisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            Action visualizationCompleted = () => {
                ChangeFrameworkElementState(true, resetVisualizationButton, diskCountInputTextBox, pegCountInputTextBox);
                ChangeFrameworkElementState(false, abortVisualizationButton);
            };
            if (puzzleVisualizer.StateEquals(startState))
            {
                puzzleVisualizer?.Start(puzzleSolver.GetSolution(), visualizationCompleted);
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
            puzzleVisualizer.Init(startState, canvas);
            ChangeFrameworkElementState(true, startVisualizationButton);
            ChangeFrameworkElementState(false, resetVisualizationButton);
        }
    }
}
