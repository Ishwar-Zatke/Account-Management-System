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

namespace Account_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepo;
        private readonly ApplicationDbContext dbContext;

        public TransactionController(IMapper mapper, ITransactionRepository transactionRepo, ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            this.transactionRepo = transactionRepo;
            this.dbContext = dbContext;
        }
        //GetAllTransaction
        //GetAllTransactionByID
        //CreateTransaction
        //GetAllTransactionByAccountID
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                if (dbContext.Transactions == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested Source Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var transaction = await transactionRepo.GetAllAsync();
                var transactionDTO = mapper.Map<List<TransactionDTO>>(transaction);
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

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            try
            {
                var transactionCheck = await dbContext.Transactions.FindAsync(id);
                if (transactionCheck == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested id Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var transaction = await transactionRepo.GetById(id);
                var transactionDTO = mapper.Map<TransactionDTO>(transaction);
                return Ok(transactionDTO);
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
        [Route("Account/{AccountId}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAllTransactionByAccountId(Guid AccountId)
        {
            try
            {
                var AccountIdCheck = await dbContext.Transactions.FirstOrDefaultAsync(x=>x.FromAccountId == AccountId);
                if(AccountIdCheck == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested id Not Found for Any Transaction",
                        statusMessage = "FAIL"
                    });
                }
                var transaction = await transactionRepo.GetAllByAccountIdAsync(AccountId);
                var transactionDTO = mapper.Map<List<TransactionDTO>>(transaction);
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

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateTransaction([FromBody] AddTransactionRequestDTO addTransactionRequestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Checking model State for valid values");
                    return BadRequest(ModelState);
                }
                var transaction = mapper.Map<Transaction>(addTransactionRequestDTO);
                transaction = await transactionRepo.CreateAsync(transaction);
                var transactionDTO = mapper.Map<TransactionDTO>(transaction);
                //return Ok(transactionDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Tansaction Successfull ",
                    statusMessage = "SUCCESS",

                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }
    }
}
