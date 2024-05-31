using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JocDeDame.Helpers;


namespace JocDeDame.Models
{
    public class GamePiece : INotifyPropertyChanged
    {
        private PieceColor color;
        private PieceType type;
        private string texture;

        public event PropertyChangedEventHandler PropertyChanged;
        public static event Action<PieceColor> PieceRemoved;

        public GamePiece(PieceColor color)
        {
            this.color = color;
            type = PieceType.Regular;
            texture = (color == PieceColor.Red) ? Utility.redPiece : Utility.whitePiece;
        }


        public GamePiece(PieceColor color, PieceType type)
        {
            this.color = color;
            this.type = type;
            texture = (color == PieceColor.Red) ? Utility.redPiece : Utility.whitePiece;
            if (type == PieceType.King)
            {
                texture = (color == PieceColor.Red) ? Utility.redKingPiece : Utility.whiteKingPiece;
            }
        }

        public PieceColor Color => color;
        public PieceType Type => type;
        public string Texture => texture;

        public void Remove()
        {
            NotifyPropertyChanged("Removed");
            PieceRemoved?.Invoke(this.color);
            OnPieceRemoved();
        }

        protected void OnPieceRemoved()
        {
            PieceRemoved?.Invoke(this.color); // Emit evenimentul cu culoarea piesei
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}