using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PentagoWeb.Model.Board;

namespace PentagoWeb.Helper
{
    public class ChessPieceConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Status.StateEnum type = (Status.StateEnum)value;
            if (type == Status.StateEnum.black)
            {
                return new BitmapImage(new Uri("/PentagoWeb;component/resource/player1.jpg", UriKind.Relative));
            }
            else if (type == Status.StateEnum.white)
            {
                return new BitmapImage(new Uri("/PentagoWeb;component/resource/player2.jpg", UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
