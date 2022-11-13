namespace CvUpAPI
{
    public static class Globals
    {
        static int _companyId;
        public static int CompanyId
        {
            set { _companyId = value; }
            get { return _companyId; }
        }

        static int _userId;
        public static int UserId
        {
            set { _userId = value; }
            get { return _userId; }
        }
    }
}
