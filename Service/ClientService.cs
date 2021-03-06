﻿using AccountSystem.Models;
using Model;
using Model.ViewModels;
using Repository;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _dbContext;

        public ClientService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool Add(Client entity)
        {
            try
            {
                _dbContext.Clients.Add(entity);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool Delete(int id)
        {

            try
            {
                var model = _dbContext.Clients.Single(x => x.Id == id);
                _dbContext.Clients.Remove(model);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Client Get(int id)
        {
            var result = new Client();
            try
            {
                result = _dbContext.Clients.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Id == id);

            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public IndexPageViewModel GetAllIndex(int page)
        {
            var result = new IndexPageViewModel();
            try
            {
                int pageToQuantity = 10;
                var clients = _dbContext.Clients
                         .OrderBy(x => x.Id)
                         .Skip((page - 1) * pageToQuantity)
                         .Include(x => x.ApplicationUser)
                         .Take(pageToQuantity)
                         .ToList();
                var TotalOfRegisters = _dbContext.Clients.Count();

                result.Clients = clients;
                result.ActuallyPage = page;
                result.TotalOfRegister = TotalOfRegisters;
                result.RegisterByPage = pageToQuantity;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public bool Update(Client entity)
        {
            try
            {
                var result = _dbContext.Clients.Single(x => x.Id == entity.Id);
                result.Name = entity.Name;
                result.LastName = entity.LastName;
                result.PhoneNumber = entity.PhoneNumber;
                result.ProfileUpdated = true;
                result.Dni = entity.Dni;
                result.Address = entity.Address;
                _dbContext.Entry(result).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IndexPageViewModel Search(string parameter, int page)
        {
            var result = new IndexPageViewModel();
            try
            {

                int pageToQuantity = 10;
                var clients = _dbContext.Clients
                    .Where(x => x.Name.Contains(parameter)
                         || x.LastName.Contains(parameter)
                         || x.PhoneNumber.Contains(parameter)
                         || x.Address.Contains(parameter)
                         || x.Dni.Contains(parameter))
                         .OrderBy(x => x.Id)
                         .Skip((page - 1) * pageToQuantity)
                         .Take(pageToQuantity).Include(x => x.ApplicationUser)
                         .ToList();
                var TotalOfRegisters = _dbContext.Clients
                    .Where(x => x.Name.Contains(parameter)
                         || x.LastName.Contains(parameter) || x.PhoneNumber.Contains(parameter))
                                        .Count();

                result.Clients = clients;
                result.ActuallyPage = page;
                result.TotalOfRegister = TotalOfRegisters;
                result.RegisterByPage = pageToQuantity;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        IEnumerable<Client> IRepository<Client>.GetAll(int page)
        {
            throw new NotImplementedException();
        }

        public Client GetByIdUser(string id)
        {
            var model = new Client() ;
            try
            {
                model = _dbContext.Clients.Include(x => x.ApplicationUser).Single(x => x.ApplicationUserId == id);
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateImg(UploadImgViewModel model)
        {
            try
            {
                var client = _dbContext.Clients.FirstOrDefault(x => x.Id == model.ClientId);
                client.Avatar = model.Avatar;
                _dbContext.Entry(client).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Client> GetAll()
        {
            var model = new List<Client>(); 
            try
            {
                model = _dbContext.Clients.Include(x => x.ApplicationUser).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            return model;
        }

        public bool VerifyClientWithAccount(string idUser, int id)
        {
            try
            {
                _dbContext.Clients.Single(x => x.ApplicationUserId == idUser && x.Id == id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
