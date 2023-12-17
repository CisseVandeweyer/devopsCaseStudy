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
        string filter = "&sp=EgIIAQ%253D%253D"; // This filter corresponds to "Filter -> Upload date -> Last hour"

        // Navigeer naar de zoekpagina van YouTube
        driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={zoekterm}{filter}");

        // Wacht tot de pagina is geladen (kan worden verbeterd met expliciete wachttijden of andere methoden)
        System.Threading.Thread.Sleep(4000);

        // Selecteer de lijst met video's
        var videoList = driver.FindElements(By.CssSelector("div#contents ytd-video-renderer"));

        // Create a list to store the scraped data
        List<VideoData> videoDataList = new List<VideoData>();

        // Loop door de eerste 5 video's en haal de gewenste gegevens op
        for (int i = 0; i < Math.Min(videoList.Count, 5); i++)
        {
            var video = videoList[i];
            string link = video.FindElement(By.CssSelector("a#thumbnail")).GetAttribute("href");
            string titel = video.FindElement(By.CssSelector("h3")).Text;
            string uploader = video.FindElement(By.CssSelector("#channel-info > #channel-name > #container > #text-container > #text > a")).Text;
            string weergaven = video.FindElement(By.CssSelector("span.inline-metadata-item")).Text;

            // Add the scraped data to the list
            videoDataList.Add(new VideoData { Link = link, Titel = titel, Uploader = uploader, Weergaven = weergaven });

            Console.WriteLine($"Video {i + 1}:");
            Console.WriteLine($"Link: {link}");
            Console.WriteLine($"Titel: {titel}");
            Console.WriteLine($"Uploader: {uploader}");
            Console.WriteLine($"Aantal weergaven: {weergaven}");
            Console.WriteLine();
        }

        // Write the data to CSV file
        using (var writer = new StreamWriter("youtube.csv"))
        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(videoDataList);
        }

        // Write the data to JSON file
        File.WriteAllText("youtube.json", JsonConvert.SerializeObject(videoDataList, Formatting.Indented));

        // Sluit de browser
        driver.Quit();
    }
}

// Define a class to represent the video data
public class VideoData
{
    public string? Link { get; set; }
    public string? Titel { get; set; }
    public string? Uploader { get; set; }
    public string? Weergaven { get; set; }
}
