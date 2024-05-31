using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JocDeDame.Helpers;


namespace JocDeDame.Models
{
    public class GameSquare : INotifyPropertyChanged
    {
        private int row;
        private int column;
        private SquareShade shade;
        private string texture;
        private GamePiece piece;
        private GameLogics gameLogics;


        private static GameSquare selectedSquare = null;




        public event PropertyChangedEventHandler PropertyChanged;

        public GameSquare(int row, int column, SquareShade shade, GamePiece piece)
        {
            this.row = row;
            this.column = column;
            this.shade = shade;
            this.piece = piece;
            this.gameLogics = gameLogics;
            texture = (shade == SquareShade.Dark) ? Utility.redSquare : Utility.whiteSquare;
        }

        public int Row => row;
        public int Column => column;
        public SquareShade Shade => shade;
        public string Texture => texture;
        public GamePiece Piece
        {
            get => piece;
            set
            {
                piece = value;
                NotifyPropertyChanged(nameof(Piece));
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //    public void HandleClick()
        //    {
        //        if(selectedSquare != null && (this.Piece == null || this.Piece.Color != selectedSquare.Piece.Color))
        //        {
        //            // Mută piesa din pătratul selectat anterior în pătratul actual
        //            this.Piece = selectedSquare.Piece;
        //            selectedSquare.Piece = null;

        //            // Notifică ambele pătrate că proprietatea 'Piece' s-a schimbat
        //            selectedSquare.NotifyPropertyChanged(nameof(Piece));
        //            NotifyPropertyChanged(nameof(Piece));

        //            // Deselectează piesa după mutare
        //            selectedSquare = null;
        //        }
        //        else if (this.Piece != null && this.Piece.Color == gameLogics.NextMoveColor) // Dacă se face clic pe un pătrat care conține o piesă
        //        {
        //            // Dacă pătratul este deja selectat, deselectați-l
        //            if (selectedSquare == this)
        //            {
        //                selectedSquare = null;
        //            }
        //            else
        //            {
        //                // Selectează pătratul curent și piesa aferentă
        //                selectedSquare = this;
        //            }
        //        }
        //    }
        //}
    
    public void HandleClick()
    {
        if (selectedSquare != null && (this.Piece == null || this.Piece.Color != selectedSquare.Piece.Color))
        {
            // Mută piesa din pătratul selectat anterior în pătratul actual
            this.Piece = selectedSquare.Piece;
            selectedSquare.Piece = null;

            // Notifică ambele pătrate că proprietatea 'Piece' s-a schimbat
            selectedSquare.NotifyPropertyChanged(nameof(Piece));
            NotifyPropertyChanged(nameof(Piece));

            // Deselectează piesa după mutare
            selectedSquare = null;
        }
        else if (this.Piece != null) // Dacă se face clic pe un pătrat care conține o piesă
        {
            // Selectează pătratul curent și piesa aferentă
            selectedSquare = this;
        }

        // Dacă se face clic pe un pătrat gol și nicio piesă nu este selectată, nu se întâmplă nimic
    }

}
}

        
