using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using System.Management;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace M2MQTT1
{
    public partial class Form1 : Form
    {
        MqttClient client;
        string clientid;
        string sicaklik;
        public Form1()
        {
            InitializeComponent();

            // broker connection
            client = new MqttClient(IPAddress.Parse("yourip"));
            clientid = Guid.NewGuid().ToString();
            client.Connect(clientid);
            calistir();
            this.Close();

            // timer code
            //System.Timers.Timer x = new System.Timers.Timer(9000);
            //x.Elapsed += new System.Timers.ElapsedEventHandler(saniyelik);
            //x.Start();
        }

        
        public void calistir()
        {
            DateTime tarih = DateTime.Now;
            // cpu temp 
            ManagementObjectSearcher insManagementObjectSearcher = new System.Management.ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject insManagementObject in insManagementObjectSearcher.Get())
            {
                sicaklik = ((Convert.ToInt32(insManagementObject["CurrentTemperature"]) - 2732) / 10).ToString();
            }
            //sql connection add value
            SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString);
            string sorgu = "insert into Temperature (temperature,DateTime) values (@temperature,@DateTime)";
            SqlCommand cmd = new SqlCommand(sorgu, cnn);
            cnn.Open();
            cmd.Parameters.AddWithValue("@temperature", sicaklik);
            cmd.Parameters.AddWithValue("@DateTime", tarih.ToString());
            cmd.ExecuteNonQuery();
            cnn.Close();
            // publisher            
            client.Publish("test", Encoding.UTF8.GetBytes(tarih.ToString() + " Sicaklik Degeri: " + sicaklik));
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
       
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        


       
    }
}
