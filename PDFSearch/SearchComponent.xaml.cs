using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFSearch
{
    /// <summary>
    /// Interaction logic for SearchComponent.xaml
    /// </summary>
    public partial class SearchComponent : System.Windows.Controls.UserControl
    {
        public SearchComponent()
        {
            InitializeComponent();

        }

        //private string SearchContent = "";
        //private List<string> SearchContent = new List<string>();
        private Dictionary<string, string> SearchContent = new Dictionary<string, string>();

        private void UploadClickHandler(object sender, RoutedEventArgs e)
        {
            SearchContent.Clear();
            DataContext = new Search();

            StringBuilder sb = new StringBuilder();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    foreach (String file in openFileDialog.FileNames)
                    {
                        //https://stackoverflow.com/questions/2550796/reading-pdf-content-with-itextsharp-dll-in-vb-net-or-c-sharp
                        PdfReader reader = new PdfReader(file);
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            ITextExtractionStrategy s = new SimpleTextExtractionStrategy();
                            string currentText = PdfTextExtractor.GetTextFromPage(reader, page, s);

                            currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                            // sb.Append(currentText);
                            SearchContent.Add(file, currentText);
                            
                        }
                        reader.Close();
                    }

                }
                fileNameLabel.Content = openFileDialog.FileName;//not really accurate if more than 1 pdf file but ok for proving it loaded
                openFileDialog.Dispose();
            }

           // SearchContent = sb.ToString();







            //resultsListBox.Items.Add(SearchContent);

            //below is more for text documents

            /* using (OpenFileDialog openFileDialog = new OpenFileDialog())
             {
                 openFileDialog.InitialDirectory = "c:\\";
                 openFileDialog.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                 openFileDialog.FilterIndex = 2;
                 openFileDialog.RestoreDirectory = true;
                 openFileDialog.Multiselect = true;



                 if (openFileDialog.ShowDialog() == DialogResult.OK)
                 {

                     foreach (String file in openFileDialog.FileNames)
                     {
                         openFileDialog.FileName = file;//since OpenFile() is specified by openFileDialog.FileName
                         var fileStream = openFileDialog.OpenFile();  



                         using (StreamReader reader = new StreamReader(fileStream))
                         {
                             resultsListBox.Items.Add(reader.ReadToEnd());
                             //pdfContentList.Add(reader.ReadToEnd());
                             reader.Close();
                         }

                     }

                 }
                 fileNameLabel.Content = openFileDialog.FileName;//not really accurate if more than 1 pdf file but ok for proving it loaded
                 openFileDialog.Dispose();
             }*/


            e.Handled = true;
        }

        private void SearchClickHandler(object sender, RoutedEventArgs e)
        {
            string searchTerms = SearchTermsBox.Text;
            //SearchContent from above will be searched for the searchTerms

            //algorithm is like index.cshtml.cs but try to do .Split(" ") and see if any term is in the SearchContent. if so...

            var termsArray = searchTerms.Split(' ');
            /*List<string> matches = new List<string>();
            foreach(var word in termsArray)
            {
                if (SearchContent.Contains(word))
                    matches.Add(word);
            }*/

            List<string> displayPdfs = new List<string>();
            foreach (KeyValuePair<string, string> kvp in SearchContent)
            {
                foreach(var word in termsArray)
                {
                    if (kvp.Value.Contains(word))
                    {
                        displayPdfs.Add(kvp.Key);
                    }
                }
            }
            foreach (var v in displayPdfs)
                resultsListBox.Items.Add(v);
                
           

            

        }



        //if datacontext is search s then display results of s since search class is a collection
        //do above in results component somehow so change datacontext? idk
    }
}
