using System.Collections.Generic;
using System.Linq;

for(int i = 0; i < 10; i++)
    Score.Add($"Player {i + 1}", 0);

Score["Player 2"] = 10;

// Below we are condensing the key value pair down 
// to a simple string
// This makes it easier for us to print
System.Console.WriteLine(string.Join("\n", 
    Score.Select(x=>$"{x.Key}: {x.Value}")));