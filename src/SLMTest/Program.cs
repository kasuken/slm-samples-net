using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

var client = new HttpClient(new MyHttpMessageHandler());

DisplayAsciiArt();

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("fake-model-name", "fake-api-key", httpClient: client)
    .Build();

Console.WriteLine("Select an example to run:");
Console.WriteLine("1. Generate a short poem about cats");
Console.WriteLine("2. Rewrite text into a professional business email");
Console.WriteLine("3. Chat with the AI assistant");
Console.WriteLine("4. Get tomorrow's horoscope (JSON format)");
Console.WriteLine("5. Rag System Prompt");
Console.Write("Enter your choice (1 or 2 or 3 or 4 or 5): ");

string choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await RunPoemExample(kernel);
        break;
    case "2":
        await RunEmailExample(kernel);
        break;
    case "3":
        await RunChatExample(kernel);
        break;
    case "4":
        await RunHoroscopeExample(kernel);
        break;
    case "5":
        await RunRagExample(kernel);
        break;

    default:
        Console.WriteLine("Invalid choice. Please run the program again.");
        break;
}

static void DisplayAsciiArt()
{
    string[] asciiArt = new[]
    {
        @" ███████╗██╗     ███╗   ███╗",
        @" ██╔════╝██║     ████╗ ████║",
        @" ███████╗██║     ██╔████╔██║",
        @" ╚════██║██║     ██║╚██╔╝██║",
        @" ███████║███████╗██║ ╚═╝ ██║",
        @" ╚══════╝╚══════╝╚═╝     ╚═╝"
    };

    ConsoleColor[] rainbowColors = new[]
    {
        ConsoleColor.Red,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.Cyan,
        ConsoleColor.Blue,
        ConsoleColor.Magenta
    };

    foreach (string line in asciiArt)
    {
        for (int i = 0; i < line.Length; i++)
        {
            Console.ForegroundColor = rainbowColors[i % rainbowColors.Length];
            Console.Write(line[i]);
        }
        Console.WriteLine();
    }

    Console.ResetColor();
    Console.WriteLine();
}

static async Task RunPoemExample(Kernel kernel)
{
    string prompt = "Write a short poem about cats";
    var response = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine("Response:");
    Console.WriteLine(response.GetValue<string>());
}

static async Task RunEmailExample(Kernel kernel)
{
    Console.WriteLine("Enter the message you want to convert to a business email:");
    string userInput = Console.ReadLine();

    if (string.IsNullOrEmpty(userInput))
    {
        userInput = "Tell David that I'm going to finish the business plan by the end of the week.";
    }

    string promptMail = """
    Rewrite the text between triple backticks into a business mail. Use a professional tone, be clear and concise.
    Sign the mail as AI Assistant.
    Text: ```{{$input}}```
    """;
    var mailFunction = kernel.CreateFunctionFromPrompt(promptMail, new OpenAIPromptExecutionSettings
    {
        Temperature = 0.7,
        MaxTokens = 1000
    });

    KernelArguments arguments = new KernelArguments
    {
        { "input", userInput }
    };

    var responseMail = await kernel.InvokeAsync(mailFunction, arguments);
    Console.WriteLine("Response:");
    Console.WriteLine(responseMail.GetValue<string>());
}

static async Task RunChatExample(Kernel kernel)
{
    var chat = kernel.GetRequiredService<IChatCompletionService>();
    var history = new ChatHistory();

    history.AddUserMessage("Hello, how are you?");
    history.AddAssistantMessage("I'm fine, thank you! How can I assist you today?");
    history.AddUserMessage("Can you tell me a joke?");
    var response = await chat.GetChatMessageContentAsync(history);

    Console.WriteLine("Response:");
    Console.WriteLine(response);

    // Add the assistant's response to the history for context in future exchanges
    history.AddAssistantMessage(response.ToString());
}

