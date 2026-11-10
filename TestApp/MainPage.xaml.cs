using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestApp
{
    /// <summary>
    /// Demo page hosting several DXModel controls; dragging a model rotates and moves it.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        // Accumulated drag offset, shared by all models on the page.
        double y = 0;
        double x = 0;

        // Uses the drag delta both as rotation (degrees) for the 3D model and as a 2D offset for the panel.
        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            y += e.Delta.Translation.Y;
            x += e.Delta.Translation.X;
            (sender as DirectXModel.DXModel).Transform.Rotation = new System.Numerics.Vector3((float)x, (float)y, 0.0f);
            ((sender as DirectXModel.DXModel).RenderTransform as TranslateTransform).X = x;
            ((sender as DirectXModel.DXModel).RenderTransform as TranslateTransform).Y= y;
        }

        private void OnBackClicked(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        // May be raised off the UI thread; use Dispatcher before touching UI elements.
        private void OnModelLoaded(object sender, bool e)
        {

        }
    }
}
