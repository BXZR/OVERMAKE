﻿#pragma checksum "..\..\..\..\Windows\AllStartServerWondow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "55BBA64313A6FFADE315712366797FBF"
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
using System.Windows.Forms.Integration;
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
using socketServer.Windows;


namespace socketServer.Windows {
    
    
    /// <summary>
    /// AllStartServerWondow
    /// </summary>
    public partial class AllStartServerWondow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button2;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label theShowLabel;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label theShowLabel_Copy;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox groupBox;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ALLIP;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label theShowLabel_Copy1;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label theShowLabel_Copy2;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ALLPort;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\Windows\AllStartServerWondow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button3;
        
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
            System.Uri resourceLocater = new System.Uri("/socketServer;component/windows/allstartserverwondow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\AllStartServerWondow.xaml"
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
            
            #line 8 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            ((socketServer.Windows.AllStartServerWondow)(target)).Closed += new System.EventHandler(this.Window_Closed);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            ((socketServer.Windows.AllStartServerWondow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.button = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            this.button.Click += new System.Windows.RoutedEventHandler(this.button_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.button2 = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            this.button2.Click += new System.Windows.RoutedEventHandler(this.button2_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.theShowLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.theShowLabel_Copy = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.groupBox = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 7:
            this.ALLIP = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.theShowLabel_Copy1 = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.theShowLabel_Copy2 = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.ALLPort = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.button1_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.button3 = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\..\..\Windows\AllStartServerWondow.xaml"
            this.button3.Click += new System.Windows.RoutedEventHandler(this.button3_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