static async Task RunRagExample(Kernel kernel)
{
    Console.WriteLine("GitHub RAG System - Ask me anything about GitHub:");
    var chat = kernel.GetRequiredService<IChatCompletionService>();
    var history = new ChatHistory();

    // Add system message with RAG context
    history.AddSystemMessage("""
        You are a helpful assistant that specializes in GitHub knowledge.
        When answering questions, use the following information from the GitHub documentation if relevant:

        GitHub is a code hosting platform for version control and collaboration.
        It lets you and others work together on projects from anywhere.

        Key GitHub concepts:
        - Repositories: Used to organize a single project with files, folders, and documentation
        - Branches: Parallel versions of a repository for working on different features
        - Commits: Saved changes with descriptive messages
        - Pull Requests: Proposed changes to a repository that can be reviewed
        - Issues: Track work, bugs, and feature requests
        - Actions: Automated workflows for CI/CD

        Use this information to provide accurate and helpful responses about GitHub.
    """);

    bool continueChat = true;

    while (continueChat)
    {
        Console.Write("\nYour question (or type 'exit' to quit): ");
        string userQuestion = Console.ReadLine();

        if (string.IsNullOrEmpty(userQuestion) || userQuestion.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            continueChat = false;
            continue;
        }

        history.AddUserMessage(userQuestion);

        Console.WriteLine("\nSearching knowledge base and generating response...");
        var response = await chat.GetChatMessageContentAsync(history);

        Console.WriteLine("\nResponse:");
        Console.WriteLine(response);

        // Add the assistant's response to the history for context in future exchanges
        history.AddAssistantMessage(response.ToString());
    }
}

static async Task RunHoroscopeExample(Kernel kernel)
{
    Console.WriteLine("Enter your zodiac sign (e.g., Aries, Taurus, Gemini, etc.):");
    var zodiacSign = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(zodiacSign))
    {
        Console.WriteLine("No sign entered. Using Aries as default.");
        zodiacSign = "Aries";
    }

    // Define the JSON schema for the horoscope in the prompt
    var horoscopePrompt = """
        Act as a professional astrologer with 20 years of experience.
        Generate tomorrow's horoscope for the {{$zodiacSign}} zodiac sign.
    """;

    var executionSettings = new OpenAIPromptExecutionSettings
    {
        Temperature = 0.7,
        MaxTokens = 2000,
        #pragma warning disable SKEXP0010
        ResponseFormat = typeof(HoroscopePrediction)
    };

    var horoscopeFunction = kernel.CreateFunctionFromPrompt(horoscopePrompt, executionSettings);

    var arguments = new KernelArguments
    {
        { "zodiacSign", zodiacSign }
    };

    Console.WriteLine($"\nGenerating tomorrow's horoscope for {zodiacSign}...");

    try
    {
        var response = await kernel.InvokeAsync(horoscopeFunction, arguments);
        var jsonResponse = response.GetValue<string>();

        // Deserialize the JSON response into the HoroscopePrediction object
        var horoscopePrediction = JsonSerializer.Deserialize<HoroscopePrediction>(jsonResponse);

        // Pretty print the JSON
        var formattedJson = JsonSerializer.Serialize(horoscopePrediction, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine("\nHoroscope (JSON format):");
        Console.WriteLine(formattedJson);

        //Extract and display key information in a user-friendly way
        DisplayHoroscopeSummary(horoscopePrediction);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating horoscope: {ex.Message}");
    }
}

static void DisplayHoroscopeSummary(HoroscopePrediction horoscopePrediction)
{
    Console.WriteLine("\n--- Horoscope Summary ---");
    Console.WriteLine($"Zodiac Sign: {horoscopePrediction.Sign}");
    Console.WriteLine($"Compatibility: {horoscopePrediction.Compatibility}");
    Console.WriteLine($"Mood: {horoscopePrediction.Mood}");
    Console.WriteLine($"General: {horoscopePrediction.Horoscope.General}");
    Console.WriteLine($"Love: {horoscopePrediction.Horoscope.Love}");
    Console.WriteLine($"Career: {horoscopePrediction.Horoscope.Career}");
    Console.WriteLine($"Health: {horoscopePrediction.Horoscope.Health}");
    Console.WriteLine($"Lucky Number: {horoscopePrediction.Horoscope.LuckyNumber}");
    Console.WriteLine("-------------------------");
}