using OpenQA.Selenium.Chrome;
public class Driver
{

  // TODO Make this a global singleton service that can be injected to each command
  /// <summary>
  /// Setup silent headless chrome driver service
  /// </summary>
  public static ChromeDriver Create()
  {
    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
    service.SuppressInitialDiagnosticInformation = true;

    ChromeOptions options = new ChromeOptions();
    options.AddArguments(
      "no-sandbox",
      "headless",
      "disable-gpu",
      "disable-logging",
      "disable-dev-shm-usage",
      "window-size=1920,1080",
      "disable-extensions",
      "log-level=OFF",
      "--user-agent=Chrome/73.0.3683.86",
      "output=/dev/null"
    );

    var driver = new ChromeDriver(service, options);
    return driver;
  }
}