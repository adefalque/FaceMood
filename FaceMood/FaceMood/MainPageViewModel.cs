using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FaceMood.CognitiveEmotion;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using Xamarin.Forms;

namespace FaceMood
{
    public class MainPageViewModel : ViewModelBase
    {
        private static string latestPhotoPath = "";
        private CognitiveEmotionResult[] latestResult = null;

        private RelayCommand takePhotoCommand;
        private RelayCommand pickPhotoCommand;
        private bool isRunning;
        private ImageSource imageSource;
        private FormattedString result;

        public MainPageViewModel()
        {
            this.ImageSource = "smiley_big.png";
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set { Set(ref isRunning, value); }
        }

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set { Set(ref imageSource, value); }
        }

        public RelayCommand TakePhotoCommand
        {
            get
            {
                return takePhotoCommand ?? new RelayCommand(async () =>
                {
                    await TakeOrPickPhotoThenAnalyze(true);
                });
            }
        }

        public RelayCommand PickPhotoCommand
        {
            get
            {
                return pickPhotoCommand ?? new RelayCommand(async () =>
                {
                    await TakeOrPickPhotoThenAnalyze(false);
                });
            }
        }

        public FormattedString Result
        {
            get { return result; }
            set { Set(ref result, value); }
        }

        private async Task TakeOrPickPhotoThenAnalyze(bool isTakePhoto)
        {
            try
            {
                this.IsRunning = true;
                latestResult = null;

                var imageBytes = await TakeOrPickPhoto(isTakePhoto);

                if (imageBytes != null)
                {
                    await Task.Delay(1000);

                    await AnalyzePhotoMood(imageBytes);
                }

                this.IsRunning = false;
            }
            catch (Exception exception)
            {
                 await UserDialogs.Instance.AlertAsync(":( Sorry some error occured", "Error", "OK");
                return;
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private async Task AnalyzePhotoMood(byte[] imageBytes)
        {
            var service = new CognitiveEmotionService();
            latestResult = await service.GetImageEmotions(imageBytes);

            var bitmap = SKBitmap.Decode(latestPhotoPath);
            var canvas = new SKCanvas(bitmap);

            var sbResult = new StringBuilder();
            for (var index = 0; index < latestResult.Length; index++)
            {
                var result = latestResult[index];
                var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    StrokeWidth = 5
                };
                var rect = result.FaceRectangle;

                canvas.DrawRect(SKRect.Create(rect.Left, rect.Top, rect.Width, rect.Height), paint);

                

                sbResult.Append(
                    $@"Face {index}\r\n
Happiness: {result.Scores.Happiness:P}
Neutral: {result.Scores.Neutral:P}
Contempt: {result.Scores.Contempt:P}
Fear: {result.Scores.Fear:P}
Disgust: {result.Scores.Disgust:P}
Sadness: {result.Scores.Sadness:P}
Anger: {result.Scores.Anger:P}
");
            }

            this.Result = sbResult.ToString();

            SKData d = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
            this.ImageSource = Xamarin.Forms.ImageSource.FromStream(() =>
            {
                return d.AsStream(true);
            });
            
        }

        private async Task<byte[]> TakeOrPickPhoto(bool isTakePhoto)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await UserDialogs.Instance.AlertAsync("No Camera", ":( No camera available.", "OK");
                return null;
            }

            MediaFile mediaFile = null;

            if (isTakePhoto)
            {
                mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Photos",
                    Name = "facemood.jpg",
                    PhotoSize = PhotoSize.Medium,
                    //CustomPhotoSize = 80
                    CompressionQuality = 50
                });
            }
            else
            {
                mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    CompressionQuality = 50
                });
            }

            if (mediaFile == null)
                return null;

            latestPhotoPath = mediaFile.Path;

            byte[] imageBytes = null;
            var imageStream = mediaFile.GetStream();
            imageBytes = new byte[imageStream.Length];
            imageStream.Read(imageBytes, 0, (int)imageStream.Length);

            this.ImageSource = ImageSource.FromStream(() =>
            {
                mediaFile.Dispose();

                return imageStream;
            });

            return imageBytes;
        }
    }
}
