﻿#pragma checksum "C:\git\SubTwitr\SubTwitr\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D49D2C29CD724E480D33758C3F65F4FD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SubTwitr
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.FilePicker = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 11 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.FilePicker).Click += this.FilePicker_Click;
                    #line default
                }
                break;
            case 2:
                {
                    this.settings = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 3:
                {
                    this.PickFile = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.PickFile).Click += this.FilePicker_Click;
                    #line default
                }
                break;
            case 4:
                {
                    this.TweetFile = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 22 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.TweetFile).Click += this.SendFile_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.OutputTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6:
                {
                    this.playPauseBorder = (global::Windows.UI.Xaml.Controls.Border)(target);
                    #line 25 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Border)this.playPauseBorder).Tapped += this.playPauseBorder_Tapped;
                    #line default
                }
                break;
            case 7:
                {
                    this.mediaElement = (global::Windows.UI.Xaml.Controls.MediaElement)(target);
                    #line 27 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.MediaElement)this.mediaElement).Tapped += this.mediaElement_Tapped_1;
                    #line default
                }
                break;
            case 8:
                {
                    this.playPause = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                    #line 28 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBlock)this.playPause).Tapped += this.playPause_Tapped;
                    #line default
                }
                break;
            case 9:
                {
                    this.image = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 10:
                {
                    this.tweetBox = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 17 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.tweetBox).GotFocus += this.tweetBox_GotFocus;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

