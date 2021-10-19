using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yelp
{
    public class Customer
    {
        public List<string> tempList = new List<string>();
        public Dictionary<string, string> tempDict = new Dictionary<string, string>();


        public Customer()
        {

        }

        public string userID { get; set; }
        public string name { get; set; }
        public string yelpingSince { get; set; }
        public long fans { get; set; }
        public string friends { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double avgStars { get; set; }
        public double tipCount { get; set; }
        public long funnyVotes { get; set; }
        public long usefuleVotes { get; set; }
        public long coolVotes { get; set; }
        public long totalLikes { get; set; }


        public List<Friend> FriendsList = new List<Friend>();
        public Query query = new Query();


        public void createCustomer(List<object> obj)
        {

            userID = (string)obj[0];
            name = (string)obj[1];
            yelpingSince = (string)obj[2].ToString();
            try
            {
                int tips = (int)obj[3];
                tipCount = tips;
            }
            catch
            {
                tipCount = (double)obj[3];
            }
            
            fans = (long)obj[4];
            avgStars = (double)obj[5];
            funnyVotes = (long)obj[6];
            usefuleVotes = (long)obj[7];
            coolVotes = (long)obj[8];
            try
            {
                int likes = (int)obj[9];
                totalLikes = likes;
            }
            catch
            {
                totalLikes = (long)obj[9];
            }
            
            latitude = (double)obj[10];
            longitude = (double)obj[11];
        }



        public void UpdateCoordinates(string longitude, string latitude)
        {
            double lon;
            Double.TryParse(longitude, out lon);
            this.longitude = lon;
            double lat;
            Double.TryParse(latitude, out lat);
            this.latitude = lat;
            query.UpdateCustomerTable(this.userID,this.latitude, this.longitude);
        }





    }
}
