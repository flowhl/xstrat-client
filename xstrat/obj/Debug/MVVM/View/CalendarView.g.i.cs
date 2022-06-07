﻿#pragma checksum "..\..\..\..\MVVM\View\CalendarView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "FDE133A1061D3EF297C4A22F2F04EB6DD0556178DBA90D4E80D1D35E2B0BC598"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace xstrat.MVVM.View {
    
    
    /// <summary>
    /// CalendarView
    /// </summary>
    public partial class CalendarView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.MVVM.View.CalendarView CalendarUI;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.Calendar.CalendarMonth CalendarMonthUI;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Row1;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox OffDayChecks;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ScrimChecks;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Row2;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Row3;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Row4;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\MVVM\View\CalendarView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Row5;
        
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
            System.Uri resourceLocater = new System.Uri("/xstrat;component/mvvm/view/calendarview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MVVM\View\CalendarView.xaml"
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
            this.CalendarUI = ((xstrat.MVVM.View.CalendarView)(target));
            return;
            case 2:
            this.CalendarMonthUI = ((xstrat.Calendar.CalendarMonth)(target));
            return;
            case 3:
            this.Row1 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.OffDayChecks = ((System.Windows.Controls.CheckBox)(target));
            
            #line 21 "..\..\..\..\MVVM\View\CalendarView.xaml"
            this.OffDayChecks.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\..\MVVM\View\CalendarView.xaml"
            this.OffDayChecks.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ScrimChecks = ((System.Windows.Controls.CheckBox)(target));
            
            #line 22 "..\..\..\..\MVVM\View\CalendarView.xaml"
            this.ScrimChecks.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\..\MVVM\View\CalendarView.xaml"
            this.ScrimChecks.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Row2 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 7:
            this.Row3 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 8:
            this.Row4 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.Row5 = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

