#pragma checksum "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "eae2db209ba15994b5be332275d5a6c8d27c69e2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(RelationTest.Pages.CMCAssigns.Pages_CMCAssigns_Details), @"mvc.1.0.razor-page", @"/Pages/CMCAssigns/Details.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.RazorPageAttribute(@"/Pages/CMCAssigns/Details.cshtml", typeof(RelationTest.Pages.CMCAssigns.Pages_CMCAssigns_Details), null)]
namespace RelationTest.Pages.CMCAssigns
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "c:\Dev\lab\EnergyApp\RelationTest\Pages\_ViewImports.cshtml"
using RelationTest;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"eae2db209ba15994b5be332275d5a6c8d27c69e2", @"/Pages/CMCAssigns/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d655207389caef8c54ff58a409a5ca9773018e6a", @"/Pages/_ViewImports.cshtml")]
    public class Pages_CMCAssigns_Details : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page", "./Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(58, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 4 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
  
    ViewData["Title"] = "Details";

#line default
#line hidden
            BeginContext(103, 130, true);
            WriteLiteral("\r\n<h1>Details</h1>\r\n\r\n<div>\r\n    <h4>CMCAssign</h4>\r\n    <hr />\r\n    <dl class=\"row\">\r\n        <dt class=\"col-sm-2\">\r\n            ");
            EndContext();
            BeginContext(234, 55, false);
#line 15 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CMCAssign.AddedDate));

#line default
#line hidden
            EndContext();
            BeginContext(289, 61, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
            EndContext();
            BeginContext(351, 51, false);
#line 18 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayFor(model => model.CMCAssign.AddedDate));

#line default
#line hidden
            EndContext();
            BeginContext(402, 60, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
            EndContext();
            BeginContext(463, 51, false);
#line 21 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CMCAssign.Meter));

#line default
#line hidden
            EndContext();
            BeginContext(514, 61, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
            EndContext();
            BeginContext(576, 50, false);
#line 24 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayFor(model => model.CMCAssign.Meter.ID));

#line default
#line hidden
            EndContext();
            BeginContext(626, 60, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
            EndContext();
            BeginContext(687, 53, false);
#line 27 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CMCAssign.Charger));

#line default
#line hidden
            EndContext();
            BeginContext(740, 61, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
            EndContext();
            BeginContext(802, 52, false);
#line 30 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayFor(model => model.CMCAssign.Charger.ID));

#line default
#line hidden
            EndContext();
            BeginContext(854, 60, true);
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
            EndContext();
            BeginContext(915, 54, false);
#line 33 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.CMCAssign.Customer));

#line default
#line hidden
            EndContext();
            BeginContext(969, 61, true);
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
            EndContext();
            BeginContext(1031, 53, false);
#line 36 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
       Write(Html.DisplayFor(model => model.CMCAssign.Customer.ID));

#line default
#line hidden
            EndContext();
            BeginContext(1084, 47, true);
            WriteLiteral("\r\n        </dd>\r\n    </dl>\r\n</div>\r\n<div>\r\n    ");
            EndContext();
            BeginContext(1132, 68, false);
#line 41 "c:\Dev\lab\EnergyApp\RelationTest\Pages\CMCAssigns\Details.cshtml"
Write(Html.ActionLink("Edit", "Edit", new { /* id = Model.PrimaryKey */ }));

#line default
#line hidden
            EndContext();
            BeginContext(1200, 8, true);
            WriteLiteral(" |\r\n    ");
            EndContext();
            BeginContext(1208, 38, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "eae2db209ba15994b5be332275d5a6c8d27c69e27499", async() => {
                BeginContext(1230, 12, true);
                WriteLiteral("Back to List");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Page = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1246, 10, true);
            WriteLiteral("\r\n</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RelationTest.Pages_CMCAssigns.DetailsModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<RelationTest.Pages_CMCAssigns.DetailsModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<RelationTest.Pages_CMCAssigns.DetailsModel>)PageContext?.ViewData;
        public RelationTest.Pages_CMCAssigns.DetailsModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591