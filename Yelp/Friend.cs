using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yelp
{
    /// <summary>
    /// −user’s friends (name and star rating of each friend and the date he/she yelps since)
    /// </summary>
    public class Friend
    {
        public string name { get; set; }
        public double avgStars { get; set; }
        public string yelpingSince { get; set; }





        public void AddFriendListRows(DataGrid dataGrid)
        {

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "Name";
            col1.Width = 60;
            dataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("totalLikes");
            col2.Header = "Total Likes";
            col2.Width = 50;
            dataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("avgStars");
            col3.Header = "Avg. Stars";
            col3.Width = 50;
            dataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("yelpingSince");
            col4.Header = "Yelping Since";
            col4.Width = 80;
            dataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("userID");
            col5.Header = "";
            col5.Width = 0;
            dataGrid.Columns.Add(col5);

        }
    }
}
