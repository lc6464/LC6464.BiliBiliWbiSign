namespace LC6464.BiliBiliWbiSign;

/// <summary>
/// Wbi 签名工具类，具体原理可参见 https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/misc/sign/wbi.md。
/// </summary>
public static class WbiSign {
	/// <summary>
	/// 获取 Wbi 签名使用的 HttpClient。
	/// </summary>
	public static HttpClient? HttpClient { get; private set; }

	/// <summary>
	/// 指示 HttpClient 是否已初始化。
	/// </summary>
	public static bool Initialized { get; private set; } = false;

	private static readonly int[] MixinKeyEncTab = [
		46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35, 27, 43, 5, 49, 33, 9, 42, 19, 29, 28, 14, 39,
		12, 38, 41, 13, 37, 48, 7, 16, 24, 55, 40, 61, 26, 17, 0, 1, 60, 51, 30, 4, 22, 25, 54, 21, 56, 59, 6, 63,
		57, 62, 11, 36, 20, 34, 44, 52
	];

	private static readonly Regex valueFilter = GeneratedRegex.ValueFilter();

	private static string GetMixinKey(string origin) => MixinKeyEncTab.Aggregate("", (s, i) => s + origin[i])[..32];

	/// <summary>
	/// 初始化 HttpClient，可以不执行，在第一次请求时会自动初始化。
	/// </summary>
	public static void Initialize(HttpClient? httpClient = null) {
		if (Initialized) {
			return;
		}

		if (httpClient is null) { // 如果未传入 HttpClient，则创建一个新的
			HttpClient = new() {
				Timeout = TimeSpan.FromSeconds(5)
			};
			HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
		} else { // 否则使用传入的 HttpClient
			HttpClient = httpClient;
		}

		Initialized = true;
	}

	/// <summary>
	/// 获取最新的 img_key 和 sub_key。
	/// </summary>
	/// <returns>获取到的 <see cref="WbiKeys"/>。</returns>
	public static async Task<WbiKeys> GetKeysAsync() {
		Initialize();

		var wbiImg = (await HttpClient!.GetFromJsonAsync<APIRoot>("https://api.bilibili.com/x/web-interface/nav").ConfigureAwait(false)).Data.WbiImg;

		// URL 中的文件名即为 key，文件实际不存在
		var imgKey = Path.GetFileNameWithoutExtension(wbiImg.ImgUrl.Segments.Last());
		var subKey = Path.GetFileNameWithoutExtension(wbiImg.SubUrl.Segments.Last());

		return new() { ImgKey = imgKey, SubKey = subKey };
	}

	/// <summary>
	/// 对参数进行 WBI 签名。
	/// </summary>
	/// <param name="parameters">参数</param>
	/// <param name="imgKey">img_key</param>
	/// <param name="subKey">sub_key</param>
	/// <returns>签名后的参数</returns>
	public static async Task<Dictionary<string, string>> EncryptAsync(Dictionary<string, string> parameters, string imgKey, string subKey) {
		var mixinKey = GetMixinKey(imgKey + subKey);
		var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

		parameters["wts"] = timestamp; // 添加 wts 字段

		parameters = parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value); // 按照 key 重排参数

		// 过滤 value 中的 "!'()*" 字符
		parameters = parameters.ToDictionary(
			p => p.Key,
			p => valueFilter.Replace(p.Value, "")
		);

		var query = await CreateQueryStringAsync(parameters); // 序列化参数

		// 计算 w_rid
		var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(query + mixinKey));

		StringBuilder stringBuilder = new();
		foreach (var b in hashBytes) {
			_ = stringBuilder.Append(b.ToString("x2"));
		}

		parameters["w_rid"] = stringBuilder.ToString();
		_ = stringBuilder.Clear();

		return parameters;
	}

	/// <summary>
	/// 对参数进行 WBI 签名。
	/// </summary>
	/// <param name="parameters">参数</param>
	/// <param name="keys">签名所需的 <see cref="WbiKeys"/></param>
	/// <returns>签名后的参数</returns>
	public static async Task<Dictionary<string, string>> EncryptAsync(Dictionary<string, string> parameters, WbiKeys keys)
		=> await EncryptAsync(parameters, keys.ImgKey, keys.SubKey);

	/// <summary>
	/// 从 Dictionary 创建查询字符串。
	/// </summary>
	/// <param name="parameters">参数字典</param>
	/// <returns>查询字符串</returns>
	public static async Task<string> CreateQueryStringAsync(Dictionary<string, string> parameters) {
		using FormUrlEncodedContent content = new(parameters);
		return await content.ReadAsStringAsync().ConfigureAwait(false);
	}
}

internal static partial class GeneratedRegex {
	[GeneratedRegex(@"[!'()*]")]
	public static partial Regex ValueFilter();
}