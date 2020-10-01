# ASP.NET Core 3.1 と Entity Framework Core で WebAPI の作成を始める（with SQL Server）
explain.how_to_start_dotnet_api.with_ef.with_mssql
このページでは、.NET CLI を使用して .NET WebAPI プロジェクトを作成し、SQL Server に接続する方法を説明する。

https://itnext.io/asp-net-core-3-1-entity-framework-core-with-postgresql-with-code-first-approach-33b102e1734f

モデルやその他の EntityFramework の詳細について、ここでは詳しく説明しない。

このページの主な目的は、Entity Framework Core を .NET Core 3.1 で使用して SQL Server に接続する方法を理解すること。

## 事前準備
手順をはじめる前に以下のツールは用意しておく。

- VSCode
- .NET Core 3.1 SDK
- SQL Server

これらのツールの導入方法については、他のページを参照する。

Scoop を使って VSCode のポータブル版を導入する方法:  
https://github.com/fs5013-furi-sutao/explain.how_to_install_vscode_portable.with_scoop

VSCode で .NET Core の開発環境構築:  
https://github.com/fs5013-furi-sutao/explain.how_to_setup_dotnet_core_dev_env.by_vscode

VSCode と .NET Core で WebAPI プロジェクトを作成:  
https://github.com/fs5013-furi-sutao/explain.how_to_setup_webapi_project.by_vscode_and_dotnet_core

SQL Server コンテナの docker-compose 構成サンプル:  
https://github.com/fs5013-furi-sutao/mssql.server.linux

a5m2 から SQL Server (mssql) への接続設定:  
https://github.com/fs5013-furi-sutao/explain.how_to_connect.from_a5m2.to_mssql

ここからは、Windows Terminal で Git Bash を起動して作業を行う。

## ソリューションの作成
プロジェクトを格納するフォルダを作成。
```console
mkdir ./api.netcore.mssql
```

作成したフォルダの中に入る。
```console
cd ./api.netcore.mssql
```

ソリューションを作成する。
```console
dotnet new sln
```

フォルダ名と同名のslnファイルが作成される。

## WebAPI プロジェクトの作成
-n オプションで新規プロジェクトに studentapi と名付けて WebAPI を作成する。このコマンドは上記と同じディレクトリ内で行う。
```console
dotnet new webapi -n studentapi
```

## sln ファイルへのプロジェクトの追加
これでプロジェクトの作成はできたが、これだけではソリューションにこのプロジェクトが追加されない。dotnet sln コマンドで sln ファイルに作成したプロジェクトを追加する。

プロジェクトの追加:
```console
dotnet sln ./api.netcore.mssql.sln add ./studentapi/studentapi.csproj
```

VSCode でソリューションフォルダ（api.netcore.mssql）を開く。

以下のメッセージが出現するので「Yes」を押して、ワークスペースに launch.json と tasks.json を追加する。
```
'api.netcore.mssql' にはビルドやデバッグに必要なアセットがありません。追加しますか？
Required assets to build and debug are missing from 'api.netcore.mssql'. Add them?
```

プロジェクトの構成は以下のようになる。

```     
studentapi
  │  appsettings.Development.json
  │  appsettings.json
  │  Program.cs
  │  Startup.cs
  │  studentapi.csproj
  │  WeatherForecast.cs
  │  
  ├─bin
  ├─Controllers
  │      WeatherForecastController.cs
  ├─obj        
  └─Properties
          launchSettings.json
```

## 拡張機能
先に進む前に VSCode に、この後の作業で必要となる 2 つの拡張機能が入っていることを確認する。
- C# for Visual Studio Code (powered by OmniSharp)
- NuGet Package Manager

以下、主要なファイルを見ていく。

## Program.cs
Program.cs には変更は加えない。

Program クラスには main メソッドがある。ここがアプリ実行の出発点。

CreateHostBuilder（）メソッドは、汎用ホストを作成する。

Host についての詳細はドキュメントを参照する。  
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-3.1&tabs=windows#host

## Startup.cs
現時点では Startup クラスには変更を加えない。
Startup クラスではアプリケーションに必要なサービスを構成する。また、HTTPリクエストパイプラインの処理方法を定義することもできる。

Startup クラスについての詳細はドキュメントを参照する。  
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-3.1&tabs=windows#the-startup-class

## 不要なクラス
デフォルトで作成されている WeatherForecastController および  WeatherForecast クラスは不要。削除しておく。

## Models
簡単なモデルを作成する。

プロジェクトのルートディレクトリに Models フォルダを追加。Models フォルダに以下のクラスを追加する。

```csharp
using System;

namespace studentapi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

## DataContext.cs
次のステップは、DataContext クラスを作成すること。Data という名前の新しいフォルダを追加し、その中にクラスを作成する。

```csharp
using Microsoft.EntityFrameworkCore;
using studentapi.Models;

