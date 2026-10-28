using System.Text;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Controllers.Token;
using Accounting_V4.Data;
using Accounting_V4.Model.ConnectionModels;
using Accounting_V4.Model.UserModels;
using Accounting_V4.Services;
using Accounting_V4.Services.AppStateServices;
using Accounting_V4.Services.AuthenServices;
using Accounting_V4.Services.DialogServices;
using Accounting_V4.Services.FeatureServices;
using Accounting_V4.Services.MenuServices;
using Accounting_V4.Services.MessageHandlerServices;
using Accounting_V4.Services.OnlineUserServices;
using Accounting_V4.Services.OptimisticLockFieldServices;
using Accounting_V4.Services.QNEServices;
using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.QNEServices.AllTaxCodeServices;
using Accounting_V4.Services.QNEServices.CurrenciesServices;
using Accounting_V4.Services.QNEServices.CustomerServices;
using Accounting_V4.Services.QNEServices.DashBoardServices;
using Accounting_V4.Services.QNEServices.DescriptionServices;
using Accounting_V4.Services.QNEServices.GeneralLedgerServices;
using Accounting_V4.Services.QNEServices.GLAccountServices;
using Accounting_V4.Services.QNEServices.InvoicesServices;
using Accounting_V4.Services.QNEServices.MaintenanceServices;
using Accounting_V4.Services.QNEServices.MiscServices;
using Accounting_V4.Services.QNEServices.ProjecsServices;
using Accounting_V4.Services.QNEServices.PurchasesServices;
using Accounting_V4.Services.QNEServices.RunningNumberServices;
using Accounting_V4.Services.QNEServices.SalesServices;
using Accounting_V4.Services.QNEServices.StockServices;
using Accounting_V4.Services.QNEServices.SupplierServices;
using Accounting_V4.Services.QNEServices.TermServices;
using Accounting_V4.Services.QNEServices.TransactionCodeServices;
using Accounting_V4.Services.QNEServices.TransferServices;
using Accounting_V4.Services.QNEServices.UOMServices;
using Accounting_V4.Services.Security;
using Accounting_V4.Services.SystemServices.TimeZoneServices;
using Accounting_V4.Services.TokenServices;
using Accounting_V4.Services.UserServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Radzen;


var builder = WebApplication.CreateBuilder(args);
//var builder = WebAssemblyHostBuilder.CreateDefault(args);
//ALL QNE SERVICES
builder.Services.AddScoped<QneServices>();
builder.Services.AddScoped<OverViewServices>();
builder.Services.AddScoped<GLAccountsServices>();
builder.Services.AddScoped<JournalServices>();
builder.Services.AddScoped<ChartOfAccountsServices>();
builder.Services.AddScoped<AllTaxCodeService>();
builder.Services.AddScoped<CustomersServices>();
builder.Services.AddScoped<MiscService>();
builder.Services.AddScoped<CurrenciesService>();
builder.Services.AddScoped<SuppliersServices>();
builder.Services.AddScoped<PurchaseRequisitionsService>();
builder.Services.AddScoped<StockItemsService>();
builder.Services.AddScoped<TransactionNumberHelper>();
builder.Services.AddScoped<BillsTransactionHelper>();
builder.Services.AddScoped<JournalTransactionHelper>();
builder.Services.AddScoped<QuotationServices>();
builder.Services.AddScoped<SalesOrdersServices>();
builder.Services.AddScoped<DeliveryOrdersServices>();
builder.Services.AddScoped<SalesInvoiceServices>();
builder.Services.AddScoped<StockItemsService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<BillServices>();
builder.Services.AddScoped<ReceiptServices>();
builder.Services.AddScoped<DescriptionsService>();
builder.Services.AddScoped<TransactionCodeService>();
builder.Services.AddScoped<ReceiptTransactionHelper>();
builder.Services.AddScoped<RunningNumberService>();

