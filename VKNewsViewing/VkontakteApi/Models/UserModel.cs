using System.Collections.Generic;

namespace VkontakteApi.Models
{
    public class UserModel
    {
        public int id;
        public string first_name;
        public string last_name;
        public string photo;
        public string deactivated;
        public int hidden = 0;
        public bool IsMember = false;
        public int friends_count;
    }

    public class UsersCollection
    {
        public int count;
        public List<UserModel> items { get; set; }
    }
}