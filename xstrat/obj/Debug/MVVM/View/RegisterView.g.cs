﻿#pragma checksum "..\..\..\..\MVVM\View\RegisterView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "31F0D2C1A7EDCEEB5660FED2CBED3E2AAB80AE0267ECE3F115D92ED314009163"
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
using xstrat.MVVM.View;


namespace xstrat.MVVM.View {
    
    
    /// <summary>
    /// RegisterView
    /// </summary>
    public partial class RegisterView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Error;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.PackIcon NameIcon;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox username;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.PackIcon EmailIcon;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox email;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.PackIcon PwIcon;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox password;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border StrengthSlider;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RegisterBtn;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\MVVM\View\RegisterView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BackBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/xstrat;component/mvvm/view/registerview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MVVM\View\RegisterView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 9 "..\..\..\..\MVVM\View\RegisterView.xaml"
            ((xstrat.MVVM.View.RegisterView)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UserControl_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Error = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.NameIcon = ((MaterialDesignThemes.Wpf.PackIcon)(target));
            return;
            case 4:
            this.username = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.EmailIcon = ((MaterialDesignThemes.Wpf.PackIcon)(target));
            return;
            case 6:
            this.email = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.PwIcon = ((MaterialDesignThemes.Wpf.PackIcon)(target));
            return;
            case 8:
            this.password = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 30 "..\..\..\..\MVVM\View\RegisterView.xaml"
            this.password.PasswordChanged += new System.Windows.RoutedEventHandler(this.password_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.StrengthSlider = ((System.Windows.Controls.Border)(target));
            return;
            case 10:
            this.RegisterBtn = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\..\MVVM\View\RegisterView.xaml"
            this.RegisterBtn.Click += new System.Windows.RoutedEventHandler(this.RegisterBtn_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.BackBtn = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\..\MVVM\View\RegisterView.xaml"
            this.BackBtn.Click += new System.Windows.RoutedEventHandler(this.BackBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

