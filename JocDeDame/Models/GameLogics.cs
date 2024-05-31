using JocDeDame.Helpers;
using JocDeDame.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace JocDeDame.Models
{
    public class GameLogics
    {
        public GameSquare SelectedSquare { get; private set; }
        public ObservableCollection<ObservableCollection<GameSquare>> Board { get; private set; }
        private PieceColor nextMoveColor = PieceColor.Red;

        public GameLogics(ObservableCollection<ObservableCollection<GameSquare>> board)
        {
            this.Board = board;
        }

        public void TogglePlayer(Player currentPlayer)
        {
            currentPlayer.Color = currentPlayer.Color == PlayerColor.Red ? PlayerColor.White : PlayerColor.Red;
            currentPlayer.ImagePath = currentPlayer.Color == PlayerColor.Red ? JocDeDame.Helpers.Utility.redPiece : JocDeDame.Helpers.Utility.whitePiece;
        }

        public void SquareClicked(GameSquare square, Player currentPlayer)
        {
            bool moveCompleted = false;
            if (SelectedSquare == null && square.Piece != null && square.Piece.Color == nextMoveColor)
            {
                SelectedSquare = square;
            }
            else if (SelectedSquare != null)
            {
                if (CanJumpOverOpponent(SelectedSquare, square))
                {
                    PerformJumpAndRemoveOpponent(SelectedSquare, square);
                    moveCompleted = true;
                    if (CanMakeMoreJumps(square))
                    {
                        SelectedSquare = square;
                        moveCompleted = false;
                    }
                    else
                    {
                        nextMoveColor = nextMoveColor == PieceColor.Red ? PieceColor.White : PieceColor.Red;
                        SelectedSquare = null;
                    }
                }
                else if (IsMoveDiagonal(SelectedSquare, square) && IsDestinationFree(square))
                {
                    MovePiece(SelectedSquare, square);
                    moveCompleted = true;
                    SelectedSquare = null;
                    nextMoveColor = nextMoveColor == PieceColor.Red ? PieceColor.White : PieceColor.Red;
                }
            }
            if (moveCompleted && SelectedSquare == null)
            {
                TogglePlayer(currentPlayer);
            }
            CheckForGameOver();
        }

        public bool CanMakeMoreJumps(GameSquare square)
        {
            int[] rowDirections = { -1, 1 };
            int[] colDirections = { -1, 1 };
            foreach (int rowDir in rowDirections)
            {
                foreach (int colDir in colDirections)
                {
                    int opponentRow = square.Row + rowDir;
                    int opponentCol = square.Column + colDir;
                    if (opponentRow >= 0 && opponentRow < 8 && opponentCol >= 0 && opponentCol < 8)
                    {
                        GameSquare opponentSquare = Board[opponentRow][opponentCol];
                        if (CanJumpOverOpponent(square, opponentSquare))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }

        private bool IsMoveDiagonal(GameSquare fromSquare, GameSquare toSquare)
        {
            bool isDiagonal = Math.Abs(fromSquare.Row - toSquare.Row) == 1 && Math.Abs(fromSquare.Column - toSquare.Column) == 1;

            if (!isDiagonal)
            {
                return false;
            }
            if (fromSquare.Piece.Type == PieceType.King)
            {
                return true;
            }
            if (fromSquare.Piece.Color == PieceColor.Red)
            {
                return fromSquare.Row > toSquare.Row;
            }
            else if (fromSquare.Piece.Color == PieceColor.White)
            {
                return fromSquare.Row < toSquare.Row;
            }

            return false;
        }

        public bool IsDestinationFree(GameSquare square)
        {
            return square.Piece == null;
        }

        public void MovePiece(GameSquare fromSquare, GameSquare toSquare)
        {
            toSquare.Piece = fromSquare.Piece;
            fromSquare.Piece = null;
            if ((toSquare.Piece.Color == PieceColor.Red && toSquare.Row == 0) || (toSquare.Piece.Color == PieceColor.White && toSquare.Row == 7))
            {
                toSquare.Piece = new GamePiece(toSquare.Piece.Color, PieceType.King);
            }
        }

        private bool CanJumpOverOpponent(GameSquare fromSquare, GameSquare toSquare)
        {
            int rowDirection = toSquare.Row - fromSquare.Row > 0 ? 1 : -1;
            int colDirection = toSquare.Column - fromSquare.Column > 0 ? 1 : -1;

            if (fromSquare.Piece.Type != PieceType.King)
            {
                if ((fromSquare.Piece.Color == PieceColor.Red && rowDirection == -1) ||
                    (fromSquare.Piece.Color == PieceColor.White && rowDirection == 1))
                {
                    int opponentRow = fromSquare.Row + rowDirection;
                    int opponentCol = fromSquare.Column + colDirection;

                    if (opponentRow >= 0 && opponentRow < 8 && opponentCol >= 0 && opponentCol < 8)
                    {
                        GameSquare opponentSquare = Board[opponentRow][opponentCol];

                        if (opponentSquare.Piece != null && opponentSquare.Piece.Color != fromSquare.Piece.Color)
                        {
                            int landingRow = opponentRow + rowDirection;
                            int landingCol = opponentCol + colDirection;

                            if (landingRow >= 0 && landingRow < 8 && landingCol >= 0 && landingCol < 8)
                            {
                                GameSquare landingSquare = Board[landingRow][landingCol];
                                return landingSquare.Piece == null;
                            }
                        }
                    }
                }
            }
            else
            {
                int opponentRow = fromSquare.Row + rowDirection;
                int opponentCol = fromSquare.Column + colDirection;

                if (opponentRow >= 0 && opponentRow < 8 && opponentCol >= 0 && opponentCol < 8)
                {
                    GameSquare opponentSquare = Board[opponentRow][opponentCol];

                    if (opponentSquare.Piece != null && opponentSquare.Piece.Color != fromSquare.Piece.Color)
                    {
                        int landingRow = opponentRow + rowDirection;
                        int landingCol = opponentCol + colDirection;

                        if (landingRow >= 0 && landingRow < 8 && landingCol >= 0 && landingCol < 8)
                        {
                            GameSquare landingSquare = Board[landingRow][landingCol];
                            return landingSquare.Piece == null;
                        }
                    }
                }
            }

            return false;
        }

        public bool HasAvailableMoves(PieceColor playerColor)
        {
            for (int row = 0; row < Board.Count; row++)
            {
                for (int column = 0; column < Board[row].Count; column++)
                {
                    GameSquare square = Board[row][column];
                    if (square.Piece != null && square.Piece.Color == playerColor)
                    {
                        if (CanMakeMoreJumps(square) || CanMoveNormally(square))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool CanMoveNormally(GameSquare square)
        {
            int[] rowDirections = square.Piece.Color == PieceColor.Red ? new int[] { -1 } : new int[] { 1 };
            int[] colDirections = new int[] { -1, 1 };

            foreach (int rowDir in rowDirections)
            {
                foreach (int colDir in colDirections)
                {
                    int newRow = square.Row + rowDir;
                    int newCol = square.Column + colDir;
                    if (newRow >= 0 && newRow < Board.Count && newCol >= 0 && newCol < Board[newRow].Count)
                    {
                        GameSquare targetSquare = Board[newRow][newCol];
                        if (IsDestinationFree(targetSquare) && IsMoveDiagonal(square, targetSquare))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public void PerformJumpAndRemoveOpponent(GameSquare fromSquare, GameSquare toSquare)
        {
            int rowDirection = toSquare.Row - fromSquare.Row > 0 ? 1 : -1;
            int colDirection = toSquare.Column - fromSquare.Column > 0 ? 1 : -1;
            int opponentRow = fromSquare.Row + rowDirection;
            int opponentCol = fromSquare.Column + colDirection;
            Board[opponentRow][opponentCol].Piece = null;
            MovePiece(fromSquare, toSquare);
        }

        public void CheckForGameOver()
        {
            bool redHasMoves = HasAvailableMoves(PieceColor.Red);
            bool whiteHasMoves = HasAvailableMoves(PieceColor.White);

            if (!redHasMoves || !whiteHasMoves)
            {
                PieceColor winner = !redHasMoves ? PieceColor.White : PieceColor.Red;
                int piecesLeft = ScoreHelper.CountPieces(Board, winner);
                ScoreHelper.UpdateScore(winner, piecesLeft);
                MessageBox.Show($"Jocul s-a încheiat. Câștigătorul este jucătorul {(winner == PieceColor.Red ? "Roșu" : "Alb")}!", "Joc Terminat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

}
