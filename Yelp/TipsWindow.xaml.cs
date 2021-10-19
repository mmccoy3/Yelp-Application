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
using Npgsql;

namespace Yelp
{
    /// <summary>
    /// Interaction logic for TipsWindow.xaml
    /// </summary>
    public partial class TipsWindow : Window
    {
        Business business = new Business();
        Customer customer = new Customer();
        private string businessID = "";
        private string tempUserID = "4XChL029mKr5hydo79Ljxg";

        public TipsWindow(string businessID)
        {
            InitializeComponent();
            this.businessID = String.Copy(businessID);
            AddTipsColumns();
            AddFriendsTipsColumns();
            loadBusinessTips();
            loadFriendsTips();
        }

        public TipsWindow(Business business, Customer customer)
        {
            this.business = business;
            this.customer = customer;


            InitializeComponent();

            AddTipsColumns();
            AddFriendsTipsColumns();
            loadBusinessTips();
            loadFriendsTips();
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
        public void ExecuteQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            myf(reader);
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

        }

        /// <summary>
        /// Get the specified query from the database.
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="myf"></param>
        public void ExecuteUpdateQuery(string sqlstr)
        {
            using (var connection = new NpgsqlConnection(BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        //while (reader.Read())
                        //    myf(reader);
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

        }

        public class TipsTable
        {
            public string tipDate { get; set; }
            public string firstName { get; set; }
            public string userID { get; set; }
            public Int64 likes { get; set; }
            public string tipText { get; set; }

        }


        /// <summary>
        /// Add columns to tips
        /// </summary>
        public void AddTipsColumns()
        {

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("tipDate");
            col1.Header = "Date";
            col1.Width = 215;
            businessTipsList.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("firstName");
            col2.Header = "UserName";
            col2.Width = 100;
            businessTipsList.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("likes");
            col3.Header = "Likes";
            col3.Width = 150;
            businessTipsList.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("tipText");
            col4.Header = "Tip";
            col4.Width = 300;
            businessTipsList.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("userID");
            col5.Header = "";
            col5.Width = 0;
            businessTipsList.Columns.Add(col5);


        }

        /// <summary>
        /// Add business info to data grid.
        /// </summary>
        /// <param name="R"></param>
        private void addTipsRow(NpgsqlDataReader R)
        {
            businessTipsList.Items.Add(new TipsTable() { tipDate = R.GetTimeStamp(0).ToString(), firstName = R.GetString(1), likes = R.GetInt64(2), tipText = R.GetString(3), userID = R.GetString(4) });
        }

        private void loadBusinessTips()
        {
            string sqlStrTipsInfo = "SELECT distinct tipdate, firstname, likes, tiptext, userid FROM customer NATURAL JOIN tip WHERE businessid = '" + this.businessID + "'";
            ExecuteQuery(sqlStrTipsInfo, addTipsRow);
        }


        /// <summary>
        /// Add columns to tips
        /// </summary>
        public void AddFriendsTipsColumns()
        {

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("tipDate");
            col1.Header = "Date";
            col1.Width = 215;
            friendsTipList.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("firstName");
            col2.Header = "UserName";
            col2.Width = 100;
            friendsTipList.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("likes");
            col3.Header = "Likes";
            col3.Width = 150;
            friendsTipList.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("tipText");
            col4.Header = "Tip";
            col4.Width = 300;
            friendsTipList.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("userID");
            col5.Header = "";
            col5.Width = 0;
            friendsTipList.Columns.Add(col5);
        }

        /// <summary>
        /// Add business info to data grid.
        /// </summary>
        /// <param name="R"></param>
        private void addFriendsTipsRow(NpgsqlDataReader R)
        {
            friendsTipList.Items.Add(new TipsTable() { tipDate = R.GetTimeStamp(0).ToString(), firstName = R.GetString(1), likes = R.GetInt64(2), tipText = R.GetString(3), userID = R.GetString(4) });
        }

        private void loadFriendsTips()
        {
            string sqlStrTipsInfo = "SELECT distinct tip.tipdate, customer.firstname, tip.likes, tip.tiptext, customer.userid FROM friends, tip, customer WHERE businessid = '" + this.businessID + "' AND tip.userid = friends.friendid AND friends.friendid = customer.userid AND friends.friendid = '" + tempUserID + "'";
            ExecuteQuery(sqlStrTipsInfo, addFriendsTipsRow);
        }

        private void addTip_Click(object sender, RoutedEventArgs e)
        {
            if (enterTip.Text != null)
            {
                DateTime dateTime = DateTime.Now;
                String timeStamp = dateTime.ToString();
                int newTipCount = 0;

                string sqlStrUpdate = "INSERT INTO tip VALUES('" + this.businessID + "', '" + timeStamp + "', '" + newTipCount + "', '" + enterTip.Text + "', '" + tempUserID + "')";
                ExecuteUpdateQuery(sqlStrUpdate);
                businessTipsList.Items.Clear();
                loadBusinessTips();
            }
        }

        private void addLike_Click(object sender, RoutedEventArgs e)
        {

            if (businessTipsList.SelectedIndex >= -1)
            {
                TipsTable tipsTable = businessTipsList.Items[businessTipsList.SelectedIndex] as TipsTable;

                string sqlStrUpdate = "UPDATE tip SET likes = likes + 1 WHERE userid = '" + tipsTable.userID.ToString() + "' AND businessid = '" + this.businessID + "'";
                ExecuteUpdateQuery(sqlStrUpdate);
                businessTipsList.Items.Clear();
                loadBusinessTips();
            }
        }
    }
    }
