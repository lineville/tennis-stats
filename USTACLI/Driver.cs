using OpenQA.Selenium.Chrome;
public class Driver
{
  /// <summary>
  /// Setup silent headless chrome driver service
  /// </summary>
  public static ChromeDriver Create()
  {
    ChromeOptions options = new ChromeOptions();
    options.AddArguments(
      "--no-sandbox",
      "--headless",
      "--disable-gpu",
      "--disable-logging",
      "--disable-dev-shm-usage",
      "--window-size=1920,1080",
      "--disable-extensions",
      "--log-level=OFF",
      "--user-agent=Chrome/73.0.3683.86",
      "--output=/dev/null"
    );

    return new ChromeDriver(options);
  }
}