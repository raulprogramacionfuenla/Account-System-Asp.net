﻿using AccountSystem.Models;
using Model;
using Model.ViewModels;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{

    public class DebService : IDebService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountClientService _Account;
        public DebService(ApplicationDbContext dbContext ,
            IAccountClientService Account)
        {
            _dbContext = dbContext;
            _Account = Account;
        }

        public bool Add(Debs entity)
        {
            try
            {
                _dbContext.Debs.Add(entity);
                var account = _dbContext.Accounts
                              .Single(x => x.Id == entity.AccountId);
                account.Total += entity.Money;
                _Account.Update(account);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var model = _dbContext.Debs.Single(x => x.Id == id);
                 model.Deleted = Model.Enums.Deleted.yes;
                 model.Money = 0;
                _dbContext.Entry(model).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Debs Get(int id)
        {
            try
            {
                return _dbContext.Debs
                    .Single(x => x.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<Debs> GetAll(int page)
        {
            throw new NotImplementedException();
        }

        public DetailPageViewModel Filter(FilterDebsViewModel model, int page)
       {
            DetailPageViewModel result = new DetailPageViewModel();
            try
            {
                DateTime from = DateTime.Parse(model.From);
                DateTime to = Convert.ToDateTime(model.To);
                int pageToQuantity = 10;
                result.Account = _dbContext.Accounts
                          .Include(x => x.Client)
                          .Single(x => x.Id == model.IdAccount);
                result.Account.Debs = _dbContext.Debs
                  .Where(x => x.AccountId == model.IdAccount && (x.DateTime >= from) && (x.DateTime <= to))
                  .OrderByDescending(x => x.DateTime)
                  .Skip((page - 1) * pageToQuantity)
                  .Take(pageToQuantity).ToList();               
                result.TotalOfRegister = _dbContext.Debs.Count(x => x.AccountId == model.IdAccount && (x.DateTime >= from) && (x.DateTime <= to));
                result.ActuallyPage = page;
                result.RegisterByPage = pageToQuantity;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public bool Update(Debs entity)
        {
            try
            {
                var model = _dbContext.Debs.Single(x => x.Id == entity.Id);
                    model.Money = entity.Money;
                    model.Description = entity.Description;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public decimal SumAll(int accountId)
        {
            try
            {
                return _dbContext.Debs.Where(x => x.AccountId == accountId && x.Deleted != Model.Enums.Deleted.payment && x.Deleted != Model.Enums.Deleted.yes)
                    .Sum(x => x.Money);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
