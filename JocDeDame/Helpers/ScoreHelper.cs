using JocDeDame.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;


namespace JocDeDame.Helpers
{
    public static class ScoreHelper
    {
        private static string filePath = "C:\\FACULTATE AN 2\\SEMESTRUL 2\\MVP\\JocDeDame\\JocDeDame\\Resources\\scores.txt";

        public static void InitializeScores()
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Red: 0,0\nWhite: 0,0");
            }
        }

        public static void UpdateScore(PieceColor winner, int piecesLeft)
        {
            if (!File.Exists(filePath))
            {
                InitializeScores();
            }

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 2) continue;

                int score;
                if (!int.TryParse(parts[0].Split(':')[1].Trim(), out score))
                {
                    continue;
                }

                int maxPieces;
                if (!int.TryParse(parts[1].Trim(), out maxPieces))
                {
                    continue;
                }

                if ((winner == PieceColor.Red && i == 0) || (winner == PieceColor.White && i == 1))
                {
                    score++;
                    maxPieces = Math.Max(maxPieces, piecesLeft);
                    lines[i] = $"{winner}: {score},{maxPieces}";
                    break;
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        //public static (int redScore, int whiteScore, int maxRedPiecesLeft, int maxWhitePiecesLeft) ReadScores()
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        InitializeScores();
        //    }

        //    string[] lines = File.ReadAllLines(filePath);
        //    int redScore = 0, whiteScore = 0, maxRedPiecesLeft = 0, maxWhitePiecesLeft = 0;

        //    try
        //    {
        //        var redParts = lines[0].Split(',');
        //        var whiteParts = lines[1].Split(',');

        //        if (int.TryParse(redParts[0].Split(':')[1].Trim(), out redScore) &&
        //            int.TryParse(redParts[1].Trim(), out maxRedPiecesLeft) &&
        //            int.TryParse(whiteParts[0].Split(':')[1].Trim(), out whiteScore) &&
        //            int.TryParse(whiteParts[1].Trim(), out maxWhitePiecesLeft))
        //        {
        //            return (redScore, whiteScore, maxRedPiecesLeft, maxWhitePiecesLeft);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error parsing scores: " + ex.Message);
        //    }

        //    return (0, 0, 0, 0); 
        //}
        public static (int redWins, int whiteWins, int piecesLeft) ReadScores()
        {
            if (!File.Exists(filePath))
            {
                InitializeScores();
            }

            string[] lines = File.ReadAllLines(filePath);
            int redWins = 0, whiteWins = 0, piecesLeft = 0;

            try
            {
                if (lines.Length >= 2)
                {
                    redWins = int.Parse(lines[0].Split(':')[1].Trim());
                    whiteWins = int.Parse(lines[1].Split(':')[1].Split(',')[0].Trim());

                    if (lines.Length >= 3)
                    {
                        piecesLeft = int.Parse(lines[2].Split(':')[1].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing scores: " + ex.Message);
            }

            return (redWins, whiteWins, piecesLeft);
        }


        public static (int redWinners, int whiteWinners) CountWinners()
        {
            if (!System.IO.File.Exists(filePath))
            {
                InitializeScores();
            }

            string[] lines = System.IO.File.ReadAllLines(filePath);
            int redWinners = 0, whiteWinners = 0;

            try
            {
                var redParts = lines[0].Split(',');
                var whiteParts = lines[1].Split(',');

                if (int.TryParse(redParts[0].Split(':')[1].Trim(), out int redScore) &&
                    int.TryParse(whiteParts[0].Split(':')[1].Trim(), out int whiteScore))
                {
                    redWinners = redScore;
                    whiteWinners = whiteScore;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error counting winners: " + ex.Message);
            }

            return (redWinners, whiteWinners);
        }
        public static int CountPieces(ObservableCollection<ObservableCollection<GameSquare>> board, PieceColor winnerColor)
        {
            int count = 0;
            foreach (var row in board)
            {
                foreach (var square in row)
                {
                    if (square.Piece != null && square.Piece.Color == winnerColor)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
