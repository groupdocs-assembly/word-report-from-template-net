using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Assembly;
using GroupDocs.Assembly.Data;

namespace WordReports
{
    /// <summary>
    /// Generating a Word report from a template, from the smallest possible case to a PDF.
    /// The calls mirror the official examples repository
    /// (GroupDocs.Assembly-for-.NET/Examples/GroupDocs.Assembly.Examples.CSharp).
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            ApplyLicense();

            string command = args.Length == 0 ? "all" : args[0].ToLowerInvariant();

            try
            {
                switch (command)
                {
                    case "object": FromObject(); return 0;
                    case "json": FromJson(); return 0;
                    case "pdf": ToPdf(); return 0;
                    case "all": FromObject(); FromJson(); ToPdf(); return 0;
                    case "-h":
                    case "--help": Usage(); return 0;
                    default:
                        Console.Error.WriteLine($"Unknown command '{command}'.");
                        Usage();
                        return 1;
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error: " + exception.Message);
                return 1;
            }
        }

        // ------------------------------------------------------------------ 1. an object

        /// <summary>
        /// The smallest useful case: a .NET object and a text template.
        ///
        /// A tag is <c>&lt;&lt;[expression]&gt;&gt;</c>. The expression is C#-like and is evaluated
        /// against the data source, which is named by the second argument of
        /// <see cref="DataSourceInfo"/> - here <c>order</c>.
        /// </summary>
        public static void FromObject()
        {
            Order order = new Order
            {
                Number = 4117,
                Customer = "Quayside Bistro",
                Placed = new DateTime(2026, 11, 3),
                Lines = new List<OrderLine>
                {
                    new OrderLine { Product = "Espresso beans, 1 kg", Quantity = 6, Price = 18.40m },
                    new OrderLine { Product = "Oat milk, 1 l", Quantity = 24, Price = 1.95m },
                    new OrderLine { Product = "Paper cups, 100", Quantity = 3, Price = 7.10m }
                }
            };

            DocumentAssembler assembler = new DocumentAssembler();
            assembler.AssembleDocument(
                RepositoryPaths.Template("order-confirmation.txt"),
                RepositoryPaths.OutputFile("order-confirmation.txt"),
                new DataSourceInfo(order, "order"));

            Console.WriteLine("wrote : " + RepositoryPaths.OutputFile("order-confirmation.txt"));
            Console.WriteLine();
            Console.WriteLine(File.ReadAllText(RepositoryPaths.OutputFile("order-confirmation.txt")));
        }

        // ------------------------------------------------------------------ 2. JSON

        /// <summary>
        /// The same engine over a JSON file and a Word template.
        ///
        /// Nothing about the template changes when the data moves from objects to a file: since
        /// 26.11 a template written for typed objects works over JSON, XML and CSV, including
        /// members of dates, Booleans in conditions and grouping over nested objects.
        /// </summary>
        public static void FromJson()
        {
            DocumentAssembler assembler = new DocumentAssembler();
            assembler.AssembleDocument(
                RepositoryPaths.Template("Customer orders.docx"),
                RepositoryPaths.OutputFile("customer-orders.docx"),
                new DataSourceInfo(new JsonDataSource(RepositoryPaths.DataFile("Customers.json")), "customers"));

            Console.WriteLine("wrote : " + RepositoryPaths.OutputFile("customer-orders.docx"));
        }

        // ------------------------------------------------------------------ 3. PDF

        /// <summary>
        /// The same template and data saved as PDF: the output format is a load-save option,
        /// not a different API. No Microsoft Word and no Adobe Acrobat are involved.
        /// </summary>
        public static void ToPdf()
        {
            DocumentAssembler assembler = new DocumentAssembler();
            assembler.AssembleDocument(
                RepositoryPaths.Template("Customer orders.docx"),
                RepositoryPaths.OutputFile("customer-orders.pdf"),
                new LoadSaveOptions(FileFormat.Pdf),
                new DataSourceInfo(new JsonDataSource(RepositoryPaths.DataFile("Customers.json")), "customers"));

            Console.WriteLine("wrote : " + RepositoryPaths.OutputFile("customer-orders.pdf"));
        }

        // ------------------------------------------------------------------ data

        public sealed class Order
        {
            public int Number { get; set; }

            public string Customer { get; set; } = string.Empty;

            public DateTime Placed { get; set; }

            public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
        }

        public sealed class OrderLine
        {
            public string Product { get; set; } = string.Empty;

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal Total => Quantity * Price;
        }

        // ------------------------------------------------------------------ plumbing

        /// <summary>Optional: without a licence the output carries evaluation marks.</summary>
        private static void ApplyLicense()
        {
            string? path = Environment.GetEnvironmentVariable("GROUPDOCS_ASSEMBLY_LICENSE")?.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (File.Exists(path))
            {
                new License().SetLicense(path);
            }
            else
            {
                Console.Error.WriteLine("GROUPDOCS_ASSEMBLY_LICENSE points at a missing file: " + path);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Word reports from templates - GroupDocs.Assembly for .NET");
            Console.WriteLine();
            Console.WriteLine("  object   a .NET object and a text template");
            Console.WriteLine("  json     a JSON file and a Word template");
            Console.WriteLine("  pdf      the same template and data, saved as PDF");
            Console.WriteLine("  all      all three (the default)");
            Console.WriteLine();
            Console.WriteLine("  dotnet run --project src/WordReports -- all");
        }
    }
}
