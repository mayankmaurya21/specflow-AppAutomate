using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BrowserStack;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TechTalk.SpecRun;

namespace TestApplication.UiTests.Drivers
{
    public class BrowserSeleniumDriverFactory
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly TestRunContext _testRunContext;

        public BrowserSeleniumDriverFactory(ConfigurationDriver configurationDriver, TestRunContext testRunContext)
        {
            _configurationDriver = configurationDriver;
            _testRunContext = testRunContext;
        }
        
        public IWebDriver GetForBrowser(string browserId)
        {
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("name", _configurationDriver.BaseSessionName + " " + browserId);
            caps.SetCapability("project", _configurationDriver.ProjectName);
            caps.SetCapability("build", _configurationDriver.BuildName);
            caps.SetCapability("app", "app_url");
            string lowerBrowserId = browserId.ToUpper();
            switch (lowerBrowserId)
            {
                case "GALAXYS10":
                    foreach(var tuple in _configurationDriver.GalaxyS10)
                    {
                        caps.SetCapability(tuple.Key, tuple.Value);
                    }
                    return new RemoteWebDriver(new Uri("https://" + _configurationDriver.BSUsername + ":" + _configurationDriver.BSAccessKey + "@" + _configurationDriver.SeleniumBaseUrl + "/wd/hub/"), caps);
                   
                case "PIXEL3":
                    foreach (var tuple in _configurationDriver.Pixel3)
                    {
                        caps.SetCapability(tuple.Key, tuple.Value);
                    }
                    return new RemoteWebDriver(new Uri("https://" + _configurationDriver.BSUsername + ":" + _configurationDriver.BSAccessKey + "@" + _configurationDriver.SeleniumBaseUrl + "/wd/hub/"), caps);
                    
                case "GALAXYNOTE20":
                    foreach (var tuple in _configurationDriver.GalaxyNote20)
                    {
                        caps.SetCapability(tuple.Key, tuple.Value);
                    }
                    return new RemoteWebDriver(new Uri("https://" + _configurationDriver.BSUsername + ":" + _configurationDriver.BSAccessKey + "@" + _configurationDriver.SeleniumBaseUrl + "/wd/hub/"), caps);
                case string browser: throw new NotSupportedException($"{browser} is not a supported browser");
                default: throw new NotSupportedException("not supported browser: <null>");
            }
           
        }
        
        public Local GetLocal(string browserId)
        {
            string lowerBrowserId = browserId.ToUpper();
            IEnumerable<IConfigurationSection> enumerator;
            switch (lowerBrowserId)
            {
                case "GALAXYNOTE20":
                    enumerator = _configurationDriver.GalaxyNote20;
                    break;
                case "GALAXYS10":
                    enumerator = _configurationDriver.GalaxyS10;
                    break;
                case "PIXEL3":
                    enumerator = _configurationDriver.Pixel3;
                    break;
                default:
                    return null;
            }
            foreach(var tuple in enumerator)
            {
                if (tuple.Key.ToString().Equals("browserstack.local") && tuple.Value.ToString().Equals("True") && !Process.GetProcessesByName("BrowserStackLocal").Any())
                {
                    Local _local = new Local();
                    List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() {
                        new KeyValuePair<string, string>("key", _configurationDriver.BSAccessKey)
                    };
                    _local.start(bsLocalArgs);
                    return _local;
                }
            }
            return null;
        }
    }
}
