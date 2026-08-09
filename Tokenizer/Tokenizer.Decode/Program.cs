using Tokenizer;

try
{
    if (args.Length == 0)
        throw new ArgumentException("Usage: decode <token1> [<token2> ...]");

    var tokenIds = args.Select((a, i) =>
    {
        if (!int.TryParse(a, out var id))
            throw new ArgumentException($"Argument {i + 1} is not a valid integer token id: '{a}'");
        return id;
    }).ToList();

    var model = new Model();

    foreach (var id in tokenIds)
    {
        if (!model.Tokens.ContainsKey(id))
            throw new ArgumentException($"Unknown token id: {id}");
    }

    var text = model.Decode(tokenIds);

    Console.WriteLine(text);
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
