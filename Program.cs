//Ryan Thurbon 600654

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG_271_Project
{
    // Income and expense class implementation

    class Income
    {
        private decimal Amount;
        private IncomeSources Source;
        private DateTime Date;
        private string Description;

        public Income(decimal amount, IncomeSources source, DateTime date, string description)
        {
            Amount = amount;
            Source = source;
            Date = date;
            Description = description;
        }

        public decimal AmountGetter { get => Amount; }
        public DateTime DateGetter { get => Date; }

        public void DisplayDetails()
        {
            Console.WriteLine($"{Date.ToShortDateString(),-12} {Source,-15} {Amount,10:C} {Description,-30}");
        }
    }

    class Expense
    {
        private decimal Amount;
        private ExpenseCategories Category;
        private DateTime Date;
        private string Description;


        public Expense(decimal amount, ExpenseCategories category, DateTime date, string description)
        {
            Amount = amount;
            Category = category;
            Date = date;
            Description = description;
        }

        public decimal AmountGetter { get => Amount; }
        public DateTime DateGetter { get => Date; }

        public void DisplayDetails()
        {
            Console.WriteLine($"{Date.ToShortDateString(),-12} {Category,-15} {Amount,10:C} {Description,-30}");
        }
    }

    enum ExpenseCategories
    {
        Food = 1,
        Transport,
        Alcohol,
        Clothes,
        Activities,
        Other
    }

    enum IncomeSources
    {
        Salary,
        PartTime_Work,
        Gift,
        Investment_Income,
        Other
    }


    //it's abstract so that common methods can be shared
    public abstract class AccountLimit
    {
        //Delegate is used for custom event handling when whatever limit we have is exceeded
        public delegate void LimitExceededEventHandler(string message);
        //This is the event that will be triggered when a limit is exceeded
        public event LimitExceededEventHandler OnLimitExceeded;

        //abstract so that any classes being derived from AccountLimit know they NEED to implement these methods
        public abstract void SetLimit(double value);
        public abstract void Validate(double value);

        //protected because we don't want classes outside of this scope to access this method - only derived classes.
        protected void TriggerLimitExceeded(string message)
        {
            //some research here
            //events work on a "subscribtion" basis - the "?" means that IF there are any "subscribers", invoke the method with a message as param
            OnLimitExceeded?.Invoke(message);
        }
    }

    //Deriving from AccountLimit...
    public class IncomeLimit : AccountLimit
    {
        private double _dailyIncomeLimit = 0.00;

        //override the abstarcted class
        public override void SetLimit(double value)
        {
            _dailyIncomeLimit = value;
        }

        //override the abstarcted class
        public override void Validate(double value)
        {
            if (value > _dailyIncomeLimit)
            {
                TriggerLimitExceeded($"Daily income amount of R{value} exceeds the daily income limit of R{_dailyIncomeLimit}");
                return;
            }
        }
    }

    public class ExpenseLimit : AccountLimit
    {
        private double _dailyExpenseLimit = 0.00;

        //override the abstarcted class
        public override void SetLimit(double value)
        {
            _dailyExpenseLimit = value;
        }

        //override the abstarcted class
        public override void Validate(double value)
        {
            if (value > _dailyExpenseLimit)
            {
                TriggerLimitExceeded($"Daily expense amount of R{value} exceeds the daily expense limit of R{_dailyExpenseLimit}");
                return;
            }
        }
    }

    internal class AccountLimitSystem
    {
        private readonly IncomeLimit _incomeLimit; //readonly since we don't need to change this value (assigned in constructor)
        private readonly ExpenseLimit _expenseLimit; //readonly since we don't need to change this value (assigned in constructor)

        public AccountLimitSystem()
        {
            _incomeLimit = new IncomeLimit();
            _expenseLimit = new ExpenseLimit();

            //Okay this is the confusing part - We previously created an event that will trigger when a limit is exceeded, called OnLimitExceeded
            //OnLimitExceeded needs to "know" what to do when it is invoked, so we use the "+=" to "subscribe" to the event with a method
            //that will handle the logic of what will happen when a limit is excceeded (in this case just writing to the console lol)
            //In this case, both income and expense share the same method
            _incomeLimit.OnLimitExceeded += HandleLimitExceeded;
            _expenseLimit.OnLimitExceeded += HandleLimitExceeded;
        }

        private void HandleLimitExceeded(string message)
        {
            Console.WriteLine($"System Alert: {message}");
        }

        public void SetIncomeLimit(double value)
        {
            _incomeLimit.SetLimit(value);
        }

        public void SetExpenseLimit(double value)
        {
            _expenseLimit.SetLimit(value);
        }

        public void ValidateIncome(double amount)
        {
            _incomeLimit.Validate(amount);
        }

        public void ValidateExpense(double amount)
        {
            _expenseLimit.Validate(amount);
        }
    }

    //Interface for Reportable Items
    internal interface IReportable
    {
        void GenerateReport();
        void DisplayReport();
    }

    //Class: FinancialReport
    internal class FinancialReport : IReportable
    {
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public List<Income> Incomes { get; set; } = new List<Income>();

        // Generates a report
        public void GenerateReport()
        {
            if (!Expenses.Any() && !Incomes.Any())
            {
                return;
            }

            Console.WriteLine("\nGenerating financial report...");
            Thread.Sleep(2000); // Simulate long operation

            DateTime startDate = GetStartDate();
            DateTime endDate = GetEndDate();
            Console.WriteLine($"Financial report generated for period: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}");
        }

        // Displays the generated report
        public void DisplayReport()
        {
            if (!Expenses.Any() && !Incomes.Any())
            {
                Console.WriteLine("\nNo transactions available to generate a report.");
                return;
            }

            Console.WriteLine("\n--- Financial Report ---\n");

            Console.WriteLine("Income:");
            Console.WriteLine($"{"Date",-12} {"Source",-15} {"Amount",10} {"Description",-30}");
            Console.WriteLine(new string('-', 70));
            foreach (var income in Incomes)
            {
                income.DisplayDetails();
            }

            Console.WriteLine("\nExpenses:");
            Console.WriteLine($"{"Date",-12} {"Category",-15} {"Amount",10} {"Description",-30}");
            Console.WriteLine(new string('-', 70));
            foreach (var expense in Expenses)
            {
                expense.DisplayDetails();
            }

            decimal totalIncome = Incomes.Sum(i => i.AmountGetter);
            decimal totalExpenses = Expenses.Sum(e => e.AmountGetter);

            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"{"Total Income:",-27} {totalIncome,10:C}");
            Console.WriteLine($"{"Total Expenses:",-27} {totalExpenses,10:C}");
            Console.WriteLine($"{"Net Savings:",-27} {(totalIncome - totalExpenses),10:C}");
            Console.WriteLine(new string('-', 70));
        }

        private DateTime GetStartDate()
        {
            var firstExpenseDate = Expenses.Any() ? Expenses.Min(e => e.DateGetter) : DateTime.MaxValue;
            var firstIncomeDate = Incomes.Any() ? Incomes.Min(i => i.DateGetter) : DateTime.MaxValue;
            return firstExpenseDate < firstIncomeDate ? firstExpenseDate : firstIncomeDate;
        }

        private DateTime GetEndDate()
        {
            var lastExpenseDate = Expenses.Any() ? Expenses.Max(e => e.DateGetter) : DateTime.MinValue;
            var lastIncomeDate = Incomes.Any() ? Incomes.Max(i => i.DateGetter) : DateTime.MinValue;
            return lastExpenseDate > lastIncomeDate ? lastExpenseDate : lastIncomeDate;
        }
    }

    class Program
    {
        static void Main()
        {
            // Login and authentication code
            List<UserLoginDetails> users = new List<UserLoginDetails>();
            users.Add(new UserLoginDetails("600658", 1234, "Jamie", "Palmer", 3000));
            users.Add(new UserLoginDetails("600871", 1234, "Arnoldus Christiaan", "Vlok", 3000));
            users.Add(new UserLoginDetails("600994", 1234, "Monwabisi", "Mashiane", 3000));
            users.Add(new UserLoginDetails("600654", 1234, "Ryan", "Thurbon", 3000));

            Console.WriteLine("Welcome...");
            Console.WriteLine("Please insert User ID");
            string idNumberCheck;
            UserLoginDetails currentUser = null;

            while (true)
            {
                idNumberCheck = Console.ReadLine();
                currentUser = users.Find(user => user.GetUserId() == idNumberCheck);
                if (currentUser != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("ID not found");
                }
            }

            Console.WriteLine("Please enter your PIN");
            int userPin;

            while (true)
            {
                userPin = int.Parse(Console.ReadLine());
                if (currentUser.GetPin() == userPin)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect PIN. Please try again");
                }
            }

            Console.WriteLine("Welcome " + currentUser.GetFirstName() + "! What do you want to do today?");
            FinancialReport report = new FinancialReport();
            AccountLimitSystem accountLimitSystem = new AccountLimitSystem();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("----- Personal Finance Manager -----");
                Console.WriteLine("Please choose from the options below (Select the number):");
                Console.WriteLine("1. Add an Income");
                Console.WriteLine("2. Add an Expense");
                Console.WriteLine("3. Generate Financial Report");
                Console.WriteLine("4. Set Income Limit");
                Console.WriteLine("5. Set Expense Limit");
                Console.WriteLine("6. Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        bool correctDetails = false;
                        while (correctDetails == false)
                        {
                            Console.WriteLine("Description: ");
                            string description = Console.ReadLine();
                            Console.WriteLine("Amount: ");
                            decimal amount = decimal.Parse(Console.ReadLine());
                            Console.WriteLine("Date: (Format: dd-mm-yyyy)");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            Console.WriteLine("Please pick the income source: ");
                            foreach (IncomeSources incomeItem in Enum.GetValues(typeof(IncomeSources)))
                            {
                                Console.WriteLine($"{(int)incomeItem}. {incomeItem.ToString()}");
                            }
                            int sourceChoice = int.Parse(Console.ReadLine());
                            IncomeSources source = (IncomeSources)sourceChoice;
                            Console.WriteLine("Please confirm if income is correct");
                            Console.WriteLine("1. Confirm");
                            Console.WriteLine("2. Retry");
                            int confirmationAnswer = int.Parse(Console.ReadLine());
                            if (confirmationAnswer == 1)
                            {
                                correctDetails = true;
                                Income income = new Income(amount, source, date, description);
                                report.Incomes.Add(income);
                                accountLimitSystem.ValidateIncome((double)amount);
                            }
                        }
                        break;

                    case 2:
                        correctDetails = false;
                        while (correctDetails == false)
                        {
                            Console.WriteLine("Description: ");
                            string description = Console.ReadLine();
                            Console.WriteLine("Amount: ");
                            decimal amount = decimal.Parse(Console.ReadLine());
                            Console.WriteLine("Date: (Format: dd-mm-yy)");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            Console.WriteLine("Please pick the category the expense belongs to: ");
                            foreach (ExpenseCategories categoryItem in Enum.GetValues(typeof(ExpenseCategories)))
                            {
                                Console.WriteLine($"{(int)categoryItem}. {categoryItem.ToString()}");
                            }
                            int categoryChoice = int.Parse(Console.ReadLine());
                            ExpenseCategories category = (ExpenseCategories)categoryChoice;
                            Console.WriteLine("Please confirm if expense is correct");
                            Console.WriteLine("1. Confirm");
                            Console.WriteLine("2. Retry");
                            int confirmationAnswer = int.Parse(Console.ReadLine());
                            if (confirmationAnswer == 1)
                            {
                                correctDetails = true;
                                Expense expense = new Expense(amount, category, date, description);
                                report.Expenses.Add(expense);
                                accountLimitSystem.ValidateExpense((double)amount);
                            }
                        }
                        break;

                    case 3:
                        correctDetails = false;
                        while (correctDetails == false)
                        {
                            correctDetails = true;
                            // Custom Thread to Generate Financial Report
                            Thread reportThread = new Thread(new ThreadStart(report.GenerateReport));
                            reportThread.Start();
                            reportThread.Join(); // Ensure the thread completes before proceeding

                            // Display the Report
                            report.DisplayReport();
                        }
                        break;

                    case 4:
                        Console.WriteLine("Enter daily income limit: ");
                        double incomeLimit = double.Parse(Console.ReadLine());
                        accountLimitSystem.SetIncomeLimit(incomeLimit);
                        break;

                    case 5:
                        Console.WriteLine("Enter daily expense limit: ");
                        double expenseLimit = double.Parse(Console.ReadLine());
                        accountLimitSystem.SetExpenseLimit(expenseLimit);
                        break;

                    case 6:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Please choose a valid option.");
                        break;
                }
            }
        }
    // User Login Details class implementation
    public class UserLoginDetails
        {
            private string UserId;
            private int Pin;
            private string FirstName;
            private string LastName;
            private double Budget;

            public UserLoginDetails(string userId, int pin, string firstName, string lastName, double budget)
            {
                UserId = userId;
                Pin = pin;
                FirstName = firstName;
                LastName = lastName;
                Budget = budget;
            }

            public string GetUserId()
            {
                return UserId;
            }

            public int GetPin()
            {
                return Pin;
            }

            public string GetFirstName()
            {
                return FirstName;
            }

            public string GetLastName()
            {
                return LastName;
            }

            public double GetBudget()
            {
                return Budget;
            }
        }
    }
}
