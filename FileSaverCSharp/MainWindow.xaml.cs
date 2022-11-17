using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
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

namespace FileSaverCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath;

        public MainWindow()
        {
            InitializeComponent();
            DataGridDocs.ItemsSource = LoadSavedDocuments();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "PDF documents (.pdf)|*.pdf";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filePath = dialog.FileName;
                FilePath.Text = filePath;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            SaveFileToDataBase(filePath);
            DataGridDocs.ItemsSource = LoadSavedDocuments();
            MessageBox.Show("File Saved");
        }

        private void SaveFileToDataBase(string filePath)
        {
            var buffer = FileUtils.ReadFileBytes(filePath);
            var fileExtension = FileUtils.GetExtension(filePath);
            var fileName = FileUtils.GetFileName(filePath);
            string insertQuery = "INSERT INTO Documents(Data, Extension, Name)VALUES(@Data, @Extension, @Name)";
            var queryParams = new List<SQLDbParameter>() {
                new SQLDbParameter
                {
                    ParameterName= "@Data",
                    SqlDbType = System.Data.SqlDbType.VarBinary,
                    Value= buffer
                },
                new SQLDbParameter
                {
                    ParameterName= "@Extension",
                    SqlDbType = System.Data.SqlDbType.Char,
                    Value= fileExtension
                },
                new SQLDbParameter
                {
                    ParameterName= "@Name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value= fileName
                }
            };

            SQLUtils.ExecuteNonQuery(insertQuery, queryParams);
        }

        private IEnumerable LoadSavedDocuments()
        {
            var selectQuery = @"SELECT Id, Data, Name, Extension FROM Documents";
            var dataTable = SQLUtils.ExecuteSelectQuery(selectQuery);
            List<object> list = new List<object>();
            for(var i = 0; i < dataTable.Rows.Count; i++)
            {
                list.Add(
                    new { 
                        Id = dataTable.Rows[i]["Id"],
                        Name = dataTable.Rows[i]["Name"],
                        Extension = dataTable.Rows[i]["Extension"],                        
                    });
            }
            return list;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRows = DataGridDocs.SelectedItems;
            foreach (dynamic item in selectedRows)
            {
                var id = (int)item.Id;
                WriteFileFromDb(id);
            }
        }

        private void WriteFileFromDb(int id)
        {
            using (var cnn = SQLUtils.GetNewConnection())
            {
                var selectQuery = "SELECT  Data, Name, Extension FROM Documents WHERE Id = @Id";
                var cmd = new SqlCommand(selectQuery, cnn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value= id;
                cnn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var extension = reader["Extension"].ToString();
                    var path = "C:\\Users\\ulise\\OneDrive\\Escritorio\\tests";
                    var fileName = reader["Name"].ToString();
                    var data = (byte[])reader["Data"];

                    FileUtils.SaveFile(path, fileName, extension, data);
                }
            }
        }
    }
}