namespace studentapi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Student> Students { get; set; }
    }
}
```

しかし、新しいクラスを作成して DbContext から継承しようとすると、以下のエラーが発生する。

エラーメッセージ:
```
型または名前空間の名前 'DbContext' が見つかりませんでした
```

Entity Framework Core パッケージをアプリケーションに追加する必要がある。.NET Core 3.0 以降、Entity Framework Coreはデフォルトでプロジェクトに追加されなくなった。

詳細についてはドキュメントを参照する。  
https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.x/breaking-changes#no-longer

## EntityFrameworkCore パッケージリファレンスの追加

Ctr + Shift + P を押し、`nu` と入力して「Nuget PackageManager Add Package」を選択。

`entity` と入力して「Microsoft.EntityFrameworkCore」を選択。

現在の安定バージョン、現状では 3.1.8 を選択する。

インストールが完了すると以下のメッセージが表示されるので、「Restore（復元する）」をクリックする。
```
未解決の依存関係があります。復元コマンドを実行して続行してください。
There are unresolved dependencies. Please execute the restore command to continue.
```

すべてが正しくインストールされているかどうかを確認するには、プロジェクトファイル（studentapi.csproj）を開く。Entity Framework Core バージョン3.1.8 への PackageReference エントリが必要となる。

studentapi.csproj:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="3.1.8"/>
  </ItemGroup>
</Project>
```

Entity Framework Core のインストールを確認できたので、DataContext クラスのエラーを修正しておく。

## DB 接続文字列の設定
Appsettings.json を開き、以下の接続文字列を追加する。

以下の例では単純なパスワードを使用しているが、実際の製品コードでは、暗号化してパスワードを複雑にしておく。

本来なら、ユーザーも初期設定の sa ではなく、適切なアクセス権を持つユーザを作成すべきことに注意する。

Appsettings.json:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=192.168.99.100,1433;Initial Catalog=StudentDb;User Id=sa;Password=msSqlserver123;"
  },
  "AllowedHosts": "*"
}
```

次に、Startup クラスに EntityFrameworkCore サービスを追加構成する必要がある。

Startup.cs:（ConfigureServices メソッドを抜粋）
```csharp
using Microsoft.EntityFrameworkCore;

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
    });
}
```

SQL Server のパッケージがないので、以下のエラーが発生する。
エラーメッセージ:
```
Entity Framework Core: 
DbContextOptionsBuilder does not contain a definition for 'UseSqlServer' and 
no extension method 'UseSqlServer'
```

今回は使用する DB が SQL Server なので、Microsoft.EntityFrameworkCore.SqlServer を Nuget しておく。これで DbContextOptionsBuilder のエラーは解消される。

PostgreSQL などを使う場合なら別途、Npgsql と Npgsql.EntityFrameworkCore.PostgreSQL などのパッケージのインストールが必要となる。

## Migration の実行
ターミナルウィンドウで、プロジェクト studentapi に移動し、以下のコマンドを実行する。

```console
dotnet ef
```

が、初めて Entity Framework Core を使った場合は ef パッケージがないため、コマンドが認識されない。以下のコマンドでグローバルに ef パッケージをインストールする。

```console
dotnet tool install --global dotnet-ef
```

インストールが成功したら、再び ef コマンドを実行して、コマンドが認識されることを確認する。
実行結果:
```
                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

Entity Framework Core .NET Command-line Tools 3.1.8
```

マイグレーションを実行する。

```console
dotnet ef migrations add InitialCreate
```

今度は、dotnet は EntityFrameworkCore.Design への参照が欠落しているというエラーが出る。

Nuget で EntityFrameworkCore.Design パッケージをインストールする。

studentapi.csproj に Design パッケージが追加されていることを確認する。

studentapi.csproj:
```
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="3.1.8"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="3.1.8"/>
  </ItemGroup>
</Project>
```

Design パッケージが追加されたので再度、migration コマンドを実行する。
```console
dotnet ef migrations add InitialCreate
```

これで完了メッセージが表示され、デフォルトの3つのファイルが作成された状態で Migrations フォルダが作成される。

```
Migrations
  ├─ 20200930051259_InitialCreate.cs
  ├─ 20200930051259_InitialCreate.Designer.cs     
  └─ DataContextModelSnapshot.cs
```

データベースに Students テーブルを作成するには、以下のコマンドを実行する。

```console
dotnet ef database update
```

a5m2 でデータベースに接続し、Students テーブルが作成できていることを確認する。

テーブルにはダミーデータを挿入しておく。

|Id |Name |EMail |CreatedAt |  
|:-- |:-- |:-- |:-- |  
1 |小野田真理子 |ono.damariko@student.com |2020/10/01 10:54:54.647 |  
2 |山本一郎 |yamamo.toichiro@student.com |2020/10/01 10:55:57.514 |  


Controllers フォルダには StudentController.cs を新規作成しておく。

StudentController.cs
```csharp
using studentapi.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace studentapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private DataContext _context = null;
        public StudentController(DataContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public ActionResult GetStudents()
        {
            return Ok(this._context.Students.ToList());
        }
    }
}
```

以上で必要なセッティングおよびコーディングは終わったので、アプリケーションを実行する。

アプリケーションの実行は dotnet コマンド、もしくは VSCode でデバッグタスクを Run する。

```console
dotnet run
```

レスポンス:
```json
[
    {
        "id": 3,
        "name": "小野田真理子",
        "eMail": "ono.damariko@student.com",
        "createdAt": "2020-10-01T10:54:54.647"
    },
    {
        "id": 4,
        "name": "山本一郎",
        "eMail": "yamamo.toichiro@student.com",
        "createdAt": "2020-10-01T10:55:57.514"
    }
]
```
