using System.ComponentModel;

namespace JocDeDame.Models
{
    public enum PlayerColor
    {
        Red,
        White
    }

    public class Player : INotifyPropertyChanged
    {
        private PlayerColor color;
        private string imagePath;

        public event PropertyChangedEventHandler PropertyChanged;

        public Player(PlayerColor color, string imagePath)
        {
            this.color = color;
            this.imagePath = imagePath;
        }

        public PlayerColor Color
        {
            get => color;
            set
            {
                if (color != value)
                {
                    color = value;
                    NotifyPropertyChanged(nameof(Color));
                }
            }
        }

        public string ImagePath
        {
            get => imagePath;
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    NotifyPropertyChanged(nameof(ImagePath));
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
