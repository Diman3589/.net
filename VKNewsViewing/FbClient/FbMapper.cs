using FacebookApi.Models;
using FbClient.Models;

namespace FbClient
{
    public static class FbMapper
    {
        public static FbClientPostModel MapPostModel(FbPostModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new FbClientPostModel
            {
                Id = model.id,
                Title = model.title,
                Description = model.description,
                Shares = model.share.share_count,
                Comments = model.share.comment_count
            };
        }
    }
}
