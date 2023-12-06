namespace TinyTotal.UnitTests.ViewModels
{
    public class MainTableViewModelTests
    {
        // TODO: Test, ob in die Logs geschrieben wird

        [Test]
        public void AddContentCandidateCommand_AddNewLine_ClearsContentCandidate()
        {
            const decimal InitialValue = 1.0M;

            var testModel = GetTestModel();
            testModel.ContentCandidate = InitialValue.ToString();
            testModel.AddContentCandidateCommand.Execute(null);
            Assert.IsNull(testModel.ContentCandidate);
        }

        [Test]
        public void AddContentCandidateCommand_AddNewLineToEmpty_HasContent()
        {
            const decimal InitialValue = 1.0M;

            var testModel = GetTestModel();
            AddContentToModel(testModel, InitialValue.ToString());
            Assert.AreEqual(testModel.HasContent, true);
        }

        [Test]
        public void AddContentCandidateCommand_AddNumberToEmpty_CalculatesTotal()
        {
            const decimal InitialValue = 1.0M;

            var testModel = GetTestModel();
            AddContentToModel(testModel, InitialValue.ToString());
            Assert.AreEqual(testModel.ColumnTotal, InitialValue);
        }

        [Test]
        public void AddContentCandidateCommand_AddNumber_CalculatesTotal()
        {
            const decimal InitialValue = 1.0M;
            const decimal AddedValue = 2.0M;
            Func<decimal> GetColumnTotal = () => InitialValue + AddedValue;

            var testModel = GetTestModel();
            AddContentToModel(testModel, InitialValue.ToString());
            AddContentToModel(testModel, AddedValue.ToString());
            Assert.AreEqual(testModel.ColumnTotal, GetColumnTotal());
        }

        [Test]
        public void AddContentCandidateCommand_AddNull_NoNewLine()
        {
            var testModel = GetTestModel();
            AddContentToModel(testModel, null);
            Assert.AreEqual(testModel.HasContent, false);
        }

        [Test]
        public void AddContentCandidateCommand_AddEmptyValue_NoNewLine()
        {
            var testModel = GetTestModel();
            AddContentToModel(testModel, string.Empty);
            Assert.AreEqual(testModel.HasContent, false);
        }

        [Test]
        public void AddContentCandidateCommand_AddText_DisplaysText()
        {
            const string DisplayText = "TestContent";

            var testModel = GetTestModel();
            AddContentToModel(testModel, DisplayText);
            var displayedLine = testModel.DataColumn.First();
            Assert.AreEqual(displayedLine.DisplayValue, DisplayText);
        }

        [Test]
        public void AddContentCandidateCommand_AddText_UnchangedTotal()
        {
            const decimal NumericValue = 1.0M;
            const string TextValue = "TestContent";

            var testModel = GetTestModel();
            AddContentToModel(testModel, NumericValue.ToString());
            AddContentToModel(testModel, TextValue);
            Assert.AreEqual(testModel.ColumnTotal, NumericValue);
        }

        [Test]
        public void AddRandomNumberCommand_AddNewLineToEmpty_HasContent()
        {
            var testModel = GetTestModel();
            testModel.AddRandomNumberCommand.Execute(null);
            Assert.AreEqual(testModel.HasContent, true);
        }

        private MainTableViewModel GetTestModel()
        {
            return new MainTableViewModel();
        }

        private void AddContentToModel(MainTableViewModel targetModel, string content)
        {
            targetModel.ContentCandidate = content;
            targetModel.AddContentCandidateCommand.Execute(null);
        }
    }
}