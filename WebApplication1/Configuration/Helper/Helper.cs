namespace Food_Ordering.Extensions.Helper
{
    public static class Helper
    {
        public static IQueryable<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
