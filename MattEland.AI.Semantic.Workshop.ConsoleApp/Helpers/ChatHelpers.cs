using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;

public static class ChatHelpers
{
    public static bool IsContentFiltered(this List<ContentFilterResultsForPrompt> filters)
        => filters.Any(f => f.ContentFilterResults.Sexual.Filtered ||
                        f.ContentFilterResults.SelfHarm.Filtered ||
                        f.ContentFilterResults.Violence.Filtered);
}
