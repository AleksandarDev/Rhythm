// (c) Copyright Jacob Johnston.
// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using WpfApplication1;
using WPFSoundVisualizationLib;

namespace Sample_NAudio
{
    class NAudioEngine : ISpectrumPlayer
    {
        #region Fields
        private static NAudioEngine instance;
        private readonly int fftDataSize = (int)FFTDataSize.FFT4096;
        private SampleAggregator sampleAggregator;
	    private WasapiLoopbackCapture capture;
        #endregion

        #region Singleton Pattern
        public static NAudioEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new NAudioEngine();
                return instance;
            }
        }
        #endregion

        #region Constructor
        private NAudioEngine()
        {
			sampleAggregator = new SampleAggregator(fftDataSize);
			
			var deviceEnumerator = new MMDeviceEnumerator();
			var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			this.capture = new WasapiLoopbackCapture(defaultDevice);
			capture.ShareMode = AudioClientShareMode.Shared;

			capture.DataAvailable += CaptureOnDataAvailable;

			capture.StartRecording();
        }

	    private void CaptureOnDataAvailable(object sender, WaveInEventArgs e)
	    {
			byte[] buffer = e.Buffer;
			int bytesRecorded = e.BytesRecorded;
			int bufferIncrement = this.capture.WaveFormat.BlockAlign;

			for (int index = 0; index < bytesRecorded; index += bufferIncrement)
			{
				float sample32 = BitConverter.ToSingle(buffer, index);
				sampleAggregator.Add(sample32, sample32);
			}
		}

	    #endregion

        #region ISpectrumPlayer
        public bool GetFFTData(float[] fftDataBuffer)
        {
            sampleAggregator.GetFFTResults(fftDataBuffer);
            return IsPlaying;
        }

        public int GetFFTFrequencyIndex(int frequency)
        {
            double maxFrequency = this.capture.WaveFormat.SampleRate / 2.0d;
            return (int)((frequency / maxFrequency) * (fftDataSize / 2));
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region Event Handlers
        private void inputStream_Sample(object sender, SampleEventArgs e)
        {
            sampleAggregator.Add(e.Left, e.Right);
        }
		
        #endregion

	    public bool IsPlaying => true;
    }
}
