var client = new HttpClient(new MyHttpMessageHandler());

DisplayAsciiArt();

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("fake-model-name", "fake-api-key", httpClient: client)
    .Build();

Console.WriteLine("Select an example to run:");
Console.WriteLine("1. Generate a short poem about cats");
Console.WriteLine("2. Rewrite text into a professional business email");
Console.Write("Enter your choice (1 or 2): ");

string choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await RunPoemExample(kernel);
        break;

    case "2":
        await RunEmailExample(kernel);
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