using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using P_PassionLecture.Models;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using P_PassionLecture.Models;

namespace P_PassionLecture.ViewModels
{
    public partial class EpubReaderViewModel : ObservableObject
    {
        HttpClient client = new();

        int currentPage = 1;

        private Book book;

        [ObservableProperty]
        private string pageText = "";

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string author = string.Empty;
        public EpubReaderViewModel(Book book)
        {
            this.book = book;
            Title = this.book.titre;
            // Author = this.book.ecrivain_prenom + " " + this.book.ecrivain_nom;
            ReadEpub();
        }

        [RelayCommand]
        private void GotoNextPage()
        {
            currentPage++;
        }

        [RelayCommand]
        private void GotoPrevPage()
        {
            currentPage--;
        }

        string NormalizePath(string path)
        {
            //Remove %20
            string normPath = HttpUtility.HtmlDecode(Regex.Unescape(path));

            normPath = normPath.Replace("%20", " ");

            //Remove toc-marker
            int index = normPath.LastIndexOf("#");
            if (index >= 0)
            {
                normPath = normPath.Remove(index);
            }

            return normPath;
        }

        private async void ReadEpub()
        {
            try
            {
                //Call API
                var response = await client.GetAsync("http://10.0.2.2:3000/api/livres/lecture/" + this.book.livre_id); //book.livre_id

                //Create temp directory
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/temp");

                //Clear temp directory
                System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/temp");
                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                if (response.IsSuccessStatusCode)
                {
                    currentPage++;
                    var content = response.Content;
                    //Open epub ZIP
                    FileStream zip = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/epub.epub", FileMode.Open);
                    ZipArchive archive = new ZipArchive(zip);
                    archive.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "/temp"); //Plus de flexibilité

                    XmlDocument doc = new XmlDocument();
                    doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/temp/OEBPS/toc.ncx");

                    Dictionary<int, string> pages = new Dictionary<int, string>();

                    XmlElement root = doc.DocumentElement;

                    foreach (XmlElement element in root.ChildNodes)
                    {
                        // Access and process each XML element here
                        if (element.Name == "navMap")
                        {
                            foreach (XmlNode nav in element.ChildNodes)
                            {
                                int playOrder = Convert.ToInt32(nav.Attributes["playOrder"].Value);
                                foreach (XmlNode node in nav.ChildNodes)
                                {
                                    if (node.Name == "content")
                                    {
                                        string path = node.Attributes["src"].Value;
                                        pages.Add(playOrder, NormalizePath(path));
                                    }
                                }
                            }
                        }
                    }

                    var contentString = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/temp/OEBPS/" + pages[currentPage]).ReadToEnd();
                    PageText = contentString;

                }
                else
                {
                    Trace.WriteLine("😀" + response.StatusCode + " - " + response.Headers + " - " + response.Content);
                    throw new Exception($"Bad status : {response.Headers},{response.Content}");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("😭 " + ex.Message + " 😿 " + ex.StackTrace);
            }
        }
    }
}
