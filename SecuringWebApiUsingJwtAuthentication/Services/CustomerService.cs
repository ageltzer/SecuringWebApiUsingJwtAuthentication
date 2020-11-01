﻿using SecuringWebApiUsingJwtAuthentication.Entities;
using SecuringWebApiUsingJwtAuthentication.Helpers;
using SecuringWebApiUsingJwtAuthentication.Interfaces;
using SecuringWebApiUsingJwtAuthentication.Requests;
using SecuringWebApiUsingJwtAuthentication.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringWebApiUsingJwtAuthentication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomersDbContext customersDbContext;
        public CustomerService(CustomersDbContext customersDbContext)
        {
            this.customersDbContext = customersDbContext;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var passwordHash = HashingHelper.ComputeSHA256Hash(loginRequest.Password);
            var customer = customersDbContext.Customers.SingleOrDefault(customer =>
                                                customer.Active &&
                                                customer.Username == loginRequest.Username &&
                                                customer.Password == passwordHash // Always make sure to save hashed passwords when working on real products
                                                );
            if (customer == null)
            {
                return null;
            }

            var token = await Task.Run( () => TokenHelper.GenerateToken(customer));

            return new LoginResponse { Username = customer.Username, FirstName = customer.FirstName, LastName = customer.LastName, Token = token };
        }

    }
}
