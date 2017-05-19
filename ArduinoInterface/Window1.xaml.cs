using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace ArduinoInterface
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        String con = System.Configuration.ConfigurationManager.ConnectionStrings["BO"].ConnectionString;
        MySqlConnection connection;
        MySqlDataAdapter adapter;
        MySqlCommand cmd;
        
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connection = new MySqlConnection(con);
            Combo();

        }
        public void Combo()
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT PlantName FROM Plant";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {

                connection.Close();
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.Text != "")
            {
                MainWindow.defaultPlant = comboBox1.SelectedItem.ToString();
                this.Close();
                MessageBox.Show(comboBox1.SelectedItem.ToString() + " as the default plant");
                MainWindow._timer.Start();
                
            }
            else 
            {
                MessageBox.Show("Please choose default plant");
            }
          

        }
    }
}
