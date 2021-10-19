using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
    
{   /// <summary>
    /// Search combines all of the selected attributes, categories and business location and creates a SQL statement.
    /// </summary>
    public class Search
    {
        public string state { get; set; }
        public string city { get; set; }
        public long zipcode { get; set; }

        public List<string> categories = new List<string>();
        public string businessId { get; set; }
        public List<string> businessIDs = new List<string>();

        public Dictionary<string, string> attributes = new Dictionary<string, string>();
        public Query q = new Query();

        public string orderby = " Order by BusinessName";

        public void setOrderby(string newOrder)
        {
            this.orderby = newOrder;
        }

        public void AddToAttributes(string attribute, string value)
        {
            this.attributes.Add(attribute, value);
        }


        public void RemoveAttributes(string attribute)
        {
            this.attributes.Remove(attribute);
        }

        public string CreateSearchQuery()
        {
            businessIDs.Clear();
            this.businessId = "";
            string categorySql = "";
            for (int i = 0; i < categories.Count(); i++)
            {
                string cat = " AND categoryname = '" + categories[i] + "'";
                string intersect = " (select distinct BusinessName, Address, City, State, Stars, numCheckin, numTips, businessid, distance from businesstable NATURAL JOIN categories Natural Join attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode + cat + ")";
                this.businessId = "( select distinct businessid from businesstable NATURAL JOIN categories Natural Join attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode + cat + ")";
                if (i != (categories.Count() - 1))
                {
                    this.businessId = this.businessId + "intersect";
                    intersect = intersect + "intersect";
                }
                categorySql = categorySql + intersect;


            }

            if (attributes.Count > 0 & categories.Count > 0)
            {
                this.businessId = this.businessId + "intersect";
                categorySql = categorySql + "intersect";
            }

            for (int i = 0; i < attributes.Count(); i++)
            {
                string key = attributes.ElementAt(i).Key;
                string value = attributes.ElementAt(i).Value;
                string sqlString = "(select distinct BusinessName, Address, City, State, Stars, numCheckin, numTips, businessid, distance from businesstable NATURAL JOIN categories NATURAL JOIN attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode + key + value + ")";
                this.businessId = this.businessId + "(select distinct businessid from businesstable NATURAL JOIN categories NATURAL JOIN attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode + key + value + ")";
                if (i != (attributes.Count() - 1))
                {
                    this.businessId = this.businessId + " intersect ";
                    sqlString = sqlString + "intersect";
                }
                categorySql = categorySql + sqlString;
            }


            if (categories.Count() == 0 && attributes.Count() == 0)
            {
                categorySql = "select distinct BusinessName, Address, City, State, Stars, numCheckin, numTips, businessid, distance from businesstable NATURAL JOIN categories Natural Join attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode;
                this.businessId = "select distinct businessid from businesstable NATURAL JOIN categories Natural Join attributes where state = '" + state + "' AND city = '" + city + "' AND zipcode = " + zipcode;
            }

            getBusinessId();
            return categorySql + orderby;
        }


        public string findcategory()
        {
            string sql = "";
            for (int i = 0; i < businessIDs.Count(); i++)
            {
                sql = sql + "(select  distinct categoryname from categories where businessid = '" + businessIDs[i] + "')";
                if (i != (businessIDs.Count() - 1))
                {
                    sql = sql + "union";
                }
            }
            return sql;
        }

        public string findAttributes()
        {
            string sql = "";
            for (int i = 0; i < businessIDs.Count(); i++)
            {
                sql = sql + "(select distinct attributename from  attributes  where businessid = '" + businessIDs[i] + "' and attributevalue <> 'False' and attributevalue<> 'no')";
                if (i != (businessIDs.Count() - 1))
                {
                    sql = sql + "union";
                }
            }
            return sql;
        }

        public void getBusinessId()
        {
            q.ExeQuery(this.businessId, "id");
            List<object> obj = new List<object>();
            obj = q.GetObjects();

            for (int i = 0; i < obj.Count(); i++)
            {
                businessIDs.Add((string)obj[i]);
            }


        }

        public void updateForSpecificBusiness(string newBusinessid)
        {
            businessIDs.Clear();
            this.businessIDs.Add(newBusinessid);
            

        }
    }
}
