using CommunityToolkit.Maui.Views;
using MonterdeOCR.ViewModels;
using Plugin.Maui.OCR;
namespace MonterdeOCR
{
    public partial class MainPage : ContentPage
    {
        private readonly IOcrService _ocr;
        public MainPage(MainPageViewModel vm, IOcrService feature)
        {
            InitializeComponent();
            BindingContext = vm;
            Camera.MediaCaptured += OnMediaCaptured;
            _ocr = feature;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _ocr.InitAsync();
        }
        readonly string imagePath;
        void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
        {
            if (Dispatcher.IsDispatchRequired)
            {
                Dispatcher.Dispatch(async () =>
                {
                    // Copy e.Media into a MemoryStream
                    byte[] imageBytes;
                    using (var ms = new MemoryStream())
                    {
                        e.Media.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }

                    // Display the image using the copied bytes
                    image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                    // Pass the same bytes to the OCR plugin
                    var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageBytes, true);
                    if (!ocrResult.Success)
                    {
                        await DisplayAlert("No Success", "Found Nothing", "Ok");
                        return;
                    }
                    await DisplayAlert("Success", ocrResult.AllText, "Ok");
                });
                return;
            }
            image.Source = ImageSource.FromStream(() => e.Media);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Camera.CaptureImage(CancellationToken.None);
        }

        private async void Button_PartialScrenshot(object sender, EventArgs e)
        {
            try
            {
                // Capture partial screenshot from your Camera
                var screenshot = await containerView.CaptureAsync();
                if (screenshot == null)
                {
                    await DisplayAlert("Capture Error", "No partial screenshot returned.", "OK");
                    return;
                }

                // Copy the screenshot stream into a byte array
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await screenshot.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                // Update the UI on the main thread
                if (Dispatcher.IsDispatchRequired)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    });
                }
                else
                {
                    image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }

}
