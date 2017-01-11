using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class VkUser
    {
        public VkUser()
        {
            VkUserFrom = new HashSet<VkUser>();
            VkUserTo = new HashSet<VkUser>();
            //Groups = new HashSet<VkGroup>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public int Hidden { get; set; }
        public int FriendsCount { get; set; }
        public bool IsMember { get; set; }

        public virtual ICollection<VkUser> VkUserTo { get; set; }
        public virtual ICollection<VkUser> VkUserFrom { get; set; }
        //public virtual ICollection<VkGroup> Groups { get; set; }
    }
}
