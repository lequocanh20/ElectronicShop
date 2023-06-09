﻿using ElectronicShop_Model.Common;
using ElectronicShop_Model.Users;
using ElectronicShop_Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ElectronicShop_ApiIntegration.Users
{
	public class UserApiClient : IUserApiClient
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserApiClient(
			IHttpClientFactory httpClientFactory,
			IHttpContextAccessor httpContextAccessor,
			 IConfiguration configuration)
		{
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<ApiResult<string>> Authenticate(LoginRequest request)
		{
			var json = JsonConvert.SerializeObject(request);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			/* Tạo một client có base address là backend api và truyền vào hàm authenticate
             của backend api một httpcontent vừa tạo ở trên sau đó sẽ trả về response một
            */
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);

			var response = await client.PostAsync(
				string.Format(
					Constants.API_USER,
					nameof(Authenticate)),
				httpContent);

			var body = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(body);
			}

			return JsonConvert.DeserializeObject<ApiErrorResult<string>>(body);
		}

		public async Task<ApiResult<bool>> Delete(Guid id)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var response = await client.DeleteAsync(
				string.Format(
					Constants.API_USER_DELETE,
					id));

			var body = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
		}

		public async Task<ApiResult<UserViewModel>> GetById(Guid id)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var response = await client.GetAsync(
				string.Format(
					Constants.API_USER_GET_BY_ID,
					id));

			var body = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
				return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModel>>(body);

			return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModel>>(body);
		}

		public async Task<ApiResult<UserViewModel>> GetByUserName(string userName)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var response = await client.GetAsync(string.Format(
					Constants.API_USER_GET_BY_USER_NAME,
					userName));

			var body = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
				return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModel>>(body);

			return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModel>>(body);
		}

		public async Task<List<UserViewModel>> GetAll()
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var url = string.Format(
				Constants.API_USER,
				nameof(GetAll));

			var response = await client.GetAsync(url);

			var body = await response.Content.ReadAsStringAsync();

			var users = JsonConvert.DeserializeObject<List<UserViewModel>>(body);

			return users;
		}


		public async Task<ApiResult<PagedResult<UserViewModel>>> GetUsersPaging(GetUserPagingRequest request)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			//var response = await client.GetAsync($"/api/users/paging?pageIndex=" +
			//    $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");

			var url = string.Format(
				Constants.API_USER,
				nameof(GetUsersPaging));
			url = string.Concat(
				url,
				string.Join(
					"&",
					string.Concat(
						"?pageIndex=",
						request.PageIndex),
					string.Concat(
						"pageSize=",
						request.PageSize),
					string.Concat(
						"keyword=",
						request.Keyword)));
			var response = await client.GetAsync(url);

			var body = await response.Content.ReadAsStringAsync();

			var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserViewModel>>>(body);

			return users!;
		}

		public async Task<ApiResult<string>> RegisterUser(RegisterRequest registerRequest)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);

			var json = JsonConvert.SerializeObject(registerRequest);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER,
				nameof(RegisterUser));

			var response = await client.PostAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
				return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(result);

			return JsonConvert.DeserializeObject<ApiErrorResult<string>>(result);
		}

		public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
		{
			var token = _httpContextAccessor
				 .HttpContext
				 .Request
				 .Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(request);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER_ROLE_ASSIGN,
				id);


			var response = await client.PutAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
		}

		public async Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(request);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER_UPDATE,
				id);

			var response = await client.PutAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				// Deserialize thành 1 object cùng type với return type BackendApi trả về ở đây là ApiSuccessResult
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

			}
			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
		}

		public async Task<ApiResult<bool>> ChangePassword(ChangePasswordViewModel model)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER,
				nameof(ChangePassword));


			var response = await client.PostAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				// Deserialize thành 1 object cùng type với return type BackendApi trả về ở đây là ApiSuccessResult
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

			}
			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
		}

		public async Task<ApiResult<bool>> ConfirmEmail(ConfirmEmailViewModel model)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER,
				nameof(ConfirmEmail));


			var response = await client.PostAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				// Deserialize thành 1 object cùng type với return type BackendApi trả về ở đây là ApiSuccessResult
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

			}
			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
		}

		public async Task<ApiResult<string>> ForgotPassword(ForgotPasswordViewModel model)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER,
				nameof(ForgotPassword));

			var response = await client.PostAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				// Deserialize thành 1 object cùng type với return type BackendApi trả về ở đây là ApiSuccessResult
				return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(result);

			}
			return JsonConvert.DeserializeObject<ApiErrorResult<string>>(result);
		}

		public async Task<ApiResult<bool>> ResetPassword(ResetPasswordViewModel model)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER,
				nameof(ResetPassword));

			var response = await client.PostAsync(
				url,
				httpContent);

			var result = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				// Deserialize thành 1 object cùng type với return type BackendApi trả về ở đây là ApiSuccessResult
				return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

			}
			return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
		}

		public async Task<bool> DisableAccount(Guid id)
		{
			var token = _httpContextAccessor
				.HttpContext
				.Request
				.Cookies[Constants.TOKEN];

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_configuration[Constants.BASE_ADDRESS]);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.BEARER, token);

			var json = JsonConvert.SerializeObject(id);
			var httpContent = new StringContent(json, Encoding.UTF8, Constants.DEFAULT_MEDIA_TYPE);

			var url = string.Format(
				Constants.API_USER_DISABLE_ACCOUNT,
				id);

			var response = await client.PatchAsync(
				url,
				httpContent);

			if (response.IsSuccessStatusCode)
				return true;
			return false;
		}
	}
}
