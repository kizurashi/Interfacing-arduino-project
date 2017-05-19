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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
namespace ArduinoInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String con = System.Configuration.ConfigurationManager.ConnectionStrings["BO"].ConnectionString;
        MySqlConnection connection;
        MySqlDataAdapter adapter;
        MySqlCommand cmd;
        String dxx;
        private SerialPort mypoort;
        public static string defaultPlant;
        public MainWindow()
        {
            InitializeComponent();
            init();
        }
 
        public static System.Timers.Timer _timer = new System.Timers.Timer();
        public static System.Timers.Timer _timers = new System.Timers.Timer();
        public void threadStart()
        {
            try
            {
                connection.Open();
                _timer.Interval = 1000; // 4seconds
                _timer.Elapsed += MyTimer_Tick;
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
        DataSet dt = new DataSet();
        
        
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            try 
            {
                if (mypoort.IsOpen)
                {
                    if (mypoort.BytesToRead > 0)
                    {
                        dxx = mypoort.ReadExisting();
                        connection.Open();
                        cmd = connection.CreateCommand();
                        string[] hum = dxx.Trim().Split(',');
                        hum[1] += "%";
                        hum[0] += " C";
                        cmd.CommandText = "INSERT INTO LOG(PlantID,Date,Time,hum,temp) VALUES((SELECT PlantID FROM Plant WHERE PlantName = '" + defaultPlant + "'),'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','"+hum[1]+"','"+hum[0]+"')";
                        cmd.ExecuteReader();
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            fillGrid();
                        }));
                    }
                }
                else
                {
                    mypoort.Open();
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
        private void fillGrid() 
        {
            dt.Clear();
            TimeSpan x = DateTime.Now.TimeOfDay;
            string query = "SELECT DATE_FORMAT(Date,'%M %d, %Y') as Dates,Time,hum,temp FROM Log WHERE Date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PlantID = (SELECT PlantID FROM Plant WHERE PlantName  = '" + comboBox1.SelectedValue + "') ORDER BY Time DESC ";
           
            //prepare adapter to run query
            adapter = new MySqlDataAdapter(query, connection);
             
            
            //get query results in dataset
            adapter.Fill(dt, "LoadDataBinding");
            dataGrid1.DataContext = dt;

        }
        private void init()     
        {

            try
            {
                mypoort = new SerialPort();
                mypoort.BaudRate = 9600;
                mypoort.PortName = "COM5";
                mypoort.Open();
            }
            catch (Exception) {
                MessageBox.Show("ERROR");
            }
            //button1.IsEnabled = true;
           // button2.IsEnabled = false;

        }
        public void threadStarts()
        {
            try
            {
                
                _timers.Interval = 1000; // 3seconds
                _timers.Elapsed += MyTimer_Ticks;

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
        //private SynchronizationContext _uiContext = SynchronizationContext.Current;
        private void MyTimer_Ticks(object sender, EventArgs e)
        {
            try
            {

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    comboBox1.Items.Clear();
                    Combo();
                    _timers.Stop();
                }));


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
            threadStart();
            threadStarts();
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
            finally {
                
                connection.Close();
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            var LoadMeUP = new Window2();
            LoadMeUP.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var LoadMeUP = new Window1();
            LoadMeUP.Show();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillGrid();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    cmd = connection.CreateCommand();
                    string query = "SELECT DATE_FORMAT(Date,'%M %d, %Y') as Dates,Time,hum,temp FROM log WHERE Date = '" + txtSearch.Text + "' AND PlantID = (SELECT PlantID FROM Plant WHERE PlantName = '" + comboBox1.Text + "') ORDER BY Time DESC";

                    cmd.CommandText = query;
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dt.Clear();
                        reader.Close();
                        //prepare adapter to run query
                        adapter = new MySqlDataAdapter(query, connection);

                        //get query results in dataset
                        adapter.Fill(dt, "LoadDataBinding");
                        dataGrid1.DataContext = dt;

                        txtSearch.Text = String.Empty;
                    }
                    else
                    {
                        MessageBox.Show("No result found.");
                        txtSearch.Text = String.Empty;
                    }
                }
                else {
                    connection.Close();
                }
                
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }finally
            {
                connection.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
