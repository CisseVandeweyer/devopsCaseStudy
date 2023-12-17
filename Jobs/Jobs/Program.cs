using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using CsvHelper;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Voer een zoekterm in: ");
        string? zoekterm = Console.ReadLine();

        // Configuratie van de Chrome-driver
        var chromeOptions = new ChromeOptions();
        //chromeOptions.AddArgument("--headless"); // Optioneel: maak de browser onzichtbaar
        IWebDriver driver = new ChromeDriver(chromeOptions);

        // Navigeer naar de ICTJob-website met de opgegeven zoekterm
        driver.Navigate().GoToUrl($"https://www.ictjob.be/nl/it-vacatures-zoeken?keywords={zoekterm}");
        var datumKnop = driver.FindElement(By.CssSelector("a#sort-by-date"));
        datumKnop.Click(); // Klik op de "datum" knop

        // Wacht tot de pagina is geladen (kan worden verbeterd met expliciete wachttijden of andere methoden)
        System.Threading.Thread.Sleep(5000);

        // Selecteer de lijst met vacatures
        var jobList = driver.FindElements(By.CssSelector("div#search-result ul.search-result-list li.clearfix"));

        // Maak een lijst om de geschaafde gegevens op te slaan
        List<JobData> jobDataList = new List<JobData>();

        // Loop door de eerste 5 vacatures en haal de gewenste gegevens op
        for (int i = 0; i < Math.Min(jobList.Count, 5); i++)
        {
            var job = jobList[i];
            string title = job.FindElement(By.CssSelector("h2")).Text;
            string company = job.FindElement(By.CssSelector("span.job-company")).Text;
            string location = job.FindElement(By.CssSelector("span.job-location")).Text;
            string keywords = job.FindElement(By.CssSelector("span.job-keywords")).Text;
            string link = job.FindElement(By.CssSelector("a.search-item-link")).GetAttribute("href");

            // Voeg de geschaafde gegevens toe aan de lijst
            jobDataList.Add(new JobData { Title = title, Company = company, Location = location, Keywords = keywords, Link = link });

            Console.WriteLine($"Job {i + 1}:");
            Console.WriteLine($"Titel: {title}");
            Console.WriteLine($"Bedrijf: {company}");
            Console.WriteLine($"Locatie: {location}");
            Console.WriteLine($"Keywords: {keywords}");
            Console.WriteLine($"Link: {link}");
            Console.WriteLine();
        }

        // Write the data to CSV file
        using (var writer = new StreamWriter("jobs.csv"))
        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(jobDataList);
        }

        // Write the data to JSON file
        File.WriteAllText("jobs.json", JsonConvert.SerializeObject(jobDataList, Formatting.Indented));

        // Sluit de browser
        driver.Quit();
    }
}

// Definieer een klasse om de vacaturegegevens voor elke baan op te slaan
public class JobData
{
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Keywords { get; set; }
    public string Link { get; set; }
}
