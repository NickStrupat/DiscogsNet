using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DiscogsNet.Api;
using DiscogsNet.Model;

namespace DiscogsNet.ReleaseViewerExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string StatusNoRelease = "No release selected";
        private const string StatusInvalidId = "Invalid release: enter ID or URL from Discogs";
        private const string StatusLoading = "Loading...";
        private const string StatusError = "Error loading release";

        private Discogs3 discogs;

        public MainWindow()
        {
            InitializeComponent();

            this.discogs = new Discogs3();

            int releaseId = -1;
            if (Clipboard.ContainsText() && Clipboard.GetText().StartsWith("http://www.discogs.com/"))
            {
                releaseId = this.GetReleaseId(Clipboard.GetText());
            }
            if (releaseId != -1)
            {
                this.textReleaseId.Text = Clipboard.GetText();
                this.DownloadRelease(releaseId);
            }
            else
            {
                this.HideRelease(StatusNoRelease);
            }
        }

        private void ShowRelease(Release release)
        {
            this.textStatus.Visibility = Visibility.Hidden;
            this.releaseViewer.Visibility = Visibility.Visible;
            this.releaseViewer.DataContext = release;
        }

        private void HideRelease(string status)
        {
            this.textStatus.Visibility = Visibility.Visible;
            this.textStatus.Text = status;
            this.releaseViewer.Visibility = Visibility.Hidden;
            this.releaseViewer.DataContext = null;
        }

        private int GetReleaseId(string text)
        {
            int releaseId;

            if (int.TryParse(text, out releaseId))
            {
                return releaseId;
            }

            releaseId = DiscogsUriParser.ParseReleaseIdFromUri(text);
            if (releaseId != 0)
            {
                return releaseId;
            }

            return -1;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            int releaseId = this.GetReleaseId(textReleaseId.Text);

            if (releaseId == -1)
            {
                this.HideRelease(StatusInvalidId);
                return;
            }

            this.DownloadRelease(releaseId);
        }

        private void DownloadRelease(int releaseId)
        {
            this.HideRelease(StatusLoading);
            this.btnGo.IsEnabled = false;

            Task<Release> downloadTask = new Task<Release>(delegate()
            {
                return this.discogs.GetRelease(releaseId);
            });
            downloadTask.ContinueWith(delegate(Task<Release> prevTask)
            {
                this.Dispatcher.Invoke(new Action(delegate()
                {
                    this.btnGo.IsEnabled = true;
                    if (prevTask.IsFaulted)
                    {
                        this.HideRelease(StatusError + ": " + prevTask.Exception.Message);
                    }
                    else
                    {
                        this.ShowRelease(prevTask.Result);
                    }
                }));
            });
            downloadTask.Start();
        }
    }
}
