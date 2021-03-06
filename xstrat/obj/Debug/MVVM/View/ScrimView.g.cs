#pragma checksum "..\..\..\..\MVVM\View\ScrimView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D347E80CFDFCDE7F0480CEF9DDF65BC6A7CBF398AFAE11F75E8388F106CDA3C5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using xstrat.Calendar;
using xstrat.MVVM.View;
using xstrat.Ui;


namespace xstrat.MVVM.View {
    
    
    /// <summary>
    /// ScrimView
    /// </summary>
    public partial class ScrimView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.MVVM.View.ScrimView CalendarUISmall;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ReloadBtn;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NewBtn;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ScrimListPanel;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.Calendar.CalendarMonth CalendarMonthUI;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\MVVM\View\ScrimView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.Ui.ScrimFinderControl SFControl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/xstrat;component/mvvm/view/scrimview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MVVM\View\ScrimView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CalendarUISmall = ((xstrat.MVVM.View.ScrimView)(target));
            return;
            case 2:
            this.ReloadBtn = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\..\MVVM\View\ScrimView.xaml"
            this.ReloadBtn.Click += new System.Windows.RoutedEventHandler(this.ReloadBtn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NewBtn = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\MVVM\View\ScrimView.xaml"
            this.NewBtn.Click += new System.Windows.RoutedEventHandler(this.NewBtn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ScrimListPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.CalendarMonthUI = ((xstrat.Calendar.CalendarMonth)(target));
            return;
            case 6:
            this.SFControl = ((xstrat.Ui.ScrimFinderControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

