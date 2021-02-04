using System.Collections.Generic;

namespace DevryService.Web.Models.Class
{
    public class CategoryViewModel
    {
        public ChannelModel Category { get; }
        public List<ChannelModel> Children { get;}

        public CategoryViewModel(ChannelModel category, List<ChannelModel> children)
        {
            Category = category;
            Children = children;
        }
    }
}