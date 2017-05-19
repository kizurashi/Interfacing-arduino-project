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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        MySqlConnection connection;
        MySqlCommand cmd;
        String con = System.Configuration.ConfigurationManager.ConnectionStrings["BO"].ConnectionString;
        
        public Window2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Plant(PlantName) VALUES('" + plantName.Text + "')";
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    this.Close();
                    MessageBox.Show("INSERTED");
                    MainWindow._timers.Start();

                }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connection = new MySqlConnection(con);
            
        }
    }
}
