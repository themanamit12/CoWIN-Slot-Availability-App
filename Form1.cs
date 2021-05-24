using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoWIN_Slot_Availability_App.Model;
using Newtonsoft.Json;

namespace CoWIN_Slot_Availability_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pincode = comboBox1.Text;
            string date = dateTimePicker1.Value.ToString("dd-MM-yyyy");

            var get = GetByPinResponse(pincode, date);

            dataGridView1.DataSource = ToDataTable(get.sessions);
        }

        /// <summary>
        /// GET JSON Response
        /// </summary>
        /// <param name="PinCode"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        public FindByPinResult GetByPinResponse(string PinCode, string Date)
        {
            string GetByPinURL = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByPin";
            string urlParameters = "?pincode={0}&date={1}";
            urlParameters = urlParameters.Replace("{0}", PinCode).Replace("{1}", Date);

            FindByPinResult model = new FindByPinResult();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetByPinURL + urlParameters);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    model = JsonConvert.DeserializeObject<FindByPinResult>(reader.ReadToEnd());
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // Log errorText
                }
                throw;
            }
            return model;
        }

        /// <summary>
        /// Convert Function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
