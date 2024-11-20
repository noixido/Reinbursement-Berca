using API.Repositories;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    
    public class AccountController : Controller
    {
        private readonly AccountRepository _repository;
        public AccountController(AccountRepository repository)
        {
            _repository = repository;
        }

        //[Authorize(Roles = "HR")]
        [HttpPost]
        public IActionResult AddAccount(AccountVM accountVM)
        {
            try
            {
                var addAccount = _repository.AddAccount(accountVM);
                if (addAccount == 0)
                {
                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Data Cannot be Registered!",
                    });
                }

                var accountData = _repository.GetLastInsertedAccount();
                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data Registered Successfully!",
                    data = (object)accountData,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }

        [Authorize(Roles = "HR")]
        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            try
            {
                var accounts = _repository.GetAllAccounts();
                if (accounts == null || !accounts.Any())
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Data Not Found",
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data Found",
                    data = (object) accounts
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }
        [Authorize(Roles = "HR, Employee, Finance")]
        [HttpGet("{email}")]
        public IActionResult GetAccountByEmail(string email)
        {
            try
            {
                var account = _repository.GetAccountByEmail(email);
                if (account == null)
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Data Not Found",
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data Found",
                    data = (object)account
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }

        [HttpGet("getAccountByEmailForUpdate/{email}")]
        public IActionResult GetAccountByEmailForUpdate(string email)
        {
            try
            {
                var account = _repository.GetAccountByEmailForUpdate(email);
                if (account == null)
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Data Not Found",
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data Found",
                    data = (object)account
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }

        [Authorize(Roles = "HR, Employee, Finance")]
        [HttpPut("{email}")]
        public IActionResult UpdateAccount(string email, AccountVM accountVM)
        {
            try
            {
                if (email != accountVM.Email)
                {
                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Email is invalid!",
                    });
                }

                var account = _repository.UpdateAccount(accountVM);
                if (account == 0)
                {
                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Data Cannot be Updated!",
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data updated successfully!",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }
        [Authorize(Roles = "HR")]
        [HttpPut("Delete/{email}")]
        public IActionResult DeleteAccount(string email)
        {
            try
            {
                var account = _repository.DeleteAccount(email);
                if (account == 0)
                {
                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Data Cannot be Deleted!",
                        data = (object)null,
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data deleted successfully!",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }
        [Authorize(Roles = "HR, Employee, Finance")]
        [HttpPut("ChangePassword/{email}")]
        public IActionResult ChangePassword(ChangePasswordVM changePasswordVM)
        {
            try
            {
                var data = _repository.ChangePassword(changePasswordVM);
                if (data == 0)
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Data Not Found",
                    });
                }
                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Data updated!",
                    data = (object)true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }
    }
}
