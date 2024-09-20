using System.Data;
using System.Dynamic;

namespace DatabaseFlex
{

    internal class Program
    {
        static void Main(string[] args)
        {
            //dynamic myClass1 = new ExpandoObject();
            //var lShop = new List<string>
            //{
            //    "Shop1", "Shop2"
            //};


            //myClass1.Name = "A1";
            ////myClass1.NameProperties = new List<string>
            ////{
            ////    "Shop1",
            ////    "Shop2"
            ////};

            //foreach (var shop in lShop)
            //{
            //    var books = new Dictionary<string, string>();

            //    if (shop == "Shop1")
            //    {
            //        books.Add("book1", "12");
            //        books.Add("book2", "15");
            //        books.Add("book3", "0");
            //    }
            //    else
            //    {
            //        books.Add("book1", "3");
            //        books.Add("book2", "4");
            //        books.Add("book3", "7");
            //    }

            //    dynamic myBook = new ExpandoObject();
            //    //myBook.NameProperties = new List<string>();

            //    foreach (var book in books)
            //    {
            //        //myBook.
            //        ((IDictionary<string, object>)myBook)[book.Key] = book.Value;
            //    }

            //     ((IDictionary<string, object>)myClass1)[shop] = myBook;
            //}

            //var value = GetNestedPropertyValue(myClass1, "Shop1.book1");

            //Console.WriteLine($"Class Name: {myClass1.Name}");
            //foreach (var prop in myClass1)
            //{
            //    Console.WriteLine("Properties: " + prop);
            //}

            //Error.OnErrorDetected += lst => {
            //    foreach (var e in lst)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //};

            //MethodA(1);
            //MethodA(6);
            //MethodA(7);
            //MethodA(2);
            //MethodA(6);


            // Tạo danh sách các ExpandoObject (danh sách tương tự như các bản ghi trong database)
            //List<ExpandoObject> dynamicList = new List<ExpandoObject>();

            //// Tạo một đối tượng ExpandoObject
            //dynamic record1 = new ExpandoObject();
            //record1.Id = 1;
            //record1.Name = "John Doe";
            //record1.Age = 30;
            //dynamicList.Add(record1);

            //dynamic record2 = new ExpandoObject();
            //record2.Id = 2;
            //record2.Name = "Jane Smith";
            //record2.Age = 25;
            //dynamicList.Add(record2);

            //// Chuyển đổi danh sách ExpandoObject thành DataTable
            //DataTable dataTable = ConvertToDataTable(dynamicList);

            //// In ra nội dung DataTable
            //PrintDataTable(dataTable);

            //// Thực hiện truy vấn trên DataTable bằng LINQ
            //var query = from row in dataTable.AsEnumerable()
            //            where row.Field<int>("Id") == 1
            //            select row;

            //// In kết quả truy vấn
            //Console.WriteLine("\nQuery Result:");
            //foreach (var row in query)
            //{
            //    Console.WriteLine($"{row["Id"]}, {row["Name"]}, {row["Age"]}");
            //}

            // create parent
            DataTable parentTable = new DataTable("TuTimKiem");
            parentTable.Columns.Add("Id", typeof(string));
            parentTable.Columns.Add("A", typeof(bool));
            parentTable.Columns.Add("B", typeof(bool));
            parentTable.Columns.Add("C", typeof(bool));

            // add data
            parentTable.Rows.Add(1, true, true, false);
            parentTable.Rows.Add(2, false, true, false);
            parentTable.Rows.Add(3, false, true, true);
            parentTable.Rows.Add(4, false, false, false);

            // get column
            var columnNames = parentTable.Columns.Cast<DataColumn>()
                                   .Select(column => column.ColumnName).ToArray();
            // get all row
            var rows = (from row in parentTable.AsEnumerable()
                        select row).ToArray();
            // get all by id
            var rowIds = (from row in parentTable.AsEnumerable()
                          select row["Id"]).ToArray();

            // condition
            var rowConditions = (from row in parentTable.AsEnumerable()
                                 where int.Parse(row.Field<string>("Id")) <= 2
                                 select row).ToArray();

            var query = from parent in parentTable.AsEnumerable()
                        join child in childTable.AsEnumerable()
                        on parent.Field<int>("Id") equals child.Field<int>("Id")
                        select new
                        {
                            ParentId = parent.Field<int>("Id"),
                            ParentName = parent.Field<string>("ParentName"),
                            ChildId = child.Field<int>("ChildId"),
                            ChildName = child.Field<string>("ChildName")
                        };

            var groupedData = (from row in parentTable.AsEnumerable()
                               group row by row.Field<bool>("A") into A
                               select new
                               {
                                   Age = A.Key,
                                   Row = A
                               }).ToArray();
            var rowGroupIds = from row in groupedData[1].Row
                              select row.Field<string>("Id");

            var selectedColumns = from row in parentTable.AsEnumerable()
                                  select new
                                  {
                                      Name = row.Field<string>("Name"),
                                      Age = row.Field<int>("Age")
                                  };

            var sortedRows = from row in parentTable.AsEnumerable()
                             orderby row.Field<string>("Name")
                             select row;


            // 2. Tạo bảng con (ChildTable)
            DataTable childTable = new DataTable("KetQua");
            childTable.Columns.Add("Id", typeof(string));
            childTable.Columns.Add("Book1", typeof(bool));
            childTable.Columns.Add("Book2", typeof(bool));
            childTable.Columns.Add("Book3", typeof(bool));

            // Thêm dữ liệu vào bảng con
            childTable.Rows.Add(1, true, true, true);
            childTable.Rows.Add(2, false, false, true);
            childTable.Rows.Add(3, false, true, false);
            childTable.Rows.Add(4, true, true, true);

            // 3. Tạo DataSet để chứa các bảng
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(parentTable);
            dataSet.Tables.Add(childTable);

            // 4. Thiết lập quan hệ khóa ngoại giữa ParentTable và ChildTable
            DataColumn parentColumn = parentTable.Columns["Id"];
            DataColumn childColumn = childTable.Columns["Id"];
            DataRelation relation = new DataRelation("ParentChildRelation", parentColumn, childColumn);
            dataSet.Relations.Add(relation);

            // 5. In dữ liệu từ cả hai bảng với quan hệ
            PrintTablesWithRelation(dataSet);


            Console.WriteLine("Hello, World!");
        }

