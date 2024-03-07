namespace WebAppAutores.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int itemsPerPage = 5;
        private readonly int maxItemsPerPage = 20;
        

        public int ItemsPerPage
        {
            get
            {
                return itemsPerPage;
            }
            set
            {
                itemsPerPage = (value > maxItemsPerPage) ? maxItemsPerPage : value; // keeps items within range
            }
        }
    }
}
