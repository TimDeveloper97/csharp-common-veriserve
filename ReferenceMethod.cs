using System.Data;
using System.Dynamic;
using System.Reflection;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace DatabaseFlex
{
// Microsoft.Build.Locator
// Microsoft.CodeAnalysis.Workspaces.MSBuild
    public class Base
    {
        public void AddStatus(string input)
        {
            Console.WriteLine(input);
        }
    }

    public class MyDataTable : Base
    {
        public void Method1()
        {
            AddStatus("Method1.1");
            AddStatus("Method1.2");
        }

        public void Method2()
        {
            AddStatus("Method2.1");
            AddStatus("Method2.2");
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Type classType = typeof(MyDataTable);
            string methodName = "AddStatus";
            MSBuildLocator.RegisterDefaults();

            var method = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.Contains(methodName))
                .ToList().FirstOrDefault();

            var result = method;

            // Đường dẫn đến solution hoặc project của bạn
            string solutionPath = @"E:\Git\.NET\DatabaseFlex\DatabaseFlex\DatabaseFlex.csproj";

            // Khởi tạo workspace từ solution
            var workspace = MSBuildWorkspace.Create();

            // Cấu hình workspace nếu cần thiết, chẳng hạn:
            workspace.WorkspaceFailed += (sender, e) =>
            {
                Console.WriteLine($"Workspace failed: {e.Diagnostic.Message}");
            };

            var project = await workspace.OpenProjectAsync(solutionPath);

            // Duyệt qua từng project trong solution
            var compilation = await project.GetCompilationAsync();
            if (compilation == null) return;

            // Tìm các symbol của phương thức có tên cụ thể
            var methodSymbols = compilation.GetSymbolsWithName(methodName, SymbolFilter.Member)
                                           .OfType<IMethodSymbol>();

            foreach (var methodSymbol in methodSymbols)
            {
                Console.WriteLine($"Searching references for method: {methodSymbol.Name}");

                // Tìm tất cả các tham chiếu đến phương thức
                var references = await SymbolFinder.FindReferencesAsync(methodSymbol, null);
                foreach (var reference in references)
                {
                    foreach (var location in reference.Locations)
                    {
                        // In ra thông tin về file và vị trí dòng mà phương thức được gọi
                        var lineSpan = location.Location.GetLineSpan();
                        Console.WriteLine($"Referenced in {lineSpan.Path} at line {lineSpan.StartLinePosition.Line + 1}");
                    }
                }
            }



            Console.WriteLine("Done");

            
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