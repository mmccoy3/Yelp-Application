using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Yelp
{
    public class Query
    {
        public string select { get; set; }
        public string from { get; set; }
        public string where { get; set; }
       
        List<object> obj = new List<object>();

        private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = CMdb; password = Love4doctor";
        }


        public void ExeQuery(string sqlstr, string typeOfInfo)
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
                
                        obj.Clear();
                        while (reader.Read())
                           // OOConvert(reader);
                            convertQuery(reader, typeOfInfo);
                                             
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


        public void updateDistanceQuery(double lat, double longitude)
        {
            string updateDistance = "update businesstable set distance = ( Select myDistance(B.latitude, B.longitude," + lat + "," + longitude + ") FROM businesstable as B where businesstable.businessid = B.businessid )";
            ExeQuery(updateDistance, "none");
        }


        public void startingQuery()
        {
            string sql = "SELECT distinct state FROM businesstable ORDER BY state";
            ExeQuery(sql, "id");
        }


        public void UpdateCustomerTable(string id, double lat, double longitude)
        {
            string sqlLat = "update customer set latitude =" + lat + "where userid = '" + id + "'";
            ExeQuery(sqlLat, "none");
            string sqlLong = "update customer set longitude =" + longitude + "where userid = '" + id + "'";
            ExeQuery(sqlLong, "none");
        }


        public void createQuery(string Select, string From, string Where, string TypeOfInfo)
        {
            List<object> obj = new List<object>();
            string sqlStrUser = "SELECT " + Select + " FROM " + From + " WHERE " + Where;
            ExeQuery(sqlStrUser, TypeOfInfo);

        }


        public List<object> GetObjects()
        {

            return obj;
        }
        

        public void OOConvert(NpgsqlDataReader R)
        {
            int length = R.FieldCount;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    obj.Add(R.GetInt64(i));
                }
                catch
                {
                    try
                    {
                        obj.Add(R.GetString(i));
                    }
                    catch
                    {
                        try
                        {
                            obj.Add(R.GetDouble(i));
                        }
                        catch
                        {
                            obj.Add(R.GetTimeStamp(i));
                        }
                        finally
                        {
                            obj.Add(0);
                        }
                    }
                }
            }
        }


        public void convertQuery(NpgsqlDataReader R, string type)
        {

            if(type == "zipcode")
            {
                obj.Add(R.GetInt64(0));
            }

                if (type == "id" || type == "category" || type == "attribute")
                {

                    obj.Add(R.GetString(0));
                    try
                    {
                        obj.Add(R.GetDouble(1));
                        obj.Add(R.GetTimeStamp(2));
                    }
                    catch
                    {
                        return;
                    }
                }
                if(type == "customer")
                {

                    obj.Add(R.GetString(0));
                    obj.Add(R.GetString(1));
                    obj.Add(R.GetTimeStamp(2));
                    try
                    {
                        obj.Add(R.GetDouble(3));
                    }
                    catch
                    {
                        obj.Add(0);
                    }
                    try
                    {
                        obj.Add(R.GetInt64(4));
                    }
                    catch
                    {
                        obj.Add(R.GetInt64(4));
                    }

                        obj.Add(R.GetDouble(5));
                        obj.Add(R.GetInt64(6));
                        obj.Add(R.GetInt64(7));
                    obj.Add(R.GetInt64(8));
                    try
                        {
                            obj.Add(R.GetInt64(9));
                        }
                        catch
                        {
                            obj.Add(0);
                        }

                        obj.Add(R.GetDouble(10));
                        obj.Add(R.GetDouble(11));               
                }
                if (type == "friends")
                {
                    obj.Add(R.GetString(0));
                    obj.Add(R.GetString(1));
                    obj.Add(R.GetString(2));
                    obj.Add(R.GetString(3));
                    obj.Add(R.GetTimeStamp(4));
                }
                if (type == "none")
                {
                    return;
                }

                if(type == "business")
            {
                obj.Add(R.GetString(0));
                obj.Add(R.GetString(1));
                obj.Add(R.GetString(2));
                obj.Add(R.GetString(3));
                obj.Add(R.GetInt64(4));
                obj.Add(R.GetInt64(5));
                obj.Add(R.GetInt64(6));
                obj.Add(R.GetDouble(8));
            }

                if(type == "hours")
            {
                obj.Add(R.GetString(0));
                obj.Add(R.GetString(1));
                obj.Add(R.GetString(2));
                obj.Add(R.GetString(3));
                obj.Add(R.GetString(4));
                obj.Add(R.GetString(5));
            }
        }
    }
}
