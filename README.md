Relorer.QuestPDF.HTML is an extension for QuestPDF that allows to generate PDF from HTML

[QuestPDF](https://github.com/QuestPDF/QuestPDF)  currently does not support inserting html into a pdf document. So I wrote a small library for this. It doesn't support the full functionality of html and css, but I think it should be enough for most cases.



#### Dependencies
- [QuestPDF](https://github.com/QuestPDF/QuestPDF)
- [HtmlAgilityPack](https://html-agility-pack.net/) is used for html parsing

#### Usage

The simplest example of use:
```
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Content().Column(col =>
            {
                col.Item().HTML(handler =>
                {
                    handler.SetHtml(html);
                });
            });
    });
}).GeneratePdf(path);
```
**I strongly recommend overloading the image upload method, because the outdated WebClient is used by default without using asynchronous.**
To do this, you can use the OverloadImgReceivingFunc:
```
col.Item().HTML(handler =>
{
    handler.OverloadImgReceivingFunc(GetImgBySrc);
    handler.SetHtml(html);
});
```

You can customize the styles of text and containers for tags:
```
handler.SetTextStyleForHtmlElement("div", TextStyle.Default.FontColor(Colors.Grey.Medium));
handler.SetTextStyleForHtmlElement("h1", TextStyle.Default.FontColor(Colors.DeepOrange.Accent4).FontSize(32).Bold());
handler.SetContainerStyleForHtmlElement("table", c => c.Background(Colors.Pink.Lighten5));
handler.SetContainerStyleForHtmlElement("ul", c => c.PaddingVertical(10));
```

You can set the vertical padding size for lists. This padding will not apply to sub-lists:
```
handler.SetListVerticalPadding(40);
```

You can use [HTMLToQPDF.Example](https://github.com/Relorer/HTMLToQPDF/releases/tag/1.0.0) to try out the capabilities of this extension.

<p align="center">
  <img src="https://user-images.githubusercontent.com/26045342/195960914-1aef2f7e-f5bb-4c4b-bbe9-cd4770a0527f.png" />
</p>


<table border="0">
 <tr>
    <td><b style="font-size:30px">Default Styles</b></td>
    <td><b style="font-size:30px">Options for changing styles</b></td>
 </tr>
 <tr>
    <td><img src="https://user-images.githubusercontent.com/26045342/195960950-8bf101e9-c64e-482c-9993-39f9646d0e2f.png" /></td>
    <td><img src="https://user-images.githubusercontent.com/26045342/195960936-6f014456-a074-4672-aa39-03cdcdcc3afc.png" /></td>
 </tr>
</table>
