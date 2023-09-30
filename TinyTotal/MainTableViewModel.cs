using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilities;

namespace SumMeUp
{
    public class MainTableViewModel : ObservableObject
    {
        public MainTableViewModel()
        {
            DataColumn = new ChildObservingCollection<ParsedEntry>();
            EnableDataColumnObservation();
            CalculateColumnTotal();

            Random rnd = new Random();
            AddContentCommand = new RelayCommand(() =>
                DataColumn.Add(new ParsedEntry(rnd.NextDouble().ToString()))
            );
        }

        public ICommand AddContentCommand { get; init; }


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
