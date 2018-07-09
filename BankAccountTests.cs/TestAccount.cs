using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace BankAccountTests.cs
{

    [TestFixture]
    public class TestAccount
    {
        private Account _account;
        private readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IMiniStatementPrinter> _miniStatementPrinter = new Mock<IMiniStatementPrinter>();

        [SetUp]
        public void Initialise()
        {
            _account = new Account(_transactionRepository.Object, _miniStatementPrinter.Object);
        }

        [Test]
        public void MakeADeposit()
        {
            var transaction = new Transaction
            {
                Money = 100,
                Date = new DateTime(2018, 3, 9, 16, 5, 7, 123)
            };
            _account.Deposit(transaction);

            _transactionRepository.Verify(a => a.Deposit(transaction));
        }

        [Test]
        public void MakeAWithdrawal()
        {
            var transaction = new Transaction
            {
                Money = 100,
                Date = new DateTime(2018, 3, 9, 16, 5, 7, 123)
            };
            _account.Withdrawal(transaction);
            _transactionRepository.Verify(a => a.Withdrawal(transaction));
        }

        [Test]
        public void PrintStatement()
        {
            var list = new List<Transaction> {new Transaction
            {
                Money = 100,
                Date = new DateTime(2018, 3, 9, 16, 5, 7, 123)
            },
                new Transaction
                {
                    Money = 200,
                    Date = new DateTime(2018, 4, 9, 16, 5, 7, 123)
                }
            };
            _transactionRepository.Setup(x => x.GetAllTransactions()).Returns(list);

            _account.PrintStatement();


            _miniStatementPrinter.Verify(a => a.Print(list));
        }
    }

    public interface IMiniStatementPrinter
    {
        void Print(List<Transaction> transactions);
    }

    public class MiniStatementPrinter : IMiniStatementPrinter
    {

        public void Print(List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                Console.WriteLine(transaction.Date + " " + transaction.Money);
            }
        }
    }

    public interface ITransactionRepository
    {
        void Deposit(Transaction transaction);
        void Withdrawal(Transaction transaction);
        List<Transaction> GetAllTransactions();
    }

    public class Transaction
    {
        public int Money { get; set; }
        public DateTime Date { get; set; }
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> transactions = new List<Transaction>();

        public void Deposit(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public void Withdrawal(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public List<Transaction> GetAllTransactions()
        {
            return transactions;
        }
    }

    public class Account
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMiniStatementPrinter _miniStatementPrinter;

        public Account(ITransactionRepository transactionRepository, IMiniStatementPrinter miniStatementPrinter)
        {
            _transactionRepository = transactionRepository;
            _miniStatementPrinter = miniStatementPrinter;
        }

        public void Deposit(Transaction transaction)
        {
            _transactionRepository.Deposit(transaction);
        }

        public void Withdrawal(Transaction transaction)
        {
            _transactionRepository.Withdrawal(transaction);
        }

        public void PrintStatement()
        {
            _miniStatementPrinter.Print(_transactionRepository.GetAllTransactions());
        }
    }

}

