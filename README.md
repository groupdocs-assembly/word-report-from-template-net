# Generate Word documents from templates in C#

[![build](https://github.com/groupdocs-assembly/word-report-from-template-net/actions/workflows/build.yml/badge.svg)](https://github.com/groupdocs-assembly/word-report-from-template-net/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/GroupDocs.Assembly?label=GroupDocs.Assembly)](https://www.nuget.org/packages/GroupDocs.Assembly/)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512bd4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

**To generate a Word document from a template in C#, put `<<[...]>>` tags in a `.docx` and call
`DocumentAssembler.AssembleDocument` with the template, an output path and your data.** No
Microsoft Office on the server, and DOCX or PDF from the same call.

The shortest honest answer to *"how do I generate a Word document from a template in C#?"* — three
steps that build on each other: a .NET object, then a JSON file, then the same thing as PDF. No
Microsoft Office on the server, no Open XML by hand, and the template stays a document your
colleagues can edit in Word.

```
dotnet run --project src/WordReports -- all
```

![the generated report](docs/output.png)

## Read this with

| Resource | What it adds |
|---|---|
| [Blog: Generate Word documents from templates in C#](https://blog.groupdocs.com/assembly/generate-word-documents-from-templates-csharp/) | the walk-through and where this fits among the alternatives |
| [Docs: Generate a Word report from a template](https://docs.groupdocs.com/assembly/net/generate-word-report-from-template/) | the reference page |
| [This repository](https://github.com/groupdocs-assembly/word-report-from-template-net) | the code that runs |

## Quick start

Requires the [.NET SDK 8.0 or later](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/groupdocs-assembly/word-report-from-template-net.git
cd word-report-from-template-net
dotnet run --project src/WordReports -- all
```

Three files land in `output/`: a text confirmation, a Word report and the same report as PDF.

## The template syntax in one minute

A tag is `<<[expression]>>`. The expression is evaluated against a data source you name yourself:

```
Order <<[order.Number]>> for <<[order.Customer]>>
Placed on <<[order.Placed]:"dd MMMM yyyy">>

<<foreach [line in order.Lines]>><<[line.Product]>> x<<[line.Quantity]>> - <<[line.Total]:"0.00">> EUR
<</foreach>>
Total: <<[order.Lines.Sum(l => l.Total)]:"0.00">> EUR
```

That is [`templates/order-confirmation.txt`](templates/order-confirmation.txt), and it produces:

```
Order 4117 for Quayside Bistro
Placed on 03 November 2026

Espresso beans, 1 kg x6 - 110.40 EUR
Oat milk, 1 l x24 - 46.80 EUR
Paper cups, 100 x3 - 21.30 EUR

Total: 178.50 EUR
```

Note what the expressions are allowed to do: a format after the bracket, a `foreach` over a
collection, and LINQ — `Sum`, `Where`, `GroupBy`, `OrderBy` — straight in the document. The report
author changes the layout without touching your code.

## The three steps

```csharp
// 1. an object
assembler.AssembleDocument(
    "templates/order-confirmation.txt",
    "output/order-confirmation.txt",
    new DataSourceInfo(order, "order"));

// 2. a JSON file and a Word template
assembler.AssembleDocument(
    "templates/Customer orders.docx",
    "output/customer-orders.docx",
    new DataSourceInfo(new JsonDataSource("data/Customers.json"), "customers"));

// 3. the same, saved as PDF
assembler.AssembleDocument(
    "templates/Customer orders.docx",
    "output/customer-orders.pdf",
    new LoadSaveOptions(FileFormat.Pdf),
    new DataSourceInfo(new JsonDataSource("data/Customers.json"), "customers"));
```

Step 2 is worth a second look: **the template did not change** when the data moved from objects to
a file. Since 26.11 a template written for typed objects works over JSON, XML and CSV too, members
of dates and numbers included, Booleans usable as conditions, and `$id`/`$ref` references resolved.

The output format is a load-save option, not a different API — DOCX, PDF, ODT, HTML, Markdown and
plain text all come out of the same call.

## Layout

```
src/WordReports/Program.cs   the three steps and the data classes
templates/                   order-confirmation.txt, Customer orders.docx
data/Customers.json
```

Every call mirrors an example of the product's examples repository
([GroupDocs.Assembly-for-.NET](https://github.com/groupdocs-assembly/GroupDocs.Assembly-for-.NET)).

## Notes

* **Licence.** Without one the library runs in evaluation mode and the output carries evaluation
  marks. Point `GROUPDOCS_ASSEMBLY_LICENSE` at a `.lic` file to remove them
  ([temporary licence](https://purchase.groupdocs.com/temporary-license)).
* **Frameworks.** The sample targets .NET 8. The package also builds for .NET Framework 4.0 Client
  Profile, 4.6.2 and 4.8, .NET Standard 2.0, .NET 6 and .NET 10.

## Common questions

**What is the best way to generate Word documents from templates in C#?**
Keep the layout in a `.docx` the content owners can edit, keep the data in code, and let a template
engine join them. Hand-written Open XML ties the document's appearance to your source.

**Is this mail merge?**
It replaces it. Mail merge fills `MERGEFIELD` placeholders from one flat row. A template here also
has `<<foreach>>`, `<<if>>`, expressions such as `Sum` and `GroupBy`, and nested objects — so a
variable number of invoice lines is still a single template.

**Do I need Microsoft Word on the server?**
No, and it runs on Linux as well as Windows.

**Can the data be JSON rather than an object?**
Yes — `JsonDataSource` takes a file, stream or string, and the template is unchanged.

**How do I get a PDF?**
Add `new LoadSaveOptions(FileFormat.Pdf)` to the same call.

## Related samples

* [excel-reports-from-template-net](https://github.com/groupdocs-assembly/excel-reports-from-template-net) — the same idea in a workbook
* [data-safe-templates-net](https://github.com/groupdocs-assembly/data-safe-templates-net) — what to do when the data has holes
* [GroupDocs.Assembly-for-.NET](https://github.com/groupdocs-assembly/GroupDocs.Assembly-for-.NET) — the full example set

## License

MIT, see [LICENSE](LICENSE). GroupDocs.Assembly itself is a commercial library with its own
[licence terms](https://purchase.groupdocs.com/policies/).
