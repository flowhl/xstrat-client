using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.MVVM.View;

namespace xstrat.StratHelper
{
    public static class xStratHelper
    {
        public static StratMakerView stratView { get; set; }
        public static WallEditorView editorView { get; set; }
        public static bool WEMode = false;
    }
}
