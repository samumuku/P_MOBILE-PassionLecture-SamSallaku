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

        private string _filePath;
        public EpubReaderViewModel(Book book, string filePath)
        {
            this.book = book;
            Title = book.titre;

            _filePath = filePath;
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
                // Create or clean temp directory
                string tempDir = Path.Combine(FileSystem.AppDataDirectory, "temp");

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);

                Directory.CreateDirectory(tempDir);

                // Extract EPUB ZIP to temp folder
                using (FileStream zip = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    using (ZipArchive archive = new ZipArchive(zip))
                    {
                        archive.ExtractToDirectory(tempDir);
                    }
                }

                // Parse the EPUB TOC (assuming EPUB2 .ncx format)
                string tocPath = Path.Combine(tempDir, "OEBPS", "toc.ncx");
                XmlDocument doc = new XmlDocument();
                doc.Load(tocPath);

                Dictionary<int, string> pages = new();

                XmlElement root = doc.DocumentElement;

                foreach (XmlElement element in root.ChildNodes)
                {
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
                                    pages[playOrder] = NormalizePath(path);
                                }
                            }
                        }
                    }
                }

                if (pages.TryGetValue(currentPage, out var pagePath))
                {
                    string fullPagePath = Path.Combine(tempDir, "OEBPS", pagePath);
                    PageText = await File.ReadAllTextAsync(fullPagePath);
                }
                else
                {
                    PageText = "Page not found.";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("😭 " + ex.Message + " 😿 " + ex.StackTrace);
                PageText = "Error loading EPUB.";
            }
        }

    }
}
