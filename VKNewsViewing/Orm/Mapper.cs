using Domain;
using VkClient.Models;

namespace Orm
{
    public static class Mapper
    {
        public static VkUser MapVkUserModel (VkClientUserModel userModel)
        {
            return new VkUser
            {
                Id = userModel.UserId,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Photo = userModel.Photo,
                Hidden = userModel.Hidden,
                FriendsCount = userModel.FriendsCount,
                IsMember = userModel.IsMember
            };

        }
        public static VkPost MapVkPostModel(VkClientPostModel post)
        {
            return new VkPost
            {
                PostId = post.PostId,
                OwnerId = post.OwnerId,
                Text = post.Text,
                Date = (long)post.Date,
                RepostId = post.SourcePostId,
                FromId = post.FromId,
                Likes = post.Likes,
                Comments = post.Comments,
                Shares = post.Reposts,
                Attachments = post.Attachments
            };

        }
    }
}