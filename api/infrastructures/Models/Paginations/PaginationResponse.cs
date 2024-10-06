namespace infrastructures.Models.Paginations
{
    public class PaginationResponse<TModel>
            where TModel : class
    {
        public List<TModel> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PaginationResponse(List<TModel> source, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            Data = source.ToList();
        }
    }
}
