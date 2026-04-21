using System;

class Program
{
    static void Main(string[] args)
    {
        // Use Environment Variables to hide sensitive data
        string apiKey = Environment.GetEnvironmentVariable("MY_APP_API_KEY");
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: API Key not set in environment variables.");
            return;
        }

        // Your functional logic here
        for (int current_idx = 0; current_idx < 5; current_idx++)
        {
            Console.WriteLine($"Processing step {current_idx}...");
            Console.Beep(800 + (current_idx * 100), 150);
        }
    }
}