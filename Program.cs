//Ryan Thurbon 600654

using System;
using System.Collections.Generic;

namespace PRG_Project
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

        public void DisplayDetails()
        {
            Console.WriteLine($"Expense Description:{Description} Amount:{Amount} Date:{Date} Category:{Source}");
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

        public void DisplayDetails()
        {
            Console.WriteLine($"Expense Description:{Description} Amount:{Amount} Date:{Date} Category:{Category}");
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
    public class WithdrawalLimit: AccountLimit
    {
        private double _dailyWithdrawalLimit = 0.00;

        //override the abstarcted class
        public override void SetLimit(double value)
        {
            _dailyWithdrawalLimit = value;
        }

        //override the abstarcted class
        public override void Validate(double value)
        {
            if (value > _dailyWithdrawalLimit)
            {
                TriggerLimitExceeded($"Daily withdrawal amount of R{value} exceeds the daily withdrawal limit of R{_dailyWithdrawalLimit}");
                //Maybe an option to update their limit in the console app...
            }

            else
            {
                //Here we need to let them withdraw...
            }
        }
    }

    //Deriving from AccountLimit...
    public class DepositLimit: AccountLimit
    {
        private double _dailyDepositLimit = 0.00;

        //override the abstarcted class
        public override void SetLimit(double value)
        {
            _dailyDepositLimit = value;
        }

        //override the abstarcted class
        public override void Validate(double value)
        {
            if (value > _dailyDepositLimit)
            {
                TriggerLimitExceeded($"Daily deposit amount of R{value} exceeds the daily deposit limit of R{_dailyDepositLimit}");
                //Maybe an option to update their limit in the console app...
            }

            else
            {
                //Here we need to let them deposit...
            }
        }
    }

    internal class AccountLimitSystem
    {
        private readonly WithdrawalLimit _withdrawalLimit; //readonly since we don't need to change this value (assigned in constructor)
        private readonly DepositLimit _depositLimit; //readonly since we don't need to change this value (assigned in constructor)

        public AccountLimitSystem()
        {
            _withdrawalLimit = new WithdrawalLimit();
            _depositLimit = new DepositLimit();


            //Okay this is the confusing part - We previously created an event that will trigger when a limit is exceeded, called OnLimitExceeded
            //OnLimitExceeded needs to "know" what to do when it is invoked, so we use the "+=" to "subscribe" to the event with a method
            //that will handle the logic of what will happen when a limit is excceeded (in this case just writing to the console lol)
            //In this case, both deposit and withdraw share the same method
            _withdrawalLimit.OnLimitExceeded += HandleLimitExceeded;
            _depositLimit.OnLimitExceeded += HandleLimitExceeded;
        }

        //the event handler method for the logic
        private void HandleLimitExceeded(string message)
        {
            Console.WriteLine($"System Alert: {message}");
        }

        public void SetWithdrawalLimit(double value)
        {
            _withdrawalLimit.SetLimit(value);
        }

        public void SetDepositLimit(double value)
        {
            _depositLimit.SetLimit(value);
        }

        public void ValidateWithdrawal(double amount)
        {
            _withdrawalLimit.Validate(amount);
        }

        public void ValidateDeposit(double amount)
        {
            _depositLimit.Validate(amount);
        }

    }



    class Program
    {
        static void Main()
        {
            // Adding Incomes and Expenses


            Console.WriteLine("----- ADDING INCOMES AND EXPENSES -----");
            Console.WriteLine("Please choose from the options below. Select the number.");
            Console.WriteLine("1. Add an Income");
            Console.WriteLine("2. Add an Expense");

            int IncExp_answer = int.Parse(Console.ReadLine());
            List<Income> incomesList = new List<Income>();
            List<Expense> expensesList = new List<Expense>();

            switch (IncExp_answer)
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
                        Console.WriteLine("Please confirm if expense is correct");
                        Console.WriteLine("1. Confirm");
                        Console.WriteLine("2. Retry");
                        int confirmationAnswer = int.Parse(Console.ReadLine());
                        if (confirmationAnswer == 1)
                        {
                            correctDetails = true;
                            Income income = new Income(amount, source, date, description);
                            incomesList.Add(income);
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
                        Console.WriteLine("Date: (Format: dd-mm-yyyy)");
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
                            expensesList.Add(expense);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Please choose 1 or 2.");
                    break;
            }

            //new class instance
            AccountLimitSystem accountLimitSystem = new AccountLimitSystem();

            //by default, the limit is 0.00, so we set a new limit of whatever we want (this needs to be able to be done in the app itself somehow idk)
            //like an option e.g
            //4. Update Withdraw Limit
            //5. Update Deposit Limit
            //Then we just read the input and call these methods
            accountLimitSystem.SetWithdrawalLimit(500.00);
            accountLimitSystem.SetDepositLimit(500.00);

            //before they can withdraw/deposit, we need to validate that the amounts they want are within the limits they set, otherwise trigger alert
            //logic for successfull validation should be done in this method aswell I think (reduce amount of code we need to write)
            accountLimitSystem.ValidateWithdrawal(600.00);
            accountLimitSystem.ValidateDeposit(600.00);

            //i just added this so I could read the output otherwise it would just close for some reason
            Console.ReadKey();
        }
    }
}