using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Views;
using MonterdeOCR.Helper;
using MonterdeOCR.ViewModels;
using Plugin.Maui.OCR;
namespace MonterdeOCR
{
    public partial class MainPage : ContentPage
    {
        private readonly IOcrService _ocr;
        private readonly PlateNumberValidator _validator;
        public MainPage(MainPageViewModel vm, IOcrService feature, PlateNumberValidator validator)
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
                    string foundPlateNumber = "not found";
                    foreach (string text in ocrResult.Lines)
                    {
                        if (IsValidPlateNumber(text))
                        {
                            foundPlateNumber = text;
                        }
                    }
                        await DisplayAlert("Success", foundPlateNumber, "Ok");
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
        private static bool IsValidPlateNumber(string plateNumber)
        {
            // This function can be placed in a helper folder wa lang nako gibutang kay hehe
            if (string.IsNullOrWhiteSpace(plateNumber))
                return false;
            // Normalize input: trim and convert to upper-case for uniformity
            plateNumber = plateNumber.Trim().ToUpperInvariant();

            // Define regex patterns for each vehicle type
            string fourWheelPattern = @"^[A-Z]{3}[-\s]?\d{4}$";   // Cars, SUVs, etc.
            string motorcyclePattern = @"^[A-Z]{2}[-\s]?\d{5}$";  // Motorcycles

            // Check if input matches either pattern
            bool matchFourWheel = Regex.IsMatch(plateNumber, fourWheelPattern);
            bool matchMotorcycle = Regex.IsMatch(plateNumber, motorcyclePattern);

            return matchFourWheel || matchMotorcycle;
        }
    }

}
