using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITC_Matrix.Models
{
    public class ViewModel
    {
        public IEnumerable<Account> AccountsModel { get; set; }
        public IEnumerable<Client> ClientsModel { get; set; }
        public IEnumerable<TrnDep> TrnDepModel { get; set; }
        public IEnumerable<PayMethod> PayMethodModel { get; set; }
        public IEnumerable<AccountCode> AccountCodes { get; set; }
        public IEnumerable<Profile> profiles { get; set; }


        public  static IEnumerable<AccountCode> accountsCodeList() {
            New_ITC_MultiplanEntities db = new New_ITC_MultiplanEntities();
            return db.AccountCodes.ToList();
        } 
    }
}