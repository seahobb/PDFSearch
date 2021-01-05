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
using System.Diagnostics;

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

        
        private Dictionary<string, string> SearchContent = new Dictionary<string, string>();

        private void UploadClickHandler(object sender, RoutedEventArgs e)
        {
            SearchContent.Clear();
            uploadListBox.Items.Clear();
            //DataContext = new Search();

            
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
                        StringBuilder combinedText = new StringBuilder();
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            ITextExtractionStrategy s = new SimpleTextExtractionStrategy();
                            string currentText = PdfTextExtractor.GetTextFromPage(reader, page, s);

                            currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                            
                            //this is in a loop and wheeler_ethan.pdf has 2 pages....
                            combinedText.Append(currentText);
                        }
                        SearchContent.Add(file, combinedText.ToString().ToLower());
                        uploadListBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file)); //file.ToString()
                        reader.Close();
                    }

                }

                
               // fileNameLabel.Content = openFileDialog.FileName;//not really accurate if more than 1 pdf file but ok for proving it loaded
                openFileDialog.Dispose();
            }

           

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
            resultsListBox.Items.Clear();

            string searchTerms = SearchTermsBox.Text;
            

            var termsArray = searchTerms.Split(' ');
           

            List<string> displayPdfs = new List<string>();
            foreach (KeyValuePair<string, string> kvp in SearchContent)
            {
                bool add = false;
                foreach (var word in termsArray)
                {
                    
                    if (kvp.Value.Contains(word.ToLower()))
                    {
                        add = true;
                        break;
                    }
                    
                }
                if (add)
                    displayPdfs.Add(kvp.Key);
            }
            foreach (var v in displayPdfs)
            {
                var newButton = new System.Windows.Controls.Button();
                newButton.Content = System.IO.Path.GetFileNameWithoutExtension(v);
                newButton.Tag = v;
                newButton.IsDefault = true;
                newButton.MouseDoubleClick += NewButton_DoubleClick;
                
                resultsListBox.Items.Add(newButton);
            }
                
                
                

        }

        private void NewButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            var buttonObj = sender as System.Windows.Controls.Button;
            string filePath = buttonObj.Tag.ToString();
            
            //https://www.codeproject.com/Questions/852563/How-to-open-file-explorer-at-given-location-in-csh
             try
             {
                 ProcessStartInfo startInfo = new ProcessStartInfo
                 {
                     Arguments = filePath,
                     FileName = "explorer.exe"
                 };

                 Process.Start(startInfo);
             }
             catch (Exception ex)
             {
                 System.Windows.MessageBox.Show(ex.Message + ": " + filePath);
             }


        }



        //if datacontext is search s then display results of s since search class is a collection
        //do above in results component somehow so change datacontext? idk
    }
}
