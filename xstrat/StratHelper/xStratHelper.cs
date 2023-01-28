using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using xstrat.MVVM.View;
using XStrat;
using xstrat.Core;
using System.IO;

namespace xstrat.StratHelper
{
    public static class xStratHelper
    {
        public static StratMakerView stratView { get; set; }
        public static WallEditorView editorView { get; set; }
        public static bool WEMode = false;
    }
}
