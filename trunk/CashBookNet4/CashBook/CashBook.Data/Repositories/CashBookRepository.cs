using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Common;

namespace CashBook.Data.Repositories
{
    public class CashBookRepository : BaseRepository, ICashBookRepository
    {
        public CashBookRepository()
        {

        }

        public UserCashBook Get(long id)
        {
            using (var context = GetContext())
            {
                var existingEntity = context.UserCashBooks.FirstOrDefault(p => p.Id == id);
                return existingEntity;
            }
        }

        public List<UserCashBook> GetAll(CashBookListType cashBookListType)
        {
            using (var context = GetContext())
            {

                var list = context.UserCashBooks.ToList();
                if (cashBookListType == CashBookListType.Lei)
                {
                    list = list.Where(p => p.IsLei).ToList();
                }
                else if (cashBookListType == CashBookListType.Other)
                {
                    list = list.Where(p => !p.IsLei).ToList();
                }
                return list;
            }
        }

        public void Create(UserCashBook item)
        {
            using (var context = GetContext())
            {
                ICompanyRepository companyRepository = new CompanyRepository();

                var existingCompany = companyRepository.GetCompany();

                item.Id = DbIdHelper.GetNextID();
                item.RegistruCasaZis = new System.Data.Objects.DataClasses.EntityCollection<DailyCashBook>();
                item.SocietateId = existingCompany.Id;
                item.InitialBalanceDate = Utils.DateTimeToDay(item.InitialBalanceDate);
                context.UserCashBooks.AddObject(item);
                Commit(context);
            }
        }

        public void Edit(long id, UserCashBook item)
        {
            using (var context = GetContext())
            {
                var existingEntity = GetCashBook(id, context);
                if (existingEntity != null)
                {
                    existingEntity.Name = item.Name;
                    existingEntity.Account = item.Account;
                    existingEntity.CashierName = item.CashierName;
                    existingEntity.CoinDecimals = item.CoinDecimals;
                    existingEntity.CoinType = item.CoinType;
                    existingEntity.InitialBalance = item.InitialBalance;
                    existingEntity.Location = item.Location;
                    existingEntity.InitialBalanceDate = Utils.DateTimeToDay(item.InitialBalanceDate);
                }
                Commit(context);
            }
        }

        public void Delete(long id)
        {
            using (var context = GetContext())
            {
                try
                {
                    //the same context needs to load this entity
                    var existingEntity = GetCashBook(id, context);
                    if (existingEntity != null)
                    {
                        foreach (var item in existingEntity.RegistruCasaZis.ToArray())
                        {
                            foreach (var it in item.RegistruCasaIntrares.ToArray())
                            {
                                context.CashBookEntries.DeleteObject(it);

                            }
                            context.DailyCashBooks.DeleteObject(item);
                        }
                        context.UserCashBooks.DeleteObject(existingEntity);
                    }
                    Commit(context);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(ex);
                    throw ex;
                }
            }
        }

        private UserCashBook GetCashBook(long id, CashBookContainer context)
        {
            return context.UserCashBooks.FirstOrDefault(p => p.Id == id);
        }


        public decimal GetInitialBalanceForDay(long selectedCashBookId, DateTime selectedDate)
        {
            using (var context = GetContext())
            {
                try
                {
                    decimal initialBalance = GetPreviousBalance(context, Utils.DateTimeToDay(selectedDate), selectedCashBookId);
                    return initialBalance;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(ex);
                    throw ex;
                }
            }
        }

        public decimal GetCurrentBalanceForDay(long selectedCashBookId, DateTime selectedDate, out DateTime? lastDateWithEntries)
        {
            using (var context = GetContext())
            {
                try
                {
                    lastDateWithEntries = null;
                    //add 1 day to the selected date to include the current day also
                    decimal initialBalance = GetPreviousBalance(context, Utils.DateTimeToDay(selectedDate).AddDays(1), selectedCashBookId);
                    var lastWithEntries=context.DailyCashBooks.OrderByDescending(p => p.Data).FirstOrDefault(p => p.RegistruCasaId == selectedCashBookId);
                    if (lastWithEntries != null)
                    {
                        lastDateWithEntries = lastWithEntries.Data;
                    }
                    return initialBalance;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(ex);
                    throw ex;
                }
            }
        }

        private decimal GetPreviousBalance(CashBookContainer context, DateTime currentDate, long selectedCashBookId)
        {
            //get all the previous registers
            var previousRegisters = context.DailyCashBooks.Where(p => p.Data < currentDate && p.RegistruCasaId == selectedCashBookId).ToList();
            decimal totalSum = 0;
            previousRegisters.ForEach(p => totalSum += p.DeltaBalance);

            //take the initial balance of the cashbook into account
            var currentCashBook = context.UserCashBooks.FirstOrDefault(p => p.Id == selectedCashBookId);
            if (currentCashBook.InitialBalanceDate <= currentDate)
            {
                totalSum += currentCashBook.InitialBalance;
            }
            return totalSum;
        }
    }
}
