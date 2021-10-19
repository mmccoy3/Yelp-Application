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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace Yelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Customer customer = new Customer();
        Business business = new Business();
        Friend friend = new Friend();
        Query query = new Query();
        Search search = new Search();

        public MainWindow()
        {
            InitializeComponent();
            query.startingQuery();
            List<object> obj = new List<object>();
            obj = query.GetObjects();
            for (int i = 0; i < obj.Count(); i++)
            {
                stateSearchList.Items.Add(obj[i]);
            }
            populateSort();
        }

        /// <summary>
        /// Connect to database
        /// </summary>
        /// <returns></returns>
     /*   private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone2; password = RubiksCube2021";
        }


        /// <summary>
        /// Get the specified query from the database.
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="myf"></param>
        public void ExecuteQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            List<object> obj = new List<object>();
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

        }*/





        /// <summary>
        /// Update list of cities when a state is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stateSearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            business.state = (string)stateSearchList.SelectedItem;
            search.state = (string)stateSearchList.SelectedItem;
            cityList.Items.Clear();
            zipcodeList.Items.Clear();
            businessSearchList.Items.Clear();

            if (stateSearchList.SelectedIndex > -1)
            {
                query.select = "distinct city";
                query.from = "businesstable";
                query.where = "state = '" + business.state + "' ORDER BY city";
                query.createQuery(query.select, query.from, query.where, "id");
                populateListBox(cityList);
                search.state = business.state;
            }
        }

        private void populateListBox(ListBox name)
        {
            List<object> obj = new List<object>();
            obj = query.GetObjects();
            for (int i = 0; i < obj.Count(); i++)
            {
                name.Items.Add(obj[i]);
            }
        }

        /// <summary>
        /// Update the list of zipcodes when a city is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            business.city = (string)cityList.SelectedItem;
            search.city = (string)cityList.SelectedItem;
            zipcodeList.Items.Clear();

            if (cityList.SelectedIndex > -1)
            {

                string select = "distinct zipcode";
                string from = "businesstable";
                string where = "city = '" + business.city + "' ORDER BY zipcode";
                query.createQuery(select, from, where, "zipcode");
                populateListBox(zipcodeList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zipcodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            categoryList.Items.Clear();
            selectedCategoryList.Items.Clear();
            if (zipcodeList.Items.Count == 0)
            {
                business.zipcode = 0;
            }
            else
            {
                business.zipcode = (long)zipcodeList.SelectedItem;
                search.zipcode = (long)zipcodeList.SelectedItem;
            }
               
           
            if (zipcodeList.SelectedIndex > -1)
            {
                query.select = "distinct categoryname";
                query.from ="categories";
                query.where = "businessid IN(select businessid from businesstable where state = '" + business.state + "' and city = '" + business.city + "' and zipcode = " + business.zipcode + ")";
                query.createQuery(query.select, query.from, query.where, "id");
                populateListBox(categoryList);
            }
            query.updateDistanceQuery(customer.latitude, customer.longitude);
        }

        


        /// <summary>
        /// Add category button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            search.categories.Add(categoryList.SelectedItem.ToString());
            business.categories.Add(categoryList.SelectedItem.ToString());
            selectedCategoryList.Items.Add(categoryList.SelectedItem.ToString());
        }

        /// <summary>
        /// Remove category button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeCategory_Click(object sender, RoutedEventArgs e)
        {
            business.categories.Remove(categoryList.SelectedItem.ToString());
            selectedCategoryList.Items.Remove(categoryList.SelectedItem.ToString());
        }

        /// <summary>
        /// Search button for business list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchBusinessButton_Click(object sender, RoutedEventArgs e)
        {

            clearBusinessTable();


            if (selectedCategoryList.Items.Count.Equals(0))
            {

                query.ExeQuery(search.CreateSearchQuery(), "business");
                populteBusinesses();

            }
            else
            {

                query.ExeQuery(search.CreateSearchQuery(), "business");
                populteBusinesses();

            }

            updateCategoryAttribute();
        }

        private void populteBusinesses()
        {
            List<object> obj = new List<object>();
            obj = query.GetObjects();

            for (int i = 0; i < obj.Count();)
            {
                businessSearchList.Items.Add(new Business() { businessName = (string)obj[i], address = (string)obj[i + 1], city = obj[i + 2].ToString(), state = (string)obj[i + 3], stars = (long)obj[i + 4], numCheckins = (long)obj[i + 5], numTips= (long)obj[i + 6], distance = (double)obj[i + 7] });
                i = i + 8;
            }
            business.AddBusinessColumns(businessSearchList);

        }


        /// <summary>
        /// Shows all the tips for the selected business.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tipsButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessSearchList.SelectedIndex > -1)
            {
                    TipsWindow tipsWindow = new TipsWindow(business, customer);
                    tipsWindow.Show();
            }
        }

        private void checkinsButton_Click(object sender, RoutedEventArgs e)
        {
            if (showBusinessName.Text != null)
            {
                BarGraph barGraph = new BarGraph(business);
                barGraph.Show();
            }

        }

        /// <summary>
        /// Show the chosen business info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void businessSearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showBusinessName.Clear();
            showBusinessAddress.Clear();
            showBusinessTime.Clear();


            if (businessSearchList.SelectedIndex > -1)
            {
                business = (Business)businessSearchList.Items[businessSearchList.SelectedIndex];
                DayOfWeek today = DateTime.Today.DayOfWeek;
                query.select = "distinct BusinessName, Address,dayofweek, open, close, businessid";
                query.from = "businesstable NATURAL JOIN hours";
                query.where= "businessname = '" + business.businessName + "' AND address ='" + business.address + "' AND dayofweek ='" + today.ToString() + "'";
                query.createQuery(query.select, query.from, query.where, "hours");

                business.createBusiness(query.GetObjects());
                populateBusinessInfo();

                search.updateForSpecificBusiness(business.businessID);
                Attributes.Items.Clear();
                Categoies.Items.Clear();
                updateCategoryAttribute();
            }

        }

        /// <summary>
        /// Add categories to ListBox
        /// </summary>
        /// <param name="R"></param>
        private void populateBusinessInfo()
        {
            showBusinessName.Text =business.businessName;
            showBusinessAddress.Text = business.address;
            showBusinessTime.Text = business.dayOfWeek + ": "+ business.open + " " + business.close;
        }



        private void ClearScreen()
        {
            idList.Items.Clear();
            friendsList.Columns.Clear();
            friendsList.Items.Clear();
            friendsTipList.Columns.Clear();
            friendsTipList.Items.Clear();
        }


        private void searchUserButton_Click(object sender, RoutedEventArgs e)
        {
                ClearScreen();
                List<object> obj = new List<object>();
                string SELECT = "userID";
                string FROM = "customer";
                string WHERE = "firstname = '" + SetCurrentUser.Text + "'";
                query.createQuery(SELECT, FROM, WHERE, "id");
                obj = query.GetObjects();
                for (int i =0; i < obj.Count(); i++)
                {
                    idList.Items.Add(obj[i]);
                }
                

                listPopup.Visibility = Visibility.Visible;
                listPopup.IsOpen = true;
                idList.Visibility = Visibility.Visible;
            
        }

        private void populateUserInfo()
        {
            ShowUserName.Text = customer.name;
            DisplayAccountStart.Text = customer.yelpingSince;
            DisplayStars.Text = customer.avgStars.ToString();
            DisplayTotalFans.Text = customer.fans.ToString();
            DisplayTotalFunnyVotes.Text = customer.funnyVotes.ToString();
            DisplayTotalCoolVotes.Text = customer.coolVotes.ToString();
            DisplayTotalUsefulVotes.Text = customer.usefuleVotes.ToString();
            DisplayTipCount.Text = customer.tipCount.ToString();
            DisplayTotalTipLikes.Text = customer.totalLikes.ToString();
            DisplayLatCoord.Text = customer.latitude.ToString();
            DisplayLongCoord.Text = customer.longitude.ToString();
        }
        private void idList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (idList.SelectedIndex > -1)
            {
                string SELECT = "*";
                string FROM = "customer";
                string WHERE = "userid = '" + idList.SelectedItem.ToString() + "'";
                query.createQuery(SELECT, FROM, WHERE, "customer");


                customer.createCustomer(query.GetObjects());
                populateUserInfo();
                idList.Visibility = Visibility.Collapsed;


                SELECT = "c2.firstname, c2.average_stars, c2.yelping_since";
                FROM = "customer as c1, friends, customer as c2";
                WHERE = "c1.userid = '" + customer.userID + "' and friends.userid = '" + customer.userID + "' and friends.friendid = c2.userid";
                query.createQuery(SELECT, FROM, WHERE, "id");               
                populteFriends();

                SELECT = "customer.firstname, businesstable.businessname, businesstable.city, tip.tiptext, tip.tipdate";
                FROM = "friends, tip, businesstable, customer";
                WHERE = "friends.userid = '" + customer.userID + "' and tip.userid = friends.friendid and businesstable.businessid = tip.businessid and customer.userid = friends.friendid and tip.tipdate IN (select MAX(tipdate) date from tip, customer, friends where customer.userid = '" + customer.userID + "' and friends.userid = '" + customer.userID + "' and tip.userid = friends.friendid group by friends.friendid)";
                query.createQuery(SELECT, FROM, WHERE, "friends");
                populateFriendsTip();
            }
        }

        private void populteFriends()
        {
            List<object> obj = new List<object>();
            obj = query.GetObjects();

            for (int i = 0; i < obj.Count();)
            {
                friendsList.Items.Add(new Friend() { name = (string)obj[i], avgStars = (double)obj[i+1], yelpingSince = (string)obj[i+2].ToString()});
                i = i+3;
            }
            friend.AddFriendListRows(friendsList);


        }

        private void populateFriendsTip()
        {
            FriendsTips tipList = new FriendsTips();
            List<object> obj = new List<object>();
            obj = query.GetObjects();
            for (int i = 0; i < obj.Count();)
            {
                friendsTipList.Items.Add(new FriendsTips() { name = (string)obj[i], businessName = (string)obj[i + 1], businessCity = (string)obj[i + 2], tipText = (string)obj[i + 3], tipDate = obj[i + 4].ToString() }); ;
                i = i + 5;
            }
            tipList.AddFriendsLastestTips(friendsTipList);
            
        }


        private void editLocationButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayLongCoord.IsReadOnly = false;
            DisplayLatCoord.IsReadOnly = false;
        }

        private void saveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            string longitude = DisplayLongCoord.Text;
            string latitude = DisplayLatCoord.Text;
            customer.UpdateCoordinates(longitude, latitude);
            DisplayLongCoord.IsReadOnly = true;
            DisplayLatCoord.IsReadOnly = true;
            

        }

        private void clearBusinessTable()
        {
            businessSearchList.Items.Clear();
            businessSearchList.Columns.Clear();
            Attributes.Items.Clear();
            Categoies.Items.Clear();

        }

        private void updateBusinessTable()
        {
            string sql = search.CreateSearchQuery();
            query.ExeQuery(sql, "business");
            populteBusinesses();
            updateCategoryAttribute();

          
        }


        public void updateCategoryAttribute()
        {
            string category = search.findcategory();
            if (category != "")
            {
                query.ExeQuery(category, "category");
                populateListBox(Categoies);
                string attributes = search.findAttributes();
                query.ExeQuery(attributes, "attribute");
                populateListBox(Attributes);
            }
            else
            {
                return;
            }

        }
        

        private void cb1_Click(object sender, RoutedEventArgs e)
        {
            
            clearBusinessTable();

            string attributename = "AND attributename = 'RestaurantsPriceRange2'"; string attributevalue = " AND attributevalue = '1'";
            if (cb1.IsChecked == true)
            {
                
                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }

        }

        private void cb2_Click(object sender, RoutedEventArgs e)
        {
 
            clearBusinessTable();
            string attributename = " AND attributename = 'RestaurantsPriceRange2' AND"; string attributevalue = " attributevalue = '2'";
            if (cb2.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable(); 
            }
        }

        private void cb3_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();

            string attributename = "And attributename = 'RestaurantsPriceRange2' "; string attributevalue = "AND attributevalue = '3'";
            if (cb3.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void cb4_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();

            string attributename = " and attributename = 'RestaurantsPriceRange2'"; string attributevalue = "AND attributevalue ='4'";
            if (cb4.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }
      

        private void resCheck_Click(object sender, RoutedEventArgs e)
        {
            //('RestaurantsReservations', 'True')

            clearBusinessTable();

            string attributename = " and attributename = 'RestaurantsReservations'"; string attributevalue = "AND attributevalue ='True'";
            if (resCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void handCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            //('WheelchairAccessible', 'True')
            string attributename = " and attributename = 'WheelchairAccessible'"; string attributevalue = "AND attributevalue ='True'";
            if (handCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

    

        private void seatCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            //('OutdoorSeating', 'False')
            string attributename = " and attributename = 'OutdoorSeating'"; string attributevalue = "AND attributevalue ='True'";
            if (seatCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void kidsCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            //('GoodForKids', 'True')
            string attributename = " and attributename = 'GoodForKids'"; string attributevalue = "AND attributevalue ='True'";
            if (kidsCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }




        private void delivCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'RestaurantsDelivery'"; string attributevalue = "AND attributevalue ='True'";
            if (delivCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
            //('RestaurantsDelivery', 'False')
        }

        private void takeCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            // ('RestaurantsTakeOut', 'True')
            string attributename = " and attributename = 'RestaurantsTakeOut'"; string attributevalue = "AND attributevalue ='True'";
            if (takeCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void wifiCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            // ('WiFi', 'free')
            string attributename = " and attributename = 'WiFi'"; string attributevalue = "AND attributevalue ='free'";
            if (wifiCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void breakCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            // ('dessert', 'False'), ('latenight', 'False'), ('lunch', 'True'), ('dinner', 'True'), ('brunch', 'False'), ('breakfast', 'False'),

            string attributename = " and attributename = 'breakfast'"; string attributevalue = "AND attributevalue ='True'";
            if (breakCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void brunchCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'brunch'"; string attributevalue = "AND attributevalue ='True'";
            if (brunchCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void lunchCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'lunch'"; string attributevalue = "AND attributevalue ='True'";
            if (lunchCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void dinCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'dinner'"; string attributevalue = "AND attributevalue ='True'";
            if (dinCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void desCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'dessert'"; string attributevalue = "AND attributevalue ='True'";
            if (desCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void lateCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            string attributename = " and attributename = 'latenight'"; string attributevalue = "AND attributevalue ='True'";
            if (lateCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void groupCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            // ('RestaurantsGoodForGroups', 'True')
            string attributename = " and attributename = 'RestaurantsGoodForGroups'"; string attributevalue = "AND attributevalue ='True'";
            if (groupCheck.IsChecked == true)
            {

                search.AddToAttributes(attributename, attributevalue);
                updateBusinessTable();
            }
            else
            {
                search.RemoveAttributes(attributename);
                updateBusinessTable();
            }
        }

        private void bikeCheck_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            //('BikeParking', 'True')
            string attributename = " and attributename = 'BikeParking'"; string attributevalue = "AND attributevalue ='True'";
            if (bikeCheck.IsChecked == true)
            {

                addAtribute(attributename, attributevalue);
            }
            else
            {
                removeAttribute(attributename);
            }
        }


        private void addAtribute(string attributename, string attributevalue)
        {
            search.AddToAttributes(attributename, attributevalue);
            updateBusinessTable();
        }

        private void removeAttribute(string attributename)
        {
            search.RemoveAttributes(attributename);
            updateBusinessTable();
        }

        private void creditCard_Click(object sender, RoutedEventArgs e)
        {
            clearBusinessTable();
            //('BusinessAcceptsCreditCards', 'True')
            string attributename = " and attributename = 'BusinessAcceptsCreditCards'"; string attributevalue = "AND attributevalue ='True'";
            if (bikeCheck.IsChecked == true)
            {

                addAtribute(attributename, attributevalue);
            }
            else
            {
                removeAttribute(attributename);
            }
        }


        public void populateSort()
        {
            sortResults.Items.Add("Name");
            sortResults.Items.Add("Rating");
            sortResults.Items.Add("Check-Ins");
            sortResults.Items.Add("Tips");
            sortResults.Items.Add("Nearest");
        }

        private void sortResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string orderby = (string)sortResults.SelectedItem;
            if (orderby == "Rating")
            {
                clearBusinessTable();
                search.setOrderby("order by stars DESC");
                updateBusinessTable();
            }
            if (orderby == "Tips")
            {
                clearBusinessTable();
                search.setOrderby("order by numtips DESC");
                updateBusinessTable();
            }
            if (orderby == "Check-Ins")
            {
                clearBusinessTable();
                search.setOrderby("order by numcheckin DESC");
                updateBusinessTable();
            }
            if (orderby == "Name")
            {
                clearBusinessTable();
                search.setOrderby("order by BusinessName");
                updateBusinessTable();
            }
            if (orderby == "Nearest")
            {
                clearBusinessTable();
                search.setOrderby("order by distance");
                updateBusinessTable();
            }
        }
    }
}
