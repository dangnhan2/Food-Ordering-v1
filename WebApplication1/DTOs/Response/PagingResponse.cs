namespace Food_Ordering.DTOs.Response
{
    public class PagingResponse<T>
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public ICollection<T> data { get; set; }

        public PagingResponse(List<T> items, int count, int currentPage, int pageSize)
        {
            this.currentPage = currentPage;
            this.pageSize = pageSize;
            total = count;
            data = items;
        }
    }
}
