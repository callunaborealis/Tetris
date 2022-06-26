using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TetrisWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/EmptyBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/IShapeBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/JShapedBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/LShapedBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/SquareBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/SShapedBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TShapedBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/ZShapedBlock.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;

        private GameState gameState = new();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid gameGrid)
        {
            Image[,] imageControls = new Image[gameGrid.Rows, gameGrid.Cols];
            int cellSize = 25;
            for (int row = 0; row < gameGrid.Rows; row++)
            {
                for (int col = 0; col < gameGrid.Cols; col++)
                {
                    Image imageControl = new()
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    // Hide 2 top spawning rows so they are not inside the canvas
                    Canvas.SetTop(imageControl, (row - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, col * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[row, col] = imageControl;
                }
            }
            return imageControls;
        }

        private void DrawGrid(GameGrid gameGrid)
        {
            for (int row = 0; row < gameGrid.Rows; row++)
            {
                for (int col = 0; col < gameGrid.Cols; col++)
                {
                    int id = gameGrid[row, col];
                    // Ensure that the ghost blocks are renderd over the actual blocks
                    imageControls[row, col].Opacity = 1;
                    imageControls[row, col].Source = tileImages[id];
                }
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropHeight = gameState.GetDropHeightOfCurrentBlock();
            foreach (Position position in block.TilePositions())
            {
                imageControls[position.Row + dropHeight, position.Col].Opacity = 0.25;
                imageControls[position.Row + dropHeight, position.Col].Source = tileImages[block.Id];
            }
        }

        private void DrawBlock(Block block)
        {
            foreach(Position position in block.TilePositions())
            {
                imageControls[position.Row, position.Col].Opacity = 1;
                imageControls[position.Row, position.Col].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block nextBlock = blockQueue.NextBlock;
            NextImage.Source = blockImages[nextBlock.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }



        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreNumber.Text = $"{gameState.Score}";
        }

        private async Task StartGame()
        {
            Draw(gameState);
            while (!gameState.GameOver)
            {
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockClockwise();
                    break;
                case Key.Z:
                    gameState.RotateBlockAnticlockwise();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.FastForwardCurrentBlockDrop();
                    break;
                default:
                    return;
            }

            Draw(gameState);

        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
           await StartGame();
        }

        private async void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            gameState = new();
            GameOverMenu.Visibility = Visibility.Hidden;
            await StartGame();

        }
    }
}
