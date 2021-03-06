namespace Twitter_BE.DTOs;

public class PaginationMetadata
{
    public PaginationMetadata(int totalCount, int currentPage, int ItemsPerPage)
    {
        TotalCount = totalCount;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)ItemsPerPage);
    }
    public int CurrentPage { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

}