        static void PrintTablesWithRelation(DataSet dataSet)
        {
            DataTable parentTable = dataSet.Tables["TuTimKiem"];
            DataTable childTable = dataSet.Tables["KetQua"];

            Console.WriteLine("TuTimKiem Table:");
            foreach (DataRow parentRow in parentTable.Rows)
            {
                Console.WriteLine($"ParentId: {parentRow["Id"]}, ParentName: {parentRow["A"]}");

                // In các bản ghi con liên kết với bản ghi cha này
                DataRow[] childRows = parentRow.GetChildRows("ParentChildRelation");
                foreach (DataRow childRow in childRows)
                {
                    Console.WriteLine($"\tChildId: {childRow["Id"]}, ChildName: {childRow["Book1"]}");
                }
            }
        }

        // Hàm chuyển đổi từ ExpandoObject thành DataTable
        static DataTable ConvertToDataTable(List<ExpandoObject> list)
        {
            DataTable table = new DataTable();
            bool columnsCreated = false;

            foreach (var record in list)
            {
                var dictionary = (IDictionary<string, object>)record;

                // Tạo cột nếu chưa có
                if (!columnsCreated)
                {
                    foreach (var kvp in dictionary)
                    {
                        table.Columns.Add(kvp.Key, kvp.Value?.GetType() ?? typeof(object));
                    }
                    columnsCreated = true;
                }

                // Thêm hàng vào DataTable
                DataRow row = table.NewRow();
                foreach (var kvp in dictionary)
                {
                    row[kvp.Key] = kvp.Value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        // Hàm in DataTable
        static void PrintDataTable(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                Console.Write($"{column.ColumnName}\t");
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item}\t");
                }
                Console.WriteLine();
            }
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

        class Status
        {
            public bool IsPass { get; set; }
            public string Message { get; set; }
        }
        class Error : Status
        {
            static public List<Error> Report { get; set; } = new List<Error>();
            static public event Action<List<Error>> OnErrorDetected;
            public Error(string message)
            {
                IsPass = false;
                Message = message;
                Report.Add(this);

                OnErrorDetected?.Invoke(Report);
            }
        }

        static Status MethodA(int i)
        {
            if (i == 6)
            {
                return new Error("6");
            }
            if (i == 7)
            {
                return new Error("7");
            }

            return new Status { IsPass = true };
        }



    }
}