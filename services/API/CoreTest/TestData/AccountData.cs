namespace AqLife.CoreTest.TestData
{
    public static class AccountTestData
    {
        public static TheoryData<string> InvalidDescData => new()
        {
            new string('A', 256),
        };
        public static TheoryData<string> ValidDescData => new()
        {
            new string('A', 255),
            new string('B', 254),
            new string('C', 1),
            new string('D', 0),
            string.Empty
        };
    }
}
