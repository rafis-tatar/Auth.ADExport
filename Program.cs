﻿using System;
using System.Collections;
using System.Linq;
using Novell.Directory.Ldap;

namespace Auth.ADExport
{
    public class Program
    {
        static void Main(string[] args)
        { 
            var users = ADUserService.GetAllUsers("tatneft.ru", "hasanovaz@hq.tatneft.ru", "4rfv$RFV");
            var databaseService = new DatabaseService();
            databaseService.Export(users);
            //var str = string.Join("; \n\n", users.Select(u => $"AccountFullName:{u.AccountFullName}, AccountName:{u.AccountName}, UserName:{u.UserName}, Title:{u.Title}, Company:{u.Company}, Path:{u.Path}, UserDept:{u.UserDept}"));
        }
    }
}
