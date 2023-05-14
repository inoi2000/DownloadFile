using NetworlPrograming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.InteropServices;

namespace DownloadUI.ViewModel
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private static CancellationTokenSource cts = new CancellationTokenSource();

        #region Properties
        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }

        private string _uri;
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; OnPropertyChanged(); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async Task DownloadingFile()
        {
            var progress = new Progress<double>();
            progress.ProgressChanged += (sender, current) =>
            {
                Progress = current;
            };

            CancellationToken token = cts.Token;

            await FileDowndoad.DownloadingFileAsync(Uri, Path, progress, token);
        }

        [RelayCommand]
        public void CancelDownloading()
        {
            try
            {
                cts.Cancel();
            }
            catch { }
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
