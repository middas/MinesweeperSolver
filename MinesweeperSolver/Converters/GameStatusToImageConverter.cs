using Minesweeper.Game;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public class GameStatusToImageConverter : IValueConverter
    {
        private const string Smile = "Resources/Smile.png";
        private const string Frown = "Resources/Frown.png";
        private const string Glasses = "Resources/Glasses.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GameStatus gs)
            {
                if (gs == GameStatus.Loss)
                {
                    return Frown;
                }
                else if (gs == GameStatus.Win)
                {
                    return Glasses;
                }
            }

            return Smile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
