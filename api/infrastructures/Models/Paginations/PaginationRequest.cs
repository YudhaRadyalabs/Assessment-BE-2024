namespace infrastructures.Models.Paginations
{
    public class PaginationRequest
    {
        public string GeneralSearch { get; set; } = "";
        public List<SearchModel> Searches { get; set; } = new List<SearchModel>();
        public List<SortModel> Sorts { get; set; } = new List<SortModel>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchModel
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public class SortModel
    {
        public string Field { get; set; }
        public string Order { get; set; }
    }
}
