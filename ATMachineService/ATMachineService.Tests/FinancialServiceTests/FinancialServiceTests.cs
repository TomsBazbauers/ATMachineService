namespace ATMachineService.Tests.FinancialServiceTests
{
    public class FinancialServiceTests
    {
        private readonly FinancialService _sut;
        private readonly Mock<IFinancialOperationsValidator> _financialOperationsValidatorMock;

        private readonly Guid _testPassword;
        private readonly ATMStatusUpdate _testPendingUpdate;

        public object PendingStatusUpdates { get; private set; }

        public FinancialServiceTests()
        {
            var registeredAccounts = new List<Account>()
            {
                new Account(new Card("90901001", "Cale Makar"), 1000m, 8222),
                new Account(new Card("90901002", "Auston Matthews"), 2000m, 3422),
                new Account(new Card("90901003", "Connor McDavid"), 3000m, 9722)
            };

            _testPassword = Guid.NewGuid();
            _testPendingUpdate = new ATMStatusUpdate("187187", new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            }), _testPassword, DateTime.Today);

            _financialOperationsValidatorMock = new Mock<IFinancialOperationsValidator>();
            _sut = new FinancialService(registeredAccounts, _financialOperationsValidatorMock.Object)
            {
                PendingStatusUpdates = new List<ATMStatusUpdate>() { _testPendingUpdate }
            };

            _sut.ChargedFees = new List<Fee>();
        }

        [Fact]
        public void LoadMachine_InputValid_RemovesCorrespondingUpdate()
        {
            // Arrange
            var testRequestUpdate = new ATMStatusUpdate("187187", new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            }), _testPassword, DateTime.Today);

            _financialOperationsValidatorMock
                .Setup(m => m.IsValidUpdate(testRequestUpdate, _sut.PendingStatusUpdates[0]))
                .Returns(true);

            // Act
            var actual = _sut.LoadMachine(testRequestUpdate);

            // Assert
            Assert.True(actual);
            Assert.Empty(_sut.PendingStatusUpdates);
        }

        [Fact]
        public void LoadMachine_InputInvalidDeposit_ThrowsException()
        {
            // Arrange
            var testRequestUpdate = new ATMStatusUpdate("187187", new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 500 },
                { 50, 100 }
            }), _testPassword, DateTime.Today);

            _financialOperationsValidatorMock
                .Setup(m => m.IsValidUpdate(testRequestUpdate, _sut.PendingStatusUpdates[0]))
                .Returns(false);

            // Assert
            Assert.Throws<InvalidStatusUpdateException>(() => _sut.LoadMachine(testRequestUpdate));
            Assert.Single(_sut.PendingStatusUpdates);
        }

        [Fact]
        public void LoadMachine_InputInvalidPassword_ThrowsException()
        {
            // Arrange
            var testRequestUpdate = new ATMStatusUpdate("187187", new Money(new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            }), Guid.NewGuid(), DateTime.Today);

            _financialOperationsValidatorMock
                .Setup(m => m.IsValidUpdate(testRequestUpdate, _sut.PendingStatusUpdates[0]))
                .Returns(false);

            // Assert
            Assert.Throws<InvalidStatusUpdateException>(() => _sut.LoadMachine(testRequestUpdate));
            Assert.Single(_sut.PendingStatusUpdates);
        }

        [Fact]
        public void IsValidCard_InputValid_ReturnsTrue()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");
            var testPIN = 8222;

            // Act
            var actual = _sut.IsValidCard(testCard, testPIN);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValidCard_InputInvalidAccount_ReturnsFalse()
        {
            // Arrange
            var testCard = new Card("909010001", "Mitchell Marner");
            var testPIN = 8222;

            // Act
            var actual = _sut.IsValidCard(testCard, testPIN);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValidCard_InputInvalidPIN_ReturnsFalse()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");
            var testPIN = 8221;

            // Act
            var actual = _sut.IsValidCard(testCard, testPIN);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void GetCardBalance_InputValid_ReturnsCorrectBalance()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");

            // Act
            var actual = _sut.GetCardBalance(testCard);

            // Assert
            Assert.Equal(1000, actual);
        }

        [Fact]
        public void GetFees_InputValid_ReturnsCorrectBalance()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");
            var testFee = new Fee("90901001", 1000, 10.5m);
            var testFeeDummy = new Fee("90901002", 1000, 10.5m);

            _sut.ChargedFees.Add(testFee);
            _sut.ChargedFees.Add(testFeeDummy);

            // Act
            var actual = _sut.GetFees(testCard);

            // Assert
            Assert.IsType<List<Fee>>(actual);
            Assert.Same(actual.First(), _sut.ChargedFees.First());
            Assert.Single(actual);
        }

        [Fact]
        public void WithdrawMoney_InputValid_DecreasesAccountBalanceReturnsTrue()
        {
            // Arrange
            var testCard = new Card("90901003", "Connor McDavid");
            var testAmount = 55;
            var expectedResult = 3000 - (testAmount + testAmount * 0.01m);

            _sut.CommissionRate = 0.01m;

            // Act
            var actual = _sut.WithdrawMoney(testCard.CardNumber, testAmount);

            // Assert
            Assert.True(actual);
            Assert.Equal(_sut.GetCardBalance(testCard), expectedResult);
            Assert.Contains(_sut.ChargedFees,
                fee => fee.WithdrawalAmount == testAmount && fee.CardNumber == testCard.CardNumber);
        }

        [Fact]
        public void WithdrawMoney_InputInvalidRequest_DoesNotAlterBalanceReturnsFalse()
        {
            // Arrange
            var testCard = new Card("90901003", "Connor McDavid");
            var testAmount = 3001;

            _sut.CommissionRate = 0.01m;

            // Act
            var actual = _sut.WithdrawMoney(testCard.CardNumber, testAmount);

            // Assert
            Assert.False(actual);
            Assert.Equal(3000m, _sut.GetCardBalance(testCard));
            Assert.DoesNotContain(_sut.ChargedFees,
                fee => fee.WithdrawalAmount == 10 && fee.CardNumber == testCard.CardNumber);
        }
    }
}