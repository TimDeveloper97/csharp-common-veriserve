using System.Dynamic;

namespace DatabaseFlex
{

    internal class Program
    {
        static void Main(string[] args)
        {
            dynamic myClass1 = new ExpandoObject();
            var lShop = new List<string>
            {
                "Shop1", "Shop2"
            };


            myClass1.Name = "A1";
            //myClass1.NameProperties = new List<string>
            //{
            //    "Shop1",
            //    "Shop2"
            //};

            foreach (var shop in lShop)
            {
                var books = new Dictionary<string, string>();

                if (shop == "Shop1")
                {
                    books.Add("book1", "12");
                    books.Add("book2", "15");
                    books.Add("book3", "0");
                }
                else
                {
                    books.Add("book1", "3");
                    books.Add("book2", "4");
                    books.Add("book3", "7");
                }

                dynamic myBook = new ExpandoObject();
                //myBook.NameProperties = new List<string>();

                foreach (var book in books)
                {
                    //myBook.
                    ((IDictionary<string, object>)myBook)[book.Key] = book.Value;
                }

                 ((IDictionary<string, object>)myClass1)[shop] = myBook;
            }

            var value = GetNestedPropertyValue(myClass1, "Shop1.book1");

            Console.WriteLine($"Class Name: {myClass1.Name}");
            foreach (var prop in myClass1)
            {
                Console.WriteLine("Properties: " + prop);
            }


            Console.WriteLine("Hello, World!");
        }

        static object GetNestedPropertyValue(dynamic obj, string propertyPath)
        {
            var properties = propertyPath.Split('.');
            object currentObject = obj;

            foreach (var property in properties)
            {
                if (currentObject is ExpandoObject)
                {
                    var dictionary = (IDictionary<string, object>)currentObject;

                    if (dictionary.ContainsKey(property))
                    {
                        currentObject = dictionary[property];
                    }
                    else
                        throw new Exception($"Property '{property}' not found.");
                }
                else
                    throw new Exception($"Property '{property}' is not an ExpandoObject.");
            }

            return currentObject;
        }
    }
}