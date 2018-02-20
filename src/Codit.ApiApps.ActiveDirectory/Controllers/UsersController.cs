﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Codit.ApiApps.ActiveDirectory.Contracts.v1;
using Codit.ApiApps.ActiveDirectory.Repositories;
using Codit.ApiApps.Common;
using Swashbuckle.Swagger.Annotations;

namespace Codit.ApiApps.ActiveDirectory.Controllers
{
    [RoutePrefix("api/v1")]
    public class UsersController : ApiController
    {
        private readonly UserRepository _userRepository = new UserRepository();

        /// <summary>
        ///     Gets all users in Active Directory
        /// </summary>
        /// <param name="companyName">Name of the company to filter on</param>
        [HttpGet]
        [Route("users")]
        [SwaggerOperation("Get Users")]
        [SwaggerResponse(HttpStatusCode.OK, "Returns all users", typeof(List<User>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "No users were found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "We were unable to successfully process the request")]
        public async Task<IHttpActionResult> GetUsers(string companyName = null)
        {
            List<User> users = await _userRepository.GetAll(companyName);

            return users.Any() ? (IHttpActionResult)Ok(users) : NotFound();
        }

        /// <summary>
        ///     Gets a specific user with a specific user principle name
        /// </summary>
        /// <param name="userPrincipleName">User principle name of the user to lookup</param>
        [HttpGet]
        [Route("users/{userPrincipleName}")]
        [SwaggerOperation("Get Users By User Principle Name")]
        [SwaggerResponse(HttpStatusCode.OK, "Returns found user", typeof(User))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Specified user principle name was not valid")]
        [SwaggerResponse(HttpStatusCode.NotFound, "User with specified user principle name was not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "We were unable to successfully process the request")]
        public async Task<IHttpActionResult> GetUserByUserPrincipleName(string userPrincipleName)
        {
            if (string.IsNullOrWhiteSpace(userPrincipleName))
            {
                return BadRequest("User principle name was not specified");
            }

            Maybe<User> potentialUser = await _userRepository.Get(userPrincipleName);
            return potentialUser.IsPresent ? (IHttpActionResult)Ok(potentialUser.Value) : NotFound();
        }
    }
}