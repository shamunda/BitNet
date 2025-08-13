using System;
using System.Collections.Generic;
using System.Linq;

namespace BitNet.Core
{
    public class Tokenizer
    {
        private readonly List<string> _vocabulary;
        private readonly Dictionary<string, int> _tokenToId;

        public Tokenizer(List<string> vocabulary)
        {
            _vocabulary = vocabulary;
            _tokenToId = new Dictionary<string, int>();
            for (int i = 0; i < vocabulary.Count; i++)
            {
                _tokenToId[vocabulary[i]] = i;
            }
        }

        public List<int> Tokenize(string text)
        {
            // This is a very basic tokenizer. It will be replaced with a more
            // sophisticated one later.
            var tokens = new List<int>();
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (_tokenToId.TryGetValue(word, out var id))
                {
                    tokens.Add(id);
                }
                else
                {
                    // Handle unknown words. For now, we'll just ignore them.
                }
            }
            return tokens;
        }

        public string Detokenize(List<int> tokens)
        {
            var words = new List<string>();
            foreach (var token in tokens)
            {
                if (token >= 0 && token < _vocabulary.Count)
                {
                    words.Add(_vocabulary[token]);
                }
            }
            return string.Join(" ", words);
        }
    }
}
