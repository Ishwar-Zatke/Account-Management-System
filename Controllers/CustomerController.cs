using Account_Management.Data;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Account_Management.Controllers;
using Swashbuckle.AspNetCore.Filters;
using Account_Management.Repository.SqlCode;
using Account_Management.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Account_Management.Middleware;

namespace Account_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICustomerRepository customerRepo;

        public CustomerController(ApplicationDbContext dbContext, IMapper mapper, ICustomerRepository customerRepo)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.customerRepo = customerRepo;
        }

        
        
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                                                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                
                var customers = await customerRepo.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

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

        [HttpGet("{id}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (dbContext.Customers == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested Source Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var customers = await customerRepo.GetByIdAsync(id);
                if (customers == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested id Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var customerDTO = mapper.Map<CustomerDTO>(customers);
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

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCustomer([FromBody] AddCustomerRequestDTO addCustomerRequestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    
                    return BadRequest(ModelState);
                }
                //Check for duplicate contact number
                var customercheck = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == addCustomerRequestDTO.ContactNumber);
                if (customercheck != null)
                {
                    return StatusCode(400, new
                    {
                        displayMessage = "Customer with same Contact Number already exists",
                        statusMessage = "FAIL"
                    });
                }
                var customer = mapper.Map<Customer>(addCustomerRequestDTO);
                customer = await customerRepo.CreateAsync(customer);
                var customerDTO = mapper.Map<CustomerDTO>(customer);
                //return Ok(customerDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Customer Added ",
                    statusMessage = "SUCCESS",
                    responseBody = new
                    {
                        customerDTO.Id
                    }
                }) ;
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

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequestDTO updateCustomerRequestDTO)
        //{
        //    var customer = mapper.Map<Customer>(updateCustomerRequestDTO);
        //    var existingCustomer = await dbContext.Customers.FindAsync(id);
        //    if(existingCustomer == null)
        //    {
        //        return NotFound();
        //    }
            
        //    existingCustomer.CustomerName = customer.CustomerName;
        //    existingCustomer.ContactNumber = customer.ContactNumber;
        //    existingCustomer.Address = customer.Address;
        //    var accountId = await dbContext.Accounts.FindAsync(customer.AccountId);
        //    if (accountId == null)
        //    {
        //        return BadRequest("Invalid AccountId. The specified Account does not exist.");
        //    }
        //    else
        //    {
        //        existingCustomer.AccountTypeId = customer.AccountTypeId;
        //    }
        //    var accountTypeId = await dbContext.AccountTypes.FindAsync(customer.AccountTypeId);
        //    if(accountTypeId == null)
        //    {
        //        return BadRequest("Invalid AccountTypeId. The specified AccountType does not exist.");
        //    }
        //    else
        //    {
        //        existingCustomer.AccountTypeId = customer.AccountTypeId;
        //    }
        //    await dbContext.SaveChangesAsync();
        //    var customerDTO = mapper.Map<CustomerDTO>(existingCustomer);
        //    return Ok(customerDTO);
        //}

        [HttpPatch("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateByPatch(Guid id, UpdateCustomerRequestDTO updateCustomerRequestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    
                    return BadRequest(ModelState);
                }
                var customercheck = dbContext.Customers.FirstOrDefault(x => x.Id == id);
                if (customercheck == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested customer Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var customer = mapper.Map<Customer>(updateCustomerRequestDTO);
                var existingCustomer = await customerRepo.UpdateAsync(id, customer);

                var customerDTO = mapper.Map<CustomerDTO>(existingCustomer);
                //return Ok(customerDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Cusomer Updated ",
                    statusMessage = "SUCCESS",
                    
                });

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> deleteCustomer(Guid id)
        {
            try
            {
                var customercheck = dbContext.Customers.FirstOrDefault(x => x.Id == id);
                if (customercheck == null)
                {
                    return BadRequest(new
                    {
                        displayMessage = "Requested customer Not Found",
                        statusMessage = "FAIL"
                    });
                }
                var customer = customerRepo.DeleteAsync(id);
                var customerDTO = mapper.Map<CustomerDTO>(customer);
                //return Ok(customerDTO);
                return StatusCode(201, new
                {
                    displayMessage = "Customer Deleted ",
                    statusMessage = "SUCCESS",

                });
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
        [Route("GetAllAccounts/{id}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllAccountsByCustomerId(Guid id)
        {
            try
            {
                var accounts = await customerRepo.GetAllAccountsByCustomerIdAsync(id);
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
    }
}
