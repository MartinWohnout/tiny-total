using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SumMeUp
{
    public class ParsedEntry : ObservableObject
    {
        internal ParsedEntry(string entry)
        {
            DisplayValue = entry ?? string.Empty;
        }

        private bool m_parsedAsNumber;
        public bool ParsedAsNumber
        {
            get => m_parsedAsNumber;
            private set => SetProperty(ref m_parsedAsNumber, value);
        }

        private string m_displayValue;
        public string DisplayValue
        {
            get => m_displayValue;
            set
            {
                if (SetProperty(ref m_displayValue, value))
                {
                    UpdateParsing(value);
                }
            }
        }

        private decimal? m_parsedValue;
        public decimal? ParsedValue
        {
            get => m_parsedValue;
            private set => SetProperty(ref m_parsedValue, value);
        }

        private void UpdateParsing(string toParse)
        {
            bool successfulParsing = false;
            decimal? parsingResult = null;
            if (!String.IsNullOrWhiteSpace(toParse))
            {
                try
                {
                    successfulParsing = Decimal.TryParse(toParse, out var res);
                    if (successfulParsing)
                    {
                        parsingResult = res;
                    }
                }
                catch (Exception e)
                {
                    // TODO need a logging framework
                }
            }
            ParsedAsNumber = successfulParsing;
            ParsedValue = parsingResult;
        }
    }
}
