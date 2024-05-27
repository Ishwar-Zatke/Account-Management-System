using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using AutoMapper;
namespace Account_Management.Mappings
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<AddCustomerRequestDTO, Customer>().ReverseMap();
            CreateMap<UpdateCustomerRequestDTO, Customer>().ReverseMap();

            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<AddAccountRequestDTO, Account>().ReverseMap();
            CreateMap<UpdateAccountRequestDTO, Account>().ReverseMap();

            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<AddTransactionRequestDTO, Transaction>().ReverseMap();

            CreateMap<CustomersAccountDTO, CustomersAccount>().ReverseMap();
            CreateMap<AccountTransactionDTO, AccountTransaction>().ReverseMap();

            CreateMap<TransactionType, TransactionTypeDTO>();
            CreateMap<AddTransactionRequestDTO, Transaction>().ReverseMap();
            CreateMap<AccountType, AccountTypeDTO>();
        }
    }
}
