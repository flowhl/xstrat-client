using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for ScrimParticipationControl.xaml
    /// </summary>
    public partial class ScrimParticipationControl : UserControl
    {
        public ScrimParticipationControl()
        {
            InitializeComponent();
        }
        public ScrimParticipationControl(int type, string name)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            InitializeComponent();
            if(type == 0)
            {
                IIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.QuestionMark;
            }
            if (type == 1)
            {
                IIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ThumbsUp;
            }
            if (type == 2)
            {
                IIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ThumbsDown;
            }
            TxtName.Content = name;
        }
    }
}
