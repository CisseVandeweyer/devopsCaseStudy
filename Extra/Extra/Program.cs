using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using CsvHelper;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter a football team (in English): ");
        string? searchTerm = Console.ReadLine();

        // Configuring the Chrome driver
        var chromeOptions = new ChromeOptions();
        //chromeOptions.AddArgument("--headless"); // Optional: make the browser invisible
        IWebDriver driver = new ChromeDriver(chromeOptions);

        // Navigate to whoscored.com search page
        driver.Navigate().GoToUrl($"https://www.whoscored.com/Search/?t={searchTerm}");

        // Click the first team link in the search results
        var teamLink = driver.FindElement(By.CssSelector("div.search-result a"));
        teamLink.Click();

        // Locate the 'Team Statistics' link by its CSS selector
        IWebElement teamStatisticsLink = driver.FindElement(By.CssSelector("ul li a[href*='/Statistics/'][class='']"));

        // Check if the link is not selected
        if (teamStatisticsLink != null && !teamStatisticsLink.GetAttribute("class").Contains("selected"))
        {
            // Click on the 'Team Statistics' link
            teamStatisticsLink.Click();
        }

        // Wait for the page to load
        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.CssSelector("table#team-goals-grid tbody tr")));

        // Find the goal type and amount from the table
        var goalRows = driver.FindElements(By.CssSelector("table#team-goals-grid tbody tr"));

        // Create a list to store the scraped goal data
        List<GoalData> goalDataList = new List<GoalData>();

        // Loop through each row and retrieve goal data
        foreach (var goalRow in goalRows)
        {
            string goalType = goalRow.FindElement(By.CssSelector(".situation-type")).Text;
            int goalAmount = int.Parse(goalRow.FindElement(By.CssSelector(".situation-value .stat-value")).Text);

            // Add the scraped goal data to the list
            goalDataList.Add(new GoalData { GoalType = goalType, GoalAmount = goalAmount });

            Console.WriteLine($"Goal Type: {goalType}, Goals: {goalAmount}");
        }

        // Write the goal data to CSV file
        using (var writer = new StreamWriter("whoscored.csv"))
        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(goalDataList);
        }

        // Write the goal data to JSON file
        File.WriteAllText("whoscored.json", JsonConvert.SerializeObject(goalDataList, Formatting.Indented));

        // Close the browser
        driver.Quit();
    }
}

// Define a class to represent the goal data
public class GoalData
{
    public string? GoalType { get; set; }
    public int GoalAmount { get; set; }
}
