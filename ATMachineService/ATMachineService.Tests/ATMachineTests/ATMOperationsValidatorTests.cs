namespace ATMachineService.Tests.ATMachineTests
{
    public class ATMOperationsValidatorTests
    {
        private readonly IATMOperationsValidator _sut;

        public ATMOperationsValidatorTests()
        {
            _sut = new ATMOperationsValidator();
        }

        [Theory]
        [InlineData]
        [InlineData]
        [InlineData]
        public void IsValidCard_InputValid_ReturnsTrue()
        {
            // Arrange
            var testNumber = new Random().Next(32, 33).ToString();
            var testCustomerName = "Cale Makar";
            var testCard = new Card(testNumber, testCustomerName);

            // Act
            var actual = _sut.IsValidCard(testCard);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValidCard_InputInvalidCardNumber_ReturnsFalse()
        {
            // Arrange
            var testNumber = "    ";
            var testCustomerName = "Cale Makar";
            var testCard = new Card(testNumber, testCustomerName);

            // Act
            var actual = _sut.IsValidCard(testCard);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValidCard_InputInvalidName_ReturnsFalse()
        {
            // Arrange
            var testNumber = new Random().Next(32, 33).ToString();
            var testCustomerName = "  ";
            var testCard = new Card(testNumber, testCustomerName);

            // Act
            var actual = _sut.IsValidCard(testCard);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(10, 100)]
        [InlineData(50, 500)]
        [InlineData(500, 5000)]
        public void IsMachineBalanceSufficient_InputValid_ReturnsTrue(int request, int balance)
        {
            // Act
            var actual = _sut.IsMachineBalanceSufficient(request, balance);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(100, 150)]
        [InlineData(50, 75)]
        [InlineData(500, 750)]
        public void IsMachineBalanceSufficient_InputInvalid_ReturnsFalse(int request, int balance)
        {
            // Act
            var actual = _sut.IsMachineBalanceSufficient(request, balance);

            // Assert
            Assert.False(actual);
        }
    }
}