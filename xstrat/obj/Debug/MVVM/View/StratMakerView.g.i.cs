﻿#pragma checksum "..\..\..\..\MVVM\View\StratMakerView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8D8FA9A4EF1FDD7D620D9586ACC2DC310D3624D0EBBF16BE1CFA286B48E03BCF"
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
using WPFExtensions.Controls;
using xstrat.MVVM.View;
using xstrat.Ui;


namespace xstrat.MVVM.View {
    
    
    /// <summary>
    /// StratMakerView
    /// </summary>
    public partial class StratMakerView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu Menu;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox MapSelector;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Alibi;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MapContent;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas WallsLayer;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal xstrat.Ui.WallControl WallControl1;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel MapStack;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\MVVM\View\StratMakerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Kommentar;
        
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
            System.Uri resourceLocater = new System.Uri("/xstrat;component/mvvm/view/stratmakerview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MVVM\View\StratMakerView.xaml"
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
            this.Menu = ((System.Windows.Controls.Menu)(target));
            return;
            case 2:
            this.MapSelector = ((System.Windows.Controls.ComboBox)(target));
            
            #line 29 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            this.MapSelector.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MapSelector_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Alibi = ((System.Windows.Controls.Image)(target));
            
            #line 31 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            this.Alibi.MouseMove += new System.Windows.Input.MouseEventHandler(this.Image_MouseMove);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 42 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            ((WPFExtensions.Controls.ZoomControl)(target)).Drop += new System.Windows.DragEventHandler(this.ZoomControl_Drop);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MapContent = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.WallsLayer = ((System.Windows.Controls.Canvas)(target));
            
            #line 44 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            this.WallsLayer.Drop += new System.Windows.DragEventHandler(this.WallsLayer_Drop);
            
            #line default
            #line hidden
            
            #line 44 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            this.WallsLayer.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.WallsLayer_PreviewMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 44 "..\..\..\..\MVVM\View\StratMakerView.xaml"
            this.WallsLayer.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.WallsLayer_PreviewMouseMove);
            
            #line default
            #line hidden
            return;
            case 7:
            this.WallControl1 = ((xstrat.Ui.WallControl)(target));
            return;
            case 8:
            this.MapStack = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.Kommentar = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

