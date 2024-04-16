# LC6464.BiliBiliWbiSign

[NuGet 包](https://www.nuget.org/packages/LC6464.BiliBiliWbiSign "NuGet.Org")
[GitHub 项目](https://github.com/lc6464/LC6464.BiliBiliWbiSign "GitHub.Com")

哔哩哔哩 WBI 签名计算器。
参考：[SocialSisterYi/bilibili-API-collect/commit/63deb66605188519d6a22b8e9bc396aca72182ad](https://github.com/SocialSisterYi/bilibili-API-collect/commit/63deb66605188519d6a22b8e9bc396aca72182ad "GitHub: SocialSisterYi/bilibili-API-collect/commit/63deb66605188519d6a22b8e9bc396aca72182ad")
使用：[lc6464/BiliBiliWbiSign](https://github.com/lc6464/BiliBiliWbiSign "GitHub: lc6464/BiliBiliWbiSign")

## 使用方法
`Example.csproj`:
``` xml
﻿<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net8.0</TargetFramework>
		<Nullable>enable</Nullable>
		<ImplicitUsings>enable</ImplicitUsings>
		<!-- 一些东西 -->
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include="LC6464.BiliBiliWbiSign" Version="1.0.0" />
		<!-- PackageReference，建议使用 Visual Studio 或 dotnet cli 等工具添加 -->
	</ItemGroup>
	<ItemGroup>
		<Using Include="LC6464.BiliBiliWbiSign" />
		<!-- 一些东西 -->
	</ItemGroup>
</Project>
```

`Program.cs`:
``` csharp
Dictionary<string, string> query = new() {
	{ "foo", "114" },
	{ "bar", "514" },
	{ "baz", "1919810" }
};

var keys = await WbiSign.GetKeysAsync();

var signedParams = await WbiSign.EncryptAsync(query, keys);

var queryString = await WbiSign.CreateQueryStringAsync(signedParams);

Console.WriteLine(queryString);
```