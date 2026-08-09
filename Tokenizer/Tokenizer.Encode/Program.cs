using Tokenizer;

try
{
    if (args.Length == 0)
        throw new ArgumentException("Usage: encode <text> [<text2> ...]");

    var text = string.Join(" ", args);
    var model = new Model();
    var tokens = model.Encode(text);

    Console.WriteLine(string.Join(", ", tokens));
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
    Environment.Exit(2);
}
