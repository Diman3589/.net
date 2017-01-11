using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class VkGroup
    {
        public VkGroup()
        {
            VkUsers = new HashSet<VkUser>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VkGroupId { get; set; }

        public string Name { get; set; }
        public virtual ICollection<VkUser> VkUsers { get; set; }
    }
}
