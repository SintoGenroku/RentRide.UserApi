using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentRide.AuthenticationApi.Models;
using RentRide.Common.Exceptions;
using Users.Api.Models.Requests.Users;
using Users.Api.Models.Responses.Errors;
using Users.Api.Models.Responses.Users;
using Users.Common;
using Users.Services.Abstracts;

namespace Users.Api.Controllers;

    /// <summary>
    ///     Controller for working with users
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
 public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IBus _bus;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IMapper mapper, IBus bus, ILogger<UsersController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _bus = bus;
            _logger = logger;
        }
        
        /// <summary>
        ///     Allows you get all users
        /// </summary>
        // <returns>Users collection</returns>
        [ProducesResponseType(typeof(IReadOnlyCollection<UserResponseModel>), StatusCodes.Status200OK)]
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

        /// <summary>
        ///     Allows delete current user by his ID
        /// </summary>
        /// <param name="id">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ADMIN")]
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
            
            _logger.LogInformation("Update {@user} IsActive status", user);
            var userQueue = _mapper.Map<User, UserQueue>(user);
            await _bus.Publish(userQueue);
            
            _logger.LogInformation("Publish {@userQueue} update activity status", userQueue);
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
        [HttpPatch("[action]/{id:guid}")]
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
            _logger.LogInformation("Update {@user} email/phone", user);
            return NoContent();
        }
        
        /// <summary>
        ///     Allows update current user 
        /// </summary>
        /// <param name="userId">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpPatch("[action]/{userId:guid}")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("invalid data");
            }

            if (user.IsDeleted)
            {
                return BadRequest("user is already deleted");
            }
            await _userService.DeleteUserDataAsync(user);
            _logger.LogInformation("Delete {@user} sensitive data", user);
            return NoContent();
        }
    }