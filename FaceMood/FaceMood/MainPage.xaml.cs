using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceMood.CognitiveEmotion;
using FFImageLoading.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FaceMood
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = new MainPageViewModel();
        }        

        private void ImgPhoto_OnError(object sender, CachedImageEvents.ErrorEventArgs e)
        {
            
        }
    }
}