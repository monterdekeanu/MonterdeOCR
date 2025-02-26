using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MonterdeOCR.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {

        [ObservableProperty]
        CameraView selectedCamera;
        [ObservableProperty]
        float currentZoom;

        [ObservableProperty]
        CancellationToken token;
        public MainPageViewModel()
        {
            selectedCamera = new CameraView();
            currentZoom = 1.0f;
            token = CancellationToken.None;
        }

        [RelayCommand]
        private void CaptureImage()
        {
            selectedCamera.CaptureImageCommand.Execute(null);
        }
    }
}