builder.Services.AddScoped<TermService>();
builder.Services.AddScoped<PurchaseOrdersService>();
builder.Services.AddScoped<GoodsReceivedNotesService>();
builder.Services.AddScoped<PurchaseInvoicesService>();

builder.Services.AddScoped<GLTransactionsServices>();
builder.Services.AddScoped<BankChargesServices>();
builder.Services.AddScoped<BankReconciliationServices>();

builder.Services.AddScoped<PurchaseTransferLinkServices>();
builder.Services.AddScoped<GoodsReceivedNotesTransferLinkServices>(); // GRN TRANSFER LINK SERVICES 
builder.Services.AddScoped<PurchaseInvoiceTransferLinkServices>(); // PI TRANSFER LINK SERVICES


builder.Services.AddScoped<SalesTransferLinkServices>();
builder.Services.AddScoped<StocksTransferLinkServices>();
builder.Services.AddScoped<TransferTable>();
builder.Services.AddScoped<InvoiceServices>();
builder.Services.AddScoped<HeaderSwitchServices>();

builder.Services.AddSingleton<AppStateService>();

builder.Services.AddScoped<SidePanelService>();

builder.Services.AddScoped<ValidPostingServices>();
builder.Services.AddScoped<CheckExistServices>();
builder.Services.AddScoped<DocumentsInfoServices>();


builder.Services.AddScoped<DeleteServices>();

//FEATURE SERVICE
builder.Services.AddScoped(typeof(RowDragDropService<>));

//SYSTEM SERVICES
builder.Services.AddScoped<OptimisticLockField>();
builder.Services.AddScoped<CustomNotificationMessage>();
builder.Services.AddScoped<RequestMessageHandler>();
builder.Services.AddScoped<NotificationHelper>();
builder.Services.AddScoped<SystemRoleServices>();
builder.Services.AddScoped<SystemUserServices>();

builder.Services.AddScoped<TimeService>();
builder.Services.AddScoped<TermsDayMonthServices>();
//builder.Services.AddHttpClient<TimeService>();
//builder.Services.AddSingleton<TimeService>();


//ALL SERVICES
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<Token>();
builder.Services.AddScoped<TabService>();
builder.Services.AddScoped<ForCloseCancelledDocuments>();
builder.Services.AddScoped<EISIdService>();

//FOR UOM CONVERSION PACKING BY RATE AND QTY
builder.Services.AddScoped<IUomConversionService, UomConversionService>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRadzenComponents();
builder.Services.AddAuthorizationCore();

//ALL AUTHENTICATION SERVICES
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

//SECURITY SERVICES
builder.Services.AddSingleton<IDevExpressPasswordService, DevExpressPasswordService>();

builder.Services.AddSingleton<OnlineUserService>();
builder.Services.AddScoped<CircuitHandler, UserPresenceCircuitHandler>();



var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";

}).AddJwtBearer("JwtBearer", jwtBearerOptions =>
{
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});



//var ServerBaseURL = builder.Configuration["ServerBaseURL"];
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServerBaseURL) });

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MyApplicationTheme"; // The name of the cookie
    options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});



//builder.Services.AddScoped<QNESQLConnection>();
builder.Services.AddScoped<QneConnectionString>();
//connection for ACCUBOOKDB
var appConnection = new AppConnectionString(builder.Configuration.GetConnectionString("DefaultConnectionString"));
builder.Services.AddSingleton(appConnection);


//connection for QNE
//builder.Services.Configure<QneConnectionSettings>(builder.Configuration.GetSection("ConnectionStrings"));
//builder.Services.Configure<CompanyCodeString>(builder.Configuration.GetSection("CompanyCode"));

//bind data from appsettings.json to static class
builder.Configuration.GetSection("ConnectionStrings").Bind(QneConnectionString.Settings);
builder.Configuration.GetSection("CompanyCode").Bind(QneConnectionString._companyCodes);


builder.Services.AddServerSideBlazor()
     .AddCircuitOptions(o => o.DetailedErrors = true);

builder.Services.AddSignalR(o =>
{
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
});


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
