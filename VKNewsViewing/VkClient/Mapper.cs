using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkClient.Models;
using VkontakteApi.Models;

namespace VkClient
{
    public static class Mapper
    {
        public static List<VkClientPostModel> MapPostModel(PostModel postModel)
        {
            if (postModel == null)
            {
                return null;
            }
            var models = new List<VkClientPostModel>();
            var model = new VkClientPostModel
            {
                PostId = postModel.id,
                OwnerId = postModel.owner_id,
                FromId = postModel.from_id,
                Text = postModel.text,
                Date = postModel.date,
                Likes = postModel.likes.count,
                Reposts = postModel.reposts.count,
                Comments = postModel.comments.count,
            };
            if (postModel.attachments != null)
            {
                var str = new StringBuilder();
                str.Append(@"{""attachments"":");
                var attach = JsonConvert.SerializeObject(postModel.attachments);
                str.Append(attach+"}");
                model.Attachments = str.ToString();
            }

            models.Add(model);
            if (postModel.copy_history == null) return models;
            model.SourcePostId = postModel.copy_history[0].id;
            model.FromId = postModel.copy_history[0].from_id;
            var originalPost = postModel.copy_history[0];

            var newModel = new VkClientPostModel
            {
                PostId = originalPost.id,
                OwnerId = originalPost.owner_id,
                FromId = originalPost.from_id,
                Text = originalPost.text,
                Date = originalPost.date,
                Likes = originalPost.likes.count,
                Reposts = originalPost.reposts.count,
                Comments = originalPost.comments.count,
            };
            if (originalPost.attachments != null)
            {
                var str = new StringBuilder();
                str.Append(@"{""attachments"":");
                var attach = JsonConvert.SerializeObject(originalPost.attachments);
                str.Append(attach+"}");
                newModel.Attachments = str.ToString();
            }
            models.Add(newModel);

            return models;
        }

        public static VkClientUserModel MapUserModel(UserModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }
            return new VkClientUserModel
            {
                UserId = userModel.id,
                FirstName = userModel.first_name,
                LastName = userModel.last_name,
                Photo = userModel.photo,
                IsMember = userModel.IsMember,
                Hidden = userModel.hidden,
                FriendsCount = userModel.friends_count
            };
        }
    }
}