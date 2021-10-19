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
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization;
using Npgsql;

namespace Yelp
{
    /// <summary>
    /// Interaction logic for BarGraph.xaml
    /// </summary>
    public partial class BarGraph : Window
    {
        Business business = new Business();
        GraphContents graphContents = new GraphContents();

        public BarGraph(Business business)
        {
            this.business = business;
            InitializeComponent();
            barGraph();
        }


        public void barGraph()
        {
            List<KeyValuePair<string, int>> graphData = new List<KeyValuePair<string, int>>();
            graphData = ExecuteQuery(business.businessName);
            checkinGraph.DataContext = graphData;
        }

        /// <summary>
        /// Connect to database
        /// </summary>
        /// <returns></returns>
        private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = CMdb; password = Love4doctor";
        }


        /// <summary>
        /// Get the specified query from the database.
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="myf"></param>
        public List<KeyValuePair<string, int>> ExecuteQuery(string bname)
        {
            //string month;
            string checkinQuery = "SELECT month, count(checkin) FROM businesstable INNER JOIN checkin ON businesstable.businessid = checkin.businessid WHERE businessname = '" + bname + "' GROUP BY month";
            List<KeyValuePair<string, int>> graphData = new List<KeyValuePair<string, int>>();

            using (var connection = new NpgsqlConnection(BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = checkinQuery;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())

                            //month = graphContents.GetMonth(reader.GetInt32(0)).ToString();
                            graphData.Add(new KeyValuePair<string, int>(graphContents.GetMonth(reader.GetInt32(0)).ToString(), reader.GetInt32(1)));

                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return graphData;
        }
    }
}
