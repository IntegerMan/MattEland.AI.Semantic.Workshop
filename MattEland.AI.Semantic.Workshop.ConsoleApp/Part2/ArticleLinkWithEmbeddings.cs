using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value - exists for deserialization

public class ArticleLinkWithEmbeddings
{
    public string Name { get; set; }
    public string Url { get; set; }
    public float[] Embeddings { get; set; }
    public double Score { get; internal set; }
}

#pragma warning restore CS8618
