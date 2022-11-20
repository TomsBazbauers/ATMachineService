namespace ATMachineService.Tests.FinancialServiceTests
{
    public class FinancialOperationsValidatorTests
    {
        private readonly IFinancialOperationsValidator _sut;

        public FinancialOperationsValidatorTests()
        {
            _sut = new FinancialOperationsValidator();
        }

        [Theory]
        [InlineData]
        [InlineData]
        [InlineData]
        public void IsValidUpdate_InputValid_ReturnsTrue()
        {
            // Arrange
            var testPassword = Guid.NewGuid();
            var testDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            var testPendingUpdate = new ATMStatusUpdate("90901001", testDeposit, testPassword, DateTime.Today);
            var testRequestUpdate = new ATMStatusUpdate("90901001", testDeposit, testPassword, DateTime.Today);

            // Act
            var actual = _sut.IsValidUpdate(testRequestUpdate, testPendingUpdate);

            // Arrange
            Assert.True(actual);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(500)]
        [InlineData(5000)]
        public void IsValidUpdate_InputInvalidDeposit_ReturnsFalse(int testNoteCount)
        {
            // Arrange
            var testPassword = Guid.NewGuid();
            var testPendingDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            var testRequestDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, testNoteCount },
                { 50, 100 }
            });

            var testPendingUpdate = new ATMStatusUpdate("90901001", testPendingDeposit, testPassword, DateTime.Today);
            var testRequestUpdate = new ATMStatusUpdate("90901001", testRequestDeposit, testPassword, DateTime.Today);

            // Act
            var actual = _sut.IsValidUpdate(testRequestUpdate, testPendingUpdate);

            // Arrange
            Assert.False(actual);
        }

        [Theory]
        [InlineData]
        [InlineData]
        [InlineData]
        [InlineData]
        public void IsValidUpdate_InputInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var testPendingPassword = Guid.NewGuid();
            var testRequestPassword = Guid.NewGuid();
            var testDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            var testPendingUpdate = new ATMStatusUpdate("90901001", testDeposit, testPendingPassword, DateTime.Today);
            var testRequestUpdate = new ATMStatusUpdate("90901001", testDeposit, testRequestPassword, DateTime.Today);

            // Act
            var actual = _sut.IsValidUpdate(testRequestUpdate, testPendingUpdate);

            // Arrange
            Assert.False(actual);
        }

        [Fact]
        public void IsValidPassword_InputValid_ReturnsTrue()
        {
            // Arrange
            var testPendingUpdatePassword = Guid.NewGuid();
            var testRequestUpdatePassword = testPendingUpdatePassword;

            // Act
            var actual = _sut.IsValidPassword(testRequestUpdatePassword, testPendingUpdatePassword);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValidPassword_InputInvalid_ReturnsFalse()
        {
            // Arrange
            var testPendingUpdatePassword = Guid.NewGuid();
            var testRequestUpdatePassword = Guid.NewGuid();

            // Act
            var actual = _sut.IsValidPassword(testRequestUpdatePassword, testPendingUpdatePassword);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValidDeposit_InputValid_ReturnsTrue()
        {
            // Arrange
            var testPendingDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            var testRequestDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            // Act
            var actual = _sut.IsValidDeposit(testRequestDeposit, testPendingDeposit);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValidDeposit_InputInvalidNoteCount_ReturnsFalse()
        {
            // Arrange
            var testPendingDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            });

            var testRequestDeposit = new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 50 },
                { 50, 50 }
            });

            // Act
            var actual = _sut.IsValidDeposit(testRequestDeposit, testPendingDeposit);

            // Assert
            Assert.False(actual);
        }
    }
}
