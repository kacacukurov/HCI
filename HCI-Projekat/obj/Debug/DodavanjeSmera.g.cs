﻿#pragma checksum "..\..\DodavanjeSmera.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BA2BF2FBF072CE6697954C38EE11BF22"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using HCI_Projekat;
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


namespace HCI_Projekat {
    
    
    /// <summary>
    /// DodavanjeSmera
    /// </summary>
    public partial class DodavanjeSmera : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HCI_Projekat.DodavanjeSmera DodavanjeSmeraWindow;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabControlSmer;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem Korak1Smer;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox OznakaSmera;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NazivSmera;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DatumUvodjenja;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox OpisSmera;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button quitButtonTab;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\DodavanjeSmera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button finishButton;
        
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
            System.Uri resourceLocater = new System.Uri("/HCI-Projekat;component/dodavanjesmera.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DodavanjeSmera.xaml"
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
            this.DodavanjeSmeraWindow = ((HCI_Projekat.DodavanjeSmera)(target));
            return;
            case 2:
            
            #line 17 "..\..\DodavanjeSmera.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.exitClickSmer);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 18 "..\..\DodavanjeSmera.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.finishClickSmer);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 19 "..\..\DodavanjeSmera.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.otvoriDatum);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tabControlSmer = ((System.Windows.Controls.TabControl)(target));
            return;
            case 6:
            this.Korak1Smer = ((System.Windows.Controls.TabItem)(target));
            return;
            case 7:
            this.OznakaSmera = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.NazivSmera = ((System.Windows.Controls.TextBox)(target));
            
            #line 37 "..\..\DodavanjeSmera.xaml"
            this.NazivSmera.KeyDown += new System.Windows.Input.KeyEventHandler(this.otvori);
            
            #line default
            #line hidden
            return;
            case 9:
            this.DatumUvodjenja = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 10:
            this.OpisSmera = ((System.Windows.Controls.TextBox)(target));
            
            #line 43 "..\..\DodavanjeSmera.xaml"
            this.OpisSmera.KeyDown += new System.Windows.Input.KeyEventHandler(this.otvoriUnazad);
            
            #line default
            #line hidden
            return;
            case 11:
            this.quitButtonTab = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\DodavanjeSmera.xaml"
            this.quitButtonTab.Click += new System.Windows.RoutedEventHandler(this.exitClickSmer);
            
            #line default
            #line hidden
            return;
            case 12:
            this.finishButton = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\DodavanjeSmera.xaml"
            this.finishButton.Click += new System.Windows.RoutedEventHandler(this.finishClickSmer);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

