using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Sample_NAudio;

namespace WpfApplication1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HttpClient client;
		private volatile bool isSending;
		private DateTime lastMessage;

		public MainWindow()
		{
			InitializeComponent();

			this.client = new HttpClient();
			this.client.BaseAddress = new Uri("http://192.168.0.111:8080");
			SetValues(0, 0, 0);

			NAudioEngine soundEngine = NAudioEngine.Instance;
			spectrumAnalyzer.RegisterSoundPlayer(soundEngine);
			var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(150), DispatcherPriority.Normal,
				(sender, args) => this.Callback(null), this.Dispatcher);
			timer.Start();
		}

		private async Task SetValues(float r, float g, float b)
		{
			r = 255-Math.Max(0, Math.Min(r, 255));
			g = 255-Math.Max(0, Math.Min(g, 255));
			b = 255-Math.Max(0, Math.Min(b, 255));

			if (this.isSending) return;
		    try
		    {
		        var request = "/?r=" + ((int) (r)).ToString().PadLeft(3, '0') + "&g=" +
		                      ((int) (g)).ToString().PadLeft(3, '0') + "&b=" + ((int) (b)).ToString().PadLeft(3, '0');
		        if (this.isSending) return;
		        this.isSending = true;
		        this.lastMessage = DateTime.Now;
		        await client.GetAsync(new Uri(request, UriKind.Relative));
		        System.Diagnostics.Debug.WriteLine(DateTime.Now - this.lastMessage);
		    }
		    finally
		    {
		        this.isSending = false;
		    }
		}

		private void Callback(object state)
		{
			if (this.spectrumAnalyzer == null) return;
			if (this.spectrumAnalyzer.channelPeakData == null) return;

			var intensity =
				this.spectrumAnalyzer.channelPeakData[0];
			intensity /= 2f;
			intensity /= 255f;

			var r = 
				this.spectrumAnalyzer.channelPeakData[0] + 
				this.spectrumAnalyzer.channelPeakData[1] * 0.9f + 
				this.spectrumAnalyzer.channelPeakData[2] * 0.8f +
				this.spectrumAnalyzer.channelPeakData[3] * 0.7f +
				this.spectrumAnalyzer.channelPeakData[4] * 0.6f +
				this.spectrumAnalyzer.channelPeakData[5] * 0.5f +

				this.spectrumAnalyzer.channelPeakData[6] * 0.25f;

			var g =
				this.spectrumAnalyzer.channelPeakData[5] * 0.4f +

				this.spectrumAnalyzer.channelPeakData[6] * 0.8f +
				this.spectrumAnalyzer.channelPeakData[7] * 0.9f +
				this.spectrumAnalyzer.channelPeakData[8] +
				this.spectrumAnalyzer.channelPeakData[9] * 0.9f +
				this.spectrumAnalyzer.channelPeakData[10] * 0.8f +

				this.spectrumAnalyzer.channelPeakData[11] * 0.4f;

			var b =
				this.spectrumAnalyzer.channelPeakData[10] * 0.3f +

				this.spectrumAnalyzer.channelPeakData[11] * 0.6f +
				this.spectrumAnalyzer.channelPeakData[12] * 0.7f +
				this.spectrumAnalyzer.channelPeakData[13] * 0.8f +
				this.spectrumAnalyzer.channelPeakData[14] * 0.9f +
				this.spectrumAnalyzer.channelPeakData[15];
			r /= 7f;
			b /= 7f;
			g /= 6f;

			r *= intensity;
			g *= intensity;
			b *= intensity;

			this.SetValues(r, g, b);
		}
	}
}
