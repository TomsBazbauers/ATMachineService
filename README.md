# ATMachineService
An ATM application - update machine status, insert/return card and get cash/account balance/fee history.

### Description:

- Built on NET 6.0
- Unit tested via *xUnit and Moq*

---

### Requirements:

- Client can withdraw cash only if card is inserted
- Machine allows to withdraw: *5*, *10*, *20*, *50* euro paper notes
- Charge the clients card withdrawal fee **1%**
- Keep track of all the money that was charged to the client
- Should be possible for the client to check his balance
- Fill ATM machine with cash (operator only)

---
- Use TDD approach
- Think about OOP design patterns and S.O.L.I.D. principles
- In case of error throw exception of different type for each situation
- No need for UI

### ATM Interface

``` 
public interface IATMachine
{
    ///<summary> ATM manufacturer.</summary>
    string Manufacturer { get; }

    ///<summary> ATM serial number.</summary>
    string SerialNumber { get; }

    ///<summary>Insert bank card into ATM mashine.</summary>
    /// <param name="cardNumber">Card number.</param>
    void InsertCard(string cardNumber);

    ///<summary> Retrieves the balance availible on the card.</summary>
    decimal GetCardBalance(); 

    ///<summary>Withdraw money from ATM.</summary>
    /// <param name="amount">Amount of money to withdraw.</param>
    Money WithdrawMoney(int amount);

    ///<summary>Returns card back to client.</summary>
    void ReturnCard();

    ///<summary>Load the money into ATM machine.</summary>
    /// <param name="money">Money loaded into ATM machine.</param>
    void LoadMoney(Money money);

    ///<summary>Load the money into ATM machine.</summary>
    /// <param name="money">Money loaded into ATM machine.</param>
    void LoadMoney(Money money);

    ///<summary>Retrieves charged fees.</summary>
    /// <returns>List of charged fees.</returns>
    IEnumerable<fee> RetrieveChargedFees()
}

public struct Money
{
    public int Amount { get; set; }
    public Dictionary<PaperNote, int> Notes { get; set; }
}

public struct Fee
{
    public struct CardNumber { get; set; }
    public decimal WithdrawalFeeAmount { get; set; }
    public DateTime WithdrawalDate { get; set; }
}
```
