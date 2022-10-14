
    
namespace LuceneLibrary
{
    public interface ILuceneService
    {
        public void BuildIndex();
        public void DocumentAdd();
        public void DocumentDelete();
        public void WarmupSearch();
        IEnumerable<SearchEntry> Search(string searchQuery);
    }
}