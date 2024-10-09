namespace TokenProject.Helper
{
    public  class Jwt
    {
        public  string key { get; set; }
        public  string issuer { get; set; }
        public  string audience { get; set; }
        public  double duration { get; set; }
    }
}
