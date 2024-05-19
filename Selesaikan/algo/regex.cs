using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    // huruf hidup boleh ilang
       static Dictionary<char, string> substitutions = new Dictionary<char, string>
    {
        {'a', "[a4@]?"},
        {'b', "[b8]"},
        {'c', "[c<({]"},
        {'d', "[d)]"},
        {'e', "[e3]?"},
        {'f', "[f#]"},
        {'g', "[g9]"},
        {'h', "[h#]"},
        {'i', "[i1!|l]?"},
        {'j', "[j]"},
        {'k', "[k]"},
        {'l', "[l1!|]"},
        {'m', "[m]"},
        {'n', "[n]"},
        {'o', "[o0]?"},
        {'p', "[p]"},
        {'q', "[q]"},
        {'r', "[r]"},
        {'s', "[s5$]"},
        {'t', "[t7+]"},
        {'u', "[u]?"},
        {'v', "[v]"},
        {'w', "[w]"},
        {'x', "[x%]"},
        {'y', "[y]"},
        {'z', "[z2]"},
    };

    static Regex BuildAlayRegex(string inputText)
    {
        string pattern = ".*?"; 
        foreach (char ch in inputText.ToLower())
        {
            if (substitutions.ContainsKey(ch))
            {
                pattern += substitutions[ch];
            }
            else
            {
                pattern += Regex.Escape(ch.ToString()) + "?";
            }
            pattern += ".*?";  
        }
        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }
    static void Main()
    {
        string inputText = "hello world";
        Regex regex = BuildAlayRegex(inputText);
        Console.WriteLine($"Regex for '{inputText}': {regex}");

        
        string[] testTexts = { "h3ll0 w0rld", "hll wrld", "he11o wor1d!", "H3|_|_0 W0r|)" };
        foreach (string test in testTexts)
        {
            if (regex.IsMatch(test))
            {
                Console.WriteLine($"Match found: '{test}'");
            }
            else
            {
                Console.WriteLine($"No match: '{test}'");
            }
        }
    }
}
