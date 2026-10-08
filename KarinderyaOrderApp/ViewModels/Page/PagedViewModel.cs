namespace KarinderyaOrderApp.ViewModels.Page
{
    public class PagedViewModel
    {
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
        public int FirstItem => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int LastItem => Math.Min(Page * PageSize, TotalCount);
    }
}
