namespace R10.Helper
{
    public class PagingModel
    {
        public int currentPage {set; get;}
        public int countPages {set; get;}
        public Func<int?, string> generateUrl {set; get;}
    }
}