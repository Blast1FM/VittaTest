namespace VittaTest.ViewModels.Messages
{
    public class CloseWindowMessage
    {
        public bool? DialogResult { get; }

        public CloseWindowMessage(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}
