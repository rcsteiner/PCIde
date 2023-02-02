using System.Windows.Controls;
using System.Windows.Media;

namespace Lift
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Interaction logic for Indicator.xaml
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class Indicator : UserControl
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Default Constructor:
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Indicator()
        {
            InitializeComponent();
            SetDirectionOff(Direction.STOPPED);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Direction
        /// </summary>
        /// <param name="dir"> The dir.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetDirectionOff(Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    Up.Fill = Brushes.Black;
                    break;
                case Direction.STOPPED:
                    Down.Fill = Brushes.Black;
                    Up.Fill = Brushes.Black;
                    break;
                case Direction.UP:
                    Down.Fill = Brushes.Black;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Method: Set the Direction
        /// </summary>
        /// <param name="dir"> The dir.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetDirectionOn(Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    Down.Fill = Brushes.Chartreuse;
                    break;
                case Direction.STOPPED:
                    Down.Fill = Brushes.Black;
                    break;
                case Direction.UP:
                    Up.Fill = Brushes.Chartreuse;
                    break;
            }
        }
    }
}

