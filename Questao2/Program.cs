using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);



        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }
    public static int getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;

 
        using (HttpClient client = new HttpClient())
        {
            string baseUrl = "https://jsonmock.hackerrank.com/api/football_matches";
            int currentPage = 1;
            int totalPages = 1;


            foreach (string teamType in new[] { "team1", "team2" })
            {
                do
                {
                    string url = $"{baseUrl}?year={year}&{teamType}={Uri.EscapeDataString(team)}&page={currentPage}";
                    var response = client.GetStringAsync(url).Result;
                    var jsonResponse = JObject.Parse(response);

                 
                    totalPages = (int)jsonResponse["total_pages"];

                    foreach (var match in jsonResponse["data"])
                    {
                        totalGoals += int.Parse((string)match[$"{teamType}goals"]);
                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                currentPage = 1;
            }
        }

        return totalGoals;
    }

}