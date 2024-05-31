using JocDeDame.Commands;
using JocDeDame.Helpers;
using JocDeDame.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Text;
using Microsoft.Win32;


namespace JocDeDame.ViewModels
{
    public class GameVM : BaseNotification
    {
        public ObservableCollection<ObservableCollection<GameSquare>> Board { get; private set; }
        public ICommand SquareClickCommand { get; private set; }
        public Player CurrentPlayer { get; private set; }
        private GameLogics gameLogic;
        private bool allowMultipleJumps;

        public bool AllowMultipleJumps
        {
            get { return allowMultipleJumps; }
            set
            {
                if (allowMultipleJumps != value)
                {
                    allowMultipleJumps = value;
                    NotifyPropertyChanged(nameof(AllowMultipleJumps));
                }
            }
        }

        public ICommand StartNewGameCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }

        private string statistics;


        private string aboutInfo;

        private int redPiecesLeft;
        private int whitePiecesLeft;

        public int RedPiecesLeft
        {
            get { return redPiecesLeft; }
            set
            {
                redPiecesLeft = value;
                NotifyPropertyChanged(nameof(RedPiecesLeft));
            }
        }

        public int WhitePiecesLeft
        {
            get { return whitePiecesLeft; }
            set
            {
                whitePiecesLeft = value;
                NotifyPropertyChanged(nameof(WhitePiecesLeft));
            }
        }


        public string Statistics
        {
            get { return statistics; }
            set
            {
                statistics = value;
                NotifyPropertyChanged(nameof(Statistics));
            }
        }

        public ICommand ShowStatisticsCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }
        public GameVM()
        {
            RedPiecesLeft = 12; // Numărul inițial de piese pentru jucătorul roșu
            WhitePiecesLeft = 12; // Numărul inițial de piese pentru jucătorul alb
            InitializePlayers();
            InitializeBoard();
            gameLogic = new GameLogics(Board);
            SquareClickCommand = new RelayCommand(param => SquareClick(param));
            StartNewGameCommand = new RelayCommand(_ => StartNewGame());
            SaveCommand = new RelayCommand(_ => SaveGameState());
            LoadCommand = new RelayCommand(_ => LoadGameState());
            ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());
            LoadAboutInfo(); 
            AboutCommand = new RelayCommand(_ => ShowAboutInfo());
        }
        public void StartNewGame()
        {
            // Resetează toate variabilele necesare pentru un joc nou
            RedPiecesLeft = 12;
            WhitePiecesLeft = 12;
            InitializePlayers();
            InitializeBoard();
            //gameLogic.Reset(); // Dacă există logica specifică jocului care trebuie resetată
        }


        private void ShowStatistics()
        {
            var (redScore, whiteScore, piecesLeft) = ScoreHelper.ReadScores();
            MessageBox.Show($"Red Wins: {redScore}, White Wins: {whiteScore},Max Pieces Left: {piecesLeft}", "Statistics");
        }

        private int redPieces;
        private int whitePieces;

        public int RedPieces
        {
            get { return redPieces; }
            set
            {
                redPieces = value;
                NotifyPropertyChanged(nameof(RedPieces));
            }
        }



        public int WhitePieces
        {
            get { return whitePieces; }
            set
            {
                whitePieces = value;
                NotifyPropertyChanged(nameof(WhitePieces));
            }
        }

        //private void SaveGameState()
        //{
        //    GameStateHelper.SaveGame(Board, CurrentPlayer);
        //}

        //private void LoadGameState()
        //{
        //    GameStateHelper.LoadGame(Board, CurrentPlayer);
        //    NotifyPropertyChanged(nameof(Board));
        //    NotifyPropertyChanged(nameof(CurrentPlayer));
        //}

        public void SaveGameState()
        {
            // Deschideți un dialog de salvare fișier
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Obțineți calea fișierului selectat de utilizator
                string filePath = saveFileDialog.FileName;

                // Salvarea stării jocului în fișier
                GameStateHelper.SaveGame(Board, CurrentPlayer, filePath);
            }
        }

        public void LoadGameState()
        {
            // Deschideți un dialog de încărcare fișier
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Obțineți calea fișierului selectat de utilizator
                string filePath = openFileDialog.FileName;

                // Încărcați starea jocului din fișier
                GameStateHelper.LoadGame(Board, CurrentPlayer, filePath);

                // Actualizați interfața utilizatorului pentru a reflecta noua stare a jocului
                NotifyPropertyChanged(nameof(Board));
                NotifyPropertyChanged(nameof(CurrentPlayer));
            }
        }



        private void InitializePlayers()
        {
            CurrentPlayer = new Player(PlayerColor.Red, JocDeDame.Helpers.Utility.redPiece);
        }

        private void InitializeBoard()
        {
            Board = new ObservableCollection<ObservableCollection<GameSquare>>();
            for (int row = 0; row < 8; row++)
            {
                var boardRow = new ObservableCollection<GameSquare>();
                for (int column = 0; column < 8; column++)
                {
                    SquareShade shade = (row + column) % 2 == 0 ? SquareShade.Light : SquareShade.Dark;
                    GamePiece piece = null;
                    if (shade == SquareShade.Dark && (row < 3 || row > 4))
                    {
                        PieceColor color = row < 3 ? PieceColor.White : PieceColor.Red;
                        piece = new GamePiece(color);
                    }
                    boardRow.Add(new GameSquare(row, column, shade, piece));
                }
                Board.Add(boardRow);
            }
        }

        private void SquareClick(object parameter)
        {
            if (parameter is GameSquare square)
            {
                gameLogic.SquareClicked(square, CurrentPlayer);
                NotifyPropertyChanged(nameof(CurrentPlayer));
                // Actualizarea numărului de piese rămase pentru fiecare jucător după fiecare mutare
                RedPiecesLeft = ScoreHelper.CountPieces(Board, PieceColor.Red);
                WhitePiecesLeft = ScoreHelper.CountPieces(Board, PieceColor.White);
            }
        }

        public void LoadAboutInfo()
        {
            string filePath = "C:\\FACULTATE AN 2\\SEMESTRUL 2\\MVP\\JocDeDame\\JocDeDame\\Resources\\AboutFile.txt";
            if (File.Exists(filePath))
            {
                aboutInfo = File.ReadAllText(filePath);
            }
            else
            {
                aboutInfo = "Informații indisponibile.";
            }
        }

        private void ShowAboutInfo()
        {
            AboutWindow aboutWindow = new AboutWindow(aboutInfo);
            aboutWindow.ShowDialog();
        }

    }
}


