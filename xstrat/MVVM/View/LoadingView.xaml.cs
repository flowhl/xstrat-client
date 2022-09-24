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
using xstrat.Core;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoadingView.xaml
    /// </summary>
    public partial class LoadingView : UserControl
    {
        public string[] messages = {
            "Discovering new ways of making you wait.",
            "Your time is very important to us. Please wait while we ignore you.",
            "Still faster than Windows update.",
            "We are not liable for any broken screens as a result of waiting.",
            "Bored of slow loading spinner?, buy more RAM!",
            "Kindly hold on until I finish a cup of coffee.",
            "We will be back in 1/0 minutes.",
            "Don't panic, Just count to infinite.",
            "Installing Word.exe",
            "Does Anyone Actually Read This?",
            "Doing Something You Don't Wanna Know About",
            "Doing The Impossible",
            "Gathering Hamsters... ",
            "Just go play Minecraft",
            "Just go and user paint or something",
            "Locating the required gigapixels to render...",
            "Spinning up the hamster...",
            "Shovelling coal into the server...",
            "Gathering 1,21 Gigawatt",
            "Loading humorous message ... Please Wait",
            "Gathering Monkeys",
            "Generating Blue Screen of Death.",
            "404 - please just use paint",
            "Optimism – is a lack of information...",
            "Getting your shit together",
            "Kindly hold on as we convert this bug to a feature...",
            "Please hold on as we reheat our coffee",
            "Let's hope it's worth the wait",
            "Whatever you do, don't look behind you...",
            "Please wait... Consulting the manual...",
            "Feel free to spin in your chair",
            "Forget you saw that password I just typed...",
            "Alt-F4 speeds things up.",
            "Initializing the initializer...",
            "Deleting all your filthy videos",
            "CAPS LOCK – Preventing Login Since 1980.",
            "Loading Error...",
            "Installing virus...",
            "What's in the canister?",
            "LMG MOUNTED AND LOADED",
            "Fucking laser sights",
            "◁◁ ▐ ▌ ▷▷",
        };
        private Random rnd = new Random();
        public LoadingView()
        {
            InitializeComponent();
            TextMessage.Text = getLoadingMessage();
            Globals.wnd.lv = this;
        }

        public string getLoadingMessage()
        {
            return messages[rnd.Next(0,messages.Length)];
        }
        public void SetStatusMessage(string message)
        {
            TextStatus.Content = message;
        }
    }
}
