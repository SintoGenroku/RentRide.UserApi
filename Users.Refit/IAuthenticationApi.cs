using Refit;
using RentRide.AuthenticationApi.Models.Requests;
using RentRide.AuthenticationApi.Models.Responses;


namespace Users.Refit;

public interface IAuthenticationApi
{
    [Post("/authentication-api/token")]
    Task<GetTokenResponseModel>GetToken([Body(BodySerializationMethod.UrlEncoded)]GetTokenRequestModel request);
}