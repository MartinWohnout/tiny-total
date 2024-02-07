using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TinyTotal
{
    internal class PasteFormatter
    {
        // Limit the amount of data to paste into this app
        private const int MAX_PASTE_SIZE = 1024 * 1024;

        private readonly char[] ACCEPTED_DELIMITERS = [
            ';',
            ' ',
            '\t',
            '\n'
        ];

        public IEnumerable<string> GetClipboardWords()
        {
            string clipboardContent = string.Empty;
            try
            {
                clipboardContent = GetClipboardContent();
            }
            catch (Exception e)
            {
                Logging.Instance.Log(e, "Unable to access clipboard."); 
            }

            return GetWords(clipboardContent);
        }

        public bool IsPasteAvailable()
        {
            return Clipboard.ContainsText();
        }


        private string GetClipboardContent()
        {
            var pastedText = Clipboard.GetText() ?? string.Empty;
            if (pastedText.Length > MAX_PASTE_SIZE)
            {
                Logging.Instance.LogInfo($"Attempted paste in excess of {MAX_PASTE_SIZE}.");
                return string.Empty;
            }

            return pastedText;
        }
        private IEnumerable<string> GetWords(string input)
        {
            return SplitByDelimiters(SanitizeInput(input));
        }
        private string[] SplitByDelimiters(string sanitizedInput)
        {
            return sanitizedInput.Split(ACCEPTED_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
        }
        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return String.Empty;
            }
            var toSanitize = input;
            toSanitize = NormalizeLinebreaks(toSanitize);
            toSanitize = RemoveMultipleDelimiters(toSanitize);
            return toSanitize;
        }
        private string NormalizeLinebreaks(string input)
        {
            return input
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }
        private string RemoveMultipleDelimiters(string input)
        {
            string delimiterList = string.Join(',', ACCEPTED_DELIMITERS);
            return Regex.Replace(input, "$[{delimiterList}]{2,}", (match) =>
            {
                return match.Value.Substring(0, 1);
            });

        }
    }
}
