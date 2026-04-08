namespace Phase2Task1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region
            /// First Task: 
            /*
             List<int> numbers = new List<int> { 5, 2, 8, 2, 1, 5, 9, 3, 8 };
            Write code to:
            1.Remove duplicates
            2.Sort the numbers in ascending order
            3.Print only numbers greater than 4
             */

            /*
            List<int> numbers = new List<int> { 5, 2, 8, 2, 1, 5, 9, 3, 8 };
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = i + 1; j < numbers.Count; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        numbers.RemoveAt(j);
                        j--;
                    }
                }
            }
            numbers.Sort();
            var nu = numbers.Where(n => n > 4);
            foreach (var i in nu)
            {
                Console.WriteLine(i);
            }*/
            #endregion

            #region
            /// Second Task:
            /*
             Dictionary<string, int> products = new Dictionary<string, int>
                {
                { "Laptop", 5 },
                { "Mouse", 12 },
                { "Keyboard", 7 }
                };

                Write code to:
                1.Check if “Mouse” exists
                2.Update the quantity of “Laptop” to 10
                Print all products whose quantity is greater than 6
             */

            /*
            Dictionary<string, int> products = new Dictionary<string, int>
            {
                { "Laptop", 5 },
                { "Mouse", 12 },
                { "Keyboard", 7},
                { "Monitor", 4 },
                { "Printer", 9 },
                { "Headphones", 3 }
            };
            foreach (var product in products)
            {
                if (product.Key == "Mouse")
                {
                    Console.WriteLine("Mouse exists");
                }
            }
            Console.WriteLine("================================\n");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Key}: {product.Value}");
            }
            Console.WriteLine("================================\n");
            products["Laptop"] = 10;
            var pro = products.Where(p => p.Value > 6);
            foreach (var p in pro)
            {
                Console.WriteLine($"{p.Key}: {p.Value}");
            }

            */

            #endregion

            #region
            /// Third Task: Linq Querys              
            /*

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Ahmed",
                    Department = "IT",
                    Salary = 12000
                },

                new Employee
                {
                    Id = 2,
                    Name = "Sara",
                    Department = "HR",
                    Salary = 8000
                },

                new Employee
                {
                    Id = 3,
                    Name = "Mona",
                    Department = "IT",
                    Salary = 15000
                },

                new Employee
                {
                    Id = 4,
                    Name = "Omar",
                    Department = "Finance",
                    Salary = 9000
                },

                new Employee
                {
                    Id = 5,
                    Name = "Ali",
                    Department = "IT",
                    Salary = 11000
                }
            };

            // 1. Get all employees in the IT department

            var ITEmployees = employees.Where(e => e.Department == "IT").ToList();
            foreach (var employee in ITEmployees)
            {
                Console.WriteLine($"Id: {employee.Id}, Name: {employee.Name}, Department: {employee.Department}, Salary: {employee.Salary}");
            }
            Console.WriteLine("================================\n");

            // 2. Get employees with salary greater than 10000

            var EmployeesSalary = employees.Where(e => e.Salary > 10000).ToList();
            foreach (var employee in EmployeesSalary)
            {
                Console.WriteLine($"Id: {employee.Id}, Name: {employee.Name}, Department: {employee.Department}, Salary: {employee.Salary}");
            }
            Console.WriteLine("================================\n");

            // 3. Get employee names only

            var EmployeeNames = employees.Select(e => e.Name).ToList();
            foreach (var name in EmployeeNames)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine("================================\n");

            //4.Sort employees by salary descending

            var EmployeeDS = employees.OrderByDescending(e => e.Salary).ToList();
            foreach (var employee in EmployeeDS)
            {
                Console.WriteLine($"Id: {employee.Id}, Name: {employee.Name}, Department: {employee.Department}, Salary: {employee.Salary}");
            }
            Console.WriteLine("================================\n");

            //5.Get the average salary

            var averageSalary = employees.Average(e => e.Salary);
            Console.WriteLine($"Average Salary: {averageSalary}");
            Console.WriteLine("================================\n");

            //6.Group employees by department
            var EmployeeBD = employees.GroupBy(e => e.Department).ToList();   //badal ma agebha 3la elmemory agebha imadatly
            foreach (var group in EmployeeBD)
            {
                Console.WriteLine($"Department: {group.Key}");
                foreach (var employee in group)
                {
                    Console.WriteLine($"Id: {employee.Id}, Name: {employee.Name}, Salary: {employee.Salary}");
                }
                Console.WriteLine("================================\n");
            }

            */
            #endregion

            #region
            /// Fourth Task: Linq filtering and sorting
            /*
             Write a LINQ query to find the second highest number from this list: 
             List<int> numbers = new List<int> { 10, 40, 20, 50, 30, 50 };
                //// 2ablny problem hena w heya eny kan f tekrar 50 fe el list, w 3ayz a3ml sort w a5od tany a3la rkam f hykon el 50 f 3mlt remove lltkrar 
             */

            /*

            List<int> numbers = new List<int> { 10, 40, 20, 50, 30, 50 };
            var secondHighest = numbers.Distinct().OrderByDescending(n => n).Skip(1).FirstOrDefault();
            Console.WriteLine(secondHighest.ToString());

            */
            #endregion

            #region
            /// quistion 10 
            /*
             Write a method to reverse a string without using built-in Reverse(). Example:
             Input: "hello" Output: "olleh"
             */

            /*
            Console.WriteLine("Enter string to reverse:");
            string input = Console.ReadLine();
            string reversedString = reverseing(input);
            Console.WriteLine(reversedString);

            string reverseing(string input)
            {
                string reversed = "";
                for (int i = input.Length - 1; i >= 0; i--)
                {
                    reversed += input[i];
                }
                return reversed;

            }
            */
            #endregion

            #region
            /// quistion 11
            /*
             Write a method to check if a string is a palindrome. Examples:
             "madam" => true
             "hello" => false
             */

            /*
            Console.WriteLine("Enter string to check if  palindrome:");
            string input = Console.ReadLine();

            bool isPalindrome = IsPalindrome(input);
            Console.WriteLine(isPalindrome ? "True" : "False");

            static bool IsPalindrome(string input)
            {
                string reverse = new string(input.Reverse().ToArray());
                return input == reverse;
            }
            */
            #endregion

            #region
            /// quistion 12
            /*
             Write a method to find the most repeated number in an array. Example:
             Input: [1, 2, 3, 2, 4, 2, 5]
             Output: 2
             */

            /*
          
                Console.WriteLine("Enter the Array size: ");
                int size = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter the elements of array: ");
                int[] arr = new int[size];
                for (int i = 0; i < size; i++)
                {
                    arr[i] = int.Parse(Console.ReadLine());
                }

                int Toprepated = repet(arr);
                Console.WriteLine($"most repeted numbers: {Toprepated}");
            

            static int repet(int[] arr)
            {
                Dictionary<int, int> dict = new Dictionary<int, int>();

                foreach (int num in arr)
                {
                    if (dict.ContainsKey(num))
                        dict[num]++;
                    else
                        dict[num] = 1;
                }

                int maxCount = 0;
                int mostRepeated = arr[0];

                foreach (var item in dict)
                {
                    if (item.Value > maxCount)
                    {
                        maxCount = item.Value;
                        mostRepeated = item.Key;
                    }
                }

                return mostRepeated;
            }
        }

            // anuther way using LINQ:
        static int FindRepetedNumber(int[] arr)
        {
            var count = arr.GroupBy(n => n)
                .Select(g => new { Number = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            if (count == null)
                return 0;

            return count.Number;
        

            */
            #endregion

            #region
            /// quistion 13:
            /*
             Write a method to determine if two strings are anagrams. Example:
            "listen" and "silent" => true
            "hello" and "world" => false
             */

            /*
            Console.WriteLine("Enter first string:");
            string str1 = Console.ReadLine();
            Console.WriteLine("Enter second string:");
            string str2 = Console.ReadLine();
            bool Thesame = CommparesString(str1, str2);
            Console.WriteLine(Thesame ? "true" : "False");

            static bool CommparesString(string str1, string str2)
            {
                char[] arr1 = str1.ToCharArray();
                char[] arr2 = str2.ToCharArray();
                Array.Sort(arr1);
                Array.Sort(arr2);
                return arr1.SequenceEqual(arr2);
            }
            */
            #endregion

            #region
            /// quistion 17:
            /*
            Write a method to return the first non-repeated character in a string. Example:
            Input: "swiss" Output: 'w'
             */

            // don't use fnrc to avoid confusion with the method name in the question

            Console.WriteLine("Enter the String: ");
            string input = Console.ReadLine();
            char result = FNRC(input);
            Console.WriteLine(result);

            static char FNRC(string str)
            {
                Dictionary<char, int> dict = new Dictionary<char, int>();

                foreach (char c in str)
                {
                    if (dict.ContainsKey(c))
                        dict[c]++;
                    else
                        dict[c] = 1;
                }

                foreach (char c in str)
                {
                    if (dict[c] == 1)
                        return c;
                }

                return '0';
            }
            #endregion

        }

    }
}


