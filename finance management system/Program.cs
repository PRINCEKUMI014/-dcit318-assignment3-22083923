using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // Core model: immutable record for transactions
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Processor interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Concrete processors with distinct behavior
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Transferring {transaction.Amount:C} for '{transaction.Category}' (Id: {transaction.Id}).");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Sending {transaction.Amount:C} for '{transaction.Category}' (Id: {transaction.Id}) via mobile money.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Broadcasting {transaction.Amount:C} for '{transaction.Category}' (Id: {transaction.Id}) to the blockchain.");
        }
    }

    // Base account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Balance = initialBalance;
        }

        // Default behavior: deduct amount
        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            Balance -= transaction.Amount;
        }
    }

    // Sealed specialized account
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            base.ApplyTransaction(transaction);
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }

    // Application orchestrator
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Instantiate a SavingsAccount
            var account = new SavingsAccount("SA-1001", 1000m);

            // ii. Create three transactions
            var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 950.00m, "Entertainment");

            // iii. Processors
            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            // Process each transaction (prints processor-specific messages)
            p1.Process(t1);
            account.ApplyTransaction(t1);

            p2.Process(t2);
            account.ApplyTransaction(t2);

            p3.Process(t3);
            account.ApplyTransaction(t3);

            // v. Add all transactions to _transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            // Summary
            Console.WriteLine("\nAll transactions recorded:");
            foreach (var t in _transactions)
            {
                Console.WriteLine($"Id:{t.Id} Date:{t.Date} Amount:{t.Amount:C} Category:{t.Category}");
            }

            Console.WriteLine($"\nFinal account balance for {account.AccountNumber}: {account.Balance:C}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
