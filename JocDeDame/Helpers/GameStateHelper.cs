using JocDeDame.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace JocDeDame.Helpers
{
    public static class GameStateHelper
    {
        private static string saveFilePath = "C:\\FACULTATE AN 2\\SEMESTRUL 2\\MVP\\JocDeDame\\JocDeDame\\Resources\\GameState.txt";

        public static void SaveGame(ObservableCollection<ObservableCollection<GameSquare>> board, Player currentPlayer, string filePath)
        {
            var sb = new StringBuilder();
            foreach (var row in board)
            {
                foreach (var square in row)
                {
                    if (square.Piece == null)
                        sb.Append('_');
                    else if (square.Piece.Color == PieceColor.Red && square.Piece.Type == PieceType.Regular)
                        sb.Append('R');
                    else if (square.Piece.Color == PieceColor.White && square.Piece.Type == PieceType.Regular)
                        sb.Append('W');
                    else if (square.Piece.Color == PieceColor.Red && square.Piece.Type == PieceType.King)
                        sb.Append('K');
                    else if (square.Piece.Color == PieceColor.White && square.Piece.Type == PieceType.King)
                        sb.Append('B');
                }
                sb.AppendLine();
            }
            sb.AppendLine(currentPlayer.Color == PlayerColor.Red ? "Red" : "White");
            File.WriteAllText(filePath, sb.ToString()); // Salvare în calea specificată de utilizator
        }

        public static void LoadGame(ObservableCollection<ObservableCollection<GameSquare>> board, Player currentPlayer, string filePath)
        {
            string[] lines = File.ReadAllLines(filePath); ;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char c = lines[i][j];
                    GamePiece piece = null;
                    if (c == 'R') piece = new GamePiece(PieceColor.Red);
                    else if (c == 'W') piece = new GamePiece(PieceColor.White);
                    else if (c == 'K') piece = new GamePiece(PieceColor.Red, PieceType.King);
                    else if (c == 'B') piece = new GamePiece(PieceColor.White, PieceType.King);
                    board[i][j].Piece = piece;
                }
            }
            currentPlayer.Color = lines[8].Trim() == "Red" ? PlayerColor.Red : PlayerColor.White;
        }
    }
}
