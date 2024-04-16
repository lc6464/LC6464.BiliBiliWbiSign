namespace LC6464.BiliBiliWbiSign;

internal readonly struct APIRoot {
	public APIData Data { get; init; }
}

internal readonly struct APIData {
	[JsonPropertyName("wbi_img")]
	public APIDataWbiImg WbiImg { get; init; }
}

internal readonly struct APIDataWbiImg {
	[JsonPropertyName("img_url")]
	public Uri ImgUrl { get; init; }

	[JsonPropertyName("sub_url")]
	public Uri SubUrl { get; init; }
}

/// <summary>
/// 哔哩哔哩 WBI 签名所需的密钥。
/// </summary>
public readonly struct WbiKeys {
	/// <summary>
	/// img_key
	/// </summary>
	public string ImgKey { get; init; }

	/// <summary>
	/// sub_key
	/// </summary>
	public string SubKey { get; init; }
}