namespace Pulsar.Client.Recovery.Browsers
{
    public class AllBrowsers
    {
        public BrowserChromium[] Chromium { get; set; }
        public BrowserGecko[] Gecko { get; set; }
    }

    public class BrowserChromium
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string LocalState { get; set; }
        public ProfileChromium[] Profiles { get; set; }
    }

    public class ProfileChromium
    {
        public string Name { get; set; }
        public string LoginData { get; set; }
        public string Cookies { get; set; }
        public string Path { get; set; }
    }

    public class BrowserGecko
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Key4 { get; set; }
        public string Logins { get; set; }
        public string ProfilesDir { get; set; }
    }
}
