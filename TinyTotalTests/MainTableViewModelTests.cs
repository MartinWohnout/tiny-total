namespace TinyTotal.UnitTests.ViewModels
{
    public class MainTableViewModelTests
    {
        // TODO: Test, ob in die Logs geschrieben wird

        [Test]
        public void AddContentCandidateCommand_AddNewLineToEmpty_HasContent()
        {
            const decimal InitialValue = 1.0M;

            var testModel = GetTestModel();
            testModel.ContentCandidate = InitialValue.ToString();
            testModel.AddContentCandidateCommand.Execute(null);
            Assert.AreEqual(testModel.HasContent, true);
        }
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
        public void AddContentCandidateCommand_AddNumber_CalculatesTotal()
        {
            const decimal InitialValue = 1.0M;
            const decimal AddedValue = 2.0M;
            Func<decimal> GetColumnTotal = () => InitialValue + AddedValue;

            var testModel = GetTestModel();
            testModel.ContentCandidate = InitialValue.ToString();
            testModel.AddContentCandidateCommand.Execute(null);
            testModel.ContentCandidate = AddedValue.ToString();
            testModel.AddContentCandidateCommand.Execute(null);
            Assert.AreEqual(testModel.ColumnTotal, GetColumnTotal());
        }

        [Test]
        public void AddContentCandidateCommand_AddEmptyValue_NoNewLine()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddContentCandidateCommand_AddText_DisplaysText()
        {
            throw new NotImplementedException();
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
    }
}