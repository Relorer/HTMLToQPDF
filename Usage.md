# HTMLToQPDF Usage Guide

HTMLToQPDF is a .NET library that extends QuestPDF to generate PDF documents from HTML content. It parses HTML using HtmlAgilityPack and converts it into QuestPDF components, supporting a wide range of HTML elements with customizable styling.

## Features

- Convert HTML to PDF with QuestPDF
- Support for common HTML elements (headings, paragraphs, lists, tables, images, links, etc.)
- Customizable text and container styles
- Image handling with custom download functions
- Nested lists and tables
- Basic text formatting (bold, italic, underline, strikethrough, etc.)

## Installation

Add the HTMLToQPDF package to your .NET project via NuGet:

```bash
dotnet add package HTMLToQPDF
```

### Dependencies

- [QuestPDF](https://www.nuget.org/packages/QuestPDF/)
- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/)

## Basic Usage

### Simple HTML to PDF

```csharp
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using HTMLToQPDF.Components;

// Create a document
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Content().Column(col =>
        {
            col.Item().HTML(handler =>
            {
                handler.SetHtml("<h1>Hello World</h1><p>This is a paragraph.</p>");
            });
        });
    });
}).GeneratePdf("output.pdf");
```

### Handling Images

By default, images are downloaded using `WebClient`. For better performance and async support, override the image receiving function:

```csharp
using System.Net.Http;
using System.Threading.Tasks;

// Define a custom image loader
private static async Task<byte[]?> GetImgBySrc(string src)
{
    using var client = new HttpClient();
    try
    {
        var response = await client.GetAsync(src);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
    catch
    {
        // Handle errors
    }
    return null;
}

// Use in HTML handler
col.Item().HTML(handler =>
{
    handler.OverloadImgReceivingFunc(src => Task.Run(() => GetImgBySrc(src)).Result);
    handler.SetHtml(htmlWithImages);
});
```

## Customizing Styles

### Text Styles

Customize text appearance for specific HTML elements:

```csharp
handler.SetTextStyleForHtmlElement("h1", TextStyle.Default.FontSize(32).Bold().FontColor(Colors.Blue.Medium));
handler.SetTextStyleForHtmlElement("p", TextStyle.Default.FontSize(12).LineHeight(1.5f));
handler.SetTextStyleForHtmlElement("strong", TextStyle.Default.Bold());
```

### Container Styles

Apply layout styles to container elements:

```csharp
handler.SetContainerStyleForHtmlElement("div", c => c.Padding(10).Background(Colors.Grey.Lighten3));
handler.SetContainerStyleForHtmlElement("ul", c => c.PaddingLeft(20));
handler.SetContainerStyleForHtmlElement("table", c => c.Border(1).BorderColor(Colors.Black));
```

### List Vertical Padding

Set padding between list items (does not apply to nested lists):

```csharp
handler.SetListVerticalPadding(15); // Default is 0
```

## Supported HTML Elements

### Block Elements
- `p`, `div`, `h1`-`h6`: Paragraphs and headings
- `ul`, `ol`, `li`: Lists (unordered and ordered)
- `table`, `tr`, `td`, `th`: Tables
- `br`: Line breaks

### Inline Elements
- `b`, `strong`: Bold text
- `i`, `em`: Italic text
- `u`: Underlined text
- `strike`, `del`, `s`: Strikethrough text
- `small`: Smaller text
- `sup`, `sub`: Superscript and subscript
- `a`: Links (rendered as underlined text)
- `img`: Images

### Notes on Support
- CSS classes and IDs are ignored; styling is based on tag names only.
- Complex layouts (flexbox, grid) are not supported.
- `pre` and `code` elements do not preserve whitespace or use monospace fonts.
- `hr` elements are not rendered.
- Form elements (`input`, `button`, etc.) are ignored.
- JavaScript and CSS are stripped during parsing.

## Examples

### Complete Example with Styling

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HTMLToQPDF.Components;

string html = @"
<h1>My Document</h1>
<p>This is a <strong>bold</strong> paragraph with <a href='https://example.com'>a link</a>.</p>
<ul>
    <li>Item 1</li>
    <li>Item 2
        <ol>
            <li>Subitem 1</li>
            <li>Subitem 2</li>
        </ol>
    </li>
</ul>
<table>
    <tr><th>Name</th><th>Age</th></tr>
    <tr><td>John</td><td>30</td></tr>
    <tr><td>Jane</td><td>25</td></tr>
</table>
";

Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.Content().Column(col =>
        {
            col.Item().HTML(handler =>
            {
                handler.SetHtml(html);
                handler.SetTextStyleForHtmlElement("h1", TextStyle.Default.FontSize(24).Bold());
                handler.SetContainerStyleForHtmlElement("table", c => c.Border(1));
                handler.SetListVerticalPadding(10);
            });
        });
    });
}).GeneratePdf("styled_output.pdf");
```

### Using with WPF or Other Frameworks

The library integrates seamlessly with QuestPDF. For WPF applications, ensure you handle threading appropriately for PDF generation.

## Limitations

- No full CSS support; only basic tag-based styling.
- Images must be loaded synchronously or handled via custom functions.
- Complex HTML structures may not render perfectly.
- No support for interactive elements or JavaScript.
- Whitespace in `pre` blocks is not preserved.

## Contributing

If you find missing features or bugs, feel free to contribute or report issues on the GitHub repository.

## License

This library is open-source. Check the LICENSE file for details.
