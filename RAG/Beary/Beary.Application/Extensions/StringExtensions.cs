﻿using Beary.Entities;

namespace Beary.Application.Extensions;

internal static class StringExtensions
{
    internal static IEnumerable<ChatContent> AsChatContents(this IEnumerable<string> articles, ChatRole role)
        => articles.Select(a => ChatContent.From(a, role));

}
