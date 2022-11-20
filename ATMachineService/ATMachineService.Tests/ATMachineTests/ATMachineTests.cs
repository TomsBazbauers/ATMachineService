namespace ATMachineService.Tests.ATMachineTests
{
    public class ATMachineTests
    {
        private readonly IATMachine _sut;
        private readonly Mock<IATMOperationsValidator> _ATMOperationsValidatorMock;
        private readonly Mock<IFinancialService> _financialServiceMock;

        public ATMachineTests()
        {
            var testSerial = "187911";
            var testNotes = new Dictionary<int, int>()
            {
                { 5, 200 },
                { 10, 200 },
                { 20, 200 },
                { 50, 200 }
            };

            _financialServiceMock = new Mock<IFinancialService>();
            _ATMOperationsValidatorMock = new Mock<IATMOperationsValidator>();
            _financialServiceMock.Object.ChargedFees = new List<Fee>() { new Fee("90901002", 1000, 11.55m) };

            _sut = new ATMachine(testSerial, _financialServiceMock.Object, _ATMOperationsValidatorMock.Object);
            _sut.CurrentMachineBalance = new Money(testNotes);
        }

        [Fact]
        public void LoadMoney_InputValid_BalanceUpdated()
        {
            // Arrange
            var testNotes = new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            };
            var testBalance = new Money(testNotes);
            var testUpdate = new ATMStatusUpdate("187911", testBalance, Guid.NewGuid(), DateTime.Today);

            _financialServiceMock
                .Setup(m => m.LoadMachine(testUpdate))
                .Returns(true);

            // Act
            _sut.LoadMoney(testUpdate);

            // Assert
            Assert.Equal(testBalance.Amount, _sut.CurrentMachineBalance.Amount);
        }

        [Fact]
        public void LoadMoney_InputInvalid_ThrowsExceptionBalanceNotUpdated()
        {
            // Arrange
            var testNotes = new Dictionary<int, int>()
            {
                { 5, 100 },
                { 10, 100 },
                { 20, 100 },
                { 50, 100 }
            };
            var testBalance = new Money(testNotes);
            var testUpdate = new ATMStatusUpdate("187911", testBalance, Guid.NewGuid(), DateTime.Today);

            _financialServiceMock
                .Setup(m => m.LoadMachine(testUpdate))
                .Returns(false);

            // Assert
            Assert.Throws<InvalidStatusUpdateException>(() => _sut.LoadMoney(testUpdate));
            Assert.NotEqual(testBalance, _sut.CurrentMachineBalance);
        }

        [Fact]
        public void InsertCard_InputValidWithdrawalsRunning_CardSet()
        {
            // Arrange
            var testCard = new Card("90901001", "Mitchell Marner");

            _ATMOperationsValidatorMock
                 .Setup(m => m.IsValidCard(testCard))
                 .Returns(true);

            _financialServiceMock
                .Setup(m => m.IsValidCard(testCard, 1622))
                .Returns(true);

            // Act
            _sut.InsertCard(testCard, 1622);

            // Assert
            Assert.Same(testCard, _sut.CurrentCard);
        }

        [Fact]
        public void InsertCard_InputValidWithdrawalsNotRunning_CardNotSetThrowsException()
        {
            // Arrange
            var testCard = new Card("90901001", "Mitchell Marner");
            _sut.CurrentMachineBalance = new Money(new Dictionary<int, int>()
            {
                { 5, 1 },
                { 10, 1 },
                { 20, 9 },
                { 50, 9 }
            });

            // Assert
            Assert.Throws<WithdrawalsNotAvailableException>(() => _sut.InsertCard(testCard, 1622));
            Assert.Null(_sut.CurrentCard.CardNumber);
        }

        [Fact]
        public void InsertCard_InputInvalidCardInfo_CardNotSetThrowsException()
        {
            // Arrange
            var testCard = new Card("", "Mitchell Marner");

            _ATMOperationsValidatorMock
                 .Setup(m => m.IsValidCard(testCard))
                 .Returns(false);

            _financialServiceMock
                .Setup(m => m.IsValidCard(testCard, 1622))
                .Returns(true);

            // Assert
            Assert.Throws<InvalidCardException>(() => _sut.InsertCard(testCard, 1622));
            Assert.Null(_sut.CurrentCard.CardNumber);
        }

        [Fact]
        public void InsertCard_InputInvalidPin_CardNotSetThrowsException()
        {
            // Arrange
            var testCard = new Card("902938", "Mitchell Marner");

            _ATMOperationsValidatorMock
                 .Setup(m => m.IsValidCard(testCard))
                 .Returns(true);

            _financialServiceMock
                .Setup(m => m.IsValidCard(testCard, 1))
                .Returns(false);

            // Assert
            Assert.Throws<InvalidCardException>(() => _sut.InsertCard(testCard, 1622));
            Assert.Null(_sut.CurrentCard.CardNumber);
        }

        [Fact]
        public void ReturnCard_InputValid_CurrentCardSetEmpty()
        {
            // Arrange
            var testCard = new Card("90901001", "Mitchell Marner");
            _sut.CurrentCard = testCard;

            // Act
            _sut.ReturnCard();

            // Assert
            Assert.Null(_sut.CurrentCard.CardNumber);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(11.55)]
        [InlineData(1.55)]
        public void GetCardBalance_InputValid_ReturnsAccountBalance(decimal testBalance)
        {
            // Arrange
            var testCard = new Card("90901001", "Mitchell Marner");
            _sut.CurrentCard = testCard;

            _financialServiceMock
                .Setup(m => m.GetCardBalance(testCard))
                .Returns(testBalance);

            // Act
            var actual = _sut.GetCardBalance();

            // Assert
            Assert.Equal(testBalance, actual);
        }

        [Theory]
        [InlineData(1000, 11.55)]
        [InlineData(100, 1.55)]
        [InlineData(50, 0.55)]
        public void RetreiveChargedFees_InputValid_RetreivesFees(int testAmount, decimal testFee)
        {
            // Arrange
            var testCard = new Card("90901001", "Mitchell Marner");
            var testChargedFee = new Fee("90901001", testAmount, testFee);
            _sut.CurrentCard = testCard;

            _financialServiceMock
                .Setup(m => m.GetFees(testCard))
                .Returns(new List<Fee>() { testChargedFee });

            // Act
            var actual = _sut.RetrieveChargedFees();

            // Assert
            Assert.Single(actual);
            Assert.IsType<List<Fee>>(actual);
            Assert.Equal(testCard.CardNumber, actual.First().CardNumber);
            Assert.DoesNotContain(actual, fee => fee.CardNumber != testCard.CardNumber);
            Assert.Contains(actual, fee => fee.WithdrawalAmount == testAmount && fee.WithdrawalFee == testFee);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(60)]
        [InlineData(125)]
        [InlineData(555)]
        public void WithdrawMoney_InputValid_ReturnsCorrectMoneyBalances(int testAmount)
        {
            // Arrange
            var currentMachineBalance = _sut.CurrentMachineBalance.Amount;
            var testCard = new Card("90901001", "Cale Makar");
            var testAccount = new Account(testCard, 1000m, 1622);
            var currentAccountBalance = testAccount.Balance;

            _sut.CurrentCard = testCard;

            _ATMOperationsValidatorMock
                .Setup(m => m.IsMachineBalanceSufficient(testAmount, _sut.CurrentMachineBalance.Amount))
                .Returns(true);

            _financialServiceMock
                .Setup(m => m.WithdrawMoney(testCard.CardNumber, testAmount)).Returns(true);

            // Act
            var actual = _sut.WithdrawMoney(testAmount);

            // Assert
            Assert.Equal(testAmount, actual.Amount);
            Assert.Equal(currentMachineBalance - testAmount, _sut.CurrentMachineBalance.Amount);
        }

        [Fact]
        public void WithdrawMoney_InputInvalidRequestAboveMachineBalance_ThrowsException()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");
            var testAmount = 100;

            _sut.CurrentCard = testCard;

            _ATMOperationsValidatorMock
                .Setup(m => m.IsMachineBalanceSufficient(testAmount, _sut.CurrentMachineBalance.Amount))
                .Returns(false);

            _financialServiceMock
                .Setup(m => m.WithdrawMoney(testCard.CardNumber, testAmount)).Returns(true);

            // Assert
            Assert.Throws<InsufficientATMBalanceException>(() => _sut.WithdrawMoney(100));
        }

        [Fact]
        public void WithdrawMoney_InputInvalidRequestAboveAccountBalance_ThrowsException()
        {
            // Arrange
            var testCard = new Card("90901001", "Cale Makar");
            var testAmount = 100;

            _sut.CurrentCard = testCard;

            _ATMOperationsValidatorMock
                .Setup(m => m.IsMachineBalanceSufficient(testAmount, _sut.CurrentMachineBalance.Amount))
                .Returns(true);

            _financialServiceMock
                .Setup(m => m.WithdrawMoney(testCard.CardNumber, testAmount)).Returns(false);

            // Assert
            Assert.Throws<InsufficientAccountBalanceException>(() => _sut.WithdrawMoney(testAmount));
            Assert.NotEqual(_sut.CurrentMachineBalance.Amount - testAmount, _sut.CurrentMachineBalance.Amount);
        }

        [Fact]
        public void RunWithdrawal_InputValidSmall_ReturnsCorrectAmount()
        {
            // Act
            var actual = _sut.RunWithdrawal(65);

            // Assert
            Assert.Equal(1, actual.Notes[5]);
            Assert.Equal(1, actual.Notes[10]);
            Assert.Equal(1, actual.Notes[50]);
        }

        [Fact]
        public void RunWithdrawal_InputValidLarge_ReturnsCorrectAmount()
        {
            // Act
            var actual = _sut.RunWithdrawal(265);

            // Assert
            Assert.Equal(1, actual.Notes[5]);
            Assert.Equal(1, actual.Notes[10]);
            Assert.Equal(5, actual.Notes[50]);
        }
    }
}