using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace InsertData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBox> textBoxes = new List<TextBox>();
        List<Grid> grids = new List<Grid>();
        List<bool> isManuallyTyped = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            textBoxes.Add(tbId);
            textBoxes.Add(tbName);
            textBoxes.Add(tbSurname);
            textBoxes.Add(tbEmail);
            textBoxes.Add(tbPhone);

            grids.Add(gridId);
            grids.Add(gridName);
            grids.Add(gridSurname);
            grids.Add(gridEmail);
            grids.Add(gridPhone);

            isManuallyTyped.Add(false);
            isManuallyTyped.Add(false);
            isManuallyTyped.Add(false);
            isManuallyTyped.Add(false);
            isManuallyTyped.Add(false);

            ShowTable();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int ind = textBoxes.IndexOf(tb);
            tb.FontSize = 20;

            if (tb.Text == (string)tb.Tag ||
                tb.Text[0] == '*' ||
                tb.Name == "tbPhone" && tb.Text == "None")
            {
                tb.Text = "";

                tb.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
                isManuallyTyped[ind] = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            byte ind = (byte)textBoxes.IndexOf(tb);

            if (tb.Text == "")
            {
                isManuallyTyped[ind] = false;
                tb.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7b7780"));
                tb.Text = (string)tb.Tag;
            }

            grids[ind].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00a2ff"));
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            bool IsInputCorrect = !CheckInputData();

            if (!IsInputCorrect)
                return;

            try
            {
                InsertData();
            }
            catch (System.Exception)
            {
                tbId.FontSize = 12;
                tbId.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7b7780"));
                gridId.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#c70000"));

                if (!int.TryParse(tbId.Text, out int a))
                    tbId.Text = $"*{tbId.Text} isn't a number";
                else
                    tbId.Text = $"*id {tbId.Text} is used";
                return;
            }

            ShowTable();

            tbPhone.Tag = "(818)244-2468";
            for (int i = 0; i < 5; i++)
            {
                textBoxes[i].Text = (string)textBoxes[i].Tag;
                textBoxes[i].Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7b7780"));
                grids[i].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00a2ff"));
                isManuallyTyped[i] = false;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TruncateData();
            ShowTable();

            tbPhone.Tag = "(818)244-2468";
            for (int i = 0; i < 5; i++)
            {
                textBoxes[i].Text = (string)textBoxes[i].Tag;
                textBoxes[i].Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7b7780"));
                grids[i].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00a2ff"));
                isManuallyTyped[i] = false;
            }
        }


        private bool CheckInputData()
        {
            bool isSomethingWrong = false;

            for (int i = 0; i < 4; i++)
            {
                if (textBoxes[i].Text == (string)textBoxes[i].Tag ||
                    textBoxes[i].Text == "" ||
                    textBoxes[i].Text[0] == '*')
                {
                    bool t = true;
                    if (textBoxes[i].Text == (string)textBoxes[i].Tag)
                    {
                        if (isManuallyTyped[i])
                            t = false;
                    }

                    if (t)
                    {
                        isSomethingWrong = true;
                        grids[i].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#c70000"));
                        textBoxes[i].Text = "*required to fill";
                        textBoxes[i].FontSize = 12;
                        textBoxes[i].Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7b7780"));
                    }
                }
                else
                {
                    grids[i].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00c224"));
                }
            }
            //Working on phone field
            gridPhone.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00c224"));
            if (string.IsNullOrWhiteSpace(tbPhone.Text) || tbPhone.Text == (string)tbPhone.Tag)
            {
                if (tbPhone.Text == (string)tbPhone.Tag)
                {
                    if (!isManuallyTyped[4])
                    {
                        tbPhone.Text = "None";
                        tbPhone.Tag = "None";
                    }
                }
                else
                {
                    tbPhone.Text = "None";
                    tbPhone.Tag = "None";
                }
                
            }

            return isSomethingWrong;
        }
        private void InsertData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbStudents"].ConnectionString;
            string insertQuery = "INSERT INTO Student(Id, Name, Surname, Email, Phone) VALUES(" +
                $"'{tbId.Text}','{tbName.Text}','{tbSurname.Text}','{tbEmail.Text}','{tbPhone.Text}')";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand command = new SqlCommand(insertQuery, con);
                command.ExecuteNonQuery();
            }
        }

        private void TruncateData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbStudents"].ConnectionString;
            string query = "TRUNCATE TABLE Student;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
            }
        }

        private void ShowTable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbStudents"].ConnectionString;

            string selectQuery = "SELECT * FROM Student;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connectionString);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "Student");

                DataTable dt = ds.Tables["Student"];
                dgTable.ItemsSource = dt.DefaultView;
            }
        }
    }
}