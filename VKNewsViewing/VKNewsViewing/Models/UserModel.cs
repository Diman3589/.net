using System.Collections.Generic;

namespace VKNewsViewing.Models
{
    public class UserModel
    {
        public int id;
        public string first_name;
        public string last_name;
        public string nickname;
        public int hidden;
        public string deactivated;
    }

    public class UsersCollection
    {
        public int count;
        public List<UserModel> items { get; set; }

        /*public UsersCollection(string json)
        {
            
            
           // var jData = JObject.Parse(json).;
            //var jArray = jData.geGetNamedArray("data");

           foreach (var item in jArray)
            {
                var itemObject = item.GetObject();
                string itemID = itemObject.GetNamedString("id");
                string itemName = itemObject.GetNamedString("name");
                var itemStyle = itemObject.GetNamedObject("style");
                string itemDescription = itemStyle.GetNamedString("description");

                //etc
            }
            var jss = new JavaScriptSerializer();

            dynamic obj = jss.Deserialize<dynamic>(json);
            var count = obj["response"]["count"];

            var obj1 = new List<object>(obj["response"]["items"]);
            Console.WriteLine(obj[0]["id"]);
        }*/
    }
}