﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Shapes;
using WpfAnimatedGif;
using Microsoft.VisualBasic;

namespace VerusSententiaeFull
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _basePath = "VerusSententiaeFull";
        private List<string> _videoFiles;
        private int _currentVideoIndex;
        private int _currentAudioIndex = 0;
        private int _currentTrueAudioIndex = 0;
        private int _demoCounter = 1;
        private int _trialCounter = 1;
        private string _isTrial = "False";
        public List<string> Audio_File_Order { get; private set; }
        private List<string> trialAudioFiles = new List<string>();
        private List<string> AudioFiles = new List<string>();
        private string isTrueTrial = "False";
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer ratingTimer;
        private object timerLock = new object();
        private DispatcherTimer splashTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            LoadValenceImage();
            LoadArousalImage();

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"logo.gif", UriKind.RelativeOrAbsolute);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(img, image);  // Assuming SplashImage is the name of your Image control in XAML
            LoadSplashScreen();
            splashTimer.Interval = TimeSpan.FromSeconds(5);
            splashTimer.Tick += SplashTimer_Tick;
            splashTimer.Start();
            this.Focusable = true;
            this.Focus();

            // Initialize the timers with 1-second intervals
            ratingTimer = new DispatcherTimer();
            ratingTimer.Tick += Timer_Tick;
            ratingTimer.Interval = TimeSpan.FromSeconds(1);

            // Design related code
            triangleCanvas.SizeChanged += Canvas_SizeChanged;

        }
        private void LoadSplashScreen()
        {
            // Load your GIF into SplashGif Image control
            // Example: SplashGif.Source = new BitmapImage(new Uri("path_to_your_gif"));
        }

        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            splashTimer.Stop();
            // Hide SplashScreen and show MainMenu
            SplashScreen.Visibility = Visibility.Collapsed;
            TitleScreen.Focus();
            TitleScreen.Visibility = Visibility.Visible;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (TitleScreen.Visibility == Visibility.Visible)
            {
                TitleScreen.Visibility = Visibility.Collapsed;

            }
            if (TitleScreen.Visibility == Visibility.Collapsed)
            {
                Introducer.Visibility = Visibility.Visible;
                ShowCode();
                Introducer.Focus();
            }

        }

        private void MainWindow_PostNum_KeyDown(object sender, RoutedEventArgs e)
        {
            if (Introducer.Visibility == Visibility.Visible)
            {
                Introducer.Visibility = Visibility.Collapsed;
                PreVideoPlayerGrid.Visibility = Visibility.Visible;
                PreVideoPlayerGrid.Focus();
            }
        }

        private void MainWindow_PlayIntroductionVideo(object sender, KeyEventArgs e)
        {
            PreVideoPlayerGrid.Visibility = Visibility.Collapsed;
            VideoPlayerGrid.Visibility = Visibility.Visible; 
            VideoPlayer.Focus();
            PlayIntroductionVideo();

        }

        private void MainWindow_DemoIntroducer(object sender, KeyEventArgs e)
        {
            if (DemoIntroducer.Visibility == Visibility.Visible)
            {
                DemoIntroducer.Visibility = Visibility.Collapsed;
                DemoTrailIntroductionStepTwo.Visibility = Visibility.Visible;
                DemoTrailIntroductionStepTwo.Focus();
            }
        }

        private string currentAudioFile = "";
        private string _currentSlideValue = "";

        private void MainWindow_DemoIntroducerPartTwo(object sender, KeyEventArgs e)
        {

            DemoTrailIntroductionStepTwo.Visibility = Visibility.Collapsed;
            ratingTimer.Stop();
            if (_isTrial == "True")
            {
                Demo_Trails_Title.Text = " Audio " + _trialCounter;
            }
            else
            {
                Demo_Trails_Title.Text = " Demo Audio " + _demoCounter;
            }
            LoadTrialAudioFiles();
            _currentTrueAudioIndex = 0;
            if (trialAudioFiles.Any())
            {
                string firstAudioFile = trialAudioFiles.First();
                currentAudioFile = firstAudioFile;
                PlayAudio(firstAudioFile);
            }            
            TrialAudio.Visibility = Visibility.Visible;
            TrialAudio.Focus();
        }

        private string _currentValenceValue;

        private void MainWindow_ValenceRating(object sender, KeyEventArgs e)
        {
            StartTimerWithWarning();
            base.OnKeyDown(e);
            if (ValenceRatingGrid.Visibility == Visibility.Visible)
            {
                if (e.Key >= Key.D1 && e.Key <= Key.D9)
                {
                    if (_isTrial == "True")
                    {
                        Demo_Trails_Title.Text = " Audio " + _trialCounter;
                        versionNumArousal.Text = _trialCounter + ".2";
                        _currentValenceValue = e.Key.ToString().Substring(1);
                        _currentSlideValue = versionNumArousal.Text;
                    }
                    else
                    {
                        Demo_Trails_Title.Text = " Demo Audio " + _demoCounter;
                        versionNumArousal.Text = "Demo " + _demoCounter + ".2";
                        _currentValenceValue = e.Key.ToString().Substring(1);
                        _currentSlideValue = versionNumArousal.Text;

                    }
                    ValenceRatingGrid.Visibility = Visibility.Collapsed;
                    ArousalRatingGrid.Visibility = Visibility.Visible;
                    LoadArousalImage();
                    ArousalRatingGrid.Focus();
                }
            }
        }

        private string _currentArousalValue;

        private void MainWindow_ArousalRating(object sender, KeyEventArgs e)
        {
            StartTimerWithWarning();
            base.OnKeyDown(e);
            SignificanceSlider.Value = 0;
            if (ArousalRatingGrid.Visibility == Visibility.Visible)
            {
                if (e.Key >= Key.D1 && e.Key <= Key.D9)
                {
                    if (_isTrial == "True")
                    {
                        Demo_Trails_Title.Text = " Audio " + _trialCounter;
                        versionNumSignificance.Text = _trialCounter + ".3";
                        _currentArousalValue = e.Key.ToString().Substring(1);
                        _currentSlideValue = versionNumSignificance.Text;

                    }
                    else
                    {
                        Demo_Trails_Title.Text = " Demo Audio " + _demoCounter;
                        versionNumSignificance.Text = "Demo " + _demoCounter + ".3";
                        _currentArousalValue = e.Key.ToString().Substring(1);
                        _currentSlideValue = versionNumSignificance.Text;

                    }
                    ArousalRatingGrid.Visibility = Visibility.Collapsed;
                    SignificanceRatingGrid.Visibility = Visibility.Visible;
                    SignificanceRatingGrid.Focus();
                }
            }
        }

        private string _currentSignificanceValue;

        private void MainWindow_SignificanceRating(object sender, KeyEventArgs e)
        {
            if (SignificanceRatingGrid.Visibility == Visibility.Visible)
            {
                _currentSignificanceValue = SignificanceSlider.Value.ToString("F2");
                string audioFileNameForOutput = currentAudioFile; // Use the current audio file name
                OutputController(audioFileNameForOutput);
                if (isTrueTrial == "True")
                {
                    // Handle the case for true trial audio files
                    if (_currentTrueAudioIndex < AudioFiles.Count - 1)
                    {
                        _currentTrueAudioIndex++;
                        string nextTrueAudioFile = AudioFiles[_currentTrueAudioIndex];
                        currentAudioFile = nextTrueAudioFile;
                        PlayAudio(nextTrueAudioFile);
                        TrialAudio.Visibility = Visibility.Visible;
                        TrialAudio.Focus();
                    }
                    else
                    {
                        TrueTrialEnder.Visibility = Visibility.Visible;
                        TrueTrialEnder.Focus();
                    }
                }
                else
                {
                    // Handle the case for regular trial audio files
                    if (_currentAudioIndex < trialAudioFiles.Count - 1)
                    {
                        _currentAudioIndex++;
                        string nextAudioFile = trialAudioFiles[_currentAudioIndex];
                        currentAudioFile = nextAudioFile;
                        PlayAudio(nextAudioFile);
                        TrialAudio.Visibility = Visibility.Visible;
                        TrialAudio.Focus();
                    }
                    else
                    {
                        // Proceed to the next part of your application
                        TrueTrialIntroduction.Visibility = Visibility.Visible;
                        TrueTrialIntroduction.Focus();
                    }
                }
                if (_isTrial == "True")
                {
                    _trialCounter++;
                    Demo_Trails_Title.Text = " Audio " + _trialCounter;
                }
                else
                {
                    _demoCounter++;
                    Demo_Trails_Title.Text = " Demo Audio " + _demoCounter;

                }
                ratingTimer.Stop();
                SignificanceRatingGrid.Visibility = Visibility.Collapsed;
            }
        }

        private DateTime _startTime;

        private void StartTimerWithWarning()
        {
            _startTime = DateTime.Now;
            ratingTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;

            if (elapsed.TotalSeconds >= 4 && elapsed.TotalSeconds < 5)
            {
                ShowAutoClosingMessageBox("Make rating now", 1);
            }
            else if (elapsed.TotalSeconds >= 6)
            {
                ratingTimer.Stop();
            }

        }


        private void ShowAutoClosingMessageBox(string message, int seconds)
        {
            if (ratingTimer.IsEnabled)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new AutoClosingMessageBox(message, seconds).Show();
                });

            }
        }


            private void MainWindow_TrueTrialIntroduction(object sender, KeyEventArgs e)
        {
            if (TrueTrialIntroduction.Visibility == Visibility.Visible)
            {
                _isTrial = "True";
                TrueTrialIntroduction.Visibility = Visibility.Collapsed;
                ratingTimer.Stop();
                TrueTrialIntroductionStepTwo.Visibility = Visibility.Visible;
                isTrueTrial = "True";             
                TrueTrialIntroductionStepTwo.Focus();
            }
        }

        private void MainWindow_TrueTrialIntroductionStepTwo(object sender, KeyEventArgs e)
        {
            if (TrueTrialIntroductionStepTwo.Visibility == Visibility.Visible)
            {
                TrueTrialIntroductionStepTwo.Visibility = Visibility.Collapsed;
                ratingTimer.Stop();
                if (_isTrial == "True")
                {
                    Demo_Trails_Title.Text = " Audio " + _trialCounter;
                }
                else
                {
                    Demo_Trails_Title.Text = " Demo Audio " + _demoCounter;
                }
                TrialAudio.Visibility = Visibility.Visible;
                LoadAudioFiles();
                _currentAudioIndex = 0;
                if (AudioFiles.Any())
                {
                    string firstAudioFile = AudioFiles.First();
                    currentAudioFile = firstAudioFile;
                    PlayAudio(firstAudioFile);
                }
                TrialAudio.Focus();
            }

        }

        // VSFMS2024 stands for Verus Sententiae For Military Service 2024
        private string adminCode = "VSFMS2024";

        private void MainWindow_TrueTrialEnder(object sender, KeyEventArgs e)
        {
            if (TrueTrialEnder.Visibility == Visibility.Visible)
            {
                // Prompt for the admin password
                string input = Interaction.InputBox("Please Enter Admin Password to Restart Exam", "Admin Password Required", "", -1, -1);

                // Check if the input matches the admin code
                if (input == adminCode)
                {
                    // Correct password; proceed to restart the exam
                    TrueTrialEnder.Visibility = Visibility.Collapsed;
                    _trialCounter = 1;
                    _demoCounter = 1;
                    _currentAudioIndex = _currentTrueAudioIndex = _currentVideoIndex = 0;
                    AudioFiles.Clear();
                    trialAudioFiles.Clear();
                    _isTrial = isTrueTrial = "False";
                    currentAudioFile = "";
                    TitleScreen.Visibility = Visibility.Visible;
                    TitleScreen.Focus();
                }
                else
                {
                    // Incorrect password; maybe show an error message or simply do nothing
                    MessageBox.Show("Incorrect admin password. Exam restart denied.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard blinkAnimation = (Storyboard)FindResource("BlinkAnimation");
            blinkAnimation.Begin(sender as TextBlock);
        }

        private string participantNum = "";

        private void ShowCode()
        {
            // Generate a random 6 digit number
            var random = new Random();
            string code = random.Next(100000, 999999).ToString();
            SamCodeTextBlock.Text = code;
            participantNum = SamCodeTextBlock.Text;
        }

        private void SkipVideo(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                VideoPlayer.Stop();
                VideoPlayerGrid.Visibility = Visibility.Collapsed;
                DemoIntroducer.Visibility = Visibility.Visible;
                DemoIntroducer.Focus();
            }
        }

        private void PlayIntroductionVideo()
        {
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;
            string videoFolderPath = System.IO.Path.Combine(projectRootPath, "VerusSententiaeFull", "SAM_Resources", "IntroductionVideo");

            if (!Directory.Exists(videoFolderPath))
            {
                MessageBox.Show("Video directory not found."+ videoFolderPath);
                return;
            }

            var videoFormats = new[] { "*.mp4", "*.mov" };
            _videoFiles = videoFormats.SelectMany(format => Directory.EnumerateFiles(videoFolderPath, format)).ToList();

            if (_videoFiles.Count > 0 && _currentVideoIndex < _videoFiles.Count)
            {
                string videoPath = _videoFiles[_currentVideoIndex];
                VideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
                VideoPlayer.Play();
            }
            else
            {
                VideoPlayerGrid.Visibility = Visibility.Collapsed; // Collapse the video player if no videos are found
            }
        }

        public void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _currentVideoIndex++;
            if (_currentVideoIndex < _videoFiles.Count)
            {
                PlayIntroductionVideo(); // Play the next video
            }
            else
            {
                VideoPlayer.Stop();
                VideoPlayerGrid.Visibility = Visibility.Collapsed; // Collapse the video player when all videos have been played
                DemoIntroducer.Visibility = Visibility.Visible;
                MessageBox.Show("All videos have been played.");
                DemoIntroducer.Focus();
            }
        }

        private void LoadTrialAudioFiles()
        {
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;
            string audioFolderPath = System.IO.Path.Combine(projectRootPath, "VerusSententiaeFull", "SAM_Resources", "Audio_Files");

            if (!Directory.Exists(audioFolderPath))
            {
                MessageBox.Show("Audio directory not found.");
                return;
            }

            var audioFormats = new[] { "*.mp3", "*.wav" }; // Add more formats if needed
            var allAudioFiles = audioFormats.SelectMany(format => Directory.EnumerateFiles(audioFolderPath, format)).ToList();

            foreach (var file in allAudioFiles)
            {
                if (System.IO.Path.GetFileName(file).Contains("trial"))
                {
                    trialAudioFiles.Add(file);
                }
                else
                {
                }
            }

            // Here you can add logic to use these lists as required
        }

        private void LoadAudioFiles()
        {
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;
            string audioFolderPath = System.IO.Path.Combine(projectRootPath, "VerusSententiaeFull", "SAM_Resources", "Audio_Files");

            if (!Directory.Exists(audioFolderPath))
            {
                MessageBox.Show("Audio directory not found.");
                return;
            }

            var audioFormats = new[] { "*.mp3", "*.wav" }; // Add more formats if needed
            var allAudioFiles = audioFormats.SelectMany(format => Directory.EnumerateFiles(audioFolderPath, format)).ToList();

            foreach (var file in allAudioFiles)
            {
                if (!System.IO.Path.GetFileName(file).Contains("trial"))
                {
                    AudioFiles.Add(file);
                }
                else
                {

                }
            }

            // Here you can add logic to use these lists as required
        }

        private void PlayAudio(string audioFilePath)
        {
            mediaPlayer.Open(new Uri(audioFilePath, UriKind.RelativeOrAbsolute));
            mediaPlayer.MediaEnded += MediaPlayerAudio_MediaEnded;
            VideoPlayerGrid.Focus();
            mediaPlayer.Play();
        }

        private void MediaPlayerAudio_MediaEnded(object sender, EventArgs e)
        {
            TrialAudio.Visibility = Visibility.Collapsed;
            LoadValenceImage();
            if (_isTrial == "True")
            {
                versionNumValence.Text = _trialCounter + ".1";
                _currentSlideValue = versionNumValence.Text;
                
            }
            else
            {
                versionNumValence.Text = "Demo " + _demoCounter + ".1";
                _currentSlideValue = versionNumValence.Text;
                
            }
            ValenceRatingGrid.Visibility = Visibility.Visible;
            StartTimerWithWarning();
            ValenceRatingGrid.Focus();
        }

        public void LoadValenceImage()
        {
            try
            {                
                // Relative path does NOT include 'VerusSententiaeBasic'
                string relativePath = @"SAM_Resources\ValenceImage\CorrectedValence.png";
                // Adjust basePath to point to the 'VerusSententiaeBasic' directory
                string basePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                basePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(basePath).FullName).FullName).FullName;
                // If 'VerusSententiaeBasic' is part of basePath, append it here
                basePath = System.IO.Path.Combine(basePath, "VerusSententiaeBasic");
                // Combine the paths
                string imagePath = System.IO.Path.Combine(basePath, relativePath);
                string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (!File.Exists(imagePath))
                {
                    string fallbackPath = System.IO.Path.Combine(AppDataPath, "SAM_Resources\\ValenceImage\\CorrectedValence.png");
                    if (File.Exists(fallbackPath))
                    {
                        imagePath = fallbackPath;
                    }
                }
                //MessageBox.Show("Base Path: " + basePath + "ImagePath" + imagePath); // Use this to see what basePath is
                // Load and display the image
                BitmapImage image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                // Set image source here
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        public void LoadArousalImage()
        {
            try
            {;
                // Relative path does NOT include 'VerusSententiaeBasic'
                string relativePath = @"SAM_Resources\ArousalImage\CorrectedArousal.png";
                // Adjust basePath to point to the 'VerusSententiaeBasic' directory
                string basePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                basePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(basePath).FullName).FullName).FullName;

                // If 'VerusSententiaeBasic' is part of basePath, append it here
                basePath = System.IO.Path.Combine(basePath, "VerusSententiaeBasic");
                // Combine the paths
                string imagePath = System.IO.Path.Combine(basePath, relativePath);
                //MessageBox.Show("Base Path: " + basePath + "ImagePath" + imagePath); // Use this to see what basePath is
                // Load and display the image
                BitmapImage image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                // Set image source here
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }


        private void SignificanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double roundedValue = Math.Round(e.NewValue, 3);
            int sliderValue = (int)SignificanceSlider.Value;
            sliderSignificanceTextBlock.Text = SignificanceSlider.Value.ToString();
            _currentSignificanceValue = SignificanceSlider.Value.ToString("F2");
        }

        private void SignificanceSlider_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // Suppress the spacebar input
                e.Handled = true;
            }
        }

        private Line topEdge; // Keep a reference to the line
        //Below is Design related code
        private void UpdateTriangleGeometry(double width, double height)
        {
            // Assuming dynamicTriangle is a Path that represents the triangle
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = new Point(0, 0) };
            figure.Segments.Add(new LineSegment(new Point(0, height), true));
            figure.Segments.Add(new LineSegment(new Point(width, height), true));
            figure.IsClosed = true;
            geometry.Figures.Add(figure);
            dynamicTriangle.Data = geometry;

            // Now, create or update the line for the top edge
            if (topEdge == null)
            {
                topEdge = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Green),
                    StrokeThickness = 6 // Adjust the thickness as needed
                };
                triangleCanvas.Children.Add(topEdge);
            }

            // Update line properties
            topEdge.X1 = 0;
            topEdge.Y1 = 0;
            topEdge.X2 = width;
            topEdge.Y2 = 0;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (canvas != null)
            {
                UpdateTriangleGeometry(canvas.ActualWidth, canvas.ActualHeight);
            }
        }
        //Output Related Controllers
        private String version = "";

        private void OutputController(string audioFileNameForOutput)
        {
            string relativePath = @"SAM_Results";
            string basePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            basePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(basePath).FullName).FullName).FullName;
            basePath = System.IO.Path.Combine(basePath, "VerusSententiaeFull");
            string outputPath = System.IO.Path.Combine(basePath, relativePath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            // Justifying File Names
            string justFileName = System.IO.Path.GetFileName(audioFileNameForOutput);
            string outputFileName = "Sam_Result.txt";
            string outputFilePath = System.IO.Path.Combine(outputPath, outputFileName);

            string outputContent = $"{participantNum}: {justFileName}: {_currentValenceValue}: {_currentArousalValue}: {_currentSignificanceValue}\n";

            // Append the content to the file, creating the file if it does not exist
            File.AppendAllText(outputFilePath, outputContent);
        }

    }
}
