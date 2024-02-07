using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TinyTotal;
using WpfUtilities;

namespace TinyTotal
{
    public class MainTableViewModel : ObservableObject
    {
        public MainTableViewModel()
        {
            DataColumn = new ChildObservingCollection<ParsedEntry>();
            EnableDataColumnObservation();
            CalculateColumnTotal();

            AddContentCandidateCommand = new RelayCommand(AddContentCandidate, CanAddContentCandidate);
            PropertyChanged += (sender, args) =>
            {
                if (string.Equals(args.PropertyName, nameof(ContentCandidate)))
                {
                    AddContentCandidateCommand.NotifyCanExecuteChanged();
                }
            };

            AddRandomNumberCommand = new RelayCommand(AddRandomNumber, CanAddRandomNumber);
            PasteCommand = new RelayCommand(PasteContent, CanPasteContent);
        }


        private PasteFormatter m_pasteHelper;
        private PasteFormatter PasteHelper
        {
            get
            {
                if (m_pasteHelper == null)
                {
                    m_pasteHelper = new PasteFormatter();
                }
                return m_pasteHelper;
            }
        }


        public IRelayCommand AddContentCandidateCommand { get; init; }
        public IRelayCommand AddRandomNumberCommand { get; init; }

        public IRelayCommand PasteCommand { get; init; }


        private void AddContentCandidate()
        {
            if (AddContent(ContentCandidate))
            {
                ContentCandidate = null;
            }
        }
        private bool CanAddContentCandidate()
        {
            return CanAddContent(ContentCandidate);
        }

        private void AddRandomNumber()
        {
            AddContent(GetRandomPositiveTestDecimal().ToString());

        }
        private bool CanAddRandomNumber()
        {
            return true;
        }

        private bool AddContent(string content)
        {
            Logging.Instance.LogDebug($"Trying to add new entry: {content ?? "<null>"}");
            var newEntry = new ParsedEntry(content);
            try
            {
                DataColumn.Add(newEntry);
            }
            catch (Exception e)
            {
                Logging.Instance.Log(e, "Unable to add entry.");
                return false;
            }
            Logging.Instance.LogDebug($"Added new entry: {newEntry.DisplayValue}");
            return true;
        }
        private bool CanAddContent(string content)
        {
            return content != null;
        }

        private readonly Random RandomNumberGenerator = new Random();
        // For testing purposes only. Please don't use this in prod.
        private decimal GetRandomPositiveTestDecimal()
        {
            var upperBound = Decimal.Floor(Math.Min(Decimal.MaxValue, Int64.MaxValue));
            decimal leftOfDot = (decimal)RandomNumberGenerator.NextInt64(0, (Int64)upperBound);
            decimal rightOfDot = (decimal)RandomNumberGenerator.NextDouble();
            return leftOfDot + rightOfDot;
        }

        private void PasteContent()
        {
            try
            {
                foreach (var word in PasteHelper.GetClipboardWords())
                {
                    AddContent(word);
                }
            }
            catch (Exception e)
            {
                Logging.Instance.Log(e, "Unable to execute paste.");
            }
        }
        private bool CanPasteContent()
        {
            return PasteHelper.IsPasteAvailable();
        }


        private string m_contentCandidate;
        public string ContentCandidate
        {
            get => m_contentCandidate;
            set => SetProperty(ref m_contentCandidate, value);
        }

        private bool m_hasContent;
        public bool HasContent
        {
            get => m_hasContent;
            private set => SetProperty(ref m_hasContent, value);
        }

        private decimal m_columnTotal;
        public decimal ColumnTotal
        {
            get => m_columnTotal;
            set => SetProperty(ref m_columnTotal, value);
        }

        public ChildObservingCollection<ParsedEntry> DataColumn { get; init; }

        private void EnableDataColumnObservation()
        {
            DataColumn.CollectionChanged += ColumnEntriesChanged;
            DataColumn.ChildPropertyChanged += SingleColumnEntryChanged;
        }

        private void ColumnEntriesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (sender is Collection<ParsedEntry> column)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        UpdateColumnTotal(args.NewItems.OfType<ParsedEntry>(), true);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        UpdateColumnTotal(args.OldItems.OfType<ParsedEntry>(), false);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        // TODO step through once to verify behavior
                        UpdateColumnTotal(args.OldItems.OfType<ParsedEntry>(), false);
                        UpdateColumnTotal(args.NewItems.OfType<ParsedEntry>(), true);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        CalculateColumnTotal();
                        break;
                    default:
                        break;
                }
                HasContent = column.Any();
            }
        }
        private void SingleColumnEntryChanged(object sender, ChildPropertyChangedEventArgs<ParsedEntry> args)
        {
            if (sender is Collection<ParsedEntry> column)
            {
                // TODO INotifyPropertyChanging for optimization
                CalculateColumnTotal();
            }
        }

        private void CalculateColumnTotal()
        {
            // TODO: Potentiellen Overflow abfangen
            ColumnTotal = DataColumn.Sum(c => c.ParsedAsNumber ? c.ParsedValue.Value : Decimal.Zero);
        }
        private void UpdateColumnTotal(IEnumerable<ParsedEntry> entries, bool entriesAdded)
        {
            foreach (var entry in entries ?? Enumerable.Empty<ParsedEntry>())
            {
                UpdateColumnTotal(entry, entriesAdded);
            }
        }
        private void UpdateColumnTotal(ParsedEntry entry, bool entryAdded)
        {
            if (entry?.ParsedAsNumber == true)
            {
                ColumnTotal = entryAdded ?
                    ColumnTotal + entry.ParsedValue.Value :
                    ColumnTotal - entry.ParsedValue.Value;
            }
        }
    }
}
