using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using Account_Management.Repository.Interfaces;
using Account_Management.Repository.SqlCode;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Account_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepo;
        private readonly ApplicationDbContext dbContext;

        public AccountController(IMapper mapper, IAccountRepository accountRepo, ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            this.accountRepo = accountRepo;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllAccounts([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                                                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                if(dbContext.Accounts == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested Source Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var accounts = await accountRepo.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
                var accountDTO = mapper.Map<List<AccountDTO>>(accounts);
                return Ok(accountDTO);
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById(Guid id)
        {

            try
            {
                if (dbContext.Accounts == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested Source Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var accounts = await accountRepo.GetByIdAsync(id);
                if (accounts == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested id Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var accountDTO = mapper.Map<AccountDTO>(accounts);
                return Ok(accountDTO);
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateAccount([FromBody] AddAccountRequestDTO addAccountRequestDTO)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Checking model State for valid values ========================>");
                    return BadRequest(ModelState);
                }
                
                Random random = new Random();
                int accountNumberGenerated = random.Next(10000, 100000);
                string accountNo = accountNumberGenerated.ToString();
                //account.AccountNumber = accountNo;
                var accountcheck = await dbContext.Accounts.FirstOrDefaultAsync(x=>x.AccountNumber.ToLower() == accountNo.ToLower());
                if (accountcheck != null)
                {
                    return StatusCode(400, new
                    {
                        displayMessage = "Account already exists",
                        statusMessage = "FAIL"
                    });
                }
                var account = mapper.Map<Account>(addAccountRequestDTO);
                account.AccountNumber = accountNo;
                account = await accountRepo.CreateAsync(account);
                if (account == null)
                {
                    throw new CustomerExceptionHandler("Customer : Internal Server Error", 500);
                }
                var accountDTO = mapper.Map<AccountDTO>(account);
                //return Ok(accountDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Account Added ",
                    statusMessage = "SUCCESS",

                });
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateByPatch(Guid id, UpdateAccountRequestDTO updateAccountRequestDTO)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    
                    return BadRequest(ModelState);
                }
                var accountCheck = await dbContext.Accounts.FindAsync(id);
                if (accountCheck == null)
                {
                    return StatusCode(400, new
                    {
                        displayMessage = "Account doesn't exists",
                        statusMessage = "FAIL"
                    });
                }
                var account = mapper.Map<Account>(updateAccountRequestDTO);
                var existingAccount = await accountRepo.UpdateAsync(id, account);
                //if (existingAccount == null)
                //{
                //    return BadRequest(new
                //    {
                //        displayMessage = "Requested Account Not Found",
                //        statusMessage = "FAIL"
                //    });
                //}
                var accountDTO = mapper.Map<AccountDTO>(existingAccount);
                //return Ok(accountDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Account Updated ",
                    statusMessage = "SUCCESS",

                });
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> deleteAccount(Guid id)
        {
            
            try
            {
                var accountCheck = await dbContext.Accounts.FindAsync(id);
                if (accountCheck == null)
                {
                    return StatusCode(400, new
                    {
                        displayMessage = "Account didn't exists",
                        statusMessage = "FAIL"
                    });
                }
               
                var account = await accountRepo.DeleteAsync(id);
                //if (account == null)
                //{
                //    return BadRequest(new
                //    {
                //        displayMessage = "Requested Account Not Found",
                //        statusMessage = "FAIL"
                //    });
                //}
                var accountDTO = mapper.Map<AccountDTO>(account);
                //return Ok(accountDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Account Deleted ",
                    statusMessage = "SUCCESS",

                });
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }

        }

        [HttpGet]
        [Route("/GetAllCustomers/{id}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAllCustomerByAccountId(Guid id)
        {
            try
            {
                var customers = await accountRepo.GetAllCustomerByAccountIdAsync(id);
                var customerDTO = mapper.Map<List<CustomerDTO>>(customers);
                return Ok(customerDTO);
            }
            catch(CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpGet]
        [Route("/GetAllTransactions/{id}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAllTransactionsByAccountId(Guid id)
        {
            try
            {
                var transactions = await accountRepo.GetAllTransactionByAccountIdAsync(id);
                var transactionDTO = mapper.Map<List<TransactionDTO>>(transactions);
                return Ok(transactionDTO);
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }

                });
            }
        }
    } 
}
