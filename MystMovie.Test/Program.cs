using System.Text;

internal class Program
{
	private static async Task Main(string[] args)
	{
		//WebClient web = new WebClient();
		//var picker = new Picker(web);
		//var downloadPath = "C://Repos/MystMovie/data/";
		//var torrentPath = downloadPath + "Torrents/test.torrent";
		//await picker.UploadTorrent("https://eng.utorr.cc/torrenti/c/cc/Lyod.3.2024.RUS.WEB-DLRip.1.46Gb.MegaPeer_51952.torrent", torrentPath);
		//picker.DownloadTorrent(torrentPath, downloadPath + "Movies");

		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

	}

}