using SuRGeoNix.BitSwarmLib;
using SuRGeoNix.BitSwarmLib.BEP;
using System.Net;
using System.Net.Http.Headers;

namespace MystMovie.Picker
{
	public class Picker
	{
		private BitSwarm _bitSwarm;
		private Torrent _torrent;
		private bool _sessionFinished;
		private int _prevHeight;
		private object _lockRefresh;
		private static View _view;

		public Picker(WebClient web)
		{
			_lockRefresh = new object();
			_view = View.Stats;
		}

		public async Task DownloadTorrent(string moviePath, string path)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders
					.Accept
					.Add(new MediaTypeWithQualityHeaderValue("application/x-bittorrent"));

				var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, moviePath));
				var responseStream = await response.Content.ReadAsByteArrayAsync();

				using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
					writer.Write(responseStream);
			}
		}

		public void DownloadMovie(string filePath, string moviePath)
		{
			var bitSwarmOptions = new Options();

			bitSwarmOptions.FolderComplete = moviePath;
			bitSwarmOptions.FolderIncomplete = moviePath;

			_bitSwarm = new BitSwarm(bitSwarmOptions);

			_bitSwarm.MetadataReceived += BitSwarm_MetadataReceived;
			_bitSwarm.StatsUpdated += BitSwarm_StatsUpdated;
			_bitSwarm.StatusChanged += BitSwarm_StatusChanged;

			_bitSwarm.Open(filePath);

			if (_sessionFinished) return;
			_bitSwarm.Start();

			while (!_sessionFinished) { }

			if (_bitSwarm != null) _bitSwarm.Dispose(true);
		}

		private void PrintMenu() { Console.WriteLine($"[1: Stats] [2: Torrent] [3: Peers] [4: Peers (w/Refresh)] [5: {(_bitSwarm.isRunning ? "Pause" : "Continue")}] [Ctrl-C: Exit]".PadLeft(100, ' ')); }

		private void BitSwarm_StatusChanged(object source, BitSwarm.StatusChangedArgs e)
		{
			if (e.Status == 0 && _bitSwarm.torrent != null && _torrent.file.name != null)
			{
				Console.WriteLine($"\r\nDownload of {_torrent.file.name} success!\r\n\r\n");
				Console.WriteLine(_bitSwarm.DumpTorrent());
				Console.WriteLine($"\r\nDownload of {_torrent.file.name} success!\r\n\r\n");
			}
			else if (e.Status == 2)
				Console.WriteLine("An error has been occured :( \r\n" + e.ErrorMsg);

			_bitSwarm?.Dispose();
			_bitSwarm = null;
			_sessionFinished = true;
		}

		private void BitSwarm_MetadataReceived(object source, BitSwarm.MetadataReceivedArgs e)
		{
			lock (_lockRefresh)
			{
				_torrent = e.Torrent;
				_view = View.Torrent;
				Console.Clear();
				Console.WriteLine(_bitSwarm.DumpTorrent());
				PrintMenu();
			}

			Thread tmp = new Thread(() =>
			{
				lock (_lockRefresh)
				{
					Thread.Sleep(3000);
					Console.Clear();
					Console.WriteLine(_bitSwarm.DumpStats());
					PrintMenu();
					_view = View.Stats;
				}
			});
			tmp.IsBackground = true;
			tmp.Start();
		}

		private void BitSwarm_StatsUpdated(object source, BitSwarm.StatsUpdatedArgs e)
		{
			try
			{
				if (_view != View.Stats && _view != View.Peers) return;

				if (Console.WindowHeight != _prevHeight) { _prevHeight = Console.WindowHeight; Console.Clear(); }

				lock (_lockRefresh)
					if (_view == View.Peers)
					{
						Console.Clear();
						Console.SetCursorPosition(0, 0);
						Console.WriteLine(_bitSwarm.DumpPeers());
					}
					else if (_view == View.Stats)
					{
						Console.SetCursorPosition(0, 0);
						Console.WriteLine(_bitSwarm.DumpStats());

					}

				PrintMenu();

			}
			catch (Exception) { }
		}
	}
}
