using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentRide.AuthenticationApi.Models;
using Users.Api.Models.Requests.Users;
using Users.Api.Models.Responses.Errors;
using Users.Api.Models.Responses.Users;
using Users.Common;
using Users.Common.Exceptions;
using Users.Services.Abstracts;
using IAuthenticationService = Users.Services.Abstracts.IAuthenticationService;

namespace Users.Api.Controllers;

/// <summary>
    ///     Controller for working with users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
 public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService; 
        private readonly IMapper _mapper;
        private readonly IBus _bus;
        //private readonly ILogger _logger;

        public UsersController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper, IBus bus/*, ILogger logger*/)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _bus = bus;
            //_logger = logger;
        }
        
        /// <summary>
        ///     Allows you get all users
        /// </summary>
        // <returns>Users collection</returns>
        [ProducesResponseType(typeof(IReadOnlyCollection<UserResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            
            var userResponseModels =
                _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserResponseModel>>(users); 
            
            return Ok(userResponseModels);
        }
        
        /// <summary>
        ///     Allows get current user with his contracts by his ID
        /// </summary>
        /// <param name="id">GUID user identifier</param>
        // <returns>User</returns>
        [ProducesResponseType(typeof(UserResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
       // [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetUserAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Such user doesn't exists");
            }
            var userResponseModel = _mapper.Map<UserResponseModel>(user);
            
            return Ok(userResponseModel);
        }
        
        /*/// <summary>
        /// Allows add new user
        /// </summary>
        /// <param name="userRequest">Personality add model</param>
        /// <returns>Created personality model</returns>
        /// <exception cref="BadRequestException">Throws if edit model is invalid</exception>
        [ProducesResponseType( typeof(UserResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType( typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateRequestModel userRequest)
        {
            if (userRequest == null)
            {
                throw new BadRequestException("Empty model");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    error => error.Key,
                    error => error.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
                throw new BadRequestException("Invalid data", errors);
            }
        
            var user = _mapper.Map<User>(userRequest);

            user = await _userService.AddUserAsync(user);

            var userResponse = _mapper.Map<UserResponseModel>(user);

        
            return CreatedAtAction(Url.RouteUrl(nameof(AddUser)), userResponse);
        }*/
        
        /// <summary>
        ///     Allows delete current user by his ID
        /// </summary>
        /// <param name="id">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> ChangeUserActivityAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Such user doesn't exists");
            }

            user.IsActive = !user.IsActive;
            await _userService.UpdateAsync(user);
            
            var userQueue = _mapper.Map<User, UserCreated>(user);
            await _bus.Publish(userQueue);
            
            //_logger.LogInformation($"Update {user.Fullname} IsActive to: {user.IsActive}");
            
            return Ok();
        }
        
        /// <summary>
        ///     Allows update current user 
        /// </summary>
        /// <param name="userEditRequestModel">special user model with updated data</param>
        /// <param name="id">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
      //  [Authorize]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> EditUserAsync(UserEditRequestModel userEditRequestModel, Guid id)
        {
            if (userEditRequestModel == null)
            {
                throw new BadRequestException("Empty model");
            }
        
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    error => error.Key,
                    error => error.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
                throw new BadRequestException("Invalid data", errors);
            }
            
            if (userEditRequestModel.Id != id)
            {
                throw new BadRequestException("Model id and request id do not match");
            }
        
            var user = await _userService.GetUserByIdAsync(userEditRequestModel.Id);

            user.MailAddress = userEditRequestModel.MailAddress;
            user.PhoneNumber = userEditRequestModel.PhoneNumber;
            
            await _userService.UpdateAsync(user);
            //_logger.LogInformation($"Update {user.Fullname} email/phone");
            return NoContent();
        }  
}